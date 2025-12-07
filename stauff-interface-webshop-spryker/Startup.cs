using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using stauff_interface_webshop_spryker_ui.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace stauff_interface_webshop_spryker {
    class Startup {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        static string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public IConfiguration Configuration { get; }
        public static MainConfiguration MainConfiguration { get; set; }
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services) {
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();
            var a = services.AddControllers()
                .AddJsonOptions(opts => {
                    var enumConverter = new JsonStringEnumConverter();
                    opts.JsonSerializerOptions.Converters.Add(enumConverter);
                });
            /*foreach(var f in ModuleHelper.ListModulesFiles()) {
                try {
                    a.AddApplicationPart(Assembly.LoadFile(f));
                    Logger.Info("Loaded " + f);
                } catch(Exception ex) {
                    Logger.Error("Could not load " + f + " " + ex.Message);
                }
            }*/
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if(/*env.IsDevelopment()*/System.Diagnostics.Debugger.IsAttached) {
                app.UseDeveloperExceptionPage();
            }

            // Fire and forget task pour faire le premier login di api en parallel
            Task.Run(() => {
                stauff_interface_webshop_spryker_ui.DIAPI.GetDIAPI();
            }).ConfigureAwait(false);

            app.UseExceptionHandler(errorApp => {
                errorApp.Run(async context => {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    Logger.Error("Path:" + exceptionHandlerPathFeature?.Path + " ; Message:" + exceptionHandlerPathFeature?.Error?.Message);

                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        Success = false,
                        Message = exceptionHandlerPathFeature?.Error?.Message,
                        Path = exceptionHandlerPathFeature?.Path,
                    }));

                    /*await context.Response.WriteAsync("<html lang=\"en\"><body>\r\n");
                    await context.Response.WriteAsync("ERROR!<br><br>\r\n");

                   

                    if (exceptionHandlerPathFeature?.Error is FileNotFoundException) {
                        await context.Response.WriteAsync(
                                                  "File error thrown!<br><br>\r\n");
                    }

                    await context.Response.WriteAsync(
                                                  "<a href=\"/\">Home</a><br>\r\n");
                    await context.Response.WriteAsync("</body></html>\r\n");
                    await context.Response.WriteAsync(new string(' ', 512));*/
                });
            });

            app.UseAuthentication();
            app.UseAuthorization();

            var cookiePolicyOptions = new CookiePolicyOptions {
                MinimumSameSitePolicy = SameSiteMode.Strict,
            };
            app.UseCookiePolicy(cookiePolicyOptions);

            app.UseDefaultFiles();
            /*
            Directory.CreateDirectory(Path.Combine(BaseDirectory, "static"));
            app.UseStaticFiles(new StaticFileOptions {
                FileProvider = new PhysicalFileProvider(Path.Combine(BaseDirectory, "static")),
                RequestPath = "",
                ServeUnknownFileTypes = true,
                OnPrepareResponse = ctx => {
                    ctx.Context.Response.Headers.Remove("ETag");
                    ctx.Context.Response.Headers.Remove("Last-Modified");
                    //ctx.Context.Response.Headers.Append("Cache-Control", "private,max-age=100");
                    /*const int durationInSeconds = 60 * 60 * 24;
                    ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.CacheControl] =
                        "public,max-age=" + durationInSeconds;*
                    var date = File.GetLastWriteTimeUtc(ctx.File.PhysicalPath);
                    ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.ETag] =
                        date.Ticks.ToString();
                    ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.LastModified] =
                        date.ToUniversalTime().ToString("R");
                }
            });
    */
            /*
            foreach(var a in MainConfiguration.StaticFiles) {
                var b = a;
                if(b.StartsWith("/"))
                    b = b.Substring(1);
                var c = b;
                b = Path.Combine(BaseDirectory, b);
                Directory.CreateDirectory(b);
                var options = new DefaultFilesOptions();
                options.DefaultFileNames.Clear();
                options.DefaultFileNames.Add("index.html");
                app.UseStaticFiles(new StaticFileOptions {
                    FileProvider = new PhysicalFileProvider(b),
                    RequestPath = "/" + c,
                    ServeUnknownFileTypes = true,
                    OnPrepareResponse = ctx => {
                        ctx.Context.Response.Headers.Remove("ETag");
                        ctx.Context.Response.Headers.Remove("Last-Modified");
                        //ctx.Context.Response.Headers.Append("Cache-Control", "private,max-age=100");
                        /*const int durationInSeconds = 60 * 60 * 24;
                        ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.CacheControl] =
                            "public,max-age=" + durationInSeconds;*
                        var date = File.GetLastWriteTimeUtc(ctx.File.PhysicalPath);
                        ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.ETag] =
                            date.Ticks.ToString();
                        ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.LastModified] =
                            date.ToUniversalTime().ToString("R");
                    }
                }).UseDefaultFiles(options);
            }
*/
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
