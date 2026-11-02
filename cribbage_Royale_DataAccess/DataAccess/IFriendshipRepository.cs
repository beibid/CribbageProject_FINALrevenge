using System.Collections.Generic;

namespace cribbage_Royale_DataAccess.DataAccess
{
    public interface IFriendshipRepository
    {
        List<FriendRecord> GetFriends(int accountId);

        List<int> GetFriendIds(int accountId);

        List<PlayerRecord> SearchPlayers(int searcherAccountId, string searchText);

        List<int> GetSentRequestRecipientIds(int accountId);

        List<int> GetReceivedRequestSenderIds(int accountId);

        bool IsAccountAvailable(int accountId);

        bool AreFriends(int firstAccountId, int secondAccountId);

        bool HasPendingRequest(int firstAccountId, int secondAccountId);

        bool HasBlock(int firstAccountId, int secondAccountId);

        long CreateFriendRequest(int senderAccountId, int recipientAccountId);
    }
}