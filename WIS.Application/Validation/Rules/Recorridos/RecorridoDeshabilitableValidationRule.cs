using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Recorridos
{
    public class RecorridoDeshabilitableValidationRule : IValidationRule
    {
        protected readonly string _esHabilitado;
        protected readonly int _idRecorrido;
        protected readonly IUnitOfWork _uow;

        public RecorridoDeshabilitableValidationRule(IUnitOfWork uow, string esHabilitado, int idRecorrido)
        {
            this._esHabilitado = esHabilitado;
            this._idRecorrido = idRecorrido;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            var esHabilitado = bool.Parse(_esHabilitado);
            var recorrido = _uow.RecorridoRepository.GetRecorridoById(_idRecorrido);

            if (!esHabilitado)
            {
                if (recorrido.EsDefault)
                {
                    errors.Add(new ValidationError("REG700_Sec0_Error_RecorridoPorDefectoNoPuedeDeshabilitarse"));
                }
                else if (_uow.RecorridoRepository.AnyPredeterminadoRecorridoAplicacion(_idRecorrido))
                {
                    errors.Add(new ValidationError("REG700_Sec0_Error_RecorridoAplicacionPredeterminadoNoPuedeDeshabilitarse"));
                }
                else if (_uow.RecorridoRepository.AnyPredeterminadoRecorridoAplicacionUsuario(_idRecorrido))
                {
                    errors.Add(new ValidationError("REG700_Sec0_Error_RecorridoAplicacionUsuarioPredeterminadoNoPuedeDeshabilitarse"));
                }
            }

            return errors;
        }
    }
}
