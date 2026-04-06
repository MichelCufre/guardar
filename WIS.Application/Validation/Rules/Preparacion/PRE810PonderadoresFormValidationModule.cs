using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Preparacion
{
    public class PRE810PonderadoresFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public PRE810PonderadoresFormValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new FormValidationSchema
            {
                ["colaDeTrabajo"] = this.ValidateColaDeTrabajo,
                ["incrementoEmpresa"] = this.ValidateIncremento,
                ["incrementoCliente"] = this.ValidateIncremento,
                ["incrementoRuta"] = this.ValidateIncremento,
                ["incrementoZona"] = this.ValidateIncremento,
                ["incrementoTipoPedido"] = this.ValidateIncremento,
                ["incrementoTipoExpedicion"] = this.ValidateIncremento,
                ["incrementoCondicionLiberacion"] = this.ValidateIncremento,
                ["incrementoFechaEntrega"] = this.ValidateIncremento,
                ["incrementoFechaEmitido"] = this.ValidateIncremento,
                ["incrementoFechaLiberado"] = this.ValidateIncremento,
            };
        }

        protected FormValidationGroup ValidateColaDeTrabajo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    //new NonNullValidationRule(field.Value),
                },
            };
        }

        protected FormValidationGroup ValidateIncremento(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule> {
                    new IntBetweenValidationRule(field.Value, -100, 100),
                },
            };
        }

    }
}
