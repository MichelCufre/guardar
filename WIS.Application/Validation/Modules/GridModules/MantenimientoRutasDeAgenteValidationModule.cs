using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoRutasDeAgenteValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _idUsuario;

        public MantenimientoRutasDeAgenteValidationModule(IUnitOfWork uow, int idUsuario)
        {
            this._uow = uow;
            this._idUsuario = idUsuario;

            this.Schema = new GridValidationSchema
            {
                ["NU_PREDIO"] = this.ValidatePredio,
                ["CD_ROTA"] = this.ValidateRuta,
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
                    new PredioUsuarioExistenteValidationRule(this._uow, this._idUsuario , cell.Value)
                },
                OnSuccess = this.ValidatePredio_OnSuccess,
                OnFailure = this.ValidatePredio_OnFailure
            };

        }

        public virtual void ValidatePredio_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            // row.GetCell("CD_ROTA").Column.UpdateSpecificValues(new GridColumnSelect("CD_ROTA", this.OptionSelectRuta(cell.Value)));

            Predio predio = _uow.PredioRepository.GetPredio(cell.Value);
            row.GetCell("DS_PREDIO").Value = predio.Descripcion;
        }

        public virtual void ValidatePredio_OnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("DS_PREDIO").Value = string.Empty;
        }

        public virtual GridValidationGroup ValidateRuta(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(cell.Value))
                return null;

            var predio = row.GetCell("NU_PREDIO").Value;
            if (string.IsNullOrEmpty(predio))
            {
                row.GetCell("NU_PREDIO").SetError("General_Sec0_Error_Error25", new List<string>());
            }


            var rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new PositiveShortNumberMaxLengthValidationRule(cell.Value,3),
                    new ExisteRutaValidationRule(_uow, cell.Value, this._idUsuario, predio),

                };

            if (row.IsNew)
            {
                rules.Add(new ExisteClienteRutaPredioValidationRule(_uow, parameters.FirstOrDefault(s => s.Id == "keyEmpresa").Value, parameters.FirstOrDefault(s => s.Id == "keyCodigo").Value, predio));
            }
            else
            {
                rules.Add(new ExisteClienteRutaPredioValidationRule(_uow, parameters.FirstOrDefault(s => s.Id == "keyEmpresa").Value, parameters.FirstOrDefault(s => s.Id == "keyCodigo").Value, predio, cell.Old));
            }


            return new GridValidationGroup
            {
                BreakValidationChain = false,
                Rules = rules,
                OnSuccess = this.ValidateRuta_OnSuccess,
                OnFailure = this.ValidateRuta_OnFailure,
                Dependencies = { "NU_PREDIO" }
            };
        }

        public virtual void ValidateRuta_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            Ruta ruta = _uow.RutaRepository.GetRuta(short.Parse(cell.Value));
            row.GetCell("DS_ROTA").Value = ruta.Descripcion;
        }

        public virtual void ValidateRuta_OnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("DS_ROTA").Value = string.Empty;
        }
    }
}
