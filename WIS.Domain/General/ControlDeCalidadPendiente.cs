using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.StockEntities;

namespace WIS.Domain.General
{
    public class ControlDeCalidadPendiente
    {
        public int Id { get; set; }
        public int? Codigo { get; set; }
        public Stock Stock { get; set; }
        public int? Etiqueta { get; set; }
        public bool Aceptado { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string Predio { get; set; }
        public int? FuncionarioAceptacion { get; set; }
        public int? Empresa { get; set; }
        public string Producto { get; set; }
        public string Identificador { get; set; }
        public decimal? Faixa { get; set; }
        public string Ubicacion { get; set; }
        public long?  NroLPN{ get; set; }
        public int? IdLpnDet { get; set; }
        public string Descripcion { get; set; }
        public long? Instancia { get; set; }

        public virtual void Aceptar(int userId)
        {
            this.Aceptado = true;
            this.FuncionarioAceptacion = userId;
            this.FechaModificacion = DateTime.Now;
        }
    }
}
