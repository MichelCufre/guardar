using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Domain.Porteria;
using WIS.Extension;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Porteria
{
    public class PorteriaVehiculosDepositoQuery : QueryObject<V_PORTERIA_VEHICULO, WISDB>
    {
        protected readonly FiltrosDispositivosVehiculos _filtros;

        public PorteriaVehiculosDepositoQuery(FiltrosDispositivosVehiculos filtros)
        {
            this._filtros = filtros;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PORTERIA_VEHICULO.AsNoTracking().Where(w => w.DT_PORTERIA_SALIDA == null && w.FL_SOLO_BALANZA != "S" && w.CD_TRANSPORTE != "API");

            if (!string.IsNullOrEmpty(_filtros.NU_VEHICULO))
            {
                int nuPorteriaVehiculo = _filtros.NU_VEHICULO.ToNumber<int>();

                this.Query = this.Query.Where(w => w.NU_PORTERIA_VEHICULO == nuPorteriaVehiculo);
            }

            //if (!_filtros.TP_REGISTRO.IsNullOrEmpty())
            //{
            //    if (_filtros.TP_REGISTRO.Equals("ENT"))
            //    {
            //        this.Query = this.Query.Where(w => w.DT_PORTERIA_SALIDA == null && w.FL_SOLO_BALANZA != "S");
            //    }
            //    else if (_filtros.TP_REGISTRO.Equals("SAL"))
            //    {
            //        this.Query = this.Query.Where(w => w.DT_PORTERIA_SALIDA != null && w.DT_PORTERIA_ENTRADA != null && w.FL_SOLO_BALANZA != "S");
            //    }
            //    else if (_filtros.TP_REGISTRO.Equals("TRA"))
            //    {
            //        this.Query = this.Query.Where(w => w.FL_SOLO_BALANZA == "S");
            //    }
            //}

            if (!_filtros.CD_EMPRESA.IsNullOrEmpty())
            {
                int cdEmpresa = _filtros.CD_EMPRESA.ToNumber<int>();

                this.Query = this.Query.Where(w => w.CD_EMPRESA == cdEmpresa);
            }

            if (!_filtros.CD_SECTOR.IsNullOrEmpty())
            {
                this.Query = this.Query.Where(w => w.CD_SECTOR == _filtros.CD_SECTOR);
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
