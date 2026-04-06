using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Parametrizacion;
using WIS.Domain.Services;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.Validation
{
    public class AtributoValidation
    {
        public static void ValidacionAsociadaAtributos(IUnitOfWork uow, int idAtributo, string value, IFormatProvider culture, bool invocarAPICustom, out List<Error> errores)
        {
            errores = new List<Error>();
            var atributo = uow.AtributoRepository.GetAtributo(idAtributo);

            var listaAtributoValidacion = uow.AtributoRepository.GetValidacionesAsociada(idAtributo);
            foreach (var atributoValidacion in listaAtributoValidacion)
            {
                IAtributoValidacionService validationRole = GetValidationService(atributo.IdTipo);
                validationRole.Validar(uow, idAtributo, atributoValidacion.IdValidacion, value, atributoValidacion.Valor, culture, invocarAPICustom, out Error error);

                if (error != null)
                    errores.Add(error);
            }
        }

        public static void ValidacionAsociadaAtributos(IUnitOfWork uow, ILpnServiceContext context, Atributo atributo, string value, IFormatProvider culture, bool invocarAPICustom, List<Error> errores)
        {
            var validacionesAsociadas = context.GetValidacionesAsociadas(atributo.Id);
            foreach (var validacion in validacionesAsociadas)
            {
                IAtributoValidacionService validationRule = GetValidationService(atributo.IdTipo, atributo, context);
                validationRule.Validar(uow, atributo.Id, validacion.IdValidacion, value, validacion.Valor, culture, invocarAPICustom, out Error error);

                if (error != null)
                    errores.Add(error);
            }
        }

        public static bool ValidarFormatoTipo(ILpnServiceContext context, Atributo atributo, string value, IFormatProvider culture, List<Error> errores)
        {
            Error error = null;
            switch (atributo.IdTipo)
            {
                case TipoAtributoDb.NUMERICO:
                    if (!string.IsNullOrEmpty(atributo.Separador))
                    {
                        if (!ValidarTipoNumerico(value, (atributo.Largo ?? 0), (atributo.Decimales ?? 0), atributo.Separador, out error))
                            errores.Add(error);
                    }
                    break;
                case TipoAtributoDb.FECHA:
                    if (!ValidarTipoFecha(value, atributo.MascaraIngreso, culture, out error))
                        errores.Add(error);
                    break;
                case TipoAtributoDb.HORA:
                    if (!ValidarTipoHora(value, atributo.MascaraIngreso, culture, out error))
                        errores.Add(error);
                    break;
                case TipoAtributoDb.TEXTO:
                    if (!ValidarTipoTexto(value, DefaultDb.LARGO_MAXIMO_CHAR_LPN_ATRIBUTO, out error))
                        errores.Add(error);
                    break;
                case TipoAtributoDb.DOMINIO:
                    if (!context.ExisteDominio(atributo.CodigoDominio, value))
                        errores.Add(new Error("WMSAPI_msg_Error_ValorDominioIncorrecto"));
                    break;
                case TipoAtributoDb.SISTEMA:
                    errores.Add(new Error("WMSAPI_msg_Error_TipoAtributoNoHabilitadoParaApi"));
                    break;
            }

            return true;
        }
        public static bool ValidarTipoNumerico(string value, short largo, short presicion, string separador, out Error error)
        {
            error = null;
            string pattern = @"^-?[0-9]{0," + largo.ToString() + "}([" + separador + "][0-9]{0," + presicion.ToString() + "})?$"; // DECIMAL

            if (!string.IsNullOrEmpty(value))
            {
                if (!decimal.TryParse(value, out decimal outValue))
                    error = new Error("General_Sec0_Error_Error14");
                else
                {
                    if (!Regex.IsMatch(value, pattern))
                    {
                        if (string.IsNullOrEmpty(separador))
                            error = new Error("General_Sec0_Error_Error89", new string[] { largo.ToString(), presicion.ToString() });
                        else
                            error = new Error("General_Sec0_Error_Error39", new string[] { largo.ToString(), presicion.ToString(), separador });
                    }
                }
            }
            return error == null;
        }
        public static bool ValidarTipoFecha(string value, string formartoFecha, IFormatProvider culture, out Error error)
        {
            error = null;
            if (!DateTime.TryParseExact(value, formartoFecha, culture, DateTimeStyles.None, out DateTime dt))
                error = new Error("General_Sec0_Error_FormatoIncorrecto", formartoFecha);

            return error == null;
        }
        public static bool ValidarTipoHora(string value, string formartoHora, IFormatProvider culture, out Error error)
        {
            error = null;
            if (!DateTime.TryParseExact(value, formartoHora, culture, DateTimeStyles.None, out DateTime dt))
                error = new Error("General_Sec0_Error_HoraInvalida", formartoHora);

            return error == null;
        }
        public static bool ValidarTipoTexto(string value, int maxLength, out Error error)
        {
            error = null;
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Length > maxLength)
                    error = new Error("General_Sec0_Error_Error64", maxLength.ToString());
            }
            return error == null;
        }

        public static IAtributoValidacionService GetValidationService(string idTipo, Atributo atributo = null, ILpnServiceContext serviceContext = null)
        {
            switch (idTipo)
            {
                case TipoAtributoDb.NUMERICO:
                    return new NumeroAtributoValidacion(atributo, serviceContext);
                case TipoAtributoDb.FECHA:
                    return new FechaAtributoValidacion(atributo, serviceContext);
                case TipoAtributoDb.HORA:
                    return new HoraAtributoValidacion(atributo, serviceContext);
                case TipoAtributoDb.TEXTO:
                    return new TextoAtributoValidacion(atributo, serviceContext);
                case TipoAtributoDb.DOMINIO:
                    return new DominioAtributoValidacion(atributo, serviceContext);
                case TipoAtributoDb.SISTEMA:
                    return new SistemaAtributoValidacion(atributo, serviceContext);
                default: return null;
            }
        }
    }
}
