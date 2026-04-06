using System;
using System.Collections.Generic;
using System.Globalization;
using WIS.Domain.DataModel;
using WIS.Domain.Picking;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class PuedeActualizarDetallePedidoValidationRule : IValidationRule
    {
        protected string pedido;
        protected string empresa;
        protected string cliente;
        protected string producto;
        protected string lote;
        protected string faixa;
        protected string espLote;
        protected decimal QtLiberado;
        protected decimal QtAnulado;
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public PuedeActualizarDetallePedidoValidationRule(string NU_PEDIDO, string CD_EMPRESA, string QT_LIBERADO, string QT_ANULADO, string CD_CLIENTE, string CD_PRODUCTO, string NU_IDENTIFICADOR, string CD_FAIXA, string ID_ESPECIFICA_IDENTIFICADOR, IUnitOfWork uow, IFormatProvider culture)
        {
            decimal.TryParse(QT_LIBERADO, NumberStyles.Number, culture, out this.QtLiberado);
            decimal.TryParse(QT_ANULADO, NumberStyles.Number, culture, out this.QtAnulado);

            this.pedido = NU_PEDIDO;
            this.empresa = CD_EMPRESA;
            this.cliente = CD_CLIENTE;
            this.producto = CD_PRODUCTO;
            this.lote = NU_IDENTIFICADOR;
            this.faixa = CD_FAIXA;
            this.espLote = ID_ESPECIFICA_IDENTIFICADOR;
            this._uow = uow;
            this._culture = culture;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            DetallePedido detallePedido = _uow.PedidoRepository.GetDetallePedido(pedido, int.Parse(empresa), cliente, producto, lote, decimal.Parse(faixa, this._culture), espLote);
            if (detallePedido.CantidadAnulada != QtAnulado || detallePedido.CantidadLiberada != QtLiberado)
            {
                List<string> argumentos = new List<string>();
                argumentos.Add(producto);
                argumentos.Add(lote);
                argumentos.Add(pedido);
                argumentos.Add(empresa);
                argumentos.Add(cliente);
                errors.Add(new ValidationError("PRE670_Sec0_Error_Er01_ModificaronSimultaneamenteElPedido", argumentos));
            }
            return errors;
        }
    }
}