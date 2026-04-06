using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using WIS.Domain.CodigoMultidato.Constants;
using WIS.Domain.CodigoMultidato.Interfaces;
using WIS.Domain.DataModel;

namespace WIS.Domain.CodigoMultidato
{
    public class EAN128Handler : ICodigoMultidatoHandler
    {
        public virtual string GetCodigo()
        {
            return TipoCodigoMultidato.EAN128;
        }

        public virtual Dictionary<string, string> GetAIs(IUnitOfWork uow, string codigo)
        {
            if (!IsValid(uow, codigo, out Dictionary<string, string> ais))
            {
                return null;
            }

            return ais;
        }

        public virtual bool IsValid(IUnitOfWork uow, string codigo, out Dictionary<string, string> ais)
        {
            var regexPattern = uow.CodigoMultidatoRepository.GetCodigoMultidato(TipoCodigoMultidato.EAN128).Regex;

            ais = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(regexPattern))
                return false;
            
            var regex = new Regex(regexPattern);
            var match = regex.Match(codigo);

            if (match.Success)
            {
                foreach (var groupName in regex.GetGroupNames())
                {
                    if (groupName.StartsWith("ID"))
                    {
                        var ai = groupName.Replace("ID", string.Empty);
                        var value = match.Groups[groupName].Value;

                        if (!string.IsNullOrEmpty(value))
                        {
                            ais[ai] = value;
                        }
                    }
                }
            }

            return ais.Count > 0;
        }

        public virtual int? GetEmpresa(IUnitOfWork uow, int userId, Dictionary<string, string> ais, CodigoMultidatoTipoLectura tipoLectura, int? empresa)
        {
            var cantidadEmpresasLpn = 0;
            var cantidadEmpresasProducto = 0;

            if (tipoLectura != CodigoMultidatoTipoLectura.Producto && ais.ContainsKey(EAN128AI.SSCC))
            {
                var empresaLpn = uow.ManejoLpnRepository.GetFirstEmpresaWithCodigoBarrasMultidato(ais[EAN128AI.SSCC], userId, empresa, out cantidadEmpresasLpn);

                if (cantidadEmpresasLpn == 1)
                {
                    return empresaLpn;
                }
            }

            if (tipoLectura != CodigoMultidatoTipoLectura.LPN && ais.ContainsKey(EAN128AI.GTIN))
            {
                var empresaProducto = uow.CodigoBarrasRepository.GetFirstEmpresaWithCodigoBarrasMultidato(ais[EAN128AI.GTIN], userId, empresa, out cantidadEmpresasProducto);

                if (cantidadEmpresasProducto == 1)
                {
                    return empresaProducto;
                }
            }

            if (tipoLectura != CodigoMultidatoTipoLectura.Producto && cantidadEmpresasLpn > 1)
                throw new TooManyEmpresaCodigoMultidatoException(ais[EAN128AI.SSCC]);

            if (tipoLectura != CodigoMultidatoTipoLectura.LPN && cantidadEmpresasProducto > 1)
                throw new TooManyEmpresaCodigoMultidatoException(ais[EAN128AI.GTIN]);

            if (tipoLectura != CodigoMultidatoTipoLectura.Producto && cantidadEmpresasLpn == 0)
                throw new InvalidCodigoMultidatoException("General_Sec0_Error_CodigoMultidatoLpnInvalido", new string[] { ais[EAN128AI.SSCC] });

            if (tipoLectura != CodigoMultidatoTipoLectura.LPN && cantidadEmpresasProducto == 0)
                throw new InvalidCodigoMultidatoException("General_Sec0_Error_CodigoMultidatoProductoInvalido", new string[] { ais[EAN128AI.GTIN] });

            return null;
        }

        public virtual object GetAIValue(Dictionary<string, string> ais, Dictionary<string, string> aiTypes, Dictionary<string, decimal> aiConversions, string fnc1, string ai)
        {
            var aiType = aiTypes[ai];
            var aiConversion = (decimal?)null;

            if (aiConversions.ContainsKey(ai))
                aiConversion = aiConversions[ai];

            if (IsDynamicAI(ai))
            {
                var prefix = string.Join(string.Empty, ai.TakeWhile(c => Char.IsDigit(c)));
                var specificAI = ais.Keys.FirstOrDefault(x => x.StartsWith(prefix));

                if (!string.IsNullOrEmpty(specificAI))
                    return Format(ai, specificAI, aiType, aiConversion, fnc1, ais[specificAI]);
            }
            else if (ais.ContainsKey(ai))
            {
                return Format(ai, ai, aiType, aiConversion, fnc1, ais[ai]);
            }

            return null;
        }

        protected virtual object Format(string ai, string specificAI, string aiType, decimal? aiConversion, string fnc1, string value)
        {
            value = value.Replace(fnc1, "");

            switch (aiType)
            {
                case TipoAI.NUMBER:
                    return long.Parse(value);
                case TipoAI.DATE:
                    return DateTime.ParseExact(value, "yyMMdd", CultureInfo.InvariantCulture);
                case TipoAI.DATETIME:
                    return DateTime.ParseExact(value, "yyMMddHHmmss", CultureInfo.InvariantCulture);
                case TipoAI.DECIMAL:
                    var decimalCount = int.Parse(specificAI.Last().ToString());

                    if (decimalCount > 0)
                    {
                        value = value.Insert(value.Length - decimalCount, CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
                    }

                    return (aiConversion ?? 1) * decimal.Parse(value, CultureInfo.InvariantCulture);
                case TipoAI.TEXT:
                default:
                    return value;
            }
        }

        protected virtual bool IsDynamicAI(string ai)
        {
            return ai.Any(c => !Char.IsDigit(c));
        }
    }
}
