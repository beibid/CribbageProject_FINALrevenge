using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cribbage_Royale_PROYECTO.Models
{
    public class MatchItem
    {
        public string OpponentText { get; set; }
        public string DetailsText { get; set; }
        public string ScoreText { get; set; }
        public string ResultText { get; set; }
        public bool IsVictory { get; set; }
    }
}
