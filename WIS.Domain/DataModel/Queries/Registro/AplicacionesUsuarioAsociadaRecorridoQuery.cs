using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Recorridos;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class AplicacionesUsuarioAsociadaRecorridoQuery : QueryObject<V_REG700_APLICACION_USER_ASO, WISDB>
    {
        protected readonly int _recorrido;


        public AplicacionesUsuarioAsociadaRecorridoQuery(int recorrido)
        {
            this._recorrido = recorrido;
            
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REG700_APLICACION_USER_ASO.Where(d => d.NU_RECORRIDO == this._recorrido);
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<AplicacionRecorridoUsuario> GetAllAplicacionesAsociadas(bool isRecorridoDefault)
        {
            return this.Query.Where(x =>x.FL_PREDETERMINADO != "S" || isRecorridoDefault).Select(d => new AplicacionRecorridoUsuario
            {
                IdRecorrido = d.NU_RECORRIDO,
                UserId = d.USERID,
                IdAplicacion = d.CD_APLICACION,
            }).ToList();
        }
    }
}
