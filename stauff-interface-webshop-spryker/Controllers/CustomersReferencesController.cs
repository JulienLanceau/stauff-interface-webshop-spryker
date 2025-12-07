using Microsoft.AspNetCore.Mvc;
using stauff_interface_webshop_spryker_ui.Configuration;
using stauff_interface_webshop_spryker_ui.Extensions;
using stauff_interface_webshop_spryker_ui;
using System.IO;
using System.Text.Json;
using System;
using System.Collections.Generic;
using SAPbobsCOM;
using System.Linq;
using System.Threading.Tasks;

namespace stauff_interface_webshop_spryker.Controllers {
    public class CustomerMaterialNumber {
        public string custMatNr { get; set; }
        public string matNr { get; set; }
    }

    public class Result {
        public string debitorNumber { get; set; }
        public List<CustomerMaterialNumber> customerMaterialNumbers { get; set; } = new List<CustomerMaterialNumber>();
    }

    public class CustomersReference {
        public List<Result> result { get; set; } = new List<Result>();
    }

    [ApiController]
    [Route("[controller]")]
    public class CustomersReferencesController : ControllerBase {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        static readonly MainConfiguration config = MainConfiguration.LoadStatic();

        [HttpGet]
        [HttpGet("{CardCode}")]
        public CustomersReference Get(string CardCode) {
            var dt = DateTime.Now;
            var result = new CustomersReference();

            Recordset rc = null;
            try {
                rc = DIAPI.GetDIAPI().GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;
                rc.Query(@"
select OSCN.CardCode, /*OSCN.ItemCode*/OITM.U_CodesSTD as ItemCode, OSCN.Substitute
from OSCN WITH (NOLOCK)
inner join OITM WITH (NOLOCK) on OITM.ItemCode = OSCN.ItemCode
inner join OCRD WITH (NOLOCK) on OCRD.CardCode = OSCN.CardCode
where 1=1
    and OITM.SellItem = 'Y' 
    and isnull(OITM.U_CodesSTD,'') != ''
    and OITM.QryGroup64 = 'N'
    and OITM.validFor = 'Y'
    and (OITM.InvntItem = 'Y' or OITM.TreeType = 'S' or OITM.ItmsGrpCod = 186)
    and OCRD.ValidFor = 'Y'
    and OCRD.FrozenFor = 'N'
    and OCRD.CardType = 'C'
    and (OCRD.CardCode = '{{CardCode}}' or '{{CardCode}}' = '')
order by OSCN.CardCode, /*OSCN.ItemCode*/OITM.U_CodesSTD, OSCN.Substitute
".Replace("{{CardCode}}", CardCode));

                var customerDict = new Dictionary<string, Result>();

                for(int i = 0, n = rc.RecordCount; i < n; i++, rc.MoveNext()) {
                    var currentCardCode = rc.Fields.Item("CardCode").Value.ToString();
                    var itemCode = rc.Fields.Item("ItemCode").Value.ToString();
                    var substitute = rc.Fields.Item("Substitute").Value.ToString();

                    if(!customerDict.TryGetValue(currentCardCode, out var customer)) {
                        customer = new Result {
                            debitorNumber = currentCardCode,
                            customerMaterialNumbers = new List<CustomerMaterialNumber>()
                        };
                        customerDict[currentCardCode] = customer;
                        result.result.Add(customer);
                    }

                    customer.customerMaterialNumbers.Add(new CustomerMaterialNumber {
                        custMatNr = substitute,
                        matNr = itemCode
                    });
                }
            } catch(Exception ex) {
                Logger.Error(ex.ToString());
                return null;
            } finally {
                rc.ReleaseComObject();
            }

            Task.Run(() => {
                try {
                    Directory.CreateDirectory(Path.Combine(config.PathToTrace, this.GetType().Name));
                    System.IO.File.WriteAllText(Path.Combine(config.PathToTrace, this.GetType().Name, dt.ToString("yyyyMMdd.HHmmss.fffffff") + "-out.json"), JsonSerializer.Serialize(result));
                } catch(Exception e) {
                    Logger.Warn("TRACE: Une erreur c'est produite lors de la sauvegarde de la réponse:\r\n" + e.ToString());
                }
            });

            return result;
        }
    }
}
