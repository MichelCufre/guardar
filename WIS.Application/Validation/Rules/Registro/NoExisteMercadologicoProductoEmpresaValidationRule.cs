using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General.Enums;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class NoExisteMercadologicoProductoEmpresaValidationRule : IValidationRule
    {
        protected readonly string _field;
        protected readonly string _empresa;
        protected readonly string _producto;
        protected readonly IUnitOfWork _uow;

        public NoExisteMercadologicoProductoEmpresaValidationRule(IUnitOfWork uow, string field, string empresa, string producto)
        {
            this._producto = producto;
            this._empresa = empresa;
            this._field = field;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            string prod = _uow.ProductoRepository.ExisteMercadologicoEmpresaProducto(this._field, int.Parse(this._empresa));

            if (!string.IsNullOrEmpty(prod) && prod.Equals(this._producto))
                errors.Add(new ValidationError("REG009_Sec0_Error_Er014_CodigoMercadologicoDefinidoProducto", new List<string> { prod , this._empresa}));

            return errors;
        }
    }
}
