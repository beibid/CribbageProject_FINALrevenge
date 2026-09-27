using cribbage_Royale_PROYECTO.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace cribbage_Royale_PROYECTO.DataAccess
{
    public class UserRepository
    {
        private const string VerifiedAccountStatus = "VERIFICADA";
        private const string SuspendedAccountStatus = "SUSPENDIDA";
        private const int MaximumFailedAttempts = 5;
        private const int LockoutMinutes = 15;
        private const string PendingVerificationStatus = "PENDIENTE_VERIFICACION";
        private const string DefaultAvatar = "avatar_por_omision";
        private const string DefaultLanguage = "es-MX";
        private const string DefaultColorBlindMode = "DESACTIVADO";
        private const byte DefaultMusicVolume = 70;
        private const byte DefaultEffectsVolume = 80;
        private const short InitialSkillLevel = 1000;

        public AuthenticationResult AuthenticateUser(string usernameOrEmail, string password)
        {
            AuthenticationResult result;

            try
            {
                result = EvaluateCredentials(usernameOrEmail, password);
            }
            catch (EntityException entityException)
            {
                Trace.WriteLine(entityException.Message);
                result = BuildFailedResult(AuthenticationOutcome.DatabaseError);
            }
            catch (SqlException sqlException)
            {
                Trace.WriteLine(sqlException.Message);
                result = BuildFailedResult(AuthenticationOutcome.DatabaseError);
            }

            return result;
        }

        private AuthenticationResult EvaluateCredentials(string usernameOrEmail, string password)
        {
            AuthenticationResult result;

            using (CribbageEntities context = new CribbageEntities())
            {
                usuario account = context.usuario.FirstOrDefault(
                    user => user.nombre_usuario == usernameOrEmail || user.correo == usernameOrEmail);

                if (account == null)
                {
                    result = BuildFailedResult(AuthenticationOutcome.InvalidCredentials);
                }
                else
                {
                    result = EvaluateAccount(context, account, password);
                }
            }

            return result;
        }

        private AuthenticationResult EvaluateAccount(CribbageEntities context, usuario account, string password)
        {
            AuthenticationResult result;

            if (IsAccountLocked(account))
            {
                result = BuildLockedResult(account);
            }
            else if (!IsPasswordCorrect(password, account.hash_contrasena))
            {
                RegisterFailedAttempt(context, account);
                result = BuildFailedResult(AuthenticationOutcome.InvalidCredentials);
            }
            else if (account.estado_cuenta == SuspendedAccountStatus)
            {
                result = BuildAccountResult(account, AuthenticationOutcome.AccountSuspended);
            }
            else if (account.estado_cuenta != VerifiedAccountStatus)
            {
                result = BuildAccountResult(account, AuthenticationOutcome.AccountNotVerified);
            }
            else
            {
                RegisterSuccessfulAccess(context, account);
                result = BuildAccountResult(account, AuthenticationOutcome.Success);
            }

            return result;
        }

        private bool IsAccountLocked(usuario account)
        {
            bool isLocked = false;

            if (account.fecha_fin_bloqueo_acceso.HasValue
                && account.fecha_fin_bloqueo_acceso.Value > DateTime.Now)
            {
                isLocked = true;
            }

            return isLocked;
        }

        private bool IsPasswordCorrect(string password, string storedHash)
        {
            bool isCorrect = false;

            if (!string.IsNullOrWhiteSpace(storedHash))
            {
                isCorrect = BCrypt.Net.BCrypt.Verify(password, storedHash);
            }

            return isCorrect;
        }

        private void RegisterFailedAttempt(CribbageEntities context, usuario account)
        {
            account.intentos_fallidos = (byte)(account.intentos_fallidos + 1);

            if (account.intentos_fallidos >= MaximumFailedAttempts)
            {
                account.intentos_fallidos = 0;
                account.fecha_fin_bloqueo_acceso = DateTime.Now.AddMinutes(LockoutMinutes);
            }

            context.SaveChanges();
        }

        private void RegisterSuccessfulAccess(CribbageEntities context, usuario account)
        {
            account.intentos_fallidos = 0;
            account.fecha_fin_bloqueo_acceso = null;
            context.SaveChanges();
        }

        private int CalculateRemainingLockoutMinutes(usuario account)
        {
            TimeSpan remainingTime = account.fecha_fin_bloqueo_acceso.Value - DateTime.Now;

            return (int)Math.Ceiling(remainingTime.TotalMinutes);
        }

        private AuthenticationResult BuildFailedResult(AuthenticationOutcome outcome)
        {
            return new AuthenticationResult
            {
                Outcome = outcome,
                Username = string.Empty,
                Email = string.Empty,
                LockoutRemainingMinutes = 0
            };
        }

        private AuthenticationResult BuildAccountResult(usuario account, AuthenticationOutcome outcome)
        {
            return new AuthenticationResult
            {
                Outcome = outcome,
                Username = account.nombre_usuario,
                Email = account.correo,
                LockoutRemainingMinutes = 0
            };
        }

        private AuthenticationResult BuildLockedResult(usuario account)
        {
            AuthenticationResult result = BuildAccountResult(account, AuthenticationOutcome.AccountLocked);
            result.LockoutRemainingMinutes = CalculateRemainingLockoutMinutes(account);

            return result;
        }
        public RegistrationOutcome RegisterAccount(string username, string email, string password)
        {
            RegistrationOutcome outcome;

            try
            {
                outcome = CreateAccount(username, email, password);
            }
            catch (EntityException entityException)
            {
                Trace.WriteLine(entityException.Message);
                outcome = RegistrationOutcome.DatabaseError;
            }
            catch (SqlException sqlException)
            {
                Trace.WriteLine(sqlException.Message);
                outcome = RegistrationOutcome.DatabaseError;
            }

            return outcome;
        }

        private RegistrationOutcome CreateAccount(string username, string email, string password)
        {
            RegistrationOutcome outcome;

            using (CribbageEntities context = new CribbageEntities())
            {
                if (IsUsernameTaken(context, username))
                {
                    outcome = RegistrationOutcome.UsernameAlreadyTaken;
                }
                else
                {
                    outcome = EvaluateEmailAvailability(context, username, email, password);
                }
            }

            return outcome;
        }

        private bool IsUsernameTaken(CribbageEntities context, string username)
        {
            return context.usuario.Any(user => user.nombre_usuario == username);
        }

        private RegistrationOutcome EvaluateEmailAvailability(
            CribbageEntities context, string username, string email, string password)
        {
            RegistrationOutcome outcome;

            usuario existingAccount = context.usuario.FirstOrDefault(user => user.correo == email);

            if (existingAccount == null)
            {
                outcome = SaveNewAccount(context, username, email, password);
            }
            else if (existingAccount.estado_cuenta == PendingVerificationStatus)
            {
                outcome = RegistrationOutcome.EmailPendingVerification;
            }
            else
            {
                outcome = RegistrationOutcome.EmailAlreadyRegistered;
            }

            return outcome;
        }

        private RegistrationOutcome SaveNewAccount(
            CribbageEntities context, string username, string email, string password)
        {
            using (DbContextTransaction transaction = context.Database.BeginTransaction())
            {
                usuario account = BuildAccount(username, email, password);
                context.usuario.Add(account);
                context.SaveChanges();

                context.perfil_jugador.Add(BuildProfile(account.id_usuario, username));
                context.ajustes_jugador.Add(BuildSettings(account.id_usuario));
                context.SaveChanges();

                transaction.Commit();
            }

            return RegistrationOutcome.Success;
        }

        private usuario BuildAccount(string username, string email, string password)
        {
            return new usuario
            {
                nombre_usuario = username,
                correo = email,
                hash_contrasena = BCrypt.Net.BCrypt.HashPassword(password),
                estado_cuenta = PendingVerificationStatus,
                fecha_registro = DateTime.Now,
                intentos_fallidos = 0
            };
        }

        private perfil_jugador BuildProfile(int userId, string username)
        {
            return new perfil_jugador
            {
                id_usuario = userId,
                nombre_mostrado = username,
                avatar = DefaultAvatar,
                tutorial_completado = false,
                nivel_habilidad = InitialSkillLevel,
                partidas_jugadas = 0,
                victorias = 0,
                derrotas = 0,
                racha_actual = 0
            };
        }

        private ajustes_jugador BuildSettings(int userId)
        {
            return new ajustes_jugador
            {
                id_usuario = userId,
                volumen_musica = DefaultMusicVolume,
                volumen_efectos = DefaultEffectsVolume,
                idioma = DefaultLanguage,
                modo_daltonismo = DefaultColorBlindMode,
                mostrar_animaciones = true,
                notificaciones_solicitudes = true
            };
        }
    }
}