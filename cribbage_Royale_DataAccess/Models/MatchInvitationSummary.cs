using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cribbage_Royale_DataAccess.Models
{
    public class MatchInvitationSummary
    {
        public long InvitationId { get; set; }
        public int SenderAccountId { get; set; }
        public int RecipientAccountId { get; set; }
        public string SenderDisplayName { get; set; }
        public int SenderSkillLevel { get; set; }
        public string Status { get; set; }
        public DateTime ExpirationDate { get; set; }
        public long? MatchId { get; set; }
    }
}
