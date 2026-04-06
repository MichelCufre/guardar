using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Recorridos;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;
using WIS.Security;

namespace WIS.Domain.Services
{
    public class RecorridoService : IRecorridoService
    {
        protected readonly IValidationService _validationService;
        protected readonly IOptions<MaxItemsSettings> _configuration;
        protected readonly IIdentityService _identity;

        public RecorridoService(IValidationService validationService, IOptions<MaxItemsSettings> configuration, IIdentityService identity)
        {
            _validationService = validationService;
            _configuration = configuration;
            _identity = identity;
        }

        public virtual async Task<ValidationsResult> ImportarDetalles(IUnitOfWork uow, List<DetalleRecorrido> detallesRecorrido, int userId, int empresa)
        {
            var serviceContext = await GetNewServiceContext(uow, detallesRecorrido, userId, empresa);

            var result = await ValidarDetallesRecorrido(uow, detallesRecorrido, serviceContext);

            if (!result.HasError())
                uow.RecorridoRepository.ImportarDetallesRecorrido(serviceContext);

            return result;
        }

        protected virtual async Task<ValidationsResult> ValidarDetallesRecorrido(IUnitOfWork uow, List<DetalleRecorrido> detallesRecorrido, IRecorridoServiceContext serviceContext)
        {
            var result = new ValidationsResult();
            var validaciones = new List<Error>();
            var nroRegistro = 1;
            var maxItems = _configuration.Value.ImportarDetallesRecorrido;

            if (!_validationService.ValidateMaxItems(result, nroRegistro, detallesRecorrido.Count, maxItems))
                return result;

            // Validar primero las bajas y luego las altas ya que se procesan masivamente en ese orden
            detallesRecorrido = detallesRecorrido.OrderByDescending(d => d.TipoOperacion).ToList();

            foreach (var detalle in detallesRecorrido)
            {
                validaciones.AddRange(await _validationService.ValidateDetalleRecorrido(detalle, serviceContext, out bool errorProcedimiento));

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

        protected virtual async Task<IRecorridoServiceContext> GetNewServiceContext(IUnitOfWork uow, List<DetalleRecorrido> registros, int userId, int empresa)
        {
            var serviceContext = new RecorridoServiceContext(uow, registros, userId, empresa);

            await serviceContext.Load();

            return serviceContext;
        }
    }
}
