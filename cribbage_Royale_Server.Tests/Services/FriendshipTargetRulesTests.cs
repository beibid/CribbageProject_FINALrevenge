using System.ServiceModel;
using CribbageRoyale.Contracts.FaultContracts;
using CribbageRoyale.Server.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CribbageRoyale.Server.Tests.Services
{
    [TestClass]
    public class FriendshipTargetRulesTests
    {
        private const int CallerAccountId = 7;

        [TestMethod]
        public void EnsureIsOtherAccount_TargetIsCaller_ThrowsCannotTargetSelf()
        {
            int targetAccountId = CallerAccountId;

            FaultException<FriendshipFault> faultException = Assert.ThrowsException<FaultException<FriendshipFault>>(
                () => FriendshipTargetRules.EnsureIsOtherAccount(CallerAccountId, targetAccountId));

            Assert.AreEqual(FriendshipFailure.CannotTargetSelf, faultException.Detail.Reason);
        }

        [TestMethod]
        public void EnsureIsOtherAccount_TargetIsZero_ThrowsInvalidAccountId()
        {
            int targetAccountId = 0;

            FaultException<FriendshipFault> faultException = Assert.ThrowsException<FaultException<FriendshipFault>>(
                () => FriendshipTargetRules.EnsureIsOtherAccount(CallerAccountId, targetAccountId));

            Assert.AreEqual(FriendshipFailure.InvalidAccountId, faultException.Detail.Reason);
        }

        [TestMethod]
        public void EnsureIsOtherAccount_TargetIsNegative_ThrowsInvalidAccountId()
        {
            int targetAccountId = -1;

            FaultException<FriendshipFault> faultException = Assert.ThrowsException<FaultException<FriendshipFault>>(
                () => FriendshipTargetRules.EnsureIsOtherAccount(CallerAccountId, targetAccountId));

            Assert.AreEqual(FriendshipFailure.InvalidAccountId, faultException.Detail.Reason);
        }

        [TestMethod]
        public void EnsureIsOtherAccount_TargetIsFirstRegisteredAccount_DoesNotThrow()
        {
            int targetAccountId = 1;

            FriendshipTargetRules.EnsureIsOtherAccount(CallerAccountId, targetAccountId);
        }
    }
}