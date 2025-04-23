using Microsoft.EntityFrameworkCore;
using MusicApp.Database;

namespace AspNetCore.Identity.Extensions;

public static class MigrationExtensions // Кастомное расширение для применения миграций
{
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();

            using MusicAppDbContext dbContext = scope.ServiceProvider.GetRequiredService<MusicAppDbContext>();

            dbContext.Database.Migrate();
        }
 }

