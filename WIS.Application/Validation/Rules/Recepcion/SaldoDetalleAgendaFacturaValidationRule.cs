using System;
using System.Collections.Generic;
using System.Globalization;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Recepcion.Enums;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Recepcion
{
    public class SaldoDetalleAgendaFacturaValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly string _valueOld;
        protected readonly string _agenda;
        protected readonly string _empresa;
        protected readonly string _producto;
        protected readonly string _identificador;
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _proveedor;

        public SaldoDetalleAgendaFacturaValidationRule(IUnitOfWork uow, string idAgenda, string idEmpresa, string codigoProducto, string identificador, string value, string valueOld, IFormatProvider proveedorDeFormato)
        {
            this._value = value;
            this._valueOld = valueOld;
            this._agenda = idAgenda;
            this._empresa = idEmpresa;
            this._producto = codigoProducto;
            this._identificador = identificador;
            this._uow = uow;
            this._proveedor = proveedorDeFormato;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var entidadAgenda = _uow.AgendaRepository.GetAgendaSinDetalles(int.Parse(_agenda));
            var tipoRecepcion = _uow.RecepcionTipoRepository.GetRecepcionTipo(entidadAgenda.TipoRecepcionInterno);

            // Si maneja factura o es de seleccion de referencia, compruebo saldo

            // TODO Mejorar 
            if (_uow.AgendaRepository.AgendaManejaFactura(int.Parse(_agenda), int.Parse(_empresa))
                || (tipoRecepcion.TipoSeleccionReferencia == TipoSeleccionReferenciaDb.MonoSeleccion
                || tipoRecepcion.TipoSeleccionReferencia == TipoSeleccionReferenciaDb.MultiSeleccion))
            {
                var referencias = _uow.ReferenciaRecepcionRepository.GetReferenciasAgenda(int.Parse(_agenda));

                decimal saldo = 0;

                foreach (var referencia in referencias)
                {
                    saldo += referencia.GetSaldoDetalles(_producto, _identificador);
                }

                var cantidad = decimal.Parse(_value, _proveedor);

                // Sumo el valor anterior para disponer del saldo
                if (!string.IsNullOrEmpty(_valueOld))
                    saldo += decimal.Parse(_valueOld, _proveedor);

                if ((saldo - cantidad) < 0)
                {
                    errors.Add(new ValidationError("General_Sec0_Error_SaldoOrdenCompraInsuficiente"));
                }

            }

            return errors;
        }


    }
}
