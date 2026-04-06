using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Stock
{
    public class MovimientosStockQuery : QueryObject<V_STO395_MOVTO_STOCK, WISDB>
    {
        protected readonly string _producto;
        protected readonly int _empresa;
        protected readonly DateTime? _fechaInicio;
        protected readonly DateTime? _fechaFin;
        protected readonly string _predio;

        public MovimientosStockQuery(int empresa, string producto, DateTime? fechaInicio, DateTime? fechaFin, string predio)
        {
            this._producto = producto;
            this._empresa = empresa;
            this._predio = (string.IsNullOrEmpty(predio) || predio == GeneralDb.PredioSinDefinir) ? null : predio;
            this._fechaInicio = fechaInicio;
            this._fechaFin = fechaFin;
        }

        public override void BuildQuery(WISDB context)
        {

            this.Query = context.V_STO395_MOVTO_STOCK.AsNoTracking().Where(d => d.CD_EMPRESA == this._empresa && d.CD_PRODUTO == this._producto);

            if (!string.IsNullOrEmpty(this._predio))
                this.Query = this.Query.Where(d => d.NU_PREDIO == _predio);

        }

        public virtual decimal GetCantidadStockInicioPeriodo()
        {
            if (this.Query == null)
                throw new InvalidOperationException("General_Sec0_Error_QueryNotReady");

            var fechaInicio = this._fechaInicio ?? DateTime.ParseExact("01/01/1980", "dd/MM/yyyy", CultureInfo.InvariantCulture);

            if (!this.Query.Any(d => d.DT_ADDROW < fechaInicio)) //No hay otra
                return 0;

            return this.Query.Where(d => d.DT_ADDROW < fechaInicio).Sum(d => d.QT_ITEM ?? 0);
        }

        public virtual decimal GetCantidadStockFinalPeriodo()
        {
            if (this.Query == null)
                throw new InvalidOperationException("General_Sec0_Error_QueryNotReady");

            var fechaFin = this._fechaFin ?? DateTime.Now;

            if (!this.Query.Any(d => d.DT_ADDROW <= fechaFin)) //No hay otra
                return 0;

            return this.Query.Where(d => d.DT_ADDROW <= fechaFin).Sum(d => d.QT_ITEM ?? 0);
        }

        /// <summary>
        /// Agrega filtro por fecha de inicio y fin. No utilizar antes de métodos de cantidades
        /// </summary>
        public virtual void FilterByDate()
        {
            if (this.Query == null)
                throw new InvalidOperationException("General_Sec0_Error_QueryNotReady");

            if (this._fechaInicio != null && this._fechaFin != null)
                this.Query = this.Query.Where(d => d.DT_ADDROW >= this._fechaInicio && d.DT_ADDROW <= this._fechaFin);
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
