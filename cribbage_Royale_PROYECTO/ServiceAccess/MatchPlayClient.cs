using System.Threading.Tasks;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;
using CribbageRoyale.Contracts.ServiceContracts;
using Microsoft.Extensions.Logging;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class MatchPlayClient
    {
        private const string EndpointName = "MatchPlayEndpoint";

        private readonly BusinessServiceInvoker<IMatchPlay, MatchFault, MatchFailure> _invoker;

        public MatchPlayClient(ILogger logger)
        {
            _invoker = new BusinessServiceInvoker<IMatchPlay, MatchFault, MatchFailure>(
                EndpointName,
                fault => fault.Reason,
                logger);
        }

        public Task<MatchActionResult> DiscardAsync(string sessionToken, CribDiscard discard)
        {
            return Task.Run(() => _invoker.Execute(
                sessionToken,
                channel => Discard(channel, discard),
                BuildFailedResult));
        }

        public Task<MatchActionResult> PlayCardAsync(string sessionToken, CardPlay play)
        {
            return Task.Run(() => _invoker.Execute(
                sessionToken,
                channel => Play(channel, play),
                BuildFailedResult));
        }

        private MatchActionResult Discard(IMatchPlay channel, CribDiscard discard)
        {
            channel.DiscardToCrib(discard);

            return new MatchActionResult(ServiceFailureReason.None, null);
        }

        private MatchActionResult Play(IMatchPlay channel, CardPlay play)
        {
            channel.PlayCard(play);

            return new MatchActionResult(ServiceFailureReason.None, null);
        }

        private MatchActionResult BuildFailedResult(ServiceFailureReason reason, MatchFailure? failure)
        {
            return new MatchActionResult(reason, failure);
        }
    }
}