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
using Microsoft.OpenApi.Any;

var builder = WebApplication.CreateBuilder(args);

// Adăugăm serviciile necesare aplicației
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

// Configurarea autentificării JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false, // Dezactivăm temporar validarea pentru debugging
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero, // Eliminăm diferențele implicite
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("aVerySecureAndLongEnoughKey1234567890!"))
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
// Configurare Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BookingApp API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Introduceti 'Bearer' urmat de token"
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
    // Mapare pentru enum
    c.MapType<RolUtilizator>(() => new OpenApiSchema
    {
        Type = "string",
        Enum = Enum.GetNames(typeof(RolUtilizator)).Select(name => new OpenApiString(name)).ToArray()
    });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

// Middleware-uri pentru autentificare și autorizare
app.UseAuthentication();
app.UseAuthorization();

// Configurare pipeline pentru mediul de dezvoltare
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookingApp API V1");
        c.RoutePrefix = "swagger";
    });
}

app.MapControllers();

app.Run();
