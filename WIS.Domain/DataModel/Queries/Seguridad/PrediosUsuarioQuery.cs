using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Seguridad
{
    public class PrediosUsuarioQuery : QueryObject<V_PREDIO_USUARIO, WISDB>
    {
        protected readonly int? _idUsuario;
        public PrediosUsuarioQuery()
        {

        }
        public PrediosUsuarioQuery(int idUsuario)
        {
            this._idUsuario = idUsuario;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PREDIO_USUARIO.Where(x => x.USERID == this._idUsuario).AsNoTracking();
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
        public virtual List<string> GetPrediosAsociados()
        {
            return this.Query.Select(d => d.NU_PREDIO).ToList();
        }
        public virtual List<string[]> GetSelectedKeysAndExclude(List<string> keysToExclude)
        {
            return this.GetResult().Select(r => string.Join("$", new string[] { r.NU_PREDIO }))
                                        .Except(keysToExclude).Select(w => w.Split('$')).ToList(); //TODO: Ver de cambiar
        }
        public virtual List<string[]> GetSelectedKeys(List<string> keysToSelect)
        {
            return this.GetResult().Select(r => string.Join("$", new string[] { r.NU_PREDIO }))
                                       .Intersect(keysToSelect).Select(w => w.Split('$')).ToList();
        }
    }
}
