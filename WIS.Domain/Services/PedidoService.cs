using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Tracking.Models;
using WIS.Domain.Tracking.Validation;
using WIS.Domain.Validation;
using WIS.Security;

namespace WIS.Domain.Services
{
    public class PedidoService : IPedidoService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IValidationService _validationService;
        protected readonly IOptions<MaxItemsSettings> _configuration;
        protected readonly IIdentityService _identity;

        public PedidoService(IUnitOfWorkFactory uowFactory,
            IValidationService validationService,
            IOptions<MaxItemsSettings> configuration,
            IIdentityService identity)
        {
            _uowFactory = uowFactory;
            _validationService = validationService;
            _configuration = configuration;
            _identity = identity;
        }

        #region WMS

        public virtual async Task<Pedido> GetPedido(string nuPedido, int empresa, string tipoAgente, string codigoAgente)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return await uow.PedidoRepository.GetPedidoOrNull(nuPedido, empresa, tipoAgente, codigoAgente);
            }
        }

        public virtual async Task<ValidationsResult> AgregarPedidos(List<Pedido> pedidos, int userId)
        {
            var result = new ValidationsResult();
            var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                int nroRegistro = 1;

                if (pedidos.Count > 0)
                {
                    uow.CreateTransactionNumber(this._identity.Application);

                    int countDetalles = 0;
                    var keys = new HashSet<string>();
                    int maxItems = _configuration.Value.Pedido;

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, pedidos.Count, maxItems))
                        return result;

                    var context = await GetNewServiceContext(uow, pedidos, userId);
                    short.TryParse(context.GetParametro(ParamManager.IE_503_CD_ROTA), out short rutaParam);

                    foreach (var pedido in pedidos)
                    {
                        var rutaAgente = uow.AgenteRepository.GetAgente(pedido.Empresa, pedido.CodigoAgente)?.Ruta?.Id;

                        if (pedido.Ruta == null)
                            pedido.Ruta = rutaAgente ?? rutaParam;

                        validaciones.Clear();

                        pedido.Transaccion = uow.GetTransactionNumber();

                        string key = $"{pedido.Id}.{pedido.Empresa}.{pedido.TipoAgente}.{pedido.CodigoAgente}";

                        if (keys.Contains(key))
                        {
                            validaciones.Add(new Error("WMSAPI_msg_Error_PedidosDuplicados", pedido.Id, pedido.Empresa, pedido.TipoAgente, pedido.CodigoAgente));
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                            break;
                        }
                        else
                            keys.Add(key);

                        validaciones.AddRange(await _validationService.ValidatePedido(pedido, context, out bool errorProcedimiento));

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, errorProcedimiento, messages));
                        }

                        countDetalles += pedido.Lineas.Count;
                        nroRegistro++;
                    }

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, countDetalles, maxItems, false))
                        return result;

                    if (result.HasError())
                        return result;

                    await uow.PedidoRepository.AddPedidos(pedidos, context);
                }
                else
                {
                    var msg = Translator.Translate(uow, new Error("WMSAPI_msg_Error_ListaDeObjetosRequerida"), _identity.UserId);
                    result.Errors.Add(new ValidationsError(nroRegistro, false, new List<string>() { msg }));
                }
            }

            return result;
        }

        public virtual async Task<IPedidoServiceContext> GetNewServiceContext(IUnitOfWork uow, List<Pedido> pedidos, int userId)
        {
            var empresa = pedidos[0].Empresa;
            var context = new PedidoServiceContext(uow, pedidos, userId, empresa);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        public virtual void AddParametros(IPedidoServiceContext context, int empresa)
        {
            context.AddParametro(ParamManager.IE_503_CD_CONDICION_LIBERACION);
            context.AddParametro(ParamManager.IE_503_CD_ROTA);
            context.AddParametro(ParamManager.IE_503_CD_TRANSPORTADORA);
            context.AddParametro(ParamManager.IE_503_TP_EXPEDICION);
            context.AddParametro(ParamManager.IE_503_TP_PEDIDO);
            context.AddParametroEmpresa(ParamManager.IE_503_HAB_DUPLICADOS, empresa);
            context.AddParametroEmpresa(ParamManager.IE_503_HAB_LPN, empresa);
            context.AddParametroEmpresa(ParamManager.IE_503_HAB_ATRIBUTOS, empresa);
            context.AddParametroEmpresa(ParamManager.IE_503_VALIDAR_FECHAS, empresa);

        }

        public virtual async Task<ValidationsResult> ModificarPedidos(List<Pedido> pedidos, int userId)
        {
            var result = new ValidationsResult();
            var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                int nroRegistro = 1;

                if (pedidos.Count > 0)
                {
                    uow.CreateTransactionNumber(this._identity.Application);

                    int countDetalles = 0;
                    var keys = new HashSet<string>();
                    int maxItems = _configuration.Value.ModificarPedido;

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, pedidos.Count, maxItems))
                        return result;

                    var context = await ModificarPedidoNewServiceContext(uow, pedidos, userId);

                    foreach (var pedido in pedidos)
                    {
                        validaciones.Clear();

                        pedido.Transaccion = uow.GetTransactionNumber();

                        string key = $"{pedido.Id}.{pedido.Empresa}.{pedido.TipoAgente}.{pedido.CodigoAgente}";

                        if (keys.Contains(key))
                        {
                            validaciones.Add(new Error("WMSAPI_msg_Error_PedidosDuplicados", pedido.Id, pedido.Empresa, pedido.TipoAgente, pedido.CodigoAgente));
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                            break;
                        }
                        else
                            keys.Add(key);

                        validaciones.AddRange(await _validationService.ValidateModificarPedido(pedido, context, out bool errorProcedimiento));

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, errorProcedimiento, messages));
                        }

                        countDetalles += pedido.Lineas.Count;
                        nroRegistro++;
                    }

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, countDetalles, maxItems, false))
                        return result;

                    if (result.HasError())
                        return result;

                    await uow.PedidoRepository.ModificarPedidos(pedidos, context);
                }
                else
                {
                    var msg = Translator.Translate(uow, new Error("WMSAPI_msg_Error_ListaDeObjetosRequerida"), _identity.UserId);
                    result.Errors.Add(new ValidationsError(nroRegistro, false, new List<string>() { msg }));
                }
            }

            return result;
        }

        public virtual async Task<IModificarPedidoServiceContext> ModificarPedidoNewServiceContext(IUnitOfWork uow, List<Pedido> pedidos, int userId)
        {
            var empresa = pedidos[0].Empresa;
            var context = new ModificarPedidoServiceContext(uow, pedidos, userId, empresa);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        public virtual void AddParametros(IModificarPedidoServiceContext context, int empresa)
        {
            context.AddParametro(ParamManager.IE_503_CD_CONDICION_LIBERACION);
            context.AddParametro(ParamManager.IE_503_CD_ROTA);
            context.AddParametro(ParamManager.IE_503_CD_TRANSPORTADORA);
            context.AddParametro(ParamManager.IE_503_TP_EXPEDICION);
            context.AddParametro(ParamManager.IE_503_TP_PEDIDO);
            context.AddParametroEmpresa(ParamManager.IE_503_HAB_DUPLICADOS, empresa);
            context.AddParametroEmpresa(ParamManager.IE_503_HAB_LPN, empresa);
            context.AddParametroEmpresa(ParamManager.IE_503_HAB_ATRIBUTOS, empresa);
            context.AddParametroEmpresa(ParamManager.IE_503_VALIDAR_FECHAS, empresa);
        }

        #endregion

        #region Tracking

        public virtual async Task<TrackingValidationResult> ActualizarPedidosPuntoEntrega(PuntoEntregaAgentes puntoEntrega, string loginName)
        {
            var result = new TrackingValidationResult();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var userId = uow.SecurityRepository.GetUserIdByLoginName(loginName) ?? 0;
                var context = await GetPuntoEntregaServiceContext(uow, puntoEntrega, userId);

                var validaciones = await _validationService.ValidarPuntoEntregaAgente(puntoEntrega, context);

                if (validaciones.Count > 0)
                {
                    var errores = Translator.Traducir(uow, validaciones, userId);
                    result.Errors.AddRange(errores);
                }

                if (result.HasError())
                    return result;

                await uow.PedidoRepository.ActualizarPedidosPuntoEntrega(puntoEntrega, context);
            }

            return result;
        }

        public virtual async Task<IPuntoEntregaServiceContext> GetPuntoEntregaServiceContext(IUnitOfWork uow, PuntoEntregaAgentes puntoEntrega, int userId)
        {
            var context = new PuntoEntregaServiceContext(uow, puntoEntrega, userId);

            AddParametros(context);

            await context.Load();

            return context;
        }

        public virtual void AddParametros(IPuntoEntregaServiceContext context)
        {
        }

        #endregion
    }
}
