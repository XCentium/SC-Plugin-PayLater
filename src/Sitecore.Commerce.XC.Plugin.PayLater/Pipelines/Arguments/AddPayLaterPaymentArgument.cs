using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;

namespace Sitecore.Commerce.XC.Plugin.PayLater.Arguments
{
    public class AddPayLaterPaymentArgument : PipelineArgument
    {
        public string CartId { get; set; }           
        public decimal Amount { get; set; }       
    }
}
