using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Ptl
{
    public class PTL010NotificarPTLAgrupacionSubclaseQuery : QueryObject<V_PTL010_AGRU_SUBCLASE_PROD, WISDB>
    {
        protected readonly int? _numeroPreparacion;
        protected readonly int? _numeroAutomatismo;
        protected readonly string _cliente;
        protected readonly int? _empresa;
        protected readonly string _vlComparteContenedorPicking;
        protected readonly string _subClase;

        public PTL010NotificarPTLAgrupacionSubclaseQuery(int? numeroPreparacion, int? numeroAutomatismo, string cliente, int? empresa, string vlComparteContenedorPicking)
        {
            _numeroPreparacion = numeroPreparacion;
            _numeroAutomatismo = numeroAutomatismo;
            _cliente = cliente;
            _empresa = empresa;
            _vlComparteContenedorPicking = vlComparteContenedorPicking;
        }

        public PTL010NotificarPTLAgrupacionSubclaseQuery(int? numeroPreparacion, int? numeroAutomatismo, string cliente, int? empresa, string vlComparteContenedorPicking, string subClase)
        {
            _numeroPreparacion = numeroPreparacion;
            _numeroAutomatismo = numeroAutomatismo;
            _cliente = cliente;
            _empresa = empresa;
            _vlComparteContenedorPicking = vlComparteContenedorPicking;
            _subClase = subClase;
        }
        public PTL010NotificarPTLAgrupacionSubclaseQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            if (_numeroPreparacion != null && _numeroAutomatismo != null && !string.IsNullOrEmpty(_cliente) && _empresa != null && !string.IsNullOrEmpty(_subClase))
                this.Query = context.V_PTL010_AGRU_SUBCLASE_PROD
                    .AsNoTracking()
                    .Where(x => x.NU_PREPARACION == _numeroPreparacion
                        && x.CD_EMPRESA == _empresa
                        && x.NU_AUTOMATISMO == _numeroAutomatismo
                        && x.CD_CLIENTE == _cliente
                        && x.VL_COMPARTE_CONTENEDOR_PICKING == _vlComparteContenedorPicking
                        && x.CD_SUB_CLASSE == _subClase);
            else if (_numeroPreparacion != null && _numeroAutomatismo != null && !string.IsNullOrEmpty(_cliente) && _empresa != null)
                this.Query = context.V_PTL010_AGRU_SUBCLASE_PROD
                    .AsNoTracking()
                    .Where(x => x.NU_PREPARACION == _numeroPreparacion
                        && x.CD_EMPRESA == _empresa
                        && x.NU_AUTOMATISMO == _numeroAutomatismo
                        && x.CD_CLIENTE == _cliente
                        && x.VL_COMPARTE_CONTENEDOR_PICKING == _vlComparteContenedorPicking);
            else
                this.Query = context.V_PTL010_AGRU_SUBCLASE_PROD.AsNoTracking();
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
