using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Impresion
{
    public class EstiloImpresionUbicacionValidationRule : IValidationRule
    {
        protected readonly string _estilo;
        protected readonly List<Ubicacion> _ubicaciones;
        protected readonly IUnitOfWork _uow;

        public EstiloImpresionUbicacionValidationRule(IUnitOfWork uow, string estilo, List<Ubicacion> ubicaciones)
        {
            this._estilo = estilo;
            this._ubicaciones = ubicaciones;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(this._estilo) && this._ubicaciones.Any())
            {
                Ubicacion ubicacion = null;
                switch (_estilo)
                {
                    case EstiloEtiquetaUbicacion.Estandar:

                        var areasNoPermitidas = GetAreasNoPermitidasEstiloEstandar();
                        ubicacion = _ubicaciones.FirstOrDefault(u => areasNoPermitidas.Contains(u.IdUbicacionArea));

                        if (ubicacion != null)
                            errors.Add(new ValidationError("IMP050_Sec0_error_EstiloUbicacion", new List<string> { ubicacion.Id, this._estilo }));

                        break;
                    case EstiloEtiquetaUbicacion.Puerta:

                        ubicacion = _ubicaciones.FirstOrDefault(u => u.IdUbicacionArea != AreaUbicacionDb.PuertaEmbarque);
                        if (ubicacion != null)
                            errors.Add(new ValidationError("IMP050_Sec0_error_EstiloPuerta", new List<string> { ubicacion.Id, this._estilo }));

                        break;
                    case EstiloEtiquetaUbicacion.Produccion:

                        ubicacion = _ubicaciones.FirstOrDefault(u=> u.IdUbicacionArea != AreaUbicacionDb.BlackBox && u.IdUbicacionArea != AreaUbicacionDb.ProduccionSalida);
                        if (ubicacion != null)
                            errors.Add(new ValidationError("IMP050_Sec0_error_EstiloProduccion", new List<string> { ubicacion.Id, this._estilo }));

                        break;
                    case EstiloEtiquetaUbicacion.Automatismo:

                        var areasAutomatismo= AreaUbicacionDb.GetAreasAutomatismo();
                        ubicacion = _ubicaciones.FirstOrDefault(u => !areasAutomatismo.Contains(u.IdUbicacionArea));

                        if (ubicacion != null)
                            errors.Add(new ValidationError("IMP050_Sec0_error_EstiloAutomatismo", new List<string> { ubicacion.Id, this._estilo }));

                        break;
                }
            }

            return errors;
        }

        public virtual List<short> GetAreasNoPermitidasEstiloEstandar()
        {
            var areasNoPermitidas = new List<short>() {
                AreaUbicacionDb.BlackBox,
                AreaUbicacionDb.ProduccionSalida,
                AreaUbicacionDb.PuertaEmbarque
            };

            areasNoPermitidas.AddRange(AreaUbicacionDb.GetAreasAutomatismo());

            return areasNoPermitidas;
        }
    }
}
