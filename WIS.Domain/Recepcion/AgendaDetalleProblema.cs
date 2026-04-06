using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Recepcion.Enums;

namespace WIS.Domain.Recepcion
{
    public class AgendaDetalleProblema
    {
        public int Id { get; set; }
        public int NumeroAgenda { get; set; }
        public string CodigoProducto { get; set; }
        public decimal? Embalaje { get; set; }
        public string Identificador { get; set; }
        public TipoProblemaAgendaDetalle TipoProblema { get; set; }
        public ProblemaAgendaDetalle Problema { get; set; }
        public decimal? Diferencia { get; set; }
        public bool Aceptado { get; set; }
        public int? Funcionario { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? FuncionarioAceptaProblema { get; set; }
        public DateTime? FechaAceptacionProblema { get; set; }
        public long? Lpn { get;  set; }

        public virtual void AceptarProblema(int idUsuario)
        {
            this.Aceptado = true;
            this.FechaAceptacionProblema = DateTime.Now;
            this.FuncionarioAceptaProblema = idUsuario;
        }
    }
}
