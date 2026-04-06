using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Produccion
{
    public class PRD113ProductosExpulsarGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public PRD113ProductosExpulsarGridValidationModule(IUnitOfWork uow, IFormatProvider culture)
        {
            this._uow = uow;
            this._culture = culture;

            this.Schema = new GridValidationSchema
            {
                ["QT_EXPULSAR"] = this.ValidateCantidadProducir,
            };
        }
        public virtual GridValidationGroup ValidateCantidadProducir(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(row.GetCell("CD_EMPRESA").Value) || string.IsNullOrEmpty(row.GetCell("CD_PRODUTO").Value))
                return null;

            var codigoProducto = row.GetCell("CD_PRODUTO").Value;
            var empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);

            var producto = _uow.ProductoRepository.GetProducto(empresa, codigoProducto);

            var qtStock = decimal.Parse(row.Cells.FirstOrDefault(x => x.Column.Id == "QT_ESTOQUE").Value, _culture);
            var qtReserva = decimal.Parse(row.Cells.FirstOrDefault(x => x.Column.Id == "QT_RESERVA_SAIDA").Value, _culture);
            var stockDisponible = qtStock - qtReserva;
            decimal.TryParse(cell.Value, _culture, out decimal value);

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new PositiveDecimalValidationRule(this._culture, cell.Value, false),
                    new DecimalLengthWithPresicionValidationRule(cell.Value, 12, 3, this._culture, producto.AceptaDecimales),
                    new NumeroDecimalMenorOIgualQueValidationRule(value, stockDisponible),
                    new ManejoIdentificadorSerieCantidadValidationRule(cell.Value, producto.ManejoIdentificador, _culture)
                }
            };
        }
    }
}
