using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Recepcion;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Recepcion
{
    public class RecepcionTipoExternoExistenteValidationRule : IValidationRule
    {
        protected readonly int _idEmpresa;
        protected readonly string _tipoAgente;
        protected readonly IUnitOfWork _uow;

        public RecepcionTipoExternoExistenteValidationRule(IUnitOfWork uow, string idEmpresa, string tipoAgente)
        {
            this._idEmpresa = int.Parse(idEmpresa);
            this._tipoAgente = tipoAgente;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!this._uow.RecepcionTipoRepository.AnyRecepcionTipoEmpresaHabilitado(this._idEmpresa, this._tipoAgente))
                errors.Add(new ValidationError("General_Sec0_Error_NoExisteRecepcionTipoExterno"));

            return errors;
        }
    }
}