using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoEliminacionPedidosPendienteAtributoValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public MantenimientoEliminacionPedidosPendienteAtributoValidationModule(IUnitOfWork uow, IFormatProvider culture )
        {
            this._uow = uow;
            this._culture = culture;

            this.Schema = new GridValidationSchema
            {
                ["AUXQT_ANULADO"] = this.ValidateCantAnular,
            };
        }


        #region ValidateCantidadAnulacion
        public virtual GridValidationGroup ValidateCantAnular(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var codigoProducto = row.GetCell("CD_PRODUTO").Value;
            var identificador = row.GetCell("NU_IDENTIFICADOR").Value;
            var empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);

            var producto = _uow.ProductoRepository.GetProducto(empresa, codigoProducto);

            var rules = new List<IValidationRule> {
                new NonNullValidationRule(cell.Value),
                new DecimalLengthWithPresicionValidationRule(cell.Value, 12, 3, this._culture, producto.AceptaDecimales),
                new CantidadAnularMayorQuePendienteValidationRule(cell.Value, row.GetCell("QT_PENDIENTE").Value, _culture),
                new CantidadAnularMayorQuePendienteValidationRule(cell.Value, row.GetCell("QT_PENDIENTE").Value, _culture)
            };

            if (identificador != ManejoIdentificadorDb.IdentificadorAuto)
                rules.Add(new ManejoIdentificadorSerieCantidadValidationRule(cell.Value, producto.ManejoIdentificador, _culture));

            return new GridValidationGroup
            {
                Rules = rules,
            };
        }
        #endregion


    }
}
