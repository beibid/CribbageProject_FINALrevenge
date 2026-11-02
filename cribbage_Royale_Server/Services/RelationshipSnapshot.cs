using System.Collections.Generic;
using CribbageRoyale.Contracts.DataContracts;

namespace CribbageRoyale.Server.Services
{
    public class RelationshipSnapshot
    {
        private readonly List<int> _friendIds;
        private readonly List<int> _sentRequestRecipientIds;
        private readonly List<int> _receivedRequestSenderIds;

        public RelationshipSnapshot(
            List<int> friendIds,
            List<int> sentRequestRecipientIds,
            List<int> receivedRequestSenderIds)
        {
            _friendIds = friendIds;
            _sentRequestRecipientIds = sentRequestRecipientIds;
            _receivedRequestSenderIds = receivedRequestSenderIds;
        }

        public RelationshipKind Resolve(int accountId)
        {
            RelationshipKind relationship = RelationshipKind.None;

            if (_friendIds.Contains(accountId))
            {
                relationship = RelationshipKind.Friend;
            }
            else if (_sentRequestRecipientIds.Contains(accountId))
            {
                relationship = RelationshipKind.RequestSent;
            }
            else if (_receivedRequestSenderIds.Contains(accountId))
            {
                relationship = RelationshipKind.RequestReceived;
            }

            return relationship;
        }
    }
}