using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Stock
{
    public class ControlCalidadEnStockQuery : QueryObject<V_ENDERECO_ESTOQUE_WSTO150, WISDB>
    {
        public ControlCalidadEnStockQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_ENDERECO_ESTOQUE_WSTO150
                .Join(context.T_TIPO_AREA,
                    ee => ee.CD_AREA_ARMAZ,
                    tp => tp.CD_AREA_ARMAZ,
                    (ee, tp) => new { Stock = ee, Area = tp })
                .GroupJoin(context.V_STOCK_PRODUCTO_LPN,
                eetp => new { eetp.Stock.CD_ENDERECO, eetp.Stock.CD_EMPRESA, eetp.Stock.CD_PRODUTO, eetp.Stock.CD_FAIXA, eetp.Stock.NU_IDENTIFICADOR },
                plpm => new { plpm.CD_ENDERECO, plpm.CD_EMPRESA, plpm.CD_PRODUTO, plpm.CD_FAIXA, plpm.NU_IDENTIFICADOR },
                (eetp, plpm) => new { Stock = eetp.Stock, Area = eetp.Area, StockLpn = plpm })
                 .SelectMany(
                    plpmdepp => plpmdepp.StockLpn.DefaultIfEmpty(),
                    (plpmdepp, plpm) => new { Stock = plpmdepp.Stock, Area = plpmdepp.Area, StockLpn = plpm == null ? 0 : plpm.QT_ESTOQUE_LPN, ReservaLpn = plpm == null ? 0 : plpm.QT_RESERVA_LPN })
                 .GroupJoin(context.V_RESERVA_LPN_ATRIBUTO,
                plpmdepp => new { plpmdepp.Stock.CD_ENDERECO, CD_EMPRESA = (int?)plpmdepp.Stock.CD_EMPRESA, plpmdepp.Stock.CD_PRODUTO, CD_FAIXA = (decimal?)plpmdepp.Stock.CD_FAIXA, plpmdepp.Stock.NU_IDENTIFICADOR },
                dplpn => new { dplpn.CD_ENDERECO, dplpn.CD_EMPRESA, dplpn.CD_PRODUTO, dplpn.CD_FAIXA, dplpn.NU_IDENTIFICADOR },
                (plpmdepp, dplpn) => new { Stock = plpmdepp.Stock, Area = plpmdepp.Area, plpmdepp.StockLpn , plpmdepp.ReservaLpn , ReservaAtrLpn = dplpn })
                 .SelectMany(
                    plpmdepp => plpmdepp.ReservaAtrLpn.DefaultIfEmpty(),
                    (plpmdepp, plpm) => new { Stock = plpmdepp.Stock, Area = plpmdepp.Area, plpmdepp.StockLpn, plpmdepp.ReservaLpn, ReservaAtrLpn = plpm == null ? 0 : plpm.QT_RESERVA})
                .Where(x =>
                    (x.Stock.QT_ESTOQUE - x.StockLpn) > 0
                    && ((x.Stock.QT_RESERVA_SAIDA ?? 0) - x.ReservaLpn - x.ReservaAtrLpn) == 0 
                    && x.Stock.ID_CTRL_CALIDAD == "C" &&
                    (x.Area.ID_ESTOQUE_GERAL == "S" &&
                    x.Area.ID_AREA_ESPERA == "N" &&
                    x.Area.ID_VEICULO == "N" &&
                    x.Area.ID_AREA_PICKING == "N" &&
                    x.Area.ID_AREA_EMBARQUE == "N"))
                .Select(x => x.Stock);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<string[]> GetSelectedKeys(List<string> keysToSelect, IFormatProvider formatProvider)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.CD_ENDERECO, r.CD_EMPRESA.ToString(), r.CD_PRODUTO, r.CD_FAIXA.ToString(formatProvider), r.NU_IDENTIFICADOR }))
                .Intersect(keysToSelect)
                .Select(w => w.Split('$'))
                .ToList();
        }

        public virtual List<string[]> GetSelectedKeysAndExclude(List<string> keysToExclude, IFormatProvider formatProvider)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.CD_ENDERECO, r.CD_EMPRESA.ToString(), r.CD_PRODUTO, r.CD_FAIXA.ToString(formatProvider), r.NU_IDENTIFICADOR }))
                .Except(keysToExclude)
                .Select(w => w.Split('$'))
                .ToList();
        }
    }
}
