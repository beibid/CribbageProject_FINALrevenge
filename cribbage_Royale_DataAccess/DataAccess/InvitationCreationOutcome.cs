using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cribbage_Royale_DataAccess.DataAccess
{
    public static class InvitationCreationOutcome
    {
        public const string Created = "CREATED";
        public const string SenderHasUnfinishedMatch = "SENDER_HAS_UNFINISHED_MATCH";
        public const string RecipientHasUnfinishedMatch = "RECIPIENT_HAS_UNFINISHED_MATCH";
        public const string RecipientOffline = "RECIPIENT_OFFLINE";
        public const string AlreadyInvitedByYou = "ALREADY_INVITED_BY_YOU";
    }
}
