using Microsoft.AspNetCore.Identity;

namespace Inventoria.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? PreferredTheme { get; set; }  // "light" | "dark"
        public string? PreferredCulture { get; set; } // "en" | "bn"
    }
}
