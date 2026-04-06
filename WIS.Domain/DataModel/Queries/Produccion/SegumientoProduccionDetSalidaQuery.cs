using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Produccion.Constants;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
    public class SegumientoProduccionDetSalidaQuery : QueryObject<V_PRDC_DET_SALIDA_KIT151, WISDB>
    {
        protected string _nuPrdcIngreso { get; }
        protected string _cdFormula { get; }

        public SegumientoProduccionDetSalidaQuery(string nuPrdcIngreso, string cdFormula)
        {
            this._nuPrdcIngreso = nuPrdcIngreso;
            this._cdFormula = cdFormula;
        }

        public override void BuildQuery(WISDB context)
        {
            var ingreso = context.T_PRDC_INGRESO
                .Where(d => d.NU_PRDC_INGRESO == this._nuPrdcIngreso)
                .FirstOrDefault();

            this.Query = context.V_PRDC_DET_SALIDA_KIT151
                .Where(v => v.CD_PRDC_DEFINICION == this._cdFormula);

            if (ingreso != null && ingreso.ND_TIPO != TipoIngresoProduccion.BlackBox)
            {
                var qtLineaSalidaIngreso  = context.T_PRDC_LINEA
                    .Join(context.T_STOCK,
                          pl => pl.CD_ENDERECO_SALIDA,
                          st => st.CD_ENDERECO,
                          (pl, st) => new { Linea = pl, Stock = st })
                    .Where(d => d.Linea.NU_PRDC_INGRESO == this._nuPrdcIngreso)
                    .GroupBy(d => new 
                    {
                        d.Stock.CD_EMPRESA,
                        d.Stock.CD_PRODUTO,                   
                        d.Stock.CD_FAIXA, 
                    })
                    .Select(d => new
                    {
                        d.Key.CD_EMPRESA,
                        d.Key.CD_PRODUTO,
                        d.Key.CD_FAIXA,
                        QT_LINEA = (decimal?)d.Sum(e => e.Stock.QT_ESTOQUE ?? 0)
                    });

                var qtProducidoIngresoHistorico = context.T_HIST_PRDC_LINEA_PRODUCIDO
                    .Where(d => d.NU_PRDC_INGRESO == this._nuPrdcIngreso)
                    .GroupBy(d => new 
                    {
                        d.CD_EMPRESA,
                        d.CD_PRODUTO,                         
                        d.CD_FAIXA, 
                    })
                    .Select(d => new 
                    {
                        d.Key.CD_EMPRESA,
                        d.Key.CD_PRODUTO,
                        d.Key.CD_FAIXA,
                        QT_PRODUCIDO = (decimal?)d.Sum(e => e.QT_PRODUCIDO ?? 0) 
                    });

                var qtProducidoIngreso = context.T_PRDC_LINEA_PRODUCIDO
                    .Where(d => d.NU_PRDC_INGRESO == this._nuPrdcIngreso)
                    .GroupBy(d => new 
                    {
                        d.CD_EMPRESA,
                        d.CD_PRODUTO,                       
                        d.CD_FAIXA 
                    })
                    .Select(d => new 
                    {
                        d.Key.CD_EMPRESA,
                        d.Key.CD_PRODUTO,
                        d.Key.CD_FAIXA,
                        QT_PRODUCIDO = (decimal?)d.Sum(e => e.QT_PRODUCIDO ?? 0) 
                    });

                var qtProducido = ingreso.CD_SITUACAO == SituacionDb.PRODUCCION_FINALIZADA ? qtProducidoIngresoHistorico : qtProducidoIngreso;

                this.Query = this.Query
                    .GroupJoin(qtLineaSalidaIngreso,
                        v => new { v.CD_EMPRESA, v.CD_PRODUTO, v.CD_FAIXA },
                        qlsi => new { qlsi.CD_EMPRESA, qlsi.CD_PRODUTO, qlsi.CD_FAIXA },
                        (v, qlsis) => new { V = v, QLSIs = qlsis })
                    .SelectMany(gj => gj.QLSIs.DefaultIfEmpty(), (gj, qlsi) => new { gj.V, QLSI = qlsi })
                    .GroupJoin(qtProducido,
                        v => new { v.V.CD_EMPRESA, v.V.CD_PRODUTO, v.V.CD_FAIXA },
                        qp => new { qp.CD_EMPRESA, qp.CD_PRODUTO, qp.CD_FAIXA },
                        (v, qps) => new { v.V, v.QLSI, QPs = qps })
                    .SelectMany(gj => gj.QPs.DefaultIfEmpty(), (gj, qp) => new { gj.V, gj.QLSI, QP = qp })
                    .Select(gj => new V_PRDC_DET_SALIDA_KIT151
                    { 
                        CD_EMPRESA = gj.V.CD_EMPRESA,
                        CD_FAIXA = gj.V.CD_FAIXA,
                        CD_PRDC_DEFINICION = gj.V.CD_PRDC_DEFINICION,
                        CD_PRODUTO = gj.V.CD_PRODUTO,
                        DS_PRODUTO = gj.V.DS_PRODUTO,
                        QT_COMPLETA = gj.V.QT_COMPLETA ?? 0,
                        QT_FORMULA = (gj.V.QT_COMPLETA ?? 0) * (ingreso.QT_FORMULA ?? 0),
                        QT_FORMULA_FORM = gj.V.QT_COMPLETA ?? 0,
                        QT_LINEA = gj.QLSI.QT_LINEA ?? 0,
                        QT_LINEA_FORM = (gj.QLSI.QT_LINEA ?? 0) / (gj.V.QT_COMPLETA ?? 0),
                        QT_PRODUCIDO = gj.QP.QT_PRODUCIDO ?? 0,
                        QT_PRODUCIDO_FORM = (gj.QP.QT_PRODUCIDO ?? 0) / (gj.V.QT_COMPLETA ?? 0),
                    });
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
