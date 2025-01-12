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
using BCrypt.Net;
using System.Linq;
using Microsoft.OpenApi.Models;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Configurează furnizorii de logare
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Trace;
});
builder.Logging.SetMinimumLevel(LogLevel.Trace);

var logger = builder.Logging.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
logger.LogInformation("Test de logare din Program.cs");

// Înregistrăm configurațiile JWT
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

// Configurăm serviciul pentru baza de date folosind SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Configurăm AutoMapper pentru maparea obiectelor DTO și entităților
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var cachePlati = new Dictionary<int, string>();
builder.Services.AddSingleton(cachePlati);


// Adăugăm serviciile proiectului în containerul de servicii
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IRezervareServiciuActualizare, RezervareServiciuActualizare>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IServiciuPlata, ServiciuPlata>();
builder.Services.AddScoped<IServiciuEmail, ServiciuEmail>();
builder.Services.AddHostedService<RezervareServiciuActualizare>();

// Adăugarea configurației Stripe
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe")); // StripeSettings trebuie să fie definit în proiect


// Configurare pentru autentificare și generarea token-urilor JWT
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

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

// Adăugăm suport pentru autorizare
builder.Services.AddAuthorization();

// Configurăm suportul pentru serializarea JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Configurăm Stripe
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
builder.Services.AddSingleton<IStripeClient>(new StripeClient(StripeConfiguration.ApiKey));

// Configurăm Swagger pentru documentarea API-ului
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

Console.WriteLine("Consola afiseaza");

// Activăm redirecționarea la HTTPS
app.UseHttpsRedirection();

// Middleware pentru logare a cererilor HTTP
app.Use(async (context, next) =>
{
    Console.WriteLine($"[Middleware] Cerere primită: {context.Request.Method} {context.Request.Path}");
    Console.WriteLine($"[Middleware] QueryString: {context.Request.QueryString}");
    await next();
});

// Configurăm pipeline-ul pentru autentificare și autorizare
app.UseAuthentication();
app.UseAuthorization();

// Configurăm pipeline-ul pentru mediul de dezvoltare, incluzând Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookingApp API V1");
        c.RoutePrefix = "swagger";
        c.EnableDeepLinking();
        c.DisplayRequestDuration();
    });
}

// Mapăm rutele pentru controlere
app.UseRouting();
app.MapControllers();
app.Run();
