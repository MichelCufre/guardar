using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class EmpresaPoseeOndasValidationRule : IValidationRule
    {
        protected readonly string _valueEmpresa;
        protected readonly IUnitOfWork _uow;

        public EmpresaPoseeOndasValidationRule(IUnitOfWork uow, string valueEmpresa)
        {
            this._valueEmpresa = valueEmpresa;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            if (int.TryParse(this._valueEmpresa, out int cdEmpresa))
            {
                if (!_uow.LiberacionRepository.AnyOnda(cdEmpresa))
                    errors.Add(new ValidationError(string.Format("General_Sec0_Error_NoHayPedidosPendientesEmpresa"), new List<String> { this._valueEmpresa }));
            }
            else
            {
                errors.Add(new ValidationError("General_Sec0_Error_EmpresaNecesaria"));
            }
            return errors;
        }
    }
}
