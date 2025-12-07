using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace stauff_interface_webshop_spryker_ui.DataContrats.Out {
    public sealed class ItemPriceList {
        [StringLength(11)]
        public string ItemCode { get; set; }
        public int PriceList { get; set; } = 2;
        [StringLength(3)]
        public string Currency { get; set; } = "EUR";
        public int Qty { get; set; } = 1;
        public decimal Price { get; set; }
        [StringLength(100)]
        public string Unit { get; set; } = "PCE";

        /*[Description("Error message")]
        public string error { get; set; }*/
    }
}