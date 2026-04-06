using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Facturacion
{

    public class ExisteTipoCalculoValidationRule : IValidationRule
    {
        protected readonly string _tipoCalculo;
        protected readonly IUnitOfWork _uow;
        protected readonly FacturacionDb FacturacionDb;
        public ExisteTipoCalculoValidationRule(IUnitOfWork uow, string tipoCalculo)
        {
            this.FacturacionDb = new FacturacionDb();
            this._tipoCalculo = tipoCalculo;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!FacturacionDb.getTiposCalculo().Any(t => t == _tipoCalculo))
                errors.Add(new ValidationError("FAC001_Sec0_Error_TipoNoExiste"));
            
                


            return errors;
        }
    }
}