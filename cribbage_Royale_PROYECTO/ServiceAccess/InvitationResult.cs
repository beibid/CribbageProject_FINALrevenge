using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class InvitationResult
    {
        public InvitationResult(ServiceFailureReason reason, InvitationFailure? failure,
            MatchInvitation invitation)
        {
            Reason = reason;
            Failure = failure;
            Invitation = invitation;
        }

        public bool Succeeded
        {
            get { return Reason == ServiceFailureReason.None; }
        }

        public ServiceFailureReason Reason { get; }

        public InvitationFailure? Failure { get; }

        public MatchInvitation Invitation { get; }
    }
}