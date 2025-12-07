using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace stauff_interface_webshop_spryker_ui.DataContrats.Common {
    public sealed class ExtendedPrice {
        [StringLength(3)]
        public string currencyCode { get; set; }
        //[Description("Price")]
        public decimal grossValue { get; set; }
        public decimal discount { get; set; }
        //[Description("Price")]
        public decimal netValue { get; set; }
    }
}
