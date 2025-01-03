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
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BCrypt.Net;
using System.Linq;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();

builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IRezervareServiciuActualizare, RezervareServiciuActualizare>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHostedService<RezervareServiciuActualizare>();

// Configurarea autentificarii JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");

// Log the JWT Key for debugging purposes
Console.WriteLine($"Program Key: {jwtSettings["Key"]}");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false, // Dezactivăm temporar validarea
        ValidateAudience = false, // Dezactivăm temporar validarea

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero, // Elimină diferențele acceptate implicit

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("aVerySecureAndLongEnoughKey1234567890!"))
    };

    // Log pentru verificarea cheii
    Console.WriteLine($"Cheia de semnare din TokenValidationParameters: {Encoding.UTF8.GetString(((SymmetricSecurityKey)options.TokenValidationParameters.IssuerSigningKey).Key)}");

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception}");
            Console.WriteLine($"Token: {context.Request.Headers["Authorization"]}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Definim documentatia Swagger pentru API
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BookingApp API", Version = "v1" });

    // Adaugam definitia de securitate pentru JWT Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,  // Definim locatia unde va fi trimis token-ul (in header)
        Name = "Authorization",  // Numele header-ului pentru token
        Type = SecuritySchemeType.ApiKey,  // Tipul securitatii
        Scheme = "bearer",  // Schema de autentificare
        BearerFormat = "JWT",  // Formatul token-ului
        Description = "Introduceti 'Bearer' urmat de  si apoi token-ul"  // Descrierea pentru Swagger UI
    });

    // Adaugam cerintele de securitate pentru utilizarea token-ului JWT
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
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

app.Use(async (context, next) => // pentru debug
{
    if (context.Request.Headers.ContainsKey("Authorization"))
    {
        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        Console.WriteLine($"Token JWT primit: {token}");

        try
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            Console.WriteLine($"Issuer: {jwtToken.Issuer}");
            Console.WriteLine($"Audience: {jwtToken.Audiences.FirstOrDefault()}");
            Console.WriteLine($"Claims:");
            foreach (var claim in jwtToken.Claims)
            {
                Console.WriteLine($"{claim.Type}: {claim.Value}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare la decodarea token-ului: {ex.Message}");
        }
    }
    else
    {
        Console.WriteLine("Lipsește header-ul Authorization.");
    }
    await next();
});

// Middleware pentru logarea header-ului Authorization
app.Use(async (context, next) =>
{
    // Verificăm dacă header-ul Authorization există
    if (context.Request.Headers.ContainsKey("Authorization"))
    {
        var token = context.Request.Headers["Authorization"];
        Console.WriteLine($"Header Authorization: {token}");
    }
    else
    {
        Console.WriteLine("Lipsește header-ul Authorization.");
    }
    await next(); // Continuăm procesarea pipeline-ului
});

Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true; // pentru debug

// Add Authentication and Authorization Middleware
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookingApp API V1");
        c.RoutePrefix = "swagger";  // Set Swagger UI route explicitly
    });
}

app.MapControllers();

app.Run();