using System.Collections.Generic;
using CribbageRoyale.Contracts.DataContracts;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class ScoreboardResult
    {
        public ScoreboardResult(bool succeeded, ServiceFailureReason reason, List<ScoreboardEntry> entries)
        {
            Succeeded = succeeded;
            Reason = reason;
            Entries = entries;
        }

        public bool Succeeded { get; }

        public ServiceFailureReason Reason { get; }

        public List<ScoreboardEntry> Entries { get; }
    }
}