using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using stauff_interface_webshop_spryker_ui.Configuration;
using System;
using Topshelf;


namespace stauff_interface_webshop_spryker {
    class Program {

        static readonly MainConfiguration mainConfiguration = MainConfiguration.LoadStatic();
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        static void Main(string[] args) {
            var config = new NLog.Config.LoggingConfiguration();

            var logfile = new NLog.Targets.FileTarget("logfile") {
                FileName = "${basedir}/Logs/lastest.log",
                ArchiveFileName = "${basedir}/Logs/{#######}.log",
                ArchiveNumbering = NLog.Targets.ArchiveNumberingMode.Date,
                ArchiveEvery = NLog.Targets.FileArchivePeriod.Day,
                MaxArchiveDays = 30
            };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, logconsole);
            config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, logfile);

            NLog.LogManager.Configuration = config;

            AppDomain.CurrentDomain.UnhandledException +=
                    (object sender, UnhandledExceptionEventArgs arg) => {
                        Exception e = (Exception)arg.ExceptionObject;
                        Logger.Error(e, "Runtime terminating: " + arg.IsTerminating);
                    };

            Logger.Debug(mainConfiguration.ServiceName);
            Logger.Debug(mainConfiguration.ServiceName.Replace(" ", "_"));

            var hostbuilder = CreateHostBuilder(args);
            Microsoft.Extensions.Hosting.IHost host;
            var rc = HostFactory.Run(x => {
                x.Service<IHost>(s => {
                    s.ConstructUsing(name => {
                        host = hostbuilder.Build();
                        return host;
                    });
                    s.WhenStarted(tc => {
                        Logger.Info("Starting");
                        tc.Start();
                        Logger.Info("Started");
                    });
                    s.WhenStopped(tc => {
                        Logger.Info("Stopping");
                        tc.StopAsync().Wait();
                        Logger.Info("Stopped");
                    });
                });

                x.RunAsLocalSystem();
                x.StartAutomaticallyDelayed();
                x.UseNLog();

#if DEBUG
                if(System.Diagnostics.Debugger.IsAttached) {
                    mainConfiguration.ServiceName = "ERT SAV WEBSERVICE DEBUG";
                }
#endif

                x.SetDescription(mainConfiguration.ServiceName);
                x.SetDisplayName(mainConfiguration.ServiceName);
                x.SetServiceName(mainConfiguration.ServiceName.Replace(" ", "_"));
                x.SetInstanceName(mainConfiguration.ServiceName.Replace(" ", "_"));

                x.EnableServiceRecovery(r => {
                    r.RestartService(0);
                    r.RestartService(0);
                    r.RestartService(0);

                    r.SetResetPeriod(0);
                });

                x.OnException(ex => {
                    Logger.Error(ex);
                });

                /*x.BeforeInstall(x => {
                });
                x.AfterInstall(x => {
                });*/
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host
                .CreateDefaultBuilder(args)

                .ConfigureWebHostDefaults(webBuilder => {
                    Startup.MainConfiguration = mainConfiguration;
                    webBuilder.UseUrls(mainConfiguration.URLs);
                    webBuilder.UseStartup<Startup>();
                });
    }
}
