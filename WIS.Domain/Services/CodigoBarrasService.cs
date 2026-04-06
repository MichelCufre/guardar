using Microsoft.Extensions.Options;
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
    public class CodigoBarrasService : ICodigoBarrasService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IValidationService _validationService;
        protected readonly IOptions<MaxItemsSettings> _configuration;
        protected readonly IIdentityService _identity;
        protected readonly IAutomatismoAutoStoreClientService _automatismoAutoStoreClientService;

        public CodigoBarrasService(IUnitOfWorkFactory uowFactory,
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

        public virtual async Task<CodigoBarras> GetCodigoDeBarras(string codigo, int empresa)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return await uow.CodigoBarrasRepository.GetCodigoBarrasOrNull(codigo, empresa);
            }
        }

        public virtual async Task<ValidationsResult> AgregarCodigosDeBarras(List<CodigoBarras> codigos, int userId)
        {
            var result = new ValidationsResult();
            var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                uow.CreateTransactionNumber(this._identity.Application);

                int nroRegistro = 1;

                if (codigos.Count > 0)
                {
                    var keys = new HashSet<string>();

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, codigos.Count, _configuration.Value.CodigoBarras))
                        return result;

                    var context = await GetNewServiceContext(uow, codigos, userId);

                    foreach (var codigo in codigos)
                    {
                        validaciones.Clear();
                        codigo.NumeroTransaccion = uow.GetTransactionNumber();

                        string key = $"{codigo.Codigo}.{codigo.Empresa}";
                        if (keys.Contains(key))
                        {
                            validaciones.Add(new Error("WMSAPI_msg_Error_CodigosDeBarrasDuplicados", codigo.Codigo, codigo.Empresa));
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                            break;
                        }
                        else
                            keys.Add(key);

                        validaciones.AddRange(await _validationService.ValidateCodigoBarras(codigo, context, out bool errorProcedimiento));

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                        }

                        nroRegistro++;
                    }

                    if (result.HasError())
                        return result;

                    await uow.CodigoBarrasRepository.AddCodigosBarras(codigos, context);

                    NotificarAutomatismo(uow, codigos);
                }
                else
                {
                    var msg = Translator.Translate(uow, new Error("WMSAPI_msg_Error_ListaDeObjetosRequerida"), _identity.UserId);
                    result.Errors.Add(new ValidationsError(nroRegistro, false, new List<string>() { msg }));
                }

            }

            return result;
        }

        public virtual void NotificarAutomatismo(IUnitOfWork uow, List<CodigoBarras> codigos)
        {
            if (_automatismoAutoStoreClientService.IsEnabled() && codigos.Count > 0)
            {
                var codigosAutomatismo = uow.AutomatismoRepository.GetCodigosBarrasAutomatismo(codigos);

                if (codigosAutomatismo.Count() > 0)
                {
                    var codigoBarrasPorCodigo = new Dictionary<string, CodigoBarras>();

                    foreach (var codigoBarras in codigosAutomatismo)
                    {
                        codigoBarrasPorCodigo[codigoBarras.Codigo] = codigoBarras;
                    }

                    foreach (var codigoBarras in codigos)
                    {
                        if (codigoBarrasPorCodigo.ContainsKey(codigoBarras.Codigo))
                        {
                            codigoBarrasPorCodigo[codigoBarras.Codigo] = codigoBarras;
                        }
                    }

                    var codigosNotificables = codigoBarrasPorCodigo.Values;
                    var nuTransaccion = codigosNotificables.First().NumeroTransaccion;
                    var empresa = codigosNotificables.First().Empresa;
                    var request = new CodigosBarrasAutomatismoRequest
                    {
                        DsReferencia = nuTransaccion.ToString(),
                        Empresa = empresa,
                        CodigosDeBarras = codigosNotificables.Select(c => new CodigoBarraAutomatismoRequest
                        {
                            TipoOperacion = c.TipoOperacionId,
                            Codigo = c.Codigo,
                            Producto = c.Producto,
                        }).ToList(),
                    };

                    _automatismoAutoStoreClientService.SendCodigosBarras(request);
                }
            }
        }

        public virtual async Task<ICodigoBarrasServiceContext> GetNewServiceContext(IUnitOfWork uow, List<CodigoBarras> codigos, int userId)
        {
            var empresa = codigos[0].Empresa;
            var context = new CodigoBarrasServiceContext(uow, codigos, userId, empresa);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        public virtual void AddParametros(ICodigoBarrasServiceContext context, int empresa)
        {
            context.AddParametro(ParamManager.IE_505_TP_CODIGO_BARRAS);
            context.AddParametro(ParamManager.LISTA_CARACTERES_COD_BARRA);

            context.AddParametroEmpresa(ParamManager.IE_505_CAMPOS_INMUTABLES, empresa);

            context.SetParametroCamposInmutables(ParamManager.IE_505_CAMPOS_INMUTABLES);
        }
    }
}
