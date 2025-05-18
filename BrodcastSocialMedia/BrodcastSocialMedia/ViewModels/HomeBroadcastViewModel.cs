using System.ComponentModel.DataAnnotations;

namespace BrodcastSocialMedia.ViewModels
{
    public class HomeBroadcastViewModel
    {
        [Required(ErrorMessage = "Message cannot be empty.")]
        public string Message { get; set; }
        public IFormFile? Image { get; set; }

    }
}
