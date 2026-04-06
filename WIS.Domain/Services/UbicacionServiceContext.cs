using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.Configuracion;
using WIS.Domain.Recorridos;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Services
{
    public class UbicacionServiceContext : ServiceContext, IUbicacionServiceContext
    {
        protected readonly List<UbicacionExterna> _registros = [];
        
        protected HashSet<string> _predios = [];
        protected HashSet<string> _ubicaciones = [];
        protected HashSet<string> _codigosDeBarras = [];
        protected HashSet<short> _tiposUbicacion = [];
        protected HashSet<int> _empresas = [];
        protected HashSet<string> _zonas = [];
        protected HashSet<string> _clases = [];
        protected HashSet<short> _rotatividades = [];
        protected HashSet<int> _familias = [];
        protected HashSet<long> _numerosOrden = [];
        protected HashSet<long> _ordenesPorDefecto = [];
        protected Dictionary<short, UbicacionArea> _areas = [];

        public Recorrido RecorridoPorDefecto { get; set; }
        public UbicacionConfiguracion UbicacionConfiguracion { get; set; }

        public UbicacionServiceContext(IUnitOfWork uow, List<UbicacionExterna> registros, int userId) : base(uow, userId, 0)
        {
            _registros = registros;
        }

        public async override Task Load()
        {
            await base.Load();

            var nuPredio = _registros.First().NumeroPredio;

            RecorridoPorDefecto = _uow.RecorridoRepository.GetRecorridoPorDefectoParaPredio(nuPredio);
            UbicacionConfiguracion = _uow.UbicacionRepository.GetUbicacionConfiguracion();

            _predios = _uow.PredioRepository.GetPrediosUsuario(UserId).Select(p => p.Numero).ToHashSet();
            _areas = _uow.UbicacionAreaRepository.GetUbicacionAreas().ToDictionary(a => a.Id, a => a);

            var registros = _registros.Where(r => r.Id.Length <= 40);
            var ubicaciones = _uow.UbicacionRepository.GetUbicaciones(registros);

            foreach (var ubicacion in ubicaciones)
            {
                ubicacion.EsUbicacionBaja = (ubicacion.IdUbicacionBaja ?? "N") == "S";
                ubicacion.EsUbicacionSeparacion = (ubicacion.IdUbicacionSeparacion ?? "N") == "S";
                ubicacion.NecesitaReabastecer = (ubicacion.IdNecesitaReabastecer ?? "N") == "S";

                _ubicaciones.Add(ubicacion.Id);
            }

            var codigosDeBarras = _uow.UbicacionRepository.GetCodigosDeBarras(_registros);

            foreach (var codigo in codigosDeBarras)
            {
                if (!codigosDeBarras.Contains(codigo)) _codigosDeBarras.Add(codigo);
            }

            _empresas = _uow.EmpresaRepository.GetEmpresasAsignadasUsuario(UserId).ToHashSet();
            _tiposUbicacion = _uow.UbicacionTipoRepository.GetUbicacionTipos().Select(tu => tu.Id).ToHashSet();
            _zonas = _uow.ZonaUbicacionRepository.GetZonas().Select(z => z.Id).ToHashSet();
            _clases = _uow.ClaseRepository.GetClases().Select(u => u.Id).ToHashSet();
            _rotatividades = _uow.ProductoRotatividadRepository.GetProductoRotatividades().Select(r => r.Id).ToHashSet();
            _familias = _uow.ProductoFamiliaRepository.GetProductoFamilias().Select(f => f.Id).ToHashSet();

            var detRec = _registros.Select(s => new DetalleRecorrido { ValorOrden = s.ValorDefecto, IdRecorrido = RecorridoPorDefecto.Id });
            var ordenesPorDefecto = _uow.UbicacionRepository.GetOrdenesPorDefecto(nuPredio);

            foreach (var ordenPorDefecto in ordenesPorDefecto)
            {
                if (!_ordenesPorDefecto.Contains(ordenPorDefecto ?? -1))
                    _ordenesPorDefecto.Add(ordenPorDefecto ?? -1);
            }
        }

        #region Métodos auxiliares

        public virtual bool ExisteUbicacion(string cdUbicacion)
        {
            return _ubicaciones.Contains(cdUbicacion);
        }

        public virtual bool ExisteEmpresa(int cdEmpresa)
        {
            return _empresas.Contains(cdEmpresa);
        }

        public virtual bool ExisteZona(string cdZona)
        {
            return _zonas.Contains(cdZona);
        }

        public virtual bool ExisteAreaUbicacion(short cdArea)
        {
            return _areas.ContainsKey(cdArea);
        }

        public virtual bool AreaUbicacionEsMantenible(short cdArea)
        {
            return _areas[cdArea].EsAreaMantenible;
        }

        public virtual bool AreaUbicacionEsRecorrible(short cdArea)
        {
            var area = _areas[cdArea];

            return area.EsAreaMantenible && ((area.EsAreaStockGeneral && area.DisponibilizaStock)
                    || (area.EsAreaPicking && area.DisponibilizaStock)
                    || area.EsAreaAveria);
        }

        public virtual bool ExisteTipoUbicacion(short cdTipoUbicacion)
        {
            return _tiposUbicacion.Contains(cdTipoUbicacion);
        }

        public virtual bool ExisteClase(string cdClase)
        {
            return _clases.Contains(cdClase);
        }

        public virtual bool ExisteRotatividad(short cdRotatividad)
        {
            return _rotatividades.Contains(cdRotatividad);
        }

        public virtual bool ExisteFamilia(int cdFamilia)
        {
            return _familias.Contains(cdFamilia);
        }

        public virtual bool ExistePredio(string nuPredio)
        {
            return _predios.Contains(nuPredio);
        }

        public virtual bool ExisteNumeroOrden(long nuOrden)
        {
            if (_numerosOrden.Contains(nuOrden)) return true;

            _numerosOrden.Add(nuOrden);

            return false;
        }

        public virtual bool CodigoDeBarrasEsUnicoParaPredio(string codigoBarras, string predio)
        {
            if (_codigosDeBarras.Any(cb => cb == codigoBarras))
                return false;
            else
                return _registros.Where(i => i.CodigoBarras == codigoBarras && i.NumeroPredio == predio).Count() == 1;
        }

        public virtual bool IdContienePrefijosWis(string ubicacion, string numeroPredio)
        {
            var prefijoFuncionario = GetParametro(ParamManager.PREFIJO_EQUIPO_FUN);
            var prefijoEquipo = GetParametro(ParamManager.PREFIJO_EQUIPO);
            var prefijoClasificacion = GetParametro(ParamManager.PREFIJO_CLASIFICACION);
            var prefijoAutomatismo = GetParametro(ParamManager.PREFIJO_AUTOMATISMO);
            var prefijoPuerta = GetParametro(ParamManager.PREFIJO_PUERTA);
            var prefijoEstacion = GetParametro(ParamManager.PREFIJO_PRODUCCION);

            string pattern = @$"^({numeroPredio})?({prefijoFuncionario}|{prefijoEquipo}|{prefijoClasificacion}|{prefijoAutomatismo}|{prefijoPuerta}|{prefijoEstacion})\b";

            return Regex.IsMatch(ubicacion, pattern, RegexOptions.IgnoreCase);
        }

        public virtual bool CodigoBarrasContienePrefijosWis(string cdBarras)
        {
            string pattern = @$"^({BarcodeDb.PREFIX_UBICACION}|{BarcodeDb.PREFIX_UBICACION_WISSD}).*";

            return Regex.IsMatch(cdBarras, pattern, RegexOptions.IgnoreCase);
        }

        public virtual bool ExisteOrdenPorDefecto(long nuOrden)
        {
            return _ordenesPorDefecto.Contains(nuOrden);
        }

        #endregion
    }
}
