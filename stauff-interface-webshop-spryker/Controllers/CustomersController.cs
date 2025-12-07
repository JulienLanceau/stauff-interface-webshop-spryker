using Microsoft.AspNetCore.Mvc;
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
    public sealed class CustomersController : ControllerBase {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        static readonly MainConfiguration config = MainConfiguration.LoadStatic();

        [HttpGet]
        public Customers Get() {
            var dt = DateTime.Now;

            var @return =  Actions.Customers(DIAPI.GetDIAPI(), config);

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
