using Irony.Parsing;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;
using WIS.Security;

namespace WIS.Domain.Services
{
    public class ProductoService : IProductoService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IValidationService _validationService;
        protected readonly IOptions<MaxItemsSettings> _configuration;
        protected readonly IIdentityService _identity;
        protected readonly IAutomatismoAutoStoreClientService _automatismoAutoStoreClientService;

        public ProductoService(IUnitOfWorkFactory uowFactory,
            IValidationService validationService,
            IOptions<MaxItemsSettings> configuration,
            IIdentityService identity,
            IAutomatismoAutoStoreClientService automatismoAutoStoreClientService)
        {
            _uowFactory = uowFactory;
            _validationService = validationService;
            _configuration = configuration;
            _identity = identity;
            _automatismoAutoStoreClientService = automatismoAutoStoreClientService;
        }

        public virtual async Task<Producto> GetProducto(string codigo, int codigoEmpresa)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return await uow.ProductoRepository.GetProductoOrNull(codigoEmpresa, codigo);
            }
        }

        public virtual async Task<ValidationsResult> AgregarProductos(List<Producto> productos, int userId)
        {
            var result = new ValidationsResult();
            var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                int nroRegistro = 1;

                if (productos.Count > 0)
                {
                    uow.CreateTransactionNumber(this._identity.Application);

                    var keys = new HashSet<string>();

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, productos.Count, _configuration.Value.Producto))
                        return result;

                    var context = await GetNewServiceContext(uow, productos, userId);

                    foreach (var producto in productos)
                    {
                        validaciones.Clear();
                        producto.NumeroTransaccion = uow.GetTransactionNumber();

                        string key = $"{producto.Codigo}.{producto.CodigoEmpresa}";

                        if (keys.Contains(key))
                        {
                            validaciones.Add(new Error("WMSAPI_msg_Error_ProductoDuplicados", producto.Codigo, producto.CodigoEmpresa));
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                            break;
                        }
                        else
                            keys.Add(key);

                        validaciones.AddRange(await _validationService.ValidateProducto(producto, context, out bool errorProcedimiento));

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, errorProcedimiento, messages));
                        }

                        nroRegistro++;
                    }

                    if (result.HasError())
                        return result;

                    await uow.ProductoRepository.AddProductos(productos, context);

                    NotificarAutomatismo(uow, productos);
                }
                else
                {
                    var msg = Translator.Translate(uow, new Error("WMSAPI_msg_Error_ListaDeObjetosRequerida"), _identity.UserId);
                    result.Errors.Add(new ValidationsError(nroRegistro, false, new List<string>() { msg }));
                }
            }

            return result;
        }

        public virtual void NotificarAutomatismo(IUnitOfWork uow, List<Producto> productos)
        {
            if (_automatismoAutoStoreClientService.IsEnabled() && productos.Count > 0)
            {
                var productosAutomatismo = uow.AutomatismoRepository.GetProductosAutomatismo(productos);

                if (productosAutomatismo.Count() > 0)
                {
                    var productoPorCodigo = new Dictionary<string, Producto>();

                    foreach (var producto in productosAutomatismo)
                    {
                        productoPorCodigo[producto.Codigo] = producto;
                    }

                    foreach (var producto in productos)
                    {
                        if (productoPorCodigo.ContainsKey(producto.Codigo))
                        {
                            productoPorCodigo[producto.Codigo] = producto;
                        }
                    }

                    var productosNotificables = productoPorCodigo.Values;
                    var nuTransaccion = productosNotificables.First().NumeroTransaccion;
                    var empresa = productosNotificables.First().CodigoEmpresa;
                    var request = new ProductosAutomatismoRequest
                    {
                        DsReferencia = nuTransaccion.ToString(),
                        Empresa = empresa,
                        Productos = productosNotificables.Select(p => new ProductoAutomatismoRequest
                        {
                            Altura = p.Altura,
                            Ancho = p.Ancho,
                            Codigo = p.Codigo,
                            Descripcion = p.Descripcion,
                            ManejoIdentificador = p.ManejoIdentificadorId,
                            PesoBruto = p.PesoBruto,
                            Profundidad = p.Profundidad,
                            TipoManejoFecha = p.TipoManejoFecha,
                            UnidadBulto = p.UnidadBulto
                        }).ToList(),
                    };

                    _automatismoAutoStoreClientService.SendProductos(request);
                }
            }
        }

        public virtual async Task<IProductoServiceContext> GetNewServiceContext(IUnitOfWork uow, List<Producto> productos, int userId)
        {
            var empresa = productos[0].CodigoEmpresa;
            var context = new ProductoServiceContext(uow, productos, userId, empresa);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        public virtual void AddParametros(IProductoServiceContext context, int empresa)
        {
            context.AddParametro(ParamManager.LISTA_CARACTERES_COD_PROD);

            context.AddParametroEmpresa(ParamManager.IE_500_FAMILIA_PRODUTO, empresa);
            context.AddParametroEmpresa(ParamManager.IE_500_ROTATIVIDADE, empresa);
            context.AddParametroEmpresa(ParamManager.IE_500_CLASSE, empresa);
            context.AddParametroEmpresa(ParamManager.IE_500_ESTOQUE_MINIMO, empresa);
            context.AddParametroEmpresa(ParamManager.IE_500_ESTOQUE_MAXIMO, empresa);
            context.AddParametroEmpresa(ParamManager.IE_500_PS_LIQUIDO, empresa);
            context.AddParametroEmpresa(ParamManager.IE_500_PS_BRUTO, empresa);
            context.AddParametroEmpresa(ParamManager.IE_500_VL_CUBAGEM, empresa);
            context.AddParametroEmpresa(ParamManager.IE_500_VL_PRECO_VENDA, empresa);
            context.AddParametroEmpresa(ParamManager.IE_500_VL_CUSTO_ULT_ENT, empresa);
            context.AddParametroEmpresa(ParamManager.IE_500_UND_DISTRIBUCION, empresa);
            context.AddParametroEmpresa(ParamManager.IE_500_UND_BULTO, empresa);
            context.AddParametroEmpresa(ParamManager.IE_500_ID_MANEJO_IDENT, empresa);
            context.AddParametroEmpresa(ParamManager.IE_500_UNIDA_DE_MEDIDA, empresa);
            context.AddParametroEmpresa(ParamManager.IE_500_TP_MANEJO_FECHA, empresa);
            context.AddParametroEmpresa(ParamManager.IE_500_CD_SITUACAO, empresa);
            context.AddParametroEmpresa(ParamManager.IE_500_CD_GRUPO_CONSULTA, empresa);
            context.AddParametroEmpresa(ParamManager.IE_500_TP_DISPLAY, empresa);
            context.AddParametroEmpresa(ParamManager.IE_500_QT_DIAS_DURACAO, empresa);
            context.AddParametroEmpresa(ParamManager.IE_500_QT_DIAS_VALIDADEA, empresa);
            context.AddParametroEmpresa(ParamManager.IE_500_ID_AGRUPACION, empresa);
            context.AddParametroEmpresa(ParamManager.IE_500_CD_RAMO_PRODUTO, empresa);
            context.AddParametroEmpresa(ParamManager.IE_500_FL_ACEPTA_DECIMALES, empresa);
            context.AddParametroEmpresa(ParamManager.IE_500_HAB_INGRESO_CLASE, empresa);
            context.AddParametroEmpresa(ParamManager.IE_500_HAB_INGRESO_FAMILIA, empresa);
            context.AddParametroEmpresa(ParamManager.IE_500_HAB_INGRESO_RAMO, empresa);
            context.AddParametroEmpresa(ParamManager.IE_500_CAMPOS_INMUTABLES, empresa);

            context.SetParametroCamposInmutables(ParamManager.IE_500_CAMPOS_INMUTABLES);
        }

        public virtual async Task<ProductoProveedor> GetProductoProveedor(string producto, int empresa, string tipoAgente, string codigoAgente)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return await uow.ProductoRepository.GetProductoProveedorOrNull(producto, empresa, tipoAgente, codigoAgente);
            }
        }

        public virtual async Task<ValidationsResult> AgregarProductosProveedor(List<ProductoProveedor> productos, int userId)
        {
            var result = new ValidationsResult();
            var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                int nroRegistro = 1;

                if (productos.Count > 0)
                {
                    var keysExternos = new HashSet<string>();
                    var keysProductos = new HashSet<string>();

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, productos.Count, _configuration.Value.ProductoProveedor))
                        return result;

                    var context = await GetNewServiceContext(uow, productos, userId);

                    foreach (var producto in productos)
                    {
                        validaciones.Clear();

                        string keyProducto = $"{producto.CodigoProducto}.{producto.Empresa}.{producto.TipoAgente}.{producto.CodigoAgente}";
                        if (keysProductos.Contains(keyProducto))
                            validaciones.Add(new Error("WMSAPI_msg_Error_ProductoProveedorDuplicado", producto.CodigoProducto, producto.Empresa, producto.TipoAgente, producto.CodigoAgente));
                        else
                            keysProductos.Add(keyProducto);

                        string keyExterno = producto.CodigoExterno;
                        if (keysExternos.Contains(keyExterno))
                            validaciones.Add(new Error("WMSAPI_msg_Error_ProductoProveedorCodigoExternoDuplicado", producto.CodigoExterno));
                        else
                            keysExternos.Add(keyExterno);

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                            break;
                        }

                        validaciones.AddRange(await _validationService.ValidateProductoProveedor(producto, context, out bool errorProcedimiento));

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                        }

                        nroRegistro++;
                    }

                    if (result.HasError())
                        return result;

                    await uow.ProductoRepository.AddProductosProveedor(productos, context);
                }
                else
                {
                    var msg = Translator.Translate(uow, new Error("WMSAPI_msg_Error_ListaDeObjetosRequerida"), _identity.UserId);
                    result.Errors.Add(new ValidationsError(nroRegistro, false, new List<string>() { msg }));
                }
            }

            return result;
        }

        public virtual async Task<IProductoProveedorServiceContext> GetNewServiceContext(IUnitOfWork uow, List<ProductoProveedor> productos, int userId)
        {
            var empresa = productos[0].Empresa;
            var context = new ProductoProveedorServiceContext(uow, productos, userId, empresa);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        public virtual void AddParametros(IProductoProveedorServiceContext context, int empresa)
        {
        }
    }
}
