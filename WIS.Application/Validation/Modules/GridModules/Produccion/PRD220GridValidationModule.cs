using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Produccion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Extension;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Produccion
{
    public class PRD220GridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public PRD220GridValidationModule(IUnitOfWork uow,
            IFormatProvider culture)
        {
            _uow = uow;
            _culture = culture;

            Schema = new GridValidationSchema
            {
                ["CD_PRODUTO"] = ValidateProducto,
                ["NU_IDENTIFICADOR"] = ValidateIdentificador,
                ["DT_VENCIMIENTO"] = ValidateVencimiento,
                ["QT_PRODUCIDO"] = ValidateCantidad
            };
        }

        public virtual GridValidationGroup ValidateProducto(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (!row.IsNew)
                return null;

            string nroIngreso = parameters.FirstOrDefault(d => d.Id == "NU_PRDC_INGRESO").Value;
            int cdEmpresa = int.Parse(parameters.FirstOrDefault(d => d.Id == "CD_EMPRESA").Value);

            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(cell.Value),
                new StringMaxLengthValidationRule(cell.Value, 40),
                new ProductoExistsValidationRule(_uow, cdEmpresa.ToString(), cell.Value)
            };

            if (row.IsNew && !string.IsNullOrEmpty(cell.Value))
            {
                Producto producto = _uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(cdEmpresa, cell.Value);

                if (producto != null && producto.IsIdentifiedByProducto())
                    rules.Add(new ProduccionProductoProducidoNotExistsValidationRule(_uow, nroIngreso, cdEmpresa, cell.Value, producto.ParseIdentificador(null), 1));
            }

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = ValidateProductoOnSuccess,
                OnFailure = ValidateProductoOnFailure
            };
        }

        public virtual GridValidationGroup ValidateIdentificador(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (!row.IsNew)
                return null;

            string nroIngreso = parameters.FirstOrDefault(d => d.Id == "NU_PRDC_INGRESO").Value;
            int cdEmpresa = int.Parse(parameters.FirstOrDefault(d => d.Id == "CD_EMPRESA").Value);
            string cdProducto = row.GetCell("CD_PRODUTO").Value;

            if (string.IsNullOrEmpty(cdProducto))
                return null;

            var rules = new List<IValidationRule> {
                new StringMaxLengthValidationRule(cell.Value, 40)
            };

            Producto producto = _uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(cdEmpresa, cdProducto);
            cell.Value = producto.ParseIdentificador(cell.Value);

            if (row.IsNew)
            {
                if (producto != null)
                    rules.Add(new ProduccionProductoProducidoNotExistsValidationRule(_uow, nroIngreso, cdEmpresa, cdProducto, cell.Value, 1));
            }

            return new GridValidationGroup
            {
                Dependencies = { "CD_PRODUTO" },
                BreakValidationChain = true,
                Rules = rules
            };
        }

        public virtual GridValidationGroup ValidateVencimiento(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (!row.IsNew)
                return null;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringIsoDateToDateTimeValidationRule(cell.Value)
                }
            };
        }

        public virtual GridValidationGroup ValidateCantidad(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            int cdEmpresa = int.Parse(parameters.FirstOrDefault(d => d.Id == "CD_EMPRESA").Value);

            string cdProducto = row.GetCell("CD_PRODUTO").Value;

            Producto producto = _uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(cdEmpresa, cdProducto);

            if (producto == null)
                return null;

            if (producto.AceptaDecimales)
            {
                return new GridValidationGroup
                {
                    Rules = new List<IValidationRule> {
                        new NonNullValidationRule(cell.Value),
                        new DecimalCultureSeparatorValidationRule(this._culture, cell.Value),
                        new PositiveDecimalValidationRule(this._culture, cell.Value),
                        new DecimalLengthWithPresicionValidationRule(cell.Value, 12, 3, this._culture),
                        new NumeroDecimalMayorQueValidationRule(decimal.Parse(cell.Value, this._culture),0)
                    }
                };
            }

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new PositiveIntValidationRule(cell.Value),
                    new NumeroEnteroMayorQueValidationRule(int.Parse(cell.Value),0)
                }
            };
        }

        public virtual void ValidateProductoOnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            int cdEmpresa = int.Parse(parameters.FirstOrDefault(d => d.Id == "CD_EMPRESA").Value);
            var cellDsProducto = row.GetCell("DS_PRODUTO");
            var cellIdentificador = row.GetCell("NU_IDENTIFICADOR");
            var cellVencimiento = row.GetCell("DT_FABRICACAO");

            Producto producto = _uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(cdEmpresa, cell.Value);

            cellDsProducto.Value = producto.Descripcion;

            if (producto.IsIdentifiedByProducto())
            {
                cellIdentificador.Value = producto.ParseIdentificador(string.Empty);
                cellIdentificador.Editable = false;
            }
            else
            {
                cellIdentificador.Editable = true;
            }

            cellVencimiento.Editable = false;

            if (producto.IsFifo())
            {
                cellVencimiento.Value = DateTime.Now.ToIsoString();
                cellVencimiento.Editable = true;
            }
            else if (producto.IsFefo())
            {
                cellVencimiento.Editable = true;
            }
            else
            {
                cellVencimiento.Value = string.Empty;
            }
        }

        public virtual void ValidateProductoOnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("DS_PRODUTO").Value = string.Empty;
        }
    }
}
