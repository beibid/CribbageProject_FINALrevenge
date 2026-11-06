using CribbageRoyale.Server.Connections;
using CribbageRoyale.Server.Projection;
using Microsoft.Extensions.Logging;

namespace CribbageRoyale.Server.Matches
{
    public class MatchWatcherServices
    {
        public IPlayerChannel PlayerChannel { get; set; }

        public IStateProjection StateProjection { get; set; }

        public ILogger Logger { get; set; }
    }
}