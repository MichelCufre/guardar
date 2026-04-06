using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;


namespace WIS.Application.Validation.Rules.Registro
{
    public class IdUbicacionTipoMonoProductoMonoLoteValidationRule : IValidationRule
    {
        protected readonly short _value;
        protected readonly string _idUbicacion;
        protected readonly IUnitOfWork _uow;

        public IdUbicacionTipoMonoProductoMonoLoteValidationRule(IUnitOfWork uow, short idUbicacionTipo, string idUbicacion)
        {
            this._value = idUbicacionTipo;
            this._idUbicacion = idUbicacion;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var tipoUbicacion = _uow.UbicacionTipoRepository.GetUbicacionTipo(_value);

            if (!tipoUbicacion.PermiteVariosProductos && _uow.StockRepository.MasDeUnProductoEnUbicacion(_idUbicacion))
                errors.Add(new ValidationError("General_Sec0_Error_IdUbicacionCambioMonoProductoInvalido"));

            if (!tipoUbicacion.PermiteVariosLotes && _uow.StockRepository.MasDeUnLoteEnUbicacion(_idUbicacion))
                errors.Add(new ValidationError("General_Sec0_Error_IdUbicacionCambioMonoLoteInvalido"));

            return errors;
        }
    }
}
