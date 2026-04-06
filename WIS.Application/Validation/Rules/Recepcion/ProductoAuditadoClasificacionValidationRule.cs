using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.Services.Interfaces;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Recepcion
{
    public class ProductoAuditadoClasificacionValidationRule : IValidationRule
    {
        protected readonly int _nuEtiquetaLote;
        protected readonly int _agenda;
        protected readonly string _cdBarras;
        protected readonly IUnitOfWork _uow;
        protected readonly IParameterService _parameterService;

        public ProductoAuditadoClasificacionValidationRule(IUnitOfWork uow,
            IParameterService parameterService,
            int nuEtiquetaLote,
            int agenda,
            string cdBarras)
        {
            this._nuEtiquetaLote = nuEtiquetaLote;
            this._uow = uow;
            this._parameterService = parameterService;
            this._agenda = agenda;
            this._cdBarras = cdBarras;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            var agenda = this._uow.AgendaRepository.GetAgenda(this._agenda);
            var clasificarProductosNoEsperados = this._parameterService.GetValueByEmpresa("CLASIFICAR_PRODS_NO_ESPERADOS", agenda.IdEmpresa);

            if (clasificarProductosNoEsperados?.ToUpper() == "N")
            {
                var detalles = this._uow.EtiquetaLoteRepository.GetDetalles(this._nuEtiquetaLote);
                var codigoProducto = this._uow.CodigoBarrasRepository.GetCodigoBarras(this._cdBarras, agenda.IdEmpresa)?.Producto;
                var cantidadProductos = detalles
                    .Where(d => d.IdEmpresa == agenda.IdEmpresa
                        && d.CodigoProducto == codigoProducto)
                    .Count();

                if (cantidadProductos == 0)
                {
                    errors.Add(new ValidationError("General_Sec0_Error_ClasificacionNuevosProdsNoPermitida"));
                }
            }

            return errors;
        }
    }
}