using System;
using System.Collections.Generic;
using WIS.Domain.General;

namespace WIS.Domain.Automatismo.Interfaces
{
    public interface IAutomatismo
    {
        string Codigo { get; set; }
        int Numero { get; set; }
        string Descripcion { get; set; }
        string Tipo { get; set; }
        string Predio { get; set; }
        DateTime FechaRegistro { get; set; }
        DateTime? FechaModificacion { get; set; }
        string ZonaUbicacion { get; set; }
        long? Transaccion { get; set; }
        bool IsEnabled { get; set; }

        List<AutomatismoPosicion> Posiciones { get; set; }
        List<AutomatismoEjecucion> Ejecuciones { get; set; }
        List<AutomatismoInterfaz> Interfaces { get; set; }
        List<AutomatismoCaracteristica> Caracteristicas { get; set; }
        List<AutomatismoPuesto> Puestos { get; set; }

        void GenerarUbicaciones(AutomatismoCantidades cantidades);
        AutomatismoInterfaz GetInterfaz(int cdInterfaz);
        List<AutomatismoPosicion> GetPosiciones(string tipo);
        AutomatismoCaracteristica GetCaracteristicaByCodigo(string codigo);
        AutomatismoPosicion GetPosicion(int id);
        AutomatismoPosicion GetPosicionByUbicacion(string ubicacion);
        void SetInterfazEnUso(int interfaz);
        AutomatismoInterfaz GetInterfazEnUso();

        void ActualizarValoresSegunCaracteristica(AutomatismoCaracteristica caracteristica, List<AutomatismoCaracteristicaConfiguracion> configDefault);
        bool AllowEditionEjecucion();
        Ubicacion GetUbicacionPorDefault(string tipoPosicion, out int? tipoAgrupacion);
    }
}
