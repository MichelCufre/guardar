using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.Interfaces;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class EgresosResponse : EntradaResponse
    {
        public List<DetalleResponse> Egresos { get; set; } = new List<DetalleResponse>();
        public EgresosResponse(InterfazEjecucion interfaz, List<DetalleResponse> egresos) : base(interfaz)
        {
            Egresos = egresos;
        }
    }
    public class DetalleResponse
    {
        public string IdExterno { get; set; }
        public int CodigoCamion { get; set; }

        public DetalleResponse(string idExterno, int codigoCamion)
        {
            IdExterno = idExterno;
            CodigoCamion = codigoCamion;
        }
    }
}
