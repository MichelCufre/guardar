using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Linq;
using WIS.Configuration;
using WIS.Data;
using WIS.Domain.DataModel.Repositories;
using WIS.Domain.Services;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel
{
    public abstract class UnitOfWorkCore : IUnitOfWork, IDisposable
    {
        protected WISDB _context { get; private set; }

        protected readonly string _application;
        protected readonly int _userId;
        protected long _numeroTransacion;
        protected IDapper _dapper;
        protected IFactoryService _factoryService;
        protected IDbContextTransaction _transaction;
        protected IOptions<DatabaseSettings> _databaseSettings;
        protected IDatabaseFactory _dbFactory;

        public UnitOfWorkCore(IDatabaseConfigurationService dbConfigService,
            string application,
            int userId,
            IOptions<DatabaseSettings> databaseSettings,
            IDapper dapper,
            IFactoryService factoryService,
            IDatabaseFactory dbFactory,
            bool openContext = true)
        {
            if (openContext)
            {
                this.SetContext(new WISDB(dbConfigService, databaseSettings.Value.ConnectionString, databaseSettings.Value.Schema));
            }

            this._application = application;
            this._userId = userId;
            this._dapper = dapper;
            this._factoryService = factoryService;
            this._databaseSettings = databaseSettings;
            this._dbFactory = dbFactory;
            Dapper.SqlMapper.AddTypeMap(typeof(string), System.Data.DbType.AnsiString);
        }

        protected virtual void SetContext(WISDB context)
        {
            this._context = context;
            this._dapper = new DapperService(this._dbFactory, context.Database.GetDbConnection());
        }

        public virtual void BeginTransaction(System.Data.IsolationLevel isolationLevel)
        {
            this._transaction = this._context.Database.BeginTransaction(isolationLevel);
        }

        public virtual void BeginTransaction()
        {
            if (this._transaction == null)
            {
                this._transaction = this._context.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
            }
        }

        public virtual void HandleQuery<T>(IQueryObject<T, WISDB> query)
        {
            query.BuildQuery(this._context);
        }

        public virtual void Commit()
        {
            if (this._transaction != null)
                this._transaction.Commit();
        }

        public virtual void Rollback()
        {
            if (this._transaction != null)
                this._transaction.Rollback();
        }

        public virtual void EndTransaction()
        {
            if (this._transaction != null)
            {
                this._transaction.Dispose();
                this._transaction = null;
            }
        }

        public virtual int SaveChanges()
        {
            //Debe estar dentro de uan transaccion para su correcto funcionamiento (uow.BeginTransaction();)
            var flag = false;
            if (this._transaction == null)
            {
                BeginTransaction();
                flag = true;
            }

            SetFuncionario(this._context, _userId);
            if (!string.IsNullOrEmpty(_application))
            {
                var app = _application.Length > 30 ? _application.Substring(0, 30) : _application;
                SetAplicacion(this._context, app);
            }

            int result = 0;
            try
            {
                result = this._context.SaveChanges();
            }
            catch (Exception ex)
            {
                if (flag)
                    Rollback();
                throw ex;
            }

            if (flag)
            {
                Commit();
                this._transaction.Dispose();
                this._transaction = null;
            }

            return result;
        }

        public bool AnyTransaction()
        {
            return this._transaction != null;
        }

        private void SetAplicacion(DbContext context, string app)
        {
            try
            {
                string sql = "K_USUARIO.SET_APLICACAO";
                _dapper.Query<object>(context.Database.GetDbConnection(), sql, new
                {
                    P_APLIC = app
                }, commandType: CommandType.StoredProcedure, transaction: _transaction?.GetDbTransaction());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SetFuncionario(DbContext context, int userId)
        {
            try
            {
                string sql = "K_USUARIO.SET_FUNCIONARIO";
                _dapper.Query<object>(context.Database.GetDbConnection(), sql, new
                {
                    P_FUNC = userId
                }, commandType: CommandType.StoredProcedure, transaction: _transaction?.GetDbTransaction());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void Dispose()
        {
            if (this._transaction != null)
                this._transaction.Dispose();

            this._context.Dispose();
        }

        public long GetTransactionNumber()
        {
            return this._context.GetTransactionNumber();
        }

        public void CreateTransactionNumber(string descripcion)
        {
            if (this.GetTransactionNumber() == 0)
            {
                this._context.SetTransactionNumber(this.TransaccionRepository.AddTransaccion(descripcion));
                this.SaveChanges(); 
            }
        }

        public void CreateTransactionNumber(string descripcion, string application, int userId)
        {
            if (this.GetTransactionNumber() == 0)
            {
                this._context.SetTransactionNumber(this.TransaccionRepository.AddTransaccion(descripcion, application, userId));
                this.SaveChanges(); 
            }
        }

        public bool IsSnapshotException(Exception ex)
        {
            return _dbFactory.IsSnapshotException(ex);
        }

        public IsolationLevel GetSnapshotIsolationLevel()
        {
            return _dbFactory.GetSnapshotIsolationLevel();
        }

        private AgendaRepository _AgendaRepository; public AgendaRepository AgendaRepository => this._AgendaRepository ?? (this._AgendaRepository = new AgendaRepository(this._context, this._application, this._userId, _dapper));
        private AlmacenamientoRepository _AlmacenamientoRepository; public AlmacenamientoRepository AlmacenamientoRepository => this._AlmacenamientoRepository ?? (this._AlmacenamientoRepository = new AlmacenamientoRepository(this._context, this._application, this._userId, _dapper));
        private AgenteRepository _AgenteRepository; public AgenteRepository AgenteRepository => this._AgenteRepository ?? (this._AgenteRepository = new AgenteRepository(this._context, this._application, this._userId, _dapper));
        private ArchivoRepository _ArchivoRepository; public ArchivoRepository ArchivoRepository => this._ArchivoRepository ?? (this._ArchivoRepository = new ArchivoRepository(this._context, this._application, this._userId, this._dapper));
        private CrossDockingRepository _CrossDockingRepository; public CrossDockingRepository CrossDockingRepository => this._CrossDockingRepository ?? (this._CrossDockingRepository = new CrossDockingRepository(this._context, this._application, this._userId, _dapper));
        private CargaRepository _CargaRepository; public CargaRepository CargaRepository => this._CargaRepository ?? (this._CargaRepository = new CargaRepository(this._context, this._application, this._userId, this._dapper));
        private ControlDeCalidadRepository _ControlDeCalidadRepository; public ControlDeCalidadRepository ControlDeCalidadRepository => this._ControlDeCalidadRepository ?? (this._ControlDeCalidadRepository = new ControlDeCalidadRepository(this._context, this._application, this._userId, this._dapper));
        private InventarioRepository _InventarioRepository; public InventarioRepository InventarioRepository => this._InventarioRepository ?? (this._InventarioRepository = new InventarioRepository(this._context, this._application, this._userId, this._dapper));
        private EmpresaRepository _EmpresaRepository; public EmpresaRepository EmpresaRepository => this._EmpresaRepository ?? (this._EmpresaRepository = new EmpresaRepository(this._context, this._application, this._userId, _dapper));
        private EstacionDeTrabajoRepository _EstacionDeTrabajoRepository; public EstacionDeTrabajoRepository EstacionDeTrabajoRepository => this._EstacionDeTrabajoRepository ?? (this._EstacionDeTrabajoRepository = new EstacionDeTrabajoRepository(this._context, this._application, this._userId));
        private EtiquetaLoteRepository _EtiquetaLoteRepository; public EtiquetaLoteRepository EtiquetaLoteRepository => this._EtiquetaLoteRepository ?? (this._EtiquetaLoteRepository = new EtiquetaLoteRepository(this._context, this._application, this._userId, this._dapper));
        private FacturacionRepository _FacturacionRepository; public FacturacionRepository FacturacionRepository => this._FacturacionRepository ?? (this._FacturacionRepository = new FacturacionRepository(this._context, this._application, this._userId, _dapper));
        private DespachanteRepository _DespachanteRepository; public DespachanteRepository DespachanteRepository => this._DespachanteRepository ?? (this._DespachanteRepository = new DespachanteRepository(this._context, this._application, this._userId));
        private DominioRepository _DominioRepository; public DominioRepository DominioRepository => this._DominioRepository ?? (this._DominioRepository = new DominioRepository(this._context, this._application, this._userId, _dapper));
        private GridConfigRepository _GridConfigRepository; public GridConfigRepository GridConfigRepository => this._GridConfigRepository ?? (this._GridConfigRepository = new GridConfigRepository(this._context, this._application, this._userId, this._dapper));
        private GrupoConsultaRepository _GrupoConsultaRepository; public GrupoConsultaRepository GrupoConsultaRepository => this._GrupoConsultaRepository ?? (this._GrupoConsultaRepository = new GrupoConsultaRepository(this._context, this._application, this._userId, _dapper));
        private InterfazEjecDataRepository _InterfazEjecDataRepository; public InterfazEjecDataRepository InterfazEjecDataRepository => this._InterfazEjecDataRepository ?? (this._InterfazEjecDataRepository = new InterfazEjecDataRepository(this._context, this._application, this._userId));
        private MonedaRepository _MonedaRepository; public MonedaRepository MonedaRepository => this._MonedaRepository ?? (this._MonedaRepository = new MonedaRepository(this._context, this._application, this._userId));
        private EtiquetaTransferenciaRepository _EtiquetaTransferenciaRepository; public EtiquetaTransferenciaRepository EtiquetaTransferenciaRepository => this._EtiquetaTransferenciaRepository ?? (this._EtiquetaTransferenciaRepository = new EtiquetaTransferenciaRepository(this._context, this._application, this._userId, this._dapper));
        private NcmRepository _NcmRepository; public NcmRepository NcmRepository => this._NcmRepository ?? (this._NcmRepository = new NcmRepository(this._context, this._application, this._userId));
        private PorteriaRepository _PorteriaRepository; public PorteriaRepository PorteriaRepository => this._PorteriaRepository ?? (this._PorteriaRepository = new PorteriaRepository(this._context, this._application, this._userId, this._dapper));
        private ProduccionRepository _ProduccionRepository; public ProduccionRepository ProduccionRepository => this._ProduccionRepository ?? (this._ProduccionRepository = new ProduccionRepository(this._context, this._application, this._userId, this._dapper));
        private IdentificadorConsumirRepository _IdentificadorConsumirRepository; public IdentificadorConsumirRepository IdentificadorConsumirRepository => this._IdentificadorConsumirRepository ?? (this._IdentificadorConsumirRepository = new IdentificadorConsumirRepository(this._context, this._application, this._userId));
        private IdentificadorProducirRepository _IdentificadorProducirRepository; public IdentificadorProducirRepository IdentificadorProducirRepository => this._IdentificadorProducirRepository ?? (this._IdentificadorProducirRepository = new IdentificadorProducirRepository(this._context, this._application, this._userId));
        private LineaRepository _LineaRepository; public LineaRepository LineaRepository => this._LineaRepository ?? (this._LineaRepository = new LineaRepository(this._context, this._application, this._userId, this._dapper));
        private FormulaRepository _FormulaRepository; public FormulaRepository FormulaRepository => this._FormulaRepository ?? (this._FormulaRepository = new FormulaRepository(this._context, this._application, this._userId, this._dapper));
        private FormulaAccionRepository _FormulaAccionRepository; public FormulaAccionRepository FormulaAccionRepository => this._FormulaAccionRepository ?? (this._FormulaAccionRepository = new FormulaAccionRepository(this._context, this._application, this._userId, this._dapper));
        private AccionRepository _AccionRepository; public AccionRepository AccionRepository => this._AccionRepository ?? (this._AccionRepository = new AccionRepository(this._context, this._application, this._userId));
        private MovimientoStockBBRepository _MovimientoStockBBRepository; public MovimientoStockBBRepository MovimientoStockBBRepository => this._MovimientoStockBBRepository ?? (this._MovimientoStockBBRepository = new MovimientoStockBBRepository(this._context, this._application, this._userId, this._dapper));
        private DocumentoRepository _DocumentoRepository; public DocumentoRepository DocumentoRepository => this._DocumentoRepository ?? (this._DocumentoRepository = new DocumentoRepository(this._context, this._application, this._userId, this._factoryService, this._dapper));
        private DocumentoTipoRepository _DocumentoTipoRepository; public DocumentoTipoRepository DocumentoTipoRepository => this._DocumentoTipoRepository ?? (this._DocumentoTipoRepository = new DocumentoTipoRepository(this._context, this._application, this._userId));
        private DocumentoEntradaProduccionReservaRepository _DocumentoEntradaProduccionReservaRepository; public DocumentoEntradaProduccionReservaRepository DocumentoEntradaProduccionReservaRepository => this._DocumentoEntradaProduccionReservaRepository ?? (this._DocumentoEntradaProduccionReservaRepository = new DocumentoEntradaProduccionReservaRepository(this._context, this._application, this._userId, this._dapper));
        private DocumentoAnulacionPreparacionReservaRepository _DocumentoAnulacionPreparacionReservaRepository; public DocumentoAnulacionPreparacionReservaRepository DocumentoAnulacionPreparacionReservaRepository => this._DocumentoAnulacionPreparacionReservaRepository ?? (this._DocumentoAnulacionPreparacionReservaRepository = new DocumentoAnulacionPreparacionReservaRepository(this._context, this._application, this._userId));
        private TipoDuaRepository _TipoDuaRepository; public TipoDuaRepository TipoDuaRepository => this._TipoDuaRepository ?? (this._TipoDuaRepository = new TipoDuaRepository(this._context, this._application, this._userId));
        private TipoReferenciaExternaRepository _TipoReferenciaExternaRepository; public TipoReferenciaExternaRepository TipoReferenciaExternaRepository => this._TipoReferenciaExternaRepository ?? (this._TipoReferenciaExternaRepository = new TipoReferenciaExternaRepository(this._context, this._application, this._userId));
        private RegimenAduaneroRepository _RegimenAduaneroRepository; public RegimenAduaneroRepository RegimenAduaneroRepository => this._RegimenAduaneroRepository ?? (this._RegimenAduaneroRepository = new RegimenAduaneroRepository(this._context, this._application, this._userId));
        private PorteriaCamionRepository _PorteriaCamionRepository; public PorteriaCamionRepository PorteriaCamionRepository => this._PorteriaCamionRepository ?? (this._PorteriaCamionRepository = new PorteriaCamionRepository(this._context, this._application, this._userId, this._dapper));
        private PuertaEmbarqueRepository _PuertaEmbarqueRepository; public PuertaEmbarqueRepository PuertaEmbarqueRepository => this._PuertaEmbarqueRepository ?? (this._PuertaEmbarqueRepository = new PuertaEmbarqueRepository(this._context, this._application, this._userId, _dapper));
        private ProductoRepository _ProductoRepository; public ProductoRepository ProductoRepository => this._ProductoRepository ?? (this._ProductoRepository = new ProductoRepository(this._context, this._application, this._userId, _dapper));
        private ProductoRotatividadRepository _ProductoRotatividadRepository; public ProductoRotatividadRepository ProductoRotatividadRepository => this._ProductoRotatividadRepository ?? (this._ProductoRotatividadRepository = new ProductoRotatividadRepository(this._context, this._application, this._userId));
        private ProductoFamiliaRepository _ProductoFamiliaRepository; public ProductoFamiliaRepository ProductoFamiliaRepository => this._ProductoFamiliaRepository ?? (this._ProductoFamiliaRepository = new ProductoFamiliaRepository(this._context, this._application, this._userId));
        private UbicacionAreaRepository _UbicacionAreaRepository; public UbicacionAreaRepository UbicacionAreaRepository => this._UbicacionAreaRepository ?? (this._UbicacionAreaRepository = new UbicacionAreaRepository(this._context, this._application, this._userId));
        private UbicacionPickingProductoRepository _UbicacionPickingProductoRepository; public UbicacionPickingProductoRepository UbicacionPickingProductoRepository => this._UbicacionPickingProductoRepository ?? (this._UbicacionPickingProductoRepository = new UbicacionPickingProductoRepository(this._context, this._application, this._userId, this._dapper));
        private FuncionarioRepository _FuncionarioRepository; public FuncionarioRepository FuncionarioRepository => this._FuncionarioRepository ?? (this._FuncionarioRepository = new FuncionarioRepository(this._context, this._application, this._userId));
        private FileRepository _FileRepository; public FileRepository FileRepository => this._FileRepository ?? (this._FileRepository = new FileRepository(this._context, this._application, this._userId));
        private EquipoRepository _EquipoRepository; public EquipoRepository EquipoRepository => this._EquipoRepository ?? (this._EquipoRepository = new EquipoRepository(this._context, this._application, this._userId, _dapper));
        private OndaRepository _OndaRepository; public OndaRepository OndaRepository => this._OndaRepository ?? (this._OndaRepository = new OndaRepository(this._context, this._application, this._userId));
        private ParametroRepository _ParametroRepository; public ParametroRepository ParametroRepository => this._ParametroRepository ?? (this._ParametroRepository = new ParametroRepository(this._context, this._application, this._userId, _dapper));
        private LocalizacionRepository _LocalizacionRepository; public LocalizacionRepository LocalizationRepository => this._LocalizacionRepository ?? (this._LocalizacionRepository = new LocalizacionRepository(this._context, this._application, this._userId, _dapper));
        private PaisRepository _PaisRepository; public PaisRepository PaisRepository => this._PaisRepository ?? (this._PaisRepository = new PaisRepository(this._context, this._application, this._userId));
        private PaisSubdivisionRepository _PaisSubdivisionRepository; public PaisSubdivisionRepository PaisSubdivisionRepository => this._PaisSubdivisionRepository ?? (this._PaisSubdivisionRepository = new PaisSubdivisionRepository(this._context, this._application, this._userId, _dapper));
        private PaisSubdivisionLocalidadRepository _PaisSubdivisionLocalidadRepository; public PaisSubdivisionLocalidadRepository PaisSubdivisionLocalidadRepository => this._PaisSubdivisionLocalidadRepository ?? (this._PaisSubdivisionLocalidadRepository = new PaisSubdivisionLocalidadRepository(this._context, this._application, this._userId, _dapper));
        private ReferenciaRecepcionRepository _ReferenciaRecepcionRepository; public ReferenciaRecepcionRepository ReferenciaRecepcionRepository => this._ReferenciaRecepcionRepository ?? (this._ReferenciaRecepcionRepository = new ReferenciaRecepcionRepository(this._context, this._application, this._userId, _dapper));
        private RecepcionTipoRepository _RecepcionTipoRepository; public RecepcionTipoRepository RecepcionTipoRepository => this._RecepcionTipoRepository ?? (this._RecepcionTipoRepository = new RecepcionTipoRepository(this._context, this._application, this._userId, this._dapper));
        private RutaRepository _RutaRepository; public RutaRepository RutaRepository => this._RutaRepository ?? (this._RutaRepository = new RutaRepository(this._context, this._application, this._userId, _dapper));
        private StockRepository _StockRepository; public StockRepository StockRepository => this._StockRepository ?? (this._StockRepository = new StockRepository(this._context, this._application, this._userId, _dapper));
        private StockTraceRepository _StockTraceRepository; public StockTraceRepository StockTraceRepository => this._StockTraceRepository ?? (this._StockTraceRepository = new StockTraceRepository(this._context, this._application, this._userId));
        private AjusteRepository _AjusteRepository; public AjusteRepository AjusteRepository => this._AjusteRepository ?? (this._AjusteRepository = new AjusteRepository(this._context, this._application, this._userId, _dapper));
        private SecurityRepository _SecurityRepository; public SecurityRepository SecurityRepository => this._SecurityRepository ?? (this._SecurityRepository = new SecurityRepository(this._context, this._application, this._userId, _dapper));
        private TipoAlmacenajeSeguroRepository _TipoAlmacenajeSeguroRepository; public TipoAlmacenajeSeguroRepository TipoAlmacenajeSeguroRepository => this._TipoAlmacenajeSeguroRepository ?? (this._TipoAlmacenajeSeguroRepository = new TipoAlmacenajeSeguroRepository(this._context, this._application, this._userId));
        private ImpresionRepository _ImpresionRepository; public ImpresionRepository ImpresionRepository => this._ImpresionRepository ?? (this._ImpresionRepository = new ImpresionRepository(this._context, this._application, this._userId, _dapper));
        private TemplateImpresionRepository _TemplateImpresionRepository; public TemplateImpresionRepository TemplateImpresionRepository => this._TemplateImpresionRepository ?? (this._TemplateImpresionRepository = new TemplateImpresionRepository(this._context, this._application, this._userId));
        private EstiloEtiquetaRepository _EstiloEtiquetaRepository; public EstiloEtiquetaRepository EstiloEtiquetaRepository => this._EstiloEtiquetaRepository ?? (this._EstiloEtiquetaRepository = new EstiloEtiquetaRepository(this._context, this._application, this._userId));
        private CodigoBarrasRepository _CodigoBarrasRepository; public CodigoBarrasRepository CodigoBarrasRepository => this._CodigoBarrasRepository ?? (this._CodigoBarrasRepository = new CodigoBarrasRepository(this._context, this._application, this._userId, _dapper));
        private TransportistaRepository _TransportistaRepository; public TransportistaRepository TransportistaRepository => this._TransportistaRepository ?? (this._TransportistaRepository = new TransportistaRepository(this._context, this._application, this._userId));
        private UbicacionRepository _UbicacionRepository; public UbicacionRepository UbicacionRepository => this._UbicacionRepository ?? (this._UbicacionRepository = new UbicacionRepository(this._context, this._application, this._userId, this._dapper));
        private UbicacionTipoRepository _UbicacionTipoRepository; public UbicacionTipoRepository UbicacionTipoRepository => this._UbicacionTipoRepository ?? (this._UbicacionTipoRepository = new UbicacionTipoRepository(this._context, this._application, this._userId));
        private UnidadMedidaRepository _UnidadMedidaRepository; public UnidadMedidaRepository UnidadMedidaRepository => this._UnidadMedidaRepository ?? (this._UnidadMedidaRepository = new UnidadMedidaRepository(this._context, this._application, this._userId));
        private ViaRepository _ViaRepository; public ViaRepository ViaRepository => this._ViaRepository ?? (this._ViaRepository = new ViaRepository(this._context, this._application, this._userId));
        private CamionRepository _CamionRepository; public CamionRepository CamionRepository => this._CamionRepository ?? (this._CamionRepository = new CamionRepository(this._context, this._application, this._userId, _dapper));
        private CargaCamionRepository _CargaCamionRepository; public CargaCamionRepository CargaCamionRepository => this._CargaCamionRepository ?? (this._CargaCamionRepository = new CargaCamionRepository(this._context, this._application, this._userId, _dapper));
        private PreparacionRepository _PreparacionRepository; public PreparacionRepository PreparacionRepository => this._PreparacionRepository ?? (this._PreparacionRepository = new PreparacionRepository(this._context, this._application, this._userId, _dapper));
        private PedidoRepository _PedidoRepository; public PedidoRepository PedidoRepository => this._PedidoRepository ?? (this._PedidoRepository = new PedidoRepository(this._context, this._application, this._userId, _dapper));
        private TransaccionRepository _TransaccionRepository; public TransaccionRepository TransaccionRepository => this._TransaccionRepository ?? (this._TransaccionRepository = new TransaccionRepository(this._context, this._application, this._userId, _dapper));
        private ContenedorRepository _ContenedorRepository; public ContenedorRepository ContenedorRepository => this._ContenedorRepository ?? (this._ContenedorRepository = new ContenedorRepository(this._context, this._application, this._userId, _dapper));
        private LiberacionRepository _LiberacionRepository; public LiberacionRepository LiberacionRepository => this._LiberacionRepository ?? (this._LiberacionRepository = new LiberacionRepository(this._context, this._application, this._userId, this._dapper));
        private PredioRepository _PredioRepository; public PredioRepository PredioRepository => this._PredioRepository ?? (this._PredioRepository = new PredioRepository(this._context, this._application, this._userId, _dapper));
        private EventoRepository _EventoRepository; public EventoRepository EventoRepository => this._EventoRepository ?? (this._EventoRepository = new EventoRepository(this._context, this._application, this._userId, this._dapper));
        private NotificacionRepository _NotificacionRepository; public NotificacionRepository NotificacionRepository => this._NotificacionRepository ?? (this._NotificacionRepository = new NotificacionRepository(this._context, this._application, this._userId, this._dapper));
        private DestinatarioRepository _DestinatarioRepository; public DestinatarioRepository DestinatarioRepository => this._DestinatarioRepository ?? (this._DestinatarioRepository = new DestinatarioRepository(this._context, this._application, this._userId, this._dapper));
        private InterfazRepository _InterfazRepository; public InterfazRepository InterfazRepository => this._InterfazRepository ?? (this._InterfazRepository = new InterfazRepository(this._context, this._application, this._userId, this._dapper));
        private ImpresoraRepository _ImpresoraRepository; public ImpresoraRepository ImpresoraRepository => this._ImpresoraRepository ?? (this._ImpresoraRepository = new ImpresoraRepository(this._context, this._application, this._userId, _dapper));
        private ReporteRepository _ReporteRepository; public ReporteRepository ReporteRepository => this._ReporteRepository ?? (this._ReporteRepository = new ReporteRepository(this._context, this._application, this._userId, this._dapper));
        private EnvaseRepository _EnvaseRepository; public EnvaseRepository EnvaseRepository => this._EnvaseRepository ?? (this._EnvaseRepository = new EnvaseRepository(this._context, this._application, this._userId));
        private SituacionRepository _SituacionRepository; public SituacionRepository SituacionRepository => this._SituacionRepository ?? (this._SituacionRepository = new SituacionRepository(this._context, this._application, this._userId));
        private ProductoCodigoBarraRepository _ProductoCodigoBarraRepository; public ProductoCodigoBarraRepository ProductoCodigoBarraRepository => this._ProductoCodigoBarraRepository ?? (this._ProductoCodigoBarraRepository = new ProductoCodigoBarraRepository(this._context, this._application, this._userId));
        private VehiculoRepository _VehiculoRepository; public VehiculoRepository VehiculoRepository => this._VehiculoRepository ?? (this._VehiculoRepository = new VehiculoRepository(this._context, this._application, this._userId, this._dapper));
        private ValidezRepository _ValidezRepository; public ValidezRepository ValidezRepository => this._ValidezRepository ?? (this._ValidezRepository = new ValidezRepository(this._context, this._application, this._userId));
        private TipoVehiculoRepository _TipoVehiculoRepository; public TipoVehiculoRepository TipoVehiculoRepository => this._TipoVehiculoRepository ?? (this._TipoVehiculoRepository = new TipoVehiculoRepository(this._context, this._application, this._userId, this._dapper));
        private AnulacionRepository _AnulacionRepository; public AnulacionRepository AnulacionRepository => this._AnulacionRepository ?? (this._AnulacionRepository = new AnulacionRepository(this._context, this._application, this._userId, this._dapper));
        private ZonaUbicacionRepository _ZonaUbicacionRepository; public ZonaUbicacionRepository ZonaUbicacionRepository => this._ZonaUbicacionRepository ?? (this._ZonaUbicacionRepository = new ZonaUbicacionRepository(this._context, this._application, this._userId, this._dapper));
        private ZonaRepository _ZonaRepository; public ZonaRepository ZonaRepository => this._ZonaRepository ?? (this._ZonaRepository = new ZonaRepository(this._context, this._application, this._userId));
        private TrackingRepository _TrackingRepository; public TrackingRepository TrackingRepository => this._TrackingRepository ?? (this._TrackingRepository = new TrackingRepository(this._context, this._application, this._userId));
        private ProductoRamoRepository _ProductoRamoRepository; public ProductoRamoRepository ProductoRamoRepository => this._ProductoRamoRepository ?? (this._ProductoRamoRepository = new ProductoRamoRepository(this._context, this._application, this._userId));
        private EjecucionRepository _EjecucionRepository; public EjecucionRepository EjecucionRepository => this._EjecucionRepository ?? (this._EjecucionRepository = new EjecucionRepository(_dapper));
        private CuentaContableRepository _CuentaContableRepository; public CuentaContableRepository CuentaContableRepository => this._CuentaContableRepository ?? (this._CuentaContableRepository = new CuentaContableRepository(this._context, this._application, this._userId));
        private CotizacionListasRepository _CotizacionListasRepository; public CotizacionListasRepository CotizacionListasRepository => this._CotizacionListasRepository ?? (this._CotizacionListasRepository = new CotizacionListasRepository(this._context, this._application, this._userId));
        private ListaPrecioRepository _ListaPrecioRepository; public ListaPrecioRepository ListaPrecioRepository => this._ListaPrecioRepository ?? (this._ListaPrecioRepository = new ListaPrecioRepository(this._context, this._application, this._userId));
        private ConfiguracionIdiomaRepository _ConfiguracionIdiomaRepository; public ConfiguracionIdiomaRepository ConfiguracionIdiomaRepository => this._ConfiguracionIdiomaRepository ?? (this._ConfiguracionIdiomaRepository = new ConfiguracionIdiomaRepository(this._context, this._application, this._userId));
        private TemplateEtiquetaRepository _TemplateEtiquetaRepository; public TemplateEtiquetaRepository TemplateEtiquetaRepository => this._TemplateEtiquetaRepository ?? (this._TemplateEtiquetaRepository = new TemplateEtiquetaRepository(this._context, this._application, this._userId));
        private EstiloContenedorRepository _EstiloContenedorRepository; public EstiloContenedorRepository EstiloContenedorRepository => this._EstiloContenedorRepository ?? (this._EstiloContenedorRepository = new EstiloContenedorRepository(this._context, this._application, this._userId));
        private LenguajeImpresionRepository _LenguajeImpresionRepository; public LenguajeImpresionRepository LenguajeImpresionRepository => this._LenguajeImpresionRepository ?? (this._LenguajeImpresionRepository = new LenguajeImpresionRepository(this._context, this._application, this._userId));
        private TareaRepository _TareaRepository; public TareaRepository TareaRepository => this._TareaRepository ?? (this._TareaRepository = new TareaRepository(this._context, this._application, this._userId, this._dapper));
        private InsumoManipuleoRepository _InsumoManipuleoRepository; public InsumoManipuleoRepository InsumoManipuleoRepository => this._InsumoManipuleoRepository ?? (this._InsumoManipuleoRepository = new InsumoManipuleoRepository(this._context, this._application, this._userId));
        private OrdenRepository _OrdenRepository; public OrdenRepository OrdenRepository => this._OrdenRepository ?? (this._OrdenRepository = new OrdenRepository(this._context, this._application, this._userId, this._dapper));
        private EstrategiaRepository _EstrategiaRepository; public EstrategiaRepository EstrategiaRepository => this._EstrategiaRepository ?? (this._EstrategiaRepository = new EstrategiaRepository(this._context, this._application, this._userId, this._dapper));
        private ManejoLpnRepository _ManejoLpnRepository; public ManejoLpnRepository ManejoLpnRepository => this._ManejoLpnRepository ?? (this._ManejoLpnRepository = new ManejoLpnRepository(this._context, this._application, this._userId, _dapper));
        private AtributoRepository _AtributoRepository; public AtributoRepository AtributoRepository => this._AtributoRepository ?? (this._AtributoRepository = new AtributoRepository(this._context, this._application, this._userId, _dapper));
        private ClaseRepository _ClaseRepository; public ClaseRepository ClaseRepository => this._ClaseRepository ?? (this._ClaseRepository = new ClaseRepository(this._context, this._application, this._userId, _dapper));
        private GrupoRepository _GrupoRepository; public GrupoRepository GrupoRepository => this._GrupoRepository ?? (this._GrupoRepository = new GrupoRepository(this._context, this._application, this._userId, _dapper));
        private MesaDeClasificacionRepository _MesaDeClasificacionRepository; public MesaDeClasificacionRepository MesaDeClasificacionRepository => this._MesaDeClasificacionRepository ?? (this._MesaDeClasificacionRepository = new MesaDeClasificacionRepository(this._context, this._application, this._userId, _dapper));
        private UnidadTransporteRepository _UnidadTransporteRepository; public UnidadTransporteRepository UnidadTransporteRepository => this._UnidadTransporteRepository ?? (this._UnidadTransporteRepository = new UnidadTransporteRepository(this._context, this._application, this._userId, _dapper));
        private EmpaquetadoPickingRepository _EmpaquetadoPickingRepository; public EmpaquetadoPickingRepository EmpaquetadoPickingRepository => this._EmpaquetadoPickingRepository ?? (this._EmpaquetadoPickingRepository = new EmpaquetadoPickingRepository(this._context, this._application, this._userId, _dapper));
        private AutomatismoRepository _AutomatismoRepository; public AutomatismoRepository AutomatismoRepository => this._AutomatismoRepository ?? (this._AutomatismoRepository = new AutomatismoRepository(this._context, this._application, this._userId, _dapper));
        private AutomatismoEjecucionRepository _AutomatismoEjecucionRepository; public AutomatismoEjecucionRepository AutomatismoEjecucionRepository => this._AutomatismoEjecucionRepository ?? (this._AutomatismoEjecucionRepository = new AutomatismoEjecucionRepository(this._context, this._application, this._userId, _dapper));
        private AutomatismoDataRepository _AutomatismoDataRepository; public AutomatismoDataRepository AutomatismoDataRepository => this._AutomatismoDataRepository ?? (this._AutomatismoDataRepository = new AutomatismoDataRepository(this._context, this._application, this._userId, _dapper));
        private AutomatismoCaracteristicaRepository _AutomatismoCaracteristicaRepository; public AutomatismoCaracteristicaRepository AutomatismoCaracteristicaRepository => this._AutomatismoCaracteristicaRepository ?? (this._AutomatismoCaracteristicaRepository = new AutomatismoCaracteristicaRepository(this._context, this._application, this._userId, _dapper));
        private AutomatismoInterfazRepository _AutomatismoInterfazRepository; public AutomatismoInterfazRepository AutomatismoInterfazRepository => this._AutomatismoInterfazRepository ?? (this._AutomatismoInterfazRepository = new AutomatismoInterfazRepository(this._context, this._application, this._userId, _dapper));
        private AutomatismoPosicionRepository _AutomatismoPosicionRepository; public AutomatismoPosicionRepository AutomatismoPosicionRepository => this._AutomatismoPosicionRepository ?? (this._AutomatismoPosicionRepository = new AutomatismoPosicionRepository(this._context, this._application, this._userId, _dapper));
        private AutomatismoProductosAsociadosRepository _AutomatismoProductosAsociadosRepository; public AutomatismoProductosAsociadosRepository AutomatismoProductosAsociadosRepository => this._AutomatismoProductosAsociadosRepository ?? (this._AutomatismoProductosAsociadosRepository = new AutomatismoProductosAsociadosRepository(this._context, this._application, this._userId, _dapper));
        private AutomatismoPuestoRepository _AutomatismoPuestoRepository; public AutomatismoPuestoRepository AutomatismoPuestoRepository => this._AutomatismoPuestoRepository ?? (this._AutomatismoPuestoRepository = new AutomatismoPuestoRepository(this._context, this._application, this._userId, _dapper));
        private IntegracionServicioRepository _IntegracionServicioRepository; public IntegracionServicioRepository IntegracionServicioRepository => this._IntegracionServicioRepository ?? (this._IntegracionServicioRepository = new IntegracionServicioRepository(this._context, this._application, this._userId, _dapper));
        private PtlRepository _PtlRepository; public PtlRepository PtlRepository => this._PtlRepository ?? (this._PtlRepository = new PtlRepository(this._context, this._application, this._userId, _dapper));
        private ContenedorPtlRepository _ContenedorPtlRepository; public ContenedorPtlRepository ContenedorPtlRepository => this._ContenedorPtlRepository ?? (this._ContenedorPtlRepository = new ContenedorPtlRepository(this._context, this._application, this._userId, this._dapper));
        private EspacioProduccionRepository _EspacioProduccionRepository; public EspacioProduccionRepository EspacioProduccionRepository => this._EspacioProduccionRepository ?? (this._EspacioProduccionRepository = new EspacioProduccionRepository(this._context, this._application, this._userId, _dapper));
        private IngresoProduccionRepository _IngresoProduccionRepository; public IngresoProduccionRepository IngresoProduccionRepository => this._IngresoProduccionRepository ?? (this._IngresoProduccionRepository = new IngresoProduccionRepository(this._context, this._application, this._userId, _dapper));
        private CodigoMultidatoRepository _CodigoMultidatoRepository; public CodigoMultidatoRepository CodigoMultidatoRepository => this._CodigoMultidatoRepository ?? (this._CodigoMultidatoRepository = new CodigoMultidatoRepository(this._context, this._application, this._userId, _dapper));

        private RecorridoRepository _RecorridoRepository; public RecorridoRepository RecorridoRepository => this._RecorridoRepository ?? (this._RecorridoRepository = new RecorridoRepository(this._context, this._application, this._userId, _dapper));
        private ColaDeTrabajoRepository _ColaDeTrabajoRepository; public ColaDeTrabajoRepository ColaDeTrabajoRepository => this._ColaDeTrabajoRepository ?? (this._ColaDeTrabajoRepository = new ColaDeTrabajoRepository(this._context, this._application, this._userId, _dapper));

        private PreferenciaRepository _PreferenciaRepository; public PreferenciaRepository PreferenciaRepository => this._PreferenciaRepository ?? (this._PreferenciaRepository = new PreferenciaRepository(this._context, this._application, this._userId, _dapper));
                
        private FacturaRepository _FacturaRepository; public FacturaRepository FacturaRepository => this._FacturaRepository ?? (this._FacturaRepository = new FacturaRepository(this._context, this._application, this._userId, _dapper));

        private TraspasoEmpresasRepository _TraspasoEmpresasRepository; public TraspasoEmpresasRepository TraspasoEmpresasRepository => this._TraspasoEmpresasRepository ?? (this._TraspasoEmpresasRepository = new TraspasoEmpresasRepository(this._context, this._application, this._userId, _dapper));
    }
}
