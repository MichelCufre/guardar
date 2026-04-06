using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Automation.Galys
{
    public class ConfirmacionMovimientoStockGalysRequest
    {
        public string codAlmacen { get; set; }
        public int tipoMovimiento { get; set; }
        public string idMatriculaOrigen { get; set; }
        public string idMatriculaDestino { get; set; }
        public string ubicacionOrigen { get; set; }
        public string ubicacionDestino { get; set; }
        public int? pesoHU { get; set; }
        public string usuario { get; set; }
        public string puesto { get; set; }
        public List<ConfirmacionMovimientoStockLineaGalysRequest> listaProductos { get; set; }
    }

    public class ConfirmacionMovimientoStockLineaGalysRequest
    {
        public string idPeticion { get; set; }
        public int lineaOrden { get; set; }
        public string codArticulo { get; set; }
        public string posDivisionOrigen { get; set; }
        public string posDivisionDestino { get; set; }
        public string lote { get; set; }
        public string fechaEntrada { get; set; }
        public string fechaCaducidad { get; set; }
        public string codigoCausa { get; set; }
        public decimal cantidad { get; set; }
    }
}
