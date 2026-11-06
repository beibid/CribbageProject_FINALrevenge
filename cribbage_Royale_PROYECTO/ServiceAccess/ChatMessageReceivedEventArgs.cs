using System;
using CribbageRoyale.Contracts.DataContracts;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class ChatMessageReceivedEventArgs : EventArgs
    {
        public ChatMessageReceivedEventArgs(ChatMessage message)
        {
            Message = message;
        }

        public ChatMessage Message { get; }
    }
}