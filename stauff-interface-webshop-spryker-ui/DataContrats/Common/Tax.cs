using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace stauff_interface_webshop_spryker_ui.DataContrats.Common {
    public sealed class Tax {
        [StringLength(3)]
        public string name { get; set; }
        public decimal rate { get; set; }
    }
}
