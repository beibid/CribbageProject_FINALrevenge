using System;
using CribbageRoyale.Contracts.DataContracts;

namespace CribbageRoyale.Server.Projection
{
    public interface IStateProjection
    {
        PresenceChange BuildPresenceChange(int accountId, bool isConnected);

        SocialEvent BuildSocialEvent(SocialEventRecord record);

        ChatMessage BuildChatMessage(int senderAccountId, string text, DateTime sentAtUtc);
    }
}