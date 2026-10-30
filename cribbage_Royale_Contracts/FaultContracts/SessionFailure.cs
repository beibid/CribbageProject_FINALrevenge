using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.FaultContracts
{
    /// <summary>
    /// Motivos por los que el servidor puede rechazar la apertura de una sesión.
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