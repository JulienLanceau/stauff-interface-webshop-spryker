using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace stauff_interface_webshop_spryker_ui.DataContrats.In {
    public sealed class Address {
        [StringLength(100)]
        [Description("Currently unused")]
        public string name { get; set; }
        //unused
        public string name2 { get; set; }
        //unused
        public string name3 { get; set; }
        //unused
        public string name4 { get; set; }
        [StringLength(100)]
        public string street { get; set; }
        [StringLength(100)]
        public string houseNumber { get; set; }
        [StringLength(20)]
        public string postalCode { get; set; }

        [StringLength(100)]
        public string city { get; set; }

        [StringLength(3)]
        [Description("2-char country codes")]
        public string countryCode { get; set; }
        [StringLength(50)]
        public string emailAddress { get; set; }
        [StringLength(20)]
        public string telephoneNumber { get; set; }
    }
}
