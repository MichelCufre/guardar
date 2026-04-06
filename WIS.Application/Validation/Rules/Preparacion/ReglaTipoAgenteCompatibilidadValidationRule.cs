using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Preparacion
{
    public class ReglaTipoAgenteCompatibilidadValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _nuRegla;
        protected readonly string _tpAgente;

        public ReglaTipoAgenteCompatibilidadValidationRule(IUnitOfWork uow, int nuRegla, string tpAgente)
        {
            this._uow = uow;
            _nuRegla = nuRegla;
            _tpAgente = tpAgente;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var regla = _uow.LiberacionRepository.GetReglaLiberacion(_nuRegla);
            if (regla.TpAgente != _tpAgente && !string.IsNullOrEmpty(_tpAgente))
            {
                string incompatible = string.Empty;

                if (_tpAgente == "PRO")
                    incompatible = "CLI";
                else
                    incompatible = "PRO";

                if (_uow.LiberacionRepository.TipoAgenteCompatible(_nuRegla, incompatible))
                    errors.Add(new ValidationError("PRE250_Sec0_Error_tpAgenteIncompatible"));
            }

            return errors;
        }
    }
}
