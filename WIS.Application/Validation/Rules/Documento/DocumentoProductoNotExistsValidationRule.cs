using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Documento
{
    public class DocumentoProductoNotExistsValidationRule : IValidationRule
    {
        protected readonly string _nroDocumento;
        protected readonly string _tipoDocumento;
        protected readonly int _empresa;
        protected readonly string _producto;
        protected readonly string _identificador;
        protected readonly decimal _faixa;
        protected readonly IUnitOfWork _uow;

        public DocumentoProductoNotExistsValidationRule(IUnitOfWork uow, string nroDocumento, string tipoDocumento, int empresa, string producto, string identificador, decimal faixa)
        {
            this._nroDocumento = nroDocumento;
            this._tipoDocumento = tipoDocumento;
            this._empresa = empresa;
            this._producto = producto;
            this._identificador = identificador;
            this._faixa = faixa;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (this._uow.DocumentoRepository.AnyDetalleDocumento(this._nroDocumento, this._tipoDocumento, this._empresa, this._producto, this._identificador, this._faixa))
                errors.Add(new ValidationError("General_Sec0_Error_Error16"));

            return errors;
        }
    }
}
