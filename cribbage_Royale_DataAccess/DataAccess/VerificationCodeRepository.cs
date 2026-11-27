using System;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using cribbage_Royale_DataAccess.Models;

namespace cribbage_Royale_DataAccess.DataAccess
{
    public class VerificationCodeRepository
    {
        public const string AccountVerificationPurpose = "ACCOUNT_VERIFICATION";
        public const string PasswordResetPurpose = "PASSWORD_RESET";
        public const string EmailChangePurpose = "EMAIL_CHANGE";

        private const string VerifiedAccountStatus = "VERIFIED";
        private const int CodeValidityHours = 24;
        private const int RandomByteCount = 4;
        private const int CodeUpperBound = 1000000;
        private const int PositiveValueMask = 0x7FFFFFFF;
        private const string CodeFormat = "D6";
        private const string HashByteSeparator = "-";

        public string CreateCode(int userId, string purpose, string email)
        {
            string code = string.Empty;

            try
            {
                code = SaveCode(userId, purpose, email);
            }
            catch (EntityException entityException)
            {
                Trace.WriteLine(entityException.Message);
            }
            catch (SqlException sqlException)
            {
                Trace.WriteLine(sqlException.Message);
            }

            return code;
        }

        public CodeValidationOutcome ValidateAccountCode(string email, string code)
        {
            CodeValidationOutcome outcome;

            try
            {
                outcome = EvaluateAccount(email, code);
            }
            catch (EntityException entityException)
            {
                Trace.WriteLine(entityException.Message);
                outcome = CodeValidationOutcome.DatabaseError;
            }
            catch (SqlException sqlException)
            {
                Trace.WriteLine(sqlException.Message);
                outcome = CodeValidationOutcome.DatabaseError;
            }

            return outcome;
        }

        private string SaveCode(int userId, string purpose, string email)
        {
            string code = GenerateCode();

            using (CribbageEntities context = new CribbageEntities())
            {
                verification_code verificationCode = new verification_code
                {
                    account_id = userId,
                    code_type = purpose,
                    code_value = ComputeHash(code),
                    target_email = email,
                    created_at = DateTime.Now,
                    expires_at = DateTime.Now.AddHours(CodeValidityHours)
                };

                context.verification_code.Add(verificationCode);
                context.SaveChanges();
            }

            return code;
        }

        private CodeValidationOutcome EvaluateAccount(string email, string code)
        {
            CodeValidationOutcome outcome;

            using (CribbageEntities context = new CribbageEntities())
            {
                account account = context.account.FirstOrDefault(user => user.email == email);

                if (account == null)
                {
                    outcome = CodeValidationOutcome.InvalidCode;
                }
                else if (account.account_status == VerifiedAccountStatus)
                {
                    outcome = CodeValidationOutcome.AccountAlreadyVerified;
                }
                else
                {
                    outcome = EvaluateStoredCode(context, account, code);
                }
            }

            return outcome;
        }

        private CodeValidationOutcome EvaluateStoredCode(
            CribbageEntities context, account account, string code)
        {
            CodeValidationOutcome outcome;
            string hashedCode = ComputeHash(code);

            verification_code storedCode = context.verification_code
                .Where(item => item.account_id == account.account_id
                    && item.code_type == AccountVerificationPurpose
                    && item.code_value == hashedCode
                    && item.used_at == null)
                .OrderByDescending(item => item.created_at)
                .FirstOrDefault();

            if (storedCode == null)
            {
                outcome = CodeValidationOutcome.InvalidCode;
            }
            else if (storedCode.expires_at < DateTime.Now)
            {
                outcome = CodeValidationOutcome.ExpiredCode;
            }
            else
            {
                ActivateAccount(context, account, storedCode);
                outcome = CodeValidationOutcome.Valid;
            }

            return outcome;
        }

        private void ActivateAccount(
            CribbageEntities context, account account, verification_code storedCode)
        {
            storedCode.used_at = DateTime.Now;
            account.account_status = VerifiedAccountStatus;
            context.SaveChanges();
        }

        private string GenerateCode()
        {
            byte[] randomBytes = new byte[RandomByteCount];

            using (RNGCryptoServiceProvider generator = new RNGCryptoServiceProvider())
            {
                generator.GetBytes(randomBytes);
            }

            int randomValue = BitConverter.ToInt32(randomBytes, 0) & PositiveValueMask;

            return (randomValue % CodeUpperBound).ToString(CodeFormat, CultureInfo.InvariantCulture);
        }

        private string ComputeHash(string code)
        {
            string hashedCode;

            using (SHA256 algorithm = SHA256.Create())
            {
                byte[] hashBytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(code));
                hashedCode = BitConverter.ToString(hashBytes)
                    .Replace(HashByteSeparator, string.Empty)
                    .ToLowerInvariant();
            }

            return hashedCode;
        }
    }
}