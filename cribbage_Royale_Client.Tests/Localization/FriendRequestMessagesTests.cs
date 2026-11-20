using CribbageRoyale.Contracts.FaultContracts;
using cribbage_Royale_PROYECTO.Localization;
using cribbage_Royale_PROYECTO.Properties.Langs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CribbageRoyale.Client.Tests.Localization
{
    [TestClass]
    public class FriendRequestMessagesTests
    {
        [TestMethod]
        public void ResolveRejection_CannotTargetSelf_ReturnsCannotAddYourselfMessage()
        {
            FriendshipFailure failure = FriendshipFailure.CannotTargetSelf;

            string message = FriendRequestMessages.ResolveRejection(failure);

            Assert.AreEqual(Lang.AddFriend_CannotAddYourselfMessage, message);
        }

        [TestMethod]
        public void ResolveRejection_FriendshipAlreadyExists_ReturnsAlreadyFriendsMessage()
        {
            FriendshipFailure failure = FriendshipFailure.FriendshipAlreadyExists;

            string message = FriendRequestMessages.ResolveRejection(failure);

            Assert.AreEqual(Lang.AddFriend_AlreadyFriendsMessage, message);
        }

        [TestMethod]
        public void ResolveRejection_RequestAlreadyPending_ReturnsRequestAlreadySentMessage()
        {
            FriendshipFailure failure = FriendshipFailure.RequestAlreadyPending;

            string message = FriendRequestMessages.ResolveRejection(failure);

            Assert.AreEqual(Lang.AddFriend_RequestAlreadySentMessage, message);
        }

        [TestMethod]
        public void ResolveRejection_FriendshipNotFound_ReturnsFriendshipNotFoundMessage()
        {
            FriendshipFailure failure = FriendshipFailure.FriendshipNotFound;

            string message = FriendRequestMessages.ResolveRejection(failure);

            Assert.AreEqual(Lang.Friends_FriendshipNotFoundMessage, message);
        }

        [TestMethod]
        public void ResolveRejection_RequestUnavailable_ReturnsRequestUnavailableMessage()
        {
            FriendshipFailure failure = FriendshipFailure.RequestUnavailable;

            string message = FriendRequestMessages.ResolveRejection(failure);

            Assert.AreEqual(Lang.FriendRequests_RequestUnavailableMessage, message);
        }

        [TestMethod]
        public void ResolveRejection_UnauthorizedRequest_ReturnsRequestUnavailableMessage()
        {
            FriendshipFailure failure = FriendshipFailure.UnauthorizedRequest;

            string message = FriendRequestMessages.ResolveRejection(failure);

            Assert.AreEqual(Lang.FriendRequests_RequestUnavailableMessage, message);
        }

        [TestMethod]
        public void ResolveRejection_OtherFailure_ReturnsPlayerUnavailableMessage()
        {
            FriendshipFailure failure = FriendshipFailure.InvalidAccountId;

            string message = FriendRequestMessages.ResolveRejection(failure);

            Assert.AreEqual(Lang.AddFriend_PlayerUnavailableMessage, message);
        }
    }
}