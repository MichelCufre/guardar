using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries
{
    public class GetPrediosUsuarioQuery : QueryObject<V_PREDIO_USUARIO, WISDB>
    {
        public GetPrediosUsuarioQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {

            this.Query = context.V_PREDIO_USUARIO.Select(s => s);
        }

        public virtual List<Predio> GetPrediosUsuario(int idUsuario)
        {
            var entities = this.Query.Where(s => s.USERID == idUsuario).ToList().OrderBy(s => s.NU_PREDIO);

            List<Predio> predios = new List<Predio>();

            foreach (var entity in entities)
            {
                var predio = new Predio()
                {
                    Numero = entity.NU_PREDIO,
                    Descripcion = entity.DS_PREDIO,
                };

                predios.Add(predio);
            }

            return predios;
        }

    }
}
