using System.Threading.Tasks;
using CribbageRoyale.Contracts.FaultContracts;
using CribbageRoyale.Contracts.ServiceContracts;
using Microsoft.Extensions.Logging;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class MatchInvitationClient
    {
        private const string EndpointName = "MatchInvitationEndpoint";

        private readonly BusinessServiceInvoker<IMatchInvitation, InvitationFault, InvitationFailure> _invoker;

        public MatchInvitationClient(ILogger logger)
        {
            _invoker = new BusinessServiceInvoker<IMatchInvitation, InvitationFault, InvitationFailure>(
                EndpointName,
                fault => fault.Reason,
                logger);
        }

        public Task<InvitationResult> SendInvitationAsync(string sessionToken, int accountId)
        {
            return Task.Run(() => _invoker.Execute(
                sessionToken,
                channel => Invite(channel, accountId),
                BuildFailedResult));
        }

        public Task<InvitationResult> RespondAsync(string sessionToken, long invitationId, bool accepted)
        {
            return Task.Run(() => _invoker.Execute(
                sessionToken,
                channel => Respond(channel, invitationId, accepted),
                BuildFailedResult));
        }

        public Task<InvitationResult> CancelAsync(string sessionToken, long invitationId)
        {
            return Task.Run(() => _invoker.Execute(
                sessionToken,
                channel => Cancel(channel, invitationId),
                BuildFailedResult));
        }

        private InvitationResult Invite(IMatchInvitation channel, int accountId)
        {
            return new InvitationResult(
                ServiceFailureReason.None,
                null,
                channel.SendInvitation(accountId));
        }

        private InvitationResult Respond(IMatchInvitation channel, long invitationId, bool accepted)
        {
            channel.RespondToInvitation(invitationId, accepted);

            return BuildSucceededResult();
        }

        private InvitationResult Cancel(IMatchInvitation channel, long invitationId)
        {
            channel.CancelInvitation(invitationId);

            return BuildSucceededResult();
        }

        private InvitationResult BuildSucceededResult()
        {
            return new InvitationResult(ServiceFailureReason.None, null, null);
        }

        private InvitationResult BuildFailedResult(ServiceFailureReason reason, InvitationFailure? failure)
        {
            return new InvitationResult(reason, failure, null);
        }
    }
}