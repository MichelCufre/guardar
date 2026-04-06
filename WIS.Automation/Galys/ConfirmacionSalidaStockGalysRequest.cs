using System.Collections.Generic;

namespace WIS.Automation.Galys
{
    public class ConfirmacionSalidaStockGalysRequest
    {
        public string codAlmacen { get; set; }

        public string numeroPedidoCliente { get; set; }

        public int estadoSalida { get; set; }

        public string usuario { get; set; }

        public string puesto { get; set; }

        public List<ConfirmacionSalidaStockLineaGalysRequest> listaLineas { get; set; }

        public List<ConfirmacionSalidaStockMatriculasGalysRequest> listaMatriculas { get; set; }
    }

    public class ConfirmacionSalidaStockLineaGalysRequest
    {
        public int lineaSalida { get; set; }

        public string codArticulo { get; set; }

        public decimal cantidadSolicitada { get; set; }

        public decimal cantidadEnBulto { get; set; }
    }

    public class ConfirmacionSalidaStockMatriculasGalysRequest
    {
        public string IdMatricula { get; set; }

        public List<ConfirmacionSalidaStockMatriculasLineaGalysRequest> listaProductos { get; set; }

    }

    public class ConfirmacionSalidaStockMatriculasLineaGalysRequest
    {
        public string codArticulo { get; set; }

        public string fechaEntrada { get; set; }

        public string fechaCaducidad { get; set; }

        public string lote { get; set; }

        public int lineaSalida { get; set; }

        public decimal cantidad { get; set; }
    }
}
