using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class ProfileResult
    {
        public ProfileResult(ServiceFailureReason reason, ProfileFailure? failure, OwnProfile profile)
        {
            Reason = reason;
            Failure = failure;
            Profile = profile;
        }

        public ServiceFailureReason Reason { get; }

        public ProfileFailure? Failure { get; }

        public OwnProfile Profile { get; }

        public bool Succeeded
        {
            get { return Reason == ServiceFailureReason.None; }
        }
    }
}