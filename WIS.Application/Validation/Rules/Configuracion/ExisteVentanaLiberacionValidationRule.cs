using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Configuracion
{
    public class ExisteVentanaLiberacionValidationRule : IValidationRule
    {
        protected readonly string _ventanaLiberacion;
        protected readonly string _empresa;
        protected readonly string _cliente;
        protected readonly IUnitOfWork _uow;

        public ExisteVentanaLiberacionValidationRule(IUnitOfWork uow, string ventanaLiberacion, string empresa, string cliente)
        {
            this._ventanaLiberacion = ventanaLiberacion;
            this._empresa = empresa;
            this._cliente = cliente;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (int.TryParse(_empresa, out int empresa))
            {
                var clienteVentanaLiberacion = _uow.AgenteRepository.GetVentanaLiberacionCliente(empresa, _cliente, this._ventanaLiberacion);

                if (clienteVentanaLiberacion != null)
                    errors.Add(new ValidationError("REG221_Sec0_Error_ClienteVentanaLiberacionExiste"));
            }

            return errors;
        }
    }
}
