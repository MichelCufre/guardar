using Custom.Domain.DataModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using WIS.Domain.DataModel;

namespace Custom.WMS_API.Extensions
{
    public static class ServiceExtensionsCustom 
    {
        public static IServiceCollection AddApplicationServicesCustom(this IServiceCollection services, IConfiguration configuration)
        {
            services.RemoveScoped<IUnitOfWorkFactory>();
            services.AddScoped<IUnitOfWorkFactory, UnitOfWorkCustomFactory>();

            services.ApplicationServices();
            services.AddDtoMappers();

            return services;
        }

        private static IServiceCollection ApplicationServices(this IServiceCollection services)
        {
            return services;
        }

        private static IServiceCollection AddDtoMappers(this IServiceCollection services)
        {
            return services;
        }


        public static IServiceCollection RemoveScoped<T>(this IServiceCollection services)
        {
            var serviceDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(T));
            if (serviceDescriptor != null) services.Remove(serviceDescriptor);

            return services;
        }

    }
}