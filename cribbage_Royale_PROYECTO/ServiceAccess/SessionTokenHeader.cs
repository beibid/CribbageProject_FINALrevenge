using System.ServiceModel;
using System.ServiceModel.Channels;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public static class SessionTokenHeader
    {
        public const string HeaderName = "SessionToken";

        public const string HeaderNamespace = "urn:cribbageroyale:services:v1";

        public static void Attach(OperationContext context, string token)
        {
            MessageHeader<string> typedHeader = new MessageHeader<string>(token);
            context.OutgoingMessageHeaders.Add(typedHeader.GetUntypedHeader(HeaderName, HeaderNamespace));
        }
    }
}