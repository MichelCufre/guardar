using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ExisteCodigoRamoProductoValidationRule : IValidationRule
    {
        protected readonly string _codigoRamoProducto;
        protected readonly IUnitOfWork _uow;

        public ExisteCodigoRamoProductoValidationRule(IUnitOfWork uow, string codigoRamoProducto)
        {
            this._codigoRamoProducto = codigoRamoProducto;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var codigoRamoPorducto = short.Parse(this._codigoRamoProducto);

            var errors = new List<IValidationError>();

            if (_uow.ProductoRamoRepository.AnyProductoRamo(codigoRamoPorducto))
                errors.Add(new ValidationError("General_Sec0_Error_CodigoRamoProductoExistente"));

            return errors;
        }
    }
}
