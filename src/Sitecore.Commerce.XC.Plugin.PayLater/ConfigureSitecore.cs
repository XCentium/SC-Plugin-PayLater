
namespace Sitecore.Commerce.XC.Plugin.PayLater
{
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;    
    using Sitecore.Commerce.XC.Plugin.PayLater.Pipelines.Blocks;
    using Sitecore.Commerce.XC.Plugin.PayLater.Pipelines;
    using Sitecore.Commerce.Plugin.Carts;

    public class ConfigureSitecore : IConfigureSitecore
    {      
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config

             .AddPipeline<IAcceptPaymentForPayLaterOrderPipeline, AcceptPaymentForPayLaterOrderPipeline>(
                configure => {
                    configure.Add<AcceptPaymentForPayLaterOrderBlock>()
                    .Add<ICalculateCartLinesPipeline>()
                    .Add<ICalculateCartPipeline>()
                    .Add<PersistCartBlock>();
                })

            .AddPipeline<IAddPayLaterPaymentDetailsPipeline, AddPayLaterPaymentDetailsPipeline>(
                configure =>
                {
                    configure.Add<AddPayLaterPaymentDetailsBlock>()
                    .Add<ICalculateCartLinesPipeline>()
                    .Add<ICalculateCartPipeline>()
                    .Add<PersistCartBlock>();
                })

               .ConfigurePipeline<IConfigureServiceApiPipeline>(configure => configure.Add<ConfigureServiceApiBlock>()));

            services.RegisterAllCommands(assembly);
        }
    }
}