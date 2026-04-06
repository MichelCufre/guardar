using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Ptl
{
    public class PTL010NotificarPTLAgrupacionVlComparteContenedorQuery : QueryObject<V_PTL010_AGRU_VL_COMP_CONT_PICK, WISDB>
    {
        protected readonly int? _numeroPreparacion;
        protected readonly int? _numeroAutomatismo;
        protected readonly string _cliente;
        protected readonly int? _empresa;
        public PTL010NotificarPTLAgrupacionVlComparteContenedorQuery(int? numeroPreparacion, int? numeroAutomatismo, string cliente, int? empresa)
        {
            _numeroPreparacion = numeroPreparacion;
            _numeroAutomatismo = numeroAutomatismo;
            _cliente = cliente;
            _empresa = empresa;
        }
        public PTL010NotificarPTLAgrupacionVlComparteContenedorQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            if (_numeroPreparacion != null && _numeroAutomatismo != null && !string.IsNullOrEmpty(_cliente) && _empresa != null)
                this.Query = context.V_PTL010_AGRU_VL_COMP_CONT_PICK
                    .AsNoTracking()
                    .Where(x => x.NU_PREPARACION == _numeroPreparacion
                        && x.NU_AUTOMATISMO == _numeroAutomatismo
                        && x.CD_CLIENTE == _cliente
                        && x.CD_EMPRESA == _empresa);
            else
                this.Query = context.V_PTL010_AGRU_VL_COMP_CONT_PICK.AsNoTracking();
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
