using Identity.Api.Data;
using Identity.Api.Data.Entities;
using Identity.Api.Data.Repositories;
using Identity.Api.DomainComponents;
using Identity.Api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseWindowsService();
builder.Services.AddDbContext<IdentityDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("BLTSMFT")));

builder.Services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();
builder.Services.AddScoped<IIdentityRepository, IdentityRepository>();
builder.Services.AddScoped<IIdentityDomainComponent, IdentityDomainComponent>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(o => o.AddPolicy("ui", p => p.WithOrigins(builder.Configuration["Frontend:Origin"] ?? "http://localhost:4200")
.AllowAnyHeader()
.AllowAnyMethod()
.AllowCredentials()));

var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing"));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = !string.IsNullOrWhiteSpace(builder.Configuration["Jwt:Issuer"]),
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = !string.IsNullOrWhiteSpace(builder.Configuration["Jwt:Audience"]),
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    });

builder.Services.AddAuthorization();
var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("ui");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
