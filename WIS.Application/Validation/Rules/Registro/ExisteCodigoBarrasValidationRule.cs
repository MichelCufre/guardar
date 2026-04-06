using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ExisteCodigoBarrasValidationRule : IValidationRule
    {
        protected readonly string _idBarra;
        protected readonly string _idEmpresa;
        protected readonly string _idProducto;
        protected readonly IUnitOfWork _uow;

        public ExisteCodigoBarrasValidationRule(IUnitOfWork uow, string idBarra, string idEmpresa, string idProducto)
        {
            this._idBarra = idBarra;
            this._idEmpresa = idEmpresa;
            this._idProducto = idProducto;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();


            if (string.IsNullOrEmpty(_idBarra) || string.IsNullOrEmpty(_idEmpresa) || string.IsNullOrEmpty(_idProducto))
            {
                errors.Add(new ValidationError("General_Sec0_Error_CodigoBarraOEmpresaFaltanteOProducto"));
            }
            else
            {
                if (_uow.ProductoCodigoBarraRepository.ExisteCodigoBarra(_idBarra,int.Parse(_idEmpresa)))
                    errors.Add(new ValidationError("General_Sec0_Error_CodigoBarraExistente"));
            }

            return errors;
        }
    }
}