using System;
using System.Globalization;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;

namespace WIS.Domain.Parametrizacion
{
    public class FechaAtributoValidacion : IAtributoValidacionService
    {
        protected Atributo _atributo;
        protected ILpnServiceContext _serviceContext;

        public FechaAtributoValidacion(Atributo atributo = null, ILpnServiceContext serviceContext = null)
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
                case ValidacionAtributoDb.FechaMayor:

                    if (DateTime.TryParseExact(valorAValidar, atributo.MascaraIngreso, culture, DateTimeStyles.None, out valor) && DateTime.TryParseExact(strValorValidacion, formaterDateTime, culture, DateTimeStyles.None, out valorComparativo))
                    {
                        if (valor <= valorComparativo)
                            error = new Error(atributoValidacion.Error, new string[] { strValorValidacion });
                    }
                    else
                        error = new Error("General_Sec0_Error_FormatoIncorrecto", new string[] { formaterDateTime });

                    break;
                case ValidacionAtributoDb.FechaMenor:

                    if (DateTime.TryParseExact(valorAValidar, atributo.MascaraIngreso, culture, DateTimeStyles.None, out valor) && DateTime.TryParseExact(strValorValidacion, formaterDateTime, culture, DateTimeStyles.None, out valorComparativo))
                    {
                        if (valor >= valorComparativo)
                            error = new Error(atributoValidacion.Error, new string[] { strValorValidacion });
                    }
                    else
                        error = new Error("General_Sec0_Error_FormatoIncorrecto", new string[] { formaterDateTime });

                    break;
                case ValidacionAtributoDb.FechaAPICustom:

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
