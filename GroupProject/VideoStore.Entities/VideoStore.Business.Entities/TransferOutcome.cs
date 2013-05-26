using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoStore.Business.Entities
{
    public class TransferOutcome
    {
        public enum TransferOutcomeResult { Successful, Failure };

        public String Message { get; set; }
        public TransferOutcomeResult Outcome { get; set; }
    }
}
