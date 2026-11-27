using System.Collections.Generic;
using CribbageRoyale.Server.Matches;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CribbageRoyale.Server.Tests.Matches
{
    [TestClass]
    public class MatchInvitationRegistryTests
    {
        private const int SenderAccountId = 7;
        private const int RecipientAccountId = 8;
        private const int ThirdAccountId = 9;
        private const int FourthAccountId = 10;

        private MatchInvitationRegistry _invitationRegistry;

        [TestInitialize]
        public void SetUp()
        {
            _invitationRegistry = new MatchInvitationRegistry();
        }

        [TestMethod]
        public void TryCreate_NewPair_CreatesInvitationWithBothAccounts()
        {
            PendingInvitation invitation;

            bool isCreated = _invitationRegistry.TryCreate(SenderAccountId, RecipientAccountId, out invitation);

            Assert.IsTrue(isCreated);
            Assert.AreEqual(SenderAccountId, invitation.SenderAccountId);
            Assert.AreEqual(RecipientAccountId, invitation.RecipientAccountId);
        }

        [TestMethod]
        public void TryCreate_SamePairTwice_RejectsSecondInvitation()
        {
            PendingInvitation firstInvitation;
            PendingInvitation secondInvitation;
            _invitationRegistry.TryCreate(SenderAccountId, RecipientAccountId, out firstInvitation);

            bool isCreated = _invitationRegistry.TryCreate(SenderAccountId, RecipientAccountId, out secondInvitation);

            Assert.IsFalse(isCreated);
        }

        [TestMethod]
        public void TryCreate_TwoInvitations_AssignsIncreasingIds()
        {
            PendingInvitation firstInvitation;
            PendingInvitation secondInvitation;

            _invitationRegistry.TryCreate(SenderAccountId, RecipientAccountId, out firstInvitation);
            _invitationRegistry.TryCreate(SenderAccountId, ThirdAccountId, out secondInvitation);

            Assert.IsTrue(secondInvitation.InvitationId > firstInvitation.InvitationId);
        }

        [TestMethod]
        public void TryTake_WrongOwner_KeepsInvitation()
        {
            PendingInvitation invitation;
            PendingInvitation takenInvitation;
            _invitationRegistry.TryCreate(SenderAccountId, RecipientAccountId, out invitation);

            bool isTakenByWrongOwner = _invitationRegistry.TryTake(
                invitation.InvitationId, pending => false, out takenInvitation);

            Assert.IsFalse(isTakenByWrongOwner);
            Assert.IsTrue(_invitationRegistry.TryTake(invitation.InvitationId, pending => true, out takenInvitation));
        }

        [TestMethod]
        public void TryTake_RightOwner_RemovesInvitation()
        {
            PendingInvitation invitation;
            PendingInvitation takenInvitation;
            _invitationRegistry.TryCreate(SenderAccountId, RecipientAccountId, out invitation);

            bool isTaken = _invitationRegistry.TryTake(invitation.InvitationId, pending => true, out takenInvitation);

            Assert.IsTrue(isTaken);
            Assert.AreSame(invitation, takenInvitation);
            Assert.IsFalse(_invitationRegistry.TryTake(invitation.InvitationId, pending => true, out takenInvitation));
        }

        [TestMethod]
        public void TakeAllFor_Account_TakesOnlyInvitationsInvolvingAccount()
        {
            PendingInvitation ignoredInvitation;
            _invitationRegistry.TryCreate(SenderAccountId, RecipientAccountId, out ignoredInvitation);
            _invitationRegistry.TryCreate(ThirdAccountId, SenderAccountId, out ignoredInvitation);
            _invitationRegistry.TryCreate(ThirdAccountId, FourthAccountId, out ignoredInvitation);

            List<PendingInvitation> takenInvitations = _invitationRegistry.TakeAllFor(SenderAccountId);

            Assert.AreEqual(2, takenInvitations.Count);
            Assert.AreEqual(1, _invitationRegistry.TakeAllFor(ThirdAccountId).Count);
        }
    }
}
