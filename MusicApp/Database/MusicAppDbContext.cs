using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicApp.Models;

namespace MusicApp.Database
{
    public class MusicAppDbContext : IdentityDbContext<AppUser>
    {
        public MusicAppDbContext(DbContextOptions<MusicAppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<AppUser>().Property(u => u.Nickname).HasMaxLength(30); // Ограничиваем длину ника до 30 символов

            builder.HasDefaultSchema("identity"); // Указываем схему по умолчанию
        }
    }
}
