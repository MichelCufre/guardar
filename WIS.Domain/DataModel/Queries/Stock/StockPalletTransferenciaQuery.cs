using System;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Stock
{
    public class StockPalletTransferenciaQuery : QueryObject<V_PALLET_TRANSF_WSTO150DET, WISDB>
    {
        protected readonly int _empresa;
        protected readonly string _ubicacion;
        protected readonly string _producto;
        protected readonly decimal _faixa;
        protected readonly string _identificador;
        protected readonly decimal _qtEntrada;
        protected readonly bool _isAreaLimitada;

        public StockPalletTransferenciaQuery(int empresa, string ubicacion, string producto, decimal faixa, string identificador, decimal qtEntrada, bool isAreaLimitada)
        {
            this._empresa = empresa;
            this._ubicacion = ubicacion;
            this._producto = producto;
            this._faixa = faixa;
            this._identificador = identificador;
            this._qtEntrada = qtEntrada;
            this._isAreaLimitada = isAreaLimitada;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PALLET_TRANSF_WSTO150DET.Where(d => d.CD_SITUACAO == SituacionDb.EnTransferencia
                && d.CD_EMPRESA == this._empresa
                && d.CD_PRODUTO == this._producto
                && d.CD_FAIXA == this._faixa
                && d.NU_IDENTIFICADOR == this._identificador);

            if (this._qtEntrada > 0)
            {
                this.Query = this.Query.Where(d => d.CD_ENDERECO_DESTINO == this._ubicacion);
            }
            else
            {
                if (this._isAreaLimitada)
                    this.Query = this.Query.Where(d => d.CD_ENDERECO_REAL == this._ubicacion);
            }
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
