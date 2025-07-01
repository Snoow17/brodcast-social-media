using BrodcastSocialMedia.Data.Migrations;
using BrodcastSocialMedia.Models;

namespace BrodcastSocialMedia.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<BroadcastViewModel> Broadcasts { get; set; }
        public string Message { get; set; }
        public IFormFile? Image { get; set; } 
        
    }
}
