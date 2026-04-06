using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.General.Enums;
using WIS.Domain.General;
using WIS.Domain.Liberacion;

namespace WIS.Domain.Tracking.Models
{
    public class RutaResponse
    {
        public short Id { get; set; }
        public string Descripcion { get; set; }
        public short? Situacion { get; set; }
        public short? CodigoPuerta { get; set; }
        public short? CodigoOnda { get; set; }
        public string ControlaOrdenDeCarga { get; set; }
        public int? CodigoTransportista { get; set; }
        public string Zona { get; set; }

        public string FechaSituacion { get; set; }
        public string FechaAlta { get; set; }
        public string FechaModificacion { get; set; }
    }
}
