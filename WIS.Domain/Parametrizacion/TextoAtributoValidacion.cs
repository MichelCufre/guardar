using System;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;

namespace WIS.Domain.Parametrizacion
{
    public class TextoAtributoValidacion : IAtributoValidacionService
    {
        protected Atributo _atributo;
        protected ILpnServiceContext _serviceContext;

        public TextoAtributoValidacion(Atributo atributo = null, ILpnServiceContext serviceContext = null)
        {
            _atributo = atributo;
            _serviceContext = serviceContext;
        }

        public virtual void Validar(IUnitOfWork uow, int idAtributo, short idValidacion, string valorAValidar, string valorComparativo, IFormatProvider culture, bool invocarAPICustom, out Error error)
        {
            error = null;
            bool encontroCaracterNoValido = false;

            AtributoValidacion atributoValidacion;

            if (_serviceContext != null)
                atributoValidacion = _serviceContext.GetAtributoValidacion(idValidacion);
            else
                atributoValidacion = uow.AtributoRepository.GetAtributoValidacion(idValidacion);

            if (string.IsNullOrEmpty(atributoValidacion.Error))
                atributoValidacion.Error = "General_lbl_Title_ErrorValidacion";

            switch (idValidacion)
            {
                case ValidacionAtributoDb.LargoMáximo:

                    var lengthMaximo = int.Parse(valorComparativo);
                    if (valorAValidar.Length > lengthMaximo)
                        error = new Error(atributoValidacion.Error, new string[] { valorComparativo });

                    break;
                case ValidacionAtributoDb.LargoMinino:

                    var lengthMinimo = int.Parse(valorComparativo);
                    if (valorAValidar.Length < lengthMinimo)
                        error = new Error(atributoValidacion.Error, new string[] { valorComparativo });

                    break;
                case ValidacionAtributoDb.SoloLetras:

                    if (!SoloLetras(valorAValidar))
                        error = new Error(atributoValidacion.Error);

                    break;
                case ValidacionAtributoDb.Mayúsculas:

                    if (!SoloMayusculas(valorAValidar))
                        error = new Error(atributoValidacion.Error);

                    break;
                case ValidacionAtributoDb.Minúsculas:

                    if (!SoloMinusculas(valorAValidar))
                        error = new Error(atributoValidacion.Error);

                    break;
                case ValidacionAtributoDb.CaracteresValidos:

                    foreach (char c in valorAValidar)
                    {
                        if (!valorComparativo.Contains(c))
                            encontroCaracterNoValido = true;
                    }

                    if (encontroCaracterNoValido)
                        error = new Error(atributoValidacion.Error, new string[] { valorComparativo });

                    break;
                case ValidacionAtributoDb.CaracteresInvalidos:

                    foreach (char c in valorAValidar)
                    {
                        if (valorComparativo.Contains(c))
                            encontroCaracterNoValido = true;
                    }

                    if (encontroCaracterNoValido)
                        error = new Error(atributoValidacion.Error, new string[] { valorComparativo });

                    break;
                case ValidacionAtributoDb.TextoAPICustom:

                    if (invocarAPICustom)
                    {
                        ValidationAtributeAPI api = new ValidationAtributeAPI();
                        api.HttpPost(valorAValidar, valorComparativo, idAtributo, out error);
                    }

                    break;
            }
        }

        public virtual bool SoloLetras(string stringToVerify)
        {
            return stringToVerify.All(c => char.IsLetter(c));
        }

        public virtual bool SoloMayusculas(string stringToVerify)
        {
            return stringToVerify.All(c => !char.IsLower(c));
        }

        public virtual bool SoloMinusculas(string stringToVerify)
        {
            return stringToVerify.All(c => !char.IsUpper(c));
        }
    }
}
