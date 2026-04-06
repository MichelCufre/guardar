using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class ContactosGruposNoSelQuery : QueryObject<V_CONTACTO_GRU_NS_WEVT030, WISDB>
    {
        protected int? _NuGrupo;

        public ContactosGruposNoSelQuery(int? nuGrupo)
        {
            this._NuGrupo = nuGrupo;
        }

        public override void BuildQuery(WISDB context)
        {
            var contactosSeleccionados = from s in context.V_CONTACTO_GRUPO_WEVT030
                                         where s.NU_CONTACTO_GRUPO == _NuGrupo
                                         select s;

            this.Query = from c in context.V_CONTACTO_GRU_NS_WEVT030
                         join s in contactosSeleccionados on c.NU_CONTACTO equals s.NU_CONTACTO into gj
                         from cs in gj.DefaultIfEmpty()
                         where cs.NU_CONTACTO_GRUPO == null
                             && context.T_CLIENTE.Any(cli =>
                                 cli.CD_CLIENTE == c.CD_CLIENTE &&
                                 cli.CD_EMPRESA == c.CD_EMPRESA &&
                                 cli.CD_SITUACAO != SituacionDb.Inactivo)
                         select c;
        }

        public virtual List<int> GetSelectedKeys(List<int> keysToSelect)
        {
            return this.GetResult()
                .Select(c => c.NU_CONTACTO)
                .Intersect(keysToSelect)
                .ToList();
        }

        public virtual List<int> GetSelectedKeysAndExclude(List<int> keysToExclude)
        {
            return this.GetResult()
                .Select(c => c.NU_CONTACTO)
                .Except(keysToExclude)
                .ToList();
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no está lista para hacer conteo");

            return this.Query.Count();
        }

    }
}
