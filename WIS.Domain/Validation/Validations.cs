using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Enums;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Interfaces;
using WIS.Extension;

namespace WIS.Domain.Validation
{
    public class Validations
    {
        /// <summary>
        /// Comprueba si un codigo GLN (Global Location Number) es válido. Esto se cumple si el último digito es correcto para el resto de los numeros del código.
        /// </summary>
        /// <param name="value">Valor a comprobar</param>
        /// <returns>True si el digito verificador es correcto, false de lo contrario</returns>
        public static bool IsValidGLN(long value)
        {
            string strValue = Convert.ToString(value);
            int checkDigit = Convert.ToInt32(strValue.Last().ToString());
            int totalValue = 0;

            strValue = strValue.Substring(0, strValue.Length - 1);

            bool even = false;

            //Generar el total sumando los digitos. Los digitos de ubicación par se multiplican por 3
            foreach (char c in strValue)
            {
                int charVal = Convert.ToInt32(c.ToString());

                charVal = (even ? 3 * charVal : charVal);

                totalValue = totalValue + charVal;

                even = !even;
            }

            //Restar mayor multiplo de 10 mas cercano al total

            int diff = ((int)(Math.Ceiling(totalValue / 10d) * 10)) - totalValue;

            return diff == checkDigit;
        }


        public static bool TryParse_Decimal(string value, int largo, int parte_decimal, string separador, IFormatProvider provider, out decimal outValue, out string msg)
        {
            msg = string.Empty;
            outValue = 0;

            bool aux = false;

            int enteros = largo - parte_decimal;
            //string pattern1 = @"^[0-9]{0,9}(\,[0-9]{0,3})?$"; // DECIMAL
            string pattern = @"^(-)?[0-9]{0," + enteros + "}(\\" + separador + "[0-9]{0," + parte_decimal + "})?$"; // DECIMAL

            if (string.IsNullOrEmpty(value))
                msg = "Valor vacio";

            value = value.Replace(',', separador.ToCharArray().FirstOrDefault());
            value = value.Replace('.', separador.ToCharArray().FirstOrDefault());

            aux = Regex.IsMatch(value, pattern);
            if (!aux)
                msg = "Formato incorrecto.";
            if (aux)
            {
                aux = true;
                try
                {
                    if (!decimal.TryParse(value, NumberStyles.Any, provider, out outValue))
                        aux = false;
                }
                catch (Exception ex)
                {
                    aux = false;
                }

                if (!aux)
                    msg = "Error en conversión";
            }
            return aux;
        }

        public static bool ValidarCaracteres(string field, string value, string caracteresPermitidos, out Error error)
        {
            error = null;

            string caracteresNoPermitidos = string.Empty;
            Char[] arrCaracteres = caracteresPermitidos.ToCharArray();

            for (int i = 0; i < value.Length; i++)
            {
                if (!arrCaracteres.Contains(value[i]))
                    caracteresNoPermitidos += value[i] + " ";
            }

            if (!string.IsNullOrEmpty(caracteresNoPermitidos) && caracteresNoPermitidos.Length == 2)
                error = new Error("WMSAPI_msg_Error_NoAdmiteCaracter", $"{field} / {value}", caracteresNoPermitidos);
            else if (!string.IsNullOrEmpty(caracteresNoPermitidos))
                error = new Error("WMSAPI_msg_Error_NoAdmiteLosCaracteres", $"{field} / {value}", caracteresNoPermitidos);

            if (error != null)
                return false;

            return true;
        }

        public static long GuardarError(IUnitOfWork uow, string loginName, int? empresa, string idRequest, int cdIntExterna, string ds_referencia, object request, List<string> errores, string archivo = null, bool isAutomatismo = false)
        {
            if (isAutomatismo)
                return GuardarErrorEjecucionAutomatismo(uow, loginName, cdIntExterna, ds_referencia, request, errores);
            else
                return GuardarErrorEjecucion(uow, loginName, empresa, idRequest, cdIntExterna, ds_referencia, request, errores, archivo);
        }

        public static long GuardarErrorEjecucion(IUnitOfWork uow, string loginName, int? empresa, string idRequest, int cdIntExterna, string ds_referencia, object request, List<string> errores, string archivo)
        {
            var user = uow.SecurityRepository.GetUsuario(loginName);

            var interfaz = new InterfazEjecucion
            {
                CdInterfazExterna = cdIntExterna,
                Archivo = archivo,
                Situacion = SituacionDb.ProcesadoConError,
                Comienzo = DateTime.Now,
                FechaSituacion = DateTime.Now,
                ErrorCarga = "S",
                ErrorProcedimiento = "N",
                Referencia = ds_referencia.Truncate(200),
                Empresa = empresa,
                IdRequest = idRequest,
                GrupoConsulta = uow.ParametroRepository.GetParametro(ParamManager.GRUPO_CONSULTA, ParamManager.PARAM_EMPR, empresa?.ToString()).Result ?? "S/N",
                UserId = user?.UserId
            };

            var taskEjecucion = uow.EjecucionRepository.AddEjecucion(interfaz);

            taskEjecucion.Wait();

            interfaz = taskEjecucion.Result;

            var itfzData = new InterfazData
            {
                Id = interfaz.Id,
                Alta = DateTime.Now,
                Data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request))
            };

            var taskEjecucionData = uow.EjecucionRepository.AddEjecucionData(itfzData);

            taskEjecucionData.Wait();

            itfzData = taskEjecucionData.Result;

            int nroRegistro = 1;

            foreach (var error in errores)
            {
                var itfzError = new InterfazError
                {
                    Id = interfaz.Id,
                    NroError = nroRegistro,
                    Registro = 1,
                    Referencia = ds_referencia.Truncate(200),
                    Descripcion = error
                };

                var taskEjecucionError = uow.EjecucionRepository.AddEjecucionError(itfzError);

                taskEjecucionError.Wait();

                itfzError = taskEjecucionError.Result;

                nroRegistro++;
            }

            return interfaz.Id;
        }

        public static long GuardarErrorEjecucionAutomatismo(IUnitOfWork uow, string loginName, int cdIntExterna, string ds_referencia, object request, List<string> errores)
        {
            var ejecucion = new AutomatismoEjecucion();

            ejecucion.Referencia = ds_referencia;
            ejecucion.InterfazExterna = cdIntExterna;
            ejecucion.IdAutomatismo = null;
            ejecucion.IdentityUser = loginName;
            ejecucion.FechaRegistro = DateTime.Now;
            ejecucion.AutomatismoInterfaz = null;
            ejecucion.Id = uow.AutomatismoEjecucionRepository.GetNextNuAutomatismoEjecucion();
            ejecucion.UserId = uow.SecurityRepository.GetUserIdByLoginName(loginName) ?? 0;
            ejecucion.Estado = EstadoEjecucion.PROCESADO_ERROR_API;

            uow.AutomatismoEjecucionRepository.Add(ejecucion);
            uow.SaveChanges();

            ejecucion.AddData(request, new ValidationsResult
            {
                Errors = new List<ValidationsError>
                {
                    new ValidationsError
                    {
                        Messages = errores
                    }
                }
            });

            uow.AutomatismoEjecucionRepository.Update(ejecucion);
            uow.SaveChanges();

            return ejecucion.Id;
        }
    }
}
