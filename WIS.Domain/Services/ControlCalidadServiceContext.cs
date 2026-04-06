using WIS.Extension;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.General.Enums;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;

namespace WIS.Domain.Services
{
    public class ControlCalidadServiceContext : ServiceContext, IControlCalidadServiceContext
    {
        protected ControlCalidadResponse _response;

        protected int _contadorInstancias;
        protected int _contadorIds;

        public bool NextNuevaInstancia => _nuevasInstancias != null || _contadorInstancias < _nuevasInstancias.Count;
        public long NextNuevaInstanciaValue
        {
            get
            {
                long toReturn = _nuevasInstancias[_contadorInstancias];
                _contadorInstancias++;

                return toReturn;
            }
        }

        public bool NextNuevaId => _nuevasIds != null || _contadorIds < _nuevasIds.Count;
        public int NextNuevaIdValue
        {
            get
            {
                int toReturn = _nuevasIds[_contadorIds];
                _contadorIds++;

                return toReturn;
            }
        }

        #region Collections

        protected readonly Dictionary<string, Producto> _productosCarga;
        protected readonly Dictionary<string, Ubicacion> _ubicacionesCarga;

        protected readonly List<ControlCalidadAPI> _controles;

        protected readonly HashSet<int> _controlesTipos;
        protected List<long> _nuevasInstancias;
        protected List<int> _nuevasIds;

        protected readonly HashSet<string> _predios;
        protected readonly Dictionary<string, Producto> _productos;
        protected readonly Dictionary<string, Ubicacion> _ubicaciones;
        protected readonly Dictionary<short, UbicacionArea> _areas;
        protected readonly List<EtiquetaLote> _etiquetas;
        protected readonly List<Lpn> _lpns;
        protected readonly List<Stock> _stocks;
        protected readonly List<ControlDeCalidadPendiente> _controlesCalidadPendiente;

        #endregion

        public ControlCalidadServiceContext(IUnitOfWork uow, List<ControlCalidadAPI> controles, int userId, int empresa) : base(uow, userId, empresa)
        {
            _controles = controles;
            _productosCarga = new Dictionary<string, Producto>();
            _ubicacionesCarga = new Dictionary<string, Ubicacion>();
            _contadorInstancias = 0;

            _areas = new Dictionary<short, UbicacionArea>();
            _predios = new HashSet<string>();
            _controlesTipos = new HashSet<int>();
            _ubicaciones = new Dictionary<string, Ubicacion>();
            _productos = new Dictionary<string, Producto>();
            _etiquetas = new List<EtiquetaLote>();
            _controlesCalidadPendiente = new List<ControlDeCalidadPendiente>();
            _lpns = new List<Lpn>();
            _stocks = new List<Stock>();
        }

        #region Load

        public override async Task Load()
        {
            await base.Load();

            List<CriterioControlCalidadAPI> criterios = new List<CriterioControlCalidadAPI>();

            foreach (var control in this._controles)
            {
                foreach (CriterioControlCalidadAPI criterio in control.Criterios)
                {
                    if (!criterios.Any(a => a.Producto == criterio.Producto.Truncate(40)
                            && a.Empresa == criterio.Empresa
                            && a.Lote == criterio.Lote.Truncate(40)
                            && a.Predio == criterio.Predio
                            && a.Faixa == criterio.Faixa))
                    {
                        criterios.Add(criterio);
                    }
                }
            }

            this.LoadControl(_controles.Select(s => s.CodigoControlCalidad).ToList());
            this.LoadPredios();
            this.LoadEtiquetas(criterios);
            this.LoadDetallesEtiquetas();
            this.LoadLpn(criterios);
            this.LoadDetallesLpn();
            this.LoadStock(criterios);
            this.LoadControlesPendientes(criterios);
            this.SetStockControlCalidadPendiente();

            foreach (ControlCalidadAPI control in _controles)
            {
                foreach (CriterioControlCalidadAPI criterio in control.Criterios)
                {
                    var keyProducto = $"{criterio.Producto}.{control.Empresa}";

                    if (!_productosCarga.ContainsKey(keyProducto))
                        _productosCarga[keyProducto] = new Producto()
                        {
                            Codigo = criterio.Producto.Truncate(40),
                            CodigoEmpresa = control.Empresa
                        };

                    if (!string.IsNullOrEmpty(criterio.Ubicacion))
                        if (!_ubicacionesCarga.ContainsKey(criterio.Ubicacion))
                            _ubicacionesCarga[criterio.Ubicacion] = new Ubicacion() { Id = criterio.Ubicacion.Truncate(40) };
                }
            }

            this.LoadProductos();

            foreach (var stock in this._stocks)
            {
                if (!_ubicacionesCarga.ContainsKey(stock.Ubicacion))
                    _ubicacionesCarga[stock.Ubicacion] = new Ubicacion() { Id = stock.Ubicacion };
            }

            foreach (var u in _uow.UbicacionRepository.GetUbicaciones(_ubicacionesCarga.Values))
            {
                u.EsUbicacionBaja = (u.IdUbicacionBaja ?? "N") == "S";
                u.EsUbicacionSeparacion = (u.IdUbicacionSeparacion ?? "N") == "S";
                u.NecesitaReabastecer = (u.IdNecesitaReabastecer ?? "N") == "S";

                _ubicaciones[u.Id] = u;
            }

            foreach (var p in _uow.UbicacionAreaRepository.GetUbicacionAreas())
                _areas[p.Id] = p;

            _ubicacionesCarga.Clear();
            _productosCarga.Clear();
        }

        public virtual void SetStockControlCalidadPendiente()
        {
            foreach (ControlDeCalidadPendiente controlPendiente in this._controlesCalidadPendiente)
            {
                if (controlPendiente.Ubicacion != null)
                {
                    controlPendiente.Stock = this._stocks.FirstOrDefault(w => w.Ubicacion == controlPendiente.Ubicacion && w.Producto == controlPendiente.Producto && w.Empresa == controlPendiente.Empresa && w.Identificador == controlPendiente.Identificador && w.Faixa == controlPendiente.Faixa);
                }
            }
        }

        public virtual void LoadProductos()
        {
            foreach (var p in _uow.ProductoRepository.GetProductos(_productosCarga.Values))
            {
                string keyProducto = $"{p.Codigo}.{p.CodigoEmpresa}";
                p.AceptaDecimales = !string.IsNullOrEmpty(p.AceptaDecimalesId) && p.AceptaDecimalesId == "S";
                p.ManejoIdentificador = (new ProductoMapper()).MapManejoIdentificador(p.ManejoIdentificadorId);
                _productos[keyProducto] = p;
            }
        }

        public virtual void LoadControl(List<int> controles)
        {
            List<ControlDeCalidad> tiposDeControles = _uow.ControlDeCalidadRepository.GetTiposControlesDeCalidad(controles);
            this._controlesTipos.AddRange(tiposDeControles.Select(s => s.Id));
        }

        public virtual void LoadPredios()
        {
            List<Predio> predios = _uow.PredioRepository.GetPredios();
            this._predios.AddRange(predios.Select(s => s.Numero));
        }

        public virtual void LoadEtiquetas(List<CriterioControlCalidadAPI> criterios)
        {
            List<EtiquetaLote> etiquetas = _uow.EtiquetaLoteRepository.GetEtiquetasCriterios(criterios).ToList();
            this._etiquetas.AddRange(etiquetas);
        }

        public virtual void LoadDetallesEtiquetas()
        {
            List<EtiquetaEnUso> colEtiquetaEnUso =
                this._etiquetas
                    .Select(s => new EtiquetaEnUso { Numero = s.Numero })
                    .ToList();

            List<EtiquetaLoteDetalle> detallesEtiquetas =
                _uow.EtiquetaLoteRepository
                    .GetEtiquetaLoteDetalle(colEtiquetaEnUso)
                    .ToList();

            foreach (EtiquetaLote etiqueta in this._etiquetas)
            {
                etiqueta.Detalles = new List<EtiquetaLoteDetalle>();
                etiqueta.Detalles.AddRange(detallesEtiquetas.Where(w => w.IdEtiquetaLote == etiqueta.Numero).ToList());
            }

        }

        public virtual void LoadDetallesLpn()
        {
            List<LpnDetalle> detallesLpn =
                _uow.ManejoLpnRepository
                    .GetDetallesLpnConStock(this._lpns)
                    .ToList();

            foreach (Lpn lpn in this._lpns)
            {
                lpn.Detalles = new List<LpnDetalle>();
                lpn.Detalles.AddRange(detallesLpn.Where(w => w.NumeroLPN == lpn.NumeroLPN).ToList());
            }
        }

        public virtual void LoadLpn(List<CriterioControlCalidadAPI> criterios)
        {
            this._lpns.AddRange(_uow.ManejoLpnRepository.GetLpnCriterios(criterios).ToList());
        }

        public virtual void LoadStock(List<CriterioControlCalidadAPI> criterios)
        {
            this._stocks.AddRange(_uow.StockRepository.GetStocksCriterios(criterios).ToList());
        }

        public virtual void LoadControlesPendientes(List<CriterioControlCalidadAPI> criterios)
        {
            this._controlesCalidadPendiente.AddRange(
                _uow.ControlDeCalidadRepository
                    .GetControlesCalidadPendientesCriterios(criterios)
                    .ToList());
        }

        public virtual void LoadNewInstancias(int cant) =>
            _nuevasInstancias = _uow.ControlDeCalidadRepository.GetNextInstanciaSequenceValue(cant);

        public virtual void LoadNewIds(int cant) =>
           _nuevasIds = _uow.ControlDeCalidadRepository.GetNextIdSequenceValue(cant);

        #endregion

        #region Any

        public virtual bool ProductoExiste(string producto, int empresa)
        {
            var keyProducto = $"{producto}.{empresa}";
            return this._productos.ContainsKey(keyProducto);
        }

        public virtual bool PredioExiste(string predio)
        {
            return this._predios.Any(a => a == predio);
        }

        public virtual bool UbicacionExiste(string ubicacion)
        {
            return this._ubicaciones.ContainsKey(ubicacion);
        }

        public virtual bool EtiquetaExternaExiste(string predio, string etiquetaExterno, out int cantidad)
        {
            cantidad = this._etiquetas.Count(w => w.NumeroExterno == etiquetaExterno && w.Predio == predio);
            return cantidad > 0;
        }

        public virtual bool LpnExternoExiste(string predio, string etiquetaExterno, out int cantidad)
        {
            cantidad = this._lpns.Count(w => w.IdExterno == etiquetaExterno && w.Predio == predio);
            return cantidad > 0;
        }

        public virtual bool EtiquetaExternaExiste(string predio, string etiquetaExterno, string tipo)
        {
            return this._etiquetas.Any(
                w => w.NumeroExterno == etiquetaExterno && w.Predio == predio && w.TipoEtiqueta == tipo);
        }

        public virtual bool LpnExternoExiste(string predio, string etiquetaExterno, string tipo)
        {
            return this._lpns.Any(w => w.IdExterno == etiquetaExterno && w.Predio == predio && w.Tipo == tipo);
        }

        public virtual bool StockLpnNoDisponible(decimal qtStockLpn, long nuLpn, string ubicacion, int empresa, string producto, string identificador, decimal faixa, int idLpnDet)
        {
            var cantidadDisponibleLpn = _uow.ManejoLpnRepository.GetCantidadStockDisponibleDetalleLpn(nuLpn, ubicacion, empresa, producto, identificador, faixa, idLpnDet);
            cantidadDisponibleLpn = cantidadDisponibleLpn + _uow.ManejoLpnRepository.GetCantidNoDisponibleEnLpn(nuLpn, empresa, producto, identificador, faixa, idLpnDet);

            if (qtStockLpn != cantidadDisponibleLpn)
                return true;

            Stock stock = this._stocks.FirstOrDefault(f => f.Ubicacion == ubicacion && f.Producto == producto && f.Identificador == identificador && f.Faixa == faixa && f.Empresa == empresa);
            if (stock.ReservaSalida > 0)
            {
                decimal cantidadStockLpn = _uow.ManejoLpnRepository.GetStockLpnUbicacion(ubicacion, empresa, producto, identificador, faixa, out decimal cantidadReservaLpn, out decimal cantidadReservaAtributo);
                decimal stockLibre = ((stock.Cantidad ?? 0) - (cantidadStockLpn));
                decimal reservaLibre = ((stock.ReservaSalida ?? 0) - cantidadReservaLpn - cantidadReservaAtributo);

                if (reservaLibre > stockLibre)
                    return true;
            }

            return false;
        }

        public virtual bool StockLibreNoDisponible(string ubicacion, int empresa, string producto, string identificador, decimal faixa)
        {
            decimal cantidadStockLpn = _uow.ManejoLpnRepository.GetStockLpnUbicacion(ubicacion, empresa, producto, identificador, faixa, out decimal cantidadReservaLpn, out decimal cantidadReservaAtributo);
            Stock stock = this._stocks.FirstOrDefault(f => f.Ubicacion == ubicacion && f.Producto == producto && f.Identificador == identificador && f.Faixa == faixa && f.Empresa == empresa);

            decimal stockLibre = ((stock.Cantidad ?? 0) - (cantidadStockLpn));
            decimal reservaLibre = ((stock.ReservaSalida ?? 0) - cantidadReservaLpn - cantidadReservaAtributo);

            if (reservaLibre > 0)
                return true;

            if (stockLibre <= 0)
                return true;

            return false;
        }

        public virtual bool TieneStockLibre(Stock stock, List<EtiquetaLote> etiquetas)
        {
            decimal stockEnEtiquetaRec = GetStockAfectadoEnEtiquetaRecepcion(stock, etiquetas.Where(w => w.NroLpn == null).ToList());
            decimal stockEnLpn = GetStockAfectadoEnLpn(stock);

            return (stock.Cantidad - stockEnEtiquetaRec - stockEnLpn) > 0;
        }

        public virtual decimal GetStockAfectadoEnEtiquetaRecepcion(Stock stock, List<EtiquetaLote> etiquetas)
        {
            decimal cantidadStockEtiqueta = (etiquetas.Sum(s => s.Detalles.Sum(d => d.Cantidad)) ?? 0);

            return cantidadStockEtiqueta;
        }

        public virtual decimal GetStockAfectadoEnLpn(Stock stock)
        {
            decimal cantidadStockLpn =
                _uow.ManejoLpnRepository.GetStockLpnUbicacion(
                    stock.Ubicacion,
                    stock.Empresa,
                    stock.Producto,
                    stock.Identificador,
                    stock.Faixa,
                    out decimal cantidadReservaLpn,
                    out decimal cantidadReservaAtributo);

            return cantidadStockLpn;
        }

        public virtual bool ProductoEnUbicacion(string ubicacion, int empresa, string producto, string identificador, decimal faixa)
        {
            return this._stocks.Any(f => f.Ubicacion == ubicacion && f.Producto == producto && f.Identificador == identificador && f.Faixa == faixa && f.Empresa == empresa);
        }

        public virtual bool UbicacionAreaDisponible(string codigoUbicacion)
        {
            Ubicacion ubicacion = _ubicaciones[codigoUbicacion];
            UbicacionArea areaUbicacion = _areas[ubicacion.IdUbicacionArea];

            return (areaUbicacion.EsAreaStockGeneral && !areaUbicacion.EsAreaEspera && !areaUbicacion.PermiteVehiculo && !areaUbicacion.EsAreaPicking && !areaUbicacion.EsAreaEmbarque);
        }

        public virtual bool UbicacionLpnAreaDisponible(string codigoUbicacion)
        {
            Ubicacion ubicacion = _ubicaciones[codigoUbicacion];
            UbicacionArea areaUbicacion = _areas[ubicacion.IdUbicacionArea];

            return areaUbicacion.EsAreaStockGeneral;
        }

        public virtual bool ExisteControlPendiente(string predio, int codigo, string producto, string lote, decimal faixa, int empresa, string ubicacion)
        {
            return this._controlesCalidadPendiente.Any(
                w => w.Codigo == codigo &&
                    w.Ubicacion == ubicacion &&
                    w.Predio == predio &&
                    w.Producto == producto &&
                    w.Identificador == lote &&
                    w.Faixa == faixa &&
                    w.Empresa == empresa &&
                    w.IdLpnDet == null &&
                    w.NroLPN == null &&
                    w.Etiqueta == null &&
                    !w.Aceptado);
        }

        public virtual bool ExisteControlPendiente(string predio, int codigo, string producto, string lote, decimal faixa, int empresa)
        {
            return this._controlesCalidadPendiente.Any(
                w => w.Codigo == codigo &&
                    w.Predio == predio &&
                    w.Producto == producto &&
                    w.Faixa == faixa &&
                    w.Identificador == lote &&
                    w.Empresa == empresa &&
                    !w.Aceptado);
        }

        public virtual bool ExisteControlPendiente(string predio, int codigo, string producto, string lote, decimal faixa, int empresa, int nuEtiquetaLote)
        {
            return this._controlesCalidadPendiente.Any(
                w => w.Codigo == codigo &&
                    w.Etiqueta == nuEtiquetaLote &&
                    w.Ubicacion == null &&
                    w.Predio == predio &&
                    w.Producto == producto &&
                    w.Faixa == faixa &&
                    w.Identificador == lote &&
                    w.Empresa == empresa &&
                    !w.Aceptado);
        }

        public virtual bool ExisteControlPendiente(string predio, int codigo, string producto, string lote, decimal faixa, int empresa, long nuLpn, int idDetLpn)
        {
            return this._controlesCalidadPendiente.Any(
                w => w.Codigo == codigo &&
                    w.IdLpnDet == idDetLpn &&
                    w.NroLPN == nuLpn &&
                    w.Predio == predio &&
                    w.Producto == producto &&
                    w.Faixa == faixa &&
                    w.Identificador == lote &&
                    w.Empresa == empresa &&
                    !w.Aceptado);
        }

        public virtual bool ExisteCodigoControl(int codControl) => _controlesTipos.Any(x => x == codControl);

        #endregion

        #region Get

        public virtual Lpn GetEtiquetaLpnExterno(string predio, string etiquetaExterno)
        {
            return this._lpns.FirstOrDefault(w => w.IdExterno == etiquetaExterno && w.Predio == predio);
        }

        public virtual Lpn GetEtiquetaLpnExterno(string predio, string etiquetaExterno, string tipo)
        {
            return this._lpns.FirstOrDefault(w => w.IdExterno == etiquetaExterno && w.Predio == predio && w.Tipo == tipo);
        }

        public virtual List<Lpn> GetEtiquetaLpnExterno(string predio, string producto, int empresa, string identificador, decimal faixa)
        {
            return this._lpns.Where(
                l => l.Detalles.Any(d =>
                    d.CodigoProducto == producto &&
                    d.Empresa == empresa &&
                    d.Lote == identificador &&
                    d.Faixa == faixa) &&
                    l.Predio == predio)
                .ToList();
        }

        public virtual LpnDetalle GetDetalleLpn(long NroLpn, int NroDetalle) =>
            _lpns
                .FirstOrDefault(x => x.NumeroLPN == NroLpn)?
                .Detalles
                .FirstOrDefault(z => z.Id == NroDetalle);

        public virtual EtiquetaLote GetEtiquetaRecepcionExterno(string predio, string etiquetaExterno)
        {
            return this._etiquetas.FirstOrDefault(w => w.NumeroExterno == etiquetaExterno && w.Predio == predio);
        }

        public virtual EtiquetaLote GetEtiquetaRecepcionExterno(string predio, string etiquetaExterno, string tipo)
        {
            return this._etiquetas.FirstOrDefault(w => w.NumeroExterno == etiquetaExterno && w.Predio == predio && w.TipoEtiqueta == tipo);
        }

        public virtual List<EtiquetaLote> GetEtiquetaRecepcionExterno(string predio, string producto, int empresa, string identificador, decimal faixa)
        {
            return this._etiquetas.Where(x =>
                x.Predio == predio &&
                x.Detalles.Any(y =>
                    y.CodigoProducto == producto &&
                    y.Identificador == identificador &&
                    y.Faixa == faixa &&
                    y.IdEmpresa == empresa))
                .ToList();
        }

        public virtual Producto GetProducto(string producto, int empresa)
        {
            var keyProducto = $"{producto}.{empresa}";
            return this._productos.GetValueOrDefault(keyProducto);
        }

        public virtual Stock GetStock(string predio, string producto, string lote, int empresa, string ubicacion, decimal faixa)
        {
            return this._stocks.FirstOrDefault(
                x =>
                    x.Predio == predio &&
                    x.Producto == producto &&
                    x.Identificador == lote &&
                    x.Empresa == empresa &&
                    x.Faixa == faixa &&
                    x.Ubicacion == ubicacion);
        }

        public virtual List<Stock> GetStock(string predio, string producto, string lote, int empresa, decimal faixa)
        {
            return this._stocks.Where(
                x =>
                    x.Predio == predio &&
                    x.Producto == producto &&
                    x.Identificador == lote &&
                    x.Empresa == empresa &&
                    x.Faixa == faixa)
                .ToList();
        }

        public virtual List<ControlDeCalidadPendiente> GetPorAprobarEtiqueta() =>
            _controlesCalidadPendiente.Where(x => x.Etiqueta != null).ToList();

        public virtual List<ControlDeCalidadPendiente> GetPorAprobarUbicacion() =>
            _controlesCalidadPendiente.Where(x => x.Ubicacion != null && x.Etiqueta == null).ToList();

        #endregion

        #region Generar

        public virtual ControlDeCalidadPendiente GenerarControl(int codigoControl, string descripcion, string ubicacion, long instancia, CriterioControlCalidadAPI criterio, EtiquetaLote etiqueta = null, Lpn lpn = null, LpnDetalle detalle = null) =>
            new ControlDeCalidadPendiente
            {
                Instancia = instancia,
                Codigo = codigoControl,
                Etiqueta = etiqueta?.Numero,
                Aceptado = false,
                FechaAlta = DateTime.Now,
                FechaModificacion = null,
                Predio = criterio.Predio,
                FuncionarioAceptacion = null,
                Empresa = criterio.Empresa,
                Producto = criterio.Producto,
                Identificador = criterio.Lote,
                Faixa = criterio.Faixa,
                Ubicacion = ubicacion,
                NroLPN = detalle?.NumeroLPN,
                IdLpnDet = detalle?.Id,
                Descripcion = descripcion,

            };

        public virtual ControlDeCalidadPendiente GenerarControlCalidadEtiquetaProcess(CriterioControlCalidadAPI criterio, int codigoControl, string descripcion, EtiquetaLote etiqueta, long instancia)
        {
            return this.GenerarControl(codigoControl, descripcion, null, instancia, criterio, etiqueta);
        }

        public virtual ControlDeCalidadPendiente GenerarControlCalidadEtiquetaProcess(CriterioControlCalidadAPI criterio, int codigoControl, string descripcion, EtiquetaLote etiqueta, Lpn lpn, long instancia, LpnDetalle detalle)
        {
            return this.GenerarControl(codigoControl, descripcion, null, instancia, criterio, etiqueta, lpn, detalle);
        }

        public virtual ControlDeCalidadPendiente GenerarControlCalidadLpnProcess(CriterioControlCalidadAPI criterio, int codigoControl, string descripcion, Lpn lpn, long instancia, ref LpnDetalle detalle)
        {
            ControlDeCalidadPendiente nuevoControl = this.GenerarControl(codigoControl, descripcion, lpn.Ubicacion, instancia, criterio, lpn: lpn, detalle: detalle);

            detalle.IdCtrlCalidad = EstadoControlCalidad.Pendiente;
            detalle.NumeroTransaccion = _uow.GetTransactionNumber();

            return nuevoControl;
        }

        public virtual ControlDeCalidadPendiente GenerarControlCalidadUbicacionProcess(CriterioControlCalidadAPI criterio, int codigoControl, string descripcion, long instancia, ref Stock stock)
        {
            ControlDeCalidadPendiente nuevoControl;
            nuevoControl = this.GenerarControl(codigoControl, descripcion, stock.Ubicacion, instancia, criterio);

            stock.NumeroTransaccion = _uow.GetTransactionNumber();
            stock.ControlCalidad = EstadoControlCalidad.Pendiente;
            stock.FechaModificacion = DateTime.Now;

            return nuevoControl;
        }

        #endregion

        #region Operaciones

        public virtual void AsignarTipoCriterioControl(ControlCalidadAPI control)
        {
            foreach (var criterio in control.Criterios)
                this.AsignarTipoCriterio(criterio);
        }

        public virtual void AsignarTipoCriterio(CriterioControlCalidadAPI criterio)
        {
            criterio.Operacion = ControlCalidadCriterio.Indefinido;

            if (criterio.EtiquetaExterna != null)
            {
                criterio.Operacion = ControlCalidadCriterio.Etiqueta;

                if (!this._etiquetas.Any(
                        w =>
                            w.NumeroExterno == criterio.EtiquetaExterna &&
                            w.Predio == criterio.Predio &&
                            w.Detalles.Any(
                                a =>
                                    a.CodigoProducto == criterio.Producto &&
                                    a.IdEmpresa == criterio.Empresa &&
                                    a.Identificador == criterio.Lote)) &&
                    this._lpns.Any(
                        w =>
                            w.IdExterno == criterio.EtiquetaExterna &&
                            w.Predio == criterio.Predio &&
                            w.Detalles.Any(
                                a =>
                                    a.CodigoProducto == criterio.Producto &&
                                    a.Empresa == criterio.Empresa &&
                                    a.Lote == criterio.Lote)))

                    criterio.Operacion = ControlCalidadCriterio.LPN;
            }
            else if (criterio.Ubicacion != null)
                criterio.Operacion = ControlCalidadCriterio.Ubicacion;
            else criterio.Operacion = ControlCalidadCriterio.Producto;

        }

        #endregion
    }
}
