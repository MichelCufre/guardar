using DocumentFormat.OpenXml.Vml.Office;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class AveriaStockConLpnValidationRule : IValidationRule
    {
        protected readonly string _ubicacion;
        protected readonly string _oldValue;
        protected readonly string _value;
        protected readonly int _empresa;
        protected readonly string _producto;
        protected readonly string _identificador;
        protected readonly decimal _faixa;
        protected readonly IUnitOfWork _uow;

        public AveriaStockConLpnValidationRule(IUnitOfWork uow, string oldValue, string value, string ubicacion, int empresa,string producto,string identificador , decimal faixa )
        {
            this._ubicacion = ubicacion;
            this._oldValue= oldValue;
            this._value = value;
            this._empresa = empresa;
            this._producto = producto;
            this._identificador = identificador;
            this._faixa = faixa;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_oldValue  != _value && _uow.ManejoLpnRepository.AnyStockUbicacionLpn(_ubicacion, _empresa, _producto, _identificador,_faixa))
                errors.Add(new ValidationError("STO150_grid1_Error_WarningLpnEnUbicacion"));

            return errors;
        }
    }
}
