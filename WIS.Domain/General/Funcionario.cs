using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
    public class Funcionario
    {
        public int Id { get; set; }
        public string NombreLogin { get; set; }
        public string Idioma { get; set; }
        public short Estado { get; set; }
        public short? Actividad { get; set; }
        public int? Equipo { get; set; }
        public string OperadorId { get; set; }
        public string UsuarioOracle { get; set; }
        public string UsuarioUnix { get; set; }
        public string DireccionArchivosExcel { get; set; }
        public string Email { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInsercion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public string NombreAbreviado { get; set; }
        public string Nombre { get; set; }
        public string IpColector { get; set; }
        public int? OrdenTrabajo { get; set; }
        public string Puntos { get; set; }
        public short CargaHoraria { get; set; }

    }
}
