using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace Custom.Application.Extension
{
    public static class ServiceExtensionCustom
    {
        public static IServiceCollection AddApplicationServicesCustom(this IServiceCollection services)
        {
            services.AddApplicationControllersCustom();

            return services;
        }

        private static IServiceCollection AddApplicationControllersCustom(this IServiceCollection services)
        {
            TypeInfo info = typeof(IAppControllerCustom).GetTypeInfo();

            var types = info.Assembly.GetTypes().Where(d => info.IsAssignableFrom(d)).Where(d => d.IsClass && !d.IsAbstract && d.IsPublic);

            foreach (var type in types)
            {
                services.AddScoped(type);
            }

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
