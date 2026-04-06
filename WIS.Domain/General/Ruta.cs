using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.General.Enums;
using WIS.Domain.Liberacion;

namespace WIS.Domain.General
{
    public class Ruta
    {
        public short Id { get; set; }
        public string Descripcion { get; set; }
        public EstadoRutaDeEntrega Estado { get; set; }
        public DateTime? FechaSituacion { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
        /// <summary>
        /// Este campo se utiliza para indicar si la carga del camión debe respetar el orden de carga indicado en los Agentes (clientes) (Colector de expedición) 
        /// </summary>
        public bool ControlaOrdenDeCarga { get; set; }

        public int? Transportista { get; set; }
        public Onda Onda { get; set; }
        public string Zona { get; set; }
        public List<Agente> Agentes { get; set; }

        public PuertaEmbarque PuertaEmbarque { get; set; }

        #region Api
        public short? EstadoId { get; set; }
        public short? PuertaEmbarqueId { get; set; }
        public short? OndaId { get; set; }
        public string ControlaOrdenDeCargaId { get; set; }

        #endregion

        public virtual void Enable()
        {
            this.Estado = EstadoRutaDeEntrega.Activo;
            this.FechaSituacion = DateTime.Now;
        }

        public virtual void Disable()
        {
            this.Estado = EstadoRutaDeEntrega.Inactivo;
            this.FechaSituacion = DateTime.Now;
        }
    }
}
