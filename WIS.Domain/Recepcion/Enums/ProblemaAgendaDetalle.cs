using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Recepcion.Enums
{

    public enum ProblemaAgendaDetalle
    {
        Unknown,
        RecibidoExcedeAgendado,
        RecibidoExcedeSaldoReferenciaRecepcion,
        RecibidoProductoNoEsperado,
        RecibidoLoteNoEsperado,
        RecibidoMenorAgendado,
        RecibidoMenorFacturado,
        RecibidoMenorSaldoReferencias,
        FacturadoExcedeAgendado,
        FacturadoExcedeSaldoReferenciaRecepcion,
        RecibidoExcedeLpnDeclarado,
        RecibidoProductoNoEsperadoLpn,
        RecibidoMenorLpnDeclarado,
    }

}