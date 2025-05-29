using BrodcastSocialMedia.Models;
using System.Collections.Generic;

namespace BrodcastSocialMedia.ViewModels
{
    public class UsersShowUserViewModel
    {
        public ApplicationUser User { get; set; }
        public List<Broadcast> Broadcasts { get; set; } = new List<Broadcast>();
        public bool IsListening { get; internal set; }
        public string CurrentUserId { get; set; }
    }
}
