using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Parametrizacion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Parametrizacion
{
    public class AtributosValidacionAsociadosQuery : QueryObject<V_ATRIBUTO_VALIDACION_ASOCIADO, WISDB>
    {
        
        public int _idAtributo;
        public AtributosValidacionAsociadosQuery( int idAtributo)
        {
            _idAtributo = idAtributo;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_ATRIBUTO_VALIDACION_ASOCIADO.Where(x =>x.ID_ATRIBUTO == _idAtributo);
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<AtributoValidacionAsociada> GetAtributoValidacionAsociada()
        {
            return this.Query.Select(d => new AtributoValidacionAsociada
            {

                IdAtributo = d.ID_ATRIBUTO,
                IdValidacion = d.ID_VALIDACION,
               
            }).ToList();
        }
    }
}