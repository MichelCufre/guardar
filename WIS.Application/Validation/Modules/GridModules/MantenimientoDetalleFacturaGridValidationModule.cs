using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoDetalleFacturaGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public MantenimientoDetalleFacturaGridValidationModule(IUnitOfWork uow, IFormatProvider culture)
        {
            this._uow = uow;
            this._culture = culture;

            this.Schema = new GridValidationSchema
            {
                ["CD_PRODUTO"] = this.ValidateProducto,
                ["NU_IDENTIFICADOR"] = this.ValidateIdentificador,
                ["QT_FACTURADA"] = this.ValidateCantidadFacturada,
                ["IM_UNITARIO_DIGITADO"] = this.ValidateUnitarioDigitado
            };
        }

        public virtual GridValidationGroup ValidateProducto(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var empresa = parameters.FirstOrDefault(s => s.Id == "empresa")?.Value;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value,40),
                    new ProductoExistsValidationRule(this._uow, empresa, cell.Value)
                },
                OnSuccess = this.ValidateProducto_OnSuccess,
                OnFailure = this.ValidateProducto_OnFailure,
            };
        }
        public virtual void ValidateProducto_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var fieldDescripcion = row.GetCell("DS_PRODUTO");
            var empresa = parameters.FirstOrDefault(s => s.Id == "empresa")?.Value;
            Producto producto = this._uow.ProductoRepository.GetProducto(int.Parse(empresa), row.GetCell("CD_PRODUTO").Value);

            if (fieldDescripcion != null)
                fieldDescripcion.Value = producto.Descripcion;

            if (parameters.Any(s => s.Id != "importExcel"))
            {
                // Dato ingresado por pantalla
                var manejo = this._uow.ProductoRepository.GetProductoManejoIdentificador(int.Parse(empresa), producto.Codigo);//cell.Value);

                if (manejo == ManejoIdentificador.Producto)
                {
                    row.GetCell("NU_IDENTIFICADOR").Value = ManejoIdentificadorDb.IdentificadorProducto;
                    row.GetCell("NU_IDENTIFICADOR").Editable = false;
                }
                else
                {
                    row.GetCell("NU_IDENTIFICADOR").Editable = true;
                }
            }
        }
        public virtual void ValidateProducto_OnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (row.GetCell("CD_PRODUTO") != null)
                row.GetCell("CD_PRODUTO").Value = string.Empty;
        }

        public virtual GridValidationGroup ValidateIdentificador(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {

            var cellProducto = row.GetCell("CD_PRODUTO");
            var agenda = parameters.FirstOrDefault(s => s.Id == "agenda")?.Value;
            var empresa = parameters.FirstOrDefault(s => s.Id == "empresa")?.Value;
            var faixa = parameters.FirstOrDefault(s => s.Id == "faixa")?.Value;

            var rules = new List<IValidationRule>();

            if (!string.IsNullOrEmpty(cellProducto.Value) && cellProducto.IsValid())
            {
                if (_uow.ProductoRepository.ExisteProducto(int.Parse(empresa), row.GetCell("CD_PRODUTO").Value))
                {
                    Producto producto = this._uow.ProductoRepository.GetPro(row.GetCell("CD_PRODUTO").Value, int.Parse(empresa));

                    cell.Value = producto.ParseIdentificador(cell.Value);

                    rules.Add(new StringMaxLengthValidationRule(cell.Value, 40));
                    rules.Add(new IdentificadorProductoValidationRule(this._uow, int.Parse(empresa), producto.Codigo, cell.Value));
                    //rules.Add(new IdentificadorProductoValidationRule(this._uow, empresa, row.GetCell("CD_PRODUTO").Value, cell.Value));

                    //if (row.IsNew || cell.Old != cell.Value || cellProducto.Value != cellProducto.Old)
                    //    rules.Add(new ProductoAgendaExistsValidationRule(_uow, agenda, empresa, row.GetCell("CD_PRODUTO").Value, faixa, cell.Value));
                }
            }

            return new GridValidationGroup
            {
                Rules = rules,
                Dependencies = { "CD_PRODUTO" },
                OnSuccess = this.ValidateIdentificador_OnSuccess
            };
        }
        public virtual void ValidateIdentificador_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            //var empresa = parameters.FirstOrDefault(s => s.Id == "empresa")?.Value;
            //var productoId = row.GetCell("CD_PRODUTO").Value;

            //Producto producto = this._uow.ProductoRepository.GetProducto(int.Parse(empresa), productoId);

            //cell.Value = producto.ParseIdentificador(cell.Value);
        }

        public virtual GridValidationGroup ValidateCantidadFacturada(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new PositiveDecimalValidationRule(this._culture, cell.Value, false),
                    new DecimalLengthWithPresicionValidationRule(cell.Value, 12, 3, this._culture)
                }
            };
        }

        public virtual GridValidationGroup ValidateUnitarioDigitado(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(cell.Value))
                return null;

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule> {
                    //new NonNullValidationRule(cell.Value),
                    new PositiveDecimalValidationRule(this._culture, cell.Value),
                    new DecimalLengthWithPresicionValidationRule(cell.Value, 15, 4, this._culture)
                }
            };
        }
    }
}
