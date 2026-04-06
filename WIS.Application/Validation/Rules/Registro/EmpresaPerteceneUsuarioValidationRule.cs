using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class EmpresaPerteceneUsuarioValidationRule : IValidationRule
    {
        protected readonly List<int> _empresas;
        protected readonly int _idEmpresaParaChequear;
        protected readonly IUnitOfWork _uow;
        protected readonly int _userId;

        public EmpresaPerteceneUsuarioValidationRule(IUnitOfWork uow, List<int> empresasUsuario, int empresaParaChequear)
        {
            this._empresas = empresasUsuario;
            this._idEmpresaParaChequear = empresaParaChequear;
            this._uow = uow;
        }

        public EmpresaPerteceneUsuarioValidationRule(IUnitOfWork uow, int userId, int empresaParaChequear)
        {
            this._userId = userId;
            this._idEmpresaParaChequear = empresaParaChequear;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            var empresasUsuario = _empresas;

            if (empresasUsuario == null)
                empresasUsuario = _uow.EmpresaRepository.GetEmpresasAsignadasUsuario(_userId);

            if (!empresasUsuario.Contains(_idEmpresaParaChequear))
                errors.Add(new ValidationError("General_Sec0_Error_EmpresaNoAsociadaAlUsuario"));

            return errors;
        }
    }
}
