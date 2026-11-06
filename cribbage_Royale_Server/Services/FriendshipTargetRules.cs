using CribbageRoyale.Contracts.FaultContracts;
using CribbageRoyale.Server.Faults;

namespace CribbageRoyale.Server.Services
{
    public static class FriendshipTargetRules
    {
        private const int FirstRegisteredAccountId = 1;

        public static void EnsureIsOtherAccount(int callerAccountId, int targetAccountId)
        {
            if (targetAccountId < FirstRegisteredAccountId)
            {
                throw FriendshipFaultBuilder.Build(FriendshipFailure.InvalidAccountId);
            }

            if (targetAccountId == callerAccountId)
            {
                throw FriendshipFaultBuilder.Build(FriendshipFailure.CannotTargetSelf);
            }
        }
    }
}