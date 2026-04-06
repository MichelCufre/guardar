using WIS.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;

namespace WIS.Domain.Services
{
    public class AjustesDeStockServiceContext : ServiceContext, IAjustesDeStockServiceContext
    {
        protected List<AjusteStock> _ajustes = new List<AjusteStock>();

        public HashSet<string> Predios { get; set; } = new HashSet<string>();
        public List<Stock> Stocks { get; set; } = new List<Stock>();
        public List<Stock> CantidadesLpns { get; set; } = new List<Stock>();
        public HashSet<Stock> SeriesExistentes { get; set; } = new HashSet<Stock>();
        public Dictionary<string, Producto> Productos { get; set; } = new Dictionary<string, Producto>();
        public Dictionary<string, Ubicacion> Ubicaciones { get; set; } = new Dictionary<string, Ubicacion>();
        public Dictionary<short, UbicacionArea> Areas { get; set; } = new Dictionary<short, UbicacionArea>();
        public Dictionary<string, MotivoAjuste> MotivosAjuste { get; set; } = new Dictionary<string, MotivoAjuste>();
        public Dictionary<short, UbicacionTipo> TiposUbicacion { get; set; } = new Dictionary<short, UbicacionTipo>();
        public Dictionary<string, UbicacionPickingProducto> UbicacionPickingProducto { get; set; } = new Dictionary<string, UbicacionPickingProducto>();

        public AjustesDeStockServiceContext(IUnitOfWork uow, List<AjusteStock> ajustes, int userId, int empresa) : base(uow, userId, empresa)
        {
            _ajustes = ajustes;
        }

        public async override Task Load()
        {
            var keysStock = new Dictionary<string, Stock>();
            var keysProducto = new Dictionary<string, Producto>();
            var keysUbicaciones = new Dictionary<string, Ubicacion>();
            var keysProductoSerie = new Dictionary<string, Producto>();
            var keysPickingProducto = new Dictionary<string, UbicacionPickingProducto>();

            await base.Load();

            foreach (var p in _uow.PredioRepository.GetPrediosUsuario(UserId))
            {
                Predios.Add(p.Numero);
            }
            foreach (var p in _uow.UbicacionAreaRepository.GetUbicacionAreas())
            {
                Areas[p.Id] = p;
            }
            foreach (var p in _uow.UbicacionTipoRepository.GetUbicacionTipos())
            {
                TiposUbicacion[p.Id] = p;
            }
            foreach (var p in _ajustes)
            {
                var keyProducto = $"{p.Producto}.{p.Empresa}";
                if (!keysProducto.ContainsKey(keyProducto))
                    keysProducto[keyProducto] = new Producto() { Codigo = p.Producto.Truncate(40), CodigoEmpresa = p.Empresa };

                var keyUbicacion = $"{p.Ubicacion}";
                if (!keysUbicaciones.ContainsKey(keyUbicacion))
                    keysUbicaciones[keyUbicacion] = new Ubicacion() { Id = p.Ubicacion.Truncate(40) };

                var keyPickingProducto = $"{p.Ubicacion}.{p.Producto}.{p.Empresa}";
                if (!keysPickingProducto.ContainsKey(keyPickingProducto))
                    keysPickingProducto[keyPickingProducto] = new UbicacionPickingProducto() { UbicacionSeparacion = p.Ubicacion.Truncate(40), CodigoProducto = p.Producto.Truncate(40), Empresa = p.Empresa };

                var keyStock = $"{p.Ubicacion}.{p.Producto}.{p.Empresa}.{p.Identificador}.{p.Faixa.ToString("#.###")}";
                if (!keysStock.ContainsKey(keyUbicacion))
                    keysStock[keyStock] = new Stock() { Ubicacion = p.Ubicacion.Truncate(40), Producto = p.Producto.Truncate(40), Empresa = p.Empresa, Identificador = p.Identificador.Truncate(40), Faixa = p.Faixa };
            }

            foreach (var p in _uow.ProductoRepository.GetProductos(keysProducto.Values))
            {
                p.AceptaDecimales = !string.IsNullOrEmpty(p.AceptaDecimalesId) && p.AceptaDecimalesId == "S";
                p.ManejoIdentificador = (new ProductoMapper()).MapManejoIdentificador(p.ManejoIdentificadorId);
                Productos[p.Codigo] = p;

                if (p.ManejoIdentificador == ManejoIdentificador.Serie)
                    keysProductoSerie[p.Codigo] = p;
            }

            foreach (var u in _uow.UbicacionRepository.GetUbicaciones(keysUbicaciones.Values))
            {
                u.EsUbicacionBaja = (u.IdUbicacionBaja ?? "N") == "S";
                u.EsUbicacionSeparacion = (u.IdUbicacionSeparacion ?? "N") == "S";
                u.NecesitaReabastecer = (u.IdNecesitaReabastecer ?? "N") == "S";

                Ubicaciones[u.Id] = u;
            }

            foreach (var p in _uow.UbicacionPickingProductoRepository.GetUbicacionPickingProducto(keysPickingProducto.Values))
            {
                var keyPickingProducto = $"{p.UbicacionSeparacion}.{p.CodigoProducto}.{p.Empresa}";
                UbicacionPickingProducto[keyPickingProducto] = p;
            }

            foreach (var d in _uow.AjusteRepository.GetsMotivosAjuste())
            {
                MotivosAjuste[d.Codigo] = d;
            }

            Stocks = _uow.StockRepository.GetStocks(keysStock.Values).ToList();
            CantidadesLpns = _uow.ManejoLpnRepository.GetStocksLpn(keysStock.Values).ToList();

            if (keysProductoSerie.Count > 0)
                SeriesExistentes = _uow.StockRepository.GetLotesExistente(keysProductoSerie.Values).ToHashSet();
        }

        public virtual Ubicacion GetUbicacion(string ubicacion)
        {
            return Ubicaciones.GetValueOrDefault(ubicacion, null);
        }

        public virtual Producto GetProducto(int empresa, string codigo)
        {
            return Productos.GetValueOrDefault(codigo, null);
        }

        public virtual UbicacionArea GetAreaUbic(short idUbicacionArea)
        {
            return Areas.GetValueOrDefault(idUbicacionArea, null);
        }

        public virtual UbicacionTipo GetTipoUbicacion(short idUbicacionTipo)
        {
            return TiposUbicacion.GetValueOrDefault(idUbicacionTipo, null);
        }

        public virtual UbicacionPickingProducto GetUbicacionPickingProducto(string cdEndereco, string cdProducto, int cdEmpresa)
        {
            var keyubicacionPickingProducto = $"{cdEndereco}.{cdProducto}.{cdEmpresa}";
            return UbicacionPickingProducto.GetValueOrDefault(keyubicacionPickingProducto, null);
        }

        public virtual Stock GetStock(AjusteStock data)
        {
            return Stocks.FirstOrDefault(x => x.Ubicacion == data.Ubicacion
                && x.Producto == data.Producto
                && x.Empresa == data.Empresa
                && x.Identificador == data.Identificador
                && x.Faixa == data.Faixa);
        }

        public virtual decimal GetCantidadDisponibleLpn(AjusteStock data)
        {
            return CantidadesLpns.FirstOrDefault(x => x.Ubicacion == data.Ubicacion
                && x.Producto == data.Producto
                && x.Empresa == data.Empresa
                && x.Identificador == data.Identificador
                && x.Faixa == data.Faixa)?.CantidadDisponibleLpn ?? 0;
        }

        public virtual decimal GetCantidadSuelta(AjusteStock data)
        {
            var cantStock = GetStock(data).Cantidad ?? 0;
            var cantDisponibleLpn = GetCantidadDisponibleLpn(data);
            return cantStock - cantDisponibleLpn;
        }

        public virtual bool ExistePredio(string predio)
        {
            return Predios.Contains(predio);
        }

        public virtual bool ExisteStock(AjusteStock data)
        {
            return Stocks.Any(x => x.Ubicacion == data.Ubicacion
                && x.Producto == data.Producto
                && x.Empresa == data.Empresa
                && x.Identificador == data.Identificador
                && x.Faixa == data.Faixa);
        }

        public virtual bool AnyProductoEnUbicacion(string cdEndereco, string cdProducto, int cdEmpresa)
        {
            return Stocks.Any(x => x.Ubicacion == cdEndereco && (x.Producto != cdProducto || x.Empresa != cdEmpresa));
        }

        public virtual bool AnyProductoLoteEnUbicacion(string cdEndereco, string cdProducto, int cdEmpresa, string lote)
        {
            return Stocks.Any(x => x.Ubicacion == cdEndereco && x.Producto == cdProducto && x.Empresa == cdEmpresa && x.Identificador != lote);
        }

        public virtual bool AnyMotivoAjuste(string cdMotivoAjuste)
        {
            return MotivosAjuste.Keys.Any(x => x == cdMotivoAjuste);
        }

        public virtual bool ExisteSerie(string codigoProducto, string identificador)
        {
            return SeriesExistentes.Any(s => s.Producto == codigoProducto && s.Identificador == identificador);
        }
    }
}
