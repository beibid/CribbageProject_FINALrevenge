using System.ServiceModel;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;

namespace CribbageRoyale.Contracts.ServiceContracts
{
    /// <summary>
    /// Operations a player performs during a match.
    /// </summary>
    [ServiceContract]
    public interface IMatchPlay
    {
        /// <summary>
        /// Sends two cards from the caller's hand to the crib.
        /// </summary>
        /// <param name="discard">Cards the caller wants to discard.</param>
        [OperationContract]
        [FaultContract(typeof(MatchFault))]
        [FaultContract(typeof(SessionFault))]
        void DiscardToCrib(CribDiscard discard);

        /// <summary>
        /// Puts one card from the caller's hand on the table.
        /// </summary>
        /// <param name="play">Card the caller wants to play.</param>
        [OperationContract]
        [FaultContract(typeof(MatchFault))]
        [FaultContract(typeof(SessionFault))]
        void PlayCard(CardPlay play);

        /// <summary>
        /// Gives up the match, handing the victory to the opponent.
        /// </summary>
        /// <param name="matchId">Identifier of the match being given up.</param>
        [OperationContract]
        [FaultContract(typeof(MatchFault))]
        [FaultContract(typeof(SessionFault))]
        void AbandonMatch(long matchId);
    }
}