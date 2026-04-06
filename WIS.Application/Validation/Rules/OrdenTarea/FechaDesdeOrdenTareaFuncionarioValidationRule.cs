using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.OrdenTarea
{
    class FechaDesdeOrdenTareaFuncionarioValidationRule : IValidationRule
    {
        protected readonly string _fechaInicioOrden;
        protected readonly string _fechaDesde;
        protected readonly string _cdFuncionario;
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;
        protected readonly string _nuOrdenTareaFuncionario;

        public FechaDesdeOrdenTareaFuncionarioValidationRule(IUnitOfWork uow, string fechaInicioOrden, string fechaDesde, string cdFuncionario, IFormatProvider culture, string nuOrdenTareaFuncionario = "")
        {
            this._uow = uow;
            this._fechaInicioOrden = fechaInicioOrden;
            this._fechaDesde = fechaDesde;
            this._cdFuncionario = cdFuncionario;
            this._culture = culture;
            this._nuOrdenTareaFuncionario = nuOrdenTareaFuncionario;
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

            if (!string.IsNullOrEmpty(_cdFuncionario))
            {
                var funcionario = int.Parse(_cdFuncionario);

                if (!string.IsNullOrEmpty(_fechaDesde))
                {
                    var desde = DateTime.Parse(_fechaDesde, _culture);

                    if (_uow.TareaRepository.AnySesionActivaFuncionario(funcionario, desde))
                    {
                        errors.Add(new ValidationError("General_ORT060_Error_SesionActivaFuncionario"));
                        return errors;
                    }

                    if (_uow.TareaRepository.AnySesionActivaFuncionarioAuxiliar(funcionario, desde))
                    {
                        errors.Add(new ValidationError("General_ORT060_Error_SesionActivaFuncionarioAuxiliar"));
                        return errors;
                    }

                    var ordenTareaFuncionario = long.TryParse(_nuOrdenTareaFuncionario, out var id) ? id : -1;
                    
                    if (_uow.TareaRepository.AnySolapamientoRegistroTareaFuncionario(funcionario, desde, ordenTareaFuncionario))
                    {
                        errors.Add(new ValidationError("General_ORT060_Error_SolapamientoRegistroTareaFuncionario"));
                        return errors;
                    }

                    if (_uow.TareaRepository.AnySolapamientoRegistroTareaAmigableFuncionario(funcionario, desde, ordenTareaFuncionario))
                    {
                        errors.Add(new ValidationError("General_ORT060_Error_SolapamientoRegistroTareaAmigableFuncionario"));
                        return errors;
                    }
                }
            }

            return errors;
        }
    }
}
