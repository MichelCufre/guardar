using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Tracking.Models
{
    public class PuntoDeEntregaResponse
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Contacto { get; set; }
        public string Observaciones { get; set; }
        public int TiempoEntrega { get; set; }
        public string Telefono { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string Agrupacion { get; set; }

        public PuntoEntregaDireccionResponse Direccion { get; set; }
        public List<PuntoEntregaAgenteResponse> Agentes { get; set; }
        public List<PuntoEntregaVentanaEntregaResponse> VentanasEntrega { get; set; }

        public PuntoDeEntregaResponse()
        {
            this.VentanasEntrega = new List<PuntoEntregaVentanaEntregaResponse>();
            this.Agentes = new List<PuntoEntregaAgenteResponse>();
        }
    }

    public class PuntoEntregaDireccionResponse
    {
        public string Direccion { get; set; }
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }
        public string Zona { get; set; }
        public string Pais { get; set; }
        public string Subdivision { get; set; }
        public string Localidad { get; set; }
        public string Puerta { get; set; }
        public string NumeroOficina { get; set; }
        public string EntreCalle1 { get; set; }
        public string EntreCalle2 { get; set; }
    }

    public class PuntoEntregaVentanaEntregaResponse
    {
        public string HoraDesde { get; set; }
        public string MinutosDesde { get; set; }
        public string HoraHasta { get; set; }
        public string MinutosHasta { get; set; }
    }

    public class PuntoEntregaAgenteResponse
    {
        public string Codigo { get; set; }
        public string Tipo { get; set; }
        public string Descripcion { get; set; }
        public int CodigoEmpresa { get; set; }
    }
}
