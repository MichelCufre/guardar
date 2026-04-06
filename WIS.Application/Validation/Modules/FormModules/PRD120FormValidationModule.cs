using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Produccion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Produccion;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class PRD120FormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _usuario;
        protected readonly string _predio;
        protected readonly List<Ubicacion> _ubicaciones;
        public PRD120FormValidationModule(IUnitOfWork uow, int usuarioLogueado, string predioLogueado, List<Ubicacion> ubicaciones = null)
        {
            this._uow = uow;
            this._usuario = usuarioLogueado;
            this._predio = predioLogueado;
            this._ubicaciones = ubicaciones;
            this.Schema = new FormValidationSchema
            {
                ["cdAccion"] = this.ValidateCD_ACCION,
                ["dsInstancia"] = this.ValidateDS_ACCION_INSTANCIA,


            };
        }

        public virtual FormValidationGroup ValidateCD_ACCION(FormField field, Form form, List<ComponentParameter> parameters)
        {

            var Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
                    new AccionExistsValidationRule(this._uow,field.Value),


            };

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = Rules,
                OnSuccess = this.ValidateCodigoAccion_OnSucess,

            };

        }
        public virtual FormValidationGroup ValidateDS_ACCION_INSTANCIA(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(field.Value),
                        new StringMaxLengthValidationRule(field.Value, 100),
                    }
            };
        }

        public virtual void ValidateCodigoAccion_OnSucess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string codigoAccion = form.GetField("cdAccion").Value;
            if (codigoAccion != "")
            {
                Accion accion = this._uow.AccionRepository.GetAccion(codigoAccion);
                form.GetField("dsAccion").Value = accion?.Descripcion;
            }
        }



    }
}
