using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Stock;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.StockEntities;
using WIS.Domain.StockEntities.Constants;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Stock
{
    public class TraspasoEmpresasPreparacionValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly TraspasoEmpresas _traspaso;

        public TraspasoEmpresasPreparacionValidationModule(IUnitOfWork uow, IIdentityService identity, ISecurityService security, TraspasoEmpresas traspaso)
        {
            this.Schema = new FormValidationSchema
            {
                ["nuPreparacion"] = this.ValidatePreparacion,
                ["confPedidoDestino"] = this.ValidateConfigPedidoDestino,
            };

            this._uow = uow;
            this._identity = identity;
            this._security = security;
            this._traspaso = traspaso;
        }

        public virtual FormValidationGroup ValidatePreparacion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var prepa = parameters.FirstOrDefault(x => x.Id == "nuPreparacion");
            if (prepa != null)
            {
                field.Value = prepa.Value;
            }

            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(field.Value),
                new StringMaxLengthValidationRule(field.Value, 6),
                new PositiveIntValidationRule(field.Value),
            };

            if (int.TryParse(field.Value, out int nuPreparacion))
            {
                rules.Add(new ExistePreparacionValidationRule(this._uow, nuPreparacion));

                if (this._traspaso.TipoTraspaso == TipoTraspasoDb.TraspasoSeleccion
                    || this._traspaso.TipoTraspaso == TipoTraspasoDb.TraspasoPreparacionPendiente)
                {
                    rules.Add(new TraspasoEmpresasPreparacionPendienteValidationRule(this._uow, nuPreparacion));
                }
                else if (this._traspaso.TipoTraspaso == TipoTraspasoDb.TraspasoPda
                    || this._traspaso.TipoTraspaso == TipoTraspasoDb.TraspasoPreparacionOrigen)
                {
                    rules.Add(new TraspasoEmpresasPreparacionPreparadoValidationRule(this._uow, nuPreparacion));
                }
            }

            return new FormValidationGroup
            {
                Rules = rules,
            };
        }

        public virtual FormValidationGroup ValidateConfigPedidoDestino(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (this._traspaso.TipoTraspaso != TipoTraspasoDb.TraspasoPreparacionOrigen)
                return null;

            return new FormValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new ExisteConfiguracionPedidoDestinoValidationRule(field.Value)
                }
            };
        }

    }
}
