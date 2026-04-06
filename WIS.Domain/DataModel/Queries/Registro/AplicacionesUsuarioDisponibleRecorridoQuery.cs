using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Recorridos;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class AplicacionesUsuarioDisponibleRecorridoQuery : QueryObject<V_REG700_APLICACION_USER_DISP, WISDB>
    {
        protected readonly int _recorrido;

        public AplicacionesUsuarioDisponibleRecorridoQuery(int recorrido)
        {
            this._recorrido = recorrido;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REG700_APLICACION_USER_DISP.Where(d => d.NU_RECORRIDO == this._recorrido);
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<AplicacionRecorridoUsuario> GetAplicacionesUsuario()
        {
            return this.Query.Select(d => new AplicacionRecorridoUsuario
            {
                IdRecorrido = d.NU_RECORRIDO,
                UserId= d.USERID,
                IdAplicacion= d.CD_APLICACION,
            }).ToList();
        }
    }
}
