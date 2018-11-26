using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;
using System.Threading.Tasks;
using Sitecore.Commerce.XC.Plugin.PayLater.Arguments;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Orders;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Plugin.ManagedLists;
using Sitecore.Commerce.Plugin.Payments;
using System.Linq;

namespace Sitecore.Commerce.XC.Plugin.PayLater.Pipelines.Blocks
{
    [PipelineDisplayName("Pipelines.Blocks.AcceptPaymentForPayLaterOrderBlock")]
    public class AcceptPaymentForPayLaterOrderBlock : PipelineBlock<AcceptPaymentForPayLaterOrderArgument, Cart, CommercePipelineExecutionContext>
    {
        private readonly IHoldOrderPipeline _holdOrderPipeline;
        private readonly IPersistEntityPipeline _persistEntityPipeline;
        private readonly IFindEntityPipeline _findEntityPipeline;
        private readonly GetOnHoldOrderCartCommand _getOnHoldOrderCartCommand;
        private readonly IAddPayLaterPaymentDetailsPipeline _storePaymentDetailsPipeline;
        private readonly IReleaseOnHoldOrderPipeline _releaseOnHoldOrderPipeline;
        private readonly IAddPaymentsPipeline _addPaymentsPipeline;

        public AcceptPaymentForPayLaterOrderBlock(IHoldOrderPipeline holdOrderPipeline, IReleaseOnHoldOrderPipeline releaseOnHoldOrderPipeline, 
                IFindEntityPipeline findEntityPipeline, IPersistEntityPipeline persistEntityPipeline, GetOnHoldOrderCartCommand getOnHoldOrderCartCommand, 
                IAddPayLaterPaymentDetailsPipeline storePaymentDetailsPipeline, IAddPaymentsPipeline addPaymentsPipeline)
        {
            _findEntityPipeline = findEntityPipeline;
            _getOnHoldOrderCartCommand = getOnHoldOrderCartCommand;
            _holdOrderPipeline = holdOrderPipeline;
            _persistEntityPipeline = persistEntityPipeline;
            _storePaymentDetailsPipeline = storePaymentDetailsPipeline;
            _releaseOnHoldOrderPipeline = releaseOnHoldOrderPipeline;
            _addPaymentsPipeline = addPaymentsPipeline;
        }

        public async override Task<Cart> Run(AcceptPaymentForPayLaterOrderArgument arg, CommercePipelineExecutionContext context)
        {
            //1] convert order to cart by holding order
            var orderId = CommerceEntity.IdPrefix<Order>() + arg.OrderId;
            var order = await _holdOrderPipeline.Run(orderId, context.ContextOptions);

            //modify the temp cart instead!
            if (order == null || !order.HasComponent<OnHoldOrderComponent>())
            {              
                context.Logger.LogError(string.Format("{0}: HoldOrderPipeline returned null order for OrderId: {1}", Name, orderId));
                var executionContext = context;
                var commerceContext = context.CommerceContext;
                var validationError = context.GetPolicy<KnownResultCodes>().ValidationError;
                const string commerceTermKey = "FailedToHoldOrder";
                var args = new object[] { arg.OrderId };
                var defaultMessage = $"Failed to hold Order with id: {arg.OrderId}.";
                executionContext.Abort(await commerceContext.AddMessage(validationError, commerceTermKey, args, defaultMessage), context);

                return null;
            }

            //2] get TEMP CART from ON HOLD order
            var tempCart = await _getOnHoldOrderCartCommand.Process(context.CommerceContext, order);

            if (tempCart == null)
            {
                context.Logger.LogError(string.Format("{0}: GetOnHoldOrderCartCommand returned null tempCart for OrderId: {1}", Name, orderId));
                var executionContext = context;
                var commerceContext = context.CommerceContext;
                var validationError = context.GetPolicy<KnownResultCodes>().ValidationError;
                const string commerceTermKey = "FailedToFindTemporaryCart";
                var args = new object[] { arg.OrderId };
                var defaultMessage = $"Failed to find temporary cart for order id: {order.Id}.";
                executionContext.Abort(await commerceContext.AddMessage(validationError, commerceTermKey, args, defaultMessage), context);

                return null;
            }
            
            // add the federated payment           
            var federatedPaymentComponents = tempCart.Components.OfType<FederatedPaymentComponent>().ToList();

            var payment = tempCart.HasComponent<FederatedPaymentComponent>() ? tempCart.GetComponent<FederatedPaymentComponent>()
                           : new FederatedPaymentComponent(new Money(context.CommerceContext.CurrentCurrency(), arg.Amount));

            payment.PaymentMethodNonce = arg.PaymentMethodNonce;
            payment.MaskedNumber = arg.MaskedNumber;
            payment.TransactionId = arg.TransactionId;
            payment.TransactionStatus = arg.TransactionStatus;
            payment.CardType = arg.Cardtype;
            payment.PaymentInstrumentType = arg.PaymentInstrumentType;
            payment.PaymentInstrumentType = "credit_card";

            if (!string.IsNullOrEmpty(arg.Expdate))
            {
                var expiryYear = arg.Expdate.Substring(arg.Expdate.Length - 2, 2);
                var expiryMonth = arg.Expdate.Replace(expiryYear, "");

                payment.ExpiresMonth = int.Parse(expiryMonth);
                payment.ExpiresYear = int.Parse(expiryYear);
            }

            payment.Amount = new Money(context.CommerceContext.CurrentCurrency(), arg.Amount);
                        
            if(payment.BillingParty == null)
            {
                payment.BillingParty = new Party() { Address1 = "PAYLATER", CountryCode = "US", FirstName = "PAYLATER", LastName = "PAYLATER" };
            }


            tempCart.SetComponent(payment);


            if (tempCart != null)
            {
                if (tempCart.HasComponent<PayLaterComponent>())
                {                    
                    //add counter pay later component instead of removing
                    var newPayLaterComponent = new PayLaterComponent(new Money(arg.Amount * -1));
                    newPayLaterComponent.Comments = $"Counter Pay Later For Pay Later component being paid via credit card {arg.TransactionId}";
                    tempCart.Components.Add(newPayLaterComponent);                    
                }
            }

            return tempCart;
        }
    }
}
