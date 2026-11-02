using CribbageRoyale.Contracts.FaultContracts;
using cribbage_Royale_PROYECTO.Properties.Langs;

namespace cribbage_Royale_PROYECTO.Localization
{
    public static class FriendRequestMessages
    {
        public static string ResolveRejection(FriendshipFailure failure)
        {
            string message;

            switch (failure)
            {
                case FriendshipFailure.CannotTargetSelf:
                    message = Lang.AddFriend_CannotAddYourselfMessage;
                    break;
                case FriendshipFailure.FriendshipAlreadyExists:
                    message = Lang.AddFriend_AlreadyFriendsMessage;
                    break;
                case FriendshipFailure.RequestAlreadyPending:
                    message = Lang.AddFriend_RequestAlreadySentMessage;
                    break;
                default:
                    message = Lang.AddFriend_PlayerUnavailableMessage;
                    break;
            }

            return message;
        }
    }
}