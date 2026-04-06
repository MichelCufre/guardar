using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{

    public class ExistePredioValidationRule : IValidationRule
    {
        protected readonly int _userId;
        protected readonly string _valuePredio;
        protected readonly string _predioLogueado;
        protected readonly IUnitOfWork _uow;

        public ExistePredioValidationRule(IUnitOfWork uow, int idUsuario, string predioLogueado, string valuePredio)
        {
            this._userId = idUsuario;
            this._valuePredio = valuePredio;
            this._predioLogueado = predioLogueado;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_predioLogueado != null)
            {

                if (_predioLogueado == GeneralDb.PredioSinDefinir) //TODO: Pasar esto a una constante o algo
                {
                    List<string> predios = this._uow.SecurityRepository.GetPrediosUsuario(this._userId);
                    if (!predios.Any(d => d == this._valuePredio))
                        errors.Add(new ValidationError("General_Sec0_Error_Er104_PredioNoAsignado"));
                }
                else
                {
                    if (!_uow.PredioRepository.AnyPredio(_predioLogueado))
                        errors.Add(new ValidationError("General_Sec0_Error_Er075_PredioNoDefinido"));
                    if (_predioLogueado != _valuePredio)
                        errors.Add(new ValidationError("General_Sec0_Error_Er105_PredioDiferenteDelLogin"));
                }
            }
            else
            {
                if (!_uow.PredioRepository.AnyPredio(_valuePredio))
                    errors.Add(new ValidationError("General_Sec0_Error_Er075_PredioNoDefinido"));
            }

            return errors;
        }
    }
}
