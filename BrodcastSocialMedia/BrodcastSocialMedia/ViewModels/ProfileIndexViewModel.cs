using System.ComponentModel.DataAnnotations;

namespace BrodcastSocialMedia.ViewModels
{
    public class ProfileIndexViewModel
    {
        [Required]
        public string Name { get; set; }
        public IFormFile? ProfileImage { get; set; }  // This is required for file upload
        public string ProfileImageUrl { get; set; }
    }

}
