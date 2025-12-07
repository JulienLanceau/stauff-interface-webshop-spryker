using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace CrystalReportsGenerator {
    internal static class Program {
        static void Main(string[] args) {
            AppDomain.CurrentDomain.UnhandledException +=
                (object sender, UnhandledExceptionEventArgs arg) => {
                    Exception e = (Exception)arg.ExceptionObject;
                    Console.WriteLine(e.ToString());
                };

            System.Threading.Thread.CurrentThread.CurrentCulture =
            System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("fr-FR");

            try {
                using(ReportDocument report = new ReportDocument()) {
                    report.Load(args[0]);

#if DEBUG
                    const string SERVER = "ERT-W10TMA", DATABASE = "ALFA", UID = "sa", PWD = "Adonix1$";
#else
                    const string SERVER = "172.20.241.169\\SAPB1FRPROD", DATABASE = "SBO_STAUFF_PROD", UID = "B1_FRANCE_DBUSER", PWD = "jQZ2awo$U$";
#endif

                    foreach(IConnectionInfo connection in report.DataSourceConnections) {
                        connection.SetConnection(SERVER, DATABASE, UID, PWD);
                    }

                    foreach(IConnectionInfo connection in report.Subreports.Cast<ReportDocument>().SelectMany(subreport => subreport.DataSourceConnections.Cast<IConnectionInfo>())) {
                        connection.SetConnection(SERVER, DATABASE, UID, PWD);
                    }

                    foreach(Table Table in report.Database.Tables) {
                        TableLogOnInfo crtableLogoninfo = (TableLogOnInfo)Table.LogOnInfo.Clone();
                        crtableLogoninfo.ConnectionInfo.ServerName = SERVER;
                        crtableLogoninfo.ConnectionInfo.DatabaseName = DATABASE;
                        crtableLogoninfo.ConnectionInfo.UserID = UID;
                        crtableLogoninfo.ConnectionInfo.Password = PWD;
                        crtableLogoninfo.ConnectionInfo.AllowCustomConnection = true;
                        crtableLogoninfo.ConnectionInfo.Attributes.Collection.Set("QE_ServerDescription", SERVER);
                        crtableLogoninfo.ConnectionInfo.LogonProperties.Set("Locale Identifier", "1033");
                        Table.ApplyLogOnInfo(crtableLogoninfo);
                    }

                    for(int i = 0; i < report.Subreports.Count; i++) {
                        foreach(Table Table in report.Subreports[i].Database.Tables) {
                            TableLogOnInfo crtableLogoninfo = (TableLogOnInfo)Table.LogOnInfo.Clone();
                            crtableLogoninfo.ConnectionInfo.ServerName = SERVER;
                            crtableLogoninfo.ConnectionInfo.DatabaseName = DATABASE;
                            crtableLogoninfo.ConnectionInfo.UserID = UID;
                            crtableLogoninfo.ConnectionInfo.Password = PWD;
                            crtableLogoninfo.ConnectionInfo.AllowCustomConnection = true;
                            crtableLogoninfo.ConnectionInfo.Attributes.Collection.Set("QE_ServerDescription", SERVER);
                            crtableLogoninfo.ConnectionInfo.LogonProperties.Set("Locale Identifier", "1033");
                            Table.ApplyLogOnInfo(crtableLogoninfo);
                        }
                    }

                    report.SetParameterValue("DocKey@", args[1]);

                    //Console.WriteLine("Export RPT modifié vers " + args[2] + ".rpt");
                    //report.SaveAs(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), args[2] + ".rpt"));

                    Console.WriteLine("Export PDF vers " + args[2]);
                    report.ExportToDisk(ExportFormatType.PortableDocFormat, Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), args[2]));
                }
            } catch(Exception ex) { Console.WriteLine(ex.ToString()); }
        }
    }
}
