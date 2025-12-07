using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace stauff_interface_webshop_spryker_ui.DataContrats.Out { 
    public sealed class Freight {
        //[StringLength(50)]
        [Description("Can be \"Ex works\", \"Standard\" or \"Express\"")]
        public string mode { get; set; }
        public Common.Price price { get; set; }
        public List<Common.Tax> taxes { get; set; }
    }
}
