using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bank.Business.Entities;
using System.ServiceModel;

namespace Bank.Services.Interfaces
{
    [ServiceContract]
    public interface IOperationOutcomeService
    {
        [OperationContract(IsOneWay = true)]
        void NotifyOperationOutcome(OperationOutcome pOutcome); 
    }
}
