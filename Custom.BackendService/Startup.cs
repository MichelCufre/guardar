using Custom.Application.Extension;
using Custom.BackendService.Services;
using Custom.Domain.DataModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WIS.Domain.DataModel;
using WIS.Translation;

namespace Custom.BackendService
{
    public class Startup : WIS.BackendService.Startup
    {

        public Startup(IConfiguration configuration) : base(configuration)
        {
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddScoped<ITranslationResourceProvider, TranslationResourceCustomProvider>();
            services.RemoveScoped<IUnitOfWorkFactory>();
            services.AddScoped<IUnitOfWorkFactory, UnitOfWorkCustomFactory>();

            services.AddApplicationServicesCustom();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            base.Configure(app, env);
        }
    }
}
