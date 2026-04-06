using System;
using System.Linq;
using WIS.Data;
using WIS.Domain.StockEntities;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Stock
{
    public class StockGeneralQuery : QueryObject<V_STO005_ESTOQUE, WISDB>
    {
        protected readonly string _producto;
        protected readonly int _empresa;

        public StockGeneralQuery(int empresa, string producto)
        {
            this._producto = producto;
            this._empresa = empresa;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_STO005_ESTOQUE.Where(d => d.CD_EMPRESA == this._empresa && d.CD_PRODUTO == this._producto);
        }

        public virtual StockGeneral GetTotalStockGeneral()
        {
            var sumFisicoAux = (decimal)(this.Query.Where(w => (w.ID_AVERIA == null || w.ID_AVERIA == "N") && (w.ID_AREA_AVARIA == null || w.ID_AREA_AVARIA == "N") && (w.ID_DISP_ESTOQUE == null || w.ID_DISP_ESTOQUE == "S") && (w.ID_CTRL_CALIDAD == null || w.ID_CTRL_CALIDAD == "C")).Sum(s => s.QT_ESTOQUE) ?? 0);
            var sumReservaAux = (decimal)(this.Query.Where(w => (w.ID_AVERIA == null || w.ID_AVERIA == "N") && (w.ID_AREA_AVARIA == null || w.ID_AREA_AVARIA == "N") && (w.ID_DISP_ESTOQUE == null || w.ID_DISP_ESTOQUE == "S") && (w.ID_CTRL_CALIDAD == null || w.ID_CTRL_CALIDAD == "C")).Sum(s => s.QT_RESERVA_SAIDA) ?? 0);

            var stockGeneral = new StockGeneral
            {
                CantidadEntrada = this.Query.Sum(d => d.QT_TRANSITO_ENTRADA ?? 0),
                CantidadReserva = this.Query.Sum(d => d.QT_RESERVA_SAIDA ?? 0),
                CantidadFisica = this.Query.Sum(d => d.QT_ESTOQUE ?? 0),
                CantidadDisponible = sumFisicoAux - sumReservaAux,
            };

            stockGeneral.CantidadNoDisponible = stockGeneral.CantidadFisica - stockGeneral.CantidadDisponible;

            //TODO: Obtener cantidad documental

            return stockGeneral;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
