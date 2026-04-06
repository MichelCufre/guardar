using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Recepcion.Enums;
using WIS.Domain.Services.Interfaces;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Recepcion
{
    public class EtiquetaLoteEnUsoValidationRule : IValidationRule
    {
        protected readonly string _cdBarras;
        protected readonly bool _controlAgendaCerrada;
        protected readonly IUnitOfWork _uow;
        protected readonly IBarcodeService _barcodeService;

        public EtiquetaLoteEnUsoValidationRule(IUnitOfWork uow, IBarcodeService barcodeService, string cdBarras, bool controlAgendaCerrada = true)
        {
            this._cdBarras = cdBarras;
            this._controlAgendaCerrada = controlAgendaCerrada;
            this._uow = uow;
            this._barcodeService = barcodeService;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            var etiqueta = this._barcodeService.GetEtiquetaLote(this._cdBarras);

            if (etiqueta == null)
                errors.Add(new ValidationError("General_Sec0_Error_EtiquetaRecepcionInvalida"));
            else
            {
                etiqueta = this._uow.EtiquetaLoteRepository.GetEtiquetaLoteActiva(etiqueta.TipoEtiqueta, etiqueta.NumeroExterno);

                if (etiqueta == null)
                    errors.Add(new ValidationError("General_Sec0_Error_EtiquetaLoteEnDesuso"));
                else
                {
                    etiqueta = this._uow.EtiquetaLoteRepository.GetEtiquetaLoteEmpresaAsociadas(etiqueta.TipoEtiqueta, etiqueta.NumeroExterno);
                    if (etiqueta == null)
                    {
                        errors.Add(new ValidationError("REC410_msg_Error_NoTienesPermisosParaTrabajarConLaEmpresa"));
                    }
                    else
                    {

                        var agenda = this._uow.AgendaRepository.GetAgenda(etiqueta.NumeroAgenda);

                        if (_controlAgendaCerrada && agenda.Estado != EstadoAgenda.Cerrada)
                            errors.Add(new ValidationError("General_Sec0_Error_AgendaNoCerrada"));
                        else if (_uow.CrossDockingRepository.AnyCrossDocking(agenda.Id, TipoCrossDockingDb.SegundaFase, EstadoCrossDockingDb.Finalizado, true))
                            errors.Add(new ValidationError("General_Sec0_Error_EtiquetaAgendaCrossDockingActivo"));

                        if (etiqueta.UbicacionSugerida != null)
                            errors.Add(new ValidationError("REC410_msg_Error_EtiquetaConUbicacionSugerida"));

                        if (_uow.AlmacenamientoRepository.AnySugerenciaAlmacenajePendienteFraccionado(etiqueta.CodigoBarras))
                        {
                            errors.Add(new ValidationError("REC410_msg_Error_EtiqutaConProductoSugerencia"));
                        }
                    }
                }
            }
            return errors;
        }
    }
}