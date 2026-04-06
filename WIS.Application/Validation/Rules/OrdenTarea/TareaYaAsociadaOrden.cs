using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.OrdenTarea
{
    class TareaYaAsociadaOrden : IValidationRule
    {
        protected readonly string _idCodigo;
        protected readonly IUnitOfWork _uow;
        protected readonly int _numeroOrden;
        protected readonly string _numeroEmpresa;
        public TareaYaAsociadaOrden(IUnitOfWork uow,int numeroOrden, string numeroEmpresa, string valueCodigo)
        {
            this._idCodigo = valueCodigo;
            this._uow = uow;
            this._numeroEmpresa = numeroEmpresa;
            this._numeroOrden = numeroOrden;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.TareaRepository.AnyOrdenTarea(this._numeroOrden, this._idCodigo, int.Parse(this._numeroEmpresa)))
                errors.Add(new ValidationError("General_ORT040_Error_OrdenYaTieneTarea"));

            return errors;
        }
    }
}
