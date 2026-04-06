using DocumentFormat.OpenXml.InkML;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.Expedicion.EXP110EmpaquetadoPicking.Dto;
using WIS.Domain.General;
using WIS.Exceptions;
using WIS.Extension;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Expedicion
{
    public class EXP110SeleccionProductoCantidadProducto : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _value;
        protected readonly int _nuContenedorOrigen;
        protected readonly int _nuPreparacionOrigen;
        protected readonly string _contenedorDestinoDataJSON;
        protected readonly Producto _producto;
        protected readonly string _rowSelectedPedProdCont;
        protected readonly string _rowSelectedPedProdLote;
        protected readonly IFormatProvider _formatProvider;


        public EXP110SeleccionProductoCantidadProducto(IUnitOfWork uow, IFormatProvider formatProvider, string value, string contenedorDestinoDataJSON, int nuContenedorOrigen, int nuPreparacionOrigen, Producto producto, string rowSelectedPedProdCont, string rowSelectedPedProdLote)
        {
            _uow = uow;
            _value = value;
            _nuContenedorOrigen = nuContenedorOrigen;
            _nuPreparacionOrigen = nuPreparacionOrigen;
            _contenedorDestinoDataJSON = contenedorDestinoDataJSON;
            _producto = producto;
            _rowSelectedPedProdCont = rowSelectedPedProdCont;
            _rowSelectedPedProdLote = rowSelectedPedProdLote;
            _formatProvider = formatProvider;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            try
            {                
                var dataContenedorDestino = !string.IsNullOrEmpty(_contenedorDestinoDataJSON) ? JsonConvert.DeserializeObject<ContenedorDestinoData>(_contenedorDestinoDataJSON) : null;

                if (dataContenedorDestino!=null && _nuContenedorOrigen == dataContenedorDestino.NumeroContenedor)
                    throw new ValidationFailedException("EXP110_form1_Error_ContenedorOrigenIgualDestino");

                decimal qtProductoInput = decimal.Parse(_value, _formatProvider);

                if (!_producto.AceptaDecimales && !int.TryParse(_value, out int result))
                    throw new ValidationFailedException("EXP110_form1_Error_ProductoNoManejaDecimales");

                var filtrarComparteContenedorEntrega = (dataContenedorDestino != null ? true : false);

                if (!_uow.EmpaquetadoPickingRepository.TieneMasDeUnPedidoProductoContenedor(_nuContenedorOrigen, _nuPreparacionOrigen, _producto.Codigo, filtrarComparteContenedorEntrega, dataContenedorDestino?.CompartContenedorEntrega)
                 && !_uow.EmpaquetadoPickingRepository.TieneMasDeUnPedidoProductoLote(_nuContenedorOrigen, _nuPreparacionOrigen, _producto.Codigo, filtrarComparteContenedorEntrega, dataContenedorDestino?.CompartContenedorEntrega, out decimal qtProducto, out string identificador))
                {
                    if (qtProductoInput > qtProducto)
                        throw new ValidationFailedException("EXP110_form1_Error_LaCantidadIngresadaMayorALaDisponible");

                    if (qtProductoInput == 0)
                        throw new ValidationFailedException("EXP110_form1_Error_LaCantidadIngresadaMayorA0");
                }
                else
                {
                    if (string.IsNullOrEmpty(_rowSelectedPedProdCont) && _uow.EmpaquetadoPickingRepository.TieneMasDeUnPedidoProductoContenedor(_nuContenedorOrigen, _nuPreparacionOrigen, _producto.Codigo, filtrarComparteContenedorEntrega, dataContenedorDestino?.CompartContenedorEntrega))
                        throw new ValidationFailedException("EXP110_form1_Error_SeleccionarPedidoProducto");

                    if (!string.IsNullOrEmpty(_rowSelectedPedProdCont))
                    {
                        var nuPedido = _rowSelectedPedProdCont.Split('$')[3];
                        if (string.IsNullOrEmpty(_rowSelectedPedProdLote) && _uow.EmpaquetadoPickingRepository.TieneMasDeUnProductoLotePorPedido(_nuContenedorOrigen, _nuPreparacionOrigen, _producto.Codigo, nuPedido))
                            throw new ValidationFailedException("EXP110_form1_Error_SeleccionarProductoLote");
                    }

                    if (!string.IsNullOrEmpty(_rowSelectedPedProdCont) && !string.IsNullOrEmpty(_rowSelectedPedProdLote))
                    {
                        var keys = _rowSelectedPedProdLote.Split('$');
                        var length = keys.Length;

                        var qtProductoRow = keys[length - 1].ToNumber<decimal>();

                        if (qtProductoInput > qtProductoRow)
                            throw new ValidationFailedException("EXP110_form1_Error_LaCantidadIngresadaMayorALaDisponible");

                        if (qtProductoInput == 0)
                            throw new ValidationFailedException("EXP110_form1_Error_LaCantidadIngresadaMayorA0");
                    }
                }


            }
            catch (ValidationFailedException ex)
            {
                errors.Add(new ValidationError(ex.Message, ex.StrArguments?.ToList()));
            }
            catch (Exception ex)
            {
                errors.Add(new ValidationError(ex.Message));
            }

            return errors;
        }
    }
}
