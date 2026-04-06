using System;
using System.Collections.Generic;
using WIS.Domain.Automatismo.Logic;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Security;
using WIS.Session;
using WIS.TrafficOfficer;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Ptl
{
    public class PTL010NotificarPTLModalContenedorValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly ISessionAccessor _session;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IBarcodeService _barcodeService;
        protected readonly string _codigoBarras;
        protected readonly int _preparacion;
        protected readonly PtlLogic _ptlLogic;

        public PTL010NotificarPTLModalContenedorValidationRule(IUnitOfWork uow, IIdentityService identity, ISessionAccessor session, ITrafficOfficerService concurrencyControl, string codigoBarras, int preparacion, IBarcodeService barcodeService)
        {
            _uow = uow;
            _identity = identity;
            _session = session;
            _concurrencyControl = concurrencyControl;
            _codigoBarras = codigoBarras;
            _preparacion = preparacion;
            _ptlLogic = new PtlLogic(barcodeService, identity);
            _barcodeService = barcodeService;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            try
            {
                if (string.IsNullOrEmpty(_codigoBarras))
                {
                    errors.Add(new ValidationError("PTL010_form1_Error_DebeIngresarUnContenedor"));
                    return errors;
                }

                if (_codigoBarras.Length < 5)
                {
                    errors.Add(new ValidationError("PTL010_form1_Error_LargoIncorrectoEtiquetaContenedor"));
                    return errors;
                }

                (bool existeContenedor, Contenedor contenedor) = _ptlLogic.ExisteContenedor(_uow, _codigoBarras);

                if (!existeContenedor)
                    return errors;

                if (contenedor.IsFacturado())
                {
                    errors.Add(new ValidationError("PTL010_form1_Error_ContenedorFacturado"));
                    return errors;
                }

                if (contenedor.Estado != EstadoContenedor.EnPreparacion)
                {
                    errors.Add(new ValidationError("PTL010_form1_Error_ContenedorNoEstaEnPreparacion"));
                    return errors;
                }

                if (_preparacion != contenedor.NumeroPreparacion)
                {
                    errors.Add(new ValidationError("PTL010_form1_Error_ElContenedorLeidoPerteneceAOtraPreparacion"));
                    return errors;
                }

                var preparacion = _uow.PreparacionRepository.GetPreparacionPorNumero(contenedor.NumeroPreparacion);

                if (preparacion.Predio != _identity.Predio)
                {
                    errors.Add(new ValidationError("PTL010_form1_Error_ContenedorNoPerteneceAlPredioLogueado"));
                    return errors;
                }

                if (_ptlLogic.TieneMasDeUnConjuntoDeAgrupacionContenedor(_uow, contenedor.NumeroPreparacion, contenedor.Numero))
                {
                    errors.Add(new ValidationError("PTL010_form1_Error_ContenedorTieneMasDeUnaAgrupacion"));
                    return errors;
                }

            }
            catch (ValidationFailedException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                errors.Add(new ValidationError(ex.Message));
            }

            return errors;
        }
    }
}
