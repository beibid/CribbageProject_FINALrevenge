using System.ServiceModel;
using CribbageRoyale.Contracts.FaultContracts;

namespace CribbageRoyale.Server.Faults
{
    public static class ChatFaultBuilder
    {
        public static FaultException<ChatFault> Build(ChatFailure reason)
        {
            ChatFault chatFault = new ChatFault();
            chatFault.Reason = reason;

            FaultException<ChatFault> faultException =
                new FaultException<ChatFault>(chatFault, new FaultReason(reason.ToString()));

            return faultException;
        }
    }
}