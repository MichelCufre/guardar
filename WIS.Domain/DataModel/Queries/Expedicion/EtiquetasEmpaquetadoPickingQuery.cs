using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Impresiones;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class EtiquetasEmpaquetadoPickingQuery : QueryObject<V_EXP110_LABEL_ESTILO, WISDB>
    {
        public readonly string _printLenguaje;
        public EtiquetasEmpaquetadoPickingQuery()
        {

        }
        public EtiquetasEmpaquetadoPickingQuery(string printLenguaje)
        {
            _printLenguaje = printLenguaje;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_EXP110_LABEL_ESTILO.AsNoTracking();

            if (!string.IsNullOrEmpty(_printLenguaje))
                this.Query = this.Query.Where(x => x.CD_LENGUAJE_IMPRESION == _printLenguaje);
        }
        public virtual List<EtiquetaEstilo> GetEtiquetasEstilo()
        {
            var listaRetorno = new List<EtiquetaEstilo>();

            var lista = this.Query.ToList();
            lista.ForEach(item =>

                listaRetorno.Add(new EtiquetaEstilo
                {
                    Id = item.CD_LABEL_ESTILO,
                    Descripcion = item.DS_LABEL_ESTILO
                })
            );

            return listaRetorno;
        }

    }
}
