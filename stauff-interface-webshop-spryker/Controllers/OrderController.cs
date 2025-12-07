using Microsoft.AspNetCore.Mvc;
using SAPbobsCOM;
using stauff_interface_webshop_spryker_ui.DataContrats.In;
using stauff_interface_webshop_spryker_ui.DataContrats.Out;
using stauff_interface_webshop_spryker_ui;
using stauff_interface_webshop_spryker_ui.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using stauff_interface_webshop_spryker_ui.Extensions;
using System.Text.Json;

namespace stauff_interface_webshop_spryker.Controllers {
    [ApiController]
    [Route("[controller]")]
    public sealed class OrderController : ControllerBase {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        static readonly MainConfiguration config = MainConfiguration.LoadStatic();

        [HttpGet]
        [Route("Pdf/{docnum}")]
        public async Task<IActionResult> Pdf(string docnum) {
            var filepath = await DocumentSearch.Pdf(DIAPI.GetDIAPI(), DocumentSearch.DocType.Order, docnum, config.CoresuiteOrderPdf);
            return new PhysicalFileResult(filepath, "application/pdf");
        }

        [HttpPost]
        [Route("bulkDetail")]
        public OrderBulkDetailOutputWrapper Detail(OrderBulkDetailInput search) {
            return DocumentSearch.OrderBulkDetail(DIAPI.GetDIAPI(), search);
        }

        [HttpPost]
        [Route("detail")]
        public DocumentDetailOutput Detail(DocumentDetailInput search) {
            return DocumentSearch.Detail(DIAPI.GetDIAPI(), DocumentSearch.DocType.Order, search);
        }

        [HttpPost]
        [Route("search")]
        public DocumentSearchOutputWrapper Search(DocumentSearchInput search) {
            if(search.DocType == DocumentSearch.DocType.Unknown)
                search.DocType = DocumentSearch.DocType.Order;
            return DocumentSearch.Search(DIAPI.GetDIAPI(), search);
        }

        [HttpPost]
        public OrderReturn Post(Order order) {
            var @return = new OrderReturn();
            var dt = DateTime.Now;

            try {
                Directory.CreateDirectory(Path.Combine(config.PathToTrace, this.GetType().Name));
                System.IO.File.WriteAllText(Path.Combine(config.PathToTrace, this.GetType().Name, dt.ToString("yyyyMMdd.HHmmss.fffffff") + "-in.json"), JsonSerializer.Serialize(order));
            } catch(Exception e) {
                Logger.Warn("TRACE: Une erreur c'est produite lors de la sauvegarde de la requête:\r\n" + e.ToString());
            }

            @return = order.Order(DIAPI.GetDIAPI(), config, (str) => {
                try {
                    Directory.CreateDirectory(Path.Combine(config.PathToTrace, this.GetType().Name));
                    System.IO.File.WriteAllText(Path.Combine(config.PathToTrace, this.GetType().Name, dt.ToString("yyyyMMdd.HHmmss.fffffff") + "-sbo-order.xml"), str);
                } catch(Exception e) {
                    Logger.Warn("TRACE: Une erreur c'est produite lors de la sauvegarde du SBO:\r\n" + e.ToString());
                }
            }, (str) => {
                try {
                    Directory.CreateDirectory(Path.Combine(config.PathToTrace, this.GetType().Name));
                    System.IO.File.WriteAllText(Path.Combine(config.PathToTrace, this.GetType().Name, dt.ToString("yyyyMMdd.HHmmss.fffffff") + "-sbo-downpayment.xml"), str);
                } catch(Exception e) {
                    Logger.Warn("TRACE: Une erreur c'est produite lors de la sauvegarde du SBO:\r\n" + e.ToString());
                }
            }, (str) => {
                try {
                    Directory.CreateDirectory(Path.Combine(config.PathToTrace, this.GetType().Name));
                    System.IO.File.WriteAllText(Path.Combine(config.PathToTrace, this.GetType().Name, dt.ToString("yyyyMMdd.HHmmss.fffffff") + "-sbo-payment.xml"), str);
                } catch(Exception e) {
                    Logger.Warn("TRACE: Une erreur c'est produite lors de la sauvegarde du SBO:\r\n" + e.ToString());
                }
            });

            try {
                Directory.CreateDirectory(Path.Combine(config.PathToTrace, this.GetType().Name));
                System.IO.File.WriteAllText(Path.Combine(config.PathToTrace, this.GetType().Name, dt.ToString("yyyyMMdd.HHmmss.fffffff") + "-out.json"), JsonSerializer.Serialize(@return));
            } catch(Exception e) {
                Logger.Warn("TRACE: Une erreur c'est produite lors de la sauvegarde de la réponse:\r\n" + e.ToString());
            }

            return @return;
        }
    }
}
