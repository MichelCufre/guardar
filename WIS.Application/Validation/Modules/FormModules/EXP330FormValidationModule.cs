using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.CodigoMultidato;
using WIS.Domain.DataModel;
using WIS.Domain.Services.Interfaces;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Session;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class EXP330FormValidationModule : FormValidationModule //TODO: Rehacer clase
    {
        protected readonly IUnitOfWork _uow;
        protected readonly ISessionAccessor _sessionAccessor;
        protected readonly IIdentityService _identity;
        protected readonly IBarcodeService _barcodeService;
        protected readonly ICodigoMultidatoService _codigoMultidatoService;

        public EXP330FormValidationModule(
            IUnitOfWork uow,
            ISessionAccessor sessionAccessor,
            IIdentityService identity,
            IBarcodeService barcodeService,
            ICodigoMultidatoService codigoMultidatoService)
        {
            this._uow = uow;
            this._sessionAccessor = sessionAccessor;
            this._identity = identity;
            this._barcodeService = barcodeService;
            this._codigoMultidatoService = codigoMultidatoService;

            this.Schema = new FormValidationSchema
            {
                ["NuContenedor"] = this.ValidateNuContenedor,
                ["CdEmpresa"] = this.ValidateCdEmpresa
            };
        }

        public virtual FormValidationGroup ValidateNuContenedor(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string pedidoSeleccionado = form.GetField("NuPedido").Value;
            string clienteSeleccionada = null;
            int empresaSeleccionada = -1;

            if (!string.IsNullOrEmpty(pedidoSeleccionado))
            {
                clienteSeleccionada = form.GetField("CdCliente").Value;
                empresaSeleccionada = int.Parse(form.GetField("CdEmpresa").Value);
            }

            if (!string.IsNullOrEmpty(form.GetField("CdEmpresa").Value))
            {
                empresaSeleccionada = int.Parse(form.GetField("CdEmpresa").Value);
            }

            var cdBarrasContenedor = field.Value = field.Value.ToUpper();

            try
            {
                var resultadoAIs = _codigoMultidatoService.GetAIs(this._uow, "EXP330", cdBarrasContenedor, new Dictionary<string, string>
                {
                    ["USERID"] = _identity.UserId.ToString(),
                    ["NU_PREDIO"] = _identity.Predio,
                    ["CD_CAMPO"] = field.Id,
                }, empresaSeleccionada == -1 ? null : empresaSeleccionada, tipoLectura: CodigoMultidatoTipoLectura.LPN).GetAwaiter().GetResult();

                var ais = resultadoAIs?.AIs;

                if (empresaSeleccionada == -1 && resultadoAIs?.Empresa != null)
                {
                    empresaSeleccionada = resultadoAIs.Empresa;
                }

                if (ais != null && ais.ContainsKey(field.Id))
                {
                    cdBarrasContenedor = ais[field.Id].ToString();
                    field.IsMultidataCodeReading = true;
                }

                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                    {
                       new NonNullValidationRule(cdBarrasContenedor),
                       new StringMaxLengthValidationRule(cdBarrasContenedor, 50),
                       new CodigoBarrasContenedorPedidoMostrador(this._uow, cdBarrasContenedor, clienteSeleccionada, empresaSeleccionada, pedidoSeleccionado, _identity, _barcodeService)
                    },
                };
            }
            catch (TooManyEmpresaCodigoMultidatoException ex)
            {
                return null;
            }
        }

        public virtual FormValidationGroup ValidateCdEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new ExisteEmpresaValidationRule(this._uow, field.Value),
                    new EmpresaPerteceneUsuarioValidationRule(this._uow, this._identity.UserId, int.Parse(field.Value))
                },
            };
        }
    }
}
