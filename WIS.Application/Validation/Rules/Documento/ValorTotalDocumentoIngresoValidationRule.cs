using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.Documento;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Documento
{
    public class ValorTotalDocumentoIngresoValidationRule : IValidationRule
    {
        protected readonly string _numeroDocumento;
        protected readonly string _tipoDocumento;
        protected readonly string _valorTotalIngresado;
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public ValorTotalDocumentoIngresoValidationRule(IUnitOfWork uow, 
            string nroDocumento, 
            string tpDocumento, 
            string vlTotal,
            IFormatProvider culture)
        {
            this._uow = uow;
            this._numeroDocumento = nroDocumento;
            this._tipoDocumento = tpDocumento;
            this._valorTotalIngresado = vlTotal;
            this._culture = culture;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            decimal valor = decimal.Parse(this._valorTotalIngresado, this._culture);

            IDocumento documento = this._uow.DocumentoRepository.GetIngreso(this._numeroDocumento, this._tipoDocumento);

            var vlTotalAux = documento.Lineas.Sum(l => l.ValorMercaderia);

            if (vlTotalAux != valor)
                errors.Add(new ValidationError("General_Sec0_Error_Error38"));

            return errors;
        }
    }
}
