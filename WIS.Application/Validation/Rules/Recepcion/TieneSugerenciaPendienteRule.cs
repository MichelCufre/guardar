using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.Recepcion;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Recepcion
{
    public class TieneSugerenciaPendienteRule : IValidationRule
    {
        protected readonly EtiquetaLote _etiquetaLote;
        protected readonly IUnitOfWork _uow;

        public TieneSugerenciaPendienteRule(EtiquetaLote etiqueta, IUnitOfWork uow)
        {
            this._etiquetaLote = etiqueta;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();


            if (!string.IsNullOrEmpty(_etiquetaLote.IdUbicacionSugerida))
                errors.Add(new ValidationError("General_msg_Error_EtiquetaConUbicacionSugerida"));
            else if (_uow.AlmacenamientoRepository.AnySugerenciaAlmacenajePendienteFraccionado(_etiquetaLote.CodigoBarras))
                errors.Add(new ValidationError("General_msg_Error_EtiqutaConProductoSugerencia"));
            else if (_uow.AlmacenamientoRepository.AnySugerenciaAlmacenajePendienteClasificacion(_etiquetaLote.CodigoBarras))
                errors.Add(new ValidationError("General_msg_Error_EtiquetaConSugerenciaClasificacion"));
            else if (_uow.AlmacenamientoRepository.AnySugerenciaAlmacenajeReabastecimintoPendienteClasificacion(_etiquetaLote.Numero))
                errors.Add(new ValidationError("General_msg_Error_EtiquetaConSugerenciaClasificacion"));

            return errors;
        }
    }
}
