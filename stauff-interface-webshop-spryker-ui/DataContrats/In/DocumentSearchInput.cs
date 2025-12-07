using stauff_interface_webshop_spryker_ui.DataContrats.Out;
using System;
using System.Collections.Generic;
using System.Text;
using static stauff_interface_webshop_spryker_ui.Extensions.DocumentSearch;

namespace stauff_interface_webshop_spryker_ui.DataContrats.In {
    public class DocumentSearchInput {
        public string DebitorNumber { get; set; }
        public string Email { get; set; }
        public string DateFrom { get; set; }
        public string DateTill { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string DeliveryNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string MaterialNumber { get; set; }
        public DocStatus Status { get; set; }
        public DocType DocType { get; set; } = DocType.Unknown;
    }
}
