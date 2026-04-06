using System.Collections.Generic;

namespace WIS.Automation.Galys
{
    public class ConfirmacionEntradaStockGalysRequest
    {
        public string codAlmacen { get; set; }  
        
        public string idEntrada { get; set; }

        public int estadoEntrada { get; set; }

        public string usuario { get; set; }

        public string puesto { get; set; }

        public List<ConfirmacionEntradaStockLineaGalysRequest> listaLineas { get; set; }
    }

    public class ConfirmacionEntradaStockLineaGalysRequest
	{
        public int lineaEntrada { get; set; }

        public string codArticulo { get; set; }

        public decimal cantidadSolicitada { get; set; }

        public decimal cantidadEnMatricula { get; set; }

    }
}
