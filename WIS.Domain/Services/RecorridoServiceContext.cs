using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Recorridos;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Services
{
    public class RecorridoServiceContext : ServiceContext, IRecorridoServiceContext
    {
        protected readonly List<DetalleRecorrido> _registros = [];
        protected Dictionary<string, Ubicacion> _ubicaciones = [];
        protected HashSet<long> _numerosOrden = [];
        protected HashSet<string> _ubicacionesOperaciones = [];
        protected HashSet<string> _ubicacionesEnRecorrido = [];
        protected Dictionary<string, string> _ordenesExistentesEnRecorrido = [];
        protected Recorrido _recorrido;
        protected Dictionary<short, UbicacionArea> _areas = [];
        protected HashSet<string> UbicacionesEntradaAutomatismo = [];

        public List<DetalleRecorrido> RegistrosBaja { get; set; } = [];
        public List<DetalleRecorrido> RegistrosAlta { get; set; } = [];

        public RecorridoServiceContext(IUnitOfWork uow, List<DetalleRecorrido> registros, int userId, int empresa) : base(uow,userId, empresa)
        {
            _registros = registros;
        }

        public async override Task Load()
        {
            await base.Load();

            _recorrido = _uow.RecorridoRepository.GetRecorridoById(_registros.FirstOrDefault().IdRecorrido);
            _areas = _uow.UbicacionAreaRepository.GetUbicacionAreas().ToDictionary(a => a.Id, a => a);
            UbicacionesEntradaAutomatismo = _uow.AutomatismoPosicionRepository.GetUbicacionesEntrada().ToHashSet();

            RegistrosAlta = _registros.Where(r => r.Ubicacion.Length <= 40 && r.TipoOperacion == RecorridosConstants.TP_OPERACION_ALTA).ToList();
            RegistrosBaja = _registros.Where(r => r.Ubicacion.Length <= 40 && r.TipoOperacion == RecorridosConstants.TP_OPERACION_BAJA).ToList();

            var ubicaciones = _uow.UbicacionRepository.GetUbicaciones(RegistrosAlta.Select(s => new Ubicacion { Id = s.Ubicacion }));

            foreach (var ubicacion in ubicaciones)
            {
                ubicacion.EsUbicacionBaja = (ubicacion.IdUbicacionBaja ?? "N") == "S";
                ubicacion.EsUbicacionSeparacion = (ubicacion.IdUbicacionSeparacion ?? "N") == "S";
                ubicacion.NecesitaReabastecer = (ubicacion.IdNecesitaReabastecer ?? "N") == "S";

                _ubicaciones.TryAdd(ubicacion.Id, ubicacion);
            }

            var detRec = _registros.Where(r => r.Ubicacion.Length <= 40);
            var detallesRecorrido = _uow.RecorridoRepository.GetUbicacionesExistentesEnRecorrido(detRec);

            foreach (var detalle in detallesRecorrido)
            {
                if (!_ubicacionesEnRecorrido.Contains(detalle.Ubicacion))
                    _ubicacionesEnRecorrido.Add(detalle.Ubicacion);
            }

            detallesRecorrido = _uow.RecorridoRepository.GetDetallesRecorridosExistentes(RegistrosAlta);

            foreach (var detalle in detallesRecorrido)
            {
                if (!_ordenesExistentesEnRecorrido.ContainsKey(detalle.ValorOrden))
                    _ordenesExistentesEnRecorrido[detalle.ValorOrden] = detalle.Ubicacion;
            }
        }

        #region Métodos auxiliares

        public virtual bool ExisteUbicacion(string key)
        {
            return _ubicaciones.ContainsKey(key);
        }

        public virtual bool UbicacionTienePredioCompatibleConRecorrido(string codigoUbicacion)
        {
            var ubicacion = _ubicaciones.GetValueOrDefault(codigoUbicacion);

            return ubicacion.NumeroPredio == _recorrido.Predio;
        }

        public virtual bool UbicacionOperacionSeRepite(string codigoUbicacion, string tipoOperacion)
        {
            var ubicacionOperacion = $"{codigoUbicacion}.{tipoOperacion}";

            if (_ubicacionesOperaciones.Contains(ubicacionOperacion)) return true;

            _ubicacionesOperaciones.Add(ubicacionOperacion);

            return false;
        }

        public virtual bool NumeroOrdenSeRepite(long nroOrden)
        {
            if (_numerosOrden.Contains(nroOrden)) return true;

            _numerosOrden.Add(nroOrden);

            return false;
        }

        public virtual bool ExisteUbicacionEnRecorrido(string codigoUbicacion, string operacion)
        {
            bool existeUbicacionRecorrido = false;

            if (_ubicacionesEnRecorrido.Contains(codigoUbicacion))
            {
                existeUbicacionRecorrido = true;

                if (operacion == RecorridosConstants.TP_OPERACION_ALTA)
                {
                    var ubicacionOperacion = $"{codigoUbicacion}.{RecorridosConstants.TP_OPERACION_BAJA}";

                    return !_ubicacionesOperaciones.Contains(ubicacionOperacion);
                }
            }

            return existeUbicacionRecorrido;
        }

        public virtual bool ExisteNumeroOrden(string vlOrden)
        {
            if (_ordenesExistentesEnRecorrido.ContainsKey(vlOrden))
            {
                var ubicacion = _ordenesExistentesEnRecorrido[vlOrden];
                var ubicacionOperacion = $"{ubicacion}.{RecorridosConstants.TP_OPERACION_BAJA}";

                return !_ubicacionesOperaciones.Contains(ubicacionOperacion);
            }

            return false;
        }

        public virtual bool AreaUbicacionEsRecorrible(string cdUbicacion)
        {
            if (_ubicaciones.ContainsKey(cdUbicacion))
            {
                var ubicacion = _ubicaciones[cdUbicacion];
                var area = _areas[ubicacion.IdUbicacionArea];

                return (area.EsAreaMantenible &&
                            ((area.EsAreaStockGeneral && area.DisponibilizaStock) ||
                            (area.EsAreaPicking && area.DisponibilizaStock) || area.EsAreaAveria)) ||
                        UbicacionesEntradaAutomatismo.Any(u => u == ubicacion.Id);
            }

            return false;
        }

        #endregion
    }
}
