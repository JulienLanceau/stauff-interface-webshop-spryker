using stauff_interface_webshop_spryker_ui.DataContrats.In;
using System;
using System.Collections.Generic;
using System.Text;
using SAPbobsCOM;
using stauff_interface_webshop_spryker_ui.Configuration;
using System.Linq;
using stauff_interface_webshop_spryker_ui.DataContrats.Out;
using stauff_interface_webshop_spryker_ui.DataContrats.Common;
using System.Globalization;

namespace stauff_interface_webshop_spryker_ui.Extensions {
    public static class SimulationExtension {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static SimulationReturn Simulation(this Order order, Company CompDI, MainConfiguration configuration) {
            SimulationReturn @return = new SimulationReturn();

            //lock(DIAPI.DIAPI_LOCK)
            {
                try {
                    if(order.items.Count == 0) {
                        throw new Exception("Simulation should have at least 1 item");
                    }

                    //var CompDI = DIAPI.GetDIAPI();
                    Dictionary<string, object> cache = new Dictionary<string, object>();
                    var isClientADummyClient = order.CardCode(CompDI, ref configuration, ref cache) == configuration.CodeClientDummy;

                    foreach(var in_item in order.items) {
                        var line_item = new stauff_interface_webshop_spryker_ui.DataContrats.Out.Item();

                        try {
                            if(in_item.baseQuantity <= 0) {
                                throw new Exception("Quantity should be more than 0");
                            }

                            // Prix toujours en quantité unitaire pour baseprice
                            line_item.baseQuantity = 1;
                            line_item.baseQuantityUnit = in_item.baseQuantityUnit;

                            // Calcul taxe
                            line_item.taxes.Add(new Tax {
                                name = "VAT",
                                rate = order.Rate(in_item, CompDI, ref configuration, ref cache),
                            });

                            // Si ItemCode non trouvé, jeter une erreur
                            if(string.IsNullOrWhiteSpace(in_item.ItemCode(CompDI, ref cache))) {
                                throw new Exception("Item not available in country database");
                            }

                            // Prix unitaire
                            line_item.basePrice = order.Price(in_item, ref configuration, CompDI, ref cache, isClientADummyClient);
                            if(line_item.basePrice.netValue <= 0 || string.IsNullOrWhiteSpace(line_item.basePrice.currencyCode)) {
                                throw new Exception("Price not configured for item '" + in_item.materialNumber + "' in country database");
                            }
                            // Arroundi à deux décimales
                            line_item.basePrice.netValue = Math.Round(line_item.basePrice.netValue, 2);
                            line_item.basePrice.grossValue = Math.Round(line_item.basePrice.grossValue, 2);

                            line_item.confirmedQuantities = in_item.ConfirmedQuantities(ref order, ref configuration, ref CompDI, ref cache);
                            if(line_item.confirmedQuantities.Count == 0 || line_item.confirmedQuantities.Sum(x => x.quantity) < in_item.baseQuantity) {
                                throw new Exception("Confirmed quantities in country database inferior to requested quantity for item '" + in_item.materialNumber + "'");
                            }

                            // Ajuster la quantité
                            in_item.baseQuantity = line_item.confirmedQuantities.Sum(x => x.quantity);

                            line_item.positionPrice.baseValue = line_item.basePrice.netValue * in_item.baseQuantity;
                            line_item.positionPrice.currencyCode = line_item.basePrice.currencyCode;

                            // Arroundi à deux décimales
                            line_item.positionPrice.baseValue = Math.Round(line_item.positionPrice.baseValue, 2);
                        } catch(Exception e) {
                            Logger.Error(e.ToString());
                            line_item.error = e.Message;
                        }
                        @return.items.Add(line_item);
                    }

                    @return.freight = order.Freight(CompDI, ref configuration, ref cache, isClientADummyClient);
                } catch(Exception e) {
                    Logger.Error(e.ToString());
                    @return.error = e.Message;
                } finally {
                }
            }
            return @return;
        }
    }
}
