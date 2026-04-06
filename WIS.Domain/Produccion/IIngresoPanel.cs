using System.Collections.Generic;

namespace WIS.Domain.Produccion
{
    public interface IIngresoPanel : IIngreso
    {
        ILinea Linea { get; set; }
        List<Pasada> Pasadas { get; set; }

        Pasada GetLatestPasada();
        Pasada GetCurrentPasada();
    }
}
