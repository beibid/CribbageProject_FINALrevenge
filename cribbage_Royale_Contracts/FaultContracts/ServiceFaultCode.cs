using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.FaultContracts
{
    /// <summary>
    /// Códigos de falla técnica que puede transportar <see cref="ServiceFault"/>.
    /// </summary>
    [DataContract]
    public enum ServiceFaultCode
    {
        /// <summary>
        /// La operación falló al leer o escribir en la base de datos.
        /// </summary>
        [EnumMember]
        DataAccessFailure,

        /// <summary>
        /// La operación no pudo entregar un correo electrónico.
        /// </summary>
        [EnumMember]
        MailDeliveryFailure,

        /// <summary>
        /// La operación falló por una condición no prevista dentro del servidor.
        /// </summary>
        [EnumMember]
        InternalFailure
    }
}