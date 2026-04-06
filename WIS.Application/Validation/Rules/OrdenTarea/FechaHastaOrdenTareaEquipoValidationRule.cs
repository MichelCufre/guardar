using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Extension;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.OrdenTarea
{
    public class FechaHastaOrdenTareaEquipoValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _fechaDesde;
        protected readonly string _fechaHasta;
        protected readonly string _cdEquipo;
        protected readonly IFormatProvider _culture;
        protected readonly string _nuOrdenTareaEquipo;

        public FechaHastaOrdenTareaEquipoValidationRule(IUnitOfWork uow, string fechaDesde, string fechaHasta, string cdEquipo, IFormatProvider culture, string nuOrdenTareaEquipo = "")
        {
            this._uow = uow;
            this._fechaDesde = fechaDesde;
            this._fechaHasta = fechaHasta;
            this._cdEquipo = cdEquipo;
            this._culture = culture;
            this._nuOrdenTareaEquipo = nuOrdenTareaEquipo;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(_fechaDesde) && !string.IsNullOrEmpty(_fechaHasta))
            {
                var desde = DateTime.Parse(_fechaDesde, this._culture);
                var hasta = DateTime.Parse(_fechaHasta, this._culture);

                if (desde >= hasta)
                {
                    errors.Add(new ValidationError("General_ORT090_Error_FechaInvalida"));
                    return errors;
                }

                if (hasta >= DateTime.Now)
                {
                    errors.Add(new ValidationError("General_Sec0_Error_FechaHoraMayorActual"));
                    return errors;
                }
            }

            if (!string.IsNullOrEmpty(_cdEquipo))
            {
                var equipo = int.Parse(_cdEquipo);

                if (!string.IsNullOrEmpty(_fechaHasta))
                {
                    var hasta = DateTime.Parse(_fechaHasta, this._culture);

                    if (_uow.TareaRepository.AnySesionActivaEquipo(equipo, hasta))
                    {
                        errors.Add(new ValidationError("General_ORT080_Error_SesionActivaEquipo"));
                        return errors;
                    }

                    var ordenTareaEquipo = !string.IsNullOrEmpty(_nuOrdenTareaEquipo) ? long.Parse(_nuOrdenTareaEquipo) : -1;
                    if (_uow.TareaRepository.AnySolapamientoRegistroTareaEquipo(equipo, hasta, ordenTareaEquipo))
                    {
                        errors.Add(new ValidationError("General_ORT080_Error_SolapamientoRegistroTareaEquipo"));
                        return errors;
                    }
                }
            }

            return errors;
        }
    }
}
