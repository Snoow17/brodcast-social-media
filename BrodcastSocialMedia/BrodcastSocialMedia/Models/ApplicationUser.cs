using Microsoft.AspNetCore.Identity;

namespace BrodcastSocialMedia.Models
{
    public class ApplicationUser : IdentityUser 
    {
        public string? Name { get; set; }
        public ICollection<Broadcast> Broadcasts { get; set; }
    }
}
