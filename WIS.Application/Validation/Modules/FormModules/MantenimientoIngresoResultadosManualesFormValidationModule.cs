using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Facturacion;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.Facturacion;
using WIS.Domain.General;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class MantenimientoIngresoResultadosManualesFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;
        protected readonly string _numeroEjecucion;
        protected readonly int _userId;

        public MantenimientoIngresoResultadosManualesFormValidationModule(IUnitOfWork uow, IFormatProvider culture, string numeroEjecucion)
        {
            this._uow = uow;
            this._culture = culture;
            this._numeroEjecucion = numeroEjecucion;

            this.Schema = new FormValidationSchema
            {
                ["empresa"] = this.ValidateCodigoEmpresa,
                ["facturacion"] = this.ValidateCodigoFacturacion,
                ["nuComponente"] = this.ValidateComponente,
                ["resultado"] = this.ValidateResultado,
                ["dsAdicional"] = this.ValidateDescripcionAdicional,
            };
        }

        public virtual FormValidationGroup ValidateCodigoEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var rules = new FormValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
                    new PositiveIntValidationRule(field.Value)
                },
                OnSuccess = this.ValidateCodigoEmpresa_OnSuccess,
            };

            string cdFacturacion = form.GetField("facturacion").Value;
            string nuComponente = form.GetField("nuComponente").Value;

            if (!string.IsNullOrEmpty(cdFacturacion) && !string.IsNullOrEmpty(nuComponente))
                rules.Rules.Add(new ExisteResultadoManualValidationRule(this._uow, int.Parse(field.Value), cdFacturacion, int.Parse(this._numeroEjecucion), nuComponente));

            return rules;
        }
        public virtual FormValidationGroup ValidateCodigoFacturacion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                Rules = new List<IValidationRule> {
                     new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 20)
                },
                OnSuccess = this.ValidateCodigoFacturacion_OnSuccess,
            };
        }
        public virtual FormValidationGroup ValidateComponente(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var rules = new FormValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 20),
                },
            };

            string cdFacturacion = form.GetField("facturacion").Value;

            if (!string.IsNullOrEmpty(cdFacturacion))
            {
                //Valida tipo calculo
                FacturacionCodigo facturacionCodigo = this._uow.FacturacionRepository.GetFacturacionCodigo(cdFacturacion);
                rules.Rules.Add(new StringEqualsValueValidationRule(facturacionCodigo.TipoCalculo, "M", "General_Sec0_Error_CodigoNoValido"));


                //Valida relacion entre componente y facturacion
                rules.Rules.Add(new ExisteCodigoComponenteValidationRule(this._uow, cdFacturacion, field.Value));
            }

            //Valida que no este ya ingresado el dato
            string componente = field.Value;
            string empresa = form.GetField("empresa").Value;

            if (!string.IsNullOrEmpty(cdFacturacion) && empresa != null)
            {
                rules.Rules.Add(new ExisteResultadoManualValidationRule(this._uow, int.Parse(empresa), cdFacturacion, int.Parse(this._numeroEjecucion), componente));
            }

            return rules;
        }
        public virtual FormValidationGroup ValidateResultado(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
                    new PositiveDecimalValidationRule(this._culture, field.Value)
                }
            };
        }
        public virtual FormValidationGroup ValidateDescripcionAdicional(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new StringMaxLengthValidationRule(field.Value, 1000),
                }
            };
        }

        public virtual void ValidateCodigoEmpresa_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
            {
                form.GetField("facturacion").Value = "";
            }
            else
            {
                form.GetField("facturacion").Options = this._uow.FacturacionRepository.GetFacturacionesCodigoByEjecucionEmpresa(int.Parse(this._numeroEjecucion), int.Parse(field.Value)).Select(w => new SelectOption(w.CodigoFacturacion, w.CodigoFacturacion)).ToList();
            }
        }

        public virtual void ValidateCodigoFacturacion_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
            {
                form.GetField("nuComponente").Value = "";
            }
            else
            {
                form.GetField("nuComponente").Options = this._uow.FacturacionRepository.GetFacturacionesCodigoComponente(field.Value).Select(w => new SelectOption(w.NumeroComponente, w.NumeroComponente + " - " + w.Descripcion)).ToList();
            }
        }
    }
}
