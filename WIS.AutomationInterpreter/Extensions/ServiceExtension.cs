using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using WIS.AutomationInterpreter.Interfaces;
using WIS.AutomationInterpreter.Models.Mappers;
using WIS.AutomationInterpreter.Models.Mappers.Interfaces;
using WIS.AutomationInterpreter.Services;
using WIS.Domain.DataModel.Mappers.Automatismo;
using AutomatismoMapper = WIS.AutomationInterpreter.Models.Mappers.AutomatismoMapper;

namespace WIS.AutomationInterpreter.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.ApplicationServices();
            services.AddDtoMappers();

            return services;
        }

        private static IServiceCollection ApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<AutomatismoClientService>();
            services.AddScoped<IAutomatismoFactory, AutomatismoFactory>();
            services.AddScoped<IPtlFactory, PtlFactory>();

            return services;
        }

        private static IServiceCollection AddDtoMappers(this IServiceCollection services)
        {
			services.AddScoped<IAutomatismoMapper, AutomatismoMapper>();
			services.AddScoped<IGalysMapper, GalysMapper>();

            return services;
        }
    }
}
