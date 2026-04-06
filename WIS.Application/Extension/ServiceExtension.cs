using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WIS.Application.File;
using WIS.Application.GridConfiguration;
using WIS.Application.Invocation;
using WIS.Application.Report;
using WIS.Application.Security;
using WIS.Application.Setup;
using WIS.Application.Validation;
using WIS.Application.Webhook;
using WIS.Domain.Automatismo.Factories;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.DataModel;
using WIS.Domain.Facturacion;
using WIS.Domain.Produccion.Factories;
using WIS.Domain.Produccion.Interfaces;
using WIS.Domain.Produccion.Logic;
using WIS.Domain.Recepcion.RecepcionAgendamiento;
using WIS.Domain.Reportes;
using WIS.Domain.Services;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Tracking;
using WIS.Domain.Tracking.Config;
using WIS.Filtering;
using WIS.Filtering.Expressions;
using WIS.FormComponent.Execution;
using WIS.GridComponent.Build;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Execution;
using WIS.Http;
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;
using WIS.TrafficOfficer;
using WIS.Translation;

namespace WIS.Application.Extension
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddApplicationControllers();

            services.AddTranslation();
            services.AddSecurity();
            services.AddInvocation();

            services.AddPageComponent();
            services.AddForm();
            services.AddGrid();

            services.AddScoped<IApplicationSetupService, ApplicationSetupService>();

            services.AddScoped<ISessionAccessor, SessionAccessor>();
            services.AddScoped<IFactoryService, FactoryService>();
            services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

            services.AddScoped<IWebApiClient, WebApiClient>();

            services.AddScoped<ITrafficOfficerService, TrafficOfficerService>();
            services.AddScoped<ITrafficOfficerSessionManager, TrafficOfficerSessionManager>();

            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IReportDownloadService, ReportDownloadService>();
            services.AddScoped<IBarcodeService, BarcodeService>();
            services.AddScoped<IReportKeyService, ReportKeyService>();
            services.AddScoped<IParameterService, ParameterService>();
            services.AddScoped<IPrintingService, PrintingService>();
            services.AddScoped<IReportGeneratorService, ReportGeneratorService>();

            services.AddScoped<IWmsApiService, WmsApiService>();
            services.AddScoped<IWebhookService, WebhookService>();
            services.AddScoped<ITaskQueueService, TaskQueueService>();
            services.AddScoped<IAPITrackingService, APITrackingService>();
            services.AddScoped<IWebhookCallerService, WebhookCallerService>();
            services.AddScoped<ITrackingConfigProvider, TrackingConfigProvider>();
            services.AddScoped<IDeshacerEmbarqueServiceLegacy, DeshacerEmbarqueServiceLegacy>();
            services.AddScoped<IValidacionFacturacionResultFormatResolver, ValidacionFacturacionResultFormatResolver>();

            services.AddScoped<IStockService, StockService>();
            services.AddScoped<IGrupoService, GrupoService>();
            services.AddScoped<IAgenteService, AgenteService>();
            services.AddScoped<IPedidoService, PedidoService>();
            services.AddScoped<IEmpresaService, EmpresaService>();
            services.AddScoped<IProductoService, ProductoService>();
            services.AddScoped<IDocumentoService, DocumentoService>();
            services.AddScoped<IEjecucionService, EjecucionService>();
            services.AddScoped<IRecepcionService, RecepcionService>();
            services.AddScoped<IValidationService, ValidationService>();
            services.AddScoped<IFacturacionService, FacturacionService>();
            services.AddScoped<IAutomatismoFactory, AutomatismoFactory>();
            services.AddScoped<ICodigoBarrasService, CodigoBarrasService>();
            services.AddScoped<IPickingProductoService, PickingProductoService>();
            services.AddScoped<IAlmacenamientoService, AlmacenamientoService>();
            services.AddScoped<IReferenciaRecepcionService, ReferenciaRecepcionService>();
            services.AddScoped<IAutomatismoPtlClientService, AutomatismoPtlClientService>();
            services.AddScoped<IAutomatismoAutoStoreClientService, AutomatismoAutoStoreClientService>();
            services.AddScoped<IUbicacionService, UbicacionService>();
            services.AddScoped<IRecorridoService, RecorridoService>();
			       
			services.AddScoped<ILogicaProduccionFactory, LogicaProduccionFactory>();
			services.AddScoped<IIngresoProduccionFactory, IngresoProduccionFactory>();
            services.AddScoped<IEspacioProduccionFactory, EspacioProduccionFactory>();

            services.AddScoped<IEspacioProduccionLogic, EspacioProduccionLogic>();

            services.AddScoped<ICodigoMultidatoHandlerFactory, CodigoMultidatoHandlerFactory>();
            services.AddScoped<ICodigoMultidatoService, CodigoMultidatoService>();
            services.AddScoped<IFacturaService, FacturaService>();

            return services;
        }

        public static IServiceCollection AddApplicationControllers(this IServiceCollection services)
        {
            TypeInfo info = typeof(IAppController).GetTypeInfo();

            var types = info.Assembly.GetTypes().Where(d => info.IsAssignableFrom(d)).Where(d => d.IsClass && !d.IsAbstract && d.IsPublic);

            foreach (var type in types)
            {
                services.AddScoped(type);
            }

            return services;
        }
        public static IServiceCollection AddGrid(this IServiceCollection services)
        {
            services.AddFiltering();
            services.AddSorting();

            services.AddScoped<IGridCellValueParsingService, GridCellValueParsingService>();
            services.AddScoped<IGridCoordinator, GridCoordinator>();
            services.AddScoped<IGridActionResolver, GridActionResolver>();
            services.AddScoped<IGridConfigService, GridConfigService>();
            services.AddScoped<IGridExcelService, GridExcelService>();
            services.AddScoped<IGridExcelBuildService, GridExcelBuildService>();
            services.AddScoped<IGridService, GridService>();
            services.AddScoped<IGridValidationService, GridValidationService>();

            return services;
        }
        public static IServiceCollection AddForm(this IServiceCollection services)
        {
            services.AddScoped<IFormActionResolver, FormActionResolver>();
            services.AddScoped<IFormCoordinator, FormCoordinator>();
            services.AddScoped<IFormValidationService, FormValidationService>();

            return services;
        }
        public static IServiceCollection AddSecurity(this IServiceCollection services)
        {
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<IUserProvider, UserProvider>();

            return services;
        }
        public static IServiceCollection AddPageComponent(this IServiceCollection services)
        {
            services.AddScoped<IPageCoordinator, PageCoordinator>();
            services.AddScoped<IPageActionResolver, PageActionResolver>();

            return services;
        }
        public static IServiceCollection AddFiltering(this IServiceCollection services)
        {
            services.AddScoped<IExpressionService, ExpressionService>();
            services.AddTransient<IFilterTokenizer, FilterTokenizer>();
            services.AddScoped<IFilterParser, FilterParser>();
            services.AddScoped<IFilterInterpreter, FilterInterpreter>();

            return services;
        }
        public static IServiceCollection AddSorting(this IServiceCollection services)
        {
            services.AddScoped<ISortingService, SortingService>();

            return services;
        }
        public static IServiceCollection AddInvocation(this IServiceCollection services)
        {
            services.AddScoped<IGridControllerInvocation, GridControllerInvocation>();
            services.AddScoped<IFormControllerInvocation, FormControllerInvocation>();
            services.AddScoped<IPageControllerInvocation, PageControllerInvocation>();

            return services;
        }
        public static IServiceCollection AddTranslation(this IServiceCollection services)
        {
            services.AddScoped<IEqualityComparer<TranslatedValue>, TranslationResourceComparer>();
            services.AddScoped<ITranslationResourceMerger, TranslationResourceMerger>();

            return services;
        }
    }
}
