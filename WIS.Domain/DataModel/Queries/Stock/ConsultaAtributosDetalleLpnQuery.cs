using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;


namespace WIS.Domain.DataModel.Queries.Stock
{
    public class ConsultaAtributosDetalleLpnQuery : QueryObject<V_STO740_ATRIBUTO, WISDB>
    {
        protected Dictionary<int, string> _atributos;
        protected string _tipo;

        public ConsultaAtributosDetalleLpnQuery(string tipo)
        {
            _atributos = new Dictionary<int, string>();
            _tipo = tipo;
        }

        public ConsultaAtributosDetalleLpnQuery(string tipo, Dictionary<int, string> atributos)
        {
            _atributos = atributos;
            _tipo = tipo;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_STO740_LPN_DET_ATRIBUTO_DET
                .Where(d => d.ID_ATRIBUTO != -1
                    && (string.IsNullOrEmpty(_tipo) || d.TP_LPN_TIPO == _tipo))
                .GroupBy(v => new V_STO740_ATRIBUTO
                {
                    ID_ATRIBUTO = v.ID_ATRIBUTO,
                    NM_ATRIBUTO = v.NM_ATRIBUTO,
                })
                .Select(g => g.Key);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}