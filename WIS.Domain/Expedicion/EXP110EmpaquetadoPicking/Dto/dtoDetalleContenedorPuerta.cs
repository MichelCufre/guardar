using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Expedicion.EXP110EmpaquetadoPicking.Dto
{
    public class dtoDetalleContenedorPuerta
    {
        public int Empresa { get; set; }
        public string Producto { get; set; }
        public decimal Faixa { get; set; }
        public string Lote { get; set; }
        public string Cliente { get; set; }
        public string Pedido { get; set; }
        public string EspecificaIdentificador { get; set; }
        public string Ubicacion { get; set; }
        public int? Camion { get; set; }
        public decimal? CantidadPreparada { get; set; }
    }
}
