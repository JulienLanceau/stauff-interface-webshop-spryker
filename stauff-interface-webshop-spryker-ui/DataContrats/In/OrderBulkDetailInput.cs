using System;
using System.Collections.Generic;
using System.Text;

namespace stauff_interface_webshop_spryker_ui.DataContrats.In {
    public class OrderBulkDetailInput {
        public string DebitorNumber { get; set; }
        public string Email { get; set; }
        public string[] OrderNumbers { get; set; }
    }
}
