using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Domain.Validation;
using WIS.Security;

namespace WIS.Domain.Services
{
    public class LpnService : ILpnService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IValidationService _validationService;
        protected readonly IOptions<MaxItemsSettings> _configuration;
        protected readonly IIdentityService _identity;

        public LpnService(IUnitOfWorkFactory uowFactory,
            IValidationService validationService,
            IOptions<MaxItemsSettings> configuration,
            IIdentityService identity)
        {
            _uowFactory = uowFactory;
            _validationService = validationService;
            _configuration = configuration;
            _identity = identity;
        }

        public virtual async Task<Lpn> GetLpn(long nuLpn)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return await uow.ManejoLpnRepository.GetLpnOrNull(nuLpn);
            }
        }

        public virtual async Task<Lpn> GetLpn(string idExterno, string tpLpn)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return await uow.ManejoLpnRepository.GetLpnOrNull(idExterno, tpLpn);
            }
        }

        public virtual async Task<ValidationsResult> AgregarLpns(List<Lpn> lpns, int empresa, int userId)
        {
            var result = new ValidationsResult();
            var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                int nroRegistro = 1;

                if (lpns.Count > 0)
                {
                    uow.CreateTransactionNumber(this._identity.Application);

                    int countDetalles = 0;
                    var keys = new HashSet<string>();
                    int maxItems = _configuration.Value.Lpn;

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, lpns.Count, maxItems))
                        return result;

                    var context = await GetNewServiceContext(uow, lpns, empresa, userId);

                    foreach (var lpn in lpns)
                    {
                        validaciones.Clear();

                        lpn.NumeroTransaccion = uow.GetTransactionNumber();

                        if (keys.Contains(lpn.IdExterno))
                        {
                            validaciones.Add(new Error("WMSAPI_msg_Error_LpnIdentificadoresDuplicados", lpn.IdExterno));
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                            break;
                        }
                        else
                            keys.Add(lpn.IdExterno);

                        validaciones.AddRange(await _validationService.ValidateLpn(uow, lpn, context, out bool errorProcedimiento));

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, errorProcedimiento, messages));
                        }

                        countDetalles += lpn.Detalles.Count;
                        countDetalles += lpn.AtributosSinDefinir.Count;

                        nroRegistro++;
                    }

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, countDetalles, maxItems, true))
                        return result;

                    if (result.HasError())
                        return result;

                    await uow.ManejoLpnRepository.AddLpns(lpns, context);
                }
                else
                {
                    var msg = Translator.Translate(uow, new Error("WMSAPI_msg_Error_ListaDeObjetosRequerida"), _identity.UserId);
                    result.Errors.Add(new ValidationsError(nroRegistro, false, new List<string>() { msg }));
                }
            }

            return result;
        }

        public virtual async Task<ILpnServiceContext> GetNewServiceContext(IUnitOfWork uow, List<Lpn> lpns, int empresa, int userId)
        {
            var context = new LpnServiceContext(uow, lpns, empresa, userId);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        public virtual void AddParametros(ILpnServiceContext context, int empresa)
        {
            context.AddParametroEmpresa(ParamManager.IE_535_TP_LPN_TIPO, empresa);
            context.AddParametroEmpresa(ParamManager.GENERAR_CB_ID_EXTERNO_LPN, empresa);
        }
    }
}
