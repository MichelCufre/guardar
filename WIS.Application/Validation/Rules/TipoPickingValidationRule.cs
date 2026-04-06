using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Validation;
namespace WIS.Application.Validation.Rules
{
    public class TipoPickingValidationRule : IValidationRule
    {
        protected IUnitOfWork _uow;
        protected readonly string _tpPicking;
        public TipoPickingValidationRule(IUnitOfWork uow, string tpPicking)
        {
            this._uow = uow;
            this._tpPicking = tpPicking;
        }
        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            var dominios = _uow.DominioRepository.GetDominios(CodigoDominioDb.TipoPicking);

            if (!dominios.Any(t => t.Valor == _tpPicking))
                errors.Add(new ValidationError("General_Sec0_Error_Error100"));
            return errors;
        }
    }
}