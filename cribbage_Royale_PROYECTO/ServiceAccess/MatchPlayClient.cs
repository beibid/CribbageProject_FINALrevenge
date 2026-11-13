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

        public Task<DiscardResult> DiscardAsync(string sessionToken, CribDiscard discard)
        {
            return Task.Run(() => _invoker.Execute(
                sessionToken,
                channel => Discard(channel, discard),
                BuildFailedResult));
        }

        private DiscardResult Discard(IMatchPlay channel, CribDiscard discard)
        {
            channel.DiscardToCrib(discard);

            return new DiscardResult(ServiceFailureReason.None, null);
        }

        private DiscardResult BuildFailedResult(ServiceFailureReason reason, MatchFailure? failure)
        {
            return new DiscardResult(reason, failure);
        }
    }
}