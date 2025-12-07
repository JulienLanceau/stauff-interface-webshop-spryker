using stauff_interface_webshop_spryker_ui.DataContrats.Out;
using System.ComponentModel.DataAnnotations;

namespace stauff_interface_webshop_spryker_ui.DataContrats.In {
    public class DocumentDetailItems {
        public int PositionNumber { get; set; }
        public string CustomerMaterialNumber { get; set; }
        public string MaterialNumber { get; set; }
        public string MaterialDescription { get; set; }
        public decimal Amount { get; set; }
        [StringLength(100)]
        public string Unit { get; set; } = "PCE";
        public DocStatus DeliveryStatus { get; set; }
        public string ShippingDate { get; set; }
        public string DeliveryNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public int OrderPosition { get; set; }
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string CustomerOrderNumber { get; set; }
        public decimal OrderedAmount { get; set; }
    }
}