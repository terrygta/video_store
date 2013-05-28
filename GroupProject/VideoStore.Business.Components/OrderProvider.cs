using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoStore.Business.Components.Interfaces;
using VideoStore.Business.Entities;
using System.Transactions;
using VideoStore.Business.Components.TransferService;
using VideoStore.Business.Components.DeliveryService;
using Microsoft.Practices.ServiceLocation;

namespace VideoStore.Business.Components
{
    public class OrderProvider : IOrderProvider
    {
        public IEmailProvider EmailProvider
        {
            get { return ServiceLocator.Current.GetInstance<IEmailProvider>(); }
        }

        public void SubmitOrder(Entities.Order pOrder)
        {
            using (TransactionScope lScope = new TransactionScope())
            using (VideoStoreEntityModelContainer lContainer = new VideoStoreEntityModelContainer())
            {
                try
                {
                    Console.WriteLine("Invoke submit");
                    pOrder.OrderNumber = Guid.NewGuid();
                    pOrder.UpdateStockLevels();
                    lContainer.Orders.ApplyChanges(pOrder);
                    lContainer.SaveChanges();
                    TransferFundsFromCustomer(pOrder.Customer.BankAccountNumber, pOrder.Total ?? 0.0, pOrder.OrderNumber.ToString());
                    //PlaceDeliveryForOrder(pOrder);
                    //lContainer.Orders.ApplyChanges(pOrder);
                    //pOrder.UpdateStockLevels();
                    //lContainer.SaveChanges();
                    lScope.Complete();
                    //SendOrderPlacedConfirmation(pOrder);
                }
                catch (Exception lException)
                {
                    SendOrderErrorMessage(pOrder, lException);
                    throw;
                }
            }
        }

        public void NotifyTransferCompletion(String reference, TransferOutcome outcome)
        {
            Order lAffectedOrder = RetrieveOrder(new Guid(reference));
            if (outcome.Outcome == TransferOutcome.TransferOutcomeResult.Failure)
            {
                using (TransactionScope lScope = new TransactionScope())
                using (VideoStoreEntityModelContainer lContainer = new VideoStoreEntityModelContainer())
                {
                    Console.WriteLine("Rolling Back");
                    lAffectedOrder.RollbackStockLevels();
                    lContainer.Orders.ApplyChanges(lAffectedOrder);
                    lScope.Complete();
                }
            }
            else
            {
                using (TransactionScope lScope = new TransactionScope())
                using (VideoStoreEntityModelContainer lContainer = new VideoStoreEntityModelContainer())
                {
                    try
                    {
                        PlaceDeliveryForOrder(lAffectedOrder);
                        lContainer.Orders.ApplyChanges(lAffectedOrder);
                        lContainer.SaveChanges();
                        lScope.Complete();
                        SendOrderPlacedConfirmation(lAffectedOrder);
                    }
                    catch (Exception lException)
                    {
                        SendOrderErrorMessage(lAffectedOrder, lException);
                        throw;
                    }
                }
            }
        }

        private void SendOrderErrorMessage(Order pOrder, Exception pException)
        {
            EmailProvider.SendMessage(new EmailMessage()
            {
                ToAddress = pOrder.Customer.Email,
                Message = "There was an error in processsing your order " + pOrder.OrderNumber + ": "+ pException.Message +". Please contact Video Store"
            });
        }

        private void SendOrderPlacedConfirmation(Order pOrder)
        {
            EmailProvider.SendMessage(new EmailMessage()
            {
                ToAddress = pOrder.Customer.Email,
                Message = "Your order " + pOrder.OrderNumber + " has been placed"
            });
        }

        private void PlaceDeliveryForOrder(Order pOrder)
        {
            Delivery lDelivery = new Delivery() { DeliveryStatus = DeliveryStatus.Submitted, SourceAddress = "Video Store Address", DestinationAddress = pOrder.Customer.Address, Order = pOrder };
            DeliveryServiceClient lClient = new DeliveryServiceClient();
            Guid lDeliveryIdentifier = lClient.SubmitDelivery(new DeliveryInfo()
            { 
                OrderNumber = lDelivery.Order.OrderNumber.ToString(),  
                SourceAddress = lDelivery.SourceAddress,
                DestinationAddress = lDelivery.DestinationAddress,
                DeliveryNotificationAddress = "net.tcp://localhost:9010/DeliveryNotificationService"
            });

            lDelivery.ExternalDeliveryIdentifier = lDeliveryIdentifier;
            pOrder.Delivery = lDelivery;
            
        }

        private void TransferFundsFromCustomer(int pCustomerAccountNumber, double pTotal, String reference)
        {
            TransferServiceClient lClient = new TransferServiceClient();
            String orderServiceAddress = "net.msmq://localhost/private/TransferNotificationQueueTransacted";
            lClient.Transfer(pTotal, pCustomerAccountNumber, RetrieveVideoStoreAccountNumber(), reference, orderServiceAddress);
        }



        private int RetrieveVideoStoreAccountNumber()
        {
            return 123;
        }

        private Order RetrieveOrder(Guid Id)
        {
            using (VideoStoreEntityModelContainer lContainer = new VideoStoreEntityModelContainer())
            {
                Order lOrder = lContainer.Orders.Include("Customer").Include("OrderItems.Media").Where((pOrder) => pOrder.OrderNumber == Id).FirstOrDefault();
                return lOrder;
            }
        }
    }
}
