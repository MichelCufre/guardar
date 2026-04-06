using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Stock
{
    public class StockUbicacionQuery : QueryObject<V_ENDERECO_ESTOQUE_WSTO150, WISDB>
    {
        protected readonly int _empresa;
        protected readonly string _ubicacion;
        protected readonly string _producto;
        protected readonly decimal? _faixa;
        protected readonly string _identificador;
        protected readonly List<short?> _areasExcluidas;

        public StockUbicacionQuery()
        {
        }

        public StockUbicacionQuery(int empresa, string ubicacion, string producto, decimal faixa, string identificador)
        {
            this._empresa = empresa;
            this._ubicacion = ubicacion;
            this._producto = producto;
            this._faixa = faixa;
            this._identificador = identificador;
        }

        public StockUbicacionQuery(int empresa, string producto, bool excliurAreasEquipoTransferencia = false)
        {

            this._empresa = empresa;
            this._producto = producto;

            if (excliurAreasEquipoTransferencia)
            {
                this._areasExcluidas = new List<short?>()
                {
                    AreaUbicacionDb.Transferencia,
                    AreaUbicacionDb.Equipamiento,
                };

            }

        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_ENDERECO_ESTOQUE_WSTO150;

            if (!string.IsNullOrEmpty(this._identificador) || this._faixa != null)
            {
                this.Query = this.Query.Where(d => d.CD_EMPRESA == this._empresa && d.CD_PRODUTO == this._producto
                    && d.CD_ENDERECO == this._ubicacion && d.CD_FAIXA == this._faixa && d.NU_IDENTIFICADOR == this._identificador);
            }
            else if ((!string.IsNullOrEmpty(this._producto)) && ((this._empresa != 0)))
            {
                this.Query = this.Query.Where(d => d.CD_EMPRESA == this._empresa && d.CD_PRODUTO == this._producto);
            }
            else if (!string.IsNullOrEmpty(this._producto))
            {
                this.Query = this.Query.Where(d => d.CD_EMPRESA == this._empresa && d.CD_PRODUTO == this._producto && !this._areasExcluidas.Contains(d.CD_AREA_ARMAZ));
            }
        }

        public virtual decimal GetAmountTransitoEntrada()
        {
            if (!this.Query.Any())
                return 0;

            return this.Query.Select(d => d.QT_TRANSITO_ENTRADA ?? 0).FirstOrDefault();
        }

        public StockEntities.Stock GetTotalizado()
        {

            StockEntities.Stock totalizado = new StockEntities.Stock()
            {
                Cantidad = 0,
                ReservaSalida = 0,
                CantidadTransitoEntrada = 0,
            };

            foreach (var item in this.Query)
            {
                totalizado.Cantidad += item.QT_ESTOQUE;
                totalizado.ReservaSalida += item.QT_RESERVA_SAIDA;
                totalizado.CantidadTransitoEntrada += item.QT_TRANSITO_ENTRADA;
            }

            return totalizado;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
