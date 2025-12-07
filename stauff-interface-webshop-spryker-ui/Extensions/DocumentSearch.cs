using SAPbobsCOM;
using stauff_interface_webshop_spryker_ui.DataContrats.In;
using stauff_interface_webshop_spryker_ui.DataContrats.Out;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using stauff_interface_webshop_spryker_ui.Configuration;
using System.IO;
using System.Xml.Linq;
using System.Reflection;
using System.Linq;
using static stauff_interface_webshop_spryker_ui.Extensions.DocumentSearch;
using System.Data;

namespace stauff_interface_webshop_spryker_ui.Extensions {
    public sealed class DocumentSearch {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static OrderBulkDetailOutputWrapper OrderBulkDetail(Company DIAPI, OrderBulkDetailInput search) {
            List<OrderBulkDetailOutput> result = new List<OrderBulkDetailOutput>();
            Recordset rc = null;
            try {
                rc = DIAPI.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;
                rc.Query(@"
SELECT
    O{{Table}}.CardCode,
    O{{Table}}.CardName,
    O{{Table}}.NumAtCard,
    O{{Table}}.DocNum,
    OITM.U_CodesSTD as ItemCode,
    {{Table}}1.LineNum,
    {{Table}}1.Dscription,
    {{Table}}1.Quantity,
    {{Table}}1.OpenQty,
    {{Table}}1.ShipDate,
    {{Table}}1.Price,
    {{Table}}1.LineTotal,
    STRING_AGG(LOD.DocNum, ',') as OdDocNum,
    STRING_AGG(LOI.DocNum, ',') as OiDocNum,
    {{Table}}1.U_NumCat,
    OSHP.TrnspName,
    {{Table}}1.Currency
FROM O{{Table}}
    INNER JOIN {{Table}}1 ON {{Table}}1.DocEntry = O{{Table}}.DocEntry
    INNER JOIN OITM on {{Table}}1.ItemCode = OITM.ItemCode
-- Livraison
    LEFT JOIN DLN1 LD1 on 1=1
        and LD1.BaseType = 17 
        and LD1.BaseEntry = {{Table}}1.DocEntry 
        and LD1.BaseLine = {{Table}}1.LineNum
    LEFT JOIN ODLN LOD on LOD.DocEntry = LD1.DocEntry
-- Facture
    LEFT JOIN INV1 LI1 on 1=1
        and LI1.BaseType = 15
        and LI1.BaseEntry = LD1.DocEntry 
        and LI1.BaseLine = LD1.LineNum
    LEFT JOIN OINV LOI on LOI.DocEntry = LI1.DocEntry
--
    LEFT JOIN OSHP on O{{Table}}.TrnspCode = OSHP.TrnspCode
WHERE 1=1
    and ('{{DebitorNumber}}' = '' or '{{DebitorNumber}}' = O{{Table}}.CardCode)
    and (O{{Table}}.Docnum in ('{{OrderNumbers}}'))
GROUP BY 
    O{{Table}}.CardCode,
    O{{Table}}.CardName,
    O{{Table}}.NumAtCard,
    O{{Table}}.DocNum,
    OITM.U_CodesSTD,
    {{Table}}1.Dscription,
    {{Table}}1.Quantity,
    {{Table}}1.OpenQty,
    {{Table}}1.ShipDate,
    {{Table}}1.Price,
    {{Table}}1.LineNum,
    {{Table}}1.LineTotal,
    {{Table}}1.U_NumCat,
    OSHP.TrnspName,
    {{Table}}1.Currency
"
                    .Replace("{{Table}}", "RDR")
                    .Replace("{{DebitorNumber}}", search.DebitorNumber)
                    .Replace("{{Email}}", search.Email)
                    .Replace("{{OrderNumbers}}", string.Join("', '", search.OrderNumbers)));

                for(int i = 0, n = rc.RecordCount; i < n; i++, rc.MoveNext()) {
                    result.Add(new OrderBulkDetailOutput {
                        DebitorNumber = rc.Fields.Item("CardCode").Value.ToString(),
                        CustomerName = rc.Fields.Item("CardName").Value.ToString(),
                        CustomerOrderNumber = rc.Fields.Item("NumAtCard").Value.ToString(),
                        OrderNumber = rc.Fields.Item("DocNum").Value.ToString(),
                        OrderPositionNumber = Convert.ToInt32(rc.Fields.Item("LineNum").Value.ToString()) + 1,
                        MaterialNumber = rc.Fields.Item("ItemCode").Value.ToString(),
                        MaterialDescription = rc.Fields.Item("Dscription").Value.ToString(),
                        CustomerMaterialNumber = rc.Fields.Item("U_NumCat").Value.ToString(),
                        TotalQuantity = (decimal)((double)rc.Fields.Item("Quantity").Value),
                        QuantityUnit = "PCE",
                        OpenQuantity = (decimal)((double)rc.Fields.Item("OpenQty").Value),
                        Price = Math.Round((decimal)((double)rc.Fields.Item("Price").Value), 2),
                        LineTotal = Math.Round((decimal)((double)rc.Fields.Item("LineTotal").Value), 2),
                        Currency = rc.Fields.Item("Currency").Value.ToString(),
                        ShippedQuantity = (decimal)((double)rc.Fields.Item("Quantity").Value) - (decimal)((double)rc.Fields.Item("OpenQty").Value),
                        ShippingDatePlanned = ((DateTime?)rc.Fields.Item("ShipDate").Value)?.ToString("yyyy-MM-dd"),
                        ShippingDateReal = ((DateTime?)rc.Fields.Item("ShipDate").Value)?.ToString("yyyy-MM-dd"),
                        DeliveryNumber = rc.Fields.Item("OdDocNum").Value.ToString(),
                        InvoiceNumber = rc.Fields.Item("OiDocNum").Value.ToString(),
                        ShippingCondition = rc.Fields.Item("TrnspName").Value.ToString(),
                    });
                }
            } catch(Exception ex) {
                Logger.Error(ex.ToString());
            } finally {
                rc?.ReleaseComObject();
            }
            return new OrderBulkDetailOutputWrapper {
                items = result?.ToArray(),
            };
        }

        public static DocumentDetailOutput Detail(Company DIAPI, DocType docType, DocumentDetailInput search) {
            DocumentDetailOutput result = null;
            Recordset rc = null;
            try {
                var table = docType switch {
                    DocType.Order => "RDR",
                    DocType.Invoice => "INV",
                    DocType.Delivery => "DLN",
                    _ => "ORDR" // peux potentiellement poser pb comme défaut ?
                };
                rc = DIAPI.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;
                rc.Query(@"
SELECT
    O{{Table}}.DocNum,
    O{{Table}}.NumAtCard,
    O{{Table}}.CreateDate,
    O{{Table}}.DocStatus,
    O{{Table}}.CANCELED,
    O{{Table}}.Address2,
    O{{Table}}.DocDueDate,
    {{Table}}1.LineNum,
    OITM.U_CodesSTD as ItemCode,
    {{Table}}1.Dscription,
    {{Table}}1.Quantity,
    {{Table}}1.LineStatus,
    {{Table}}1.ShipDate,
    STRING_AGG(LOD.DocNum, ',') as OdDocNum,
    LOR.DocNum as OrDocNum,
    LOR.NumAtCard as OrNumAtCard,
    LOR.CreateDate as OrCreateDate,
    LR1.Quantity as R1Quantity,
    LR1.LineNum as R1LineNum,
	STRING_AGG(LOI.DocNum, ',') as OiDocNum,
    {{Table}}1.U_NumCat,
    OSHP.TrnspName,
    {{Table}}1.VisOrder
FROM O{{Table}}
    INNER JOIN {{Table}}1 ON {{Table}}1.DocEntry = O{{Table}}.DocEntry
    INNER JOIN OITM on {{Table}}1.ItemCode = OITM.ItemCode
-- Livraison dans le cas d'une commande
    LEFT JOIN DLN1 LD1 on '{{Table}}' = 'RDR' 
        and LD1.BaseType = 17 
        and LD1.BaseEntry = {{Table}}1.DocEntry 
        and LD1.BaseLine = {{Table}}1.LineNum
    LEFT JOIN ODLN LOD on LOD.DocEntry = LD1.DocEntry
-- Facture dans le cas d'une commande
    LEFT JOIN INV1 LI1 on '{{Table}}' = 'RDR' 
        and LI1.BaseType = 15
        and LI1.BaseEntry = LD1.DocEntry 
        and LI1.BaseLine = LD1.LineNum
    LEFT JOIN OINV LOI on LOI.DocEntry = LI1.DocEntry
-- Commande dans le cas d'une livraison
    LEFT JOIN RDR1 LR1 on '{{Table}}' = 'DLN' 
        and {{Table}}1.BaseType = 17 
        and LR1.DocEntry = {{Table}}1.BaseEntry
        and LR1.LineNum = {{Table}}1.BaseLine
    LEFT JOIN ORDR LOR on LOR.DocEntry = LR1.DocEntry
--
    LEFT JOIN OSHP on O{{Table}}.TrnspCode = OSHP.TrnspCode
WHERE 1 = 1
    and ('{{DocumentNumber}}' = '' or '{{DocumentNumber}}' = O{{Table}}.DocNum)
    and ('{{Email}}' = '' or '{{Email}}' = O{{Table}}.Email or O{{Table}}.U_EmailWeb like '%{{Email}}%')
    and ('{{DebitorNumber}}' = '' or '{{DebitorNumber}}' = O{{Table}}.CardCode)
GROUP BY     
	O{{Table}}.DocNum,
    O{{Table}}.NumAtCard,
    O{{Table}}.CreateDate,
    O{{Table}}.DocStatus,
    O{{Table}}.CANCELED,
    O{{Table}}.Address2,
    O{{Table}}.DocDueDate,
    {{Table}}1.LineNum,
    OITM.U_CodesSTD,
    {{Table}}1.Dscription,
    {{Table}}1.Quantity,
    {{Table}}1.LineStatus,
    {{Table}}1.ShipDate,
    LOR.DocNum,
    LOR.NumAtCard,
    LOR.CreateDate,
    LR1.Quantity,
    LR1.LineNum,
    {{Table}}1.U_NumCat,
    OSHP.TrnspName,
    {{Table}}1.VisOrder
"
        .Replace("{{Table}}", table)
        .Replace("{{DocumentNumber}}", search.DocumentNumber)
        .Replace("{{Email}}", search.Email)
        .Replace("{{DebitorNumber}}", search.DebitorNumber));

                result = new DocumentDetailOutput {
                    DocumentNumber = rc.Fields.Item("DocNum").Value.ToString(),
                    DocumentType = docType,
                    CustomerDocumentNumber = rc.Fields.Item("NumAtCard").Value.ToString(),
                    DocumentDate = ((DateTime?)rc.Fields.Item("CreateDate").Value)?.ToString("yyyy-MM-dd"),
                    Status = ChoseDocStatus(
                            rc.Fields.Item("DocStatus").Value.ToString(),
                            rc.Fields.Item("CANCELED").Value.ToString()),
                    ShippingAddress = rc.Fields.Item("Address2").Value.ToString(),
                    ShippingDate = ((DateTime?)rc.Fields.Item("DocDueDate").Value)?.ToString("yyyy-MM-dd"),
                    ShippingCondition = rc.Fields.Item("TrnspName").Value.ToString(),
                };
                for(int i = 0, n = rc.RecordCount; i < n; i++, rc.MoveNext()) {
                    var shipdate = ((DateTime?)rc.Fields.Item("ShipDate").Value);
                    if(shipdate.HasValue && shipdate.Value.Day == 1 && shipdate.Value.Month == 1) {
                        shipdate = null;
                    }
                    result.DocumentPositions.Add(new DocumentDetailItems {
                        PositionNumber = Convert.ToInt32(rc.Fields.Item("VisOrder").Value.ToString()) + 1,
                        MaterialNumber = rc.Fields.Item("ItemCode").Value.ToString(),
                        CustomerMaterialNumber = rc.Fields.Item("U_NumCat").Value.ToString(),
                        MaterialDescription = rc.Fields.Item("Dscription").Value.ToString(),
                        Amount = Convert.ToDecimal(rc.Fields.Item("Quantity").Value),
                        DeliveryStatus = ChoseDocStatus(
                            rc.Fields.Item("LineStatus").Value.ToString(),
                            rc.Fields.Item("CANCELED").Value.ToString()),
                        ShippingDate = shipdate?.ToString("yyyy-MM-dd"),
                        InvoiceNumber = docType switch {
                            DocType.Order => rc.Fields.Item("OiDocNum").Value.ToString(),
                            //DocType.Delivery => result.DocumentNumber,
                            _ => "",
                        },
                        DeliveryNumber = docType switch {
                            DocType.Order => rc.Fields.Item("OdDocNum").Value.ToString(),
                            DocType.Delivery => result.DocumentNumber,
                            _ => "",
                        },
                        OrderNumber = docType switch {
                            DocType.Order => result.DocumentNumber,
                            DocType.Delivery => rc.Fields.Item("OrDocNum").Value.ToString(),
                            _ => "",
                        },
                        CustomerOrderNumber = docType switch {
                            DocType.Order => result.CustomerDocumentNumber,
                            DocType.Delivery => rc.Fields.Item("OrNumAtCard").Value.ToString(),
                            _ => "",
                        },
                        OrderDate = docType switch {
                            DocType.Order => result.DocumentDate,
                            DocType.Delivery => ((DateTime?)rc.Fields.Item("OrCreateDate").Value)?.ToString("yyyy-MM-dd"),
                            _ => "",
                        },
                        OrderedAmount = docType switch {
                            DocType.Order => Convert.ToDecimal(rc.Fields.Item("Quantity").Value),
                            DocType.Delivery => Convert.ToDecimal(rc.Fields.Item("R1Quantity").Value),
                            _ => 0,
                        },
                        OrderPosition = Convert.ToInt32(docType switch {
                            DocType.Order => rc.Fields.Item("LineNum").Value.ToString(),
                            DocType.Delivery => rc.Fields.Item("R1LineNum").Value.ToString(),
                            _ => "0",
                        }) + 1,
                    });
                }
            } catch(Exception ex) {
                Logger.Error(ex.ToString());
            } finally {
                rc?.ReleaseComObject();
            }
            return result;
        }

        public enum DocType {
            Unknown,
            Order,
            Delivery,
            Invoice,
            CreditNote,
            InvoiceAndCreditNote
        }
        private static DocStatus ChoseDocStatus(string statusFlag, string cancelFlag, string lineStatusFlag = null) {
            if(cancelFlag == "Y")
                return DocStatus.CANCELLED;
            var result = statusFlag switch {
                "C" => DocStatus.SHIPPED,
                "O" => DocStatus.OPEN,
                _ => DocStatus.PREPARATION // ?
            };
            if(lineStatusFlag == null)
                return result;
            return lineStatusFlag switch {
                "C" => DocStatus.SHIPPED,
                "O" => result,
                _ => DocStatus.PREPARATION // ?
            };
        }

        private static readonly Dictionary<DocType, string> SearchQuery = new Dictionary<DocType, string> {
            { DocType.Order, @"
SELECT DISTINCT
    ORDR.DocNum, 
    ORDR.NumAtCard, 
    ORDR.CreateDate, 
    ORDR.DocDueDate, 
    ORDR.CANCELED, 
    ORDR.DocStatus,
    ORDR.ObjType
FROM ORDR
    INNER JOIN OCRD on OCRD.CardCode = ORDR.CardCode
    LEFT JOIN OCPR on OCRD.CardCode = OCPR.CardCode
    INNER JOIN RDR1 on RDR1.DocEntry = ORDR.DocEntry
    INNER JOIN OITM on RDR1.ItemCode = OITM.ItemCode and ('{{MaterialNumber}}' = '' or OITM.U_CodesSTD = '{{MaterialNumber}}')
-- Livraison dans le cas d'une commande
    LEFT JOIN DLN1 LD1 on 'RDR' = 'RDR' 
        and LD1.BaseType = 17 
        and LD1.BaseEntry = RDR1.DocEntry 
        and LD1.BaseLine = RDR1.LineNum
    LEFT JOIN ODLN LOD on LOD.DocEntry = LD1.DocEntry
-- Facture dans le cas d'une commande
    LEFT JOIN INV1 LI1 on 'RDR' = 'RDR' 
        and LI1.BaseType = 15
        and LI1.BaseEntry = LD1.DocEntry 
        and LI1.BaseLine = LD1.LineNum
    LEFT JOIN OINV LOI on LOI.DocEntry = LI1.DocEntry
WHERE 1 = 1
    AND ORDR.U_AppPL in ('Y', 'D')
-- Date From/To
    and ('{{DateFrom}}' = '' or ORDR.CreateDate >= '{{DateFrom}}')
    and ('{{DateTill}}' = '' or ORDR.CreateDate <= '{{DateTill}}')
-- Document numbers 
    and ('{{DocumentNumber}}' = '' or ORDR.DocNum like '%{{DocumentNumber}}%')
    and ('{{CustomerOrderNumber}}' = '' or ORDR.NumAtCard like '%{{CustomerOrderNumber}}%')
-- Id client
    and ('{{Email}}' = '' or ORDR.Email like '%{{Email}}%' or ORDR.U_EmailWeb like '%{{Email}}%' or OCPR.E_MailL like '%{{Email}}%' or OCRD.E_Mail like '%{{Email}}%')
    and ('{{DebitorNumber}}' = '' or OCRD.CardCode like '%{{DebitorNumber}}%')
-- Status
    and ('{{Status}}' = '' OR '{{Status}}' = 'UNKNOWN'
            or ('{{Status}}' = 'CANCELLED' and ORDR.CANCELED = 'Y')
            or ('{{Status}}' = 'OPEN' and ORDR.DocStatus = 'O' and ORDR.CANCELED = 'N')
            or ('{{Status}}' = 'SHIPPED' and ORDR.DocStatus = 'C' and ORDR.CANCELED = 'N')
    )
-- DeliveryNumber
    and (
            ('RDR' = 'RDR' and ('{{DeliveryNumber}}' = '' or LOD.DocNum = '{{DeliveryNumber}}')) 
            or ('RDR' != 'RDR')
        )
-- InvoiceNumber
    and (
            ('RDR' = 'RDR' and ('{{InvoiceNumber}}' = '' or LOI.DocNum = '{{InvoiceNumber}}')) 
            or ('RDR' != 'RDR')
        )
GROUP BY
    ORDR.DocNum, 
    ORDR.NumAtCard, 
    ORDR.CreateDate, 
    ORDR.DocDueDate, 
    ORDR.CANCELED, 
    ORDR.DocStatus,
    ORDR.ObjType
--ORDER BY CreateDate DESC
" },
            { DocType.Invoice, @"
SELECT DISTINCT
    OINV.DocNum, 
    OINV.NumAtCard, 
    OINV.CreateDate, 
    OINV.DocDueDate, 
    OINV.CANCELED, 
    OINV.DocStatus,
    OINV.ObjType
FROM OINV
    INNER JOIN OCRD on OCRD.CardCode = OINV.CardCode
    LEFT JOIN OCPR on OCRD.CardCode = OCPR.CardCode
    INNER JOIN INV1 on INV1.DocEntry = OINV.DocEntry
    INNER JOIN OITM on INV1.ItemCode = OITM.ItemCode and ('{{MaterialNumber}}' = '' or OITM.U_CodesSTD = '{{MaterialNumber}}')
WHERE 1 = 1
-- Date From/To
    and ('{{DateFrom}}' = '' or OINV.CreateDate >= '{{DateFrom}}')
    and ('{{DateTill}}' = '' or OINV.CreateDate <= '{{DateTill}}')
-- Document numbers 
    and ('{{DocumentNumber}}' = '' or OINV.DocNum like '%{{DocumentNumber}}%')
    and ('{{CustomerOrderNumber}}' = '' or OINV.NumAtCard like '%{{CustomerOrderNumber}}%')
-- Id client
    and ('{{Email}}' = '' or OINV.Email like '%{{Email}}%' or OINV.U_EmailWeb like '%{{Email}}%' or OCPR.E_MailL like '%{{Email}}%' or OCRD.E_Mail like '%{{Email}}%')
    and ('{{DebitorNumber}}' = '' or OCRD.CardCode like '%{{DebitorNumber}}%')
-- Status
    and ('{{Status}}' = '' OR '{{Status}}' = 'UNKNOWN'
            or ('{{Status}}' = 'CANCELLED' and OINV.CANCELED = 'Y')
            or ('{{Status}}' = 'OPEN' and OINV.DocStatus = 'O' and OINV.CANCELED = 'N')
            or ('{{Status}}' = 'SHIPPED' and OINV.DocStatus = 'C' and OINV.CANCELED = 'N')
    )
GROUP BY
    OINV.DocNum, 
    OINV.NumAtCard, 
    OINV.CreateDate, 
    OINV.DocDueDate, 
    OINV.CANCELED, 
    OINV.DocStatus,
    OINV.ObjType
--ORDER BY CreateDate DESC
" },
            { DocType.CreditNote, @"
SELECT DISTINCT
    ORIN.DocNum, 
    ORIN.NumAtCard, 
    ORIN.CreateDate, 
    ORIN.DocDueDate, 
    ORIN.CANCELED, 
    ORIN.DocStatus,
    ORIN.ObjType
FROM ORIN
    INNER JOIN OCRD on OCRD.CardCode = ORIN.CardCode
    LEFT JOIN OCPR on OCRD.CardCode = OCPR.CardCode
    INNER JOIN RIN1 on RIN1.DocEntry = ORIN.DocEntry
    INNER JOIN OITM on RIN1.ItemCode = OITM.ItemCode and ('{{MaterialNumber}}' = '' or OITM.U_CodesSTD = '{{MaterialNumber}}')
WHERE 1 = 1
-- Date From/To
    and ('{{DateFrom}}' = '' or ORIN.CreateDate >= '{{DateFrom}}')
    and ('{{DateTill}}' = '' or ORIN.CreateDate <= '{{DateTill}}')
-- Document numbers 
    and ('{{DocumentNumber}}' = '' or ORIN.DocNum like '%{{DocumentNumber}}%')
    and ('{{CustomerOrderNumber}}' = '' or ORIN.NumAtCard like '%{{CustomerOrderNumber}}%')
-- Id client
    and ('{{Email}}' = '' or ORIN.Email like '%{{Email}}%' or ORIN.U_EmailWeb like '%{{Email}}%' or OCPR.E_MailL like '%{{Email}}%' or OCRD.E_Mail like '%{{Email}}%')
    and ('{{DebitorNumber}}' = '' or OCRD.CardCode like '%{{DebitorNumber}}%')
-- Status
    and ('{{Status}}' = '' OR '{{Status}}' = 'UNKNOWN'
            or ('{{Status}}' = 'CANCELLED' and ORIN.CANCELED = 'Y')
            or ('{{Status}}' = 'OPEN' and ORIN.DocStatus = 'O' and ORIN.CANCELED = 'N')
            or ('{{Status}}' = 'SHIPPED' and ORIN.DocStatus = 'C' and ORIN.CANCELED = 'N')
    )
GROUP BY
    ORIN.DocNum, 
    ORIN.NumAtCard, 
    ORIN.CreateDate, 
    ORIN.DocDueDate, 
    ORIN.CANCELED, 
    ORIN.DocStatus,
    ORIN.ObjType
--ORDER BY CreateDate DESC
" },
            { DocType.Delivery, @"
SELECT DISTINCT
    ODLN.DocNum, 
    ODLN.NumAtCard, 
    ODLN.CreateDate, 
    ODLN.DocDueDate, 
    ODLN.CANCELED, 
    ODLN.DocStatus,
    ODLN.ObjType
FROM ODLN
    INNER JOIN OCRD on OCRD.CardCode = ODLN.CardCode
    LEFT JOIN OCPR on OCRD.CardCode = OCPR.CardCode
    INNER JOIN DLN1 on DLN1.DocEntry = ODLN.DocEntry
    INNER JOIN OITM on DLN1.ItemCode = OITM.ItemCode and ('{{MaterialNumber}}' = '' or OITM.U_CodesSTD = '{{MaterialNumber}}')
WHERE 1 = 1
-- Date From/To
    and ('{{DateFrom}}' = '' or ODLN.CreateDate >= '{{DateFrom}}')
    and ('{{DateTill}}' = '' or ODLN.CreateDate <= '{{DateTill}}')
-- Document numbers 
    and ('{{DocumentNumber}}' = '' or ODLN.DocNum like '%{{DocumentNumber}}%')
    and ('{{CustomerOrderNumber}}' = '' or ODLN.NumAtCard like '%{{CustomerOrderNumber}}%')
-- Id client
    and ('{{Email}}' = '' or ODLN.Email like '%{{Email}}%' or ODLN.U_EmailWeb like '%{{Email}}%' or OCPR.E_MailL like '%{{Email}}%' or OCRD.E_Mail like '%{{Email}}%')
    and ('{{DebitorNumber}}' = '' or OCRD.CardCode like '%{{DebitorNumber}}%')
-- Status
    and ('{{Status}}' = '' OR '{{Status}}' = 'UNKNOWN'
            or ('{{Status}}' = 'CANCELLED' and ODLN.CANCELED = 'Y')
            or ('{{Status}}' = 'OPEN' and ODLN.DocStatus = 'O' and ODLN.CANCELED = 'N')
            or ('{{Status}}' = 'SHIPPED' and ODLN.DocStatus = 'C' and ODLN.CANCELED = 'N')
    )
GROUP BY
    ODLN.DocNum, 
    ODLN.NumAtCard, 
    ODLN.CreateDate, 
    ODLN.DocDueDate, 
    ODLN.CANCELED, 
    ODLN.DocStatus,
    ODLN.ObjType
--ORDER BY CreateDate DESC
" },
        };
        public static DocumentSearchOutputWrapper Search(Company DIAPI, DocumentSearchInput search) {
            var result = new List<DocumentSearchOutput>();
            Recordset rc = null;
            try {
                rc = DIAPI.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;
                DocType extra = DocType.Unknown;
                if(search.DocType == DocType.InvoiceAndCreditNote) {
                    extra = DocType.CreditNote;
                    search.DocType = DocType.Invoice;
                }
                var query = SearchQuery[search.DocType];
                if(extra != DocType.Unknown) {
                    query += " UNION ALL " + SearchQuery[extra];
                }
                query = query
                    .Replace("{{DocumentNumber}}", search.DocType switch {
                        DocType.Order => search.OrderNumber,
                        DocType.Invoice => search.InvoiceNumber,
                        DocType.Delivery => search.DeliveryNumber,
                        DocType.CreditNote => search.InvoiceNumber,
                        _ => search.OrderNumber // peux potentiellement poser pb comme défaut ?
                    })//
                    /*.Replace("{{Table}}", search.DocType switch {
                        DocType.Order => "RDR",
                        DocType.Invoice => "INV",
                        DocType.Delivery => "DLN",
                        _ => "ORDR" // peux potentiellement poser pb comme défaut ?
                    })*/
                    .Replace("{{DebitorNumber}}", search.DebitorNumber)//
                    .Replace("{{Email}}", search.Email)//
                    .Replace("{{DateFrom}}", search.DateFrom)//
                    .Replace("{{DateTill}}", search.DateTill)//
                    .Replace("{{OrderNumber}}", search.OrderNumber)
                    .Replace("{{CustomerOrderNumber}}", search.CustomerOrderNumber)//
                    .Replace("{{DeliveryNumber}}", search.DeliveryNumber)
                    .Replace("{{InvoiceNumber}}", search.InvoiceNumber)
                    .Replace("{{MaterialNumber}}", search.MaterialNumber)//
                    .Replace("{{Status}}", search.Status.ToString());
                rc.Query(query);
                for(int i = 0, n = rc.RecordCount; i < n; i++, rc.MoveNext()) {
                    result.Add(new DocumentSearchOutput {
                        DocumentNumber = rc.Fields.Item("DocNum").Value.ToString(),
                        DocumentType = rc.Fields.Item("ObjType").Value.ToString() switch {
                            "14" => DocType.CreditNote,
                            "17" => DocType.Order,
                            "15" => DocType.Delivery,
                            "13" => DocType.Invoice,
                            _ => search.DocType,
                        },
                        CustomerDocumentNumber = rc.Fields.Item("NumAtCard").Value.ToString(),
                        CreationDate = ((DateTime?)rc.Fields.Item("CreateDate").Value)?.ToString("yyyy-MM-dd"),
                        ShippingDate = ((DateTime?)rc.Fields.Item("DocDueDate").Value)?.ToString("yyyy-MM-dd"),
                        Status = ChoseDocStatus(
                            rc.Fields.Item("DocStatus").Value.ToString(),
                            rc.Fields.Item("CANCELED").Value.ToString()),
                    });
                }
            } catch(Exception ex) {
                Logger.Error(ex.ToString());
            } finally {
                rc?.ReleaseComObject();
            }

            return new DocumentSearchOutputWrapper {
                items = result.ToArray()
            };
        }

        public static async Task<string> Pdf(Company company, DocType docType, string docNum, CoresuitePdf config) {
            var docEntry = "";
            {
                var table = docType switch {
                    DocType.Order => "RDR",
                    DocType.Invoice => "INV",
                    DocType.Delivery => "DLN",
                    DocType.CreditNote => "RIN",
                    _ => "RDR" // peux potentiellement poser pb comme défaut ?
                };
                Recordset rc = null;
                try {
                    rc = company.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;
                    rc.Query("SELECT DocEntry from O{{TABLE}} where DocNum = '{{DocNum}}'"
                        .Replace("{{TABLE}}", table)
                        .Replace("{{DocNum}}", docNum));
                    docEntry = rc?.Fields?.Item("DocEntry")?.Value?.ToString();
                    /*} catch(Exception ex) {
                        throw;*/
                } finally {
                    if(rc != null) {
                        GC.WaitForPendingFinalizers();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(rc);
#pragma warning disable IDE0059 // Assignation inutile d'une valeur
                        rc = null;
#pragma warning restore IDE0059 // Assignation inutile d'une valeur
                    }
                }

                if(string.IsNullOrEmpty(docEntry) || docEntry == "0") {
                    return null;
                }

                var filepath = Path.Combine(
                    config.DossierPdf,
                    config.FilenamePattern
                        .Replace("{{DocEntry}}", docEntry)
                        .Replace("{{DocNum}}", docNum)
                );

                if(!File.Exists(filepath)) {
                    switch(config.Type) {
                        case PdfType.CORESUITE: {
                                var xmlFilepath = Path.Combine(config.DossierXml, docType + "_" + docEntry + ".xml");
                                Logger.Warn("Generation PDF via " + xmlFilepath);

                                var xmlRequest = @"
<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<job>
    <type mode=""PDF"" layoutid=""0"" formtype=""{{FormType}}""/>
    <paramsets>
        <paramset>
            <params>
                <param key=""DocEntry"" value=""{{DocEntry}}""/>
                <param key=""PrintDef"" value=""{{PrintDef}}""/>
            </params>
        </paramset>
    </paramsets>
</job>".Trim()
                                    .Replace("{{DocEntry}}", docEntry)
                                    .Replace("{{PrintDef}}", config.PrintDef)
                                    .Replace("{{FormType}}", docType switch {
                                        DocType.Order => "139",
                                        DocType.Delivery => "140",
                                        DocType.Invoice => "133",
                                        DocType.CreditNote => "179",
                                        _ => "139", //?
                                    });

                                File.WriteAllText(xmlFilepath, xmlRequest);
                            }
                            break;
                        case PdfType.CRYSTALREPORTS: {
                                System.Diagnostics.Process process = new System.Diagnostics.Process {
                                    StartInfo = new System.Diagnostics.ProcessStartInfo {
                                        WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                                        FileName = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "CrystalReportsGenerator.exe"),
                                        Arguments = "\"" + config.Rpt + "\" " + docEntry + " \"" + filepath + "\""
                                    }
                                };
                                process.Start();
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    int retries = 0;
                    const int retryDelay = 250;
                    const int maxDelay = 30 * 1000;
                    const int maxReties = maxDelay / retryDelay; // rety every maxDelay sec for at maximum maxDelay sec total

                    while(retries < maxReties && !File.Exists(filepath)) {
                        await Task.Delay(retryDelay);
                        retries++;
                    }

                    if(retries >= maxReties) {
                        Logger.Error(filepath + " not found ; pdf generation failed");
                        throw new Exception("File not available");
                    }

                    await Task.Delay(1000); // attendre une seconde de plus si le fichier vient juste d'être généré
                }

                return filepath;
            }
        }
    }
}
