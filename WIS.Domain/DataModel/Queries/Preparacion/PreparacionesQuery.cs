using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class PreparacionesQuery : QueryObject<V_PRE052_PICKING, WISDB>
    {
        protected readonly int? _idEmpresa;
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE052_PICKING;

            if(_idEmpresa != null)
            {
                this.Query = this.Query.Where(x => x.CD_EMPRESA == _idEmpresa);
            }
        }
        public PreparacionesQuery()
        {

        }
        public PreparacionesQuery(int empresa)
        {
            this._idEmpresa = empresa;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<int?> FiltroEmpresasConLiberacion(List<Empresa> idesEmpresas)
        {

            List<int> idesExistentesEmpresa = new List<int>();

            foreach(var emp in idesEmpresas)
            {
                idesExistentesEmpresa.Add(emp.Id);
            }

            List<int?> lista = this.Query.Where(x => idesExistentesEmpresa.Contains(x.CD_EMPRESA ?? 0)).Select(x=>x.CD_EMPRESA).Distinct().ToList();
            return lista;
        }

    }
}
