using System;
using cribbage_Royale_DataAccess.DataAccess;
using CribbageRoyale.Contracts.DataContracts;

namespace CribbageRoyale.Server.Projection
{
    public static class RecordProjection
    {
        public static AccountReference BuildAccount(PlayerRecord record)
        {
            AccountReference account = new AccountReference();
            account.AccountId = record.AccountId;
            account.UserName = record.UserName;
            account.DisplayName = record.DisplayName;

            return account;
        }

        public static DateTime ToUtc(DateTime? localDate)
        {
            DateTime utcDate = DateTime.MinValue;

            if (localDate.HasValue)
            {
                utcDate = localDate.Value.ToUniversalTime();
            }

            return utcDate;
        }
    }
}