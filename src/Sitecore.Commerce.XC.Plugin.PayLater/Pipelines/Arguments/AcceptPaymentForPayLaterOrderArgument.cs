using Sitecore.Commerce.Core;

namespace Sitecore.Commerce.XC.Plugin.PayLater.Arguments
{
    public class AcceptPaymentForPayLaterOrderArgument : PipelineArgument
    {
        public string OrderId { get; set; }
        public string TransactionId { get; set; }
        public string PaymentMethodNonce { get; set; }
        public string MaskedNumber { get; set; }
        public decimal Amount { get; set; }
        public string TransactionStatus { get; set; }
        public string CardType { get; set; }
        public string PaymentInstrumentType { get; set; }       
        public string Expdate { get; set; }        
        public string Cardtype { get; set; }
    }
}
