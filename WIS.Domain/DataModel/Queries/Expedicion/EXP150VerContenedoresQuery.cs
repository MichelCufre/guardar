using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class EXP150VerContenedoresQuery : QueryObject<V_CONTENEDOR_WISEX150, WISDB>
    {
        private readonly string _Pedido = "";
        private readonly string _Cliente = "";
        private readonly int _Empresa = 0;

        public EXP150VerContenedoresQuery(string pedido, string cliente, int empresa)
        {
            _Pedido = pedido;
            _Cliente = cliente;
            _Empresa = empresa;
        }
        public EXP150VerContenedoresQuery()
        {

        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_CONTENEDOR_WISEX150.AsNoTracking();

            if (!String.IsNullOrEmpty(this._Pedido))
                this.Query = this.Query.Where(x => x.NU_PEDIDO == _Pedido && x.CD_CLIENTE == _Cliente && x.CD_EMPRESA == _Empresa);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
