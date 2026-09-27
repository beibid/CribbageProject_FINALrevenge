using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cribbage_Royale_PROYECTO.Models
{
    public class PlayingCardItem
    {
        public string Rank { get; set; }

        public string Suit { get; set; }

        public bool IsRedSuit { get; set; }

        public bool IsSelected { get; set; }

        public bool IsPlayable { get; set; } = true;

        public string DisplayText
        {
            get { return Rank + " " + Suit; }
        }
    }
}
