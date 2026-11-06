namespace CribbageRoyale.Server.Matches
{
    public class PendingInvitation
    {
        public PendingInvitation(long invitationId, int senderAccountId, int recipientAccountId)
        {
            InvitationId = invitationId;
            SenderAccountId = senderAccountId;
            RecipientAccountId = recipientAccountId;
        }

        public long InvitationId { get; }

        public int SenderAccountId { get; }

        public int RecipientAccountId { get; }
    }
}