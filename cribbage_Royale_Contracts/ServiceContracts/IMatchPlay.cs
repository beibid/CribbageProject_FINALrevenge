using System.ServiceModel;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;

namespace CribbageRoyale.Contracts.ServiceContracts
{
    /// <summary>
    /// Operaciones que un jugador invoca durante una partida en curso.
    /// </summary>
    [ServiceContract]
    public interface IMatchPlay
    {
        /// <summary>
        /// Envía dos cartas de la mano del jugador al crib del repartidor.
        /// </summary>
        /// <param name="discard">Cartas a descartar y versión del estado conocida.</param>
        [OperationContract]
        [FaultContract(typeof(MatchFault))]
        [FaultContract(typeof(SessionFault))]
        void DiscardToCrib(CribDiscard discard);
    }
}