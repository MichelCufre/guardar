using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Seguridad
{
    public class GrupoConsultaQuery : QueryObject<V_SEG030_GRUPO_CONSULTA, WISDB>
    {
        protected readonly int? _idUsuario;
        protected readonly bool _modo;

        public GrupoConsultaQuery()
        {

        }
        public GrupoConsultaQuery(int userId, bool modo)
        {
            this._idUsuario = userId;
            this._modo = modo;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_SEG030_GRUPO_CONSULTA;
            if (this._idUsuario != null)
            {
                var listGrupos = context.V_SEG030_GRUPO_CONSULTA_FUNC.Where(x => x.USERID == this._idUsuario).Select(x => x.CD_GRUPO_CONSULTA).ToList();
                if (this._modo)
                    this.Query = this.Query.Where(x => listGrupos.Contains(x.CD_GRUPO_CONSULTA));
                else
                    this.Query = this.Query.Where(x => !listGrupos.Contains(x.CD_GRUPO_CONSULTA));
            }
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<string> GetGruposConsulta()
        {
            return this.Query.Select(x => x.CD_GRUPO_CONSULTA).ToList();
        }

    }
}
