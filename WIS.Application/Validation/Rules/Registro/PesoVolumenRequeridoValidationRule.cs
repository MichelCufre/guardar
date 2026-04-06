using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class PesoVolumenRequeridoValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly string _empresa;
        protected readonly bool _modo;
        protected readonly IUnitOfWork _uow;

        public PesoVolumenRequeridoValidationRule(IUnitOfWork uow, string value, string empresa, bool modo = false)
        {
            this._uow = uow;
            this._value = value;
            this._empresa = empresa;
            this._modo = modo;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            string parametro = string.Empty;

            if (!string.IsNullOrEmpty(this._empresa))
            {
                Dictionary<string, string> colParams = new Dictionary<string, string>();
                colParams[ParamManager.PARAM_EMPR] = string.Format("{0}_{1}", ParamManager.PARAM_EMPR, this._empresa);

                if (this._modo)
                    parametro = "CADASTRO_VOLUMEN";
                else
                    parametro = "CADASTRO_PESO";

                string paramValue = this._uow.ParametroRepository.GetParameter(parametro, colParams);
                if (!string.IsNullOrEmpty(paramValue) && paramValue == "S")
                {
                    if (string.IsNullOrEmpty(this._value))
                        errors.Add(new ValidationError("General_Sec0_Error_Error76"));
                }
            }
            return errors;
        }
    }
}
