using System;
using System.Collections.Generic;
using System.Linq;
using cribbage_Royale_DataAccess.Models;

namespace cribbage_Royale_DataAccess.DataAccess
{
    public class BlockRepository
    {
        private const int BlockedPlayerListLimit = 500;

        public List<BlockedPlayerSummary> GetBlockedPlayers(int accountId)
        {
            List<BlockedPlayerSummary> blockedPlayers = new List<BlockedPlayerSummary>();
            using (CribbageEntities context = new CribbageEntities())
            {
                List<user_block> blockEntities = context.user_block
                    .Where(blockRow => blockRow.blocker_account_id == accountId)
                    .OrderByDescending(blockRow => blockRow.blocked_at)
                    .Take(BlockedPlayerListLimit)
                    .ToList();
                foreach (user_block blockEntity in blockEntities)
                {
                    account blockedUserEntity = context.account.First(userRow => userRow.account_id == blockEntity.blocked_account_id);
                    player_profile blockedProfileEntity = context.player_profile.First(profileRow => profileRow.account_id == blockEntity.blocked_account_id);
                    BlockedPlayerSummary summary = new BlockedPlayerSummary();
                    summary.AccountId = blockedUserEntity.account_id;
                    summary.DisplayName = blockedProfileEntity.display_name;
                    summary.Username = blockedUserEntity.username;
                    summary.BlockDate = blockEntity.blocked_at;
                    blockedPlayers.Add(summary);
                }
            }
            return blockedPlayers;
        }

        public void UnblockPlayer(int accountId, int blockedAccountId)
        {
            using (CribbageEntities context = new CribbageEntities())
            {
                user_block blockEntity = context.user_block.First(blockRow =>
                    blockRow.blocker_account_id == accountId && blockRow.blocked_account_id == blockedAccountId);
                context.user_block.Remove(blockEntity);
                context.SaveChanges();
            }
        }
        public void BlockPlayer(int accountId, int blockedAccountId)
        {
            using (CribbageEntities context = new CribbageEntities())
            {
                user_block existingBlock = context.user_block.FirstOrDefault(currentBlock => currentBlock.blocker_account_id == accountId && currentBlock.blocked_account_id == blockedAccountId);

                if (existingBlock == null)
                {
                    user_block newBlock = new user_block();
                    newBlock.blocker_account_id = accountId;
                    newBlock.blocked_account_id = blockedAccountId;
                    newBlock.blocked_at = DateTime.Now;
                    context.user_block.Add(newBlock);
                    context.SaveChanges();
                }
            }
        }
    }
}