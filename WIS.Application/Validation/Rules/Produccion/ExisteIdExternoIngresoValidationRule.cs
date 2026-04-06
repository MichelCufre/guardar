using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Produccion
{
    public class ExisteIdExternoIngresoValidationRule : IValidationRule
    {
        protected readonly string _idExterno;
        protected readonly string _empresa;
        protected readonly IUnitOfWork _uow;

        public ExisteIdExternoIngresoValidationRule(IUnitOfWork uow, string idExterno, string empresa)
        {
            this._uow = uow;
            this._idExterno = idExterno;
            this._empresa = empresa;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            var empresa = int.TryParse(_empresa, out int e) ? e : -1;

            if (_uow.ProduccionRepository.AnyIngresoByIdExternoEmpresa(_idExterno, empresa))
                errors.Add(new ValidationError("General_Sec0_Error_IdExternoIngresoExiste", new List<string> { _idExterno, _empresa }));

            return errors;
        }
    }
}
