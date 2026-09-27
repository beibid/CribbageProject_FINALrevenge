using System;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using cribbage_Royale_PROYECTO.Models;

namespace cribbage_Royale_PROYECTO.DataAccess
{
    public class VerificationCodeRepository
    {
        public const string AccountVerificationPurpose = "VERIFICACION_CUENTA";
        public const string PasswordResetPurpose = "RESTABLECIMIENTO_CONTRASENA";
        public const string EmailChangePurpose = "CAMBIO_CORREO";

        private const string VerifiedAccountStatus = "VERIFICADA";
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
                codigo_verificacion verificationCode = new codigo_verificacion
                {
                    id_usuario = userId,
                    tipo = purpose,
                    valor_codigo = ComputeHash(code),
                    correo_destino = email,
                    fecha_creacion = DateTime.Now,
                    fecha_expiracion = DateTime.Now.AddHours(CodeValidityHours)
                };

                context.codigo_verificacion.Add(verificationCode);
                context.SaveChanges();
            }

            return code;
        }

        private CodeValidationOutcome EvaluateAccount(string email, string code)
        {
            CodeValidationOutcome outcome;

            using (CribbageEntities context = new CribbageEntities())
            {
                usuario account = context.usuario.FirstOrDefault(user => user.correo == email);

                if (account == null)
                {
                    outcome = CodeValidationOutcome.InvalidCode;
                }
                else if (account.estado_cuenta == VerifiedAccountStatus)
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
            CribbageEntities context, usuario account, string code)
        {
            CodeValidationOutcome outcome;
            string hashedCode = ComputeHash(code);

            codigo_verificacion storedCode = context.codigo_verificacion
                .Where(item => item.id_usuario == account.id_usuario
                    && item.tipo == AccountVerificationPurpose
                    && item.valor_codigo == hashedCode
                    && item.fecha_uso == null)
                .OrderByDescending(item => item.fecha_creacion)
                .FirstOrDefault();

            if (storedCode == null)
            {
                outcome = CodeValidationOutcome.InvalidCode;
            }
            else if (storedCode.fecha_expiracion < DateTime.Now)
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
            CribbageEntities context, usuario account, codigo_verificacion storedCode)
        {
            storedCode.fecha_uso = DateTime.Now;
            account.estado_cuenta = VerifiedAccountStatus;
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