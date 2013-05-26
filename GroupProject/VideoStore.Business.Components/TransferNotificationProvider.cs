using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoStore.Business.Components.Interfaces;
using VideoStore.Business.Entities;

namespace VideoStore.Business.Components
{
    public class TransferNotificationProvider : ITransferNotificationProvider
    {
        public void NotifyTransferCompletion(TransferOutcome outcome)
        {
            if (outcome.Outcome == TransferOutcome.TransferOutcomeResult.Failure)
            {
                throw new Exception(outcome.Message);
            }
        }
    }
}
