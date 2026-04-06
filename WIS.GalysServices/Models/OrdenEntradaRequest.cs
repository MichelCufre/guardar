using System.Collections.Generic;

namespace WIS.GalysServices.Models
{
    public class OrdenEntradaRequest
    {
        public string codAlmacen { get; set; }
        public string idEntrada { get; set; }
        public string codProveedor { get; set; }
        public string denomProveedor { get; set; }
        public string fechaEntradaPrevista { get; set; }
        public List<OrdenEntradaDetalleRequest> listaLineas { get; set; }
    }

    public class OrdenEntradaDetalleRequest
    {
        public int lineaEntrada { get; set; }
        public string codArticulo { get; set; }
        public string fechaEntrada { get; set; }
        public string fechaCaducidad { get; set; }
        public string idCarro { get; set; }
        public string lote { get; set; }
        public int cantidad { get; set; }
    }
}
