using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Jugador encontrado en una búsqueda.
    /// </summary>
    [DataContract]
    public class PlayerSummary
    {
        /// <summary>
        /// Identidad pública del jugador.
        /// </summary>
        [DataMember(IsRequired = true)]
        public AccountReference Account { get; set; }

        /// <summary>
        /// Indica si el jugador tiene una foto de perfil propia.
        /// </summary>
        [DataMember(IsRequired = true)]
        public bool HasProfileImage { get; set; }

        /// <summary>
        /// Relación del jugador con quien buscó.
        /// </summary>
        [DataMember(IsRequired = true)]
        public RelationshipKind Relationship { get; set; }
    }
}