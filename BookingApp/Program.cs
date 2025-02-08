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
using System.Net.WebSockets;

var builder = WebApplication.CreateBuilder(args);

// ✅ Serviciu WebSocket
builder.Services.AddSingleton<IServiciuWebSocket, ServiciuWebSocket>();

// ✅ Adăugăm SignalR
builder.Services.AddSignalR();

// ✅ Configurare Identity
builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
    options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

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

// ✅ Configurare baza de date SQL Server
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
builder.Services.AddScoped<IStatisticiService, StatisticiService>();
builder.Services.AddScoped<IServiciuUtilizator, ServiciuUtilizator>();

// ✅ Servicii pentru coordonate și recenzii
builder.Services.AddHttpClient<IServiciuCoordonateOnline, ServiciuCoordonateOnline>();
builder.Services.AddScoped<IServiciuCoordonate, ServiciuCoordonate>();
builder.Services.AddScoped<IRecenzieService, RecenzieService>();
builder.Services.AddScoped<IRecenzieRepository, RecenzieRepository>();

// ✅ Servicii pentru hartă și filtrare hoteluri
builder.Services.AddScoped<IServiciuFiltrareHoteluri, ServiciuFiltrareHoteluri>();
builder.Services.AddScoped<IServiciuGenerareHtml, ServiciuGenerareHtml>();
builder.Services.AddScoped<IServiciuSalvareHarta, ServiciuSalvareHarta>();
builder.Services.AddScoped<IServiciuHarta, ServiciuHarta>();

// ✅ Servicii pentru plăți și Stripe
builder.Services.AddSingleton<IStripeClient>(new StripeClient(stripeSection["SecretKey"]!));
builder.Services.AddScoped<IServiciuStripe, ServiciuStripe>();
builder.Services.AddScoped<IServiciuPlata, ServiciuPlata>();

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

// ✅ Configurăm Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "BookingApp API", Version = "v1" });

    // Adăugăm suport pentru autentificare cu JWT în Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Introduceți token-ul JWT în formatul: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ✅ Middleware-uri standard
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
// ✅ Activăm WebSockets
app.UseWebSockets();

// ✅ Middleware pentru WebSocket
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var webSocketManager = context.RequestServices.GetRequiredService<IServiciuWebSocket>();
            await webSocketManager.HandleWebSocketConnection(webSocket);
        }
        else
        {
            context.Response.StatusCode = 400;
        }
    }
    else
    {
        await next();
    }
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookingApp API v1");
});




// ✅ Mapăm WebSocket și SignalR
app.MapHub<NotificariAdministrator>("/notificari-admin");
app.MapControllers();

// ✅ Pornim aplicația
app.Run();
