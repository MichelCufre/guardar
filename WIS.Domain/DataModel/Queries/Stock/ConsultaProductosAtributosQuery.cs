using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Exceptions;
using WIS.Persistence.Database;


namespace WIS.Domain.DataModel.Queries.Stock
{
    public class ConsultaProductosAtributosQuery : QueryObject<V_STO740_LPN_DET, WISDB>
    {
        protected Dictionary<int, string> _atributosCabezal;
        protected Dictionary<int, string> _atributosDetalle;
        protected string _tipoLpn;
        protected string _numberDecimalSeparator;
        protected string _datetimeFormatDate;
        protected string _datetimeFormatDateHours;
        protected string _datetimeFormatDateSeconds;
        protected Dictionary<int, string> _tiposAtributos;

        public ConsultaProductosAtributosQuery(string tipoLpn)
        {
            _tipoLpn = tipoLpn;
            _atributosCabezal = new Dictionary<int, string>();
            _atributosDetalle = new Dictionary<int, string>();
            _tiposAtributos = new Dictionary<int, string>();
        }

        public ConsultaProductosAtributosQuery(string tipoLpn, Dictionary<int, string> atributosCabezal, Dictionary<int, string> atributosDetalle)
        {
            _tipoLpn = tipoLpn;
            _atributosCabezal = atributosCabezal;
            _atributosDetalle = atributosDetalle;
            _tiposAtributos = new Dictionary<int, string>();
        }

        public override void BuildQuery(WISDB context)
        {
            var query = context.T_LPN_DET
                .Include("T_LPN")
                .Where(v => string.IsNullOrEmpty(_tipoLpn) || v.T_LPN.TP_LPN_TIPO == _tipoLpn);

            _numberDecimalSeparator = context.T_LPARAMETRO_CONFIGURACION
                .AsNoTracking()
                .Where(x => x.CD_PARAMETRO == ParamManager.NUMBER_DECIMAL_SEPARATOR
                    && x.ND_ENTIDAD == ParamManager.PARAM_GRAL
                    && x.DO_ENTIDAD_PARAMETRIZABLE == ParamManager.PARAM_GRAL)
                .FirstOrDefault().VL_PARAMETRO;

            _datetimeFormatDate = context.T_LPARAMETRO_CONFIGURACION
                .AsNoTracking()
                .Where(x => x.CD_PARAMETRO == ParamManager.DATETIME_FORMAT_DATE
                    && x.ND_ENTIDAD == ParamManager.PARAM_GRAL
                    && x.DO_ENTIDAD_PARAMETRIZABLE == ParamManager.PARAM_GRAL)
                .FirstOrDefault().VL_PARAMETRO;

            _datetimeFormatDateHours = context.T_LPARAMETRO_CONFIGURACION
                .AsNoTracking()
                .Where(x => x.CD_PARAMETRO == ParamManager.DATETIME_FORMAT_DATE_HOURS
                    && x.ND_ENTIDAD == ParamManager.PARAM_GRAL
                    && x.DO_ENTIDAD_PARAMETRIZABLE == ParamManager.PARAM_GRAL)
                .FirstOrDefault().VL_PARAMETRO;

            _datetimeFormatDateSeconds = context.T_LPARAMETRO_CONFIGURACION
                .AsNoTracking()
                .Where(x => x.CD_PARAMETRO == ParamManager.DATETIME_FORMAT_DATE_SECONDS
                    && x.ND_ENTIDAD == ParamManager.PARAM_GRAL
                    && x.DO_ENTIDAD_PARAMETRIZABLE == ParamManager.PARAM_GRAL)
                .FirstOrDefault().VL_PARAMETRO;

            foreach (var atributo in context.T_ATRIBUTO
                .AsNoTracking()
                .Where(x => _atributosCabezal.Keys.Contains(x.ID_ATRIBUTO)
                    || _atributosDetalle.Keys.Contains(x.ID_ATRIBUTO)))
            {
                _tiposAtributos[atributo.ID_ATRIBUTO] = atributo.ID_ATRIBUTO_TIPO;
            };

            foreach (var idAtributo in _atributosCabezal.Keys)
            {
                var valor = _atributosCabezal[idAtributo];
                var queryDetalleAtributo = query
                    .Join(context.V_STO740_LPN_DET_ATRIBUTO_CAB,
                        d => new { d.ID_LPN_DET, d.NU_LPN, d.CD_PRODUTO, d.CD_FAIXA, d.CD_EMPRESA, d.NU_IDENTIFICADOR },
                        v => new { v.ID_LPN_DET, v.NU_LPN, v.CD_PRODUTO, v.CD_FAIXA, v.CD_EMPRESA, v.NU_IDENTIFICADOR, },
                        (d, v) => new { DetalleLpn = d, Atributo = v })
                    .Where(da => da.Atributo.ID_ATRIBUTO == idAtributo);

                var isNegation = false;

                if (valor.StartsWith("!"))
                {
                    valor = valor.Substring(1);
                    isNegation = true;
                }

                if (valor.StartsWith("="))
                {
                    string[] args;

                    if (valor.ToUpper().StartsWith("=BETWEEN(")
                        && valor.EndsWith(")")
                        && TryParseFilterArgs(valor, "=BETWEEN(", 2, 2, out args))
                    {
                        var bottom = NormalizeFilter(idAtributo, args[0]);
                        var top = NormalizeFilter(idAtributo, args[1]);

                        if (isNegation)
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => !(da.Atributo.VL_NORMALIZADO.CompareTo(bottom) >= 0 && da.Atributo.VL_NORMALIZADO.CompareTo(top) <= 0));
                        }
                        else
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => da.Atributo.VL_NORMALIZADO.CompareTo(bottom) >= 0 && da.Atributo.VL_NORMALIZADO.CompareTo(top) <= 0);
                        }
                    }
                    else if (valor.ToUpper().StartsWith("=IN(")
                        && valor.EndsWith(")")
                        && TryParseFilterArgs(valor, "=IN(", 1, null, out args))
                    {
                        for (int i = 0; i < args.Length; i++)
                        {
                            args[i] = NormalizeFilter(idAtributo, args[i]);
                        }

                        if (isNegation)
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => !args.Contains(da.Atributo.VL_NORMALIZADO));
                        }
                        else
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => args.Contains(da.Atributo.VL_NORMALIZADO));
                        }
                    }
                    else
                    {
                        valor = NormalizeFilter(idAtributo, valor.Substring(1));

                        if (isNegation)
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => !(da.Atributo.VL_NORMALIZADO == valor));
                        }
                        else
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => da.Atributo.VL_NORMALIZADO == valor);
                        }
                    }
                }
                else if (valor.StartsWith(">"))
                {
                    if (valor.StartsWith(">="))
                    {
                        valor = NormalizeFilter(idAtributo, valor.Substring(2));

                        if (isNegation)
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => !(da.Atributo.VL_NORMALIZADO.CompareTo(valor) >= 0));
                        }
                        else
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => da.Atributo.VL_NORMALIZADO.CompareTo(valor) >= 0);
                        }
                    }
                    else
                    {
                        valor = NormalizeFilter(idAtributo, valor.Substring(1));

                        if (isNegation)
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => !(da.Atributo.VL_NORMALIZADO.CompareTo(valor) > 0));
                        }
                        else
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => da.Atributo.VL_NORMALIZADO.CompareTo(valor) > 0);
                        }
                    }
                }
                else if (valor.StartsWith("<"))
                {
                    if (valor.StartsWith("<="))
                    {
                        valor = NormalizeFilter(idAtributo, valor.Substring(2));

                        if (isNegation)
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => !(da.Atributo.VL_NORMALIZADO.CompareTo(valor) <= 0));
                        }
                        else
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => da.Atributo.VL_NORMALIZADO.CompareTo(valor) <= 0);
                        }
                    }
                    else
                    {
                        valor = NormalizeFilter(idAtributo, valor.Substring(1));

                        if (isNegation)
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => !(da.Atributo.VL_NORMALIZADO.CompareTo(valor) < 0));
                        }
                        else
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => da.Atributo.VL_NORMALIZADO.CompareTo(valor) < 0);
                        }
                    }
                }
                else
                {
                    valor = NormalizeFilter(idAtributo, valor);

                    if (isNegation)
                    {
                        queryDetalleAtributo = queryDetalleAtributo
                           .Where(da => !EF.Functions.Like(da.Atributo.VL_NORMALIZADO, valor));
                    }
                    else
                    {
                        queryDetalleAtributo = queryDetalleAtributo
                            .Where(da => EF.Functions.Like(da.Atributo.VL_NORMALIZADO, valor));
                    }
                }

                query = queryDetalleAtributo.Select(da => da.DetalleLpn);
            }

            foreach (var idAtributo in _atributosDetalle.Keys)
            {
                var valor = _atributosDetalle[idAtributo];
                var queryDetalleAtributo = query
                    .Join(context.V_STO740_LPN_DET_ATRIBUTO_DET,
                        d => new { d.ID_LPN_DET, d.NU_LPN, d.CD_PRODUTO, d.CD_FAIXA, d.CD_EMPRESA, d.NU_IDENTIFICADOR },
                        v => new { v.ID_LPN_DET, v.NU_LPN, v.CD_PRODUTO, v.CD_FAIXA, v.CD_EMPRESA, v.NU_IDENTIFICADOR, },
                        (d, v) => new { DetalleLpn = d, Atributo = v })
                    .Where(da => da.Atributo.ID_ATRIBUTO == idAtributo);

                var isNegation = false;

                if (valor.StartsWith("!"))
                {
                    valor = valor.Substring(1);
                    isNegation = true;
                }

                if (valor.StartsWith("="))
                {
                    string[] args;

                    if (valor.ToUpper().StartsWith("=BETWEEN(")
                        && valor.EndsWith(")")
                        && TryParseFilterArgs(valor, "=BETWEEN(", 2, 2, out args))
                    {
                        var bottom = NormalizeFilter(idAtributo, args[0]);
                        var top = NormalizeFilter(idAtributo, args[1]);

                        if (isNegation)
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => !(da.Atributo.VL_NORMALIZADO.CompareTo(bottom) >= 0 && da.Atributo.VL_NORMALIZADO.CompareTo(top) <= 0));
                        }
                        else
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => da.Atributo.VL_NORMALIZADO.CompareTo(bottom) >= 0 && da.Atributo.VL_NORMALIZADO.CompareTo(top) <= 0);
                        }
                    }
                    else if (valor.ToUpper().StartsWith("=IN(")
                        && valor.EndsWith(")")
                        && TryParseFilterArgs(valor, "=IN(", 1, null, out args))
                    {
                        for (int i = 0; i < args.Length; i++)
                        {
                            args[i] = NormalizeFilter(idAtributo, args[i]);
                        }

                        if (isNegation)
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => !args.Contains(da.Atributo.VL_NORMALIZADO));
                        }
                        else
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => args.Contains(da.Atributo.VL_NORMALIZADO));
                        }
                    }
                    else
                    {
                        valor = NormalizeFilter(idAtributo, valor.Substring(1));

                        if (isNegation)
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => !(da.Atributo.VL_NORMALIZADO == valor));
                        }
                        else
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => da.Atributo.VL_NORMALIZADO == valor);
                        }
                    }
                }
                else if (valor.StartsWith(">"))
                {
                    if (valor.StartsWith(">="))
                    {
                        valor = NormalizeFilter(idAtributo, valor.Substring(2));

                        if (isNegation)
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => !(da.Atributo.VL_NORMALIZADO.CompareTo(valor) >= 0));
                        }
                        else
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => da.Atributo.VL_NORMALIZADO.CompareTo(valor) >= 0);
                        }
                    }
                    else
                    {
                        valor = NormalizeFilter(idAtributo, valor.Substring(1));

                        if (isNegation)
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => !(da.Atributo.VL_NORMALIZADO.CompareTo(valor) > 0));
                        }
                        else
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => da.Atributo.VL_NORMALIZADO.CompareTo(valor) > 0);
                        }
                    }
                }
                else if (valor.StartsWith("<"))
                {
                    if (valor.StartsWith("<="))
                    {
                        valor = NormalizeFilter(idAtributo, valor.Substring(2));

                        if (isNegation)
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => !(da.Atributo.VL_NORMALIZADO.CompareTo(valor) <= 0));
                        }
                        else
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => da.Atributo.VL_NORMALIZADO.CompareTo(valor) <= 0);
                        }
                    }
                    else
                    {
                        valor = NormalizeFilter(idAtributo, valor.Substring(1));

                        if (isNegation)
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => !(da.Atributo.VL_NORMALIZADO.CompareTo(valor) < 0));
                        }
                        else
                        {
                            queryDetalleAtributo = queryDetalleAtributo
                                .Where(da => da.Atributo.VL_NORMALIZADO.CompareTo(valor) < 0);
                        }
                    }
                }
                else
                {
                    valor = NormalizeFilter(idAtributo, valor);

                    if (isNegation)
                    {
                        queryDetalleAtributo = queryDetalleAtributo
                           .Where(da => !EF.Functions.Like(da.Atributo.VL_NORMALIZADO, valor));
                    }
                    else
                    {
                        queryDetalleAtributo = queryDetalleAtributo
                            .Where(da => EF.Functions.Like(da.Atributo.VL_NORMALIZADO, valor));
                    }
                }

                query = queryDetalleAtributo
                    .Select(da => da.DetalleLpn);
            }

            query = query
                .GroupBy(d => new { d.ID_LPN_DET, d.NU_LPN, d.CD_PRODUTO, d.CD_FAIXA, d.CD_EMPRESA, d.NU_IDENTIFICADOR })
                .Select(g => g.Key)
                .Join(context.T_LPN_DET,
                    k => new { k.ID_LPN_DET, k.NU_LPN, k.CD_PRODUTO, k.CD_FAIXA, k.CD_EMPRESA, k.NU_IDENTIFICADOR },
                    d => new { d.ID_LPN_DET, d.NU_LPN, d.CD_PRODUTO, d.CD_FAIXA, d.CD_EMPRESA, d.NU_IDENTIFICADOR },
                    (k, d) => d);

            this.Query = query
                .Join(context.T_LPN,
                    d => d.NU_LPN,
                    l => l.NU_LPN,
                    (d, l) => new { Lpn = l, DetalleLpn = d })
                .Join(context.T_LPN_TIPO,
                    dl => dl.Lpn.TP_LPN_TIPO,
                    lt => lt.TP_LPN_TIPO,
                    (dl, lt) => new { dl.Lpn, dl.DetalleLpn, TipoLpn = lt })
                .Join(context.T_EMPRESA,
                    dllt => dllt.DetalleLpn.CD_EMPRESA,
                    e => e.CD_EMPRESA,
                    (dllt, e) => new { dllt.Lpn, dllt.DetalleLpn, dllt.TipoLpn, Empresa = e })
                .Join(context.T_PRODUTO,
                    dllte => new { dllte.DetalleLpn.CD_EMPRESA, dllte.DetalleLpn.CD_PRODUTO },
                    p => new { p.CD_EMPRESA, p.CD_PRODUTO },
                    (dllte, p) => new { dllte.Lpn, dllte.DetalleLpn, dllte.TipoLpn, dllte.Empresa, Producto = p })
                .Select(dlltep => new V_STO740_LPN_DET
                {
                    NU_LPN = dlltep.Lpn.NU_LPN,
                    ID_LPN_EXTERNO = dlltep.Lpn.ID_LPN_EXTERNO,
                    TP_LPN_TIPO = dlltep.Lpn.TP_LPN_TIPO,
                    NM_LPN_TIPO = dlltep.Lpn.T_LPN_TIPO.NM_LPN_TIPO,
                    NU_AGENDA = dlltep.Lpn.NU_AGENDA,
                    CD_ENDERECO = dlltep.Lpn.CD_ENDERECO,
                    DT_ACTIVACION = dlltep.Lpn.DT_ACTIVACION,
                    DT_ADDROW = dlltep.Lpn.DT_ADDROW,
                    DT_FIN = dlltep.Lpn.DT_FIN,
                    DT_UPDROW = dlltep.Lpn.DT_UPDROW,
                    ID_ESTADO = dlltep.Lpn.ID_ESTADO,
                    ID_PACKING = dlltep.Lpn.ID_PACKING,
                    ID_LPN_DET = dlltep.DetalleLpn.ID_LPN_DET,
                    ID_LINEA_SISTEMA_EXTERNO = dlltep.DetalleLpn.ID_LINEA_SISTEMA_EXTERNO,
                    CD_PRODUTO = dlltep.DetalleLpn.CD_PRODUTO,
                    DS_PRODUTO = dlltep.Producto.DS_PRODUTO,
                    CD_FAIXA = dlltep.DetalleLpn.CD_FAIXA,
                    CD_EMPRESA = dlltep.DetalleLpn.CD_EMPRESA,
                    NM_EMPRESA = dlltep.Empresa.NM_EMPRESA,
                    NU_IDENTIFICADOR = dlltep.DetalleLpn.NU_IDENTIFICADOR,
                    DT_FABRICACAO = dlltep.DetalleLpn.DT_FABRICACAO,
                    QT_ESTOQUE = dlltep.DetalleLpn.QT_ESTOQUE,
                    QT_DECLARADA = dlltep.DetalleLpn.QT_DECLARADA,
                    QT_RECIBIDA = dlltep.DetalleLpn.QT_RECIBIDA,
                    QT_RESERVA_SAIDA = dlltep.DetalleLpn.QT_RESERVA_SAIDA,
                    QT_EXPEDIDA = dlltep.DetalleLpn.QT_EXPEDIDA,
                    ID_AVERIA = dlltep.DetalleLpn.ID_AVERIA,
                    ID_INVENTARIO = dlltep.DetalleLpn.ID_INVENTARIO,
                    ID_CTRL_CALIDAD = dlltep.DetalleLpn.ID_CTRL_CALIDAD,
                });
        }

        protected virtual bool TryParseFilterArgs(string filter, string prefix, int? minCount, int? maxCount, out string[] args)
        {
            args = filter.Substring(prefix.Length, filter.Length - prefix.Length - 1).Split(';', StringSplitOptions.RemoveEmptyEntries);

            return (!minCount.HasValue || args.Length >= minCount) && (!maxCount.HasValue || args.Length <= maxCount);
        }

        protected virtual string NormalizeFilter(int idAtributo, string value)
        {
            var type = _tiposAtributos[idAtributo];

            switch (type)
            {
                case TipoAtributoDb.NUMERICO:

                    var args = value.Split(_numberDecimalSeparator, StringSplitOptions.RemoveEmptyEntries);

                    if (args.Count() == 2)
                    {
                        value = args[0].PadLeft(31, '0') + "." + args[1].PadRight(31, '0');
                    }
                    else if (args.Count() == 1)
                    {
                        value = args[0].PadLeft(31, '0') + "." + string.Empty.PadRight(31, '0');
                    } 
                    else 
                    {
                        throw new InvalidFilterException("General_Sec0_Error_FormatoIncorrecto", new string[] { $"{string.Empty.PadRight(31, '#')}{_numberDecimalSeparator}{string.Empty.PadRight(31, '#')}" });
                    }

                    break;

                case TipoAtributoDb.FECHA:

                    DateTime dateTime;

                    if (DateTime.TryParseExact(value, _datetimeFormatDateSeconds, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                    {
                        value = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else if (DateTime.TryParseExact(value, _datetimeFormatDateHours, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                    {
                        value = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else if (DateTime.TryParseExact(value, _datetimeFormatDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                    {
                        value = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else
                    {
                        throw new InvalidFilterException("General_Sec1_Error_FormatoIncorrecto", new string[] { $"{_datetimeFormatDate}, {_datetimeFormatDateHours}, {_datetimeFormatDateSeconds}" });
                    }

                    break;
            }

            return value;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}