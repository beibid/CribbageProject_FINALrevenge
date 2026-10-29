using System.Collections.Generic;
using System.ServiceModel;
using CribbageRoyale.Contracts.FaultContracts;
using CribbageRoyale.Contracts.DataContracts;

namespace CribbageRoyale.Contracts.ServiceContracts
{
    [ServiceContract]
    public interface IStatistics
    {
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        List<ScoreboardEntry> GetScoreboard();
    }
}