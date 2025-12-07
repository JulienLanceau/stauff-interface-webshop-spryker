using stauff_interface_webshop_spryker_ui.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace stauff_interface_webshop_spryker_ui.DataContrats.Out {
    public enum DocStatus {
        UNKNOWN,
        OPEN,
        PREPARATION,
        SHIPPED,
        CANCELLED,
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class DocumentSearchOutputWrapper {
        public DocumentSearchOutput[] items { get; set; }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class DocumentSearchOutput {
        public string DocumentNumber { get; set; }
        public string CustomerDocumentNumber { get; set; }
        public string CreationDate { get; set; }
        public string ShippingDate { get; set; }
        public DocStatus Status { get; set; }
        public DocumentSearch.DocType DocumentType { get; set; } = DocumentSearch.DocType.Order;

        public override string ToString() {
            return DocumentType + " " + DocumentNumber + " / " + CustomerDocumentNumber + " - " + CreationDate + " ; " + Status;
        }
    }
}
