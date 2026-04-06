using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Stock
{
    public class TraspasoEmpresasDestinoDocumentalValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _empresaOrigen;
        protected readonly int _empresaDestino;

        public TraspasoEmpresasDestinoDocumentalValidationRule(IUnitOfWork uow,int empresaOrigen, int empresaDestino)
        {
            this._uow = uow;
            this._empresaOrigen = empresaOrigen;
            this._empresaDestino = empresaDestino;

        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            var isEmpresaDocumental = this._uow.EmpresaRepository.IsEmpresaDocumental(_empresaDestino);

            var config = this._uow.TraspasoEmpresasRepository.GetConfiguracionTraspasoByEmpresa(_empresaOrigen);

            if (isEmpresaDocumental && (config != null && string.IsNullOrEmpty(config.TipoDocumentoIngreso) || string.IsNullOrEmpty(config.TipoDocumentoEgreso)))
            {
                errors.Add(new ValidationError("STO820_Sec0_Error_EmpresaOrigenTieneQueConfigurarTipoDocumento"));
                return errors;
            }

            return errors;
        }
    }
}