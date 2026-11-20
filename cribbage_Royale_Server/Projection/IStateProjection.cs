using System;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Rules;

namespace CribbageRoyale.Server.Projection
{
    public interface IStateProjection
    {
        PresenceChange BuildPresenceChange(int accountId, bool isConnected);

        SocialEvent BuildSocialEvent(SocialEventRecord record);

        ChatMessage BuildChatMessage(int senderAccountId, string text, DateTime sentAtUtc);

        MatchInvitation BuildMatchInvitation(long invitationId, int senderAccountId, int recipientAccountId);

        MatchStart BuildMatchStart(long matchId, int opponentAccountId, bool isHost);

        MatchState BuildMatchState(CribbageMatch match, int accountId);
    }
}