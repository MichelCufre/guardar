using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Interfaces;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;
using WIS.Security;
using WIS.TrafficOfficer;

namespace WIS.Domain.Services
{
    public class ConsumirProduccionService : IConsumirProduccionService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IValidationService _validationService;
        protected readonly IOptions<MaxItemsSettings> _configuration;
        protected readonly IIdentityService _identity;
        protected readonly ITaskQueueService _taskQueue;
        protected readonly ILogicaProduccionFactory _logicaProduccionFactory;
        protected readonly ITrafficOfficerService _concurrencyControl;

        public ConsumirProduccionService(IUnitOfWorkFactory uowFactory, IValidationService validationService, IOptions<MaxItemsSettings> configuration, IIdentityService identity, ITaskQueueService taskQueue, ILogicaProduccionFactory logicaProduccionFactory, ITrafficOfficerService concurrencyControl)
        {
            _uowFactory = uowFactory;
            _validationService = validationService;
            _configuration = configuration;
            _identity = identity;
            _taskQueue = taskQueue;
            _logicaProduccionFactory = logicaProduccionFactory;
            _concurrencyControl = concurrencyControl;
        }

        public virtual async Task<ValidationsResult> ProcesarConsumo(ConsumirProduccion consumo, int userId)
        {
            var result = new ValidationsResult();
            _concurrencyControl.CreateToken();
            var transactionTO = _concurrencyControl.CreateTransaccion();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                int nroRegistro = 1;
                var maxItems = _configuration.Value.ConsumirProduccion;

                if (!_validationService.ValidateMaxItems(result, nroRegistro, consumo.Insumos.Count, maxItems))
                    return result;

                var context = await GetNewServiceContext(uow, consumo, userId);

                var validaciones = await _validationService.ValidateConsumirProduccion(consumo, context, out bool errorProcedimiento);

                if (validaciones.Count > 0)
                {
                    var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                    result.Errors.Add(new ValidationsError(nroRegistro, errorProcedimiento, messages));
                }

                if (result.HasError())
                    return result;

                //Validaciones en conjunto de cantidades
                var errores = await _validationService.ValidateSaldosConsumo(consumo, context);
                if (errores.Count > 0)
                {
                    var messages = Translator.Translate(uow, errores, _identity.UserId);
                    result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                }

                if (result.HasError())
                    return result;

                var nuIngresoProducccion = context.GetIngreso().Id;
                try
                {
                    BloquearRegistros(uow, context, result, nroRegistro, validaciones, nuIngresoProducccion, transactionTO);

                    if (result.HasError())
                    {
                        _concurrencyControl.DeleteTransaccion(transactionTO);
                        return result;
                    }

                    await uow.IngresoProduccionRepository.ConsumirProduccion(consumo, context);
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    _concurrencyControl.DeleteTransaccion(transactionTO);
                }

                if (_taskQueue.IsEnabled() && context.KeysAjustes.Any())
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.AjustesDeStock, context.KeysAjustes);

                if (_taskQueue.IsEnabled() && context.NotificarProduccion)
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.ConfirmacionProduccion, nuIngresoProducccion);
            }

            return result;
        }

        public virtual async Task<IConsumirProduccionServiceContext> GetNewServiceContext(IUnitOfWork uow, ConsumirProduccion consumo, int userId)
        {
            var empresa = consumo.Empresa;
            var context = new ConsumirProduccionServiceContext(uow, consumo, userId, _logicaProduccionFactory, empresa);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        public virtual void AddParametros(IConsumirProduccionServiceContext context, int empresa)
        {
        }

        public virtual void BloquearRegistros(IUnitOfWork uow, IConsumirProduccionServiceContext context, ValidationsResult result, int nroRegistro, List<Error> validaciones, string nuIngresoProducccion, TrafficOfficerTransaction transactionTO)
        {
            if (_concurrencyControl.IsLocked("T_PRDC_INGRESO", nuIngresoProducccion, true))
            {
                validaciones.Add(new Error("General_msg_Error_ProduccionBloqueada"));
                var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
            }
            else
            {
                _concurrencyControl.AddLock("T_PRDC_INGRESO", nuIngresoProducccion, transactionTO, true);

                var idsBloqueos = context.GetIdsBloqueos(_identity.GetFormatProvider());
                if (idsBloqueos.Count > 0)
                {

                    var listLock = this._concurrencyControl.GetLockList("T_STOCK", idsBloqueos, transactionTO);

                    if (listLock.Count > 0)
                    {
                        var keyBloqueo = listLock.FirstOrDefault().Id_Bloqueo.Split("#");
                        validaciones.Add(new Error("PRD111ConsumirStock_grid1_msg_RegistrosBloqueados", [keyBloqueo[0], keyBloqueo[1], keyBloqueo[2], keyBloqueo[4]]));
                        var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                        result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                    }
                    else
                        _concurrencyControl.AddLockList("T_STOCK", idsBloqueos, transactionTO, true);
                }
            }
        }
    }
}
