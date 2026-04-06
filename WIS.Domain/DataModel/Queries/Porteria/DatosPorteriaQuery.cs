using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Porteria
{
	public class DatosPorteriaQuery : QueryObject<V_PORTERIA_PERSONA, WISDB>
    {
        protected readonly long? numPorteriaVehicul;
        protected readonly List<int> _personas;
        protected readonly List<int> _personasRemove;

        public DatosPorteriaQuery(long? _numPorteriaVehicul, List<int> personas, List<int> personasRemove)
        {
            this.numPorteriaVehicul = _numPorteriaVehicul;
            this._personas = personas;
            this._personasRemove = personasRemove;
        }

        public override void BuildQuery(WISDB context)
        {

            this.Query = context.V_PORTERIA_PERSONA.AsNoTracking();

            if (this.numPorteriaVehicul != null)
            {
                this.Query = this.Query.Where(w => ((w.NU_PORTERIA_VEHICULO_ENTRADA == this.numPorteriaVehicul && !_personasRemove.Contains(w.NU_POTERIA_PERSONA ?? -1)) || _personas.Contains(w.NU_POTERIA_PERSONA ?? -1)) && w.DT_PERSONA_SALIDA == null);
            }
            else
            {
                this.Query = this.Query.Where(w => w.DT_PERSONA_SALIDA == null && _personas.Contains(w.NU_POTERIA_PERSONA ?? -1));
            }

        }

        public virtual List<string> GetKeysRowsSelected()
        {
            return this.Query
                .Select(r => r.NU_PORTERIA_REGISTRO_PERSONA.ToString())
                .ToList();
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
