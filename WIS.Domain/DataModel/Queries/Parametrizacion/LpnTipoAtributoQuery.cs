using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Parametrizacion
{

    public class LpnTipoAtributoQuery : QueryObject<V_LPN_TIPO_ATRIBUTO, WISDB>
    {
        protected string _TP_LPN_TIPO { get; }

        public LpnTipoAtributoQuery(string TP_LPN_TIPO)
        {
            _TP_LPN_TIPO = TP_LPN_TIPO;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_LPN_TIPO_ATRIBUTO.Where(x =>x.TP_LPN_TIPO == _TP_LPN_TIPO);
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}