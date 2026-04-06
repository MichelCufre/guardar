using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.Documento;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Documento
{
    public class CantidadTotalLineasDocumentoIngresoValidationRule : IValidationRule
    {
        protected readonly string _numeroDocumento;
        protected readonly string _tipoDocumento;
        protected readonly string _qtTotalIngresada;
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public CantidadTotalLineasDocumentoIngresoValidationRule(IUnitOfWork uow, 
            string nroDocumento, 
            string tpDocumento, 
            string qtTotal,
            IFormatProvider culture)
        {
            this._uow = uow;
            this._numeroDocumento = nroDocumento;
            this._tipoDocumento = tpDocumento;
            this._qtTotalIngresada = qtTotal;
            this._culture = culture;
        }


        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            IDocumento documento = this._uow.DocumentoRepository.GetIngreso(this._numeroDocumento, this._tipoDocumento);
            decimal qtTotalIng = decimal.Parse(this._qtTotalIngresada, this._culture);
            var qtTotalAux = documento.Lineas.Sum(l => l.CantidadIngresada);

            if (qtTotalAux != qtTotalIng)
                errors.Add(new ValidationError("General_Sec0_Error_Error12"));

            return errors;
        }
    }
}
