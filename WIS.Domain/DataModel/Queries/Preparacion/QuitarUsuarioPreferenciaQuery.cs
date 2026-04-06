using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Picking;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class QuitarUsuarioPreferenciaQuery : QueryObject<V_PRE811_PREF_USUARIO, WISDB>
    {
        protected readonly int _preferencia;

        public QuitarUsuarioPreferenciaQuery(int preferencia)
        {
            this._preferencia = preferencia;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE811_PREF_USUARIO.AsNoTracking().Where(d => d.NU_PREFERENCIA_USUARIO != null && d.NU_PREFERENCIA_USUARIO == this._preferencia);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<PreferenciaAsociarUsuario> GetSelectedKeys(List<string> keysToSelect, int nuPreferencia)
        {
            return this.GetResult().Select(r => string.Join("$", new string[] { r.USERID.ToString() }))
                                       .Intersect(keysToSelect).Select(w => new PreferenciaAsociarUsuario { userId = int.Parse(w), nuPreferencia = nuPreferencia }).ToList();
        }

        public virtual List<PreferenciaAsociarUsuario> GetSelectedKeysAndExclude(List<string> keysToExclude, int nuPreferencia)
        {
            return this.GetResult().Select(r => string.Join("$", new string[] { r.USERID.ToString() }))
                                        .Except(keysToExclude).Select(w => new PreferenciaAsociarUsuario { userId = int.Parse(w), nuPreferencia = nuPreferencia }).ToList();
        }
    }
}