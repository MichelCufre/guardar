using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Documento;
using WIS.Domain.Documento;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Documento
{
    public class GetCantidadesDocumentoQuery : QueryObject<V_DET_DOC_DUA_DOC020, WISDB>
    {
        protected readonly int _empresa;
        protected readonly string _producto;
        protected readonly DocumentoMapper _mapper;

        public GetCantidadesDocumentoQuery(int empresa, string producto)
        {
            this._empresa = empresa;
            this._producto = producto;
            this._mapper = new DocumentoMapper();
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_DET_DOC_DUA_DOC020.Where(c => c.CD_EMPRESA == this._empresa && c.CD_PRODUTO == this._producto);
        }

        public virtual List<SaldoDetalleDocumento> GetSumaSaldosDisponible()
        {
            return this.Query.ToList().Select(d => this._mapper.MapToSaldoDetalle(d)).ToList();
        }

        public virtual decimal GetCantidadDcoumentos()
        {
            return this.Query.Sum(q => q.QT_DISPONIBLE ?? 0);
        }
    }

}
