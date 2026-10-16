using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using CribbageRoyale.Contracts.Faults;
using CribbageRoyale.Contracts.Transfer;

namespace CribbageRoyale.Contracts.Services
{
    [ServiceContract]
    public interface IStatistics
    {
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        Task<List<ScoreboardEntry>> GetScoreboardAsync();
    }
}