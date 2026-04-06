using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.Parametrizacion;

namespace WIS.Domain.General
{
    public class DatosAjusteStockLpnAtributos
    {
        public long NroLPN { get; set; } // NU_LPN
        public string IdExterno { get; set; } // ID_LPN_EXTERNO
        public string Tipo { get; set; } // TP_LPN_TIPO

        public List<AtributoValor> AtributosCabezal { get; set; }
        public List<AtributoValor> AtributosDetalle { get; set; }

        public DatosAjusteStockLpnAtributos()
        {
            AtributosCabezal = new List<AtributoValor>();
            AtributosDetalle = new List<AtributoValor>();
        }

        public DatosAjusteStockLpnAtributos(long nroLPN, string idExterno, string tipo)
        {
            NroLPN = nroLPN;
            IdExterno = idExterno;
            Tipo = tipo;

            AtributosCabezal = new List<AtributoValor>();
            AtributosDetalle = new List<AtributoValor>();
        }
    }
}
