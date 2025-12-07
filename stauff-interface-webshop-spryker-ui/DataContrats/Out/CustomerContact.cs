using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace stauff_interface_webshop_spryker_ui.DataContrats.Out {
    public sealed class CustomerContact {
        [StringLength(11)]
        public string ContactID { get; set; }
        public string FirstName { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }
        [StringLength(100)]
        public string EMailAddress { get; set; }
        [StringLength(3)]
        [Description("Contains Mr or Mme")]
        public string Salutation { get; set; }
        [StringLength(5)]
        [Description("Contains male or female")]
        public string Gender { get; set; }
        [StringLength(2)]
        [Description("Contains FR or EN")]
        public string Language { get; set; }

        public int PermissionLevel { get; set; }

        /*[Description("Error message")]
        public string error { get; set; }*/
    }
}