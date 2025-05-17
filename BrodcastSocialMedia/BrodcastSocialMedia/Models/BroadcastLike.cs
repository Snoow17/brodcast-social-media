namespace BrodcastSocialMedia.Models
{
    public class BroadcastLike
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int BroadcastId { get; set; }
        public Broadcast Broadcast { get; set; }

        public DateTime LikedAt { get; set; }
    }
}
