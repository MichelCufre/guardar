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
    public class EmpresaService : IEmpresaService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IValidationService _validationService;
        protected readonly IOptions<MaxItemsSettings> _configuration;
        protected readonly IIdentityService _identity;

        public EmpresaService(IUnitOfWorkFactory uowFactory,
            IValidationService validationService,
            IOptions<MaxItemsSettings> configuration,
            IIdentityService identity)
        {
            _uowFactory = uowFactory;
            _validationService = validationService;
            _configuration = configuration;
            _identity = identity;
        }

        public virtual async Task<Empresa> GetEmpresa(int codigo)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return await uow.EmpresaRepository.GetEmpresaOrNull(codigo);
            }
        }

        public virtual async Task<ValidationsResult> AgregarEmpresas(List<Empresa> empresas, int empresaEjecucion, int userId)
        {
            var result = new ValidationsResult();
            var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                int nroRegistro = 1;

                if (empresas.Count > 0)
                {
                    var keys = new HashSet<string>();
                    int maxItems = _configuration.Value.Empresa;

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, empresas.Count, maxItems))
                        return result;

                    var context = await GetNewServiceContext(uow, empresas, empresaEjecucion, userId);

                    foreach (var empresa in empresas)
                    {
                        validaciones.Clear();
                        string key = $"{empresa.Id}";

                        if (keys.Contains(key))
                        {
                            validaciones.Add(new Error("WMSAPI_msg_Error_EmpresasDuplicadas", empresa.Id));
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                            break;
                        }
                        else
                            keys.Add(key);

                        validaciones.AddRange(await _validationService.ValidateEmpresa(empresa, context, out bool errorProcedimiento));

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, errorProcedimiento, messages));
                        }

                        nroRegistro++;
                    }

                    if (result.HasError())
                        return result;

                    await uow.EmpresaRepository.AddEmpresas(empresas, context);
                }
                else
                {
                    var msg = Translator.Translate(uow, new Error("WMSAPI_msg_Error_ListaDeObjetosRequerida"), _identity.UserId);
                    result.Errors.Add(new ValidationsError(nroRegistro, false, new List<string>() { msg }));
                }
            }

            return result;
        }

        public virtual async Task<IEmpresaServiceContext> GetNewServiceContext(IUnitOfWork uow, List<Empresa> empresas, int empresaEjecucion, int userId)
        {
            var context = new EmpresaServiceContext(uow, empresas, userId, empresaEjecucion);

            AddParametros(context, empresaEjecucion);

            await context.Load();

            return context;
        }

        public virtual void AddParametros(IEmpresaServiceContext context, int empresaEjecucion)
        {
            context.AddParametro(ParamManager.IE_522_CD_SITUACAO);
            context.AddParametro(ParamManager.IE_522_TIPOS_RECEPCION);

            context.AddParametroEmpresa(ParamManager.IE_522_CAMPOS_INMUTABLES, empresaEjecucion);

            context.SetParametroCamposInmutables(ParamManager.IE_522_CAMPOS_INMUTABLES);
        }

        public virtual byte[] GetFirma(int empresa, string contenido)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return uow.EmpresaRepository.GetFirma(empresa, contenido);
            }
        }

        public virtual async Task UpdateLock(int empresa, bool isLocked)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                await uow.EmpresaRepository.UpdateLock(empresa, isLocked);
            }
        }
    }
}
