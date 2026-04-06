using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Expedicion
{
    public class RutaNoAsociadaValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _codigoRuta;
        protected readonly string _predio;

        public RutaNoAsociadaValidationRule(IUnitOfWork uow, string codigoRuta, string predio)
        {
            this._uow = uow;
            this._codigoRuta = codigoRuta;
            this._predio = predio;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!this._uow.CamionRepository.AnyCamionConRutaAsociada(short.Parse(this._codigoRuta)))
                errors.Add(new ValidationError("WEXP040_Sec0_Error_Er009_RutaYaAsociada"));

            return errors;
        }
    }
}
