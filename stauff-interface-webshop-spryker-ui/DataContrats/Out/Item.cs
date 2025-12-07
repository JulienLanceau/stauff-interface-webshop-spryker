using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace stauff_interface_webshop_spryker_ui.DataContrats.Out {
    public sealed class Item {
        public Common.Price positionPrice { get; set; } = new Common.Price { };
        public List<Common.Tax> taxes { get; set; } = new List<Common.Tax> { };
        public Common.ExtendedPrice basePrice { get; set; } = new Common.ExtendedPrice { };
        public decimal baseQuantity { get; set; }
        [StringLength(20)]
        public string baseQuantityUnit { get; set; }
        public List<ConfirmedQuantity> confirmedQuantities { get; set; } = new List<ConfirmedQuantity> { };
        //[StringLength(int.MaxValue)]
        [Description("Error message")]
        public string error { get; set; }
    }
}
