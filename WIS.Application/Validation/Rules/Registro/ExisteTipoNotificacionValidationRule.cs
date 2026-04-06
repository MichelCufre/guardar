using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ExisteTipoNotificacionValidationRule : IValidationRule
    {
        protected readonly string _valueClase;
        protected readonly IUnitOfWork _uow;

        public ExisteTipoNotificacionValidationRule(IUnitOfWork uow, string tipoNotificacion)
        {
            this._valueClase = tipoNotificacion;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            List<DominioDetalle> tiposNotificaciones = this._uow.DominioRepository.GetDominios("TPNOTIF");

            if (!tiposNotificaciones.Any(d => d.Id == _valueClase))
                errors.Add(new ValidationError("General_Sec0_Error_NoExisteTipoNotificacion"));

            return errors;
        }
    }
}