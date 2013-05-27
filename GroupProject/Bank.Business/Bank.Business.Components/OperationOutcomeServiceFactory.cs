using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bank.Services.Interfaces;
using System.ServiceModel;

namespace Bank.Business.Components
{
    public class OperationOutcomeServiceFactory
    {
        public static IOperationOutcomeService GetOperationOutcomeService(String pAddress)
        {
            IOperationOutcomeService foo;
            try
            {
                ChannelFactory<IOperationOutcomeService> lChannelFactory =
                    new ChannelFactory<IOperationOutcomeService>(new NetMsmqBinding("NetMsmqBinding_IOperationOutcomeService"), new EndpointAddress(pAddress));
                foo = lChannelFactory.CreateChannel();
            }
            catch (Exception lException)
            {
                Console.WriteLine("Error:  " + lException.Message);
                throw;
            }
            return foo;
        }
    }
}
