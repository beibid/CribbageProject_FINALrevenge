using cribbage_Royale_DataAccess.DataAccess;
using CribbageRoyale.Server.Connections;
using CribbageRoyale.Server.Matches;
using CribbageRoyale.Server.Projection;
using CribbageRoyale.Server.Sessions;

namespace CribbageRoyale.Server.Services
{
    public class MatchInvitationServices
    {
        public CallerSession CallerSession { get; set; }

        public IFriendshipRepository FriendshipRepository { get; set; }

        public IPlayerChannel PlayerChannel { get; set; }

        public IStateProjection StateProjection { get; set; }

        public MatchInvitationRegistry InvitationRegistry { get; set; }

        public MatchRoomRegistry MatchRooms { get; set; }
    }
}