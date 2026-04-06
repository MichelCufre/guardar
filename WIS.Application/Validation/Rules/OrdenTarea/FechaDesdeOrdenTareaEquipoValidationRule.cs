using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Extension;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.OrdenTarea
{
    class FechaDesdeOrdenTareaEquipoValidationRule : IValidationRule
    {
        protected readonly string _fechaInicioOrden;
        protected readonly string _fechaDesde;
        protected readonly string _cdEquipo;
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;
        protected readonly string _nuOrdenTareaEquipo;

        public FechaDesdeOrdenTareaEquipoValidationRule(IUnitOfWork uow, string fechaInicioOrden, string fechaDesde, string cdEquipo, IFormatProvider culture, string nuOrdenTareaEquipo = "")
        {
            this._uow = uow;
            this._fechaInicioOrden = fechaInicioOrden;
            this._fechaDesde = fechaDesde;
            this._cdEquipo = cdEquipo;
            this._culture = culture;
            this._nuOrdenTareaEquipo = nuOrdenTareaEquipo;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(_fechaInicioOrden) && !string.IsNullOrEmpty(_fechaDesde))
            {
                var inicioOrden = DateTime.Parse(_fechaInicioOrden, _culture);
                var desde = DateTime.Parse(_fechaDesde, _culture);

                if (inicioOrden >= desde)
                {
                    errors.Add(new ValidationError("General_ORT060_Error_FechaInvalida"));
                    return errors;
                }

                if (desde >= DateTime.Now)
                {
                    errors.Add(new ValidationError("General_Sec0_Error_FechaHoraMayorActual"));
                    return errors;
                }
            }

            if (!string.IsNullOrEmpty(_cdEquipo))
            {
                var equipo = int.Parse(_cdEquipo);

                if (!string.IsNullOrEmpty(_fechaDesde))
                {
                    var desde = DateTime.Parse(_fechaDesde, _culture);

                    if (_uow.TareaRepository.AnySesionActivaEquipo(equipo, desde))
                    {
                        errors.Add(new ValidationError("General_ORT080_Error_SesionActivaEquipoAuxiliar"));
                        return errors;
                    }

                    var ordenTareaEquipo = !string.IsNullOrEmpty(_nuOrdenTareaEquipo) ? long.Parse(_nuOrdenTareaEquipo) : -1;
                    if (_uow.TareaRepository.AnySolapamientoRegistroTareaEquipo(equipo, desde, ordenTareaEquipo))
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
