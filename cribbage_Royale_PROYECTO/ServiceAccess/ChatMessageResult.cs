using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class ChatMessageResult
    {
        public ChatMessageResult(ServiceFailureReason reason, ChatFailure? failure, ChatMessage message)
        {
            Reason = reason;
            Failure = failure;
            Message = message;
        }

        public bool Succeeded
        {
            get { return Reason == ServiceFailureReason.None; }
        }

        public ServiceFailureReason Reason { get; }

        public ChatFailure? Failure { get; }

        public ChatMessage Message { get; }
    }
}