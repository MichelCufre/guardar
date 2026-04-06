using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class OndaPertenecePredioDePuertaValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _codigoOnda;
        protected readonly string _nuPredio;

        public OndaPertenecePredioDePuertaValidationRule(IUnitOfWork uow, string codigoOnda, string predio)
        {
            this._uow = uow;
            this._codigoOnda = codigoOnda;
            this._nuPredio = predio;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var onda = _uow.OndaRepository.GetOnda(short.TryParse(_codigoOnda, out short cdOnda) ? cdOnda : ((short)-1));

            if (onda.Predio != _nuPredio)
                errors.Add(new ValidationError("General_Sec0_Error_PredioOndaDistintoPredioPuerta"));

            return errors;
        }
    }
}
