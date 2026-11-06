using System;
using System.Collections.Generic;
using System.Linq;

namespace CribbageRoyale.Server.Matches
{
    public class MatchInvitationRegistry
    {
        private readonly Dictionary<long, PendingInvitation> _invitations =
            new Dictionary<long, PendingInvitation>();

        private readonly object _syncRoot = new object();

        private long _lastInvitationId;

        public bool TryCreate(int senderAccountId, int recipientAccountId, out PendingInvitation invitation)
        {
            bool created = false;
            invitation = null;

            lock (_syncRoot)
            {
                if (!ContainsPair(senderAccountId, recipientAccountId))
                {
                    invitation = BuildInvitation(senderAccountId, recipientAccountId);
                    _invitations[invitation.InvitationId] = invitation;
                    created = true;
                }
            }

            return created;
        }

        public bool TryTake(long invitationId, Func<PendingInvitation, bool> isOwner,
            out PendingInvitation invitation)
        {
            PendingInvitation found;
            bool taken = false;
            invitation = null;

            lock (_syncRoot)
            {
                if (_invitations.TryGetValue(invitationId, out found) && isOwner(found))
                {
                    _invitations.Remove(invitationId);
                    invitation = found;
                    taken = true;
                }
            }

            return taken;
        }

        public List<PendingInvitation> TakeAllFor(int accountId)
        {
            List<PendingInvitation> taken;

            lock (_syncRoot)
            {
                taken = _invitations.Values.Where(invitation => Involves(invitation, accountId)).ToList();
                RemoveAll(taken);
            }

            return taken;
        }

        private PendingInvitation BuildInvitation(int senderAccountId, int recipientAccountId)
        {
            _lastInvitationId++;

            return new PendingInvitation(_lastInvitationId, senderAccountId, recipientAccountId);
        }

        private bool ContainsPair(int senderAccountId, int recipientAccountId)
        {
            return _invitations.Values.Any(
                invitation => IsSamePair(invitation, senderAccountId, recipientAccountId));
        }

        private bool IsSamePair(PendingInvitation invitation, int senderAccountId, int recipientAccountId)
        {
            return invitation.SenderAccountId == senderAccountId
                && invitation.RecipientAccountId == recipientAccountId;
        }

        private bool Involves(PendingInvitation invitation, int accountId)
        {
            return invitation.SenderAccountId == accountId || invitation.RecipientAccountId == accountId;
        }

        private void RemoveAll(List<PendingInvitation> invitations)
        {
            foreach (PendingInvitation invitation in invitations)
            {
                _invitations.Remove(invitation.InvitationId);
            }
        }
    }
}