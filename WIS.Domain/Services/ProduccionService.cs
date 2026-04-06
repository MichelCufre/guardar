using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Produccion;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;
using WIS.Security;
using WIS.Domain.Produccion.DTOs;

namespace WIS.Domain.Services
{
    public class ProduccionService : IProduccionService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IValidationService _validationService;
        protected readonly IOptions<MaxItemsSettings> _configuration;
        protected readonly IIdentityService _identity;

        public ProduccionService(
            IUnitOfWorkFactory uowFactory,
            IValidationService validationService,
            IOptions<MaxItemsSettings> configuration,
            IIdentityService identity)
        {
            _uowFactory = uowFactory;
            _validationService = validationService;
            _configuration = configuration;
            _identity = identity;
        }

        public virtual async Task<IngresoProduccion> GetProduccion(string nroIngresoProduccion)
        {
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                return await uow.IngresoProduccionRepository.GetProduccionOrNull(nroIngresoProduccion);
            }
        }

        public virtual async Task<ValidationsResult> AgregarIngresos(List<IngresoProduccion> ingresos, int userId, List<IngresosGeneradosApiProduccion> ingresosGenerados)
        {
            var result = new ValidationsResult();
            var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                int nroRegistro = 1;

                if (ingresos.Count > 0)
                {
                    uow.CreateTransactionNumber(this._identity.Application);

                    int countDetalles = 0;
                    var keys = new HashSet<string>();
                    int maxItems = _configuration.Value.Produccion;

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, ingresos.Count, maxItems))
                        return result;

                    var context = await GetNewServiceContext(uow, ingresos, userId);

                    foreach (var ingreso in ingresos)
                    {
                        validaciones.Clear();
                        ingreso.NuTransaccion = uow.GetTransactionNumber();

                        if (!string.IsNullOrEmpty(ingreso.IdProduccionExterno))
                        {
                            if (keys.Contains(ingreso.IdProduccionExterno))
                            {
                                validaciones.Add(new Error("WMSAPI_msg_Error_IdsExternosProduccionDuplicados", ingreso.IdProduccionExterno));
                                var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                                result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                                break;
                            }
                            else
                                keys.Add(ingreso.IdProduccionExterno);
                        }

                        validaciones.AddRange(await _validationService.ValidateIngreso(ingreso, context, out bool errorProcedimiento));

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, errorProcedimiento, messages));
                        }

                        countDetalles += ingreso.Detalles.Count;
                        nroRegistro++;
                    }

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, countDetalles, maxItems))
                        return result;

                    if (result.HasError())
                        return result;

                    await uow.IngresoProduccionRepository.AddIngresos(ingresos, context, ingresosGenerados);
                }
                else
                {
                    var msg = Translator.Translate(uow, new Error("WMSAPI_msg_Error_ListaDeObjetosRequerida"), _identity.UserId);
                    result.Errors.Add(new ValidationsError(nroRegistro, false, new List<string>() { msg }));
                }
            }

            return result;
        }

        public virtual async Task<IProduccionServiceContext> GetNewServiceContext(IUnitOfWork uow, List<IngresoProduccion> ingresos, int userId)
        {
            var empresa = ingresos[0].Empresa.Value;
            var context = new ProduccionServiceContext(uow, ingresos, userId, empresa);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        public virtual void AddParametros(IProduccionServiceContext context, int empresa)
        {
            context.AddParametroEmpresa(ParamManager.MANEJO_DOCUMENTAL, empresa);
        }
    }
}
