using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.Services.Interfaces;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Recepcion
{
    public class CantidadAuditadaClasificacionValidationRule : IValidationRule
    {
        protected readonly string _cantidadAuditada;
        protected readonly int _nuEtiquetaLote;
        protected readonly int _agenda;
        protected readonly string _producto;
        protected readonly string _lote;
        protected readonly IUnitOfWork _uow;
        protected readonly IParameterService _parameterService;
        protected readonly IFormatProvider _culture;

        public CantidadAuditadaClasificacionValidationRule(IUnitOfWork uow,
            IParameterService parameterService,
            IFormatProvider culture,
            string cantidadAuditada,
            int nuEtiquetaLote,
            int agenda,
            string producto,
            string lote)
        {
            this._cantidadAuditada = cantidadAuditada;
            this._nuEtiquetaLote = nuEtiquetaLote;
            this._uow = uow;
            this._parameterService = parameterService;
            this._agenda = agenda;
            this._producto = producto;
            this._lote = lote;
            this._culture = culture;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            var agenda = this._uow.AgendaRepository.GetAgenda(this._agenda);
            var clasificarCantidadesDeMas = this._parameterService.GetValueByEmpresa("CLASIFICAR_CANTIDADES_DE_MAS", agenda.IdEmpresa);

            if (clasificarCantidadesDeMas?.ToUpper() == "N")
            {
                var detalles = this._uow.EtiquetaLoteRepository.GetDetalles(this._nuEtiquetaLote);
                var cantidadTeorica = detalles
                    .Where(d => d.IdEmpresa == agenda.IdEmpresa
                        && d.CodigoProducto == this._producto
                        && d.Faixa == 1
                        && d.Identificador == this._lote)
                    .Select(d => d.Cantidad ?? 0)
                    .DefaultIfEmpty(0)
                    .Sum();

                if (cantidadTeorica < decimal.Parse(this._cantidadAuditada, this._culture))
                {
                    errors.Add(new ValidationError("General_Sec0_Error_ClasificacionSobrantesNoPermitida"));
                }
            }

            return errors;
        }
    }
}