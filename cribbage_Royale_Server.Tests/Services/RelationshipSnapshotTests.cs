using System.Collections.Generic;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Server.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CribbageRoyale.Server.Tests.Services
{
    [TestClass]
    public class RelationshipSnapshotTests
    {
        private const int FriendAccountId = 10;
        private const int SentRequestAccountId = 20;
        private const int ReceivedRequestAccountId = 30;
        private const int UnrelatedAccountId = 40;

        private RelationshipSnapshot _relationshipSnapshot;

        [TestInitialize]
        public void SetUp()
        {
            _relationshipSnapshot = new RelationshipSnapshot(
                new List<int> { FriendAccountId },
                new List<int> { SentRequestAccountId },
                new List<int> { ReceivedRequestAccountId });
        }

        [TestMethod]
        public void Resolve_FriendAccount_ReturnsFriend()
        {
            RelationshipKind relationship = _relationshipSnapshot.Resolve(FriendAccountId);

            Assert.AreEqual(RelationshipKind.Friend, relationship);
        }

        [TestMethod]
        public void Resolve_SentRequestAccount_ReturnsRequestSent()
        {
            RelationshipKind relationship = _relationshipSnapshot.Resolve(SentRequestAccountId);

            Assert.AreEqual(RelationshipKind.RequestSent, relationship);
        }

        [TestMethod]
        public void Resolve_ReceivedRequestAccount_ReturnsRequestReceived()
        {
            RelationshipKind relationship = _relationshipSnapshot.Resolve(ReceivedRequestAccountId);

            Assert.AreEqual(RelationshipKind.RequestReceived, relationship);
        }

        [TestMethod]
        public void Resolve_UnrelatedAccount_ReturnsNone()
        {
            RelationshipKind relationship = _relationshipSnapshot.Resolve(UnrelatedAccountId);

            Assert.AreEqual(RelationshipKind.None, relationship);
        }

        [TestMethod]
        public void Resolve_FriendWithSentRequest_ReturnsFriend()
        {
            RelationshipSnapshot relationshipSnapshot = new RelationshipSnapshot(
                new List<int> { FriendAccountId },
                new List<int> { FriendAccountId },
                new List<int>());

            RelationshipKind relationship = relationshipSnapshot.Resolve(FriendAccountId);

            Assert.AreEqual(RelationshipKind.Friend, relationship);
        }
    }
}
