using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Security;
using WIS.Domain.Validation;
using System.Security.Permissions;

namespace WIS.Domain.Services
{
    public class FacturaService : IFacturaService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IValidationService _validationService;
        protected readonly IOptions<MaxItemsSettings> _configuration;
        protected readonly IIdentityService _identity;

        public FacturaService(IUnitOfWorkFactory uowFactory,
            IValidationService validationService,
            IOptions<MaxItemsSettings> configuration,
            IIdentityService identity)
        {
            _validationService = validationService;
            _uowFactory = uowFactory;
            _configuration = configuration;
            _identity = identity;
        }

        public virtual async Task<ValidationsResult> AgregarFacturas(List<Factura> facturas, int userId)
        {
            var result = new ValidationsResult();
            var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                int nroRegistro = 1;

                if (facturas.Count > 0)
                {
                   uow.CreateTransactionNumber(this._identity.Application);
                          
                    int countDetalles = 0;
                    var keys = new HashSet<string>();
                    int maxItems = _configuration.Value.Facturas;

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, facturas.Count, maxItems))
                        return result;

                    var context = await GetNewServiceContext(uow, facturas, userId);

                    foreach (var factura in facturas)
                    {
                        validaciones.Clear();
                        
                        factura.NumeroTransaccion = uow.GetTransactionNumber();

                        string keyFactura = $"{factura.NumeroFactura}.{factura.IdEmpresa}.{factura.Serie}.{factura.CodigoInternoCliente}";
                        if (keys.Contains(keyFactura))
                        {
                            validaciones.Add(new Error("WMSAPI_msg_Error_FacturasDuplicadas", factura.NumeroFactura, factura.IdEmpresa, factura.Serie,  factura.CodigoAgente));
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, true, messages));
                            break;
                        }
                        else
                            keys.Add(keyFactura);

                        validaciones.AddRange(await _validationService.ValidateFactura(factura, context, out bool errorProcedimiento));

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, errorProcedimiento, messages));
                        }

                        countDetalles += factura.Detalles.Count;
                        nroRegistro++;
                    }

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, countDetalles, maxItems, false))
                        return result;

                    if (result.HasError())
                        return result;

                    await uow.FacturaRepository.AddFacturas(facturas, context);
                 
                }
                else
                {
                    var msg = Translator.Translate(uow, new Error("WMSAPI_msg_Error_ListaDeObjetosRequerida"), _identity.UserId);
                    result.Errors.Add(new ValidationsError(nroRegistro, false, new List<string>() { msg }));
                }
            }

            return result;
        }

        public virtual async Task<IFacturaServiceContext> GetNewServiceContext(IUnitOfWork uow, List<Factura> facturas, int userId)
        {
            var empresa = facturas[0].IdEmpresa;
            var context = new FacturaServiceContext(uow, facturas, userId, empresa);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        public virtual void AddParametros(IFacturaServiceContext context, int empresa)
        {
            context.AddParametroEmpresa(ParamManager.IE_425_VALIDAR_FECHAS, empresa);
        }

        public virtual async Task<Factura> GetFactura(string nuFactura, int codigoEmpresa, string serie, string codigoAgente, string tipoAgente)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return await uow.FacturaRepository.GetFacturaOrNull(nuFactura, codigoEmpresa, serie, tipoAgente, codigoAgente);
            }
        }
    }
}
