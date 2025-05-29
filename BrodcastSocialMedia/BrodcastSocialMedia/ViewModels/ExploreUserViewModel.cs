namespace BrodcastSocialMedia.ViewModels
{
    public class ExploreUserViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int BroadcastCount { get; set; }
        public string ProfileImageUrl { get; set; }
        public bool IsListening { get; internal set; }
    }
}
