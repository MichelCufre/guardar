using WIS.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;

namespace WIS.Domain.Services
{
    public class PickingProductoServiceContext : ServiceContext
    {
        protected int _empresa = 0;
        protected int _userId = 0;

        private List<UbicacionPickingProducto> _ubicacionesPicking = new List<UbicacionPickingProducto>();

        public List<Empresa> Empresas = new List<Empresa>();

        public List<DominioDetalle> CodigosUnidadCajaAutValidos = new List<DominioDetalle>();

        public Dictionary<string, Producto> Productos = new Dictionary<string, Producto>();

        public Dictionary<string, UbicacionPickingProducto> UbicacionPickingProducto = new Dictionary<string, UbicacionPickingProducto>();

        public Dictionary<string, Ubicacion> Ubicaciones = new Dictionary<string, Ubicacion>();

        public Dictionary<string, Ubicacion> UbicacionesAutomatismo = new Dictionary<string, Ubicacion>();

        public Dictionary<string, bool> UbicacionesValidaPicking = new Dictionary<string, bool>();

        public Dictionary<string, bool> UbicacionesYaExistentes = new Dictionary<string, bool>();

        public Dictionary<string, bool> UbicacionesInvalidaMonoProducto = new Dictionary<string, bool>();

        public Dictionary<string, bool> UbicacionesInvalidaMonoProductoOtroStock = new Dictionary<string, bool>();

        public Dictionary<string, bool> UbicacionesInvalidaClaseProducto = new Dictionary<string, bool>();

        public PickingProductoServiceContext(IUnitOfWork uow, List<UbicacionPickingProducto> ubicacionesPicking, int userId, int empresa) : base(uow, userId, empresa)
        {
            _userId = userId;
            _ubicacionesPicking = ubicacionesPicking;
            _empresa = ubicacionesPicking[0].Empresa;
        }

        public virtual bool ExisteUbicacion(string ubicacion)
        {
            return Ubicaciones.ContainsKey(ubicacion);
        }

        public virtual bool EsUbicacionAutomatismo(string ubicacion)
        {
            if (UbicacionesAutomatismo.ContainsKey(ubicacion))
                return true;
            else
                return false;
        }

        public virtual bool ExisteEmpresa(int empresa)
        {
            return Empresas.Any(u => u.Id == empresa);
        }

        public virtual bool ExisteCodigoCajaAutomatismo(string codigo)
        {
            return CodigosUnidadCajaAutValidos.Any(u => u.Valor == codigo);
        }

        public virtual bool ExisteProducto(string producto, int empresa)
        {
            var keyProducto = $"{empresa}.{producto}";
            return Productos.ContainsKey(keyProducto);
        }

        public virtual bool UbicacionValida(string _ubicacion)
        {
            return UbicacionesValidaPicking.TryGetValue(_ubicacion, out var esValida) && esValida;
        }

        public virtual bool ExisteUbicacionPicking(string producto, int empresa, int padron, string predio, int prioridad)
        {
            var keyUbicacionInvalida = $"{producto}.{empresa}.{padron}.{predio}.{prioridad}";
            return UbicacionesYaExistentes.ContainsKey(keyUbicacionInvalida);
        }

        public virtual bool EsUbicacionInvalidaMonoProducto(string ubicacion)
        {
            return UbicacionesInvalidaMonoProducto.ContainsKey(ubicacion);
        }

        public virtual bool EsUbicacionInvalidaMonoProductoOtroStock(string ubicacion)
        {
            return UbicacionesInvalidaMonoProductoOtroStock.ContainsKey(ubicacion);
        }

        public virtual bool EsUbicacionInvalidaClaseProducto(string ubicacion)
        {
            return UbicacionesInvalidaClaseProducto.ContainsKey(ubicacion);
        }

        public async override Task Load()
        {
            await base.Load();

            var tiposOperaciones = new Dictionary<string, string>();
            var keysProductos = new Dictionary<string, Producto>();
            var keysPickingProducto = new Dictionary<string, UbicacionPickingProducto>();
            var keysUbicaciones = new Dictionary<string, Ubicacion>();

            Empresas = _uow.EmpresaRepository.GetEmpresas();
            CodigosUnidadCajaAutValidos = _uow.DominioRepository.GetDominios("UNDCAJAAUT");

            foreach (var ubicacionPicking in _ubicacionesPicking)
            {
                var keyProducto = $"{ubicacionPicking.CodigoProducto}.{_empresa}";
                tiposOperaciones[$"{ubicacionPicking.Empresa}.{ubicacionPicking.CodigoProducto}.{ubicacionPicking.Ubicacion}"] = ubicacionPicking.TipoOperacionId;
                keysProductos[keyProducto] = new Producto() { Codigo = ubicacionPicking.CodigoProducto.Truncate(40), CodigoEmpresa = _empresa };

                var keyPickingProducto = $"{ubicacionPicking.UbicacionSeparacion}.{ubicacionPicking.CodigoProducto}.{ubicacionPicking.Empresa}.{ubicacionPicking.Padron}.{ubicacionPicking.Prioridad}";

                if (!keysPickingProducto.ContainsKey(keyPickingProducto))
                    keysPickingProducto[keyPickingProducto] = new UbicacionPickingProducto() { UbicacionSeparacion = ubicacionPicking.UbicacionSeparacion.Truncate(40), CodigoProducto = ubicacionPicking.CodigoProducto.Truncate(40), Empresa = ubicacionPicking.Empresa, Padron = ubicacionPicking.Padron, Prioridad = ubicacionPicking.Prioridad };

                var keyUbicacion = $"{ubicacionPicking.UbicacionSeparacion}";

                if (!keysUbicaciones.ContainsKey(keyUbicacion))
                    keysUbicaciones[keyUbicacion] = new Ubicacion() { Id = ubicacionPicking.UbicacionSeparacion.Truncate(40) };

                var predio = _uow.UbicacionRepository.GetPredio(ubicacionPicking.UbicacionSeparacion);
                var padron = (int?)ubicacionPicking.Padron;

                if (padron != null && padron != 0)
                {
                    var _padron = padron.Value;
                    var ubicacionPickingProductoPadronPrioridadExiste = _uow.UbicacionPickingProductoRepository.AnyUbicacionProductoPadronPrioridad(ubicacionPicking.CodigoProducto, ubicacionPicking.Empresa, _padron, predio, ubicacionPicking.Prioridad);

                    if (ubicacionPickingProductoPadronPrioridadExiste)
                    {
                        var keyUbicacionInvalida = $"{ubicacionPicking.CodigoProducto}.{ubicacionPicking.Empresa}.{_padron}.{predio}.{ubicacionPicking.Prioridad}";
                        UbicacionesYaExistentes[keyUbicacionInvalida] = true;
                    }
                }

                var tipoUbicacion = _uow.UbicacionTipoRepository.GetTipoByUbicacion(ubicacionPicking.UbicacionSeparacion);

                if (tipoUbicacion != null)
                {
                    if (!tipoUbicacion.PermiteVariosProductos)
                    {
                        if (_uow.UbicacionPickingProductoRepository.AnyUbicacionPickingOtroProducto(ubicacionPicking.UbicacionSeparacion, _empresa, ubicacionPicking.CodigoProducto))
                        {
                            var keyUbicacionInvalida = $"{ubicacionPicking.UbicacionSeparacion}";
                            UbicacionesInvalidaMonoProducto[keyUbicacionInvalida] = true;
                        }
                        else if (_uow.StockRepository.AnyStockOtroProducto(_empresa, ubicacionPicking.CodigoProducto, ubicacionPicking.UbicacionSeparacion))
                        {
                            var keyUbicacionInvalida = $"{ubicacionPicking.UbicacionSeparacion}";
                            UbicacionesInvalidaMonoProductoOtroStock[keyUbicacionInvalida] = true;
                        }
                    }

                    if (tipoUbicacion.RespetaClase)
                    {
                        var producto = _uow.ProductoRepository.GetProducto(_empresa, ubicacionPicking.CodigoProducto);
                        var ubicacion = _uow.UbicacionRepository.GetUbicacion(ubicacionPicking.UbicacionSeparacion);

                        if (producto != null)
                        {
                            if (ubicacion.CodigoClase != producto.CodigoClase)
                            {
                                var keyUbicacionInvalida = $"{ubicacionPicking.UbicacionSeparacion}";
                                UbicacionesInvalidaClaseProducto[keyUbicacionInvalida] = true;
                            }
                        }
                    }
                }
            }

            foreach (var producto in _uow.ProductoRepository.GetProductos(keysProductos.Values))
            {
                producto.AceptaDecimales = !string.IsNullOrEmpty(producto.AceptaDecimalesId) && producto.AceptaDecimalesId == "S";
                producto.ManejoIdentificador = (new ProductoMapper()).MapManejoIdentificador(producto.ManejoIdentificadorId);

                var keyProducto = $"{producto.CodigoEmpresa}.{producto.Codigo}";
                Productos[keyProducto] = producto;
            }

            foreach (var ubicacionPickingProducto in _uow.UbicacionPickingProductoRepository.GetUbicacionPickingProductoByKeys(keysPickingProducto.Values))
            {
                var _padron = (int?)ubicacionPickingProducto.Padron;

                ubicacionPickingProducto.Padron = Math.Truncate(ubicacionPickingProducto.Padron);

                var keyPickingProducto = $"{ubicacionPickingProducto.UbicacionSeparacion}.{ubicacionPickingProducto.CodigoProducto}.{ubicacionPickingProducto.Empresa}.{_padron}.{ubicacionPickingProducto.Prioridad}";
                UbicacionPickingProducto[keyPickingProducto] = ubicacionPickingProducto;
            }

            foreach (var ubicacion in _uow.UbicacionRepository.GetUbicaciones(keysUbicaciones.Values))
            {
                Ubicaciones[ubicacion.Id] = ubicacion;

                var esUbicacionAutomatismo = _uow.AutomatismoRepository.IsUbicacionAutomatismo(ubicacion.Id);

                if (esUbicacionAutomatismo)
                    UbicacionesAutomatismo[ubicacion.Id] = ubicacion;

                var ubicacionValidaPicking = _uow.UbicacionRepository.AnyUbicacionValidaPicking(ubicacion.Id);

                if (ubicacionValidaPicking)
                    UbicacionesValidaPicking[ubicacion.Id] = true;
                else
                    UbicacionesValidaPicking[ubicacion.Id] = false;
            }
        }

        public virtual Producto GetProducto(int empresa, string codigo)
        {
            var keyProducto = $"{empresa}.{codigo}";
            return Productos.GetValueOrDefault(keyProducto, null);
        }

        public virtual Ubicacion GetUbicacion(string ubicacion)
        {
            return Ubicaciones.GetValueOrDefault(ubicacion, null);
        }

        public virtual UbicacionPickingProducto GetUbicacionPicking(string cdEndereco, string cdProducto, int cdEmpresa, decimal _padron, int prioridad)
        {
            var padron = (int?)_padron;

            if (padron != null && padron != 0)
            {
                var padronValue = padron.Value;

                var keyubicacionPickingProducto = $"{cdEndereco}.{cdProducto}.{cdEmpresa}.{padronValue}.{prioridad}";
                return UbicacionPickingProducto.GetValueOrDefault(keyubicacionPickingProducto, null);
            }

            return null;
        }

        public virtual string GetPredioUbicacion(string cdEndereco)
        {
            if (Ubicaciones.TryGetValue(cdEndereco, out var ubicacion))
                return ubicacion.NumeroPredio;
            else
                return null;
        }
    }
}
