namespace BrodcastSocialMedia.ViewModels
{
    public class ProfileIndexViewModel
    {
        public string Name { get; set; }
        public IFormFile ProfileImage { get; set; }  // This is required for file upload
        public string ProfileImageUrl { get; set; }
    }

}
