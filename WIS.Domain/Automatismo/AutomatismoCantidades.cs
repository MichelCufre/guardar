using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Automatismo
{
    public class AutomatismoCantidades
    {
        public int UbicacionesPicking { get; set; }
        public int UbicacionesEntrada { get; set; }
        public int UbicacionesAjuste { get; set; }
        public int UbicacionesRechazo { get; set; }
        public int UbicacionesTransito { get; set; }
        public int UbicacionesSalida { get; set; }

        public AutomatismoCantidades(int ubicacionesPicking, int ubicacionesEntrada, int ubicacionesAjuste, int ubicacionesRechazo, int ubicacionesTransito, int ubicacionesSalida)
        {
            UbicacionesPicking = ubicacionesPicking;
            UbicacionesEntrada = ubicacionesEntrada;
            UbicacionesAjuste = ubicacionesAjuste;
            UbicacionesRechazo = ubicacionesRechazo;
            UbicacionesTransito = ubicacionesTransito;
            UbicacionesSalida = ubicacionesSalida;
        }
    }

}
