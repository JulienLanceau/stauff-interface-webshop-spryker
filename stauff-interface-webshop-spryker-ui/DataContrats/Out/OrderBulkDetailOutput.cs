using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace stauff_interface_webshop_spryker_ui.DataContrats.Out {
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class OrderBulkDetailOutputWrapper {
        public OrderBulkDetailOutput[] items { get; set; }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class OrderBulkDetailOutput {
        public string DebitorNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string OrderNumber { get; set; }
        public int OrderPositionNumber { get; set; }
        public string CustomerMaterialNumber { get; set; }
        public string MaterialNumber { get; set; }
        public string MaterialDescription { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal Price { get; set; }
        public decimal LineTotal { get; set; }
        public string QuantityUnit { get; set; }
        public decimal OpenQuantity { get; set; }
        //public decimal PlannedQuantity { get; set; }
        public decimal ShippedQuantity { get; set; }
        public string ShippingDatePlanned { get; set; }
        public string ShippingDateReal { get; set; }
        //public string FreightForwarder { get; set; }
        public string ShippingCondition { get; set; }
        public string DeliveryNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string Currency { get; internal set; }
        //public string TrackingNumber { get; set; }
    }
}
