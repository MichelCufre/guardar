using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class ContactosInstanciasQuery : QueryObject<V_CONTACTO_INSTANCIA_WEVT040, WISDB>
    {
        protected int _NuInstancia;
        protected string _TipoSeleccion;

        public ContactosInstanciasQuery(string tipoSeleccion, int nuInstancia)
        {
            this._TipoSeleccion = tipoSeleccion;
            this._NuInstancia = nuInstancia;
        }

        public override void BuildQuery(WISDB context)
        {
            var contactosSeleccionados = from c in context.V_CONTACTO_INSTANCIA_WEVT040
                                         where c.NU_EVENTO_INSTANCIA == _NuInstancia
                                         select c;

            if (_TipoSeleccion == "AGREGAR") 
            {             
                this.Query = from c in context.V_CONTACTO_INSTANCIA_WEVT040
                             where c.NU_EVENTO_INSTANCIA == null
                             join s in contactosSeleccionados on c.NU_CONTACTO equals s.NU_CONTACTO
                             into gj
                             from cs in gj.DefaultIfEmpty()
                             where cs.NU_EVENTO_INSTANCIA == null
                                 && context.T_CLIENTE.Any(cli =>
                                     cli.CD_CLIENTE == c.CD_CLIENTE &&
                                     cli.CD_EMPRESA == c.CD_EMPRESA &&
                                     cli.CD_SITUACAO != SituacionDb.Inactivo)
                             select c;
            }
            else
            {
                this.Query = contactosSeleccionados;
            }
        }

        public virtual List<int> GetSelectedKeysAndExclude(List<int> keysToExclude)
        {
            return (from c in this.Query
                    where !keysToExclude.Contains(c.NU_CONTACTO)
                    select c.NU_CONTACTO).ToList();
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no está lista para hacer conteo");

            return this.Query.Count();
        }

    }
}
