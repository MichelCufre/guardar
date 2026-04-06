using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Configuracion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoEstiloEtiquetaGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        public MantenimientoEstiloEtiquetaGridValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["CD_LABEL_ESTILO"] = this.ValidateIdEstiloEtiqueta,
                ["DS_LABEL_ESTILO"] = this.ValidateDescipcionEstiloEtiqueta,
                ["TP_LABEL"] = this.ValidateTipoEstiloEtiqueta,
            };
        }

        public virtual GridValidationGroup ValidateIdEstiloEtiqueta(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var rules = new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 15),
                    new StringSoloUpperValidationRule(cell.Value),
                }
            };

            if (row.IsNew || row.IsModified && row.GetCell("CD_LABEL_ESTILO").Old != row.GetCell("CD_LABEL_ESTILO").Value)
            {
                rules.Rules.Add(new ExisteEstiloEtiquetaValidationRule(cell.Value, this._uow));
            }

            return rules;
        }

        public virtual GridValidationGroup ValidateDescipcionEstiloEtiqueta(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new StringMaxLengthValidationRule(cell.Value, 30),
                }
            };
        }

        public virtual GridValidationGroup ValidateTipoEstiloEtiqueta(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            string dominio = cell.Value;

            if (dominio.Length <= 3 && dominio != "")
                dominio = CodigoDominioDb.TipoEtiquetas + cell.Value;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 20),
                    new ExisteDominioValidationRule(dominio, this._uow)
                },
                OnSuccess = this.OnSuccessGridValidateTipoEstiloEtiqueta
            };
        }
        public virtual void OnSuccessGridValidateTipoEstiloEtiqueta(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("DS_DOMINIO_VALOR").Value = this._uow.DominioRepository.GetDominio(cell.Value)?.Descripcion;
        }

    }
}
