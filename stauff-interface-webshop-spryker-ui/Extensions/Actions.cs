using SAPbobsCOM;
using stauff_interface_webshop_spryker_ui.Configuration;
using stauff_interface_webshop_spryker_ui.DataContrats.Out;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace stauff_interface_webshop_spryker_ui.Extensions {
    public sealed class Actions {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        #region PriceList
        public static ItemsPriceList PriceLists(Company DIAPI, MainConfiguration config, int pricelist_id) {
            var priceLists = new ItemsPriceList();
            Recordset rcOitm = null;
            try {
                rcOitm = DIAPI.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;
                rcOitm.Query(@$"
select OITM.U_CodesSTD, OITM.SalUnitMsr, ITM1.Price
from OITM
inner join ITM1 on ITM1.ItemCode = OITM.ItemCode and ITM1.PriceList = {pricelist_id}
where Isnull(U_CodesSTD,'') != ''
and QryGroup64 = 'N'
and validFor = 'Y'
and (InvntItem = 'Y' or TreeType = 'S' or ItmsGrpCod = 186)
and ITM1.Price <> 0
");
                rcOitm.MoveFirst();
                for(int i = 0, n = rcOitm.RecordCount; i < n; i++) {
                    var pricelist = new ItemPriceList {
                        ItemCode = rcOitm.Fields.Item("U_CodesSTD").Value.ToString(),
                        Price = Math.Round(Convert.ToDecimal(rcOitm.Fields.Item("Price").Value.ToString()), 2),
                        PriceList = pricelist_id,
                    };
                    //pricelist.Unit = rcOitm.Fields.Item("SalUnitMsr").Value.ToString();

                    priceLists.Items.Add(pricelist);
                    rcOitm.MoveNext();
                }
            } catch(Exception ex) {
                Logger.Error(ex.ToString() + ex.InnerException);
                return null;
            } finally {
                rcOitm.ReleaseComObject();
            }

            return priceLists;
        }

        #endregion

        #region Customer
        private static List<CustomerContact> getContacts(Recordset rc, string CardCode, string LangCode) {
            var contacts = new List<CustomerContact>();

            rc.Query(@"
SELECT CntctCode, Name, E_MailL, U_Titre, U_Contact
from OCPR
Where CardCode = '{{CardCode}}'
    and Active = 'Y'
    and (U_Contact = 'W' or U_Contact = 'W1')
".Replace("{{CardCode}}", CardCode.Replace("'", "''")));
            rc.MoveFirst();
            for(int i = 0, n = rc.RecordCount; i < n; i++) {
                var titre = rc.Fields.Item("U_Titre").Value.ToString();
                var permlevel = rc.Fields.Item("U_Contact").Value.ToString();
                var contact = new CustomerContact {
                    ContactID = rc.Fields.Item("CntctCode").Value.ToString(),
                    LastName = rc.Fields.Item("Name").Value.ToString(),
                    EMailAddress = rc.Fields.Item("E_MailL").Value.ToString(),
                    Language = LangCode switch {
                        "22" => "FR",
                        _ => "EN",
                    },
                    Salutation = titre switch {
                        "0" => "Mr",
                        _ => "Mme"
                    },
                    Gender = titre switch {
                        "0" => "male",
                        _ => "female"
                    },
                    PermissionLevel = permlevel switch {
                        "W" => 2,
                        "W1" => 1,
                        _ => 0
                    }
                };
                contacts.Add(contact);
                rc.MoveNext();
            }

            return contacts;
        }
        private static List<CustomerAddress> getAddresses(Recordset rc, string CardCode, string BilltoDef, string ShipToDef) {
            var addresses = new List<CustomerAddress>();

            rc.Query(@"
SELECT AdresType, Address, LineNum, County, Street, ZipCode, City, Country, AdresType
from CRD1
Where CardCode = '{{CardCode}}'
    and isnull(City,'') != '' 
    and isnull(ZipCode, '') != '' 
    and Address != '.'
    and (AdresType = 'S' or (AdresType = 'B' and Address = '{{BilltoDef}}'))
    and U_VisibleWeb = 'Y'
".Replace("{{CardCode}}", CardCode.Replace("'", "''"))
.Replace("{{BilltoDef}}", BilltoDef.Replace("'", "''")));
            rc.MoveFirst();
            for(int i = 0, n = rc.RecordCount; i < n; i++) {
                var address = new CustomerAddress();
                var AddressRq = rc.Fields.Item("Address").Value.ToString();
                address.AddressID = rc.Fields.Item("AdresType").Value.ToString() + rc.Fields.Item("LineNum").Value.ToString();
                address.Name1 = AddressRq;
                address.Name2 = rc.Fields.Item("County").Value.ToString();
                address.Street = rc.Fields.Item("Street").Value.ToString();
                address.ZipCode = rc.Fields.Item("ZipCode").Value.ToString();
                address.City = rc.Fields.Item("City").Value.ToString();
                address.CountryCode = rc.Fields.Item("Country").Value.ToString();
                {
                    var AdresType = rc.Fields.Item("AdresType").Value.ToString();
                    address.IsBilAdr = AdresType == "B";
                    address.IsDefShip = AdresType == "S" && AddressRq == ShipToDef;
                }

                addresses.Add(address);
                rc.MoveNext();
            }

            return addresses;
        }
        public static Customers Customers(Company DIAPI, MainConfiguration config) {
            var customers = new Customers();

            Recordset rcOcrd = null;
            Recordset rcOcprOrCrd1 = null;

            try {
                rcOcrd = DIAPI.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;
                rcOcprOrCrd1 = DIAPI.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;

                rcOcrd.Query(@"
SELECT CardCode, CardName, LangCode, BilltoDef, ShipToDef, ListNum
FROM OCRD
where ValidFor = 'Y' 
    and FrozenFor = 'N'
    and CardType = 'C'
    and QryGroup63 = 'Y'
");
                rcOcrd.MoveFirst();
                for(int i = 0, n = rcOcrd.RecordCount; i < n; i++) {
                    var customer = new Customer();

                    var cardcode = rcOcrd.Fields.Item("CardCode").Value.ToString();
                    var LangCode = rcOcrd.Fields.Item("LangCode").Value.ToString();
                    var BilltoDef = rcOcrd.Fields.Item("BilltoDef").Value.ToString();
                    var ShipToDef = rcOcrd.Fields.Item("ShipToDef").Value.ToString();
                    customer.DebitorNumber = cardcode;
                    customer.DebitorName = rcOcrd.Fields.Item("CardName").Value.ToString();
                    customer.PriceList = Convert.ToInt32(rcOcrd.Fields.Item("ListNum").Value);

                    customer.Contacts = getContacts(rcOcprOrCrd1, cardcode, LangCode);
                    customer.Addresses = getAddresses(rcOcprOrCrd1, cardcode, BilltoDef, ShipToDef);

                    customers.Debitors.Add(customer);
                    rcOcrd.MoveNext();
                }
            } catch(Exception ex) {
                Logger.Error(ex.ToString());
                return null;
            } finally {
                rcOcrd.ReleaseComObject();
                rcOcprOrCrd1.ReleaseComObject();
            }

            return customers;
        }
        #endregion
    }
}
