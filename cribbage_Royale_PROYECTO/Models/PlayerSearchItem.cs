using System.Windows;

namespace cribbage_Royale_PROYECTO.Models
{
    public class PlayerSearchItem
    {
        public int AccountId { get; set; }

        public string DisplayName { get; set; }

        public string UserName { get; set; }

        public string RelationshipText { get; set; }

        public Visibility SendRequestVisibility { get; set; }
    }
}