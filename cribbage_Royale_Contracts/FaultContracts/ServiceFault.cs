using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.FaultContracts
{
    /// <summary>
    /// Falla técnica del servidor que no corresponde a ninguna regla de negocio.
    /// </summary>
    [DataContract]
    public class ServiceFault
    {
        /// <summary>
        /// Código que indica el origen técnico de la falla.
        /// </summary>
        [DataMember(IsRequired = true)]
        public ServiceFaultCode Code { get; set; }

        /// <summary>
        /// Identificador que permite ubicar el registro correspondiente en la bitácora del servidor.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string CorrelationId { get; set; }
    }
}