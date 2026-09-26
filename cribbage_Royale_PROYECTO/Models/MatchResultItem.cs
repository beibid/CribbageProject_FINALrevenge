using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cribbage_Royale_PROYECTO.Models
{
    public class MatchResultItem
    {
        public string OpponentName { get; set; }

        public int PlayerScore { get; set; }

        public int OpponentScore { get; set; }

        public bool IsVictory { get; set; }

        public MatchEndReason EndReason { get; set; }

        public TimeSpan Duration { get; set; }

        public int SkillLevel { get; set; }

        public int SkillLevelChange { get; set; }
    }
}
