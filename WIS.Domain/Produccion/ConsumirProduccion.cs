using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.Produccion.Models;

namespace WIS.Domain.Produccion
{
    public class ConsumirProduccion
    {
        public int Empresa { get; set; }

        public string IdProduccionExterno { get; set; }

        public bool ConfirmarMovimiento { get; set; }

        public bool FinalizarProduccion { get; set; }

        public string IdEspacio { get; set; }

        public bool IniciarProduccion { get; set; }

        public List<IngresoProduccionDetalle> Insumos { get; set; }
    }
}
