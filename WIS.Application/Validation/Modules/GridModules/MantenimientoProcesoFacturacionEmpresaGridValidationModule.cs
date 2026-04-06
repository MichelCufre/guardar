using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Facturacion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.Facturacion;
using WIS.Domain.General;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoProcesoFacturacionEmpresaGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;
        public MantenimientoProcesoFacturacionEmpresaGridValidationModule(IUnitOfWork uow, IFormatProvider culture)
        {
            this._uow = uow;
            this._culture = culture;

            this.Schema = new GridValidationSchema
            {
                ["CD_EMPRESA"] = this.ValidateCodigoEmpresa,
                ["CD_PROCESO"] = this.ValidateCodigoProceso,
                ["QT_RESULTADO"] = this.ValidateResultado,
            };
        }

        public virtual GridValidationGroup ValidateCodigoEmpresa(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var rules = new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value)
                },
                OnSuccess = ValidateCodigoEmpresa_OnSuccess
            };

            string cdProceso = row.GetCell("CD_PROCESO").Value;

            if (!string.IsNullOrEmpty(cdProceso))
            {
                rules.Rules.Add(new ExisteFacturacionEmpresaProcesoRegistradoValidationRule(_uow, int.Parse(cell.Value), cdProceso));
            }

            return rules;
        }

        public virtual GridValidationGroup ValidateCodigoProceso(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var rules = new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value)
                },
                OnSuccess = ValidateCodigoProceso_OnSuccess
            };

            string cdEmpresa = row.GetCell("CD_EMPRESA").Value;

            if (!string.IsNullOrEmpty(cdEmpresa))
            {
                rules.Rules.Add(new ExisteFacturacionEmpresaProcesoRegistradoValidationRule(_uow, int.Parse(cdEmpresa), cell.Value));
            }

            return rules;
        }
        public virtual GridValidationGroup ValidateResultado(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            string cdProceso = row.GetCell("CD_PROCESO").Value;

            if (!string.IsNullOrEmpty(cdProceso))
            {
                FacturacionProceso facturacionProceso = this._uow.FacturacionRepository.GetFacturacionProceso(cdProceso);

                if (facturacionProceso.TipoProceso != "P")
                    return null;
            }
            else
                return null;

            row.GetCell("QT_RESULTADO").Value = row.GetCell("QT_RESULTADO").Value.Replace(".", ",");

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new PositiveDecimalValidationRule(this._culture, cell.Value)
                }
            };
        }
        public virtual void ValidateCodigoEmpresa_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            Empresa empresa = this._uow.EmpresaRepository.GetEmpresa(int.Parse(cell.Value));

            row.GetCell("NM_EMPRESA").Value = empresa.Nombre;

        }
        public virtual void ValidateCodigoProceso_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            FacturacionProceso facturacionProceso = this._uow.FacturacionRepository.GetFacturacionProceso(cell.Value);
            
            row.GetCell("TP_PROCESO").Value = facturacionProceso.TipoProceso;
            row.GetCell("DS_PROCESO").Value = facturacionProceso.DescripcionProceso;

            if (facturacionProceso.TipoProceso == "P")
                row.GetCell("QT_RESULTADO").Editable = true;
            else
                row.GetCell("QT_RESULTADO").Editable = false;
        }
    }
}
