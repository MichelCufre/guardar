using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Recepcion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class CreateUsuarioValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _usuarioLogueado;
        protected readonly string _aplicacion;

        public CreateUsuarioValidationModule(IUnitOfWork uow, int usuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._usuarioLogueado = usuarioLogueado;
            this._aplicacion = aplicacion;

            this.Schema = new FormValidationSchema
            {
                ["nomUsuario"] = this.ValidateUsuario,
                ["nomCompleto"] = this.ValidateNombreCompleto,
                ["email"] = this.ValidateEmail,
                ["tipoUsu"] = this.ValidateTipoUsuario,
                ["idioma"] = this.ValidateIdioma
            };
        }

        public virtual FormValidationGroup ValidateNombreCompleto(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value,100)
                }
            };
        }

        public virtual FormValidationGroup ValidateUsuario(FormField field, Form form, List<ComponentParameter> parameters)
        {

            if (field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value,50),
                    new ExisteUsuarioValidationRule(this._uow,field.Value),
                }
            };
        }

        public virtual FormValidationGroup ValidateEmail(FormField field, Form form, List<ComponentParameter> parameters)
        {
            int? user = null;
            if (parameters.Count > 0 && parameters.Find(x => x.Id == "idUsuario") != null)
                user = int.Parse(parameters.Find(x => x.Id == "idUsuario").Value);

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 100),
                    new EmailValidationRule(this._uow, field.Value, validacionUsuario:true, userId:user),
                }
            };
        }

        public virtual FormValidationGroup ValidateTipoUsuario(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value)
                },
            };
        }

        public virtual FormValidationGroup ValidateIdioma(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value)
                },
            };
        }

    }
}
