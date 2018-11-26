
namespace Sitecore.Commerce.XC.Plugin.PayLater.Commands
{
    using System;
    using Sitecore.Commerce.Core.Commands;
    using PayLater.Pipelines;
    using PayLater.Arguments;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.Core;        
    using System.Threading.Tasks;

    public class AcceptPayLaterPaymentOnOrderCommand : CommerceCommand
    {
        private readonly IAcceptPaymentForPayLaterOrderPipeline _acceptPaymentForPayLaterOrderPipeline;
        private readonly IReleaseOnHoldOrderPipeline _releaseOnHoldOrderPipeline;
        private readonly ISetOrderStatusPipeline _setOrderStatusPipeline;
        private readonly IGetOrderPipeline _getOrderPripeline;

        public AcceptPayLaterPaymentOnOrderCommand(IAcceptPaymentForPayLaterOrderPipeline acceptPaymentForPayLaterOrderPipeline, IServiceProvider serviceProvider,
            IReleaseOnHoldOrderPipeline releaseOnHoldOrderPipeline,
            ISetOrderStatusPipeline setOrderStatusPipeline,
            IGetOrderPipeline getOrderPipeline)
        {

            _releaseOnHoldOrderPipeline = releaseOnHoldOrderPipeline;
            _setOrderStatusPipeline = setOrderStatusPipeline;
            _getOrderPripeline = getOrderPipeline;
            _acceptPaymentForPayLaterOrderPipeline = acceptPaymentForPayLaterOrderPipeline;

        }

        public virtual async Task<bool> Process(CommerceContext commerceContext, AcceptPaymentForPayLaterOrderArgument arg)
        {
            var knownOrderStatus = string.Empty;
            Cart tempCart;
            Order resultOrder = null;
            var orderStatusChanged = false;

            //Get current order status
            using (var activity = CommandActivity.Start(commerceContext, this))
            {
                var orderId = CommerceEntity.IdPrefix<Order>() + arg.OrderId;
                var order = await _getOrderPripeline.Run(orderId, new CommercePipelineExecutionContextOptions(commerceContext));

                if (order != null)
                {
                    knownOrderStatus = order.Status;
                }
            }


            //set order status to pending/problem which will allow us to make changes to an order
            using (var activity = CommandActivity.Start(commerceContext, this))
            {
                var entityOrderId = CommerceEntity.IdPrefix<Order>() + arg.OrderId;
                orderStatusChanged = await _setOrderStatusPipeline.Run(new SetOrderStatusArgument(entityOrderId, "Pending"), new CommercePipelineExecutionContextOptions(commerceContext));
            }

            if (orderStatusChanged)
            {
                using (CommandActivity.Start(commerceContext, this))
                {
                    CommercePipelineExecutionContextOptions pipelineContextOptions = commerceContext.GetPipelineContextOptions();
                    tempCart = await _acceptPaymentForPayLaterOrderPipeline.Run(arg, pipelineContextOptions);
                }

                if (tempCart != null)
                {
                    //remove hold on order
                    using (var activity = CommandActivity.Start(commerceContext, this))
                    {
                        var pipelineContextOptions = commerceContext.GetPipelineContextOptions();
                        resultOrder = await _releaseOnHoldOrderPipeline.Run($"{CommerceEntity.IdPrefix<Order>()}{arg.OrderId}", pipelineContextOptions);
                    }
                }
            }

            return resultOrder != null;
        }
    }
}
