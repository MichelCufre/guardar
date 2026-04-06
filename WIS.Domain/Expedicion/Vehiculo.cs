using System;
using WIS.Domain.Tracking.Models;

namespace WIS.Domain.Expedicion
{
    public class Vehiculo
    {
        public int Id { get; set; }
        public int? Transportista { get; set; }
        public string Matricula { get; set; }
        public string Descripcion { get; set; }
        public string Predio { get; set; }
        public string Marca { get; set; }
        public TimeSpan HoraDisponibilidadDesde { get; set; }
        public TimeSpan HoraDisponibilidadHasta { get; set; }
        public string Estado { get; set; }
        public VehiculoEspecificacion Caracteristicas { get; set; }
        public bool SincronizacionRealizada { get; set; }

        public virtual bool CanDelete()
        {
            return this.Estado != EstadoVehiculoDb.EnViaje;
        }
    }
}
