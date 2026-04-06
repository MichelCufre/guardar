using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Seguridad
{
    public class UsuariosQuery : QueryObject<V_SEG030_USUARIOS, WISDB>
    {
        public UsuariosQuery()
        {
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_SEG030_USUARIOS;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual int GetTipoUsuarioId(int usuario)
        {
            return this.Query.FirstOrDefault(x => x.USERID == usuario).USERTYPEID ?? 0;
        }
    }
}
