using System;
using WIS.Domain.Produccion.Enums;

namespace WIS.Domain.Produccion
{
    public interface ILinea
    {
        string Id { get; set; }
        string Descripcion { get; set; }
        string UbicacionEntrada { get; set; }
        string UbicacionSalida { get; set; }
        TipoProduccionLinea Tipo { get; set; }
        string Predio { get; set; }
        DateTime? FechaAlta { get; set; }
        DateTime? FechaModificacion { get; set; }
        string NumeroIngreso { get; set; }
    }
}
