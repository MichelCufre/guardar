using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Picking;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class AnulacionDePreparacionesQuery : QueryObject<V_PRE120_ANULACION_PREPARACION, WISDB>
    {
        public AnulacionDePreparacionesQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE120_ANULACION_PREPARACION.Select(d => d);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<DetallePreparacion> GetSelectedKeys(List<string> keysToSelect, IFormatProvider formatProvider)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[]
                {
                    r.CD_PRODUTO,
                    r.CD_EMPRESA.ToString(),
                    r.NU_IDENTIFICADOR,
                    r.CD_FAIXA.ToString(formatProvider),
                    r.NU_PREPARACION.ToString(),
                    r.NU_PEDIDO,
                    r.CD_CLIENTE,
                    r.CD_ENDERECO,
                    r.NU_SEQ_PREPARACION.ToString() }))
                .Intersect(keysToSelect)
                .Select(w => SelectionQuery(w, formatProvider))
                .ToList();
        }

        public virtual List<DetallePreparacion> GetSelectedKeysAndExclude(List<string> keysToExclude, IFormatProvider formatProvider)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[]
                {
                    r.CD_PRODUTO,
                    r.CD_EMPRESA.ToString(),
                    r.NU_IDENTIFICADOR,
                    r.CD_FAIXA.ToString(formatProvider),
                    r.NU_PREPARACION.ToString(),
                    r.NU_PEDIDO,
                    r.CD_CLIENTE,
                    r.CD_ENDERECO,
                    r.NU_SEQ_PREPARACION.ToString() }))
                .Except(keysToExclude)
                .Select(w => SelectionQuery(w, formatProvider))
                .ToList();
        }

        public virtual DetallePreparacion SelectionQuery(string key, IFormatProvider formatProvider)
        {
            var keys = key.Split('$');
            return new DetallePreparacion()
            {
                Producto = keys[0],
                Empresa = int.Parse(keys[1]),
                Lote = keys[2],
                Faixa = decimal.Parse(keys[3], formatProvider),
                NumeroPreparacion = int.Parse(keys[4]),
                Pedido = keys[5],
                Cliente = keys[6],
                Ubicacion = keys[7],
                NumeroSecuencia = int.Parse(keys[8])
            };
        }
    }
}
