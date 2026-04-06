using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Preparacion
{
    public class PreparacionEmpresaValidationRule : IValidationRule
    {
        protected readonly string _preparacion;
        protected readonly string _empresa;
        protected readonly IUnitOfWork _uow;

        public PreparacionEmpresaValidationRule(string preparacion, string empresa, IUnitOfWork uow)
        {
            _preparacion = preparacion;
            _empresa = empresa;
            _uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(_preparacion) && !string.IsNullOrEmpty(_empresa))
            {
                if (int.Parse(_empresa) != (_uow.PreparacionRepository.GetPreparacionPorNumero(int.Parse(_preparacion)).Empresa ?? -1))
                    errors.Add(new ValidationError("General_Sec0_Error_PreparacionEmpresa"));
            }

            return errors;
        }
    }
}
