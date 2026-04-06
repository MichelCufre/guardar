using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.FormComponent.Validation
{
    public abstract class FormValidationModule : IFormValidationModule
    {
        public FormValidator Validator { get; set; }
        public FormValidationSchema Schema { private get; set; }

        public FormValidationModule()
        {
        }

        public virtual void Validate(FormField field, Form form)
        {
            if (form == null)
                throw new ArgumentNullException("No se especifico formulario para validar");

            if (field == null)
                throw new ArgumentNullException("No se especifico campo para validar");

            if (this.Validator == null)
                throw new InvalidOperationException("No se especifico validator");

            if (this.Schema == null)
                throw new InvalidOperationException("No se especifico esquema de validacion");

            this.Validator.Schema = this.Schema;

            this.Validator.Validate(field, form);
        }

        public virtual void Validate(Form form)
        {
            if(form == null)
                throw new ArgumentNullException("No se especifico formulario para validar");

            if (form.Fields == null)
                throw new InvalidOperationException("No se especificaron filas para validar");

            if (this.Validator == null)
                throw new InvalidOperationException("No se especifico validator");

            if (this.Schema == null)
                throw new InvalidOperationException("No se especifico esquema de validacion");

            this.Validator.Schema = this.Schema;

            foreach (var field in form.Fields)
            {
                this.Validator.Validate(field, form);
            }
        }
    }
}
