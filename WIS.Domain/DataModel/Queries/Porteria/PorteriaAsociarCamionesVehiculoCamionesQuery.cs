using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Porteria;
using WIS.Extension;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Porteria
{
	public class PorteriaAsociarCamionesVehiculoCamionesQuery : QueryObject<V_PORTERIA_EGRESOS, WISDB>
    {
        protected FiltrosAsociarCamionesVehiculoCamion _filtros;
        protected bool _isFilter;
        protected bool _isAdd;

        public PorteriaAsociarCamionesVehiculoCamionesQuery(FiltrosAsociarCamionesVehiculoCamion filtros, bool isFilter, bool isAdd = false)
        {
            this._filtros = filtros;
            this._isFilter = isFilter;
            this._isAdd = isAdd;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PORTERIA_EGRESOS.AsNoTracking();

            int nuVehiculo = _filtros.NU_PORTERIA_VEHICULO.ToNumber<int>();

            if (_isAdd)
            {
                int? cdEmpresa = _filtros.CD_EMPRESA.ToNumber<int?>();

                if (_isFilter)
                {

                    if (cdEmpresa != null)
                    {
                        this.Query = this.Query.Where(w => w.CD_EMPRESA == cdEmpresa);
                    }

                }

                this.Query = this.Query.Where(w => !context.T_PORTERIA_VEHICULO_CAMION.Any(a => a.NU_PORTERIA_VEHICULO == nuVehiculo
                && a.CD_CAMION == w.CD_CAMION));
            }
            else
            {
                this.Query = this.Query.Where(w => context.T_PORTERIA_VEHICULO_CAMION.Any(a => a.NU_PORTERIA_VEHICULO == nuVehiculo
                && a.CD_CAMION == w.CD_CAMION));
            }

        }

        public virtual List<string> GetKeysRowsSelected(bool allSelected, List<string> keys)
        {

            if (allSelected)
            {
                return this.GetResult()
                    .Select(w => w.CD_CAMION.ToString())
                    .Except(keys)
                    .ToList();
            }
            else
            {
                return this.GetResult()
                    .Select(w => w.CD_CAMION.ToString())
                    .Intersect(keys)
                    .ToList();
            }
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

    }
}
