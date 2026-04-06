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
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Models;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Services
{
    public class ProduccionServiceContext : ServiceContext, IProduccionServiceContext
    {
        protected int _cantidadDetallesTeoricos = 0;
        protected List<IngresoProduccion> _ingresos = new List<IngresoProduccion>();

        public Agente Agente { get;set;} = new Agente();
        public Empresa EmpresaEjecucion { get;set;} = new Empresa();
        public HashSet<string> Predios { get;set;} = new HashSet<string>();
        public HashSet<string> TiposModalidadLote { get;set;} = new HashSet<string>();
        public HashSet<string> TiposIngresoProduccion { get;set;} = new HashSet<string>();
        public HashSet<IngresoBlackBox> IdsProduccionExterno { get;set;} = new HashSet<IngresoBlackBox>();
        public IEnumerable<FormulaSalida> FormulaDetallesSalida { get;set;} = new List<FormulaSalida>();
        public IEnumerable<FormulaEntrada> FormulaDetallesEntrada { get;set;} = new List<FormulaEntrada>();
        public Dictionary<string, Formula> Formulas { get;set;} = new Dictionary<string, Formula>();
        public Dictionary<string, Producto> Productos { get;set;} = new Dictionary<string, Producto>();
        public Dictionary<string, EstacionDeTrabajo> Espacios { get;set;} = new Dictionary<string, EstacionDeTrabajo>();
        public Dictionary<string, string> Parameters { get;set;} = new Dictionary<string, string>();

        public ProduccionServiceContext(IUnitOfWork uow, List<IngresoProduccion> ingresos, int userId, int empresa) : base(uow, userId, empresa)
        {
            _ingresos = ingresos;
        }

        public async override Task Load()
        {
            await base.Load();

            var keysFormulas = new Dictionary<string, Formula>();
            var keysProductos = new Dictionary<string, Producto>();
            var keysEspacios = new Dictionary<string, EstacionDeTrabajo>();
            var keysIdExternos = new Dictionary<string, IngresoBlackBox>();

            EmpresaEjecucion = _uow.EmpresaRepository.GetEmpresa(base.Empresa);
            Agente = _uow.AgenteRepository.GetAgente(EmpresaEjecucion.Id, EmpresaEjecucion.CdClienteArmadoKit);
            Predios = _uow.PredioRepository.GetPrediosUsuario(UserId).Select(d => d.Numero).ToHashSet();
            TiposIngresoProduccion = _uow.DominioRepository.GetDominios(CodigoDominioDb.TipoIngresoProduccion).Select(d => d.Id).ToHashSet();
            TiposModalidadLote = _uow.DominioRepository.GetDominios(CodigoDominioDb.ProduccionModalidadIngresoLote).Select(d => d.Valor).ToHashSet();
            Parameters = this.LoadParamDictionary();

            foreach (var p in _ingresos)
            {
                if (!string.IsNullOrEmpty(p.IdEspacioProducion))
                {
                    keysEspacios[p.IdEspacioProducion] = new EstacionDeTrabajo() { Id = p.IdEspacioProducion.Truncate(10) };
                }

                if (!string.IsNullOrEmpty(p.IdProduccionExterno))
                {
                    keysIdExternos[p.IdProduccionExterno] = new IngresoBlackBox() { IdProduccionExterno = p.IdProduccionExterno.Truncate(100) };
                }

                if (!string.IsNullOrEmpty(p.IdFormula))
                {
                    keysFormulas[p.IdFormula] = new Formula() { Id = p.IdFormula.Truncate(10) };
                }

                foreach (var l in p.Detalles)
                {
                    var keyProducto = $"{l.Producto}.{base.Empresa}";

                    if (!keysProductos.ContainsKey(keyProducto))
                        keysProductos[keyProducto] = new Producto() { Codigo = l.Producto.Truncate(40), CodigoEmpresa = base.Empresa };

                    _cantidadDetallesTeoricos++;
                }
            }

            FormulaDetallesEntrada = _uow.FormulaRepository.GetDetallesEntradaFormula(keysFormulas.Values);
            FormulaDetallesSalida = _uow.FormulaRepository.GetDetallesSalidaFormula(keysFormulas.Values);

            foreach (var f in _uow.FormulaRepository.GetFormulas(keysFormulas.Values))
            {
                f.Entrada = GetDetallesEntrada(f.Id).ToList();
                f.Salida = GetDetallesSalida(f.Id).ToList();
                Formulas[f.Id] = f;
            }

            _cantidadDetallesTeoricos += FormulaDetallesEntrada.Count();
            _cantidadDetallesTeoricos += FormulaDetallesSalida.Count();

            foreach (var id in _uow.IngresoProduccionRepository.GetIdsExternos(keysIdExternos.Values))
            {
                IdsProduccionExterno.Add(id);
            }

            foreach (var ep in _uow.EspacioProduccionRepository.GetEspaciosProduccion(keysEspacios.Values))
            {
                Espacios[ep.Id] = ep;
            }

            var productosFormula = FormulaDetallesEntrada.Select(x => new { x.Producto, x.Empresa }).ToList();
            productosFormula.AddRange(FormulaDetallesSalida.Select(x => new { x.Producto, x.Empresa }));

            foreach (var key in productosFormula)
            {
                var keyProducto = $"{key.Producto}.{key.Empresa}";

                if (!keysProductos.ContainsKey(keyProducto))
                    keysProductos[keyProducto] = new Producto() { Codigo = key.Producto.Truncate(40), CodigoEmpresa = key.Empresa };
            }

            foreach (var p in _uow.ProductoRepository.GetProductos(keysProductos.Values))
            {
                p.AceptaDecimales = !string.IsNullOrEmpty(p.AceptaDecimalesId) && p.AceptaDecimalesId == "S";
                p.ManejoIdentificador = (new ProductoMapper()).MapManejoIdentificador(p.ManejoIdentificadorId);
                Productos[p.Codigo] = p;
            }
        }

        #region Metodos Auxiliares

        public virtual bool ExisteTipoIngreso(string tipo)
        {
            return TiposIngresoProduccion.Contains(tipo);
        }

        public virtual bool ExisteIdProduccionExternoEmpresa(string idExterno, int empresa)
        {
            return IdsProduccionExterno.Any(i => i.IdProduccionExterno.ToUpper() == idExterno.ToUpper() && i.Empresa == empresa);
        }

        public virtual bool ExisteModalidadLote(string id)
        {
            return TiposModalidadLote.Contains(id);
        }

        public virtual bool ExistePredio(string predio)
        {
            return Predios.Contains(predio);
        }

        public virtual EstacionDeTrabajo GetEspacioProduccion(string id)
        {
            return Espacios.GetValueOrDefault(id, null);
        }

        public virtual Formula GetFormula(string id)
        {
            return Formulas.GetValueOrDefault(id, null);
        }

        public virtual Producto GetProducto(string codigo)
        {
            return Productos.GetValueOrDefault(codigo, null);
        }

        public virtual IEnumerable<FormulaEntrada> GetDetallesEntrada(string idFormula)
        {
            if (FormulaDetallesEntrada == null)
                return new List<FormulaEntrada>();

            return FormulaDetallesEntrada.Where(d => d.IdFormula == idFormula);
        }

        public virtual IEnumerable<FormulaSalida> GetDetallesSalida(string idFormula)
        {
            if (FormulaDetallesSalida == null)
                return new List<FormulaSalida>();

            return FormulaDetallesSalida.Where(d => d.IdFormula == idFormula);
        }

        public virtual int GetCantidadDetallesTeoricos()
        {
            return _cantidadDetallesTeoricos;
        }

        public virtual Dictionary<string, string> LoadParamDictionary()
        {
            return _uow.ParametroRepository.GetParameters(new List<string>
            {
                "PRDC_PED_CD_CON_VAL",
                "PRD112_LIB_ONDA",
                "PRD112_LIB_AGRUPACION",
                "PRD112_LIB_RESP_FIFO_AUTO",
                "PRD112_LIB_CTRL_STK_DOCUMENTAL",
                "PRD112_LIB_CURSOR_STOCK",
                "PRD112_LIB_CURSOR_PEDIDO",
                "PRD112_LIB_LIBERAR_CURVAS",
                "PRD112_LIB_LIBERAR_UNIDADES",
                "PRD112_LIB_REPARTIR_ESCASEZ",
                "PRD112_LIB_AGRUP_CAMION",
                "PRD112_LIB_PREP_SOLO_CAMION",
                "PRD112_LIB_MODO_PALLET_COMPLEO",
                "PRD112_LIB_MODO_PALLET_INCO",
                "PRD112_LIB_PRIORIZAR_DESBORDE",
                "PRD112_LIB_MANEJA_VIDA_UTIL",
                "PRD112_LIB_PICKING_DOS_FACES",
                "PRD112_LIB_EXCLUIR_PICKING",
            });
        }

        public virtual string GetParamValue(string id)
        {
            return this.Parameters.GetValueOrDefault(id);
        }

        #endregion
    }
}
