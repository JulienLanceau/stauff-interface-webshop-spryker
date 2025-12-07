using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace stauff_interface_webshop_spryker_ui.DataContrats.Out {
    public sealed class OrderReturn {
        [StringLength(11)]
        public string orderNumber { get; set; }
        //[StringLength(int.MaxValue)]
        [Description("Error message")]
        public string error { get; set; }
    }
}
