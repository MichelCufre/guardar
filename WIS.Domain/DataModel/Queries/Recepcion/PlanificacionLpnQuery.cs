using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class PlanificacionLpnQuery : QueryObject<V_REC170_LPN_PLANIFICACION, WISDB>
    {
        protected readonly int? _nuAgenda;
        protected readonly int _empresa;

        public PlanificacionLpnQuery(int empresa, int? nuAgenda = null)
        {
            _nuAgenda = nuAgenda;
            _empresa = empresa;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC170_LPN_PLANIFICACION.Where(d => d.CD_EMPRESA == _empresa);

            if (_nuAgenda.HasValue)
                this.Query = this.Query.Where(d => d.NU_AGENDA == _nuAgenda.Value);
            else
				this.Query = this.Query.Where(d => d.NU_AGENDA == null);
		}
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<long> GetSelectedKeys(List<string> keysToSelect)
        {
            return this.GetResult()
                .Select(r => r.NU_LPN.ToString())
                .Intersect(keysToSelect)
                .Select(w => long.Parse(w))
                .ToList();
        }

        public virtual List<long> GetSelectedKeysAndExclude(List<string> keysToExclude)
        {
            return this.GetResult()
                .Select(r => r.NU_LPN.ToString())
                .Except(keysToExclude)
                .Select(w => long.Parse(w))
                .ToList();
        }
    }
}
