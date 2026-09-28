using System;
using System.Collections.Generic;
using System.Linq;
using cribbage_Royale_PROYECTO.Models;

namespace cribbage_Royale_PROYECTO.DataAccess
{
    public class BlockRepository
    {
        private const int BlockedPlayerListLimit = 500;

        public List<BlockedPlayerSummary> GetBlockedPlayers(int accountId)
        {
            List<BlockedPlayerSummary> blockedPlayers = new List<BlockedPlayerSummary>();
            using (CribbageEntities context = new CribbageEntities())
            {
                List<bloqueo> blockEntities = context.bloqueo
                    .Where(blockRow => blockRow.id_usuario_que_bloquea == accountId)
                    .OrderByDescending(blockRow => blockRow.fecha_bloqueo)
                    .Take(BlockedPlayerListLimit)
                    .ToList();
                foreach (bloqueo blockEntity in blockEntities)
                {
                    usuario blockedUserEntity = context.usuario.First(userRow => userRow.id_usuario == blockEntity.id_usuario_bloqueado);
                    perfil_jugador blockedProfileEntity = context.perfil_jugador.First(profileRow => profileRow.id_usuario == blockEntity.id_usuario_bloqueado);
                    BlockedPlayerSummary summary = new BlockedPlayerSummary();
                    summary.AccountId = blockedUserEntity.id_usuario;
                    summary.DisplayName = blockedProfileEntity.nombre_mostrado;
                    summary.Username = blockedUserEntity.nombre_usuario;
                    summary.BlockDate = blockEntity.fecha_bloqueo;
                    blockedPlayers.Add(summary);
                }
            }
            return blockedPlayers;
        }

        public void UnblockPlayer(int accountId, int blockedAccountId)
        {
            using (CribbageEntities context = new CribbageEntities())
            {
                bloqueo blockEntity = context.bloqueo.First(blockRow =>
                    blockRow.id_usuario_que_bloquea == accountId && blockRow.id_usuario_bloqueado == blockedAccountId);
                context.bloqueo.Remove(blockEntity);
                context.SaveChanges();
            }
        }
        public void BlockPlayer(int accountId, int blockedAccountId)
        {
            using (CribbageEntities context = new CribbageEntities())
            {
                bloqueo existingBlock = context.bloqueo.FirstOrDefault(currentBlock => currentBlock.id_usuario_que_bloquea == accountId && currentBlock.id_usuario_bloqueado == blockedAccountId);

                if (existingBlock == null)
                {
                    bloqueo newBlock = new bloqueo();
                    newBlock.id_usuario_que_bloquea = accountId;
                    newBlock.id_usuario_bloqueado = blockedAccountId;
                    newBlock.fecha_bloqueo = DateTime.Now;
                    context.bloqueo.Add(newBlock);
                    context.SaveChanges();
                }
            }
        }
    }
}