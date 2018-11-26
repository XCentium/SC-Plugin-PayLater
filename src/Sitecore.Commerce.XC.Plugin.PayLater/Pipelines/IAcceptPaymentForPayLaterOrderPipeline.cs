using Sitecore.Commerce.XC.Plugin.PayLater.Arguments;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Pipelines;

namespace Sitecore.Commerce.XC.Plugin.PayLater.Pipelines
{
    [PipelineDisplayName("PayFlowPro.pipeline.StorePaymentDetailsForPayLaterOrderPipeline")]
    public interface IAcceptPaymentForPayLaterOrderPipeline : IPipeline<AcceptPaymentForPayLaterOrderArgument, Cart, CommercePipelineExecutionContext>
    {

    }
}
