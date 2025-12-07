using Microsoft.AspNetCore.Mvc;
using stauff_interface_webshop_spryker_ui.Configuration;
using stauff_interface_webshop_spryker_ui.DataContrats.In;
using stauff_interface_webshop_spryker_ui.DataContrats.Out;
using stauff_interface_webshop_spryker_ui.Extensions;
using stauff_interface_webshop_spryker_ui;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Threading.Tasks;

namespace stauff_interface_webshop_spryker.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class DeliveryController : ControllerBase {
        //private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        static readonly MainConfiguration config = MainConfiguration.LoadStatic();

        [HttpGet]
        [Route("Pdf/{docnum}")]
        public async Task<IActionResult> Pdf(string docnum) {
            var filepath = await DocumentSearch.Pdf(DIAPI.GetDIAPI(), DocumentSearch.DocType.Delivery, docnum, config.CoresuiteDeliveryPdf);
            return new PhysicalFileResult(filepath, "application/pdf");
        }
        [HttpPost]
        [Route("detail")]
        public DocumentDetailOutput Detail(DocumentDetailInput search) {
            return DocumentSearch.Detail(DIAPI.GetDIAPI(), DocumentSearch.DocType.Delivery, search);
        }

        [HttpPost]
        [Route("search")]
        public DocumentSearchOutputWrapper Search(DocumentSearchInput search) {
            if(search.DocType == DocumentSearch.DocType.Unknown)
                search.DocType = DocumentSearch.DocType.Delivery;
            return DocumentSearch.Search(DIAPI.GetDIAPI(), search);
        }
    }
}
