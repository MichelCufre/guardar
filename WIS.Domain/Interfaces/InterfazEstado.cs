using System.Collections.Generic;

namespace WIS.Domain.Interfaces
{
    public class InterfazEstado
    {
        public InterfazEjecucion Interfaz { get; set; }
        public List<InterfazError> Errores { get; set; }

        public InterfazEstado()
        {
            Errores = new List<InterfazError>();
        }

        public InterfazEstado(InterfazEjecucion interfaz, List<InterfazError> errores)
        {
            Interfaz = interfaz;
            Errores = errores;
        }
    }
}
