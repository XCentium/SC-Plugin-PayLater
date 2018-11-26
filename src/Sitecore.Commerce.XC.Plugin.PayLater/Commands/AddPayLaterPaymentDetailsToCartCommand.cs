namespace Sitecore.Commerce.XC.Plugin.PayLater
{
    using System;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using PayLater.Pipelines;
    using PayLater.Arguments;
    using Sitecore.Commerce.Plugin.Carts;

    public class AddPayLaterPaymentDetailsToCartCommand : CommerceCommand
    {
        private readonly IAddPayLaterPaymentDetailsPipeline _addPayLaterDetailsPipeline;

        public AddPayLaterPaymentDetailsToCartCommand(IAddPayLaterPaymentDetailsPipeline storePaymentPipeline, IServiceProvider serviceProvider)
        : base(serviceProvider)
        {
            _addPayLaterDetailsPipeline = storePaymentPipeline;
        }

        public virtual async Task<Cart> Process(CommerceContext commerceContext, AddPayLaterPaymentArgument arg)
        {
            Cart result;
            using (CommandActivity.Start(commerceContext, this))
            {
                CommercePipelineExecutionContextOptions pipelineContextOptions = commerceContext.GetPipelineContextOptions();
                result = await _addPayLaterDetailsPipeline.Run(arg, pipelineContextOptions);
            }
            return result;
        }

    }
}