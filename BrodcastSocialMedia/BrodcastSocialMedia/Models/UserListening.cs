
using System.ComponentModel.DataAnnotations.Schema;

namespace BrodcastSocialMedia.Models
{
    public class UserListening
    {
        public string ListenerId { get; set; }
        public ApplicationUser Listener { get; set; }

        public string TargetId { get; set; }
        public ApplicationUser Target { get; set; }
        [NotMapped]
        public IEnumerable<Broadcast> Broadcasts => Target?.Broadcasts;
    }
}
