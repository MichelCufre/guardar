using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Evento;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class EVT030GridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        public EVT030GridValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["NM_GRUPO"] = this.ValidateNombreGrupo,
            };
        }

        public virtual GridValidationGroup ValidateNombreGrupo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 100),
                    new NombreGrupoExistenteValidationRule(this._uow, cell.Value)
                },
            };
        }
    }
}
