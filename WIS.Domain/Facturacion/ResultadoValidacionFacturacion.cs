using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.General;

namespace WIS.Domain.Facturacion
{
    public class ResultadoValidacionFacturacion
    {
        public List<Contenedor> ContenedoresConProblemas { get; set; }
        public List<IErrorFacturacionPedido> Errores { get; set; }
    }
}