using CribbageRoyale.Server.Connections;
using CribbageRoyale.Server.Matches;
using CribbageRoyale.Server.Projection;
using CribbageRoyale.Server.Sessions;

namespace CribbageRoyale.Server.Services
{
    public class MatchPlayServices
    {
        public CallerSession CallerSession { get; set; }

        public IPlayerChannel PlayerChannel { get; set; }

        public IStateProjection StateProjection { get; set; }

        public MatchRoomRegistry MatchRooms { get; set; }
    }
}