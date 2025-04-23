using AspNetCore.Identity.Extensions; // Кастомный неймспейс для приминения миграций
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicApp.Database;
using MusicApp.Extensions;
using MusicApp.Models;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    // Указываем схему аутентификации по умолчанию
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
})
    .AddCookie(IdentityConstants.ApplicationScheme)
    .AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddIdentityCore<AppUser>()
    .AddEntityFrameworkStores<MusicAppDbContext>()
    .AddApiEndpoints();

builder.Services.AddDbContext<MusicAppDbContext>(options => options
    .UseNpgsql(builder.Configuration.GetConnectionString("Database"))); // Подключаем к БД

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MusicApp API V1");
    });

    app.ApplyMigrations(); // Свое свойство для применения миграций
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapIdentityApi<AppUser>(); // Стандартные эндпоинты
app.MapCustomIdentityApi(); // Кастомные эндпоинты

app.Run();
