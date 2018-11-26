namespace Sitecore.Commerce.XC.Plugin.PayLater
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.OData.Builder;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using Sitecore.Commerce.XC.Plugin.PayLater.Arguments;
    using Sitecore.Commerce.XC.Plugin.PayLater.Pipelines;

    [PipelineDisplayName("PayLaterPluginConfigureServiceApiBlock")]
    public class ConfigureServiceApiBlock : PipelineBlock<ODataConventionModelBuilder, ODataConventionModelBuilder, CommercePipelineExecutionContext>
    {
        
        public override Task<ODataConventionModelBuilder> Run(ODataConventionModelBuilder modelBuilder, CommercePipelineExecutionContext context)
        {
            Condition.Requires(modelBuilder).IsNotNull($"{this.Name}: The argument cannot be null.");

            ActionConfiguration actionConfigurationSaveForLater = modelBuilder.Action("AddPaymentDetailsForPayLater");
            actionConfigurationSaveForLater.Parameter<string>("CartId");
            actionConfigurationSaveForLater.Parameter<string>("Amount");            
            actionConfigurationSaveForLater.ReturnsFromEntitySet<CommerceCommand>("Commands");

            ActionConfiguration storePaymentDetailsForPayLaterOrder = modelBuilder.Action("AcceptPayLaterPaymentOnOrder");
            storePaymentDetailsForPayLaterOrder.Parameter<string>("OrderId");
            storePaymentDetailsForPayLaterOrder.Parameter<string>("PaymentInstrumentType");
            storePaymentDetailsForPayLaterOrder.Parameter<string>("MaskedNumber");
            storePaymentDetailsForPayLaterOrder.Parameter<string>("PaymentMethodNonce");
            storePaymentDetailsForPayLaterOrder.Parameter<string>("Amount");
            storePaymentDetailsForPayLaterOrder.Parameter<string>("TransactionId");
            storePaymentDetailsForPayLaterOrder.Parameter<string>("TransactionStatus");
            storePaymentDetailsForPayLaterOrder.Parameter<string>("Expdate");
            storePaymentDetailsForPayLaterOrder.Parameter<string>("Cardtype");            
            storePaymentDetailsForPayLaterOrder.ReturnsFromEntitySet<CommerceCommand>("Commands");

            return Task.FromResult(modelBuilder);
        }
    }
}
