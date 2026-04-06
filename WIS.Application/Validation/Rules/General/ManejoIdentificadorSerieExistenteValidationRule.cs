using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.Enums;
using WIS.Exceptions;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.General
{
    public class ManejoIdentificadorSerieExistenteValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _empresa;
        protected readonly string _codigoProducto;
        protected readonly string _identificador;
        protected readonly string _ubicacion;

        public ManejoIdentificadorSerieExistenteValidationRule(IUnitOfWork uow, int empresa, string codigoProducto, string identificador, string ubicacion = null)
        {
            _uow = uow;
            _empresa = empresa;
            _codigoProducto = codigoProducto;
            _identificador = identificador;
            _ubicacion = ubicacion;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var producto = _uow.ProductoRepository.GetProducto(_empresa, _codigoProducto);

            if (producto.ManejoIdentificador == ManejoIdentificador.Serie)
            {
                if ((string.IsNullOrEmpty(_ubicacion) && _uow.StockRepository.ExisteSerie(_codigoProducto, _empresa, _identificador)) ||
                    (!string.IsNullOrEmpty(_ubicacion) && _uow.StockRepository.ExisteSerieEnOtraUbicacion(_ubicacion, _codigoProducto, _empresa, _identificador)))
                {
                    errors.Add(new ValidationError("General_Sec0_Error_SerieYaExiste", new List<string> { _identificador, _codigoProducto, _empresa.ToString() }));
                }
            }

            return errors;
        }
    }
}
