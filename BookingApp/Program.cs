using AutoMapper;
using BookingApp.Data;
using BookingApp.Mapping;
using BookingApp.Models;
using BookingApp.Repository;
using BookingApp.Repository.Abstractions;
using BookingApp.Helpers;
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

// Configurează furnizorii de logare
builder.Logging.ClearProviders(); // Curăță furnizorii existenți
builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Trace; // Setăm nivelul minim la Information
});
builder.Logging.SetMinimumLevel(LogLevel.Trace); // Setăm nivelul minim global la Information

// Test de logare
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

// Adăugăm serviciile proiectului în containerul de servicii
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IRezervareServiciuActualizare, RezervareServiciuActualizare>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHostedService<RezervareServiciuActualizare>();
builder.Services.AddScoped<IUserDropdownService, UserDropdownService>();
builder.Services.AddTransient<SetAdminDropdownFilter>();


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
    var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Activăm validarea pentru issuer
        ValidateAudience = true, // Activăm validarea pentru audience
        ValidateLifetime = true, // Validăm timpul de expirare
        ClockSkew = TimeSpan.Zero, // Eliminăm diferențele implicite de timp
        ValidateIssuerSigningKey = true, // Validăm cheia de semnare
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)), // Folosim cheia secretă din setări
        ValidIssuer = jwtSettings.Issuer, // Folosim Issuer din setări
        ValidAudience = jwtSettings.Audience // Folosim Audience din setări
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            // Gestionăm erorile de autentificare
            return Task.CompletedTask;
        }
    };
});


// Adăugăm suport pentru autorizare
builder.Services.AddAuthorization();

// Configurăm suportul pentru serializarea JSON, inclusiv pentru enum-uri
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Configurăm Swagger pentru documentarea API-ului
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations(); // Activează suportul pentru adnotări

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BookingApp API", Version = "v1" });
    c.EnableAnnotations();

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

    // Include XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Adaugă operation filter-ul
    c.OperationFilter<SetAdminOperationFilter>();
});





var app = builder.Build();

Console.WriteLine("Consola afiseaza"); // Adaugă acest mesaj pentru a testa consola


using (var scope = app.Services.CreateScope())
{
    var userDropdownService = scope.ServiceProvider.GetRequiredService<IUserDropdownService>();
    var usersForDropdown = await userDropdownService.GetUsersForDropdownAsync();
    UserDropdownCache.PopulateUsers(usersForDropdown);
}

// Activăm redirecționarea la HTTPS
app.UseHttpsRedirection();

// Middleware pentru logare a cererilor HTTP
app.Use(async (context, next) =>
{
    Console.WriteLine($"[Middleware] Cerere primită: {context.Request.Method} {context.Request.Path}");
    Console.WriteLine($"[Middleware] QueryString: {context.Request.QueryString}");
    await next();
});

// **Adăugăm suport pentru servirea fișierelor statice din wwwroot**
app.UseStaticFiles(); // Permite servirea fișierelor statice, inclusiv custom-swagger.js

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
        c.InjectJavascript("/swagger/custom-swagger.js");
        c.RoutePrefix = "swagger";
        c.EnableDeepLinking();
        c.DisplayRequestDuration();

        // Adaugă cod inline pentru testare
        //c.HeadContent += "<script>console.log('Script inline funcționează!'); alert('Script inline încărcat!');</script>";

    });
}

// Adăugăm middleware-ul pentru rutare explicit, deși nu e necesar dacă folosești app.MapControllers()
app.UseRouting();

// Mapăm rutele pentru controlere
app.MapControllers();



// Pornim aplicația
app.Run();
