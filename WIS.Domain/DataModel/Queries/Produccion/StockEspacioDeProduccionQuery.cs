using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
    public class StockEspacioDeProduccionQuery : QueryObject<V_PRD111_STOCK_PRODUCCION, WISDB>
    {
        protected readonly string _codigoEspacioProduccion;

        public StockEspacioDeProduccionQuery(string codigoEspacioProduccion)
        {
            _codigoEspacioProduccion = codigoEspacioProduccion;
        }

        public override void BuildQuery(WISDB context)
        {
            Query = context.V_PRD111_STOCK_PRODUCCION.Where(w => w.CD_PRDC_LINEA == _codigoEspacioProduccion);
        }

        public virtual int GetCount()
        {
            if (Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return Query.Count();
        }

        public List<StockEntities.Stock> GetSelectedKeys(List<string> keysToSelect, IFormatProvider formatProvider, long nuTransaccion)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[]
                {
                    r.CD_ENDERECO,
                    r.CD_EMPRESA.ToString(),
                    r.CD_PRODUTO,
                    r.CD_FAIXA.ToString(formatProvider),
                    r.NU_IDENTIFICADOR}))
                .Intersect(keysToSelect)
                .Select(w => SelectionQuery(w, formatProvider, nuTransaccion))
                .ToList();
        }

        public List<StockEntities.Stock> GetSelectedKeysAndExclude(List<string> keysToExclude, IFormatProvider formatProvider, long nuTransaccion)
        {
            var keyExcluir = this.Query
                .Where(s => s.QT_RESERVA_SAIDA > 0)
                .Select(r => string.Join("$", new string[]
                {
                    r.CD_ENDERECO,
                    r.CD_EMPRESA.ToString(),
                    r.CD_PRODUTO,
                    r.CD_FAIXA.ToString(formatProvider),
                    r.NU_IDENTIFICADOR
                }))
                .ToList();

            keysToExclude.AddRange(keyExcluir);
            keysToExclude = keysToExclude.Distinct().ToList();

            return this.GetResult()
                .Select(r => string.Join("$", new string[]
                {
                    r.CD_ENDERECO,
                    r.CD_EMPRESA.ToString(),
                    r.CD_PRODUTO,
                    r.CD_FAIXA.ToString(formatProvider),
                    r.NU_IDENTIFICADOR
                }))
                .Except(keysToExclude)
                .Select(w => SelectionQuery(w, formatProvider, nuTransaccion))
                .ToList();
        }

        public StockEntities.Stock SelectionQuery(string key, IFormatProvider formatProvider, long nuTransaccion)
        {
            var keys = key.Split('$');
            return new StockEntities.Stock()
            {
                Ubicacion = keys[0],
                Empresa = int.Parse(keys[1]),
                Producto = keys[2],
                Faixa = decimal.Parse(keys[3], formatProvider),
                Identificador = keys[4],
                NumeroTransaccion = nuTransaccion,
            };
        }
    }
}
