using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Picking.Dtos;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;
using WIS.Security;

namespace WIS.Domain.Services
{
    public class CrossDockingService : ICrossDockingService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IValidationService _validationService;
        protected readonly IOptions<MaxItemsSettings> _configuration;
        protected readonly IIdentityService _identity;
        protected readonly IBarcodeService _barcodeService;

        public CrossDockingService(IUnitOfWorkFactory uowFactory, IValidationService validationService, IOptions<MaxItemsSettings> configuration, IIdentityService identity, IBarcodeService barcodeService)
        {
            _uowFactory = uowFactory;
            _validationService = validationService;
            _configuration = configuration;
            _identity = identity;
            _barcodeService = barcodeService;
        }

        public virtual async Task<ValidationsResult> CrossDockingUnaFase(List<CrossDockingUnaFase> detalles, int userId)
        {
            var result = new ValidationsResult();
            var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                    var nroRegistro = 1;

                if (detalles.Count > 0)
                {
                    var aplicacion = _identity.Application;

                    if (aplicacion.Length > 30)
                        aplicacion = aplicacion.Substring(0, 30);

                    uow.CreateTransactionNumber(aplicacion);

                    var keys = new HashSet<string>();
                    var keysAgenda = new List<string>();

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, detalles.Count, _configuration.Value.CrossDockingUnaFase))
                        return result;

                    var context = await GetNewServiceContext(uow, detalles, userId);

                    foreach (var detalle in detalles)
                    {
                        validaciones.Clear();

                        detalle.NuTransaccion = uow.GetTransactionNumber();
                        detalle.CdAplicacion = aplicacion;
                        
                        var keyAgendaUbicacion = $"{detalle.Agenda}.{detalle.Ubicacion}";
                        var keyAgenda = $"{detalle.Agenda}";

                        if (!keysAgenda.Contains(keyAgendaUbicacion) && keysAgenda.Where(x => x == keyAgenda).Count() == 0)
                            keysAgenda.Add(keyAgendaUbicacion);
                        else if (!keysAgenda.Contains(keyAgendaUbicacion) && keysAgenda.Where(x => x.Contains(keyAgenda)).Count() == 1)
                            validaciones.Add(new Error("WMSAPI_msg_Error_CrossDockingDuplicadoAgenda", detalle.Agenda, detalle.Ubicacion));

                        var key = $"{detalle.IdExternoContenedor}.{detalle.TipoContenedor}.{detalle.Preparacion}.{detalle.CodigoAgente}.{detalle.TipoAgente}.{detalle.Empresa}.{detalle.Producto}.{detalle.Identificador}.{detalle.Agenda}";

                        if (keys.Contains(key))
                            validaciones.Add(new Error("WMSAPI_msg_Error_CrossDockingDuplicadoContenedor", detalle.IdExternoContenedor, detalle.TipoContenedor, detalle.Preparacion, detalle.CodigoAgente, detalle.TipoAgente, detalle.Producto, detalle.Empresa, detalle.Identificador, detalle.Agenda));
                        else
                            keys.Add(key);

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                            break;
                        }

                        validaciones.AddRange(await _validationService.ValidateCrossDocking(detalle, context, out bool errorProcedimiento));

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                            break;
                        }

                        nroRegistro++;
                    }

                    if (result.HasError())
                        return result;

                    var detallesAgrupados = detalles.GroupBy(x => new { x.Empresa, x.Agenda, x.Producto, x.Identificador, x.Cliente, x.Preparacion })
                    .Select(x => new CrossDockingUnaFase
                    {
                        Empresa = x.Key.Empresa,
                        Agenda = x.Key.Agenda,
                        Producto = x.Key.Producto,
                        Identificador = x.Key.Identificador,
                        Cliente = x.Key.Cliente,
                        Preparacion = x.Key.Preparacion,
                        Cantidad = x.Sum(d => d.Cantidad)
                    });

                    foreach (var detalle in detallesAgrupados)
                    {
                        validaciones.AddRange(await _validationService.ValidateDetalleCrossDocking(detalle, context, out bool errorProcedimiento));

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                            break;
                        }

                        nroRegistro++;
                    }

                    if (result.HasError())
                        return result;

                    await uow.CrossDockingRepository.ProcesarCrossDockingUnaFase(detalles, context, uow.GetTransactionNumber(), userId, _barcodeService);
                }
                else
                {
                    var msg = Translator.Translate(uow, new Error("WMSAPI_msg_Error_ListaDeObjetosRequerida"), _identity.UserId);
                    result.Errors.Add(new ValidationsError(nroRegistro, false, new List<string>() { msg }));
                }
            }

            return result;
        }

        public virtual async Task<ICrossDockingServiceContext> GetNewServiceContext(IUnitOfWork uow, List<CrossDockingUnaFase> detalles, int userId)
        {
            var empresa = detalles[0].Empresa;
            var context = new CrossDockingServiceContext(uow, detalles, userId, empresa);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        public virtual void AddParametros(ICrossDockingServiceContext context, int empresa)
        {
        }
    }
}
