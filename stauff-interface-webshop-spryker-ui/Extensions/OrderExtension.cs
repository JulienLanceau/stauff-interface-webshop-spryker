using SAPbobsCOM;
using stauff_interface_webshop_spryker_ui.Configuration;
using stauff_interface_webshop_spryker_ui.DataContrats.In;
using stauff_interface_webshop_spryker_ui.DataContrats.Out;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace stauff_interface_webshop_spryker_ui.Extensions {
    public static class OrderExtension {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static OrderReturn Order(
                this Order order, Company CompDI, MainConfiguration configuration,
                Action<string> log_order, Action<string> log_downpayment, Action<string> log_payment,
                bool test = false) {
            var @return = new OrderReturn();

            if(string.IsNullOrWhiteSpace(order?.customerOrderNumber))
                order.customerOrderNumber = order.sprykerOrderNumber;

            if(string.IsNullOrEmpty(order.shipping.address.emailAddress))
                order.shipping.address.emailAddress = order.customerAddress.emailAddress;

            if(string.IsNullOrEmpty(order.shipping.address.telephoneNumber))
                order.shipping.address.telephoneNumber = order.customerAddress.telephoneNumber;

            //lock(DIAPI.DIAPI_LOCK) 
            {
                Documents sbo_order = null;
                Documents sbo_downpayment = null;
                Payments sbo_payment = null;
                try {
                    if(order.items.Count == 0) {
                        throw new Exception("Order sould have at least 1 item");
                    }
                    order.freight ??= new DataContrats.In.Freight();
                    if(string.IsNullOrWhiteSpace(order.freight.mode)) {
                        order.freight.mode = "Standard";
                    }
                    Dictionary<string, object> cache = new Dictionary<string, object>();

                    sbo_order = CompDI.GetBusinessObject(BoObjectTypes.oOrders) as Documents;

                    if(configuration.OrderSeries > 0) {
                        sbo_order.Series = configuration.OrderSeries;
                    }

                    sbo_order.CardCode = order.CardCode(CompDI, ref configuration, ref cache);
                    sbo_order.DocumentsOwner = configuration.CodeSalarie;
                    sbo_order.DocDate = DateTime.Now;
                    sbo_order.Confirmed = BoYesNoEnum.tNO;
                    sbo_order.PartialSupply = order.completeDelivery ? BoYesNoEnum.tNO : BoYesNoEnum.tYES;
                    sbo_order.NumAtCard = order.customerOrderNumber;
                    sbo_order.Indicator = "W"; // NOTE: Web
                    if(!string.IsNullOrWhiteSpace(order.orderComments))
                        sbo_order.Comments = order.orderComments;

                    var isClientADummyClient = sbo_order.CardCode == configuration.CodeClientDummy;
                    var isClientSoumisASurcharge = sbo_order.CardCode != configuration.CodeClientNonSoumisASurcharge;

                    var isPayPerInvoice = string.IsNullOrWhiteSpace(order.paymentReference) && isClientADummyClient;

                    if(!isPayPerInvoice && isClientADummyClient) {
                        sbo_order.Confirmed = BoYesNoEnum.tYES;
                    }
                    if(isPayPerInvoice) {
                        sbo_order.PaymentGroupCode = configuration.PaymentMethodPayPerInvoice;
                        sbo_order.UserField("U_AppPL").Value = "C";
                        sbo_order.UserField("U_PROFORMA").Value = "Y";
                    }

                    #region FederalTaxID/vatNumber/LicTradNum
                    if(!string.IsNullOrWhiteSpace(order.vatNumber))
                        sbo_order.FederalTaxID = order.vatNumber;
                    #endregion

                    #region Contact
                    var contact = order.ContactCode(CompDI, ref configuration, ref cache);
                    if(contact != -1) {
                        sbo_order.ContactPersonCode = contact;
                    }
                    #endregion

                    #region Addresses
                    var ship_address = String.Join("\r\n",
                        order.shipping.address.name,
                        (order.shipping.address.houseNumber?.Trim() + " " + order.shipping.address.street?.Trim()).Trim(),
                        (order.shipping.address.postalCode?.Trim() + " " + order.shipping.address.city?.Trim()).Trim(),
                        order.shipping.address.countryCode?.CountryNameFromTwoLetterISO()
                        );

                    sbo_order.ShipToCode = CompDI.QueryFirstValue("SELECT ShipToDef FROM OCRD WHERE CardCode = '" + order.CardCode(CompDI, ref configuration, ref cache).Replace("'", "''") + "'").ToString();

                    sbo_order.AddressExtension.ShipToCity = order.shipping.address.city;
                    sbo_order.AddressExtension.ShipToZipCode = order.shipping.address.postalCode;
                    sbo_order.AddressExtension.ShipToStreet = (order.shipping.address.houseNumber?.Trim() + " " + order.shipping.address.street?.Trim()).Trim();
                    sbo_order.AddressExtension.ShipToCountry = order.shipping.address.countryCode;

                    sbo_order.Address2 = ship_address;

                    var bill_address = String.Join("\r\n",
                        order.customerAddress.name,
                        (order.customerAddress.houseNumber?.Trim() + " " + order.customerAddress.street?.Trim()).Trim(),
                        (order.customerAddress.postalCode?.Trim() + " " + order.customerAddress.city?.Trim()).Trim(),
                        order.customerAddress.countryCode?.CountryNameFromTwoLetterISO()
                        );

                    sbo_order.PayToCode = CompDI.QueryFirstValue("SELECT BillToDef FROM OCRD WHERE CardCode = '" + order.CardCode(CompDI, ref configuration, ref cache).Replace("'", "''") + "'").ToString();

                    sbo_order.AddressExtension.BillToCity = order.customerAddress.city;
                    sbo_order.AddressExtension.BillToZipCode = order.customerAddress.postalCode;
                    sbo_order.AddressExtension.BillToStreet = order.customerAddress.houseNumber + " " + order.customerAddress.street;
                    sbo_order.AddressExtension.BillToCountry = order.customerAddress.countryCode;

                    sbo_order.Address = bill_address;
                    #endregion

                    #region remplissage de ZUs
                    sbo_order.UserField("U_ACCENC").Value = "Y";
                    sbo_order.UserField("U_NCdeWeb").SetValueCutIfNecessary(order.sprykerOrderNumber ?? order.customerOrderNumber);
                    sbo_order.UserField("U_TelWeb").SetValueCutIfNecessary(order.shipping.address.telephoneNumber ?? order.customerAddress.telephoneNumber);
                    sbo_order.UserField("U_EmailWeb").SetValueCutIfNecessary(order.shipping.address.emailAddress ?? order.customerAddress.emailAddress);
                    sbo_order.UserField("U_NOMCORRE").SetValueCutIfNecessary(CompDI.QueryFirstValue(@"
SELECT T1.""Name"" from OCRD T0
Inner join ""@CORRESP""  t1 on T0.""U_CORRESP"" = T1.""Code""
 where '{{CardCode}}' = T0.""Cardcode""
"
.Replace("{{CardCode}}", order.CardCode(CompDI, ref configuration, ref cache))
                    )?.ToString() ?? "");

                    sbo_order.UserField("U_AGREM").SetValueCutIfNecessary(CompDI.QueryFirstValue(
                            "SELECT Case Coalesce(T0.U_AGRM1,0)  when 0 then t0.U_AGREM ELSE T0.U_AGRM1 END from OCRD T0 where '$[Ordr.CardCode]' = T0.Cardcode"
                            .Replace("$[Ordr.CardCode]", order.CardCode(CompDI, ref configuration, ref cache))
                            )?.ToString() ?? "");
                    sbo_order.UserField("U_ENTJUR").SetValueCutIfNecessary(CompDI.QueryFirstValue(
                            "SELECT T0.U_ENTJUR from OCRD T0 where '$[ORDR.Cardcode]' = T0.Cardcode"
                            .Replace("$[ORDR.Cardcode]", order.CardCode(CompDI, ref configuration, ref cache))
                            )?.ToString() ?? "");
                    sbo_order.UserField("U_MINI").SetValueCutIfNecessary(CompDI.QueryFirstValue(
                            "SELECT T0.U_MINI from OCRD T0 where '$[ORDR.Cardcode]' = T0.Cardcode"
                            .Replace("$[ORDR.Cardcode]", order.CardCode(CompDI, ref configuration, ref cache))
                            )?.ToString() ?? "");
                    sbo_order.UserField("U_GROUPAGE").SetValueCutIfNecessary(CompDI.QueryFirstValue(
                            "SELECT T0.U_GROUPAGE from OCRD T0 where '$[ORDR.Cardcode]' = T0.Cardcode"
                            .Replace("$[ORDR.Cardcode]", order.CardCode(CompDI, ref configuration, ref cache))
                            )?.ToString() ?? "");
                    sbo_order.UserField("U_REMC").SetValueCutIfNecessary(CompDI.QueryFirstValue(
                            "SELECT T0.U_REMC from OCRD T0 where '$[ORDR.Cardcode]' = T0.Cardcode"
                            .Replace("$[ORDR.Cardcode]", order.CardCode(CompDI, ref configuration, ref cache))
                            )?.ToString() ?? "");
                    {
                        var a = CompDI.QueryFirstValue(
                            @"SELECT T0.U_FRANCO from OCRD T0 where '{{CardCode}}' = T0.""CardCode"""
                            .Replace("{{CardCode}}", order.CardCode(CompDI, ref configuration, ref cache))
                            )?.ToString() ?? "0";
                        if(!string.IsNullOrWhiteSpace(a)) {
                            sbo_order.UserField("U_FRANCO").Value = a;
                        }
                    }
                    #endregion

                    #region Articles
                    foreach(var in_item in order.items) {
                        if(in_item.baseQuantity <= 0) {
                            throw new Exception("Quantity should be more than 0");
                        }
                        #region ItemCode
                        var itemcode = in_item.ItemCode(CompDI, ref cache);
                        if(string.IsNullOrWhiteSpace(itemcode)) {
                            throw new KeyNotFoundException("Item not available in country database");
                        }
                        #endregion

                        foreach(var in_confirmedquantities in in_item.ConfirmedQuantities(ref order, ref configuration, ref CompDI, ref cache)) {
                            if(!string.IsNullOrWhiteSpace(sbo_order.Lines.ItemCode)) {
                                sbo_order.Lines.Add();
                            }
                            sbo_order.Lines.ItemCode = in_item.ItemCode(CompDI, ref cache);

                            sbo_order.Lines.Quantity = Convert.ToDouble(in_confirmedquantities.quantity);
                            sbo_order.Lines.MeasureUnit = in_item.baseQuantityUnit;
                            sbo_order.Lines.WarehouseCode = configuration.CodeMagasin1;

                            var ShipDate = DateTime.ParseExact(in_confirmedquantities.deliveryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            sbo_order.Lines.ShipDate = ShipDate;

                            if(sbo_order.DocDueDate < ShipDate)
                                sbo_order.DocDueDate = ShipDate;

                            var baseprice = order.Price(in_item, ref configuration, CompDI, ref cache, isClientADummyClient);
                            sbo_order.Lines.Currency = baseprice.currencyCode;
                            sbo_order.Lines.UnitPrice = Convert.ToDouble(baseprice.grossValue);
                            sbo_order.Lines.DiscountPercent = Convert.ToDouble(baseprice.discount);

                            sbo_order.Lines.ShipToDescription = ship_address.Replace("\r\n", " ");

                            #region U_STPREPA
                            var quatity_dispo = CompDI.QueryFirstValue(@"
select ""OnHand""-""IsCommited""
from OITW
where ""WhsCode"" = '{{WhsCode}}'
and ""ItemCode"" = '{{ItemCode}}'
"
.Replace("{{ItemCode}}", in_item.ItemCode(CompDI, ref cache))
.Replace("{{WhsCode}}", configuration.CodeMagasin1)
).ToString();
                            if(string.IsNullOrWhiteSpace(quatity_dispo))
                                quatity_dispo = "0";

                            if(Convert.ToDecimal(quatity_dispo) > in_confirmedquantities.quantity) {
                                sbo_order.Lines.UserField("U_STPREPA").Value = "2";
                            } else {
                                sbo_order.Lines.UserField("U_STPREPA").Value = "1";
                            }
                            #endregion
                            if(isClientADummyClient) {
                                sbo_order.Lines.UserField("U_FORCE").SetValueCutIfNecessary("O");
                            }
                            sbo_order.Lines.UserField("U_CodeSTD").SetValueCutIfNecessary(in_item.materialNumber);
                        }
                    }
                    #endregion

                    #region Fret / tansport / expédition
                    switch(order.freight.mode.ToLower().Trim()) {
                        case "standard": {
                                sbo_order.TransportationCode = configuration.FeightStandard;
                            }
                            break;
                        case "express": {
                                sbo_order.TransportationCode = configuration.FeightExpress;
                            }
                            break;
                        case "ex works": {
                                sbo_order.TransportationCode = configuration.FeightExWorks;
                            }
                            break;
                        case "ns": {
                                sbo_order.TransportationCode = configuration.FeightNS;
                            }
                            break;
                        default:
                            throw new Exception("Invalid freight");
                    }

                    var freight = order.Freight(CompDI, ref configuration, ref cache, isClientADummyClient);

                    if(isClientADummyClient) {
                        sbo_order.Expenses.ExpenseCode = configuration.ExpenseCode;
                        sbo_order.Expenses.LineTotal = Convert.ToDouble(freight.price.baseValue);
                        sbo_order.Expenses.Add();
                    }
                    #endregion
                    sbo_order.DiscountPercent = 0;

                    #region Log XML
                    try {
                        try {
                            XmlDocument document = new XmlDocument();
                            document.Load(new StringReader(sbo_order.GetAsXML()));

                            StringBuilder builder = new StringBuilder();
                            using(XmlTextWriter writer = new XmlTextWriter(new StringWriter(builder))) {
                                writer.Formatting = Formatting.Indented;
                                document.Save(writer);
                            }

                            log_order(builder.ToString());
                        } catch {
                            log_order(sbo_order.GetAsXML());
                        }
                    } catch { }
                    #endregion

                    if(test) {
                        CompDI.StartTransaction();
                    }

                    if(sbo_order.Add() != 0) {
                        var errorMessage = CompDI.GetLastErrorDescription();
                        Logger.Warn(errorMessage);
                        throw new Exception("DI API,Order: " + errorMessage);
                    } else {
                        @return.orderNumber = CompDI.GetNewObjectKey();
                        sbo_order.GetByKey(Convert.ToInt32(@return.orderNumber));
                        Logger.Info("Commande créée : " + sbo_order.DocNum);
                        @return.orderNumber = sbo_order.DocNum.ToString();
                    }

                    if(isClientSoumisASurcharge && (sbo_order.DocTotal - sbo_order.VatSum) < Convert.ToDouble(configuration.MontantInferieurAuquelAjoutSurcharge)) {
                        if(!string.IsNullOrEmpty(sbo_order.Lines.ItemCode)) {
                            sbo_order.Lines.Add();
                        }
                        sbo_order.Lines.ItemCode = configuration.ArticleSurcharge;
                        sbo_order.Lines.Quantity = 1;
                        if(sbo_order.Update() != 0) {
                            var errorMessage = CompDI.GetLastErrorDescription();
                            throw new Exception("DI API,Order(Update Surcharge): " + errorMessage);
                        } else {
                            sbo_order.GetByKey(Convert.ToInt32(@return.orderNumber));
                        }
                    }

                    if(isPayPerInvoice) {
                        // xml coresuite
                        var xmlFilepath = Path.Combine(configuration.CoresuiteOrderMail.DossierXml, "139_" + sbo_order.DocEntry + ".xml");
                        Logger.Warn("Generation PDF via " + xmlFilepath);

                        var xmlRequest = @"
<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<job>
    <type mode=""Email"" layoutid=""0"" formtype=""{{FormType}}""/>
    <paramsets>
        <paramset>
            <params>
                <param key=""DocEntry"" value=""{{DocEntry}}""/>
                <param key=""PrintDef"" value=""{{PrintDef}}""/>
            </params>
        </paramset>
    </paramsets>
</job>"
                        .Trim()
                            .Replace("{{DocEntry}}", sbo_order.DocEntry.ToString())
                            .Replace("{{PrintDef}}", configuration.CoresuiteOrderMail.PrintDef)
                            .Replace("{{FormType}}", "139");

                        File.WriteAllText(xmlFilepath, xmlRequest);
                    }

                    if(!isPayPerInvoice && !string.IsNullOrWhiteSpace(order.paymentReference) && !string.IsNullOrWhiteSpace(@return.orderNumber)) {
                        #region Facture d'acompte
                        Logger.Debug("Facture d'acompte");
                        sbo_downpayment = CompDI.GetBusinessObject(BoObjectTypes.oDownPayments) as Documents;
                        sbo_downpayment.CardCode = sbo_order.CardCode;
                        sbo_downpayment.DocDate = sbo_order.DocDate;
                        sbo_downpayment.NumAtCard = sbo_order.NumAtCard;

                        sbo_downpayment.PaymentMethod = configuration.PaymentMethodDummy; // NOTE(TM) : config pas correcte pour ma base de dev
                        sbo_downpayment.DownPaymentType = DownPaymentTypeEnum.dptInvoice;
                        sbo_downpayment.DownPaymentAmount = sbo_order.DocTotal;

                        #region Articles
                        for(int i = 0, n = sbo_order.Lines.Count; i < n; i++) {
                            sbo_order.Lines.SetCurrentLine(i);

                            sbo_downpayment.Lines.BaseEntry = sbo_order.DocEntry;
                            sbo_downpayment.Lines.BaseLine = sbo_order.Lines.LineNum;
                            sbo_downpayment.Lines.BaseType = (int)BoObjectTypes.oOrders;

                            sbo_downpayment.Lines.ItemCode = sbo_order.Lines.ItemCode;
                            sbo_downpayment.Lines.Quantity = sbo_order.Lines.Quantity;
                            sbo_downpayment.Lines.VatGroup = configuration.VatGroupDownPayment;
                            sbo_downpayment.Lines.Add();
                        }
                        #endregion
                        #region Frets
                        for(int i = 0, n = sbo_order.Expenses.Count; i < n; i++) {
                            sbo_order.Expenses.SetCurrentLine(i);

                            sbo_downpayment.Lines.ItemCode = configuration.ExpenseItemCode;
                            sbo_downpayment.Lines.Quantity = 1;
                            sbo_downpayment.Lines.LineTotal = sbo_order.Expenses.LineTotal;
                            sbo_downpayment.Lines.VatGroup = configuration.VatGroupDownPayment;
                            sbo_downpayment.Lines.Add();

                            //Pas de fret possible sur les factures d'acompte
                            /*
                            sbo_downpayment.Expenses.ExpenseCode = sbo_order.Expenses.ExpenseCode;
                            sbo_downpayment.Expenses.LineTotal = sbo_order.Expenses.LineTotal;
                            sbo_downpayment.Add();
                            */
                        }
                        #endregion

                        #region Log XML
                        try {
                            XmlDocument document = new XmlDocument();
                            document.Load(new StringReader(sbo_downpayment.GetAsXML()));

                            StringBuilder builder = new StringBuilder();
                            using(XmlTextWriter writer = new XmlTextWriter(new StringWriter(builder))) {
                                writer.Formatting = Formatting.Indented;
                                document.Save(writer);
                            }

                            log_downpayment(builder.ToString());
                        } catch {
                            log_downpayment(sbo_downpayment.GetAsXML());
                        }
                        #endregion

                        sbo_downpayment.DocTotal = sbo_order.DocTotal;

                        if(sbo_downpayment.Add() != 0) {
                            throw new Exception("DI API,DownPayment: " + CompDI.GetLastErrorDescription());
                        } else {
                            sbo_downpayment.GetByKey(Convert.ToInt32(CompDI.GetNewObjectKey()));
                            Logger.Info("Facture d'acompte créée : " + CompDI.GetNewObjectKey());
                            sbo_downpayment.GetByKey(Convert.ToInt32(CompDI.GetNewObjectKey()));
                        }
                        #endregion

                        #region Encaissement
                        Logger.Debug("Encaissement");
                        sbo_payment = CompDI.GetBusinessObject(BoObjectTypes.oIncomingPayments) as Payments;

                        sbo_payment.CardCode = order.CardCode(CompDI, ref configuration, ref cache);
                        sbo_payment.TransferDate = sbo_payment.DocDate = sbo_downpayment.DocDate;
                        sbo_payment.TransferAccount = configuration.PaymentAccountDummy;
                        sbo_payment.Remarks
                            = sbo_payment.JournalRemarks
                            = sbo_payment.TransferReference
                            = sbo_downpayment.NumAtCard;

                        sbo_payment.TransferSum
                            = sbo_payment.Invoices.SumApplied
                            = sbo_downpayment.DocTotal;
                        sbo_payment.Invoices.DocEntry = sbo_downpayment.DocEntry;
                        sbo_payment.Invoices.InvoiceType = BoRcptInvTypes.it_DownPayment;
                        sbo_payment.Invoices.Add();

                        #region Log XML
                        try {
                            XmlDocument document = new XmlDocument();
                            document.Load(new StringReader(sbo_payment.GetAsXML()));

                            StringBuilder builder = new StringBuilder();
                            using(XmlTextWriter writer = new XmlTextWriter(new StringWriter(builder))) {
                                writer.Formatting = Formatting.Indented;
                                document.Save(writer);
                            }

                            log_payment(builder.ToString());
                        } catch {
                            log_payment(sbo_payment.GetAsXML());
                        }
                        #endregion

                        if(sbo_payment.Add() != 0) {
                            throw new Exception("DI API,Payment: " + CompDI.GetLastErrorDescription());
                        } else {
                            Logger.Info("Encaissement créée : " + CompDI.GetNewObjectKey());
                            sbo_payment.GetByKey(Convert.ToInt32(CompDI.GetNewObjectKey()));
                            //@return.orderNumber = CompDI.GetNewObjectKey();
                        }
                        #endregion
                    }

                    if(test) {
                        CompDI.EndTransaction(BoWfTransOpt.wf_RollBack);
                    }
                } catch(Exception e) {
                    Logger.Error(e.ToString());
                    #region Log XML
                    if(sbo_order != null) {
                        try {
                            try {
                                XmlDocument document = new XmlDocument();
                                document.Load(new StringReader(sbo_order.GetAsXML()));

                                StringBuilder builder = new StringBuilder();
                                using(XmlTextWriter writer = new XmlTextWriter(new StringWriter(builder))) {
                                    writer.Formatting = Formatting.Indented;
                                    document.Save(writer);
                                }

                                log_order(builder.ToString());
                            } catch {
                                log_order(sbo_order.GetAsXML());
                            }
                        } catch { }
                    }
                    if(sbo_downpayment != null) {
                        try {
                            try {
                                XmlDocument document = new XmlDocument();
                                document.Load(new StringReader(sbo_downpayment.GetAsXML()));

                                StringBuilder builder = new StringBuilder();
                                using(XmlTextWriter writer = new XmlTextWriter(new StringWriter(builder))) {
                                    writer.Formatting = Formatting.Indented;
                                    document.Save(writer);
                                }

                                log_downpayment(builder.ToString());
                            } catch {
                                log_downpayment(sbo_downpayment.GetAsXML());
                            }
                        } catch { }
                    }
                    if(sbo_payment != null) {
                        try {
                            try {
                                XmlDocument document = new XmlDocument();
                                document.Load(new StringReader(sbo_payment.GetAsXML()));

                                StringBuilder builder = new StringBuilder();
                                using(XmlTextWriter writer = new XmlTextWriter(new StringWriter(builder))) {
                                    writer.Formatting = Formatting.Indented;
                                    document.Save(writer);
                                }

                                log_payment(builder.ToString());
                            } catch {
                                log_payment(sbo_payment.GetAsXML());
                            }
                        } catch { }
                    }
                    #endregion
                    @return.error = e.Message;
                } finally {
                    if(CompDI?.InTransaction == true) {
                        CompDI.EndTransaction(BoWfTransOpt.wf_RollBack);
                    }
                    sbo_payment.ReleaseComObject();
                    sbo_downpayment.ReleaseComObject();
                    sbo_order.ReleaseComObject();
                }
            }

            return @return;
        }
    }
}
