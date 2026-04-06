using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Recorridos
{
    public class RecorridoRenombrableValidationRule : IValidationRule
    {
        protected readonly string _nombre;
        protected readonly int? _idRecorrido;
        protected readonly IUnitOfWork _uow;

        public RecorridoRenombrableValidationRule(IUnitOfWork uow, string nombre, int? idRecorrido)
        {
            this._nombre = nombre;
            this._idRecorrido = idRecorrido;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            if (_idRecorrido != null)
            {
                var recorrido = _uow.RecorridoRepository.GetRecorridoById(_idRecorrido??-1);

                if (recorrido.EsDefault)
                {
                    if (_nombre?.ToUpper()?.Trim() != recorrido.Nombre.ToUpper().Trim())
                    {
                        errors.Add(new ValidationError("REG700_Sec0_Error_RecorridoPorDefectoNoPuedeRenombrarse"));
                    }
                }
            }

            return errors;
        }
    }
}
