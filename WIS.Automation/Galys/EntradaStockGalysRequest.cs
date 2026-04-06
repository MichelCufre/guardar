using System.Collections.Generic;

namespace WIS.Automation.Galys
{
	public class EntradaStockGalysRequest
	{
        public string codAlmacen { get; set; }

        public string idEntrada { get; set; }

        public string codProveedor { get; set; }

        public string denomProveedor { get; set; }

        public string fechaEntradaPrevista { get; set; }

        public List<EntradaStockLineaGalysRequest> listaLineas { get; set; }
    }

    public class EntradaStockLineaGalysRequest
    {
        public int lineaEntrada { get; set; }

        public string codArticulo { get; set; }

        public string idCarro { get; set; }

        public string fechaEntrada { get; set; }

        public string lote { get; set; }

        public string fechaCaducidad { get; set; }

        public int cantidad { get; set; }
    }
}
