using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel;
using WIS.Domain.General.Enums;
using WIS.Validation;
using WIS.Domain.DataModel.Mappers.Constants;

namespace WIS.Application.Validation.Rules.Registro
{
    public class TipoManejoFechaModalidadLoteValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _tipoManejoFecha;
        protected readonly string _modalidadIngresoLote;

        public TipoManejoFechaModalidadLoteValidationRule(IUnitOfWork uow, string tipoManejoFecha, string modalidadIngresoLote)
        {
            _uow = uow;
            _tipoManejoFecha = tipoManejoFecha;
            _modalidadIngresoLote = modalidadIngresoLote;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_tipoManejoFecha == ManejoFechaProductoDb.Fifo && _modalidadIngresoLote == ModalidadIngresoLoteDb.Vencimiento)
                errors.Add(new ValidationError("REG009_msg_Error_ManejoFechaIncompatibleModalidadLote"));

            return errors;
        }
    }
}
