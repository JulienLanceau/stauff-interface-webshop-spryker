using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace stauff_interface_webshop_spryker_ui.DataContrats.Out {
    public sealed class ConfirmedQuantity {
        public decimal quantity { get; set; }
        [StringLength(10)]
        [Description("Can contain a valid date format YYYY-MM-DD or empty")]
        public string deliveryDate { get; set; }

        public override string ToString() {
            return quantity + " - " + deliveryDate;
        }
    }
}
