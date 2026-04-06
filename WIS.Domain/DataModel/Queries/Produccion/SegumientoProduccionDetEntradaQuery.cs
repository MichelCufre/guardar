using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Produccion.Constants;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
    public class SegumientoProduccionDetEntradaQuery : QueryObject<V_PRDC_DET_ENTRADA_KIT151, WISDB>
    {
        protected string _nuPrdcIngreso { get; }
        protected string _cdFormula { get; }

        public SegumientoProduccionDetEntradaQuery(string nuPrdcIngreso, string cdFormula)
        {
            this._nuPrdcIngreso = nuPrdcIngreso;
            this._cdFormula = cdFormula;
        }

        public override void BuildQuery(WISDB context)
        {
            var ingreso = context.T_PRDC_INGRESO
                .Where(d => d.NU_PRDC_INGRESO == this._nuPrdcIngreso)
                .FirstOrDefault();

            this.Query = context.V_PRDC_DET_ENTRADA_KIT151
                .Where(v => v.CD_PRDC_DEFINICION == this._cdFormula);

            if (ingreso != null && ingreso.ND_TIPO != TipoIngresoProduccion.BlackBox)
            {
                var qtLineaIngreso = context.T_PRDC_LINEA
                    .Join(context.T_STOCK,
                            pl => pl.CD_ENDERECO_ENTRADA,
                            st => st.CD_ENDERECO,
                            (pl, st) => new { Linea = pl, Stock = st })
                    .Where(d => d.Linea.NU_PRDC_INGRESO == this._nuPrdcIngreso)
                    .GroupBy(d => new
                    {
                        d.Stock.CD_EMPRESA,
                        d.Stock.CD_PRODUTO,
                        d.Stock.CD_FAIXA
                    })
                    .Select(d => new
                    {
                        d.Key.CD_EMPRESA,
                        d.Key.CD_PRODUTO,
                        d.Key.CD_FAIXA,
                        QT_LINEA = (decimal?)d.Sum(e => e.Stock.QT_ESTOQUE ?? 0)
                    });

                var qtPedidoLiberado = context.T_PEDIDO_SAIDA
                    .Join(context.T_DET_PEDIDO_SAIDA,
                            ps => new { ps.NU_PEDIDO, ps.CD_CLIENTE, ps.CD_EMPRESA },
                            dps => new { dps.NU_PEDIDO, dps.CD_CLIENTE, dps.CD_EMPRESA },
                            (ps, dps) => new { Pedido = ps, DetPedido = dps })
                    .Where(d => d.Pedido.NU_PRDC_INGRESO == this._nuPrdcIngreso)
                    .GroupBy(d => new
                    {
                        d.DetPedido.CD_EMPRESA,
                        d.DetPedido.CD_PRODUTO,
                        d.DetPedido.CD_FAIXA
                    })
                    .Select(d => new
                    {
                        d.Key.CD_EMPRESA,
                        d.Key.CD_PRODUTO,
                        d.Key.CD_FAIXA,
                        QT_PEDIDO = (decimal?)d.Sum(e => e.DetPedido.QT_PEDIDO ?? 0),
                        QT_LIBERADO = (decimal?)d.Sum(e => e.DetPedido.QT_LIBERADO ?? 0)
                    });

                var qtPreparadoIngreso = context.T_DET_PICKING
                    .Join(context.T_PEDIDO_SAIDA,
                            pk => new { pk.NU_PEDIDO, pk.CD_CLIENTE, pk.CD_EMPRESA },
                            ps => new { ps.NU_PEDIDO, ps.CD_CLIENTE, ps.CD_EMPRESA },
                            (pk, ps) => new { DetPicking = pk, Pedido = ps })
                    .Join(context.T_CONTENEDOR,
                            pks => new { pks.DetPicking.NU_PREPARACION, pks.DetPicking.NU_CONTENEDOR },
                            co => new { co.NU_PREPARACION, NU_CONTENEDOR = (int?)co.NU_CONTENEDOR },
                            (pks, co) => new { pks.DetPicking, pks.Pedido, Contenedor = co })
                    .Where(d => d.Contenedor.CD_SITUACAO != SituacionDb.ContenedorTransferido
                        && d.Pedido.NU_PRDC_INGRESO == this._nuPrdcIngreso)
                    .GroupBy(d => new
                    {
                        d.DetPicking.CD_EMPRESA,
                        d.DetPicking.CD_PRODUTO,
                        d.DetPicking.CD_FAIXA
                    })
                    .Select(d => new
                    {
                        d.Key.CD_EMPRESA,
                        d.Key.CD_PRODUTO,
                        d.Key.CD_FAIXA,
                        QT_PREPARADO = (decimal?)d.Sum(e => e.DetPicking.QT_PREPARADO ?? 0)
                    });

                var qtConsumidoIngresoHistorico = context.T_HIST_PRDC_LINEA_CONSUMIDO
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
                        QT_CONSUMIDO = (decimal?)d.Sum(e => e.QT_CONSUMIDO ?? 0)
                    });

                var qtConsumidoIngreso = context.T_PRDC_LINEA_CONSUMIDO
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
                        QT_CONSUMIDO = (decimal?)d.Sum(e => e.QT_CONSUMIDO ?? 0)
                    });

                var qtConsumido = ingreso.CD_SITUACAO == SituacionDb.PRODUCCION_FINALIZADA ? qtConsumidoIngresoHistorico : qtConsumidoIngreso;

                this.Query = this.Query
                    .GroupJoin(qtLineaIngreso,
                        v => new { v.CD_EMPRESA, v.CD_PRODUTO, v.CD_FAIXA },
                        qli => new { qli.CD_EMPRESA, qli.CD_PRODUTO, qli.CD_FAIXA },
                        (v, qlis) => new { V = v, QLIs = qlis })
                    .SelectMany(gj => gj.QLIs.DefaultIfEmpty(), (gj, qli) => new { gj.V, QLI = qli })
                    .GroupJoin(qtPedidoLiberado,
                        v => new { v.V.CD_EMPRESA, v.V.CD_PRODUTO, v.V.CD_FAIXA },
                        qpl => new { qpl.CD_EMPRESA, qpl.CD_PRODUTO, qpl.CD_FAIXA },
                        (v, qpls) => new { v.V, v.QLI, QPLs = qpls })
                    .SelectMany(gj => gj.QPLs.DefaultIfEmpty(), (gj, qpl) => new { gj.V, gj.QLI, QPL = qpl })
                    .GroupJoin(qtPreparadoIngreso,
                        v => new { v.V.CD_EMPRESA, v.V.CD_PRODUTO, v.V.CD_FAIXA },
                        qpi => new { qpi.CD_EMPRESA, qpi.CD_PRODUTO, qpi.CD_FAIXA },
                        (v, qpis) => new { v.V, v.QLI, v.QPL, QPIs = qpis })
                    .SelectMany(gj => gj.QPIs.DefaultIfEmpty(), (gj, qpi) => new { gj.V, gj.QLI, gj.QPL, QPI = qpi })
                    .GroupJoin(qtConsumido,
                        v => new { v.V.CD_EMPRESA, v.V.CD_PRODUTO, v.V.CD_FAIXA },
                        qc => new { qc.CD_EMPRESA, qc.CD_PRODUTO, qc.CD_FAIXA },
                        (v, qcs) => new { v.V, v.QLI, v.QPL, v.QPI, QCs = qcs })
                    .SelectMany(gj => gj.QCs.DefaultIfEmpty(), (gj, qc) => new { gj.V, gj.QLI, gj.QPL, gj.QPI, QC = qc })
                    .Select(gj => new V_PRDC_DET_ENTRADA_KIT151
                    {
                        CD_COMPONENTE = gj.V.CD_COMPONENTE,
                        CD_EMPRESA = gj.V.CD_EMPRESA,
                        CD_EMPRESA_PEDIDO = gj.V.CD_EMPRESA_PEDIDO,
                        CD_FAIXA = gj.V.CD_FAIXA,
                        CD_PRDC_DEFINICION = gj.V.CD_PRDC_DEFINICION,
                        CD_PRODUTO = gj.V.CD_PRODUTO,
                        DS_PRODUTO = gj.V.CD_PRODUTO,
                        NU_PRIORIDAD = gj.V.NU_PRIORIDAD,
                        QT_COMPLETA = gj.V.QT_COMPLETA,
                        QT_CONSUMIDO = gj.QC.QT_CONSUMIDO ?? 0,
                        QT_CONSUMIDO_FORM = gj.V.QT_CONSUMIDO_FORM,
                        QT_FORMULA = (gj.V.QT_COMPLETA ?? 0) * (ingreso.QT_FORMULA ?? 0),
                        QT_FORMULA_FORM = gj.V.QT_FORMULA_FORM,
                        QT_LIBERADO = gj.QPL.QT_LIBERADO ?? 0,
                        QT_LIBERADO_FORM = gj.V.QT_LIBERADO_FORM,
                        QT_LINEA = gj.QLI.QT_LINEA ?? 0,
                        QT_LINEA_FORM = gj.V.QT_LINEA_FORM,
                        QT_PEDIDO = gj.QPL.QT_PEDIDO ?? 0,
                        QT_PEDIDO_FORM = gj.V.QT_PEDIDO_FORM,
                        QT_PREPARADO = gj.QPI.QT_PREPARADO ?? 0,
                        QT_PREPARADO_FORM = gj.V.QT_PREPARADO_FORM,
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
