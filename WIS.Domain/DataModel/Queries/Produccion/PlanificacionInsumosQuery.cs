using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.Produccion.Queries
{
    public class PlanificacionInsumosQuery : QueryObject<V_PRDC_PLANIFICACION_INSUMO, WISDB>
    {
        protected readonly string _idIngreso;
        protected readonly int _empresa;
        protected readonly IFormatProvider _format;

        public PlanificacionInsumosQuery(string idIngreso, int empresa, IFormatProvider format)
        {
            _idIngreso = idIngreso;
			_empresa = empresa;
            _format = format;
        }

        public override void BuildQuery(WISDB context)
        {
            Query = context.V_PRDC_PLANIFICACION_INSUMO.Where(w => w.NU_PRDC_INGRESO == _idIngreso && w.CD_EMPRESA == _empresa /*&& (w.QT_DISPONIBLE + w.QT_RESERVA + w.QT_PENDIENTE) > 0*/);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
