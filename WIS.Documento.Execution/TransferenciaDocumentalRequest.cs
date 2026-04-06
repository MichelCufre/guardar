using System.Collections.Generic;
using System.Linq;

namespace WIS.Documento.Execution
{
    public class TransferenciaDocumentalRequest
    {
        public string NroTransferencia { get; set; }
        public int EmpresaEgreso { get; set; }
        public int EmpresaIngreso { get; set; }
        public int? Preparacion { get; set; }
        public string Aplicacion { get; set; }
        public int Usuario { get; set; }
        public List<LineaEgresoDocumentalRequest> LineasEgreso { get; set; }
        public List<LineaIngresoDocumentalRequest> LineasIngreso { get; set; }

        public TransferenciaDocumentalRequest()
        {
            LineasEgreso = new List<LineaEgresoDocumentalRequest>();
            LineasIngreso = new List<LineaIngresoDocumentalRequest>();
        }

        public void AddLineaEgreso(int preparacion, int codigoEmpresa, string codigoProducto, string identificador, decimal? faixa, decimal? cantidadAfectada, string semiacabado, string consumible)
        {
            if (LineasEgreso == null)
                LineasEgreso = new List<LineaEgresoDocumentalRequest>();

            var linea = LineasEgreso.FirstOrDefault(s => s.Empresa == codigoEmpresa
                && s.Producto == codigoProducto
                && s.Identificador == identificador
                && s.Faixa == faixa
                && s.Preparacion == preparacion);

            if (linea == null)
            {
                linea = new LineaEgresoDocumentalRequest()
                {
                    Empresa = codigoEmpresa,
                    Producto = codigoProducto,
                    Identificador = identificador,
                    Faixa = faixa,
                    Preparacion = preparacion,
                    Semiacabado = semiacabado,
                    Consumible = consumible
                };
                LineasEgreso.Add(linea);
            }

            linea.CantidadAfectada = (linea.CantidadAfectada ?? 0) + (cantidadAfectada ?? 0);
        }
    }
}
