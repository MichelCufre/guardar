using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Configuracion
{
    public class ConfiguracionIdiomaQuery : QueryObject<V_LOCALIZACION_WCOF050, WISDB>
    {
        protected string _lenguajeModificarFiltro;

        public ConfiguracionIdiomaQuery(string lenguajeModificarFiltro)
        {
            this._lenguajeModificarFiltro = lenguajeModificarFiltro;
        }

        public override void BuildQuery(WISDB context)
        {
            if (string.IsNullOrEmpty(_lenguajeModificarFiltro))
            {
                this.Query = context.V_LOCALIZACION_WCOF050;
            }
            else
            {
                this.AplicarFiltro(context);
            }
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual void AplicarFiltro(WISDB context)
        {
            this.Query = context.T_LOCALIZACION
                .Where(d => d.CD_IDIOMA == "base")
                .GroupJoin(context.T_LOCALIZACION.Where(d => d.CD_IDIOMA == this._lenguajeModificarFiltro),
                    t1 => new { t1.CD_APLICACION, t1.CD_BLOQUE, t1.CD_CLAVE, t1.CD_TIPO },
                    t2 => new { t2.CD_APLICACION, t2.CD_BLOQUE, t2.CD_CLAVE, t2.CD_TIPO },
                    (t1, t2) => new { Base = t1, Traduccion = t2 })
                .SelectMany(e => e.Traduccion.DefaultIfEmpty(),
                    (e, f) => new { Base = e.Base, Traduccion = f })
                .Select(d => new V_LOCALIZACION_WCOF050
                {
                    CD_APLICACION = d.Base.CD_APLICACION,
                    CD_BLOQUE = d.Base.CD_BLOQUE,
                    CD_CLAVE = d.Base.CD_CLAVE,
                    CD_TIPO = d.Base.CD_TIPO,
                    CD_IDIOMA = d.Base.CD_IDIOMA,
                    DS_VALOR = d.Base.DS_VALOR,
                    CD_IDIOMA_NUEVO = this._lenguajeModificarFiltro,
                    DS_VALOR_NUEVO = d.Traduccion.DS_VALOR
                });
        }
    }
}
