using cribbage_Royale_DataAccess.DataAccess;
using CribbageRoyale.Server.Connections;
using CribbageRoyale.Server.Projection;

namespace CribbageRoyale.Server.Services
{
    public class FriendRequestsServices
    {
        public CallerSession CallerSession { get; set; }

        public IFriendshipRepository FriendshipRepository { get; set; }

        public IPlayerChannel PlayerChannel { get; set; }

        public IStateProjection StateProjection { get; set; }
    }
}