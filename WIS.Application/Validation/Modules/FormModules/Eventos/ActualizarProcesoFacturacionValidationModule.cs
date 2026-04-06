using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Facturacion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Eventos
{
    public class ActualizarProcesoFacturacionValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _usuario;
        protected readonly string _predio;
        protected readonly List<Ubicacion> _ubicaciones;
        public ActualizarProcesoFacturacionValidationModule(IUnitOfWork uow, int usuarioLogueado, string predioLogueado, List<Ubicacion> ubicaciones = null)
        {
            this._uow = uow;
            this._usuario = usuarioLogueado;
            this._predio = predioLogueado;
            this._ubicaciones = ubicaciones;
            this.Schema = new FormValidationSchema
            {
                ["descripcionProceso"] = this.ValidateDescripcionProceso,
                ["numeroCuentaContable"] = this.ValidateNumeroCuentaContable,

            };
        }

        public virtual FormValidationGroup ValidateDescripcionProceso(FormField field, Form form, List<ComponentParameter> parameters)
        {

            return new FormValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 200),
                    new StringDescripcionSoloUpperValidationRule(field.Value),
                },

            };
        }
        public virtual FormValidationGroup ValidateNumeroCuentaContable(FormField field, Form form, List<ComponentParameter> parameters)
        {

            var Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
                    new StringSoloUpperValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 10),
                    new ExisteNumeroCuentaContable(this._uow, field.Value),

                };

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = Rules,
            };

        }


    }
}
