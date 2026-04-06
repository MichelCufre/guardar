using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Recepcion
{
    public class EtiquetaLoteEnEstacionClasificacionValidationRule : IValidationRule
    {
        protected readonly int _nuEtiquetaLote;
        protected readonly int _cdEstacion;
        protected readonly IUnitOfWork _uow;

        public EtiquetaLoteEnEstacionClasificacionValidationRule(IUnitOfWork uow,
            int nuEtiquetaLote,
            int cdEstacion)
        {
            this._cdEstacion = cdEstacion;
            this._nuEtiquetaLote = nuEtiquetaLote;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var etiqueta = this._uow.EtiquetaLoteRepository.GetEtiquetaLote(this._nuEtiquetaLote);
            var estacion = this._uow.MesaDeClasificacionRepository.GetEstacionDeClasificacion(this._cdEstacion);

            if (etiqueta.IdUbicacion != estacion.Ubicacion)
                errors.Add(new ValidationError("General_Sec0_Error_ClasificacionEtiquetaLoteReubicada"));

            return errors;
        }
    }
}