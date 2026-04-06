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
    public class NoExisteReferenciaProductoEmpresaValidationRule : IValidationRule
    {
        protected readonly string _field;
        protected readonly string _empresa;
        protected readonly string _producto;
        protected readonly IUnitOfWork _uow;

        public NoExisteReferenciaProductoEmpresaValidationRule(IUnitOfWork uow, string field, string empresa, string producto)
        {
            this._field = field;
            this._producto = producto;
            this._empresa = empresa;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            if (_uow.ProductoRepository.ExisteReferenciaProductoEmpresa(this._field, int.Parse(this._empresa)))
            {
                string referenciaProducto = _uow.ProductoRepository.GetReferenciaProductoEmpresa(this._field, int.Parse(this._empresa));
                if (!referenciaProducto.Equals(this._producto))
                    errors.Add(new ValidationError("REG009_Sec0_Error_Er013_ReferenciaUtilizadaProducto", new List<string> { referenciaProducto }));
            }


            return errors;
        }
    }
}
