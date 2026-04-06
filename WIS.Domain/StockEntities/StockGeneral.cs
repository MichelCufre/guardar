using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.General;

namespace WIS.Domain.StockEntities
{
    public class StockGeneral
    {
        public decimal? CantidadEntrada { get; set; }
        public decimal? CantidadFisica { get; set; }
        public decimal? CantidadReserva { get; set; }
        public decimal? CantidadNoDisponible { get; set; }
        public decimal? CantidadDisponible { get; set; }
        public decimal? CantidadDisponibleDocumental { get; set; }
        public ProductoFamilia Familia { get; set; }
        public Clase Clase { get; set; }
        public ProductoRotatividad Rotatividad { get; set; }
        public string DescripcionReducida { get; set; }
        public string CodigoMercadologico { get; set; }
        public string CodigoProductoEmpresa { get; set; }
        public short Situacion { get; set; }
        public virtual void SetDatosProducto(Producto producto)
        {
            Rotatividad = producto.Rotatividad;
            Familia = producto.Familia;
            Clase = producto.Clase;
            DescripcionReducida = producto.DescripcionReducida;
            CodigoMercadologico = producto.CodigoMercadologico;
            CodigoProductoEmpresa = producto.CodigoProductoEmpresa;
            Situacion = producto.Situacion;
        }
    }
}
