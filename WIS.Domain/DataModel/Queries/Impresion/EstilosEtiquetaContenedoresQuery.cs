using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Impresiones;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Impresion
{
    public class EstilosEtiquetaContenedoresQuery : QueryObject<V_IMP110_LABEL_ESTILO, WISDB>
    {
        protected readonly string _tipo;
        public EstilosEtiquetaContenedoresQuery()
        {
        }

        public EstilosEtiquetaContenedoresQuery(string tipo)
        {
            this._tipo = tipo;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_IMP110_LABEL_ESTILO.AsNoTracking();
            if (!string.IsNullOrEmpty(this._tipo))
                this.Query = this.Query.Where(x => x.TP_LABEL == this._tipo);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        //No se utiliza mas
        public virtual List<EtiquetaEstilo> GetEtiquetasEstilo(string tipoLabel)
        {
            var listaRetorno = new List<EtiquetaEstilo>();

            var lista = this.Query.Where(x => x.TP_LABEL == tipoLabel).ToList();
            lista.ForEach(item =>

                listaRetorno.Add(new EtiquetaEstilo { 
                    Id = item.CD_LABEL_ESTILO,
                    Descripcion = item.DS_LABEL_ESTILO
                })
            );

            return listaRetorno;
        }

        public virtual List<EtiquetaEstilo> GetEtiquetasEstilo(string tipoLabel, string tipoContenedor)
        {
            var listaRetorno = new List<EtiquetaEstilo>();

            var lista = this.Query.Where(x => x.TP_LABEL == tipoLabel && x.TP_CONTENEDOR == tipoContenedor && x.FL_HABILITADO == "S").ToList();
            lista.ForEach(item =>

                listaRetorno.Add(new EtiquetaEstilo
                {
                    Id = item.CD_LABEL_ESTILO,
                    Descripcion = item.DS_LABEL_ESTILO
                })
            );

            return listaRetorno;
        }
        //
    }
}
