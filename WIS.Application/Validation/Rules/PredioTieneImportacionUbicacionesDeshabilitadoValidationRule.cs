using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class PredioTieneImportacionUbicacionesDeshabilitadoValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _value;

        public PredioTieneImportacionUbicacionesDeshabilitadoValidationRule(IUnitOfWork uow, string value)
        {
            _uow = uow;
            _value = value;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var colParams = new Dictionary<string, string>
            {
                [ParamManager.PARAM_PRED] = $"{ParamManager.PARAM_PRED}_{_value}"
            };

            var predioHabilitaImportUbic = _uow.ParametroRepository.GetParameter(ParamManager.REG040_PERMITE_IMPORT_UBIC, colParams)?.ToUpper() == "S";

            if (predioHabilitaImportUbic) errors.Add(new ValidationError("REG040_Sec0_Error_PredioTieneImportarUbicacionesHabilitado"));

            return errors;
        }
    }
}
