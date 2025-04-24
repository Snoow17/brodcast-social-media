using BrodcastSocialMedia.Models;

namespace BrodcastSocialMedia.ViewModels
{
    public class UsersShowUserViewModel
    {
        public ApplicationUser User { get; set; }
        public List<Broadcast> Broadcasts { get; set; }
    }
}
