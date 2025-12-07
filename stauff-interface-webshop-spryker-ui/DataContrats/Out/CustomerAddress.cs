using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace stauff_interface_webshop_spryker_ui.DataContrats.Out {
    public sealed class CustomerAddress {
        [StringLength(62)]
        public string AddressID { get; set; }
        [StringLength(50)]
        public string Name1 { get; set; }
        [StringLength(100)]
        public string Name2 { get; set; }
        public string Name3 { get; set; }
        public string Name4 { get; set; }
        [StringLength(100)]
        public string Street { get; set; }
        [StringLength(20)]
        public string ZipCode { get; set; }
        [StringLength(100)]
        public string City { get; set; }
        [StringLength(3)]
        public string CountryCode { get; set; }
        [Description("Is bill_to address")]
        public bool IsBilAdr { get; set; }
        [Description("Is default ship_to Address")]
        public bool IsDefShip { get; set; }

        /*[Description("Error message")]
        public string error { get; set; }*/
    }
}