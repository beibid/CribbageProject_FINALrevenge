using System;
using System.ServiceModel;
using CribbageRoyale.Contracts.FaultContracts;

namespace CribbageRoyale.Server.Faults
{
    public static class ServiceFaultBuilder
    {
        public static FaultException<ServiceFault> Build(ServiceFaultCode code)
        {
            ServiceFault serviceFault = new ServiceFault();
            serviceFault.Code = code;
            serviceFault.CorrelationId = Guid.NewGuid().ToString("N");

            FaultException<ServiceFault> faultException =
                new FaultException<ServiceFault>(serviceFault, new FaultReason(code.ToString()));

            return faultException;
        }
    }
}