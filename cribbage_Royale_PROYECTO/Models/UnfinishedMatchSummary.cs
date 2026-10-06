using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cribbage_Royale_PROYECTO.Models
{
    public class UnfinishedMatchSummary
    {
        public long MatchId { get; set; }
        public string OpponentDisplayName { get; set; }
        public int ViewerScore { get; set; }
        public int OpponentScore { get; set; }
    }
}
