using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
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
    public class PickingProductoService : IPickingProductoService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IValidationService _validationService;
        protected readonly IOptions<MaxItemsSettings> _configuration;
        protected readonly IIdentityService _identity;
        protected readonly IAutomatismoAutoStoreClientService _automatismoAutoStoreClientService;

        public PickingProductoService(IUnitOfWorkFactory uowFactory,
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

        public virtual async Task<ValidationsResult> AgregarUbicacionesDePicking(List<UbicacionPickingProducto> ubicacionesPicking, int userId)
        {
            var result = new ValidationsResult();
            var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                uow.CreateTransactionNumber(this._identity.Application);

                if (ubicacionesPicking.Count > 0)
                {
                    int nroRegistro = 1;
                    var keys = new HashSet<string>();
                    var keysPk = new HashSet<string>();

                    if (ubicacionesPicking.Count > _configuration.Value.UbicacionesPicking)
                    {
                        string msg = $"La cantidad de items enviados no puede superar el máximo de {_configuration.Value.UbicacionesPicking}.";
                        result.Errors.Add(new ValidationsError(nroRegistro, false, new List<string>() { msg }));
                        return result;
                    }

                    var context = await GetNewServiceContext(uow, ubicacionesPicking, userId);

                    foreach (var ubicacion in ubicacionesPicking)
                    {
                        ubicacion.NuTransaccion = uow.GetTransactionNumber();

                        string key = $"{ubicacion.UbicacionSeparacion}.{ubicacion.Empresa}.{ubicacion.CodigoProducto}.{ubicacion.Padron}.{ubicacion.Prioridad}";

                        if (keys.Contains(key))
                        {
                            validaciones.Add(new Error("WMSAPI_msg_Error_UbicacionesPickingDuplicadas", ubicacion.UbicacionSeparacion, ubicacion.Empresa, ubicacion.CodigoProducto, ubicacion.Padron, ubicacion.Prioridad));
                        }
                        else
                            keys.Add(key);

                        validaciones.AddRange(await _validationService.ValidateUbicacionesPicking(ubicacion, context, out bool errorProcedimiento));

                        string keyPk = $"{ubicacion.Predio}.{ubicacion.Empresa}.{ubicacion.CodigoProducto}.{ubicacion.Padron}.{ubicacion.Prioridad}";

                        if (keysPk.Contains(keyPk))
                        {
                            validaciones.Add(new Error("WMSAPI_msg_Error_UbicacionesPickingDuplicadasPk", ubicacion.Predio, ubicacion.Empresa, ubicacion.CodigoProducto, ubicacion.Padron, ubicacion.Prioridad));
                        }
                        else
                            keysPk.Add(keyPk);

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                        }

                        nroRegistro++;
                    }

                    if (result.HasError())
                        return result;

                    await uow.UbicacionPickingProductoRepository.AddUbicacionesPicking(ubicacionesPicking, context);
                }

                NotificarAutomatismo(uow, ubicacionesPicking);
            }

            return result;
        }

        public virtual void NotificarAutomatismo(IUnitOfWork uow, List<UbicacionPickingProducto> _ubicacionesPicking)
        {
            var ubicacionesPicking = new List<UbicacionPickingTipoOperacion>();

            foreach (var ubicacion in _ubicacionesPicking)
            {
                var ubicacionNotificar = new UbicacionPickingTipoOperacion();

                ubicacionNotificar.Ubicacion = ubicacion;
                ubicacionNotificar.TipoOperacion = ubicacion.TipoOperacionId;

                ubicacionesPicking.Add(ubicacionNotificar);
            }

            if (_automatismoAutoStoreClientService.IsEnabled() && ubicacionesPicking.Count > 0)
            {
                var nuTransaccion = uow.GetTransactionNumber();
                var ubicacionesPickingPorKey = new Dictionary<string, UbicacionPickingTipoOperacion>();
                var ubicacionesAutomatismo = uow.AutomatismoRepository.GetUbicacionesPickingAutomatismo(ubicacionesPicking.Select(up => new UbicacionPickingProducto
                {
                    UbicacionSeparacion = up.Ubicacion.UbicacionSeparacion,
                    Empresa = up.Ubicacion.Empresa,
                    CodigoProducto = up.Ubicacion.CodigoProducto,
                }));

                if (ubicacionesAutomatismo.Count() > 0)
                {
                    foreach (var ubicacionAutomatismo in ubicacionesAutomatismo)
                    {
                        var key = GetUbicacionPickingKey(ubicacionAutomatismo);
                        ubicacionesPickingPorKey[key] = new UbicacionPickingTipoOperacion();
                    }

                    foreach (var ubicacionPicking in ubicacionesPicking)
                    {
                        var key = GetUbicacionPickingKey(ubicacionPicking);

                        if (ubicacionesPickingPorKey.ContainsKey(key))
                            ubicacionesPickingPorKey[key] = ubicacionPicking;
                    }

                    var ubicacionesNotificablesPorEmpresa = new Dictionary<int, List<UbicacionPickingAutomatismoRequest>>();

                    foreach (var ubicacionPicking in ubicacionesPickingPorKey.Values)
                    {
                        var empresa = ubicacionPicking.Ubicacion.Empresa;

                        if (!ubicacionesNotificablesPorEmpresa.ContainsKey(empresa))
                            ubicacionesNotificablesPorEmpresa[empresa] = new List<UbicacionPickingAutomatismoRequest>();

                        ubicacionesNotificablesPorEmpresa[empresa].Add(new UbicacionPickingAutomatismoRequest
                        {
                            Ubicacion = ubicacionPicking.Ubicacion.UbicacionSeparacion,
                            Producto = ubicacionPicking.Ubicacion.CodigoProducto,
                            TipoOperacion = ubicacionPicking.TipoOperacion,
                        });
                    }

                    foreach (var empresa in ubicacionesNotificablesPorEmpresa.Keys)
                    {
                        _automatismoAutoStoreClientService.SendUbicacionesPicking(new UbicacionesPickingAutomatismoRequest
                        {
                            DsReferencia = nuTransaccion.ToString(),
                            Empresa = empresa,
                            Ubicaciones = ubicacionesNotificablesPorEmpresa[empresa],
                        });
                    }
                }
            }
        }

        public virtual async Task<PickingProductoServiceContext> GetNewServiceContext(IUnitOfWork uow, List<UbicacionPickingProducto> ubicacionesPicking, int userId)
        {
            var empresa = ubicacionesPicking[0].Empresa;
            var context = new PickingProductoServiceContext(uow, ubicacionesPicking, userId, empresa);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        public virtual void AddParametros(PickingProductoServiceContext context, int empresa)
        {
            context.AddParametroEmpresa(ParamManager.IE_570_CAMPOS_INMUTABLES, empresa);
            context.SetParametroCamposInmutables(ParamManager.IE_570_CAMPOS_INMUTABLES);

            context.AddParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
            context.AddParametro(ParamManager.IE_570_CODIGO_UND_CAJA_AUT);
            context.AddParametro(ParamManager.IE_570_CANT_UND_CAJA_AUT);
            context.AddParametro(ParamManager.IE_570_FL_CONFIRMAR_BARRA_AUT);

        }

        protected virtual string GetUbicacionPickingKey(UbicacionPickingProducto ubicacionPicking)
        {
            return $"{ubicacionPicking.Empresa}.{ubicacionPicking.CodigoProducto}.{ubicacionPicking.UbicacionSeparacion}";
        }

        protected virtual string GetUbicacionPickingKey(UbicacionPickingTipoOperacion ubicacionPicking)
        {
            return GetUbicacionPickingKey(ubicacionPicking.Ubicacion);
        }
    }
}
