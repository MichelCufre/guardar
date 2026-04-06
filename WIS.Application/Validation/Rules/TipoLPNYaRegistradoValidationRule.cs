using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class TipoLPNYaRegistradoValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;
        public TipoLPNYaRegistradoValidationRule(string value, IUnitOfWork uow)
        {
            this._value = value;
            this._uow = uow;
        }


        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.ManejoLpnRepository.AnyTipoLpn(this._value))
                errors.Add(new ValidationError("PAR401_Sec0_Error_Er021_ExisteEnSistema"));
            else if (_uow.ContenedorRepository.ExisteTipoContenedor(this._value))
                errors.Add(new ValidationError("PAR401_Sec0_Error_ExisteTipoContenedorTipoLpn"));

            return errors;
        }
    }
}
