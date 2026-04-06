using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;
using WIS.Security;

namespace WIS.Domain.Services
{
    public class UbicacionService : IUbicacionService
    {
        protected readonly IValidationService _validationService;
        protected readonly IOptions<MaxItemsSettings> _configuration;
        protected readonly IIdentityService _identity;

        public UbicacionService(IValidationService validationService, IOptions<MaxItemsSettings> configuration, IIdentityService identity)
        {
            _validationService = validationService;
            _configuration = configuration;
            _identity = identity;
        }

        public virtual async Task<ValidationsResult> ImportarUbicaciones(List<UbicacionExterna> ubicaciones, IUnitOfWork uow)
        {
            var serviceContext = await GetNewServiceContext(uow, ubicaciones);

            var result = await ValidarUbicaciones(uow, ubicaciones, serviceContext);

            if (!result.HasError()) 
                uow.UbicacionRepository.AgregarUbicacionesExternas(ubicaciones, serviceContext);

            return result;
        }

        public virtual async Task<ValidationsResult> ValidarUbicaciones(IUnitOfWork uow, List<UbicacionExterna> ubicaciones, IUbicacionServiceContext serviceContext)
        {
            var result = new ValidationsResult();
            var validaciones = new List<Error>();
            var nroRegistro = 1;
            var maxItems = _configuration.Value.ImportarUbicacion;

            if (!_validationService.ValidateMaxItems(result, nroRegistro, ubicaciones.Count, maxItems)) 
                return result;

            foreach (var ubicacion in ubicaciones)
            {
                validaciones.AddRange(await _validationService.ValidateUbicacionImportada(ubicacion, serviceContext, out bool errorProcedimiento));

                if (validaciones.Count > 0)
                {
                    var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                    result.Errors.Add(new ValidationsError(nroRegistro, errorProcedimiento, messages));
                    validaciones = [];
                }

                nroRegistro++;
            }

            return result;
        }

        public virtual async Task<IUbicacionServiceContext> GetNewServiceContext(IUnitOfWork uow, List<UbicacionExterna> registros)
        {
            var serviceContext = new UbicacionServiceContext(uow, registros, _identity.UserId);

            AddParameters(serviceContext);

            await serviceContext.Load();

            return serviceContext;
        }

        public virtual void AddParameters(IUbicacionServiceContext serviceContext)
        {
            serviceContext.AddParametro(ParamManager.LISTA_CARACTERES_COD_BARRA);
            serviceContext.AddParametro(ParamManager.PREFIJO_EQUIPO_FUN);
            serviceContext.AddParametro(ParamManager.PREFIJO_EQUIPO);
            serviceContext.AddParametro(ParamManager.PREFIJO_CLASIFICACION);
            serviceContext.AddParametro(ParamManager.PREFIJO_AUTOMATISMO);
            serviceContext.AddParametro(ParamManager.PREFIJO_PUERTA);
            serviceContext.AddParametro(ParamManager.PREFIJO_PRODUCCION);
        }
    }
}
