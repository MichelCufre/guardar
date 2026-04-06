using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services.Interfaces;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Recepcion
{
    public class EtiquetaPosicionEquipoValidationRule : IValidationRule
    {
        protected readonly string _cdBarras;
        protected readonly string _destino;
        protected readonly string _zona;
        protected readonly int _estacion;
        protected readonly IUnitOfWork _uow;
        protected readonly IBarcodeService _barcodeService;

        public EtiquetaPosicionEquipoValidationRule(IUnitOfWork uow, IBarcodeService barcodeService, string cdBarras, string destino, string zona, int estacion)
        {
            this._cdBarras = cdBarras;
            this._uow = uow;
            this._barcodeService = barcodeService;
            this._destino = destino;
            this._zona = zona;
            this._estacion = estacion;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            var etiqueta = this._barcodeService.GetEtiquetaPosicionEquipo(this._cdBarras);
            var estacion = this._uow.MesaDeClasificacionRepository.GetEstacionDeClasificacion(this._estacion);

            if (etiqueta != null)
            {
                var equipo = this._uow.EquipoRepository.GetEquipo(etiqueta.Equipo);

                if (equipo == null)
                {
                    errors.Add(new ValidationError("General_Sec0_Error_EquipoNoExiste"));
                }
                else if (equipo.Ubicacion != null && estacion.Predio != equipo.Ubicacion.NumeroPredio)
                {
                    errors.Add(new ValidationError("General_Sec0_Error_ClasificacionDistintoPredio"));
                }
                else if (!string.IsNullOrEmpty(equipo.TipoOperacion) && TipoOperacionDb.Clasificacion != equipo.TipoOperacion)
                {
                    errors.Add(new ValidationError("General_Sec0_Error_ErrorEquipoVinculadoAOtraOperativa"));
                }
                else
                {
                    var etiquetaTransferencia = _uow.EtiquetaTransferenciaRepository.GetEtiquetaTransferencia(etiqueta.Tipo, etiqueta.NumeroExterno);
                    if (!string.IsNullOrEmpty(_destino)
                        && etiquetaTransferencia != null
                        && !string.IsNullOrEmpty(etiquetaTransferencia.UbicacionDestino)
                        && _destino != etiquetaTransferencia.UbicacionDestino)
                    {
                        errors.Add(new ValidationError("REC410_msg_Error_EtiqPosicionDestinoDistinto"));
                    }
                    else if (etiquetaTransferencia != null && etiquetaTransferencia.NroLpn != null)
                        errors.Add(new ValidationError("REC410_Sec0_Error_EtiquetaAsociadoLpn"));
                    else
                    {
                        var sugerencias = this._uow.MesaDeClasificacionRepository.GetSugerenciasDeClasificacion(estacion, _destino, _zona);

                        if (!sugerencias.Any(s => s.Equipo == etiqueta.Equipo))
                            errors.Add(new ValidationError("General_Sec0_Error_ClasificacionNoValida"));
                    }
                }
            }
            else
            {
                errors.Add(new ValidationError("General_Sec0_Error_EtiquetaPosicionEquipoInvalida"));
            }

            return errors;
        }
    }
}