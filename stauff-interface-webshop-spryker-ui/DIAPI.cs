using stauff_interface_webshop_spryker_ui;
using stauff_interface_webshop_spryker_ui.Configuration;
using stauff_interface_webshop_spryker_ui.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace stauff_interface_webshop_spryker_ui {
    public static class DIAPI {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        static MainConfiguration mainConfiguration = MainConfiguration.LoadStatic();
        public static string DIAPI_LOCK { get; set; } = "";
        static SAPbobsCOM.Company CompDI = null;
        static DateTime DateTime = DateTime.MinValue;
        public static void SetMainConfiguration(MainConfiguration MainConfiguration) {
            mainConfiguration = MainConfiguration;
        }
        public static SAPbobsCOM.Company GetDIAPI() {
            if(CompDI != null && (DateTime.Now - DateTime).TotalDays > 1) {
                DisconnectAndClean();
            }
            if(CompDI != null && !CompDI.Connected) {
                DisconnectAndClean();
            }
            if(CompDI != null && CompDI.Connected) {
                try {
                    var rc = CompDI.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    rc.ReleaseComObject();
                } catch(Exception e) {
                    Logger.Warn(e);
                    DisconnectAndClean();
                }
            }

            if(CompDI == null) {
                DateTime = DateTime.Now;
                CompDI = new SAPbobsCOM.Company();

                Logger.Debug("UserName: " + mainConfiguration.UserName);
                CompDI.UserName = mainConfiguration.UserName;
                CompDI.Password = mainConfiguration.Password;

                Logger.Debug("Server: " + mainConfiguration.Server);
                CompDI.Server = mainConfiguration.Server;

                if(!string.IsNullOrWhiteSpace(mainConfiguration.SLDServer)) {
                    Logger.Debug("SLDServer: " + mainConfiguration.SLDServer);
                    CompDI.SLDServer = mainConfiguration.SLDServer;
                }

                Logger.Debug("DbServerType: " + ((SAPbobsCOM.BoDataServerTypes)mainConfiguration.DbServerType));
                CompDI.DbServerType = (SAPbobsCOM.BoDataServerTypes)mainConfiguration.DbServerType;

                Logger.Debug("CompanyDB: " + mainConfiguration.CompanyDB);
                CompDI.CompanyDB = mainConfiguration.CompanyDB;

                CompDI.UseTrusted = false;

                CompDI.XmlExportType = SAPbobsCOM.BoXmlExportTypes.xet_ExportImportMode;
                //CompDI.language = SAPbobsCOM.BoSuppLangs.ln_English;
                CompDI.language = SAPbobsCOM.BoSuppLangs.ln_French;

                if(CompDI.Connect() != 0) {
                    throw new Exception("La connexion à SBO à échoué : " + CompDI.GetLastErrorDescription());
                }
            }

            return CompDI;
        }

        public static void DisconnectAndClean() {
            if(CompDI != null && CompDI.Connected) {
                try {
                    CompDI.Disconnect();
                } catch(Exception e) {
                    Logger.Warn(e.ToString());
                }
            }
            if(CompDI != null) {
                CompDI.ReleaseComObject();
                CompDI = null;
            }
        }
    }
}
