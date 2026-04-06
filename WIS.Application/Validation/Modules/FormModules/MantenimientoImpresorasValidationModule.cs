using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Impresion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class MantenimientoImpresorasValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _usuarioLogueado;
        protected readonly string _predioLogueado;

        public MantenimientoImpresorasValidationModule(IUnitOfWork uow, int usuario, string predioLogueado)
        {
            this._predioLogueado = predioLogueado;
            this._usuarioLogueado = usuario;
            this._uow = uow;

            this.Schema = new FormValidationSchema
            {
                ["codigo"] = this.ValidateCodigo,
                ["predio"] = this.ValidatePredio,
                ["descripcion"] = this.ValidateDescripcion,
                ["lenguaje"] = this.ValidateLenguaje,
                ["servidor"] = this.ValidateServidor,
            };
        }


        public virtual FormValidationGroup ValidateCodigo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 50),
                    new NoExisteImpresoraSistemaValidationRule(_uow,form.GetField("predio").Value,field.Value)
                },
                Dependencies = { "predio" }

            };
        }

        public virtual FormValidationGroup ValidatePredio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new ExistePredioValidationRule(_uow, this._usuarioLogueado, this._predioLogueado, field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateDescripcion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 100)
                }
            };
        }

        public virtual FormValidationGroup ValidateLenguaje(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new ExisteLenguajeImpresionValidationRule(_uow,field.Value)
                }
            };
        }
        
        public virtual FormValidationGroup ValidateServidor(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new ExisteServidorImpresionValidationRule(_uow,  field.Value)
                }
            };
        }
    }
}
