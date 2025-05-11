using Microsoft.AspNetCore.Identity;

namespace BrodcastSocialMedia.Models
{
    public class ApplicationUser : IdentityUser 
    {
        public string? Name { get; set; }
        public string? ProfileImageUrl { get; set; }

        public ICollection<Broadcast> Broadcasts { get; set; }

        public ICollection<ApplicationUser> ListeningTo { get; set; } = new List<ApplicationUser>();
    }
}
