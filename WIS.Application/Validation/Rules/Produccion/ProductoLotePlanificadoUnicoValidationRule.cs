using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Produccion
{
    public class ProductoLotePlanificadoUnicoValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _empresa;
        protected readonly string _producto;
        protected readonly string _idIngreso;
        protected readonly string _identificador;
        protected readonly bool _planificacionPedido;

        public ProductoLotePlanificadoUnicoValidationRule(IUnitOfWork uow, int empresa, string idIngreso, string producto, string identificador, bool planificacionPedido)
        {
            this._uow = uow;
            this._empresa = empresa;
            this._producto = producto;
            this._idIngreso = idIngreso;
            this._identificador = identificador;
            this._planificacionPedido = planificacionPedido;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (this._uow.ProduccionRepository.AnyInsumoPlanificacion(this._idIngreso, this._empresa, this._producto, this._planificacionPedido, this._identificador))
                errors.Add(new ValidationError("PRD110DetallePedido_grid1_error_ProductoLoteYaExisteInsumos"));
            else if (_identificador == ManejoIdentificadorDb.IdentificadorAuto && _uow.ProduccionRepository.AnyInsumoPlanificacion(this._idIngreso, this._empresa, this._producto, this._planificacionPedido))
                errors.Add(new ValidationError("WMSAPI_msg_Error_EnvioLoteEspecificoyAutoNoPermitido"));
            else if (_identificador != ManejoIdentificadorDb.IdentificadorAuto && _uow.ProduccionRepository.AnyInsumoPlanificacion(this._idIngreso, this._empresa, this._producto, this._planificacionPedido, ManejoIdentificadorDb.IdentificadorAuto))
                errors.Add(new ValidationError("WMSAPI_msg_Error_EnvioLoteEspecificoyAutoNoPermitido"));

            return errors;
        }
    }
}
