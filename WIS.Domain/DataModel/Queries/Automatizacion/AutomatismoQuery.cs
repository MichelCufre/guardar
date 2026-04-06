using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.DataModel.Mappers.Automatismo;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Automatizacion
{
    public class AutomatismoQuery : QueryObject<T_AUTOMATISMO, WISDB>
    {
        protected AutomatismoMapper _mapper;

        public AutomatismoQuery(IAutomatismoFactory automatismoFactory)
        {
            _mapper = new AutomatismoMapper(automatismoFactory);
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.T_AUTOMATISMO.AsNoTracking();
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<IAutomatismo> GetAutomatismos(string predioLogin, List<string> prediosUsuario)
        {
            List<IAutomatismo> automatismos = new List<IAutomatismo>();
            IList<T_AUTOMATISMO> entities = this.GetResult();

            if (predioLogin == GeneralDb.PredioSinDefinir)
            {
                entities = entities.Where(w => prediosUsuario.Contains(w.NU_PREDIO)).ToList();
            }
            else
            {
                entities = entities.Where(w => w.NU_PREDIO == predioLogin).ToList();
            }

            foreach (var entity in entities)
            {
                automatismos.Add(_mapper.Map(entity));
            }

            return automatismos;
        }
    }
}
