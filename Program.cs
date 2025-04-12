global using FastEndpoints;
global using FluentValidation;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ripple.Data;
using Ripple.Entities;
using Ripple.Features.User.RefreshToken;

var builder = WebApplication.CreateBuilder();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<RefreshTokenService<TokenRequest, TokenResponse>, TokenService>();



var secretKey = builder.Configuration["JwtSettings:SecretKey"];

builder.Services
    .AddAuthenticationJwtBearer(s =>
        s.SigningKey =
            builder.Configuration["JwtSettings:SecretKey"])
    .AddAuthorization()
    .AddFastEndpoints()
    .SwaggerDocument();

var app = builder.Build();
app.UseFastEndpoints()
    .UseSwaggerGen();
app.Run();