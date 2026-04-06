using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Picking;
using WIS.Persistence.Database;
using WIS.Security;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
	public class LpnPickingQuery : QueryObject<V_PRE052_STOCK_LPN, WISDB>
	{
		protected readonly int _empresa;
        protected readonly string _predio;
        protected readonly bool _permitePickearVencido;
        protected readonly int _cantidadPrevVenc;
        protected readonly string _condicionStock;

        public LpnPickingQuery(int empresa, string predio, bool permitePickearVencido, int cantidadPrevVenc, string condicionStock)
		{
            _empresa = empresa;
            _predio = predio;
            _permitePickearVencido = permitePickearVencido;
            _cantidadPrevVenc = cantidadPrevVenc;
            _condicionStock = condicionStock;
        }

		public override void BuildQuery(WISDB context)
		{
			Query = context.V_PRE052_STOCK_LPN.Where(x =>x.CD_EMPRESA == _empresa && x.NU_PREDIO == _predio);

            if (!_permitePickearVencido)
            {
                DateTime fechaComparar = DateTime.Now.AddDays(_cantidadPrevVenc);
                Query = Query.Where(x => x.DT_FABRICACAO >= fechaComparar || x.DT_FABRICACAO == null);
            }
            if (_condicionStock == CodigoDominioDb.SelectCondicionStockSano)
                Query = Query.Where(x => x.ID_AVERIA == "N");
            else if (_condicionStock == CodigoDominioDb.SelectCondicionStockAveriado)
                Query = Query.Where(x => x.ID_AVERIA == "S");
        }

		public virtual int GetCount()
		{
			if (Query == null)
				throw new InvalidOperationException("La query no esta lista para hacer conteo");

			return Query.Count();
		}
        public virtual List<LpnPicking> GetLpnPicking(IIdentityService identity)
        {
            return this.Query.Select(d => new LpnPicking
            {
                Ubicacion = d.CD_ENDERECO,
                Empresa = d.CD_EMPRESA,
                Lpn = d.NU_LPN
                
            }).ToList();
        }

    }
}
