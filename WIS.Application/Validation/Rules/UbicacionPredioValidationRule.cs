using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class UbicacionPredioValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _cdEndereco;
        protected readonly string _predio;


        public UbicacionPredioValidationRule(IUnitOfWork uow, string cdEndereco, string predio)
        {
            _uow = uow;
            _cdEndereco = cdEndereco;
            _predio = predio;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var ubicacion = _uow.UbicacionRepository.GetUbicacion(_cdEndereco);
            if (ubicacion.NumeroPredio != _predio)
                errors.Add(new ValidationError("General_Sec0_Error_UbicacionNoPertenecePredio", new List<string>() { _predio }));

            return errors;
        }
    }
}
