namespace Sitecore.Commerce.XC.Plugin.PayLater
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Payments;
    using System;

    public class PayLaterComponent : PaymentComponent
    {
        public DateTime DateCreated { get; set; }

        public PayLaterComponent(Money money) : base(money)
        {
            DateCreated = DateTime.UtcNow;
        }
    }
}