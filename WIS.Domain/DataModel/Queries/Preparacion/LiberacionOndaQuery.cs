using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Liberacion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class LiberacionOndaQuery : QueryObject<V_PRE050_ONDA, WISDB>
    {
        protected readonly int? _idEmpresa;
        protected readonly int? _onda;

        public LiberacionOndaQuery(int? idEmpresa = null)
        {
            this._idEmpresa = idEmpresa;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE050_ONDA.Where(x => x.CD_SITUACAO == SituacionDb.Activo);
            if (_idEmpresa != null)
            {
                this.Query = this.Query.Where(x => x.CD_EMPRESA == _idEmpresa);
            }
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<Onda> GetByNombreOrCodePartial(string value, string predio)
        {
            List<Onda> ondas = new List<Onda>();
            List<V_PRE050_ONDA> entities = new List<V_PRE050_ONDA>();

            bool ignorarPredio = string.IsNullOrEmpty(predio);
            var queryFiltred = this.Query.Where(x => x.CD_EMPRESA == _idEmpresa && x.CD_SITUACAO == SituacionDb.Activo);

            if (short.TryParse(value, out short cdOnda))
                entities = queryFiltred.Where(d => (ignorarPredio || d.NU_PREDIO == predio || d.NU_PREDIO == null) && (d.CD_ONDA == cdOnda || d.DS_ONDA.ToLower().Contains(value.ToLower()))).ToList();
            else
                entities = queryFiltred.Where(d => (ignorarPredio || d.NU_PREDIO == predio || d.NU_PREDIO == null) && d.DS_ONDA.ToLower().Contains(value.ToLower())).ToList();

            foreach (var entity in entities)
            {
                ondas.Add(new Onda()
                {
                    Descripcion = entity.DS_ONDA,
                    Id = entity.CD_ONDA,
                    Predio = entity.NU_PREDIO
                });
            }

            return ondas;
        }
    }
}
