using System;
using System.Collections.Generic;

namespace WIS.Domain.Reportes.Dtos
{
    public class DtoDetallesPackingListSinLpn
    {
        public DtoDetallesPackingListSinLpn()
        {
            Contenedores = new List<DtoListaAgenteContenedorPackingListSinLpn>();
        }

        public string RazonSocial { get; set; }

        public string DireccionEmpresaBase { get; set; }

        public string Telefono { get; set; }

        public string Rut { get; set; }

        public int CodigoCamion { get; set; }

        public string DescripcionCamion { get; set; }

        public string CodigoCliente { get; set; }

        public string TipoCliente { get; set; }

        public string CodigoAgente { get; set; }

        public string DescripcionAgente { get; set; }

        public string TipoAgente { get; set; }

        public int CodigoTransportista { get; set; }

        public string DescripcionTransportista { get; set; }

        public int Empresa { get; set; }

        public string DescripcionEmpresa { get; set; }

        public string DireccionEntrega { get; set; }

        public DateTime? Fecha { get; set; }

        public string DescripcionEntrega { get; set; }

        public string Matricula { get; set; }

        public string Predio { get; set; }

        public string DireccionDeposito { get; set; }

        public short? Ruta { get; set; }

        public string DescripcionRuta { get; set; }

        public List<DtoListaAgenteContenedorPackingListSinLpn> Contenedores { get; set; }

        public virtual Dictionary<string, decimal> GetTotalContenedorPorTipo()
        {
            var contenedoresPorTipo = new Dictionary<string, decimal>();

            foreach (var contenedor in Contenedores)
            {
                if (contenedoresPorTipo.ContainsKey(contenedor.TipoContenedor))
                {
                    decimal total = contenedoresPorTipo[contenedor.TipoContenedor];
                    contenedoresPorTipo[contenedor.TipoContenedor] = total + contenedor.Bulto;
                }
                else
                {
                    contenedoresPorTipo.Add(contenedor.TipoContenedor, contenedor.Bulto);
                }
            }

            return contenedoresPorTipo;
        }
    }
}
