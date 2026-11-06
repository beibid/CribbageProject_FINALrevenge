using CribbageRoyale.Contracts.FaultContracts;
using cribbage_Royale_PROYECTO.Properties.Langs;

namespace cribbage_Royale_PROYECTO.Localization
{
    public static class ChatMessages
    {
        public static string ResolveRejection(ChatFailure failure)
        {
            string message;

            switch (failure)
            {
                case ChatFailure.FriendshipNotFound:
                    message = Lang.Chat_FriendshipNotFoundMessage;
                    break;
                case ChatFailure.RecipientUnavailable:
                    message = Lang.Chat_RecipientUnavailableMessage;
                    break;
                case ChatFailure.InvalidMessageText:
                    message = Lang.Chat_InvalidMessageTextMessage;
                    break;
                default:
                    message = Lang.Chat_SendErrorMessage;
                    break;
            }

            return message;
        }
    }
}