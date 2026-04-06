using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Seguridad
{
    public class PrediosDisponiblesQuery : QueryObject<V_PREDIOS, WISDB>
    {
        protected readonly int? _idUsuario;
        public PrediosDisponiblesQuery()
        {

        }
        public PrediosDisponiblesQuery(int idUsuario)
        {
            this._idUsuario = idUsuario;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PREDIOS;
            if (this._idUsuario != null)
            {
                var listPredios = context.V_PREDIO_USUARIO.Where(x => x.USERID == this._idUsuario).Select(x => x.NU_PREDIO).ToList();

                this.Query = this.Query.Where(x => !listPredios.Contains(x.NU_PREDIO));
            }
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<string> GetPredios()
        {
            return this.Query.Select(d => d.NU_PREDIO).ToList();
        }
    }
}
