using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.FaultContracts
{
    /// <summary>
    /// Motivos por los que el servidor rechaza el inicio o el cierre de una sesión, o una operación
    /// que exige una sesión válida.
    /// </summary>
    [DataContract]
    public enum SessionFailure
    {
        /// <summary>
        /// El nombre de usuario, el correo o la contraseña no coinciden.
        /// </summary>
        [EnumMember]
        InvalidCredentials,

        /// <summary>
        /// La cuenta existe pero aún no ha sido verificada.
        /// </summary>
        [EnumMember]
        AccountNotVerified,

        /// <summary>
        /// La cuenta se encuentra suspendida.
        /// </summary>
        [EnumMember]
        AccountSuspended,

        /// <summary>
        /// La cuenta fue eliminada.
        /// </summary>
        [EnumMember]
        AccountDeleted,

        /// <summary>
        /// El encabezado del testigo de sesión está ausente o vacío.
        /// </summary>
        [EnumMember]
        MissingSessionToken,

        /// <summary>
        /// El testigo de sesión no es válido o ya fue revocado.
        /// </summary>
        [EnumMember]
        InvalidSession,

        /// <summary>
        /// La operación exige una cuenta registrada y la sesión es de invitado.
        /// </summary>
        [EnumMember]
        GuestNotAllowed,

        /// <summary>
        /// El nombre propuesto por el invitado no cumple el formato permitido.
        /// </summary>
        [EnumMember]
        InvalidGuestName,

        /// <summary>
        /// El nombre propuesto por el invitado ya está en uso.
        /// </summary>
        [EnumMember]
        GuestNameUnavailable
    }
}