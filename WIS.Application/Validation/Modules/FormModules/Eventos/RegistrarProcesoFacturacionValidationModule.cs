using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Facturacion;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.Facturacion;
using WIS.Domain.General;
using WIS.Domain.Impresiones;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Eventos
{
    public class RegistrarProcesoFacturacionValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _usuario;
        protected readonly string _predio;
        protected readonly List<Ubicacion> _ubicaciones;
        public RegistrarProcesoFacturacionValidationModule(IUnitOfWork uow, int usuarioLogueado, string predioLogueado, List<Ubicacion> ubicaciones = null)
        {
            this._uow = uow;
            this._usuario = usuarioLogueado;
            this._predio = predioLogueado;
            this._ubicaciones = ubicaciones;
            this.Schema = new FormValidationSchema
            {
                ["codigoProceso"] = this.ValidateCodigoProceso,
                ["descripcionProceso"] = this.ValidateDescripcionProceso,
                ["codigoFactura"] = this.ValidateCodigoFacturacion,
                ["componente"] = this.ValidateNumeroComponente,
                ["numeroCuentaContable"] = this.ValidateNumeroCuentaContable,

            };
        }

        public virtual FormValidationGroup ValidateCodigoFacturacion(FormField field, Form form, List<ComponentParameter> parameters)
        {

            var Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
                    new StringSoloUpperValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 20),
                    new ExisteCodigoFacturacionComponente(this._uow, field.Value),


                };

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = Rules,
                OnSuccess = this.ValidateCodigoFacturacion_OnSucess 
            };

        }

        public virtual FormValidationGroup ValidateNumeroComponente(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string numeroDeFactura = form.GetField("codigoFactura").Value;
            var Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
                    new StringSoloUpperValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 20),
                    new FacturaYaTieneComponenteValidationRule(field.Value, numeroDeFactura, this._uow),
                    new ExisteCodigoComponenteEnFactura(this._uow, field.Value),
        };



            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = Rules

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
        public virtual FormValidationGroup ValidateCodigoProceso(FormField field, Form form, List<ComponentParameter> parameters)
        {

            var Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
                    new StringSoloUpperValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 20),
                    new ExisteCodigoProceso(this._uow , field.Value),

                };

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = Rules,
               
            };

        }

        public virtual void ValidateCodigoFacturacion_OnSucess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string codigoFactura = form.GetField("codigoFactura").Value;
            FormField selectComponentes = form.GetField("componente");
            selectComponentes.Options = new List<SelectOption>();

            List<FacturacionCodigoComponente> listaComponentesPorFactura = _uow.FacturacionRepository.GetComponentesByFacturacion(codigoFactura);

            foreach (var componente in listaComponentesPorFactura)
            {
                selectComponentes.Options.Add(new SelectOption(componente.NumeroComponente, $"{componente.NumeroComponente} - {componente.Descripcion}"));
            };

            form.GetField("componente").ReadOnly = false;
        }

    }
}
