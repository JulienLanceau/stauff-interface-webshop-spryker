using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace stauff_interface_webshop_spryker_ui.DataContrats.In {
    public sealed class Shipping {
        [StringLength(100)]
        [Description("Currently unused")]
        public string partnerNumber { get; set; }
        public Address address { get; set; } = new Address { };
    }
}
