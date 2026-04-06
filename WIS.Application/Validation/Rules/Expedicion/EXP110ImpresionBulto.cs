using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.General.Auxiliares;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Expedicion
{
    public class EXP110ImpresionBulto : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _codigoBarras;
        protected readonly IIdentityService _identity;
        protected readonly IBarcodeService _barcodeService;

        public EXP110ImpresionBulto(IUnitOfWork uow, string codigoBarras, IIdentityService identity, IBarcodeService barcodeService)
        {
            _uow = uow;
            _codigoBarras = codigoBarras;
            _identity = identity;
            _barcodeService = barcodeService;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            try
            {
                if (_codigoBarras.Length < 5)
                    throw new ValidationFailedException("EXP110_form1_Error_ContenedorNoExiste");

                _barcodeService.ValidarEtiquetaContenedor(this._codigoBarras, _identity.UserId, out AuxContenedor datosContenedor, out int cantidadEmpresa);

                if (datosContenedor == null || datosContenedor?.NuPreparacion == -1)
                    throw new ValidationFailedException("EXP110_form1_Error_ContenedorNoExiste");

                WIS.Domain.Picking.Preparacion preparacion = _uow.PreparacionRepository.GetPreparacionPorNumero(datosContenedor.NuPreparacion);

                if (preparacion.Predio != _identity.Predio)
                    throw new ValidationFailedException("EXP110_form1_Error_ContenedorNoPerteneceAlPredioLogueado");

                var datosContenedorBulto = _uow.EmpaquetadoPickingRepository.GetDatosContenedorBulto(datosContenedor.NuPreparacion, datosContenedor.NuContenedor);

                if (datosContenedorBulto == null)
                    throw new ValidationFailedException("EXP110ImpresionBulto_form1_Error_SituacionContenedor");

            }
            catch (ValidationFailedException ex)
            {
                //throw ex;
                errors.Add(new ValidationError(ex.Message, ex.StrArguments?.ToList()));
            }
            catch (Exception ex)
            {
                errors.Add(new ValidationError(ex.Message));
            }

            return errors;
        }
    }
}
