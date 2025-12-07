using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace stauff_interface_webshop_spryker_ui.DataContrats.Out {
    public sealed class Customer {
        [StringLength(15)]
        public string DebitorNumber { get; set; }
        [StringLength(100)]
        public string DebitorName { get; set; }
        public int PriceList { get; set; } = 2;
        public List<CustomerContact> Contacts { get; set; } = new List<CustomerContact> { };
        public List<CustomerAddress> Addresses { get; set; } = new List<CustomerAddress> { };

        /*[Description("Error message")]
        public string error { get; set; }*/
    }
}