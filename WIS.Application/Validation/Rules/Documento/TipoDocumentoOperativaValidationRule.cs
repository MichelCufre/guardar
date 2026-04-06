using System.Collections.Generic;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services.Interfaces;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Documento
{
    public class TipoDocumentoOperativaValidationRule : IValidationRule
    {
        protected readonly string _tpNuDocumento;
        protected readonly string _tpOperativa;
        protected readonly bool _isIngreso;
        protected readonly string _empresa;
        protected readonly IParameterService _parameterService;

        public TipoDocumentoOperativaValidationRule(string tpNuDocumento, string tpOperativa, bool isIngreso, string empresa, IParameterService parameterService)
        {
            this._tpNuDocumento = tpNuDocumento;
            this._tpOperativa = tpOperativa;
            this._isIngreso = isIngreso;
            this._empresa = empresa;
            this._parameterService = parameterService;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(this._tpNuDocumento) && !string.IsNullOrEmpty(this._tpOperativa) && !string.IsNullOrEmpty(this._empresa))
            {
                var paramTpDoc = TipoOperativaDocumental.GetParamTpDocIngreso(this._tpOperativa);
                var tpDocumento = this._tpNuDocumento.Substring(0, this._tpNuDocumento.IndexOf("_"));

                if (!this._isIngreso)
                    paramTpDoc = TipoOperativaDocumental.GetParamTpDocEgreso(this._tpOperativa);

                var tpDocOperativa = this._parameterService.GetValueByEmpresa(paramTpDoc, int.Parse(this._empresa));

                if (tpDocumento != tpDocOperativa)
                    errors.Add(new ValidationError("General_Sec0_Error_TipoDocumentoOperativa"));
            }

            return errors;
        }
    }
}
