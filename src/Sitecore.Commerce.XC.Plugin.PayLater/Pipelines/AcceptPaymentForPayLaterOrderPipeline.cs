using Sitecore.Commerce.XC.Plugin.PayLater.Arguments;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Pipelines;

namespace Sitecore.Commerce.XC.Plugin.PayLater.Pipelines
{
    public class AcceptPaymentForPayLaterOrderPipeline : CommercePipeline<AcceptPaymentForPayLaterOrderArgument, Cart>, IAcceptPaymentForPayLaterOrderPipeline
    {
        public AcceptPaymentForPayLaterOrderPipeline(IPipelineConfiguration<IAcceptPaymentForPayLaterOrderPipeline> configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {

        }
    }
}
