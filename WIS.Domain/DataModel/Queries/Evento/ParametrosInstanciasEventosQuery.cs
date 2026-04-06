using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class ParametrosInstanciasEventosQuery : QueryObject<V_EVENTO_PARAM_INS_WEVT040, WISDB>
    {
        protected string _tipoNotificacion;
        protected int _numeroEvento;
        protected int? _numeroInstancia;

        public ParametrosInstanciasEventosQuery(string tipoNotificacion, int numeroEvento, int? numeroInstancia)
        {
            this._tipoNotificacion = tipoNotificacion;
            this._numeroEvento = numeroEvento;
            this._numeroInstancia = numeroInstancia;
        }

        public override void BuildQuery(WISDB context)
        {
            var parametros = from p in context.V_EVENTO_PARAM_INS_WEVT040
                             where p.NU_EVENTO == _numeroEvento
                                && p.TP_NOTIFICACION == _tipoNotificacion
                             select p;

            var parametros_i = from p in parametros
                               where p.NU_EVENTO_INSTANCIA == _numeroInstancia
                               select p;

            this.Query = from p in parametros
                         join pi in parametros_i on new { p.NU_EVENTO , p.TP_NOTIFICACION , p.CD_EVENTO_PARAMETRO } equals new { pi.NU_EVENTO , pi.TP_NOTIFICACION , pi.CD_EVENTO_PARAMETRO }
                         into gj
                         from pis in gj.DefaultIfEmpty()
                         where p.NU_EVENTO_INSTANCIA == _numeroInstancia || p.NU_EVENTO_INSTANCIA == 0 && pis.NU_EVENTO_INSTANCIA == null
                         select p;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no está lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
