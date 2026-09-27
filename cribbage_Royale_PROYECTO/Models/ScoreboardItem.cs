using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cribbage_Royale_PROYECTO.Models
{
    public class ScoreboardItem
    {
        public int Position { get; set; }

        public string PlayerName { get; set; }

        public short Level { get; set; }

        public int Played { get; set; }

        public int Won { get; set; }

        public int Lost { get; set; }

        public decimal WinRate { get; set; }

        public string WinRateText { get; set; }

        public bool IsCurrentPlayer { get; set; }
    }
}
