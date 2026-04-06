using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;
using WIS.Security;

namespace WIS.Domain.Services
{
    public class EgresoService : IEgresoService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IValidationService _validationService;
        protected readonly IOptions<MaxItemsSettings> _configuration;
        protected readonly IIdentityService _identity;

        public EgresoService(IUnitOfWorkFactory uowFactory,
            IValidationService validationService,
            IOptions<MaxItemsSettings> configuration,
            IIdentityService identity)
        {
            _uowFactory = uowFactory;
            _validationService = validationService;
            _configuration = configuration;
            _identity = identity;
        }

        public virtual async Task<Camion> GetCamion(int cdCamion)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return await uow.CamionRepository.GetCamionOrNull(cdCamion, true);
            }
        }

        public virtual async Task<Camion> GetCamionByIdExterno(string idExterno, int empExterna)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return await uow.CamionRepository.GetCamionByIdExterno(idExterno, empExterna, true);
            }
        }

        public virtual async Task<ValidationsResult> AgregarEgresos(int empresa, List<Camion> egresos, int userId)
        {
            var result = new ValidationsResult();
            var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                    int nroRegistro = 1;

                if (egresos.Count > 0)
                {
                    int countDetalles = 0;
                    var keys = new HashSet<string>();
                    int maxItems = _configuration.Value.Egreso;

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, egresos.Count, maxItems))
                        return result;

                    var context = await GetNewServiceContext(uow, empresa, egresos, userId);

                    foreach (var egreso in egresos)
                    {
                        validaciones.Clear();

                        if (!string.IsNullOrEmpty(egreso.IdExterno))
                        {
                            if (keys.Contains(egreso.IdExterno))
                            {
                                validaciones.Add(new Error("WMSAPI_msg_Error_EgresoIdentificadoresDuplicados", egreso.IdExterno));
                                var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                                result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                                break;
                            }
                            else
                                keys.Add(egreso.IdExterno);
                        }

                        validaciones.AddRange(await _validationService.ValidateEgreso(egreso, context, out bool errorProcedimiento));

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, errorProcedimiento, messages));
                        }

                        countDetalles += egreso.GetCountPedidos();
                        countDetalles += egreso.GetCountCargas();
                        countDetalles += egreso.GetCountContenedores();

                        nroRegistro++;
                    }

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, countDetalles, maxItems, false))
                        return result;

                    if (result.HasError())
                        return result;

                    await uow.CamionRepository.AddEgresos(egresos, context);
                }
                else
                {
                    var msg = Translator.Translate(uow, new Error("WMSAPI_msg_Error_ListaDeObjetosRequerida"), _identity.UserId);
                    result.Errors.Add(new ValidationsError(nroRegistro, false, new List<string>() { msg }));
                }
            }

            return result;
        }

        public virtual async Task<IEgresoServiceContext> GetNewServiceContext(IUnitOfWork uow, int empresaEjecucion, List<Camion> egresos, int userId)
        {
            var context = new EgresoServiceContext(uow, egresos, userId, empresaEjecucion);

            AddParametros(context, empresaEjecucion);

            await context.Load();

            return context;
        }

        public virtual void AddParametros(IEgresoServiceContext context, int empresaEjecucion)
        {
            context.AddParametroEmpresa(ParamManager.CD_TRANSPORTADORA_DEFAULT, empresaEjecucion);
        }

    }
}
