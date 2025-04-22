using Microsoft.AspNetCore.Identity;

namespace MusicApp.Models
{
    public class AppUser : IdentityUser
    {
        public required string Nickname { get; set; }
    }
}
