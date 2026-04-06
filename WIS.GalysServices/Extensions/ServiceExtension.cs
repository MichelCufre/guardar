using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WIS.GalysServices.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.ApplicationServices();
            services.AddDtoMappers();;

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
    }
}
