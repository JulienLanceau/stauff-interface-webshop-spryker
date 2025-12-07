using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace stauff_interface_webshop_spryker_ui.DataContrats.Out {
    public sealed class ItemsPriceList {
        public List<ItemPriceList> Items { get; set; } = new List<ItemPriceList>();

        /*[Description("Error message")]
        public string error { get; set; }*/
    }
}
