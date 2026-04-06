using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Recepcion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class AjusteEtiquetaRecepcionGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public AjusteEtiquetaRecepcionGridValidationModule(IUnitOfWork uow, IFormatProvider culture)
        {
            this._uow = uow;
            this._culture = culture;

            this.Schema = new GridValidationSchema
            {
                ["QT_PRODUTO_RECIBIDO"] = this.ValidateProductoRecibido,
            };
        }
        public virtual GridValidationGroup ValidateProductoRecibido(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(row.GetCell("CD_EMPRESA").Value))
            {
                cell.SetError(new ComponentError("General_Sec0_Error_CampoEmpresaReuqerido", new List<string>()));
                return null;
            }

            var empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
            var producto = _uow.ProductoRepository.GetProducto(empresa, row.GetCell("CD_PRODUTO").Value);
            var etiqueta = _uow.EtiquetaLoteRepository.GetEtiquetaLote(int.Parse(row.GetCell("NU_ETIQUETA_LOTE").Value));
            var rules = new List<IValidationRule>()
            {
                new NonNullValidationRule(cell.Value),
                new PositiveDecimalValidationRule(this._culture, cell.Value),
                new DecimalLengthWithPresicionValidationRule(cell.Value, 12, 3, this._culture, producto.AceptaDecimales),
                new DecimalLowerThanValidationRule(this._culture, cell.Value, decimal.Parse(row.GetCell("QT_PRODUTO").Value, this._culture)),
                new ManejoIdentificadorSerieCantidadValidationRule(cell.Value, producto.ManejoIdentificador, _culture, allowZero: true),
                new TieneSugerenciaPendienteRule(etiqueta,_uow)
            };

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = this.ValidateProductoRecibido_OnSuccess
            };
        }

        public virtual void ValidateProductoRecibido_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            cell.Value = decimal.Parse(cell.Value, this._culture).ToString(this._culture);
        }
    }
}
