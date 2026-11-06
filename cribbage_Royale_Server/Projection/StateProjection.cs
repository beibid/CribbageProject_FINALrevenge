using System;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Server.Sessions;

namespace CribbageRoyale.Server.Projection
{
    public class StateProjection : IStateProjection
    {
        private readonly PlayerRegistry _playerRegistry;

        public StateProjection(PlayerRegistry playerRegistry)
        {
            _playerRegistry = playerRegistry;
        }

        public PresenceChange BuildPresenceChange(int accountId, bool isConnected)
        {
            PresenceChange presence = new PresenceChange();
            presence.Account = ResolveAccount(accountId);
            presence.Scope = PresenceScope.Friend;
            presence.IsConnected = isConnected;
            presence.MatchId = null;

            return presence;
        }

        public SocialEvent BuildSocialEvent(SocialEventRecord record)
        {
            SocialEvent social = new SocialEvent();
            social.Counterpart = ResolveAccount(record.CounterpartAccountId);
            social.FriendRequestId = record.FriendRequestId;
            social.InvitationId = null;
            social.OccurredAtUtc = record.OccurredAtUtc;

            return social;
        }

        public ChatMessage BuildChatMessage(int senderAccountId, string text, DateTime sentAtUtc)
        {
            ChatMessage message = new ChatMessage();
            message.Sender = ResolveAccount(senderAccountId);
            message.Text = text;
            message.SentAtUtc = sentAtUtc;

            return message;
        }

        public MatchInvitation BuildMatchInvitation(long invitationId, int senderAccountId,
            int recipientAccountId)
        {
            MatchInvitation invitation = new MatchInvitation();
            invitation.InvitationId = invitationId;
            invitation.Sender = ResolveAccount(senderAccountId);
            invitation.Recipient = ResolveAccount(recipientAccountId);

            return invitation;
        }

        public MatchStart BuildMatchStart(long matchId, int opponentAccountId, bool isHost)
        {
            MatchStart matchStart = new MatchStart();
            matchStart.MatchId = matchId;
            matchStart.Opponent = ResolveAccount(opponentAccountId);
            matchStart.IsHost = isHost;

            return matchStart;
        }

        private AccountReference ResolveAccount(int accountId)
        {
            AccountReference account = _playerRegistry.Find(accountId);

            if (account == null)
            {
                account = new AccountReference();
                account.AccountId = accountId;
            }

            return account;
        }
    }
}