using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bank.Services.Interfaces;
using Bank.Business.Entities;
using VideoStore.Business.Components.Interfaces;
using Microsoft.Practices.ServiceLocation;
using VideoStore.Business.Entities;

namespace VideoStore.Services
{
    class TransferNotificationService : IOperationOutcomeService
    {
        IOrderProvider Provider
        {
            get { return ServiceLocator.Current.GetInstance<IOrderProvider>(); }
        }

        public void NotifyOperationOutcome(String reference, OperationOutcome pOutcome) 
        {
            Console.WriteLine("Received tranfer outcome");
            Provider.NotifyTransferCompletion(reference, getOutcome(pOutcome));
        }

        private TransferOutcome getOutcome(OperationOutcome pOutcome)
        {
            if (pOutcome.Outcome == OperationOutcome.OperationOutcomeResult.Successful)
            {
                return new TransferOutcome() { Outcome = TransferOutcome.TransferOutcomeResult.Successful, Message = pOutcome.Message };
            }
            else
            {
                return new TransferOutcome() { Outcome = TransferOutcome.TransferOutcomeResult.Failure, Message = pOutcome.Message };
            }
        }
    }
}
