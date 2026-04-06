using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Vml.Office;
using WIS.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Repositories;
using WIS.Domain.General;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class AveriaStockLpnValidationRule : IValidationRule
    {
        protected readonly int _idLpnDet;
        protected readonly long _nuLpn;
        protected readonly string _ubicacion;
        protected readonly string _value;
        protected readonly int _empresa;
        protected readonly string _producto;
        protected readonly string _identificador;
        protected readonly decimal _faixa;
        protected readonly decimal _qtStock;
        protected readonly IUnitOfWork _uow;
        protected readonly string _oldValue;

        public AveriaStockLpnValidationRule(IUnitOfWork uow, string value,string oldValue, long nuLpn, int idLpnDet, string ubicacion, int empresa, string producto, string identificador, decimal faixa, decimal qtStock)
        {
            this._ubicacion = ubicacion;
            this._value = value;
            this._empresa = empresa;
            this._producto = producto;
            this._identificador = identificador;
            this._faixa = faixa;
            this._nuLpn = nuLpn;
            this._idLpnDet = idLpnDet;
            this._qtStock = qtStock;
            this._uow = uow;
            this._oldValue = oldValue;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            decimal cantidadDisponibleLpn = _uow.ManejoLpnRepository.GetCantidadStockDisponibleDetalleLpn(_nuLpn, _ubicacion, _empresa, _producto, _identificador, _faixa, _idLpnDet);
            cantidadDisponibleLpn = cantidadDisponibleLpn + _uow.ManejoLpnRepository.GetCantidNoDisponibleEnLpn(_nuLpn, _empresa, _producto, _identificador, _faixa,_idLpnDet);

            if (_value != _oldValue && _value == "S"  && _qtStock != cantidadDisponibleLpn)
                errors.Add(new ValidationError("STO510_Sec0_Error_COL19_ProdXReservaPendiente", new List<string> { _producto, _uow.ProductoRepository.GetDescripcion( _empresa, _producto) }));

            return errors;
        }
    }
}
