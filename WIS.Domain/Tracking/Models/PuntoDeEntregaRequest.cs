using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Tracking.Models
{
    public class PuntoDeEntregaRequest
    {
        public PuntoEntregaDireccion Direccion { get; set; }

        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Contacto { get; set; }
        public string Observaciones { get; set; }
        public int? TiempoEntrega { get; set; }
        public string Telefono { get; set; }
        public string Agrupacion { get; set; }
        public string TipoTarea { get; set; }

        public List<PuntoEntregaVentanaEntrega> VentanasEntrega { get; set; }

        public List<PuntoDeEntregaAgente> Agentes { get; set; }

        public PuntoDeEntregaRequest()
        {
            this.TiempoEntrega = 0;
            this.VentanasEntrega = new List<PuntoEntregaVentanaEntrega>();
            this.Agentes = new List<PuntoDeEntregaAgente>();
        }
    }

    public class PuntoEntregaDireccion
    {
        public string Direccion { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string Zona { get; set; }
        public string Pais { get; set; }
        public string Subdivision { get; set; }
        public string Localidad { get; set; }
        public string Puerta { get; set; }
        public string NumeroOficina { get; set; }
        public string EntreCalle1 { get; set; }
        public string EntreCalle2 { get; set; }

        public PuntoEntregaDireccion()
        {
            Zona = "S/G";
        }
    }
    public class PuntoEntregaVentanaEntrega
    {
        public string HoraDesde { get; set; }
        public string MinutosDesde { get; set; }
        public string HoraHasta { get; set; }
        public string MinutosHasta { get; set; }

        public PuntoEntregaVentanaEntrega()
        {
            HoraDesde = "00";
            MinutosDesde = "00";
            HoraHasta = "23";
            MinutosHasta = "59";
        }
    }
    public class PuntoDeEntregaAgente
    {
        public int CodigoEmpresa { get; set; }
        public string Codigo { get; set; }
        public string Tipo { get; set; }
    }
}
