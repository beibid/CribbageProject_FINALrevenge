using System.Collections.Generic;
using CribbageRoyale.Contracts.DataContracts;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class ChatMessageStore
    {
        private readonly Dictionary<int, List<ChatMessage>> _conversations =
            new Dictionary<int, List<ChatMessage>>();

        public void Add(int accountId, ChatMessage message)
        {
            ResolveConversation(accountId).Add(message);
        }

        public List<ChatMessage> GetConversation(int accountId)
        {
            return new List<ChatMessage>(ResolveConversation(accountId));
        }

        private List<ChatMessage> ResolveConversation(int accountId)
        {
            List<ChatMessage> conversation;

            if (!_conversations.TryGetValue(accountId, out conversation))
            {
                conversation = new List<ChatMessage>();
                _conversations[accountId] = conversation;
            }

            return conversation;
        }
    }
}