using System.Threading.Tasks;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;
using CribbageRoyale.Contracts.ServiceContracts;
using Microsoft.Extensions.Logging;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class FriendChatClient
    {
        private const string EndpointName = "FriendChatEndpoint";

        private readonly BusinessServiceInvoker<IFriendChat, ChatFault, ChatFailure> _invoker;

        public FriendChatClient(ILogger logger)
        {
            _invoker = new BusinessServiceInvoker<IFriendChat, ChatFault, ChatFailure>(
                EndpointName,
                fault => fault.Reason,
                logger);
        }

        public Task<ChatMessageResult> SendMessageAsync(string sessionToken, int accountId, string text)
        {
            return Task.Run(() => _invoker.Execute(
                sessionToken,
                channel => Send(channel, accountId, text),
                BuildFailedResult));
        }

        private ChatMessageResult Send(IFriendChat channel, int accountId, string text)
        {
            return new ChatMessageResult(
                ServiceFailureReason.None,
                null,
                channel.SendMessage(accountId, text));
        }

        private ChatMessageResult BuildFailedResult(ServiceFailureReason reason, ChatFailure? failure)
        {
            return new ChatMessageResult(reason, failure, null);
        }
    }
}