using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Facturacion
{
    public class FAC008Query : QueryObject<V_FAC008_RESULTADO_DETALLE, WISDB>
    {
        protected int _nuEjecucion;
        protected int _emp;
        protected string _cdFacturacion;
        protected string _nuComponente;
        protected bool _params;

        public FAC008Query()
        {
            _params = false;
        }

        public FAC008Query(int nuEjecucion, int emp, string cdFacturacion, string nuComponente)
        {
            _nuEjecucion = nuEjecucion;
            _emp = emp;
            _cdFacturacion = cdFacturacion;
            _nuComponente = nuComponente;
            _params = true;
        }

        public override void BuildQuery(WISDB context)
        {
            if (_params)
                this.Query = context.V_FAC008_RESULTADO_DETALLE.AsNoTracking().Where(r => r.NU_EJECUCION == _nuEjecucion && r.CD_EMPRESA == _emp && r.CD_FACTURACION.ToUpper().Trim() == _cdFacturacion.ToUpper().Trim() && r.NU_COMPONENTE.ToUpper().Trim() == _nuComponente.ToUpper().Trim());
            else
                this.Query = context.V_FAC008_RESULTADO_DETALLE.AsNoTracking();
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
