using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ExistenciaPadronPredioProductoValidationRule : IValidationRule
    {
        protected readonly string _padron;
        protected readonly int? _empresa;
        protected readonly string _predio;
        protected readonly string _producto;
        protected readonly string _prioridad;
        protected readonly IUnitOfWork _uow;

        public ExistenciaPadronPredioProductoValidationRule(IUnitOfWork uow, string padron, string producto, int? empresa, string predio, string prioridad)
        {
            this._uow = uow;
            this._producto = producto;
            this._empresa = empresa;
            this._padron = padron;
            this._predio = predio;
            this._prioridad = prioridad;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (int.TryParse(_padron, out int padronValue) && _empresa.HasValue && int.TryParse(_prioridad, out int prioridadValue))
            {
                if (_uow.UbicacionPickingProductoRepository.AnyUbicacionProductoPadronPrioridad(_producto, _empresa.Value, padronValue, _predio, prioridadValue))
                    errors.Add(new ValidationError("General_Sec0_Error_PredioPadronProductoExistente"));
            }

            return errors;
        }
    }
}
