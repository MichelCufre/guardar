using WIS.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;

namespace WIS.Domain.Services
{
    public class TransferenciaStockServiceContext : ServiceContext, ITransferenciaStockServiceContext
    {
        protected List<TransferenciaStock> _transferencias = new List<TransferenciaStock>();

        public string UbicacionEquipo { get; set; } = string.Empty;
        public HashSet<string> Predios { get; set; } = new HashSet<string>();
        public HashSet<Stock> Stocks { get; set; } = new HashSet<Stock>();
        public List<Stock> CantidadesLpns { get; set; } = new List<Stock>();
        public Dictionary<string, Producto> Productos { get; set; } = new Dictionary<string, Producto>();
        public Dictionary<string, Ubicacion> Ubicaciones { get; set; } = new Dictionary<string, Ubicacion>();
        public Dictionary<short, UbicacionArea> Areas { get; set; } = new Dictionary<short, UbicacionArea>();
        public Dictionary<short, UbicacionTipo> TiposUbicacion { get; set; } = new Dictionary<short, UbicacionTipo>();
        public Dictionary<string, UbicacionPickingProducto> PickingsProducto { get; set; } = new Dictionary<string, UbicacionPickingProducto>();

        public TransferenciaStockServiceContext(IUnitOfWork uow, List<TransferenciaStock> transferencias, int userId, int empresa) : base(uow, userId, empresa)
        {
            _transferencias = transferencias;
        }

        public async override Task Load()
        {
            var keysStock = new Dictionary<string, Stock>();
            var keysProducto = new Dictionary<string, Producto>();
            var keysUbicaciones = new Dictionary<string, Ubicacion>();
            var keysPickingProducto = new Dictionary<string, UbicacionPickingProducto>();

            Predios = _uow.PredioRepository.GetPrediosUsuario(UserId).Select(p => p.Numero).ToHashSet();
            Areas = _uow.UbicacionAreaRepository.GetUbicacionAreas().ToDictionary(a => a.Id, a => a);
            TiposUbicacion = _uow.UbicacionTipoRepository.GetUbicacionTipos().ToDictionary(t => t.Id, t => t);

            foreach (var t in _transferencias)
            {
                var keyProducto = $"{t.Producto}.{t.Empresa}";
                if (!keysProducto.ContainsKey(keyProducto))
                    keysProducto[keyProducto] = new Producto() { Codigo = t.Producto.Truncate(40), CodigoEmpresa = t.Empresa };

                if (!keysUbicaciones.ContainsKey(t.Ubicacion))
                    keysUbicaciones[t.Ubicacion] = new Ubicacion() { Id = t.Ubicacion.Truncate(40) };

                if (!keysUbicaciones.ContainsKey(t.UbicacionDestino))
                    keysUbicaciones[t.UbicacionDestino] = new Ubicacion() { Id = t.UbicacionDestino.Truncate(40) };

                var keyPickingProducto = $"{t.UbicacionDestino}.{t.Producto}.{t.Empresa}";
                if (!keysPickingProducto.ContainsKey(keyPickingProducto))
                    keysPickingProducto[keyPickingProducto] = new UbicacionPickingProducto() { UbicacionSeparacion = t.UbicacionDestino.Truncate(40), CodigoProducto = t.Producto.Truncate(40), Empresa = t.Empresa };

                var keyStockOrigen = $"{t.Ubicacion}.{t.Producto}.{t.Empresa}.{t.Identificador}.{t.Faixa.ToString("#.###")}";
                if (!keysStock.ContainsKey(keyStockOrigen))
                    keysStock[keyStockOrigen] = new Stock() { Ubicacion = t.Ubicacion.Truncate(40), Producto = t.Producto.Truncate(40), Empresa = t.Empresa, Identificador = t.Identificador.Truncate(40), Faixa = t.Faixa };

                var keyStockDestino = $"{t.UbicacionDestino}.{t.Producto}.{t.Empresa}.{t.Identificador}.{t.Faixa.ToString("#.###")}";
                if (!keysStock.ContainsKey(keyStockDestino))
                    keysStock[keyStockDestino] = new Stock() { Ubicacion = t.UbicacionDestino.Truncate(40), Producto = t.Producto.Truncate(40), Empresa = t.Empresa, Identificador = t.Identificador.Truncate(40), Faixa = t.Faixa };

            }

            foreach (var p in _uow.ProductoRepository.GetProductos(keysProducto.Values))
            {
                p.AceptaDecimales = !string.IsNullOrEmpty(p.AceptaDecimalesId) && p.AceptaDecimalesId == "S";
                p.ManejoIdentificador = (new ProductoMapper()).MapManejoIdentificador(p.ManejoIdentificadorId);

                var keyProducto = $"{p.Codigo}.{p.CodigoEmpresa}";
                Productos[keyProducto] = p;
            }
                        
            foreach (var u in _uow.UbicacionRepository.GetUbicaciones(keysUbicaciones.Values))
            {
                u.EsUbicacionBaja = (u.IdUbicacionBaja ?? "N") == "S";
                u.EsUbicacionSeparacion = (u.IdUbicacionSeparacion ?? "N") == "S";
                u.NecesitaReabastecer = (u.IdNecesitaReabastecer ?? "N") == "S";

                Ubicaciones[u.Id] = u;
            }

            var predio = Ubicaciones.Values.FirstOrDefault()?.NumeroPredio;
            AddParametroPredio(ParamManager.API_UBICACION_EQUIPO, predio);

            PickingsProducto = _uow.UbicacionPickingProductoRepository.GetUbicacionPickingProducto(keysPickingProducto.Values)
                                .ToDictionary(p => $"{p.UbicacionSeparacion}.{p.CodigoProducto}.{p.Empresa}", p => p);

            await base.Load();

            SetUbicacionEquipo();
            foreach (var t in _transferencias)
            {
                var keyStockEquipo = $"{UbicacionEquipo}.{t.Producto}.{t.Empresa}.{t.Identificador}.{t.Faixa.ToString("#.###")}";
                if (!keysStock.ContainsKey(keyStockEquipo))
                    keysStock[keyStockEquipo] = new Stock() { Ubicacion = UbicacionEquipo, Producto = t.Producto.Truncate(40), Empresa = t.Empresa, Identificador = t.Identificador.Truncate(40), Faixa = t.Faixa };
            }

            Stocks = _uow.StockRepository.GetStocks(keysStock.Values).ToHashSet();
            CantidadesLpns = _uow.ManejoLpnRepository.GetStocksLpn(keysStock.Values).ToList();
        }

        #region Metodos
        public virtual bool PredioUnico()
        {
            return Ubicaciones.Values.Min(u => u.NumeroPredio) == Ubicaciones.Values.Max(u => u.NumeroPredio);
        }

        public virtual bool ExisteUbicacion(string key)
        {
            return Ubicaciones.TryGetValue(key, out Ubicacion u);
        }

        public virtual bool ExisteAsignacionPicking(string ubicacion, string producto, int empresa)
        {
            var key = $"{ubicacion}.{producto}.{empresa}";
            return PickingsProducto.TryGetValue(key, out UbicacionPickingProducto v);
        }

        public virtual bool AnyProductoUbicacion(string ubicacion, string producto, int empresa)
        {
            return Stocks.Any(x => x.Ubicacion == ubicacion && (x.Producto != producto || x.Empresa != empresa));
        }

        public virtual bool AnyProductoLoteUbicacion(string ubicacion, string producto, int empresa, string lote)
        {
            return Stocks.Any(x => x.Ubicacion == ubicacion && x.Producto == producto && x.Empresa == empresa && x.Identificador != lote);
        }

        public virtual bool UbicacionEquipoValida()
        {
            return !string.IsNullOrEmpty(UbicacionEquipo);
        }

        public virtual UbicacionArea GetArea(short area)
        {
            return Areas.GetValueOrDefault(area, null);
        }

        public virtual Ubicacion GetUbicacion(string ubicacion)
        {
            return Ubicaciones.GetValueOrDefault(ubicacion, null);
        }

        public virtual UbicacionTipo GetTipoUbicacion(short tipo)
        {
            return TiposUbicacion.GetValueOrDefault(tipo, null);
        }

        public virtual Producto GetProducto(string codigo, int empresa)
        {
            var key = $"{codigo}.{empresa}";
            return Productos.GetValueOrDefault(key, null);
        }

        public virtual Stock GetStock(string ubicacion, string producto, int empresa, string identificador, decimal faixa)
        {
            return Stocks.FirstOrDefault(x => x.Ubicacion == ubicacion
                && x.Producto == producto
                && x.Empresa == empresa
                && x.Identificador == identificador
                && x.Faixa == faixa);
        }

        public virtual decimal GetCantidadDisponibleLpn(string ubicacion, string producto, int empresa, string identificador, decimal faixa)
        {
            return CantidadesLpns.FirstOrDefault(x => x.Ubicacion == ubicacion
                && x.Producto == producto
                && x.Empresa == empresa
                && x.Identificador == identificador
                && x.Faixa == faixa)?.CantidadDisponibleLpn ?? 0;
        }

        public virtual decimal GetCantidadDisponible(Stock sto)
        {
            var cantStockDisponible = sto.CantidadDisponible();
            var cantDisponibleLpn = GetCantidadDisponibleLpn(sto.Ubicacion, sto.Producto, sto.Empresa, sto.Identificador, sto.Faixa);
            return cantStockDisponible - cantDisponibleLpn;
        }

        public virtual DateTime? GetVencimientoStock(TransferenciaStock data, bool destino = false)
        {
            return Stocks.FirstOrDefault(x => x.Ubicacion == (destino ? data.UbicacionDestino : data.Ubicacion)
                    && x.Producto == data.Producto && x.Empresa == data.Empresa && x.Identificador == data.Identificador && x.Faixa == data.Faixa)?.Vencimiento;
        }

        public virtual string GetPredioOperacion()
        {
            return Ubicaciones.Values.FirstOrDefault()?.NumeroPredio;
        }

        public virtual void SetUbicacionEquipo()
        {
            var predio = GetPredioOperacion();
            var ubicacionEquipo = _uow.UbicacionRepository.GetUbicacionEquipo(UserId, predio);

            if (!string.IsNullOrEmpty(ubicacionEquipo))
                UbicacionEquipo = ubicacionEquipo;
            else
            {
                var param = GetParametro(ParamManager.API_UBICACION_EQUIPO);
                var ubicacionParam = _uow.UbicacionRepository.GetUbicacion(param);
                if (ubicacionParam != null
                    && _uow.EquipoRepository.AnyEquipoManualByUbicacion(ubicacionParam.Id)
                    && ubicacionParam.NumeroPredio == predio
                    && ubicacionParam.IdUbicacionArea == AreaUbicacionDb.Equipamiento)
                {
                    UbicacionEquipo = ubicacionParam.Id;
                }
            }
        }

    }

    #endregion
}

