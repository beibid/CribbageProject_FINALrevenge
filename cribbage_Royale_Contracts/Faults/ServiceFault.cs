using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.Faults
{
    [DataContract]
    public class ServiceFault
    {
        [DataMember]
        public ServiceFaultCode Code { get; set; }

        [DataMember]
        public string Detail { get; set; }
    }
}
