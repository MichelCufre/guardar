using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Produccion;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Enums;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
    public class LineasDisponiblesQuery : QueryObject<V_PRDC_KIT170_LI_CD_PRDC_LINEA, WISDB>
    {
        protected readonly TipoProduccionLinea _tipo;
        protected readonly LineaMapper _mapper;
        public LineasDisponiblesQuery(TipoProduccionLinea tipo, LineaMapper mapper)
        {
            this._tipo = tipo;
            this._mapper = mapper;
        }

        public override void BuildQuery(WISDB context)
        {
            string tpLinea = _mapper.MapTipoLineaToString(_tipo);
            this.Query = context.V_PRDC_KIT170_LI_CD_PRDC_LINEA
                .Where(d => d.ND_TIPO_LINEA == tpLinea);
        }

        public virtual List<ILinea> GetLineasDisponibles()
        {
            var lineasView = this.Query.ToList();
            List<ILinea> lineasDisponibles = new List<ILinea>();

            foreach (var entity in lineasView)
            {
                ILinea linea = null;
                switch (_tipo)
                {
                    case TipoProduccionLinea.WhiteBox:
                        linea = new LineaWhiteBox();
                        break;
                    case TipoProduccionLinea.BlackBox:
                        linea = new LineaBlackBox();
                        break;
                }

                linea.Id = entity.CD_PRDC_LINEA;
                linea.Descripcion = entity.DS_PRDC_LINEA;
                linea.Predio = entity.NU_PREDIO;

                lineasDisponibles.Add(linea);
            }

            return lineasDisponibles;
        }
    }
}
