using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Produccion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    class REG100CodigosMultidatoGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        public REG100CodigosMultidatoGridValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["CD_APLICACION"] = this.ValidateAplicacion,
                ["CD_CAMPO"] = this.ValidateCampo,
                ["CD_AI"] = this.ValidateCodigoAi,
            };
        }

        public virtual GridValidationGroup ValidateAplicacion(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                },
                OnSuccess = this.ValidateAplicacion_OnSucess,
            };
        }

        public virtual void ValidateAplicacion_OnSucess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("DS_APLICACION").Value = _uow.CodigoMultidatoRepository.GetAplicacion(cell.Value).Descripcion;
        }

        public virtual GridValidationGroup ValidateCampo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var aplicacion = row.GetCell("CD_APLICACION")?.Value;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new CampoExisteAplicacionValidationRule(aplicacion, cell.Value, _uow),
                },
                OnSuccess = this.ValidateCampo_OnSucess,
            };
        }

        public virtual void ValidateCampo_OnSucess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var aplicacion = row.GetCell("CD_APLICACION").Value;

            if (string.IsNullOrEmpty(aplicacion))
                return;

            row.GetCell("DS_CAMPO").Value = _uow.CodigoMultidatoRepository.GetCampo(aplicacion, cell.Value).Descripcion;
        }

        public virtual GridValidationGroup ValidateCodigoAi(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var empresa = int.Parse(parameters.FirstOrDefault(x => x.Id == "empresa").Value);
            var codigoMultidato = parameters.FirstOrDefault(x => x.Id == "codigoMultidato").Value;
            var codigoAplicacion = row.GetCell("CD_APLICACION").Value;
            var codigoCampo = row.GetCell("CD_CAMPO").Value;

            var validationGroup = new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                },
                OnSuccess = this.ValidateCodigoAi_OnSucess,
            };

            if(!string.IsNullOrEmpty(codigoAplicacion) && !string.IsNullOrEmpty(codigoCampo))
                validationGroup.Rules.Add(new ExisteDetalleCodigoMultidatoEmpresaValidationRule(_uow, empresa, codigoMultidato, codigoAplicacion, codigoCampo, cell.Value));

            return validationGroup;
        }

        public virtual void ValidateCodigoAi_OnSucess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var codigoMultidato = parameters.FirstOrDefault(x => x.Id == "codigoMultidato").Value;

            if (string.IsNullOrEmpty(codigoMultidato))
                return;

            row.GetCell("DS_AI").Value = _uow.CodigoMultidatoRepository.GetCodigoAi(codigoMultidato, cell.Value).Descripcion;
        }
    }
}