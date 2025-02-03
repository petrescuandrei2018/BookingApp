using AutoMapper;
using BookingApp.Data;
using BookingApp.Mapping;
using BookingApp.Models;
using BookingApp.Repository;
using BookingApp.Repository.Abstractions;
using BookingApp.Services;
using BookingApp.Services.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Stripe;
using System.Globalization;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.FileProviders;
using BookingApp.CanaleComunicare;

var builder = WebApplication.CreateBuilder(args);

// ✅ Debugging Environment
Console.WriteLine($"Mediul curent: {builder.Environment.EnvironmentName}");

// ✅ Validare StripeSettings
var stripeSection = builder.Configuration.GetSection("Stripe");
if (string.IsNullOrEmpty(stripeSection["SecretKey"]) || string.IsNullOrEmpty(stripeSection["PublishableKey"]))
{
    throw new InvalidOperationException("Configurarea Stripe nu este completă. Verifică SecretKey și PublishableKey în appsettings.json.");
}
StripeConfiguration.ApiKey = stripeSection["SecretKey"];

// ✅ Configurare logare
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Trace);

// ✅ Configurare cultură default
var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// ✅ Înregistrăm configurațiile JWT
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

// ✅ Configurăm baza de date (SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.LogTo(Console.WriteLine, LogLevel.Information);
});

// ✅ Înregistrăm AutoMapper
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


// ✅ Configurăm Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// ✅ Înregistrăm serviciile principale
builder.Services.AddScoped<IServiciuCacheRedis, ServiciuCacheRedis>();
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IRezervareServiciuActualizare, RezervareServiciuActualizare>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<IStatisticiService, StatisticiService>(); // 🔹 Adăugat aici
builder.Services.AddSignalR();


// ✅ Servicii pentru căutarea coordonatelor online
builder.Services.AddHttpClient<IServiciuCoordonateOnline, ServiciuCoordonateOnline>(client =>
{
    var config = builder.Configuration;
    var apiKey = config["OpenWeather:ApiKey"];

    if (string.IsNullOrEmpty(apiKey))
    {
        throw new InvalidOperationException("API Key pentru OpenWeather lipsește din configurație.");
    }

    client.DefaultRequestHeaders.Add("User-Agent", "BookingApp");
});
builder.Services.AddScoped<IServiciuCoordonateOnline, ServiciuCoordonateOnline>();
builder.Services.AddScoped<IServiciuCoordonate, ServiciuCoordonate>();
builder.Services.AddScoped<IRecenzieService, RecenzieService>();
builder.Services.AddScoped<IRecenzieRepository, RecenzieRepository>();


// ✅ Servicii pentru hartă și filtrare hoteluri
builder.Services.AddScoped<IServiciuFiltrareHoteluri, ServiciuFiltrareHoteluri>();
builder.Services.AddScoped<IServiciuGenerareHtml, ServiciuGenerareHtml>();
builder.Services.AddScoped<IServiciuSalvareHarta, ServiciuSalvareHarta>();
builder.Services.AddScoped<IServiciuHarta, ServiciuHarta>();

// ✅ Servicii pentru plăți și Stripe
builder.Services.AddScoped<IServiciuStripe, ServiciuStripe>();
builder.Services.AddScoped<IServiciuPlata, ServiciuPlata>();
builder.Services.AddHostedService<RezervareServiciuActualizare>();

// ✅ Servicii pentru e-mail
builder.Services.AddScoped<ServiciuEmailMock>();
builder.Services.AddScoped<ServiciuEmailSmtp>();
builder.Services.AddScoped<IServiciuEmail>(furnizorServicii =>
{
    var configuratie = furnizorServicii.GetRequiredService<IConfiguration>();
    var folosesteMock = configuratie.GetValue<bool>("FolosesteEmailMock");

    return folosesteMock ? furnizorServicii.GetRequiredService<ServiciuEmailMock>()
                         : furnizorServicii.GetRequiredService<ServiciuEmailSmtp>();
});
builder.Services.AddScoped<FabricaServiciuEmail>();

// ✅ Configurare autentificare și JWT
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
if (string.IsNullOrEmpty(jwtSettings?.Key))
{
    throw new InvalidOperationException("Cheia JWT lipsește din configurație.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience
    };
});

builder.Services.AddAuthorization();

// ✅ Configurăm serializarea JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// ✅ Configurăm Stripe
builder.Services.AddSingleton<IStripeClient>(new StripeClient(StripeConfiguration.ApiKey));

// ✅ Configurăm Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BookingApp API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Introduceți 'Bearer' urmat de token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

app.MapHub<NotificariAdministrator>("/notificari-admin");


Console.WriteLine("Consola afiseaza");

// ✅ Activăm redirecționarea la HTTPS
app.UseHttpsRedirection();

// ✅ Middleware pentru logare a cererilor HTTP
app.Use(async (context, next) =>
{
    Console.WriteLine($"[Middleware] Cerere primită: {context.Request.Method} {context.Request.Path}");
    Console.WriteLine($"[Middleware] QueryString: {context.Request.QueryString}");
    await next();
});

app.UseRouting();

// ✅ Adăugăm fișierele statice pentru fișierele HTML generate
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.GetTempPath()),
    RequestPath = "/Harta"
});

// ✅ Configurăm autentificare și autorizare
app.UseAuthentication();
app.UseAuthorization();

// ✅ Configurăm Swagger în modul Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookingApp API V1");
        c.RoutePrefix = "swagger";
    });
}

// ✅ Mapăm rutele pentru controlere
app.MapControllers();
app.Run();
