using Microsoft.AspNetCore.Mvc;
using stauff_interface_webshop_spryker_ui.Configuration;
using stauff_interface_webshop_spryker_ui.DataContrats.In;
using stauff_interface_webshop_spryker_ui.DataContrats.Out;
using stauff_interface_webshop_spryker_ui.Extensions;
using stauff_interface_webshop_spryker_ui;
using System.Threading.Tasks;

namespace stauff_interface_webshop_spryker.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class InvoiceController : Controller {
        //private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        static readonly MainConfiguration config = MainConfiguration.LoadStatic();

        [HttpGet]
        [Route("Pdf/{docnum}")]
        public async Task<IActionResult> Pdf(string docnum) {
            var filepath = await DocumentSearch.Pdf(DIAPI.GetDIAPI(), DocumentSearch.DocType.Invoice, docnum, config.CoresuiteInvoicePdf);
            if(string.IsNullOrEmpty(filepath)) {
                filepath = await DocumentSearch.Pdf(DIAPI.GetDIAPI(), DocumentSearch.DocType.CreditNote, docnum, config.CoresuiteCreditNotePdf);
            }
            return new PhysicalFileResult(filepath, "application/pdf");
        }

        [HttpPost]
        [Route("detail")]
        public DocumentDetailOutput Detail(DocumentDetailInput search) {
            return DocumentSearch.Detail(DIAPI.GetDIAPI(), DocumentSearch.DocType.Invoice, search);
        }

        [HttpPost]
        [Route("search")]
        public DocumentSearchOutputWrapper Search(DocumentSearchInput search) {
            if(search.DocType == DocumentSearch.DocType.Unknown)
                search.DocType = DocumentSearch.DocType.InvoiceAndCreditNote;
            return DocumentSearch.Search(DIAPI.GetDIAPI(), search);
        }
    }
}
