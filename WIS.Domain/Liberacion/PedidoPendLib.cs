using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Liberacion
{
    public class PedidoPendLib
    {
        public short? cdOnda { get; set; }

        public int cdEmpresa { get; set; }

        public string nuPedido { get; set; }

        public string cdCliente { get; set; }

        public string dsCliente { get; set; }

        public string cdCondicionLiberacion { get; set; }

        public int? cdRuta { get; set; }

        public string dsRuta { get; set; }

        public string tpPedido { get; set; }

        public string tpExpedicion { get; set; }

        public string cdZona { get; set; }

        public string cdGrupoExpedicion { get; set; }

        public DateTime? dtliberarDesde { get; set; }

        public DateTime? dtLiberarhasta { get; set; }

        public DateTime? dtentrega { get; set; }

        public DateTime? auxHrEntrega { get; set; }

        public DateTime? dtEmitido { get; set; }

        public short? nuOrdenLiberacion { get; set; }

        public string dsEndereco { get; set; }

        public string dsAnexo4 { get; set; }

        public int? cdTransportadora { get; set; }

        public string dsTransportadora { get; set; }

        public string cdUf { get; set; }

        public string dsUf { get; set; }

        public string dsAnexo1 { get; set; }

        public string nuPredio { get; set; }

        public int? nuUltimaPreparacion { get; set; }

        public long? qtlineas { get; set; }

        public decimal? vlPesoTotal { get; set; }

        public decimal? vlVolumenTotal { get; set; }

        public int? qtProductoSinVolumen { get; set; }

        public int? qtProductoSinPeso { get; set; }

        public decimal? qtUnidades { get; set; }

        public decimal? qtLiberado { get; set; }

        public decimal? auxVlVolumen { get; set; }

        public short? auxNuOrdenLiberacion { get; set; }

        public decimal? auxNuOrden { get; set; }

        public string vlComparteContenedorPicking { get; set; }
    }
}
