using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Documento;
using WIS.Domain.Documento;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Documento
{
    class DocumentosConSaldoByProducto : QueryObject<V_DOCUMENTO_SALDO_DETALLE, WISDB>
    {
        protected readonly string _producto;
        protected readonly string _lote;
        protected readonly int _empresa;
        protected readonly decimal _faixa;
        protected readonly DocumentoMapper _mapper;

        public DocumentosConSaldoByProducto(string producto, string lote, decimal faixa, int empresa)
        {
            this._producto = producto;
            this._lote = lote;
            this._empresa = empresa;
            this._faixa = faixa;
            this._mapper = new DocumentoMapper();
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_DOCUMENTO_SALDO_DETALLE
              .Where(d => d.CD_EMPRESA == _empresa
                  && d.CD_FAIXA == _faixa
                  && d.CD_PRODUTO == _producto
                  && d.NU_IDENTIFICADOR == _lote
                  && (d.QT_DISPONIBLE ?? 0) > 0)
              .OrderBy(d => d.DT_ADDROW);
        }

        public virtual List<SaldoDetalleDocumento> GetSaldos()
        {
            return this.Query.Select(d => this._mapper.MapToSaldoDetalle(d)).ToList();
        }

        public virtual decimal GetTotalSaldo()
        {
            return this.Query.Sum(d => (d.QT_DISPONIBLE ?? 0));
        }
    }
}
