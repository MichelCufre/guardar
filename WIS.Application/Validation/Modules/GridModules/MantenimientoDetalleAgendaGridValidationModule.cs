using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Recepcion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.Enums;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;
using WIS.Application.Validation.Rules.Registro;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoDetalleAgendaGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public MantenimientoDetalleAgendaGridValidationModule(IUnitOfWork uow, IFormatProvider culture)
        {
            this._uow = uow;
            this._culture = culture;

            this.Schema = new GridValidationSchema
            {
                ["CD_PRODUTO"] = this.ValidateProducto,
                ["NU_IDENTIFICADOR"] = this.ValidateIdentificador,
                ["QT_AGENDADO"] = this.ValidateCantidadAgendado,
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

            if (fieldDescripcion != null)
                fieldDescripcion.Value = this._uow.ProductoRepository.GetDescripcion(int.Parse(parameters.FirstOrDefault(s => s.Id == "empresa").Value), cell.Value);

            var empresa = parameters.FirstOrDefault(s => s.Id == "empresa")?.Value;

            if (parameters.Any(s => s.Id != "importExcel"))
            {
                var manejo = this._uow.ProductoRepository.GetProductoManejoIdentificador(int.Parse(empresa), cell.Value);

                if (manejo == ManejoIdentificador.Producto)
                {
                    row.GetCell("NU_IDENTIFICADOR").Value = ManejoIdentificadorDb.IdentificadorProducto;
                    row.GetCell("NU_IDENTIFICADOR").Editable = false;
                }
                else
                    row.GetCell("NU_IDENTIFICADOR").Editable = true;
            }
        }
        public virtual void ValidateProducto_OnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (row.GetCell("DS_PRODUTO") != null)
                row.GetCell("DS_PRODUTO").Value = string.Empty;
        }

        public virtual GridValidationGroup ValidateIdentificador(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var cellProducto = row.GetCell("CD_PRODUTO");
            var agenda = parameters.FirstOrDefault(s => s.Id == "agenda")?.Value;
            var empresa = int.Parse(parameters.FirstOrDefault(s => s.Id == "empresa").Value);
            var faixa = parameters.FirstOrDefault(s => s.Id == "faixa")?.Value;

            var rules = new List<IValidationRule>();

            if (!string.IsNullOrEmpty(cellProducto.Value) && cellProducto.IsValid())
            {
                if (_uow.ProductoRepository.ExisteProducto(empresa, row.GetCell("CD_PRODUTO").Value))
                {
                    var producto = this._uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(empresa, row.GetCell("CD_PRODUTO").Value);

                    cell.Value = producto.ParseIdentificador(cell.Value);

                    rules.Add(new StringMaxLengthValidationRule(cell.Value, 40));
                    rules.Add(new IdentificadorProductoValidationRule(this._uow, empresa, row.GetCell("CD_PRODUTO").Value, cell.Value));

                    if (row.IsNew || cell.Old != cell.Value || cellProducto.Value != cellProducto.Old)
                        rules.Add(new ProductoAgendaExistsValidationRule(_uow, agenda, empresa, row.GetCell("CD_PRODUTO").Value, faixa, cell.Value, this._culture));
                }
            }

            return new GridValidationGroup
            {
                Rules = rules,
                Dependencies = { "CD_PRODUTO" }
            };
        }

        public virtual GridValidationGroup ValidateCantidadAgendado(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var idAgenda = parameters.FirstOrDefault(s => s.Id == "agenda")?.Value;
            var empresa = parameters.FirstOrDefault(s => s.Id == "empresa")?.Value;

            var codigoProducto = row.GetCell("CD_PRODUTO").Value;
            var identificador = row.GetCell("NU_IDENTIFICADOR").Value;

            if (string.IsNullOrEmpty(codigoProducto) || string.IsNullOrEmpty(identificador))
                return null;

            var producto = _uow.ProductoRepository.GetProducto(int.Parse(empresa), codigoProducto);

            var rules = new List<IValidationRule> {
                new NonNullValidationRule(cell.Value),
                new PositiveDecimalValidationRule(this._culture, cell.Value, allowZero: false),
                new DecimalLengthWithPresicionValidationRule(cell.Value, 12, 3, this._culture, producto.AceptaDecimales),
                new SaldoDetalleAgendaFacturaValidationRule(_uow, idAgenda, empresa, codigoProducto, identificador, cell.Value, cell.Old, this._culture)
            };

            if (identificador != ManejoIdentificadorDb.IdentificadorAuto)
                rules.Add(new ManejoIdentificadorSerieCantidadValidationRule(cell.Value, producto.ManejoIdentificador, _culture));

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules
            };
        }
    }
}
