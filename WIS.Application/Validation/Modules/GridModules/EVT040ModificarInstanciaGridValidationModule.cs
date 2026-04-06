using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Session;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class EVT040ModificarInstanciaGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly ISessionAccessor _sessionAccessor;
        protected readonly Grid _grid;
        protected readonly string _empresa;
        protected readonly string _tipoAgente;

        public EVT040ModificarInstanciaGridValidationModule(IUnitOfWork uow, string empresa, string tipoAgente, ISessionAccessor sessionAccessor)
        {
            this._uow = uow;
            this._empresa = empresa;
            this._tipoAgente = tipoAgente;
            this._sessionAccessor = sessionAccessor;

            this.Schema = new GridValidationSchema
            {
                ["VL_PARAMETRO"] = this.GridValidateVlParametro,
            };
        }

        public virtual GridValidationGroup GridValidateVlParametro(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            List<IValidationRule> rules = new List<IValidationRule>();
            var isRequired = row.GetCell("FL_REQUERIDO").Value == "S";
            if (isRequired)
                rules.Add(new NonNullValidationRule(cell.Value));

            rules.Add(new StringMaxLengthValidationRule(cell.Value, 100));

            string tipo = row.GetCell("TP_PARAMETRO").Value;
            string expresion = row.GetCell("VL_EXPRESION_REGULAR").Value;

            switch (tipo)
            {
                case EventoParametroTipoDb.DB:
                    this.ValidarParametrosEnBase(cell.Value, row.GetCell("CD_EVENTO_PARAMETRO").Value, rules);
                    break;
                case EventoParametroTipoDb.NUMERO:
                    rules.Add(new PositiveIntValidationRule(cell.Value, true));
                    break;
            }

            if (!string.IsNullOrEmpty(expresion))
            {
                rules.Add(new CumpleExpresionRegularRule(cell.Value, expresion));
            }

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual void ValidarParametrosEnBase(string value, string parametro, List<IValidationRule> rules)
        {
            if (!string.IsNullOrEmpty(value))
            {
                switch (parametro)
                {
                    case EventoParametroDb.CD_EMPRESA:
                        rules.Add(new PositiveNumberMaxLengthValidationRule(value, 10));
                        rules.Add(new ExisteEmpresaValidationRule(this._uow, value));
                        break;
                    case EventoParametroDb.NU_PREDIO:
                        rules.Add(new StringMaxLengthValidationRule(value, 10));
                        rules.Add(new ExistePredioValidationRule(this._uow, 0, null, value));
                        break;
                    case EventoParametroDb.TP_AGENTE:
                        rules.Add(new StringMaxLengthValidationRule(value, 10));
                        rules.Add(new ExisteTipoAgenteValidationRule(this._uow, value));
                        break;
                    case EventoParametroDb.CD_AGENTE:
                        rules.Add(new StringMaxLengthValidationRule(value, 40));
                        rules.Add(new ExisteCodigoAgenteValidationRule(this._uow, value, _tipoAgente, _empresa));
                        break;
                }
            }
        }
    }
}
