using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;
using System.Threading.Tasks;
using Sitecore.Commerce.Plugin.Carts;
using System.Linq;
using Sitecore.Commerce.XC.Plugin.PayLater.Arguments;

namespace Sitecore.Commerce.XC.Plugin.PayLater.Pipelines.Blocks
{
    [PipelineDisplayName("Pipelines.Blocks.AddPayLaterPaymentDetailsBlock")]
    public class AddPayLaterPaymentDetailsBlock : PipelineBlock<AddPayLaterPaymentArgument, Cart, CommercePipelineExecutionContext>
    {
        private readonly IFindEntityPipeline _findEntityPipeline;
        public AddPayLaterPaymentDetailsBlock(IFindEntityPipeline findEntityPipeline)
        {
            _findEntityPipeline = findEntityPipeline;
        }

        public async override Task<Cart> Run(AddPayLaterPaymentArgument arg, CommercePipelineExecutionContext context)
        {
            var cartId = arg.CartId;
            FindEntityArgument findEntityArgument = new FindEntityArgument(typeof(Cart), cartId, false);

            Cart cart = await _findEntityPipeline.Run(findEntityArgument, context) as Cart;

            if (cart == null)
            {
                string message = await context.CommerceContext.AddMessage(context.GetPolicy<KnownResultCodes>().ValidationError, "EntityNotFound", new object[1]
                {
                   cartId
                }, $"Entity {cartId} was not found. Payment Not Saved");

                context.Abort(message, context);
                return null;
            }
                                   
           
            var amount = arg.Amount;
            PayLaterComponent payLaterComponent = new PayLaterComponent(new Money(context.CommerceContext.CurrentCurrency(), amount));

            //If cart has existing PayLaterComponent(s) which has the same amount, skip adding a new PayLaterComponent. It is used in case of retry.
            if (cart.HasComponent<PayLaterComponent>())
            {
                var payLaterComponents = cart.Components.OfType<PayLaterComponent>();

                var matchingPayLaterComponent = payLaterComponents.FirstOrDefault(x => x.Amount.Amount == amount);

                if (matchingPayLaterComponent == null)
                {
                    cart.Components.Add(payLaterComponent);
                }
            }
            else
            {
                cart.Components.Add(payLaterComponent);
            }            

            return cart;
        }
    }
}
