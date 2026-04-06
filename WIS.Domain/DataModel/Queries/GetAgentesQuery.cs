using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.Auxiliares;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries
{
    public class GetAgentesQuery : QueryObject<V_AGENTE, WISDB>
    {
        protected readonly string _filtro;
        protected readonly int _empresa;
        public GetAgentesQuery(string filtro, int empresa)
        {
            this._filtro = filtro;
            this._empresa = empresa;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_AGENTE.AsNoTracking()
                .Where(x => context.T_CLIENTE.Any(cli =>
                    cli.CD_CLIENTE == x.CD_CLIENTE &&
                    cli.CD_EMPRESA == x.CD_EMPRESA &&
                    cli.CD_SITUACAO != SituacionDb.Inactivo));
        }

        public virtual List<AgenteAuxiliar> GetByDescripcionOrAgentePartial(string searchValue, int idEmpresa)
        {
            var agentes = new List<AgenteAuxiliar>();

            var entries = this.Query
                .Where(d => d.CD_EMPRESA == idEmpresa
                    && (d.DS_AGENTE.ToLower().Contains(searchValue.ToLower()) || d.CD_AGENTE.ToLower() == searchValue.ToLower() || d.CD_CLIENTE.ToLower() == searchValue.ToLower())).ToList();

            foreach (var entry in entries)
            {
                agentes.Add(new AgenteAuxiliar()
                {
                    Codigo = entry.CD_AGENTE,
                    CodigoInterno = entry.CD_CLIENTE,
                    Descripcion = entry.DS_AGENTE,
                    DescripcionTipo = entry.DS_TIPO_AGENTE,
                    IdEmpresa = entry.CD_EMPRESA,
                    Tipo = entry.TP_AGENTE
                });
            }

            return agentes;

        }
    }
}
