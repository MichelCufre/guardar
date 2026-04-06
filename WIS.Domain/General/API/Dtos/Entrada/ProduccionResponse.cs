using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.Interfaces;
using WIS.Domain.Produccion.Enums;
using WIS.Domain.Produccion;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class ProduccionResponse
    {
        public string NroIngresoProduccion { get; set; }

        public string IdProduccionExterno { get; set; }

        public int Empresa { get; set; }

        public string Predio { get; set; }

        public string Tipo { get; set; }

        public string EspacioProducion { get; set; }

        public string CodigoFormula { get; set; }

        public int? CantidadFormula { get; set; }

        public string IdModalidadLote { get; set; }

        public string GeneraPedido { get; set; }

        public int? Funcionario { get; set; }

        public short? Situacion { get; set; }

        public string FechaAlta { get; set; }

        public string FechaModificacion { get; set; }

        public string NroIngresoProduccionOriginal { get; set; }

        public string Anexo1 { get; set; }

        public string Anexo2 { get; set; }

        public string Anexo3 { get; set; }

        public string Anexo4 { get; set; }

        public string Anexo5 { get; set; }

        public string FechaInicioProduccion { get; set; }

        public string FechaFinProduccion { get; set; }

        public string IdManual { get; set; }

        public long? NroInterfazEjecucionEntrada { get; set; }

        public int? PosicionEnCola { get; set; }

        public string PermiteAutoAsignarLinea { get; set; }

        public long? NroUltInterfazEjecucion { get; set; }

        public List<ProduccionDetalleTeoricoResponse> DetallesTeoricos { get; set; }
        public List<ProduccionInsumoResponse> Insumos { get; set; }
        public List<ProduccionProducidosResponse> ProductosProducidos { get; set; }

        public ProduccionResponse()
        {
            DetallesTeoricos = new List<ProduccionDetalleTeoricoResponse>();
            Insumos = new List<ProduccionInsumoResponse>();
            ProductosProducidos = new List<ProduccionProducidosResponse>();
        }
    }

    public class ProduccionDetalleTeoricoResponse
    {
        public string Tipo { get; set; }

        public string Producto { get; set; }

        public string Identificador { get; set; }

        public decimal? CantidadTeorica { get; set; }

        public decimal? CantidadPedidoGenerada { get; set; }

        public decimal? CantidadAbastecido { get; set; }

        public decimal? CantidadConsumida { get; set; }

        public string Anexo1 { get; set; }

        public string Anexo2 { get; set; }

        public string Anexo3 { get; set; }

        public string Anexo4 { get; set; }
    }

    public class ProduccionInsumoResponse
    {
        public string Producto { get; set; }

        public string Identificador { get; set; }

        public decimal? CantidadReal { get; set; }

        public decimal? CantidadRealOriginal { get; set; }

        public decimal? CantidadDesafectada { get; set; }

        public decimal? CantidadNotificada { get; set; }

        public decimal? CantidadMerma { get; set; }

        public long? NuOrden { get; set; }

        public string Anexo1 { get; set; }

        public string Anexo2 { get; set; }

        public string Anexo3 { get; set; }

        public string Anexo4 { get; set; }

        public string Referencia { get; set; }
    }

    public class ProduccionProducidosResponse
    {
        public string Producto { get; set; }

        public string Identificador { get; set; }

        public string Vencimiento { get; set; }

        public decimal? CantidadTeorica { get; set; }

        public decimal? CantidadProducida { get; set; }

        public decimal? CantidadNotificada { get; set; }

        public string Motivo { get; set; }

        public string DescMotivo { get; set; }

        public long? NuOrden { get; set; }

        public string Anexo1 { get; set; }

        public string Anexo2 { get; set; }

        public string Anexo3 { get; set; }

        public string Anexo4 { get; set; }

        public long? NuPrdcIngresoTeorico { get; set; }
    }
}
