using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Picking;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class PendienteLiberacionQuery : QueryObject<V_PRE050_PEND_LIB, WISDB>
    {
        protected readonly int? _idEmpresa;
        protected readonly short? _onda;
        protected readonly string _predio;

        public PendienteLiberacionQuery(int? empresa, short? onda, string predio)
        {
            this._idEmpresa = empresa;
            this._onda = onda;
            this._predio = string.IsNullOrEmpty(predio) ? null : predio;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE050_PEND_LIB.AsNoTracking();

            if (this._idEmpresa != null && this._onda != null)
            {
                this.Query = this.Query.Where(x => x.CD_EMPRESA == this._idEmpresa
                                                && x.CD_ONDA == this._onda
                                                && x.TP_PEDIDO != TipoPedidoDb.Wismac
                                                && x.TP_PEDIDO != TipoPedidoDb.Reserva);

                if (!string.IsNullOrEmpty(this._predio))
                    this.Query = this.Query.Where(x => x.NU_PREDIO == this._predio || x.NU_PREDIO == null);
            }
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual IEnumerable<LiberacionOndaPedido> GetSelectedKeys(List<string> keysToSelect)
        {
            return this.GetResult()
                .Select(r => string.Join("$", [r.NU_PEDIDO, r.CD_EMPRESA.ToString(), r.CD_CLIENTE]))
                .Intersect(keysToSelect)
                .Select(w => SelectionQuery(w));
        }

        public virtual IEnumerable<LiberacionOndaPedido> GetSelectedKeysAndExclude(List<string> keysToExclude)
        {
            return this.GetResult()
                .Select(r => string.Join("$", [r.NU_PEDIDO, r.CD_EMPRESA.ToString(), r.CD_CLIENTE]))
                .Except(keysToExclude)
                .Select(w => SelectionQuery(w));
        }

        public virtual LiberacionOndaPedido SelectionQuery(string key)
        {
            var keys = key.Split('$');
            return new LiberacionOndaPedido()
            {
                Pedido = keys[0],
                Empresa = int.Parse(keys[1]),
                Cliente = keys[2]
            };
        }

        public virtual IEnumerable<LiberacionOndaPedido> GetRegistros()
        {
            return this.Query.Select(d => new LiberacionOndaPedido
            {
                Pedido = d.NU_PEDIDO,
                Cliente = d.CD_CLIENTE,
                Empresa = d.CD_EMPRESA,
                PorcentajeVidaUtil = (d.VL_PORCENTAJE_VIDA_UTIL ?? 0)
            });
        }
    }
}
