using System;
using System.Linq;
using cribbage_Royale_DataAccess.Models;

namespace cribbage_Royale_DataAccess.DataAccess
{
    public class PlayerProfileRepository
    {
        public PublicProfileSummary GetPublicProfile(int viewerAccountId, int targetAccountId)
        {
            PublicProfileSummary summary = new PublicProfileSummary();
            using (CribbageEntities context = new CribbageEntities())
            {
                account targetUserEntity = context.account.First(userRow => userRow.account_id == targetAccountId);
                player_profile targetProfileEntity = context.player_profile.First(profileRow => profileRow.account_id == targetAccountId);
                summary.AccountId = targetAccountId;
                summary.DisplayName = targetProfileEntity.display_name;
                summary.Username = targetUserEntity.username;
                summary.RegistrationDate = targetUserEntity.registered_at;
                summary.MatchesPlayed = targetProfileEntity.matches_played;
                summary.Wins = targetProfileEntity.wins;
                summary.WinPercentage = CalculateWinPercentage(targetProfileEntity.matches_played, targetProfileEntity.wins);
                summary.SkillLevel = targetProfileEntity.skill_level;
                summary.RelationshipStatus = DetermineRelationshipStatus(context, viewerAccountId, targetAccountId);
            }
            return summary;
        }

        private decimal CalculateWinPercentage(int matchesPlayed, int wins)
        {
            decimal winPercentage;
            if (matchesPlayed == 0)
            {
                winPercentage = 0m;
            }
            else
            {
                winPercentage = Math.Round(wins * 100m / matchesPlayed, 2);
            }
            return winPercentage;
        }

        private string DetermineRelationshipStatus(CribbageEntities context, int viewerAccountId, int targetAccountId)
        {
            string relationshipStatus;
            user_block blockEntity = context.user_block.FirstOrDefault(blockRow =>
                blockRow.blocker_account_id == viewerAccountId && blockRow.blocked_account_id == targetAccountId);
            friendship friendshipEntity = context.friendship.FirstOrDefault(friendRow =>
                (friendRow.requester_account_id == viewerAccountId && friendRow.addressee_account_id == targetAccountId)
                || (friendRow.requester_account_id == targetAccountId && friendRow.addressee_account_id == viewerAccountId));
            if (blockEntity != null)
            {
                relationshipStatus = RelationshipStatus.Blocked;
            }
            else if (friendshipEntity == null)
            {
                relationshipStatus = RelationshipStatus.None;
            }
            else if (friendshipEntity.status == FriendshipStatus.Accepted)
            {
                relationshipStatus = RelationshipStatus.Friend;
            }
            else if (friendshipEntity.status == FriendshipStatus.Pending && friendshipEntity.requester_account_id == viewerAccountId)
            {
                relationshipStatus = RelationshipStatus.RequestSent;
            }
            else if (friendshipEntity.status == FriendshipStatus.Pending && friendshipEntity.requester_account_id == targetAccountId)
            {
                relationshipStatus = RelationshipStatus.RequestReceived;
            }
            else
            {
                relationshipStatus = RelationshipStatus.None;
            }
            return relationshipStatus;
        }
    }
}