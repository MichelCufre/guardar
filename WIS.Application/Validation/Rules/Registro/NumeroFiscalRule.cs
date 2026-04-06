using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class NumeroFiscalRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly string _valueTipoFiscal;
        protected readonly IUnitOfWork _uow;

        public NumeroFiscalRule(IUnitOfWork uow, string tipoFiscal, string value)
        {
            this._value = value;
            this._valueTipoFiscal = tipoFiscal;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (string.IsNullOrEmpty(_valueTipoFiscal))
                return errors;

            switch (_valueTipoFiscal)
            {
                case "TIPOFISCAL_RUT": //TODO: Ver como solucionar esto
                    validateRut(errors);
                    break;
                default:
                    break;
            }

            return errors;
        }


        public virtual bool validateRut(List<IValidationError> errors)
        {
            string rutString = this._value;

            long rutNum, dosPrimerasPos, deTresAOcho, novenaYDecima;


            if (!long.TryParse(rutString, out rutNum))
            {
                errors.Add(new ValidationError("General_Sec0_Error_RutValidation_RutTieneCaractInv"));
                return false;
            }

            if (rutString.Length != 12 && rutString.Length != 11)
            {
                errors.Add(new ValidationError("General_Sec0_Error_RutValidation_RutCantidadDigitos"));
                return false;
            }

            if (rutNum < 0)
            {
                errors.Add(new ValidationError("General_Sec0_Error_RutValidation_RutNegativo"));
                return false;
            }

            dosPrimerasPos = long.Parse(rutString.Substring(0, 2));

            if (dosPrimerasPos < 01 || dosPrimerasPos > 21)
            {
                errors.Add(new ValidationError("General_Sec0_Error_RutValidation_PosicionesIniciales"));
                return false;
            }

            deTresAOcho = long.Parse(rutString.Substring(2, 6));

            if (deTresAOcho == 000000)
            {
                errors.Add(new ValidationError("General_Sec0_Error_RutValidation_PosicionesTresAOcho"));
                return false;
            }

            novenaYDecima = long.Parse(rutString.Substring(8, 2));

            if (novenaYDecima != 00)
            {
                errors.Add(new ValidationError("General_Sec0_Error_RutValidation_PosicionesNovenaYDecima"));
                return false;
            }

            return validarDigitoVerificador(rutString, errors);

        }
        public virtual bool validarDigitoVerificador(string rutString, List<IValidationError> errors)
        {
            var vectorRut = rutString.ToArray();
            var vectorContsantes = "43298765432".ToArray();
            var sumaProductos = 0;
            var digitoVerificadorIngresado = 0;
            var digitoVerificador = 0;


            for (int i = 0; i < vectorContsantes.Length; i++)
            {
                char aux1 = vectorRut[i];
                char aux2 = vectorContsantes[i];
                sumaProductos = sumaProductos + (int.Parse(aux1.ToString()) * int.Parse(vectorContsantes[i].ToString()));
            }

            var resto = sumaProductos % 11;
            var checkDigitAux = 11 - resto;

            if (rutString.Length == 12)
            {
                char charVerificador = vectorRut[11];
                digitoVerificadorIngresado = int.Parse(charVerificador.ToString());

                if (checkDigitAux == 11)
                {
                    digitoVerificador = 0;

                    if (digitoVerificador != digitoVerificadorIngresado)
                    {
                        errors.Add(new ValidationError("General_Sec0_Error_RutValidation_DVIncorrecto"));
                        return false;
                    }
                }
                else if (checkDigitAux < 10)
                {
                    digitoVerificador = checkDigitAux;
                    if (digitoVerificador != digitoVerificadorIngresado)
                    {
                        errors.Add(new ValidationError("General_Sec0_Error_RutValidation_DVIncorrecto"));
                        return false;
                    }
                }
            }
            else if (rutString.Length == 11)
            {
                if (checkDigitAux != 10)
                {
                    errors.Add(new ValidationError("General_Sec0_Error_RutValidation_DVIncorrecto"));
                    return false;
                }
            }

            return true;

        }


    }
}







