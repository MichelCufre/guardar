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
	public class PorteriaAsociarAgendasVehiculoAgendasQuery : QueryObject<V_PORTERIA_AGENDAS, WISDB>
    {
        protected FiltrosAsociarAgendasVehiculoAgenda _filtros;
        protected bool _isFilter;
        protected bool _isAdd;

        public PorteriaAsociarAgendasVehiculoAgendasQuery(FiltrosAsociarAgendasVehiculoAgenda filtros, bool isFilter, bool isAdd = false)
        {
            this._filtros = filtros;
            this._isFilter = isFilter;
            this._isAdd = isAdd;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PORTERIA_AGENDAS.AsNoTracking();

            int nuVehiculo = _filtros.NU_PORTERIA_VEHICULO.ToNumber<int>();

            if (_isAdd)
            {
                int? cdEmpresa = _filtros.CD_EMPRESA.ToNumber<int?>();
                string cdCliente = _filtros.CD_CLIENTE;

                if (_isFilter)
                {

                    if (!string.IsNullOrEmpty(cdCliente))
                    {
                        this.Query = this.Query.Where(w => w.CD_CLIENTE == cdCliente);
                    }

                    if (cdEmpresa != null)
                    {
                        this.Query = this.Query.Where(w => w.CD_EMPRESA == cdEmpresa);
                    }

                }

                this.Query = this.Query.Where(w => !context.T_PORTERIA_VEHICULO_AGENDA.Any(a => a.NU_PORTERIA_VEHICULO == nuVehiculo
                && a.NU_AGENDA == w.NU_AGENDA));
            }
            else
            {
                this.Query = this.Query.Where(w => context.T_PORTERIA_VEHICULO_AGENDA.Any(a => a.NU_PORTERIA_VEHICULO == nuVehiculo
                && a.NU_AGENDA == w.NU_AGENDA));
            }


        }

        public virtual List<string> GetKeysRowsSelected(bool allSelected, List<string> keys)
        {
            if (allSelected)
            {
                return this.GetResult()
                    .Select(w => w.NU_AGENDA.ToString())
                    .Except(keys)
                    .ToList();
            }
            else
            {
                return this.GetResult()
                    .Select(w => w.NU_AGENDA.ToString())
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
