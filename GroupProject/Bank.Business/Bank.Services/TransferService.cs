﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bank.Services.Interfaces;
using Bank.Business.Components.Interfaces;
using System.ServiceModel;
using Microsoft.Practices.ServiceLocation;

namespace Bank.Services
{
    public class TransferService : ITransferService
    {
        private ITransferProvider TransferProvider
        {
            get { return ServiceLocator.Current.GetInstance<ITransferProvider>(); }
        }

        [OperationBehavior(TransactionScopeRequired=false)]
        public void Transfer(double pAmount, int pFromAcctNumber, int pToAcctNumber, String reference, String pResultReturnAddress)
        {
            TransferProvider.Transfer(pAmount, pFromAcctNumber, pToAcctNumber, reference, pResultReturnAddress);
        }
    }
}
