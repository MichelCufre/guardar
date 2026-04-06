using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Persistence.Database;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExcluirLpnAndSuelto : IValidationRule
    {
        protected readonly string _excluirLpn;
        protected readonly string _excluirSuelto;

        public ExcluirLpnAndSuelto(string excluirSuelto, string excluirLpn)
        {
            this._excluirLpn = excluirLpn;
            this._excluirSuelto = excluirSuelto;

        }

        public virtual List<IValidationError> Validate()
        {
            List<IValidationError> errors = new List<IValidationError>();

            if (_excluirLpn == "S" && _excluirSuelto == "S")
            {
                errors.Add(new ValidationError("INV410_Sec0_msg_NoSePuedeExcluirSueltoYLpn"));
            }

            return errors;
        }
    }
}
