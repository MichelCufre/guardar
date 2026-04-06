using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Parametrizacion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Parametrizacion
{
    public class AtributosValidacionDisponibleQuery : QueryObject<V_ATRIBUTO_VALIDACION_DISP, WISDB>
    {
        public List<short> _id_validacionAsociados = null;
        public string _Atributos_tp;
        public AtributosValidacionDisponibleQuery(List<short> id_validacionAsociados , string Atributos_tp)
        {
            _id_validacionAsociados = id_validacionAsociados;
            _Atributos_tp = Atributos_tp;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_ATRIBUTO_VALIDACION_DISP.Where(x =>x.ID_ATRIBUTO_TIPO == _Atributos_tp && !_id_validacionAsociados.Contains(x.ID_VALIDACION));
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<AtributoValidacion> GetAtributoValidacion()
        {
            return this.Query.Select(d => new AtributoValidacion
            {
                Id = d.ID_VALIDACION,
            }).ToList();
        }
    }
}