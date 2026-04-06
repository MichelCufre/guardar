using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Extension;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    class MantenimientoDetalleReferenciaRecepcionGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;
        public MantenimientoDetalleReferenciaRecepcionGridValidationModule(IUnitOfWork uow, IFormatProvider culture)
        {
            this._uow = uow;
            this._culture = culture;

            this.Schema = new GridValidationSchema
            {
                ["ID_LINEA_SISTEMA_EXTERNO"] = this.ValidateLineaSistemaExterno,
                ["CD_PRODUTO"] = this.ValidateProducto,
                ["NU_IDENTIFICADOR"] = this.ValidateIdentificador,
                ["QT_REFERENCIA"] = this.ValidateCantidadReferencia,
                ["DT_VENCIMIENTO"] = this.ValidateVencimiento,
                ["DS_ANEXO1"] = this.ValidateAnexo1,
                ["IM_UNITARIO"] = this.ValidateUnitario,
            };
        }

        public virtual GridValidationGroup ValidateLineaSistemaExterno(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {


            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value,40),

                },
            };
        }

        public virtual GridValidationGroup ValidateProducto(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {

            var empresa = parameters.FirstOrDefault(s => s.Id == "empresa")?.Value;
            if (string.IsNullOrEmpty(empresa))
            {
                cell.SetError(new ComponentError("General_Sec0_Error_CampoEmpresaReuqerido", new List<string>()));
                return null;
            }

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value,40),
                    new ProductoExistsValidationRule(this._uow, empresa, cell.Value)
                },
                OnSuccess = this.ValidateProducto_OnSuccess,
                OnFailure = this.ValidateProducto_OnFailure,
                Dependencies = { "ID_LINEA_SISTEMA_EXTERNO" }
            };

        }

        public virtual void ValidateProducto_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var fieldDescripcion = row.GetCell("DS_PRODUTO");

            if (fieldDescripcion != null)
                fieldDescripcion.Value = this._uow.ProductoRepository.GetDescripcion(int.Parse(parameters.FirstOrDefault(s => s.Id == "empresa").Value), cell.Value);

            var empresa = parameters.FirstOrDefault(s => s.Id == "empresa")?.Value;

            if (!parameters.Any(s => s.Id == "importExcel"))
            {
                // Dato ingresado por pantalla
                var manejo = this._uow.ProductoRepository.GetProductoManejoIdentificador(int.Parse(empresa), cell.Value);

                if (manejo == ManejoIdentificador.Producto)
                {
                    // Se comenta manejo de identificador editable, ya que no tengo forma de saltear el campo con el foco
                    //row.GetCell("NU_IDENTIFICADOR").Editable = false;
                    row.GetCell("NU_IDENTIFICADOR").Value = ManejoIdentificadorDb.IdentificadorProducto;
                }
                else
                {
                    //row.GetCell("NU_IDENTIFICADOR").Editable = true;
                }

                var manejofecha = this._uow.ProductoRepository.GetProductoManejoFecha(int.Parse(empresa), cell.Value);

                if (manejofecha == ManejoFechaProducto.Expirable)
                {
                    row.GetCell("DT_VENCIMIENTO").Editable = true;
                }
                else if (manejofecha == ManejoFechaProducto.Fifo)
                {
                    row.GetCell("DT_VENCIMIENTO").Editable = false;
                    row.GetCell("DT_VENCIMIENTO").Value = DateTime.Now.ToIsoString();
                }

            }

        }
        public virtual void ValidateProducto_OnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (row.GetCell("DS_PRODUTO") != null)
                row.GetCell("DS_PRODUTO").Value = string.Empty;
        }

        public virtual GridValidationGroup ValidateIdentificador(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {

            if (string.IsNullOrEmpty(parameters.FirstOrDefault(s => s.Id == "empresa")?.Value))
            {
                cell.SetError(new ComponentError("General_Sec0_Error_CampoEmpresaReuqerido", new List<string>()));
                return null;
            }

            var empresa = int.Parse(parameters.FirstOrDefault(s => s.Id == "empresa").Value);

            var cellProducto = row.GetCell("CD_PRODUTO");

            var rules = new List<IValidationRule>();

            if (!string.IsNullOrEmpty(cellProducto.Value) && cellProducto.IsValid())
            {
                if (_uow.ProductoRepository.ExisteProducto(empresa, row.GetCell("CD_PRODUTO").Value))
                {
                    Producto producto = this._uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(empresa, row.GetCell("CD_PRODUTO").Value);
                    cell.Value = producto.ParseIdentificador(cell.Value);

                    rules.Add(new StringMaxLengthValidationRule(cell.Value, 40));
                    rules.Add(new IdentificadorProductoValidationRule(this._uow, empresa, row.GetCell("CD_PRODUTO").Value, cell.Value));
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

        public virtual GridValidationGroup ValidateCantidadReferencia(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var empresa = parameters.FirstOrDefault(s => s.Id == "empresa")?.Value;

            if (string.IsNullOrEmpty(empresa))
            {
                cell.SetError(new ComponentError("General_Sec0_Error_CampoEmpresaReuqerido", new List<string>()));
                return null;
            }

            var cellProducto = row.GetCell("CD_PRODUTO");
            var cellIdentificador = row.GetCell("NU_IDENTIFICADOR");

            if (string.IsNullOrEmpty(cellProducto.Value) || !cellProducto.IsValid() || string.IsNullOrEmpty(cellIdentificador.Value) || !cellIdentificador.IsValid())
                return null;

            var producto = _uow.ProductoRepository.GetProducto(int.Parse(empresa), cellProducto.Value);
            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(cell.Value),
                new PositiveDecimalValidationRule(this._culture, cell.Value),
                new DecimalLengthWithPresicionValidationRule(cell.Value, 15, 3, this._culture, producto.AceptaDecimales),
            };

            if (cellIdentificador.Value != ManejoIdentificadorDb.IdentificadorAuto)
                rules.Add(new ManejoIdentificadorSerieCantidadValidationRule(cell.Value, producto.ManejoIdentificador, _culture));

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                Dependencies = { "CD_PRODUTO" },
                OnSuccess = this.ValidateCantidadReferencia_OnSuccess
            };
        }
        public virtual void ValidateCantidadReferencia_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var empresa = parameters.FirstOrDefault(s => s.Id == "empresa")?.Value;
            var aceptaDecimales = this._uow.ProductoRepository.ProductoAceptaDecimales(int.Parse(empresa), row.GetCell("CD_PRODUTO").Value);

            if (aceptaDecimales)
            {
                cell.Value = decimal.Parse(cell.Value, this._culture).ToString(this._culture);
            }
            else
            {
                cell.Value = int.Parse(cell.Value).ToString();
            }
        }

        public virtual GridValidationGroup ValidateVencimiento(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(cell.Value))
                return null;

            var empresa = parameters.FirstOrDefault(s => s.Id == "empresa")?.Value;
            var cellProducto = row.GetCell("CD_PRODUTO");

            if (string.IsNullOrEmpty(cellProducto.Value) || !cellProducto.IsValid())
                return null;

            var manejofecha = this._uow.ProductoRepository.GetProductoManejoFecha(int.Parse(empresa), cellProducto.Value);

            if (manejofecha != ManejoFechaProducto.Expirable)
                return null;

            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(cell.Value)
            };

            if (parameters.Any(s => s.Id == "importExcel"))
            {
                rules.Add(new DateTimeImportExcelValidationRule(this._culture, cell.Value));
            }
            else
            {
                rules.Add(new DateTimeValidationRule(cell.Value));

            }

            return new GridValidationGroup
            {
                Rules = rules,
                Dependencies = { "CD_PRODUTO" },
                OnSuccess = this.ValidateVencimiento_OnSuccess
            };
        }
        public virtual void ValidateVencimiento_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (parameters.Any(s => s.Id == "importExcel"))
            {
                if (DateTime.TryParse(cell.Value, _culture, DateTimeStyles.None, out DateTime fecha))
                    cell.Value = fecha.ToIsoString();
            }
        }

        public virtual GridValidationGroup ValidateAnexo1(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(cell.Value))
                return null;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                      new StringMaxLengthValidationRule(cell.Value, 200),
                },
            };
        }

        public virtual GridValidationGroup ValidateUnitario(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(cell.Value))
                return null;

            return new GridValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                    {
                        new PositiveDecimalValidationRule(this._culture, cell.Value),
                        new DecimalLengthWithPresicionValidationRule(cell.Value, 15, 3, this._culture),
                    }
            };
        }
    }
}