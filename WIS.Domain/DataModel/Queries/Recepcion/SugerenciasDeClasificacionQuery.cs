using System;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Recepcion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class SugerenciasDeClasificacionQuery : QueryObject<V_REC410_SUGERENCIAS, WISDB>
    {
        protected readonly EstacionDeClasificacion _estacion;
        protected readonly string _destino;
        protected readonly string _zona;

        public SugerenciasDeClasificacionQuery(EstacionDeClasificacion estacion, string destino, string zona)
        {
            this._estacion = estacion;
            this._destino = destino;
            this._zona = string.IsNullOrEmpty(zona) ? null : zona;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC410_SUGERENCIAS
                .Where(s => s.NU_PREDIO == this._estacion.Predio
                    && s.CD_ESTACION == this._estacion.Codigo
                    && s.TP_OPERATIVA == TipoOperacionDb.Clasificacion
                    && ((string.IsNullOrEmpty(s.CD_ENDERECO_DESTINO) && s.CD_ZONA == this._zona)
                        || (!string.IsNullOrEmpty(this._destino) && s.CD_ENDERECO_DESTINO == this._destino)));
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
