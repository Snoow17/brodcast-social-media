namespace BrodcastSocialMedia.ViewModels
{
    public class BroadcastViewModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string ImageUrl { get; set; }
        public string UserId { get; set; }
        public DateTime Published { get; set; }
        public string ProfileImageUrl { get; set; }
        public string UserName { get; set; }
        public int LikeCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public bool IsOwnedByCurrentUser { get; set; }

    }
}
