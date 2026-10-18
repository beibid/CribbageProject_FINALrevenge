using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    [DataContract]
    public class ScoreboardEntry
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public short SkillLevel { get; set; }

        [DataMember]
        public int MatchesPlayed { get; set; }

        [DataMember]
        public int Wins { get; set; }

        [DataMember]
        public int Losses { get; set; }

        [DataMember]
        public decimal WinRate { get; set; }
    }
}
