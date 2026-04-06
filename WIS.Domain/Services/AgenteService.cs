using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;
using WIS.Security;

namespace WIS.Domain.Services
{
    public class AgenteService : IAgenteService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IValidationService _validationService;
        protected readonly IOptions<MaxItemsSettings> _configuration;
        protected readonly IIdentityService _identity;

        public AgenteService(IUnitOfWorkFactory uowFactory,
            IValidationService validationService,
            IOptions<MaxItemsSettings> configuration,
            IIdentityService identity)
        {
            _uowFactory = uowFactory;
            _validationService = validationService;
            _configuration = configuration;
            _identity = identity;
        }

        public virtual async Task<Agente> GetAgente(string codigo, int empresa, string tipo)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return await uow.AgenteRepository.GetAgenteOrNull(empresa, codigo, tipo);
            }
        }

        public virtual async Task<Agente> GetAgente(string codigoInterno, int empresa)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return await uow.AgenteRepository.GetAgenteOrNull(empresa, codigoInterno);
            }
        }

        public virtual async Task<Dictionary<string, Agente>> GetAgentesEgreso(int cdCamion)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return await uow.AgenteRepository.GetAgentesEgreso(cdCamion);
            }
        }

        public virtual async Task<ValidationsResult> AgregarAgentes(List<Agente> agentes, int userId)
        {
            var result = new ValidationsResult();
            var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                int nroRegistro = 1;

                if (agentes.Count > 0)
                {
                    uow.CreateTransactionNumber(this._identity.Application);

                    var keys = new HashSet<string>();
                    int maxItems = _configuration.Value.Agente;

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, agentes.Count, maxItems))
                        return result;

                    var context = await GetNewServiceContext(uow, agentes, userId);

                    foreach (var agente in agentes)
                    {
                        validaciones.Clear();
                        string key = $"{agente.Codigo}.{agente.Empresa}.{agente.Tipo}";

                        agente.Transaccion = uow.GetTransactionNumber();

                        if (keys.Contains(key))
                        {
                            validaciones.Add(new Error("WMSAPI_msg_Error_AgentesDuplicados", agente.Codigo, agente.Empresa, agente.Tipo));
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                            break;
                        }
                        else
                            keys.Add(key);

                        validaciones.AddRange(await _validationService.ValidateAgente(agente, context, out bool errorProcedimiento));

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, errorProcedimiento, messages));
                        }

                        nroRegistro++;
                    }

                    if (result.HasError())
                        return result;

                    await uow.AgenteRepository.AddAgentes(agentes, context);
                }
                else
                {
                    var msg = Translator.Translate(uow, new Error("WMSAPI_msg_Error_ListaDeObjetosRequerida"), _identity.UserId);
                    result.Errors.Add(new ValidationsError(nroRegistro, false, new List<string>() { msg }));
                }
            }

            return result;
        }

        public virtual async Task<IAgenteServiceContext> GetNewServiceContext(IUnitOfWork uow, List<Agente> agentes, int userId)
        {
            var empresa = agentes[0].Empresa;
            var context = new AgenteServiceContext(uow, agentes, userId, empresa);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        public virtual void AddParametros(IAgenteServiceContext context, int empresa)
        {
            context.AddParametro(ParamManager.IE_507_CD_ROTA);
            context.AddParametro(ParamManager.IE_507_CD_SITUACAO);

            context.AddParametroEmpresa(ParamManager.IE_507_CAMPOS_INMUTABLES, empresa);

            context.SetParametroCamposInmutables(ParamManager.IE_507_CAMPOS_INMUTABLES);
        }
    }
}
