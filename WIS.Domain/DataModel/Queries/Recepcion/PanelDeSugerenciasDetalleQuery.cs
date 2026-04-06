using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class PanelDeSugerenciasDetalleQuery : QueryObject<V_REC280_PANEL_SUGERENCIA_DET, WISDB>
    {
        public readonly int _estrategia;
        public readonly string _predio;
        public readonly string _tipoOperativa;
        public readonly string _codigoOperativa;
        public readonly string _codigoClase;
        public readonly string _codigoGrupo;
        public readonly int _empresa;
        public readonly string _producto;
        public readonly string _codigoReferencia;
        public readonly string _codigoAgrupador;
        public readonly string _enderecoSugerido;
        public readonly long _nuAlmSugerencia;

        public PanelDeSugerenciasDetalleQuery(
        int estrategia,
        string predio,
        string tipoOperativa,
        string codigoOperativa,
        string codigoClase,
        string codigoGrupo,
        int empresa,
        string producto,
        string codigoReferencia,
        string codigoAgrupador,
        string enderecoSugerido,
        long nuAlmSugerencia)
        {
            _estrategia = estrategia;
            _predio = predio;
            _tipoOperativa = tipoOperativa;
            _codigoOperativa = codigoOperativa;
            _codigoClase = codigoClase;
            _codigoGrupo = codigoGrupo;
            _empresa = empresa;
            _producto = producto;
            _codigoReferencia = codigoReferencia;
            _codigoAgrupador = codigoAgrupador;
            _enderecoSugerido = enderecoSugerido;
            _nuAlmSugerencia = nuAlmSugerencia;

        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC280_PANEL_SUGERENCIA_DET
                .Where(x => x.NU_ALM_ESTRATEGIA == _estrategia
                    && x.NU_PREDIO == _predio
                    && x.TP_ALM_OPERATIVA_ASOCIABLE == _tipoOperativa
                    && x.CD_ALM_OPERATIVA_ASOCIABLE == _codigoOperativa
                    && x.CD_CLASSE == _codigoClase
                    && x.CD_GRUPO == _codigoGrupo
                    && x.CD_EMPRESA_PRODUTO == _empresa
                    && x.CD_PRODUTO == _producto
                    && x.CD_REFERENCIA == _codigoReferencia
                    && x.CD_AGRUPADOR == _codigoAgrupador
                    && x.CD_ENDERECO_SUGERIDO == _enderecoSugerido
                    && x.NU_ALM_SUGERENCIA == _nuAlmSugerencia
                );
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
