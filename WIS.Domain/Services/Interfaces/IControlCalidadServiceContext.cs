using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.Recepcion;
using WIS.Domain.StockEntities;

namespace WIS.Domain.Services.Interfaces
{
    public interface IControlCalidadServiceContext : IServiceContext
    {
        bool NextNuevaId { get; }
        int NextNuevaIdValue { get; }
        bool NextNuevaInstancia { get; }
        long NextNuevaInstanciaValue { get; }

        Task Load();

        void AsignarTipoCriterio(CriterioControlCalidadAPI criterio);
        void AsignarTipoCriterioControl(ControlCalidadAPI control);
        bool EtiquetaExternaExiste(string predio, string etiquetaExterno, out int cantidad);
        bool EtiquetaExternaExiste(string predio, string etiquetaExterno, string tipo);
        bool ExisteCodigoControl(int codControl);
        bool ExisteControlPendiente(string predio, int codigo, string producto, string lote, decimal faixa, int empresa);
        bool ExisteControlPendiente(string predio, int codigo, string producto, string lote, decimal faixa, int empresa, int nuEtiquetaLote);
        bool ExisteControlPendiente(string predio, int codigo, string producto, string lote, decimal faixa, int empresa, long nuLpn, int idDetLpn);
        bool ExisteControlPendiente(string predio, int codigo, string producto, string lote, decimal faixa, int empresa, string ubicacion);
        ControlDeCalidadPendiente GenerarControl(int codigoControl, string descripcion, string ubicacion, long instancia, CriterioControlCalidadAPI criterio, EtiquetaLote etiqueta = null, Lpn lpn = null, LpnDetalle detalle = null);
        ControlDeCalidadPendiente GenerarControlCalidadEtiquetaProcess(CriterioControlCalidadAPI criterio, int codigoControl, string descripcion, EtiquetaLote etiqueta, long instancia);
        ControlDeCalidadPendiente GenerarControlCalidadEtiquetaProcess(CriterioControlCalidadAPI criterio, int codigoControl, string descripcion, EtiquetaLote etiqueta, Lpn lpn, long instancia, LpnDetalle detalle);
        ControlDeCalidadPendiente GenerarControlCalidadLpnProcess(CriterioControlCalidadAPI criterio, int codigoControl, string descripcion, Lpn lpn, long instancia, ref LpnDetalle detalle);
        ControlDeCalidadPendiente GenerarControlCalidadUbicacionProcess(CriterioControlCalidadAPI criterio, int codigoControl, string descripcion, long instancia, ref Stock stock);
        LpnDetalle GetDetalleLpn(long NroLpn, int NroDetalle);
        Lpn GetEtiquetaLpnExterno(string predio, string etiquetaExterno);
        List<Lpn> GetEtiquetaLpnExterno(string predio, string producto, int empresa, string identificador, decimal faixa);
        Lpn GetEtiquetaLpnExterno(string predio, string etiquetaExterno, string tipo);
        EtiquetaLote GetEtiquetaRecepcionExterno(string predio, string etiquetaExterno);
        List<EtiquetaLote> GetEtiquetaRecepcionExterno(string predio, string producto, int empresa, string identificador, decimal faixa);
        EtiquetaLote GetEtiquetaRecepcionExterno(string predio, string etiquetaExterno, string tipo);
        List<ControlDeCalidadPendiente> GetPorAprobarEtiqueta();
        List<ControlDeCalidadPendiente> GetPorAprobarUbicacion();
        Producto GetProducto(string producto, int empresa);
        List<Stock> GetStock(string predio, string producto, string lote, int empresa, decimal faixa);
        Stock GetStock(string predio, string producto, string lote, int empresa, string ubicacion, decimal faixa);
        decimal GetStockAfectadoEnEtiquetaRecepcion(Stock stock, List<EtiquetaLote> etiquetas);
        decimal GetStockAfectadoEnLpn(Stock stock);
        void LoadControl(List<int> controles);
        void LoadControlesPendientes(List<CriterioControlCalidadAPI> criterios);
        void LoadDetallesEtiquetas();
        void LoadDetallesLpn();
        void LoadEtiquetas(List<CriterioControlCalidadAPI> criterios);
        void LoadLpn(List<CriterioControlCalidadAPI> criterios);
        void LoadNewIds(int cant);
        void LoadNewInstancias(int cant);
        void LoadPredios();
        void LoadProductos();
        void LoadStock(List<CriterioControlCalidadAPI> criterios);
        bool LpnExternoExiste(string predio, string etiquetaExterno, out int cantidad);
        bool LpnExternoExiste(string predio, string etiquetaExterno, string tipo);
        bool PredioExiste(string predio);
        bool ProductoEnUbicacion(string ubicacion, int empresa, string producto, string identificador, decimal faixa);
        bool ProductoExiste(string producto, int empresa);
        void SetStockControlCalidadPendiente();
        bool StockLibreNoDisponible(string ubicacion, int empresa, string producto, string identificador, decimal faixa);
        bool StockLpnNoDisponible(decimal qtStockLpn, long nuLpn, string ubicacion, int empresa, string producto, string identificador, decimal faixa, int idLpnDet);
        bool TieneStockLibre(Stock stock, List<EtiquetaLote> etiquetas);
        bool UbicacionAreaDisponible(string codigoUbicacion);
        bool UbicacionExiste(string ubicacion);
        bool UbicacionLpnAreaDisponible(string codigoUbicacion);
    }
}