using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Configuracion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.Impresiones;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoEstiloContenedoresGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        public MantenimientoEstiloContenedoresGridValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["CD_LABEL_ESTILO"] = this.ValidateIdEstiloEtiqueta,
                ["TP_CONTENEDOR"] = this.ValidateTipoEstiloEtiqueta,
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
                    new NoExisteEstiloEtiquetaValidationRule(cell.Value, this._uow)
                },
                OnSuccess = this.OnSuccessGridValidateIdEstiloEtiqueta
            };

            string tipoContenedor = row.GetCell("TP_CONTENEDOR").Value;
            if (!string.IsNullOrEmpty(tipoContenedor))
            {
                rules.Rules.Add(new ExisteEstiloContenedorValidationRule(cell.Value, tipoContenedor, this._uow));
            }

            return rules;
        }
        public virtual GridValidationGroup ValidateTipoEstiloEtiqueta(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var rules = new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 10),
                    new StringSoloUpperValidationRule(cell.Value),
                    new ExisteTipoContenedorValidationRule(cell.Value, this._uow)
                },
            };

            string estiloContenedor = row.GetCell("CD_LABEL_ESTILO").Value;
            if (!string.IsNullOrEmpty(estiloContenedor))
            {
                rules.Rules.Add(new ExisteEstiloContenedorValidationRule(estiloContenedor, cell.Value, this._uow));
            }

            return rules;
        }
        public virtual void OnSuccessGridValidateIdEstiloEtiqueta(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            EtiquetaEstilo etiqueta = this._uow.EstiloEtiquetaRepository.GetEtiquetaEstilo(cell.Value);

            row.GetCell("DS_LABEL_ESTILO").Value = etiqueta?.Descripcion; 
            row.GetCell("TP_LABEL").Value = etiqueta?.Tipo;
        }
    }
}
