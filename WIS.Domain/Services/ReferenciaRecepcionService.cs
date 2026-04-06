using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;
using WIS.Security;

namespace WIS.Domain.Services
{
    public class ReferenciaRecepcionService : IReferenciaRecepcionService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IValidationService _validationService;
        protected readonly IOptions<MaxItemsSettings> _configuration;
        protected readonly IIdentityService _identity;

        public ReferenciaRecepcionService(IUnitOfWorkFactory uowFactory,
            IValidationService validationService,
            IOptions<MaxItemsSettings> configuration,
            IIdentityService identity)
        {
            _validationService = validationService;
            _uowFactory = uowFactory;
            _configuration = configuration;
            _identity = identity;
        }

        public virtual async Task<ReferenciaRecepcion> GetReferenciaById(int idReferencia)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return await uow.ReferenciaRecepcionRepository.GetReferenciaOrNull(idReferencia);
            }
        }

        public virtual async Task<ReferenciaRecepcion> GetReferencia(string nuReferencia, int codigoEmpresa, string tipo, string tipoAgente, string codigoAgente)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return await uow.ReferenciaRecepcionRepository.GetReferenciaOrNull(nuReferencia, codigoEmpresa, tipo, tipoAgente, codigoAgente);
            }
        }

        #region AgregarReferencias

        public virtual async Task<ValidationsResult> AgregarReferencias(List<ReferenciaRecepcion> referencias, int userId)
        {
            var result = new ValidationsResult();
            var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                int nroRegistro = 1;

                if (referencias.Count > 0)
                {
                    uow.CreateTransactionNumber(this._identity.Application);

                    int countDetalles = 0;
                    var keys = new HashSet<string>();
                    int maxItems = _configuration.Value.ReferenciaRecepcion;

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, referencias.Count, maxItems))
                        return result;

                    var context = await GetNewServiceContext(uow, referencias, userId);

                    foreach (var referencia in referencias)
                    {
                        validaciones.Clear();
                        referencia.NumeroTransaccion = uow.GetTransactionNumber();

                        string keyReferencia = $"{referencia.Numero}.{referencia.IdEmpresa}.{referencia.TipoReferencia}.{referencia.TipoAgente}.{referencia.CodigoAgente}";
                        if (keys.Contains(keyReferencia))
                        {
                            validaciones.Add(new Error("WMSAPI_msg_Error_ReferenciasDuplicadas", referencia.Numero, referencia.IdEmpresa, referencia.TipoReferencia, referencia.TipoAgente, referencia.CodigoAgente));
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                            break;
                        }
                        else
                            keys.Add(keyReferencia);

                        validaciones.AddRange(await _validationService.ValidateReferenciaRecepcion(referencia, context, out bool errorProcedimiento));

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, errorProcedimiento, messages));
                        }

                        countDetalles += referencia.Detalles.Count;
                        nroRegistro++;
                    }

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, countDetalles, maxItems, false))
                        return result;

                    if (result.HasError())
                        return result;

                    await uow.ReferenciaRecepcionRepository.AddReferencias(referencias, context);
                }
                else
                {
                    var msg = Translator.Translate(uow, new Error("WMSAPI_msg_Error_ListaDeObjetosRequerida"), _identity.UserId);
                    result.Errors.Add(new ValidationsError(nroRegistro, false, new List<string>() { msg }));
                }
            }

            return result;
        }

        public virtual async Task<IReferenciaRecepcionServiceContext> GetNewServiceContext(IUnitOfWork uow, List<ReferenciaRecepcion> referencias, int userId)
        {
            var empresa = referencias[0].IdEmpresa;
            IReferenciaRecepcionServiceContext context = new ReferenciaRecepcionServiceContext(uow, referencias, userId, empresa);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        public virtual void AddParametros(IReferenciaRecepcionServiceContext context, int empresa)
        {
            context.AddParametroEmpresa(ParamManager.IE_510_VALIDAR_FECHAS, empresa);
        }

        #endregion

        #region ModificarReferencias

        public virtual async Task<ValidationsResult> ModificarReferencias(List<ReferenciaRecepcion> referencias, int userId)
        {
            var result = new ValidationsResult();
            var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                int nroRegistro = 1;

                if (referencias.Count > 0)
                {
                    uow.CreateTransactionNumber(this._identity.Application);

                    int countDetalles = 0;
                    var keys = new HashSet<string>();
                    int maxItems = _configuration.Value.ModificarDetalleReferencia;

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, referencias.Count, maxItems))
                        return result;

                    var context = await GetNewModificarDetalleReferenciaServiceContext(uow, referencias, userId);

                    foreach (var referencia in referencias)
                    {
                        validaciones.Clear();
                        referencia.NumeroTransaccion = uow.GetTransactionNumber();

                        string key = $"{referencia.Numero}.{referencia.IdEmpresa}.{referencia.TipoReferencia}.{referencia.TipoAgente}.{referencia.CodigoAgente}";
                        if (keys.Contains(key))
                        {
                            validaciones.Add(new Error("WMSAPI_msg_Error_ReferenciasDuplicadas", referencia.Numero, referencia.IdEmpresa, referencia.TipoReferencia, referencia.TipoAgente, referencia.CodigoAgente));
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                            break;
                        }
                        else
                            keys.Add(key);

                        validaciones.AddRange(await _validationService.ValidateModificarDetalleReferencia(referencia, context, out bool errorProcedimiento));

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, errorProcedimiento, messages));
                        }

                        countDetalles += referencia.Detalles.Count;
                        nroRegistro++;
                    }

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, countDetalles, maxItems, false))
                        return result;

                    if (result.HasError())
                        return result;

                    await uow.ReferenciaRecepcionRepository.ModificarReferencias(referencias, context);
                }
                else
                {
                    var msg = Translator.Translate(uow, new Error("WMSAPI_msg_Error_ListaDeObjetosRequerida"), _identity.UserId);
                    result.Errors.Add(new ValidationsError(nroRegistro, false, new List<string>() { msg }));
                }
            }

            return result;
        }

        public virtual async Task<IModificarDetalleReferenciaServiceContext> GetNewModificarDetalleReferenciaServiceContext(IUnitOfWork uow, List<ReferenciaRecepcion> referencias, int userId)
        {
            var empresa = referencias[0].IdEmpresa;
            IModificarDetalleReferenciaServiceContext context = new ModificarDetalleReferenciaServiceContext(uow, referencias, userId, empresa);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        public virtual void AddParametros(IModificarDetalleReferenciaServiceContext context, int empresa)
        {
        }
        
        #endregion

        #region AnularReferencias

        public virtual async Task<ValidationsResult> AnularReferencias(List<ReferenciaRecepcion> referencias, int userId)
        {
            var result = new ValidationsResult();
            var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                int nroRegistro = 1;

                if (referencias.Count > 0)
                {
                    uow.CreateTransactionNumber(this._identity.Application);

                    var keys = new HashSet<string>();
                    int maxItems = _configuration.Value.AnulacionReferenciaRecepcion;

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, referencias.Count, maxItems))
                        return result;

                    var context = await GetNewAnularReferenciaServiceContext(uow, referencias, userId);

                    foreach (var referencia in referencias)
                    {
                        validaciones.Clear();
                        referencia.NumeroTransaccion = uow.GetTransactionNumber();

                        string key = $"{referencia.Numero}.{referencia.IdEmpresa}.{referencia.TipoReferencia}.{referencia.TipoAgente}.{referencia.CodigoAgente}";
                        if (keys.Contains(key))
                        {
                            validaciones.Add(new Error("WMSAPI_msg_Error_ReferenciasDuplicadas", referencia.Numero, referencia.IdEmpresa, referencia.TipoReferencia, referencia.TipoAgente, referencia.CodigoAgente));
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                            break;
                        }
                        else
                            keys.Add(key);

                        validaciones.AddRange(await _validationService.ValidateAnularReferencia(referencia, context, out bool errorProcedimiento));

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, errorProcedimiento, messages));
                        }

                        nroRegistro++;
                    }

                    if (result.HasError())
                        return result;

                    await uow.ReferenciaRecepcionRepository.AnularReferencias(referencias, context);
                }
                else
                {
                    var msg = Translator.Translate(uow, new Error("WMSAPI_msg_Error_ListaDeObjetosRequerida"), _identity.UserId);
                    result.Errors.Add(new ValidationsError(nroRegistro, false, new List<string>() { msg }));
                }
            }

            return result;
        }

        public virtual async Task<IAnularReferenciaServiceContext> GetNewAnularReferenciaServiceContext(IUnitOfWork uow, List<ReferenciaRecepcion> referencias, int userId)
        {
            var empresa = referencias[0].IdEmpresa;
            IAnularReferenciaServiceContext context = new AnularReferenciaServiceContext(uow, referencias, userId, empresa);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        public virtual void AddParametros(IAnularReferenciaServiceContext context, int empresa)
        {
        }

        #endregion
    }
}
