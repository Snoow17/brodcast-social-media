using BrodcastSocialMedia.Models;

namespace BrodcastSocialMedia.ViewModels
{
    public class UsersIndexViewModel
    {
        public string Search { get; set; }
        public List<ApplicationUser> Result { get; set; } = new List<ApplicationUser>();
    }
}
