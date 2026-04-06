using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Produccion
{
    public class PRD113ProducirGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;
        public PRD113ProducirGridValidationModule(IUnitOfWork uow, IFormatProvider culture)
        {
            this._uow = uow;
            this._culture = culture;

            this.Schema = new GridValidationSchema
            {
                ["QT_PRODUCIR"] = this.ValidateCantidadProducir,
                ["ND_MOTIVO"] = this.ValidateMotivo,
            };
        }
        public virtual GridValidationGroup ValidateCantidadProducir(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var codigoProducto = row.GetCell("CD_PRODUTO").Value;
            var nuIngresoProduccion = parameters.FirstOrDefault(p => p.Id == "nuIngresoProduccion")?.Value;

            if (string.IsNullOrEmpty(codigoProducto) || string.IsNullOrEmpty(nuIngresoProduccion))
                return null;

            var ingreso = _uow.IngresoProduccionRepository.GetIngresoById(nuIngresoProduccion);

            var producto = _uow.ProductoRepository.GetProducto(ingreso.Empresa.Value, codigoProducto);

            return new GridValidationGroup
            {
                Dependencies = new List<string>() { "ND_MOTIVO" },
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new PositiveDecimalValidationRule(this._culture, cell.Value, false),
                    new DecimalLengthWithPresicionValidationRule(cell.Value, 12, 3, this._culture,producto.AceptaDecimales),
                    new ManejoIdentificadorSerieCantidadValidationRule(cell.Value, producto.ManejoIdentificador, _culture)
                }
            };
        }

        public virtual GridValidationGroup ValidateMotivo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new ExisteDominioValidationRule(cell.Value, this._uow),
                },
                OnSuccess = this.ValidateMotivoProducirOnSuccess
            };
        }

        public virtual void ValidateMotivoProducirOnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var descripcionMotivo = this._uow.DominioRepository.GetDominio(cell.Value).Descripcion;
            row.GetCell("DS_MOTIVO").Value = descripcionMotivo;
        }
    }
}
