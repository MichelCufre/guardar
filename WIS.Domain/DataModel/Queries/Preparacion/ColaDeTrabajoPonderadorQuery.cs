using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class ColaDeTrabajoPonderadorQuery : QueryObject<V_COLA_TRABAJO_PONDERADOR_DET, WISDB>
    {
        protected int? _colaDeTrabajo;
        protected string _cdPonderador;

        public ColaDeTrabajoPonderadorQuery(int colaDeTrabajo, string CdPonderador)
        {
            this._colaDeTrabajo = colaDeTrabajo;
            this._cdPonderador = CdPonderador;
        }

        public override void BuildQuery(WISDB context)
        {
            if (_colaDeTrabajo != null && !string.IsNullOrEmpty(_cdPonderador))
                this.Query = context.V_COLA_TRABAJO_PONDERADOR_DET.Where(w => w.NU_COLA_TRABAJO == this._colaDeTrabajo && w.CD_PONDERADOR == _cdPonderador);

        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
