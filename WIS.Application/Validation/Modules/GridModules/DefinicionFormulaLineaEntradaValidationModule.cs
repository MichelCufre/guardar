using DocumentFormat.OpenXml.Vml.Office;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Produccion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class DefinicionFormulaLineaEntradaValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _formatProvider;

        public DefinicionFormulaLineaEntradaValidationModule(IUnitOfWork uow, IFormatProvider formatProvider)
        {
            this._uow = uow;
            this._formatProvider = formatProvider;

            this.Schema = new GridValidationSchema
            {
                ["CD_PRODUTO"] = this.ValidateCodigoProducto,
                ["QT_CONSUMIDA_LINEA"] = this.ValidateCantidad,
            };
        }

        public virtual GridValidationGroup ValidateCodigoProducto(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var codigoFormula = parameters.Any(s => s.Id == "formula") ? parameters.FirstOrDefault(s => s.Id == "formula").Value : string.Empty;

            if (string.IsNullOrEmpty(codigoFormula))
                return null;

            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(cell.Value),
            };

            var validarProductoRepetidos = (_uow.ParametroRepository.GetParameter(ParamManager.PRD100_VALIDAR_PROD_DISTINTOS) ?? "N") == "S";
            if (validarProductoRepetidos) //Control cruzado
                rules.Add(new FormulaProductoFinalExistenteValidationRule(_uow, codigoFormula, cell.Value));

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = this.OnSuccessGridValidateProducto
            };
        }
        public virtual void OnSuccessGridValidateProducto(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var empresa = int.Parse(parameters.Any(s => s.Id == "empresa") ? parameters.FirstOrDefault(s => s.Id == "empresa").Value : row.GetCell("CD_EMPRESA").Value);
            var cdProducto = row.GetCell("CD_PRODUTO").Value;
            var producto = _uow.ProductoRepository.GetProducto(empresa, cdProducto);

            if (row.GetCell("DS_PRODUTO") != null)
                row.GetCell("DS_PRODUTO").Value = producto.Descripcion;
        }

        public virtual GridValidationGroup ValidateCantidad(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var cdProducto = row.GetCell("CD_PRODUTO").Value;

            if (string.IsNullOrEmpty(cdProducto))
                return null;

            var empresa = int.Parse(parameters.Any(s => s.Id == "empresa") ? parameters.FirstOrDefault(s => s.Id == "empresa").Value : row.GetCell("CD_EMPRESA").Value);

            var producto = this._uow.ProductoRepository.GetProducto(empresa, cdProducto);

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new PositiveDecimalValidationRule(this._formatProvider, cell.Value, allowZero: false),
                    new DecimalLengthWithPresicionValidationRule(cell.Value, 12, 3, this._formatProvider, producto.AceptaDecimales),

                }
            };
        }

    }
}
