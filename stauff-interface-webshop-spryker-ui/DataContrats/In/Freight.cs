using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace stauff_interface_webshop_spryker_ui.DataContrats.In { 
    public sealed class Freight {
        //[StringLength(50)]
        [Description("Can be \"Ex Works\", \"Standard\" or \"Express\"; default \"Standard\"")]
        public string mode { get; set; } = "Standard";
    }
}
