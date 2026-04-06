using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Evento
{
    public class NombreGrupoExistenteValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _nmGrupo;
        protected readonly int _nuGrupo;
        public NombreGrupoExistenteValidationRule(IUnitOfWork uow, string nmGrupo, int nuGrupo = 0)
        {
            this._uow = uow;
            this._nmGrupo = nmGrupo;
            this._nuGrupo = nuGrupo;
        }

        public virtual List<IValidationError> Validate()
        {

            var errors = new List<IValidationError>();

            if (_uow.DestinatarioRepository.AnyNombreGrupo(_nmGrupo, _nuGrupo))
                errors.Add(new ValidationError("General_Db_Error_NombreGrupoContactosYaExiste"));


            return errors;
        }
    }
}
