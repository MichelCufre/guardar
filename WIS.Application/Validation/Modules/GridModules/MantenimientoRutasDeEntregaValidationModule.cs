using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Liberacion;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoRutasDeEntregaValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;


        public MantenimientoRutasDeEntregaValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["CD_ROTA"] = this.ValidateCodigo,
                ["DS_ROTA"] = this.ValidateDescripcion,
                ["CD_ONDA"] = this.ValidateCodigoOnda,
                ["CD_TRANSPORTADORA"] = this.ValidateCodigoTransportadora,
                ["CD_PORTA"] = this.ValidateCodigoPuerta
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
                    new IdRutaNoExistenteValidationRule(_uow,cell.Value)
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
                reglas.Add(new DescripcionDeRutaNoExistenteValidationRule(_uow, cell.Value));
            }

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = reglas,
            };

        }

        public virtual GridValidationGroup ValidateCodigoOnda(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule> {
                new NonNullValidationRule(cell.Value),
                new PositiveShortNumberMaxLengthValidationRule(cell.Value, 3),
                new IdOndaExistenteValidationRule(_uow,cell.Value),

            };

            if (short.TryParse(row.GetCell("CD_PORTA").Value, out short idPuerta))
            {
                var nuPredio = _uow.PuertaEmbarqueRepository.GetPuertaEmbarque(idPuerta).NumPredio;

                rules.Add(new OndaPertenecePredioDePuertaValidationRule(_uow, cell.Value, nuPredio));
            }

            return new GridValidationGroup
            {
                Rules = rules,
                OnSuccess = this.ValidateCodigoOnda_OnSuccess,
                OnFailure = this.ValidateCodigoOnda_OnFailure
            };
        }
        public virtual void ValidateCodigoOnda_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var fieldDescripcionOnda = row.GetCell("DS_ONDA");

            Onda onda = _uow.OndaRepository.GetOnda(short.Parse(cell.Value));

            fieldDescripcionOnda.Value = onda.Descripcion;
        }
        public virtual void ValidateCodigoOnda_OnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("DS_ONDA").Value = string.Empty;
        }

        public virtual GridValidationGroup ValidateCodigoTransportadora(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new PositiveIntValidationRule(cell.Value),
                    new IdTransportistaExistenteValidationRule(_uow,cell.Value)
                },
                OnSuccess = this.ValidateCodigoTransportista_OnSuccess,
                OnFailure = this.ValidateCodigoTransportista_OnFailure
            };
        }
        public virtual void ValidateCodigoTransportista_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var fieldDescripcionTransportista = row.GetCell("DS_TRANSPORTADORA");

            Transportista transportista = _uow.TransportistaRepository.GetTransportista(int.Parse(cell.Value));

            fieldDescripcionTransportista.Value = transportista.Descripcion;
        }
        public virtual void ValidateCodigoTransportista_OnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("DS_TRANSPORTADORA").Value = string.Empty;
        }

        public virtual GridValidationGroup ValidateCodigoPuerta(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new PositiveShortNumberMaxLengthValidationRule(cell.Value, 3),
                    new IdPuertaExistenteValidationRule(_uow,cell.Value)
                },
                OnSuccess = this.ValidateCodigoPuerta_OnSuccess,
                OnFailure = this.ValidateCodigoPuerta_OnFailure
            };
        }
        public virtual void ValidateCodigoPuerta_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var opciones = new List<SelectOption>();

            var fieldDescripcionPuerta = row.GetCell("DS_PORTA");

            var puerta = _uow.PuertaEmbarqueRepository.GetPuertaEmbarque(short.Parse(cell.Value));

            fieldDescripcionPuerta.Value = puerta.Descripcion;
        }
        public virtual void ValidateCodigoPuerta_OnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("DS_PORTA").Value = string.Empty;
        }

    }
}
