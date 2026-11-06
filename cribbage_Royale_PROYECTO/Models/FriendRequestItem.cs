using System.Windows;

namespace cribbage_Royale_PROYECTO.Models
{
    public class FriendRequestItem
    {
        public long RequestId { get; set; }

        public string DisplayName { get; set; }

        public string StatusText { get; set; }

        public Visibility ResponseVisibility { get; set; }

        public Visibility CancelVisibility { get; set; }
    }
}