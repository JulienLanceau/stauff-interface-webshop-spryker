using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace stauff_interface_webshop_spryker_ui.DataContrats.Out {
    public sealed class Customers {
        public List<Customer> Debitors { get; set; } = new List<Customer>();
        /*[Description("Error message")]
        public string error { get; set; }*/
    }
}
