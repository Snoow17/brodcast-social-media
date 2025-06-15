using BrodcastSocialMedia.Models;
using System.Collections.Generic;

namespace BrodcastSocialMedia.ViewModels
{
    public class UsersShowUserViewModel
    {
        public ApplicationUser User { get; set; }
        public List<BroadcastViewModel> Broadcasts { get; set; }
        public bool IsListening { get; set; }
        public string CurrentUserId { get; set; }
    }
}
