using stauff_interface_webshop_spryker_ui.DataContrats.Out;
using stauff_interface_webshop_spryker_ui.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace stauff_interface_webshop_spryker_ui.DataContrats.In {
    public class DocumentDetailOutput {
        public string DocumentNumber { get; set; }
        public string CustomerDocumentNumber { get; set; }
        public string DocumentDate { get; set; }
        public string ShippingDate { get; set; }
        public DocStatus Status { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingCondition { get; set; }
        //public string FreightForwarder { get; set; }
        //public List<string> TrackingCodes { get; set; } = new List<string>();
        //public string DeliveryNotes { get; set; }

        public List<DocumentDetailItems> DocumentPositions { get; set; } = new List<DocumentDetailItems>();
        public DocumentSearch.DocType DocumentType { get; set; } = DocumentSearch.DocType.Order;
    }
}
