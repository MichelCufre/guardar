using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.Picking.Dtos;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;
using WIS.Security;

namespace WIS.Domain.Services
{
    public class PreparacionService : IPreparacionService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IValidationService _validationService;
        protected readonly IOptions<MaxItemsSettings> _configuration;
        protected readonly IIdentityService _identity;

        public PreparacionService(IUnitOfWorkFactory uowFactory, IValidationService validationService, IOptions<MaxItemsSettings> configuration, IIdentityService identity)
        {
            _uowFactory = uowFactory;
            _validationService = validationService;
            _configuration = configuration;
            _identity = identity;
        }

        #region Picking

        public virtual async Task<ValidationsResult> ProcesarPicking(List<DetallePreparacion> pickeos, int userId)
        {
            var result = new ValidationsResult();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (pickeos.Count > 0)
                {
                    int nroRegistro = 1;
                    var keys = new HashSet<string>();
                    var prepContenedor = new HashSet<DetallePreparacion>();

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, pickeos.Count, _configuration.Value.Picking))
                        return result;

                    var context = await GetNewServiceContext(uow, pickeos, userId);

                    foreach (var pick in pickeos)
                    {
                        var validaciones = new List<Error>();

                        if (!ValidarDuplicados(context, pick, keys, prepContenedor, validaciones))
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, false, messages));
                            break;
                        }

                        validaciones.AddRange(await _validationService.ValidatePicking(pick, context, out bool errorProcedimiento));

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
                    var errores = await _validationService.ValidatePickingSaldos(pickeos, context);
                    if (errores.Count > 0)
                    {
                        var messages = Translator.Translate(uow, errores, _identity.UserId);
                        result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                    }

                    if (result.HasError())
                        return result;

                    await uow.PreparacionRepository.Preparar(pickeos, context);
                }
            }

            return result;
        }

        public virtual async Task<IPickingServiceContext> GetNewServiceContext(IUnitOfWork uow, List<DetallePreparacion> pickeos, int userId)
        {
            var empresa = pickeos[0].Empresa;
            var context = new PickingServiceContext(uow, pickeos, userId, empresa);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        public virtual void AddParametros(IPickingServiceContext context, int empresa)
        {
            context.AddParametroEmpresa(ParamManager.MANEJO_DOCUMENTAL, empresa);
        }

        public virtual bool ValidarDuplicados(IPickingServiceContext context, DetallePreparacion pick, HashSet<string> keys, HashSet<DetallePreparacion> prepContenedor, List<Error> validaciones)
        {
            string key = $"{pick.NumeroPreparacion}.{pick.Ubicacion}.{pick.Empresa}.{pick.Pedido}.{pick.CodigoAgente}.{pick.TipoAgente}.{pick.Producto}.{pick.Lote}.{pick.Faixa.ToString("#.###")}.{pick.IdExternoContenedor}.{pick.TipoContenedor}";

            var preparacion = context.GetPreparacion(pick.NumeroPreparacion);
            switch (preparacion.Agrupacion)
            {
                case Agrupacion.Pedido:
                    key += $".{pick.Pedido}.{pick.CodigoAgente}.{pick.TipoAgente}";
                    break;
                case Agrupacion.Cliente:
                    key += $".{pick.CodigoAgente}.{pick.TipoAgente}.{pick.ComparteContenedorPicking}";
                    break;
                case Agrupacion.Ruta:
                    key += $".{pick.Carga}.{pick.ComparteContenedorPicking}";
                    break;
                case Agrupacion.Onda:
                    key += $".{pick.ComparteContenedorPicking}";
                    break;
                default:
                    validaciones.Add(new Error("WMSAPI_msg_Error_PreparacionNoTieneAgrupacion", pick.NumeroPreparacion));
                    return false;
            }

            if (keys.Contains(key))
            {
                validaciones.Add(new Error(GetMensaje(pick)));
                return false;
            }
            else
                keys.Add(key);

            if (prepContenedor.Any(p => p.IdExternoContenedor == pick.IdExternoContenedor && p.TipoContenedor == pick.TipoContenedor && p.NumeroPreparacion != pick.NumeroPreparacion))
            {
                validaciones.Add(new Error("WMSAPI_msg_Error_MismoContenedorDistintasPreparaciones", pick.NumeroPreparacion, pick.NroContenedor));
                return false;
            }
            else
            {
                prepContenedor.Add(new DetallePreparacion()
                {
                    IdExternoContenedor = pick.IdExternoContenedor,
                    TipoContenedor = pick.TipoContenedor,
                    NumeroPreparacion = pick.NumeroPreparacion
                });
            }
            return true;
        }

        public virtual string GetMensaje(DetallePreparacion pick)
        {
            string msg = $@"Lineas duplicadas. 
                            Preparación: {pick.NumeroPreparacion} - 
                            Ubicación: {pick.Ubicacion} - 
                            Empresa: {pick.Empresa} - 
                            Producto: {pick.Producto} - 
                            Identificador: {pick.Lote} - 
                            Faixa: {pick.Faixa} - 
                            Contenedor: {pick.NroContenedor} - ";

            switch (pick.Agrupacion)
            {
                case Agrupacion.Pedido:
                    msg += $@"Pedido: {pick.Pedido} - CodigoAgente: {pick.CodigoAgente} - TipoAgente: {pick.TipoAgente}.";
                    break;
                case Agrupacion.Cliente:
                    msg += $"CodigoAgente: {pick.CodigoAgente} - TipoAgente: {pick.TipoAgente} - ComparteContenedorPicking: {pick.ComparteContenedorPicking}.";
                    break;
                case Agrupacion.Ruta:
                    msg += $"Carga: {pick.Carga} - ComparteContenedorPicking: {pick.ComparteContenedorPicking}.";
                    break;
                case Agrupacion.Onda:
                    msg += $"ComparteContenedorPicking: {pick.ComparteContenedorPicking}.";
                    break;
            }
            return msg;
        }

        #endregion

        #region AnularPickingPedidoPendiente

        public virtual async Task<ValidationsResult> AnularPickingPedidoPendiente(List<AnularPickingPedidoPendiente> detalles, int userId)
        {
            var result = new ValidationsResult();
            var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                int nroRegistro = 1;

                if (detalles.Count > 0)
                {
                    string aplicacion = _identity.Application;

                    if (aplicacion.Length > 30)
                    {
                        aplicacion = aplicacion.Substring(0, 30);
                    }

                    uow.CreateTransactionNumber(aplicacion);

                    var keys = new HashSet<string>();

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, detalles.Count, _configuration.Value.AnularPickingPedidoPendiente))
                        return result;

                    var context = await GetNewServiceContext(uow, detalles, userId);

                    foreach (var detalle in detalles)
                    {
                        validaciones.Clear();

                        detalle.NuTransaccion = uow.GetTransactionNumber();

                        string key = $"{detalle.Pedido}.{detalle.Preparacion}.{detalle.CodigoAgente}.{detalle.TipoAgente}.{detalle.Empresa}";

                        if (keys.Contains(key))
                        {
                            validaciones.Add(new Error("WMSAPI_msg_Error_AnulacionDuplicada", detalle.Pedido, detalle.Preparacion, detalle.CodigoAgente, detalle.TipoAgente));
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                            break;
                        }
                        else
                            keys.Add(key);

                        validaciones.AddRange(await _validationService.ValidateAnularPickingPedidoPendiente(detalle, context, out bool errorProcedimiento));

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, errorProcedimiento, messages));
                        }

                        nroRegistro++;
                    }

                    if (result.HasError())
                        return result;

                    await uow.AnulacionRepository.MarcarPickingParaAnular(detalles, context, uow.GetTransactionNumber(), userId);
                }
                else
                {
                    var msg = Translator.Translate(uow, new Error("WMSAPI_msg_Error_ListaDeObjetosRequerida"), _identity.UserId);
                    result.Errors.Add(new ValidationsError(nroRegistro, false, new List<string>() { msg }));
                }
            }
            return result;
        }

        public virtual async Task<IAnularPickingPedidoPendienteContext> GetNewServiceContext(IUnitOfWork uow, List<AnularPickingPedidoPendiente> detalles, int userId)
        {
            var empresa = detalles[0].Empresa;
            var context = new AnularPickingPedidoPendienteContext(uow, detalles, userId, empresa);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        public virtual void AddParametros(IAnularPickingPedidoPendienteContext context, int empresa)
        {
        }

        #endregion

        #region AnularPickingPedidoPendienteAutomatismo
        public virtual async Task<ValidationsResult> AnularPickingPedidoPendienteAutomatismo(List<AnularPickingPedidoPendienteAutomatismo> detalles, int userId)
        {
            var result = new ValidationsResult();
            var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (detalles.Count > 0)
                {
                    string aplicacion = _identity.Application;
                    if (aplicacion.Length > 30)
                    {
                        aplicacion = aplicacion.Substring(0, 30);
                    }
                    uow.CreateTransactionNumber(aplicacion);

                    int nroRegistro = 1;
                    var keys = new HashSet<string>();
                    if (detalles.Count > _configuration.Value.AnularPickingPedidoPendiente)
                    {
                        string msg = $"La cantidad de items enviados no puede superar el máximo de {_configuration.Value.AnularPickingPedidoPendiente}.";
                        result.Errors.Add(new ValidationsError(nroRegistro, false, new List<string>() { msg }));
                        return result;
                    }

                    var context = await GetNewServiceContext(uow, detalles, userId);

                    foreach (var detalle in detalles)
                    {

                        detalle.NuTransaccion = uow.GetTransactionNumber();


                        string key = $"{detalle.Pedido}.{detalle.Preparacion}.{detalle.CodigoAgente}.{detalle.TipoAgente}.{detalle.Empresa}";

                        if (keys.Contains(key))
                            validaciones.Add(new Error("WMSAPI_msg_Error_AnulacionDuplicada", detalle.Pedido, detalle.Preparacion, detalle.CodigoAgente, detalle.TipoAgente));
                        else
                            keys.Add(key);

                        validaciones.AddRange(await _validationService.ValidateAnularPickingPedidoPendienteAutomatismo(detalle, context, out bool errorProcedimiento));

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, errorProcedimiento, messages));
                        }


                        nroRegistro++;
                    }
                    if (result.HasError())
                        return result;


                    await uow.AnulacionRepository.MarcarPickingParaAnularAutomatismo(detalles, context, uow.GetTransactionNumber(), userId);
                }
            }
            return result;
        }


        public virtual async Task<AnularPickingPedidoPendienteAutomatismoContext> GetNewServiceContext(IUnitOfWork uow, List<AnularPickingPedidoPendienteAutomatismo> detalles, int userId)
        {
            var empresa = detalles[0].Empresa;
            var context = new AnularPickingPedidoPendienteAutomatismoContext(uow, detalles, userId, empresa);
            AddParametros(context, empresa);
            await context.Load();

            return context;
        }
        public virtual void AddParametros(AnularPickingPedidoPendienteAutomatismoContext context, int empresa)
        {
            context.AddParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
        }

        #endregion
    }
}
