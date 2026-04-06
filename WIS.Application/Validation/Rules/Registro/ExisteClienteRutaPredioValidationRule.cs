using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Domain.General;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ExisteClienteRutaPredioValidationRule : IValidationRule
    {
        protected readonly string _idEmpresa;
        protected readonly string _codigoInternoCliente;
        protected readonly string _predio;
        protected readonly string _rutaAnterior;
        protected readonly IUnitOfWork _uow;

        public ExisteClienteRutaPredioValidationRule(IUnitOfWork uow, string idEmpresa, string codigoInternoCliente, string predio, string rutaAnterior = null)
        {
            this._uow = uow;
            this._idEmpresa = idEmpresa;
            this._codigoInternoCliente = codigoInternoCliente;
            this._predio = predio;
            this._rutaAnterior = rutaAnterior;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (string.IsNullOrEmpty(this._rutaAnterior))
            {
                if (_uow.AgenteRepository.AnyClienteRutaPredio(this._codigoInternoCliente, int.Parse(this._idEmpresa), this._predio))
                    errors.Add(new ValidationError("General_Sec0_Error_ExisteClienteRutaPredio"));
            }
            else
            {
                if (_uow.AgenteRepository.AnyClienteRutaPredio(this._codigoInternoCliente, int.Parse(this._idEmpresa), this._predio, short.Parse(this._rutaAnterior)))
                    errors.Add(new ValidationError("General_Sec0_Error_ExisteClienteRutaPredio"));
            }

            return errors;
        }

    }
}
