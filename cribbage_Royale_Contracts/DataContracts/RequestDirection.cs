using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Sentido de una solicitud desde el punto de vista de quien consulta.
    /// </summary>
    [DataContract]
    public enum RequestDirection
    {
        /// <summary>
        /// Quien consulta es el destinatario de la solicitud.
        /// </summary>
        [EnumMember]
        Received,

        /// <summary>
        /// Quien consulta es el remitente de la solicitud.
        /// </summary>
        [EnumMember]
        Sent
    }
}