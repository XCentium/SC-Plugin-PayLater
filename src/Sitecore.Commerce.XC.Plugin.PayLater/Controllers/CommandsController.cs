namespace Sitecore.Commerce.XC.Plugin.PayLater
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Http.OData;
    using Microsoft.AspNetCore.Mvc;
    using Sitecore.Commerce.Core;
    using PayLater.Commands;
    using PayLater.Arguments;
    using Sitecore.Commerce.Plugin.Carts;
    
    public class CommandsController : CommerceController
    {
       
        public CommandsController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment)
            : base(serviceProvider, globalEnvironment)
        {
        }

        [HttpPut]
        [Route("AddPaymentDetailsForPayLater()")]
        public async Task<IActionResult> AddPaymentDetailsForPayLater([FromBody] ODataActionParameters value)
        {

            // IMPORTANT: BEFORE CALLING THIS API, CALL ADDPAYMENTS FROM SITECORE WITH AMT = 0 and set a billing party as below
            // AddPaymentInfoRequest request = new AddPaymentInfoRequest(cart, new List<PaymentInfo> { federatedPaymentInfo });
            // AddPaymentInfoResult result = CartServiceProvider?.AddPaymentInfo(request);
            // This plugin relies on accepting the billing address during checkout
            // If billing address is not collected during checkout for pay later, then extend the AcceptPayLaterPaymentOnOrder to accept billing party

            CommandsController commandsController = this;
            if (!commandsController.ModelState.IsValid || value == null)
                return new BadRequestObjectResult(commandsController.ModelState);
            if (value.ContainsKey("CartId"))
            {
                var cartId = value["CartId"] != null ? value["CartId"].ToString() : string.Empty;               
                var amt = value["Amount"] != null ? value["Amount"].ToString() : string.Empty;

                AddPayLaterPaymentArgument arg = new AddPayLaterPaymentArgument()
                {
                    Amount = Decimal.Parse(amt),
                    CartId = cartId
                };

                AddPayLaterPaymentDetailsToCartCommand command = commandsController.Command<AddPayLaterPaymentDetailsToCartCommand>();
                Cart cart = await command.Process(commandsController.CurrentContext, arg).ConfigureAwait(false);
                return new ObjectResult(command);
            }

            return new BadRequestObjectResult(value);
        }

        [HttpPut]
        [Route("AcceptPayLaterPaymentOnOrder()")]
        public async Task<IActionResult> AcceptPayLaterPaymentOnOrder([FromBody] ODataActionParameters value)
        {
            CommandsController commandsController = this;

            if (!commandsController.ModelState.IsValid || value == null)
            {
                return new BadRequestObjectResult(commandsController.ModelState);
            }

            if (value.ContainsKey("OrderId"))
            {
                var orderId = value["OrderId"] != null ? value["OrderId"].ToString() : string.Empty;
                var paymentInstrumentType = value["PaymentInstrumentType"] != null ? value["PaymentInstrumentType"].ToString() : string.Empty;
                var maskedNumber = value["MaskedNumber"] != null ? value["MaskedNumber"].ToString() : string.Empty;
                var paymentMethodNonce = value["PaymentMethodNonce"] != null ? value["PaymentMethodNonce"].ToString() : string.Empty;
                var amt = value["Amount"] != null ? value["Amount"].ToString() : string.Empty;
                var transactionId = value["TransactionId"] != null ? value["TransactionId"].ToString() : string.Empty;
                var transactionStatus = value["TransactionStatus"] != null ? value["TransactionStatus"].ToString() : string.Empty;                
                var expdate = value["Expdate"] != null ? value["Expdate"].ToString() : string.Empty;                
                var cardtype = value["Cardtype"] != null ? value["Cardtype"].ToString() : string.Empty;
                
                AcceptPaymentForPayLaterOrderArgument arg = new AcceptPaymentForPayLaterOrderArgument()
                {
                    Cardtype = cardtype,
                    OrderId = orderId,
                    Expdate = expdate,
                    Amount = Decimal.Parse(amt),
                    CardType = cardtype,
                    MaskedNumber = maskedNumber,
                    PaymentInstrumentType = paymentInstrumentType,
                    PaymentMethodNonce = paymentMethodNonce,
                    TransactionId = transactionId,
                    TransactionStatus = transactionStatus
                };

                AcceptPayLaterPaymentOnOrderCommand command = commandsController.Command<AcceptPayLaterPaymentOnOrderCommand>();
                var result = await command.Process(commandsController.CurrentContext, arg).ConfigureAwait(false);
                return new ObjectResult(command);
            }

            return new BadRequestObjectResult(value);
        }
    }
}

