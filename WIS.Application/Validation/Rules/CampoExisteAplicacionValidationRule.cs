using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class CampoExisteAplicacionValidationRule : IValidationRule
    {
        protected readonly string _aplicacion;
        protected readonly string _campo;
        protected readonly IUnitOfWork _uow;

        public CampoExisteAplicacionValidationRule(string aplicacion, string campo, IUnitOfWork uow)
        {
            this._aplicacion = aplicacion;
            this._campo = campo;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(_campo) && !string.IsNullOrEmpty(_aplicacion))
            {
                if (!_uow.CodigoMultidatoRepository.ExisteCampoAplicacion(_campo, _aplicacion))
                    errors.Add(new ValidationError("REG100_Sec0_Error_CampoNoExiste"));
            }
                
            return errors;
        }
    }
}
