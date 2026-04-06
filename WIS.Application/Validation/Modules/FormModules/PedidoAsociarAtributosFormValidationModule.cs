using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Preparacion;
using WIS.Application.Validation.Rules.General;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Picking.Dtos;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;
using DocumentFormat.OpenXml.Vml.Office;
using WIS.Domain.General;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class PedidoAsociarAtributosFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected DetallePedidoLpnEspecifico _datos;

        public PedidoAsociarAtributosFormValidationModule(IUnitOfWork uow, IIdentityService identity, DetallePedidoLpnEspecifico datos)
        {
            this._uow = uow;
            this._identity = identity;
            this._datos = datos;

            this.Schema = new FormValidationSchema
            {
                ["producto"] = this.ValidateProducto,
                ["identificador"] = this.ValidateIdentificador,
                ["cantidad"] = this.ValidateCantidad
            };
        }


        public virtual FormValidationGroup ValidateProducto(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 40),
                },
                OnSuccess = this.ValidateProductoOnSuccess
            };
        }


        public virtual void ValidateProductoOnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var producto = _uow.ProductoRepository.GetProducto(_datos.Empresa, field.Value);

            if (producto != null)
            {
                form.GetField("identificador").ReadOnly = false;
                if (producto.IsIdentifiedByProducto())
                {
                    form.GetField("identificador").Value = ManejoIdentificadorDb.IdentificadorProducto;
                    form.GetField("identificador").ReadOnly = true;
                }
                else if (string.IsNullOrEmpty(form.GetField("identificador").Value) || form.GetField("identificador").Value == ManejoIdentificadorDb.IdentificadorProducto)
                    form.GetField("identificador").Value = ManejoIdentificadorDb.IdentificadorAuto;

            }
        }

        public virtual FormValidationGroup ValidateIdentificador(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(form.GetField("producto").Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 40),
                    new IdentificadorProductoValidationRule(this._uow, _datos.Empresa, form.GetField("producto").Value, field.Value),
                }
            };
        }

        public virtual FormValidationGroup ValidateCantidad(FormField field, Form form, List<ComponentParameter> parameters)
        {
            _datos.Cantidad = (!string.IsNullOrEmpty(field.Value) && decimal.TryParse(field.Value, _identity.GetFormatProvider(), out decimal parsedValue))
                ? decimal.Parse(field.Value, _identity.GetFormatProvider()) : 0;

            var codigoProducto = form.GetField("producto").Value;
            var identificador = form.GetField("identificador").Value;

            if (string.IsNullOrEmpty(codigoProducto) || string.IsNullOrEmpty(identificador))
                return null;

            var producto = _uow.ProductoRepository.GetProducto(_datos.Empresa, codigoProducto);

            _datos.Producto = producto.Codigo;
            _datos.ManejoIdentificador = producto.ManejoIdentificador;
            _datos.Identificador = string.IsNullOrEmpty(identificador) ? ManejoIdentificadorDb.IdentificadorAuto : identificador;
            _datos.IdEspecificaIdentificador = (identificador != ManejoIdentificadorDb.IdentificadorAuto ? "S" : "N");

            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(field.Value),
                new PositiveDecimalValidationRule(_identity.GetFormatProvider(), field.Value, false),
                new DecimalLengthWithPresicionValidationRule(field.Value, 12, 3, _identity.GetFormatProvider(), producto.AceptaDecimales),
                new CantidadPedidaDetalleAtributoValidationRule(_uow, _datos)
            };

            if (identificador != ManejoIdentificadorDb.IdentificadorAuto)
                rules.Add(new ManejoIdentificadorSerieCantidadValidationRule(field.Value, producto.ManejoIdentificador, _identity.GetFormatProvider()));

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }
    }
}
