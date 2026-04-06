using System;

namespace WIS.Domain.Picking.Dtos
{
    public class CrossDockingUnaFase
    {
        public int Agenda { get; set; }
        public string Cliente { get; set; }
        public int Preparacion { get; set; }
        public string Producto { get; set; }
        public int Empresa { get; set; }
        public string Ubicacion { get; set; }
        public decimal Cantidad { get; set; }
        public string Identificador { get; set; }
        public int Contenedor { get; set; }
        public string CdAplicacion { get; set; }
        public long NuTransaccion { get; set; }
        public string CodigoAgente { get; set; }
        public string TipoAgente { get; set; }
        public short CodigoPuerta { get; set; }
        public decimal? Faixa { get; set; }
        public int SituacionDestino { get; set; }
        public DateTime Fecha { get; set; }

        public string IdExternoContenedor { get; set; }
        public string TipoContenedor { get; set; }
        public string CodigoBarras { get; set; }

    }
}
