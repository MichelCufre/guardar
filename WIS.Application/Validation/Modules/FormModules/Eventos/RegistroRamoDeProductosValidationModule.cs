using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Facturacion;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Eventos
{
    public class RegistroRamoDeProductosValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;
        public RegistroRamoDeProductosValidationModule(
             IUnitOfWork uow,
             IFormatProvider culture)
        {
            this._uow = uow;
            this._culture = culture;
            this.Schema = new GridValidationSchema
            {
                ["CD_RAMO_PRODUTO"] = this.ValidateCodigoRamoProducto,
                ["DS_RAMO_PRODUTO"] = this.ValidateDescripcionRamoProducto,
            };
        }


        public virtual GridValidationGroup ValidateCodigoRamoProducto(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            ExisteCodigoRamoProductoValidationRule existeCodigoRamoProducto = new ExisteCodigoRamoProductoValidationRule(this._uow, cell.Value);
            var Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new NumberValidationRule<short?>(cell.Value, _culture),
                };

            if (row.IsNew)
            {

                Rules.Add(existeCodigoRamoProducto);
            }
            else
            {
                Rules.Remove(existeCodigoRamoProducto);
            }

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = Rules

            };

        }

        public virtual GridValidationGroup ValidateDescripcionRamoProducto(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {

            var Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringSoloUpperValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 200),
                };

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = Rules

            };
        }
    }
}