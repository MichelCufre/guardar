using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.Produccion;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Produccion
{
    public class PRD113ConsumoParcialFromValidationModule : FormValidationModule
    {
        protected readonly IFormatProvider _culture;
        protected readonly IUnitOfWork _uow;

        public PRD113ConsumoParcialFromValidationModule(IUnitOfWork uow, IFormatProvider culture)
        {
            _uow = uow;
            _culture = culture;
            Schema = new FormValidationSchema
            {
                ["consumo"] = ValidateConsumo,
                ["motivo"] = ValidateMotivo,
            };
        }

        public virtual FormValidationGroup ValidateConsumo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var codigoProducto = form.GetField("producto").Value;
            var nuIngresoProduccion = parameters.FirstOrDefault(p => p.Id == "nuIngresoProduccion")?.Value;

            if (string.IsNullOrEmpty(codigoProducto) || string.IsNullOrEmpty(nuIngresoProduccion))
                return null;

            var ingreso = _uow.IngresoProduccionRepository.GetIngresoById(nuIngresoProduccion);

            var producto = _uow.ProductoRepository.GetProducto(ingreso.Empresa.Value, codigoProducto);

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new PositiveDecimalValidationRule(_culture, string.IsNullOrEmpty(field.Value) ? "0" : field.Value),
                    new DecimalLengthWithPresicionValidationRule(string.IsNullOrEmpty(field.Value) ? "0" : field.Value, 12, 3, this._culture, producto.AceptaDecimales),
                    new ManejoIdentificadorSerieCantidadValidationRule(field.Value, producto.ManejoIdentificador, _culture, allowZero: true)
                }
            };
        }

        public virtual FormValidationGroup ValidateMotivo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new ExisteDominioValidationRule(field.Value, this._uow)
                }
            };
        }
    }
}
