using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace stauff_interface_webshop_spryker_ui.DataContrats.Common {
    public sealed class Price {
        //[Description("Price")]
        public decimal baseValue { get; set; }
        [StringLength(3)]
        public string currencyCode { get; set; }
    }
}
