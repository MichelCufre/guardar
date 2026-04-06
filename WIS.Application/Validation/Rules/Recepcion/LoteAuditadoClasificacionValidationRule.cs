using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services.Interfaces;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Recepcion
{
    public class LoteAuditadoClasificacionValidationRule : IValidationRule
    {
        protected readonly int _nuEtiquetaLote;
        protected readonly int _agenda;
        protected readonly string _producto;
        protected readonly string _lote;
        protected readonly IUnitOfWork _uow;
        protected readonly IParameterService _parameterService;

        public LoteAuditadoClasificacionValidationRule(IUnitOfWork uow,
            IParameterService parameterService,
            int nuEtiquetaLote,
            int agenda,
            string producto,
            string lote)
        {
            this._nuEtiquetaLote = nuEtiquetaLote;
            this._uow = uow;
            this._parameterService = parameterService;
            this._agenda = agenda;
            this._producto = producto;
            this._lote = lote;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var agenda = this._uow.AgendaRepository.GetAgenda(this._agenda);
            var clasificarLotesNoEsperados = this._parameterService.GetValueByEmpresa("CLASIFICAR_LOTES_NO_ESPERADOS", agenda.IdEmpresa);

            if (clasificarLotesNoEsperados?.ToUpper() == "N")
            {
                var detalles = this._uow.EtiquetaLoteRepository.GetDetalles(this._nuEtiquetaLote);
                var cantidadLotes = detalles
                    .Where(d => d.IdEmpresa == agenda.IdEmpresa
                        && d.CodigoProducto == this._producto
                        && (d.Identificador == this._lote || d.Identificador == ManejoIdentificadorDb.IdentificadorAuto))
                    .Count();

                if (cantidadLotes == 0)
                {
                    errors.Add(new ValidationError("General_Sec0_Error_ClasificacionNuevosLotesNoPermitida"));
                }
            }

            return errors;
        }
    }
}