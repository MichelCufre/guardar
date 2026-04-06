using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Services.Common;
using MigraDoc.DocumentObjectModel.Tables;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.AutomationManager.Interfaces;
using WIS.AutomationManager.Models;
using WIS.AutomationManager.Models.Mappers.Interfaces;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Security;
using WIS.Domain.Validation;
using WIS.Security;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace WIS.AutomationManager.Services
{
    public class AutomatismoService : IAutomatismoService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IOptions<AutomationSettings> _configuration;
        protected readonly IAutomatismoNotificationFactory _serviceFactory;
        protected readonly IAutomatismoValidationService _validationService;
        private readonly IIdentityService _identity;
        protected readonly IConfirmacionMovimientoAutomatismoMapper _movimientoStockAutomatismoMapper;
        protected readonly INotificacionAjusteStockAutomatismoMapper _ajusteStockAutomatismoMapper;

        public AutomatismoService(IUnitOfWorkFactory uowFactory,
            IOptions<AutomationSettings> configuration,
            IAutomatismoNotificationFactory serviceFactory,
            IAutomatismoValidationService validationService,
            IIdentityService identity,
            IConfirmacionMovimientoAutomatismoMapper confirmacionMovimientoAutomatismoMapper,
            INotificacionAjusteStockAutomatismoMapper ajusteStockAutomatismoMapper)
        {
            _uowFactory = uowFactory;
            _configuration = configuration;
            _serviceFactory = serviceFactory;
            _validationService = validationService;
            _identity = identity;
            _movimientoStockAutomatismoMapper = confirmacionMovimientoAutomatismoMapper;
            _ajusteStockAutomatismoMapper = ajusteStockAutomatismoMapper;
        }

        public virtual IAutomatismo GetByZona(IUnitOfWork uow, string zona)
        {
            return uow.AutomatismoRepository.GetAutomatismoByZona(zona);
        }

        public virtual IAutomatismo GetByCodigo(IUnitOfWork uow, string codigo)
        {
            return uow.AutomatismoRepository.GetAutomatismoByCodigo(codigo);
        }

        public virtual List<IAutomatismo> GetAllByTipo(IUnitOfWork uow, string tipo)
        {
            return uow.AutomatismoRepository.GetAllByTipo(tipo);
        }

        public virtual IAutomatismo GetByPuesto(IUnitOfWork uow, string puesto)
        {
            return uow.AutomatismoRepository.GetAutomatismoByPuesto(puesto);
        }

        public virtual async Task<ValidationsResult> NotificarProductos(ProductosAutomatismoRequest request, AutomatismoEjecucion ejecucion)
        {
            var response = new ValidationsResult();


            if (request.Productos.Count > 0)
            {
                var validaciones = new List<Error>();
                int nroRegistro = 1;
                var keys = new HashSet<string>();
                var empresa = request.Empresa;
                var zonaProductos = new Dictionary<string, List<ProductoAutomatismoRequest>>();
                var codigoProducto = new Dictionary<string, ProductoAutomatismoRequest>();

                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    if (request.Productos.Count > _configuration.Value.MaxProductos)
                    {
                        validaciones.Add(new Error("WMSAPI_msg_Error_AutomatismoCantidadItems", _configuration.Value.MaxProductos));
                        var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                        response.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                        return response;
                    }

                    var context = await GetNewServiceContext(uow, request, ejecucion.UserId);

                    foreach (var producto in request.Productos)
                    {
                        var key = $"{request.Empresa}.{producto.Codigo}";

                        if (keys.Contains(key))
                        {
                            validaciones.Add(new Error("WMSAPI_msg_Error_AutomatismoProductosDuplicados", request.Empresa, producto.Codigo));
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            response.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                            return response;
                        }
                        else
                            keys.Add(key);

                        validaciones.AddRange(_validationService.ValidateProductoAutomatismo(producto, context, out bool errorProcedimiento));

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            response.Errors.Add(new ValidationsError(nroRegistro, errorProcedimiento, messages));
                        }

                        nroRegistro++;
                        codigoProducto[producto.Codigo] = producto;
                    }

                    if (response.HasError())
                        return response;

                    var productoZonas = this.GetZonasAutomatismoByProductos(uow, empresa, codigoProducto.Keys);

                    foreach (var producto in productoZonas.Keys)
                    {
                        foreach (var zona in productoZonas[producto])
                        {
                            zonaProductos[zona.Zona] = new List<ProductoAutomatismoRequest>();
                        }
                    }

                    var (valEnvInt, zonaAutomatismo) = await this._validationService.ValidateEnvioInterfaz(uow, zonaProductos.Keys, CodigoInterfazAutomatismoDb.CD_INTERFAZ_PRODUCTOS);

                    if (valEnvInt.HasError())
                        return valEnvInt;

                    foreach (var cdProducto in productoZonas.Keys)
                    {
                        foreach (var zona in productoZonas[cdProducto])
                        {
                            var automatismo = zonaAutomatismo[zona.Zona];
                            var producto = codigoProducto[cdProducto];

                            producto.Predio = automatismo.Codigo;
                            producto.UnidadCaja = zona.UnidadCaja;
                            producto.CantidadUnidadCaja = zona.CantidadUnidadCaja;
                            producto.ConfirmarCodigoBarras = zona.ConfirmarCodigoBarras;

                            zonaProductos[zona.Zona].Add(producto);
                        }
                    }

                    foreach (var zona in zonaProductos.Keys)
                    {
                        var automatismo = zonaAutomatismo[zona];
                        var productosZona = zonaProductos[zona];

                        automatismo.SetInterfazEnUso(CodigoInterfazAutomatismoDb.CD_INTERFAZ_PRODUCTOS);
                        ejecucion.SetAutomatismo(automatismo);

                        var service = _serviceFactory.Create(automatismo);
                        var serviceResponse = service.NotificarProductos(automatismo, new ProductosAutomatismoRequest
                        {
                            Archivo = request.Archivo,
                            DsReferencia = request.DsReferencia,
                            Empresa = request.Empresa,
                            Productos = productosZona,
                        });

                        if (serviceResponse.IsError)
                            response.AddError(serviceResponse.Mensaje);

                        if (response.HasError())
                            return response;
                    }
                }
            }

            return response;
        }

        public virtual async Task<ValidationsResult> NotificarCodigosBarras(CodigosBarrasAutomatismoRequest request, AutomatismoEjecucion ejecucion)
        {
            var response = new ValidationsResult();

            if (request.CodigosDeBarras.Count > 0)
            {
                var validaciones = new List<Error>();
                int nroRegistro = 1;
                var keys = new HashSet<string>();
                var empresa = request.Empresa;
                var zonaCodigosBarras = new Dictionary<string, List<CodigoBarraAutomatismoRequest>>();
                var codigoCodigoBarras = new Dictionary<string, CodigoBarraAutomatismoRequest>();

                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    if (request.CodigosDeBarras.Count > _configuration.Value.MaxCodigosBarras)
                    {
                        validaciones.Add(new Error("WMSAPI_msg_Error_AutomatismoCantidadItems", _configuration.Value.MaxCodigosBarras));
                        var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                        response.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                        return response;
                    }

                    var context = await GetNewServiceContext(uow, request, ejecucion.UserId);

                    foreach (var codigo in request.CodigosDeBarras)
                    {
                        var key = $"{request.Empresa}.{codigo.Codigo}";

                        if (keys.Contains(key))
                        {
                            validaciones.Add(new Error("WMSAPI_msg_Error_AutomatismoCodigosBarrasDuplicados", request.Empresa, codigo.Codigo));
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            response.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                            return response;
                        }
                        else
                            keys.Add(key);

                        validaciones.AddRange(_validationService.ValidateCodigoBarrasAutomatismo(codigo, context, out bool errorProcedimiento));

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            response.Errors.Add(new ValidationsError(nroRegistro, errorProcedimiento, messages));
                        }

                        nroRegistro++;
                        codigoCodigoBarras[codigo.Codigo] = codigo;
                    }

                    if (response.HasError())
                        return response;

                    var codigoBarrasZonas = this.GetZonasAutomatismoByCodigosBarras(uow, codigoCodigoBarras.Values.Select(c => new CodigoBarras
                    {
                        Codigo = c.Codigo,
                        Empresa = empresa,
                        Producto = c.Producto,
                    }));

                    foreach (var codigo in codigoBarrasZonas.Keys)
                    {
                        foreach (var zonaCodigo in codigoBarrasZonas[codigo])
                        {
                            zonaCodigosBarras[zonaCodigo] = new List<CodigoBarraAutomatismoRequest>();
                        }
                    }

                    var (valEnvInt, zonaAutomatismo) = await this._validationService.ValidateEnvioInterfaz(uow, zonaCodigosBarras.Keys, CodigoInterfazAutomatismoDb.CD_INTERFAZ_CODIGO_BARRAS);

                    if (valEnvInt.HasError())
                        return valEnvInt;

                    foreach (var cdCodigo in codigoBarrasZonas.Keys)
                    {
                        foreach (var zonaCodigo in codigoBarrasZonas[cdCodigo])
                        {
                            var automatismo = zonaAutomatismo[zonaCodigo];
                            var codigo = codigoCodigoBarras[cdCodigo];

                            codigo.Predio = automatismo.Codigo;
                            zonaCodigosBarras[zonaCodigo].Add(codigo);
                        }
                    }

                    foreach (var zonaCodigo in zonaCodigosBarras.Keys)
                    {
                        var automatismo = zonaAutomatismo[zonaCodigo];
                        var codigosZona = zonaCodigosBarras[zonaCodigo];

                        automatismo.SetInterfazEnUso(CodigoInterfazAutomatismoDb.CD_INTERFAZ_CODIGO_BARRAS);
                        ejecucion.SetAutomatismo(automatismo);

                        var service = this._serviceFactory.Create(automatismo);
                        var serviceResponse = service.NotificarCodigosBarras(automatismo, new CodigosBarrasAutomatismoRequest
                        {
                            Archivo = request.Archivo,
                            CodigosDeBarras = codigosZona,
                            DsReferencia = request.DsReferencia,
                            Empresa = request.Empresa,
                        });

                        if (serviceResponse.IsError)
                            response.AddError(serviceResponse.Mensaje);

                        if (response.HasError())
                            return response;
                    }
                }
            }

            return response;
        }

        protected virtual async Task<AutomatismoCodigoBarraServiceContext> GetNewServiceContext(IUnitOfWork uow, CodigosBarrasAutomatismoRequest request, int userId)
        {
            var empresa = request.Empresa;
            var context = new AutomatismoCodigoBarraServiceContext(uow, request, userId);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        protected virtual void AddParametros(AutomatismoCodigoBarraServiceContext context, int empresa)
        {
            context.AddParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
        }

        public virtual Dictionary<string, List<ProductoZonaAutomatismo>> GetZonasAutomatismoByProductos(IUnitOfWork uow, int empresa, IEnumerable<string> productos)
        {
            var resultado = new Dictionary<string, List<ProductoZonaAutomatismo>>();
            var productoZonas = uow.AutomatismoRepository.GetZonasAutomatismoByProductos(productos.Select(p => new Producto
            {
                CodigoEmpresa = empresa,
                Codigo = p,
            }));

            foreach (var productoZona in productoZonas)
            {
                if (!resultado.ContainsKey(productoZona.Codigo))
                    resultado[productoZona.Codigo] = new List<ProductoZonaAutomatismo>();

                resultado[productoZona.Codigo].Add(productoZona);
            }

            return resultado;
        }

        public virtual Dictionary<string, List<string>> GetZonasAutomatismoByCodigosBarras(IUnitOfWork uow, IEnumerable<CodigoBarras> codigos)
        {
            var resultado = new Dictionary<string, List<string>>();
            var codigoBarrasZonas = uow.AutomatismoRepository.GetZonasAutomatismoByCodigosBarras(codigos);

            foreach (var codigoBarrasZona in codigoBarrasZonas)
            {
                if (!resultado.ContainsKey(codigoBarrasZona.Codigo))
                    resultado[codigoBarrasZona.Codigo] = new List<string>();

                resultado[codigoBarrasZona.Codigo].Add(codigoBarrasZona.Zona);
            }

            return resultado;
        }

        protected virtual async Task<AutomatismoProductoServiceContext> GetNewServiceContext(IUnitOfWork uow, ProductosAutomatismoRequest request, int userId)
        {
            var empresa = request.Empresa;
            var context = new AutomatismoProductoServiceContext(uow, request, userId);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        protected virtual void AddParametros(AutomatismoProductoServiceContext context, int empresa)
        {
            context.AddParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
        }

        public virtual async Task<ValidationsResult> NotificarEntrada(EntradaStockAutomatismoRequest request, AutomatismoEjecucion ejecucion)
        {
            var response = new ValidationsResult();
            var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (request.Detalles.Count > _configuration.Value.MaxEntradas)
                {
                    validaciones.Add(new Error("WMSAPI_msg_Error_AutomatismoCantidadLineas", _configuration.Value.MaxEntradas));
                    var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                    response.Errors.Add(new ValidationsError(0, false, messages));
                    return response;
                }

                var context = await GetNewServiceContext(uow, request, ejecucion.UserId);

                validaciones.AddRange(_validationService.ValidateEntradaAutomatismo(request, context, out bool errorProcedimiento));

                if (validaciones.Count > 0)
                {
                    var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                    response.Errors.Add(new ValidationsError(0, errorProcedimiento, messages));
                }

                if (response.HasError())
                    return response;

                var zona = uow.UbicacionRepository.GetUbicacion(request.Ubicacion).IdUbicacionZona;
                var valEnvInt = await this._validationService.ValidateEnvioInterfaz(uow, zona, CodigoInterfazAutomatismoDb.CD_INTERFAZ_ENTRADAS);

                if (valEnvInt.HasError())
                    return valEnvInt;

                var automatismo = this.GetByZona(uow, zona);

                automatismo.SetInterfazEnUso(CodigoInterfazAutomatismoDb.CD_INTERFAZ_ENTRADAS);
                ejecucion.SetAutomatismo(automatismo);

                request.Predio = automatismo.Codigo;

                var productos = this.GetProductosAutomatismo(uow, zona, request.Empresa, request.Detalles);
                var service = _serviceFactory.Create(automatismo);
                var serviceResponse = service.NotificarEntrada(automatismo, new EntradaStockAutomatismoRequest
                {
                    Archivo = request.Archivo,
                    Detalles = request.Detalles.Where(d => productos.Contains(d.Producto)).ToList(),
                    DsReferencia = request.DsReferencia,
                    Ejecucion = request.Ejecucion,
                    Empresa = request.Empresa,
                    Predio = request.Predio,
                });

                if (serviceResponse.IsError)
                    response.AddError(serviceResponse.Mensaje);
            }

            return response;
        }

        protected virtual async Task<AutomatismoEntradaServiceContext> GetNewServiceContext(IUnitOfWork uow, EntradaStockAutomatismoRequest request, int userId)
        {
            var empresa = request.Empresa;
            var context = new AutomatismoEntradaServiceContext(uow, request, userId);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        protected virtual void AddParametros(AutomatismoEntradaServiceContext context, int empresa)
        {
            context.AddParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
        }

        public virtual async Task<ValidationsResult> NotificarSalida(SalidaStockAutomatismoRequest request, AutomatismoEjecucion ejecucion)
        {
            var validaciones = new List<Error>();
            int nroRegistro = 1;
            var keys = new HashSet<string>();
            var keyDetalle = new Dictionary<string, SalidaStockLineaAutomatismoRequest>();
            var detallesPorZona = new Dictionary<string, List<SalidaStockLineaAutomatismoRequest>>();
            var response = new ValidationsResult();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (request.Detalles.Count > _configuration.Value.MaxSalidas)
                {
                    validaciones.Add(new Error("WMSAPI_msg_Error_AutomatismoCantidadLineas", _configuration.Value.MaxSalidas));
                    var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                    response.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                    return response;
                }

                var context = await GetNewServiceContext(uow, request, ejecucion.UserId);

                foreach (var detalle in request.Detalles)
                {
                    var key = $"{request.Empresa}.{detalle.Preparacion}.{detalle.Pedido}.{detalle.TipoAgente}.{detalle.CodigoAgente}.{detalle.Producto}.{detalle.Identificador}.{detalle.Carga}.{detalle.ComparteContenedorPicking}";

                    if (keys.Contains(key))
                    {
                        validaciones.Add(new Error("WMSAPI_msg_Error_AutomatismoDetallesDuplicados", request.Empresa, detalle.Preparacion, detalle.Pedido, detalle.TipoAgente, detalle.CodigoAgente, detalle.Producto, detalle.Identificador, detalle.Carga, detalle.ComparteContenedorPicking));
                        var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                        response.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                        return response;
                    }
                    else
                        keys.Add(key);

                    validaciones.AddRange(_validationService.ValidateSalidaAutomatismo(detalle, context, out bool errorProcedimiento));

                    if (validaciones.Count > 0)
                    {
                        var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                        response.Errors.Add(new ValidationsError(0, errorProcedimiento, messages));
                    }

                    nroRegistro++;
                    keyDetalle[key] = detalle;
                }

                if (response.HasError())
                    return response;

                foreach (var detalle in request.Detalles)
                {
                    if (!detallesPorZona.ContainsKey(detalle.Zona))
                        detallesPorZona[detalle.Zona] = new List<SalidaStockLineaAutomatismoRequest>();

                    detallesPorZona[detalle.Zona].Add(detalle);
                }

                foreach (var zona in detallesPorZona.Keys)
                {
                    var valEnvInt = await this._validationService.ValidateEnvioInterfaz(uow, zona, CodigoInterfazAutomatismoDb.CD_INTERFAZ_SALIDA);

                    if (valEnvInt.HasError())
                        return valEnvInt;
                }

                foreach (var zona in detallesPorZona.Keys)
                {
                    var automatismo = this.GetByZona(uow, zona);
                    var detallesZona = detallesPorZona[zona];

                    automatismo.SetInterfazEnUso(CodigoInterfazAutomatismoDb.CD_INTERFAZ_SALIDA);
                    ejecucion.SetAutomatismo(automatismo);

                    detallesZona.ForEach(d =>
                    {
                        d.ModoLanzamiento = automatismo.GetCaracteristicaByCodigo(CaracteristicasAutomatismoDb.MODALIDAD_LANZAMIENTO)?.Valor;
                        d.Predio = automatismo.Codigo;
                    });

                    var service = _serviceFactory.Create(automatismo);
                    var serviceResponse = service.NotificarSalida(automatismo, new SalidaStockAutomatismoRequest
                    {
                        Archivo = request.Archivo,
                        Detalles = detallesZona,
                        DsReferencia = request.DsReferencia,
                        Empresa = request.Empresa,
                        Ejecucion = request.Ejecucion
                    });

                    if (serviceResponse.IsError)
                        response.AddError(serviceResponse.Mensaje);
                }
            }

            return response;
        }

        protected virtual async Task<AutomatismoSalidaServiceContext> GetNewServiceContext(IUnitOfWork uow, SalidaStockAutomatismoRequest request, int userId)
        {
            var empresa = request.Empresa;
            var context = new AutomatismoSalidaServiceContext(uow, request, userId);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        public virtual void AddParametros(AutomatismoSalidaServiceContext context, int empresa)
        {
            context.AddParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
        }

        public virtual async Task<ValidationsResult> ProcesarNotificacionAjusteStock(string puesto, AjustesDeStockRequest request, AutomatismoEjecucion ejecucion)
        {
            var validaciones = new List<Error>();
            var response = new ValidationsResult();

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                if (request.Ajustes.Count > _configuration.Value.MaxNotificacionAjuste)
                {
                    validaciones.Add(new Error("WMSAPI_msg_Error_AutomatismoCantidadLineas", _configuration.Value.MaxNotificacionAjuste));
                    var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                    response.Errors.Add(new ValidationsError(0, false, messages));
                    return response;
                }

                var valEnvInt = await this._validationService.ValidarEnvioInterfazByPuesto(uow, puesto, CodigoInterfazAutomatismoDb.CD_INTERFAZ_NOTIF_AJUSTES);

                if (valEnvInt.HasError())
                    return valEnvInt;

                var automatismo = this.GetByPuesto(uow, puesto);

                ejecucion.SetAutomatismo(automatismo);

                var posicionAjuste = automatismo.GetPosiciones(AutomatismoPosicionesTipoDb.POS_PICKING).FirstOrDefault();

                foreach (var ajuste in request.Ajustes)
                {
                    ajuste.Ubicacion = posicionAjuste.IdUbicacion;
                }

                request.DsReferencia = automatismo.Codigo;

                var ajustesPorEmpresa = new Dictionary<int, AjustesDeStockRequest>();

                if (request.Empresa == -1)
                {
                    var empresaPorUbicacionPicking = this.GetEmpresasAutomatismoByUbicacionesPicking(uow, request.Ajustes);

                    foreach (var ajuste in request.Ajustes)
                    {
                        var key = $"{ajuste.Producto}.{ajuste.Ubicacion}";
                        var empresa = empresaPorUbicacionPicking.ContainsKey(key) ? empresaPorUbicacionPicking[key] : -1;

                        if (!ajustesPorEmpresa.ContainsKey(empresa))
                        {
                            ajustesPorEmpresa[empresa] = new AjustesDeStockRequest
                            {
                                Ajustes = new List<AjusteStockRequest>(),
                                Archivo = request.Archivo,
                                DsReferencia = request.DsReferencia,
                                Empresa = empresa,
                                Usuario = request.Usuario
                            };
                        }

                        ajustesPorEmpresa[empresa].Ajustes.Add(ajuste);
                    }
                }
                else
                {
                    ajustesPorEmpresa[request.Empresa] = request;
                }

                foreach (var empresa in ajustesPorEmpresa.Keys)
                {
                    var ajustesEmpresa = ajustesPorEmpresa[empresa];

                    var context = await GetNewServiceContext(uow, ajustesEmpresa, ejecucion.UserId);
                    validaciones.AddRange(_validationService.ValidateNotificacionAjustesAutomatismo(ajustesEmpresa, context, out bool errorProcedimiento));

                    if (validaciones.Count > 0)
                    {
                        var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                        response.Errors.Add(new ValidationsError(0, errorProcedimiento, messages));
                    }

                    if (response.HasError())
                        return response;
                }

                var service = _serviceFactory.Create(automatismo);

                foreach (var empresa in ajustesPorEmpresa.Keys)
                {
                    var ajustesEmpresa = ajustesPorEmpresa[empresa];

                    response = service.ProcesarNotificacionAjustes(uow, automatismo, ajustesEmpresa);

                    if (response.HasError())
                        return response;
                }
            }

            return response;
        }

        protected virtual async Task<AutomatismoNotificacionAjusteStockServiceContext> GetNewServiceContext(IUnitOfWork uow, AjustesDeStockRequest request, int userId)
        {
            var empresa = request.Empresa;
            var context = new AutomatismoNotificacionAjusteStockServiceContext(uow, request, userId);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        protected virtual void AddParametros(AutomatismoNotificacionAjusteStockServiceContext context, int empresa)
        {
            context.AddParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
        }

        public virtual async Task<ValidationsResult> ProcesarConfirmacionEntrada(string puesto, TransferenciaStockRequest request, AutomatismoEjecucion ejecucion)
        {
            var response = new ValidationsResult();
            var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (request.Transferencias.Count > _configuration.Value.MaxConfirmacionEntrada)
                {
                    validaciones.Add(new Error("WMSAPI_msg_Error_AutomatismoCantidadLineas", _configuration.Value.MaxConfirmacionEntrada));
                    var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                    response.Errors.Add(new ValidationsError(0, false, messages));
                    return response;
                }

                var valEnvInt = await this._validationService.ValidarEnvioInterfazByPuesto(uow, puesto, CodigoInterfazAutomatismoDb.CD_INTERFAZ_CONF_ENTRADAS);

                if (valEnvInt.HasError())
                    return valEnvInt;

                var nuEjecucionEntrada = int.Parse(request.IdEntrada);
                var ejecucionEntrada = uow.AutomatismoEjecucionRepository.GetAutomatismoEjecucionWithData(nuEjecucionEntrada);

                if (ejecucionEntrada == null)
                {
                    validaciones.Add(new Error("WMSAPI_msg_Error_AutomatismEjecucionNoExiste", nuEjecucionEntrada));
                    var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                    response.Errors.Add(new ValidationsError(0, false, messages));
                    return response;
                }
                else if (ejecucionEntrada.InterfazExterna != CodigoInterfazAutomatismoDb.CD_INTERFAZ_ENTRADAS)
                {
                    validaciones.Add(new Error("WMSAPI_msg_Error_AutomatismCodigoInterfazNoCoincide"));
                    var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                    response.Errors.Add(new ValidationsError(0, false, messages));
                    return response;
                }

                var automatismo = this.GetByPuesto(uow, puesto);

                ejecucion.SetAutomatismo(automatismo);

                var ejecucionEntradaData = ejecucionEntrada.AutomatismoData.OrderByDescending(o => o.Id).FirstOrDefault();
                var entradaNotificada = JsonConvert.DeserializeObject<EntradaStockAutomatismoRequest>(ejecucionEntradaData.RequestData);
                var posicionOrigen = automatismo.GetPosiciones(AutomatismoPosicionesTipoDb.POS_ENTRADA).FirstOrDefault();
                var posicionRechazo = automatismo.GetPosiciones(AutomatismoPosicionesTipoDb.POS_RECHAZO).FirstOrDefault();

                var context = await GetNewServiceContext(uow, request, entradaNotificada, ejecucion.UserId);

                validaciones.AddRange(_validationService.ValidateConfirmacionEntradaAutomatismo(request, context, out bool errorProcedimiento));

                if (validaciones.Count > 0)
                {
                    var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                    response.Errors.Add(new ValidationsError(0, errorProcedimiento, messages));
                }

                if (response.HasError())
                    return response;

                //Se notifican las unidades sin entrada, para enviar a rechazo
                var transferenciasANotificar = new List<TransferenciaRequest>();

                foreach (var transferencia in request.Transferencias.Where(w => w.Cantidad < w.CantidadSolicitada))
                {
                    transferenciasANotificar.Add(new TransferenciaRequest
                    {
                        Ubicacion = posicionOrigen.IdUbicacion,
                        UbicacionDestino = posicionRechazo.IdUbicacion,
                        Cantidad = transferencia.CantidadSolicitada - transferencia.Cantidad,
                        CodigoProducto = transferencia.CodigoProducto,
                        Identificador = string.IsNullOrEmpty(transferencia.Identificador) ? "*" : transferencia.Identificador,
                        IdLinea = transferencia.IdLinea,
                    });
                }

                request.Transferencias = transferenciasANotificar;
                request.DsReferencia = automatismo.Codigo;

                var service = _serviceFactory.Create(automatismo);

                response = service.ProcesarConfirmacionEntrada(uow, automatismo, request, entradaNotificada, context);
            }

            return response;
        }

        protected virtual async Task<AutomatismoConfirmacionEntradaServiceContext> GetNewServiceContext(IUnitOfWork uow, TransferenciaStockRequest entradasConfirmadas, EntradaStockAutomatismoRequest ordenesEntrada, int userId)
        {
            var empresa = ordenesEntrada.Empresa;
            var context = new AutomatismoConfirmacionEntradaServiceContext(uow, entradasConfirmadas, ordenesEntrada, userId);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        protected virtual void AddParametros(AutomatismoConfirmacionEntradaServiceContext context, int empresa)
        {
            context.AddParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
        }

        public virtual async Task<ValidationsResult> ProcesarConfirmacionSalida(string puesto, PickingRequest request, AutomatismoEjecucion ejecucion)
        {
            var response = new ValidationsResult();
            var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (request.Detalles.Count > _configuration.Value.MaxConfirmacionSalida)
                {
                    validaciones.Add(new Error("WMSAPI_msg_Error_AutomatismoCantidadLineas", _configuration.Value.MaxConfirmacionSalida));
                    var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                    response.Errors.Add(new ValidationsError(0, false, messages));
                    return response;
                }

                var valEnvInt = await this._validationService.ValidarEnvioInterfazByPuesto(uow, puesto, CodigoInterfazAutomatismoDb.CD_INTERFAZ_CONF_SALIDAS);

                if (valEnvInt.HasError())
                    return valEnvInt;

                var automatismo = this.GetByPuesto(uow, puesto);
                ejecucion.SetAutomatismo(automatismo);

                var context = await GetNewServiceContext(uow, request, ejecucion.UserId);
                validaciones.AddRange(_validationService.ValidateConfirmacionSalidaAutomatismo(request, context, out bool errorProcedimiento));

                if (validaciones.Count > 0)
                {
                    var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                    response.Errors.Add(new ValidationsError(0, errorProcedimiento, messages));
                }

                if (response.HasError())
                    return response;

                var nuEjecucionSalida = int.Parse(request.IdRequest);
                var ejecucionSalida = uow.AutomatismoEjecucionRepository.GetAutomatismoEjecucionWithData(nuEjecucionSalida);

                if (ejecucionSalida == null)
                {
                    validaciones.Add(new Error("WMSAPI_msg_Error_AutomatismEjecucionNoExiste", nuEjecucionSalida));
                    var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                    response.Errors.Add(new ValidationsError(0, false, messages));
                    return response;
                }
                else if (ejecucionSalida.InterfazExterna != CodigoInterfazAutomatismoDb.CD_INTERFAZ_SALIDA)
                {
                    validaciones.Add(new Error("WMSAPI_msg_Error_AutomatismCodigoInterfazNoCoincide"));
                    var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                    response.Errors.Add(new ValidationsError(0, false, messages));
                    return response;
                }

                var service = _serviceFactory.Create(automatismo);

                response = service.ProcesarConfirmacionSalida(uow, automatismo, request);
            }

            return response;
        }

        protected virtual async Task<AutomatismoConfirmacionSalidaServiceContext> GetNewServiceContext(IUnitOfWork uow, PickingRequest request, int userId)
        {
            var empresa = request.Empresa;
            var context = new AutomatismoConfirmacionSalidaServiceContext(uow, request, userId);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        protected virtual void AddParametros(AutomatismoConfirmacionSalidaServiceContext context, int empresa)
        {
            context.AddParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
        }

        public virtual async Task<ValidationsResult> NotificarUbicacionesPicking(UbicacionesPickingAutomatismoRequest request, AutomatismoEjecucion ejecucion)
        {
            var response = new ValidationsResult();

            if (request.Ubicaciones.Count > 0)
            {
                var validaciones = new List<Error>();
                int nroRegistro = 1;
                var keys = new HashSet<string>();
                var empresa = request.Empresa;
                var ubicacionPickingPorKey = new Dictionary<string, UbicacionPickingAutomatismoRequest>();

                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    if (request.Ubicaciones.Count > _configuration.Value.MaxUbicacionesPicking)
                    {
                        validaciones.Add(new Error("WMSAPI_msg_Error_AutomatismoCantidadLineas", _configuration.Value.MaxUbicacionesPicking));
                        var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                        response.Errors.Add(new ValidationsError(nroRegistro, false, messages));
                        return response;
                    }

                    var context = await GetNewServiceContext(uow, request, ejecucion.UserId);

                    foreach (var ubicacion in request.Ubicaciones)
                    {
                        var key = $"{request.Empresa}.{ubicacion.Producto}.{ubicacion.Ubicacion}";

                        if (keys.Contains(key))
                        {
                            validaciones.Add(new Error("WMSAPI_msg_Error_AutomatismUbicacionesPickingDuplicadas", request.Empresa, ubicacion.Producto, ubicacion.Ubicacion));
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            response.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                            return response;
                        }
                        else
                            keys.Add(key);

                        validaciones.AddRange(_validationService.ValidateUbicacionPickingAutomatismo(ubicacion, context, out bool errorProcedimiento));

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            response.Errors.Add(new ValidationsError(nroRegistro, errorProcedimiento, messages));
                        }

                        nroRegistro++;
                        ubicacionPickingPorKey[key] = ubicacion;
                    }

                    if (response.HasError())
                        return response;

                    var productosPorZona = this.GetProductosAutomatismoByUbicacionesPicking(uow, empresa, ubicacionPickingPorKey);
                    var codigosPorZona = this.GetCodigosBarrasAutomatismoByUbicacionesPicking(uow, empresa, ubicacionPickingPorKey);
                    var (valEnvInt, zonaAutomatismo) = await this._validationService.ValidateEnvioInterfaz(uow, productosPorZona.Keys, CodigoInterfazAutomatismoDb.CD_INTERFAZ_CODIGO_BARRAS);

                    if (valEnvInt.HasError())
                        return valEnvInt;

                    foreach (var zona in productosPorZona.Keys)
                    {
                        var automatismo = zonaAutomatismo[zona];

                        foreach (var producto in productosPorZona[zona])
                        {
                            producto.Predio = automatismo.Codigo;
                        }
                        if (codigosPorZona != null && codigosPorZona.Any())
                        {
                            foreach (var codigo in codigosPorZona[zona])
                            {
                                codigo.Predio = automatismo.Codigo;
                            }
                        }
                    }

                    foreach (var zona in productosPorZona.Keys)
                    {
                        var automatismo = zonaAutomatismo[zona];
                        var productosZona = productosPorZona[zona];

                        automatismo.SetInterfazEnUso(CodigoInterfazAutomatismoDb.CD_INTERFAZ_UBICACIONES_PICKING);
                        ejecucion.SetAutomatismo(automatismo);

                        var service = this._serviceFactory.Create(automatismo);
                        var serviceResponse = service.NotificarProductos(automatismo, new ProductosAutomatismoRequest
                        {
                            Archivo = request.Archivo,
                            DsReferencia = request.DsReferencia,
                            Empresa = request.Empresa,
                            Productos = productosZona,
                        });

                        if (serviceResponse.IsError)
                        {
                            response.AddError(serviceResponse.Mensaje);
                            return response;
                        }
                        if (codigosPorZona != null && codigosPorZona.Any())
                        {
                            var codigosZona = codigosPorZona[zona];

                            serviceResponse = service.NotificarCodigosBarras(automatismo, new CodigosBarrasAutomatismoRequest
                            {
                                Archivo = request.Archivo,
                                DsReferencia = request.DsReferencia,
                                Empresa = request.Empresa,
                                CodigosDeBarras = codigosZona,
                            });
                        }

                        if (serviceResponse.IsError)
                        {
                            response.AddError(serviceResponse.Mensaje);
                            return response;
                        }
                    }
                }
            }

            return response;
        }

        protected virtual async Task<AutomatismoUbicacionPickingServiceContext> GetNewServiceContext(IUnitOfWork uow, UbicacionesPickingAutomatismoRequest request, int userId)
        {
            var empresa = request.Empresa;
            var context = new AutomatismoUbicacionPickingServiceContext(uow, request, userId);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        protected virtual void AddParametros(AutomatismoUbicacionPickingServiceContext context, int empresa)
        {
            context.AddParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
        }

        protected virtual Dictionary<string, List<string>> GetZonasAutomatismoByUbicacionesPicking(IUnitOfWork uow, int empresa, Dictionary<string, UbicacionPickingAutomatismoRequest> ubicacionesPickingPorKey)
        {
            var resultado = new Dictionary<string, List<string>>();
            var ubicacionPickingZonas = uow.AutomatismoRepository.GetZonasAutomatismoByUbicacionesPicking(ubicacionesPickingPorKey.Values.Select(up => new UbicacionPickingProducto
            {
                Empresa = empresa,
                CodigoProducto = up.Producto,
                UbicacionSeparacion = up.Ubicacion
            }));

            foreach (var ubicacionPickingZona in ubicacionPickingZonas)
            {
                var key = $"{ubicacionPickingZona.Producto}.{ubicacionPickingZona.Ubicacion}";

                if (!resultado.ContainsKey(key))
                    resultado[key] = new List<string>();

                resultado[key].Add(ubicacionPickingZona.Zona);
            }

            return resultado;
        }

        protected virtual Dictionary<string, List<ProductoAutomatismoRequest>> GetProductosAutomatismoByUbicacionesPicking(IUnitOfWork uow, int empresa, Dictionary<string, UbicacionPickingAutomatismoRequest> ubicacionesPickingPorKey)
        {
            var resultado = new Dictionary<string, List<ProductoAutomatismoRequest>>();
            var ubicaciones = ubicacionesPickingPorKey.Values;

            ubicaciones.ForEach(u => { u.Empresa = empresa; });

            var productosZonas = uow.AutomatismoRepository.GetProductosAutomatismoByUbicacionesPicking(ubicaciones);

            foreach (var productoZona in productosZonas)
            {
                if (!resultado.ContainsKey(productoZona.Zona))
                    resultado[productoZona.Zona] = new List<ProductoAutomatismoRequest>();

                resultado[productoZona.Zona].Add(productoZona);
            }

            return resultado;
        }

        protected virtual Dictionary<string, List<CodigoBarraAutomatismoRequest>> GetCodigosBarrasAutomatismoByUbicacionesPicking(IUnitOfWork uow, int empresa, Dictionary<string, UbicacionPickingAutomatismoRequest> ubicacionesPickingPorKey)
        {
            var resultado = new Dictionary<string, List<CodigoBarraAutomatismoRequest>>();

            //Solo se mandan Codigos de Barra cuando es alto de ubi picking, si es modificacion no se vuelven a enviar
            var ubicaciones = ubicacionesPickingPorKey.Values.Where(w => w.TipoOperacion == TipoOperacionDb.Alta);

            ubicaciones.ForEach(u => { u.Empresa = empresa; });

            var codigosZonas = uow.AutomatismoRepository.GetCodigosBarrasAutomatismoByUbicacionesPicking(ubicaciones);

            foreach (var codigoZona in codigosZonas)
            {
                if (!resultado.ContainsKey(codigoZona.Zona))
                    resultado[codigoZona.Zona] = new List<CodigoBarraAutomatismoRequest>();

                resultado[codigoZona.Zona].Add(codigoZona);
            }

            return resultado;
        }

        protected virtual Dictionary<string, int> GetEmpresasAutomatismoByUbicacionesPicking(IUnitOfWork uow, List<AjusteStockRequest> ajustes)
        {
            var resultado = new Dictionary<string, int>();
            var ubicaciones = uow.AutomatismoRepository.GetEmpresasAutomatismoByUbicacionesPicking(ajustes);

            foreach (var ubicacion in ubicaciones)
            {
                var key = $"{ubicacion.CodigoProducto}.{ubicacion.UbicacionSeparacion}";
                resultado[key] = ubicacion.Empresa;
            }

            return resultado;
        }

        protected virtual HashSet<string> GetProductosAutomatismo(IUnitOfWork uow, string zona, int empresa, List<EntradaStockLineaAutomatismoRequest> detalles)
        {
            var resultado = new HashSet<string>();
            var productos = uow.AutomatismoRepository.GetProductosAutomatismo(zona, empresa, detalles);

            foreach (var producto in productos)
            {
                if (!resultado.Contains(producto))
                    resultado.Add(producto);
            }

            return resultado;
        }

        public bool IsValidUser(UsuarioRequest usuario)
        {
            if (usuario != null)
            {
                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    return SecurityLogic.IsValidUser(uow, usuario.LoginName, usuario.Hash);
                }
            }

            return false;
        }

        public bool IsValidUser(UsuarioRequest usuario, string puesto)
        {
            bool isValid = false;

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (usuario != null)
                {
                    isValid = SecurityLogic.IsValidUser(uow, usuario.LoginName, usuario.Hash);
                }

                if (!isValid)
                {
                    string loginNameGenerico = uow.AutomatismoCaracteristicaRepository.GetAutomatismoCaracteristicaByPuestoAndCodigo(puesto, CaracteristicasAutomatismoDb.USERDEFAULT)?.Valor;

                    if (!string.IsNullOrEmpty(loginNameGenerico))
                    {
                        var user = uow.SecurityRepository.GetUser(loginNameGenerico);

                        if (user?.IsEnabled ?? false)
                        {
                            usuario.LoginName = loginNameGenerico;
                            usuario.Hash = Signer.ComputeHash(SecurityLogic.GetSecret(uow), usuario.LoginName);
                            isValid = true;
                        }
                    }
                }
            }

            return isValid;
        }

        public virtual async Task<ValidationsResult> ProcesarConfirmacionMovimiento(string puesto, ConfirmacionMovimientoStockRequest request, AutomatismoEjecucion ejecucion)
        {
            var response = new ValidationsResult();
            var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (request.Detalles.Count > _configuration.Value.MaxConfirmacionEntrada)
                {
                    validaciones.Add(new Error("WMSAPI_msg_Error_AutomatismoCantidadLineas", _configuration.Value.MaxConfirmacionEntrada));
                    var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                    response.Errors.Add(new ValidationsError(0, false, messages));
                    return response;
                }

                var valEnvInt = await this._validationService.ValidarEnvioInterfazByPuesto(uow, puesto, CodigoInterfazAutomatismoDb.CD_INTERFAZ_CONF_ENTRADAS);

                if (valEnvInt.HasError())
                    return valEnvInt;

                var automatismo = this.GetByPuesto(uow, puesto);

                ejecucion.SetAutomatismo(automatismo);

                var posicionEntrada = automatismo.GetPosiciones(AutomatismoPosicionesTipoDb.POS_ENTRADA).FirstOrDefault();
                var posicionPicking = automatismo.GetPosiciones(AutomatismoPosicionesTipoDb.POS_PICKING).FirstOrDefault();

                if (request.Empresa == -1)
                    request.Empresa = int.Parse((automatismo.GetCaracteristicaByCodigo(AutomatismoDb.CARACTERISTICA_EMPRESA_POR_DEFECTO)?.Valor) ?? "1");

                if (request.TipoMovimiento == AutomatismoTipoMovimientoGalys.SALIDA_MANUAL_AUTOSTORE
                    || request.TipoMovimiento == AutomatismoTipoMovimientoGalys.ENTRADA_AUTOSTORE)
                {
                    string ubiOrigen = posicionEntrada.IdUbicacion;
                    string ubiDestino = posicionPicking.IdUbicacion;

                    if (request.TipoMovimiento == AutomatismoTipoMovimientoGalys.SALIDA_MANUAL_AUTOSTORE)
                    {
                        ubiOrigen = posicionPicking.IdUbicacion;
                        ubiDestino = posicionEntrada.IdUbicacion;

                        request.Detalles.ForEach(s => s.Cantidad = s.Cantidad * -1); //En las salidas manuales llega en negativo
                    }

                    var transferencias = _movimientoStockAutomatismoMapper.Map(request, ubiOrigen, ubiDestino);

                    request.DsReferencia = automatismo.Codigo;

                    var context = await GetNewServiceContext(uow, transferencias, request, ejecucion.UserId);

                    validaciones.AddRange(_validationService.ValidateConfirmacionMovimientoAutomatismo(transferencias, context, out bool errorProcedimiento));

                    if (validaciones.Count > 0)
                    {
                        var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                        response.Errors.Add(new ValidationsError(0, errorProcedimiento, messages));
                    }

                    if (response.HasError())
                        return response;

                    var service = _serviceFactory.Create(automatismo);

                    response = service.ProcesarConfirmacionMovimiento(uow, automatismo, transferencias, context);

                    if (response.HasError())
                        return response;

                }
                else if (request.TipoMovimiento == AutomatismoTipoMovimientoGalys.SALIDA_AUTOSTORE)
                {
                    //no se envía el contenedor, por lo tanto se prepara en la confirmación de salida

                    return response;
                }
                else if (request.TipoMovimiento == AutomatismoTipoMovimientoGalys.AJUSTE_NEGATIVO_AUTOSTORE
                         || request.TipoMovimiento == AutomatismoTipoMovimientoGalys.AJUSTE_POSITIVO_AUTOSTORE)
                {

                    var motivo = "";

                    if (request.TipoMovimiento == AutomatismoTipoMovimientoGalys.AJUSTE_NEGATIVO_AUTOSTORE)
                        motivo = (automatismo.GetCaracteristicaByCodigo(AutomatismoDb.CARACTERISTICA_MOTIVO_AJUSTE_BAJA)?.Valor) ?? CaracteristicasAutomatismoDb.CD_MOTIVO_AJUSTE_BAJA;
                    else
                        motivo = (automatismo.GetCaracteristicaByCodigo(AutomatismoDb.CARACTERISTICA_MOTIVO_AJUSTE_ALTA)?.Valor) ?? CaracteristicasAutomatismoDb.CD_MOTIVO_AJUSTE_ALTA;


                    var ajustesConfirmados = this._ajusteStockAutomatismoMapper.Map(request, posicionPicking.IdUbicacion, motivo);


                    var context = await GetNewServiceContext(uow, ajustesConfirmados, request, ejecucion.UserId);

                    var service = _serviceFactory.Create(automatismo);

                    response = service.ProcesarNotificacionAjustes(uow, automatismo, ajustesConfirmados);

                    if (response.HasError())
                        return response;

                }
                else
                {
                    validaciones.Add(new Error("WMSAPI_msg_Error_TipoMovimientoNoImplementado", request.TipoMovimiento));
                    var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                    response.Errors.Add(new ValidationsError(0, false, messages));
                    return response;
                }

            }

            return response;
        }

        protected virtual async Task<AutomatismoConfirmacionMovimientoServiceContext> GetNewServiceContext(IUnitOfWork uow, TransferenciaStockRequest entradasConfirmadas, ConfirmacionMovimientoStockRequest confMovimiento, int userId)
        {
            var empresa = confMovimiento.Empresa;
            var context = new AutomatismoConfirmacionMovimientoServiceContext(uow, entradasConfirmadas, confMovimiento, userId);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }
        protected virtual async Task<AutomatismoConfirmacionMovimientoServiceContext> GetNewServiceContext(IUnitOfWork uow, AjustesDeStockRequest ajustesConfirmadas, ConfirmacionMovimientoStockRequest confMovimiento, int userId)
        {
            var empresa = confMovimiento.Empresa;
            var context = new AutomatismoConfirmacionMovimientoServiceContext(uow, ajustesConfirmadas, confMovimiento, userId);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        protected virtual void AddParametros(AutomatismoConfirmacionMovimientoServiceContext context, int empresa)
        {
            context.AddParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
        }

    }
}
