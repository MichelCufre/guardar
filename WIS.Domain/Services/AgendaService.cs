using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;
using WIS.Security;

namespace WIS.Domain.Services
{
    public class AgendaService : IAgendaService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IValidationService _validationService;
        protected readonly IOptions<MaxItemsSettings> _configuration;
        protected readonly IIdentityService _identity;

        public AgendaService(
            IUnitOfWorkFactory uowFactory,
            IValidationService validationService,
            IOptions<MaxItemsSettings> configuration,
            IIdentityService identity)
        {
            _uowFactory = uowFactory;
            _validationService = validationService;
            _configuration = configuration;
            _identity = identity;
        }

        public virtual async Task<Agenda> GetAgenda(int nuAgenda)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return await uow.AgendaRepository.GetAgendaOrNull(nuAgenda);
            }
        }

        public virtual async Task<ValidationsResult> AgregarAgendas(int empresa, List<Agenda> agendas, int userId)
        {
            var result = new ValidationsResult();
            var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (uow.EmpresaRepository.IsEmpresaDocumental(empresa))
                {
                    validaciones.Add(new Error("WMSAPI_msg_Error_OperacionNoDisponibleDocumental"));
                    var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                    result.Errors.Add(new ValidationsError(1, true, messages));
                    return result;
                }

                int nroRegistro = 1;

                if (agendas.Count > 0)
                {
                    uow.CreateTransactionNumber(this._identity.Application);

                    var keys = new HashSet<string>();
                    int maxItems = _configuration.Value.Agenda;

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, agendas.Count, maxItems))
                        return result;

                    var context = await GetNewServiceContext(uow, agendas, userId);

                    foreach (var agenda in agendas)
                    {
                        validaciones.Clear();
                        agenda.NumeroTransaccion = uow.GetTransactionNumber();

                        if (!string.IsNullOrEmpty(agenda.NumeroDocumento))
                        {
                            string key = $"{agenda.NumeroDocumento}.{agenda.IdEmpresa}.{agenda.TipoReferenciaId}.{agenda.TipoAgente}.{agenda.CodigoAgente}";
                            if (keys.Contains(key))
                            {
                                validaciones.Add(new Error("WMSAPI_msg_Error_ReferenciasDuplicadas", agenda.NumeroDocumento, agenda.IdEmpresa, agenda.TipoReferenciaId, agenda.TipoAgente, agenda.CodigoAgente));
                                var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                                result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                                break;
                            }
                            else
                                keys.Add(key);
                        }

                        validaciones.AddRange(await _validationService.ValidateAgenda(agenda, context, out bool errorProcedimiento));

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, errorProcedimiento, messages));
                        }

                        nroRegistro++;
                    }

                    if (result.HasError())
                        return result;

                    await uow.AgendaRepository.AddAgendas(agendas, context);
                }
                else
                {
                    var msg = Translator.Translate(uow, new Error("WMSAPI_msg_Error_ListaDeObjetosRequerida"), _identity.UserId);
                    result.Errors.Add(new ValidationsError(nroRegistro, false, new List<string>() { msg }));
                }
            }

            return result;
        }

        public virtual async Task<IAgendaServiceContext> GetNewServiceContext(IUnitOfWork uow, List<Agenda> agendas, int userId)
        {
            var empresa = agendas[0].IdEmpresa;
            var context = new AgendaServiceContext(uow, agendas, userId, empresa);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        public virtual void AddParametros(IAgendaServiceContext context, int empresa)
        {
        }
    }
}
