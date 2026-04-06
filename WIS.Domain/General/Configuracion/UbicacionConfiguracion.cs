using System.Collections.Generic;

namespace WIS.Domain.General.Configuracion
{
    public class UbicacionConfiguracion
    {
        /// <summary>
        /// Tipo de predio (Numerico / Alfabetica )
        /// </summary>
        public bool PredioNumerico { get; set; }
        public short PredioLargo { get; set; }

        /// <summary>
        /// Tipo de bloque (Numerico / Alfabetica )
        /// </summary>
        public bool BloqueNumerico { get; set; }
        public short BloqueLargo { get; set; }

        /// <summary>
        /// Tipo de calle (Numerico / Alfabetica )
        /// </summary>
        public bool CalleNumerico { get; set; }
        public short CalleLargo { get; set; }

        public short ColumnaLargo { get; set; }

        public short AlturaLargo { get; set; }
        public string UbicacionZonaPorDefecto { get; set; }
        public short EstadoCreacion { get; set; }

        public List<short> AreasMantenibles { get; set; }

        public UbicacionConfiguracion()
        {
            AreasMantenibles = new List<short>();
        }
    }
}
