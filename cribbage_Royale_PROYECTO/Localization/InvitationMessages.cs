using CribbageRoyale.Contracts.FaultContracts;
using cribbage_Royale_PROYECTO.Properties.Langs;

namespace cribbage_Royale_PROYECTO.Localization
{
    public static class InvitationMessages
    {
        public static string ResolveRejection(InvitationFailure failure)
        {
            string message;

            switch (failure)
            {
                case InvitationFailure.RecipientUnavailable:
                    message = Lang.AwaitingInvite_RecipientOfflineMessage;
                    break;
                case InvitationFailure.RecipientBusy:
                    message = Lang.AwaitingInvite_RecipientInMatchMessage;
                    break;
                case InvitationFailure.CallerBusy:
                    message = Lang.MatchInvite_UnfinishedMatchMessage;
                    break;
                case InvitationFailure.InvitationAlreadySent:
                    message = Lang.AwaitingInvite_AlreadyInvitedMessage;
                    break;
                case InvitationFailure.InvitationNotFound:
                    message = Lang.MatchInvite_UnavailableMessage;
                    break;
                case InvitationFailure.FriendshipNotFound:
                    message = Lang.Invitation_FriendshipNotFoundMessage;
                    break;
                default:
                    message = Lang.Invitation_ErrorMessage;
                    break;
            }

            return message;
        }
    }
}