using SAPbobsCOM;
using stauff_interface_webshop_spryker_ui.Configuration;
using stauff_interface_webshop_spryker_ui.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace stauff_interface_webshop_spryker_ui.DataContrats.In {
    public sealed class Item {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [StringLength(11)]
        public string materialNumber { get; set; }
        public decimal baseQuantity { get; set; } = 1;
        [StringLength(20)]
        public string baseQuantityUnit { get; set; }
        public bool completeDelivery { get; set; }

        [StringLength(10)]
        [Description("Can contain a valid date format YYYY-MM-DD or empty")]
        public string desiredDeliveryDate { get; set; }

        private void AddOrUpdate(List<DataContrats.Out.ConfirmedQuantity> a, DataContrats.Out.ConfirmedQuantity b, DateTime datetimenow) {
            var found = false;
            if(DateTime.ParseExact(b.deliveryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture) < datetimenow.AddDays(1)) {
                b.deliveryDate = datetimenow.AddDays(1).ToString("yyyy-MM-dd");
            }
            for(int i = 0, n = a.Count; i < n; i++) {
                if(a[i].deliveryDate == b.deliveryDate) {
                    a[i].quantity += b.quantity;
                    found = true;
                }
            }
            if(!found)
                a.Add(b);
        }

        public class Stauff_webshop_dispo_date_line {
            public string Type { get; set; }
            public decimal Quantite { get; set; }
            public DateTime Date { get; set; }
            public override string ToString() {
                return "Type: " + Type + ", Quantite: " + Quantite + ", Date:" + Date;
            }
        }
        public class Stauff_webshop_dispo_stock_line {
            public decimal Mag1 { get; set; }
            public decimal Mag2 { get; set; }
            public int LeadTime { get; set; }
            public int U_Jourrecep { get; set; }
            public decimal SalFactor2 { get; set; }
            public override string ToString() {
                return "Mag1: " + Mag1 + ", Mag2: " + Mag2 + ", LeadTime:" + LeadTime + ", U_Jourrecep:" + U_Jourrecep + " SalFactor2:" + SalFactor2;
            }
        }
        public Stauff_webshop_dispo_stock_line FetchDispoStock(ref Company CompDI, ref Recordset rc, ref Dictionary<string, object> cache) {
            rc.Query(@"
select 
    STAUFF_webshop_dispo_stock.LeadTime,
    STAUFF_webshop_dispo_stock.Mag1,
    STAUFF_webshop_dispo_stock.Mag2, 
    STAUFF_webshop_dispo_stock.U_Jourrecep,
    OITM.SalFactor2
from STAUFF_webshop_dispo_stock
inner join OITM on OITM.ItemCode = STAUFF_webshop_dispo_stock.ItemCode
where STAUFF_webshop_dispo_stock.ItemCode = '{{ItemCode}}'
"
        .Replace("{{ItemCode}}", this.ItemCode(CompDI, ref cache))
        );
            rc.MoveFirst();
            return new Stauff_webshop_dispo_stock_line {
                Mag1 = Convert.ToDecimal(rc.Fields.Item("Mag1").Value?.ToString() ?? "0"),
                Mag2 = Convert.ToDecimal(rc.Fields.Item("Mag2").Value?.ToString() ?? "0"),
                LeadTime = Math.Max(Convert.ToInt32(rc.Fields.Item("LeadTime").Value?.ToString() ?? "0"), 0),
                U_Jourrecep = Math.Max(Convert.ToInt32(rc.Fields.Item("U_Jourrecep").Value?.ToString() ?? "0"), 0),
                SalFactor2 = Math.Max(Convert.ToDecimal(rc.Fields.Item("SalFactor2").Value?.ToString() ?? "1"), 1),
            };
        }
        public List<Stauff_webshop_dispo_date_line> FetchDispoDate(ref Company CompDI, ref Recordset rc, ref Dictionary<string, object> cache) {
            rc.Query(@"
select *
from STAUFF_webshop_dispo_date
where STAUFF_webshop_dispo_date.ItemCode = '{{ItemCode}}'
    and Date is not null
    and Quantite is not null
    and Quantite > 0
--order by Date
".Replace("{{ItemCode}}", this.ItemCode(CompDI, ref cache)));
            rc.MoveFirst();

            var dispo_date_lines = new List<Stauff_webshop_dispo_date_line>();
            for(int i = 0, n = rc.RecordCount; i < n; i++) {
                dispo_date_lines.Add(new Stauff_webshop_dispo_date_line {
                    Type = rc.Fields.Item("Type").Value.ToString(),
                    Quantite = Convert.ToDecimal(rc.Fields.Item("Quantite").Value),
                    Date = (DateTime)rc.Fields.Item("Date").Value,
                });
                rc.MoveNext();
            }
            return dispo_date_lines;
        }

        public List<DataContrats.Out.ConfirmedQuantity> CalculateConfirmQuantities(
            Stauff_webshop_dispo_stock_line dispo_stock,
            List<Stauff_webshop_dispo_date_line> dispo_date_lines,
            decimal qty,
            bool isDummy,
            DateTime datetimenow,
            string cardCodeDumy,
            Recordset rc, 
            string itemcode) {
            // ignorer le packaging si client dummy
            if(isDummy) {
                dispo_stock.SalFactor2 = 1;

                try {
                    rc.Query(@$"
SELECT ""U_WEBCONDDUMMY""
FROM OITM
WHERE ""ItemCode"" = '{itemcode}'
    AND ""U_WEBCONDDUMMY"" is not null
UNION ALL
SELECT ""U_WEBCONDDUMMY""
FROM OCRD
WHERE ""CardCode"" = '{cardCodeDumy}'
    AND ""U_WEBCONDDUMMY"" is not null
");
                    if(rc.RecordCount > 0) {
                        var a = Convert.ToInt32(rc.Fields.Item("U_WEBCONDDUMMY").Value);
                        if(a > 0) {
                            dispo_stock.SalFactor2 = a;
                        }
                    }
                }catch (Exception ex) {
                    Logger.Error(ex);
                }
            }

            dispo_date_lines ??= new List<Stauff_webshop_dispo_date_line>();
            var needed = qty;
            var dispo = dispo_stock.Mag1 + dispo_stock.Mag2;
            var r = new List<DataContrats.Out.ConfirmedQuantity>();
            var datetimenow_leadtime_plus_7 = datetimenow.AddDays(dispo_stock.LeadTime + 7);

            dispo_date_lines = dispo_date_lines.OrderBy(x => x.Date).ToList();

            DateTime? date_vente = dispo > 0 ? (DateTime?)datetimenow : null;
            DateTime? date_achat = null;
            for(int i = 0, n = dispo_date_lines.Count; i < n; i++) {
                // Quantitée demandée déjà remplie
                if(needed <= 0) break;

                if(dispo_date_lines[i].Type == "V") {
                    if(date_vente == null || date_vente > dispo_date_lines[0].Date) {
                        date_vente = dispo_date_lines[i].Date;
                    }
                    dispo -= dispo_date_lines[i].Quantite;
                    Logger.Debug("Dispo: " + dispo);
                    Logger.Debug("Needed: " + needed);
                    Logger.Debug(dispo_date_lines[i].ToString());
                    if(dispo == 0)
                        date_vente = null;
                } else if(dispo_date_lines[i].Type == "A") {
                    if(date_achat == null || date_achat < dispo_date_lines[0].Date) {
                        date_achat = dispo_date_lines[i].Date;
                    }
                    if(dispo > 0 && needed > 0 && date_vente != null) {
                        var confirmQuantity = dispo_stock.SalFactor2 * (int)Math.Ceiling(Math.Min(needed, dispo) / dispo_stock.SalFactor2);
                        if(confirmQuantity > dispo)
                            confirmQuantity -= dispo_stock.SalFactor2;
                        if(confirmQuantity > 0) {
                            AddOrUpdate(r, new DataContrats.Out.ConfirmedQuantity {
                                quantity = confirmQuantity,
                                deliveryDate = date_vente?.AddDays(1).ToString("yyyy-MM-dd"),
                            }, datetimenow);
                            needed -= confirmQuantity;
                            dispo -= confirmQuantity;
                            date_vente = null;
                            Logger.Debug("Dispo: " + dispo);
                            Logger.Debug("Needed: " + needed);
                            Logger.Debug(dispo_date_lines[i].ToString());
                        }
                    }

                    dispo += dispo_date_lines[i].Quantite;
                    Logger.Debug("Dispo: " + dispo);
                }
            }

            // dernière date d'acaht ?
            if(needed > 0 && dispo > 0) {
                var confirmQuantity = dispo_stock.SalFactor2 * (int)Math.Ceiling(Math.Min(needed, dispo) / dispo_stock.SalFactor2);
                if(confirmQuantity > dispo)
                    confirmQuantity -= dispo_stock.SalFactor2;
                if(date_achat == null)
                    date_achat = datetimenow;
                if(confirmQuantity > 0) {
                    AddOrUpdate(r, new DataContrats.Out.ConfirmedQuantity {
                        quantity = confirmQuantity,
                        deliveryDate = date_achat?.Date.AddDays(dispo_stock.U_Jourrecep).ToString("yyyy-MM-dd"),
                        //deliveryDate = datetimenow.Date.AddDays(1).ToString("yyyy-MM-dd"),
                    }, datetimenow);
                    needed -= confirmQuantity;
                    dispo -= confirmQuantity;
                    Logger.Debug("Dispo: " + dispo);
                    Logger.Debug("Needed: " + needed);
                    Logger.Debug(datetimenow.ToString());
                }
            }

            // Quantitée demandée pas encore remplie
            if(needed > 0) {
                AddOrUpdate(r, new DataContrats.Out.ConfirmedQuantity {
                    quantity = dispo_stock.SalFactor2 * (int)Math.Ceiling(needed / dispo_stock.SalFactor2),
                    deliveryDate = datetimenow.AddDays(dispo_stock.LeadTime).AddDays(7).ToString("yyyy-MM-dd"),
                }, datetimenow);
                dispo -= needed;
                needed = 0;
                Logger.Debug("Dispo: " + dispo);
                Logger.Debug("Needed: " + needed);
            }

            return r.OrderBy(x => x.deliveryDate ?? DateTime.MaxValue.ToString()).ToList();
        }
        public List<DataContrats.Out.ConfirmedQuantity> ConfirmedQuantities(ref Order order, ref MainConfiguration configuration, ref SAPbobsCOM.Company CompDI, ref Dictionary<string, object> cache) {
            Recordset rc = null;
            try {
                var DesiredDeliveryDate = DateTime.Now;
                if(!string.IsNullOrWhiteSpace(this.desiredDeliveryDate))
                    DesiredDeliveryDate = DateTime.ParseExact(this.desiredDeliveryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                rc = CompDI.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;

                var dispo_stock = FetchDispoStock(ref CompDI, ref rc, ref cache);

                var needed = this.baseQuantity;

                var dispo_date_lines = FetchDispoDate(ref CompDI, ref rc, ref cache);

                var isDummy = order.CardCode(CompDI, ref configuration, ref cache) == configuration.CodeClientDummy;

                return CalculateConfirmQuantities(dispo_stock, dispo_date_lines, this.baseQuantity, isDummy, DateTime.Now, configuration.CodeClientDummy, rc, this.ItemCode(CompDI, ref cache));
            } catch(Exception) {
                throw;
            } finally {
                rc.ReleaseComObject();
            }
        }
        public string ItemCode(SAPbobsCOM.Company CompDI, ref Dictionary<string, object> cache) {
            var key = "ItemCode:" + this.materialNumber;
            if(!cache.ContainsKey(key)) {
                cache.Add(key, CompDI.QueryFirstValue(
                        @"
SELECT OITM.""ItemCode""
from OITM
WHERE OITM.""validFor"" <> 'N' and OITM.""U_CodesSTD"" = '{{Substitute}}'"
                        .Replace("{{Substitute}}", this.materialNumber)
                        )?.ToString());
            }
            return (string)cache[key];
        }
    }
}
