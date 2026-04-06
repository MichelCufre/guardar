using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.General
{
    public class ExisteRutaPedidoValidationRule : IValidationRule
    {
        protected readonly string _ruta;
        protected readonly string _predio;
        protected readonly IUnitOfWork _uow;

        public ExisteRutaPedidoValidationRule(IUnitOfWork uow, string ruta, string predio)
        {
            this._ruta = ruta;
            this._predio = predio;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (string.IsNullOrEmpty(this._predio))
            {
                if (!this._uow.RutaRepository.AnyRuta(short.Parse(this._ruta)))
                    errors.Add(new ValidationError("General_Sec0_Error_CodigoRutaInvalido"));
            }
            else
            {
                if (!this._uow.RutaRepository.AnyRuta(short.Parse(this._ruta), this._predio))
                    errors.Add(new ValidationError("PRE100_Sec0_Error_Er051_RutaPredioInvalido"));
            }

            return errors;
        }
    }
}
