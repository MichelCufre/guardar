using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.General.Enums;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;
using WIS.Application.Validation.Rules.Registro;

namespace WIS.Application.Validation.Produccion.Modules.Forms
{
    public class PRD113ProudctosNoEsperadosFromValidationModule : FormValidationModule
    {
        protected readonly IFormatProvider _culture;
        protected readonly IUnitOfWork _uow;

        public PRD113ProudctosNoEsperadosFromValidationModule(IUnitOfWork uow, IFormatProvider culture)
        {
            _uow = uow;
            _culture = culture;
            Schema = new FormValidationSchema
            {
                ["producto"] = ValidateCodigo,
                ["empresa"] = ValidateCodigoEmpresa,
                ["motivo"] = ValidateMotivo,
                ["dsMotivo"] = ValidateDsMotivo,
                ["fechaVencimiento"] = ValidateVencimiento,
                ["cantidad"] = ValidateCantidad,
            };
        }

        public virtual FormValidationGroup ValidateCodigoEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly && form.GetField("empresa").IsValidated)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                },
            };
        }

        public virtual FormValidationGroup ValidateCodigo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringSoloUpperValidationRule(field.Value),
                    new CaracteresAdmitidosCodigoProductoValidationRule(_uow, field.Value),
                    new ProductoExistsValidationRule(_uow,form.GetField("empresa").Value, field.Value),
                },
                Dependencies = { "empresa" },
                OnSuccess = ValidateCodigo_OnSucess,
            };
        }

        public virtual void ValidateCodigo_OnSucess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            FormField empresa = form.GetField("empresa");
            FormField producto = form.GetField("producto");

            if (empresa.IsValid() && producto.IsValid())
            {
                int cdEmpresa = int.Parse(empresa.Value);

                var tipoManejoFecha = _uow.ProductoRepository.GetProductoManejoFecha(cdEmpresa, producto.Value);
                if (tipoManejoFecha != ManejoFechaProducto.Expirable)
                {
                    form.GetField("fechaVencimiento").Value = string.Empty;
                    form.GetField("fechaVencimiento").ReadOnly = true;
                }
                else
                {
                    form.GetField("fechaVencimiento").ReadOnly = false;
                }
            }
        }

        public virtual FormValidationGroup ValidateVencimiento(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new DateTimeValidationRule(field.Value),
                }
            };
        }

        public virtual FormValidationGroup ValidateDsMotivo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 50)
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
                    new MotivosProduccionValidationRule(_uow, field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateCantidad(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var codigoProducto = form.GetField("producto").Value;

            if (string.IsNullOrEmpty(codigoProducto) || string.IsNullOrEmpty(form.GetField("empresa").Value))
                return null;

            var empresa = int.Parse(form.GetField("empresa").Value);

            var producto = _uow.ProductoRepository.GetProducto(empresa, codigoProducto);

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new PositiveDecimalValidationRule(_culture, field.Value),
                    new DecimalLengthWithPresicionValidationRule(field.Value, 12, 3, this._culture, producto.AceptaDecimales),
                    new ManejoIdentificadorSerieCantidadValidationRule(field.Value, producto.ManejoIdentificador, _culture)
                }
            };
        }
    }
}
