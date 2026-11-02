using System.ServiceModel;
using CribbageRoyale.Contracts.FaultContracts;

namespace CribbageRoyale.Server.Faults
{
    public static class FriendshipFaultBuilder
    {
        public static FaultException<FriendshipFault> Build(FriendshipFailure reason)
        {
            FriendshipFault friendshipFault = new FriendshipFault();
            friendshipFault.Reason = reason;

            FaultException<FriendshipFault> faultException =
                new FaultException<FriendshipFault>(friendshipFault, new FaultReason(reason.ToString()));

            return faultException;
        }
    }
}