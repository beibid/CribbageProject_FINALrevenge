using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.ServiceContracts;

namespace CribbageRoyale.Server.Connections
{
    public class ChatNotification : Notification
    {
        private readonly ChatMessage _message;

        public ChatNotification(ChatMessage message)
        {
            _message = message;
        }

        public override void Deliver(IGameClientCallback callback)
        {
            callback.ChatMessageReceived(_message);
        }
    }
}