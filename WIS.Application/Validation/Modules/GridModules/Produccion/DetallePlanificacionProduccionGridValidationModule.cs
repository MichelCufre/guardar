using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Produccion;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class DetallePlanificacionProduccionGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _empresa;
        protected readonly string _cliente;
        protected readonly string _idIngreso;
        protected readonly bool _planificacionPedido;
        protected readonly IFormatProvider _formatProvider;

        public DetallePlanificacionProduccionGridValidationModule(IUnitOfWork uow, IFormatProvider formatProvider, int empresa, string idIngreso, bool planificacionPedido)
        {
            this._uow = uow;
            this._empresa = empresa;
            this._formatProvider = formatProvider;
            this._idIngreso = idIngreso;
            this._planificacionPedido = planificacionPedido;

            this.Schema = new GridValidationSchema
            {
                ["CD_PRODUTO"] = this.ValidateProducto,
                ["NU_IDENTIFICADOR"] = this.ValidateIdentificador,
                ["QT_RESERVAR"] = this.ValidateCantidadReserva,
                ["QT_PEDIR"] = this.ValidateCantidadPedir
            };
        }

        public virtual GridValidationGroup ValidateProducto(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule> {
                new NonNullValidationRule(cell.Value),
                new StringMaxLengthValidationRule(cell.Value, 40),
                new ProductoExistsValidationRule(this._uow, this._empresa.ToString(), cell.Value)
            };

            if (row.IsNew)
                rules.Add(new ProductoPlanificadoUnicoValidationRule(this._uow, _empresa, _idIngreso, cell.Value, _planificacionPedido));

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = this.ValidateProductoOnSuccess
            };
        }
        public virtual void ValidateProductoOnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var cdProducto = row.GetCell("CD_PRODUTO").Value;

            var producto = _uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(_empresa, cdProducto);

            row.GetCell("DS_PRODUTO").Value = producto.Descripcion;

            if (producto.IsIdentifiedByProducto())
            {
                row.GetCell("NU_IDENTIFICADOR").Editable = true;
                row.GetCell("NU_IDENTIFICADOR").Value = "*";
                row.GetCell("NU_IDENTIFICADOR").Editable = false;
            }
            else if (string.IsNullOrEmpty(row.GetCell("NU_IDENTIFICADOR").Value) || (!producto.IsIdentifiedByProducto() && row.GetCell("NU_IDENTIFICADOR").Value == ManejoIdentificadorDb.IdentificadorProducto))
            {
                row.GetCell("NU_IDENTIFICADOR").Editable = true;
                row.GetCell("NU_IDENTIFICADOR").Value = ManejoIdentificadorDb.IdentificadorAuto;
            }
        }

        public virtual GridValidationGroup ValidateIdentificador(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var cdProducto = row.GetCell("CD_PRODUTO").Value;

            var rules = new List<IValidationRule>();

            if (_uow.ProductoRepository.ExisteProducto(_empresa, cdProducto))
            {
                var producto = this._uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(_empresa, cdProducto);
                cell.Value = producto.ParseIdentificador(cell.Value);

                rules.Add(new StringMaxLengthValidationRule(cell.Value, 40));
                rules.Add(new IdentificadorProductoValidationRule(this._uow, _empresa, cdProducto, cell.Value));

                if ((row.IsNew || (!row.IsNew && row.IsModified && cell.Old != cell.Value)))
                {
                    var idIngreso = parameters.FirstOrDefault(f => f.Id == "idIngreso").Value;
                    rules.Add(new ProductoLotePlanificadoUnicoValidationRule(this._uow, _empresa, idIngreso, cdProducto, cell.Value, _planificacionPedido));
                }
            }

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Dependencies = { "CD_PRODUTO" },
                Rules = rules,
            };
        }

        public virtual GridValidationGroup ValidateCantidadReserva(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var cdProducto = row.GetCell("CD_PRODUTO").Value;
            var identificador = row.GetCell("NU_IDENTIFICADOR").Value;

            if (row.IsNew || string.IsNullOrEmpty(cdProducto) || string.IsNullOrEmpty(identificador))
                return null;

            decimal.TryParse(row.Cells.FirstOrDefault(x => x.Column.Id == "QT_DISPONIBLE").Value, this._formatProvider, out decimal qtDisponible);
            decimal.TryParse(row.Cells.FirstOrDefault(x => x.Column.Id == "QT_RESERVAR").Value, this._formatProvider, out decimal qtReservar);
            decimal.TryParse(row.Cells.FirstOrDefault(x => x.Column.Id == "QT_RESERVA").Value, this._formatProvider, out decimal qtReserva);

            var producto = _uow.ProductoRepository.GetProducto(_empresa, cdProducto);

            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(cell.Value),
                new PositiveDecimalValidationRule(this._formatProvider, cell.Value),
                new DecimalLengthWithPresicionValidationRule(cell.Value, 12, 3, this._formatProvider, producto.AceptaDecimales),
                new NumeroDecimalMenorOIgualQueValidationRule(qtReservar, qtDisponible + qtReserva ),
            };

            if (identificador != ManejoIdentificadorDb.IdentificadorAuto)
                rules.Add(new ManejoIdentificadorSerieCantidadValidationRule(cell.Value, producto.ManejoIdentificador, _formatProvider, allowZero: true));

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = this.ValidateCantidadReservaOnSuccess
            };
        }
        public virtual void ValidateCantidadReservaOnSuccess(GridCell cell, GridRow row, List<ComponentParameter> list)
        {
            decimal.TryParse(row.Cells.FirstOrDefault(x => x.Column.Id == "QT_RESERVAR").Value, this._formatProvider, out decimal qtReserva);
            decimal.TryParse(row.Cells.FirstOrDefault(x => x.Column.Id == "QT_PENDIENTE").Value, this._formatProvider, out decimal qtPendiente);
            decimal cantidad = 0;
            if (qtPendiente - qtReserva > 0)
            {
                cantidad = qtPendiente - qtReserva;
            }
            row.Cells.FirstOrDefault(x => x.Column.Id == "QT_PEDIR_SUGERIDA").Value = cantidad.ToString(_formatProvider);
        }

        public virtual GridValidationGroup ValidateCantidadPedir(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var cdProducto = row.GetCell("CD_PRODUTO").Value;
            var identificador = row.GetCell("NU_IDENTIFICADOR").Value;

            if (string.IsNullOrEmpty(cdProducto) || string.IsNullOrEmpty(identificador))
                return null;

            var producto = _uow.ProductoRepository.GetProducto(_empresa, cdProducto);

            var rules = new List<IValidationRule>
            {
                new PositiveDecimalValidationRule(this._formatProvider, cell.Value),
                new DecimalLengthWithPresicionValidationRule(cell.Value, 12, 3, this._formatProvider, producto.AceptaDecimales),
            };

            if (identificador != ManejoIdentificadorDb.IdentificadorAuto)
                rules.Add(new ManejoIdentificadorSerieCantidadValidationRule(cell.Value, producto.ManejoIdentificador, _formatProvider, allowZero: true));

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = this.ValidateCantidadPedirOnSuccess
            };
        }
        public virtual void ValidateCantidadPedirOnSuccess(GridCell cell, GridRow row, List<ComponentParameter> list)
        {

            var cellCantidadPedir = row.Cells.FirstOrDefault(x => x.Column.Id == "QT_PEDIR");
            var cellCantidadPedirSugerida = row.Cells.FirstOrDefault(x => x.Column.Id == "QT_PEDIR_SUGERIDA");
            decimal.TryParse(cellCantidadPedir.Value, this._formatProvider, out decimal cantidadPedir);

            if (cellCantidadPedirSugerida != null)
            {
                if (cantidadPedir > 0)
                {
                    cellCantidadPedir.CssClass = "lightGreen";
                    cellCantidadPedirSugerida.CssClass = "";
                }
                else
                {
                    cellCantidadPedir.CssClass = "";
                    cellCantidadPedirSugerida.CssClass = "lightGreen";
                }
            }
            else
            {
                var cellCantidadPendiente = row.Cells.FirstOrDefault(x => x.Column.Id == "QT_PENDIENTE");
                if (cantidadPedir > 0)
                {
                    cellCantidadPedir.CssClass = "lightGreen";
                    cellCantidadPendiente.CssClass = "";
                }
                else
                {
                    cellCantidadPedir.CssClass = "";
                    cellCantidadPendiente.CssClass = "lightGreen";
                }
            }
        }
    }
}
