using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.Documento;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Documento
{
    public class DOC340GridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public DOC340GridValidationModule(
            string tpDocumento,
            string nuDocumento,
            IUnitOfWork uow,
            IFormatProvider culture)
        {
            this._uow = uow;
            this._culture = culture;

            var documento = uow.DocumentoRepository.GetDocumento(nuDocumento, tpDocumento);

            if (documento is IDocumentoEgreso)
            {
                Schema = new GridValidationSchema
                {
                    ["VL_TRIBUTO"] = this.ValidateImporte,
                    ["NU_PROCESO"] = this.ValidateProceso
                };
            }
            else if (documento is IDocumentoActa)
            {
                var acta = (IDocumentoActa)documento;
                if (acta.Lineas.Count > 0)
                {
                    // Acta positiva
                    Schema = new GridValidationSchema
                    {
                        ["VL_TRIBUTO"] = this.ValidateImporte,
                        ["VL_MERCADERIA"] = this.ValidateImporte,
                        ["VL_CIF_INGRESO"] = this.ValidateImporte
                    };
                }
                else
                {
                    //Negativa
                    Schema = new GridValidationSchema
                    {
                        ["VL_TRIBUTO"] = this.ValidateImporte,
                        ["VL_CIF_INGRESO"] = this.ValidateImporte,
                        ["VL_FOB_INGRESO"] = this.ValidateImporte,
                        ["NU_PROCESO"] = this.ValidateProceso
                    };
                }
            }
        }

        public virtual GridValidationGroup ValidateImporte(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(cell.Value))
                return new GridValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                    {
                        new DecimalLengthWithPresicionValidationRule(cell.Value,12,3, this._culture),
                        new DecimalCultureSeparatorValidationRule(this._culture, cell.Value)
                    }
                };
            else
                return null;
        }

        public virtual GridValidationGroup ValidateProceso(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(cell.Value))
                return new GridValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(cell.Value,40)
                    }
                };
            else
                return null;
        }
    }
}
