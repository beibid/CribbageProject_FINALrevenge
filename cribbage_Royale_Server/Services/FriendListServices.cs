using cribbage_Royale_DataAccess.DataAccess;
using CribbageRoyale.Server.Connections;

namespace CribbageRoyale.Server.Services
{
    public class FriendListServices
    {
        public CallerSession CallerSession { get; set; }

        public IFriendshipRepository FriendshipRepository { get; set; }

        public IPlayerChannel PlayerChannel { get; set; }
    }
}