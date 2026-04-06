using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExisteDetalleCodigoMultidatoEmpresaValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _empresa;
        protected readonly string _codigoMultidato;
        protected readonly string _codigoAplicacion;
        protected readonly string _codigoCampo;
        protected readonly string _codigoAI;

        public ExisteDetalleCodigoMultidatoEmpresaValidationRule(IUnitOfWork uow, int empresa, string codigoMultidato, string codigoAplicacion, string codigoCampo, string codigoAI)
        {
            this._uow = uow;
            this._empresa = empresa;
            this._codigoMultidato = codigoMultidato;
            this._codigoAplicacion = codigoAplicacion;
            this._codigoCampo = codigoCampo;
            this._codigoAI = codigoAI;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.CodigoMultidatoRepository.ExisteDetalleCodigoMultidatoEmpresa(_empresa, _codigoMultidato, _codigoAplicacion, _codigoCampo, _codigoAI))
                errors.Add(new ValidationError("REG100_Sec0_Error_DetalleExistente", new List<string>() { this._codigoAI, _codigoCampo, _codigoAplicacion }));

            return errors;
        }
    }
}
