using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
	public class EmpresasPedPendientesQuery : QueryObject<V_PRE050_EMPRESAS_PED_PENDIEN, WISDB>
    {
        protected readonly int? _userId;
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE050_EMPRESAS_PED_PENDIEN.AsNoTracking();
            if (_userId != null)
                this.Query = this.Query.Where(x => x.USERID == _userId);
        }
        public EmpresasPedPendientesQuery()
        {

        }
        public EmpresasPedPendientesQuery(int user)
        {
            this._userId = user;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<Empresa> GetByNombreOrCodePartial(string value, string cdEmpresa = null)
        {
            List<Empresa> empresas = new List<Empresa>();
            List<V_PRE050_EMPRESAS_PED_PENDIEN> entities = new List<V_PRE050_EMPRESAS_PED_PENDIEN>();

            var queryFiltred = this.Query;

            if (cdEmpresa != null)
            {
                foreach (var x in queryFiltred)
                {
                    if ((x.CD_EMPRESA.ToString().Contains(cdEmpresa) || x.NM_EMPRESA.ToLower().Contains(value.ToLower())) && !entities.Contains(x))
                        entities.Add(x);
                }
            }
            else
            {
                entities = queryFiltred
                    .Where(e => (e.NM_EMPRESA.ToLower().Contains(value.ToLower())))
                    .ToList();
            }

            foreach (var entity in entities)
            {
                var onda = new Empresa()
                {
                    Id = entity.CD_EMPRESA,
                    Nombre = entity.NM_EMPRESA,
                };

                empresas.Add(onda);
            }

            return empresas;
        }

    }
}
