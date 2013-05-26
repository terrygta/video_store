using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoStore.Business.Entities;

namespace VideoStore.Business.Components.Interfaces
{
    public interface ITransferNotificationProvider
    {
        void NotifyTransferCompletion(TransferOutcome outcome);
    }
}
