using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class GruposInstanciaQuery : QueryObject<V_GRUPO_INSTANCIA__WEVT040, WISDB>
    {
        protected int _NuInstancia;
        protected string _TipoSeleccion;

        public GruposInstanciaQuery(string tipoSeleccion, int nuInstancia)
        {
            this._TipoSeleccion = tipoSeleccion;
            this._NuInstancia = nuInstancia;
        }

        public override void BuildQuery(WISDB context)
        {
            var gruposSeleccionados = from g in context.V_GRUPO_INSTANCIA__WEVT040
                                      where g.NU_EVENTO_INSTANCIA == _NuInstancia
                                      select g;

            if (_TipoSeleccion == "AGREGAR")
            {
                this.Query = from g in context.V_GRUPO_INSTANCIA__WEVT040
                             where g.NU_EVENTO_INSTANCIA == null
                             join s in gruposSeleccionados on g.NU_CONTACTO_GRUPO equals s.NU_CONTACTO_GRUPO
                             into gj
                             from gs in gj.DefaultIfEmpty()
                             where gs.NU_EVENTO_INSTANCIA == null
                             select g;
            }
            else 
            {
                this.Query = gruposSeleccionados;
            }
        }

        public virtual List<int> GetSelectedKeys(List<int> keysToSelect)
        {
            return (from c in this.Query
                    where keysToSelect.Contains(c.NU_CONTACTO_GRUPO)
                    select c.NU_CONTACTO_GRUPO).ToList();
        }

        public virtual List<int> GetSelectedKeysAndExclude(List<int> keysToExclude)
        {
            return (from c in this.Query
                    where !keysToExclude.Contains(c.NU_CONTACTO_GRUPO)
                    select c.NU_CONTACTO_GRUPO).ToList();
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
