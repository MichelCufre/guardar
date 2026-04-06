using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.Services.Interfaces;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Recepcion
{
    public class EtiquetaLotePredioClasificacionValidationRule : IValidationRule
    {
        protected readonly string _cdBarras;
        protected readonly int _cdEstacion;
        protected readonly IUnitOfWork _uow;
        protected readonly IBarcodeService _barcodeService;

        public EtiquetaLotePredioClasificacionValidationRule(IUnitOfWork uow,
            IBarcodeService barcodeService,
            string cdBarras,
            int cdEstacion)
        {
            this._cdEstacion = cdEstacion;
            this._cdBarras = cdBarras;
            this._uow = uow;
            this._barcodeService = barcodeService;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var etiqueta = this._barcodeService.GetEtiquetaLote(this._cdBarras);
            var estacion = this._uow.MesaDeClasificacionRepository.GetEstacionDeClasificacion(this._cdEstacion);

            etiqueta = this._uow.EtiquetaLoteRepository.GetEtiquetaLoteActiva(etiqueta.TipoEtiqueta, etiqueta.NumeroExterno);
            
            var ubicacion = this._uow.UbicacionRepository.GetUbicacion(etiqueta.IdUbicacion);

            if (ubicacion.NumeroPredio != estacion.Predio)
                errors.Add(new ValidationError("General_Sec0_Error_ClasificacionPredioEtiquetaLote"));
            
            return errors;
        }
    }
}