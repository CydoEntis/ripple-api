global using FastEndpoints;
global using FluentValidation;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ripple.Data;
using Ripple.Entities;

var builder = WebApplication.CreateBuilder();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddScoped<Ripple.Features.User.Register.RegisterData>();

builder.Services
    .AddAuthenticationJwtBearer(s =>
        s.SigningKey =
            "thS2WreJFQ2XKL17MuDy2fEFb7rfMERHSUpzlrmGOqITJfHwQR3uyK552PqO4Bhlq04lQl0ppx8SHtMCk3TIy9o4qSistlxG1wVOoVGBmmxKpRs1PPx8vtR7dkCGaMjrYmtBdPMijHEGbszOFDMkCVsRsF6FUjH3AT5K9xk8WH5baxkFPzYeT6WcvPKAbwbRqq4KfCb0QjRzqEutLu4hAI5zcp2pmFikZh84elH7iEOEgW8HIww3RmKkm5MYlX8h") // Test Secret
    .AddAuthorization()
    .AddFastEndpoints()
    .SwaggerDocument();

var app = builder.Build();
app.UseFastEndpoints()
    .UseSwaggerGen();
app.Run();