using Microsoft.AspNetCore.Identity;

namespace MusicApp.Models
{
    public class AppUser : IdentityUser
    {
        public string? Nickname { get; set; } // Кастомное поле для юзера, должно быть nullable
    }
}
