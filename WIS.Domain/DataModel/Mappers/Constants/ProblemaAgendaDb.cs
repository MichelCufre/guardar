using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.DataModel.Mappers.Constants
{
    public class ProblemaAgendaDb
    {
        //TP_PROBLEMA
        public const string TipoProblema = "PRO";
        public const string TipoNotificacion = "NOT";

        public const string TIPO_AGENDA_PROBLEMA = "TPAGEND";
        public const string TIPO_PROBELMA = "TRAGENDA";

        //PROBELMA
        public const string RecibidoExcedeAgendado = "REA";
        public const string RecibidoExcedeSaldoReferenciaRecepcion = "RESRR";
        public const string RecibidoProductoNoEsperado = "RPE";
        public const string RecibidoLoteNoEsperado = "RLE";
        public const string RecibidoMenorAgendado = "RMA";
        public const string RecibidoMenorFacturado = "RMF";
        public const string RecibidoMenorSaldoReferencias = "RMSR";
        public const string FacturadoExcedeAgendado = "FEA";
        public const string FacturadoExcedeSaldoReferenciaRecepcion = "FESRR";

        public const string DescripcionExcedeSaldoReferenciaRecepcion = "RecibExcedeSaldoOC-";

        public const string RecibidoExcedeLpnDeclarado = "RELPN";
        public const string RecibidoMenorLpnDeclarado = "RMLPN";
        public const string RecibidoProductoNoEsperadoLpn = "RPELPN";
    }
}
