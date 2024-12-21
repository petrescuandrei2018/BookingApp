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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("AppSettings:JwtOptions"));
builder.Services.AddIdentity<UserRegistered, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();

builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IHotelRepository,HotelRepository>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IRezervareServiciuActualizare, RezervareServiciuActualizare>();
builder.Services.AddHostedService<RezervareServiciuActualizare>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


/*< Project Sdk = "Microsoft.NET.Sdk.Web" >

  < PropertyGroup >
    < TargetFramework > net8.0 </ TargetFramework >
    < Nullable > enable </ Nullable >
    < ImplicitUsings > enable </ ImplicitUsings >
  </ PropertyGroup >

  < ItemGroup >
    < PackageReference Include = "AutoMapper" Version = "13.0.1" />
    < PackageReference Include = "Microsoft.EntityFrameworkCore" Version = "8.0.8" />
    < PackageReference Include = "Microsoft.EntityFrameworkCore.SqlServer" Version = "8.0.8" />
    < PackageReference Include = "Microsoft.EntityFrameworkCore.Tools" Version = "8.0.8" >
      < PrivateAssets > all </ PrivateAssets >
      < IncludeAssets > runtime; build; native; contentfiles; analyzers; buildtransitive </ IncludeAssets >
    </ PackageReference >
    < PackageReference Include = "Swashbuckle.AspNetCore" Version = "6.4.0" />
  </ ItemGroup >

  < ItemGroup >
    < Folder Include = "Migrations\" />
  </ ItemGroup >

</ Project >*/