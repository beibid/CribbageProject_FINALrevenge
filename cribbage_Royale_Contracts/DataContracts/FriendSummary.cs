using System;
using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Identidad y presencia de un amigo.
    /// </summary>
    [DataContract]
    public class FriendSummary
    {
        /// <summary>
        /// Identidad pública del amigo.
        /// </summary>
        [DataMember(IsRequired = true)]
        public AccountReference Account { get; set; }

        /// <summary>
        /// Indica si el amigo tiene una foto de perfil propia.
        /// </summary>
        [DataMember(IsRequired = true)]
        public bool HasProfileImage { get; set; }

        /// <summary>
        /// Presencia conocida por el servidor al momento de consultar.
        /// </summary>
        [DataMember(IsRequired = true)]
        public bool IsOnline { get; set; }

        /// <summary>
        /// Instante en UTC en que quedó establecida la amistad.
        /// </summary>
        [DataMember(IsRequired = true)]
        public DateTime FriendsSinceUtc { get; set; }
    }
}