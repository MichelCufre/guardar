using System;
using System.Globalization;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;

namespace WIS.Domain.Parametrizacion
{
    public class NumeroAtributoValidacion : IAtributoValidacionService
    {
        protected Atributo _atributo;
        protected ILpnServiceContext _serviceContext;

        public NumeroAtributoValidacion(Atributo atributo = null, ILpnServiceContext serviceContext = null)
        {
            _atributo = atributo;
            _serviceContext = serviceContext;
        }


        public virtual void Validar(IUnitOfWork uow, int idAtributo, short idValidacion, string valorAValidar, string strValorValidacion, IFormatProvider culture, bool invocarAPICustom, out Error error)
        {
            error = null;
            decimal valor;
            decimal valorComparativo;

            string separador;
            Atributo atributo;
            AtributoValidacion atributoValidacion;

            if (_serviceContext != null)
            {
                atributo = _atributo;
                atributoValidacion = _serviceContext.GetAtributoValidacion(idValidacion);
                separador = ((CultureInfo)culture).NumberFormat.NumberDecimalSeparator;
            }
            else
            {
                atributo = uow.AtributoRepository.GetAtributo(idAtributo);
                atributoValidacion = uow.AtributoRepository.GetAtributoValidacion(idValidacion);
                separador = ((CultureInfo)culture).NumberFormat.NumberDecimalSeparator;
            }

            if (string.IsNullOrEmpty(atributoValidacion.Error))
                atributoValidacion.Error = "General_lbl_Title_ErrorValidacion";

            valorAValidar = valorAValidar.Replace(atributo.Separador, separador);

            switch (idValidacion)
            {
                case ValidacionAtributoDb.NumeroMayor:

                    if (decimal.TryParse(valorAValidar, NumberStyles.Number, culture, out valor) && decimal.TryParse(strValorValidacion, NumberStyles.Number, culture, out valorComparativo))
                    {
                        if (valor <= valorComparativo)
                            error = new Error(atributoValidacion.Error, new string[] { strValorValidacion });
                    }
                    else
                        error = new Error("General_Sec0_Error_Error35");

                    break;
                case ValidacionAtributoDb.NumeroMenor:

                    if (decimal.TryParse(valorAValidar, NumberStyles.Number, culture, out valor) && decimal.TryParse(strValorValidacion, NumberStyles.Number, culture, out valorComparativo))
                    {
                        if (valor >= valorComparativo)
                            error = new Error(atributoValidacion.Error, new string[] { strValorValidacion });
                    }
                    else
                        error = new Error("General_Sec0_Error_Error35");

                    break;
                case ValidacionAtributoDb.NumeroAPICustom:

                    if (invocarAPICustom)
                    {
                        ValidationAtributeAPI api = new ValidationAtributeAPI();
                        api.HttpPost(valorAValidar, strValorValidacion, idAtributo, out error);
                    }

                    break;
            }
        }
    }
}

