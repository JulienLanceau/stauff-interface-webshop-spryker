using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAPbobsCOM;
using stauff_interface_webshop_spryker_ui;
using stauff_interface_webshop_spryker_ui.Configuration;
using stauff_interface_webshop_spryker_ui.DataContrats.Out;
using stauff_interface_webshop_spryker_ui.Extensions;
using System;
using System.IO;
using System.Text.Json;

namespace stauff_interface_webshop_spryker.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class ItemsPricesController : ControllerBase {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        static readonly MainConfiguration config = MainConfiguration.LoadStatic();

        private static string default_id = null;

        [HttpGet]
        [HttpGet("{id}")]
        public ItemsPriceList Get(string id = null) {
            if(string.IsNullOrWhiteSpace(id)) {
                if(default_id == null) {
                    var rc = DIAPI.GetDIAPI().GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;
                    rc.Query($@"
SELECT ListNum
FROM OCRD
where CardCode = '{config.CodeClientDummy}'
");
                    rc.MoveFirst();
                    default_id = rc.Fields.Item("ListNum").Value.ToString();
                }
                id = default_id;
            }
            var dt = DateTime.Now;

            var @return =  Actions.PriceLists(DIAPI.GetDIAPI(), config, Convert.ToInt32(id));

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
