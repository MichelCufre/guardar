using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Parametrizacion
{
    public class LpnBarras
    {
        public string CodigoBarras { get; set; }
        public int IdLpnBarras { get; set; }
        public long NumeroLpn { get; set; }
        public short? Orden { get; set; }
        public string Tipo { get; set; }

        #region Api
        public string EstadoLpn { get; set; }
        #endregion
    }
}
