using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Stock;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Produccion
{
    public class PRD200GridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public PRD200GridValidationModule(IUnitOfWork uow,
            IFormatProvider culture)
        {
            _uow = uow;
            _culture = culture;

            Schema = new GridValidationSchema
            {
                ["QT_MOVIMIENTO_BB"] = ValidateCantidadesMovimiento,
                ["QT_RECHAZO_AVERIA"] = ValidateCantidadesMovimiento,
                ["QT_RECHAZO_SANO"] = ValidateCantidadesMovimiento
            };
        }

        public virtual GridValidationGroup ValidateCantidadesMovimiento(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            string cdProducto = row.GetCell("CD_PRODUTO").Value;
            int empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);

            Producto producto = _uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(empresa, cdProducto);

            if (producto == null)
                return null;

            if (producto.AceptaDecimales)
            {
                return new GridValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(cell.Value),
                        new PositiveDecimalValidationRule(this._culture, cell.Value),
                        new DecimalLengthWithPresicionValidationRule(cell.Value,12,3, this._culture),
                        new SaldoStockSuficienteEntradaBBValidationRule(_uow, row.GetCell("QT_MOVIMIENTO_BB").Value, row.GetCell("QT_RECHAZO_AVERIA").Value, row.GetCell("QT_RECHAZO_SANO").Value, row.GetCell("QT_RESERVA_SAIDA").Value,_culture)
                    }
                };
            }

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new PositiveIntValidationRule(cell.Value),
                    new SaldoStockSuficienteEntradaBBValidationRule(_uow, row.GetCell("QT_MOVIMIENTO_BB").Value, row.GetCell("QT_RECHAZO_AVERIA").Value, row.GetCell("QT_RECHAZO_SANO").Value, row.GetCell("QT_RESERVA_SAIDA").Value,_culture)
                }
            };
        }
    }
}
