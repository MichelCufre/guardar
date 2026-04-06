using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ControlDeCalidadEnProductoInexistenteValidationRule : IValidationRule
    {
        protected readonly string _control;
        protected readonly string _empresa;
        protected readonly string _producto;
        protected readonly IUnitOfWork _uow;

        public ControlDeCalidadEnProductoInexistenteValidationRule(IUnitOfWork uow, string control, string empresa, string producto)
        {
            this._control = control;
            this._empresa = empresa;
            this._producto = producto;
            this._uow = uow;
        }
        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            if (!int.TryParse(this._empresa, out int idEmpresa))
            {
                errors.Add(new ValidationError("REG602_Sec0_Error_EmpresaNecesaria"));
                return errors;
            }
            if (String.IsNullOrEmpty(this._producto))
            {
                errors.Add(new ValidationError("REG602_Sec0_Error_ProductoNecesario"));
                return errors;
            }
            int idControl = int.Parse(_control);
            if(_uow.ControlDeCalidadRepository.AnyControlDeCalidadProducto(idControl, idEmpresa, _producto))
                errors.Add(new ValidationError("General_Sec0_Error_CodigoControlDeCalidadClaseExistente"));
            
            return errors;
        }
    }
}
