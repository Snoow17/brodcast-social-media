using BrodcastSocialMedia.Data.Migrations;
using BrodcastSocialMedia.Models;

namespace BrodcastSocialMedia.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<BroadcastViewModel> Broadcasts { get; set; }
        public string Message { get; set; }
        public IFormFile? Image { get; set; } // For broadcast image
        public int Id { get; set; }

        public string Title { get; set; }

        public int LikeCount { get; set; }

        public bool IsLikedByCurrentUser { get; set; }
    }
}
