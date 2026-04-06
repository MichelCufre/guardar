using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.OrdenTarea
{
    class FechaHastaOrdenTareaFuncionarioValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _fechaDesde;
        protected readonly string _fechaHasta;
        protected readonly string _cdFuncionario;
        protected readonly IFormatProvider _culture;
        protected readonly string _nuOrdenTareaFuncionario;

        public FechaHastaOrdenTareaFuncionarioValidationRule(IUnitOfWork uow, string fechaDesde, string fechaHasta, string cdFuncionario, IFormatProvider culture, string nuOrdenTareaFuncionario = "")
        {
            this._uow = uow;
            this._fechaDesde = fechaDesde;
            this._fechaHasta = fechaHasta;
            this._cdFuncionario = cdFuncionario;
            this._culture = culture;
            _nuOrdenTareaFuncionario = nuOrdenTareaFuncionario;
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

            if (!string.IsNullOrEmpty(_cdFuncionario))
            {
                var funcionario = int.Parse(_cdFuncionario);

                if (!string.IsNullOrEmpty(_fechaHasta))
                {
                    var hasta = DateTime.Parse(_fechaHasta, this._culture);

                    if (_uow.TareaRepository.AnySesionActivaFuncionario(funcionario, hasta))
                    {
                        errors.Add(new ValidationError("General_ORT060_Error_SesionActivaFuncionario"));
                        return errors;
                    }

                    if (_uow.TareaRepository.AnySesionActivaFuncionarioAuxiliar(funcionario, hasta))
                    {
                        errors.Add(new ValidationError("General_ORT060_Error_SesionActivaFuncionarioAuxiliar"));
                        return errors;
                    }

                    var ordenTareaFuncionario = long.TryParse(_nuOrdenTareaFuncionario, out var id) ? id : -1;

                    if (_uow.TareaRepository.AnySolapamientoRegistroTareaFuncionario(funcionario, hasta, ordenTareaFuncionario))
                    {
                        errors.Add(new ValidationError("General_ORT060_Error_SolapamientoRegistroTareaFuncionario"));
                        return errors;
                    }

                    if (_uow.TareaRepository.AnySolapamientoRegistroTareaAmigableFuncionario(funcionario, hasta, ordenTareaFuncionario))
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
