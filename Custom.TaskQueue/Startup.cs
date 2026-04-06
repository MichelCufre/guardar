using Custom.Domain.DataModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WIS.Domain.DataModel;

namespace Custom.TaskQueue
{
    public class Startup : WIS.TaskQueue.Startup
    {

        public Startup(IConfiguration configuration) : base(configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
        }

        public override void AddUnitOfWorkFactoryService(IServiceCollection services)
        {
            services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkCustomFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            base.Configure(app, env);
        }
    }
}
