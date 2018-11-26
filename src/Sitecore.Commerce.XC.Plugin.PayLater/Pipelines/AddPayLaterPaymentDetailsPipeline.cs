using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;
using Sitecore.Commerce.XC.Plugin.PayLater.Arguments;
using Sitecore.Commerce.Plugin.Carts;

namespace Sitecore.Commerce.XC.Plugin.PayLater.Pipelines
{
    public class AddPayLaterPaymentDetailsPipeline : CommercePipeline<AddPayLaterPaymentArgument, Cart>, IAddPayLaterPaymentDetailsPipeline
    {
        public AddPayLaterPaymentDetailsPipeline(IPipelineConfiguration<IAddPayLaterPaymentDetailsPipeline> configuration, ILoggerFactory loggerFactory)
           : base(configuration, loggerFactory)
        {
        }
    }
}
