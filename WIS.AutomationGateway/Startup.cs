using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Web;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using WIS.AutomationGateway.Extensions;

namespace WIS.AutomationGateway
{
    public class Startup
    {
        Logger _logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddOcelot(Configuration).AddPolly();
            services.AddSwaggerForOcelot(Configuration);
            services.AddMvc();

            services.Configure<IISServerOptions>(options =>
                options.MaxRequestBodySize = Configuration.GetValue<long>("AppSettings:MaxRequestBodySize")
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
            app.UseSwaggerForOcelotUI(null, c =>
            {
                c.InjectJavascript("/script/jquery-3.6.3.min.js");
                c.InjectJavascript("/script/jquery.initialize.min.js");
                c.InjectJavascript("/script/translate/es.js");
                c.InjectJavascript("/script/translate/translate.js");
                c.InjectJavascript("/script/rapipdf-min.js");
                c.InjectJavascript("/script/custom.js");
            });
            app.UseOcelot().Wait();
            app.ConfigureExceptionHandler(_logger);
        }
    }
}
