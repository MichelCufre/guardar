using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Expedicion;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Preparacion;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.Picking.Dtos;
using WIS.Domain.Tracking.Models;
using WIS.Domain.Tracking.Validation;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class PedidoAsociarAtributosLpnFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected DetallePedidoLpnEspecifico _datos;

        public PedidoAsociarAtributosLpnFormValidationModule(IUnitOfWork uow, IIdentityService identity, DetallePedidoLpnEspecifico datos)
        {
            this._uow = uow;
            this._identity = identity;
            this._datos = datos;

            this.Schema = new FormValidationSchema
            {
                ["cantidad"] = this.ValidateCantidad
            };
        }

        public virtual FormValidationGroup ValidateCantidad(FormField field, Form form, List<ComponentParameter> parameters)
        {
            _datos.Cantidad = (!string.IsNullOrEmpty(field.Value) && decimal.TryParse(field.Value, _identity.GetFormatProvider(), out decimal parsedValue))
                ? decimal.Parse(field.Value, _identity.GetFormatProvider()) : 0;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveDecimalValidationRule(_identity.GetFormatProvider(), field.Value, false),
                    new DecimalLengthWithPresicionValidationRule(field.Value, 12, 3, _identity.GetFormatProvider()),
                    new CantidadPedidaLpnAtributoValidationRule(_uow, _datos)
                }
            };
        }
    }
}
