using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Inventario
{
	public class INV030GridQuery : QueryObject<V_AJUSTE_STOCK_WINV030, WISDB>
    {
        protected readonly string _fechaRealizado;
        protected readonly string _producto;
        protected readonly int? _empresa;
        protected readonly string _documento;
        protected readonly IFormatProvider _culture;

        public INV030GridQuery()
        {

        }

        public INV030GridQuery(int empresa, string producto, string documento, string fechaRealizado, IFormatProvider culture)
        {
            this._empresa = empresa;
            this._producto = producto;
            this._documento = documento;
            this._fechaRealizado = fechaRealizado;
            this._culture = culture;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_AJUSTE_STOCK_WINV030;
            if (this._fechaRealizado != null && this._producto != null && this._empresa != null)
            {
                DateTime fechaRealizadoAjuste = DateTime.Parse(this._fechaRealizado, this._culture);

                var fechaCropped = DateTime.ParseExact(fechaRealizadoAjuste.Date.Day.ToString().PadLeft(2, '0') + "/" + fechaRealizadoAjuste.Date.Month.ToString().PadLeft(2, '0') + "/" + fechaRealizadoAjuste.Date.Year.ToString().Substring(2), "dd/MM/yy", _culture);
                var horaRealizado = fechaRealizadoAjuste.TimeOfDay.ToString();

                this.Query = this.Query
                    .Where(d => d.CD_PRODUTO == this._producto
                        && d.CD_EMPRESA == this._empresa
                        && d.NU_DOCUMENTO == this._documento
                        && d.DT_REALIZADO == fechaCropped
                        && d.HR_REALIZADO == horaRealizado);
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
