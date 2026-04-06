using System;
using System.Globalization;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;

namespace WIS.Domain.Parametrizacion
{
    public class HoraAtributoValidacion : IAtributoValidacionService
    {
        protected Atributo _atributo;
        protected ILpnServiceContext _serviceContext;

        public HoraAtributoValidacion(Atributo atributo = null, ILpnServiceContext serviceContext = null)
        {
            _atributo = atributo;
            _serviceContext = serviceContext;
        }

        public virtual void Validar(IUnitOfWork uow, int idAtributo, short idValidacion, string valorAValidar, string strValorValidacion, IFormatProvider culture, bool invocarAPICustom, out Error error)
        {
            error = null;
            DateTime valor;
            DateTime valorComparativo;

            Atributo atributo;
            string formaterDateTime;
            AtributoValidacion atributoValidacion;

            if (_serviceContext != null)
            {
                atributo = _atributo;
                atributoValidacion = _serviceContext.GetAtributoValidacion(idValidacion);
                formaterDateTime = _serviceContext.GetParametro(ParamManager.DATETIME_FORMAT_DATE_SECONDS);
            }
            else
            {
                atributo = uow.AtributoRepository.GetAtributo(idAtributo);
                atributoValidacion = uow.AtributoRepository.GetAtributoValidacion(idValidacion);
                formaterDateTime = uow.ParametroRepository.GetParameter(ParamManager.DATETIME_FORMAT_DATE_SECONDS);
            }

            if (string.IsNullOrEmpty(atributoValidacion.Error))
                atributoValidacion.Error = "General_lbl_Title_ErrorValidacion";

            switch (idValidacion)
            {
                case ValidacionAtributoDb.HoraMayor:

                    if (DateTime.TryParseExact(valorAValidar, atributo.MascaraIngreso, CultureInfo.InvariantCulture, DateTimeStyles.None, out valor) && DateTime.TryParseExact(strValorValidacion, CDateFormats.HORA_MINUTOS, CultureInfo.InvariantCulture, DateTimeStyles.None, out valorComparativo))
                    {
                        if (valor <= valorComparativo)
                            error = new Error(atributoValidacion.Error, new string[] { strValorValidacion });
                    }
                    else
                        error = new Error("General_Sec0_Error_FormatoIncorrecto", new string[] { CDateFormats.HORA_MINUTOS });

                    break;
                case ValidacionAtributoDb.HoraMenor:

                    if (DateTime.TryParseExact(valorAValidar, atributo.MascaraIngreso, CultureInfo.InvariantCulture, DateTimeStyles.None, out valor) && DateTime.TryParseExact(strValorValidacion, CDateFormats.HORA_MINUTOS, CultureInfo.InvariantCulture, DateTimeStyles.None, out valorComparativo))
                    {
                        if (valor >= valorComparativo)
                            error = new Error(atributoValidacion.Error, new string[] { strValorValidacion });
                    }
                    else
                        error = new Error("General_Sec0_Error_FormatoIncorrecto", new string[] { CDateFormats.HORA_MINUTOS });

                    break;
                case ValidacionAtributoDb.HoraAPICustom:

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
