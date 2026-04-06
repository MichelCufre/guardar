using WIS.Domain.Produccion.Models;
using System;
using System.Collections.Generic;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.Produccion;
using WIS.Domain.StockEntities;
using WIS.TrafficOfficer;

namespace WIS.Domain.Produccion.Interfaces
{
    public interface ILogicaProduccion
    {
        IngresoProduccion CrearIngresoProduccion(string tipoIngreso, int empresa, string predio, List<IngresoProduccionDetalleTeorico> detalles, string idExterno = null, string idEspacioProduccion = null);

        bool TieneEspacioProduccion();

        void AddIngresoProduccion();

        int GetEmpresa();

        string GetPredio();

        void AsociarEspacioProduccion(string idEspacio);

        void AddDetalleTeorico(IngresoProduccionDetalleTeorico detalle);

        void UpdateDetalleTeorico(IngresoProduccionDetalleTeorico detalle);

        void DeleteDetalleTeorico(IngresoProduccionDetalleTeorico detalle);

        IngresoProduccionDetalleTeorico GetDetalleTeorico(int idDetalle);

        EspacioProduccion GetEspacioProduccion();

        IngresoProduccionDetalleReal GetInsumoProduccion(long idInsumo);

        void UpdateSituacion(short sitacion);

        bool IsSituacion(short sitacion);

        bool PuedeIniciarProduccion(out string mensaje, out List<string> errorArg);

        bool ProduccionHabilitadaParaFabricar();

        bool ProduccionHabilitadaParaNotificar();

        bool ProduccionEnProcesoDeNotificacion();

        bool HayPendientesDeNotificacion();

        bool EsProductoEsperado(int empresa, string producto, string tipoRegistro);

        bool HayDiferenciasEnProduccion();

        void AddDetalleProductoNoEsperado(string ubicacion, string producto, decimal faixa, int empresa, string lote, decimal producido, string codMotivo, string dsMotivo, DateTime? dtVencimiento);

        void GenerarProductoNoEsperado(Producto producto, decimal faixa, int empresa, string lote, decimal producido, DateTime? vencimiento, string codMotivo, string dsMotivo, out string keyAjuste);

        List<IngresoProduccionDetalleReal> GetInsumosProduccion();

        void AddInsumoProduccion(IngresoProduccionDetalleReal detalle);

        Pedido GenerarPedido(List<IngresoProduccionDetallePedidoTemporal> detallesTeporalesPedidoInsumos);

        void DefinirLotesPedido(Pedido pedido, List<DetallePedido> detallesDefinidos, IFormatProvider format);

        void ConsumirInsumoCompleto(long idInsumo, string ubicacion, decimal cantidadConsumir, out DateTime? vencimiento, bool isConsumible = false);

        void DesafectarInsumo(long idInsumo, string ubicacion, ITrafficOfficerService concurrencyControl, TrafficOfficerTransaction transactionTO);

        void DesafectarInsumosConSaldo(string ubicacion, ITrafficOfficerService concurrencyControl, TrafficOfficerTransaction transactionTO);

        void ConsumirInsumoParcial(long idInsumo, string ubicacion, decimal qtConsumir, out DateTime? vencimiento, bool isConsumible = false);

        abstract void UpdatePedido(Pedido pedido);

        void AfectarSobrantes(string cdProducto, string nuIdentificador, int cdEmpresa, decimal cdFaixa, decimal qtAfectar, string ubicacion);

        void DesafectarSobrantes(string cdProducto, string nuIdentificador, int cdEmpresa, decimal cdFaixa, decimal qtDesafectar, string ubicacion);

        void FinalizarProduccion();

        void IniciarProduccion();

        IngresoProduccionDetalleReal ExisteIngresoReal(string codigoProducto, string identificador);

        IngresoProduccion GetIngresoProduccion();
    }
}
