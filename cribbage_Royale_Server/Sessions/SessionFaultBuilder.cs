using System.ServiceModel;
using CribbageRoyale.Contracts.FaultContracts;

namespace CribbageRoyale.Server.Faults
{
    public static class SessionFaultBuilder
    {
        public static FaultException<SessionFault> Build(SessionFailure failure)
        {
            SessionFault sessionFault = new SessionFault();
            sessionFault.Failure = failure;

            FaultException<SessionFault> faultException =
                new FaultException<SessionFault>(sessionFault, new FaultReason(failure.ToString()));

            return faultException;
        }
    }
}