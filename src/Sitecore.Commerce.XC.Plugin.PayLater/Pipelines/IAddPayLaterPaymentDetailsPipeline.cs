using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;
using Sitecore.Commerce.XC.Plugin.PayLater.Arguments;
using Sitecore.Commerce.Plugin.Carts;

namespace Sitecore.Commerce.XC.Plugin.PayLater.Pipelines
{
    [PipelineDisplayName("PayFlowPro.pipeline.StorePaymentDetailsPipeline")]
    public interface IAddPayLaterPaymentDetailsPipeline : IPipeline<AddPayLaterPaymentArgument, Cart, CommercePipelineExecutionContext>
    {
    }
}
