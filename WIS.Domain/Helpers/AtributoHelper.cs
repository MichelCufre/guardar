using System;
using System.Globalization;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Parametrizacion;
using WIS.Extension;
using WIS.FormComponent;

namespace WIS.Domain.Helpers
{
    public class AtributoHelper
    {
        public static string GetValorByIdTipo(Form form, Atributo atributo, string separador, string formaterDateTime)
        {
            string valorReturn = "";
            switch (atributo.IdTipo)
            {
                case TipoAtributoDb.NUMERICO:
                    if (!string.IsNullOrEmpty(atributo.Separador))
                        valorReturn = form.GetField("NUMERO").Value.Replace(atributo.Separador, separador);
                    else
                        valorReturn = form.GetField("NUMERO").Value;
                    break;
                case TipoAtributoDb.FECHA:
                    if (!string.IsNullOrEmpty(form.GetField("FECHA").Value))
                        valorReturn = DateTimeExtension.ParseFromIso(form.GetField("FECHA").Value)?.Date.ToString(formaterDateTime);
                    break;
                case TipoAtributoDb.HORA:
                    valorReturn = form.GetField("HORA").Value;
                    break;
                case TipoAtributoDb.TEXTO:
                    valorReturn = form.GetField("TEXTO").Value;
                    break;
                case TipoAtributoDb.DOMINIO:
                    valorReturn = form.GetField("CD_DOMINIO").Value;
                    break;
                case TipoAtributoDb.SISTEMA:
                    valorReturn = "";
                    break;
            }
            return valorReturn;
        }

        public static string GetValorDisplayByIdTipo(string valor, Atributo atributo, string separador, string formaterDateTime, bool formatoDb)
        {
            string valorReturn = "";
            switch (atributo.IdTipo)
            {
                case TipoAtributoDb.NUMERICO:

                    if (!string.IsNullOrEmpty(valor))
                        valorReturn = valor.Replace(atributo.Separador, separador);

                    break;
                case TipoAtributoDb.FECHA:

                    if (!string.IsNullOrEmpty(valor))
                    {
                        if (formatoDb)
                            valorReturn = DateTime.ParseExact(valor, formaterDateTime, CultureInfo.InvariantCulture).ToString(atributo.MascaraDisplay);
                        else
                            valorReturn = DateTime.ParseExact(valor, atributo.MascaraIngreso, CultureInfo.InvariantCulture).ToString(atributo.MascaraDisplay);
                    }
                    break;
                case TipoAtributoDb.HORA:

                    if (!string.IsNullOrEmpty(valor))
                    {
                        if (formatoDb)
                            valorReturn = DateTime.ParseExact(valor, CDateFormats.HORA_MINUTOS, CultureInfo.InvariantCulture).ToString(atributo.MascaraDisplay);
                        else
                            valorReturn = DateTime.ParseExact(valor, atributo.MascaraIngreso, CultureInfo.InvariantCulture).ToString(atributo.MascaraDisplay);
                    }

                    break;
                case TipoAtributoDb.TEXTO:
                    valorReturn = valor;
                    break;
                case TipoAtributoDb.DOMINIO:
                    valorReturn = valor;
                    break;
                case TipoAtributoDb.SISTEMA:
                    valorReturn = valor;
                    break;
            }

            return valorReturn;
        }

        public static string GetValorByIdTipo(IUnitOfWork uow, Atributo atributo, string valor, IFormatProvider culture)
        {
            var formaterDateTime = uow.ParametroRepository.GetParameter(ParamManager.DATETIME_FORMAT_DATE_SECONDS);
            var numberDecimalSeparator = uow.ParametroRepository.GetParameter(ParamManager.NUMBER_DECIMAL_SEPARATOR);

            var valorReturn = string.Empty;
            switch (atributo.IdTipo)
            {
                case TipoAtributoDb.NUMERICO:

                    if (!string.IsNullOrEmpty(atributo.Separador))
                        valorReturn = valor.Replace(atributo.Separador, numberDecimalSeparator);
                    else
                        valorReturn = valor;

                    break;
                case TipoAtributoDb.FECHA:

                    if (!string.IsNullOrEmpty(valor))
                        valorReturn = DateTime.ParseExact(valor, atributo.MascaraIngreso, CultureInfo.InvariantCulture).ToString(formaterDateTime);

                    break;
                case TipoAtributoDb.HORA:

                    if (!string.IsNullOrEmpty(valor))
                        valorReturn = DateTime.ParseExact(valor, atributo.MascaraIngreso, CultureInfo.InvariantCulture).ToString(CDateFormats.HORA_MINUTOS);

                    break;
                case TipoAtributoDb.TEXTO:
                    valorReturn = valor;
                    break;
                case TipoAtributoDb.DOMINIO:
                    valorReturn = valor;
                    break; 
                case TipoAtributoDb.SISTEMA:
                    valorReturn = valor;
                    break;
            }
            return valorReturn;
        }

    }
}
