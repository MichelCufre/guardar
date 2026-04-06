using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Domain.Validation;
using WIS.Persistence.Database;
using WIS.Security;

namespace WIS.Domain.Services
{
    public class StockService : IStockService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IValidationService _validationService;
        protected readonly IOptions<MaxItemsSettings> _configuration;
        protected readonly IIdentityService _identity;
        protected readonly ITaskQueueService _taskQueue;

        public StockService(IUnitOfWorkFactory uowFactory, IValidationService validationService, IOptions<MaxItemsSettings> configuration, IIdentityService identity, ITaskQueueService taskQueue)
        {
            _uowFactory = uowFactory;
            _validationService = validationService;
            _configuration = configuration;
            _identity = identity;
            _taskQueue = taskQueue;
        }

        #region Consulta de stock

        public virtual async Task<StockValidationsResult> GetStock(FiltrosStock filtros, string loginName, int empresaEjecucion)
        {
            var result = new StockValidationsResult();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (filtros != null)
                {
                    var userId = uow.SecurityRepository.GetUserIdByLoginName(loginName) ?? 0;

                    var context = await GetNewServiceContext(uow, filtros, userId, empresaEjecucion);
                    var validaciones = _validationService.ValidateFiltrosStock(uow, filtros, context, out bool errorProcedimiento);

                    if (validaciones.Count > 0)
                    {
                        var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                        result.ValidationsResult.Errors.Add(new ValidationsError(1, errorProcedimiento, messages));
                    }
                }

                if (result.ValidationsResult.HasError())
                    return result;

                result.StockResult = uow.StockRepository.ConsultaStock(filtros, _configuration.Value.PageSizeStock);
            }

            return result;
        }

        public virtual async Task<IStockServiceContext> GetNewServiceContext(IUnitOfWork uow, FiltrosStock filtros, int userId, int empresaEjecucion)
        {
            var context = new StockServiceContext(uow, filtros, userId, empresaEjecucion);

            AddParametros(context, empresaEjecucion);

            await context.Load();

            return context;
        }

        public virtual void AddParametros(IStockServiceContext context, int empresaEjecucion)
        {
        }

        #endregion

        #region Ajuste

        public virtual async Task<ValidationsResult> ProcesarAjuste(List<AjusteStock> ajustes, int userId)
        {
            var result = new ValidationsResult();
            var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                int nroRegistro = 1;

                if (ajustes.Count > 0)
                {
                    uow.CreateTransactionNumber(this._identity.Application);

                    var keys = new HashSet<string>();

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, ajustes.Count, _configuration.Value.AjustesStock))
                        return result;

                    var context = await GetNewServiceContext(uow, ajustes, userId);

                    foreach (var ajuste in ajustes)
                    {
                        validaciones.Clear();
                        ajuste.NuTransaccion = uow.GetTransactionNumber();
                        ajuste.Aplicacion = this._identity.Application;

                        string key = $"{ajuste.Ubicacion}.{ajuste.Producto}.{ajuste.Empresa}.{ajuste.Identificador}.{ajuste.Faixa.ToString("#.###")}";

                        if (keys.Contains(key))
                        {
                            validaciones.Add(new Error("WMSAPI_msg_Error_AjusteDuplicado", ajuste.Ubicacion, ajuste.Producto, ajuste.Empresa, ajuste.Identificador, ajuste.Faixa));
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                            break;
                        }
                        else
                            keys.Add(key);

                        validaciones.AddRange(await _validationService.ValidateAjusteStock(ajuste, context, out bool errorProcedimiento));

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, errorProcedimiento, messages));
                        }

                        nroRegistro++;
                    }

                    if (result.HasError())
                        return result;

                    List<string> keysAjustes = new List<string>();
                    uow.AjusteRepository.AddAjustesStock(ajustes, context, out keysAjustes);

                    if (keysAjustes.Any() && _taskQueue.IsEnabled())
                        _taskQueue.Enqueue("API", CInterfazExterna.AjustesDeStock, keysAjustes);
                }
                else
                {
                    var msg = Translator.Translate(uow, new Error("WMSAPI_msg_Error_ListaDeObjetosRequerida"), _identity.UserId);
                    result.Errors.Add(new ValidationsError(nroRegistro, false, new List<string>() { msg }));
                }
            }

            return result;
        }

        public virtual async Task<IAjustesDeStockServiceContext> GetNewServiceContext(IUnitOfWork uow, List<AjusteStock> ajustes, int userId)
        {
            var empresa = ajustes[0].Empresa;
            var context = new AjustesDeStockServiceContext(uow, ajustes, userId, empresa);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        public virtual void AddParametros(IAjustesDeStockServiceContext context, int empresa)
        {
        }

        #endregion

        #region Transferencia

        public virtual async Task<ValidationsResult> ProcesarTransferencia(List<TransferenciaStock> transferencias, int userId)
        {
            var result = new ValidationsResult();
            var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (transferencias.Count > 0)
                {
                    int nroRegistro = 1;
                    var keys = new HashSet<string>();

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, transferencias.Count, _configuration.Value.TransferenciaStock))
                        return result;

                    var context = await GetNewServiceContext(uow, transferencias, userId);

                    foreach (var tr in transferencias)
                    {
                        validaciones.Clear();

                        string key = $"{tr.Ubicacion}.{tr.UbicacionDestino}.{tr.Empresa}.{tr.Producto}.{tr.Identificador}.{tr.Faixa.ToString("#.###")}";

                        if (keys.Contains(key))
                        {
                            validaciones.Add(new Error("WMSAPI_msg_Error_TransferenciaDuplicada", tr.Ubicacion, tr.UbicacionDestino, tr.Empresa, tr.Producto, tr.Identificador, tr.Faixa));
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                            break;
                        }
                        else
                            keys.Add(key);

                        validaciones.AddRange(await _validationService.ValidateTransferencia(tr, context, out bool errorProcedimiento));

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, errorProcedimiento, messages));
                        }

                        nroRegistro++;
                    }

                    if (result.HasError())
                        return result;

                    //Validaciones en conjunto de cantidades

                    var errores = await _validationService.ValidateTransferenciaSaldos(transferencias, context);
                    if (errores.Count > 0)
                    {
                        var messages = Translator.Translate(uow, errores, _identity.UserId);
                        result.Errors.Add(new ValidationsError(nroRegistro, false, messages));
                    }

                    if (result.HasError())
                        return result;

                    await uow.StockRepository.TransferirStock(transferencias, context);
                }
            }

            return result;
        }

        public virtual async Task<ITransferenciaStockServiceContext> GetNewServiceContext(IUnitOfWork uow, List<TransferenciaStock> transferencias, int userId)
        {
            var empresa = transferencias[0].Empresa;
            var context = new TransferenciaStockServiceContext(uow, transferencias, userId, empresa);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        public virtual void AddParametros(ITransferenciaStockServiceContext context, int empresa)
        {
        }

        #endregion

        #region Transferencia Automatismo

        public virtual async Task<ValidationsResult> ProcesarTransferenciaAutomatismo(List<TransferenciaStock> transferencias, int userId)
        {
            var result = new ValidationsResult();
            var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (transferencias.Count > 0)
                {
                    int nroRegistro = 1;
                    var keys = new HashSet<string>();

                    if (transferencias.Count > _configuration.Value.TransferenciaStock)
                    {
                        string msg = $"La cantidad de items enviados no puede superar el máximo de {_configuration.Value.TransferenciaStock}.";
                        result.Errors.Add(new ValidationsError(nroRegistro, false, new List<string>() { msg }));
                        return result;
                    }

                    var context = await GetNewServiceContext(uow, transferencias, userId);

                    foreach (var tr in transferencias)
                    {

                        string key = $"{tr.Ubicacion}.{tr.UbicacionDestino}.{tr.Empresa}.{tr.Producto}.{tr.Identificador}.{tr.Faixa.ToString("#.###")}";

                        if (keys.Contains(key))
                        {
                            validaciones.Add(new Error("WMSAPI_msg_Error_TransferenciaDuplicada", tr.Ubicacion, tr.UbicacionDestino, tr.Empresa, tr.Producto, tr.Identificador, tr.Faixa));
                        }
                        else
                            keys.Add(key);

                        validaciones.AddRange(await _validationService.ValidateTransferencia(tr, context, out bool errorProcedimiento));

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, errorProcedimiento, messages));
                        }

                        nroRegistro++;
                    }

                    if (result.HasError())
                        return result;

                    //Validaciones en conjunto de cantidades

                    var errores = await _validationService.ValidateTransferenciaSaldos(transferencias, context);
                    if (errores.Count > 0)
                    {
                        var messages = Translator.Translate(uow, errores, _identity.UserId);
                        result.Errors.Add(new ValidationsError(nroRegistro, false, messages));
                    }

                    if (result.HasError())
                        return result;

                    await uow.StockRepository.TransferirStockAutomatismo(transferencias, context);
                }
            }

            return result;
        }

        #endregion

    }
}
