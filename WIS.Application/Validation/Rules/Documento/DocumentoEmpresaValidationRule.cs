using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Documento
{
    public class DocumentoEmpresaValidationRule : IValidationRule
    {
        protected readonly string _tpNuDocumento;
        protected readonly string _empresa;
        protected readonly IUnitOfWork _uow;

        public DocumentoEmpresaValidationRule(string tpNuDocumento, string empresa, IUnitOfWork uow)
        {
            this._tpNuDocumento = tpNuDocumento;
            this._empresa = empresa;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(this._tpNuDocumento) && !string.IsNullOrEmpty(this._empresa))
            {
                var index = this._tpNuDocumento.IndexOf("_");
                var nuDocumento = this._tpNuDocumento.Substring(index + 1);
                var tpDocumento = this._tpNuDocumento.Substring(0, index);

                if (int.Parse(this._empresa) != (this._uow.DocumentoRepository.GetDocumento(nuDocumento, tpDocumento).Empresa ?? -1))
                    errors.Add(new ValidationError("General_Sec0_Error_DocumentoEmpresa"));
            }

            return errors;
        }
    }
}
