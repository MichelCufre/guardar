using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class INV410GridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public INV410GridValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["DS_INVENTARIO"] = this.GridValidateDsInventario,
                ["ND_CIERRE_CONTEO"] = this.GridValidateCierreConteo,
                ["FL_CONTROLAR_VENCIMIENTO"] = this.GridValidateBooleanInString,
                ["FL_BLOQ_USR_CONTEO_SUCESIVO"] = this.GridValidateBooleanInString,
                ["FL_MODIFICAR_STOCK_EN_DIF"] = this.GridValidateBooleanInString,
                ["FL_PERMITE_INGRESO_MOTIVO"] = this.GridValidateBooleanInString,
                ["FL_ACTUALIZAR_CONTEO_FIN_AUTO"] = this.GridValidateBooleanInString,
                ["FL_EXCLUIR_SUELTOS"] = this.GridValidateBooleanInStringExcluir,
                ["FL_EXCLUIR_LPNS"] = this.GridValidateBooleanInStringExcluir,
                ["FL_RESTABLECER_LPN_FINALIZADO"] = this.GridValidateBooleanInString,
                ["FL_GENERAR_PRIMER_CONTEO"] = this.GridValidateGenerarPrimerConteo,
            };
        }

        public virtual GridValidationGroup GridValidateDsInventario(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(cell.Value, 200)
                }
            };
        }
        public virtual GridValidationGroup GridValidateCierreConteo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 20),
                    new ExisteEstadoCierreConteo(this._uow, cell.Value)
                },
                OnFailure = this.OnFailure_GridValidateCierreConteo,
                OnSuccess = this.OnSuccess_GridValidateCierreConteo

            };
        }

        public virtual void OnFailure_GridValidateCierreConteo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("DS_CIERRE_CONTEO").Value = string.Empty;
        }

        public virtual void OnSuccess_GridValidateCierreConteo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("DS_CIERRE_CONTEO").Value = this._uow.DominioRepository.GetDominio(cell.Value)?.Descripcion;
        }

        public virtual GridValidationGroup GridValidateBooleanInString(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new BooleanStringGridValidationRule(cell.Value)
                }
            };
        }

        public virtual GridValidationGroup GridValidateBooleanInStringExcluir(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var excluirSuelto = row.GetCell("FL_EXCLUIR_SUELTOS").Value;
            var excluirLpn = row.GetCell("FL_EXCLUIR_LPNS").Value;

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new BooleanStringGridValidationRule(cell.Value),
                    new ExcluirLpnAndSuelto(excluirSuelto,excluirLpn)
                }
            };
        }

        public virtual GridValidationGroup GridValidateGenerarPrimerConteo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var tipoCierreConteo = row.GetCell("ND_CIERRE_CONTEO").Value;

            if (string.IsNullOrEmpty(tipoCierreConteo))
                return null;

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Dependencies = { "ND_CIERRE_CONTEO" },
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new BooleanStringGridValidationRule(cell.Value),
                    new GenerarPrimerConteoValidationRule(cell.Value, tipoCierreConteo)
                }
            };
        }
    }
}
