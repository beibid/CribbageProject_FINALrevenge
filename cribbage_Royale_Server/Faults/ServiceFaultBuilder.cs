using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using CribbageRoyale.Contracts.Faults;

namespace CribbageRoyale.Server.Faults
{
    public static class ServiceFaultBuilder
    {
        public static FaultException<ServiceFault> Build(ServiceFaultCode code, string detail)
        {
            ServiceFault fault = new ServiceFault
            {
                Code = code,
                Detail = detail
            };

            return new FaultException<ServiceFault>(fault, new FaultReason(code.ToString()));
        }
    }
}