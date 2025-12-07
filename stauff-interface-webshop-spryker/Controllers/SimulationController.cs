using Microsoft.AspNetCore.Mvc;
using stauff_interface_webshop_spryker_ui.DataContrats.In;
using stauff_interface_webshop_spryker_ui.DataContrats.Out;
using stauff_interface_webshop_spryker_ui.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using stauff_interface_webshop_spryker_ui;
using SAPbobsCOM;
using System.Xml.Serialization;
using System.IO;
using stauff_interface_webshop_spryker_ui.Extensions;
using System.Text.Json;

namespace stauff_interface_webshop_spryker.Controllers {
    [ApiController]
    [Route("[controller]")]
    public sealed class SimulationController : ControllerBase {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        static readonly MainConfiguration config = MainConfiguration.LoadStatic();

        [HttpPost]
        public SimulationReturn Post(Order order) {
            var dt = DateTime.Now;

            try {
                Directory.CreateDirectory(Path.Combine(config.PathToTrace, this.GetType().Name));
                System.IO.File.WriteAllText(Path.Combine(config.PathToTrace, this.GetType().Name, dt.ToString("yyyyMMdd.HHmmss.fffffff") + "-in.json"), JsonSerializer.Serialize(order));
            } catch(Exception e) {
                Logger.Warn("TRACE: Une erreur c'est produite lors de la sauvegarde de la requête:\r\n" + e.ToString());
            }

            var @return = order.Simulation(DIAPI.GetDIAPI(), config);

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
