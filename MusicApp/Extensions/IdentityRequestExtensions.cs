using Microsoft.AspNetCore.Identity;
using MusicApp.Models;
using MusicApp.Database;
using Microsoft.EntityFrameworkCore;

namespace MusicApp.Extensions
{
    public static class IdentityRequestExtensions
    {
        public static IEndpointRouteBuilder MapCustomIdentityApi(this IEndpointRouteBuilder endpoints)
        {
            // Эндпоинт для получения всех данных текущего пользователя
            endpoints.MapGet("/users/me", async (
                UserManager<AppUser> userManager,
                HttpContext httpContext) =>
            {
                // Получаем текущего пользователя из контекста
                var user = await userManager.GetUserAsync(httpContext.User);
                if (user == null)
                {
                    return Results.NotFound("User not found");
                }

                // Возвращаем все данные пользователя
                return Results.Ok(new
                {
                    user.Id,
                    user.UserName,
                    user.NormalizedUserName,
                    user.Email,
                    user.NormalizedEmail,
                    user.EmailConfirmed,
                    user.PasswordHash,
                    user.SecurityStamp,
                    user.ConcurrencyStamp,
                    user.PhoneNumber,
                    user.PhoneNumberConfirmed,
                    user.TwoFactorEnabled,
                    user.LockoutEnd,
                    user.LockoutEnabled,
                    user.AccessFailedCount,
                    user.Nickname // Кастомное поле
                });
            })
            .RequireAuthorization();

            // Эндпоинт для обновления данных пользователя (без Email и UserName)
            endpoints.MapPut("/users/me", async (
                UserManager<AppUser> userManager,
                MusicAppDbContext dbContext,
                HttpContext httpContext,
                UpdateUserRequest request) =>
            {
                // Получаем текущего пользователя из контекста
                var user = await userManager.GetUserAsync(httpContext.User);
                if (user == null)
                {
                    return Results.NotFound("User not found");
                }

                // Проверка на уникальность Nickname
                if (!string.IsNullOrEmpty(request.Nickname))
                {
                    var existingUserByNickname = await dbContext.Users
                        .FirstOrDefaultAsync(u => u.Nickname == request.Nickname);
                    if (existingUserByNickname != null && existingUserByNickname.Id != user.Id)
                    {
                        return Results.BadRequest("Nickname is already taken by another user.");
                    }
                    user.Nickname = request.Nickname;
                }

                // Проверка на уникальность PhoneNumber
                if (!string.IsNullOrEmpty(request.PhoneNumber))
                {
                    var existingUserByPhone = await userManager.Users
                        .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);
                    if (existingUserByPhone != null && existingUserByPhone.Id != user.Id)
                    {
                        return Results.BadRequest("Phone number is already taken by another user.");
                    }
                    user.PhoneNumber = request.PhoneNumber;
                }

                // Обрабатываем пароль, если он передан
                if (!string.IsNullOrEmpty(request.Password))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await userManager.ResetPasswordAsync(user, token, request.Password);
                    if (!result.Succeeded)
                    {
                        return Results.ValidationProblem(result.Errors.ToDictionary(
                            e => e.Code,
                            e => new[] { e.Description }));
                    }
                }

                // Обновляем пользователя в базе данных
                var updateResult = await userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    return Results.ValidationProblem(updateResult.Errors.ToDictionary(
                        e => e.Code,
                        e => new[] { e.Description }));
                }

                return Results.Ok();
            })
            .RequireAuthorization();

            return endpoints;
        }
    }

    // Модель для запроса обновления данных пользователя (без Email и UserName)
    public class UpdateUserRequest
    {
        public string? Password { get; set; }
        public string? Nickname { get; set; }
        public string? PhoneNumber { get; set; }
    }
}

