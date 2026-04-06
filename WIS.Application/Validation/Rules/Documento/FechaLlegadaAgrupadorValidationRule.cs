using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Documento
{
    public class FechaLlegadaAgrupadorValidationRule : IValidationRule
    {
        protected readonly string _tipoAgrupador;
        protected readonly string _numeroValue;
        protected readonly IUnitOfWork _uow;
        protected readonly DateTime? _fechaLlegada;


        public FechaLlegadaAgrupadorValidationRule(string tipoValue, string numeroValue, IUnitOfWork uow, string fieldValue, IFormatProvider culture)
        {
            this._tipoAgrupador = tipoValue;
            this._numeroValue = numeroValue;
            this._uow = uow;
            this._fechaLlegada = string.IsNullOrEmpty(fieldValue) ? null : (DateTime?)DateTime.Parse(fieldValue, culture);
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var agrupador = this._uow.DocumentoRepository.GetAgrupador(this._numeroValue, this._tipoAgrupador);

            if (agrupador == null)
            {
                errors.Add(new ValidationError("DOC350_Sec0_Error_AgrupadorNoExiste"));
            }
            else if (agrupador.FechaLlegada != null)
            {
                errors.Add(new ValidationError("DOC350_Sec0_Error_FechaLlegadaYaConfirmada"));
            }


            if (this._fechaLlegada != null)
            {
                if (agrupador.FechaSalida != null)
                {
                    if (this._fechaLlegada < agrupador.FechaSalida)
                    {
                        errors.Add(new ValidationError("DOC350_Sec0_Error_FechaLlegadaMayorFechaSalida"));
                    }
                }
                else
                {
                    if (this._fechaLlegada < agrupador.FechaAlta)
                    {
                        errors.Add(new ValidationError("DOC350_Sec0_Error_FechaLlegadaMayorFechaSalida"));
                    }
                }
            }

            return errors;
        }
    }
}
