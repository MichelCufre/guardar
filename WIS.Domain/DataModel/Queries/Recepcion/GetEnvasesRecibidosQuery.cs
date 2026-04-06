using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Recepcion.RecepcionAgendamiento;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class GetEnvasesRecibidosQuery : QueryObject<V_REC170_RECIBIDA_FICTICIA, WISDB>
    {

        protected readonly int _idAgenda;


        public GetEnvasesRecibidosQuery(int idAgenda)
        {
            this._idAgenda = idAgenda;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC170_RECIBIDA_FICTICIA.AsNoTracking()
                .Where(s => s.NU_AGENDA == _idAgenda);
        }

        public virtual List<EnvaseRecepcion> GetEnvasesRecibidos()
        {
            var recibidos = new List<EnvaseRecepcion>();

            var entries = this.Query.ToList();

            foreach (var entry in entries)
            {
                recibidos.Add(new EnvaseRecepcion()
                {
                    IdAgenda = entry.NU_AGENDA,
                    IdEmpresa = entry.CD_EMPRESA,
                    Codigo = entry.CD_ENVASE,
                    Faixa = entry.CD_FAIXA_ENVASE,
                    Cantidad = entry.QT_ENVASE ?? 0
                });
            }

            return recibidos;

        }

    }
}
