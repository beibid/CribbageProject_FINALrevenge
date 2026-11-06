using System.ServiceModel;
using CribbageRoyale.Contracts.FaultContracts;

namespace CribbageRoyale.Server.Faults
{
    public static class InvitationFaultBuilder
    {
        public static FaultException<InvitationFault> Build(InvitationFailure failure)
        {
            InvitationFault fault = new InvitationFault();
            fault.Reason = failure;

            return new FaultException<InvitationFault>(fault, new FaultReason(failure.ToString()));
        }
    }
}