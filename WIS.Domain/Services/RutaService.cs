using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Tracking.Models;
using WIS.Domain.Tracking.Validation;
using WIS.Domain.Validation;
using WIS.Security;

namespace WIS.Domain.Services.Interfaces
{
    public class RutaService : IRutaService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IValidationService _validationService;
        protected readonly IIdentityService _identity;

        public RutaService(IUnitOfWorkFactory uowFactory, IValidationService validationService, IIdentityService identity)
        {
            _uowFactory = uowFactory;
            _validationService = validationService;
            _identity = identity;
        }

        public virtual async Task<Ruta> GetRutaByZona(string zona)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return await uow.RutaRepository.GetRutaByZona(zona);
            }
        }

        public virtual async Task<TrackingValidationResult> AddRutaByZona(Ruta ruta, string loginName)
        {
            var result = new TrackingValidationResult();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var userId = uow.SecurityRepository.GetUserIdByLoginName(loginName) ?? 0;

                var validaciones = await _validationService.ValidarRutaZona(ruta, true);

                if (validaciones.Count > 0)
                {
                    var errores = Translator.Traducir(uow, validaciones, userId);
                    result.Errors.AddRange(errores);
                }

                if (result.HasError())
                    return result;

                ruta.Id = (short)(uow.RutaRepository.GetUltimaRuta() + 1);
                await uow.RutaRepository.AddRutaByZona(ruta);
            }

            return result;
        }

        public virtual async Task<TrackingValidationResult> UpdateRutaByZona(Ruta ruta, string loginName)
        {
            var result = new TrackingValidationResult();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var userId = uow.SecurityRepository.GetUserIdByLoginName(loginName) ?? 0;

                var validaciones = await _validationService.ValidarRutaZona(ruta, false);

                if (validaciones.Count > 0)
                {
                    var errores = Translator.Traducir(uow, validaciones, userId);
                    result.Errors.AddRange(errores);
                }

                if (result.HasError())
                    return result;

                await uow.RutaRepository.UpdateRutaByZona(ruta);
            }

            return result;
        }
    }
}
