using CribbageRoyale.Contracts.FaultContracts;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.ServiceAccess;

namespace cribbage_Royale_PROYECTO.Localization
{
    public static class ProfileMessages
    {
        public static string Resolve(ProfileResult result)
        {
            string message;

            if (result.Reason == ServiceFailureReason.RequestRejected)
            {
                message = ResolveRejection(result.Failure);
            }
            else
            {
                message = ResolveServiceFailure(result.Reason);
            }

            return message;
        }

        private static string ResolveRejection(ProfileFailure? failure)
        {
            string message;

            switch (failure)
            {
                case ProfileFailure.InvalidDisplayName:
                    message = Lang.Profile_InvalidDisplayNameMessage;
                    break;
                case ProfileFailure.InvalidPassword:
                    message = Lang.Profile_InvalidPasswordMessage;
                    break;
                case ProfileFailure.WrongPassword:
                    message = Lang.Profile_WrongPasswordMessage;
                    break;
                case ProfileFailure.InvalidImage:
                    message = Lang.Profile_InvalidImageMessage;
                    break;
                default:
                    message = Lang.Application_UnexpectedErrorMessage;
                    break;
            }

            return message;
        }

        private static string ResolveServiceFailure(ServiceFailureReason reason)
        {
            string message = Lang.Application_UnexpectedErrorMessage;

            if (reason == ServiceFailureReason.ServerUnavailable || reason == ServiceFailureReason.Timeout)
            {
                message = Lang.Profile_ServerUnavailableMessage;
            }

            return message;
        }
    }
}