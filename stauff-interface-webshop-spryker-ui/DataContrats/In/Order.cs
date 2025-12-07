using SAPbobsCOM;
using stauff_interface_webshop_spryker_ui.Configuration;
using stauff_interface_webshop_spryker_ui.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace stauff_interface_webshop_spryker_ui.DataContrats.In {
    public sealed class Order {
        [StringLength(15)]
        public string debitorNumber { get; set; }

        [StringLength(20)]
        [Description("Not used in simulation")]
        public string vatNumber { get; set; }
        [StringLength(100)]
        [Description("Not used in simulation")]
        public string customerOrderNumber { get; set; }

        [Description("Currently only checks if empty or not, Not used in simulation")]
        public string paymentReference { get; set; }
        [StringLength(50)]
        [Description("Not used in simulation")]
        public string sprykerOrderNumber { get; set; }
        [StringLength(254)]
        [Description("Not used in simulation")]
        public string orderComments { get; set; }
        [Description("Not used in simulation")]
        public Address customerAddress { get; set; } = new Address();
        [Description("Not used in simulation")]
        public Shipping shipping { get; set; } = new Shipping();
        [Description("Not used in simulation")]
        public bool completeDelivery { get; set; }
        public Freight freight { get; set; } = new Freight();

        public List<Item> items { get; set; } = new List<Item>();

        //methods
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public int ContactCode(SAPbobsCOM.Company CompDI, ref MainConfiguration configuration, ref Dictionary<string, object> cache) {
            var r = CompDI.QueryFirstValue(@"
select ""CntctCode""
FROM OCPR
where ""CardCode"" = '{{CardCode}}' and ""E_MailL"" = '{{EMail}}'
"
                            .Replace("{{EMail}}", this.customerAddress.emailAddress)
                            .Replace("{{CardCode}}", this.CardCode(CompDI, ref configuration, ref cache)))
                ?.ToString();
            if(string.IsNullOrWhiteSpace(r))
                r = null;
            return Convert.ToInt32(r ?? "-1");
        }
        public string CardCode(ICompany CompDI, ref MainConfiguration configuration, ref Dictionary<string, object> cache) {
            var key = "CardCode:" + this.debitorNumber;
            if(!cache.ContainsKey(key)) {
                var cardCode = this.debitorNumber;
                if(string.IsNullOrWhiteSpace(this.debitorNumber)) {
                    cardCode = configuration?.CodeClientDummy;
                    Logger.Debug("Client utilise le code Dummy");
                }
                var r = CompDI.QueryFirstValue(@"SELECT ""CardCode"" from OCRD where ""CardCode"" = '{{CardCode}}'".Replace("{{CardCode}}", cardCode))?.ToString();
                if(!string.IsNullOrWhiteSpace(r))
                    cache.Add(key, r);
                else
                    return configuration?.CodeClientDummy;
            }
            return (string)cache[key];
        }
        public DataContrats.Common.ExtendedPrice Price(
            Item item,
            ref MainConfiguration configuration,
            SAPbobsCOM.Company CompDI,
            ref Dictionary<string, object> cache,
            bool isDummy) {
            if(item == null) throw new Exception("No item specified");
            if(string.IsNullOrWhiteSpace(item.materialNumber)) throw new Exception("No item specified");

            var key = "Price:" + item.materialNumber;
            if(!cache.ContainsKey(key)) {
                Recordset rc = null;
                var r = new DataContrats.Common.ExtendedPrice();
                try {
                    rc = (Recordset)CompDI.GetBusinessObject(BoObjectTypes.BoRecordset);
                    rc.Query(@"
select top 1  ISNULL(SPP2.""Price"", ISNULL(SPP1.""Price"", OSPP.""Price"")) as Price, ISNULL(SPP2.""Currency"", ISNULL(SPP1.""Currency"", OSPP.""Currency"")) as ""Currency""
from OSPP 
LEFT JOIN SPP1 on SPP1.""ItemCode"" = OSPP.""ItemCode"" and SPP1.""CardCode"" = OSPP.""CardCode""
	and SPP1.""FromDate"" <= CAST(CAST(GETDATE() as DATE) as DATETIME) and SPP1.""ToDate"" >= CAST(CAST(GETDATE() as DATE) as DATETIME)
LEFT JOIN SPP2 on SPP2.""ItemCode"" = SPP1.""ItemCode"" and SPP2.""CardCode"" = SPP1.""CardCode"" and SPP2.""SPP1LNum"" = SPP1.LINENUM
where OSPP.""CardCode"" = '{{CardCode}}' and OSPP.""ItemCode"" = '{{ItemCode}}' and (SPP2.""Amount"" is null or SPP2.""Amount"" > {{Quantity}})
order by SPP2.""Amount"" desc, SPP1.""FromDate"" desc"
                        .Replace("{{ItemCode}}", item.ItemCode(CompDI, ref cache))
                        .Replace("{{CardCode}}", this.CardCode(CompDI, ref configuration, ref cache))
                        .Replace("{{Quantity}}", item.baseQuantity.ToString(CultureInfo.InvariantCulture)));
                    if(rc.RecordCount > 0) {
                        var grossValue = Convert.ToDecimal(rc.Fields.Item("Price").Value);
                        var currencyCode = rc.Fields.Item("Currency").Value.ToString();
                        if(!string.IsNullOrWhiteSpace(currencyCode) && grossValue > 0) {
                            r.grossValue = grossValue;
                            r.currencyCode = currencyCode;
                            Logger.Info((new { table = "OSPP", r.currencyCode, r.grossValue, r.netValue, r.discount }).ToString());
                        }
                    } 

                    if(string.IsNullOrWhiteSpace(r.currencyCode) || r.grossValue == 0) {
                        rc.Query(@"
select ITM1.""Price"", ITM1.""Currency"", OITM.""ItmsGrpCod""
from ITM1
Inner JOIN OITM on OITM.""ItemCode"" = ITM1.""ItemCode""
where ITM1.""ItemCode"" = '{{ItemCode}}' and ITM1.""PriceList"" = (select TOP 1 OCRD.""ListNum"" from OCRD where ""CardCode"" = '{{CardCode}}')
"
            .Replace("{{ItemCode}}", item.ItemCode(CompDI, ref cache))
            .Replace("{{CardCode}}", this.CardCode(CompDI, ref configuration, ref cache)));
                        if(rc.RecordCount > 0) {
                            r.grossValue = Convert.ToDecimal(rc.Fields.Item("Price").Value);
                            r.currencyCode = rc.Fields.Item("Currency").Value.ToString();
                            Logger.Info((new { table = "ITM1", r.currencyCode, r.grossValue, r.netValue, r.discount }).ToString());
                            var group = rc.Fields.Item("ItmsGrpCod").Value.ToString();

                            rc.Query(@"
select ""remise""
from ""STAUFF_webshop_remises""
where ""CardCode"" = '{{CardCode}}' and ""groupeArticle"" = '{{ItmsGrpCod}}'
"
            .Replace("{{ItmsGrpCod}}", group)
            .Replace("{{CardCode}}", this.CardCode(CompDI, ref configuration, ref cache)));

                            if(rc.RecordCount > 0) {
                                r.discount += Convert.ToDecimal(rc.Fields.Item("remise").Value);
                                Logger.Info((new { table = "STAUFF_webshop_remises", r.currencyCode, r.grossValue, r.netValue, r.discount }).ToString());
                            }
                        }
                    }

                    rc.Query(@"
SELECT TOP 1 QryGroup63
FROM OITM
where OITM.""ItemCode"" = '{{ItemCode}}'
".Replace("{{ItemCode}}", item.ItemCode(CompDI, ref cache)));

                    if(rc.RecordCount > 0 && rc.Fields.Item("QryGroup63").Value.ToString() == "N") {
                        rc.Query(@"
select TOP 1 ""Discount""
from OCRD
where ""CardCode"" = '{{CardCode}}'
"
.Replace("{{CardCode}}", this.CardCode(CompDI, ref configuration, ref cache)));
                        if(rc.RecordCount > 0) {
                            r.grossValue *= (1m - (Convert.ToDecimal(rc.Fields.Item("Discount").Value) / 100m));
                            Logger.Info((new { table = "OCRD Discount", r.currencyCode, r.grossValue, r.netValue, r.discount }).ToString());
                        }
                    }

                    if(isDummy) {
                        rc.Query(@"
select TOP 1 ""U_WEBREMISE""
from OCRD
where ""CardCode"" = '{{CardCode}}'
and ""U_WEBREMISE"" is not null
".Replace("{{CardCode}}", this.CardCode(CompDI, ref configuration, ref cache)));
                        if(rc.RecordCount > 0) {
                            r.discount += Convert.ToDecimal(rc.Fields.Item("U_WEBREMISE").Value);
                            Logger.Info((new { table = "OCRD U_WEBREMISE", r.currencyCode, r.grossValue, r.netValue, r.discount }).ToString());
                        }

                        rc.Query(@"
select TOP 1 OITB.""U_WEBREMISE""
from OITB
INNER JOIN OITM ON OITM.""ItmsGrpCod"" = OITB.""ItmsGrpCod""
where 1=1
and ""OITM"".""ItemCode"" = '{{ItemCode}}'
AND (OITB.""U_WEBREMISEFROM"" is null or OITB.""U_WEBREMISEFROM"" <= CAST(CAST(GETDATE() as DATE) as DATETIME)) 
and (OITB.""U_WEBREMISETO"" is null or OITB.""U_WEBREMISETO"" >= CAST(CAST(GETDATE() as DATE) as DATETIME))
and OITB.""U_WEBREMISE"" is not null
".Replace("{{ItemCode}}", item.ItemCode(CompDI, ref cache)));
                        if(rc.RecordCount > 0) {
                            r.discount += Convert.ToDecimal(rc.Fields.Item("U_WEBREMISE").Value);
                            Logger.Info((new { table = "OITB U_WEBREMISE", r.currencyCode, r.grossValue, r.netValue, r.discount }).ToString());
                        }
                    }

                    r.netValue = r.grossValue * (1m - (r.discount / 100m));
                    Logger.Info((new { table = "netvalue calc", r.currencyCode, r.grossValue, r.netValue, r.discount }).ToString());
                } catch(Exception ex) {
                    Logger.Warn(ex.ToString());
                    throw;
                } finally {
                    rc.ReleaseComObject();
                }
                cache.Add(key, r);
            }
            return (DataContrats.Common.ExtendedPrice)cache[key];
        }

        public DataContrats.Out.Freight Freight(Company CompDI, ref MainConfiguration configuration, ref Dictionary<string, object> cache, bool isDummy) {
            this.freight ??= new Freight();
            if(string.IsNullOrWhiteSpace(this.freight.mode)) {
                this.freight.mode = "Standard";
            }

            if(isDummy && (this.freight.mode.ToLower() == "Standard".ToLower() || this.freight.mode.ToLower() == "Express".ToLower())) {
                return new DataContrats.Out.Freight {
                    mode = this.freight.mode,
                    price = new DataContrats.Common.Price {
                        baseValue = this.freight.mode.ToLower() == "Standard".ToLower() ? 10 : 25,
                        currencyCode = "EUR",
                    },
                    taxes = new List<DataContrats.Common.Tax> {
                        new DataContrats.Common.Tax {
                            name = "VAT",
                            rate = this.Rate(null, CompDI, ref configuration, ref cache, configuration.ExpenseItemCode),
                        }
                    },
                };
            }

            var Franco = 0m;
            {
                var r = CompDI.QueryFirstValue(
                    @"SELECT T0.""U_FRANCO"" from OCRD T0 where '{{CardCode}}' = T0.""CardCode"""
                    .Replace("{{CardCode}}", this.CardCode(CompDI, ref configuration, ref cache))
                    )?.ToString();
                if(string.IsNullOrWhiteSpace(r))
                    r = null;
                Convert.ToDecimal(r ?? "0");
            }

            var total_price = 0m;
            foreach(var item in this.items) {
                total_price += Convert.ToDecimal(this.Price(item, ref configuration, CompDI, ref cache, isDummy).grossValue);
            }

            // si doctotal > franco, pas de frais de transport
            if(this.freight.mode.ToLower().Trim() != "ex works" && this.freight.mode.ToLower().Trim() != "ns" && Franco < total_price) {
                Recordset rc = null;
                try {
                    var weight = 0m;
                    foreach(var in_item in this.items) {
                        var itemcode = in_item.ItemCode(CompDI, ref cache);
                        var poidsArticle = Math.Ceiling(Convert.ToDecimal(CompDI.QueryFirstValue(@"
select ""SWeight1""
from OITM
where ""ItemCode"" = '{{ItemCode}}'
"
        .Replace("{{ItemCode}}", itemcode)), CultureInfo.InvariantCulture) * in_item.baseQuantity / 1000.0m);
                        Logger.Info("Poids Article " + itemcode + " pour quantité " + in_item.baseQuantity + " : " + poidsArticle);
                        weight += poidsArticle;
                    }
                    Logger.Info("Poids total commande: " + weight);
                    rc = CompDI.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;
                    var tot = 0m;
                    var old_weight = -1m;
                    while(weight > 0) {
                        if(old_weight == weight)
                            throw new Exception("Error in freight data @TARIF_TRANSPORT");
                        old_weight = weight;

                        var type = "";
                        switch(this.freight.mode.ToLower().Trim()) {
                            case "standard": {
                                    type = "Petit colis";
                                }
                                break;
                            case "express": {
                                    type = "Express";
                                }
                                break;
                            default:
                                throw new Exception("Invalid freight");
                        }

                        rc.Query(@"
select TOP 1 U_POIDS2, U_PRIX
from ""@TARIF_TRANSPORT""
where {{POIDS}} >= U_POIDS1 and U_TYPE_FRAIS = '{{TYPE_FRAIS}}'
order by U_POIDS2 DESC"
        .Replace("{{POIDS}}", weight.ToString(CultureInfo.InvariantCulture))
        .Replace("{{TYPE_FRAIS}}", type)
        );
                        rc.MoveFirst();

                        if(Convert.ToDecimal(rc.Fields.Item("U_POIDS2").Value) <= 0)
                            throw new Exception("Error in freight data @TARIF_TRANSPORT");

                        var prix = Convert.ToDecimal(rc.Fields.Item("U_PRIX").Value);
                        var poids2 = Convert.ToDecimal(rc.Fields.Item("U_POIDS2").Value);

                        #region Raccourci si le poids est plusieurs multiples plus grand que ce qu'il y a dans la table
                        var times = Math.Round(weight / poids2);
                        if(times > 1) {
                            poids2 *= times;
                            prix *= times;
                        }
                        #endregion

                        tot += prix;
                        weight -= poids2;
                    }

                    return new DataContrats.Out.Freight {
                        mode = this.freight.mode,
                        price = new DataContrats.Common.Price {
                            baseValue = tot,
                            currencyCode = "EUR",
                        },
                        taxes = new List<DataContrats.Common.Tax> {
                            new DataContrats.Common.Tax {
                                name = "VAT",
                                rate = this.Rate(null, CompDI, ref configuration, ref cache, configuration.ExpenseItemCode),
                            }
                        },
                    };
                } finally {
                    rc.ReleaseComObject();
                }
            } else {
                return new DataContrats.Out.Freight {
                    mode = this.freight.mode,
                    taxes = new List<DataContrats.Common.Tax> {
                            new DataContrats.Common.Tax {
                                name = "VAT",
                                rate = this.Rate(null, CompDI, ref configuration, ref cache, configuration.ExpenseItemCode),
                            }
                        },
                    price = new DataContrats.Common.Price()
                };
            }
        }
        public decimal Rate(Item item, SAPbobsCOM.Company CompDI, ref MainConfiguration configuration, ref Dictionary<string, object> cache, string itemcode = null) {
            if(string.IsNullOrEmpty(itemcode))
                itemcode = null;
            // client
            var r = CompDI.QueryFirstValue(@"
SELECT /*T0.[Code], T0.[Name], */
    T0.""Rate""
/*, T0.""EffecDate"" */
FROM OVTG T0  
INNER JOIN OCRD T1 ON T0.""Code"" = T1.""ECVatGroup""
WHERE T1.""CardCode"" = '{{CardCode}}'
order by T0.""EffecDate""  desc"
                            .Replace("{{CardCode}}", this.CardCode(CompDI, ref configuration, ref cache)))
                .ToString();
            if((r == "0" || string.IsNullOrWhiteSpace(r)) && (item != null || !string.IsNullOrEmpty(itemcode))) {
                // article
                r = CompDI.QueryFirstValue(@"
SELECT /*T0.[Code], T0.[Name], */
    T0.""Rate""
/*, T0.""EffecDate"" */
FROM OVTG T0  
INNER JOIN OITM T1 ON T0.[Code] = T1.[VatGourpSa] 
Where ItemCode = '{{ItemCode}}'
order by T0.[EffecDate]  desc"
                            .Replace("{{ItemCode}}", itemcode ?? item.ItemCode(CompDI, ref cache)))
                    .ToString();
            }
            if(string.IsNullOrWhiteSpace(r)) {
                r = "0";
            }

            Logger.Debug("Rate: " + r);
            Logger.Debug("Item null: " + (item == null));
            Logger.Debug("Itemcode null: " + (itemcode == null));
            return Convert.ToDecimal(r);
        }
    }
}
