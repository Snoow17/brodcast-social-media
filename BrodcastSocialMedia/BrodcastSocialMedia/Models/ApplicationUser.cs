using BrodcastSocialMedia.Models;
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string? Name { get; set; }
    public string? ProfileImageUrl { get; set; }

    public int BroadcastCount { get; set; }

    public ICollection<Broadcast> Broadcasts { get; set; } = new List<Broadcast>();

    public ICollection<UserListening> ListeningTo { get; set; } = new List<UserListening>();
    public ICollection<UserListening> ListenedBy { get; set; } = new List<UserListening>();
}
