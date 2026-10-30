using System.Collections.Generic;

namespace cribbage_Royale_DataAccess.DataAccess
{
    public interface IFriendshipRepository
    {
        List<FriendRecord> GetFriends(int accountId);

        List<int> GetFriendIds(int accountId);
    }
}