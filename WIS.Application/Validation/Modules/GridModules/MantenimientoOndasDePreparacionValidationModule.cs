using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Liberacion;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoOndasDePreparacionValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _idUsuario;

        public MantenimientoOndasDePreparacionValidationModule(IUnitOfWork uow, int idUsuario)
        {
            this._uow = uow;
            this._idUsuario = idUsuario;

            this.Schema = new GridValidationSchema
            {
                ["CD_ONDA"] = this.ValidateCodigo,
                ["DS_ONDA"] = this.ValidateDescripcion,
                ["NU_PREDIO"] = this.ValidatePredio,
            };
        }

        public virtual GridValidationGroup ValidateCodigo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (!row.IsNew)
                return null;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new PositiveShortNumberMaxLengthValidationRule(cell.Value, 3),
                    new IdOndaNoExistenteValidationRule(_uow,cell.Value)
                }
            };
        }
        public virtual GridValidationGroup ValidateDescripcion(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var reglas = new List<IValidationRule>() {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 30)
                };

            if (cell.Old != cell.Value)
            {
                reglas.Add(new DescripcionOndaNoExistenteValidationRule(_uow, cell.Value));
            }

            return new GridValidationGroup
            {
                Rules = reglas,
            };

        }
        public virtual GridValidationGroup ValidatePredio(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(cell.Value))
                return null;

            return new GridValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                  //  new IdPredioExistenteValidationRule(this._uow, field.Value),
                    new PredioUsuarioExistenteValidationRule(this._uow, this._idUsuario , cell.Value)
                },
                OnSuccess = this.ValidatePredio_OnSuccess,
                OnFailure = this.ValidatePredio_OnFailure
            };

        }

        public virtual void ValidatePredio_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (row.GetCell("DS_PREDIO") != null)
            {
                Predio predio = _uow.PredioRepository.GetPredio(cell.Value);
                row.GetCell("DS_PREDIO").Value = predio.Descripcion;
            }
        }
        public virtual void ValidatePredio_OnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (row.GetCell("DS_PREDIO") != null)
            {
                row.GetCell("DS_PREDIO").Value = string.Empty;
            }
        }

    }
}
