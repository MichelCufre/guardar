using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Expedicion
{
    public class CargasEmpresaCamionValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _empresaId;
        protected readonly int _camion;

        public CargasEmpresaCamionValidationRule(IUnitOfWork uow, int camion, string empresaId)
        {
            this._uow = uow;
            this._empresaId = empresaId;
            this._camion = camion;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var empresa = int.Parse(this._empresaId);

            if(this._uow.CamionRepository.AnyCargaOtraEmpresa(this._camion, empresa))
                errors.Add(new ValidationError("WEXP040_Sec0_Error_Er010_NoCambiarEmpresa"));

            return errors;
        }
    }
}