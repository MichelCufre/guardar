namespace WIS.Persistence.Database
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using NLog.Extensions.Logging;
    using System;
    using System.Linq;
    using WIS.Persistence.Conventions;
    using WIS.Persistence.Extensions;

    public partial class WISDB : DbContext
    {
        protected long _numeroTransacion;
        public long GetTransactionNumber()
        {
            return _numeroTransacion;
        }

        public void SetTransactionNumber(long transaccion)
        {
            this._numeroTransacion = transaccion;
        }

        public static readonly ILoggerFactory _dbLoggerFactory = LoggerFactory.Create(builder =>
        {
            NLog.LogManager.LoadConfiguration("nlog.config");
            builder.AddProvider(new NLogLoggerProvider());
        });

        protected readonly IDatabaseConfigurationService _dbConfigService;
        protected readonly string _connectionString;
        protected readonly string _schema;
        protected readonly string _blobDataType;

        public WISDB(IDatabaseConfigurationService dbConfigService, string connectionString, string schema)
            : base()
        {
            this._schema = schema;
            this._dbConfigService = dbConfigService;
            this._connectionString = connectionString;
            this._blobDataType = dbConfigService.GetBlobDataType();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            this._dbConfigService.Configure(optionsBuilder);
        }

        public virtual DbSet<DbSequence> DbSequence { get; set; }

        #region >> - TABLAS -
        public virtual DbSet<T_RECORRIDO> T_RECORRIDO { get; set; }
        public virtual DbSet<T_RECORRIDO_DET> T_RECORRIDO_DET { get; set; }
        public virtual DbSet<V_REG700_RECORRIDOS> V_REG700_RECORRIDOS { get; set; }
        public virtual DbSet<V_REG700_RECORRIDO_DET> V_REG700_RECORRIDO_DET { get; set; }
        public virtual DbSet<I_E_ESTAN_AGENTE> I_E_ESTAN_AGENTE { get; set; }
        public virtual DbSet<I_E_ESTAN_CODIGO_BARRAS> I_E_ESTAN_CODIGO_BARRAS { get; set; }
        public virtual DbSet<I_E_ESTAN_CONVERTEDOR> I_E_ESTAN_CONVERTEDOR { get; set; }
        public virtual DbSet<I_E_ESTAN_PEDIDO_SAIDA> I_E_ESTAN_PEDIDO_SAIDA { get; set; }
        public virtual DbSet<I_E_ESTAN_PRODUTO> I_E_ESTAN_PRODUTO { get; set; }
        public virtual DbSet<I_E_ESTAN_REF_RECEPCION> I_E_ESTAN_REF_RECEPCION { get; set; }
        public virtual DbSet<I_S_ESTAN_CONF_PEDI_PEDIDO> I_S_ESTAN_CONF_PEDI_PEDIDO { get; set; }
        public virtual DbSet<I_S_ESTAN_CONF_PEDI_PEDIDO_DET> I_S_ESTAN_CONF_PEDI_PEDIDO_DET { get; set; }
        public virtual DbSet<I_S_ESTAN_PEDIDO_ANULADO> I_S_ESTAN_PEDIDO_ANULADO { get; set; }
        public virtual DbSet<I_S_ESTAN_AJUSTE_STOCK> I_S_ESTAN_AJUSTE_STOCK { get; set; }
        public virtual DbSet<I_S_ESTAN_CONF_PEDI_CAMION> I_S_ESTAN_CONF_PEDI_CAMION { get; set; }
        public virtual DbSet<I_E_ESTAN_PEDIDO_SAIDA_DET> I_E_ESTAN_PEDIDO_SAIDA_DET { get; set; }
        public virtual DbSet<I_E_ESTAN_REF_RECEPCION_DET> I_E_ESTAN_REF_RECEPCION_DET { get; set; }
        public virtual DbSet<T_CLASSE> T_CLASSE { get; set; }
        public virtual DbSet<T_CLIENTE> T_CLIENTE { get; set; }
        public virtual DbSet<T_CLIENTE_RUTA_PREDIO> T_CLIENTE_RUTA_PREDIO { get; set; }
        public virtual DbSet<T_CODIGO_BARRAS> T_CODIGO_BARRAS { get; set; }
        public virtual DbSet<T_ARCHIVO> T_ARCHIVO { get; set; }
        public virtual DbSet<T_CONDICION_LIBERACION> T_CONDICION_LIBERACION { get; set; }
        public virtual DbSet<T_TEMP_CONDICION_LIBERACION> T_TEMP_CONDICION_LIBERACION { get; set; }
        public virtual DbSet<T_CONTROL_ACCESO> T_CONTROL_ACCESO { get; set; }
        public virtual DbSet<T_CTR_CALIDAD_NECESARIO> T_CTR_CALIDAD_NECESARIO { get; set; }
        public virtual DbSet<T_DET_AGENDA> T_DET_AGENDA { get; set; }
        public virtual DbSet<T_DET_ETIQUETA_LOTE> T_DET_ETIQUETA_LOTE { get; set; }
        public virtual DbSet<T_DOCUMENTO_SEL_TP_LIBERACION> T_DOCUMENTO_SEL_TP_LIBERACION { get; set; }
        public virtual DbSet<T_DOCUMENTO_LIBERACION> T_DOCUMENTO_LIBERACION { get; set; }
        public virtual DbSet<T_EMPRESA> T_EMPRESA { get; set; }
        public virtual DbSet<T_ENDERECO_ESTOQUE> T_ENDERECO_ESTOQUE { get; set; }
        public virtual DbSet<T_ESTACION_TRABAJO> T_ESTACION_TRABAJO { get; set; }
        public virtual DbSet<T_ETIQUETA_LOTE> T_ETIQUETA_LOTE { get; set; }
        public virtual DbSet<T_ETIQUETAS_EN_USO> T_ETIQUETAS_EN_USO { get; set; }
        public virtual DbSet<T_FAMILIA_PRODUTO> T_FAMILIA_PRODUTO { get; set; }
        public virtual DbSet<T_FERRAMENTAS> T_FERRAMENTAS { get; set; }
        public virtual DbSet<T_IMPRESION> T_IMPRESION { get; set; }
        public virtual DbSet<T_LOG_ETIQUETA> T_LOG_ETIQUETA { get; set; }
        public virtual DbSet<T_EQUIPO> T_EQUIPO { get; set; }
        public virtual DbSet<T_LOG_PEDIDO_ANULADO> T_LOG_PEDIDO_ANULADO { get; set; }
        public virtual DbSet<T_LOG_PEDIDO_ANULADO_LPN> T_LOG_PEDIDO_ANULADO_LPN { get; set; }
        public virtual DbSet<T_ONDA> T_ONDA { get; set; }
        public virtual DbSet<T_PALLET_TRANSFERENCIA> T_PALLET_TRANSFERENCIA { get; set; }
        public virtual DbSet<T_DET_PALLET_TRANSFERENCIA> T_DET_PALLET_TRANSFERENCIA { get; set; }
        public virtual DbSet<T_PAIS> T_PAIS { get; set; }
        public virtual DbSet<T_PAIS_SUBDIVISION> T_PAIS_SUBDIVISION { get; set; }
        public virtual DbSet<T_PAIS_SUBDIVISION_LOCALIDAD> T_PAIS_SUBDIVISION_LOCALIDAD { get; set; }
        public virtual DbSet<T_PICKING_PRODUTO> T_PICKING_PRODUTO { get; set; }
        public virtual DbSet<T_PORTA_EMBARQUE> T_PORTA_EMBARQUE { get; set; }
        public virtual DbSet<T_PRODUTO> T_PRODUTO { get; set; }
        public virtual DbSet<T_PRODUTO_PALLET> T_PRODUTO_PALLET { get; set; }
        public virtual DbSet<T_PRODUTO_FAIXA> T_PRODUTO_FAIXA { get; set; }
        public virtual DbSet<T_RECEPCION_AGENDA_PROBLEMA> T_RECEPCION_AGENDA_PROBLEMA { get; set; }
        public virtual DbSet<T_RECEPCION_AGENDA_REFERENCIA> T_RECEPCION_AGENDA_REFERENCIA { get; set; }
        public virtual DbSet<T_RECEPC_AGENDA_REFERENCIA_REL> T_RECEPC_AGENDA_REFERENCIA_REL { get; set; }
        public virtual DbSet<T_RECEPCION_EMP_TIPO_REPORTE> T_RECEPCION_EMP_TIPO_REPORTE { get; set; }
        public virtual DbSet<T_RECEPCION_REL_EMPRESA_TIPO> T_RECEPCION_REL_EMPRESA_TIPO { get; set; }
        public virtual DbSet<T_RECEPCION_REFERENCIA> T_RECEPCION_REFERENCIA { get; set; }
        public virtual DbSet<T_RECEPCION_REFERENCIA_DET> T_RECEPCION_REFERENCIA_DET { get; set; }
        public virtual DbSet<T_RECEPCION_REFERENCIA_TIPO> T_RECEPCION_REFERENCIA_TIPO { get; set; }
        public virtual DbSet<T_RECEPCION_TIPO> T_RECEPCION_TIPO { get; set; }
        public virtual DbSet<T_ESTACION_CLASIFICACION> T_ESTACION_CLASIFICACION { get; set; }
        public virtual DbSet<T_REPORTE_DEFINICION> T_REPORTE_DEFINICION { get; set; }
        public virtual DbSet<T_ROTA> T_ROTA { get; set; }
        public virtual DbSet<T_ROTATIVIDADE> T_ROTATIVIDADE { get; set; }
        public virtual DbSet<T_RAMO_PRODUTO> T_RAMO_PRODUTO { get; set; }
        public virtual DbSet<T_SITUACAO> T_SITUACAO { get; set; }
        public virtual DbSet<T_SUB_CLASSE> T_SUB_CLASSE { get; set; }
        public virtual DbSet<T_TIPO_CODIGO_BARRAS> T_TIPO_CODIGO_BARRAS { get; set; }
        public virtual DbSet<T_TIPO_CTR_CALIDAD> T_TIPO_CTR_CALIDAD { get; set; }
        public virtual DbSet<T_TIPO_ENDERECO> T_TIPO_ENDERECO { get; set; }
        public virtual DbSet<T_TIPO_ENDERECO_PALLET> T_TIPO_ENDERECO_PALLET { get; set; }
        public virtual DbSet<T_TRANSPORTADORA> T_TRANSPORTADORA { get; set; }
        public virtual DbSet<T_USUARIO_CONFIGURACION> T_USUARIO_CONFIGURACION { get; set; }
        public virtual DbSet<T_ZONA> T_ZONA { get; set; }
        public virtual DbSet<T_ZONA_UBICACION> T_ZONA_UBICACION { get; set; }
        public virtual DbSet<T_FUNCIONARIO> T_FUNCIONARIO { get; set; }
        public virtual DbSet<T_FACTURACION_CODIGO_COMPONEN> T_FACTURACION_CODIGO_COMPONEN { get; set; }
        public virtual DbSet<T_FACTURACION_CUENTA_CONTABLE> T_FACTURACION_CUENTA_CONTABLE { get; set; }
        public virtual DbSet<T_FACTURACION_LISTA_COTIZACION> T_FACTURACION_LISTA_COTIZACION { get; set; }
        public virtual DbSet<T_FACTURACION_LISTA_PRECIO> T_FACTURACION_LISTA_PRECIO { get; set; }
        public virtual DbSet<T_FACTURACION_PROCESO> T_FACTURACION_PROCESO { get; set; }
        public virtual DbSet<T_FACTURACION_CODIGO> T_FACTURACION_CODIGO { get; set; }
        public virtual DbSet<T_ORT_ORDEN_TAREA_DATO> T_ORT_ORDEN_TAREA_DATO { get; set; }
        public virtual DbSet<T_DOCUMENTO_ANU_PREP_RESERVA> T_DOCUMENTO_ANU_PREP_RESERVA { get; set; }
        public virtual DbSet<T_DOCUMENTO_PRDC_ENTRADA> T_DOCUMENTO_PRDC_ENTRADA { get; set; }
        public virtual DbSet<T_DOCUMENTO_AGRUPADOR> T_DOCUMENTO_AGRUPADOR { get; set; }
        public virtual DbSet<T_DOCUMENTO_AGRUPADOR_TIPO> T_DOCUMENTO_AGRUPADOR_TIPO { get; set; }
        public virtual DbSet<T_DOCUMENTO_AGRUPADOR_GRUPO> T_DOCUMENTO_AGRUPADOR_GRUPO { get; set; }
        public virtual DbSet<T_DOCUMENTO_TIPO_EXTERNO> T_DOCUMENTO_TIPO_EXTERNO { get; set; }
        public virtual DbSet<T_DOCUMENTO_TIPO_EDITABLE_DET> T_DOCUMENTO_TIPO_EDITABLE_DET { get; set; }
        public virtual DbSet<T_PRDC_MOVIMIENTO_BB> T_PRDC_MOVIMIENTO_BB { get; set; }
        public virtual DbSet<I_E_PRDC_SALIDA_PRD> I_E_PRDC_SALIDA_PRD { get; set; }
        public virtual DbSet<I_E_PRDC_SALIDA_PRD_INSUMO> I_E_PRDC_SALIDA_PRD_INSUMO { get; set; }
        public virtual DbSet<I_E_PRDC_SALIDA_PRD_PRODUCIDO> I_E_PRDC_SALIDA_PRD_PRODUCIDO { get; set; }
        public virtual DbSet<T_DET_DOCU_EGRESO_CAMBIO> T_DET_DOCU_EGRESO_CAMBIO { get; set; }
        public virtual DbSet<T_DET_DOCU_EGRESO_RESERV> T_DET_DOCU_EGRESO_RESERV { get; set; }
        public virtual DbSet<T_PRDC_CUENTA_CORRIENTE_INSUMO> T_PRDC_CUENTA_CORRIENTE_INSUMO { get; set; }
        public virtual DbSet<LT_CAMBIO_DOCUMENTO_DET> LT_CAMBIO_DOCUMENTO_DET { get; set; }
        public virtual DbSet<T_CAMBIO_DOCUMENTO_DET> T_CAMBIO_DOCUMENTO_DET { get; set; }
        public virtual DbSet<T_PRDC_CUENTA_CAMBIO_DOC> T_PRDC_CUENTA_CAMBIO_DOC { get; set; }
        public virtual DbSet<T_PRDC_INGRESO> T_PRDC_INGRESO { get; set; }
        public virtual DbSet<T_PRDC_LINEA> T_PRDC_LINEA { get; set; }
        public virtual DbSet<T_PRDC_LINEA_CONSUMIDO> T_PRDC_LINEA_CONSUMIDO { get; set; }
        public virtual DbSet<T_PRDC_LINEA_PRODUCIDO> T_PRDC_LINEA_PRODUCIDO { get; set; }
        public virtual DbSet<T_PRDC_DEFINICION> T_PRDC_DEFINICION { get; set; }
        public virtual DbSet<T_HIST_PRDC_LINEA_CONSUMIDO> T_HIST_PRDC_LINEA_CONSUMIDO { get; set; }
        public virtual DbSet<T_HIST_PRDC_LINEA_PRODUCIDO> T_HIST_PRDC_LINEA_PRODUCIDO { get; set; }
        public virtual DbSet<T_REGIMEN_ADUANA> T_REGIMEN_ADUANA { get; set; }
        public virtual DbSet<T_DOCUMENTO_AJUSTE_STOCK> T_DOCUMENTO_AJUSTE_STOCK { get; set; }
        public virtual DbSet<T_DOCUMENTO_AJUSTE_STOCK_HIST> T_DOCUMENTO_AJUSTE_STOCK_HIST { get; set; }
        public virtual DbSet<T_PRDC_CONFIGURAR_PASADA> T_PRDC_CONFIGURAR_PASADA { get; set; }
        public virtual DbSet<T_PRDC_DET_ENTRADA> T_PRDC_DET_ENTRADA { get; set; }
        public virtual DbSet<T_PRDC_DET_SALIDA> T_PRDC_DET_SALIDA { get; set; }
        public virtual DbSet<T_PRDC_INGRESO_PASADA> T_PRDC_INGRESO_PASADA { get; set; }
        public virtual DbSet<T_PRDC_INGRESO_DOCUMENTO> T_PRDC_INGRESO_DOCUMENTO { get; set; }
        public virtual DbSet<T_HIST_PRDC_INGRESO_PASADA> T_HIST_PRDC_INGRESO_PASADA { get; set; }
        public virtual DbSet<T_PRDC_ACCION> T_PRDC_ACCION { get; set; }
        public virtual DbSet<T_PRDC_ACCION_INSTANCIA> T_PRDC_ACCION_INSTANCIA { get; set; }
        public virtual DbSet<T_PRDC_CONSUMO_IDENTIFICADOR> T_PRDC_CONSUMO_IDENTIFICADOR { get; set; }
        public virtual DbSet<T_PRDC_EGRESO_IDENTIFICADOR> T_PRDC_EGRESO_IDENTIFICADOR { get; set; }
        public virtual DbSet<T_TIPO_DUA_DOCUMENTO> T_TIPO_DUA_DOCUMENTO { get; set; }
        public virtual DbSet<T_TIPO_REFERENCIA_EXTERNA> T_TIPO_REFERENCIA_EXTERNA { get; set; }
        public virtual DbSet<T_TIPO_REFERENCIA_EXTERNA_DOCUMENTO> T_TIPO_REFERENCIA_EXTERNA_DOCUMENTO { get; set; }
        public virtual DbSet<T_LPN_TIPO> T_LPN_TIPO { get; set; }
        public virtual DbSet<T_LPN_BARRAS> T_LPN_BARRAS { get; set; }
        public virtual DbSet<T_ATRIBUTO> T_ATRIBUTO { get; set; }
        public virtual DbSet<T_ATRIBUTO_ESTADO> T_ATRIBUTO_ESTADO { get; set; }
        public virtual DbSet<T_PORTERIA_EMPRESA> T_PORTERIA_EMPRESA { get; set; }
        public virtual DbSet<T_PORTERIA_PERSONA> T_PORTERIA_PERSONA { get; set; }
        public virtual DbSet<T_PORTERIA_REGISTRO_PERSONA> T_PORTERIA_REGISTRO_PERSONA { get; set; }
        public virtual DbSet<T_PORTERIA_REGISTRO_VEHICULO> T_PORTERIA_REGISTRO_VEHICULO { get; set; }
        public virtual DbSet<T_PORTERIA_SECTOR> T_PORTERIA_SECTOR { get; set; }
        public virtual DbSet<T_PORTERIA_VEHICULO_AGENDA> T_PORTERIA_VEHICULO_AGENDA { get; set; }
        public virtual DbSet<T_PORTERIA_VEHICULO_CAMION> T_PORTERIA_VEHICULO_CAMION { get; set; }
        public virtual DbSet<T_PORTERIA_VEHICULO_OBJETO> T_PORTERIA_VEHICULO_OBJETO { get; set; }
        public virtual DbSet<T_REPORTE> T_REPORTE { get; set; }
        public virtual DbSet<T_REPORTE_RELACION> T_REPORTE_RELACION { get; set; }
        public virtual DbSet<T_INTERFAZ_EXTERNA> T_INTERFAZ_EXTERNA { get; set; }
        public virtual DbSet<T_INVENTARIO> T_INVENTARIO { get; set; }
        public virtual DbSet<T_INVENTARIO_ENDERECO> T_INVENTARIO_ENDERECO { get; set; }
        public virtual DbSet<T_INVENTARIO_ENDERECO_DET> T_INVENTARIO_ENDERECO_DET { get; set; }
        public virtual DbSet<T_GRID_DEFAULT_CONFIG> T_GRID_DEFAULT_CONFIG { get; set; }
        public virtual DbSet<T_GRID_USER_CONFIG> T_GRID_USER_CONFIG { get; set; }
        public virtual DbSet<T_STOCK> T_STOCK { get; set; }
        public virtual DbSet<T_TRACE_STOCK> T_TRACE_STOCK { get; set; }
        public virtual DbSet<T_DOCUMENTO> T_DOCUMENTO { get; set; }
        public virtual DbSet<T_DET_DOCUMENTO> T_DET_DOCUMENTO { get; set; }
        public virtual DbSet<T_DET_DOCUMENTO_ACTA> T_DET_DOCUMENTO_ACTA { get; set; }
        public virtual DbSet<T_DET_DOCUMENTO_EGRESO> T_DET_DOCUMENTO_EGRESO { get; set; }
        public virtual DbSet<T_VIA> T_VIA { get; set; }
        public virtual DbSet<T_MONEDA> T_MONEDA { get; set; }
        public virtual DbSet<T_DET_DOMINIO> T_DET_DOMINIO { get; set; }
        public virtual DbSet<T_DOMINIO> T_DOMINIO { get; set; }
        public virtual DbSet<T_ORT_ORDEN> T_ORT_ORDEN { get; set; }
        public virtual DbSet<T_ORT_ORDEN_TAREA> T_ORT_ORDEN_TAREA { get; set; }
        public virtual DbSet<T_DESPACHANTE> T_DESPACHANTE { get; set; }
        public virtual DbSet<T_TIPO_ALMACENAJE_SEGURO> T_TIPO_ALMACENAJE_SEGURO { get; set; }
        public virtual DbSet<T_UNIDADE_MEDIDA> T_UNIDADE_MEDIDA { get; set; }
        public virtual DbSet<T_DOCUMENTO_ESTADO> T_DOCUMENTO_ESTADO { get; set; }
        public virtual DbSet<T_DOCUMENTO_TIPO_ESTADO> T_DOCUMENTO_TIPO_ESTADO { get; set; }
        public virtual DbSet<T_DOCUMENTO_ESTADO_ORDEN> T_DOCUMENTO_ESTADO_ORDEN { get; set; }
        public virtual DbSet<T_TIPO_DUA> T_TIPO_DUA { get; set; }
        public virtual DbSet<T_DOCUMENTO_TIPO> T_DOCUMENTO_TIPO { get; set; }
        public virtual DbSet<T_DOCUMENTO_PREPARACION_RESERV> T_DOCUMENTO_PREPARACION_RESERV { get; set; }
        public virtual DbSet<LT_DELETE_DOCUMENTO_PREPARACION_RESERV> LT_DELETE_DOCUMENTO_PREPARACION_RESERV { get; set; }
        public virtual DbSet<LT_DET_DOCUMENTO> LT_DET_DOCUMENTO { get; set; }
        public virtual DbSet<LT_DET_DOCUMENTO_EGRESO> LT_DET_DOCUMENTO_EGRESO { get; set; }
        public virtual DbSet<T_DOCUMENTO_PRODUCCION> T_DOCUMENTO_PRODUCCION { get; set; }
        public virtual DbSet<T_DOCUMENTO_TRANSFERENCIA> T_DOCUMENTO_TRANSFERENCIA { get; set; }
        public virtual DbSet<T_LOCALIZACION> T_LOCALIZACION { get; set; }
        public virtual DbSet<T_LOCALIZACION_VERSION> T_LOCALIZACION_VERSION { get; set; }
        public virtual DbSet<T_AJUSTE_STOCK> T_AJUSTE_STOCK { get; set; }
        public virtual DbSet<T_NAM> T_NAM { get; set; }
        public virtual DbSet<T_VALIDEZ> T_VALIDEZ { get; set; }
        public virtual DbSet<T_TIPO_AREA> T_TIPO_AREA { get; set; }
        public virtual DbSet<T_MOTIVO_AJUSTE> T_MOTIVO_AJUSTE { get; set; }
        public virtual DbSet<T_LPARAMETRO> T_LPARAMETRO { get; set; }
        public virtual DbSet<T_LPARAMETRO_CONFIGURACION> T_LPARAMETRO_CONFIGURACION { get; set; }
        public virtual DbSet<T_LPARAMETRO_NIVEL> T_LPARAMETRO_NIVEL { get; set; }
        public virtual DbSet<T_PRODUTO_CONVERTOR> T_PRODUTO_CONVERTOR { get; set; }
        public virtual DbSet<PROFILERESOURCES> PROFILERESOURCES { get; set; }
        public virtual DbSet<PROFILES> PROFILES { get; set; }
        public virtual DbSet<RESOURCES> RESOURCES { get; set; }
        public virtual DbSet<T_EMPRESA_FUNCIONARIO> T_EMPRESA_FUNCIONARIO { get; set; }
        public virtual DbSet<T_GRUPO_CONSULTA> T_GRUPO_CONSULTA { get; set; }
        public virtual DbSet<T_GRUPO_CONSULTA_FUNCIONARIO> T_GRUPO_CONSULTA_FUNCIONARIO { get; set; }
        public virtual DbSet<USER_TOKEN> USER_TOKEN { get; set; }
        public virtual DbSet<USERDATA> USERDATA { get; set; }
        public virtual DbSet<USERPERMISSIONS> USERPERMISSIONS { get; set; }
        public virtual DbSet<USERS> USERS { get; set; }
        public virtual DbSet<USERTYPES> USERTYPES { get; set; }
        public virtual DbSet<T_GRID_FILTER> T_GRID_FILTER { get; set; }
        public virtual DbSet<T_GRID_FILTER_DET> T_GRID_FILTER_DET { get; set; }
        public virtual DbSet<T_PREDIO_USUARIO> T_PREDIO_USUARIO { get; set; }
        public virtual DbSet<T_PREDIO> T_PREDIO { get; set; }
        public virtual DbSet<T_INV_ENDERECO_DET_ERROR> T_INV_ENDERECO_DET_ERROR { get; set; }
        public virtual DbSet<T_CAMION> T_CAMION { get; set; }
        public virtual DbSet<T_CLIENTE_CAMION> T_CLIENTE_CAMION { get; set; }
        public virtual DbSet<T_CONTENEDOR> T_CONTENEDOR { get; set; }
        public virtual DbSet<T_DET_PEDIDO_SAIDA> T_DET_PEDIDO_SAIDA { get; set; }
        public virtual DbSet<T_DET_PEDIDO_SAIDA_DUP> T_DET_PEDIDO_SAIDA_DUP { get; set; }
        public virtual DbSet<T_DET_PICKING> T_DET_PICKING { get; set; }
        public virtual DbSet<T_PICKING> T_PICKING { get; set; }
        public virtual DbSet<T_TRANSACCION> T_TRANSACCION { get; set; }
        public virtual DbSet<T_TIPO_EXPEDICION> T_TIPO_EXPEDICION { get; set; }
        public virtual DbSet<T_DET_PEDIDO_EXPEDIDO> T_DET_PEDIDO_EXPEDIDO { get; set; }
        public virtual DbSet<T_PARAM> T_PARAM { get; set; }
        public virtual DbSet<T_TEMP_PEDIDO_MOSTRADOR> T_TEMP_PEDIDO_MOSTRADOR { get; set; }
        public virtual DbSet<T_CONTENEDORES_PREDEFINIDOS> T_CONTENEDORES_PREDEFINIDOS { get; set; }
        public virtual DbSet<T_CARGA> T_CARGA { get; set; }
        public virtual DbSet<T_CROSS_DOCK> T_CROSS_DOCK { get; set; }
        public virtual DbSet<T_DET_CROSS_DOCK> T_DET_CROSS_DOCK { get; set; }
        public virtual DbSet<T_REGLA_CONDICION_LIBERACION> T_REGLA_CONDICION_LIBERACION { get; set; }
        public virtual DbSet<T_REGLA_LIBERACION> T_REGLA_LIBERACION { get; set; }
        public virtual DbSet<T_REGLA_TIPO_EXPEDICION> T_REGLA_TIPO_EXPEDICION { get; set; }
        public virtual DbSet<T_REGLA_TIPO_PEDIDO> T_REGLA_TIPO_PEDIDO { get; set; }
        public virtual DbSet<T_REGLA_CLIENTES> T_REGLA_CLIENTES { get; set; }
        public virtual DbSet<T_PREP_NO_ANULAR> T_PREP_NO_ANULAR { get; set; }
        public virtual DbSet<T_AGENDA> T_AGENDA { get; set; }
        public virtual DbSet<T_PEDIDO_SAIDA> T_PEDIDO_SAIDA { get; set; }
        public virtual DbSet<T_INTERFAZ_EJECUCION> T_INTERFAZ_EJECUCION { get; set; }
        public virtual DbSet<T_INTERFAZ_EJECUCION_ERROR> T_INTERFAZ_EJECUCION_ERROR { get; set; }
        public virtual DbSet<T_CTR_CALIDAD_PENDIENTE> T_CTR_CALIDAD_PENDIENTE { get; set; }
        public virtual DbSet<T_TIPO_CONTENEDOR> T_TIPO_CONTENEDOR { get; set; }
        public virtual DbSet<T_IMPRESORA> T_IMPRESORA { get; set; }
        public virtual DbSet<T_LENGUAJE_IMPRESION> T_LENGUAJE_IMPRESION { get; set; }
        public virtual DbSet<T_LABEL_ESTILO> T_LABEL_ESTILO { get; set; }
        public virtual DbSet<T_LABEL_TEMPLATE> T_LABEL_TEMPLATE { get; set; }
        public virtual DbSet<T_REL_LABELESTILO_TIPOCONT> T_REL_LABELESTILO_TIPOCONT { get; set; }
        public virtual DbSet<T_DET_IMPRESION> T_DET_IMPRESION { get; set; }
        public virtual DbSet<T_INTERFAZ> T_INTERFAZ { get; set; }
        public virtual DbSet<T_INTERFAZ_EJECUCION_DATA> T_INTERFAZ_EJECUCION_DATA { get; set; }
        public virtual DbSet<T_INTERFAZ_EJECUCION_DATEXT> T_INTERFAZ_EJECUCION_DATEXT { get; set; }
        public virtual DbSet<T_INTERFAZ_EJECUCION_DATEXTDET> T_INTERFAZ_EJECUCION_DATEXTDET { get; set; }
        public virtual DbSet<I_E_ESTAN_FACTURA_REC> I_E_ESTAN_FACTURA_REC { get; set; }
        public virtual DbSet<I_E_ESTAN_FACTURA_REC_DET> I_E_ESTAN_FACTURA_REC_DET { get; set; }
        public virtual DbSet<T_CONTAINER> T_CONTAINER { get; set; }
        public virtual DbSet<T_RECEPC_AGENDA_CONTAINER_REL> T_RECEPC_AGENDA_CONTAINER_REL { get; set; }
        public virtual DbSet<T_RECEPCION_FACTURA> T_RECEPCION_FACTURA { get; set; }
        public virtual DbSet<T_FACTURACION_EJECUCION> T_FACTURACION_EJECUCION { get; set; }
        public virtual DbSet<T_FACTURACION_EJEC_EMPRESA> T_FACTURACION_EJEC_EMPRESA { get; set; }
        public virtual DbSet<T_FACTURACION_PALLET> T_FACTURACION_PALLET { get; set; }
        public virtual DbSet<T_LOG_FACTURACION_ALM_BULTO> T_LOG_FACTURACION_ALM_BULTO { get; set; }
        public virtual DbSet<T_LOG_FACTURACION_ALM_COR_04> T_LOG_FACTURACION_ALM_COR_04 { get; set; }
        public virtual DbSet<T_FACTURACION_RESULTADO> T_FACTURACION_RESULTADO { get; set; }
        public virtual DbSet<T_FACTURACION_PALLET_DET> T_FACTURACION_PALLET_DET { get; set; }
        public virtual DbSet<T_FACTURACION_EMPRESA_PROCESO> T_FACTURACION_EMPRESA_PROCESO { get; set; }
        public virtual DbSet<T_LOG_FACTURACION_COR_10> T_LOG_FACTURACION_COR_10 { get; set; }
        public virtual DbSet<T_LOG_FACTURACION_MOV_EG_BULTO> T_LOG_FACTURACION_MOV_EG_BULTO { get; set; }
        public virtual DbSet<T_FACTURACION_UNIDAD_MEDIDA> T_FACTURACION_UNIDAD_MEDIDA { get; set; }
        public virtual DbSet<T_FACTURACION_UND_MEDIDA_EMP> T_FACTURACION_UND_MEDIDA_EMP { get; set; }
        public virtual DbSet<T_ORT_TAREA> T_ORT_TAREA { get; set; }
        public virtual DbSet<T_ORT_INSUMO_MANIPULEO> T_ORT_INSUMO_MANIPULEO { get; set; }
        public virtual DbSet<T_ORT_INSUMO_MANIPULEO_EMPRESA> T_ORT_INSUMO_MANIPULEO_EMPRESA { get; set; }
        public virtual DbSet<T_ORT_ORDEN_TAREA_FUNCIONARIO> T_ORT_ORDEN_TAREA_FUNCIONARIO { get; set; }
        public virtual DbSet<T_ORT_ORDEN_TAREA_EQUIPO> T_ORT_ORDEN_TAREA_EQUIPO { get; set; }
        public virtual DbSet<T_ORT_ORDEN_SESION_EQUIPO> T_ORT_ORDEN_SESION_EQUIPO { get; set; }
        public virtual DbSet<T_ORT_ORDEN_SESION> T_ORT_ORDEN_SESION { get; set; }
        public virtual DbSet<USERDATA_PASS_HISTORY> USERDATA_PASS_HISTORY { get; set; }
        public virtual DbSet<T_CONTACTO> T_CONTACTO { get; set; }
        public virtual DbSet<T_CONTACTO_GRUPO> T_CONTACTO_GRUPO { get; set; }
        public virtual DbSet<T_EVENTO> T_EVENTO { get; set; }
        public virtual DbSet<T_EVENTO_TEMPLATE> T_EVENTO_TEMPLATE { get; set; }
        public virtual DbSet<T_EVENTO_BANDEJA> T_EVENTO_BANDEJA { get; set; }
        public virtual DbSet<T_EVENTO_BANDEJA_INSTANCIA> T_EVENTO_BANDEJA_INSTANCIA { get; set; }
        public virtual DbSet<T_EVENTO_INSTANCIA> T_EVENTO_INSTANCIA { get; set; }
        public virtual DbSet<T_EVENTO_NOTIFICACION> T_EVENTO_NOTIFICACION { get; set; }
        public virtual DbSet<T_EVENTO_NOTIFICACION_ARCHIVO> T_EVENTO_NOTIFICACION_ARCHIVO { get; set; }
        public virtual DbSet<T_EVENTO_NOTIFICACION_EMAIL> T_EVENTO_NOTIFICACION_EMAIL { get; set; }
        public virtual DbSet<T_EVENTO_PARAMETRO> T_EVENTO_PARAMETRO { get; set; }
        public virtual DbSet<T_EVENTO_PARAMETRO_INSTANCIA> T_EVENTO_PARAMETRO_INSTANCIA { get; set; }
        public virtual DbSet<T_EVENTO_GRUPO_INSTANCIA_REL> T_EVENTO_GRUPO_INSTANCIA_REL { get; set; }
        public virtual DbSet<T_CONTACTO_GRUPO_REL> T_CONTACTO_GRUPO_REL { get; set; }
        public virtual DbSet<T_ARCHIVO_ADJUNTO> T_ARCHIVO_ADJUNTO { get; set; }
        public virtual DbSet<T_ARCHIVO_ADJUNTO_VERSION> T_ARCHIVO_ADJUNTO_VERSION { get; set; }
        public virtual DbSet<T_ARCHIVO_DOCUMENTO> T_ARCHIVO_DOCUMENTO { get; set; }
        public virtual DbSet<T_ARCHIVO_MANEJO> T_ARCHIVO_MANEJO { get; set; }
        public virtual DbSet<T_STOCK_ENVASE> T_STOCK_ENVASE { get; set; }
        public virtual DbSet<T_VEICULO> T_VEICULO { get; set; }
        public virtual DbSet<T_TIPO_VEICULO> T_TIPO_VEICULO { get; set; }
        public virtual DbSet<T_ANULACIONES_PENDIENTES> T_ANULACIONES_PENDIENTES { get; set; }
        public virtual DbSet<T_TIPO_PEDIDO> T_TIPO_PEDIDO { get; set; }
        public virtual DbSet<T_ANULACION_PREPARACION> T_ANULACION_PREPARACION { get; set; }
        public virtual DbSet<T_ANULACION_PREPARACION_ERROR> T_ANULACION_PREPARACION_ERROR { get; set; }
        public virtual DbSet<T_DET_ANULACION_PREPARACION> T_DET_ANULACION_PREPARACION { get; set; }
        public virtual DbSet<T_IMPRESORA_SERVIDOR> T_IMPRESORA_SERVIDOR { get; set; }
        public virtual DbSet<T_CROSS_DOCK_TEMP> T_CROSS_DOCK_TEMP { get; set; }
        public virtual DbSet<T_TIPO_CROSS_DOCK> T_TIPO_CROSS_DOCK { get; set; }
        public virtual DbSet<T_ALM_ESTRATEGIA> T_ALM_ESTRATEGIA { get; set; }
        public virtual DbSet<T_ALM_ASOCIACION> T_ALM_ASOCIACION { get; set; }
        public virtual DbSet<T_ALM_LOGICA_INSTANCIA> T_ALM_LOGICA_INSTANCIA { get; set; }
        public virtual DbSet<T_ALM_LOGICA_INSTANCIA_PARAM> T_ALM_LOGICA_INSTANCIA_PARAM { get; set; }
        public virtual DbSet<T_DOCUMENTO_PREPARACION> T_DOCUMENTO_PREPARACION { get; set; }
        public virtual DbSet<T_ALM_SUGERENCIA> T_ALM_SUGERENCIA { get; set; }
        public virtual DbSet<T_ALM_SUGERENCIA_DET> T_ALM_SUGERENCIA_DET { get; set; }
        public virtual DbSet<T_UNIDAD_TRANSPORTE> T_UNIDAD_TRANSPORTE { get; set; }
        public virtual DbSet<T_GRUPO> T_GRUPO { get; set; }
        public virtual DbSet<T_GRUPO_PARAM> T_GRUPO_PARAM { get; set; }
        public virtual DbSet<T_GRUPO_REGLA> T_GRUPO_REGLA { get; set; }
        public virtual DbSet<T_GRUPO_REGLA_PARAM> T_GRUPO_REGLA_PARAM { get; set; }
        public virtual DbSet<T_AUTOMATISMO> T_AUTOMATISMO { get; set; }
        public virtual DbSet<T_AUTOMATISMO_CARACTERISTICA> T_AUTOMATISMO_CARACTERISTICA { get; set; }
        public virtual DbSet<T_AUTOMATISMO_CARACTERISTICA_CONFIG> T_AUTOMATISMO_CARACTERISTICA_CONFIG { get; set; }
        public virtual DbSet<T_AUTOMATISMO_DATA> T_AUTOMATISMO_DATA { get; set; }
        public virtual DbSet<T_AUTOMATISMO_EJECUCION> T_AUTOMATISMO_EJECUCION { get; set; }
        public virtual DbSet<T_AUTOMATISMO_INTERFAZ> T_AUTOMATISMO_INTERFAZ { get; set; }
        public virtual DbSet<T_AUTOMATISMO_POSICION> T_AUTOMATISMO_POSICION { get; set; }
        public virtual DbSet<T_AUTOMATISMO_PUESTO> T_AUTOMATISMO_PUESTO { get; set; }
        public virtual DbSet<T_INTEGRACION_SERVICIO> T_INTEGRACION_SERVICIO { get; set; }
        public virtual DbSet<T_TIPO_AGRUPACION_ENDERECO> T_TIPO_AGRUPACION_ENDERECO { get; set; }
        public virtual DbSet<T_AUTOMATISMO_CONF_ENTRADA> T_AUTOMATISMO_CONF_ENTRADA { get; set; }
        public virtual DbSet<T_LPN_AUDITORIA> T_LPN_AUDITORIA { get; set; }
        public virtual DbSet<T_LPN_AUDITORIA_ATRIBUTO> T_LPN_AUDITORIA_ATRIBUTO { get; set; }
        public virtual DbSet<T_DET_PEDIDO_SAIDA_LPN> T_DET_PEDIDO_SAIDA_LPN { get; set; }
        public virtual DbSet<T_LPN_ATRIBUTO> T_LPN_ATRIBUTO { get; set; }
        public virtual DbSet<T_LPN_DET_ATRIBUTO> T_LPN_DET_ATRIBUTO { get; set; }
        public virtual DbSet<T_LPN> T_LPN { get; set; }
        public virtual DbSet<T_LPN_TIPO_ATRIBUTO> T_LPN_TIPO_ATRIBUTO { get; set; }
        public virtual DbSet<T_LPN_CONSOLIDACION_TIPO> T_LPN_CONSOLIDACION_TIPO { get; set; }
        public virtual DbSet<T_LPN_TIPO_ATRIBUTO_DET> T_LPN_TIPO_ATRIBUTO_DET { get; set; }
        public virtual DbSet<T_ATRIBUTO_VALIDACION_ASOCIADA> T_ATRIBUTO_VALIDACION_ASOCIADA { get; set; }
        public virtual DbSet<T_ATRIBUTO_TIPO> T_ATRIBUTO_TIPO { get; set; }
        public virtual DbSet<T_ATRIBUTO_SISTEMA> T_ATRIBUTO_SISTEMA { get; set; }
        public virtual DbSet<T_LPN_DET> T_LPN_DET { get; set; }
        public virtual DbSet<T_ATRIBUTO_VALIDACION> T_ATRIBUTO_VALIDACION { get; set; }
        public virtual DbSet<T_LPN_TIPO_BARRAS> T_LPN_TIPO_BARRAS { get; set; }
        public virtual DbSet<T_ALM_REABASTECIMIENTO> T_ALM_REABASTECIMIENTO { get; set; }
        public virtual DbSet<T_DET_PICKING_LPN> T_DET_PICKING_LPN { get; set; }
        public virtual DbSet<T_DET_PEDIDO_SAIDA_ATRIB> T_DET_PEDIDO_SAIDA_ATRIB { get; set; }
        public virtual DbSet<T_DET_PEDIDO_SAIDA_ATRIB_DET> T_DET_PEDIDO_SAIDA_ATRIB_DET { get; set; }
        public virtual DbSet<T_DET_PEDIDO_SAIDA_LPN_ATRIB> T_DET_PEDIDO_SAIDA_LPN_ATRIB { get; set; }
        public virtual DbSet<T_TEMP_DET_PEDIDO_SAIDA_ATRIB> T_TEMP_DET_PEDIDO_SAIDA_ATRIB { get; set; }
        public virtual DbSet<T_TEMP_DET_PEDIDO_SAIDA_LPN_ATRIB> T_TEMP_DET_PEDIDO_SAIDA_LPN_ATRIB { get; set; }
        public virtual DbSet<T_AGENDA_LPN_PLANIFICACION> T_AGENDA_LPN_PLANIFICACION { get; set; }
        public virtual DbSet<T_NOTIFICACIONES> T_NOTIFICACIONES { get; set; }
        public virtual DbSet<T_PRDC_DET_INGRESO_TEORICO> T_PRDC_DET_INGRESO_TEORICO { get; set; }
        public virtual DbSet<T_PRDC_INGRESO_CONTROLES> T_PRDC_INGRESO_CONTROLES { get; set; }
        public virtual DbSet<T_PRDC_INGRESO_INSTRUCCION> T_PRDC_INGRESO_INSTRUCCION { get; set; }
        public virtual DbSet<T_PRDC_INGRESO_DET_PEDIDO_TEMP> T_PRDC_INGRESO_DET_PEDIDO_TEMP { get; set; }
        public virtual DbSet<T_PRDC_DET_INGRESO_REAL> T_PRDC_DET_INGRESO_REAL { get; set; }
        public virtual DbSet<T_PRDC_DET_SALIDA_REAL> T_PRDC_DET_SALIDA_REAL { get; set; }
        public virtual DbSet<T_PRDC_DET_INGRESO_REAL_MOV> T_PRDC_DET_INGRESO_REAL_MOV { get; set; }
        public virtual DbSet<T_PRDC_DET_SALIDA_REAL_MOV> T_PRDC_DET_SALIDA_REAL_MOV { get; set; }
        public virtual DbSet<T_TIPO_MOVIMIENTO> T_TIPO_MOVIMIENTO { get; set; }
        public virtual DbSet<T_EVENTO_INSTANCIA_EJECUCION> T_EVENTO_INSTANCIA_EJECUCION { get; set; }
        public virtual DbSet<T_CODIGO_MULTIDATO_EMPRESA_DET> T_CODIGO_MULTIDATO_EMPRESA_DET { get; set; }
        public virtual DbSet<T_CODIGO_MULTIDATO_APLICACION> T_CODIGO_MULTIDATO_APLICACION { get; set; }
        public virtual DbSet<T_CODIGO_MULTIDATO_EMPRESA> T_CODIGO_MULTIDATO_EMPRESA { get; set; }
        public virtual DbSet<T_CODIGO_MULTIDATO> T_CODIGO_MULTIDATO { get; set; }
        public virtual DbSet<T_CODIGO_MULTIDATO_DET> T_CODIGO_MULTIDATO_DET { get; set; }
        public virtual DbSet<T_INVENTARIO_ENDERECO_DET_ATR> T_INVENTARIO_ENDERECO_DET_ATR { get; set; }
        public virtual DbSet<T_APLICACION> T_APLICACION { get; set; }
        public virtual DbSet<T_APLICACION_RECORRIDO> T_APLICACION_RECORRIDO { get; set; }
        public virtual DbSet<T_APLICACION_RECORRIDO_USUARIO> T_APLICACION_RECORRIDO_USUARIO { get; set; }
        public virtual DbSet<T_ALM_DET_PRODUTO_TEMP> T_ALM_DET_PRODUTO_TEMP { get; set; }
        public virtual DbSet<T_RECEPCION_FACTURA_DET> T_RECEPCION_FACTURA_DET { get; set; }

        public virtual DbSet<T_COLA_TRABAJO> T_COLA_TRABAJO { get; set; }
        public virtual DbSet<T_COLA_TRABAJO_PONDERADOR> T_COLA_TRABAJO_PONDERADOR { get; set; }
        public virtual DbSet<T_COLA_TRABAJO_PONDERADOR_DET> T_COLA_TRABAJO_PONDERADOR_DET { get; set; }
        public virtual DbSet<T_COLA_TRABAJO_PONDERADOR_INST> T_COLA_TRABAJO_PONDERADOR_INST { get; set; }
        public virtual DbSet<T_COLA_PICKING> T_COLA_PICKING { get; set; }
        public virtual DbSet<T_PREFERENCIA> T_PREFERENCIA { get; set; }
        public virtual DbSet<T_PREFERENCIA_CLASSE> T_PREFERENCIA_CLASSE { get; set; }
        public virtual DbSet<T_PREFERENCIA_COND_LIBERACION> T_PREFERENCIA_COND_LIBERACION { get; set; }
        public virtual DbSet<T_PREFERENCIA_EMPRESA> T_PREFERENCIA_EMPRESA { get; set; }
        public virtual DbSet<T_PREFERENCIA_FAMILIA_PRODUTO> T_PREFERENCIA_FAMILIA_PRODUTO { get; set; }
        public virtual DbSet<T_PREFERENCIA_ROTA> T_PREFERENCIA_ROTA { get; set; }
        public virtual DbSet<T_PREFERENCIA_TIPO_EXPEDICION> T_PREFERENCIA_TIPO_EXPEDICION { get; set; }
        public virtual DbSet<T_PREFERENCIA_TIPO_PEDIDO> T_PREFERENCIA_TIPO_PEDIDO { get; set; }
        public virtual DbSet<T_PREFERENCIA_ZONA> T_PREFERENCIA_ZONA { get; set; }
        public virtual DbSet<T_PREFERENCIA_CLIENTE> T_PREFERENCIA_CLIENTE { get; set; }
        public virtual DbSet<T_PREFERENCIA_CONTROL_ACCESO> T_PREFERENCIA_CONTROL_ACCESO { get; set; }
        public virtual DbSet<T_PREFERENCIA_REL> T_PREFERENCIA_REL { get; set; }

        public virtual DbSet<T_TRASPASO_CONFIGURACION> T_TRASPASO_CONFIGURACION { get; set; }
        public virtual DbSet<T_TRASPASO_CONF_DESTINO> T_TRASPASO_CONF_DESTINO { get; set; }
        public virtual DbSet<T_TRASPASO_CONF_TIPO_TRASPASO> T_TRASPASO_CONF_TIPO_TRASPASO { get; set; }
        public virtual DbSet<T_TRASPASO_MAPEO_PRODUTO> T_TRASPASO_MAPEO_PRODUTO { get; set; }
        public virtual DbSet<T_TRASPASO> T_TRASPASO { get; set; }
        public virtual DbSet<T_TRASPASO_DET_PEDIDO> T_TRASPASO_DET_PEDIDO { get; set; }
        public virtual DbSet<T_CLIENTE_DIASVALIDEZ_VENTANA> T_CLIENTE_DIASVALIDEZ_VENTANA { get; set; }
        
        #endregion << - TABLAS -

        #region >> - VISTAS -
        public virtual DbSet<V_REC500_SEARCH_PRODUCTO> V_REC500_SEARCH_PRODUCTO { get; set; }
        public virtual DbSet<V_REC500_FACTURAS_DET> V_REC500_FACTURAS_DET { get; set; }
        public virtual DbSet<V_REC500_FACTURAS> V_REC500_FACTURAS { get; set; }
        public virtual DbSet<V_AGENTE> V_AGENTE { get; set; }
        public virtual DbSet<V_AGENTES_WREG220> V_AGENTES_WREG220 { get; set; }
        public virtual DbSet<V_CLASSE_WREG035> V_CLASSE_WREG035 { get; set; }
        public virtual DbSet<V_CODIGO_BARRAS_WREG603> V_CODIGO_BARRAS_WREG603 { get; set; }
        public virtual DbSet<V_EMPRESAS_WREG100> V_EMPRESAS_WREG100 { get; set; }
        public virtual DbSet<V_EMPRESA_DOCUMENTAL> V_EMPRESA_DOCUMENTAL { get; set; }
        public virtual DbSet<V_EGRESO_CAMION_WEXP> V_EGRESO_CAMION_WEXP { get; set; }
        public virtual DbSet<V_EGRESO_GEN_DOCOCUMENTO_WEXP> V_EGRESO_GEN_DOCOCUMENTO_WEXP { get; set; }
        public virtual DbSet<V_EGRESO_CONTROL_DOC_WEXP> V_EGRESO_CONTROL_DOC_WEXP { get; set; }
        public virtual DbSet<V_ENDERECO_ESTOQUE_WREG040> V_ENDERECO_ESTOQUE_WREG040 { get; set; }
        public virtual DbSet<V_FAMILIA_PRODUCTO_WREG020> V_FAMILIA_PRODUCTO_WREG020 { get; set; }
        public virtual DbSet<V_INT050_INTERFAZ_EJECUCION> V_INT050_INTERFAZ_EJECUCION { get; set; }
        public virtual DbSet<V_INT070_INT_EJECUCION_ERROR> V_INT070_INT_EJECUCION_ERROR { get; set; }
        public virtual DbSet<V_INTERFAZ_EJEC_DATA> V_INTERFAZ_EJEC_DATA { get; set; }
        public virtual DbSet<V_LOG_ETIQUETAS_WREC180> V_LOG_ETIQUETAS_WREC180 { get; set; }
        public virtual DbSet<V_REG140_ONDAS> V_REG140_ONDAS { get; set; }
        public virtual DbSet<V_REG911_DETALLE_DOMINIO> V_REG911_DETALLE_DOMINIO { get; set; }
        public virtual DbSet<V_REG910_DOMINIO> V_REG910_DOMINIO { get; set; }
        public virtual DbSet<V_PAIS_SUBDIVISION_LOCALIDAD> V_PAIS_SUBDIVISION_LOCALIDAD { get; set; }
        public virtual DbSet<V_DET_PREPARACION_WPRE061> V_DET_PREPARACION_WPRE061 { get; set; }
        public virtual DbSet<V_PEDIDOS_PENDIENTES> V_PEDIDOS_PENDIENTES { get; set; }
        public virtual DbSet<V_PICKING_PENDIENTE_WPRE160> V_PICKING_PENDIENTE_WPRE160 { get; set; }
        public virtual DbSet<V_ARCHIVOS> V_ARCHIVOS { get; set; }
        public virtual DbSet<V_PICKING_PENDIENTE_WPRE161> V_PICKING_PENDIENTE_WPRE161 { get; set; }
        public virtual DbSet<V_PICKING_PENDIENTE_WPRE162> V_PICKING_PENDIENTE_WPRE162 { get; set; }
        public virtual DbSet<V_PRE350_STOCK_PICKING_REABAST> V_PRE350_STOCK_PICKING_REABAST { get; set; }
        public virtual DbSet<V_PRE351_STOCK_PICKING_REABAST> V_PRE351_STOCK_PICKING_REABAST { get; set; }
        public virtual DbSet<V_RECEPCION_REFERENCIA_WREC010> V_RECEPCION_REFERENCIA_WREC010 { get; set; }
        public virtual DbSet<V_REC141_AGENDA_PROBLEMA> V_REC141_AGENDA_PROBLEMA { get; set; }
        public virtual DbSet<V_REC173_REL_RECEPCION_TP_EMP> V_REC173_REL_RECEPCION_TP_EMP { get; set; }
        public virtual DbSet<V_REC_REFERENCIA_DET_WREC011> V_REC_REFERENCIA_DET_WREC011 { get; set; }
        public virtual DbSet<V_REG602_PRODUCTO_CONTROL_CALIDAD> V_REG602_PRODUCTO_CONTROL_CALIDAD { get; set; }
        public virtual DbSet<V_REG220_CLIENTE_RUTA_PREDIO> V_REG220_CLIENTE_RUTA_PREDIO { get; set; }
        public virtual DbSet<V_ROTA_WREG130> V_ROTA_WREG130 { get; set; }
        public virtual DbSet<V_REG220_RUTA_ONDA> V_REG220_RUTA_ONDA { get; set; }
        public virtual DbSet<V_REG015_PRODUTOS_PROVEEDOR> V_REG015_PRODUTOS_PROVEEDOR { get; set; }
        public virtual DbSet<V_STOCK_POR_PRODUCTO> V_STOCK_POR_PRODUCTO { get; set; }
        public virtual DbSet<V_STO710_LT_LPN_ATRIBUTO_CABEZAL> V_STO710_LT_LPN_ATRIBUTO_CABEZAL { get; set; }
        public virtual DbSet<V_STO710_LT_LPN_ATRIBUTO_DETALLE> V_STO710_LT_LPN_ATRIBUTO_DETALLE { get; set; }
        public virtual DbSet<V_LOG_CONTROL_PICKEO_WPRE221> V_LOG_CONTROL_PICKEO_WPRE221 { get; set; }
        public virtual DbSet<V_SUB_CLASSE_WREG036> V_SUB_CLASSE_WREG036 { get; set; }
        public virtual DbSet<V_REG601_CONTROLES_CALIDAD> V_REG601_CONTROLES_CALIDAD { get; set; }
        public virtual DbSet<V_PRE101_DET_PEDIDO_SAIDA> V_PRE101_DET_PEDIDO_SAIDA { get; set; }
        public virtual DbSet<V_REC170_VALIDAR_FACTURA> V_REC170_VALIDAR_FACTURA { get; set; }
        public virtual DbSet<V_REC170_REFERENCIAS_ASIGNADAS> V_REC170_REFERENCIAS_ASIGNADAS { get; set; }
        public virtual DbSet<V_REC170_REFERENCIAS_DISPONIB> V_REC170_REFERENCIAS_DISPONIB { get; set; }
        public virtual DbSet<V_REG020_FAMILIA_PRODUCTO> V_REG020_FAMILIA_PRODUCTO { get; set; }
        public virtual DbSet<V_REG035_CLASSE> V_REG035_CLASSE { get; set; }
        public virtual DbSet<V_REG036_SUB_CLASSE> V_REG036_SUB_CLASSE { get; set; }
        public virtual DbSet<V_REG040_ENDERECO_ESTOQUE> V_REG040_ENDERECO_ESTOQUE { get; set; }
        public virtual DbSet<V_REG100_EMPRESAS> V_REG100_EMPRESAS { get; set; }
        public virtual DbSet<V_REG104_ZONA> V_REG104_ZONA { get; set; }
        public virtual DbSet<V_REG130_ROTA> V_REG130_ROTA { get; set; }
        public virtual DbSet<V_REG220_AGENTES> V_REG220_AGENTES { get; set; }
        public virtual DbSet<V_SALDO_ORDEN_COMPRA_FAC> V_SALDO_ORDEN_COMPRA_FAC { get; set; }
        public virtual DbSet<V_STO005_ESTOQUE> V_STO005_ESTOQUE { get; set; }
        public virtual DbSet<V_STO110_CTR_CALIDAD> V_STO110_CTR_CALIDAD { get; set; }
        public virtual DbSet<V_STO498_PALLET_TRANSFERENCIA> V_STO498_PALLET_TRANSFERENCIA { get; set; }
        public virtual DbSet<V_STO030_TRACE_STOCK> V_STO030_TRACE_STOCK { get; set; }
        public virtual DbSet<V_STO060_CTR_CALIDAD_ETIQ> V_STO060_CTR_CALIDAD_ETIQ { get; set; }
        public virtual DbSet<V_STO060_CTR_CALIDAD_UBIC> V_STO060_CTR_CALIDAD_UBIC { get; set; }
        public virtual DbSet<V_SEG030_GRUPO_CONSULTA> V_SEG030_GRUPO_CONSULTA { get; set; }
        public virtual DbSet<V_REG010_EQUIPO> V_REG010_EQUIPO { get; set; }
        public virtual DbSet<V_REC190_ETIQUETA_LOTE> V_REC190_ETIQUETA_LOTE { get; set; }
        public virtual DbSet<V_IMP110_LABEL_ESTILO> V_IMP110_LABEL_ESTILO { get; set; }
        public virtual DbSet<V_LIMP010_IMPRESION> V_LIMP010_IMPRESION { get; set; }
        public virtual DbSet<V_LIMP010DET_IMPRESION> V_LIMP010DET_IMPRESION { get; set; }
        public virtual DbSet<V_COF070_REPORTE> V_COF070_REPORTE { get; set; }
        public virtual DbSet<V_STO395_MOVTO_STOCK> V_STO395_MOVTO_STOCK { get; set; }
        public virtual DbSet<V_ETIQUETA_LOTE_DET_WREC170> V_ETIQUETA_LOTE_DET_WREC170 { get; set; }
        public virtual DbSet<V_REC170_RECIBIDA_FICTICIA> V_REC170_RECIBIDA_FICTICIA { get; set; }
        public virtual DbSet<V_FACTURAC_CODIGO_COMP_WFAC250> V_FACTURAC_CODIGO_COMP_WFAC250 { get; set; }
        public virtual DbSet<V_FACTURACION_PROC_FAC_WFAC251> V_FACTURACION_PROC_FAC_WFAC251 { get; set; }
        public virtual DbSet<V_INT100_ESTAN_PEDIDOS_SAIDA> V_INT100_ESTAN_PEDIDOS_SAIDA { get; set; }
        public virtual DbSet<V_INT100_ESTAN_PEDID_SAIDA_DET> V_INT100_ESTAN_PEDID_SAIDA_DET { get; set; }
        public virtual DbSet<V_INT101_ESTAN_PRODUCTOS> V_INT101_ESTAN_PRODUCTOS { get; set; }
        public virtual DbSet<V_INT102_ESTAN_CODIGO_BARRAS> V_INT102_ESTAN_CODIGO_BARRAS { get; set; }
        public virtual DbSet<V_INT103_ESTAN_REF_RECEPCION> V_INT103_ESTAN_REF_RECEPCION { get; set; }
        public virtual DbSet<V_INT103_ESTAN_REF_RECEPC_DET> V_INT103_ESTAN_REF_RECEPC_DET { get; set; }
        public virtual DbSet<V_INT104_ESTAN_PEDIDO_ANULADO> V_INT104_ESTAN_PEDIDO_ANULADO { get; set; }
        public virtual DbSet<V_INT105_ESTAN_AJUSTE_STOCK> V_INT105_ESTAN_AJUSTE_STOCK { get; set; }
        public virtual DbSet<V_INT106_ESTAN_AGENTE> V_INT106_ESTAN_AGENTE { get; set; }
        public virtual DbSet<V_INT107_ESTAN_CONF_PED_PEDIDO> V_INT107_ESTAN_CONF_PED_PEDIDO { get; set; }
        public virtual DbSet<V_INT107_ESTAN_CONF_PEDI_DET> V_INT107_ESTAN_CONF_PEDI_DET { get; set; }
        public virtual DbSet<V_INT108_ESTAN_CONVERTEDOR> V_INT108_ESTAN_CONVERTEDOR { get; set; }
        public virtual DbSet<V_INT109_ESTAN_FACTURA_REC> V_INT109_ESTAN_FACTURA_REC { get; set; }
        public virtual DbSet<V_INT109_ESTAN_FACTURA_REC_DET> V_INT109_ESTAN_FACTURA_REC_DET { get; set; }
        public virtual DbSet<V_DOCUMENTO_SALDO_DETALLE> V_DOCUMENTO_SALDO_DETALLE { get; set; }
        public virtual DbSet<V_PRDC_SALDO_INGRESADO> V_PRDC_SALDO_INGRESADO { get; set; }
        public virtual DbSet<V_DET_DOCUMENTO_RESERVA> V_DET_DOCUMENTO_RESERVA { get; set; }
        public virtual DbSet<V_PRDC_SALDO_CC> V_PRDC_SALDO_CC { get; set; }
        public virtual DbSet<V_CAMBIO_DOC_DOC401> V_CAMBIO_DOC_DOC401 { get; set; }
        public virtual DbSet<V_KIT200_DET_PEDIDO_SAIDA> V_KIT200_DET_PEDIDO_SAIDA { get; set; }
        public virtual DbSet<V_DOCUMENTO_AJUSTE_STOCK> V_DOCUMENTO_AJUSTE_STOCK { get; set; }
        public virtual DbSet<V_DOCUMENTO_MOV_MERC_DOC350> V_DOCUMENTO_MOV_MERC_DOC350 { get; set; }
        public virtual DbSet<V_CAMBIO_DOC_DOC500> V_CAMBIO_DOC_DOC500 { get; set; }
        public virtual DbSet<V_CONSULTA_IP_CUENTA_DOC501> V_CONSULTA_IP_CUENTA_DOC501 { get; set; }
        public virtual DbSet<V_DOCUMENTO_AGRUPABLE_DOC330> V_DOCUMENTO_AGRUPABLE_DOC330 { get; set; }
        public virtual DbSet<V_DOCUMENTO_AGRUPADOR_DOC320> V_DOCUMENTO_AGRUPADOR_DOC320 { get; set; }
        public virtual DbSet<V_CAMBIO_DOC_DOC400> V_CAMBIO_DOC_DOC400 { get; set; }
        public virtual DbSet<V_DOC361_AJUSTES_STOCK> V_DOC361_AJUSTES_STOCK { get; set; }
        public virtual DbSet<V_DOC363_AJUSTE_ACTA> V_DOC363_AJUSTE_ACTA { get; set; }
        public virtual DbSet<V_DOC362_DOCUMENTO_INGRESO> V_DOC362_DOCUMENTO_INGRESO { get; set; }
        public virtual DbSet<V_ROLLBACK_CAMBIO_DOC> V_ROLLBACK_CAMBIO_DOC { get; set; }
        public virtual DbSet<V_DOC363_DOCUMENTO_INGRESO> V_DOC363_DOCUMENTO_INGRESO { get; set; }
        public virtual DbSet<V_STO721_HISTORIAL_CABEZAL> V_STO721_HISTORIAL_CABEZAL { get; set; }
        public virtual DbSet<V_DOCUMENTOS_ACTA_DOC310> V_DOCUMENTOS_ACTA_DOC310 { get; set; }
        public virtual DbSet<V_DET_DOCUMENTO> V_DET_DOCUMENTO { get; set; }
        public virtual DbSet<V_DOC_SALDO_TEMP_AUX> V_DOC_SALDO_TEMP_AUX { get; set; }
        public virtual DbSet<V_DOC363_SALDO_LINEA_INGRESO> V_DOC363_SALDO_LINEA_INGRESO { get; set; }
        public virtual DbSet<V_DET_INT_SALIDA_BB_KIT260> V_DET_INT_SALIDA_BB_KIT260 { get; set; }
        public virtual DbSet<V_INTERFACES_SALIDA_BB_KIT250> V_INTERFACES_SALIDA_BB_KIT250 { get; set; }
        public virtual DbSet<V_KIT240_MOVIMIENTOS_STOCK_BB> V_KIT240_MOVIMIENTOS_STOCK_BB { get; set; }
        public virtual DbSet<V_STOCK_CONSUMIR_BB_KIT210> V_STOCK_CONSUMIR_BB_KIT210 { get; set; }
        public virtual DbSet<V_PRDC_KIT200_STOCK_ENTRADA_BB> V_PRDC_KIT200_STOCK_ENTRADA_BB { get; set; }
        public virtual DbSet<V_STOCK_PRODUCIR_BB_KIT220> V_STOCK_PRODUCIR_BB_KIT220 { get; set; }
        public virtual DbSet<V_PRDC_KIT230_STOCK_SALIDA_BB> V_PRDC_KIT230_STOCK_SALIDA_BB { get; set; }
        public virtual DbSet<V_ACTA_DET_DOCUMENTO> V_ACTA_DET_DOCUMENTO { get; set; }
        public virtual DbSet<V_PEDIDO_SAIDA_KIT260> V_PEDIDO_SAIDA_KIT260 { get; set; }
        public virtual DbSet<V_PRDC_PROD_SOBRANTE_PREP> V_PRDC_PROD_SOBRANTE_PREP { get; set; }
        public virtual DbSet<V_PRDC_PROD_SALIDA_PREP> V_PRDC_PROD_SALIDA_PREP { get; set; }
        public virtual DbSet<V_PRDC_PROD_ENTRADA_PREP> V_PRDC_PROD_ENTRADA_PREP { get; set; }
        public virtual DbSet<V_ACCION_INSTANCIA_KIT120> V_ACCION_INSTANCIA_KIT120 { get; set; }
        public virtual DbSet<V_PRDC_CONFIG_PASADA_KIT102> V_PRDC_CONFIG_PASADA_KIT102 { get; set; }
        public virtual DbSet<V_PRDC_CONSUMIDO_BB_KIT151> V_PRDC_CONSUMIDO_BB_KIT151 { get; set; }
        public virtual DbSet<V_PRDC_DEFINICION_KIT100> V_PRDC_DEFINICION_KIT100 { get; set; }
        public virtual DbSet<V_PRDC_DET_ENTRADA_KIT101> V_PRDC_DET_ENTRADA_KIT101 { get; set; }
        public virtual DbSet<V_PRDC_DET_ENTRADA_KIT151> V_PRDC_DET_ENTRADA_KIT151 { get; set; }
        public virtual DbSet<V_PRDC_DET_SALIDA_KIT101> V_PRDC_DET_SALIDA_KIT101 { get; set; }
        public virtual DbSet<V_PRDC_DET_SALIDA_KIT151> V_PRDC_DET_SALIDA_KIT151 { get; set; }
        public virtual DbSet<V_PRDC_EGR_IDENT_KIT191> V_PRDC_EGR_IDENT_KIT191 { get; set; }
        public virtual DbSet<V_PRDC_INGRESO_KIT110> V_PRDC_INGRESO_KIT110 { get; set; }
        public virtual DbSet<V_PRDC_INGRESO_KIT150> V_PRDC_INGRESO_KIT150 { get; set; }
        public virtual DbSet<V_PRDC_INGRESO_KIT170> V_PRDC_INGRESO_KIT170 { get; set; }
        public virtual DbSet<V_PRDC_INGRESO_PASADA_KIT180> V_PRDC_INGRESO_PASADA_KIT180 { get; set; }
        public virtual DbSet<V_PRDC_KIT170_LI_CD_PRDC_LINEA> V_PRDC_KIT170_LI_CD_PRDC_LINEA { get; set; }
        public virtual DbSet<V_PRDC_PRODUCIDO_BB_KIT151> V_PRDC_PRODUCIDO_BB_KIT151 { get; set; }
        public virtual DbSet<V_PRDC_LINEA_KIT190> V_PRDC_LINEA_KIT190 { get; set; }
        public virtual DbSet<V_PEDIDO_SAIDA_KIT130> V_PEDIDO_SAIDA_KIT130 { get; set; }
        public virtual DbSet<V_PAR050_TIPO_ESTRUTURA> V_PAR050_TIPO_ESTRUTURA { get; set; }
        public virtual DbSet<V_PAR400_ATRIBUTOS> V_PAR400_ATRIBUTOS { get; set; }
        public virtual DbSet<V_PAR400_ATRIBUTOS_LPN_TIPO> V_PAR400_ATRIBUTOS_LPN_TIPO { get; set; }
        public virtual DbSet<V_PAR400_ATRIBUTOS_VALIDACION_ASOCIADA> V_PAR400_ATRIBUTOS_VALIDACION_ASOCIADA { get; set; }
        public virtual DbSet<V_PAR400_TIPO_ATRIBUTO> V_PAR400_TIPO_ATRIBUTO { get; set; }
        public virtual DbSet<V_PAR400_ATRIBUTOS_SISTEMA> V_PAR400_ATRIBUTOS_SISTEMA { get; set; }
        public virtual DbSet<V_PAR400_MASCARA_FECHA> V_PAR400_MASCARA_FECHA { get; set; }
        public virtual DbSet<V_PAR400_MASCARA_HORA> V_PAR400_MASCARA_HORA { get; set; }
        public virtual DbSet<V_REC400_ESTACIONES_DE_CLASIFICACION> V_REC400_ESTACIONES_DE_CLASIFICACION { get; set; }
        public virtual DbSet<V_PAR401_ASOCIAR_ATRIBUTO_TIPO> V_PAR401_ASOCIAR_ATRIBUTO_TIPO { get; set; }
        public virtual DbSet<V_PAR401_ASOCIAR_ATRIBUTO_TIPO_DET> V_PAR401_ASOCIAR_ATRIBUTO_TIPO_DET { get; set; }
        public virtual DbSet<V_REC410_SUGERENCIAS> V_REC410_SUGERENCIAS { get; set; }
        public virtual DbSet<V_CARGA_WREC220> V_CARGA_WREC220 { get; set; }
        public virtual DbSet<V_PEDIDO_ASIG_CAMION> V_PEDIDO_ASIG_CAMION { get; set; }
        public virtual DbSet<V_ANULACIONES_PENDIENTES> V_ANULACIONES_PENDIENTES { get; set; }
        public virtual DbSet<V_EVENTO_INTERFAZ_EJECUCION> V_EVENTO_INTERFAZ_EJECUCION { get; set; }
        public virtual DbSet<V_INVENTARIO_ENDE_DET> V_INVENTARIO_ENDE_DET { get; set; }
        public virtual DbSet<V_DET_CONT_CON_PROBLEMA> V_DET_CONT_CON_PROBLEMA { get; set; }
        public virtual DbSet<V_CANT_PREPARADA_WEXP> V_CANT_PREPARADA_WEXP { get; set; }
        public virtual DbSet<V_ENDERECO_ESTOQUE_WSTO150> V_ENDERECO_ESTOQUE_WSTO150 { get; set; }
        public virtual DbSet<V_STOCK_TRASPASO_WSTO040> V_STOCK_TRASPASO_WSTO040 { get; set; }
        public virtual DbSet<V_ENDERECO_ESTOQUE> V_ENDERECO_ESTOQUE { get; set; }
        public virtual DbSet<V_STOCK> V_STOCK { get; set; }
        public virtual DbSet<V_STOCK_PRODUTO> V_STOCK_PRODUTO { get; set; }
        public virtual DbSet<V_STOCK_TOTAL> V_STOCK_TOTAL { get; set; }
        public virtual DbSet<V_STOCK_TRASPASO_DEST> V_STOCK_TRASPASO_DEST { get; set; }
        public virtual DbSet<V_STOCK_TRASPASO_ORIGEN> V_STOCK_TRASPASO_ORIGEN { get; set; }
        public virtual DbSet<V_ORT_FUNC_COMP_COR18> V_ORT_FUNC_COMP_COR18 { get; set; }
        public virtual DbSet<V_CAMION_EXP050> V_CAMION_EXP050 { get; set; }
        public virtual DbSet<V_DET_DOCUMENTO_DOC081> V_DET_DOCUMENTO_DOC081 { get; set; }
        public virtual DbSet<V_DET_DOC_DUA_DOC020> V_DET_DOC_DUA_DOC020 { get; set; }
        public virtual DbSet<V_DOCUMENTO_DOC095> V_DOCUMENTO_DOC095 { get; set; }
        public virtual DbSet<V_DET_DOCUMENTO_EGRESO> V_DET_DOCUMENTO_EGRESO { get; set; }
        public virtual DbSet<V_DET_DOCUMENTO_INGRESO> V_DET_DOCUMENTO_INGRESO { get; set; }
        public virtual DbSet<V_LOG_DOCUMENTO> V_LOG_DOCUMENTO { get; set; }
        public virtual DbSet<V_DET_DOC_DUA_DOC021> V_DET_DOC_DUA_DOC021 { get; set; }
        public virtual DbSet<V_DOCUMENTO_LINEA_DET_DOC082> V_DOCUMENTO_LINEA_DET_DOC082 { get; set; }
        public virtual DbSet<V_DOCUMENTO_PROD_DOC290> V_DOCUMENTO_PROD_DOC290 { get; set; }
        public virtual DbSet<V_LT_DET_DOCUMENTO> V_LT_DET_DOCUMENTO { get; set; }
        public virtual DbSet<V_LT_DET_DOCUMENTO_EGRESO> V_LT_DET_DOCUMENTO_EGRESO { get; set; }
        public virtual DbSet<V_LT_DOCUMENTO> V_LT_DOCUMENTO { get; set; }
        public virtual DbSet<V_DOCUMENTO_RESERVA_DOC300> V_DOCUMENTO_RESERVA_DOC300 { get; set; }

        public virtual DbSet<V_INVENTARIO_STOCK> V_INVENTARIO_STOCK { get; set; }
        public virtual DbSet<V_AJUSTE_STOCK_WINV030> V_AJUSTE_STOCK_WINV030 { get; set; }
        public virtual DbSet<V_ORT_ORDEN_TAREA_FUNC_WORT060> V_ORT_ORDEN_TAREA_FUNC_WORT060 { get; set; }
        public virtual DbSet<V_ORT_ORDEN_TAREA_DATO_WORT070> V_ORT_ORDEN_TAREA_DATO_WORT070 { get; set; }
        public virtual DbSet<V_ORT080_ORDEN_TAREA_EQUIPO> V_ORT080_ORDEN_TAREA_EQUIPO { get; set; }
        public virtual DbSet<V_ASIGN_MOTIVO_AJUSTE_WINV060> V_ASIGN_MOTIVO_AJUSTE_WINV060 { get; set; }

        public virtual DbSet<V_RESOURCES_WSEG020> V_RESOURCES_WSEG020 { get; set; }
        public virtual DbSet<V_CONTENEDOR_CON_PROBLEMA> V_CONTENEDOR_CON_PROBLEMA { get; set; }
        public virtual DbSet<V_PREDIOS> V_PREDIOS { get; set; }
        public virtual DbSet<V_CONT_PRODUTO_WSTO150DET> V_CONT_PRODUTO_WSTO150DET { get; set; }
        public virtual DbSet<V_DET_PICKING_WSTO150DET> V_DET_PICKING_WSTO150DET { get; set; }
        public virtual DbSet<V_ETIQUETAS_LOTE_WSTO150DET> V_ETIQUETAS_LOTE_WSTO150DET { get; set; }
        public virtual DbSet<V_PALLET_TRANSF_WSTO150DET> V_PALLET_TRANSF_WSTO150DET { get; set; }
        public virtual DbSet<V_ETIQUETAS_WREC150> V_ETIQUETAS_WREC150 { get; set; }
        public virtual DbSet<V_PEDIDOS_CAMION_EXP050> V_PEDIDOS_CAMION_EXP050 { get; set; }
        public virtual DbSet<V_PUERTA_EMBARQUE_WEXP040> V_PUERTA_EMBARQUE_WEXP040 { get; set; }
        public virtual DbSet<V_DET_PICKING_CAMION_WEXP040> V_DET_PICKING_CAMION_WEXP040 { get; set; }
        public virtual DbSet<V_QT_CAMIONES_CONTENEDOR> V_QT_CAMIONES_CONTENEDOR { get; set; }
        public virtual DbSet<V_CONTENEDOR_PRECINTO> V_CONTENEDOR_PRECINTO { get; set; }
        public virtual DbSet<V_CONT_CAMION_FACT> V_CONT_CAMION_FACT { get; set; }
        public virtual DbSet<V_CON_SIN_FIN_CONT> V_CON_SIN_FIN_CONT { get; set; }
        public virtual DbSet<V_CONT_EMBARCADOS_WEXP041> V_CONT_EMBARCADOS_WEXP041 { get; set; }
        public virtual DbSet<V_CONT_SIN_EMBARCAR_WEXP041> V_CONT_SIN_EMBARCAR_WEXP041 { get; set; }
        public virtual DbSet<V_PRODUCTOS_SIN_PREP_WEXP041> V_PRODUCTOS_SIN_PREP_WEXP041 { get; set; }
        public virtual DbSet<V_CANTIDAD_ENVIADA_EXP> V_CANTIDAD_ENVIADA_EXP { get; set; }
        public virtual DbSet<V_CANT_CONTE_PUERTA_WEXP> V_CANT_CONTE_PUERTA_WEXP { get; set; }
        public virtual DbSet<V_DET_PICKING_MOSTRADOR> V_DET_PICKING_MOSTRADOR { get; set; }
        public virtual DbSet<V_TODO_ASIGNADO_CAMION> V_TODO_ASIGNADO_CAMION { get; set; }
        public virtual DbSet<V_DET_CONTENEDORES_WEXP330> V_DET_CONTENEDORES_WEXP330 { get; set; }
        public virtual DbSet<V_REC200_PEDIDOS_CROSS_DOCK> V_REC200_PEDIDOS_CROSS_DOCK { get; set; }
        public virtual DbSet<V_REC200_SELECCION_CROSS_DOCK> V_REC200_SELECCION_CROSS_DOCK { get; set; }
        public virtual DbSet<V_REC210_CROSS_DOCKING> V_REC210_CROSS_DOCKING { get; set; }
        public virtual DbSet<V_TEMP_PEDIDO_MOSTRADOR> V_TEMP_PEDIDO_MOSTRADOR { get; set; }
        public virtual DbSet<V_CONDICION_LIBERACION> V_CONDICION_LIBERACION { get; set; }
        public virtual DbSet<V_TIPO_EXPEDICION> V_TIPO_EXPEDICION { get; set; }
        public virtual DbSet<V_TIPO_PEDIDO> V_TIPO_PEDIDO { get; set; }
        public virtual DbSet<V_PREDIOS_PICKING_PRE050> V_PREDIOS_PICKING_PRE050 { get; set; }
        public virtual DbSet<V_ONDA> V_ONDA { get; set; }
        public virtual DbSet<V_PEDIDOS_COMPATIBLES_WPRE051> V_PEDIDOS_COMPATIBLES_WPRE051 { get; set; }
        public virtual DbSet<V_PEND_LIB_PRE050> V_PEND_LIB_PRE050 { get; set; }
        public virtual DbSet<V_LIBERACION_AUTOMATICA_PEND> V_LIBERACION_AUTOMATICA_PEND { get; set; }
        public virtual DbSet<V_PREP_NO_ANULAR_SEL_WPRE450> V_PREP_NO_ANULAR_SEL_WPRE450 { get; set; }
        public virtual DbSet<V_PREP_NO_ANULAR_WPRE450> V_PREP_NO_ANULAR_WPRE450 { get; set; }
        public virtual DbSet<V_EVENTO_AGENDA_CERRADAS> V_EVENTO_AGENDA_CERRADAS { get; set; }
        public virtual DbSet<V_EVENTO_SALDO_REF> V_EVENTO_SALDO_REF { get; set; }
        public virtual DbSet<V_EVENTO_SALDO_FAC> V_EVENTO_SALDO_FAC { get; set; }
        public virtual DbSet<V_VERIFICACION_PED_MAIL> V_VERIFICACION_PED_MAIL { get; set; }
        public virtual DbSet<V_ORT_ORDEN_TAREA_WORT040> V_ORT_ORDEN_TAREA_WORT040 { get; set; }
        public virtual DbSet<V_RESUMEN_AGENDA_WREC250> V_RESUMEN_AGENDA_WREC250 { get; set; }
        public virtual DbSet<V_SEG_PED_PRE640> V_SEG_PED_PRE640 { get; set; }
        public virtual DbSet<V_TRACE_STOCK> V_TRACE_STOCK { get; set; }
        public virtual DbSet<V_ESTOQUE_STO005> V_ESTOQUE_STO005 { get; set; }
        public virtual DbSet<V_PALLET_TRANSFERENCIA_WSTO498> V_PALLET_TRANSFERENCIA_WSTO498 { get; set; }
        public virtual DbSet<V_CTR_CALIDAD_WSTO110> V_CTR_CALIDAD_WSTO110 { get; set; }
        public virtual DbSet<V_CTR_CALIDAD_WSTO060> V_CTR_CALIDAD_WSTO060 { get; set; }
        public virtual DbSet<V_PLANIFICACION_LIB_WPRE670> V_PLANIFICACION_LIB_WPRE670 { get; set; }
        public virtual DbSet<V_CONSULTA_PRE660> V_CONSULTA_PRE660 { get; set; }
        public virtual DbSet<V_RETORNO_BULTO_EXP340> V_RETORNO_BULTO_EXP340 { get; set; }
        public virtual DbSet<V_INV_CONTROL_MONO_PROD_LOTE> V_INV_CONTROL_MONO_PROD_LOTE { get; set; }
        public virtual DbSet<V_AGENDA_CALENDARIO_REC710> V_AGENDA_CALENDARIO_REC710 { get; set; }
        public virtual DbSet<V_DET_PEDIDO_SAIDA_WPRE101> V_DET_PEDIDO_SAIDA_WPRE101 { get; set; }
        public virtual DbSet<V_DETALLE_AGENDA_WREC171> V_DETALLE_AGENDA_WREC171 { get; set; }
        public virtual DbSet<V_PEDIDO_SAIDA_WPRE100> V_PEDIDO_SAIDA_WPRE100 { get; set; }
        public virtual DbSet<V_PRE080_ANALISIS_RECHAZO> V_PRE080_ANALISIS_RECHAZO { get; set; }
        public virtual DbSet<V_RECEPCION_WREC170> V_RECEPCION_WREC170 { get; set; }
        public virtual DbSet<V_PUERTA_EMBARQUE_WREG080> V_PUERTA_EMBARQUE_WREG080 { get; set; }
        public virtual DbSet<V_PRODS_SIN_PICKING_WREG060> V_PRODS_SIN_PICKING_WREG060 { get; set; }
        public virtual DbSet<V_REAB_PRE680> V_REAB_PRE680 { get; set; }
        public virtual DbSet<V_REG090_RAMO_PRODUTO> V_REG090_RAMO_PRODUTO { get; set; }
        public virtual DbSet<V_PRODUCTO_PALLET_WREG605> V_PRODUCTO_PALLET_WREG605 { get; set; }
        public virtual DbSet<V_REG605_PALLETS> V_REG605_PALLETS { get; set; }
        public virtual DbSet<V_USERS> V_USERS { get; set; }
        public virtual DbSet<V_EXP040_CAMIONES> V_EXP040_CAMIONES { get; set; }
        public virtual DbSet<V_COF010_LENGUAJE_IMPRESION> V_COF010_LENGUAJE_IMPRESION { get; set; }
        public virtual DbSet<V_COF020_CONFIGURACION> V_COF020_CONFIGURACION { get; set; }
        public virtual DbSet<V_COF030_TEMPLATE_ETIQUETA> V_COF030_TEMPLATE_ETIQUETA { get; set; }
        public virtual DbSet<V_LOCALIZACION_WCOF050> V_LOCALIZACION_WCOF050 { get; set; }
        public virtual DbSet<V_COF060_SERVIDORES_IMPRESION> V_COF060_SERVIDORES_IMPRESION { get; set; }
        public virtual DbSet<V_LPARAMETROS_LCON010> V_LPARAMETROS_LCON010 { get; set; }
        public virtual DbSet<V_LPARAMETROS_CONFIG_LCON020> V_LPARAMETROS_CONFIG_LCON020 { get; set; }
        public virtual DbSet<V_NAM_WREG410> V_NAM_WREG410 { get; set; }
        public virtual DbSet<V_INV_ENDERECO_DET_ERROR> V_INV_ENDERECO_DET_ERROR { get; set; }
        public virtual DbSet<V_UNIDAD_DE_MEDIDA_WPAR110> V_UNIDAD_DE_MEDIDA_WPAR110 { get; set; }
        public virtual DbSet<V_TIPOS_CODIGOS_BARRA_WPAR604> V_TIPOS_CODIGOS_BARRA_WPAR604 { get; set; }
        public virtual DbSet<V_FACTURAC_CUENTA_CONT_WFAC405> V_FACTURAC_CUENTA_CONT_WFAC405 { get; set; }
        public virtual DbSet<V_FACTURA_COD_LIST_COT_WFAC256> V_FACTURA_COD_LIST_COT_WFAC256 { get; set; }
        public virtual DbSet<V_FACTURAC_LISTA_PREC_WFAC255> V_FACTURAC_LISTA_PREC_WFAC255 { get; set; }
        public virtual DbSet<V_FACTURACION_PROC_WFAC004> V_FACTURACION_PROC_WFAC004 { get; set; }
        public virtual DbSet<V_FACTURACION_RESULTA_WFAC006> V_FACTURACION_RESULTA_WFAC006 { get; set; }
        public virtual DbSet<V_FACTURACION_RESULTA_WFAC012> V_FACTURACION_RESULTA_WFAC012 { get; set; }
        public virtual DbSet<V_FACTURACION_PRECIO_EMPRESA> V_FACTURACION_PRECIO_EMPRESA { get; set; }
        public virtual DbSet<V_FOTO_STOCK_PALLET> V_FOTO_STOCK_PALLET { get; set; }
        public virtual DbSet<V_FACTURACION_COR_DET_10> V_FACTURACION_COR_DET_10 { get; set; }
        public virtual DbSet<V_FACTURACION_COR_DET_11> V_FACTURACION_COR_DET_11 { get; set; }
        public virtual DbSet<V_FACTURACION_COR_DET_05> V_FACTURACION_COR_DET_05 { get; set; }
        public virtual DbSet<V_FACTURACION_COR_DET_06> V_FACTURACION_COR_DET_06 { get; set; }
        public virtual DbSet<V_FACTURACION_COR_DET_08> V_FACTURACION_COR_DET_08 { get; set; }
        public virtual DbSet<V_FACTURACION_COR_DET_09> V_FACTURACION_COR_DET_09 { get; set; }
        public virtual DbSet<V_FACTURACION_COR_DET_12> V_FACTURACION_COR_DET_12 { get; set; }
        public virtual DbSet<V_FACTURACION_COR_DET_14> V_FACTURACION_COR_DET_14 { get; set; }
        public virtual DbSet<V_FACTURACION_COR_DET_21> V_FACTURACION_COR_DET_21 { get; set; }
        public virtual DbSet<V_COD_04> V_COD_04 { get; set; }
        public virtual DbSet<V_COD_05> V_COD_05 { get; set; }
        public virtual DbSet<V_COD_06> V_COD_06 { get; set; }
        public virtual DbSet<V_COD_07> V_COD_07 { get; set; }
        public virtual DbSet<V_COD_08> V_COD_08 { get; set; }
        public virtual DbSet<V_FACTURA_ERROR_RESULT_WFAC003> V_FACTURA_ERROR_RESULT_WFAC003 { get; set; }
        public virtual DbSet<V_FACTURACION_CODIGO_WFAC249> V_FACTURACION_CODIGO_WFAC249 { get; set; }
        public virtual DbSet<V_FACTURAC_EJECUCION_WFAC001> V_FACTURAC_EJECUCION_WFAC001 { get; set; }
        public virtual DbSet<V_FACTURA_HABILIT_CALCULO> V_FACTURA_HABILIT_CALCULO { get; set; }
        public virtual DbSet<V_FACTURACION_RESULT_WFAC002> V_FACTURACION_RESULT_WFAC002 { get; set; }
        public virtual DbSet<V_FACTURACION_PROC_WFAC010> V_FACTURACION_PROC_WFAC010 { get; set; }
        public virtual DbSet<V_FACTUR_RESULT_WFAC007> V_FACTUR_RESULT_WFAC007 { get; set; }
        public virtual DbSet<V_COR_11> V_COR_11 { get; set; }
        public virtual DbSet<V_COR_02> V_COR_02 { get; set; }
        public virtual DbSet<V_COR_03> V_COR_03 { get; set; }
        public virtual DbSet<V_COR_04> V_COR_04 { get; set; }
        public virtual DbSet<V_COR_05> V_COR_05 { get; set; }
        public virtual DbSet<V_COR_06> V_COR_06 { get; set; }
        public virtual DbSet<V_COR_08> V_COR_08 { get; set; }
        public virtual DbSet<V_COR_09> V_COR_09 { get; set; }
        public virtual DbSet<V_COR_12> V_COR_12 { get; set; }
        public virtual DbSet<V_COR_21> V_COR_21 { get; set; }
        public virtual DbSet<V_COR_14> V_COR_14 { get; set; }
        public virtual DbSet<V_FACTURAC_PROC_EMP_WFAC005> V_FACTURAC_PROC_EMP_WFAC005 { get; set; }
        public virtual DbSet<V_FACTURA_UNIDAD_MEDID_WFAC200> V_FACTURA_UNIDAD_MEDID_WFAC200 { get; set; }
        public virtual DbSet<V_FACTURA_UND_MEDI_EMP_WFAC230> V_FACTURA_UND_MEDI_EMP_WFAC230 { get; set; }
        public virtual DbSet<V_ORT_TAREA_WORT010> V_ORT_TAREA_WORT010 { get; set; }
        public virtual DbSet<V_ORT_INSUMOS_WORT020> V_ORT_INSUMOS_WORT020 { get; set; }
        public virtual DbSet<V_ORT_ORDEN_WORT030> V_ORT_ORDEN_WORT030 { get; set; }
        public virtual DbSet<V_TAREAS_WORT120> V_TAREAS_WORT120 { get; set; }
        public virtual DbSet<V_FACTURACION_PRECIO_EMPRESA_FAC011> V_FACTURACION_PRECIO_EMPRESA_FAC011 { get; set; }
        public virtual DbSet<V_TIPO_ENDERECO_WPAR050> V_TIPO_ENDERECO_WPAR050 { get; set; }
        public virtual DbSet<V_EXP110_LABEL_ESTILO> V_EXP110_LABEL_ESTILO { get; set; }
        public virtual DbSet<V_CONT_SIN_EMBARCAR_PED_SINCAM> V_CONT_SIN_EMBARCAR_PED_SINCAM { get; set; }
        public virtual DbSet<V_PRD_PEDIDO_LOTE_CONTENEDOR> V_PRD_PEDIDO_LOTE_CONTENEDOR { get; set; }
        public virtual DbSet<V_PEDIDO_EMPAQUE> V_PEDIDO_EMPAQUE { get; set; }
        public virtual DbSet<V_PEDIDO_PRODUTO_CONTENEDOR> V_PEDIDO_PRODUTO_CONTENEDOR { get; set; }
        public virtual DbSet<V_PRODUTOS_SIN_PREP_PED_SINCAM> V_PRODUTOS_SIN_PREP_PED_SINCAM { get; set; }
        public virtual DbSet<V_EXP110_DET_PICKING> V_EXP110_DET_PICKING { get; set; }
        public virtual DbSet<V_PEDIDO_PREPARADO_COMPLETO> V_PEDIDO_PREPARADO_COMPLETO { get; set; }
        public virtual DbSet<V_CONTENEDOR_WISEX150> V_CONTENEDOR_WISEX150 { get; set; }
        public virtual DbSet<V_DET_PEDIDO_PRE340> V_DET_PEDIDO_PRE340 { get; set; }
        public virtual DbSet<V_CONTENEDOR_PEDIDO_CONSULTA> V_CONTENEDOR_PEDIDO_CONSULTA { get; set; }
        public virtual DbSet<V_PEDIDO_FACTURADO_CONSOLIDADO> V_PEDIDO_FACTURADO_CONSOLIDADO { get; set; }
        public virtual DbSet<V_PEDIDO_PRODUTO_CAMION> V_PEDIDO_PRODUTO_CAMION { get; set; }
        public virtual DbSet<V_PRODUTOS_SIN_CAMION> V_PRODUTOS_SIN_CAMION { get; set; }
        public virtual DbSet<V_PEDIDO_CLIENTE_PR340> V_PEDIDO_CLIENTE_PR340 { get; set; }
        public virtual DbSet<V_EXP110_CONT_PREP> V_EXP110_CONT_PREP { get; set; }
        public virtual DbSet<V_CONTACTO_GRUPO_WEVT030> V_CONTACTO_GRUPO_WEVT030 { get; set; }
        public virtual DbSet<V_CONTACTO_GRU_NS_WEVT030> V_CONTACTO_GRU_NS_WEVT030 { get; set; }
        public virtual DbSet<V_CONTACTO_WEVT020> V_CONTACTO_WEVT020 { get; set; }
        public virtual DbSet<V_EVENTO_INSTANCIA_WEVT040> V_EVENTO_INSTANCIA_WEVT040 { get; set; }
        public virtual DbSet<V_EVENTO_MAIL_SALIDA_WEVT050> V_EVENTO_MAIL_SALIDA_WEVT050 { get; set; }
        public virtual DbSet<V_EVENTO_NOTIFICACION_WEVT050> V_EVENTO_NOTIFICACION_WEVT050 { get; set; }
        public virtual DbSet<V_EVENTO_ARCHIVO_WEVT050> V_EVENTO_ARCHIVO_WEVT050 { get; set; }
        public virtual DbSet<V_EVENTO_PARAM_INS_WEVT040> V_EVENTO_PARAM_INS_WEVT040 { get; set; }
        public virtual DbSet<V_EVENTO_WEVT010> V_EVENTO_WEVT010 { get; set; }
        public virtual DbSet<V_EVT060_TEMPLATES_NOTIFICACION> V_EVT060_TEMPLATES_NOTIFICACION { get; set; }
        public virtual DbSet<V_CONTACTO_INSTANCIA_WEVT040> V_CONTACTO_INSTANCIA_WEVT040 { get; set; }
        public virtual DbSet<V_GRUPO_INSTANCIA__WEVT040> V_GRUPO_INSTANCIA__WEVT040 { get; set; }
        public virtual DbSet<V_GRUPOS_WEVT030> V_GRUPOS_WEVT030 { get; set; }
        public virtual DbSet<V_PEDIDOS_EXPEDIDOS_EXEL> V_PEDIDOS_EXPEDIDOS_EXEL { get; set; }
        public virtual DbSet<V_CONTACTOS_INSTANCIA> V_CONTACTOS_INSTANCIA { get; set; }
        public virtual DbSet<V_ARCHIVO_ADJUNTO> V_ARCHIVO_ADJUNTO { get; set; }
        public virtual DbSet<V_ARCHIVO_VERSION> V_ARCHIVO_VERSION { get; set; }
        public virtual DbSet<V_CLASIFICACION_STOCK_WSTO640> V_CLASIFICACION_STOCK_WSTO640 { get; set; }
        public virtual DbSet<V_LOG_STOCK_ENVASE> V_LOG_STOCK_ENVASE { get; set; }
        public virtual DbSet<V_STOCK_ENVASE> V_STOCK_ENVASE { get; set; }
        public virtual DbSet<V_STO700_LPN> V_STO700_LPN { get; set; }
        public virtual DbSet<V_STO722_HISTORIAL_DETALLE> V_STO722_HISTORIAL_DETALLE { get; set; }
        public virtual DbSet<V_STO722_HISTORIAL_ATRIBUTO_DETALLE> V_STO722_HISTORIAL_ATRIBUTO_DETALLE { get; set; }
        public virtual DbSet<V_STO700_LT_LPN> V_STO700_LT_LPN { get; set; }
        public virtual DbSet<V_STO710_CONSULTA_LPN_ATRIBUTOS> V_STO710_CONSULTA_LPN_ATRIBUTOS { get; set; }
        public virtual DbSet<V_STO720_LPN_LINEAS> V_STO720_LPN_LINEAS { get; set; }
        public virtual DbSet<V_STO730_AUDITORIA_LPN> V_STO730_AUDITORIA_LPN { get; set; }
        public virtual DbSet<V_STO730_DET_AUDITORIA_LPN> V_STO730_DET_AUDITORIA_LPN { get; set; }
        public virtual DbSet<V_STO740_LPN_DET_ATRIBUTO_CAB> V_STO740_LPN_DET_ATRIBUTO_CAB { get; set; }
        public virtual DbSet<V_STO740_LPN_DET_ATRIBUTO_DET> V_STO740_LPN_DET_ATRIBUTO_DET { get; set; }
        public virtual DbSet<V_STO800_TRASPASO_CONFIG> V_STO800_TRASPASO_CONFIG { get; set; }
        public virtual DbSet<V_STO800_TIPOS_TRASPASO> V_STO800_TIPOS_TRASPASO { get; set; }
        public virtual DbSet<V_STO810_TRASP_MAPEO_PRODUTO> V_STO810_TRASP_MAPEO_PRODUTO { get; set; }
        public virtual DbSet<V_STO820_TRASPASOS_EMPRESAS> V_STO820_TRASPASOS_EMPRESAS { get; set; }
        public virtual DbSet<V_STO820_PREPARACION_PENDIENTE> V_STO820_PREPARACION_PENDIENTE { get; set; }
        public virtual DbSet<V_STO820_PREPARACION_PREPARADO> V_STO820_PREPARACION_PREPARADO { get; set; }
        public virtual DbSet<V_STO820_DETALLE_PEDIDOS> V_STO820_DETALLE_PEDIDOS { get; set; }
        public virtual DbSet<V_REC010_RECEPCION_REFERENCIA> V_REC010_RECEPCION_REFERENCIA { get; set; }
        public virtual DbSet<V_REC011_REC_REFERENCIA_DET> V_REC011_REC_REFERENCIA_DET { get; set; }
        public virtual DbSet<V_REC150_ETIQUETAS> V_REC150_ETIQUETAS { get; set; }
        public virtual DbSet<V_REG009_PRODUCTOS> V_REG009_PRODUCTOS { get; set; }
        public virtual DbSet<V_REG009_PRODUCTO_COMPONENTE1> V_REG009_PRODUCTO_COMPONENTE1 { get; set; }
        public virtual DbSet<V_REG050_PICKING_PRODUCTO> V_REG050_PICKING_PRODUCTO { get; set; }
        public virtual DbSet<V_REG603_CODIGO_BARRAS> V_REG603_CODIGO_BARRAS { get; set; }
        public virtual DbSet<V_REC180_LOG_ETIQUETAS> V_REC180_LOG_ETIQUETAS { get; set; }
        public virtual DbSet<V_PRE050_ONDA> V_PRE050_ONDA { get; set; }
        public virtual DbSet<V_PRE050_PEND_LIB> V_PRE050_PEND_LIB { get; set; }
        public virtual DbSet<V_PRE050_EMPRESAS_PED_PENDIEN> V_PRE050_EMPRESAS_PED_PENDIEN { get; set; }
        public virtual DbSet<V_PRE051_PEDIDOS_COMPATIBLES> V_PRE051_PEDIDOS_COMPATIBLES { get; set; }
        public virtual DbSet<V_PRE052_PICKING> V_PRE052_PICKING { get; set; }
        public virtual DbSet<V_PRE061_DET_PREPARACION> V_PRE061_DET_PREPARACION { get; set; }
        public virtual DbSet<V_PRE110_DET_PEDIDO_SALIDA> V_PRE110_DET_PEDIDO_SALIDA { get; set; }
        public virtual DbSet<V_PRE150_DETALLE_PEDIDO> V_PRE150_DETALLE_PEDIDO { get; set; }
        public virtual DbSet<V_PRE150_DET_PEDIDO_SALIDA> V_PRE150_DET_PEDIDO_SALIDA { get; set; }
        public virtual DbSet<V_PRE152_DETALLE_PEDIDO_LPN> V_PRE152_DETALLE_PEDIDO_LPN { get; set; }
        public virtual DbSet<V_PRE153_ATRIBUTO_LPN_DETALLE_PEDIDO> V_PRE153_ATRIBUTO_LPN_DETALLE_PEDIDO { get; set; }
        public virtual DbSet<V_PRE154_DETALLE_ATRIBUTOS_LPN_DETALLE_PEDIDO> V_PRE154_DETALLE_ATRIBUTOS_LPN_DETALLE_PEDIDO { get; set; }
        public virtual DbSet<V_PRE155_ATRIBUTOS_DE_DETALLE_DE_PEDIDO> V_PRE155_ATRIBUTOS_DE_DETALLE_DE_PEDIDO { get; set; }
        public virtual DbSet<V_PRE156_DETALLE_ATRIBUTOS_DE_DETALLE_PEDIDO> V_PRE156_DETALLE_ATRIBUTOS_DE_DETALLE_PEDIDO { get; set; }
        public virtual DbSet<V_PRE161_PICKING_PENDIENTE> V_PRE161_PICKING_PENDIENTE { get; set; }
        public virtual DbSet<V_PRE162_PICKING_PENDIENTE> V_PRE162_PICKING_PENDIENTE { get; set; }
        public virtual DbSet<V_PRE160_PICKING_PENDIENTE> V_PRE160_PICKING_PENDIENTE { get; set; }
        public virtual DbSet<V_PRE170_ANALISIS_RECHAZO> V_PRE170_ANALISIS_RECHAZO { get; set; }
        public virtual DbSet<V_PRE170_ANALISIS_RECHAZO_LPN> V_PRE170_ANALISIS_RECHAZO_LPN { get; set; }
        public virtual DbSet<V_PRE060_CONTENEDOR> V_PRE060_CONTENEDOR { get; set; }
        public virtual DbSet<V_PRE130_DET_PICKING> V_PRE130_DET_PICKING { get; set; }
        public virtual DbSet<V_PRE360_STOCK_PICKING_MAL> V_PRE360_STOCK_PICKING_MAL { get; set; }
        public virtual DbSet<V_PREDIO_USUARIO> V_PREDIO_USUARIO { get; set; }
        public virtual DbSet<V_REC170_RECEPCION> V_REC170_RECEPCION { get; set; }
        public virtual DbSet<V_REC171_AGENDA_DETALLE> V_REC171_AGENDA_DETALLE { get; set; }
        public virtual DbSet<V_STO060_CTR_CALIDAD> V_STO060_CTR_CALIDAD { get; set; }
        public virtual DbSet<V_STO500_STOCK_POR_PRODUCTO> V_STO500_STOCK_POR_PRODUCTO { get; set; }
        public virtual DbSet<V_EXP041_PEDIDOS_EXPEDIDOS> V_EXP041_PEDIDOS_EXPEDIDOS { get; set; }
        public virtual DbSet<V_EXP043_PEDIDOS_PEND_CAMION> V_EXP043_PEDIDOS_PEND_CAMION { get; set; }
        public virtual DbSet<V_EXP045_CONT_EMBARCADOS> V_EXP045_CONT_EMBARCADOS { get; set; }
        public virtual DbSet<V_EXP045_CONT_SIN_EMBARCAR> V_EXP045_CONT_SIN_EMBARCAR { get; set; }
        public virtual DbSet<V_EXP045_PRODUCTOS> V_EXP045_PRODUCTOS { get; set; }
        public virtual DbSet<V_EXP045_PRODUCTOS_SIN_PREP> V_EXP045_PRODUCTOS_SIN_PREP { get; set; }
        public virtual DbSet<V_EXP051_PRODS_PROBLEMAS> V_EXP051_PRODS_PROBLEMAS { get; set; }
        public virtual DbSet<V_CAMION_WEXP> V_CAMION_WEXP { get; set; }
        public virtual DbSet<V_RECURSOS_USUARIO> V_RECURSOS_USUARIO { get; set; }
        public virtual DbSet<V_REG250_TIPO_VEHICULO> V_REG250_TIPO_VEHICULO { get; set; }
        public virtual DbSet<V_REG240_VEHICULO> V_REG240_VEHICULO { get; set; }
        public virtual DbSet<V_SEG020_PERFILES> V_SEG020_PERFILES { get; set; }
        public virtual DbSet<V_SEG020_RECURSOS> V_SEG020_RECURSOS { get; set; }
        public virtual DbSet<V_SEG020_RECURSOS_ASIGNADOS> V_SEG020_RECURSOS_ASIGNADOS { get; set; }
        public virtual DbSet<V_SEG030_USUARIOS> V_SEG030_USUARIOS { get; set; }
        public virtual DbSet<V_SEG030_RECURSOS_ASIGNADOS> V_SEG030_RECURSOS_ASIGNADOS { get; set; }
        public virtual DbSet<V_SEG030_GRUPO_CONSULTA_FUNC> V_SEG030_GRUPO_CONSULTA_FUNC { get; set; }
        public virtual DbSet<V_COF040_IMPRESORAS> V_COF040_IMPRESORAS { get; set; }
        public virtual DbSet<V_EXP011_CONTENEDOR_CAMION> V_EXP011_CONTENEDOR_CAMION { get; set; }
        public virtual DbSet<V_EXP010_CARGA_CAMION> V_EXP010_CARGA_CAMION { get; set; }
        public virtual DbSet<V_EXP013_PEDIDO_CAMION> V_EXP013_PEDIDO_CAMION { get; set; }
        public virtual DbSet<V_ANULACIONES_PENDIENT_CAMION> V_ANULACIONES_PENDIENT_CAMION { get; set; }
        public virtual DbSet<V_PRE100_PEDIDO_SAIDA> V_PRE100_PEDIDO_SAIDA { get; set; }
        public virtual DbSet<V_REL_TP_EXPEDICION_TP_PEDIDO> V_REL_TP_EXPEDICION_TP_PEDIDO { get; set; }
        public virtual DbSet<V_PRE052_PEDIDOS_ASOCIADOS> V_PRE052_PEDIDOS_ASOCIADOS { get; set; }
        public virtual DbSet<V_PRE052_PEDIDOS_DISPONIBLES> V_PRE052_PEDIDOS_DISPONIBLES { get; set; }
        public virtual DbSet<V_PRE120_ANULACION_PREPARACION> V_PRE120_ANULACION_PREPARACION { get; set; }
        public virtual DbSet<V_REC300_MOTIVO_ALMACENAMIENTO> V_REC300_MOTIVO_ALMACENAMIENTO { get; set; }
        public virtual DbSet<V_DISPONIBLE_CROSS_DOCKING> V_DISPONIBLE_CROSS_DOCKING { get; set; }
        public virtual DbSet<V_REG070_ZONA_UBICACION> V_REG070_ZONA_UBICACION { get; set; }
        public virtual DbSet<V_PRE250_REGLA_LIBERACION> V_PRE250_REGLA_LIBERACION { get; set; }
        public virtual DbSet<V_PRE250_CLIENTES_DISPONIBLES> V_PRE250_CLIENTES_DISPONIBLES { get; set; }
        public virtual DbSet<V_PRE250_CLIENTES_SELECIONADOS> V_PRE250_CLIENTES_SELECIONADOS { get; set; }
        public virtual DbSet<V_TAREA_REFERENCIA_TRACKING> V_TAREA_REFERENCIA_TRACKING { get; set; }
        public virtual DbSet<V_PUNTOS_ENTREGA_CARGA> V_PUNTOS_ENTREGA_CARGA { get; set; }
        public virtual DbSet<V_PUNTOS_ENTREGA_EXPEDIDOS> V_PUNTOS_ENTREGA_EXPEDIDOS { get; set; }
        public virtual DbSet<V_PUNTOS_ENTREGA_TRACKING> V_PUNTOS_ENTREGA_TRACKING { get; set; }
        public virtual DbSet<V_PEDIDOS_PLANIFICADOS_CAMION> V_PEDIDOS_PLANIFICADOS_CAMION { get; set; }
        public virtual DbSet<V_CONTENEDORES_ENTREGA> V_CONTENEDORES_ENTREGA { get; set; }
        public virtual DbSet<V_CONTENEDORES_ENTREGA_EXP> V_CONTENEDORES_ENTREGA_EXP { get; set; }
        public virtual DbSet<V_PEDIDOS_NO_PLANIFICADOS> V_PEDIDOS_NO_PLANIFICADOS { get; set; }
        public virtual DbSet<V_PEDIDOS_NO_PLANIFICADOS_JOB> V_PEDIDOS_NO_PLANIFICADOS_JOB { get; set; }
        public virtual DbSet<V_CROSS_DOCKING_PEND_REC220> V_CROSS_DOCKING_PEND_REC220 { get; set; }
        public virtual DbSet<V_CROSS_DOCK_TEMP_WREC220> V_CROSS_DOCK_TEMP_WREC220 { get; set; }
        public virtual DbSet<V_ETIQUETA_LOTE_WREC270> V_ETIQUETA_LOTE_WREC270 { get; set; }
        public virtual DbSet<V_REC275_ESTRATEGIA> V_REC275_ESTRATEGIA { get; set; }
        public virtual DbSet<V_REC275_LOGICA_INSTANCIA> V_REC275_LOGICA_INSTANCIA { get; set; }
        public virtual DbSet<V_REC275_LOGICAS> V_REC275_LOGICAS { get; set; }
        public virtual DbSet<V_REC275_PARAMETROS> V_REC275_PARAMETROS { get; set; }
        public virtual DbSet<V_REC275_PARAMETROS_INSTANCIAS> V_REC275_PARAMETROS_INSTANCIAS { get; set; }
        public virtual DbSet<V_REC275_ASOCIACIONES> V_REC275_ASOCIACIONES { get; set; }
        public virtual DbSet<V_REC275_ALM_OPERATIVA_ASOCIABLE> V_REC275_ALM_OPERATIVA_ASOCIABLE { get; set; }
        public virtual DbSet<V_REC275_OPERATIVAS_ASOCIADAS> V_REC275_OPERATIVAS_ASOCIADAS { get; set; }
        public virtual DbSet<V_REC275_GRUPOS> V_REC275_GRUPOS { get; set; }
        public virtual DbSet<V_REC280_PANEL_SUGERENCIA> V_REC280_PANEL_SUGERENCIA { get; set; }
        public virtual DbSet<V_REC280_PANEL_SUGERENCIA_DET> V_REC280_PANEL_SUGERENCIA_DET { get; set; }
        public virtual DbSet<V_REC280_PANEL_SUGERENCIA_REABASTECIMIENTO> V_REC280_PANEL_SUGERENCIA_REABASTECIMIENTO { get; set; }
        public virtual DbSet<V_CONTROL_CONT_ENV_WPRE220> V_CONTROL_CONT_ENV_WPRE220 { get; set; }
        public virtual DbSet<V_ETIQUETA_PRE_SEP_WREC270> V_ETIQUETA_PRE_SEP_WREC270 { get; set; }
        public virtual DbSet<V_AGENDAS_WREC270> V_AGENDAS_WREC270 { get; set; }
        public virtual DbSet<V_FACTU_SIN_EXPEDIR_REC270> V_FACTU_SIN_EXPEDIR_REC270 { get; set; }
        public virtual DbSet<V_PED_CAR_EXP_REC270> V_PED_CAR_EXP_REC270 { get; set; }
        public virtual DbSet<V_INT050_EMPRESAS_BLOQUEADAS> V_INT050_EMPRESAS_BLOQUEADAS { get; set; }
        public virtual DbSet<V_FACTURACION_COR_DET_02> V_FACTURACION_COR_DET_02 { get; set; }
        public virtual DbSet<V_PEDIDOS_SIN_CERRAR> V_PEDIDOS_SIN_CERRAR { get; set; }
        public virtual DbSet<V_DOC100_DOC_PREPARACION> V_DOC100_DOC_PREPARACION { get; set; }
        public virtual DbSet<V_DOC_DISP_ASOCIAR_PREP> V_DOC_DISP_ASOCIAR_PREP { get; set; }
        public virtual DbSet<V_PREP_DISP_ASOCIAR> V_PREP_DISP_ASOCIAR { get; set; }
        public virtual DbSet<V_FAC008_RESULTADO_DETALLE> V_FAC008_RESULTADO_DETALLE { get; set; }
        public virtual DbSet<V_CARGAS_CON_MULTIPLE_PEDIDO> V_CARGAS_CON_MULTIPLE_PEDIDO { get; set; }
        public virtual DbSet<V_CAMION_CARGA_PEDIDO> V_CAMION_CARGA_PEDIDO { get; set; }
        public virtual DbSet<V_PICKING_WISEX150> V_PICKING_WISEX150 { get; set; }
        public virtual DbSet<V_DET_ETIQUETA_SIN_CLASIFICAR> V_DET_ETIQUETA_SIN_CLASIFICAR { get; set; }
        public virtual DbSet<V_LPN_TIPO> V_LPN_TIPO { get; set; }
        public virtual DbSet<V_LPN_TIPO_ATRIBUTO> V_LPN_TIPO_ATRIBUTO { get; set; }
        public virtual DbSet<V_LPN_TIPO_ATRIBUTO_DET> V_LPN_TIPO_ATRIBUTO_DET { get; set; }
        public virtual DbSet<V_LPN_CODIGOS_BARRAS> V_LPN_CODIGOS_BARRAS { get; set; }
        public virtual DbSet<V_ATRIBUTO_VALIDACION_DISP> V_ATRIBUTO_VALIDACION_DISP { get; set; }
        public virtual DbSet<V_ATRIBUTO_VALIDACION_ASOCIADO> V_ATRIBUTO_VALIDACION_ASOCIADO { get; set; }
        public virtual DbSet<V_CARGAS_EGRESO_DOCUMENTAL> V_CARGAS_EGRESO_DOCUMENTAL { get; set; }
        public virtual DbSet<V_PED_NUEVAS_CARGAS> V_PED_NUEVAS_CARGAS { get; set; }
        public virtual DbSet<V_PLANIFICACION_CAMION> V_PLANIFICACION_CAMION { get; set; }
        public virtual DbSet<V_EGRESOS_A_MARCAR> V_EGRESOS_A_MARCAR { get; set; }
        public virtual DbSet<V_PUNTOS_ENTREGA_CLIENTE> V_PUNTOS_ENTREGA_CLIENTE { get; set; }
        public virtual DbSet<V_PLANIFICACION_DEVOLUCION> V_PLANIFICACION_DEVOLUCION { get; set; }
        public virtual DbSet<V_PLANIFICACION_DEVOLUCION_DET> V_PLANIFICACION_DEVOLUCION_DET { get; set; }
        public virtual DbSet<V_REG300_GRUPOS> V_REG300_GRUPOS { get; set; }
        public virtual DbSet<V_REG300_GRUPOS_ELIMINABLES> V_REG300_GRUPOS_ELIMINABLES { get; set; }
        public virtual DbSet<V_REG300_REGLAS> V_REG300_REGLAS { get; set; }
        public virtual DbSet<V_REG300_PARAMETROS> V_REG300_PARAMETROS { get; set; }
        public virtual DbSet<V_REG300_PARAMETROS_REGLA> V_REG300_PARAMETROS_REGLA { get; set; }
        public virtual DbSet<V_REPORTE_CONF_RECEPCION> V_REPORTE_CONF_RECEPCION { get; set; }
        public virtual DbSet<V_REPORTE_CONF_RECEPCION_DET> V_REPORTE_CONF_RECEPCION_DET { get; set; }
        public virtual DbSet<V_REPORTE_PACKING_LIST> V_REPORTE_PACKING_LIST { get; set; }
        public virtual DbSet<V_REPORTE_PACKING_LIST_DET> V_REPORTE_PACKING_LIST_DET { get; set; }
        public virtual DbSet<V_REPORTE_CONT_CAMION> V_REPORTE_CONT_CAMION { get; set; }
        public virtual DbSet<V_REPORTE_CONT_CAMION_DET> V_REPORTE_CONT_CAMION_DET { get; set; }
        public virtual DbSet<V_REPORTE_CONTROL_CAMBIO> V_REPORTE_CONTROL_CAMBIO { get; set; }
        public virtual DbSet<V_REC410_SUGERENCIA_REABAST> V_REC410_SUGERENCIA_REABAST { get; set; }
        public virtual DbSet<V_STOCK_PARA_REABASTECIMIENTO> V_STOCK_PARA_REABASTECIMIENTO { get; set; }
        public virtual DbSet<V_REC170_ETIQUETA_UT> V_REC170_ETIQUETA_UT { get; set; }
        public virtual DbSet<V_AUT100_EJECUCIONES> V_AUT100_EJECUCIONES { get; set; }
        public virtual DbSet<V_AUT100_CARACTERISTICAS2> V_AUT100_CARACTERISTICAS2 { get; set; }
        public virtual DbSet<V_AUT100_CARACTERISTICAS> V_AUT100_CARACTERISTICAS { get; set; }
        public virtual DbSet<V_AUT100_POSICIONES> V_AUT100_POSICIONES { get; set; }
        public virtual DbSet<V_STOCK_AUTOMATISMO_AUT100> V_STOCK_AUTOMATISMO_AUT100 { get; set; }
        public virtual DbSet<V_PRODUCTOS_ASOCIADOS_AUTOMATISMO> V_PRODUCTOS_ASOCIADOS_AUTOMATISMO { get; set; }
        public virtual DbSet<V_AUT100_INTERFACES> V_AUT100_INTERFACES { get; set; }
        public virtual DbSet<V_COF110_SERVICIOS_INTEGRACION> V_COF110_SERVICIOS_INTEGRACION { get; set; }
        public virtual DbSet<V_PTL010_PICK_TO_LIGHT> V_PTL010_PICK_TO_LIGHT { get; set; }
        public virtual DbSet<V_PTL010_AGRU_VL_COMP_CONT_PICK> V_PTL010_AGRU_VL_COMP_CONT_PICK { get; set; }
        public virtual DbSet<V_PTL010_AGRU_SUBCLASE_PROD> V_PTL010_AGRU_SUBCLASE_PROD { get; set; }
        public virtual DbSet<V_POTERIA_VEHICULO_AGENDA> V_POTERIA_VEHICULO_AGENDA { get; set; }
        public virtual DbSet<V_PORTERIA_VEHICULO> V_PORTERIA_VEHICULO { get; set; }
        public virtual DbSet<V_PORTERIA_PERSONA> V_PORTERIA_PERSONA { get; set; }
        public virtual DbSet<V_PORTERIA_CONT_PRE_REG> V_PORTERIA_CONT_PRE_REG { get; set; }
        public virtual DbSet<V_POTERIA_VEHICULO_CAMION> V_POTERIA_VEHICULO_CAMION { get; set; }
        public virtual DbSet<V_PORTERIA_EGRESOS> V_PORTERIA_EGRESOS { get; set; }
        public virtual DbSet<V_DOCUMENTO_DOC080> V_DOCUMENTO_DOC080 { get; set; }
        public virtual DbSet<V_PORTERIA_AGENDAS> V_PORTERIA_AGENDAS { get; set; }
        public virtual DbSet<V_PORTERIA_SALIDA_SIN_EGRESO> V_PORTERIA_SALIDA_SIN_EGRESO { get; set; }
        public virtual DbSet<V_PORTERIA_REGISTRO_PERSONA> V_PORTERIA_REGISTRO_PERSONA { get; set; }
        public virtual DbSet<V_PORTERIA_ENTRADA_SIN_AGENDA> V_PORTERIA_ENTRADA_SIN_AGENDA { get; set; }
        public virtual DbSet<V_DOCUMENTO_DOC260> V_DOCUMENTO_DOC260 { get; set; }
        public virtual DbSet<V_STO730_DET_ATRIBUTO_LPN> V_STO730_DET_ATRIBUTO_LPN { get; set; }
        public virtual DbSet<V_PED_PREP_CAMION_DOC_WEXP090> V_PED_PREP_CAMION_DOC_WEXP090 { get; set; }
        public virtual DbSet<V_FOTO_STOCK_WSTO310> V_FOTO_STOCK_WSTO310 { get; set; }
        public virtual DbSet<V_REC170_LPN> V_REC170_LPN { get; set; }
        public virtual DbSet<V_STO151_STOCK_LPN> V_STO151_STOCK_LPN { get; set; }
        public virtual DbSet<V_STOCK_PRODUCTO_LPN> V_STOCK_PRODUCTO_LPN { get; set; }
        public virtual DbSet<V_PRE100_LPN_AGREGADOS> V_PRE100_LPN_AGREGADOS { get; set; }
        public virtual DbSet<V_PRE100_LPN_DISPONIBLES> V_PRE100_LPN_DISPONIBLES { get; set; }
        public virtual DbSet<V_PRE100_DET_PEDIDO_LPN> V_PRE100_DET_PEDIDO_LPN { get; set; }
        public virtual DbSet<V_PRE100_DET_PEDIDO_LPN_ATRIB> V_PRE100_DET_PEDIDO_LPN_ATRIB { get; set; }
        public virtual DbSet<V_PRE100_ATRIBUTOS_TIPO_SIN_DEFINIR> V_PRE100_ATRIBUTOS_TIPO_SIN_DEFINIR { get; set; }
        public virtual DbSet<V_PRE100_ATRIBUTOS_LPN_DEFINIDOS> V_PRE100_ATRIBUTOS_LPN_DEFINIDOS { get; set; }
        public virtual DbSet<V_PRE100_DET_PEDIDO_ATRIBUTO> V_PRE100_DET_PEDIDO_ATRIBUTO { get; set; }

        public virtual DbSet<V_PRE100_ATRIBUTOS_SIN_DEFINIR> V_PRE100_ATRIBUTOS_SIN_DEFINIR { get; set; }
        public virtual DbSet<V_PRE100_ATRIBUTOS_DEFINIDOS> V_PRE100_ATRIBUTOS_DEFINIDOS { get; set; }
        public virtual DbSet<V_PRE110_DET_PEDIDO_LPN> V_PRE110_DET_PEDIDO_LPN { get; set; }
        public virtual DbSet<V_PRE110_DET_PEDIDO_ATRIB> V_PRE110_DET_PEDIDO_ATRIB { get; set; }
        public virtual DbSet<V_PRE110_DET_PEDIDO_LPN_ATR> V_PRE110_DET_PEDIDO_LPN_ATR { get; set; }
        public virtual DbSet<V_PRE110_ATRIBUTOS_LPN_DEFINIDOS> V_PRE110_ATRIBUTOS_LPN_DEFINIDOS { get; set; }
        public virtual DbSet<V_PRE110_ATRIBUTOS_DEFINIDOS> V_PRE110_ATRIBUTOS_DEFINIDOS { get; set; }
        public virtual DbSet<V_STO750_CONSULTA_UT> V_STO750_CONSULTA_UT { get; set; }
        public virtual DbSet<V_REC170_LPN_PLANIFICACION> V_REC170_LPN_PLANIFICACION { get; set; }
        public virtual DbSet<V_REC170_RECEPCION_LPNS> V_REC170_RECEPCION_LPNS { get; set; }
        public virtual DbSet<V_STOCK_LPN> V_STOCK_LPN { get; set; }
        public virtual DbSet<V_STOCK_SUELTO> V_STOCK_SUELTO { get; set; }
        public virtual DbSet<V_STO153_CTRL_CALIDAD_DET_LPN> V_STO153_CTRL_CALIDAD_DET_LPN { get; set; }
        public virtual DbSet<V_RESERVA_LPN_ATRIBUTO> V_RESERVA_LPN_ATRIBUTO { get; set; }
        public virtual DbSet<V_ESTILOS_LENGUAJES> V_ESTILOS_LENGUAJES { get; set; }
        public virtual DbSet<V_CAMION_CTRL_CONTENEDORES> V_CAMION_CTRL_CONTENEDORES { get; set; }
        public virtual DbSet<V_CONTENEDORES_PRODUCCION> V_CONTENEDORES_PRODUCCION { get; set; }
        public virtual DbSet<V_PRD110_DETALLES_INGRESO> V_PRD110_DETALLES_INGRESO { get; set; }
        public virtual DbSet<V_STOCK_DISP_ESPECIFICAR_LOTE> V_STOCK_DISP_ESPECIFICAR_LOTE { get; set; }
        public virtual DbSet<V_PRODUCTOS_FINALES_PRODUCCION> V_PRODUCTOS_FINALES_PRODUCCION { get; set; }
        public virtual DbSet<V_PRD113_PRODUCTOS_ESPERADOS> V_PRD113_PRODUCTOS_ESPERADOS { get; set; }
        public virtual DbSet<V_PRDC_PLANIFICACION_INSUMO> V_PRDC_PLANIFICACION_INSUMO { get; set; }
        public virtual DbSet<V_PRD112_RESULTADO_TEORICO> V_PRD112_RESULTADO_TEORICO { get; set; }
        public virtual DbSet<V_PRD112_INSUMOS_SOLICITADOS_EN_COMUN> V_PRD112_INSUMOS_SOLICITADOS_EN_COMUN { get; set; }
        public virtual DbSet<V_PRD113_STOCK_INSUMOS> V_PRD113_STOCK_INSUMOS { get; set; }
        public virtual DbSet<V_PRD113_PRODUCTOS_NO_ESPERADOS> V_PRD113_PRODUCTOS_NO_ESPERADOS { get; set; }
        public virtual DbSet<V_PRD113_PRODUCIR> V_PRD113_PRODUCIR { get; set; }
        public virtual DbSet<V_PRD113_STOCK_PRODUCCION> V_PRD113_STOCK_PRODUCCION { get; set; }
        public virtual DbSet<V_PRD113_PRODUCTOS_EXPULSABLE> V_PRD113_PRODUCTOS_EXPULSABLE { get; set; }
        public virtual DbSet<V_CONSUMIDOS_PRODUCCION> V_CONSUMIDOS_PRODUCCION { get; set; }
        public virtual DbSet<V_PRODUCIDOS_PRODUCCION> V_PRODUCIDOS_PRODUCCION { get; set; }
        public virtual DbSet<V_PRD113_REMANENTE_PRODUCIDO> V_PRD113_REMANENTE_PRODUCIDO { get; set; }
        public virtual DbSet<V_PRD110_DETALLES_PRODUCIDOS> V_PRD110_DETALLES_PRODUCIDOS { get; set; }
        public virtual DbSet<V_PRD110_DETALLES_CONSUMIDOS> V_PRD110_DETALLES_CONSUMIDOS { get; set; }
        public virtual DbSet<V_PRDC_PLANIFICACION_PEDIDO> V_PRDC_PLANIFICACION_PEDIDO { get; set; }

        public virtual DbSet<V_CODIGO_MULTIDATO_EMPRESA_DET> V_CODIGO_MULTIDATO_EMPRESA_DET { get; set; }
        public virtual DbSet<V_CODIGO_MULTIDATO> V_CODIGO_MULTIDATO { get; set; }
        public virtual DbSet<V_APLICACION> V_APLICACION { get; set; }
        public virtual DbSet<V_APLICACION_CAMPO> V_APLICACION_CAMPO { get; set; }
        public virtual DbSet<V_CODIGO_MULTIDATO_DET> V_CODIGO_MULTIDATO_DET { get; set; }
        public virtual DbSet<V_CODIGO_MULTIDATO_ASOCIADOS> V_CODIGO_MULTIDATO_ASOCIADOS { get; set; }

        public virtual DbSet<V_INV050_AVANCE_INVENTARIO> V_INV050_AVANCE_INVENTARIO { get; set; }
        public virtual DbSet<V_INV100_ANALISIS_INVENTARIO> V_INV100_ANALISIS_INVENTARIO { get; set; }
        public virtual DbSet<V_INV410_INVENTARIO> V_INV410_INVENTARIO { get; set; }
        public virtual DbSet<V_INV410_LPN_ATRIBUTO_CAB> V_INV410_LPN_ATRIBUTO_CAB { get; set; }
        public virtual DbSet<V_INV410_LPN_DET_ATRIBUTO_CAB> V_INV410_LPN_DET_ATRIBUTO_CAB { get; set; }
        public virtual DbSet<V_INV410_LPN_DET_ATRIBUTO_DET> V_INV410_LPN_DET_ATRIBUTO_DET { get; set; }
        public virtual DbSet<V_INV412_DET_CONTEO> V_INV412_DET_CONTEO { get; set; }
        public virtual DbSet<V_INV413_REG_DISP> V_INV413_REG_DISP { get; set; }
        public virtual DbSet<V_INV413_REG_SEL> V_INV413_REG_SEL { get; set; }
        public virtual DbSet<V_INV411_UBIC_DISP> V_INV411_UBIC_DISP { get; set; }
        public virtual DbSet<V_INV411_UBIC_SEL> V_INV411_UBIC_SEL { get; set; }
        public virtual DbSet<V_INV416_REG_DISP> V_INV416_REG_DISP { get; set; }
        public virtual DbSet<V_INV416_REG_SEL> V_INV416_REG_SEL { get; set; }
        public virtual DbSet<V_INV417_REG_DISP> V_INV417_REG_DISP { get; set; }
        public virtual DbSet<V_INV417_REG_SEL> V_INV417_REG_SEL { get; set; }
        public virtual DbSet<V_INV418_ATRIBUTOS> V_INV418_ATRIBUTOS { get; set; }

        public virtual DbSet<V_REPORTE_PACKING_LIST_SIN_LPN> V_REPORTE_PACKING_LIST_SIN_LPN { get; set; }
        public virtual DbSet<V_PRD111_STOCK_PRODUCCION> V_PRD111_STOCK_PRODUCCION { get; set; }
        public virtual DbSet<V_REG700_UBIC_SIN_RECORRIDOS> V_REG700_UBIC_SIN_RECORRIDOS { get; set; }

        public virtual DbSet<V_REG700_APLICACION_ASO> V_REG700_APLICACION_ASO { get; set; }
        public virtual DbSet<V_REG700_APLICACION_DISP> V_REG700_APLICACION_DISP { get; set; }
        public virtual DbSet<V_REG700_APLICACION_USER_DISP> V_REG700_APLICACION_USER_DISP { get; set; }
        public virtual DbSet<V_REG700_APLICACION_USER_ASO> V_REG700_APLICACION_USER_ASO { get; set; }
        public virtual DbSet<V_APLICACION_RECORRIDO> V_APLICACION_RECORRIDO { get; set; }
        public virtual DbSet<V_APLICACION_RECORRIDO_USUARIO> V_APLICACION_RECORRIDO_USUARIO { get; set; }
        public virtual DbSet<V_REC500_FACTURA_AGENDA> V_REC500_FACTURA_AGENDA { get; set; }
        public virtual DbSet<V_REC400_FACTURAS> V_REC400_FACTURAS { get; set; }
        public virtual DbSet<V_REC500_DETALLE_POR_REFERENCIA> V_REC500_DETALLE_POR_REFERENCIA { get; set; }


        public virtual DbSet<V_COLA_TRABAJO_PONDERADOR_DET> V_COLA_TRABAJO_PONDERADOR_DET { get; set; }
        public virtual DbSet<V_COLA_TRABAJO_POND_EMPRESAS> V_COLA_TRABAJO_POND_EMPRESAS { get; set; }
        public virtual DbSet<V_COLA_TRABAJO_POND_CLIENTES> V_COLA_TRABAJO_POND_CLIENTES { get; set; }
        public virtual DbSet<V_COLA_TRABAJO_POND_RUTAS> V_COLA_TRABAJO_POND_RUTAS { get; set; }
        public virtual DbSet<V_COLA_TRABAJO_POND_ZONAS> V_COLA_TRABAJO_POND_ZONAS { get; set; }
        public virtual DbSet<V_COLA_TRABAJO_POND_TP_EXP> V_COLA_TRABAJO_POND_TP_EXP { get; set; }
        public virtual DbSet<V_COLA_TRABAJO_POND_TP_PED> V_COLA_TRABAJO_POND_TP_PED { get; set; }
        public virtual DbSet<V_COLA_TRABAJO_POND_CON_LIB> V_COLA_TRABAJO_POND_CON_LIB { get; set; }
        public virtual DbSet<V_SEG_COLA_TRABAJO_PRE812> V_SEG_COLA_TRABAJO_PRE812 { get; set; }
        public virtual DbSet<V_PRE811_PREF_CLASSE> V_PRE811_PREF_CLASSE { get; set; }
        public virtual DbSet<V_PRE811_PREF_COND_LIB> V_PRE811_PREF_COND_LIB { get; set; }
        public virtual DbSet<V_PRE811_PREF_EMPRESA> V_PRE811_PREF_EMPRESA { get; set; }
        public virtual DbSet<V_PRE811_PREF_FAMILIA> V_PRE811_PREF_FAMILIA { get; set; }
        public virtual DbSet<V_PRE811_PREF_ROTA> V_PRE811_PREF_ROTA { get; set; }
        public virtual DbSet<V_PRE811_PREF_TP_EXP> V_PRE811_PREF_TP_EXP { get; set; }
        public virtual DbSet<V_PRE811_PREF_TP_PEDIDO> V_PRE811_PREF_TP_PEDIDO { get; set; }
        public virtual DbSet<V_PRE811_PREF_ZONA> V_PRE811_PREF_ZONA { get; set; }
        public virtual DbSet<V_PRE811_PREFERENCIAS> V_PRE811_PREFERENCIAS { get; set; }
        public virtual DbSet<V_PRE811_PREF_CONT_ACCESO> V_PRE811_PREF_CONT_ACCESO { get; set; }
        public virtual DbSet<V_ZOOMIN_PRE812> V_ZOOMIN_PRE812 { get; set; }
        public virtual DbSet<V_CALCULO_PONDERACION_PRE812> V_CALCULO_PONDERACION_PRE812 { get; set; }
        public virtual DbSet<V_PRE811_PREF_EQUIPO> V_PRE811_PREF_EQUIPO { get; set; }
        public virtual DbSet<V_PRE811_PREF_USUARIO> V_PRE811_PREF_USUARIO { get; set; }
        public virtual DbSet<V_PRE811_PREF_CLIENTE> V_PRE811_PREF_CLIENTE { get; set; }
        public virtual DbSet<V_PRE052_STOCK_SUELTO> V_PRE052_STOCK_SUELTO { get; set; }
        public virtual DbSet<V_PRE052_STOCK_LPN> V_PRE052_STOCK_LPN { get; set; }
        public virtual DbSet<V_CLIENTE_CONFIG_DIAS_VALIDEZ> V_CLIENTE_CONFIG_DIAS_VALIDEZ { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(this._schema);

            //TODO: Sacar de proyecto, mover a servicio de backend
            modelBuilder.HasDbFunction(typeof(CustomDbFunctions).GetMethod(nameof(CustomDbFunctions.NullableDateToChar), new[] { typeof(DateTime?) }));
            modelBuilder.HasDbFunction(typeof(CustomDbFunctions).GetMethod(nameof(CustomDbFunctions.NullableDecimalToChar), new[] { typeof(decimal?) }));
            modelBuilder.HasDbFunction(typeof(CustomDbFunctions).GetMethod(nameof(CustomDbFunctions.NullableIntToChar), new[] { typeof(int?) }));
            modelBuilder.HasDbFunction(typeof(CustomDbFunctions).GetMethod(nameof(CustomDbFunctions.NullableShortToChar), new[] { typeof(short?) }));
            modelBuilder.HasDbFunction(typeof(CustomDbFunctions).GetMethod(nameof(CustomDbFunctions.NullableDoubleToChar), new[] { typeof(double?) }));
            modelBuilder.HasDbFunction(typeof(CustomDbFunctions).GetMethod(nameof(CustomDbFunctions.NullableLongToChar), new[] { typeof(long?) }));
            modelBuilder.HasDbFunction(typeof(CustomDbFunctions).GetMethod(nameof(CustomDbFunctions.IntToChar), new[] { typeof(int) }));
            modelBuilder.HasDbFunction(typeof(CustomDbFunctions).GetMethod(nameof(CustomDbFunctions.DecimalToChar), new[] { typeof(decimal) }));
            modelBuilder.HasDbFunction(typeof(CustomDbFunctions).GetMethod(nameof(CustomDbFunctions.ShortToChar), new[] { typeof(short) }));
            modelBuilder.HasDbFunction(typeof(CustomDbFunctions).GetMethod(nameof(CustomDbFunctions.DoubleToChar), new[] { typeof(double) }));
            modelBuilder.HasDbFunction(typeof(CustomDbFunctions).GetMethod(nameof(CustomDbFunctions.LongToChar), new[] { typeof(long) }));

            modelBuilder.Entity<DbSequence>().HasNoKey();

            #region Claves compuestas
            modelBuilder.Entity<V_REC500_DETALLE_POR_REFERENCIA>().HasKey(d => new { d.NU_RECEPCION_REFERENCIA });
            modelBuilder.Entity<I_E_ESTAN_AGENTE>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_REGISTRO });
            modelBuilder.Entity<I_E_ESTAN_CODIGO_BARRAS>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_REGISTRO, d.NU_REGISTRO_PADRE });
            modelBuilder.Entity<I_E_ESTAN_CONVERTEDOR>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_REGISTRO });
            modelBuilder.Entity<I_E_ESTAN_FACTURA_REC>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_REGISTRO });
            modelBuilder.Entity<I_E_ESTAN_FACTURA_REC_DET>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_REGISTRO });
            modelBuilder.Entity<I_E_ESTAN_PEDIDO_SAIDA>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_REGISTRO });
            modelBuilder.Entity<I_E_ESTAN_PEDIDO_SAIDA_DET>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_REGISTRO });
            modelBuilder.Entity<I_E_ESTAN_PRODUTO>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_REGISTRO });
            modelBuilder.Entity<I_E_ESTAN_REF_RECEPCION>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_REGISTRO });
            modelBuilder.Entity<I_E_ESTAN_REF_RECEPCION_DET>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_REGISTRO });
            modelBuilder.Entity<I_S_ESTAN_AJUSTE_STOCK>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_REGISTRO });
            modelBuilder.Entity<I_S_ESTAN_CONF_PEDI_CAMION>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_REGISTRO });
            modelBuilder.Entity<I_S_ESTAN_CONF_PEDI_PEDIDO>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_REGISTRO });
            modelBuilder.Entity<I_S_ESTAN_CONF_PEDI_PEDIDO_DET>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_REGISTRO });
            modelBuilder.Entity<I_S_ESTAN_PEDIDO_ANULADO>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_REGISTRO });
            modelBuilder.Entity<LT_DET_DOCUMENTO_EGRESO>().HasKey(d => new { d.NU_DOCUMENTO, d.TP_DOCUMENTO, d.NU_SECUENCIA, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<T_ARCHIVO_ADJUNTO>().HasKey(d => new { d.NU_ARCHIVO_ADJUNTO, d.CD_EMPRESA, d.CD_MANEJO, d.DS_REFERENCIA });
            modelBuilder.Entity<T_ARCHIVO_ADJUNTO_VERSION>().HasKey(d => new { d.NU_ARCHIVO_ADJUNTO, d.CD_EMPRESA, d.CD_MANEJO, d.DS_REFERENCIA, d.NU_VERSION });
            modelBuilder.Entity<T_ARCHIVO_MANEJO_DOCUMENTO>().HasKey(d => new { d.CD_MANEJO, d.CD_DOCUMENTO });
            modelBuilder.Entity<T_CLIENTE>().HasKey(d => new { d.CD_EMPRESA, d.CD_CLIENTE });
            modelBuilder.Entity<T_CLIENTE_CAMION>().HasKey(d => new { d.CD_CAMION, d.NU_CARGA, d.CD_CLIENTE });
            modelBuilder.Entity<T_CLIENTE_RUTA_PREDIO>().HasKey(d => new { d.CD_EMPRESA, d.CD_CLIENTE, d.NU_PREDIO });
            modelBuilder.Entity<T_CODIGO_BARRAS>().HasKey(d => new { d.CD_BARRAS, d.CD_EMPRESA });
            modelBuilder.Entity<T_ARCHIVO>().HasKey(d => new { d.CD_ARCHIVO });
            modelBuilder.Entity<T_CONTACTO_GRUPO_REL>().HasKey(d => new { d.NU_CONTACTO_GRUPO, d.NU_CONTACTO });
            modelBuilder.Entity<T_CONTAINER>().HasKey(d => new { d.NU_CONTAINER, d.NU_SEQ_CONTAINER });
            modelBuilder.Entity<T_CONTENEDOR>().HasKey(d => new { d.NU_PREPARACION, d.NU_CONTENEDOR });
            modelBuilder.Entity<T_CONTENEDORES_PREDEFINIDOS>().HasKey(d => new { d.NU_CONTENEDOR, d.TP_CONTENEDOR });
            modelBuilder.Entity<T_CROSS_DOCK>().HasKey(d => new { d.NU_AGENDA, d.NU_PREPARACION });
            modelBuilder.Entity<T_CTR_CALIDAD_NECESARIO>().HasKey(d => new { d.CD_CONTROL, d.CD_EMPRESA, d.CD_PRODUTO });
            modelBuilder.Entity<T_DET_AGENDA>().HasKey(d => new { d.NU_AGENDA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.CD_FAIXA, d.CD_EMPRESA });
            modelBuilder.Entity<T_DET_CROSS_DOCK>().HasKey(d => new { d.NU_AGENDA, d.NU_PREPARACION, d.CD_CLIENTE, d.NU_PEDIDO, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.CD_EMPRESA, d.NU_PREPARACION_PICKEO });
            modelBuilder.Entity<T_DET_DOCUMENTO>().HasKey(d => new { d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.CD_EMPRESA, d.NU_DOCUMENTO, d.TP_DOCUMENTO });
            modelBuilder.Entity<V_TODO_ASIGNADO_CAMION>().HasKey(d => new { d.NU_PEDIDO, d.CD_EMPRESA, d.CD_CLIENTE, d.NU_SEQ_PREPARACION });
            modelBuilder.Entity<V_PRE052_PEDIDOS_ASOCIADOS>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA });
            modelBuilder.Entity<V_PRE052_PEDIDOS_DISPONIBLES>().HasKey(d => new { d.CD_EMPRESA, d.NU_PEDIDO, d.CD_CLIENTE });
            modelBuilder.Entity<V_REC300_MOTIVO_ALMACENAMIENTO>().HasKey(d => new { d.NU_MOTIVO_ALMACENAMIENTO, d.CD_EMPRESA, d.CD_PRODUTO, d.NU_ETIQUETA_LOTE });
            modelBuilder.Entity<V_PRE120_ANULACION_PREPARACION>().HasKey(d => new { d.CD_PRODUTO, d.CD_EMPRESA, d.NU_IDENTIFICADOR, d.CD_FAIXA, d.NU_PREPARACION, d.NU_PEDIDO, d.CD_CLIENTE, d.CD_ENDERECO, d.NU_SEQ_PREPARACION });
            modelBuilder.Entity<V_INT070_INT_EJECUCION_ERROR>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_ERROR });
            modelBuilder.Entity<V_POTERIA_VEHICULO_AGENDA>().HasKey(d => new { d.NU_PORTERIA_VEHICULO, d.NU_PORTERIA_VEHICULO_AGENDA, d.NU_AGENDA });
            modelBuilder.Entity<V_POTERIA_VEHICULO_CAMION>().HasKey(d => new { d.NU_PORTERIA_VEHICULO, d.NU_PORTERIA_VEHICULO_CAMION, d.CD_CAMION });
            modelBuilder.Entity<V_ARCHIVOS>().HasKey(d => new { d.CD_ARCHIVO });
            modelBuilder.Entity<T_DET_DOCUMENTO_ACTA>().HasKey(d => new { d.NU_DOCUMENTO, d.TP_DOCUMENTO, d.NU_ACTA, d.TP_ACTA });
            modelBuilder.Entity<T_DET_DOCUMENTO_EGRESO>().HasKey(d => new { d.NU_DOCUMENTO, d.TP_DOCUMENTO, d.NU_SECUENCIA });
            modelBuilder.Entity<T_DET_ETIQUETA_LOTE>().HasKey(d => new { d.NU_ETIQUETA_LOTE, d.CD_PRODUTO, d.CD_FAIXA, d.CD_EMPRESA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<T_ETIQUETAS_EN_USO>().HasKey(d => new { d.NU_EXTERNO_ETIQUETA, d.TP_ETIQUETA });
            modelBuilder.Entity<T_DET_IMPRESION>().HasKey(d => new { d.NU_IMPRESION, d.NU_REGISTRO });
            modelBuilder.Entity<T_DET_PEDIDO_EXPEDIDO>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.CD_CAMION, d.ID_ESPECIFICA_IDENTIFICADOR });
            modelBuilder.Entity<T_DET_PEDIDO_SAIDA>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR });
            modelBuilder.Entity<T_DET_PEDIDO_SAIDA_DUP>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR, d.ID_LINEA_SISTEMA_EXTERNO });
            modelBuilder.Entity<T_DET_PICKING>().HasKey(d => new { d.NU_PREPARACION, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.CD_EMPRESA, d.CD_ENDERECO, d.NU_PEDIDO, d.CD_CLIENTE, d.NU_SEQ_PREPARACION });
            modelBuilder.Entity<T_DOCUMENTO>().HasKey(d => new { d.NU_DOCUMENTO, d.TP_DOCUMENTO });
            modelBuilder.Entity<T_PRODUTO_PALLET>().HasKey(d => new { d.CD_PRODUTO, d.CD_FAIXA, d.CD_EMPRESA, d.CD_PALLET });
            modelBuilder.Entity<T_TIPO_ENDERECO_PALLET>().HasKey(d => new { d.CD_TIPO_ENDERECO, d.CD_PALLET });
            modelBuilder.Entity<T_DOCUMENTO_PREPARACION_RESERV>().HasKey(d => new { d.NU_DOCUMENTO, d.TP_DOCUMENTO, d.NU_PREPARACION, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR_PICKING_DET, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<LT_DELETE_DOCUMENTO_PREPARACION_RESERV>().HasKey(d => new { d.NU_DOCUMENTO, d.TP_DOCUMENTO, d.NU_PREPARACION, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR_PICKING_DET, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<T_DOCUMENTO_PRODUCCION>().HasKey(d => new { d.NU_DOCUMENTO_EGR, d.TP_DOCUMENTO_EGR, d.NU_DOCUMENTO_ING, d.TP_DOCUMENTO_ING, d.NU_PRDC_INGRESO });
            modelBuilder.Entity<T_DOCUMENTO_TRANSFERENCIA>().HasKey(d => new { d.NU_DOCUMENTO_EGR, d.TP_DOCUMENTO_EGR, d.NU_DOCUMENTO_ING, d.TP_DOCUMENTO_ING, d.NU_TRANSFERENCIA });
            modelBuilder.Entity<T_DOCUMENTO_SEL_TP_LIBERACION>().HasKey(d => new { d.NU_PREPARACION, d.TP_DOCUMENTO });
            modelBuilder.Entity<T_DOCUMENTO_LIBERACION>().HasKey(d => new { d.NU_PREPARACION, d.TP_DOCUMENTO, d.NU_DOCUMENTO });
            modelBuilder.Entity<T_DOCUMENTO_TIPO>().HasKey(d => new { d.TP_DOCUMENTO });
            modelBuilder.Entity<T_DOCUMENTO_TIPO_EDITABLE_DET>().HasKey(d => new { d.TP_DOCUMENTO, d.NM_DATAFIELD });
            modelBuilder.Entity<V_STO710_LT_LPN_ATRIBUTO_DETALLE>().HasKey(d => new { d.NU_LOG_SECUENCIA });
            modelBuilder.Entity<T_EMPRESA_FUNCIONARIO>().HasKey(d => new { d.USERID, d.CD_EMPRESA });
            modelBuilder.Entity<T_EVENTO_INSTANCIA_CONTACTO>().HasKey(d => new { d.NU_INSTANCIA, d.NU_CONTACTO });
            modelBuilder.Entity<T_EVENTO_NOTIFICACION_ARCHIVO>().HasKey(d => new { d.NU_EVENTO_NOTIFICACION_ARCHIVO, d.NU_EVENTO_NOTIFICACION });
            modelBuilder.Entity<T_EVENTO_PARAMETRO>().HasKey(d => new { d.CD_EVENTO_PARAMETRO, d.NU_EVENTO, d.TP_NOTIFICACION });
            modelBuilder.Entity<T_EVENTO_PARAMETRO_INSTANCIA>().HasKey(d => new { d.CD_EVENTO_PARAMETRO, d.NU_EVENTO_INSTANCIA, d.NU_EVENTO, d.TP_NOTIFICACION });
            modelBuilder.Entity<T_EVENTO_TEMPLATE>().HasKey(d => new { d.CD_LABEL_ESTILO, d.TP_NOTIFICACION, d.NU_EVENTO });
            modelBuilder.Entity<T_FACTURACION_CODIGO_COMPONEN>().HasKey(d => new { d.CD_FACTURACION, d.NU_COMPONENTE });
            modelBuilder.Entity<V_ORT_INSUMOS_WORT020>().HasKey(d => new { d.CD_INSUMO_MANIPULEO });
            modelBuilder.Entity<V_ORT_ORDEN_WORT030>().HasKey(d => new { d.NU_ORT_ORDEN });
            modelBuilder.Entity<T_PRODUTO_CONVERTOR>().HasKey(d => new { d.CD_PRODUTO, d.CD_CLIENTE, d.CD_EMPRESA });
            modelBuilder.Entity<T_FACTURACION_LISTA_COTIZACION>().HasKey(d => new { d.CD_FACTURACION, d.NU_COMPONENTE, d.CD_LISTA_PRECIO });
            modelBuilder.Entity<T_GRID_DEFAULT_CONFIG>().HasKey(d => new { d.CD_APLICACION, d.CD_BLOQUE, d.NM_DATAFIELD });
            modelBuilder.Entity<T_GRID_FILTER_DET>().HasKey(d => new { d.CD_FILTRO, d.CD_COLUMNA });
            modelBuilder.Entity<T_GRID_USER_CONFIG>().HasKey(d => new { d.CD_APLICACION, d.CD_BLOQUE, d.NM_DATAFIELD, d.USERID });
            modelBuilder.Entity<T_GRUPO_CONSULTA_FUNCIONARIO>().HasKey(d => new { d.USERID, d.CD_GRUPO_CONSULTA });
            modelBuilder.Entity<T_IMPRESORA>().HasKey(d => new { d.CD_IMPRESORA, d.NU_PREDIO });
            modelBuilder.Entity<V_STO721_HISTORIAL_CABEZAL>().HasKey(d => new { d.NU_LOG_SECUENCIA });
            modelBuilder.Entity<T_INTERFAZ_EJECUCION_DATEXTDET>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_PAQUETE });
            modelBuilder.Entity<T_INTERFAZ_EJECUCION_ERROR>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_ERROR });
            modelBuilder.Entity<T_LABEL_TEMPLATE>().HasKey(d => new { d.CD_LABEL_ESTILO, d.CD_LENGUAJE_IMPRESION });
            modelBuilder.Entity<T_LOCALIZACION>().HasKey(d => new { d.CD_APLICACION, d.CD_BLOQUE, d.CD_TIPO, d.CD_CLAVE, d.CD_IDIOMA });
            modelBuilder.Entity<T_LPARAMETRO_NIVEL>().HasKey(d => new { d.CD_PARAMETRO, d.DO_ENTIDAD_PARAMETRIZABLE });
            modelBuilder.Entity<T_PALLET_TRANSFERENCIA>().HasKey(d => new { d.NU_ETIQUETA, d.NU_SEC_ETIQUETA });
            modelBuilder.Entity<T_DET_PALLET_TRANSFERENCIA>().HasKey(d => new { d.NU_ETIQUETA, d.NU_SEC_ETIQUETA, d.NU_SEC_DETALLE, d.CD_ENDERECO_ORIGEN, d.NU_IDENTIFICADOR, d.CD_PRODUTO, d.CD_FAIXA, d.CD_EMPRESA });
            modelBuilder.Entity<T_PARAM>().HasKey(d => new { d.CD_APLICACAO, d.TP_APLICACAO, d.CD_PARAMETRO });
            modelBuilder.Entity<T_PEDIDO_SAIDA>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA });
            modelBuilder.Entity<T_PRDC_CONFIGURAR_PASADA>().HasKey(d => new { d.CD_PRDC_DEFINICION, d.CD_ACCION_INSTANCIA });
            modelBuilder.Entity<T_PRDC_CONSUMO_IDENTIFICADOR>().HasKey(d => new { d.CD_ENDERECO, d.CD_PRODUTO, d.CD_FAIXA, d.CD_EMPRESA, d.NU_ORDEN });
            modelBuilder.Entity<T_PRDC_DET_ENTRADA>().HasKey(d => new { d.CD_PRDC_DEFINICION, d.CD_COMPONENTE, d.NU_PRIORIDAD });
            modelBuilder.Entity<T_PRDC_DET_SALIDA>().HasKey(d => new { d.CD_PRDC_DEFINICION, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA });
            modelBuilder.Entity<T_PRDC_EGRESO_IDENTIFICADOR>().HasKey(d => new { d.CD_ENDERECO, d.CD_PRODUTO, d.CD_FAIXA, d.CD_EMPRESA, d.NU_ORDEN });
            modelBuilder.Entity<T_PRDC_INGRESO_DOCUMENTO>().HasKey(d => new { d.NU_DOCUMENTO_EGR, d.TP_DOCUMENTO_EGR, d.NU_DOCUMENTO_ING, d.TP_DOCUMENTO_ING, d.NU_PRDC_INGRESO });
            modelBuilder.Entity<T_PRDC_INGRESO_PASADA>().HasKey(d => new { d.NU_PRDC_INGRESO, d.QT_PASADAS, d.NU_ORDEN });
            modelBuilder.Entity<T_PRDC_LINEA_CONSUMIDO>().HasKey(d => new { d.CD_PRDC_DEFINICION, d.NU_PRDC_INGRESO, d.NU_FORMULA, d.NU_PASADA, d.CD_PRODUTO, d.CD_FAIXA, d.CD_EMPRESA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<T_PRDC_LINEA_PRODUCIDO>().HasKey(d => new { d.CD_PRDC_DEFINICION, d.NU_PRDC_INGRESO, d.NU_FORMULA, d.NU_PASADA, d.CD_PRODUTO, d.CD_FAIXA, d.CD_EMPRESA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<T_PREDIO_USUARIO>().HasKey(d => new { d.NU_PREDIO, d.USERID });
            modelBuilder.Entity<T_PREDIO>().HasKey(d => new { d.NU_PREDIO });
            modelBuilder.Entity<T_PREP_NO_ANULAR>().HasKey(d => new { d.CD_EMPRESA, d.NU_PREPARACION, d.NU_PEDIDO, d.CD_CLIENTE });
            modelBuilder.Entity<T_PRODUTO>().HasKey(d => new { d.CD_EMPRESA, d.CD_PRODUTO });
            modelBuilder.Entity<T_PRODUTO_FAIXA>().HasKey(d => new { d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA });
            modelBuilder.Entity<T_ORT_INSUMO_MANIPULEO>().HasKey(d => new { d.CD_INSUMO_MANIPULEO });
            modelBuilder.Entity<T_ORT_INSUMO_MANIPULEO_EMPRESA>().HasKey(d => new { d.CD_INSUMO_MANIPULEO, d.CD_EMPRESA });
            modelBuilder.Entity<T_ORT_ORDEN>().HasKey(d => new { d.NU_ORT_ORDEN });
            modelBuilder.Entity<T_ORT_ORDEN_TAREA>().HasKey(d => new { d.NU_ORDEN_TAREA });
            modelBuilder.Entity<T_ORT_ORDEN_TAREA_FUNCIONARIO>().HasKey(d => new { d.NU_ORT_ORDEN_TAREA_FUNC });
            modelBuilder.Entity<T_ORT_ORDEN_TAREA_EQUIPO>().HasKey(d => new { d.NU_ORT_ORDEN_TAREA_EQUIPO });
            modelBuilder.Entity<T_ORT_ORDEN_SESION_EQUIPO>().HasKey(d => new { d.NU_ORT_ORDEN_SESION_EQUIPO });
            modelBuilder.Entity<T_ORT_ORDEN_SESION>().HasKey(d => new { d.NU_ORT_ORDEN_SESION });
            modelBuilder.Entity<T_RECEPCION_AGENDA_REFERENCIA>().HasKey(d => new { d.NU_AGENDA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.CD_FAIXA, d.CD_EMPRESA, d.NU_RECEPCION_REFERENCIA_DET });
            modelBuilder.Entity<T_RECEPCION_TIPO_REPORTE_DEF>().HasKey(d => new { d.CD_REPORTE, d.TP_RECEPCION });
            modelBuilder.Entity<T_RAMO_PRODUTO>().HasKey(d => new { d.CD_RAMO_PRODUTO });
            modelBuilder.Entity<T_REGLA_CLIENTES>().HasKey(d => new { d.NU_REGLA, d.CD_CLIENTE, d.CD_EMPRESA });
            modelBuilder.Entity<T_REGLA_CONDICION_LIBERACION>().HasKey(d => new { d.NU_REGLA, d.CD_CONDICION_LIBERACION });
            modelBuilder.Entity<T_REGLA_TIPO_EXPEDICION>().HasKey(d => new { d.NU_REGLA, d.TP_EXPEDICION });
            modelBuilder.Entity<T_REGLA_TIPO_PEDIDO>().HasKey(d => new { d.NU_REGLA, d.TP_PEDIDO });
            modelBuilder.Entity<T_STOCK>().HasKey(d => new { d.CD_ENDERECO, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<T_STOCK_ENVASE>().HasKey(d => new { d.ID_ENVASE, d.ND_TP_ENVASE });
            modelBuilder.Entity<T_TEMP_CONDICION_LIBERACION>().HasKey(d => new { d.CD_CONDICION_LIBERACION, d.NU_TEMP_CONDICION_LIBERACION });
            modelBuilder.Entity<T_TEMP_PEDIDO_MOSTRADOR>().HasKey(d => new { d.NU_PREPARACION, d.NU_CONTENEDOR, d.NU_PEDIDO, d.CD_EMPRESA, d.CD_CLIENTE, d.NU_CARGA });
            modelBuilder.Entity<T_FACTURACION_EJEC_EMPRESA>().HasKey(d => new { d.NU_EJECUCION, d.CD_EMPRESA, d.CD_PROCESO });
            modelBuilder.Entity<T_FACTURACION_EMPRESA_PROCESO>().HasKey(d => new { d.CD_EMPRESA, d.CD_PROCESO });
            modelBuilder.Entity<T_LOG_FACTURACION_COR_10>().HasKey(d => new { d.NU_EJECUCION, d.CD_EMPRESA, d.CD_PROCESO, d.NU_COMPONENTE, d.DT_FECHA, d.NU_PALLET, d.TP_RESULTADO });
            modelBuilder.Entity<T_LOG_FACTURACION_MOV_EG_BULTO>().HasKey(d => new { d.NU_EJECUCION, d.CD_EMPRESA, d.CD_PROCESO, d.NU_COMPONENTE, d.DT_FECHA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.TP_RESULTADO });
            modelBuilder.Entity<T_FACTURACION_EJECUCION>().HasKey(d => d.NU_EJECUCION);
            modelBuilder.Entity<T_FACTURACION_PROCESO>().HasKey(d => d.CD_PROCESO);
            modelBuilder.Entity<T_FACTURACION_LISTA_PRECIO>().HasKey(d => d.CD_LISTA_PRECIO);
            modelBuilder.Entity<T_LOG_FACTURACION_ALM_BULTO>().HasKey(d => new { d.NU_EJECUCION, d.CD_EMPRESA, d.CD_PROCESO, d.NU_COMPONENTE, d.DT_FECHA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.TP_RESULTADO });
            modelBuilder.Entity<T_LOG_FACTURACION_ALM_COR_04>().HasKey(d => new { d.NU_EJECUCION, d.CD_EMPRESA, d.CD_PROCESO, d.NU_COMPONENTE, d.DT_FECHA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.TP_RESULTADO });
            modelBuilder.Entity<T_FACTURACION_RESULTADO>().HasKey(d => new { d.NU_EJECUCION, d.CD_EMPRESA, d.CD_FACTURACION, d.NU_COMPONENTE });
            modelBuilder.Entity<T_FACTURACION_UND_MEDIDA_EMP>().HasKey(d => new { d.CD_UNIDADE_MEDIDA, d.CD_EMPRESA });
            modelBuilder.Entity<T_TIPO_DUA_DOCUMENTO>().HasKey(d => new { d.TP_DUA, d.TP_DOCUMENTO });
            modelBuilder.Entity<T_TIPO_REFERENCIA_EXTERNA_DOCUMENTO>().HasKey(d => new { d.TP_REFERENCIA_EXTERNA, d.TP_DOCUMENTO });
            modelBuilder.Entity<T_ORT_ORDEN_TAREA_DATO>().HasKey(d => new { d.NU_ORT_ORDEN_TAREA_DATO });
            modelBuilder.Entity<T_ESTACION_CLASIFICACION>().HasKey(d => new { d.CD_ESTACION });
            modelBuilder.Entity<T_DET_PICKING_LPN>().HasKey(d => new { d.NU_PREPARACION, d.ID_DET_PICKING_LPN });
            modelBuilder.Entity<T_DET_PEDIDO_SAIDA_ATRIB>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR, d.NU_DET_PED_SAI_ATRIB });
            modelBuilder.Entity<T_DET_PEDIDO_SAIDA_ATRIB_DET>().HasKey(d => new { d.NU_DET_PED_SAI_ATRIB, d.ID_ATRIBUTO, d.FL_CABEZAL });
            modelBuilder.Entity<T_DET_PEDIDO_SAIDA_LPN_ATRIB>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR, d.ID_LPN_EXTERNO, d.TP_LPN_TIPO, d.NU_DET_PED_SAI_ATRIB });

            modelBuilder.Entity<V_REC400_ESTACIONES_DE_CLASIFICACION>().HasKey(d => new { d.CD_ESTACION });
            modelBuilder.Entity<V_REC410_SUGERENCIAS>().HasKey(d => new { d.CD_EQUIPO, d.NU_POSICION });
            modelBuilder.Entity<V_STO710_LT_LPN_ATRIBUTO_CABEZAL>().HasKey(d => new { d.NU_LOG_SECUENCIA });
            modelBuilder.Entity<V_ORT_ORDEN_TAREA_WORT040>().HasKey(d => new { d.NU_ORDEN_TAREA });
            modelBuilder.Entity<V_AGENTE>().HasKey(d => new { d.CD_EMPRESA, d.CD_CLIENTE });
            modelBuilder.Entity<V_AGENTES_WREG220>().HasKey(d => new { d.CD_EMPRESA, d.CD_CLIENTE });
            modelBuilder.Entity<V_ANULACIONES_PENDIENTES>().HasKey(d => new { d.NU_PEDIDO, d.CD_EMPRESA, d.CD_CLIENTE, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.NU_PREPARACION });
            modelBuilder.Entity<V_ANULACIONES_PENDIENT_CAMION>().HasKey(d => new { d.CD_CAMION, d.CD_EMPRESA, d.CD_CLIENTE, d.NU_CARGA, d.NU_PEDIDO, d.NU_CONTENEDOR, d.CD_PRODUTO, d.NU_PREPARACION });
            modelBuilder.Entity<V_ARCHIVO_ADJUNTO>().HasKey(d => new { d.NU_ARCHIVO_ADJUNTO, d.CD_EMPRESA, d.CD_MANEJO, d.DS_REFERENCIA });
            modelBuilder.Entity<V_ARCHIVO_VERSION>().HasKey(d => new { d.NU_ARCHIVO_ADJUNTO, d.CD_EMPRESA, d.CD_MANEJO, d.DS_REFERENCIA, d.NU_VERSION });
            modelBuilder.Entity<V_CAMION_EXP050>().HasKey(d => new { d.CD_CAMION, d.CD_SITUACAO });
            modelBuilder.Entity<V_CANTIDAD_ENVIADA_EXP>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_CANT_CONTE_PUERTA_WEXP>().HasKey(d => new { d.CD_EMPRESA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.CD_CLIENTE, d.NU_PEDIDO });
            modelBuilder.Entity<V_CANT_PREPARADA_WEXP>().HasKey(d => new { d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.CD_CLIENTE, d.NU_PEDIDO, d.CD_CAMION });
            modelBuilder.Entity<V_CODIGO_BARRAS_WREG603>().HasKey(d => new { d.CD_EMPRESA, d.CD_PRODUTO, d.CD_BARRAS });
            modelBuilder.Entity<V_COF070_REPORTE>().HasKey(d => new { d.NU_REPORTE, d.NU_REPORTE_RELACION, d.CD_CLAVE, d.NM_TABLA, d.FULLNAME });
            modelBuilder.Entity<V_CONSULTA_PRE660>().HasKey(d => new { d.NU_PEDIDO, d.CD_EMPRESA, d.CD_CLIENTE });
            modelBuilder.Entity<V_CONTENEDOR_CON_PROBLEMA>().HasKey(d => new { d.NU_PREPARACION, d.NU_CONTENEDOR });
            modelBuilder.Entity<V_CONTENEDOR_PRECINTO>().HasKey(d => new { d.CD_CAMION, d.CD_CLIENTE, d.CD_EMPRESA, d.NU_PEDIDO, d.NU_CONTENEDOR });
            modelBuilder.Entity<V_CONT_CAMION_FACT>().HasKey(d => new { d.CD_CAMION, d.NU_CONTENEDOR, d.NU_PREPARACION });
            modelBuilder.Entity<V_CONT_PRODUTO_WSTO150DET>().HasKey(d => new { d.NU_CONTENEDOR, d.NU_PREPARACION, d.NU_PEDIDO });
            modelBuilder.Entity<V_CONT_SIN_EMBARCAR_WEXP041>().HasKey(d => new { d.NU_CONTENEDOR, d.CD_CLIENTE, d.CD_EMPRESA });
            modelBuilder.Entity<V_CON_SIN_FIN_CONT>().HasKey(d => new { d.CD_CAMION, d.NU_CARGA });
            modelBuilder.Entity<V_REC210_CROSS_DOCKING>().HasKey(d => new { d.NU_AGENDA, d.NU_PREPARACION, d.CD_CLIENTE, d.NU_PEDIDO, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.CD_EMPRESA, d.ID_ESPECIFICA_IDENTIFICADOR, d.NU_CARGA, d.NU_PREPARACION_PICKEO });
            modelBuilder.Entity<V_DETALLE_AGENDA_WREC171>().HasKey(d => new { d.NU_AGENDA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.CD_FAIXA, d.CD_EMPRESA });
            modelBuilder.Entity<V_DET_CONTENEDORES_WEXP330>().HasKey(d => new { d.NU_PREPARACION, d.NU_CONTENEDOR, d.CD_PRODUTO, d.CD_EMPRESA, d.NU_IDENTIFICADOR, d.NU_PEDIDO, d.CD_CLIENTE });
            modelBuilder.Entity<V_DET_CONT_CON_PROBLEMA>().HasKey(d => new { d.NU_SEQ_PREPARACION, d.NU_PREPARACION, d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.CD_FAIXA, d.CD_ENDERECO });
            modelBuilder.Entity<V_DET_DOCUMENTO_DOC081>().HasKey(d => new { d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.CD_FAIXA, d.CD_EMPRESA, d.NU_DOCUMENTO, d.TP_DOCUMENTO });
            modelBuilder.Entity<V_DET_DOCUMENTO_EGRESO>().HasKey(d => new { d.NU_SECUENCIA, d.TP_DOCUMENTO_EGRESO, d.NU_DOCUMENTO_EGRESO });
            modelBuilder.Entity<V_DET_DOCUMENTO_INGRESO>().HasKey(d => new { d.CD_EMPRESA, d.CD_FAIXA, d.CD_PRODUTO, d.NU_DOCUMENTO, d.NU_IDENTIFICADOR, d.TP_DOCUMENTO });
            modelBuilder.Entity<V_DET_DOC_DUA_DOC020>().HasKey(d => new { d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.CD_EMPRESA, d.NU_DOCUMENTO, d.TP_DOCUMENTO });
            modelBuilder.Entity<V_DET_DOC_DUA_DOC021>().HasKey(d => new { d.NU_DOCUMENTO_INGRESO_DUA, d.TP_DOCUMENTO_INGRESO_DUA, d.CD_EMPRESA, d.NU_DOCUMENTO, d.TP_DOCUMENTO, d.CD_NAM });
            modelBuilder.Entity<V_DET_PEDIDO_SAIDA_WPRE101>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR });
            modelBuilder.Entity<V_DET_PICKING_CAMION_WEXP040>().HasKey(d => new { d.CD_CAMION, d.NU_CARGA, d.CD_EMPRESA, d.NU_PEDIDO, d.CD_CLIENTE });
            modelBuilder.Entity<V_DET_PICKING_MOSTRADOR>().HasKey(d => new { d.NU_PREPARACION, d.NU_CONTENEDOR, d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA });
            modelBuilder.Entity<V_DET_PICKING_WSTO150DET>().HasKey(d => new { d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.CD_EMPRESA, d.CD_ENDERECO, d.NU_SEQ_PREPARACION, d.NU_PEDIDO, d.CD_CLIENTE });
            modelBuilder.Entity<V_DET_PREPARACION_WPRE061>().HasKey(d => new { d.NU_PREPARACION, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.CD_CLIENTE, d.CD_ENDERECO, d.NU_PEDIDO, d.NU_SEQ_PREPARACION });
            modelBuilder.Entity<V_DOCUMENTO_DOC080>().HasKey(d => new { d.NU_DOCUMENTO, d.TP_DOCUMENTO });
            modelBuilder.Entity<V_DOCUMENTO_DOC095>().HasKey(d => new { d.NU_DOCUMENTO, d.TP_DOCUMENTO });
            modelBuilder.Entity<V_DOCUMENTO_DOC260>().HasKey(d => new { d.TP_DOCUMENTO, d.NU_DOCUMENTO });
            modelBuilder.Entity<V_PED_PREP_CAMION_DOC_WEXP090>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.NU_PREPARACION, d.NU_CARGA, d.CD_CAMION });
            modelBuilder.Entity<V_FOTO_STOCK_WSTO310>().HasKey(d => new { d.NU_FOTO, d.NU_PREDIO, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_ORT_ORDEN_TAREA_FUNC_WORT060>().HasKey(d => new { d.NU_ORT_ORDEN_TAREA_FUNC });
            modelBuilder.Entity<V_ORT_ORDEN_TAREA_DATO_WORT070>().HasKey(d => new { d.NU_ORT_ORDEN_TAREA_DATO });
            modelBuilder.Entity<V_ORT080_ORDEN_TAREA_EQUIPO>().HasKey(d => new { d.NU_ORT_ORDEN_TAREA_EQUIPO });
            modelBuilder.Entity<V_DOCUMENTO_LINEA_DET_DOC082>().HasKey(d => new { d.NU_DOCUMENTO, d.TP_DOCUMENTO, d.CD_EMPRESA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.TP_DOCUMENTO_ASOCIADO, d.NU_DOCUMENTO_ASOCIADO });
            modelBuilder.Entity<V_DOCUMENTO_PROD_DOC290>().HasKey(d => new { d.NU_DOCUMENTO_EGR, d.TP_DOCUMENTO_EGR, d.NU_DOCUMENTO_ING, d.TP_DOCUMENTO_ING, d.NU_PRDC_INGRESO, d.NU_PEDIDO, d.CD_EMPRESA, d.NM_EMPRESA });
            modelBuilder.Entity<V_DOCUMENTO_RESERVA_DOC300>().HasKey(d => new { d.NU_DOCUMENTO, d.TP_DOCUMENTO, d.NU_PREPARACION, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR_PICKING_DET, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_ENDERECO_ESTOQUE>().HasKey(d => new { d.CD_ENDERECO, d.CD_PRODUTO, d.CD_FAIXA, d.CD_EMPRESA, d.NU_IDENTIFICADOR, d.CD_TIPO_ENDERECO, d.DS_PRODUTO, d.CD_CLASSE, d.CD_MERCADOLOGICO, d.CD_SITUACAO });
            modelBuilder.Entity<V_ENDERECO_ESTOQUE_WSTO150>().HasKey(d => new { d.CD_ENDERECO, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_ESTOQUE_STO005>().HasKey(d => new { d.CD_ENDERECO, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_ETIQUETAS_LOTE_WSTO150DET>().HasKey(d => new { d.NU_AGENDA, d.NU_ETIQUETA_LOTE, d.NU_EXTERNO_ETIQUETA, d.TP_ETIQUETA, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_ETIQUETAS_WREC150>().HasKey(d => new { d.NU_AGENDA, d.NU_ETIQUETA_LOTE, d.NU_EXTERNO_ETIQUETA, d.TP_ETIQUETA, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_ETIQUETA_LOTE_DET_WREC170>().HasKey(d => new { d.NU_ETIQUETA_LOTE, d.NU_AGENDA, d.NU_EXTERNO_ETIQUETA, d.TP_ETIQUETA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.CD_FAIXA, d.CD_EMPRESA });
            modelBuilder.Entity<V_EVENTO_PARAM_INS_WEVT040>().HasKey(d => new { d.CD_EVENTO_PARAMETRO, d.NU_EVENTO_INSTANCIA, d.TP_NOTIFICACION, d.NU_EVENTO });
            modelBuilder.Entity<V_EVENTO_SALDO_FAC>().HasKey(d => new { d.NU_AGENDA, d.CD_PRODUTO });
            modelBuilder.Entity<V_EVENTO_SALDO_REF>().HasKey(d => new { d.NU_REFERENCIA, d.NU_AGENDA });
            modelBuilder.Entity<V_EVT060_TEMPLATES_NOTIFICACION>().HasKey(d => new { d.NU_EVENTO, d.TP_NOTIFICACION, d.CD_LABEL_ESTILO });
            modelBuilder.Entity<V_EXP010_CARGA_CAMION>().HasKey(d => new { d.CD_CLIENTE, d.CD_EMPRESA, d.NU_PREPARACION, d.NU_CARGA });
            modelBuilder.Entity<V_EXP011_CONTENEDOR_CAMION>().HasKey(d => new { d.CD_CLIENTE, d.CD_EMPRESA, d.NU_PREPARACION, d.NU_CARGA, d.NU_CONTENEDOR });
            modelBuilder.Entity<V_EXP013_PEDIDO_CAMION>().HasKey(d => new { d.CD_EMPRESA, d.CD_CLIENTE, d.NU_PEDIDO });
            modelBuilder.Entity<V_EXP041_PEDIDOS_EXPEDIDOS>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR, d.CD_CAMION });
            modelBuilder.Entity<V_EXP043_PEDIDOS_PEND_CAMION>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA });
            modelBuilder.Entity<V_EXP045_CONT_SIN_EMBARCAR>().HasKey(d => new { d.NU_CONTENEDOR, d.CD_CLIENTE, d.CD_EMPRESA });
            modelBuilder.Entity<V_EXP045_PRODUCTOS>().HasKey(d => new { d.DT_PICKEO, d.CD_CAMION, d.NU_CARGA, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.NU_PEDIDO, d.NU_PREPARACION, d.CD_FAIXA });
            modelBuilder.Entity<V_EXP045_PRODUCTOS_SIN_PREP>().HasKey(d => new { d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.CD_FAIXA, d.CD_EMPRESA, d.NU_PREPARACION, d.CD_CAMION, d.CD_CLIENTE });
            modelBuilder.Entity<V_EXP051_PRODS_PROBLEMAS>().HasKey(d => new { d.NU_PEDIDO, d.CD_PRODUTO, d.CD_EMPRESA, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR, d.CD_CLIENTE });
            modelBuilder.Entity<V_IMP110_LABEL_ESTILO>().HasKey(d => new { d.CD_LABEL_ESTILO, d.TP_CONTENEDOR });
            modelBuilder.Entity<V_INT100_ESTAN_PEDIDOS_SAIDA>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_REGISTRO });
            modelBuilder.Entity<V_INT100_ESTAN_PEDID_SAIDA_DET>().HasKey(d => new { d.NU_REGISTRO, d.NU_INTERFAZ_EJECUCION });
            modelBuilder.Entity<V_INT101_ESTAN_PRODUCTOS>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_REGISTRO });
            modelBuilder.Entity<V_INT102_ESTAN_CODIGO_BARRAS>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_REGISTRO });
            modelBuilder.Entity<V_INT103_ESTAN_REF_RECEPCION>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_REGISTRO });
            modelBuilder.Entity<V_INT103_ESTAN_REF_RECEPC_DET>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_REGISTRO });
            modelBuilder.Entity<V_INT104_ESTAN_PEDIDO_ANULADO>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_REGISTRO });
            modelBuilder.Entity<V_INT105_ESTAN_AJUSTE_STOCK>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_REGISTRO });
            modelBuilder.Entity<V_INT106_ESTAN_AGENTE>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_REGISTRO });
            modelBuilder.Entity<V_INT107_ESTAN_CONF_PED_PEDIDO>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_REGISTRO });
            modelBuilder.Entity<V_INT107_ESTAN_CONF_PEDI_DET>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_REGISTRO });
            modelBuilder.Entity<V_INT108_ESTAN_CONVERTEDOR>().HasKey(d => new { d.NU_REGISTRO, d.NU_INTERFAZ_EJECUCION });
            modelBuilder.Entity<V_INT109_ESTAN_FACTURA_REC>().HasKey(d => new { d.NU_REGISTRO, d.NU_INTERFAZ_EJECUCION });
            modelBuilder.Entity<V_INT109_ESTAN_FACTURA_REC_DET>().HasKey(d => new { d.NU_REGISTRO, d.NU_INTERFAZ_EJECUCION });
            modelBuilder.Entity<V_INV100_ANALISIS_INVENTARIO>().HasKey(d => new { d.NU_INVENTARIO, d.NU_INVENTARIO_ENDERECO, d.NU_INVENTARIO_ENDERECO_DET });
            modelBuilder.Entity<V_INVENTARIO_ENDE_DET>().HasKey(d => new { d.NU_INVENTARIO, d.NU_INVENTARIO_ENDERECO, d.NU_INVENTARIO_ENDERECO_DET });
            modelBuilder.Entity<V_INV413_REG_SEL>().HasKey(d => new { d.NU_INVENTARIO, d.NU_INVENTARIO_ENDERECO, d.NU_INVENTARIO_ENDERECO_DET });
            modelBuilder.Entity<V_INV413_REG_DISP>().HasKey(d => new { d.CD_ENDERECO, d.CD_PRODUTO, d.CD_EMPRESA, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_LIBERACION_AUTOMATICA_PEND>().HasKey(d => new { d.NU_REGLA });
            modelBuilder.Entity<V_LIMP010DET_IMPRESION>().HasKey(d => new { d.NU_IMPRESION, d.NU_REGISTRO });
            modelBuilder.Entity<V_LOG_DOCUMENTO>().HasKey(d => new { d.NU_LOG_DOCUMENTO, d.NM_FUNCIONARIO });
            modelBuilder.Entity<V_LOG_ETIQUETAS_WREC180>().HasKey(d => new { d.NU_AGENDA, d.NU_ETIQUETA_LOTE, d.TP_ETIQUETA, d.NU_EXTERNO_ETIQUETA, d.NU_LOG_ETIQUETA });
            modelBuilder.Entity<V_LT_DET_DOCUMENTO>().HasKey(d => new { d.NU_LOG_SECUENCIA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.CD_EMPRESA, d.NU_DOCUMENTO });
            modelBuilder.Entity<V_LT_DET_DOCUMENTO_EGRESO>().HasKey(d => new { d.NU_DOCUMENTO, d.TP_DOCUMENTO, d.NU_SECUENCIA, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_LT_DOCUMENTO>().HasKey(d => new { d.NU_LOG_SECUENCIA, d.NM_FUNCIONARIO });

            modelBuilder.Entity<V_ONDA>().HasKey(d => new { d.CD_ONDA, d.CD_EMPRESA });
            modelBuilder.Entity<V_PALLET_TRANSFERENCIA_WSTO498>().HasKey(d => new { d.NU_ETIQUETA, d.NU_SEC_ETIQUETA, d.CD_ENDERECO_ORIGEN, d.CD_FAIXA, d.CD_EMPRESA, d.NU_IDENTIFICADOR, d.CD_PRODUTO, d.NU_SEC_DETALLE });
            modelBuilder.Entity<V_PALLET_TRANSF_WSTO150DET>().HasKey(d => new { d.NU_ETIQUETA, d.NU_SEC_ETIQUETA, d.CD_PRODUTO, d.CD_EMPRESA });
            modelBuilder.Entity<V_PEDIDOS_CAMION_EXP050>().HasKey(d => new { d.CD_CAMION, d.CD_CLIENTE, d.CD_EMPRESA, d.NU_PEDIDO });
            modelBuilder.Entity<V_PEDIDOS_COMPATIBLES_WPRE051>().HasKey(d => new { d.NU_PEDIDO_AUTO, d.CD_PRODUTO, d.NU_PEDIDO_ESPE, d.CD_CLIENTE_ESPE, d.CD_EMPRESA, d.CD_EMPRESA_ESPECIFICADO });
            modelBuilder.Entity<V_REC200_PEDIDOS_CROSS_DOCK>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.NU_AGENDA });
            modelBuilder.Entity<V_PEDIDOS_EXPEDIDOS_EXEL>().HasKey(d => new { d.CD_CAMION, d.NU_PEDIDO, d.CD_EMPRESA, d.CD_CLIENTE, d.CD_PRODUTO, d.CD_ROTA, d.CD_TRANSPORTADORA });
            modelBuilder.Entity<V_PEDIDOS_PENDIENTES>().HasKey(d => new { d.NU_PEDIDO, d.CD_EMPRESA, d.CD_CLIENTE, d.CD_ROTA, d.NU_PREDIO });
            modelBuilder.Entity<V_PEDIDO_SAIDA_WPRE100>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA });
            modelBuilder.Entity<V_PRE080_ANALISIS_RECHAZO>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.NU_PREPARACION, d.CD_PRODUTO, d.CD_EMPRESA, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR });
            modelBuilder.Entity<V_PEND_LIB_PRE050>().HasKey(d => new { d.CD_EMPRESA, d.NU_PEDIDO, d.CD_CLIENTE });
            modelBuilder.Entity<V_PICKING_PENDIENTE_WPRE160>().HasKey(d => new { d.CD_EMPRESA, d.NU_PREDIO });
            modelBuilder.Entity<V_PICKING_PENDIENTE_WPRE161>().HasKey(d => new { d.CD_EMPRESA, d.NU_PREPARACION });
            modelBuilder.Entity<V_PICKING_PENDIENTE_WPRE162>().HasKey(d => new { d.CD_EMPRESA, d.NU_PREPARACION, d.CD_CLIENTE, d.NU_PEDIDO });
            modelBuilder.Entity<V_PLANIFICACION_LIB_WPRE670>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_PRDC_CONFIG_PASADA_KIT102>().HasKey(d => new { d.CD_PRDC_DEFINICION, d.CD_ACCION_INSTANCIA });
            modelBuilder.Entity<V_PRDC_CONSUMIDO_BB_KIT151>().HasKey(d => new { d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.CD_EMPRESA, d.CD_FAIXA });
            modelBuilder.Entity<V_PRDC_DEFINICION_KIT100>().HasKey(d => new { d.CD_PRDC_DEFINICION, d.CD_EMPRESA });
            modelBuilder.Entity<V_PRDC_DET_ENTRADA_KIT101>().HasKey(d => new { d.CD_PRDC_DEFINICION, d.CD_COMPONENTE, d.NU_PRIORIDAD });
            modelBuilder.Entity<V_PRDC_DET_ENTRADA_KIT151>().HasKey(d => new { d.CD_COMPONENTE, d.NU_PRIORIDAD, d.CD_PRDC_DEFINICION });
            modelBuilder.Entity<V_PRDC_DET_SALIDA_KIT101>().HasKey(d => new { d.CD_PRDC_DEFINICION, d.CD_PRODUTO });
            modelBuilder.Entity<V_PRDC_DET_SALIDA_KIT151>().HasKey(d => new { d.CD_PRDC_DEFINICION, d.CD_PRODUTO });
            modelBuilder.Entity<V_PRDC_EGR_IDENT_KIT191>().HasKey(d => new { d.CD_ENDERECO, d.CD_PRODUTO, d.CD_FAIXA, d.CD_EMPRESA, d.NU_ORDEN });
            modelBuilder.Entity<V_PRDC_INGRESO_PASADA_KIT180>().HasKey(d => new { d.NU_PRDC_INGRESO, d.QT_PASADAS, d.NU_ORDEN });
            modelBuilder.Entity<V_PRDC_PRODUCIDO_BB_KIT151>().HasKey(d => new { d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.CD_EMPRESA, d.CD_FAIXA });
            modelBuilder.Entity<V_PRE050_PEND_LIB>().HasKey(d => new { d.CD_EMPRESA, d.NU_PEDIDO, d.CD_CLIENTE });
            modelBuilder.Entity<V_PRE051_PEDIDOS_COMPATIBLES>().HasKey(d => new { d.NU_PEDIDO_AUTO, d.CD_PRODUTO, d.NU_PEDIDO_ESPE, d.CD_CLIENTE_ESPE, d.CD_EMPRESA, d.CD_EMPRESA_ESPECIFICADO });
            modelBuilder.Entity<V_PRE060_CONTENEDOR>().HasKey(d => new { d.NU_PREPARACION, d.NU_CONTENEDOR, d.TP_CONTENEDOR, d.NU_PREDIO });
            modelBuilder.Entity<V_PRE061_DET_PREPARACION>().HasKey(d => new { d.NU_PREPARACION, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.CD_CLIENTE, d.CD_ENDERECO, d.NU_PEDIDO, d.NU_SEQ_PREPARACION });
            modelBuilder.Entity<V_PRE100_PEDIDO_SAIDA>().HasKey(d => new { d.CD_CLIENTE, d.CD_EMPRESA, d.NU_PEDIDO });
            modelBuilder.Entity<V_PRE101_DET_PEDIDO_SAIDA>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_PRE110_DET_PEDIDO_SALIDA>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR });
            modelBuilder.Entity<V_PRE130_DET_PICKING>().HasKey(d => new { d.NU_PREPARACION, d.NU_PEDIDO, d.CD_CLIENTE, d.NU_SEQ_PREPARACION, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.CD_ENDERECO });
            modelBuilder.Entity<V_PRE150_DETALLE_PEDIDO>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR });
            modelBuilder.Entity<V_PRE150_DET_PEDIDO_SALIDA>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR });
            modelBuilder.Entity<V_PRE152_DETALLE_PEDIDO_LPN>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR, d.ID_LPN_EXTERNO, d.TP_LPN_TIPO });
            modelBuilder.Entity<V_PRE153_ATRIBUTO_LPN_DETALLE_PEDIDO>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR, d.ID_LPN_EXTERNO, d.TP_LPN_TIPO, d.NU_DET_PED_SAI_ATRIB });
            modelBuilder.Entity<V_PRE154_DETALLE_ATRIBUTOS_LPN_DETALLE_PEDIDO>().HasKey(d => new { d.ID_LPN_EXTERNO, d.NU_DET_PED_SAI_ATRIB, d.ID_ATRIBUTO, d.FL_CABEZAL });
            modelBuilder.Entity<V_PRE155_ATRIBUTOS_DE_DETALLE_DE_PEDIDO>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR, d.NU_DET_PED_SAI_ATRIB });
            modelBuilder.Entity<V_PRE156_DETALLE_ATRIBUTOS_DE_DETALLE_PEDIDO>().HasKey(d => new { d.NU_DET_PED_SAI_ATRIB, d.ID_ATRIBUTO, d.FL_CABEZAL });
            modelBuilder.Entity<V_PRE160_PICKING_PENDIENTE>().HasKey(d => new { d.CD_EMPRESA, d.NM_EMPRESA, d.NU_PREDIO });
            modelBuilder.Entity<V_PRE161_PICKING_PENDIENTE>().HasKey(d => new { d.CD_EMPRESA, d.NU_PREPARACION });
            modelBuilder.Entity<V_PRE162_PICKING_PENDIENTE>().HasKey(d => new { d.CD_EMPRESA, d.NU_PREPARACION, d.CD_CLIENTE, d.NU_PEDIDO });
            modelBuilder.Entity<V_PRE170_ANALISIS_RECHAZO>().HasKey(d => new { d.NU_PREPARACION, d.CD_EMPRESA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR });
            modelBuilder.Entity<V_PRE170_ANALISIS_RECHAZO_LPN>().HasKey(d => new { d.NU_ANALISIS_RECHAZO, d.NU_PREPARACION, d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR });
            modelBuilder.Entity<V_PRE350_STOCK_PICKING_REABAST>().HasKey(d => new { d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.CD_ENDERECO_PI, d.QT_PADRAO_PI });
            modelBuilder.Entity<V_PRE351_STOCK_PICKING_REABAST>().HasKey(d => new { d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.CD_ENDERECO_PICKING, d.CD_ENDERECO });
            modelBuilder.Entity<V_PRE360_STOCK_PICKING_MAL>().HasKey(d => new { d.CD_PRODUTO, d.CD_EMPRESA, d.CD_ENDERECO, d.PICKING_ASIGNADO });
            modelBuilder.Entity<V_PRE811_PREF_ZONA>().HasKey(d => new { d.CD_ZONA, d.NU_PREFERENCIA });
            modelBuilder.Entity<V_PREDIO_USUARIO>().HasKey(d => new { d.NU_PREDIO, d.USERID });
            modelBuilder.Entity<V_PREP_NO_ANULAR_SEL_WPRE450>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.NU_PREPARACION });
            modelBuilder.Entity<V_PREP_NO_ANULAR_WPRE450>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.NU_PREPARACION });
            modelBuilder.Entity<V_PRODUCTOS_SIN_PREP_WEXP041>().HasKey(d => new { d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.CD_FAIXA, d.CD_EMPRESA, d.NU_PREPARACION, d.CD_CAMION, d.CD_CLIENTE });
            modelBuilder.Entity<V_QT_CAMIONES_CONTENEDOR>().HasKey(d => new { d.NU_CONTENEDOR, d.NU_PREPARACION });
            modelBuilder.Entity<V_REAB_PRE680>().HasKey(d => new { d.NU_PREDIO_NECESIDAD, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.CD_EMPRESA });
            modelBuilder.Entity<V_REC150_ETIQUETAS>().HasKey(d => new { d.NU_AGENDA, d.NU_ETIQUETA_LOTE, d.NU_EXTERNO_ETIQUETA, d.TP_ETIQUETA, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_REC170_RECIBIDA_FICTICIA>().HasKey(d => new { d.CD_ENVASE, d.CD_FAIXA_ENVASE, d.CD_EMPRESA, d.NU_AGENDA });
            modelBuilder.Entity<V_REC170_REFERENCIAS_ASIGNADAS>().HasKey(d => new { d.NU_AGENDA, d.NU_RECEPCION_REFERENCIA });
            modelBuilder.Entity<V_REC171_AGENDA_DETALLE>().HasKey(d => new { d.NU_AGENDA, d.CD_EMPRESA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.CD_FAIXA });
            modelBuilder.Entity<V_REC180_LOG_ETIQUETAS>().HasKey(d => new { d.NU_AGENDA, d.NU_ETIQUETA_LOTE, d.TP_ETIQUETA, d.NU_EXTERNO_ETIQUETA, d.NU_LOG_ETIQUETA });
            modelBuilder.Entity<V_REC190_ETIQUETA_LOTE>().HasKey(d => new { d.NU_ETIQUETA_LOTE, d.CD_EMPRESA });
            modelBuilder.Entity<V_RECURSOS_USUARIO>().HasKey(d => new { d.RESOURCEID, d.USERID });
            modelBuilder.Entity<V_REG009_PRODUCTOS>().HasKey(d => new { d.CD_EMPRESA, d.CD_PRODUTO });
            modelBuilder.Entity<V_REG050_PICKING_PRODUCTO>().HasKey(d => new { d.NU_SEC_PICKING_PRODUTO});
            modelBuilder.Entity<V_PRODS_SIN_PICKING_WREG060>().HasKey(d => new { d.CD_EMPRESA, d.CD_PRODUTO });
            modelBuilder.Entity<V_REG090_RAMO_PRODUTO>().HasKey(d => new { d.CD_RAMO_PRODUTO });
            modelBuilder.Entity<V_REG220_AGENTES>().HasKey(d => new { d.CD_EMPRESA, d.CD_CLIENTE });
            modelBuilder.Entity<V_REG220_CLIENTE_RUTA_PREDIO>().HasKey(d => new { d.CD_EMPRESA, d.CD_CLIENTE, d.CD_ROTA, d.NU_PREDIO });
            modelBuilder.Entity<V_REG602_PRODUCTO_CONTROL_CALIDAD>().HasKey(d => new { d.CD_CONTROL, d.CD_EMPRESA, d.CD_PRODUTO });
            modelBuilder.Entity<V_REG603_CODIGO_BARRAS>().HasKey(d => new { d.CD_EMPRESA, d.CD_PRODUTO, d.CD_BARRAS });
            modelBuilder.Entity<V_PRODUCTO_PALLET_WREG605>().HasKey(d => new { d.CD_PRODUTO, d.CD_FAIXA, d.CD_EMPRESA, d.CD_PALLET });
            modelBuilder.Entity<V_REG605_PALLETS>().HasKey(d => new { d.CD_PALLET });
            modelBuilder.Entity<V_REG015_PRODUTOS_PROVEEDOR>().HasKey(d => new { d.CD_PRODUTO, d.CD_EMPRESA, d.CD_CLIENTE });
            modelBuilder.Entity<V_SALDO_ORDEN_COMPRA_FAC>().HasKey(d => new { d.NU_RECEPCION_REFERENCIA, d.CD_EMPRESA, d.CD_PRODUTO, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_SEG030_GRUPO_CONSULTA_FUNC>().HasKey(d => new { d.CD_GRUPO_CONSULTA, d.USERID });
            modelBuilder.Entity<V_SEG_PED_PRE640>().HasKey(d => new { d.NU_PEDIDO, d.CD_EMPRESA, d.CD_CLIENTE });
            modelBuilder.Entity<V_REC200_SELECCION_CROSS_DOCK>().HasKey(d => new { d.NU_AGENDA, d.NU_PREPARACION, d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA });
            modelBuilder.Entity<V_STO005_ESTOQUE>().HasKey(d => new { d.CD_ENDERECO, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_STO395_MOVTO_STOCK>().HasKey(d => new { d.DT_ADDROW, d.NU_DOCTO, d.NU_DOCTO_EXT, d.CD_PRODUTO, d.CD_EMPRESA });
            modelBuilder.Entity<V_STO498_PALLET_TRANSFERENCIA>().HasKey(d => new { d.NU_ETIQUETA, d.NU_SEC_ETIQUETA, d.CD_ENDERECO_ORIGEN, d.CD_FAIXA, d.CD_EMPRESA, d.NU_IDENTIFICADOR, d.CD_PRODUTO, d.NU_SEC_DETALLE });
            modelBuilder.Entity<V_STO500_STOCK_POR_PRODUCTO>().HasKey(d => new { d.CD_PRODUTO, d.CD_EMPRESA, d.NU_PREDIO });
            modelBuilder.Entity<V_STO700_LPN>().HasKey(d => new { d.NU_LPN });
            modelBuilder.Entity<V_STO722_HISTORIAL_DETALLE>().HasKey(d => new { d.NU_LOG_SECUENCIA });
            modelBuilder.Entity<V_STO722_HISTORIAL_ATRIBUTO_DETALLE>().HasKey(d => new { d.NU_LOG_SECUENCIA });
            modelBuilder.Entity<V_STO710_CONSULTA_LPN_ATRIBUTOS>().HasKey(d => new { d.ID_LPN_DET, d.TP_LPN_TIPO, d.ID_ATRIBUTO, d.CD_PRODUTO, d.CD_FAIXA, d.CD_EMPRESA, d.NU_IDENTIFICADOR, d.NU_LPN, });
            modelBuilder.Entity<V_STO720_LPN_LINEAS>().HasKey(d => new { d.NU_LPN, d.ID_LPN_DET });
            modelBuilder.Entity<V_STO730_AUDITORIA_LPN>().HasKey(d => new { d.NU_AUDITORIA_AGRUPADOR });
            modelBuilder.Entity<V_STO730_DET_AUDITORIA_LPN>().HasKey(d => new { d.NU_AUDITORIA_AGRUPADOR, d.NU_AUDITORIA });
            modelBuilder.Entity<V_STO730_DET_ATRIBUTO_LPN>().HasKey(d => new { d.NU_AUDITORIA, d.ID_ATRIBUTO });
            modelBuilder.Entity<V_STO740_LPN_DET_ATRIBUTO_CAB>().HasKey(d => new { d.ID_LPN_DET, d.NU_LPN, d.CD_PRODUTO, d.CD_FAIXA, d.CD_EMPRESA, d.NU_IDENTIFICADOR, d.ID_ATRIBUTO });
            modelBuilder.Entity<V_STO740_LPN_DET_ATRIBUTO_DET>().HasKey(d => new { d.ID_LPN_DET, d.NU_LPN, d.CD_PRODUTO, d.CD_FAIXA, d.CD_EMPRESA, d.NU_IDENTIFICADOR, d.ID_ATRIBUTO });
            modelBuilder.Entity<V_STO800_TRASPASO_CONFIG>().HasKey(d => new { d.NU_TRASPASO_CONFIGURACION });
            modelBuilder.Entity<V_STO800_TIPOS_TRASPASO>().HasKey(d => new { d.TP_TRASPASO });
            modelBuilder.Entity<V_STO810_TRASP_MAPEO_PRODUTO>().HasKey(d => new { d.CD_EMPRESA, d.CD_EMPRESA_DESTINO, d.CD_PRODUTO, d.CD_FAIXA });
            modelBuilder.Entity<V_STO820_TRASPASOS_EMPRESAS>().HasKey(d => new { d.NU_TRASPASO });
            modelBuilder.Entity<V_STO820_DETALLE_PEDIDOS>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA });
            modelBuilder.Entity<V_STO820_PREPARACION_PREPARADO>().HasKey(d => new { d.NU_PREPARACION });
            modelBuilder.Entity<V_STO820_PREPARACION_PENDIENTE>().HasKey(d => new { d.NU_PREPARACION });
            modelBuilder.Entity<V_ORT_FUNC_COMP_COR18>().HasKey(d => new { d.NU_COMPONENTE });
            modelBuilder.Entity<V_STOCK>().HasKey(d => new { d.CD_ENDERECO, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA });
            modelBuilder.Entity<V_STOCK_ENVASE>().HasKey(d => new { d.ID_ENVASE, d.ND_TP_ENVASE });
            modelBuilder.Entity<V_STOCK_POR_PRODUCTO>().HasKey(d => new { d.CD_PRODUTO, d.CD_EMPRESA });
            modelBuilder.Entity<V_STOCK_PRODUTO>().HasKey(d => new { d.CD_ENDERECO, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.DS_PRODUTO, d.CD_MERCADOLOGICO });
            modelBuilder.Entity<V_STOCK_TOTAL>().HasKey(d => new { d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_STOCK_TRASPASO_DEST>().HasKey(d => new { d.NU_TRASPASO, d.CD_PRODUTO_ORIGEN });
            modelBuilder.Entity<V_STOCK_TRASPASO_ORIGEN>().HasKey(d => new { d.NU_TRASPASO, d.CD_EMPRESA_ORIGEN, d.CD_PRODUTO_ORIGEN, d.NU_IDENTIFICADOR_ORIGEN, d.CD_FAIXA_ORIGEN });
            modelBuilder.Entity<V_STOCK_TRASPASO_WSTO040>().HasKey(d => new { d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.CD_ENDERECO, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_TEMP_PEDIDO_MOSTRADOR>().HasKey(d => new { d.NU_PREPARACION, d.NU_CONTENEDOR, d.NU_PEDIDO, d.CD_EMPRESA, d.CD_CLIENTE, d.NU_CARGA });
            modelBuilder.Entity<V_TIPO_PEDIDO>().HasKey(d => new { d.TP_PEDIDO, d.DS_TIPO_PEDIDO, d.DS_MEMO });
            modelBuilder.Entity<V_VERIFICACION_PED_MAIL>().HasKey(d => new { d.CD_CLIENTE, d.CD_EMPRESA, d.NU_PREDIO });
            modelBuilder.Entity<V_INVENTARIO_STOCK>().HasKey(d => new { d.CD_ENDERECO, d.CD_PRODUTO, d.CD_EMPRESA, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_COD_04>().HasKey(d => new { d.NU_AGENDA, d.NU_EJECUCION, d.CD_EMPRESA, d.CD_PROCESO, d.CD_FACTURACION, d.NU_IDENTIFICADOR, d.NU_COMPONENTE, d.CD_PRODUTO, d.NU_ETIQUETA_LOTE });
            modelBuilder.Entity<V_COD_05>().HasKey(d => new { d.NU_EJECUCION, d.CD_FACTURACION, d.NU_FOTO, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.NU_COMPONENTE, d.CD_FAIXA, d.NU_PREDIO, d.CD_EMPRESA, d.CD_PROCESO });
            modelBuilder.Entity<V_COD_06>().HasKey(d => new { d.NU_EJECUCION, d.CD_FACTURACION, d.NU_PEDIDO, d.NU_COMPONENTE, d.CD_EMPRESA, d.CD_PROCESO, d.CD_CLIENTE });
            modelBuilder.Entity<V_COD_07>().HasKey(d => new { d.NU_EJECUCION, d.CD_FACTURACION, d.NU_PEDIDO, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.NU_COMPONENTE, d.CD_EMPRESA, d.CD_PROCESO, d.CD_CLIENTE });
            modelBuilder.Entity<V_COD_08>().HasKey(d => new { d.NU_EJECUCION, d.CD_FACTURACION, d.NU_PEDIDO, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.NU_COMPONENTE, d.CD_EMPRESA, d.CD_PROCESO, d.CD_CLIENTE });
            modelBuilder.Entity<V_FACTURACION_COR_DET_05>().HasKey(d => new { d.NU_EJECUCION, d.CD_EMPRESA, d.CD_PROCESO, d.NU_COMPONENTE, d.DT_FECHA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.TP_RESULTADO });
            modelBuilder.Entity<V_FACTURACION_COR_DET_06>().HasKey(d => new { d.NU_PALLET, d.NU_EJECUCION, d.NU_COMPONENTE, d.CD_EMPRESA, d.CD_PROCESO });
            modelBuilder.Entity<V_FACTURACION_COR_DET_08>().HasKey(d => new { d.NU_PALLET, d.NU_EJECUCION, d.NU_COMPONENTE, d.CD_EMPRESA, d.CD_PROCESO });
            modelBuilder.Entity<V_FACTURACION_COR_DET_09>().HasKey(d => new { d.NU_EJECUCION, d.CD_EMPRESA, d.CD_PROCESO, d.NU_COMPONENTE, d.DT_FECHA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.TP_RESULTADO });
            modelBuilder.Entity<V_FACTURACION_COR_DET_10>().HasKey(d => new { d.NU_EJECUCION, d.CD_EMPRESA, d.CD_PROCESO, d.NU_PALLET, d.NU_COMPONENTE, d.DT_FECHA, d.TP_RESULTADO });
            modelBuilder.Entity<V_FACTURACION_COR_DET_11>().HasKey(d => new { d.NU_EJECUCION, d.CD_EMPRESA, d.CD_PROCESO, d.NU_COMPONENTE, d.DT_FECHA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.TP_RESULTADO });
            modelBuilder.Entity<V_FACTURACION_COR_DET_12>().HasKey(d => new { d.NU_EJECUCION, d.CD_EMPRESA, d.CD_PROCESO, d.NU_COMPONENTE, d.DT_FECHA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.TP_RESULTADO });
            modelBuilder.Entity<V_FACTURACION_COR_DET_14>().HasKey(d => new { d.NU_AGENDA, d.NU_EJECUCION, d.CD_EMPRESA, d.CD_PROCESO, d.CD_FACTURACION, d.NU_COMPONENTE, d.DT_FECHA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.TP_RESULTADO });
            modelBuilder.Entity<V_FACTURACION_COR_DET_21>().HasKey(d => new { d.NU_PALLET, d.NU_EJECUCION, d.NU_COMPONENTE, d.CD_EMPRESA, d.CD_PROCESO });
            modelBuilder.Entity<V_FACTURACION_PRECIO_EMPRESA>().HasKey(d => new { d.CD_EMPRESA, d.CD_LISTA_PRECIO, d.CD_FACTURACION, d.NU_COMPONENTE });
            modelBuilder.Entity<V_FACTURACION_PROC_WFAC004>().HasKey(d => new { d.NU_EJECUCION, d.CD_EMPRESA, d.CD_PROCESO });
            modelBuilder.Entity<V_FACTURACION_RESULTA_WFAC012>().HasKey(d => new { d.NU_EJECUCION, d.CD_EMPRESA, d.NU_COMPONENTE });
            modelBuilder.Entity<V_FACTURACION_RESULTA_WFAC006>().HasKey(d => new { d.NU_EJECUCION, d.CD_EMPRESA, d.NU_COMPONENTE, d.CD_FACTURACION });
            modelBuilder.Entity<V_FOTO_STOCK_PALLET>().HasKey(d => new { d.DT_FOTO, d.CD_EMPRESA, d.CD_PRODUTO, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_FACTURA_ERROR_RESULT_WFAC003>().HasKey(d => new { d.NU_EJECUCION, d.NU_LINEA, d.CD_FACTURACION, d.NU_COMPONENTE, d.CD_EMPRESA });
            modelBuilder.Entity<V_FACTURACION_CODIGO_WFAC249>().HasKey(d => new { d.CD_FACTURACION });
            modelBuilder.Entity<V_FACTURAC_CODIGO_COMP_WFAC250>().HasKey(d => new { d.CD_FACTURACION, d.NU_COMPONENTE });
            modelBuilder.Entity<V_FACTURACION_PROC_FAC_WFAC251>().HasKey(d => new { d.CD_PROCESO });
            modelBuilder.Entity<V_FACTURACION_RESULT_WFAC002>().HasKey(d => new { d.NU_EJECUCION, d.CD_EMPRESA, d.CD_FACTURACION, d.NU_COMPONENTE });
            modelBuilder.Entity<V_FACTURACION_PROC_WFAC010>().HasKey(d => new { d.CD_EMPRESA, d.CD_PROCESO });
            modelBuilder.Entity<V_FACTUR_RESULT_WFAC007>().HasKey(d => new { d.NU_EJECUCION, d.CD_EMPRESA, d.CD_FACTURACION, d.NU_COMPONENTE });
            modelBuilder.Entity<V_COR_11>().HasKey(d => new { d.NU_EJECUCION, d.CD_EMPRESA, d.CD_PROCESO, d.NU_COMPONENTE, d.DT_FECHA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.TP_RESULTADO });
            modelBuilder.Entity<V_COR_03>().HasKey(d => new { d.NU_EJECUCION, d.CD_EMPRESA, d.CD_PROCESO, d.NU_COMPONENTE, d.DT_FECHA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.TP_RESULTADO });
            modelBuilder.Entity<V_COR_04>().HasKey(d => new { d.NU_EJECUCION, d.CD_EMPRESA, d.CD_PROCESO, d.NU_COMPONENTE, d.DT_FECHA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.TP_RESULTADO });
            modelBuilder.Entity<V_COR_05>().HasKey(d => new { d.NU_AGENDA, d.CD_FACTURACION, d.NU_EJECUCION, d.CD_EMPRESA, d.CD_PROCESO, d.NU_COMPONENTE, d.DT_FECHA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.TP_RESULTADO });
            modelBuilder.Entity<V_COR_06>().HasKey(d => new { d.NU_AGENDA, d.CD_FACTURACION, d.NU_EJECUCION, d.CD_EMPRESA, d.CD_PROCESO, d.NU_COMPONENTE, d.DT_FECHA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.TP_RESULTADO });
            modelBuilder.Entity<V_COR_08>().HasKey(d => new { d.NU_AGENDA, d.CD_FACTURACION, d.NU_EJECUCION, d.CD_EMPRESA, d.CD_PROCESO, d.NU_COMPONENTE, d.DT_FECHA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.TP_RESULTADO });
            modelBuilder.Entity<V_COR_09>().HasKey(d => new { d.NU_AGENDA, d.CD_FACTURACION, d.NU_EJECUCION, d.CD_EMPRESA, d.CD_PROCESO, d.NU_COMPONENTE, d.DT_FECHA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.TP_RESULTADO });
            modelBuilder.Entity<V_COR_12>().HasKey(d => new { d.NU_AGENDA, d.CD_FACTURACION, d.NU_EJECUCION, d.CD_EMPRESA, d.CD_PROCESO, d.NU_COMPONENTE, d.DT_FECHA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.TP_RESULTADO });
            modelBuilder.Entity<V_COR_21>().HasKey(d => new { d.NU_AGENDA, d.CD_FACTURACION, d.NU_EJECUCION, d.CD_EMPRESA, d.CD_PROCESO, d.NU_COMPONENTE, d.DT_FECHA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.TP_RESULTADO });
            modelBuilder.Entity<V_COR_14>().HasKey(d => new { d.NU_AGENDA, d.CD_FACTURACION, d.NU_EJECUCION, d.CD_EMPRESA, d.CD_PROCESO, d.NU_COMPONENTE, d.DT_FECHA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.TP_RESULTADO });
            modelBuilder.Entity<V_FACTURAC_PROC_EMP_WFAC005>().HasKey(d => new { d.CD_EMPRESA, d.CD_PROCESO });
            modelBuilder.Entity<V_FACTURA_UND_MEDI_EMP_WFAC230>().HasKey(d => new { d.CD_UNIDADE_MEDIDA, d.CD_EMPRESA });
            modelBuilder.Entity<V_TAREAS_WORT120>().HasKey(d => new { d.CD_TAREA, d.NU_ORDEN_TAREA, d.NM_EMPRESA, d.CD_EMPRESA, d.NU_ORT_ORDEN });
            modelBuilder.Entity<V_EXP110_LABEL_ESTILO>().HasKey(d => new { d.CD_LABEL_ESTILO });

            modelBuilder.Entity<T_ANULACION_PREPARACION>().HasKey(d => new { d.NU_ANULACION_PREPARACION });
            modelBuilder.Entity<T_DET_ANULACION_PREPARACION>().HasKey(d => new { d.NU_ANULACION_PREPARACION, d.NU_PREPARACION, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.CD_EMPRESA, d.CD_ENDERECO, d.NU_PEDIDO, d.CD_CLIENTE, d.NU_SEQ_PREPARACION });
            modelBuilder.Entity<T_ANULACION_PREPARACION_ERROR>().HasKey(d => new { d.NU_ANULACION_PREPARACION_ERROR });
            modelBuilder.Entity<T_IMPRESORA_SERVIDOR>().HasKey(d => new { d.CD_SERVIDOR });
            modelBuilder.Entity<V_DISPONIBLE_CROSS_DOCKING>().HasKey(d => new { d.NU_AGENDA, d.NU_ETIQUETA_LOTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_PEDIDO_ASIG_CAMION>().HasKey(d => new { d.NU_PREPARACION, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.CD_EMPRESA, d.CD_ENDERECO, d.NU_PEDIDO, d.CD_CLIENTE, d.NU_SEQ_PREPARACION });
            modelBuilder.Entity<V_REG070_ZONA_UBICACION>().HasKey(d => new { d.CD_ZONA_UBICACION, });
            modelBuilder.Entity<V_PRE250_REGLA_LIBERACION>().HasKey(d => new { d.NU_REGLA, });
            modelBuilder.Entity<V_PRE250_CLIENTES_DISPONIBLES>().HasKey(d => new { d.CD_CLIENTE, d.CD_EMPRESA });
            modelBuilder.Entity<V_PRE250_CLIENTES_SELECIONADOS>().HasKey(d => new { d.NU_REGLA, d.CD_CLIENTE, d.CD_EMPRESA });
            modelBuilder.Entity<V_TAREA_REFERENCIA_TRACKING>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA });
            modelBuilder.Entity<V_PUNTOS_ENTREGA_CARGA>().HasKey(d => new { d.CD_CAMION, d.CD_CLIENTE, d.CD_EMPRESA, d.NU_PEDIDO });
            modelBuilder.Entity<V_PUNTOS_ENTREGA_EXPEDIDOS>().HasKey(d => new { d.CD_CAMION, d.CD_CLIENTE, d.CD_EMPRESA, d.NU_PEDIDO });
            modelBuilder.Entity<V_PUNTOS_ENTREGA_TRACKING>().HasKey(d => new { d.CD_CAMION, d.CD_CLIENTE, d.CD_EMPRESA, d.NU_PEDIDO });
            modelBuilder.Entity<V_PEDIDOS_PLANIFICADOS_CAMION>().HasKey(d => new { d.CD_CAMION, d.CD_CLIENTE, d.CD_EMPRESA, d.NU_PEDIDO });
            modelBuilder.Entity<V_CONTENEDORES_ENTREGA>().HasKey(d => new { d.NU_CONTENEDOR, d.CD_EMPRESA, d.CD_CLIENTE, d.NU_PEDIDO });
            modelBuilder.Entity<V_CONTENEDORES_ENTREGA_EXP>().HasKey(d => new { d.NU_CONTENEDOR, d.CD_EMPRESA, d.CD_CLIENTE, d.NU_PEDIDO });
            modelBuilder.Entity<V_PEDIDOS_NO_PLANIFICADOS>().HasKey(d => new { d.NU_PEDIDO, d.CD_EMPRESA, d.CD_CLIENTE });
            modelBuilder.Entity<V_PEDIDOS_NO_PLANIFICADOS_JOB>().HasKey(d => new { d.NU_PEDIDO, d.CD_EMPRESA, d.CD_CLIENTE });
            modelBuilder.Entity<T_CROSS_DOCK_TEMP>().HasKey(d => new { d.NU_AGENDA, d.CD_CLIENTE, d.NU_PEDIDO, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR });
            modelBuilder.Entity<V_CROSS_DOCKING_PEND_REC220>().HasKey(d => new { d.CD_EMPRESA, d.NM_EMPRESA, d.NU_AGENDA });
            modelBuilder.Entity<V_CROSS_DOCK_TEMP_WREC220>().HasKey(d => new { d.NU_AGENDA, d.CD_EMPRESA, d.CD_CLIENTE, d.CD_ROTA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_PEDIDO, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR });
            modelBuilder.Entity<V_FACTU_SIN_EXPEDIR_REC270>().HasKey(d => new { d.NU_CARGA, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_CAMION });
            modelBuilder.Entity<V_REC275_ESTRATEGIA>().HasKey(d => new { d.NU_ALM_ESTRATEGIA });
            modelBuilder.Entity<V_REC275_ASOCIACIONES>().HasKey(d => new { d.NU_PREDIO, d.TP_ENTIDAD, d.CD_ENTIDAD, d.CD_EMPRESA, d.TP_ALM_OPERATIVA_ASOCIABLE, d.TP_RECEPCION });
            modelBuilder.Entity<V_REC275_PARAMETROS_INSTANCIAS>().HasKey(d => new { d.NU_ALM_LOGICA_INSTANCIA_PARAM });
            modelBuilder.Entity<V_REC275_LOGICAS>().HasKey(d => new { d.NU_ALM_LOGICA });
            modelBuilder.Entity<V_REC275_LOGICA_INSTANCIA>().HasKey(d => new { d.NU_ALM_LOGICA_INSTANCIA });
            modelBuilder.Entity<V_REC275_PARAMETROS>().HasKey(d => new { d.NU_ALM_PARAMETRO });
            modelBuilder.Entity<V_REC275_ALM_OPERATIVA_ASOCIABLE>().HasKey(d => new { d.TP_RECEPCION, d.TP_ALM_OPERATIVA_ASOCIABLE });
            modelBuilder.Entity<V_REC275_OPERATIVAS_ASOCIADAS>().HasKey(d => new { d.NU_PREDIO, d.TP_ENTIDAD, d.CD_ENTIDAD, d.CD_EMPRESA, d.TP_ALM_OPERATIVA_ASOCIABLE, d.TP_RECEPCION });
            modelBuilder.Entity<V_REC275_GRUPOS>().HasKey(d => new { d.CD_GRUPO });
            modelBuilder.Entity<V_LOG_CONTROL_PICKEO_WPRE221>().HasKey(d => new { d.NU_LOG_CONT_PICKEO });
            modelBuilder.Entity<V_REG911_DETALLE_DOMINIO>().HasKey(d => new { d.NU_DOMINIO });
            modelBuilder.Entity<V_REC275_GRUPOS>().HasKey(d => new { d.CD_GRUPO });
            modelBuilder.Entity<V_REC280_PANEL_SUGERENCIA>().HasKey(d => new { d.NU_ALM_ESTRATEGIA, d.NU_PREDIO, d.TP_ALM_OPERATIVA_ASOCIABLE, d.CD_ALM_OPERATIVA_ASOCIABLE, d.CD_CLASSE, d.CD_GRUPO, d.CD_EMPRESA_PRODUTO, d.CD_PRODUTO, d.CD_REFERENCIA, d.CD_AGRUPADOR, d.CD_ENDERECO_SUGERIDO, d.NU_ALM_SUGERENCIA });
            modelBuilder.Entity<V_REC280_PANEL_SUGERENCIA_DET>().HasKey(d => new { d.NU_ALM_ESTRATEGIA, d.NU_PREDIO, d.TP_ALM_OPERATIVA_ASOCIABLE, d.CD_ALM_OPERATIVA_ASOCIABLE, d.CD_CLASSE, d.CD_GRUPO, d.CD_EMPRESA_PRODUTO, d.CD_PRODUTO, d.CD_REFERENCIA, d.CD_AGRUPADOR, d.CD_ENDERECO_SUGERIDO, d.CD_EMPRESA, d.CD_PRODUTO_AGRUPADOR, d.CD_FAIXA_AGRUPADOR, d.NU_IDENTIFICADOR_AGRUPADOR, d.NU_ALM_SUGERENCIA_DET });
            modelBuilder.Entity<V_REC280_PANEL_SUGERENCIA_REABASTECIMIENTO>().HasKey(d => new { d.NU_ALM_REABASTECIMIENTO });
            modelBuilder.Entity<V_REG911_DETALLE_DOMINIO>().HasKey(d => new { d.NU_DOMINIO });
            modelBuilder.Entity<V_LOG_CONTROL_PICKEO_WPRE221>().HasKey(d => new { d.NU_LOG_CONT_PICKEO });
            modelBuilder.Entity<V_CONTROL_CONT_ENV_WPRE220>().HasKey(d => new { d.NU_PREPARACION, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.NU_CONTENEDOR, d.NU_LOG_CONT_PICKEO });
            modelBuilder.Entity<V_REG910_DOMINIO>().HasKey(d => new { d.CD_DOMINIO });
            modelBuilder.Entity<T_ALM_ASOCIACION>().HasKey(d => new { d.NU_ALM_ASOCIACION });
            modelBuilder.Entity<T_ALM_LOGICA_INSTANCIA_PARAM>().HasKey(d => new { d.NU_ALM_LOGICA_INSTANCIA_PARAM });
            modelBuilder.Entity<T_ALM_ASOCIACION>().HasKey(d => new { d.NU_ALM_ASOCIACION });
            modelBuilder.Entity<V_CONTROL_CONT_ENV_WPRE220>().HasKey(d => new { d.NU_PREPARACION, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.NU_CONTENEDOR, d.NU_LOG_CONT_PICKEO });
            modelBuilder.Entity<T_ALM_LOGICA_INSTANCIA_PARAM>().HasKey(d => new { d.NU_ALM_LOGICA_INSTANCIA_PARAM });
            modelBuilder.Entity<T_ALM_LOGICA_INSTANCIA>().HasKey(d => new { d.NU_ALM_LOGICA_INSTANCIA });
            modelBuilder.Entity<T_ALM_ESTRATEGIA>().HasKey(d => new { d.NU_ALM_ESTRATEGIA });
            modelBuilder.Entity<V_FACTURACION_COR_DET_02>().HasKey(d => new { d.NU_PALLET_DET });
            modelBuilder.Entity<V_PED_CAR_EXP_REC270>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.NU_PREPARACION });
            modelBuilder.Entity<V_COF010_LENGUAJE_IMPRESION>().HasKey(d => new { d.CD_LENGUAJE_IMPRESION });
            modelBuilder.Entity<V_COF030_TEMPLATE_ETIQUETA>().HasKey(d => new { d.CD_LABEL_ESTILO, d.CD_LENGUAJE_IMPRESION });
            modelBuilder.Entity<V_LOCALIZACION_WCOF050>().HasKey(d => new { d.CD_APLICACION, d.CD_BLOQUE, d.CD_TIPO, d.CD_CLAVE, d.CD_IDIOMA });
            modelBuilder.Entity<V_LPARAMETROS_LCON010>().HasKey(d => new { d.CD_PARAMETRO, d.DO_ENTIDAD_PARAMETRIZABLE });
            modelBuilder.Entity<V_LPARAMETROS_CONFIG_LCON020>().HasKey(d => new { d.NU_PARAMETRO_CONFIGURACION, d.CD_PARAMETRO, d.DO_ENTIDAD_PARAMETRIZABLE, d.ND_ENTIDAD });
            modelBuilder.Entity<V_FACTURA_COD_LIST_COT_WFAC256>().HasKey(d => new { d.CD_FACTURACION, d.NU_COMPONENTE, d.CD_LISTA_PRECIO });
            modelBuilder.Entity<V_FACTURACION_PRECIO_EMPRESA>().HasKey(d => new { d.CD_EMPRESA, d.CD_LISTA_PRECIO, d.CD_FACTURACION, d.NU_COMPONENTE });
            modelBuilder.Entity<V_INT050_EMPRESAS_BLOQUEADAS>().HasKey(d => new { d.CD_EMPRESA });
            modelBuilder.Entity<V_PEDIDOS_SIN_CERRAR>().HasKey(d => new { d.NU_PEDIDO, d.CD_EMPRESA, d.CD_CLIENTE });
            modelBuilder.Entity<T_DOCUMENTO_PREPARACION>().HasKey(d => new { d.NU_DOCUMENTO_PREPARACION });
            modelBuilder.Entity<V_DOC100_DOC_PREPARACION>().HasKey(d => new { d.NU_DOCUMENTO_PREPARACION });
            modelBuilder.Entity<V_DOC_DISP_ASOCIAR_PREP>().HasKey(d => new { d.NU_DOCUMENTO, d.TP_DOCUMENTO });
            modelBuilder.Entity<V_PREP_DISP_ASOCIAR>().HasKey(d => new { d.TP_OPERATIVA, d.NU_PREPARACION });
            modelBuilder.Entity<V_FAC008_RESULTADO_DETALLE>().HasKey(d => new { d.NU_RESULTADO_DETALLE });
            modelBuilder.Entity<T_FUNCIONARIO>().HasKey(d => new { d.CD_FUNCIONARIO });
            modelBuilder.Entity<V_TIPO_ENDERECO_WPAR050>().HasKey(d => new { d.CD_TIPO_ENDERECO });
            modelBuilder.Entity<V_PAR050_TIPO_ESTRUTURA>().HasKey(d => new { d.CD_TP_ESTR });
            modelBuilder.Entity<V_PAR400_ATRIBUTOS>().HasKey(d => new { d.ID_ATRIBUTO });
            modelBuilder.Entity<V_PAR400_ATRIBUTOS_LPN_TIPO>().HasKey(d => new { d.ID_ATRIBUTO, d.TP_LPN_TIPO });
            modelBuilder.Entity<V_PAR400_ATRIBUTOS_VALIDACION_ASOCIADA>().HasKey(d => new { d.ID_ATRIBUTO, d.ID_VALIDACION });
            modelBuilder.Entity<V_PAR400_TIPO_ATRIBUTO>().HasKey(d => new { d.ID_ATRIBUTO_TIPO });
            modelBuilder.Entity<V_PAR400_ATRIBUTOS_SISTEMA>().HasKey(d => new { d.NM_CAMPO });
            modelBuilder.Entity<V_PAR400_MASCARA_FECHA>().HasKey(d => new { d.VL_MASCARA });
            modelBuilder.Entity<V_PAR400_MASCARA_HORA>().HasKey(d => new { d.VL_MASCARA_HORA });
            modelBuilder.Entity<V_PAR401_ASOCIAR_ATRIBUTO_TIPO>().HasKey(d => new { d.ID_ATRIBUTO, d.NM_ATRIBUTO, d.TP_LPN_TIPO });
            modelBuilder.Entity<V_PAR401_ASOCIAR_ATRIBUTO_TIPO_DET>().HasKey(d => new { d.ID_ATRIBUTO, d.NM_ATRIBUTO, d.TP_LPN_TIPO });
            modelBuilder.Entity<T_ALM_SUGERENCIA>().HasKey(d => new { d.NU_ALM_ESTRATEGIA, d.NU_PREDIO, d.TP_ALM_OPERATIVA_ASOCIABLE, d.CD_ALM_OPERATIVA_ASOCIABLE, d.CD_CLASSE, d.CD_GRUPO, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_REFERENCIA, d.CD_AGRUPADOR, d.CD_ENDERECO_SUGERIDO, d.NU_ALM_SUGERENCIA });
            modelBuilder.Entity<T_ALM_SUGERENCIA_DET>().HasKey(d => new { d.NU_ALM_ESTRATEGIA, d.NU_PREDIO, d.TP_ALM_OPERATIVA_ASOCIABLE, d.CD_ALM_OPERATIVA_ASOCIABLE, d.CD_CLASSE, d.CD_GRUPO, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_REFERENCIA, d.CD_AGRUPADOR, d.CD_ENDERECO_SUGERIDO, d.CD_EMPRESA_AGRUPADOR, d.CD_PRODUTO_AGRUPADOR, d.CD_FAIXA_AGRUPADOR, d.NU_IDENTIFICADOR_AGRUPADOR, d.NU_ALM_SUGERENCIA, d.CD_ENDERECO_SUGERIDO_AGRUPADOR, d.NU_ALM_SUGERENCIA_DET });
            modelBuilder.Entity<V_CARGAS_CON_MULTIPLE_PEDIDO>().HasKey(d => new { d.NU_CARGA });
            modelBuilder.Entity<V_CAMION_CARGA_PEDIDO>().HasKey(d => new { d.CD_CAMION, d.NU_CARGA, d.CD_CLIENTE, d.CD_EMPRESA, d.NU_PEDIDO, d.NU_PREPARACION });
            modelBuilder.Entity<T_LPN_TIPO>().HasKey(d => new { d.TP_LPN_TIPO });
            modelBuilder.Entity<V_LPN_TIPO>().HasKey(e => new { e.TP_LPN_TIPO });
            modelBuilder.Entity<T_LPN_ATRIBUTO>().HasKey(d => new { d.NU_LPN, d.TP_LPN_TIPO, d.ID_ATRIBUTO });
            modelBuilder.Entity<T_LPN_DET_ATRIBUTO>().HasKey(d => new { d.ID_LPN_DET, d.TP_LPN_TIPO, d.ID_ATRIBUTO, d.CD_PRODUTO, d.CD_FAIXA, d.CD_EMPRESA, d.NU_IDENTIFICADOR, d.NU_LPN });
            modelBuilder.Entity<V_LPN_TIPO_ATRIBUTO>().HasKey(d => new { d.ID_ATRIBUTO, d.TP_LPN_TIPO });
            modelBuilder.Entity<V_LPN_TIPO_ATRIBUTO_DET>().HasKey(d => new { d.ID_ATRIBUTO, d.TP_LPN_TIPO });
            modelBuilder.Entity<V_LPN_CODIGOS_BARRAS>().HasKey(d => new { d.NU_LPN, d.ID_LPN_BARRAS });
            modelBuilder.Entity<T_ATRIBUTO>().HasKey(d => new { d.ID_ATRIBUTO });
            modelBuilder.Entity<T_ATRIBUTO_ESTADO>().HasKey(d => new { d.ID_ESTADO });
            modelBuilder.Entity<T_LPN>().HasKey(d => new { d.NU_LPN });
            modelBuilder.Entity<T_LPN_TIPO_ATRIBUTO>().HasKey(d => new { d.TP_LPN_TIPO, d.ID_ATRIBUTO });
            modelBuilder.Entity<T_LPN_CONSOLIDACION_TIPO>().HasKey(d => new { d.ID_CONSOLIDACION_TIPO });
            modelBuilder.Entity<T_LPN_TIPO_ATRIBUTO_DET>().HasKey(d => new { d.TP_LPN_TIPO, d.ID_ATRIBUTO });
            modelBuilder.Entity<T_LPN_BARRAS>().HasKey(d => new { d.ID_LPN_BARRAS, d.NU_LPN });
            modelBuilder.Entity<V_ATRIBUTO_VALIDACION_DISP>().HasKey(d => new { d.ID_VALIDACION });
            modelBuilder.Entity<V_ATRIBUTO_VALIDACION_ASOCIADO>().HasKey(d => new { d.ID_ATRIBUTO, d.ID_VALIDACION });
            modelBuilder.Entity<T_ATRIBUTO_VALIDACION_ASOCIADA>().HasKey(d => new { d.ID_ATRIBUTO, d.ID_VALIDACION });
            modelBuilder.Entity<T_ATRIBUTO_TIPO>().HasKey(d => new { d.ID_ATRIBUTO_TIPO });
            modelBuilder.Entity<T_ATRIBUTO_SISTEMA>().HasKey(d => new { d.NM_CAMPO });
            modelBuilder.Entity<V_CARGAS_EGRESO_DOCUMENTAL>().HasKey(d => new { d.CD_CAMION, d.NU_CARGA, d.CD_CLIENTE, d.CD_EMPRESA });
            modelBuilder.Entity<V_PED_NUEVAS_CARGAS>().HasKey(d => new { d.CD_CAMION, d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA });
            modelBuilder.Entity<V_PLANIFICACION_CAMION>().HasKey(d => new { d.CD_CAMION, d.CD_PUNTO_ENTREGA, d.VL_COMPARTE_CONTENEDOR_ENTREGA, d.CD_EMPRESA, d.CD_CLIENTE, d.NU_CONTENEDOR });
            modelBuilder.Entity<T_LPN_DET>().HasKey(d => new { d.ID_LPN_DET, d.NU_LPN, d.CD_PRODUTO, d.CD_FAIXA, d.CD_EMPRESA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_STO700_LT_LPN>().HasKey(d => new { d.NU_LOG_SECUENCIA });
            modelBuilder.Entity<T_ATRIBUTO_VALIDACION>().HasKey(d => new { d.ID_VALIDACION });
            modelBuilder.Entity<V_EGRESOS_A_MARCAR>().HasKey(d => new { d.CD_CAMION });
            modelBuilder.Entity<V_PUNTOS_ENTREGA_CLIENTE>().HasKey(d => new { d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PUNTO_ENTREGA_PEDIDO });
            modelBuilder.Entity<V_PLANIFICACION_DEVOLUCION>().HasKey(d => new { d.NU_AGENDA });
            modelBuilder.Entity<V_PLANIFICACION_DEVOLUCION_DET>().HasKey(d => new { d.NU_AGENDA, d.CD_EMPRESA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.CD_FAIXA });
            modelBuilder.Entity<T_LPN_TIPO_BARRAS>().HasKey(d => new { d.TP_BARRAS });
            modelBuilder.Entity<V_CONT_SIN_EMBARCAR_PED_SINCAM>().HasKey(d => new { d.NU_CONTENEDOR, d.NU_PREPARACION, d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA });
            modelBuilder.Entity<V_PRD_PEDIDO_LOTE_CONTENEDOR>().HasKey(d => new { d.CD_PRODUTO, d.CD_EMPRESA, d.NU_IDENTIFICADOR, d.CD_FAIXA, d.NU_PREPARACION, d.NU_PEDIDO, d.CD_CLIENTE });
            modelBuilder.Entity<V_PEDIDO_EMPAQUE>().HasKey(d => new { d.CD_EMPRESA, d.CD_CLIENTE, d.NU_PEDIDO, d.CD_ENDERECO });
            modelBuilder.Entity<V_PRODUTOS_SIN_PREP_PED_SINCAM>().HasKey(d => new { d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.NU_PEDIDO, d.CD_FAIXA, d.CD_EMPRESA, d.NU_PREPARACION, d.CD_CLIENTE });
            modelBuilder.Entity<V_PEDIDO_PREPARADO_COMPLETO>().HasKey(d => new { d.CD_EMPRESA, d.CD_CLIENTE, d.TP_PEDIDO, d.NU_PEDIDO });
            modelBuilder.Entity<V_CONTENEDOR_WISEX150>().HasKey(d => new { d.CD_CLIENTE, d.NU_PEDIDO, d.CD_EMPRESA, d.NU_CONTENEDOR, d.NU_PREPARACION });
            modelBuilder.Entity<V_DET_PEDIDO_PRE340>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_PRODUTO, d.CD_EMPRESA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_CONTENEDOR_PEDIDO_CONSULTA>().HasKey(d => new { d.NU_CONTENEDOR, d.NU_PREPARACION, d.CD_CLIENTE, d.CD_EMPRESA, d.NU_PEDIDO });
            modelBuilder.Entity<V_PEDIDO_FACTURADO_CONSOLIDADO>().HasKey(d => new { d.NU_PEDIDO, d.CD_EMPRESA, d.CD_CLIENTE, d.CD_CAMION });
            modelBuilder.Entity<V_PRODUTOS_SIN_CAMION>().HasKey(d => new { d.CD_EMPRESA, d.CD_CLIENTE, d.NU_PEDIDO, d.CD_PRODUTO, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_TIPO_EXPEDICION>().HasKey(d => new { d.TP_EXPEDICION });
            modelBuilder.Entity<V_PICKING_WISEX150>().HasKey(d => new { d.CD_PRODUTO, d.CD_EMPRESA, d.NU_IDENTIFICADOR, d.CD_FAIXA, d.NU_PREPARACION, d.NU_PEDIDO, d.CD_CLIENTE, d.CD_ENDERECO, d.NU_SEQ_PREPARACION });
            modelBuilder.Entity<V_PEDIDO_PRODUTO_CONTENEDOR>().HasKey(d => new { d.CD_PRODUTO, d.CD_EMPRESA, d.NU_PREPARACION, d.NU_PEDIDO, d.CD_CLIENTE });
            modelBuilder.Entity<V_EXP110_DET_PICKING>().HasKey(d => new { d.CD_PRODUTO, d.CD_EMPRESA, d.NU_IDENTIFICADOR, d.CD_FAIXA, d.NU_PREPARACION, d.NU_PEDIDO, d.CD_CLIENTE, d.CD_ENDERECO, d.NU_SEQ_PREPARACION });
            modelBuilder.Entity<V_PEDIDO_PRODUTO_CAMION>().HasKey(d => new { d.CD_EMPRESA, d.CD_CLIENTE, d.NU_PEDIDO, d.CD_CAMION, d.CD_PRODUTO, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_PEDIDO_CLIENTE_PR340>().HasKey(d => new { d.CD_EMPRESA, d.CD_CLIENTE, d.NU_PEDIDO });
            modelBuilder.Entity<V_EXP110_CONT_PREP>().HasKey(d => new { d.NU_PREPARACION, d.NU_CONTENEDOR, d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA });
            modelBuilder.Entity<V_CONTACTO_GRU_NS_WEVT030>().HasKey(d => new { d.NU_CONTACTO });
            modelBuilder.Entity<V_CONTACTO_GRUPO_WEVT030>().HasKey(d => new { d.NU_CONTACTO, d.NU_CONTACTO_GRUPO });

            modelBuilder.Entity<T_GRUPO>().HasKey(d => new { d.CD_GRUPO });
            modelBuilder.Entity<T_GRUPO_PARAM>().HasKey(d => new { d.NU_PARAM });
            modelBuilder.Entity<T_GRUPO_REGLA>().HasKey(d => new { d.NU_GRUPO_REGLA });
            modelBuilder.Entity<T_GRUPO_REGLA_PARAM>().HasKey(d => new { d.NU_GRUPO_REGLA_PARAM });

            modelBuilder.Entity<V_REG300_GRUPOS>().HasKey(d => new { d.CD_GRUPO });
            modelBuilder.Entity<V_REG300_GRUPOS_ELIMINABLES>().HasKey(d => new { d.CD_GRUPO });
            modelBuilder.Entity<V_REG300_REGLAS>().HasKey(d => new { d.NU_GRUPO_REGLA });
            modelBuilder.Entity<V_REG300_PARAMETROS>().HasKey(d => new { d.NU_PARAM });
            modelBuilder.Entity<V_REG300_PARAMETROS_REGLA>().HasKey(d => new { d.NU_GRUPO_REGLA_PARAM });
            modelBuilder.Entity<V_DET_ETIQUETA_SIN_CLASIFICAR>().HasKey(d => new { d.NU_ETIQUETA_LOTE, d.NU_AGENDA, d.NU_EXTERNO_ETIQUETA, d.TP_ETIQUETA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.CD_FAIXA, d.CD_EMPRESA });
            modelBuilder.Entity<T_ALM_REABASTECIMIENTO>().HasKey(d => new { d.NU_ALM_REABASTECIMIENTO });
            modelBuilder.Entity<V_REC410_SUGERENCIA_REABAST>().HasKey(d => new { d.CD_PRODUTO, d.CD_EMPRESA, d.CD_FAIXA, d.CD_ENDERECO_PI, d.NU_PREDIO });
            modelBuilder.Entity<V_STOCK_PARA_REABASTECIMIENTO>().HasKey(d => new { d.CD_FAIXA, d.QT_PADRAO_PI, d.CD_PRODUTO, d.ID_ENDERECO_BAIXO_ST, d.NU_IDENTIFICADOR_ST, d.CD_ENDERECO_ST, d.CD_EMPRESA, d.CD_ENDERECO_PI });

            modelBuilder.Entity<T_AUTOMATISMO>().HasKey(d => new { d.NU_AUTOMATISMO });
            modelBuilder.Entity<T_AUTOMATISMO_CARACTERISTICA>().HasKey(d => new { d.NU_AUTOMATISMO_CARACTERISTICA });
            modelBuilder.Entity<T_AUTOMATISMO_CARACTERISTICA_CONFIG>().HasKey(d => new { d.TP_AUTOMATISMO, d.CD_AUTOMATISMO_CARACTERISTICA, d.VL_AUTOMATISMO_CARACTERISTICA });
            modelBuilder.Entity<T_AUTOMATISMO_DATA>().HasKey(d => new { d.NU_AUTOMATISMO_DATA });
            modelBuilder.Entity<T_AUTOMATISMO_EJECUCION>().HasKey(d => new { d.NU_AUTOMATISMO_EJECUCION });
            modelBuilder.Entity<T_AUTOMATISMO_INTERFAZ>().HasKey(d => new { d.NU_AUTOMATISMO_INTERFAZ });
            modelBuilder.Entity<T_AUTOMATISMO_POSICION>().HasKey(d => new { d.NU_AUTOMATISMO_POSICION });
            modelBuilder.Entity<T_AUTOMATISMO_PUESTO>().HasKey(d => new { d.NU_AUTOMATISMO_PUESTO });
            modelBuilder.Entity<T_INTEGRACION_SERVICIO>().HasKey(d => new { d.NU_INTEGRACION });

            modelBuilder.Entity<V_AUT100_EJECUCIONES>().HasKey(d => new { d.NU_AUTOMATISMO_EJECUCION });
            modelBuilder.Entity<V_AUT100_CARACTERISTICAS2>().HasKey(d => new { d.NU_AUTOMATISMO_CARACTERISTICA });
            modelBuilder.Entity<V_AUT100_CARACTERISTICAS>().HasKey(d => new { d.NU_AUTOMATISMO_CARACTERISTICA });
            modelBuilder.Entity<V_AUT100_POSICIONES>().HasKey(d => new { d.NU_AUTOMATISMO_POSICION });
            modelBuilder.Entity<V_STOCK_AUTOMATISMO_AUT100>().HasKey(d => new { d.CD_ENDERECO, d.CD_PRODUTO, d.CD_FAIXA, d.CD_EMPRESA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_AUT100_INTERFACES>().HasKey(d => new { d.NU_AUTOMATISMO_INTERFAZ });
            modelBuilder.Entity<V_PRODUCTOS_ASOCIADOS_AUTOMATISMO>().HasKey(d => new { d.CD_PRODUTO, d.CD_EMPRESA, d.CD_ZONA_UBICACION, d.NU_PREDIO });
            modelBuilder.Entity<T_TIPO_AGRUPACION_ENDERECO>().HasKey(d => new { d.TP_AGRUPACION_UBIC });
            modelBuilder.Entity<V_COF110_SERVICIOS_INTEGRACION>().HasKey(d => new { d.NU_INTEGRACION });
            modelBuilder.Entity<V_PTL010_PICK_TO_LIGHT>().HasKey(d => new { d.NU_PREPARACION, d.CD_CLIENTE, d.CD_EMPRESA });
            modelBuilder.Entity<V_PTL010_AGRU_VL_COMP_CONT_PICK>().HasKey(d => new { d.NU_PREPARACION, d.CD_CLIENTE, d.CD_EMPRESA });
            modelBuilder.Entity<V_PTL010_AGRU_SUBCLASE_PROD>().HasKey(d => new { d.NU_PREPARACION, d.CD_CLIENTE, d.CD_EMPRESA });
            modelBuilder.Entity<T_AUTOMATISMO_CONF_ENTRADA>().HasKey(d => new { d.NU_INTERFAZ_EJECUCION, d.NU_INTERFAZ_EJECUCION_ENT, d.CD_EMPRESA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.ID_LINEA_ENTRADA });

            modelBuilder.Entity<V_REPORTE_CONF_RECEPCION>().HasKey(d => new { d.NU_AGENDA });
            modelBuilder.Entity<V_REPORTE_CONF_RECEPCION_DET>().HasKey(d => new { d.NU_AGENDA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.CD_FAIXA, d.CD_EMPRESA });
            modelBuilder.Entity<V_REPORTE_PACKING_LIST>().HasKey(d => new { d.CD_CAMION });
            modelBuilder.Entity<V_REPORTE_PACKING_LIST_DET>().HasKey(d => new { d.CD_CLIENTE, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.CD_EMPRESA, d.CD_CAMION });
            modelBuilder.Entity<V_REPORTE_CONT_CAMION>().HasKey(d => new { d.CD_CAMION });
            modelBuilder.Entity<V_REPORTE_CONT_CAMION_DET>().HasKey(d => new { d.CD_CAMION, d.CD_CLIENTE, d.CD_EMPRESA, d.TP_CONTENEDOR });
            modelBuilder.Entity<V_REPORTE_CONTROL_CAMBIO>().HasKey(d => new { d.NU_PREPARACION, d.NU_CONTENEDOR, d.CD_CAMION, d.CD_CLIENTE, d.CD_EMPRESA });
            modelBuilder.Entity<T_UNIDAD_TRANSPORTE>().HasKey(d => new { d.NU_UNIDAD_TRANSPORTE });
            modelBuilder.Entity<V_REC170_ETIQUETA_UT>().HasKey(d => new { d.NU_UNIDAD_TRANSPORTE, d.CD_EMPRESA });
            modelBuilder.Entity<V_POTERIA_VEHICULO_CAMION>().HasKey(d => new { d.NU_PORTERIA_VEHICULO, d.NU_PORTERIA_VEHICULO_CAMION, d.CD_CAMION });
            modelBuilder.Entity<V_DOCUMENTO_DOC080>().HasKey(d => new { d.NU_DOCUMENTO, d.TP_DOCUMENTO });
            modelBuilder.Entity<V_DOCUMENTO_DOC260>().HasKey(d => new { d.TP_DOCUMENTO, d.NU_DOCUMENTO });
            modelBuilder.Entity<T_LPN_AUDITORIA>().HasKey(d => new { d.NU_AUDITORIA });
            modelBuilder.Entity<T_LPN_AUDITORIA_ATRIBUTO>().HasKey(d => new { d.NU_AUDITORIA, d.ID_ATRIBUTO });
            modelBuilder.Entity<T_DET_PEDIDO_SAIDA_LPN>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR, d.ID_LPN_EXTERNO, d.TP_LPN_TIPO });
            modelBuilder.Entity<V_REC170_LPN>().HasKey(d => new { d.NU_LPN });
            modelBuilder.Entity<V_STO151_STOCK_LPN>().HasKey(d => new { d.NU_LPN, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_STOCK_PRODUCTO_LPN>().HasKey(d => new { d.CD_ENDERECO, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_PRE100_LPN_AGREGADOS>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.TP_LPN_TIPO, d.ID_LPN_EXTERNO });
            modelBuilder.Entity<V_PRE100_LPN_DISPONIBLES>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.TP_LPN_TIPO, d.ID_LPN_EXTERNO });
            modelBuilder.Entity<V_PRE100_DET_PEDIDO_LPN>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR, d.TP_LPN_TIPO, d.ID_LPN_EXTERNO });
            modelBuilder.Entity<V_PRE100_DET_PEDIDO_LPN_ATRIB>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR, d.TP_LPN_TIPO, d.ID_LPN_EXTERNO, d.NU_DET_PED_SAI_ATRIB });

            modelBuilder.Entity<T_TEMP_DET_PEDIDO_SAIDA_ATRIB>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR, d.ID_ATRIBUTO, d.USERID, d.FL_CABEZAL });
            modelBuilder.Entity<T_TEMP_DET_PEDIDO_SAIDA_LPN_ATRIB>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR, d.TP_LPN_TIPO, d.ID_LPN_EXTERNO, d.ID_ATRIBUTO, d.USERID, d.FL_CABEZAL });
            modelBuilder.Entity<T_PRDC_DET_INGRESO_TEORICO>().HasKey(d => new { d.NU_PRDC_DET_TEORICO });
            modelBuilder.Entity<T_PRDC_INGRESO_CONTROLES>().HasKey(d => new { d.NU_PRDC_DET_TEORICO, d.CD_CONTROL });
            modelBuilder.Entity<T_PRDC_INGRESO_INSTRUCCION>().HasKey(d => new { d.NU_PRDC_INGRESO_INSTRUCCION });
            modelBuilder.Entity<T_PRDC_INGRESO_DET_PEDIDO_TEMP>().HasKey(d => new { d.NU_PRDC_INGRESO, d.CD_PRODUTO, d.CD_EMPRESA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<T_PRDC_DET_INGRESO_REAL>().HasKey(d => new { d.NU_PRDC_INGRESO_REAL });
            modelBuilder.Entity<T_PRDC_DET_SALIDA_REAL>().HasKey(d => new { d.NU_PRDC_SALIDA_REAL });

            modelBuilder.Entity<V_PRE100_ATRIBUTOS_TIPO_SIN_DEFINIR>().HasKey(d => new { d.TP_LPN_TIPO, d.ID_ATRIBUTO, d.FL_CABEZAL });
            modelBuilder.Entity<V_PRE100_ATRIBUTOS_LPN_DEFINIDOS>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR, d.TP_LPN_TIPO, d.ID_LPN_EXTERNO, d.NU_DET_PED_SAI_ATRIB, d.ID_ATRIBUTO, d.FL_CABEZAL });
            modelBuilder.Entity<V_PRE100_DET_PEDIDO_ATRIBUTO>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR, d.NU_DET_PED_SAI_ATRIB });
            modelBuilder.Entity<V_PRE100_ATRIBUTOS_DEFINIDOS>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR, d.NU_DET_PED_SAI_ATRIB, d.ID_ATRIBUTO, d.FL_CABEZAL });
            modelBuilder.Entity<V_PRE100_ATRIBUTOS_SIN_DEFINIR>().HasKey(d => new { d.ID_ATRIBUTO, d.FL_CABEZAL });
            modelBuilder.Entity<V_PRE110_DET_PEDIDO_LPN>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.TP_LPN_TIPO, d.ID_LPN_EXTERNO, d.ID_ESPECIFICA_IDENTIFICADOR, d.NU_IDENTIFICADOR, d.CD_FAIXA, d.CD_PRODUTO, d.CD_EMPRESA });
            modelBuilder.Entity<V_PRE110_DET_PEDIDO_ATRIB>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR, d.NU_DET_PED_SAI_ATRIB });
            modelBuilder.Entity<V_PRE110_DET_PEDIDO_LPN_ATR>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.TP_LPN_TIPO, d.ID_LPN_EXTERNO, d.NU_DET_PED_SAI_ATRIB, d.ID_ESPECIFICA_IDENTIFICADOR, d.NU_IDENTIFICADOR, d.CD_FAIXA, d.CD_PRODUTO, d.CD_EMPRESA });
            modelBuilder.Entity<V_PRE110_ATRIBUTOS_LPN_DEFINIDOS>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR, d.TP_LPN_TIPO, d.ID_LPN_EXTERNO, d.NU_DET_PED_SAI_ATRIB, d.ID_ATRIBUTO, d.FL_CABEZAL });
            modelBuilder.Entity<V_PRE110_ATRIBUTOS_DEFINIDOS>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.ID_ESPECIFICA_IDENTIFICADOR, d.NU_DET_PED_SAI_ATRIB, d.ID_ATRIBUTO, d.FL_CABEZAL });
            modelBuilder.Entity<T_LOG_PEDIDO_ANULADO_LPN>().HasKey(d => new { d.NU_LOG_PEDIDO_ANULADO_LPN });
            modelBuilder.Entity<V_STO750_CONSULTA_UT>().HasKey(d => new { d.NU_UNIDAD_TRANSPORTE });
            modelBuilder.Entity<T_AGENDA_LPN_PLANIFICACION>().HasKey(d => new { d.NU_AGENDA, d.NU_LPN });
            modelBuilder.Entity<V_REC170_LPN_PLANIFICACION>().HasKey(d => new { d.NU_LPN });
            modelBuilder.Entity<V_REC170_RECEPCION_LPNS>().HasKey(d => new { d.NU_AGENDA, d.NU_LPN });
            modelBuilder.Entity<V_STOCK_LPN>().HasKey(d => new { d.NU_LPN, d.ID_LPN_DET, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.CD_EMPRESA, d.CD_FAIXA });
            modelBuilder.Entity<V_STOCK_SUELTO>().HasKey(d => new { d.CD_ENDERECO, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_STO153_CTRL_CALIDAD_DET_LPN>().HasKey(d => new { d.NU_LPN, d.ID_LPN_DET });
            modelBuilder.Entity<V_RESERVA_LPN_ATRIBUTO>().HasKey(d => new { d.CD_ENDERECO, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_ESTILOS_LENGUAJES>().HasKey(d => new { d.CD_LABEL_ESTILO, d.CD_LENGUAJE_IMPRESION });
            modelBuilder.Entity<V_CAMION_CTRL_CONTENEDORES>().HasKey(d => new { d.CD_CAMION, d.NU_CARGA, d.NU_CONTENEDOR });
            modelBuilder.Entity<V_CONTENEDORES_PRODUCCION>().HasKey(d => new { d.NU_CONTENEDOR, d.NU_PREPARACION });
            modelBuilder.Entity<V_PRD110_DETALLES_INGRESO>().HasKey(d => new { d.NU_PRDC_DET_TEORICO });
            modelBuilder.Entity<V_STOCK_DISP_ESPECIFICAR_LOTE>().HasKey(d => new { d.CD_ENDERECO, d.CD_PRODUTO, d.CD_EMPRESA, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_PRODUCTOS_FINALES_PRODUCCION>().HasKey(d => new { d.NU_PRDC_INGRESO, d.CD_PRODUTO, d.CD_EMPRESA });
            modelBuilder.Entity<V_PRD113_PRODUCTOS_ESPERADOS>().HasKey(d => new { d.NU_PRDC_DET_TEORICO, d.CD_PRODUTO, d.CD_EMPRESA, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_PRDC_PLANIFICACION_INSUMO>().HasKey(d => new { d.NU_PRDC_INGRESO, d.CD_ENDERECO, d.CD_PRODUTO, d.CD_EMPRESA, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_PRD112_RESULTADO_TEORICO>().HasKey(d => new { d.NU_PRDC_DET_TEORICO });
            modelBuilder.Entity<V_PRD112_INSUMOS_SOLICITADOS_EN_COMUN>().HasKey(d => new { d.CD_PRDC_LINEA, d.CD_PRODUTO, d.CD_EMPRESA, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.NU_PEDIDO });
            modelBuilder.Entity<V_PRD113_STOCK_INSUMOS>().HasKey(d => new { d.NU_PRDC_INGRESO_REAL, d.NU_PRDC_INGRESO, d.CD_PRODUTO, d.CD_EMPRESA, d.NU_IDENTIFICADOR, d.FL_CONSUMIBLE });
            modelBuilder.Entity<V_PRD113_PRODUCTOS_NO_ESPERADOS>().HasKey(d => new { d.NU_PRDC_SALIDA_REAL });
            modelBuilder.Entity<V_PRD113_PRODUCIR>().HasKey(d => new { d.NU_PRDC_INGRESO, d.NU_PRDC_DET_TEORICO });
            modelBuilder.Entity<V_PRD113_PRODUCTOS_EXPULSABLE>().HasKey(d => new { d.CD_ENDERECO, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_PRD113_STOCK_PRODUCCION>().HasKey(d => new { d.CD_ENDERECO, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<T_PRDC_DET_INGRESO_REAL_MOV>().HasKey(d => new { d.NU_INGRESO_REAL_MOV });
            modelBuilder.Entity<T_PRDC_DET_SALIDA_REAL_MOV>().HasKey(d => new { d.NU_SALIDA_REAL_MOV });
            modelBuilder.Entity<V_CONSUMIDOS_PRODUCCION>().HasKey(d => new { d.NU_PRDC_INGRESO, d.CD_PRODUTO, d.CD_EMPRESA });
            modelBuilder.Entity<V_PRODUCIDOS_PRODUCCION>().HasKey(d => new { d.NU_PRDC_INGRESO, d.CD_PRODUTO, d.CD_EMPRESA });
            modelBuilder.Entity<T_TIPO_MOVIMIENTO>().HasKey(d => new { d.CD_MOVIMIENTO });
            modelBuilder.Entity<V_PRD113_REMANENTE_PRODUCIDO>().HasKey(d => new { d.CD_ENDERECO, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_PRD110_DETALLES_PRODUCIDOS>().HasKey(d => new { d.NU_PRDC_SALIDA_REAL });
            modelBuilder.Entity<V_PRD110_DETALLES_CONSUMIDOS>().HasKey(d => new { d.NU_PRDC_INGRESO_REAL });
            modelBuilder.Entity<V_PRDC_PLANIFICACION_PEDIDO>().HasKey(d => new { d.NU_PRDC_INGRESO, d.CD_PRODUTO, d.CD_EMPRESA, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<T_EVENTO_INSTANCIA_EJECUCION>().HasKey(d => new { d.NU_EVENTO_INSTANCIA });
            modelBuilder.Entity<V_EVENTO_ARCHIVO_WEVT050>().HasKey(d => new { d.NU_EVENTO_NOTIFICACION_ARCHIVO, d.NU_EVENTO_NOTIFICACION });
            modelBuilder.Entity<T_CODIGO_MULTIDATO_EMPRESA_DET>().HasKey(d => new { d.CD_EMPRESA, d.CD_CODIGO_MULTIDATO, d.CD_APLICACION, d.CD_CAMPO, d.CD_AI });
            modelBuilder.Entity<T_CODIGO_MULTIDATO_APLICACION>().HasKey(d => new { d.CD_CODIGO_MULTIDATO, d.CD_APLICACION, d.CD_CAMPO, d.CD_AI });
            modelBuilder.Entity<V_CODIGO_MULTIDATO_EMPRESA_DET>().HasKey(d => new { d.CD_EMPRESA, d.CD_CODIGO_MULTIDATO, d.CD_APLICACION, d.CD_CAMPO, d.CD_AI });
            modelBuilder.Entity<V_CODIGO_MULTIDATO>().HasKey(d => new { d.CD_CODIGO_MULTIDATO });
            modelBuilder.Entity<T_CODIGO_MULTIDATO>().HasKey(d => new { d.CD_CODIGO_MULTIDATO });
            modelBuilder.Entity<V_APLICACION>().HasKey(d => new { d.CD_APLICACION });
            modelBuilder.Entity<V_APLICACION_CAMPO>().HasKey(d => new { d.CD_APLICACION, d.CD_CAMPO });
            modelBuilder.Entity<V_CODIGO_MULTIDATO_DET>().HasKey(d => new { d.CD_CODIGO_MULTIDATO, d.CD_AI });
            modelBuilder.Entity<V_CODIGO_MULTIDATO_ASOCIADOS>().HasKey(d => new { d.CD_EMPRESA, d.CD_CODIGO_MULTIDATO });
            modelBuilder.Entity<T_CODIGO_MULTIDATO_EMPRESA>().HasKey(d => new { d.CD_EMPRESA, d.CD_CODIGO_MULTIDATO });
            modelBuilder.Entity<T_CODIGO_MULTIDATO_DET>().HasKey(d => new { d.CD_CODIGO_MULTIDATO, d.CD_AI });
            modelBuilder.Entity<V_INV411_UBIC_DISP>().HasKey(d => new { d.CD_ENDERECO });
            modelBuilder.Entity<V_INV411_UBIC_SEL>().HasKey(d => new { d.NU_INVENTARIO_ENDERECO });
            modelBuilder.Entity<V_INV416_REG_DISP>().HasKey(d => new { d.CD_ENDERECO, d.CD_PRODUTO, d.CD_EMPRESA, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.NU_LPN });
            modelBuilder.Entity<V_INV416_REG_SEL>().HasKey(d => new { d.NU_INVENTARIO, d.NU_INVENTARIO_ENDERECO, d.NU_INVENTARIO_ENDERECO_DET });
            modelBuilder.Entity<V_INV417_REG_DISP>().HasKey(d => new { d.CD_ENDERECO, d.CD_PRODUTO, d.CD_EMPRESA, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.NU_LPN, d.ID_LPN_DET });
            modelBuilder.Entity<V_INV417_REG_SEL>().HasKey(d => new { d.NU_INVENTARIO, d.NU_INVENTARIO_ENDERECO, d.NU_INVENTARIO_ENDERECO_DET });
            modelBuilder.Entity<V_INV410_LPN_ATRIBUTO_CAB>().HasKey(d => new { d.NU_LPN, d.ID_ATRIBUTO });
            modelBuilder.Entity<V_INV410_LPN_DET_ATRIBUTO_CAB>().HasKey(d => new { d.ID_LPN_DET, d.NU_LPN, d.CD_PRODUTO, d.CD_FAIXA, d.CD_EMPRESA, d.NU_IDENTIFICADOR, d.ID_ATRIBUTO });
            modelBuilder.Entity<V_INV410_LPN_DET_ATRIBUTO_DET>().HasKey(d => new { d.ID_LPN_DET, d.NU_LPN, d.CD_PRODUTO, d.CD_FAIXA, d.CD_EMPRESA, d.NU_IDENTIFICADOR, d.ID_ATRIBUTO });
            modelBuilder.Entity<T_INVENTARIO_ENDERECO_DET_ATR>().HasKey(d => new { d.NU_INVENTARIO_ENDERECO_DET, d.ID_ATRIBUTO });
            modelBuilder.Entity<V_INV418_ATRIBUTOS>().HasKey(d => new { d.NU_INVENTARIO_ENDERECO_DET, d.ID_ATRIBUTO });
            modelBuilder.Entity<V_REPORTE_PACKING_LIST_SIN_LPN>().HasKey(d => new { d.CD_CAMION, d.NU_PREPARACION, d.NU_CONTENEDOR, d.CD_EMPRESA, d.NU_PEDIDO, d.CD_CLIENTE, d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.VL_VENCIMIENTO });
            modelBuilder.Entity<V_PRD111_STOCK_PRODUCCION>().HasKey(d => new { d.CD_ENDERECO, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR });

            modelBuilder.Entity<T_RECORRIDO>().HasKey(d => new { d.NU_RECORRIDO });
            modelBuilder.Entity<T_RECORRIDO_DET>().HasKey(d => new { d.NU_RECORRIDO_DET });
            modelBuilder.Entity<T_APLICACION>().HasKey(d => new { d.CD_APLICACION });
            modelBuilder.Entity<T_APLICACION_RECORRIDO>().HasKey(d => new { d.NU_RECORRIDO, d.CD_APLICACION });
            modelBuilder.Entity<T_APLICACION_RECORRIDO_USUARIO>().HasKey(d => new { d.NU_RECORRIDO, d.CD_APLICACION, d.USERID });

            modelBuilder.Entity<V_REG700_RECORRIDOS>().HasKey(d => new { d.NU_RECORRIDO });
            modelBuilder.Entity<V_REG700_RECORRIDO_DET>().HasKey(d => new { d.NU_RECORRIDO_DET });
            modelBuilder.Entity<V_REG700_UBIC_SIN_RECORRIDOS>().HasKey(d => new { d.NU_RECORRIDO, d.CD_ENDERECO });
            modelBuilder.Entity<V_REG700_APLICACION_ASO>().HasKey(d => new { d.NU_RECORRIDO, d.CD_APLICACION });
            modelBuilder.Entity<V_REG700_APLICACION_DISP>().HasKey(d => new { d.NU_RECORRIDO, d.CD_APLICACION });
            modelBuilder.Entity<V_REG700_APLICACION_USER_DISP>().HasKey(d => new { d.NU_RECORRIDO, d.CD_APLICACION, d.USERID });
            modelBuilder.Entity<V_REG700_APLICACION_USER_ASO>().HasKey(d => new { d.NU_RECORRIDO, d.CD_APLICACION, d.USERID });
            modelBuilder.Entity<V_APLICACION_RECORRIDO>().HasKey(d => new { d.NU_RECORRIDO, d.CD_APLICACION });
            modelBuilder.Entity<V_APLICACION_RECORRIDO_USUARIO>().HasKey(d => new { d.NU_RECORRIDO, d.CD_APLICACION, d.USERID });
            modelBuilder.Entity<T_ALM_DET_PRODUTO_TEMP>().HasKey(d => new { d.NU_ETIQUETA_LOTE, d.USERID, d.CD_PRODUTO, d.CD_EMPRESA, d.NU_IDENTIFICADOR });

            modelBuilder.Entity<T_COLA_TRABAJO>().HasKey(d => new { d.NU_COLA_TRABAJO });
            modelBuilder.Entity<T_COLA_TRABAJO_PONDERADOR>().HasKey(d => new { d.NU_COLA_TRABAJO, d.CD_PONDERADOR });
            modelBuilder.Entity<T_COLA_TRABAJO_PONDERADOR_DET>().HasKey(d => new { d.NU_COLA_TRABAJO, d.CD_PONDERADOR, d.CD_INST_PONDERADOR });
            modelBuilder.Entity<T_COLA_TRABAJO_PONDERADOR_INST>().HasKey(d => new { d.CD_PONDERADOR });
            modelBuilder.Entity<T_COLA_PICKING>().HasKey(d => new { d.NU_COLA_PICKING });
            modelBuilder.Entity<V_COLA_TRABAJO_PONDERADOR_DET>().HasKey(d => new { d.CD_INST_PONDERADOR, d.CD_PONDERADOR, d.NU_COLA_TRABAJO });
            modelBuilder.Entity<V_COLA_TRABAJO_POND_EMPRESAS>().HasKey(d => new { d.CD_EMPRESA });
            modelBuilder.Entity<V_COLA_TRABAJO_POND_CLIENTES>().HasKey(d => new { d.CD_EMPRESA, d.CD_CLIENTE });
            modelBuilder.Entity<V_COLA_TRABAJO_POND_RUTAS>().HasKey(d => new { d.CD_ROTA });
            modelBuilder.Entity<V_COLA_TRABAJO_POND_ZONAS>().HasKey(d => new { d.CD_ZONA });
            modelBuilder.Entity<V_COLA_TRABAJO_POND_TP_EXP>().HasKey(d => new { d.TP_EXPEDICION });
            modelBuilder.Entity<V_COLA_TRABAJO_POND_TP_PED>().HasKey(d => new { d.TP_PEDIDO });
            modelBuilder.Entity<V_COLA_TRABAJO_POND_CON_LIB>().HasKey(d => new { d.CD_CONDICION_LIBERACION });
            modelBuilder.Entity<V_SEG_COLA_TRABAJO_PRE812>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA });
            modelBuilder.Entity<V_PRE811_PREF_CLASSE>().HasKey(d => new { d.CD_CLASSE, d.NU_PREFERENCIA });
            modelBuilder.Entity<V_PRE811_PREF_COND_LIB>().HasKey(d => new { d.CD_CONDICION_LIBERACION, d.NU_PREFERENCIA });
            modelBuilder.Entity<V_PRE811_PREF_EMPRESA>().HasKey(d => new { d.CD_EMPRESA, d.NU_PREFERENCIA });
            modelBuilder.Entity<V_PRE811_PREF_FAMILIA>().HasKey(d => new { d.CD_FAMILIA_PRODUTO, d.NU_PREFERENCIA });
            modelBuilder.Entity<V_PRE811_PREF_ROTA>().HasKey(d => new { d.CD_ROTA, d.NU_PREFERENCIA });
            modelBuilder.Entity<V_PRE811_PREF_TP_EXP>().HasKey(d => new { d.TP_EXPEDICION, d.NU_PREFERENCIA });
            modelBuilder.Entity<V_PRE811_PREF_TP_PEDIDO>().HasKey(d => new { d.TP_PEDIDO, d.NU_PREFERENCIA });
            modelBuilder.Entity<V_PRE811_PREFERENCIAS>().HasKey(d => new { d.NU_PREFERENCIA });
            modelBuilder.Entity<V_PRE811_PREF_CONT_ACCESO>().HasKey(d => new { d.CD_CONTROL_ACCESO, d.NU_PREFERENCIA });
            modelBuilder.Entity<V_ZOOMIN_PRE812>().HasKey(d => new { d.CD_PRODUTO, d.CD_EMPRESA, d.NU_IDENTIFICADOR, d.CD_FAIXA, d.NU_PREPARACION, d.NU_PEDIDO, d.CD_CLIENTE, d.CD_ENDERECO, d.NU_SEQ_PREPARACION });
            modelBuilder.Entity<V_CALCULO_PONDERACION_PRE812>().HasKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA, d.CD_PONDEREDOR });
            modelBuilder.Entity<V_PRE811_PREF_EQUIPO>().HasKey(d => new { d.CD_EQUIPO, d.NU_PREFERENCIA });
            modelBuilder.Entity<V_PRE811_PREF_USUARIO>().HasKey(d => new { d.USERID, d.NU_PREFERENCIA });
            modelBuilder.Entity<V_PRE811_PREF_CLIENTE>().HasKey(d => new { d.NU_PREFERENCIA, d.CD_CLIENTE, d.CD_EMPRESA });
            modelBuilder.Entity<T_PREFERENCIA>().HasKey(d => new { d.NU_PREFERENCIA });
            modelBuilder.Entity<T_PREFERENCIA_CLASSE>().HasKey(d => new { d.NU_PREFERENCIA, d.CD_CLASSE });
            modelBuilder.Entity<T_PREFERENCIA_COND_LIBERACION>().HasKey(d => new { d.NU_PREFERENCIA, d.CD_CONDICION_LIBERACION });
            modelBuilder.Entity<T_PREFERENCIA_EMPRESA>().HasKey(d => new { d.NU_PREFERENCIA, d.CD_EMPRESA });
            modelBuilder.Entity<T_PREFERENCIA_FAMILIA_PRODUTO>().HasKey(d => new { d.NU_PREFERENCIA, d.CD_FAMILIA_PRODUTO });
            modelBuilder.Entity<T_PREFERENCIA_ROTA>().HasKey(d => new { d.NU_PREFERENCIA, d.CD_ROTA });
            modelBuilder.Entity<T_PREFERENCIA_TIPO_EXPEDICION>().HasKey(d => new { d.NU_PREFERENCIA, d.TP_EXPEDICION });
            modelBuilder.Entity<T_PREFERENCIA_TIPO_PEDIDO>().HasKey(d => new { d.NU_PREFERENCIA, d.TP_PEDIDO });
            modelBuilder.Entity<T_PREFERENCIA_ZONA>().HasKey(d => new { d.NU_PREFERENCIA, d.CD_ZONA });
            modelBuilder.Entity<T_PREFERENCIA_CLIENTE>().HasKey(d => new { d.NU_PREFERENCIA, d.CD_CLIENTE, d.CD_EMPRESA });
            modelBuilder.Entity<T_PREFERENCIA_CONTROL_ACCESO>().HasKey(d => new { d.NU_PREFERENCIA, d.CD_CONTROL_ACCESO });
            modelBuilder.Entity<T_PREFERENCIA_REL>().HasKey(d => new { d.NU_PREFERENCIA_REL });
            modelBuilder.Entity<V_REC400_FACTURAS>().HasKey(d => new { d.NU_RECEPCION_FACTURA });
            modelBuilder.Entity<V_REC500_FACTURA_AGENDA>().HasKey(d => new { d.NU_AGENDA, d.NU_RECEPCION_FACTURA });
            modelBuilder.Entity<T_RECEPCION_FACTURA>().HasKey(d => new { d.NU_RECEPCION_FACTURA });
            modelBuilder.Entity<T_RECEPCION_FACTURA_DET>().HasKey(d => new { d.NU_RECEPCION_FACTURA_DET });
            modelBuilder.Entity<T_RECEPCION_FACTURA_DET>().HasOne(e => e.T_RECEPCION_FACTURA).WithMany(e => e.T_RECEPCION_FACTURA_DET).HasForeignKey(d => d.NU_RECEPCION_FACTURA);
            modelBuilder.Entity<T_TRASPASO_CONFIGURACION>().HasKey(d => new { d.NU_TRASPASO_CONFIGURACION });
            modelBuilder.Entity<T_TRASPASO_CONF_DESTINO>().HasKey(d => new { d.NU_TRASPASO_CONF_DESTINO });
            modelBuilder.Entity<T_TRASPASO_CONF_TIPO_TRASPASO>().HasKey(d => new { d.NU_TRASPASO_CONF_TIPO_TRASPASO });
            modelBuilder.Entity<T_TRASPASO_MAPEO_PRODUTO>().HasKey(d => new { d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.CD_EMPRESA_DESTINO });
            modelBuilder.Entity<T_TRASPASO>().HasKey(d => new { d.NU_TRASPASO });
            modelBuilder.Entity<T_TRASPASO_DET_PEDIDO>().HasKey(d => new { d.NU_TRASPASO_DET_PEDIDO });
            modelBuilder.Entity<V_PRE052_STOCK_SUELTO>().HasKey(d => new { d.CD_ENDERECO, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR });
            modelBuilder.Entity<V_PRE052_STOCK_LPN>().HasKey(d => new { d.NU_LPN });
            modelBuilder.Entity<T_CLIENTE_DIASVALIDEZ_VENTANA>().HasKey(d => new { d.CD_CLIENTE, d.CD_EMPRESA, d.CD_VENTANA_LIBERACION });
            modelBuilder.Entity<V_CLIENTE_CONFIG_DIAS_VALIDEZ>().HasKey(d => new { d.CD_CLIENTE, d.CD_EMPRESA, d.CD_VENTANA_LIBERACION });
            #endregion

            #region Relations

            modelBuilder.Entity<PROFILERESOURCES>().HasOne(e => e.PROFILES).WithMany(e => e.PROFILERESOURCES).HasForeignKey(d => d.PROFILEID);
            modelBuilder.Entity<PROFILERESOURCES>().HasOne(e => e.RESOURCES).WithMany(e => e.PROFILERESOURCES).HasForeignKey(d => d.RESOURCEID);
            modelBuilder.Entity<PROFILES>().HasOne(e => e.USERTYPES).WithMany(e => e.PROFILES).HasForeignKey(d => d.USERTYPEID);
            modelBuilder.Entity<PROFILES>().HasMany(e => e.PROFILERESOURCES).WithOne(e => e.PROFILES).HasForeignKey(d => d.PROFILEID);
            modelBuilder.Entity<PROFILES>().HasMany(e => e.USERPERMISSIONS).WithOne(e => e.PROFILES).HasForeignKey(d => d.PROFILEID);
            modelBuilder.Entity<RESOURCES>().HasOne(e => e.USERTYPES).WithMany(e => e.RESOURCES).HasForeignKey(d => d.USERTYPEID);
            modelBuilder.Entity<RESOURCES>().HasMany(e => e.PROFILERESOURCES).WithOne(e => e.RESOURCES).HasForeignKey(d => d.RESOURCEID);
            modelBuilder.Entity<RESOURCES>().HasMany(e => e.USERPERMISSIONS).WithOne(e => e.RESOURCES).HasForeignKey(d => d.RESOURCEID);
            modelBuilder.Entity<T_AGENDA>().HasMany(e => e.T_DET_AGENDA).WithOne(e => e.T_AGENDA).HasForeignKey(d => d.NU_AGENDA);
            modelBuilder.Entity<T_AGENDA>().HasMany(e => e.T_RECEPC_AGENDA_CONTAINER_REL).WithOne(e => e.T_AGENDA).HasForeignKey(d => d.NU_AGENDA);
            modelBuilder.Entity<T_AGENDA>().HasMany(e => e.T_RECEPC_AGENDA_REFERENCIA_REL).WithOne(e => e.T_AGENDA).HasForeignKey(d => d.NU_AGENDA);
            modelBuilder.Entity<T_ARCHIVO_ADJUNTO>().HasOne(e => e.T_ARCHIVO_MANEJO).WithMany(e => e.T_ARCHIVO_ADJUNTO).HasForeignKey(d => d.CD_MANEJO);
            modelBuilder.Entity<T_ARCHIVO_ADJUNTO>().HasMany(e => e.T_ARCHIVO_ADJUNTO_VERSION).WithOne(e => e.T_ARCHIVO_ADJUNTO).HasForeignKey(d => new { d.NU_ARCHIVO_ADJUNTO, d.CD_EMPRESA, d.CD_MANEJO, d.DS_REFERENCIA });
            modelBuilder.Entity<T_ARCHIVO_ADJUNTO_VERSION>().HasOne(e => e.T_ARCHIVO_ADJUNTO).WithMany(e => e.T_ARCHIVO_ADJUNTO_VERSION).HasForeignKey(d => new { d.NU_ARCHIVO_ADJUNTO, d.CD_EMPRESA, d.CD_MANEJO, d.DS_REFERENCIA });
            modelBuilder.Entity<T_ARCHIVO_DOCUMENTO>().HasMany(e => e.T_ARCHIVO_MANEJO_DOCUMENTO).WithOne(e => e.T_ARCHIVO_DOCUMENTO).HasForeignKey(d => d.CD_DOCUMENTO);
            modelBuilder.Entity<T_ARCHIVO_MANEJO>().HasMany(e => e.T_ARCHIVO_MANEJO_DOCUMENTO).WithOne(e => e.T_ARCHIVO_MANEJO).HasForeignKey(d => d.CD_MANEJO);
            modelBuilder.Entity<T_ARCHIVO_MANEJO_DOCUMENTO>().HasOne(e => e.T_ARCHIVO_DOCUMENTO).WithMany(e => e.T_ARCHIVO_MANEJO_DOCUMENTO).HasForeignKey(d => d.CD_DOCUMENTO);
            modelBuilder.Entity<T_ARCHIVO_MANEJO_DOCUMENTO>().HasOne(e => e.T_ARCHIVO_MANEJO).WithMany(e => e.T_ARCHIVO_MANEJO_DOCUMENTO).HasForeignKey(d => d.CD_MANEJO);
            modelBuilder.Entity<T_CAMION>().HasMany(e => e.T_CLIENTE_CAMION).WithOne(e => e.T_CAMION).HasForeignKey(d => d.CD_CAMION);
            modelBuilder.Entity<T_CLASSE>().HasOne(e => e.T_SUB_CLASSE).WithMany(e => e.T_CLASSE).HasForeignKey(d => d.CD_SUB_CLASSE);
            modelBuilder.Entity<T_CLIENTE>().HasOne(e => e.T_PAIS_SUBDIVISION_LOCALIDAD).WithMany(e => e.T_CLIENTE).HasForeignKey(d => d.ID_LOCALIDAD);
            modelBuilder.Entity<T_CLIENTE>().HasOne(e => e.T_ROTA).WithMany(e => e.T_CLIENTE).HasForeignKey(d => d.CD_ROTA);
            modelBuilder.Entity<T_CLIENTE>().HasMany(e => e.T_CLIENTE_RUTA_PREDIO).WithOne(e => e.T_CLIENTE).HasForeignKey(d => new { d.CD_EMPRESA, d.CD_CLIENTE });
            modelBuilder.Entity<T_CLIENTE_CAMION>().HasOne(e => e.T_CAMION).WithMany(e => e.T_CLIENTE_CAMION).HasForeignKey(d => d.CD_CAMION);
            modelBuilder.Entity<T_CLIENTE_RUTA_PREDIO>().HasOne(e => e.T_ROTA).WithMany(e => e.T_CLIENTE_RUTA_PREDIO).HasForeignKey(d => d.CD_ROTA);
            modelBuilder.Entity<T_CLIENTE_RUTA_PREDIO>().HasOne(e => e.T_CLIENTE).WithMany(e => e.T_CLIENTE_RUTA_PREDIO).HasForeignKey(d => new { d.CD_EMPRESA, d.CD_CLIENTE });
            modelBuilder.Entity<T_CODIGO_BARRAS>().HasOne(e => e.T_TIPO_CODIGO_BARRAS).WithMany(e => e.T_CODIGO_BARRAS).HasForeignKey(d => d.TP_CODIGO_BARRAS);
            modelBuilder.Entity<T_CONTACTO>().HasMany(e => e.T_CONTACTO_GRUPO_REL).WithOne(e => e.T_CONTACTO).HasForeignKey(d => d.NU_CONTACTO);
            modelBuilder.Entity<T_CONTACTO>().HasMany(e => e.T_EVENTO_GRUPO_INSTANCIA_REL).WithOne(e => e.T_CONTACTO).HasForeignKey(d => d.NU_CONTACTO);
            modelBuilder.Entity<T_CONTACTO_GRUPO>().HasMany(e => e.T_CONTACTO_GRUPO_REL).WithOne(e => e.T_CONTACTO_GRUPO).HasForeignKey(d => d.NU_CONTACTO_GRUPO);
            modelBuilder.Entity<T_CONTACTO_GRUPO>().HasMany(e => e.T_EVENTO_GRUPO_INSTANCIA_REL).WithOne(e => e.T_CONTACTO_GRUPO).HasForeignKey(d => d.NU_CONTACTO_GRUPO);
            modelBuilder.Entity<T_CONTACTO_GRUPO_REL>().HasOne(e => e.T_CONTACTO_GRUPO).WithMany(e => e.T_CONTACTO_GRUPO_REL).HasForeignKey(d => d.NU_CONTACTO_GRUPO);
            modelBuilder.Entity<T_CONTACTO_GRUPO_REL>().HasOne(e => e.T_CONTACTO).WithMany(e => e.T_CONTACTO_GRUPO_REL).HasForeignKey(d => d.NU_CONTACTO);
            modelBuilder.Entity<T_CONTAINER>().HasMany(e => e.T_RECEPC_AGENDA_CONTAINER_REL).WithOne(e => e.T_CONTAINER).HasForeignKey(d => new { d.NU_CONTAINER, d.NU_SEQ_CONTAINER });
            modelBuilder.Entity<T_CONTENEDOR>().HasOne(e => e.T_PICKING).WithMany(e => e.T_CONTENEDOR).HasForeignKey(d => d.NU_PREPARACION);
            modelBuilder.Entity<T_CONTENEDOR>().HasMany(e => e.T_DET_PICKING).WithOne(e => e.T_CONTENEDOR).HasForeignKey(d => new { d.NU_PREPARACION, d.NU_CONTENEDOR });
            modelBuilder.Entity<T_CROSS_DOCK>().HasMany(e => e.T_DET_CROSS_DOCK).WithOne(e => e.T_CROSS_DOCK).HasForeignKey(d => new { d.NU_AGENDA, d.NU_PREPARACION });
            modelBuilder.Entity<T_DET_AGENDA>().HasOne(e => e.T_AGENDA).WithMany(e => e.T_DET_AGENDA).HasForeignKey(d => d.NU_AGENDA);
            modelBuilder.Entity<T_DET_CROSS_DOCK>().HasOne(e => e.T_CROSS_DOCK).WithMany(e => e.T_DET_CROSS_DOCK).HasForeignKey(d => new { d.NU_AGENDA, d.NU_PREPARACION });
            modelBuilder.Entity<T_DET_DOCUMENTO>().HasOne(e => e.T_DOCUMENTO).WithMany(e => e.T_DET_DOCUMENTO).HasForeignKey(d => new { d.NU_DOCUMENTO, d.TP_DOCUMENTO });
            modelBuilder.Entity<T_DET_DOCUMENTO_ACTA>().HasOne(e => e.T_DOCUMENTO_INGRESO).WithMany(e => e.T_DET_DOCUMENTO_ACTA_INGRESO).HasForeignKey(d => new { d.NU_DOCUMENTO, d.TP_DOCUMENTO });
            modelBuilder.Entity<T_DET_DOCUMENTO_EGRESO>().HasOne(e => e.T_DOCUMENTO_INGRESO).WithMany(e => e.T_DET_DOCUMENTO_EGRESO_INGRESO).HasForeignKey(d => new { d.NU_DOCUMENTO_INGRESO, d.TP_DOCUMENTO_INGRESO });
            modelBuilder.Entity<T_DET_DOCUMENTO_EGRESO>().HasMany(e => e.T_DET_DOCU_EGRESO_RESERV).WithOne(e => e.T_DET_DOCUMENTO_EGRESO).HasForeignKey(d => new { d.NU_DOCUMENTO, d.TP_DOCUMENTO, d.NU_SECUENCIA });
            modelBuilder.Entity<T_DET_DOMINIO>().HasOne(e => e.T_DOMINIO).WithMany(e => e.T_DET_DOMINIO).HasForeignKey(d => d.CD_DOMINIO);
            modelBuilder.Entity<T_DET_ETIQUETA_LOTE>().HasOne(e => e.T_ETIQUETA_LOTE).WithMany(e => e.T_DET_ETIQUETA_LOTE).HasForeignKey(d => d.NU_ETIQUETA_LOTE);
            modelBuilder.Entity<T_DET_ETIQUETA_LOTE>().HasOne(e => e.T_PRODUTO).WithMany(e => e.T_DET_ETIQUETA_LOTE).HasForeignKey(d => new { d.CD_EMPRESA, d.CD_PRODUTO });
            modelBuilder.Entity<T_DET_PEDIDO_SAIDA>().HasOne(e => e.T_PEDIDO_SAIDA).WithMany(e => e.T_DET_PEDIDO_SAIDA).HasForeignKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA });
            modelBuilder.Entity<T_DET_PICKING>().HasOne(e => e.T_PICKING).WithMany(e => e.T_DET_PICKING).HasForeignKey(d => d.NU_PREPARACION);
            modelBuilder.Entity<T_DET_PICKING>().HasOne(e => e.T_CONTENEDOR).WithMany(e => e.T_DET_PICKING).HasForeignKey(d => new { d.NU_PREPARACION, d.NU_CONTENEDOR });
            modelBuilder.Entity<T_DOCUMENTO>().HasMany(e => e.T_DET_DOCUMENTO).WithOne(e => e.T_DOCUMENTO).HasForeignKey(d => new { d.NU_DOCUMENTO, d.TP_DOCUMENTO });
            modelBuilder.Entity<T_DOCUMENTO>().HasMany(e => e.T_DET_DOCUMENTO_ACTA).WithOne(e => e.T_DOCUMENTO).HasForeignKey(d => new { d.NU_ACTA, d.TP_ACTA });
            modelBuilder.Entity<T_DOCUMENTO>().HasMany(e => e.T_DET_DOCUMENTO_EGRESO).WithOne(e => e.T_DOCUMENTO).HasForeignKey(d => new { d.NU_DOCUMENTO, d.TP_DOCUMENTO });
            modelBuilder.Entity<T_DOCUMENTO>().HasMany(e => e.T_DOCUMENTO_PREPARACION_RESERV).WithOne(e => e.T_DOCUMENTO).HasForeignKey(d => new { d.NU_DOCUMENTO, d.TP_DOCUMENTO });
            modelBuilder.Entity<T_DOCUMENTO>().HasMany(e => e.LT_DELETE_DOCUMENTO_PREPARACION_RESERV).WithOne(e => e.T_DOCUMENTO).HasForeignKey(d => new { d.NU_DOCUMENTO, d.TP_DOCUMENTO });
            modelBuilder.Entity<T_DOCUMENTO>().HasOne(e => e.T_DOCUMENTO_TIPO).WithMany(e => e.T_DOCUMENTO).HasForeignKey(d => d.TP_DOCUMENTO);
            modelBuilder.Entity<T_DOCUMENTO>().HasOne(e => e.T_DOCUMENTO_TIPO_ESTADO).WithMany(e => e.T_DOCUMENTO).HasForeignKey(d => new { d.TP_DOCUMENTO, d.ID_ESTADO });
            modelBuilder.Entity<T_DOCUMENTO_PREPARACION_RESERV>().HasOne(e => e.T_DOCUMENTO).WithMany(e => e.T_DOCUMENTO_PREPARACION_RESERV).HasForeignKey(d => new { d.NU_DOCUMENTO, d.TP_DOCUMENTO });
            modelBuilder.Entity<LT_DELETE_DOCUMENTO_PREPARACION_RESERV>().HasOne(e => e.T_DOCUMENTO).WithMany(e => e.LT_DELETE_DOCUMENTO_PREPARACION_RESERV).HasForeignKey(d => new { d.NU_DOCUMENTO, d.TP_DOCUMENTO });
            modelBuilder.Entity<T_DOCUMENTO_PRODUCCION>().HasOne(e => e.T_DOCUMENTO_INGRESO).WithMany(e => e.T_DOCUMENTO_PRODUCCION_INGRESO).HasForeignKey(d => new { d.NU_DOCUMENTO_ING, d.TP_DOCUMENTO_ING });
            modelBuilder.Entity<T_DOCUMENTO_PRODUCCION>().HasOne(e => e.T_DOCUMENTO_EGRESO).WithMany(e => e.T_DOCUMENTO_PRODUCCION_EGRESO).HasForeignKey(d => new { d.NU_DOCUMENTO_EGR, d.TP_DOCUMENTO_EGR });
            modelBuilder.Entity<T_DOCUMENTO_TRANSFERENCIA>().HasOne(e => e.T_DOCUMENTO_INGRESO).WithMany(e => e.T_DOCUMENTO_TRANSFERENCIA_INGRESO).HasForeignKey(d => new { d.NU_DOCUMENTO_ING, d.TP_DOCUMENTO_ING });
            modelBuilder.Entity<T_DOCUMENTO_TRANSFERENCIA>().HasOne(e => e.T_DOCUMENTO_EGRESO).WithMany(e => e.T_DOCUMENTO_TRANSFERENCIA_EGRESO).HasForeignKey(d => new { d.NU_DOCUMENTO_EGR, d.TP_DOCUMENTO_EGR });
            modelBuilder.Entity<T_DOMINIO>().HasMany(e => e.T_DET_DOMINIO).WithOne(e => e.T_DOMINIO).HasForeignKey(d => d.CD_DOMINIO);
            modelBuilder.Entity<T_EMPRESA>().HasMany(e => e.T_EMPRESA_FUNCIONARIO).WithOne(e => e.T_EMPRESA).HasForeignKey(d => d.CD_EMPRESA);
            modelBuilder.Entity<T_EMPRESA>().HasMany(e => e.T_RECEPCION_REL_EMPRESA_TIPO).WithOne(e => e.T_EMPRESA).HasForeignKey(d => d.CD_EMPRESA);
            modelBuilder.Entity<T_EMPRESA>().HasMany(e => e.T_PRDC_DEFINICION).WithOne(e => e.T_EMPRESA).HasForeignKey(d => d.CD_EMPRESA);
            modelBuilder.Entity<T_EMPRESA_FUNCIONARIO>().HasOne(e => e.T_EMPRESA).WithMany(e => e.T_EMPRESA_FUNCIONARIO).HasForeignKey(d => d.CD_EMPRESA);
            modelBuilder.Entity<T_EMPRESA_FUNCIONARIO>().HasOne(e => e.USERS).WithMany(e => e.T_EMPRESA_FUNCIONARIO).HasForeignKey(d => d.USERID);
            modelBuilder.Entity<T_ENDERECO_ESTOQUE>().HasOne(e => e.T_EMPRESA).WithMany(e => e.T_ENDERECO_ESTOQUE).HasForeignKey(d => d.CD_EMPRESA);
            modelBuilder.Entity<T_ENDERECO_ESTOQUE>().HasOne(e => e.T_CONTROL_ACCESO).WithMany(e => e.T_ENDERECO_ESTOQUE).HasForeignKey(d => d.CD_CONTROL_ACCESO);
            modelBuilder.Entity<T_ENDERECO_ESTOQUE>().HasOne(e => e.T_ZONA_UBICACION).WithMany(e => e.T_ENDERECO_ESTOQUE).HasForeignKey(d => d.CD_ZONA_UBICACION);
            modelBuilder.Entity<T_ENDERECO_ESTOQUE>().HasMany(e => e.T_PICKING_PRODUTO).WithOne(e => e.T_ENDERECO_ESTOQUE).HasForeignKey(d => d.CD_ENDERECO_SEPARACAO);
            modelBuilder.Entity<T_ENDERECO_ESTOQUE>().HasMany(e => e.T_PRDC_LINEA_ENTRADA).WithOne(e => e.T_ENDERECO_ESTOQUE_ENTRADA).HasForeignKey(d => d.CD_ENDERECO_ENTRADA);
            modelBuilder.Entity<T_ENDERECO_ESTOQUE>().HasMany(e => e.T_PRDC_LINEA_SALIDA).WithOne(e => e.T_ENDERECO_ESTOQUE_SALIDA).HasForeignKey(d => d.CD_ENDERECO_SALIDA);
            modelBuilder.Entity<T_ENDERECO_ESTOQUE>().HasMany(e => e.T_PRDC_LINEA_SALIDA_TRAN).WithOne(e => e.T_ENDERECO_ESTOQUE_SALIDA_TRAN).HasForeignKey(d => d.CD_ENDERECO_SALIDA_TRAN);
            modelBuilder.Entity<T_ENDERECO_ESTOQUE>().HasMany(e => e.T_PRDC_LINEA_PRODUCCION).WithOne(e => e.T_ENDERECO_ESTOQUE_PRODUCCION).HasForeignKey(d => d.CD_ENDERECO_PRODUCCION);
            modelBuilder.Entity<T_ENDERECO_ESTOQUE>().HasMany(e => e.T_ETIQUETA_LOTE).WithOne(e => e.T_ENDERECO_ESTOQUE).HasForeignKey(d => d.CD_ENDERECO);
            modelBuilder.Entity<T_ENDERECO_ESTOQUE>().HasMany(e => e.T_ETIQUETA_LOTE_SUGERIDO).WithOne(e => e.T_ENDERECO_ESTOQUE_SUGERIDO).HasForeignKey(d => d.CD_ENDERECO_SUGERIDO);
            modelBuilder.Entity<T_ENDERECO_ESTOQUE>().HasOne(e => e.T_TIPO_ENDERECO).WithMany(e => e.T_ENDERECO_ESTOQUE).HasForeignKey(d => d.CD_TIPO_ENDERECO);
            modelBuilder.Entity<T_ENDERECO_ESTOQUE>().HasOne(e => e.T_TIPO_AREA).WithMany(e => e.T_ENDERECO_ESTOQUE).HasForeignKey(d => d.CD_AREA_ARMAZ);
            modelBuilder.Entity<T_TIPO_ENDERECO>().HasMany(e => e.T_TIPO_ENDERECO_PALLET).WithOne(e => e.T_TIPO_ENDERECO).HasForeignKey(d => d.CD_TIPO_ENDERECO);
            modelBuilder.Entity<T_STOCK>().HasOne(e => e.T_PRODUTO_FAIXA).WithMany(e => e.T_STOCK).HasForeignKey(d => new { d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA });
            modelBuilder.Entity<T_PRODUTO_FAIXA>().HasOne(e => e.T_PRODUTO).WithMany(e => e.T_PRODUTO_FAIXA).HasForeignKey(d => new { d.CD_EMPRESA, d.CD_PRODUTO });
            modelBuilder.Entity<T_ETIQUETA_LOTE>().HasOne(e => e.T_ENDERECO_ESTOQUE).WithMany(e => e.T_ETIQUETA_LOTE).HasForeignKey(d => d.CD_ENDERECO);
            modelBuilder.Entity<T_ETIQUETA_LOTE>().HasOne(e => e.T_ENDERECO_ESTOQUE_SUGERIDO).WithMany(e => e.T_ETIQUETA_LOTE_SUGERIDO).HasForeignKey(d => d.CD_ENDERECO_SUGERIDO);
            modelBuilder.Entity<T_ETIQUETA_LOTE>().HasOne(e => e.T_AGENDA).WithMany(e => e.T_ETIQUETA_LOTE).HasForeignKey(d => d.NU_AGENDA);
            modelBuilder.Entity<T_ETIQUETA_LOTE>().HasMany(e => e.T_DET_ETIQUETA_LOTE).WithOne(e => e.T_ETIQUETA_LOTE).HasForeignKey(d => d.NU_ETIQUETA_LOTE);
            modelBuilder.Entity<T_EVENTO>().HasMany(e => e.T_EVENTO_PARAMETRO_INSTANCIA).WithOne(e => e.T_EVENTO).HasForeignKey(d => d.NU_EVENTO);
            modelBuilder.Entity<T_EVENTO_BANDEJA>().HasOne(e => e.T_EVENTO).WithMany(e => e.T_EVENTO_BANDEJA).HasForeignKey(d => d.NU_EVENTO);
            modelBuilder.Entity<T_EVENTO_BANDEJA>().HasMany(e => e.T_EVENTO_BANDEJA_INSTANCIA).WithOne(e => e.T_EVENTO_BANDEJA).HasForeignKey(d => d.NU_EVENTO_BANDEJA);
            modelBuilder.Entity<T_EVENTO_BANDEJA_INSTANCIA>().HasOne(e => e.T_EVENTO_BANDEJA).WithMany(e => e.T_EVENTO_BANDEJA_INSTANCIA).HasForeignKey(d => d.NU_EVENTO_BANDEJA);
            modelBuilder.Entity<T_EVENTO_BANDEJA_INSTANCIA>().HasOne(e => e.T_EVENTO_INSTANCIA).WithMany(e => e.T_EVENTO_BANDEJA_INSTANCIA).HasForeignKey(d => d.NU_EVENTO_INSTANCIA);
            modelBuilder.Entity<T_EVENTO_GRUPO_INSTANCIA_REL>().HasOne(e => e.T_CONTACTO).WithMany(e => e.T_EVENTO_GRUPO_INSTANCIA_REL).HasForeignKey(d => d.NU_CONTACTO);
            modelBuilder.Entity<T_EVENTO_GRUPO_INSTANCIA_REL>().HasOne(e => e.T_CONTACTO_GRUPO).WithMany(e => e.T_EVENTO_GRUPO_INSTANCIA_REL).HasForeignKey(d => d.NU_CONTACTO_GRUPO);
            modelBuilder.Entity<T_EVENTO_GRUPO_INSTANCIA_REL>().HasOne(e => e.T_EVENTO_INSTANCIA).WithMany(e => e.T_EVENTO_GRUPO_INSTANCIA_REL).HasForeignKey(d => d.NU_EVENTO_INSTANCIA);
            modelBuilder.Entity<T_EVENTO_INSTANCIA>().HasOne(e => e.T_EVENTO).WithMany(e => e.T_EVENTO_INSTANCIA).HasForeignKey(d => d.NU_EVENTO);
            modelBuilder.Entity<T_EVENTO_INSTANCIA>().HasMany(e => e.T_EVENTO_BANDEJA_INSTANCIA).WithOne(e => e.T_EVENTO_INSTANCIA).HasForeignKey(d => d.NU_EVENTO_INSTANCIA);
            modelBuilder.Entity<T_EVENTO_INSTANCIA>().HasMany(e => e.T_EVENTO_GRUPO_INSTANCIA_REL).WithOne(e => e.T_EVENTO_INSTANCIA).HasForeignKey(d => d.NU_EVENTO_INSTANCIA);
            modelBuilder.Entity<T_EVENTO_INSTANCIA>().HasMany(e => e.T_EVENTO_PARAMETRO_INSTANCIA).WithOne(e => e.T_EVENTO_INSTANCIA).HasForeignKey(d => d.NU_EVENTO_INSTANCIA);
            modelBuilder.Entity<T_EVENTO_NOTIFICACION>().HasOne(e => e.T_EVENTO_INSTANCIA).WithMany(e => e.T_EVENTO_NOTIFICACION).HasForeignKey(d => d.NU_EVENTO_INSTANCIA);
            modelBuilder.Entity<T_EVENTO_NOTIFICACION>().HasMany(e => e.T_EVENTO_NOTIFICACION_ARCHIVO).WithOne(e => e.T_EVENTO_NOTIFICACION).HasForeignKey(d => d.NU_EVENTO_NOTIFICACION);
            modelBuilder.Entity<T_EVENTO_NOTIFICACION>().HasOne(e => e.T_EVENTO_NOTIFICACION_EMAIL).WithOne(e => e.T_EVENTO_NOTIFICACION).HasForeignKey<T_EVENTO_NOTIFICACION_EMAIL>(d => d.NU_EVENTO_NOTIFICACION);
            modelBuilder.Entity<T_EVENTO_NOTIFICACION_ARCHIVO>().HasOne(e => e.T_EVENTO_NOTIFICACION).WithMany(e => e.T_EVENTO_NOTIFICACION_ARCHIVO).HasForeignKey(d => d.NU_EVENTO_NOTIFICACION);
            modelBuilder.Entity<T_EVENTO_NOTIFICACION_EMAIL>().HasOne(e => e.T_EVENTO_NOTIFICACION).WithOne(e => e.T_EVENTO_NOTIFICACION_EMAIL).HasForeignKey<T_EVENTO_NOTIFICACION>(d => d.NU_EVENTO_NOTIFICACION);
            modelBuilder.Entity<T_EVENTO_PARAMETRO>().HasOne(e => e.T_EVENTO).WithMany(e => e.T_EVENTO_PARAMETRO).HasForeignKey(d => d.NU_EVENTO);
            modelBuilder.Entity<T_EVENTO_PARAMETRO>().HasMany(e => e.T_EVENTO_PARAMETRO_INSTANCIA).WithOne(e => e.T_EVENTO_PARAMETRO).HasForeignKey(d => new { d.CD_EVENTO_PARAMETRO, d.NU_EVENTO, d.TP_NOTIFICACION });
            modelBuilder.Entity<T_EVENTO_PARAMETRO_INSTANCIA>().HasOne(e => e.T_EVENTO_PARAMETRO).WithMany(e => e.T_EVENTO_PARAMETRO_INSTANCIA).HasForeignKey(d => new { d.CD_EVENTO_PARAMETRO, d.NU_EVENTO, d.TP_NOTIFICACION });
            modelBuilder.Entity<T_EVENTO_PARAMETRO_INSTANCIA>().HasOne(e => e.T_EVENTO_INSTANCIA).WithMany(e => e.T_EVENTO_PARAMETRO_INSTANCIA).HasForeignKey(d => d.NU_EVENTO_INSTANCIA);
            modelBuilder.Entity<T_EVENTO_PARAMETRO_INSTANCIA>().HasOne(e => e.T_EVENTO).WithMany(e => e.T_EVENTO_PARAMETRO_INSTANCIA).HasForeignKey(d => d.NU_EVENTO);
            modelBuilder.Entity<T_FACTURACION_EJEC_EMPRESA>().HasOne(e => e.T_FACTURACION_EJECUCION).WithMany(e => e.T_FACTURACION_EJEC_EMPRESA).HasForeignKey(d => d.NU_EJECUCION);
            modelBuilder.Entity<T_FACTURACION_EJEC_EMPRESA>().HasOne(e => e.T_FACTURACION_PROCESO).WithMany(e => e.T_FACTURACION_EJEC_EMPRESA).HasForeignKey(d => d.CD_PROCESO);
            modelBuilder.Entity<T_FACTURACION_EJECUCION>().HasMany(e => e.T_FACTURACION_EJEC_EMPRESA).WithOne(e => e.T_FACTURACION_EJECUCION).HasForeignKey(d => d.NU_EJECUCION);
            modelBuilder.Entity<T_FACTURACION_PROCESO>().HasMany(e => e.T_FACTURACION_EJEC_EMPRESA).WithOne(e => e.T_FACTURACION_PROCESO).HasForeignKey(d => d.CD_PROCESO);
            modelBuilder.Entity<T_FACTURACION_RESULTADO>().HasOne(e => e.T_FACTURACION_CUENTA_CONTABLE).WithMany(e => e.T_FACTURACION_RESULTADO).HasForeignKey(d => d.NU_CUENTA_CONTABLE);
            modelBuilder.Entity<T_FACTURACION_RESULTADO>().HasOne(e => e.T_FACTURACION_EJECUCION).WithMany(e => e.T_FACTURACION_RESULTADO).HasForeignKey(d => d.NU_EJECUCION);
            modelBuilder.Entity<T_FACTURACION_CUENTA_CONTABLE>().HasMany(e => e.T_FACTURACION_CODIGO_COMPONEN).WithOne(e => e.T_FACTURACION_CUENTA_CONTABLE).HasForeignKey(d => d.NU_CUENTA_CONTABLE);
            modelBuilder.Entity<T_FACTURACION_CUENTA_CONTABLE>().HasMany(e => e.T_FACTURACION_RESULTADO).WithOne(e => e.T_FACTURACION_CUENTA_CONTABLE).HasForeignKey(d => d.NU_CUENTA_CONTABLE);
            modelBuilder.Entity<T_FACTURACION_EJECUCION>().HasMany(e => e.T_FACTURACION_RESULTADO).WithOne(e => e.T_FACTURACION_EJECUCION).HasForeignKey(d => d.NU_EJECUCION);
            modelBuilder.Entity<T_FACTURACION_PALLET_DET>().HasOne(e => e.T_FACTURACION_PALLET).WithMany(e => e.T_FACTURACION_PALLET_DET).HasForeignKey(d => d.NU_PALLET);
            modelBuilder.Entity<T_FACTURACION_PALLET>().HasMany(e => e.T_FACTURACION_PALLET_DET).WithOne(e => e.T_FACTURACION_PALLET).HasForeignKey(d => d.NU_PALLET);
            modelBuilder.Entity<T_FACTURACION_PROCESO>().HasOne(e => e.T_FACTURACION_CODIGO_COMPONEN).WithMany(e => e.T_FACTURACION_PROCESO).HasForeignKey(d => new { d.CD_FACTURACION, d.NU_COMPONENTE });
            modelBuilder.Entity<T_FACTURACION_CODIGO_COMPONEN>().HasMany(e => e.T_FACTURACION_PROCESO).WithOne(e => e.T_FACTURACION_CODIGO_COMPONEN).HasForeignKey(d => new { d.CD_FACTURACION, d.NU_COMPONENTE });
            modelBuilder.Entity<T_FACTURACION_CODIGO_COMPONEN>().HasOne(e => e.T_FACTURACION_CUENTA_CONTABLE).WithMany(e => e.T_FACTURACION_CODIGO_COMPONEN).HasForeignKey(d => new { d.NU_CUENTA_CONTABLE });
            modelBuilder.Entity<T_FACTURACION_CODIGO_COMPONEN>().HasOne(e => e.T_FACTURACION_CODIGO).WithMany(e => e.T_FACTURACION_CODIGO_COMPONEN).HasForeignKey(d => new { d.CD_FACTURACION });
            modelBuilder.Entity<T_FACTURACION_CODIGO>().HasMany(e => e.T_FACTURACION_CODIGO_COMPONEN).WithOne(e => e.T_FACTURACION_CODIGO).HasForeignKey(d => d.CD_FACTURACION);
            modelBuilder.Entity<T_UNIDADE_MEDIDA>().HasMany(e => e.T_PRODUTO).WithOne(e => e.T_UNIDADE_MEDIDA).HasForeignKey(d => d.CD_UNIDADE_MEDIDA);
            modelBuilder.Entity<T_GRID_FILTER>().HasMany(e => e.T_GRID_FILTER_DET).WithOne(e => e.T_GRID_FILTER).HasForeignKey(d => d.CD_FILTRO);
            modelBuilder.Entity<T_GRID_FILTER_DET>().HasOne(e => e.T_GRID_FILTER).WithMany(e => e.T_GRID_FILTER_DET).HasForeignKey(d => d.CD_FILTRO);
            modelBuilder.Entity<T_GRUPO_CONSULTA>().HasMany(e => e.T_GRUPO_CONSULTA_FUNCIONARIO).WithOne(e => e.T_GRUPO_CONSULTA).HasForeignKey(d => d.CD_GRUPO_CONSULTA);
            modelBuilder.Entity<T_GRUPO_CONSULTA_FUNCIONARIO>().HasOne(e => e.USERS).WithMany(e => e.T_GRUPO_CONSULTA_FUNCIONARIO).HasForeignKey(d => d.USERID);
            modelBuilder.Entity<T_GRUPO_CONSULTA_FUNCIONARIO>().HasOne(e => e.T_GRUPO_CONSULTA).WithMany(e => e.T_GRUPO_CONSULTA_FUNCIONARIO).HasForeignKey(d => d.CD_GRUPO_CONSULTA);
            modelBuilder.Entity<T_IMPRESORA>().HasOne(e => e.T_LENGUAJE_IMPRESION).WithMany(e => e.T_IMPRESORA).HasForeignKey(d => d.CD_LENGUAJE_IMPRESION);
            modelBuilder.Entity<T_INTERFAZ_EJECUCION>().HasOne(e => e.T_INTERFAZ_EXTERNA).WithMany(e => e.T_INTERFAZ_EJECUCION).HasForeignKey(d => d.CD_INTERFAZ_EXTERNA);
            modelBuilder.Entity<T_INTERFAZ_EJECUCION>().HasOne(e => e.T_INTERFAZ_EJECUCION_DATA).WithOne(e => e.T_INTERFAZ_EJECUCION).HasForeignKey<T_INTERFAZ_EJECUCION_DATA>(d => d.NU_INTERFAZ_EJECUCION);
            modelBuilder.Entity<T_ORT_ORDEN_TAREA>().HasOne(e => e.T_ORT_TAREA).WithMany(e => e.T_ORT_ORDEN_TAREA).HasForeignKey(d => d.CD_TAREA);
            modelBuilder.Entity<T_ORT_ORDEN_TAREA>().HasOne(e => e.T_ORT_ORDEN).WithMany(e => e.T_ORT_ORDEN_TAREA).HasForeignKey(d => d.NU_ORT_ORDEN);
            modelBuilder.Entity<T_ORT_ORDEN>().HasMany(e => e.T_ORT_ORDEN_TAREA).WithOne(e => e.T_ORT_ORDEN).HasForeignKey(d => d.NU_ORT_ORDEN);
            modelBuilder.Entity<T_ORT_TAREA>().HasMany(e => e.T_ORT_ORDEN_TAREA).WithOne(e => e.T_ORT_TAREA).HasForeignKey(d => d.CD_TAREA);
            modelBuilder.Entity<T_ORT_ORDEN_TAREA_FUNCIONARIO>().HasOne(e => e.T_ORT_ORDEN_TAREA).WithMany(e => e.T_ORT_ORDEN_TAREA_FUNCIONARIO).HasForeignKey(d => d.NU_ORDEN_TAREA);
            modelBuilder.Entity<T_ORT_ORDEN_TAREA>().HasMany(e => e.T_ORT_ORDEN_TAREA_FUNCIONARIO).WithOne(e => e.T_ORT_ORDEN_TAREA).HasForeignKey(d => d.NU_ORDEN_TAREA);
            modelBuilder.Entity<T_ORT_ORDEN_TAREA_EQUIPO>().HasOne(e => e.T_ORT_ORDEN_TAREA).WithMany(e => e.T_ORT_ORDEN_TAREA_EQUIPO).HasForeignKey(d => d.NU_ORDEN_TAREA);
            modelBuilder.Entity<T_ORT_ORDEN_TAREA>().HasMany(e => e.T_ORT_ORDEN_TAREA_EQUIPO).WithOne(e => e.T_ORT_ORDEN_TAREA).HasForeignKey(d => d.NU_ORDEN_TAREA);
            modelBuilder.Entity<T_INTERFAZ_EJECUCION>().HasOne(e => e.T_INTERFAZ_EJECUCION_DATEXT).WithOne(e => e.T_INTERFAZ_EJECUCION).HasForeignKey<T_INTERFAZ_EJECUCION_DATEXT>(d => d.NU_INTERFAZ_EJECUCION);
            modelBuilder.Entity<T_INTERFAZ_EJECUCION>().HasMany(e => e.T_INTERFAZ_EJECUCION_DATEXTDET).WithOne(e => e.T_INTERFAZ_EJECUCION).HasForeignKey(d => d.NU_INTERFAZ_EJECUCION);
            modelBuilder.Entity<T_INTERFAZ_EJECUCION>().HasMany(e => e.T_INTERFAZ_EJECUCION_ERROR).WithOne(e => e.T_INTERFAZ_EJECUCION).HasForeignKey(d => d.NU_INTERFAZ_EJECUCION);
            modelBuilder.Entity<T_INTERFAZ_EJECUCION_DATA>().HasOne(e => e.T_INTERFAZ_EJECUCION).WithOne(e => e.T_INTERFAZ_EJECUCION_DATA).HasForeignKey<T_INTERFAZ_EJECUCION>(d => d.NU_INTERFAZ_EJECUCION);
            modelBuilder.Entity<T_INTERFAZ_EJECUCION_DATEXT>().HasOne(e => e.T_INTERFAZ_EJECUCION).WithOne(e => e.T_INTERFAZ_EJECUCION_DATEXT).HasForeignKey<T_INTERFAZ_EJECUCION>(d => d.NU_INTERFAZ_EJECUCION);
            modelBuilder.Entity<T_INTERFAZ_EJECUCION_DATEXTDET>().HasOne(e => e.T_INTERFAZ_EJECUCION).WithMany(e => e.T_INTERFAZ_EJECUCION_DATEXTDET).HasForeignKey(d => d.NU_INTERFAZ_EJECUCION);
            modelBuilder.Entity<T_INTERFAZ_EJECUCION_ERROR>().HasOne(e => e.T_INTERFAZ_EJECUCION).WithMany(e => e.T_INTERFAZ_EJECUCION_ERROR).HasForeignKey(d => d.NU_INTERFAZ_EJECUCION);
            modelBuilder.Entity<T_INTERFAZ_EXTERNA>().HasOne(e => e.T_INTERFAZ).WithMany(e => e.T_INTERFAZ_EXTERNA).HasForeignKey(d => d.CD_INTERFAZ);
            modelBuilder.Entity<T_INVENTARIO_ENDERECO>().HasOne(e => e.T_INVENTARIO).WithMany(e => e.T_INVENTARIO_ENDERECO).HasForeignKey(d => d.NU_INVENTARIO);
            modelBuilder.Entity<T_INVENTARIO_ENDERECO>().HasMany(e => e.T_INVENTARIO_ENDERECO_DET).WithOne(e => e.T_INVENTARIO_ENDERECO).HasForeignKey(d => d.NU_INVENTARIO_ENDERECO);
            modelBuilder.Entity<T_INVENTARIO_ENDERECO_DET>().HasOne(e => e.T_INVENTARIO_ENDERECO).WithMany(e => e.T_INVENTARIO_ENDERECO_DET).HasForeignKey(d => d.NU_INVENTARIO_ENDERECO);
            modelBuilder.Entity<T_LABEL_ESTILO>().HasMany(e => e.T_LABEL_TEMPLATE).WithOne(e => e.T_LABEL_ESTILO).HasForeignKey(d => d.CD_LABEL_ESTILO);
            modelBuilder.Entity<T_LABEL_TEMPLATE>().HasOne(e => e.T_LABEL_ESTILO).WithMany(e => e.T_LABEL_TEMPLATE).HasForeignKey(d => d.CD_LABEL_ESTILO);
            modelBuilder.Entity<T_LENGUAJE_IMPRESION>().HasMany(e => e.T_IMPRESORA).WithOne(e => e.T_LENGUAJE_IMPRESION).HasForeignKey(d => d.CD_LENGUAJE_IMPRESION);
            modelBuilder.Entity<T_LPARAMETRO_CONFIGURACION>().HasOne(e => e.T_LPARAMETRO_NIVEL).WithMany(e => e.T_LPARAMETRO_CONFIGURACION).HasForeignKey(d => new { d.CD_PARAMETRO, d.DO_ENTIDAD_PARAMETRIZABLE });
            modelBuilder.Entity<T_LPARAMETRO_NIVEL>().HasOne(e => e.T_LPARAMETRO).WithMany(e => e.T_LPARAMETRO_NIVEL).HasForeignKey(d => d.CD_PARAMETRO);
            modelBuilder.Entity<T_LPARAMETRO_NIVEL>().HasMany(e => e.T_LPARAMETRO_CONFIGURACION).WithOne(e => e.T_LPARAMETRO_NIVEL).HasForeignKey(d => new { d.CD_PARAMETRO, d.DO_ENTIDAD_PARAMETRIZABLE });
            modelBuilder.Entity<T_PAIS_SUBDIVISION>().HasOne(e => e.T_PAIS).WithMany(e => e.T_PAIS_SUBDIVISION).HasForeignKey(d => d.CD_PAIS);
            modelBuilder.Entity<T_PAIS_SUBDIVISION_LOCALIDAD>().HasOne(e => e.T_PAIS_SUBDIVISION).WithMany(e => e.T_PAIS_SUBDIVISION_LOCALIDAD).HasForeignKey(d => d.CD_SUBDIVISION);
            modelBuilder.Entity<T_PEDIDO_SAIDA>().HasMany(e => e.T_DET_PEDIDO_SAIDA).WithOne(e => e.T_PEDIDO_SAIDA).HasForeignKey(d => new { d.NU_PEDIDO, d.CD_CLIENTE, d.CD_EMPRESA });
            modelBuilder.Entity<T_PICKING>().HasMany(e => e.T_DET_PICKING).WithOne(e => e.T_PICKING).HasForeignKey(d => d.NU_PREPARACION);
            modelBuilder.Entity<T_PICKING>().HasMany(e => e.T_DOCUMENTO_LIBERACION).WithOne(e => e.T_PICKING).HasForeignKey(d => d.NU_PREPARACION);
            modelBuilder.Entity<T_PICKING_PRODUTO>().HasOne(e => e.T_ENDERECO_ESTOQUE).WithMany(e => e.T_PICKING_PRODUTO).HasForeignKey(d => d.CD_ENDERECO_SEPARACAO);
            modelBuilder.Entity<T_PORTA_EMBARQUE>().HasOne(e => e.T_ENDERECO_ESTOQUE).WithMany(e => e.T_PORTA_EMBARQUE).HasForeignKey(d => d.CD_ENDERECO);
            modelBuilder.Entity<T_PRODUTO>().HasOne(e => e.T_CLASSE).WithMany(e => e.T_PRODUTO).HasForeignKey(d => d.CD_CLASSE);
            modelBuilder.Entity<T_PRODUTO>().HasOne(e => e.T_ROTATIVIDADE).WithMany(e => e.T_PRODUTO).HasForeignKey(d => d.CD_ROTATIVIDADE);
            modelBuilder.Entity<T_PRODUTO>().HasOne(e => e.T_FAMILIA_PRODUTO).WithMany(e => e.T_PRODUTO).HasForeignKey(d => d.CD_FAMILIA_PRODUTO);
            modelBuilder.Entity<T_RECEPC_AGENDA_CONTAINER_REL>().HasOne(e => e.T_AGENDA).WithMany(e => e.T_RECEPC_AGENDA_CONTAINER_REL).HasForeignKey(d => d.NU_AGENDA);
            modelBuilder.Entity<T_RECEPC_AGENDA_CONTAINER_REL>().HasOne(e => e.T_CONTAINER).WithMany(e => e.T_RECEPC_AGENDA_CONTAINER_REL).HasForeignKey(d => new { d.NU_CONTAINER, d.NU_SEQ_CONTAINER });
            modelBuilder.Entity<T_RECEPC_AGENDA_REFERENCIA_REL>().HasOne(e => e.T_AGENDA).WithMany(e => e.T_RECEPC_AGENDA_REFERENCIA_REL).HasForeignKey(d => d.NU_AGENDA);
            modelBuilder.Entity<T_RECEPC_AGENDA_REFERENCIA_REL>().HasOne(e => e.T_RECEPCION_REFERENCIA).WithMany(e => e.T_RECEPC_AGENDA_REFERENCIA_REL).HasForeignKey(d => d.NU_RECEPCION_REFERENCIA);
            modelBuilder.Entity<T_RECEPCION_AGENDA_REFERENCIA>().HasOne(e => e.T_RECEPCION_REFERENCIA_DET).WithMany(e => e.T_RECEPCION_AGENDA_REFERENCIA).HasForeignKey(d => d.NU_RECEPCION_REFERENCIA_DET);
            modelBuilder.Entity<T_RECEPCION_REFERENCIA>().HasOne(e => e.T_EMPRESA).WithMany(e => e.T_RECEPCION_REFERENCIA).HasForeignKey(d => d.CD_EMPRESA);
            modelBuilder.Entity<T_RECEPCION_REFERENCIA>().HasMany(e => e.T_RECEPC_AGENDA_REFERENCIA_REL).WithOne(e => e.T_RECEPCION_REFERENCIA).HasForeignKey(d => d.NU_RECEPCION_REFERENCIA);
            modelBuilder.Entity<T_RECEPCION_REFERENCIA_DET>().HasOne(e => e.T_RECEPCION_REFERENCIA).WithMany(e => e.T_RECEPCION_REFERENCIA_DET).HasForeignKey(d => d.NU_RECEPCION_REFERENCIA);
            modelBuilder.Entity<T_RECEPCION_REFERENCIA_DET>().HasMany(e => e.T_RECEPCION_AGENDA_REFERENCIA).WithOne(e => e.T_RECEPCION_REFERENCIA_DET).HasForeignKey(d => d.NU_RECEPCION_REFERENCIA_DET);
            modelBuilder.Entity<T_RECEPCION_REL_EMPRESA_TIPO>().HasOne(e => e.T_RECEPCION_TIPO).WithMany(e => e.T_RECEPCION_REL_EMPRESA_TIPO).HasForeignKey(d => d.TP_RECEPCION);
            modelBuilder.Entity<T_RECEPCION_REL_EMPRESA_TIPO>().HasOne(e => e.T_EMPRESA).WithMany(e => e.T_RECEPCION_REL_EMPRESA_TIPO).HasForeignKey(d => d.CD_EMPRESA);
            modelBuilder.Entity<T_RECEPCION_TIPO>().HasMany(e => e.T_RECEPCION_REL_EMPRESA_TIPO).WithOne(e => e.T_RECEPCION_TIPO).HasForeignKey(d => d.TP_RECEPCION);
            modelBuilder.Entity<T_RECEPCION_TIPO>().HasMany(e => e.T_RECEPCION_TIPO_REPORTE_DEF).WithOne(e => e.T_RECEPCION_TIPO).HasForeignKey(d => d.TP_RECEPCION);
            modelBuilder.Entity<T_RECEPCION_TIPO_REPORTE_DEF>().HasOne(e => e.T_RECEPCION_TIPO).WithMany(e => e.T_RECEPCION_TIPO_REPORTE_DEF).HasForeignKey(d => d.TP_RECEPCION);
            modelBuilder.Entity<T_RECEPCION_TIPO_REPORTE_DEF>().HasOne(e => e.T_REPORTE_DEFINICION).WithMany(e => e.T_RECEPCION_TIPO_REPORTE_DEF).HasForeignKey(d => d.CD_REPORTE);
            modelBuilder.Entity<T_REGLA_CLIENTES>().HasOne(e => e.T_REGLA_LIBERACION).WithMany(e => e.T_REGLA_CLIENTES).HasForeignKey(d => d.NU_REGLA);
            modelBuilder.Entity<T_REGLA_CONDICION_LIBERACION>().HasOne(e => e.T_REGLA_LIBERACION).WithMany(e => e.T_REGLA_CONDICION_LIBERACION).HasForeignKey(d => d.NU_REGLA);
            modelBuilder.Entity<T_REGLA_LIBERACION>().HasMany(e => e.T_REGLA_CLIENTES).WithOne(e => e.T_REGLA_LIBERACION).HasForeignKey(d => d.NU_REGLA);
            modelBuilder.Entity<T_REGLA_LIBERACION>().HasMany(e => e.T_REGLA_CONDICION_LIBERACION).WithOne(e => e.T_REGLA_LIBERACION).HasForeignKey(d => d.NU_REGLA);
            modelBuilder.Entity<T_REGLA_LIBERACION>().HasMany(e => e.T_REGLA_TIPO_EXPEDICION).WithOne(e => e.T_REGLA_LIBERACION).HasForeignKey(d => d.NU_REGLA);
            modelBuilder.Entity<T_REGLA_LIBERACION>().HasMany(e => e.T_REGLA_TIPO_PEDIDO).WithOne(e => e.T_REGLA_LIBERACION).HasForeignKey(d => d.NU_REGLA);
            modelBuilder.Entity<T_REGLA_TIPO_EXPEDICION>().HasOne(e => e.T_REGLA_LIBERACION).WithMany(e => e.T_REGLA_TIPO_EXPEDICION).HasForeignKey(d => d.NU_REGLA);
            modelBuilder.Entity<T_REGLA_TIPO_PEDIDO>().HasOne(e => e.T_REGLA_LIBERACION).WithMany(e => e.T_REGLA_TIPO_PEDIDO).HasForeignKey(d => d.NU_REGLA);
            modelBuilder.Entity<T_REL_LABELESTILO_TIPOCONT>().HasKey(e => new { e.CD_LABEL_ESTILO, e.TP_CONTENEDOR });
            modelBuilder.Entity<T_REL_LABELESTILO_TIPOCONT>().HasOne(e => e.T_LABEL_ESTILO).WithMany(e => e.T_REL_LABELESTILO_TIPOCONT).HasForeignKey(d => d.CD_LABEL_ESTILO);
            modelBuilder.Entity<T_REL_LABELESTILO_TIPOCONT>().HasOne(e => e.T_TIPO_CONTENEDOR).WithMany(e => e.T_REL_LABELESTILO_TIPOCONT).HasForeignKey(d => d.TP_CONTENEDOR);
            modelBuilder.Entity<T_REPORTE>().HasMany(e => e.T_REPORTE_RELACION).WithOne(e => e.T_REPORTE).HasForeignKey(d => d.NU_REPORTE);
            modelBuilder.Entity<T_REPORTE_DEFINICION>().HasMany(e => e.T_RECEPCION_TIPO_REPORTE_DEF).WithOne(e => e.T_REPORTE_DEFINICION).HasForeignKey(d => d.CD_REPORTE);
            modelBuilder.Entity<T_REPORTE_RELACION>().HasOne(e => e.T_REPORTE).WithMany(e => e.T_REPORTE_RELACION).HasForeignKey(d => d.NU_REPORTE);
            modelBuilder.Entity<T_ROTA>().HasOne(e => e.T_ONDA).WithMany(e => e.T_ROTA).HasForeignKey(d => d.CD_ONDA);
            modelBuilder.Entity<T_ROTA>().HasOne(e => e.T_PORTA_EMBARQUE).WithMany(e => e.T_ROTA).HasForeignKey(d => d.CD_PORTA);
            modelBuilder.Entity<T_ROTA>().HasMany(e => e.T_CLIENTE_RUTA_PREDIO).WithOne(e => e.T_ROTA).HasForeignKey(d => d.CD_ROTA);
            modelBuilder.Entity<T_SITUACAO>().HasMany(e => e.T_PRDC_INGRESO).WithOne(e => e.T_SITUACAO).HasForeignKey(d => d.NU_PRDC_INGRESO);
            modelBuilder.Entity<T_SITUACAO>().HasMany(e => e.T_PRDC_DEFINICION).WithOne(e => e.T_SITUACAO).HasForeignKey(d => d.CD_PRDC_DEFINICION);
            modelBuilder.Entity<T_TIPO_CODIGO_BARRAS>().HasMany(e => e.T_CODIGO_BARRAS).WithOne(e => e.T_TIPO_CODIGO_BARRAS).HasForeignKey(d => d.TP_CODIGO_BARRAS);
            modelBuilder.Entity<T_TIPO_VEICULO>().HasMany(e => e.T_VEICULO).WithOne(e => e.T_TIPO_VEICULO).HasForeignKey(d => d.CD_TIPO_VEICULO);
            modelBuilder.Entity<T_USUARIO_CONFIGURACION>().HasOne(e => e.USERS).WithOne(e => e.T_USUARIO_CONFIGURACION).HasForeignKey<USERS>(d => d.USERID);
            modelBuilder.Entity<T_VEICULO>().HasOne(e => e.T_TIPO_VEICULO).WithMany(e => e.T_VEICULO).HasForeignKey(d => d.CD_TIPO_VEICULO);
            modelBuilder.Entity<USER_TOKEN>().HasOne(e => e.USERS).WithOne(e => e.USER_TOKEN).HasForeignKey<USER_TOKEN>(d => d.USERID);
            modelBuilder.Entity<USERDATA>().HasOne(e => e.USERS).WithOne(e => e.USERDATA).HasForeignKey<USERS>(d => d.USERID);
            modelBuilder.Entity<USERPERMISSIONS>().HasOne(e => e.USERS).WithMany(e => e.USERPERMISSIONS).HasForeignKey(d => d.USERID);
            modelBuilder.Entity<USERPERMISSIONS>().HasOne(e => e.PROFILES).WithMany(e => e.USERPERMISSIONS).HasForeignKey(d => d.PROFILEID);
            modelBuilder.Entity<USERPERMISSIONS>().HasOne(e => e.RESOURCES).WithMany(e => e.USERPERMISSIONS).HasForeignKey(d => d.RESOURCEID);
            modelBuilder.Entity<USERS>().HasOne(e => e.USERTYPES).WithMany(e => e.USERS).HasForeignKey(d => d.USERTYPEID);
            modelBuilder.Entity<USERS>().HasMany(e => e.T_EMPRESA_FUNCIONARIO).WithOne(e => e.USERS).HasForeignKey(d => d.USERID);
            modelBuilder.Entity<USERS>().HasMany(e => e.T_GRUPO_CONSULTA_FUNCIONARIO).WithOne(e => e.USERS).HasForeignKey(d => d.USERID);
            modelBuilder.Entity<USERS>().HasOne(e => e.T_USUARIO_CONFIGURACION).WithOne(e => e.USERS).HasForeignKey<T_USUARIO_CONFIGURACION>(d => d.USERID);
            modelBuilder.Entity<USERS>().HasOne(e => e.USER_TOKEN).WithOne(e => e.USERS).HasForeignKey<USER_TOKEN>(d => d.USERID);
            modelBuilder.Entity<USERS>().HasOne(e => e.USERDATA).WithOne(e => e.USERS).HasForeignKey<USERDATA>(d => d.USERID);
            modelBuilder.Entity<USERS>().HasMany(e => e.USERPERMISSIONS).WithOne(e => e.USERS).HasForeignKey(d => d.USERID);
            modelBuilder.Entity<T_ALM_LOGICA_INSTANCIA_PARAM>().HasOne(e => e.T_ALM_LOGICA_INSTANCIA).WithMany(e => e.T_ALM_LOGICA_INSTANCIA_PARAM).HasForeignKey(d => d.NU_ALM_LOGICA_INSTANCIA);
            modelBuilder.Entity<T_ALM_LOGICA_INSTANCIA_PARAM>().HasOne(e => e.T_ALM_PARAMETRO).WithMany(e => e.T_ALM_LOGICA_INSTANCIA_PARAM).HasForeignKey(d => d.NU_ALM_PARAMETRO);
            modelBuilder.Entity<T_ALM_SUGERENCIA>().HasOne(e => e.T_ALM_ESTRATEGIA).WithMany(e => e.T_ALM_SUGERENCIA).HasForeignKey(d => d.NU_ALM_ESTRATEGIA);
            modelBuilder.Entity<T_ALM_SUGERENCIA>().HasOne(e => e.T_ALM_LOGICA_INSTANCIA).WithMany(e => e.T_ALM_SUGERENCIA).HasForeignKey(d => d.NU_ALM_LOGICA_INSTANCIA);
            modelBuilder.Entity<T_ALM_SUGERENCIA_DET>().HasOne(e => e.T_ALM_LOGICA_INSTANCIA).WithMany(e => e.T_ALM_SUGERENCIA_DET).HasForeignKey(d => d.NU_ALM_LOGICA_INSTANCIA);
            modelBuilder.Entity<T_ALM_SUGERENCIA>().HasMany(e => e.T_ALM_SUGERENCIA_DET).WithOne(e => e.T_ALM_SUGERENCIA).HasForeignKey(d => new { d.NU_ALM_ESTRATEGIA, d.NU_PREDIO, d.TP_ALM_OPERATIVA_ASOCIABLE, d.CD_ALM_OPERATIVA_ASOCIABLE, d.CD_CLASSE, d.CD_GRUPO, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_REFERENCIA, d.CD_AGRUPADOR, d.CD_ENDERECO_SUGERIDO, d.NU_ALM_SUGERENCIA });
            modelBuilder.Entity<T_FACTURACION_EMPRESA_PROCESO>().HasOne(e => e.T_FACTURACION_PROCESO).WithMany(e => e.T_FACTURACION_EMPRESA_PROCESO).HasForeignKey(d => d.CD_PROCESO);

            modelBuilder.Entity<T_LPN_TIPO>().HasOne(e => e.T_LPN).WithOne(e => e.T_LPN_TIPO).HasForeignKey<T_LPN>(d => d.TP_LPN_TIPO);

            modelBuilder.Entity<T_LPN>().HasMany(e => e.T_LPN_DET).WithOne(e => e.T_LPN).HasForeignKey(d => new { d.NU_LPN });
            modelBuilder.Entity<T_LPN_DET>().HasOne(e => e.T_LPN).WithMany(e => e.T_LPN_DET).HasForeignKey(d => new { d.NU_LPN });

            modelBuilder.Entity<T_LPN_ATRIBUTO>().HasOne(e => e.T_ATRIBUTO).WithMany(e => e.T_LPN_ATRIBUTO).HasForeignKey(d => new { d.ID_ATRIBUTO });
            modelBuilder.Entity<T_LPN_DET_ATRIBUTO>().HasOne(e => e.T_ATRIBUTO).WithMany(e => e.T_LPN_DET_ATRIBUTO).HasForeignKey(d => new { d.ID_ATRIBUTO });

            modelBuilder.Entity<T_EQUIPO>().HasOne(e => e.T_FERRAMENTAS).WithMany(e => e.T_EQUIPO).HasForeignKey(d => d.CD_FERRAMENTA);
            modelBuilder.Entity<T_EQUIPO>().HasOne(e => e.T_ENDERECO_ESTOQUE).WithMany(e => e.T_EQUIPO).HasForeignKey(d => d.CD_ENDERECO);

            modelBuilder.Entity<T_AUTOMATISMO>().HasMany(e => e.T_AUTOMATISMO_POSICION).WithOne(e => e.T_AUTOMATISMO).HasForeignKey(d => d.NU_AUTOMATISMO);
            modelBuilder.Entity<T_AUTOMATISMO>().HasMany(e => e.T_AUTOMATISMO_EJECUCION).WithOne(e => e.T_AUTOMATISMO).HasForeignKey(d => d.NU_AUTOMATISMO);
            modelBuilder.Entity<T_AUTOMATISMO>().HasMany(e => e.T_AUTOMATISMO_INTERFAZ).WithOne(e => e.T_AUTOMATISMO).HasForeignKey(d => d.NU_AUTOMATISMO);
            modelBuilder.Entity<T_AUTOMATISMO>().HasMany(e => e.T_AUTOMATISMO_PUESTO).WithOne(e => e.T_AUTOMATISMO).HasForeignKey(d => d.NU_AUTOMATISMO);
            modelBuilder.Entity<T_AUTOMATISMO_CARACTERISTICA>().HasOne(e => e.T_AUTOMATISMO).WithMany(e => e.T_AUTOMATISMO_CARACTERISTICA).HasForeignKey(d => d.NU_AUTOMATISMO);
            modelBuilder.Entity<T_AUTOMATISMO_DATA>().HasOne(e => e.T_AUTOMATISMO_EJECUCION).WithMany(e => e.T_AUTOMATISMO_DATA).HasForeignKey(d => d.NU_AUTOMATISMO_EJECUCION);
            modelBuilder.Entity<T_AUTOMATISMO_EJECUCION>().HasOne(e => e.T_AUTOMATISMO).WithMany(e => e.T_AUTOMATISMO_EJECUCION).HasForeignKey(d => d.NU_AUTOMATISMO);
            modelBuilder.Entity<T_AUTOMATISMO_EJECUCION>().HasMany(e => e.T_AUTOMATISMO_DATA).WithOne(e => e.T_AUTOMATISMO_EJECUCION).HasForeignKey(d => d.NU_AUTOMATISMO_EJECUCION);
            modelBuilder.Entity<T_AUTOMATISMO_INTERFAZ>().HasOne(e => e.T_AUTOMATISMO).WithMany(e => e.T_AUTOMATISMO_INTERFAZ).HasForeignKey(d => d.NU_AUTOMATISMO);
            modelBuilder.Entity<T_AUTOMATISMO_INTERFAZ>().HasOne(e => e.T_INTEGRACION_SERVICIO).WithMany(e => e.T_AUTOMATISMO_INTERFAZ).HasForeignKey(d => d.NU_INTEGRACION);
            modelBuilder.Entity<T_AUTOMATISMO_POSICION>().HasOne(e => e.T_AUTOMATISMO).WithMany(e => e.T_AUTOMATISMO_POSICION).HasForeignKey(d => d.NU_AUTOMATISMO);
            modelBuilder.Entity<T_AUTOMATISMO_PUESTO>().HasOne(e => e.T_AUTOMATISMO).WithMany(e => e.T_AUTOMATISMO_PUESTO).HasForeignKey(d => d.NU_AUTOMATISMO);
            modelBuilder.Entity<T_INTEGRACION_SERVICIO>().HasMany(e => e.T_AUTOMATISMO_INTERFAZ).WithOne(e => e.T_INTEGRACION_SERVICIO).HasForeignKey(d => d.NU_INTEGRACION);

            modelBuilder.Entity<T_IMPRESION>().HasMany(e => e.T_DET_IMPRESION).WithOne(e => e.T_IMPRESION).HasForeignKey(d => d.NU_IMPRESION);


            modelBuilder.Entity<T_COLA_TRABAJO>().HasMany(e => e.T_COLA_TRABAJO_PONDERADOR).WithOne(e => e.T_COLA_TRABAJO).HasForeignKey(e => e.NU_COLA_TRABAJO);
            modelBuilder.Entity<T_COLA_TRABAJO>().HasMany(e => e.T_COLA_TRABAJO_PONDERADOR_DET).WithOne(e => e.T_COLA_TRABAJO);
            modelBuilder.Entity<T_COLA_TRABAJO_PONDERADOR>().HasMany(e => e.T_COLA_TRABAJO_PONDERADOR_DET).WithOne(e => e.T_COLA_TRABAJO_PONDERADOR).HasForeignKey(e => new { e.NU_COLA_TRABAJO, e.CD_PONDERADOR });
            #endregion

            #region Properties

            modelBuilder.Entity<V_REC500_SEARCH_PRODUCTO>()
                   .HasKey(e => new { e.CD_EMPRESA, e.CD_PRODUTO });


            #region - FABRICA TABLAS - 
            #region >> T_AGENDA
            modelBuilder.Entity<T_AGENDA>()
                .Property(e => e.VL_TEMP_CAMION)
                .HasPrecision(6, 2);
            modelBuilder.Entity<T_AGENDA>()
                .Property(e => e.VL_TEMP_INICIO_REC)
                .HasPrecision(6, 2);
            modelBuilder.Entity<T_AGENDA>()
                .Property(e => e.VL_TEMP_MITAD_REC)
                .HasPrecision(6, 2);
            modelBuilder.Entity<T_AGENDA>()
                .Property(e => e.VL_TEMP_FINAL_REC)
                .HasPrecision(6, 2);
            //modelBuilder.Entity<T_AGENDA>()
            //    .HasMany(e => e.T_DET_AGENDA)
            //    .WithOne(e => e.T_AGENDA)
            //    .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<T_AGENDA>()
            //    .HasMany(e => e.T_RECEPC_AGENDA_REFERENCIA_REL)
            //    .WithOne(e => e.T_AGENDA)
            //    .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<T_AGENDA>()
            //    .HasMany(e => e.T_RECEPC_AGENDA_CONTAINER_REL)
            //    .WithOne(e => e.T_AGENDA)
            //    .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<T_AGENDA>()
            //    .HasMany(e => e.T_ETIQUETA_LOTE)
            //    .WithOne(e => e.T_AGENDA)
            //    .OnDelete(DeleteBehavior.Restrict);
            #endregion << T_AGENDA

            #region >> T_CLIENTE 
            //modelBuilder.Entity<T_CLIENTE>()
            //    .HasMany(e => e.T_CLIENTE_RUTA_PREDIO)
            //    .WithOne(e => e.T_CLIENTE)
            //    .HasForeignKey(e => new { e.CD_EMPRESA, e.CD_CLIENTE })
            //    .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<T_CLIENTE>()
            //    .HasOne(e => e.T_ROTA)
            //    .WithMany(e => e.T_CLIENTE)
            //    .HasForeignKey(e => e.CD_ROTA)
            //    .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<T_CLIENTE>()
            //    .HasOne(e => e.T_PAIS_SUBDIVISION_LOCALIDAD)
            //    .WithMany(e => e.T_CLIENTE)
            //    .HasForeignKey(e => e.ID_LOCALIDAD)
            //    .OnDelete(DeleteBehavior.Restrict);
            #endregion << T_CLIENTE

            #region T_CLIENTE_RUTA_PREDIO 
            //modelBuilder.Entity<T_CLIENTE_RUTA_PREDIO>()
            //    .HasOne(e => e.T_ROTA)
            //    .WithMany(e => e.T_CLIENTE_RUTA_PREDIO)
            //    .HasForeignKey(e => e.CD_ROTA)
            //    .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region >> T_CODIGO_BARRAS
            modelBuilder.Entity<T_CODIGO_BARRAS>()
                .Property(e => e.QT_EMBALAGEM)
                .HasPrecision(38, 0);
            #endregion << T_CODIGO_BARRAS

            #region >> T_DET_AGENDA
            modelBuilder.Entity<T_DET_AGENDA>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<T_DET_AGENDA>()
                .Property(e => e.QT_AGENDADO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_DET_AGENDA>()
                .Property(e => e.QT_CROSS_DOCKING)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_DET_AGENDA>()
                .Property(e => e.VL_PRECIO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_DET_AGENDA>()
                .Property(e => e.QT_RECIBIDA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_DET_AGENDA>()
                .Property(e => e.QT_ACEPTADA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_DET_AGENDA>()
                .Property(e => e.QT_AGENDADO_ORIGINAL)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_DET_AGENDA>()
                .Property(e => e.QT_RECIBIDA_FICTICIA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_DET_AGENDA>()
                .Property(e => e.VL_CIF)
                .HasPrecision(12, 3);
            #endregion << T_DET_AGENDA

            #region >> T_DET_ETIQUETA_LOTE
            modelBuilder.Entity<T_DET_ETIQUETA_LOTE>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<T_DET_ETIQUETA_LOTE>()
                .Property(e => e.QT_PRODUTO_RECIBIDO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_DET_ETIQUETA_LOTE>()
                .Property(e => e.QT_PRODUTO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_DET_ETIQUETA_LOTE>()
                .Property(e => e.QT_AJUSTE_RECIBIDO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_DET_ETIQUETA_LOTE>()
                .Property(e => e.QT_ETIQUETA_GENERADA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_DET_ETIQUETA_LOTE>()
                .Property(e => e.QT_ALMACENADO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_DET_ETIQUETA_LOTE>()
                .Property(e => e.QT_RASTREO_PALLET)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_DET_ETIQUETA_LOTE>()
                .Property(e => e.QT_MOVILIZADO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_DET_ETIQUETA_LOTE>()
                .Property(e => e.PS_PRODUTO_RECIBIDO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_DET_ETIQUETA_LOTE>()
                .Property(e => e.PS_PRODUTO)
                .HasPrecision(12, 3);
            #endregion << T_DET_ETIQUETA_LOTE

            #region >> T_EMPRESA
            modelBuilder.Entity<T_EMPRESA>()
                .Property(e => e.VL_POS_PALETE)
                .HasPrecision(15, 2);
            modelBuilder.Entity<T_EMPRESA>()
                .Property(e => e.VL_POS_PALETE_DIA)
                .HasPrecision(15, 2);
            modelBuilder.Entity<T_EMPRESA>()
                .Property(e => e.IM_MINIMO_STOCK)
                .HasPrecision(17, 3);
            //modelBuilder.Entity<T_EMPRESA>()
            //    .HasMany(e => e.T_PRODUTO)
            //    .WithOne(e => e.T_EMPRESA)
            //    .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<T_EMPRESA>()
            //    .HasMany(e => e.T_EMPRESA_FUNCIONARIO)
            //    .WithOne(e => e.T_EMPRESA)
            //    .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<T_EMPRESA>()
            //    .HasMany(e => e.T_ENDERECO_ESTOQUE)
            //    .WithOne(e => e.T_EMPRESA)
            //    .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<T_EMPRESA>()
            //  .HasMany(e => e.T_RECEPCION_REFERENCIA)
            //  .WithOne(e => e.T_EMPRESA)
            //  .OnDelete(DeleteBehavior.Restrict);
            #endregion << T_EMPRESA        

            #region >> T_ETIQUETA_LOTE
            modelBuilder.Entity<T_ETIQUETA_LOTE>()
                .HasMany(e => e.T_DET_ETIQUETA_LOTE)
                .WithOne(e => e.T_ETIQUETA_LOTE)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion << T_ETIQUETA_LOTE

            #region >> T_LOG_ETIQUETA
            modelBuilder.Entity<T_LOG_ETIQUETA>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<T_LOG_ETIQUETA>()
                .Property(e => e.QT_MOVIMIENTO)
                .HasPrecision(12, 3);
            #endregion << T_LOG_ETIQUETA

            #region >> T_PAIS
            modelBuilder.Entity<T_PAIS>()
                .HasMany(e => e.T_PAIS_SUBDIVISION)
                .WithOne(e => e.T_PAIS)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion << T_PAIS

            #region >> T_PAIS_SUBDIVISION
            modelBuilder.Entity<T_PAIS_SUBDIVISION>()
                .HasMany(e => e.T_PAIS_SUBDIVISION_LOCALIDAD)
                .WithOne(e => e.T_PAIS_SUBDIVISION)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion << T_PAIS_SUBDIVISION

            #region >> T_PICKING_PRODUTO
            modelBuilder.Entity<T_PICKING_PRODUTO>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<T_PICKING_PRODUTO>()
                .Property(e => e.QT_PADRAO_PICKING)
                .HasPrecision(18, 3);
            #endregion << T_PICKING_PRODUTO

            #region >> T_PRODUTO
            modelBuilder.Entity<T_PRODUTO>()
                .Property(e => e.PS_LIQUIDO)
                .HasPrecision(15, 6);
            modelBuilder.Entity<T_PRODUTO>()
                .Property(e => e.PS_BRUTO)
                .HasPrecision(15, 6);
            modelBuilder.Entity<T_PRODUTO>()
                .Property(e => e.FT_CONVERSAO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_PRODUTO>()
                .Property(e => e.VL_CUBAGEM)
                .HasPrecision(14, 4);
            modelBuilder.Entity<T_PRODUTO>()
                .Property(e => e.VL_PRECO_VENDA)
                .HasPrecision(16, 2);
            modelBuilder.Entity<T_PRODUTO>()
                .Property(e => e.VL_CUSTO_ULT_ENT)
                .HasPrecision(16, 2);
            modelBuilder.Entity<T_PRODUTO>()
                .Property(e => e.VL_ALTURA)
                .HasPrecision(10, 4);
            modelBuilder.Entity<T_PRODUTO>()
                .Property(e => e.VL_LARGURA)
                .HasPrecision(10, 4);
            modelBuilder.Entity<T_PRODUTO>()
                .Property(e => e.VL_PROFUNDIDADE)
                .HasPrecision(10, 4);
            modelBuilder.Entity<T_PRODUTO>()
                .Property(e => e.VL_AVISO_AJUSTE)
                .HasPrecision(14, 4);
            modelBuilder.Entity<T_PRODUTO>()
                .Property(e => e.QT_UND_DISTRIBUCION)
                .HasPrecision(9, 3);
            modelBuilder.Entity<T_PRODUTO>()
                .Property(e => e.QT_UND_BULTO)
                .HasPrecision(9, 3);
            modelBuilder.Entity<T_PRODUTO>()
                .Property(e => e.VL_PRECIO_SEG_DISTR)
                .HasPrecision(10, 4);
            modelBuilder.Entity<T_PRODUTO>()
                .Property(e => e.VL_PRECIO_SEG_STOCK)
                .HasPrecision(10, 4);
            modelBuilder.Entity<T_PRODUTO>()
                .Property(e => e.VL_PRECIO_DISTRIB)
                .HasPrecision(10, 4);
            modelBuilder.Entity<T_PRODUTO>()
                .Property(e => e.VL_PRECIO_EGRESO)
                .HasPrecision(10, 4);
            modelBuilder.Entity<T_PRODUTO>()
                .Property(e => e.VL_PRECIO_INGRESO)
                .HasPrecision(10, 4);
            modelBuilder.Entity<T_PRODUTO>()
                .Property(e => e.VL_PRECIO_STOCK)
                .HasPrecision(10, 4);
            //modelBuilder.Entity<T_PRODUTO>()
            //    .HasOne(e => e.T_CLASSE)
            //    .WithMany(e => e.T_PRODUTO)
            //    .HasForeignKey(d => d.CD_CLASSE)
            //    .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<T_PRODUTO>()
            //    .HasOne(e => e.T_ROTATIVIDADE)
            //    .WithMany(e => e.T_PRODUTO)
            //    .HasForeignKey(d => d.CD_ROTATIVIDADE)
            //    .OnDelete(DeleteBehavior.Restrict);
            #endregion << T_PRODUTO

            #region >> T_RECEPCION_AGENDA_REFERENCIA
            modelBuilder.Entity<T_RECEPCION_AGENDA_REFERENCIA>()
             .Property(e => e.CD_FAIXA)
             .HasPrecision(9, 3);
            modelBuilder.Entity<T_RECEPCION_AGENDA_REFERENCIA>()
                .Property(e => e.QT_RECIBIDA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_RECEPCION_AGENDA_REFERENCIA>()
                .Property(e => e.QT_AGENDADA)
                .HasPrecision(12, 3);
            #endregion << T_RECEPCION_AGENDA_REFERENCIA

            #region >> T_RECEPCION_AGENDA_PROBLEMA

            modelBuilder.Entity<T_RECEPCION_AGENDA_PROBLEMA>()
                .Property(e => e.VL_DIFERENCIA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_RECEPCION_AGENDA_PROBLEMA>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<T_RECEPCION_AGENDA_PROBLEMA>()
                .HasOne(e => e.T_AGENDA)
                .WithMany(e => e.T_RECEPCION_AGENDA_PROBLEMA)
                .HasForeignKey(e => e.NU_AGENDA);

            #endregion << T_RECEPCION_AGENDA_PROBLEMA

            #region T_RECEPCION_TIPO_REPORTE_DEF
            //modelBuilder.Entity<T_RECEPCION_TIPO_REPORTE_DEF>()
            //    .HasKey(d => new { d.CD_REPORTE, d.TP_RECEPCION });
            //modelBuilder.Entity<T_RECEPCION_TIPO_REPORTE_DEF>()
            //    .HasOne(e => e.T_RECEPCION_TIPO)
            //    .WithMany(e => e.T_RECEPCION_TIPO_REPORTE_DEF)
            //    .HasForeignKey(e => e.TP_RECEPCION);
            //modelBuilder.Entity<T_RECEPCION_TIPO_REPORTE_DEF>()
            //    .HasOne(e => e.T_REPORTE_DEFINICION)
            //    .WithMany(e => e.T_RECEPCION_TIPO_REPORTE_DEF)
            //    .HasForeignKey(e => e.TP_RECEPCION);
            #endregion

            #region >> T_RECEPCION_REFERENCIA
            //modelBuilder.Entity<T_RECEPCION_REFERENCIA>()
            //    .HasMany(e => e.T_RECEPCION_REFERENCIA_DET)
            //    .WithOne(e => e.T_RECEPCION_REFERENCIA)
            //    .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<T_RECEPCION_REFERENCIA>()
            //   .HasMany(e => e.T_RECEPC_AGENDA_REFERENCIA_REL)
            //   .WithOne(e => e.T_RECEPCION_REFERENCIA)
            //   .OnDelete(DeleteBehavior.Restrict);
            #endregion << T_RECEPCION_REFERENCIA

            #region >> T_RECEPCION_REFERENCIA_DET
            modelBuilder.Entity<T_RECEPCION_REFERENCIA_DET>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<T_RECEPCION_REFERENCIA_DET>()
                .Property(e => e.QT_REFERENCIA)
                .HasPrecision(15, 3);
            modelBuilder.Entity<T_RECEPCION_REFERENCIA_DET>()
                .Property(e => e.QT_ANULADA)
                .HasPrecision(15, 3);
            modelBuilder.Entity<T_RECEPCION_REFERENCIA_DET>()
                .Property(e => e.QT_AGENDADA)
                .HasPrecision(15, 3);
            modelBuilder.Entity<T_RECEPCION_REFERENCIA_DET>()
                .Property(e => e.QT_RECIBIDA)
                .HasPrecision(15, 3);
            modelBuilder.Entity<T_RECEPCION_REFERENCIA_DET>()
                .Property(e => e.QT_CONFIRMADA_INTERFAZ)
                .HasPrecision(15, 3);
            modelBuilder.Entity<T_RECEPCION_REFERENCIA_DET>()
                .Property(e => e.IM_UNITARIO)
                .HasPrecision(15, 3);
            //modelBuilder.Entity<T_RECEPCION_REFERENCIA_DET>()
            //  .HasMany(e => e.T_RECEPCION_AGENDA_REFERENCIA)
            //  .WithOne(e => e.T_RECEPCION_REFERENCIA_DET)
            //  .OnDelete(DeleteBehavior.Restrict);
            #endregion << T_RECEPCION_REFERENCIA_DET

            #region >> T_LOG_PEDIDO_ANULADO
            modelBuilder.Entity<T_LOG_PEDIDO_ANULADO>()
                .Property(e => e.QT_ANULADO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_LOG_PEDIDO_ANULADO>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            #endregion << T_LOG_PEDIDO_ANULADO

            #region >> T_ROTA
            //modelBuilder.Entity<T_ROTA>()
            //    .HasMany(e => e.T_CLIENTE)
            //    .WithOne(e => e.T_ROTA)
            //    .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<T_ROTA>()
            //    .HasMany(e => e.T_CLIENTE_RUTA_PREDIO)
            //    .WithOne(e => e.T_ROTA)
            //    .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<T_ROTA>()
            //    .HasOne(e => e.T_PORTA_EMBARQUE)
            //    .WithMany(e => e.T_ROTA)
            //    .HasForeignKey(e => e.CD_PORTA);
            //modelBuilder.Entity<T_ROTA>()
            //    .HasOne(e => e.T_ONDA)
            //    .WithMany(e => e.T_ROTA)
            //    .HasForeignKey(e => e.CD_ONDA);
            #endregion << T_ROTA

            #region >> T_TIPO_ENDERECO
            modelBuilder.Entity<T_TIPO_ENDERECO>()
                .Property(e => e.VL_ALTURA)
                .HasPrecision(12, 5);
            modelBuilder.Entity<T_TIPO_ENDERECO>()
                .Property(e => e.VL_LARGURA)
                .HasPrecision(12, 5);
            modelBuilder.Entity<T_TIPO_ENDERECO>()
                .Property(e => e.VL_COMPRIMENTO)
                .HasPrecision(12, 5);
            modelBuilder.Entity<T_TIPO_ENDERECO>()
                .Property(e => e.VL_PESO_MAXIMO)
                .HasPrecision(9, 3);
            modelBuilder.Entity<T_TIPO_ENDERECO>()
                .Property(e => e.QT_VOLUMEN_UNIDAD_FACTURACION)
                .HasPrecision(11, 4);
            #endregion << T_TIPO_ENDERECO

            #region >> T_ZONA_UBICACION
            /*modelBuilder.Entity<T_ZONA_UBICACION>()
                .HasMany(e => e.T_ZONA_UBICACION1)
                .WithOne(e => e.T_ZONA_UBICACION2)
                .HasForeignKey(e => e.CD_ZONA_UBICACION_PICKING);*/
            #endregion << T_ZONA_UBICACION

            #endregion - FIN FABRICA TABLAS -

            #region - FABRICA VISTAS -

            #region >> V_REG009_PRODUCTOS
            modelBuilder.Entity<V_REG009_PRODUCTOS>()
                .Property(e => e.PS_BRUTO)
                .HasPrecision(15, 6);
            modelBuilder.Entity<V_REG009_PRODUCTOS>()
                .Property(e => e.PS_LIQUIDO)
                .HasPrecision(15, 6);
            modelBuilder.Entity<V_REG009_PRODUCTOS>()
                .Property(e => e.VL_ALTURA)
                .HasPrecision(10, 4);
            modelBuilder.Entity<V_REG009_PRODUCTOS>()
                .Property(e => e.VL_LARGURA)
                .HasPrecision(10, 4);
            modelBuilder.Entity<V_REG009_PRODUCTOS>()
                .Property(e => e.VL_PROFUNDIDADE)
                .HasPrecision(10, 4);
            modelBuilder.Entity<V_REG009_PRODUCTOS>()
                .Property(e => e.VL_CUBAGEM)
                .HasPrecision(14, 4);
            modelBuilder.Entity<V_REG009_PRODUCTOS>()
                .Property(e => e.FT_CONVERSAO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REG009_PRODUCTOS>()
                .Property(e => e.VL_AVISO_AJUSTE)
                .HasPrecision(14, 4);
            modelBuilder.Entity<V_REG009_PRODUCTOS>()
                .Property(e => e.QT_UND_DISTRIBUCION)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_REG009_PRODUCTOS>()
                .Property(e => e.VL_CUSTO_ULT_ENT)
                .HasPrecision(16, 2);
            modelBuilder.Entity<V_REG009_PRODUCTOS>()
                .Property(e => e.VL_PRECO_VENDA)
                .HasPrecision(16, 2);
            modelBuilder.Entity<V_REG009_PRODUCTOS>()
                .Property(e => e.QT_UND_BULTO)
                .HasPrecision(9, 3);
            #endregion << V_REG009_PRODUCTOS

            #region >> V_REG050_PICKING_PRODUCTOS
            modelBuilder.Entity<V_REG050_PICKING_PRODUCTO>()
                .Property(e => e.DS_QT_BULTO)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_REG050_PICKING_PRODUCTO>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_REG050_PICKING_PRODUCTO>()
                .Property(e => e.QT_PADRAO_PICKING)
                .HasPrecision(18, 3);
            #endregion << V_REG050_PICKING_PRODUCTOS

            #region >> V_EMPRESAS_WREG100
            modelBuilder.Entity<V_EMPRESAS_WREG100>()
                .Property(e => e.IM_MINIMO_STOCK)
                .HasPrecision(17, 3);
            #endregion << V_EMPRESAS_WREG100

            #region >> V_EGRESO_CAMION_WEXP
            modelBuilder.Entity<V_EGRESO_CAMION_WEXP>()
                .HasKey(e => new { e.CD_CAMION, e.CD_PRODUTO, e.CD_EMPRESA, e.CD_CLIENTE, e.NU_IDENTIFICADOR, e.CD_FAIXA });
            #endregion << V_EGRESO_CAMION_WEXP

            #region >> V_EGRESO_GEN_DOCOCUMENTO_WEXP
            modelBuilder.Entity<V_EGRESO_GEN_DOCOCUMENTO_WEXP>()
                .HasKey(e => new { e.NU_PREPARACION, e.CD_CAMION, e.CD_EMPRESA, e.CD_PRODUTO, e.CD_FAIXA, e.NU_IDENTIFICADOR });
            #endregion << V_EGRESO_GEN_DOCOCUMENTO_WEXP

            #region >> V_EGRESO_CONTROL_DOC_WEXP
            modelBuilder.Entity<V_EGRESO_CONTROL_DOC_WEXP>()
                .HasKey(e => new { e.NU_PREPARACION, e.CD_CAMION, e.CD_EMPRESA, e.CD_PRODUTO, e.CD_FAIXA, e.NU_IDENTIFICADOR });
            #endregion << V_EGRESO_CONTROL_DOC_WEXP

            #region >> V_EXP041_PEDIDOS_EXPEDIDOS
            modelBuilder.Entity<V_EXP041_PEDIDOS_EXPEDIDOS>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_EXP041_PEDIDOS_EXPEDIDOS>()
                .Property(e => e.QT_PRODUTO)
                .HasPrecision(12, 3);
            #endregion

            #region >> V_PEDIDOS_PENDIENTES
            modelBuilder.Entity<V_PEDIDOS_PENDIENTES>()
                .Property(e => e.QT_PENDIENTE)
                .HasPrecision(38, 0);
            #endregion << V_PEDIDOS_PENDIENTES

            #region >> V_PRE061_DET_PREPARACION
            modelBuilder.Entity<V_PRE061_DET_PREPARACION>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_PRE061_DET_PREPARACION>()
                .Property(e => e.QT_PRODUTO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRE061_DET_PREPARACION>()
                .Property(e => e.QT_PREPARADO)
                .HasPrecision(12, 3);
            #endregion  << V_DET_PREPARACION_WPRE061

            #region >> V_PRE110_DET_PEDIDO_SALIDA
            modelBuilder.Entity<V_PRE110_DET_PEDIDO_SALIDA>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_PRE110_DET_PEDIDO_SALIDA>()
                .Property(e => e.QT_PENDIENTE)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PRE110_DET_PEDIDO_SALIDA>()
                .Property(e => e.AUXQT_ANULADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PRE110_DET_PEDIDO_SALIDA>()
                .Property(e => e.QT_PEDIDO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRE110_DET_PEDIDO_SALIDA>()
                .Property(e => e.QT_LIBERADO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRE110_DET_PEDIDO_SALIDA>()
                .Property(e => e.QT_ANULADO)
                .HasPrecision(12, 3);
            #endregion << V_PRE110_DET_PEDIDO_SALIDA

            #region >> V_PRE130_DET_PICKING
            modelBuilder.Entity<V_PRE130_DET_PICKING>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_PRE130_DET_PICKING>()
                .Property(e => e.QT_PRODUTO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRE130_DET_PICKING>()
                .Property(e => e.QT_PREPARADO)
                .HasPrecision(12, 3);
            #endregion << V_PRE130_DET_PICKING

            #region >> V_PRE150_DETALLE_PEDIDO
            modelBuilder.Entity<V_PRE150_DETALLE_PEDIDO>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_PRE150_DETALLE_PEDIDO>()
                .Property(e => e.QT_PEDIDO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRE150_DETALLE_PEDIDO>()
                .Property(e => e.QT_LIBERADO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRE150_DETALLE_PEDIDO>()
                .Property(e => e.QT_ANULADO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRE150_DETALLE_PEDIDO>()
                .Property(e => e.QT_PENDIENTE)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PRE150_DETALLE_PEDIDO>()
                .Property(e => e.QT_PENDIENTE_PREP)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PRE150_DETALLE_PEDIDO>()
                .Property(e => e.QT_EXPEDIDO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PRE150_DETALLE_PEDIDO>()
                .Property(e => e.QT_PEDIDO_ORIGINAL)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRE150_DETALLE_PEDIDO>()
                .Property(e => e.QT_PREPARADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PRE150_DETALLE_PEDIDO>()
                .Property(e => e.QT_FACTURADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PRE150_DETALLE_PEDIDO>()
                .Property(e => e.NU_GENERICO_1)
                .HasPrecision(15, 3);
            #endregion << V_PRE150_DETALLE_PEDIDO

            #region >> V_PRE150_DET_PEDIDO_SALIDA
            modelBuilder.Entity<V_PRE150_DET_PEDIDO_SALIDA>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_PRE150_DET_PEDIDO_SALIDA>()
                .Property(e => e.QT_PEDIDO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRE150_DET_PEDIDO_SALIDA>()
                .Property(e => e.QT_LIBERADO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRE150_DET_PEDIDO_SALIDA>()
                .Property(e => e.QT_ANULADO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRE150_DET_PEDIDO_SALIDA>()
                .Property(e => e.QT_PENDIENTE)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PRE150_DET_PEDIDO_SALIDA>()
                .Property(e => e.QT_PENDIENTE_PREP)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PRE150_DET_PEDIDO_SALIDA>()
                .Property(e => e.QT_EXPEDIDO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PRE150_DET_PEDIDO_SALIDA>()
                .Property(e => e.QT_PEDIDO_ORIGINAL)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRE150_DET_PEDIDO_SALIDA>()
                .Property(e => e.QT_PREPARADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PRE150_DET_PEDIDO_SALIDA>()
                .Property(e => e.QT_FACTURADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PRE150_DET_PEDIDO_SALIDA>()
                .Property(e => e.NU_GENERICO_1)
                .HasPrecision(15, 3);
            #endregion << V_PRE150_DET_PEDIDO_SALIDA

            #region >> V_PRE160_PICKING_PENDIENTE
            modelBuilder.Entity<V_PRE160_PICKING_PENDIENTE>()
                .Property(e => e.QT_PREPARACIONES)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PRE160_PICKING_PENDIENTE>()
                .Property(e => e.QT_PEDIDOS)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PRE160_PICKING_PENDIENTE>()
                .Property(e => e.QT_CLIENTES)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PRE160_PICKING_PENDIENTE>()
                .Property(e => e.QT_PRODUCTOS)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PRE160_PICKING_PENDIENTE>()
                .Property(e => e.QT_PICKEOS)
                .HasPrecision(38, 0);
            #endregion

            #region >> V_PRE161_PICKING_PENDIENTE

            modelBuilder.Entity<V_PRE161_PICKING_PENDIENTE>()
                .Property(e => e.QT_PRODUCTOS)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PRE161_PICKING_PENDIENTE>()
                .Property(e => e.AUXUNIDADES_PREPARADAS)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PRE161_PICKING_PENDIENTE>()
                .Property(e => e.AUXPORC_UNIDADES)
                .HasPrecision(38, 0);

            #endregion

            #region >> V_PRE162_PICKING_PENDIENTE
            modelBuilder.Entity<V_PRE162_PICKING_PENDIENTE>()
                .Property(e => e.QT_PRODUCTOS)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PRE162_PICKING_PENDIENTE>()
                .Property(e => e.AUXUNIDADES_PREPARADAS)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PRE162_PICKING_PENDIENTE>()
                .Property(e => e.AUXPORC_UNIDADES)
                .HasPrecision(38, 0);

            modelBuilder.Entity<V_PRE162_PICKING_PENDIENTE>()
                .Property(e => e.AUXPICKEOS_PREPARADOS)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PRE162_PICKING_PENDIENTE>()
                .Property(e => e.AUXPORC_PICKEOS)
                .HasPrecision(38, 0);
            #endregion

            #region >> V_PRE170_ANALISIS_RECHAZO
            modelBuilder.Entity<V_PRE170_ANALISIS_RECHAZO>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_PRE170_ANALISIS_RECHAZO>()
                .Property(e => e.QT_RECHAZADO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRE170_ANALISIS_RECHAZO>()
                .Property(e => e.QT_ESTOQUE)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRE170_ANALISIS_RECHAZO>()
                .Property(e => e.QT_AVERIA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRE170_ANALISIS_RECHAZO>()
                .Property(e => e.QT_PREPARACION)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRE170_ANALISIS_RECHAZO>()
                .Property(e => e.QT_DIFERENCIA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRE170_ANALISIS_RECHAZO>()
                .Property(e => e.QT_CONTENEDOR)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRE170_ANALISIS_RECHAZO>()
                .Property(e => e.QT_PENDIENTE_ALMACENAR)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRE170_ANALISIS_RECHAZO>()
                .Property(e => e.QT_TRANSFERENCIA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRE170_ANALISIS_RECHAZO>()
                .Property(e => e.QT_CTRL_CALIDAD)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRE170_ANALISIS_RECHAZO>()
                .Property(e => e.QT_DUAS)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRE170_ANALISIS_RECHAZO>()
                .Property(e => e.QT_EN_AREA_NO_DISP)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRE170_ANALISIS_RECHAZO>()
                .Property(e => e.QT_VENCIDO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRE170_ANALISIS_RECHAZO>()
                .Property(e => e.QT_DESPREPARADO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRE170_ANALISIS_RECHAZO>()
                .Property(e => e.QT_OTRO_PREDIO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRE170_ANALISIS_RECHAZO>()
                .Property(e => e.QT_ESTOQUE_ALMACEN)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PRE170_ANALISIS_RECHAZO>()
                .Property(e => e.QT_ESTOQUE_MENUDENCIA)
                .HasPrecision(38, 0);
            #endregion << V_PRE170_ANALISIS_RECHAZO            

            #region >> V_REC170_REFERENCIA
            modelBuilder.Entity<V_REC170_RECEPCION>()
                .Property(e => e.VL_TEMP_CAMION)
                .HasPrecision(6, 2);
            modelBuilder.Entity<V_REC170_RECEPCION>()
                .Property(e => e.VL_TEMP_INICIO_REC)
                .HasPrecision(6, 2);
            modelBuilder.Entity<V_REC170_RECEPCION>()
                .Property(e => e.VL_TEMP_MITAD_REC)
                .HasPrecision(6, 2);
            modelBuilder.Entity<V_REC170_RECEPCION>()
                .Property(e => e.VL_TEMP_FINAL_REC)
                .HasPrecision(6, 2);

            modelBuilder.Entity<V_REC170_VALIDAR_FACTURA>()
                   .HasKey(e => new { e.NU_AGENDA, e.CD_PRODUTO });

            #endregion << V_REC170_REFERENCIA

            #region >> V_REC171_AGENDA_DETALLE
            modelBuilder.Entity<V_REC171_AGENDA_DETALLE>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_REC171_AGENDA_DETALLE>()
                .Property(e => e.QT_AGENDADO_ORIGINAL)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_REC171_AGENDA_DETALLE>()
                .Property(e => e.QT_AGENDADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_REC171_AGENDA_DETALLE>()
                .Property(e => e.PESO_AGENDADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_REC171_AGENDA_DETALLE>()
                .Property(e => e.VOLUMEN_AGENDADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_REC171_AGENDA_DETALLE>()
                .Property(e => e.QT_RECIBIDA)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_REC171_AGENDA_DETALLE>()
                .Property(e => e.PESO_RECIBIDO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_REC171_AGENDA_DETALLE>()
                .Property(e => e.VOLUMEN_RECIBIDO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_REC171_AGENDA_DETALLE>()
                .Property(e => e.CANT_ETIQUETAS)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_REC171_AGENDA_DETALLE>()
                .Property(e => e.QT_UND_BULTO)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_REC171_AGENDA_DETALLE>()
                .Property(e => e.QT_BULTOS_RECIBIDO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_REC171_AGENDA_DETALLE>()
                .Property(e => e.QT_BULTOS_AGENDADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_REC171_AGENDA_DETALLE>()
                .Property(e => e.VL_PRECO_VENDA)
                .HasPrecision(16, 2);
            #endregion << V_REC171_AGENDA_DETALLE

            #region >> V_REC011_REC_REFERENCIA_DET
            modelBuilder.Entity<V_REC011_REC_REFERENCIA_DET>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_REC011_REC_REFERENCIA_DET>()
                .Property(e => e.QT_REFERENCIA)
                .HasPrecision(15, 3);
            modelBuilder.Entity<V_REC011_REC_REFERENCIA_DET>()
                .Property(e => e.QT_ANULADA)
                .HasPrecision(15, 3);
            modelBuilder.Entity<V_REC011_REC_REFERENCIA_DET>()
                .Property(e => e.QT_AGENDADA)
                .HasPrecision(15, 3);
            modelBuilder.Entity<V_REC011_REC_REFERENCIA_DET>()
                .Property(e => e.QT_RECIBIDA)
                .HasPrecision(15, 3);
            modelBuilder.Entity<V_REC011_REC_REFERENCIA_DET>()
                .Property(e => e.QT_CONFIRMADA_INTERFAZ)
                .HasPrecision(15, 3);
            modelBuilder.Entity<V_REC011_REC_REFERENCIA_DET>()
                .Property(e => e.IM_UNITARIO)
                .HasPrecision(15, 3);
            modelBuilder.Entity<V_REC011_REC_REFERENCIA_DET>()
                .Property(e => e.QT_SALDO)
                .HasPrecision(38, 0);
            #endregion << V_REC011_REC_REFERENCIA_DET

            #region >> V_REC141_AGENDA_PROBLEMA
            modelBuilder.Entity<V_REC141_AGENDA_PROBLEMA>()
                .Property(e => e.VL_DIFERENCIA)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_REC141_AGENDA_PROBLEMA>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            #endregion << V_REC141_AGENDA_PROBLEMA

            #region >> V_REC150_ETIQUETAS
            modelBuilder.Entity<V_REC150_ETIQUETAS>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_REC150_ETIQUETAS>()
                .Property(e => e.QT_PRODUTO_RECIBIDO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REC150_ETIQUETAS>()
                .Property(e => e.QT_ALMACENADO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REC150_ETIQUETAS>()
                .Property(e => e.QT_PRODUTO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REC150_ETIQUETAS>()
                .Property(e => e.QT_ETIQUETA_GENERADA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REC150_ETIQUETAS>()
                .Property(e => e.QT_MOVILIZADO)
                .HasPrecision(12, 3);
            #endregion << V_REC150_ETIQUETAS

            #region >> V_REC180_LOG_ETIQUETAS
            modelBuilder.Entity<V_REC180_LOG_ETIQUETAS>()
                .Property(e => e.QT_PRODUTO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REC180_LOG_ETIQUETAS>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            #endregion << V_REC180_LOG_ETIQUETAS            

            #region >> V_STO060_CTR_CALIDAD
            modelBuilder.Entity<V_STO060_CTR_CALIDAD>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            #endregion << V_STO060_CTR_CALIDAD

            #region >> V_STO500_STOCK_POR_PRODUCTO
            modelBuilder.Entity<V_STO500_STOCK_POR_PRODUCTO>()
                .Property(e => e.QT_REAL_ESTOQUE)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_STO500_STOCK_POR_PRODUCTO>()
                .Property(e => e.QT_ESTOQUE)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_STO500_STOCK_POR_PRODUCTO>()
                .Property(e => e.QT_RESERVA_SAIDA)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_STO500_STOCK_POR_PRODUCTO>()
                .Property(e => e.QT_TRANSITO_ENTRADA)
                .HasPrecision(38, 0);
            #endregion << V_STO500_STOCK_POR_PRODUCTO

            #endregion - FIN FABRICA VISTAS -

            #region >> Porteria
            modelBuilder.Entity<T_PORTERIA_REGISTRO_VEHICULO>()
                .Property(e => e.VL_PESO_ENTRADA)
                .HasPrecision(12, 4);
            modelBuilder.Entity<T_PORTERIA_REGISTRO_VEHICULO>()
                .Property(e => e.VL_PESO_SALIDA)
                .HasPrecision(12, 4);
            modelBuilder.Entity<V_PORTERIA_ENTRADA_SIN_AGENDA>()
                .Property(e => e.VL_PESO_ENTRADA)
                .HasPrecision(12, 4);
            modelBuilder.Entity<V_PORTERIA_ENTRADA_SIN_AGENDA>()
                .Property(e => e.VL_PESO_SALIDA)
                .HasPrecision(12, 4);
            modelBuilder.Entity<V_PORTERIA_PERSONA>()
                .Property(e => e.VL_PESO_ENTRADA_VE)
                .HasPrecision(12, 4);
            modelBuilder.Entity<V_PORTERIA_PERSONA>()
                .Property(e => e.VL_PESO_SALIDA_VE)
                .HasPrecision(12, 4);
            modelBuilder.Entity<V_PORTERIA_PERSONA>()
                .Property(e => e.VL_PESO_ENTRADA_VS)
                .HasPrecision(12, 4);
            modelBuilder.Entity<V_PORTERIA_PERSONA>()
                .Property(e => e.VL_PESO_SALIDA_VS)
                .HasPrecision(12, 4);
            modelBuilder.Entity<V_PORTERIA_SALIDA_SIN_EGRESO>()
                .Property(e => e.VL_PESO_ENTRADA)
                .HasPrecision(12, 4);
            modelBuilder.Entity<V_PORTERIA_SALIDA_SIN_EGRESO>()
                .Property(e => e.VL_PESO_SALIDA)
                .HasPrecision(12, 4);
            modelBuilder.Entity<V_PORTERIA_VEHICULO>()
                .Property(e => e.VL_PESO_ENTRADA)
                .HasPrecision(12, 4);
            modelBuilder.Entity<V_PORTERIA_VEHICULO>()
                .Property(e => e.VL_PESO_SALIDA)
                .HasPrecision(12, 4);
            modelBuilder.Entity<V_POTERIA_VEHICULO_AGENDA>()
                .Property(e => e.VL_PESO_ENTRADA)
                .HasPrecision(12, 4);
            modelBuilder.Entity<V_POTERIA_VEHICULO_AGENDA>()
                .Property(e => e.VL_PESO_SALIDA)
                .HasPrecision(12, 4);
            modelBuilder.Entity<V_POTERIA_VEHICULO_CAMION>()
                .Property(e => e.VL_PESO_ENTRADA)
                .HasPrecision(12, 4);
            modelBuilder.Entity<V_POTERIA_VEHICULO_CAMION>()
                .Property(e => e.VL_PESO_SALIDA)
                .HasPrecision(12, 4);
            #endregion << Porteria

            #region >> - PRDC PRODUCCION - 

            #region T_PRDC_DEFINICION

            modelBuilder.Entity<T_PRDC_DEFINICION>()
                .Property(e => e.CD_PRDC_DEFINICION)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_DEFINICION>()
                .Property(e => e.NM_PRDC_DEFINICION)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_DEFINICION>()
                .Property(e => e.DS_PRDC_DEFINICION)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_DEFINICION>()
                .Property(e => e.TP_PRDC_DEFINICION)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_DEFINICION>()
                .HasMany(e => e.T_PRDC_DET_ENTRADA)
                .WithOne(e => e.T_PRDC_DEFINICION)
                .HasForeignKey(e => e.CD_PRDC_DEFINICION)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<T_PRDC_DEFINICION>()
                .HasMany(e => e.T_PRDC_DET_SALIDA)
                .WithOne(e => e.T_PRDC_DEFINICION)
                .HasForeignKey(e => e.CD_PRDC_DEFINICION)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<T_PRDC_DEFINICION>()
                .HasMany(e => e.T_PRDC_INGRESO)
                .WithOne(e => e.T_PRDC_DEFINICION)
                .HasForeignKey(e => e.CD_PRDC_DEFINICION)

                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<T_PRDC_DEFINICION>()
                .HasOne(e => e.T_SITUACAO)
                .WithMany(e => e.T_PRDC_DEFINICION)
                .HasForeignKey(e => e.CD_SITUACAO)
                .IsRequired();

            modelBuilder.Entity<T_PRDC_DEFINICION>()
               .HasOne(e => e.T_EMPRESA)
               .WithMany(e => e.T_PRDC_DEFINICION)
               .HasForeignKey(e => e.CD_EMPRESA)
               .IsRequired();

            #endregion

            #region T_PRDC_DET_ENTRADA

            modelBuilder.Entity<T_PRDC_DET_ENTRADA>()
                .Property(e => e.CD_PRDC_DEFINICION)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_DET_ENTRADA>()
                .Property(e => e.CD_COMPONENTE)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_DET_ENTRADA>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_DET_ENTRADA>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<T_PRDC_DET_ENTRADA>()
                .Property(e => e.QT_COMPLETA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_PRDC_DET_ENTRADA>()
                .Property(e => e.QT_INCOMPLETA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_PRDC_DET_ENTRADA>()
                .Property(e => e.QT_CONSUMIDA_LINEA)
                .HasPrecision(12, 3);

            #endregion

            #region T_PRDC_DET_SALIDA

            modelBuilder.Entity<T_PRDC_DET_SALIDA>()
                .Property(e => e.CD_PRDC_DEFINICION)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_DET_SALIDA>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_DET_SALIDA>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<T_PRDC_DET_SALIDA>()
                .Property(e => e.QT_COMPLETA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_PRDC_DET_SALIDA>()
                .Property(e => e.QT_INCOMPLETA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_PRDC_DET_SALIDA>()
                .Property(e => e.QT_CONSUMIDA_LINEA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_PRDC_DET_SALIDA>()
                .Property(e => e.ID_PRODUTO_FINAL)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_DET_SALIDA>()
                .HasOne(e => e.T_PRODUTO_FAIXA)
                .WithMany(e => e.T_PRDC_DET_SALIDA)
                .HasForeignKey(e => new { e.CD_EMPRESA, e.CD_PRODUTO, e.CD_FAIXA })
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region T_PRDC_INGRESO

            modelBuilder.Entity<T_PRDC_INGRESO>()
                .Property(e => e.NU_PRDC_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_INGRESO>()
                .Property(e => e.CD_PRDC_DEFINICION)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_INGRESO>()
                .Property(e => e.ID_GENERAR_PEDIDO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_INGRESO>()
                .Property(e => e.NU_PRDC_ORIGINAL)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_INGRESO>()
                .Property(e => e.DS_ANEXO1)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_INGRESO>()
                .Property(e => e.DS_ANEXO2)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_INGRESO>()
                .Property(e => e.DS_ANEXO3)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_INGRESO>()
                .Property(e => e.DS_ANEXO4)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_INGRESO>()
                .Property(e => e.ND_TIPO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_INGRESO>()
                .Property(e => e.CD_PRDC_LINEA)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_INGRESO>()
                .HasMany(e => e.T_PRDC_INGRESO_DOCUMENTO)
                .WithOne(e => e.T_PRDC_INGRESO)
                .HasForeignKey(e => e.NU_PRDC_INGRESO)
                .IsRequired();

            modelBuilder.Entity<T_PRDC_INGRESO>()
                .HasMany(e => e.T_PRDC_INGRESO_PASADA)
                .WithOne(e => e.T_PRDC_INGRESO)
                .HasForeignKey(e => e.NU_PRDC_INGRESO)
                .IsRequired();

            modelBuilder.Entity<T_PRDC_INGRESO>()
                .HasOne(e => e.T_PRDC_DEFINICION)
                .WithMany(e => e.T_PRDC_INGRESO)
                .HasForeignKey(e => e.CD_PRDC_DEFINICION);

            modelBuilder.Entity<T_PRDC_INGRESO>()
                .HasOne(e => e.T_SITUACAO)
                .WithMany(e => e.T_PRDC_INGRESO)
                .HasForeignKey(e => e.CD_SITUACAO)
                .IsRequired();

            modelBuilder.Entity<T_PRDC_INGRESO>()
               .Property(e => e.TP_FLUJO)
               .IsUnicode(false);

            #endregion



            #region T_PRDC_INGRESO_DOCUMENTO

            modelBuilder.Entity<T_PRDC_INGRESO_DOCUMENTO>()
                .Property(e => e.NU_DOCUMENTO_EGR)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_INGRESO_DOCUMENTO>()
                .Property(e => e.TP_DOCUMENTO_EGR)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_INGRESO_DOCUMENTO>()
                .Property(e => e.NU_DOCUMENTO_ING)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_INGRESO_DOCUMENTO>()
                .Property(e => e.TP_DOCUMENTO_ING)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_INGRESO_DOCUMENTO>()
                .Property(e => e.NU_PRDC_INGRESO)
                .IsUnicode(false);

            #endregion

            #region T_PRDC_LINEA

            modelBuilder.Entity<T_PRDC_LINEA>()
                .Property(e => e.CD_PRDC_LINEA)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_LINEA>()
                .Property(e => e.DS_PRDC_LINEA)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_LINEA>()
                .Property(e => e.CD_ENDERECO_ENTRADA)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_LINEA>()
                .Property(e => e.CD_ENDERECO_SALIDA)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_LINEA>()
                .Property(e => e.CD_ENDERECO_SALIDA_TRAN)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_LINEA>()
                .Property(e => e.NU_PRDC_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_LINEA>()
                .Property(e => e.ND_TIPO_LINEA)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_LINEA>()
                .Property(e => e.CD_ENDERECO_PRODUCCION)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_LINEA>()
                .Property(e => e.NU_PREDIO)
                .IsUnicode(false);

            #endregion

            #region T_PRDC_LINEA_CONSUMIDO

            modelBuilder.Entity<T_PRDC_LINEA_CONSUMIDO>()
                .Property(e => e.CD_PRDC_DEFINICION)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_LINEA_CONSUMIDO>()
                .Property(e => e.NU_PRDC_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_LINEA_CONSUMIDO>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_LINEA_CONSUMIDO>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<T_PRDC_LINEA_CONSUMIDO>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_LINEA_CONSUMIDO>()
                .Property(e => e.QT_CONSUMIDO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_PRDC_LINEA_CONSUMIDO>()
               .Property(e => e.FL_SEMIACABADO)
               .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_LINEA_CONSUMIDO>()
              .Property(e => e.FL_CONSUMIBLE)
              .IsUnicode(false);

            #endregion

            #region T_PRDC_LINEA_PRODUCIDO

            modelBuilder.Entity<T_PRDC_LINEA_PRODUCIDO>()
                .Property(e => e.CD_PRDC_DEFINICION)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_LINEA_PRODUCIDO>()
                .Property(e => e.NU_PRDC_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_LINEA_PRODUCIDO>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_LINEA_PRODUCIDO>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<T_PRDC_LINEA_PRODUCIDO>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_LINEA_PRODUCIDO>()
                .Property(e => e.QT_PRODUCIDO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_PRDC_LINEA_PRODUCIDO>()
               .Property(e => e.FL_SEMIACABADO)
               .IsUnicode(false);

            #endregion

            #region T_PRDC_CONSUMO_IDENTIFICADOR

            modelBuilder.Entity<T_PRDC_CONSUMO_IDENTIFICADOR>()
                .Property(e => e.CD_ENDERECO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_CONSUMO_IDENTIFICADOR>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_CONSUMO_IDENTIFICADOR>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<T_PRDC_CONSUMO_IDENTIFICADOR>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_CONSUMO_IDENTIFICADOR>()
                .Property(e => e.QT_STOCK)
                .HasPrecision(12, 3);

            #endregion

            #region T_HIST_PRDC_LINEA_CONSUMIDO

            modelBuilder.Entity<T_HIST_PRDC_LINEA_CONSUMIDO>()
               .Property(e => e.CD_PRDC_DEFINICION)
               .IsUnicode(false);

            modelBuilder.Entity<T_HIST_PRDC_LINEA_CONSUMIDO>()
                .Property(e => e.NU_PRDC_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<T_HIST_PRDC_LINEA_CONSUMIDO>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_HIST_PRDC_LINEA_CONSUMIDO>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<T_HIST_PRDC_LINEA_CONSUMIDO>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<T_HIST_PRDC_LINEA_CONSUMIDO>()
                .Property(e => e.QT_CONSUMIDO)
                .HasPrecision(12, 3);

            #endregion

            #region T_HIST_PRDC_LINEA_PRODUCIDO

            modelBuilder.Entity<T_HIST_PRDC_LINEA_PRODUCIDO>()
            .Property(e => e.CD_PRDC_DEFINICION)
            .IsUnicode(false);

            modelBuilder.Entity<T_HIST_PRDC_LINEA_PRODUCIDO>()
                .Property(e => e.NU_PRDC_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<T_HIST_PRDC_LINEA_PRODUCIDO>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_HIST_PRDC_LINEA_PRODUCIDO>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<T_HIST_PRDC_LINEA_PRODUCIDO>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<T_HIST_PRDC_LINEA_PRODUCIDO>()
                .Property(e => e.QT_PRODUCIDO)
                .HasPrecision(12, 3);

            #endregion

            #region V_PRDC_CONSUMIDO_BB_KIT151

            modelBuilder.Entity<V_PRDC_CONSUMIDO_BB_KIT151>()
                .Property(e => e.NU_PRDC_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_CONSUMIDO_BB_KIT151>()
                .Property(e => e.CD_PRDC_DEFINICION)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_CONSUMIDO_BB_KIT151>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_CONSUMIDO_BB_KIT151>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_CONSUMIDO_BB_KIT151>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<V_PRDC_CONSUMIDO_BB_KIT151>()
                .Property(e => e.QT_CONSUMIDO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_CONSUMIDO_BB_KIT151>()
                .Property(e => e.DS_PRODUTO)
                .IsUnicode(false);

            #endregion

            #region V_PRDC_DET_ENTRADA_KIT151

            modelBuilder.Entity<V_PRDC_DET_ENTRADA_KIT151>()
                .Property(e => e.CD_COMPONENTE)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_DET_ENTRADA_KIT151>()
                .Property(e => e.CD_PRDC_DEFINICION)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_DET_ENTRADA_KIT151>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_DET_ENTRADA_KIT151>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<V_PRDC_DET_ENTRADA_KIT151>()
                .Property(e => e.QT_COMPLETA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_DET_ENTRADA_KIT151>()
                .Property(e => e.QT_FORMULA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_DET_ENTRADA_KIT151>()
                .Property(e => e.QT_PEDIDO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_DET_ENTRADA_KIT151>()
                .Property(e => e.QT_LIBERADO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_DET_ENTRADA_KIT151>()
                .Property(e => e.QT_PREPARADO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_DET_ENTRADA_KIT151>()
                .Property(e => e.QT_LINEA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_DET_ENTRADA_KIT151>()
                .Property(e => e.QT_CONSUMIDO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_DET_ENTRADA_KIT151>()
                .Property(e => e.QT_FORMULA_FORM)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_DET_ENTRADA_KIT151>()
                .Property(e => e.QT_PEDIDO_FORM)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_DET_ENTRADA_KIT151>()
                .Property(e => e.QT_LIBERADO_FORM)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_DET_ENTRADA_KIT151>()
                .Property(e => e.QT_PREPARADO_FORM)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_DET_ENTRADA_KIT151>()
                .Property(e => e.QT_LINEA_FORM)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_DET_ENTRADA_KIT151>()
                .Property(e => e.QT_CONSUMIDO_FORM)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_DET_ENTRADA_KIT151>()
                .Property(e => e.DS_PRODUTO)
                .IsUnicode(false);

            #endregion

            #region V_PRDC_DET_SALIDA_KIT151

            modelBuilder.Entity<V_PRDC_DET_SALIDA_KIT151>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_PRDC_DET_SALIDA_KIT151>()
                .Property(e => e.QT_COMPLETA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRDC_DET_SALIDA_KIT151>()
                .Property(e => e.QT_FORMULA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRDC_DET_SALIDA_KIT151>()
                .Property(e => e.QT_LINEA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRDC_DET_SALIDA_KIT151>()
                .Property(e => e.QT_PRODUCIDO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRDC_DET_SALIDA_KIT151>()
                .Property(e => e.QT_FORMULA_FORM)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRDC_DET_SALIDA_KIT151>()
                .Property(e => e.QT_LINEA_FORM)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PRDC_DET_SALIDA_KIT151>()
                .Property(e => e.QT_PRODUCIDO_FORM)
                .HasPrecision(12, 3);

            #endregion

            #region V_PRDC_PRODUCIDO_BB_KIT151

            modelBuilder.Entity<V_PRDC_PRODUCIDO_BB_KIT151>()
                .Property(e => e.NU_PRDC_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_PRODUCIDO_BB_KIT151>()
                .Property(e => e.CD_PRDC_DEFINICION)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_PRODUCIDO_BB_KIT151>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_PRODUCIDO_BB_KIT151>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_PRODUCIDO_BB_KIT151>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<V_PRDC_PRODUCIDO_BB_KIT151>()
                .Property(e => e.QT_PRODUCIDO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_PRODUCIDO_BB_KIT151>()
                .Property(e => e.DS_PRODUTO)
                .IsUnicode(false);

            #endregion

            #region >> V_PRDC_INGRESO_KIT150

            modelBuilder.Entity<V_PRDC_INGRESO_KIT150>()
                 .Property(e => e.NU_PRDC_INGRESO)
                 .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT150>()
                .Property(e => e.CD_PRDC_DEFINICION)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT150>()
                .Property(e => e.ID_GENERAR_PEDIDO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT150>()
                .Property(e => e.DS_ANEXO1)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT150>()
                .Property(e => e.DS_ANEXO2)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT150>()
                .Property(e => e.DS_ANEXO3)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT150>()
                .Property(e => e.DS_ANEXO4)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT150>()
                .Property(e => e.NU_PRDC_ORIGINAL)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT150>()
                .Property(e => e.ND_TIPO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT150>()
                .Property(e => e.NM_PRDC_DEFINICION)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT150>()
                .Property(e => e.NU_DOCUMENTO_EGR)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT150>()
                .Property(e => e.TP_DOCUMENTO_EGR)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT150>()
                .Property(e => e.NU_DOCUMENTO_ING)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT150>()
                .Property(e => e.TP_DOCUMENTO_ING)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT150>()
                .Property(e => e.DS_SITUACAO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT150>()
                .Property(e => e.NM_EMPRESA)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT150>()
                .Property(e => e.NM_FUNCIONARIO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT150>()
                .Property(e => e.QT_PEDIDO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT150>()
                .Property(e => e.QT_LIBERADO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT150>()
                .Property(e => e.QT_PREPARADO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT150>()
                .Property(e => e.QT_LINEA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT150>()
                .Property(e => e.QT_ELABORADO)
                .HasPrecision(12, 3);

            #endregion

            #region T_PRDC_MOVIMIENTO_BB

            modelBuilder.Entity<T_PRDC_MOVIMIENTO_BB>()
                .Property(e => e.NU_MOVIMIENTO_BB)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_MOVIMIENTO_BB>()
                .Property(e => e.CD_ENDERECO_ORIGEN)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_MOVIMIENTO_BB>()
                .Property(e => e.CD_ENDERECO_DESTINO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_MOVIMIENTO_BB>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_MOVIMIENTO_BB>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_MOVIMIENTO_BB>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<T_PRDC_MOVIMIENTO_BB>()
                .Property(e => e.QT_MOVIMIENTO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_PRDC_MOVIMIENTO_BB>()
                .Property(e => e.ND_ACCION_MOVIMIENTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_MOVIMIENTO_BB>()
                .Property(e => e.NU_PRDC_INGRESO)
                .IsUnicode(false);

            #endregion

            #region I_E_PRDC_SALIDA_PRD

            modelBuilder.Entity<I_E_PRDC_SALIDA_PRD>()
                .HasKey(e => new { e.NU_INTERFAZ_EJECUCION, e.NU_REGISTRO });

            modelBuilder.Entity<I_E_PRDC_SALIDA_PRD>()
               .Property(e => e.ID_PROCESADO)
               .IsUnicode(false);

            modelBuilder.Entity<I_E_PRDC_SALIDA_PRD>()
                .Property(e => e.NU_REGISTRO)
                .IsUnicode(false);

            modelBuilder.Entity<I_E_PRDC_SALIDA_PRD>()
                .Property(e => e.NU_PRDC_INGRESO)
                .IsUnicode(false);

            #endregion

            #region I_E_PRDC_SALIDA_PRD_INSUMO

            modelBuilder.Entity<I_E_PRDC_SALIDA_PRD_INSUMO>()
                .HasKey(e => new { e.NU_INTERFAZ_EJECUCION, e.NU_REGISTRO });

            modelBuilder.Entity<I_E_PRDC_SALIDA_PRD_INSUMO>()
                .Property(e => e.ID_PROCESADO)
                .IsUnicode(false);

            modelBuilder.Entity<I_E_PRDC_SALIDA_PRD_INSUMO>()
                .Property(e => e.NU_REGISTRO)
                .IsUnicode(false);

            modelBuilder.Entity<I_E_PRDC_SALIDA_PRD_INSUMO>()
                .Property(e => e.NU_REGISTRO_PADRE)
                .IsUnicode(false);

            modelBuilder.Entity<I_E_PRDC_SALIDA_PRD_INSUMO>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<I_E_PRDC_SALIDA_PRD_INSUMO>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<I_E_PRDC_SALIDA_PRD_INSUMO>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<I_E_PRDC_SALIDA_PRD_INSUMO>()
                .Property(e => e.ND_ACCION_MOVIMIENTO)
                .IsUnicode(false);

            modelBuilder.Entity<I_E_PRDC_SALIDA_PRD_INSUMO>()
                .Property(e => e.QT_SALIDA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<I_E_PRDC_SALIDA_PRD_INSUMO>()
               .Property(e => e.FL_SEMIACABADO)
               .IsUnicode(false);

            modelBuilder.Entity<I_E_PRDC_SALIDA_PRD_INSUMO>()
             .Property(e => e.FL_CONSUMIBLE)
             .IsUnicode(false);

            #endregion

            #region I_E_PRDC_SALIDA_PRD_PRODUCIDO

            modelBuilder.Entity<I_E_PRDC_SALIDA_PRD_PRODUCIDO>()
                .HasKey(e => new { e.NU_INTERFAZ_EJECUCION, e.NU_REGISTRO });

            modelBuilder.Entity<I_E_PRDC_SALIDA_PRD_PRODUCIDO>()
               .Property(e => e.ID_PROCESADO)
               .IsUnicode(false);

            modelBuilder.Entity<I_E_PRDC_SALIDA_PRD_PRODUCIDO>()
                .Property(e => e.NU_REGISTRO)
                .IsUnicode(false);

            modelBuilder.Entity<I_E_PRDC_SALIDA_PRD_PRODUCIDO>()
                .Property(e => e.NU_REGISTRO_PADRE)
                .IsUnicode(false);

            modelBuilder.Entity<I_E_PRDC_SALIDA_PRD_PRODUCIDO>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<I_E_PRDC_SALIDA_PRD_PRODUCIDO>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<I_E_PRDC_SALIDA_PRD_PRODUCIDO>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<I_E_PRDC_SALIDA_PRD_PRODUCIDO>()
                .Property(e => e.VL_MERCADERIA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<I_E_PRDC_SALIDA_PRD_PRODUCIDO>()
                .Property(e => e.VL_TRIBUTO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<I_E_PRDC_SALIDA_PRD_PRODUCIDO>()
                .Property(e => e.QT_PRODUCIDO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<I_E_PRDC_SALIDA_PRD_PRODUCIDO>()
                .Property(e => e.ND_ACCION_MOVIMIENTO)
                .IsUnicode(false);

            modelBuilder.Entity<I_E_PRDC_SALIDA_PRD_PRODUCIDO>()
              .Property(e => e.FL_SEMIACABADO)
              .IsUnicode(false);

            #endregion

            #region T_PRDC_CUENTA_CORRIENTE_INSUMO

            modelBuilder.Entity<T_PRDC_CUENTA_CORRIENTE_INSUMO>()
                .HasKey(e => new { e.NU_DOCUMENTO_EGRESO, e.TP_DOCUMENTO_EGRESO, e.NU_DOCUMENTO_EGRESO_PRDC, e.TP_DOCUMENTO_EGRESO_PRDC, e.TP_DOCUMENTO_INGRESO, e.NU_DOCUMENTO_INGRESO, e.TP_DOCUMENTO_INGRESO_ORIGINAL, e.NU_DOCUMENTO_INGRESO_ORIGINAL, e.CD_EMPRESA, e.CD_PRODUTO, e.CD_FAIXA, e.NU_IDENTIFICADOR, e.CD_PRODUTO_PRODUCIDO, e.NU_NIVEL });

            modelBuilder.Entity<T_PRDC_CUENTA_CORRIENTE_INSUMO>()
                .Property(e => e.NU_DOCUMENTO_EGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_CUENTA_CORRIENTE_INSUMO>()
                .Property(e => e.TP_DOCUMENTO_EGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_CUENTA_CORRIENTE_INSUMO>()
                .Property(e => e.NU_DOCUMENTO_EGRESO_PRDC)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_CUENTA_CORRIENTE_INSUMO>()
                .Property(e => e.TP_DOCUMENTO_EGRESO_PRDC)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_CUENTA_CORRIENTE_INSUMO>()
                .Property(e => e.TP_DOCUMENTO_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_CUENTA_CORRIENTE_INSUMO>()
                .Property(e => e.NU_DOCUMENTO_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_CUENTA_CORRIENTE_INSUMO>()
                .Property(e => e.TP_DOCUMENTO_INGRESO_ORIGINAL)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_CUENTA_CORRIENTE_INSUMO>()
                .Property(e => e.NU_DOCUMENTO_INGRESO_ORIGINAL)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_CUENTA_CORRIENTE_INSUMO>()
                .Property(e => e.QT_DECLARADA_ORIGINAL)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_PRDC_CUENTA_CORRIENTE_INSUMO>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_CUENTA_CORRIENTE_INSUMO>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<T_PRDC_CUENTA_CORRIENTE_INSUMO>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_CUENTA_CORRIENTE_INSUMO>()
                .Property(e => e.QT_MOVIMIENTO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_PRDC_CUENTA_CORRIENTE_INSUMO>()
                .Property(e => e.CD_PRODUTO_PRODUCIDO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_CUENTA_CORRIENTE_INSUMO>()
                .Property(e => e.NU_NIVEL)
                .HasPrecision(38, 0);

            modelBuilder.Entity<T_PRDC_CUENTA_CORRIENTE_INSUMO>()
                .Property(e => e.TP_DOCUMENTO_CAMBIO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_CUENTA_CORRIENTE_INSUMO>()
                .Property(e => e.NU_DOCUMENTO_CAMBIO)
                .IsUnicode(false);

            #endregion

            #region V_PRDC_PROD_SALIDA_PREP

            modelBuilder.Entity<V_PRDC_PROD_SALIDA_PREP>()
                .HasKey(d => new { d.CD_PRDC_DEFINICION, d.NU_PREPARACION, d.NU_CONTENEDOR, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA });

            #endregion

            #region V_PRDC_PROD_SOBRANTE_PREP

            modelBuilder.Entity<V_PRDC_PROD_SOBRANTE_PREP>()
                .HasKey(d => new { d.CD_PRDC_DEFINICION, d.NU_PREPARACION, d.NU_CONTENEDOR, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA });

            #endregion

            #region V_PRDC_PROD_ENTRADA_PREP

            modelBuilder.Entity<V_PRDC_PROD_ENTRADA_PREP>()
                .HasKey(d => new { d.CD_PRDC_DEFINICION, d.NU_PREPARACION, d.NU_CONTENEDOR, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA });

            #endregion

            #region >> T_HIST_PRDC_INGRESO_PASADA

            modelBuilder.Entity<T_HIST_PRDC_INGRESO_PASADA>()
                .Property(e => e.NU_PRDC_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<T_HIST_PRDC_INGRESO_PASADA>()
                .Property(e => e.VL_ACCION_INSTANCIA)
                .IsUnicode(false);

            modelBuilder.Entity<T_HIST_PRDC_INGRESO_PASADA>()
                .Property(e => e.CD_PRDC_LINEA)
                .IsUnicode(false);

            #endregion << T_HIST_PRDC_INGRESO_PASADA

            #region >> T_PRDC_ACCION

            modelBuilder.Entity<T_PRDC_ACCION>()
               .Property(e => e.CD_ACCION)
               .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_ACCION>()
                .Property(e => e.DS_ACCION)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_ACCION>()
                .Property(e => e.TP_ACCION)
                .IsUnicode(false);

            #endregion << T_PRDC_ACCION

            #region T_PRDC_ACCION_INSTANCIA

            modelBuilder.Entity<T_PRDC_ACCION_INSTANCIA>()
                .Property(e => e.DS_ACCION_INSTANCIA)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_ACCION_INSTANCIA>()
                .Property(e => e.CD_ACCION)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_ACCION_INSTANCIA>()
                .Property(e => e.VL_PARAMETRO1)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_ACCION_INSTANCIA>()
                .Property(e => e.VL_PARAMETRO2)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_ACCION_INSTANCIA>()
                .Property(e => e.VL_PARAMETRO3)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_ACCION_INSTANCIA>()
                .HasMany(e => e.T_PRDC_CONFIGURAR_PASADA)
                .WithOne(e => e.T_PRDC_ACCION_INSTANCIA)
                .HasForeignKey(d => d.CD_ACCION_INSTANCIA);

            modelBuilder.Entity<T_PRDC_ACCION_INSTANCIA>()
                .HasMany(e => e.T_PRDC_INGRESO_PASADA)
                .WithOne(e => e.T_PRDC_ACCION_INSTANCIA)
                .HasForeignKey(d => d.CD_ACCION_INSTANCIA);

            #endregion

            #region T_PRDC_EGRESO_IDENTIFICADOR

            modelBuilder.Entity<T_PRDC_EGRESO_IDENTIFICADOR>()
              .Property(e => e.CD_ENDERECO)
              .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_EGRESO_IDENTIFICADOR>()
              .Property(e => e.CD_PRODUTO)
              .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_EGRESO_IDENTIFICADOR>()
              .Property(e => e.NU_IDENTIFICADOR)
              .IsUnicode(false);

            #endregion

            #region >> V_ACCION_INSTANCIA_KIT120 - 

            modelBuilder.Entity<V_ACCION_INSTANCIA_KIT120>()
                .Property(e => e.DS_ACCION_INSTANCIA)
                .IsUnicode(false);

            modelBuilder.Entity<V_ACCION_INSTANCIA_KIT120>()
                .Property(e => e.VL_PARAMETRO1)
                .IsUnicode(false);

            modelBuilder.Entity<V_ACCION_INSTANCIA_KIT120>()
                .Property(e => e.VL_PARAMETRO2)
                .IsUnicode(false);

            modelBuilder.Entity<V_ACCION_INSTANCIA_KIT120>()
                .Property(e => e.VL_PARAMETRO3)
                .IsUnicode(false);

            modelBuilder.Entity<V_ACCION_INSTANCIA_KIT120>()
                .Property(e => e.CD_ACCION)
                .IsUnicode(false);

            modelBuilder.Entity<V_ACCION_INSTANCIA_KIT120>()
                .Property(e => e.DS_ACCION)
                .IsUnicode(false);

            modelBuilder.Entity<V_ACCION_INSTANCIA_KIT120>()
                .Property(e => e.TP_ACCION)
                .IsUnicode(false);

            #endregion << V_ACCION_INSTANCIA_KIT120

            #region V_PRDC_CONFIG_PASADA_KIT102

            modelBuilder.Entity<V_PRDC_CONFIG_PASADA_KIT102>()
                .Property(e => e.CD_PRDC_DEFINICION)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_CONFIG_PASADA_KIT102>()
                .Property(e => e.DS_ACCION_INSTANCIA)
                .IsUnicode(false);

            #endregion

            #region V_PRDC_DEFINICION_KIT100

            modelBuilder.Entity<V_PRDC_DEFINICION_KIT100>()
                .Property(e => e.NM_EMPRESA)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_DEFINICION_KIT100>()
                .Property(e => e.NM_PRDC_DEFINICION)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_DEFINICION_KIT100>()
                .Property(e => e.CD_PRDC_DEFINICION)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_DEFINICION_KIT100>()
                .Property(e => e.DS_PRDC_DEFINICION)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_DEFINICION_KIT100>()
                .Property(e => e.DS_SITUACAO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_DEFINICION_KIT100>()
                .Property(e => e.ACTIVO)
                .IsUnicode(false);

            #endregion

            #region V_PRDC_DET_ENTRADA_KIT101

            modelBuilder.Entity<V_PRDC_DET_ENTRADA_KIT101>()
                .Property(e => e.CD_PRDC_DEFINICION)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_DET_ENTRADA_KIT101>()
                .Property(e => e.CD_COMPONENTE)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_DET_ENTRADA_KIT101>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_DET_ENTRADA_KIT101>()
                .Property(e => e.CD_PRDC_DEFINICION)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_DET_ENTRADA_KIT101>()
                .Property(e => e.NM_EMPRESA)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_DET_ENTRADA_KIT101>()
                .Property(e => e.DS_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_DET_ENTRADA_KIT101>()
                .Property(e => e.NM_EMPRESA_PEDIDO)
                .IsUnicode(false);

            #endregion

            #region V_PRDC_DET_SALIDA_KIT101

            modelBuilder.Entity<V_PRDC_DET_SALIDA_KIT101>()
                .Property(e => e.CD_PRDC_DEFINICION)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_DET_SALIDA_KIT101>()
                .Property(e => e.ID_PRODUTO_FINAL)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_DET_SALIDA_KIT101>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_DET_SALIDA_KIT101>()
                .Property(e => e.NM_EMPRESA)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_DET_SALIDA_KIT101>()
                .Property(e => e.DS_PRODUTO)
                .IsUnicode(false);

            #endregion

            #region V_PRDC_DET_SALIDA_KIT151
            modelBuilder.Entity<V_PRDC_DET_SALIDA_KIT151>()
                .Property(e => e.CD_PRDC_DEFINICION)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_DET_SALIDA_KIT151>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_DET_SALIDA_KIT151>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<V_PRDC_DET_SALIDA_KIT151>()
                .Property(e => e.QT_COMPLETA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_DET_SALIDA_KIT151>()
                .Property(e => e.QT_FORMULA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_DET_SALIDA_KIT151>()
                .Property(e => e.QT_LINEA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_DET_SALIDA_KIT151>()
                .Property(e => e.QT_PRODUCIDO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_DET_SALIDA_KIT151>()
                .Property(e => e.QT_FORMULA_FORM)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_DET_SALIDA_KIT151>()
                .Property(e => e.QT_LINEA_FORM)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_DET_SALIDA_KIT151>()
                .Property(e => e.QT_PRODUCIDO_FORM)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_DET_SALIDA_KIT151>()
                .Property(e => e.DS_PRODUTO)
                .IsUnicode(false);
            #endregion

            #region V_PRDC_EGR_IDENT_KIT191

            modelBuilder.Entity<V_PRDC_EGR_IDENT_KIT191>()
                .Property(e => e.CD_ENDERECO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_EGR_IDENT_KIT191>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_EGR_IDENT_KIT191>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            #endregion

            #region V_PRDC_INGRESO_KIT110

            modelBuilder.Entity<V_PRDC_INGRESO_KIT110>()
              .Property(e => e.NU_PRDC_INGRESO)
              .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT110>()
                .Property(e => e.CD_PRDC_DEFINICION)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT110>()
                .Property(e => e.ID_GENERAR_PEDIDO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT110>()
                .Property(e => e.ND_TIPO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT110>()
                .Property(e => e.NM_FUNCIONARIO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT110>()
                .Property(e => e.TP_FLUJO)
                .IsUnicode(false);


            #endregion

            #region >> V_PRDC_INGRESO_KIT170

            modelBuilder.Entity<V_PRDC_INGRESO_KIT170>()
              .Property(e => e.NU_PRDC_INGRESO)
              .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT170>()
                .Property(e => e.ID_GENERAR_PEDIDO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT170>()
                .Property(e => e.LOGINNAME)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT170>()
                .Property(e => e.DS_SITUACAO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT170>()
                .Property(e => e.CD_PRDC_DEFINICION)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT170>()
                .Property(e => e.NM_PRDC_DEFINICION)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT170>()
                .Property(e => e.ND_TIPO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT170>()
                .Property(e => e.CD_DOMINIO_VALOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT170>()
                .Property(e => e.DS_DOMINIO_VALOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT170>()
                .Property(e => e.CD_PRDC_LINEA)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT170>()
                .Property(e => e.DS_PRDC_LINEA)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT170>()
                .Property(e => e.CD_ENDERECO_ENTRADA)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT170>()
                .Property(e => e.CD_ENDERECO_SALIDA)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_KIT170>()
                .Property(e => e.CD_ENDERECO_BLACKBOX)
                .IsUnicode(false);

            #endregion << V_PRDC_INGRESO_KIT170

            #region
            modelBuilder.Entity<V_PRDC_INGRESO_PASADA_KIT180>()
             .Property(e => e.NU_PRDC_INGRESO)
             .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_PASADA_KIT180>()
                .Property(e => e.VL_ACCION_INSTANCIA)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_INGRESO_PASADA_KIT180>()
                .Property(e => e.CD_PRDC_LINEA)
                .IsUnicode(false);
            #endregion

            #region >> V_PRDC_KIT170_LI_CD_PRDC_LINEA

            modelBuilder.Entity<V_PRDC_KIT170_LI_CD_PRDC_LINEA>()
                .Property(e => e.CD_PRDC_LINEA)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_KIT170_LI_CD_PRDC_LINEA>()
                .Property(e => e.DS_PRDC_LINEA)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_KIT170_LI_CD_PRDC_LINEA>()
                .Property(e => e.ND_TIPO_LINEA)
                .IsUnicode(false);

            #endregion << V_PRDC_KIT170_LI_CD_PRDC_LINEA

            #region >> V_PRDC_LINEA_KIT190

            modelBuilder.Entity<V_PRDC_LINEA_KIT190>()
               .Property(e => e.CD_PRDC_LINEA)
               .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_LINEA_KIT190>()
                .Property(e => e.DS_PRDC_LINEA)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_LINEA_KIT190>()
                .Property(e => e.CD_ENDERECO_ENTRADA)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_LINEA_KIT190>()
                .Property(e => e.CD_ENDERECO_SALIDA)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_LINEA_KIT190>()
                .Property(e => e.NU_PRDC_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_LINEA_KIT190>()
                .Property(e => e.ND_TIPO_LINEA)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_LINEA_KIT190>()
                .Property(e => e.CD_TIPO_LINEA)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_LINEA_KIT190>()
                .Property(e => e.CD_ENDERECO_BLACKBOX)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_LINEA_KIT190>()
               .Property(e => e.FL_CONF_MAN)
               .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_LINEA_KIT190>()
               .Property(e => e.FL_STOCK_CONSUMIBLE)
               .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_LINEA_KIT190>()
                .Property(e => e.NU_PREDIO)
                .IsUnicode(false);

            #endregion << V_PRDC_LINEA_KIT190

            #region V_REG104_ZONA
            modelBuilder.Entity<V_REG104_ZONA>()
                .Property(e => e.CD_ZONA)
                .IsUnicode(false);

            modelBuilder.Entity<V_REG104_ZONA>()
                .Property(e => e.NM_ZONA)
                .IsUnicode(false);

            modelBuilder.Entity<V_REG104_ZONA>()
                .Property(e => e.DS_ZONA)
                .IsUnicode(false);

            modelBuilder.Entity<V_REG104_ZONA>()
                .Property(e => e.CD_DEPARTAMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_REG104_ZONA>()
                .Property(e => e.CD_LOCALIDAD)
                .IsUnicode(false);

            modelBuilder.Entity<V_REG104_ZONA>()
                .Property(e => e.DS_LOCALIDAD)
                .IsUnicode(false);
            #endregion

            #region >>  V_PEDIDO_SAIDA_KIT130 -

            #endregion << V_PEDIDO_SAIDA_KIT130

            modelBuilder.Entity<V_PEDIDO_SAIDA_KIT130>()
                .HasKey(e => new { e.NU_PEDIDO, e.CD_CLIENTE, e.CD_EMPRESA });

            #endregion << - PRDC PRODUCCION - 

            #region INVENTARIO

            #region T_INVENTARIO
            modelBuilder.Entity<T_INVENTARIO>()
               .Property(e => e.NU_INVENTARIO)
               .HasPrecision(38, 0);
            modelBuilder.Entity<T_INVENTARIO>()
                .Property(e => e.NU_CONTEO)
                .HasPrecision(38, 0);
            #endregion

            #region T_INVENTARIO_ENDERECO
            modelBuilder.Entity<T_INVENTARIO_ENDERECO>()
                .Property(e => e.NU_INVENTARIO_ENDERECO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<T_INVENTARIO_ENDERECO>()
                .Property(e => e.NU_INVENTARIO)
                .HasPrecision(38, 0);
            //modelBuilder.Entity<T_INVENTARIO_ENDERECO>()
            //    .HasMany(e => e.T_INVENTARIO_ENDERECO_DET)
            //    .WithOne(e => e.T_INVENTARIO_ENDERECO)
            //    .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region T_INVENTARIO_ENDERECO_DET
            modelBuilder.Entity<T_INVENTARIO_ENDERECO_DET>()
                .Property(e => e.NU_INVENTARIO_ENDERECO_DET)
                .HasPrecision(38, 0);
            modelBuilder.Entity<T_INVENTARIO_ENDERECO_DET>()
                .Property(e => e.NU_INVENTARIO_ENDERECO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<T_INVENTARIO_ENDERECO_DET>()
                .Property(e => e.NU_CONTEO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<T_INVENTARIO_ENDERECO_DET>()
                .Property(e => e.QT_INVENTARIO)
                .HasPrecision(15, 3);
            modelBuilder.Entity<T_INVENTARIO_ENDERECO_DET>()
                .Property(e => e.QT_DIFERENCIA)
                .HasPrecision(15, 3);
            modelBuilder.Entity<T_INVENTARIO_ENDERECO_DET>()
                .Property(e => e.QT_TIEMPO_INSUMIDO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<T_INVENTARIO_ENDERECO_DET>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            #endregion

            #region >> V_AJUSTE_STOCK_WINV030
            modelBuilder.Entity<V_AJUSTE_STOCK_WINV030>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_AJUSTE_STOCK_WINV030>()
                .Property(e => e.QT_MOVIMIENTO)
                .HasPrecision(12, 3);
            #endregion << V_AJUSTE_STOCK_WINV030

            #region >> V_INV100_ANALISIS_INVENTARIO
            modelBuilder.Entity<V_INV100_ANALISIS_INVENTARIO>()
               .Property(e => e.NU_INVENTARIO)
               .HasPrecision(38, 0);
            modelBuilder.Entity<V_INV100_ANALISIS_INVENTARIO>()
                .Property(e => e.NU_INVENTARIO_ENDERECO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_INV100_ANALISIS_INVENTARIO>()
                .Property(e => e.NU_INVENTARIO_ENDERECO_DET)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_INV100_ANALISIS_INVENTARIO>()
                .Property(e => e.QT_INVENTARIO)
                .HasPrecision(15, 3);
            modelBuilder.Entity<V_INV100_ANALISIS_INVENTARIO>()
                .Property(e => e.QT_DIFERENCIA)
                .HasPrecision(15, 3);
            #endregion << 

            #region >> V_ASIGN_MOTIVO_AJUSTE_WINV060
            modelBuilder.Entity<V_ASIGN_MOTIVO_AJUSTE_WINV060>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_ASIGN_MOTIVO_AJUSTE_WINV060>()
                .Property(e => e.QT_MOVIMIENTO)
                .HasPrecision(12, 3);
            #endregion << 

            #region V_INVENTARIO_ENDE_DET
            modelBuilder.Entity<V_INVENTARIO_ENDE_DET>()
               .Property(e => e.NU_INVENTARIO)
               .HasPrecision(38, 0);
            modelBuilder.Entity<V_INVENTARIO_ENDE_DET>()
                .Property(e => e.NU_INVENTARIO_ENDERECO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_INVENTARIO_ENDE_DET>()
                .Property(e => e.NU_INVENTARIO_ENDERECO_DET)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_INVENTARIO_ENDE_DET>()
                .Property(e => e.NU_CONTEO_INV_DET)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_INVENTARIO_ENDE_DET>()
                .Property(e => e.QT_INVENTARIO)
                .HasPrecision(15, 3);
            modelBuilder.Entity<V_INVENTARIO_ENDE_DET>()
                .Property(e => e.QT_DIFERENCIA)
                .HasPrecision(15, 3);
            modelBuilder.Entity<V_INVENTARIO_ENDE_DET>()
                .Property(e => e.QT_TIEMPO_INSUMIDO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_INVENTARIO_ENDE_DET>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            #endregion

            #region V_INV412_DET_CONTEO
            modelBuilder.Entity<V_INV412_DET_CONTEO>()
                .Property(e => e.NU_INVENTARIO_ENDERECO_DET)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_INV412_DET_CONTEO>()
                .Property(e => e.NU_INVENTARIO_ENDERECO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_INV412_DET_CONTEO>()
                .Property(e => e.NU_INVENTARIO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_INV412_DET_CONTEO>()
                .Property(e => e.NU_CONTEO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_INV412_DET_CONTEO>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_INV412_DET_CONTEO>()
                .Property(e => e.QT_STOCK)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_INV412_DET_CONTEO>()
                .Property(e => e.QT_INVENTARIO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_INV412_DET_CONTEO>()
                .Property(e => e.QT_DIFERENCIA)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_INV412_DET_CONTEO>()
                .Property(e => e.QT_TIEMPO_INSUMIDO)
                .HasPrecision(38, 0);
            #endregion

            #region V_INV411_UBIC_SEL
            modelBuilder.Entity<V_INV411_UBIC_SEL>()
                .Property(e => e.NU_INVENTARIO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_INV411_UBIC_SEL>()
                .Property(e => e.NU_INVENTARIO_ENDERECO)
                .HasPrecision(38, 0);
            #endregion

            #region V_INV413_REG_SEL
            modelBuilder.Entity<V_INV413_REG_SEL>()
                .Property(e => e.NU_INVENTARIO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_INV413_REG_SEL>()
                .Property(e => e.NU_INVENTARIO_ENDERECO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_INV413_REG_SEL>()
                .Property(e => e.NU_INVENTARIO_ENDERECO_DET)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_INV413_REG_SEL>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            #endregion

            #region V_INV410_INVENTARIO         
            modelBuilder.Entity<V_INV410_INVENTARIO>()
               .Property(e => e.NU_INVENTARIO)
               .HasPrecision(38, 0);
            #endregion

            #region V_INV_ENDERECO_DET_ERROR
            modelBuilder.Entity<V_INV_ENDERECO_DET_ERROR>()
                .Property(e => e.NU_ERROR)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_INV_ENDERECO_DET_ERROR>()
                .Property(e => e.NU_INVENTARIO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_INV_ENDERECO_DET_ERROR>()
                .Property(e => e.NU_INVENTARIO_ENDERECO_DET)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_INV_ENDERECO_DET_ERROR>()
                .Property(e => e.QT_INVENTARIO)
                .HasPrecision(15, 3);
            modelBuilder.Entity<V_INV_ENDERECO_DET_ERROR>()
                .Property(e => e.QT_DIFERENCIA)
                .HasPrecision(15, 3);
            #endregion

            #region V_INVENTARIO_STOCK
            modelBuilder.Entity<V_INVENTARIO_STOCK>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_INVENTARIO_STOCK>()
                .Property(e => e.QT_ESTOQUE)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_INVENTARIO_STOCK>()
                .Property(e => e.QT_RESERVA_SAIDA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_INVENTARIO_STOCK>()
                .Property(e => e.QT_TRANSITO_ENTRADA)
                .HasPrecision(12, 3);
            #endregion

            #endregion

            #region T_AUTOMATISMO_DATA

            if (!string.IsNullOrEmpty(_blobDataType))
            {
                modelBuilder.Entity<T_AUTOMATISMO_DATA>()
                    .Property(e => e.VL_DATA_REQUEST)
                    .HasColumnType(_blobDataType);

                modelBuilder.Entity<T_AUTOMATISMO_DATA>()
                    .Property(e => e.VL_DATA_RESPONSE)
                    .HasColumnType(_blobDataType);
            }

            #endregion

            #region T_CODIGO_MULTIDATO

            if (!string.IsNullOrEmpty(_blobDataType))
            {
                modelBuilder.Entity<T_CODIGO_MULTIDATO>()
                    .Property(e => e.VL_REGEX)
                    .HasColumnType(_blobDataType);
            }

            #endregion

            #region T_CROSS_DOCK
            modelBuilder.Entity<T_CROSS_DOCK>()
       .Property(e => e.ND_ESTADO)
       .IsUnicode(false);

            modelBuilder.Entity<T_CROSS_DOCK>()
                .Property(e => e.TP_CROSS_DOCKING)
                .IsUnicode(false);
            #endregion

            #region T_DET_CROSS_DOCK
            modelBuilder.Entity<T_DET_CROSS_DOCK>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            #endregion

            #region T_DOCUMENTO

            modelBuilder.Entity<T_DOCUMENTO>()
                 .HasMany(e => e.T_DET_DOCUMENTO)
                 .WithOne(e => e.T_DOCUMENTO)
                 .IsRequired()
                 .HasForeignKey(e => new { e.NU_DOCUMENTO, e.TP_DOCUMENTO });

            modelBuilder.Entity<T_DOCUMENTO>()
               .HasMany(e => e.T_DOCUMENTO_PREPARACION_RESERV)
               .WithOne(e => e.T_DOCUMENTO)
               .IsRequired()
               .HasForeignKey(e => new { e.NU_DOCUMENTO, e.TP_DOCUMENTO });

            modelBuilder.Entity<T_DOCUMENTO>()
               .HasMany(e => e.T_DOCUMENTO_PRODUCCION_INGRESO)
               .WithOne(e => e.T_DOCUMENTO_INGRESO)
               .IsRequired()
               .HasForeignKey(e => new { e.NU_DOCUMENTO_ING, e.TP_DOCUMENTO_ING });

            modelBuilder.Entity<T_DOCUMENTO>()
               .HasMany(e => e.T_DOCUMENTO_TRANSFERENCIA_INGRESO)
               .WithOne(e => e.T_DOCUMENTO_INGRESO)
               .IsRequired()
               .HasForeignKey(e => new { e.NU_DOCUMENTO_ING, e.TP_DOCUMENTO_ING });

            modelBuilder.Entity<T_DOCUMENTO>()
                .HasMany(e => e.T_DOCUMENTO_PRODUCCION_EGRESO)
                .WithOne(e => e.T_DOCUMENTO_EGRESO)
                .IsRequired()
                .HasForeignKey(e => new { e.NU_DOCUMENTO_EGR, e.TP_DOCUMENTO_EGR });

            modelBuilder.Entity<T_DOCUMENTO>()
                .HasMany(e => e.T_DOCUMENTO_TRANSFERENCIA_EGRESO)
                .WithOne(e => e.T_DOCUMENTO_EGRESO)
                .IsRequired()
                .HasForeignKey(e => new { e.NU_DOCUMENTO_EGR, e.TP_DOCUMENTO_EGR });

            #endregion

            #region T_DOCUMENTO_PRODUCCION
            #endregion

            #region T_DET_DOCUMENTO
            modelBuilder.Entity<T_DET_DOCUMENTO>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DET_DOCUMENTO>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<T_DET_DOCUMENTO>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<T_DET_DOCUMENTO>()
                .Property(e => e.NU_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DET_DOCUMENTO>()
                .Property(e => e.VL_MERCADERIA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_DET_DOCUMENTO>()
               .Property(e => e.VL_TRIBUTO)
               .HasPrecision(12, 3);

            modelBuilder.Entity<T_DET_DOCUMENTO>()
                .Property(e => e.QT_INGRESADA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_DET_DOCUMENTO>()
                .Property(e => e.QT_RESERVADA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_DET_DOCUMENTO>()
                .Property(e => e.QT_DESAFECTADA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_DET_DOCUMENTO>()
                .Property(e => e.ID_DISPONIBLE)
                .IsUnicode(false);

            modelBuilder.Entity<T_DET_DOCUMENTO>()
                .Property(e => e.TP_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DET_DOCUMENTO>()
                .Property(e => e.VL_CIF)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_DET_DOCUMENTO>()
                .Property(e => e.DS_PRODUTO_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DET_DOCUMENTO>()
                .Property(e => e.QT_DESCARGADA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_DET_DOCUMENTO>()
             .Property(e => e.VL_DATO_AUDITORIA)
             .IsUnicode(false);

            #endregion

            #region T_DET_DOCUMENTO_EGRESO

            modelBuilder.Entity<T_DET_DOCUMENTO_EGRESO>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DET_DOCUMENTO_EGRESO>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<T_DET_DOCUMENTO_EGRESO>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<T_DET_DOCUMENTO_EGRESO>()
                .Property(e => e.NU_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DET_DOCUMENTO_EGRESO>()
                .Property(e => e.NU_SECUENCIA)
                .HasPrecision(6, 0);

            modelBuilder.Entity<T_DET_DOCUMENTO_EGRESO>()
               .Property(e => e.VL_TRIBUTO)
               .HasPrecision(12, 3);

            modelBuilder.Entity<T_DET_DOCUMENTO_EGRESO>()
                .Property(e => e.CD_EMPRESA)
                .HasPrecision(8, 0);

            modelBuilder.Entity<T_DET_DOCUMENTO_EGRESO>()
                .Property(e => e.TP_DOCUMENTO_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DET_DOCUMENTO_EGRESO>()
                .Property(e => e.QT_DESAFECTADA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_DET_DOCUMENTO_EGRESO>()
                .Property(e => e.NU_DOCUMENTO_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DET_DOCUMENTO_EGRESO>()
                .Property(e => e.TP_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DET_DOCUMENTO_EGRESO>()
                .Property(e => e.VL_CIF)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_DET_DOCUMENTO_EGRESO>()
                .Property(e => e.VL_FOB)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_DET_DOCUMENTO_EGRESO>()
                .Property(e => e.QT_DESCARGADA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_DET_DOCUMENTO_EGRESO>()
             .Property(e => e.VL_DATO_AUDITORIA)
             .IsUnicode(false);

            #endregion

            #region T_DET_DOMINIO

            #endregion

            #region T_DESPACHANTE
            #endregion

            #region T_DOCUMENTO_TIPO_ESTADO

            modelBuilder.Entity<T_DOCUMENTO_TIPO_ESTADO>()
                .HasKey(d => new { d.TP_DOCUMENTO, d.ID_ESTADO });

            modelBuilder.Entity<T_DOCUMENTO_TIPO_ESTADO>()
                .HasOne(d => d.T_DOCUMENTO_ESTADO)
                .WithMany(d => d.T_DOCUMENTO_TIPO_ESTADO)
                .HasForeignKey(d => d.ID_ESTADO)
                .IsRequired();

            modelBuilder.Entity<T_DOCUMENTO_TIPO_ESTADO>()
                .HasOne(d => d.T_DOCUMENTO_TIPO)
                .WithMany(d => d.T_DOCUMENTO_TIPO_ESTADO)
                .HasForeignKey(d => d.TP_DOCUMENTO)
                .IsRequired();

            #endregion

            #region T_DOCUMENTO_ESTADO

            modelBuilder.Entity<T_DOCUMENTO_ESTADO>()
                .HasMany(d => d.T_DOCUMENTO_ESTADO_ORDEN_ORIGEN)
                .WithOne(d => d.T_DOCUMENTO_ESTADO_ORIGEN)
                .HasForeignKey(d => d.ID_ESTADO_ORIGEN);

            modelBuilder.Entity<T_DOCUMENTO_ESTADO>()
                .HasMany(d => d.T_DOCUMENTO_ESTADO_ORDEN_DESTINO)
                .WithOne(d => d.T_DOCUMENTO_ESTADO_DESTINO)
                .HasForeignKey(d => d.ID_ESTADO_DESTINO);

            #endregion

            #region T_DOCUMENTO_ESTADO_ORDEN

            modelBuilder.Entity<T_DOCUMENTO_ESTADO_ORDEN>()
                .HasOne(d => d.T_DOCUMENTO_ESTADO_ORIGEN)
                .WithMany(d => d.T_DOCUMENTO_ESTADO_ORDEN_ORIGEN)
                .HasForeignKey(d => d.ID_ESTADO_ORIGEN)
                .IsRequired();

            modelBuilder.Entity<T_DOCUMENTO_ESTADO_ORDEN>()
                .HasOne(d => d.T_DOCUMENTO_ESTADO_DESTINO)
                .WithMany(d => d.T_DOCUMENTO_ESTADO_ORDEN_DESTINO)
                .HasForeignKey(d => d.ID_ESTADO_DESTINO)
                .IsRequired();

            modelBuilder.Entity<T_DOCUMENTO_ESTADO_ORDEN>()
                .HasOne(d => d.T_DOCUMENTO_TIPO)
                .WithMany(d => d.T_DOCUMENTO_ESTADO_ORDEN)
                .HasForeignKey(d => d.TP_DOCUMENTO)
                .IsRequired();

            #endregion

            #region T_DOCUMENTO_PREPARACION_RESERV
            modelBuilder.Entity<T_DOCUMENTO_PREPARACION_RESERV>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<T_DOCUMENTO_PREPARACION_RESERV>()
                .Property(e => e.QT_PRODUTO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_DOCUMENTO_PREPARACION_RESERV>()
                .Property(e => e.QT_ANULAR)
                .HasPrecision(12, 3);
            #endregion

            #region T_DOMINIO
            //modelBuilder.Entity<T_DOMINIO>()
            //    .HasMany(e => e.T_DET_DOMINIO)
            //    .WithOne(e => e.T_DOMINIO)
            //    .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region T_GRID_DEFAULT_CONFIG
            modelBuilder.Entity<T_GRID_DEFAULT_CONFIG>()
                .Property(e => e.VL_WIDTH)
                .HasPrecision(10, 3);
            #endregion

            #region T_GRID_FILTER_DET
            //modelBuilder.Entity<T_GRID_FILTER>()
            //    .HasMany(e => e.T_GRID_FILTER_DET)
            //    .WithOne(e => e.T_GRID_FILTER)
            //    .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region T_GRID_USER_CONFIG
            modelBuilder.Entity<T_GRID_USER_CONFIG>()
                .Property(e => e.VL_WIDTH)
                .HasPrecision(10, 3);
            #endregion

            #region T_GRUPO_CONSULTA_FUNCIONARIO
            //modelBuilder.Entity<T_GRUPO_CONSULTA_FUNCIONARIO>()
            //    .HasOne(d => d.T_GRUPO_CONSULTA)
            //    .WithMany(d => d.T_GRUPO_CONSULTA_FUNCIONARIO)
            //    .HasForeignKey(d => d.CD_GRUPO_CONSULTA);
            //modelBuilder.Entity<T_GRUPO_CONSULTA_FUNCIONARIO>()
            //    .HasOne(d => d.USERS)
            //    .WithMany(d => d.T_GRUPO_CONSULTA_FUNCIONARIO)
            //    .HasForeignKey(d => d.USERID);
            #endregion

            #region >> T_LPARAMETRO
            //modelBuilder.Entity<T_LPARAMETRO>()
            //    .HasMany(e => e.T_LPARAMETRO_NIVEL)
            //    .WithOne(e => e.T_LPARAMETRO)
            //    .OnDelete(DeleteBehavior.Restrict);
            #endregion << T_LPARAMETRO

            #region T_REPORTE

            if (!string.IsNullOrEmpty(_blobDataType))
            {
                modelBuilder.Entity<T_REPORTE>()
                    .Property(e => e.VL_DATA)
                    .HasColumnType(_blobDataType);
            }

            #endregion

            #region T_STOCK
            modelBuilder.Entity<T_STOCK>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<T_STOCK>()
                .Property(e => e.QT_ESTOQUE)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_STOCK>()
                .Property(e => e.QT_RESERVA_SAIDA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_STOCK>()
                .Property(e => e.QT_TRANSITO_ENTRADA)
                .HasPrecision(12, 3);
            #endregion

            #region T_TRACE_STOCK
            modelBuilder.Entity<T_TRACE_STOCK>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<T_TRACE_STOCK>()
                .Property(e => e.QT_MOVIMIENTO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_TRACE_STOCK>()
                .Property(e => e.QT_ESTOQUE)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_TRACE_STOCK>()
                .Property(e => e.QT_RESERVA_SAIDA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_TRACE_STOCK>()
                .Property(e => e.QT_TRANSITO_ENTRADA)
                .HasPrecision(12, 3);
            #endregion

            #region USERDATA
            modelBuilder.Entity<USERDATA>()
                .Property(e => e.LOCKCAUSE)
                .HasPrecision(38, 0);
            modelBuilder.Entity<USERDATA>()
                .Property(e => e.FAILEDPASSWORDATTEMPTCOUNT)
                .HasPrecision(38, 0);
            modelBuilder.Entity<USERDATA>()
                .Property(e => e.PASSWORDFORMAT)
                .HasPrecision(38, 0);
            #endregion

            #region USERS
            //modelBuilder.Entity<USERS>()
            //    .HasMany(e => e.T_EMPRESA_FUNCIONARIO)
            //    .WithOne(e => e.USERS)
            //    .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<USERS>()
            //    .HasOne(e => e.USER_TOKEN)
            //    .WithOne(e => e.USERS)
            //    .HasForeignKey<USER_TOKEN>(e => e.USERID); ;
            //modelBuilder.Entity<USERS>()
            //    .HasOne(e => e.USERDATA)
            //    .WithOne(e => e.USERS)
            //    .HasForeignKey<USERDATA>(e => e.USERID);
            //modelBuilder.Entity<USERS>()
            //    .HasOne(e => e.USERTYPES)
            //    .WithMany(e => e.USERS)
            //    .HasForeignKey(e => e.USERTYPEID);
            //modelBuilder.Entity<USERS>()
            //    .HasMany(e => e.USERPERMISSIONS)
            //    .WithOne(e => e.USERS)
            //    .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<USERS>()
            //     .HasOne(e => e.T_USUARIO_CONFIGURACION)
            //     .WithOne(e => e.USERS)
            //     .HasForeignKey<T_USUARIO_CONFIGURACION>(e => e.USERID);
            #endregion

            #region USERPERMISSIONS
            //modelBuilder.Entity<USERPERMISSIONS>()
            //     .HasOne(e => e.USERS)
            //     .WithMany(e => e.USERPERMISSIONS)
            //     .HasForeignKey(e => e.USERID);
            //modelBuilder.Entity<USERPERMISSIONS>()
            //     .HasOne(e => e.RESOURCES)
            //     .WithMany(e => e.USERPERMISSIONS)
            //     .HasForeignKey(e => e.RESOURCEID);
            //modelBuilder.Entity<USERPERMISSIONS>()
            //     .HasOne(e => e.PROFILES)
            //     .WithMany(e => e.USERPERMISSIONS)
            //     .HasForeignKey(e => e.PROFILEID);
            //#endregion
            //#region PROFILERESOURCES
            //modelBuilder.Entity<PROFILERESOURCES>()
            //     .HasOne(e => e.PROFILES)
            //     .WithMany(e => e.PROFILERESOURCES)
            //     .HasForeignKey(e => e.PROFILEID);
            //modelBuilder.Entity<PROFILERESOURCES>()
            //     .HasOne(e => e.RESOURCES)
            //     .WithMany(e => e.PROFILERESOURCES)
            //     .HasForeignKey(e => e.RESOURCEID);
            #endregion

            #region V_DOCUMENTO_DOC080
            modelBuilder.Entity<V_DOCUMENTO_DOC080>()
                .Property(e => e.VL_ARBITRAJE)
                .HasPrecision(9, 4);
            modelBuilder.Entity<V_DOCUMENTO_DOC080>()
                .Property(e => e.VL_SEGURO)
                .HasPrecision(9, 4);
            modelBuilder.Entity<V_DOCUMENTO_DOC080>()
                .Property(e => e.QT_VOLUMEN)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DOCUMENTO_DOC080>()
                .Property(e => e.QT_PESO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DOCUMENTO_DOC080>()
                .Property(e => e.VL_FLETE)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DOCUMENTO_DOC080>()
                .Property(e => e.VL_OTROS_GASTOS)
                .HasPrecision(12, 3);
            #endregion

            #region V_DET_DOCUMENTO_DOC081
            modelBuilder.Entity<V_DET_DOCUMENTO_DOC081>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_DET_DOCUMENTO_DOC081>()
                .Property(e => e.QT_INGRESADA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DET_DOCUMENTO_DOC081>()
                .Property(e => e.VL_MERCADERIA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DET_DOCUMENTO_DOC081>()
                .Property(e => e.VL_CIF)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DET_DOCUMENTO_DOC081>()
                .Property(e => e.VL_TRIBUTO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DET_DOCUMENTO_DOC081>()
                .Property(e => e.QT_RESERVADA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DET_DOCUMENTO_DOC081>()
                .Property(e => e.QT_DESAFECTADA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DET_DOCUMENTO_DOC081>()
                .Property(e => e.QT_DESCARGADA)
                .HasPrecision(12, 3);
            #endregion

            #region V_ENDERECO_ESTOQUE
            modelBuilder.Entity<V_ENDERECO_ESTOQUE>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_ENDERECO_ESTOQUE>()
                .Property(e => e.QT_ESTOQUE)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_ENDERECO_ESTOQUE>()
                .Property(e => e.QT_RESERVA_SAIDA)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_ENDERECO_ESTOQUE>()
                .Property(e => e.QT_TRANSITO_ENTRADA)
                .HasPrecision(38, 0);
            #endregion

            #region V_ENDERECO_ESTOQUE_WSTO150
            modelBuilder.Entity<V_ENDERECO_ESTOQUE_WSTO150>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_ENDERECO_ESTOQUE_WSTO150>()
                .Property(e => e.QT_ESTOQUE)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_ENDERECO_ESTOQUE_WSTO150>()
                .Property(e => e.QT_RESERVA_SAIDA)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_ENDERECO_ESTOQUE_WSTO150>()
                .Property(e => e.QT_TRANSITO_ENTRADA)
                .HasPrecision(38, 0);
            #endregion

            #region V_STOCK
            modelBuilder.Entity<V_STOCK>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_STOCK>()
                .Property(e => e.QT_ESTOQUE)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_STOCK>()
                .Property(e => e.QT_RESERVA_SAIDA)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_STOCK>()
                .Property(e => e.QT_TRANSITO_ENTRADA)
                .HasPrecision(38, 0);
            #endregion

            #region V_STOCK_PRODUTO
            modelBuilder.Entity<V_STOCK_PRODUTO>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_STOCK_PRODUTO>()
                .Property(e => e.QT_ESTOQUE)
                .HasPrecision(12, 3);
            #endregion

            #region V_STOCK_TOTAL
            modelBuilder.Entity<V_STOCK_TOTAL>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_STOCK_TOTAL>()
                .Property(e => e.QT_FISICO_SL)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_STOCK_TOTAL>()
                .Property(e => e.QT_FISICO_SD)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_STOCK_TOTAL>()
                .Property(e => e.QT_SALIDA_SL)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_STOCK_TOTAL>()
                .Property(e => e.QT_SALIDA_SD)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_STOCK_TOTAL>()
                .Property(e => e.QT_ENTRADA_SL)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_STOCK_TOTAL>()
                .Property(e => e.QT_ENTRADA_SD)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_STOCK_TOTAL>()
                .Property(e => e.QT_LIBRE)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_STOCK_TOTAL>()
                .Property(e => e.QT_DISPONIBLE)
                .HasPrecision(38, 0);
            #endregion

            #region V_STOCK_TRASPASO_WSTO040
            modelBuilder.Entity<V_STOCK_TRASPASO_WSTO040>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_STOCK_TRASPASO_WSTO040>()
                .Property(e => e.QT_RESERVA_SAIDA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_STOCK_TRASPASO_WSTO040>()
                .Property(e => e.QT_TRANSITO_ENTRADA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_STOCK_TRASPASO_WSTO040>()
                .Property(e => e.QT_ESTOQUE)
                .HasPrecision(12, 3);
            #endregion

            #region V_STOCK_TRASPASO_DEST
            modelBuilder.Entity<V_STOCK_TRASPASO_DEST>()
                .Property(e => e.CD_FAIXA_DEST)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_STOCK_TRASPASO_DEST>()
                .Property(e => e.QT_TRASPASO_DEST)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_STOCK_TRASPASO_ORIGEN>()
                .Property(e => e.CD_FAIXA_ORIGEN)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_STOCK_TRASPASO_ORIGEN>()
                .Property(e => e.QT_TRASPASO_ORIGEN)
                .HasPrecision(38, 0);
            #endregion

            #region V_DOCUMENTO_DOC080
            modelBuilder.Entity<V_DOCUMENTO_DOC080>()
                .Property(e => e.VL_ARBITRAJE)
                .HasPrecision(9, 4);
            modelBuilder.Entity<V_DOCUMENTO_DOC080>()
                .Property(e => e.VL_SEGURO)
                .HasPrecision(9, 4);
            modelBuilder.Entity<V_DOCUMENTO_DOC080>()
                .Property(e => e.QT_VOLUMEN)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DOCUMENTO_DOC080>()
                .Property(e => e.QT_PESO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DOCUMENTO_DOC080>()
                .Property(e => e.VL_FLETE)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DOCUMENTO_DOC080>()
                .Property(e => e.VL_OTROS_GASTOS)
                .HasPrecision(12, 3);
            #endregion

            #region V_CONT_PRODUTO_WSTO150DET
            modelBuilder.Entity<V_CONT_PRODUTO_WSTO150DET>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_CONT_PRODUTO_WSTO150DET>()
                .Property(e => e.QT_PREPARADO)
                .HasPrecision(38, 0);
            #endregion

            #region V_DET_DOC_DUA_DOC020

            modelBuilder.Entity<V_DET_DOC_DUA_DOC020>()
                   .Property(e => e.TP_DUA)
                   .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOC_DUA_DOC020>()
                    .Property(e => e.NU_DUA)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOC_DUA_DOC020>()
                    .Property(e => e.NU_DOCUMENTO_INGRESO_DUA)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOC_DUA_DOC020>()
                    .Property(e => e.TP_DOCUMENTO_INGRESO_DUA)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOC_DUA_DOC020>()
                    .Property(e => e.ID_ESTADO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOC_DUA_DOC020>()
                    .Property(e => e.VL_ARBITRAJE)
                    .HasPrecision(38, 0);

            modelBuilder.Entity<V_DET_DOC_DUA_DOC020>()
                    .Property(e => e.CD_MONEDA)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOC_DUA_DOC020>()
                    .Property(e => e.ID_MANUAL)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOC_DUA_DOC020>()
                    .Property(e => e.NU_AGRUPADOR)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOC_DUA_DOC020>()
                    .Property(e => e.TP_AGRUPADOR)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOC_DUA_DOC020>()
                    .Property(e => e.CD_PRODUTO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOC_DUA_DOC020>()
                    .Property(e => e.DS_PRODUTO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOC_DUA_DOC020>()
                    .Property(e => e.NCM)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOC_DUA_DOC020>()
                    .Property(e => e.CD_FAIXA)
                    .HasPrecision(9, 3);

            modelBuilder.Entity<V_DET_DOC_DUA_DOC020>()
                    .Property(e => e.NU_IDENTIFICADOR)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOC_DUA_DOC020>()
                    .Property(e => e.NM_EMPRESA)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOC_DUA_DOC020>()
                    .Property(e => e.NU_DOCUMENTO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOC_DUA_DOC020>()
                    .Property(e => e.TP_DOCUMENTO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOC_DUA_DOC020>()
                    .Property(e => e.QT_INGRESADA)
                    .HasPrecision(38, 0);

            modelBuilder.Entity<V_DET_DOC_DUA_DOC020>()
                    .Property(e => e.QT_RESERVADA)
                    .HasPrecision(38, 0);

            modelBuilder.Entity<V_DET_DOC_DUA_DOC020>()
                    .Property(e => e.QT_DESAFECTADA)
                    .HasPrecision(38, 0);

            modelBuilder.Entity<V_DET_DOC_DUA_DOC020>()
                    .Property(e => e.ID_DISPONIBLE)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOC_DUA_DOC020>()
                    .Property(e => e.VL_MERCADERIA)
                    .HasPrecision(38, 0);

            modelBuilder.Entity<V_DET_DOC_DUA_DOC020>()
                    .Property(e => e.QT_MERCADERIA)
                    .HasPrecision(38, 0);

            modelBuilder.Entity<V_DET_DOC_DUA_DOC020>()
                    .Property(e => e.QT_DISPONIBLE)
                    .HasPrecision(38, 0);

            modelBuilder.Entity<V_DET_DOC_DUA_DOC020>()
                    .Property(e => e.VL_EXISTENCIA_USD)
                    .HasPrecision(20, 3);

            #endregion

            #region V_DOCUMENTO_DOC095
            modelBuilder.Entity<V_DOCUMENTO_DOC095>()
                .Property(e => e.QT_LINEAS)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_DOCUMENTO_DOC095>()
                .Property(e => e.QT_PRODUCTO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_DOCUMENTO_DOC095>()
                .Property(e => e.QT_DOCUMENTO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_DOCUMENTO_DOC095>()
                .Property(e => e.QT_CIF)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_DOCUMENTO_DOC095>()
                .Property(e => e.VL_SEGURO)
                .HasPrecision(9, 4);
            modelBuilder.Entity<V_DOCUMENTO_DOC095>()
                .Property(e => e.VL_FLETE)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DOCUMENTO_DOC095>()
                .Property(e => e.QT_PESO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DOCUMENTO_DOC095>()
                .Property(e => e.QT_VOLUMEN)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DOCUMENTO_DOC095>()
                .Property(e => e.VL_ARBITRAJE)
                .HasPrecision(9, 4);
            modelBuilder.Entity<V_DOCUMENTO_DOC095>()
                .Property(e => e.VL_OTROS_GASTOS)
                .HasPrecision(12, 3);
            #endregion

            #region V_DOCUMENTO_RESERVA_DOC300
            modelBuilder.Entity<V_DOCUMENTO_RESERVA_DOC300>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_DOCUMENTO_RESERVA_DOC300>()
                .Property(e => e.QT_PRODUTO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DOCUMENTO_RESERVA_DOC300>()
                .Property(e => e.QT_ANULAR)
                .HasPrecision(12, 3);
            #endregion

            #region V_DET_DOCUMENTO_EGRESO
            modelBuilder.Entity<V_DET_DOCUMENTO_EGRESO>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_DET_DOCUMENTO_EGRESO>()
                .Property(e => e.VL_FOB_INGRESO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_DET_DOCUMENTO_EGRESO>()
                .Property(e => e.VL_CIF_INGRESO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_DET_DOCUMENTO_EGRESO>()
                .Property(e => e.QT_EGRESO)
                .HasPrecision(38, 0);
            #endregion

            #region V_DET_DOCUMENTO_INGRESO
            modelBuilder.Entity<V_DET_DOCUMENTO_INGRESO>()
              .Property(e => e.CD_FAIXA)
              .HasPrecision(9, 3);
            modelBuilder.Entity<V_DET_DOCUMENTO_INGRESO>()
                .Property(e => e.QT_DESAFECTADA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DET_DOCUMENTO_INGRESO>()
                .Property(e => e.QT_DESCARGADA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DET_DOCUMENTO_INGRESO>()
                .Property(e => e.QT_INGRESADA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DET_DOCUMENTO_INGRESO>()
                .Property(e => e.QT_RESERVADA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DET_DOCUMENTO_INGRESO>()
                .Property(e => e.VL_CIF)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DET_DOCUMENTO_INGRESO>()
                .Property(e => e.VL_MERCADERIA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DET_DOCUMENTO_INGRESO>()
                .Property(e => e.QT_DISPONIBLE)
                .HasPrecision(38, 0);
            #endregion

            #region V_LOG_DOCUMENTO
            modelBuilder.Entity<V_LOG_DOCUMENTO>()
                .Property(e => e.QT_PESO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_LOG_DOCUMENTO>()
                .Property(e => e.QT_VOLUMEN)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_LOG_DOCUMENTO>()
                .Property(e => e.VL_ARBITRAJE)
                .HasPrecision(9, 4);
            modelBuilder.Entity<V_LOG_DOCUMENTO>()
                .Property(e => e.VL_FLETE)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_LOG_DOCUMENTO>()
                .Property(e => e.VL_OTROS_GASTOS)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_LOG_DOCUMENTO>()
                .Property(e => e.VL_SEGURO)
                .HasPrecision(9, 4);
            #endregion

            #region V_DET_DOC_DUA_DOC021
            modelBuilder.Entity<V_DET_DOC_DUA_DOC021>()
                .Property(e => e.VL_ARBITRAJE)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_DET_DOC_DUA_DOC021>()
                .Property(e => e.QT_INGRESADA)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_DET_DOC_DUA_DOC021>()
                .Property(e => e.QT_RESERVADA)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_DET_DOC_DUA_DOC021>()
                .Property(e => e.QT_DESAFECTADA)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_DET_DOC_DUA_DOC021>()
                .Property(e => e.VL_MERCADERIA)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_DET_DOC_DUA_DOC021>()
                .Property(e => e.QT_MERCADERIA)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_DET_DOC_DUA_DOC021>()
                .Property(e => e.QT_DISPONIBLE)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_DET_DOC_DUA_DOC021>()
                .Property(e => e.QT_EXISTENCIA)
                .HasPrecision(38, 0);
            #endregion

            #region V_DET_PICKING_WSTO150DET
            modelBuilder.Entity<V_DET_PICKING_WSTO150DET>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_DET_PICKING_WSTO150DET>()
                .Property(e => e.QT_PRODUTO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DET_PICKING_WSTO150DET>()
                .Property(e => e.QT_PREPARADO)
                .HasPrecision(12, 3);
            #endregion

            #region V_ETIQUETAS_LOTE_WSTO150DET
            modelBuilder.Entity<V_ETIQUETAS_LOTE_WSTO150DET>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_ETIQUETAS_LOTE_WSTO150DET>()
                .Property(e => e.QT_PRODUTO_RECIBIDO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_ETIQUETAS_LOTE_WSTO150DET>()
                .Property(e => e.QT_ALMACENADO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_ETIQUETAS_LOTE_WSTO150DET>()
                .Property(e => e.QT_PRODUTO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_ETIQUETAS_LOTE_WSTO150DET>()
                .Property(e => e.QT_ETIQUETA_GENERADA)
                .HasPrecision(12, 3);
            #endregion

            #region V_ETIQUETAS_WREC150
            modelBuilder.Entity<V_ETIQUETAS_WREC150>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_ETIQUETAS_WREC150>()
                .Property(e => e.QT_PRODUTO_RECIBIDO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_ETIQUETAS_WREC150>()
                .Property(e => e.QT_ALMACENADO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_ETIQUETAS_WREC150>()
                .Property(e => e.QT_PRODUTO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_ETIQUETAS_WREC150>()
                .Property(e => e.QT_ETIQUETA_GENERADA)
                .HasPrecision(12, 3);
            #endregion

            #region V_PALLET_TRANSF_WSTO150DET
            modelBuilder.Entity<V_PALLET_TRANSF_WSTO150DET>()
                .Property(e => e.NU_ETIQUETA)
                .HasPrecision(20, 0);
            modelBuilder.Entity<V_PALLET_TRANSF_WSTO150DET>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_PALLET_TRANSF_WSTO150DET>()
                .Property(e => e.QT_PRODUTO)
                .HasPrecision(12, 3);
            #endregion

            #region T_DOCUMENTO_TIPO

            modelBuilder.Entity<T_DOCUMENTO_TIPO>()
                   .Property(e => e.TP_DOCUMENTO)
                   .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_TIPO>()
                    .Property(e => e.TP_OPERACION)
                    .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_TIPO>()
                    .Property(e => e.DS_TP_DOCUMENTO)
                    .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_TIPO>()
                    .Property(e => e.FL_NUMERO_AUTOGENERADO)
                    .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_TIPO>()
                    .Property(e => e.FL_INGRESO_MANUAL)
                    .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_TIPO>()
                    .Property(e => e.FL_HABILITADO)
                    .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_TIPO>()
                    .Property(e => e.FL_MANEJA_AGRUPADOR)
                    .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_TIPO>()
                    .Property(e => e.FL_MANEJA_CAMBIO_ESTADO)
                    .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_TIPO>()
                    .Property(e => e.FL_REQUIERE_DUA)
                    .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_TIPO>()
                    .Property(e => e.FL_PERMITE_EDICION)
                    .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_TIPO>()
                    .Property(e => e.FL_REQUIERE_FACTURA)
                    .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_TIPO>()
                    .Property(e => e.FL_AUTOAGENDABLE)
                    .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_TIPO>()
                    .Property(e => e.FL_MANEJA_AGENDA)
                    .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_TIPO>()
                    .Property(e => e.FL_MANEJA_CAMION)
                    .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_TIPO>()
                    .HasMany(e => e.T_DOCUMENTO_TIPO_EXTERNO)
                    .WithOne(e => e.T_DOCUMENTO_TIPO)
                    .HasForeignKey(d => d.TP_DOCUMENTO)
                    .IsRequired();

            modelBuilder.Entity<T_DOCUMENTO_TIPO>()
                    .HasMany(e => e.T_DOCUMENTO)
                    .WithOne(e => e.T_DOCUMENTO_TIPO)
                    .HasForeignKey(d => d.TP_DOCUMENTO);

            modelBuilder.Entity<T_DOCUMENTO_TIPO>()
                    .HasMany(e => e.T_TIPO_DUA_DOCUMENTO)
                    .WithOne(e => e.T_DOCUMENTO_TIPO)
                    .HasForeignKey(d => d.TP_DOCUMENTO);

            modelBuilder.Entity<T_DOCUMENTO_TIPO>()
                    .HasMany(e => e.T_TIPO_REFERENCIA_EXTERNA_DOCUMENTO)
                    .WithOne(e => e.T_DOCUMENTO_TIPO)
                    .HasForeignKey(d => d.TP_DOCUMENTO);

            modelBuilder.Entity<T_DOCUMENTO_TIPO>()
                    .HasMany(e => e.T_DOCUMENTO_ESTADO_ORDEN)
                    .WithOne(e => e.T_DOCUMENTO_TIPO)
                    .HasForeignKey(d => d.TP_DOCUMENTO);

            #endregion

            #region T_TIPO_DUA_DOCUMENTO

            modelBuilder.Entity<T_TIPO_DUA_DOCUMENTO>()
                    .HasOne(e => e.T_DOCUMENTO_TIPO)
                    .WithMany(e => e.T_TIPO_DUA_DOCUMENTO)
                    .HasForeignKey(d => d.TP_DOCUMENTO)
                    .IsRequired();

            modelBuilder.Entity<T_TIPO_DUA_DOCUMENTO>()
                    .HasOne(e => e.T_TIPO_DUA)
                    .WithMany(e => e.T_TIPO_DUA_DOCUMENTO)
                    .HasForeignKey(d => d.TP_DUA)
                    .IsRequired();

            #endregion

            #region T_TIPO_REFERENCIA_EXTERNA_DOCUMENTO

            modelBuilder.Entity<T_TIPO_REFERENCIA_EXTERNA_DOCUMENTO>()
                    .HasOne(e => e.T_DOCUMENTO_TIPO)
                    .WithMany(e => e.T_TIPO_REFERENCIA_EXTERNA_DOCUMENTO)
                    .HasForeignKey(d => d.TP_DOCUMENTO)
                    .IsRequired();

            modelBuilder.Entity<T_TIPO_REFERENCIA_EXTERNA_DOCUMENTO>()
                    .HasOne(e => e.T_TIPO_REFERENCIA_EXTERNA)
                    .WithMany(e => e.T_TIPO_REFERENCIA_EXTERNA_DOCUMENTO)
                    .HasForeignKey(d => d.TP_REFERENCIA_EXTERNA)
                    .IsRequired();

            #endregion

            #region V_DOCUMENTO_LINEA_DET_DOC082
            modelBuilder.Entity<V_DOCUMENTO_LINEA_DET_DOC082>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_DOCUMENTO_LINEA_DET_DOC082>()
                .Property(e => e.QT_ASOCIADA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DOCUMENTO_LINEA_DET_DOC082>()
                .Property(e => e.VL_MERCADERIA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DOCUMENTO_LINEA_DET_DOC082>()
                .Property(e => e.VL_CIF)
                .HasPrecision(12, 3);
            #endregion

            #region LT_DET_DOCUMENTO
            modelBuilder.Entity<LT_DET_DOCUMENTO>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<LT_DET_DOCUMENTO>()
                .Property(e => e.VL_MERCADERIA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<LT_DET_DOCUMENTO>()
                .Property(e => e.QT_INGRESADA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<LT_DET_DOCUMENTO>()
                .Property(e => e.QT_RESERVADA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<LT_DET_DOCUMENTO>()
                .Property(e => e.QT_DESAFECTADA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<LT_DET_DOCUMENTO>()
                .Property(e => e.VL_CIF)
                .HasPrecision(12, 3);
            modelBuilder.Entity<LT_DET_DOCUMENTO>()
                .Property(e => e.QT_DESCARGADA)
                .HasPrecision(12, 3);
            #endregion

            #region LT_DET_DOCUMENTO_EGRESO
            modelBuilder.Entity<LT_DET_DOCUMENTO_EGRESO>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<LT_DET_DOCUMENTO_EGRESO>()
                .Property(e => e.QT_DESAFECTADA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<LT_DET_DOCUMENTO_EGRESO>()
                .Property(e => e.VL_FOB)
                .HasPrecision(12, 3);
            modelBuilder.Entity<LT_DET_DOCUMENTO_EGRESO>()
                .Property(e => e.VL_CIF)
                .HasPrecision(12, 3);
            modelBuilder.Entity<LT_DET_DOCUMENTO_EGRESO>()
                .Property(e => e.QT_DESCARGADA)
                .HasPrecision(12, 3);
            #endregion

            #region V_LT_DET_DOCUMENTO
            modelBuilder.Entity<V_LT_DET_DOCUMENTO>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_LT_DET_DOCUMENTO>()
                .Property(e => e.VL_MERCADERIA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_LT_DET_DOCUMENTO>()
                .Property(e => e.QT_INGRESADA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_LT_DET_DOCUMENTO>()
                .Property(e => e.QT_RESERVADA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_LT_DET_DOCUMENTO>()
                .Property(e => e.QT_DESAFECTADA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_LT_DET_DOCUMENTO>()
                .Property(e => e.VL_CIF)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_LT_DET_DOCUMENTO>()
                .Property(e => e.QT_DESCARGADA)
                .HasPrecision(12, 3);
            #endregion

            #region V_LT_DET_DOCUMENTO_EGRESO
            modelBuilder.Entity<V_LT_DET_DOCUMENTO_EGRESO>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_LT_DET_DOCUMENTO_EGRESO>()
                .Property(e => e.QT_DESAFECTADA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_LT_DET_DOCUMENTO_EGRESO>()
                .Property(e => e.VL_FOB)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_LT_DET_DOCUMENTO_EGRESO>()
                .Property(e => e.VL_CIF)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_LT_DET_DOCUMENTO_EGRESO>()
                .Property(e => e.QT_DESCARGADA)
                .HasPrecision(12, 3);
            #endregion

            #region V_LT_DOCUMENTO
            modelBuilder.Entity<V_LT_DOCUMENTO>()
                .Property(e => e.VL_ARBITRAJE)
                .HasPrecision(9, 4);
            modelBuilder.Entity<V_LT_DOCUMENTO>()
                .Property(e => e.VL_SEGURO)
                .HasPrecision(9, 4);
            modelBuilder.Entity<V_LT_DOCUMENTO>()
                .Property(e => e.QT_VOLUMEN)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_LT_DOCUMENTO>()
                .Property(e => e.QT_PESO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_LT_DOCUMENTO>()
                .Property(e => e.VL_FLETE)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_LT_DOCUMENTO>()
                .Property(e => e.VL_OTROS_GASTOS)
                .HasPrecision(12, 3);
            #endregion


            #region ajuste stock
            modelBuilder.Entity<T_AJUSTE_STOCK>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<T_AJUSTE_STOCK>()
                .Property(e => e.QT_MOVIMIENTO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_AJUSTE_STOCK>()
                .Property(e => e.NU_INVENTARIO_ENDERECO_DET)
                .HasPrecision(38, 0);
            #endregion

            #region V_RESOURCES_WSEG020
            #endregion

            #region V_CONTENEDOR_CON_PROBLEMA
            modelBuilder.Entity<V_CONTENEDOR_CON_PROBLEMA>()
                .Property(e => e.PS_REAL)
                .HasPrecision(15, 3);
            modelBuilder.Entity<V_CONTENEDOR_CON_PROBLEMA>()
                .Property(e => e.VL_ALTURA)
                .HasPrecision(12, 5);
            modelBuilder.Entity<V_CONTENEDOR_CON_PROBLEMA>()
                .Property(e => e.VL_LARGURA)
                .HasPrecision(12, 5);
            modelBuilder.Entity<V_CONTENEDOR_CON_PROBLEMA>()
                .Property(e => e.VL_PROFUNDIDADE)
                .HasPrecision(12, 5);
            modelBuilder.Entity<V_CONTENEDOR_CON_PROBLEMA>()
                .Property(e => e.VL_CUBAGEM)
                .HasPrecision(15, 3);
            #endregion

            #region T_PEDIDO_SAIDA
            //modelBuilder.Entity<T_PEDIDO_SAIDA>()
            //    .HasMany(e => e.T_DET_PEDIDO_SAIDA)
            //    .WithOne(e => e.T_PEDIDO_SAIDA)
            //    .HasForeignKey(e => new { e.NU_PEDIDO, e.CD_CLIENTE, e.CD_EMPRESA })
            //    .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region T_DET_PEDIDO_SAIDA
            modelBuilder.Entity<T_DET_PEDIDO_SAIDA>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<T_DET_PEDIDO_SAIDA>()
                .Property(e => e.QT_PEDIDO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_DET_PEDIDO_SAIDA>()
                .Property(e => e.QT_LIBERADO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_DET_PEDIDO_SAIDA>()
                .Property(e => e.QT_ANULADO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_DET_PEDIDO_SAIDA>()
                .Property(e => e.QT_PEDIDO_ORIGINAL)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_DET_PEDIDO_SAIDA>()
                .Property(e => e.VL_PORCENTAJE_TOLERANCIA)
                .HasPrecision(38, 0);
            modelBuilder.Entity<T_DET_PEDIDO_SAIDA>()
                .Property(e => e.QT_PREPARADO)
                .HasPrecision(15, 3);
            modelBuilder.Entity<T_DET_PEDIDO_SAIDA>()
                .Property(e => e.QT_EXPEDIDO)
                .HasPrecision(15, 3);
            modelBuilder.Entity<T_DET_PEDIDO_SAIDA>()
                .Property(e => e.QT_FACTURADO)
                .HasPrecision(15, 3);
            modelBuilder.Entity<T_DET_PEDIDO_SAIDA>()
                .Property(e => e.QT_CROSS_DOCK)
                .HasPrecision(15, 3);
            modelBuilder.Entity<T_DET_PEDIDO_SAIDA>()
                .Property(e => e.QT_CONTROLADO)
                .HasPrecision(15, 3);
            modelBuilder.Entity<T_DET_PEDIDO_SAIDA>()
                .Property(e => e.QT_TRANSFERIDO)
                .HasPrecision(15, 3);
            modelBuilder.Entity<T_DET_PEDIDO_SAIDA>()
                .Property(e => e.QT_ABASTECIDO)
                .HasPrecision(15, 3);
            modelBuilder.Entity<T_DET_PEDIDO_SAIDA>()
                .Property(e => e.QT_CARGADO)
                .HasPrecision(15, 3);
            modelBuilder.Entity<T_DET_PEDIDO_SAIDA>()
                .Property(e => e.QT_ANULADO_FACTURA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_DET_PEDIDO_SAIDA>()
                .Property(e => e.NU_GENERICO_1)
                .HasPrecision(15, 3);
            #endregion

            #region T_PICKING
            modelBuilder.Entity<T_PICKING>()
                .Property(e => e.VL_PORCENTAJE_REPARTO_COMUN)
                .HasPrecision(38, 0);
            modelBuilder.Entity<T_PICKING>()
                .Property(e => e.QT_RECHAZOS)
                .HasPrecision(38, 0);
            //modelBuilder.Entity<T_PICKING>()
            //    .HasMany(e => e.T_CONTENEDOR)
            //    .WithOne(e => e.T_PICKING)
            //    .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<T_PICKING>()
            //    .HasMany(e => e.T_DET_PICKING)
            //    .WithOne(e => e.T_PICKING)
            //    .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region T_DET_PICKING
            modelBuilder.Entity<T_DET_PICKING>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<T_DET_PICKING>()
                .Property(e => e.QT_PRODUTO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_DET_PICKING>()
                .Property(e => e.QT_PREPARADO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_DET_PICKING>()
                .Property(e => e.QT_PICKEO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_DET_PICKING>()
                .Property(e => e.QT_CONTROLADO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_DET_PICKING>()
                .Property(e => e.QT_CONTROL)
                .HasPrecision(12, 3);
            #endregion

            #region T_CONTENEDOR
            modelBuilder.Entity<T_CONTENEDOR>()
                .Property(e => e.PS_REAL)
                .HasPrecision(15, 3);
            modelBuilder.Entity<T_CONTENEDOR>()
                .Property(e => e.VL_ALTURA)
                .HasPrecision(12, 5);
            modelBuilder.Entity<T_CONTENEDOR>()
                .Property(e => e.VL_LARGURA)
                .HasPrecision(12, 5);
            modelBuilder.Entity<T_CONTENEDOR>()
                .Property(e => e.VL_PROFUNDIDADE)
                .HasPrecision(12, 5);
            modelBuilder.Entity<T_CONTENEDOR>()
                .Property(e => e.VL_CUBAGEM)
                .HasPrecision(15, 3);
            //modelBuilder.Entity<T_CONTENEDOR>()
            //    .HasMany(e => e.T_DET_PICKING)
            //    .WithOne(e => e.T_CONTENEDOR)
            //    .HasForeignKey(e => new { e.NU_PREPARACION, e.NU_CONTENEDOR });
            #endregion

            #region V_CONT_SIN_EMBARCAR_WEXP041
            modelBuilder.Entity<V_PRODUCTOS_SIN_PREP_WEXP041>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_PRODUCTOS_SIN_PREP_WEXP041>()
                .Property(e => e.QT_PRODUTO)
                .HasPrecision(38, 0);
            #region T_INV_ENDERECO_DET_ERROR
            modelBuilder.Entity<T_INV_ENDERECO_DET_ERROR>()
               .Property(e => e.NU_ERROR)
               .HasPrecision(38, 0);
            modelBuilder.Entity<T_INV_ENDERECO_DET_ERROR>()
                .Property(e => e.NU_INVENTARIO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<T_INV_ENDERECO_DET_ERROR>()
                .Property(e => e.NU_INVENTARIO_ENDERECO_DET)
                .HasPrecision(38, 0);
            #endregion
            #endregion

            #region T_DET_PEDIDO_EXPEDIDO
            modelBuilder.Entity<T_DET_PEDIDO_EXPEDIDO>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<T_DET_PEDIDO_EXPEDIDO>()
                .Property(e => e.QT_PRODUTO)
                .HasPrecision(12, 3);
            #endregion

            #region V_CANTIDAD_ENVIADA_EXP
            modelBuilder.Entity<V_CANTIDAD_ENVIADA_EXP>()
                .Property(e => e.QT_PRODUTO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_CANTIDAD_ENVIADA_EXP>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            #endregion

            #region V_CANT_CONTE_PUERTA_WEXP
            modelBuilder.Entity<V_CANT_CONTE_PUERTA_WEXP>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_CANT_CONTE_PUERTA_WEXP>()
                .Property(e => e.QT_PREPARADO)
                .HasPrecision(38, 0);
            #endregion

            #region T_DOCUMENTO

            modelBuilder.Entity<T_DOCUMENTO>()
               .Property(e => e.NU_DOCUMENTO)
               .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.TP_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.DS_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.TP_DUA)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.NU_DUA)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.CD_MONEDA)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.VL_ARBITRAJE)
                .HasPrecision(9, 4);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.ID_GENERAR_AGENDA)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
               .Property(e => e.VL_VALIDADO)
               .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.CD_VIA)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.NU_FACTURA)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.NU_CONOCIMIENTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.ID_ESTADO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.CD_UNIDAD_MEDIDA_BULTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.NU_IMPORT)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.NU_EXPORT)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.VL_SEGURO)
                .HasPrecision(9, 4);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.QT_VOLUMEN)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.QT_PESO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_DOCUMENTO>()
               .Property(e => e.FL_BALANCEADO)
               .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.NU_PREDIO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.VL_FLETE)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.ID_MANUAL)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.ID_AGENDAR_AUTOMATICAMENTE)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.VL_OTROS_GASTOS)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.NU_DOC_TRANSPORTE)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.DS_ANEXO1)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.DS_ANEXO2)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.DS_ANEXO3)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.DS_ANEXO4)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.DS_ANEXO5)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.DS_ANEXO6)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.ID_FICTICIO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.NU_CORRELATIVO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.NU_DTI)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.NU_CORRELATIVO_2)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.VL_VALIDADO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.CD_CLIENTE)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
                .Property(e => e.VL_DATO_AUDITORIA)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
               .Property(e => e.NU_AGRUPADOR)
               .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
               .Property(e => e.TP_AGRUPADOR)
               .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
               .Property(e => e.TP_DOCUMENTO_EXTERNO)
               .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO>()
               .Property(e => e.ICMS)
               .HasPrecision(9, 4);

            modelBuilder.Entity<T_DOCUMENTO>()
              .Property(e => e.II)
              .HasPrecision(9, 4);

            modelBuilder.Entity<T_DOCUMENTO>()
              .Property(e => e.IPI)
              .HasPrecision(9, 4);

            modelBuilder.Entity<T_DOCUMENTO>()
              .Property(e => e.IISUSPENSO)
              .HasPrecision(9, 4);

            modelBuilder.Entity<T_DOCUMENTO>()
              .Property(e => e.IPISUSPENSO)
              .HasPrecision(9, 4);

            modelBuilder.Entity<T_DOCUMENTO>()
           .Property(e => e.PISCONFINS)
           .HasPrecision(9, 4);

            modelBuilder.Entity<T_DOCUMENTO>()
           .Property(e => e.CD_REGIMEN_ADUANA)
           .HasPrecision(9, 0);

            #endregion

            #region V_DET_CONTENEDORES_WEXP330
            modelBuilder.Entity<V_DET_CONTENEDORES_WEXP330>()
                .Property(e => e.QT_PREPARADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_DET_CONTENEDORES_WEXP330>()
                .Property(e => e.ID_TEMPORAL)
                .IsFixedLength()
                .IsUnicode(false);
            #endregion

            #region V_REC210_CROSS_DOCKING
            modelBuilder.Entity<V_REC210_CROSS_DOCKING>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_REC210_CROSS_DOCKING>()
                .Property(e => e.QT_PRODUTO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REC210_CROSS_DOCKING>()
                .Property(e => e.QT_PREPARADO)
                .HasPrecision(12, 3);
            #endregion

            #region V_INV_CONTROL_MONO_PROD_LOTE
            modelBuilder.Entity<V_INV_CONTROL_MONO_PROD_LOTE>()
               .Property(e => e.NU_INVENTARIO_ENDERECO_DET)
               .HasPrecision(38, 0);
            modelBuilder.Entity<V_INV_CONTROL_MONO_PROD_LOTE>()
                .Property(e => e.NU_INVENTARIO_ENDERECO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_INV_CONTROL_MONO_PROD_LOTE>()
                .Property(e => e.NU_INVENTARIO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_INV_CONTROL_MONO_PROD_LOTE>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_INV_CONTROL_MONO_PROD_LOTE>()
                .Property(e => e.QT_ESTOQUE)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_INV_CONTROL_MONO_PROD_LOTE>()
                .Property(e => e.QT_DIFERENCIA)
                .HasPrecision(15, 3);
            modelBuilder.Entity<V_INV_CONTROL_MONO_PROD_LOTE>()
                .Property(e => e.QT_SALDO)
                .HasPrecision(38, 0);
            #endregion

            #region V_DET_CONT_CON_PROBLEMA
            modelBuilder.Entity<V_DET_CONT_CON_PROBLEMA>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_DET_CONT_CON_PROBLEMA>()
                .Property(e => e.QT_PRODUTO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DET_CONT_CON_PROBLEMA>()
                .Property(e => e.QT_PREPARADO)
                .HasPrecision(12, 3);
            #endregion

            #region Modulo eventos
            #region T_CONTACTO
            modelBuilder.Entity<T_CONTACTO>()
                .HasMany(e => e.T_CONTACTO_GRUPO_REL)
                .WithOne(e => e.T_CONTACTO)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion
            #region T_EVENTO
            modelBuilder.Entity<T_EVENTO>()
                .HasMany(e => e.T_EVENTO_BANDEJA)
                .WithOne(e => e.T_EVENTO)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion
            #region T_EVENTO_BANDEJA
            modelBuilder.Entity<T_EVENTO_BANDEJA>()
                .HasMany(e => e.T_EVENTO_BANDEJA_INSTANCIA)
                .WithOne(e => e.T_EVENTO_BANDEJA)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion
            #region T_EVENTO_INSTANCIA
            modelBuilder.Entity<T_EVENTO_INSTANCIA>()
                .HasMany(e => e.T_EVENTO_PARAMETRO_INSTANCIA)
                .WithOne(e => e.T_EVENTO_INSTANCIA)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion
            #region T_EVENTO_TEMPLATE

            modelBuilder.Entity<T_EVENTO_TEMPLATE>()
                .HasMany(e => e.T_EVENTO_INSTANCIA)
                .WithOne(e => e.T_EVENTO_TEMPLATE)
                .HasForeignKey(e => new { e.CD_LABEL_ESTILO, e.TP_NOTIFICACION, e.NU_EVENTO });

            if (!string.IsNullOrEmpty(_blobDataType))
            {
                modelBuilder.Entity<T_EVENTO_TEMPLATE>()
                    .Property(e => e.VL_CUERPO)
                    .HasColumnType(_blobDataType);
            }

            #endregion
            #region T_EVENTO_NOTIFICACION
            modelBuilder.Entity<T_EVENTO_NOTIFICACION>()
                .HasMany(e => e.T_EVENTO_NOTIFICACION_ARCHIVO)
                .WithOne(e => e.T_EVENTO_NOTIFICACION)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<T_EVENTO_NOTIFICACION>()
                .HasOne(e => e.T_EVENTO_NOTIFICACION_EMAIL)
                .WithOne(e => e.T_EVENTO_NOTIFICACION)
                .HasForeignKey<T_EVENTO_NOTIFICACION_EMAIL>(e => e.NU_EVENTO_NOTIFICACION);
            #endregion
            #region T_EVENTO_NOTIFICACION_ARCHIVO

            if (!string.IsNullOrEmpty(_blobDataType))
            {
                modelBuilder.Entity<T_EVENTO_NOTIFICACION_ARCHIVO>()
                    .Property(e => e.VL_DATA)
                    .HasColumnType(_blobDataType);
            }

            #endregion
            #region T_EVENTO_NOTIFICACION_EMAIL

            if (!string.IsNullOrEmpty(_blobDataType))
            {
                modelBuilder.Entity<T_EVENTO_NOTIFICACION_EMAIL>()
                    .Property(e => e.DS_CUERPO)
                    .HasColumnType(_blobDataType);
            }

            #endregion
            #region T_EVENTO_PARAMETRO
            modelBuilder.Entity<T_EVENTO_INSTANCIA>()
                .HasMany(e => e.T_EVENTO_PARAMETRO_INSTANCIA)
                .WithOne(e => e.T_EVENTO_INSTANCIA)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion
            #region  V_PEDIDOS_EXPEDIDOS_EXEL
            modelBuilder.Entity<V_PEDIDOS_EXPEDIDOS_EXEL>()
                .Property(e => e.QT_PRODUTO)
                .HasPrecision(12, 3);
            #endregion
            #region WREC250
            modelBuilder.Entity<V_RESUMEN_AGENDA_WREC250>()
                .Property(e => e.NU_COLOR)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_RESUMEN_AGENDA_WREC250>()
                .Property(e => e.QT_ACEPTADA)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_RESUMEN_AGENDA_WREC250>()
                .Property(e => e.QT_RECIBIDA)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_RESUMEN_AGENDA_WREC250>()
                .Property(e => e.QT_CROSS_DOCKING)
                .HasPrecision(38, 0);
            #endregion

            //#region T_REGLA_LIBERACION
            //modelBuilder.Entity<T_REGLA_LIBERACION>()
            //    .HasMany(e => e.T_REGLA_CLIENTES)
            //    .WithOne(e => e.T_REGLA_LIBERACION)
            //    .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<T_REGLA_LIBERACION>()
            //    .HasMany(e => e.T_REGLA_CONDICION_LIBERACION)
            //    .WithOne(e => e.T_REGLA_LIBERACION)
            //    .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<T_REGLA_LIBERACION>()
            //    .HasMany(e => e.T_REGLA_TIPO_EXPEDICION)
            //    .WithOne(e => e.T_REGLA_LIBERACION)
            //    .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<T_REGLA_LIBERACION>()
            //    .HasMany(e => e.T_REGLA_TIPO_PEDIDO)
            //    .WithOne(e => e.T_REGLA_LIBERACION)
            //    .OnDelete(DeleteBehavior.Restrict);
            //#endregion
            #region T_TEMP_CONDICION_LIBERACION
            modelBuilder.Entity<T_TEMP_CONDICION_LIBERACION>()
                .Property(e => e.NU_TEMP_CONDICION_LIBERACION)
                .HasPrecision(38, 0);
            modelBuilder.Entity<T_TEMP_CONDICION_LIBERACION>()
                .Property(e => e.CD_EMPRESA)
                .HasPrecision(38, 0);
            #endregion

            #region V_DET_CONT_CON_PROBLEMA
            modelBuilder.Entity<V_DET_CONT_CON_PROBLEMA>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_DET_CONT_CON_PROBLEMA>()
                .Property(e => e.QT_PRODUTO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DET_CONT_CON_PROBLEMA>()
                .Property(e => e.QT_PREPARADO)
                .HasPrecision(12, 3);
            #endregion
            #endregion

            #region >> V_LOG_STOCK_ENVASE
            modelBuilder.Entity<V_LOG_STOCK_ENVASE>()
                 .Property(e => e.NU_LOG_STOCK_ENVASE)
                 .HasPrecision(38, 0);
            #endregion << V_LOG_STOCK_ENVASE

            #region V_CONSULTA_PRE660
            modelBuilder.Entity<V_CONSULTA_PRE660>()
                .Property(e => e.QT_EXPEDIDO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_CONSULTA_PRE660>()
                .Property(e => e.QT_PEDIDO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_CONSULTA_PRE660>()
                .Property(e => e.QT_LIBERADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_CONSULTA_PRE660>()
                .Property(e => e.QT_PREPARADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_CONSULTA_PRE660>()
                .Property(e => e.QT_FACTURADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_CONSULTA_PRE660>()
                .Property(e => e.QT_CROSS_DOCK)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_CONSULTA_PRE660>()
                .Property(e => e.QT_CONTROLADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_CONSULTA_PRE660>()
                .Property(e => e.QT_TRANSFERIDO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_CONSULTA_PRE660>()
                .Property(e => e.QT_ABASTECIDO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_CONSULTA_PRE660>()
                .Property(e => e.QT_CARGADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_CONSULTA_PRE660>()
                .Property(e => e.QT_ANULADO)
                .HasPrecision(38, 0);
            #endregion

            #region V_EVENTO_AGENDA_CERRADAS
            modelBuilder.Entity<V_EVENTO_AGENDA_CERRADAS>()
                .Property(e => e.VL_TEMP_CAMION)
                .HasPrecision(6, 2);
            modelBuilder.Entity<V_EVENTO_AGENDA_CERRADAS>()
                .Property(e => e.VL_TEMP_INICIO_REC)
                .HasPrecision(6, 2);
            modelBuilder.Entity<V_EVENTO_AGENDA_CERRADAS>()
                .Property(e => e.VL_TEMP_MITAD_REC)
                .HasPrecision(6, 2);
            modelBuilder.Entity<V_EVENTO_AGENDA_CERRADAS>()
                .Property(e => e.VL_TEMP_FINAL_REC)
                .HasPrecision(6, 2);
            #endregion

            #region V_SEG_PED_PRE640
            modelBuilder.Entity<V_SEG_PED_PRE640>()
                .Property(e => e.QT_EXPEDIDO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_SEG_PED_PRE640>()
                .Property(e => e.QT_PEDIDO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_SEG_PED_PRE640>()
                .Property(e => e.QT_LIBERADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_SEG_PED_PRE640>()
                .Property(e => e.QT_PREPARADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_SEG_PED_PRE640>()
                .Property(e => e.QT_ANULADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_SEG_PED_PRE640>()
                .Property(e => e.QT_FACTURADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_SEG_PED_PRE640>()
                .Property(e => e.QT_CROSS_DOCK)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_SEG_PED_PRE640>()
                .Property(e => e.QT_CONTROLADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_SEG_PED_PRE640>()
                .Property(e => e.QT_TRANSFERIDO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_SEG_PED_PRE640>()
                .Property(e => e.QT_ABASTECIDO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_SEG_PED_PRE640>()
                .Property(e => e.QT_CARGADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_SEG_PED_PRE640>()
                .Property(e => e.QT_ANULADO)
                .HasPrecision(38, 0);
            #endregion

            #region >> T_CONTAINER
            modelBuilder.Entity<T_CONTAINER>()
                .Property(e => e.PS_TARA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<T_CONTAINER>()
                .Property(e => e.CD_FUNCIONARIO_APERTURA)
                .HasPrecision(38, 0);
            modelBuilder.Entity<T_CONTAINER>()
                .Property(e => e.CD_FUNCIONARIO_CIERRE)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_SEG_PED_PRE640>()
                .Property(e => e.QT_PENDIENTE_LIB)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_SEG_PED_PRE640>()
                .Property(e => e.QT_PENDIENTE_PRE)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_SEG_PED_PRE640>()
                .Property(e => e.QT_PENDIENTE_EXP)
                .HasPrecision(38, 0);
            //modelBuilder.Entity<T_CONTAINER>()
            //    .HasMany(e => e.T_RECEPC_AGENDA_CONTAINER_REL)
            //    .WithOne(e => e.T_CONTAINER)
            //    .HasForeignKey(e => new { e.NU_CONTAINER, e.NU_SEQ_CONTAINER })
            //    .OnDelete(DeleteBehavior.Restrict);
            #endregion << T_CONTAINER

            #region V_PEDIDOS_CAMION_EXP050
            modelBuilder.Entity<V_PEDIDOS_CAMION_EXP050>()
                .Property(e => e.QT_NO_LIBERADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PEDIDOS_CAMION_EXP050>()
                .Property(e => e.QT_NO_PREPARADO)
                .HasPrecision(38, 0);
            #endregion

            #region V_CANT_PREPARADA_WEXP
            modelBuilder.Entity<V_CANT_PREPARADA_WEXP>()
                .Property(e => e.QT_PREPARADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_CANT_PREPARADA_WEXP>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            #endregion

            #region V_EVENTO_SALDO_FAC
            modelBuilder.Entity<V_EVENTO_SALDO_FAC>()
                .Property(e => e.QT_AGENDADA)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_EVENTO_SALDO_FAC>()
                .Property(e => e.QT_RECIBIDA)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_EVENTO_SALDO_FAC>()
                .Property(e => e.QT_RESTANTE)
                .HasPrecision(38, 0);
            #endregion

            #region V_EVENTO_SALDO_REF
            modelBuilder.Entity<V_EVENTO_SALDO_REF>()
                .Property(e => e.QT_REFERENCIA)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_EVENTO_SALDO_REF>()
                .Property(e => e.QT_RECIBIDA)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_EVENTO_SALDO_REF>()
                .Property(e => e.QT_RESTANTE)
                .HasPrecision(38, 0);
            #endregion

            #region T_REPORTE
            //modelBuilder.Entity<T_REPORTE>()
            //    .HasMany(e => e.T_REPORTE_RELACION)
            //    .WithOne(e => e.T_REPORTE)
            //    .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region V_ANULACIONES_PENDIENTES
            modelBuilder.Entity<V_ANULACIONES_PENDIENTES>()
                .Property(e => e.QT_PENDIENTE)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_ANULACIONES_PENDIENTES>()
                .Property(e => e.NU_CONTENEDOR)
                .HasPrecision(38, 0);
            #endregion

            #region >> Archivos adjuntos
            #region >> T_ARCHIVO_ADJUNTO
            //modelBuilder.Entity<T_ARCHIVO_ADJUNTO>()
            //    .HasMany(e => e.T_ARCHIVO_ADJUNTO_VERSION)
            //    .WithOne(e => e.T_ARCHIVO_ADJUNTO)
            //    .HasForeignKey(e => new { e.NU_ARCHIVO_ADJUNTO, e.CD_EMPRESA, e.CD_MANEJO, e.DS_REFERENCIA })
            //    .OnDelete(DeleteBehavior.Restrict);
            #endregion << T_ARCHIVO_ADJUNTO
            #region T_ARCHIVO_MANEJO_DOCUMENTO
            //modelBuilder.Entity<T_ARCHIVO_MANEJO_DOCUMENTO>()
            //    .HasOne(d => d.T_ARCHIVO_DOCUMENTO)
            //    .WithMany(d => d.T_ARCHIVO_MANEJO_DOCUMENTO)
            //    .HasForeignKey(d => d.CD_MANEJO);
            //modelBuilder.Entity<T_ARCHIVO_MANEJO_DOCUMENTO>()
            //    .HasOne(d => d.T_ARCHIVO_MANEJO)
            //    .WithMany(d => d.T_ARCHIVO_MANEJO_DOCUMENTO)
            //    .HasForeignKey(d => d.CD_DOCUMENTO);
            #endregion
            #region >> T_ARCHIVO_MANEJO
            //modelBuilder.Entity<T_ARCHIVO_MANEJO>()
            //    .HasMany(e => e.T_ARCHIVO_ADJUNTO)
            //    .WithOne(e => e.T_ARCHIVO_MANEJO)
            //    .OnDelete(DeleteBehavior.Restrict);
            #endregion << T_ARCHIVO_MANEJO
            #region >> V_CLASIFICACION_STOCK_WSTO640
            modelBuilder.Entity<V_CLASIFICACION_STOCK_WSTO640>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            #endregion << V_CLASIFICACION_STOCK_WSTO640
            #endregion << Archivos adjuntos

            #region V_PLANIFICACION_LIB_WPRE670
            modelBuilder.Entity<V_PLANIFICACION_LIB_WPRE670>()
                .Property(e => e.NU_GENERICO_1)
                .HasPrecision(15, 3);
            modelBuilder.Entity<V_PLANIFICACION_LIB_WPRE670>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_PLANIFICACION_LIB_WPRE670>()
                .Property(e => e.QT_PEDIDO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PLANIFICACION_LIB_WPRE670>()
                .Property(e => e.QT_LIBERADO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PLANIFICACION_LIB_WPRE670>()
                .Property(e => e.QT_ANULADO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PLANIFICACION_LIB_WPRE670>()
                .Property(e => e.QT_PEDIDO_ORIGINAL)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_PLANIFICACION_LIB_WPRE670>()
                .Property(e => e.VL_PORCENTAJE_TOLERANCIA)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PLANIFICACION_LIB_WPRE670>()
                .Property(e => e.NU_GENERICO_1_DET)
                .HasPrecision(15, 3);
            #endregion

            #region T_RECEPCION_FACTURA
            modelBuilder.Entity<T_RECEPCION_FACTURA>()
                .Property(e => e.IM_TOTAL_DIGITADO)
                .HasPrecision(15, 4);
            #endregion

            #region USERDATA_PASS_HISTORY
            modelBuilder.Entity<USERDATA_PASS_HISTORY>()
                .Property(e => e.PASSWORDFORMAT)
                .HasPrecision(38, 0);
            #endregion

            #region V_AGENDA_CALENDARIO_REC710
            modelBuilder.Entity<V_AGENDA_CALENDARIO_REC710>()
                .Property(e => e.QT_UNIDADES_TOTAL)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_AGENDA_CALENDARIO_REC710>()
                .Property(e => e.QT_CAJAS_TOTAL)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_AGENDA_CALENDARIO_REC710>()
                .Property(e => e.QT_PESO_TOTAL)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_AGENDA_CALENDARIO_REC710>()
                .Property(e => e.QT_VOLUMEN_TOTAL)
                .HasPrecision(38, 0);
            #endregion

            #region V_DET_PEDIDO_SAIDA_WPRE101
            modelBuilder.Entity<V_DET_PEDIDO_SAIDA_WPRE101>()
                .Property(e => e.QT_UND_BULTO)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_DET_PEDIDO_SAIDA_WPRE101>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_DET_PEDIDO_SAIDA_WPRE101>()
                .Property(e => e.QT_PEDIDO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DET_PEDIDO_SAIDA_WPRE101>()
                .Property(e => e.QT_LIBERADO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DET_PEDIDO_SAIDA_WPRE101>()
                .Property(e => e.QT_ANULADO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DET_PEDIDO_SAIDA_WPRE101>()
                .Property(e => e.QT_PEDIDO_ORIGINAL)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DET_PEDIDO_SAIDA_WPRE101>()
                .Property(e => e.VL_PORCENTAJE_TOLERANCIA)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_DET_PEDIDO_SAIDA_WPRE101>()
                .Property(e => e.QT_CONV)
                .HasPrecision(38, 0);
            #endregion

            #region V_DETALLE_AGENDA_WREC171
            modelBuilder.Entity<V_DETALLE_AGENDA_WREC171>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_DETALLE_AGENDA_WREC171>()
                .Property(e => e.QT_AGENDADO_ORIGINAL)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DETALLE_AGENDA_WREC171>()
                .Property(e => e.QT_AGENDADO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DETALLE_AGENDA_WREC171>()
                .Property(e => e.PESO_AGENDADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_DETALLE_AGENDA_WREC171>()
                .Property(e => e.VOLUMEN_AGENDADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_DETALLE_AGENDA_WREC171>()
                .Property(e => e.QT_RECIBIDA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_DETALLE_AGENDA_WREC171>()
                .Property(e => e.PESO_RECIBIDO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_DETALLE_AGENDA_WREC171>()
                .Property(e => e.VOLUMEN_RECIBIDO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_DETALLE_AGENDA_WREC171>()
                .Property(e => e.CANT_ETIQUETAS)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_DETALLE_AGENDA_WREC171>()
                .Property(e => e.QT_UND_BULTO)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_DETALLE_AGENDA_WREC171>()
                .Property(e => e.QT_CONV)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_DETALLE_AGENDA_WREC171>()
                .Property(e => e.QT_CONV_AGENDADA)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_DETALLE_AGENDA_WREC171>()
                .Property(e => e.VL_PRECO_VENDA)
                .HasPrecision(16, 2);
            #endregion

            #region V_PEDIDO_SAIDA_WPRE100
            modelBuilder.Entity<V_PEDIDO_SAIDA_WPRE100>()
                .Property(e => e.QT_PEDIDO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PEDIDO_SAIDA_WPRE100>()
                .Property(e => e.QT_LIBERADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PEDIDO_SAIDA_WPRE100>()
                .Property(e => e.QT_ANULADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PEDIDO_SAIDA_WPRE100>()
                .Property(e => e.QT_PENDIENTE_LIB)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PEDIDO_SAIDA_WPRE100>()
                .Property(e => e.QT_PENDIENTE_PREP)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PEDIDO_SAIDA_WPRE100>()
                .Property(e => e.QT_PREPARADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PEDIDO_SAIDA_WPRE100>()
                .Property(e => e.QT_FACTURADO)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PEDIDO_SAIDA_WPRE100>()
                .Property(e => e.QT_EXPEDIDA)
                .HasPrecision(38, 0);
            modelBuilder.Entity<V_PEDIDO_SAIDA_WPRE100>()
                .Property(e => e.QT_PEND_EXPEDIR)
                .HasPrecision(38, 0);
            #endregion

            #region V_RECEPCION_WREC170
            modelBuilder.Entity<V_RECEPCION_WREC170>()
                .Property(e => e.VL_TEMP_CAMION)
                .HasPrecision(6, 2);
            modelBuilder.Entity<V_RECEPCION_WREC170>()
                .Property(e => e.VL_TEMP_INICIO_REC)
                .HasPrecision(6, 2);
            modelBuilder.Entity<V_RECEPCION_WREC170>()
                .Property(e => e.VL_TEMP_MITAD_REC)
                .HasPrecision(6, 2);
            modelBuilder.Entity<V_RECEPCION_WREC170>()
                .Property(e => e.VL_TEMP_FINAL_REC)
                .HasPrecision(6, 2);
            #endregion

            #region V_REAB_PRE680
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_UND_BULTO)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_UNIDADES)
                .HasPrecision(15, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_NECESIDAD)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_NECESIDAD_FINAL)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_PEDIDOS_REAB)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_RESERVA)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_STOCK_CONSIDERADO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_TRANSITO)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_DISOPNIBLE_PRED1)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_PEDIDOS_REAB_PRED1)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_DISOPNIBLE_PRED2)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_PEDIDOS_REAB_PRED2)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_DISOPNIBLE_PRED3)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_PEDIDOS_REAB_PRED3)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_DISOPNIBLE_PRED4)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_PEDIDOS_REAB_PRED4)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_DISOPNIBLE_PRED5)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_PEDIDOS_REAB_PRED5)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_DISOPNIBLE_PRED6)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_PEDIDOS_REAB_PRED6)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_DISOPNIBLE_PRED7)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_PEDIDOS_REAB_PRED7)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_DISOPNIBLE_PRED8)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_PEDIDOS_REAB_PRED8)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_DISOPNIBLE_PRED9)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_PEDIDOS_REAB_PRED9)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_DISOPNIBLE_PRED10)
                .HasPrecision(12, 3);
            modelBuilder.Entity<V_REAB_PRE680>()
                .Property(e => e.QT_PEDIDOS_REAB_PRED10)
                .HasPrecision(12, 3);
            #endregion

            #region T_INTERFAZ
            //modelBuilder.Entity<T_INTERFAZ>()
            //    .HasMany(e => e.T_INTERFAZ_EXTERNA)
            //    .WithOne(e => e.T_INTERFAZ)
            //    .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region T_INTERFAZ_EJECUCION
            //modelBuilder.Entity<T_INTERFAZ_EJECUCION>()
            //    .HasMany(e => e.T_INTERFAZ_EJECUCION_ERROR)
            //    .WithOne(e => e.T_INTERFAZ_EJECUCION)
            //    .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<T_INTERFAZ_EJECUCION>()
            //    .HasMany(e => e.T_INTERFAZ_EJECUCION_DATEXTDET)
            //    .WithOne(e => e.T_INTERFAZ_EJECUCION)
            //    .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<T_INTERFAZ_EJECUCION>()
            //    .HasOne(e => e.T_INTERFAZ_EJECUCION_DATA)
            //    .WithOne(e => e.T_INTERFAZ_EJECUCION)
            //    .HasForeignKey<T_INTERFAZ_EJECUCION_DATA>(d => d.NU_INTERFAZ_EJECUCION);

            //modelBuilder.Entity<T_INTERFAZ_EJECUCION>()
            //    .HasOne(e => e.T_INTERFAZ_EJECUCION_DATEXT)
            //    .WithOne(e => e.T_INTERFAZ_EJECUCION)
            //    .HasForeignKey<T_INTERFAZ_EJECUCION_DATEXT>(d => d.NU_INTERFAZ_EJECUCION);
            #endregion

            #region T_INTERFAZ_EJECUCION_DATA

            if (!string.IsNullOrEmpty(_blobDataType))
            {
                modelBuilder.Entity<T_INTERFAZ_EJECUCION_DATA>()
                    .Property(e => e.DATA)
                    .HasColumnType(_blobDataType);
            }

            #endregion

            #region T_INTERFAZ_EJECUCION_DATEXTDET

            if (!string.IsNullOrEmpty(_blobDataType))
            {
                modelBuilder.Entity<T_INTERFAZ_EJECUCION_DATEXTDET>()
                    .Property(e => e.DATA)
                    .HasColumnType(_blobDataType);
            }

            #endregion

            #region V_INTERFAZ_EJEC_DATA

            if (!string.IsNullOrEmpty(_blobDataType))
            {
                modelBuilder.Entity<V_INTERFAZ_EJEC_DATA>()
                    .Property(e => e.DATA)
                    .HasColumnType(_blobDataType);
            }

            #endregion

            #region V_PRE120_ANULACION_PREPARACION
            modelBuilder.Entity<V_PRE120_ANULACION_PREPARACION>()
                .Property(e => e.NU_PEDIDO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRE120_ANULACION_PREPARACION>()
                .Property(e => e.CD_CLIENTE)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRE120_ANULACION_PREPARACION>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRE120_ANULACION_PREPARACION>()
                .Property(e => e.DS_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRE120_ANULACION_PREPARACION>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<V_PRE120_ANULACION_PREPARACION>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRE120_ANULACION_PREPARACION>()
                .Property(e => e.CD_ENDERECO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRE120_ANULACION_PREPARACION>()
                .Property(e => e.QT_PRODUTO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRE120_ANULACION_PREPARACION>()
                .Property(e => e.ID_AGRUPACION)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRE120_ANULACION_PREPARACION>()
                .Property(e => e.CD_PRODUTO_EMPRESA)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRE120_ANULACION_PREPARACION>()
                .Property(e => e.DS_REDUZIDA)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRE120_ANULACION_PREPARACION>()
                .Property(e => e.CD_MERCADOLOGICO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRE120_ANULACION_PREPARACION>()
                .Property(e => e.AUXQT_PRODUTO_ANULAR)
                .HasPrecision(38, 0);

            modelBuilder.Entity<V_PRE120_ANULACION_PREPARACION>()
                .Property(e => e.TP_ARMADO_EGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRE120_ANULACION_PREPARACION>()
                .Property(e => e.FL_FACTURA_AUTO_COMPLETAR)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRE120_ANULACION_PREPARACION>()
                .Property(e => e.ID_ESPECIFICA_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRE120_ANULACION_PREPARACION>()
                .Property(e => e.QT_PREPARADO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRE120_ANULACION_PREPARACION>()
                .Property(e => e.NU_PREDIO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRE120_ANULACION_PREPARACION>()
                .Property(e => e.ND_ESTADO)
                .IsUnicode(false);
            #endregion

            #region V_REC300_MOTIVO_ALMACENAMIENTO
            modelBuilder.Entity<V_REC300_MOTIVO_ALMACENAMIENTO>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_REC300_MOTIVO_ALMACENAMIENTO>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_REC300_MOTIVO_ALMACENAMIENTO>()
                .Property(e => e.CD_MOTIVO)
                .IsUnicode(false);

            modelBuilder.Entity<V_REC300_MOTIVO_ALMACENAMIENTO>()
                .Property(e => e.QT_REMITO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_REC300_MOTIVO_ALMACENAMIENTO>()
                .Property(e => e.QT_ALMACENADA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_REC300_MOTIVO_ALMACENAMIENTO>()
                .Property(e => e.CD_ENDERECO)
                .IsUnicode(false);

            modelBuilder.Entity<V_REC300_MOTIVO_ALMACENAMIENTO>()
                .Property(e => e.DS_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_REC300_MOTIVO_ALMACENAMIENTO>()
                .Property(e => e.NU_EXTERNO_ETIQUETA)
                .IsUnicode(false);

            modelBuilder.Entity<V_REC300_MOTIVO_ALMACENAMIENTO>()
                .Property(e => e.NU_PREDIO)
                .IsUnicode(false);
            #endregion

            #region V_PEDIDO_ASIG_CAMION
            modelBuilder.Entity<V_PEDIDO_ASIG_CAMION>()
                .Property(e => e.NU_PEDIDO)
                .IsUnicode(false);
            modelBuilder.Entity<V_PEDIDO_ASIG_CAMION>()
                .Property(e => e.CD_CLIENTE)
                .IsUnicode(false);
            #endregion

            #region V_CARGA_WREC220
            modelBuilder.Entity<V_CARGA_WREC220>()
                .Property(e => e.DS_ROTA)
                .IsUnicode(false);

            modelBuilder.Entity<V_CARGA_WREC220>()
                .Property(e => e.NU_PREDIO)
                .IsUnicode(false);
            #endregion

            #region V_CROSS_DOCKING_PEND_REC220
            modelBuilder.Entity<V_CROSS_DOCKING_PEND_REC220>()
               .Property(e => e.NM_EMPRESA)
               .IsUnicode(false);
            #endregion

            #region T_CROSS_DOCK_TEMP
            modelBuilder.Entity<T_CROSS_DOCK_TEMP>()
             .Property(e => e.CD_CLIENTE)
             .IsUnicode(false);

            modelBuilder.Entity<T_CROSS_DOCK_TEMP>()
                .Property(e => e.NU_PEDIDO)
                .IsUnicode(false);

            modelBuilder.Entity<T_CROSS_DOCK_TEMP>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_CROSS_DOCK_TEMP>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<T_CROSS_DOCK_TEMP>()
                .Property(e => e.QT_PRODUTO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_CROSS_DOCK_TEMP>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<T_CROSS_DOCK_TEMP>()
                .Property(e => e.ID_ESPECIFICA_IDENTIFICADOR)
                .IsUnicode(false);
            #endregion

            #region T_TIPO_CROSS_DOCK
            modelBuilder.Entity<T_TIPO_CROSS_DOCK>()
              .Property(e => e.CD_TIPO_CROSS_DOCK)
              .IsUnicode(false);

            modelBuilder.Entity<T_TIPO_CROSS_DOCK>()
                .Property(e => e.DS_TIPO_CROSS_DOCK)
                .IsUnicode(false);

            modelBuilder.Entity<T_TIPO_CROSS_DOCK>()
                .Property(e => e.FL_REQUIERE_CIERRE_AGENDA)
                .IsUnicode(false);

            modelBuilder.Entity<T_TIPO_CROSS_DOCK>()
                .Property(e => e.FL_REQUIERE_LIBERACION_AGENDA)
                .IsUnicode(false);

            modelBuilder.Entity<T_TIPO_CROSS_DOCK>()
                .Property(e => e.FL_ACTIVO)
                .IsUnicode(false);
            #endregion

            #region V_CROSS_DOCK_TEMP_WREC220
            modelBuilder.Entity<V_CROSS_DOCK_TEMP_WREC220>()
    .Property(e => e.CD_CLIENTE)
    .IsUnicode(false);

            modelBuilder.Entity<V_CROSS_DOCK_TEMP_WREC220>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_CROSS_DOCK_TEMP_WREC220>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<V_CROSS_DOCK_TEMP_WREC220>()
                .Property(e => e.QT_PRODUTO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_CROSS_DOCK_TEMP_WREC220>()
                .Property(e => e.NU_PEDIDO)
                .IsUnicode(false);

            modelBuilder.Entity<V_CROSS_DOCK_TEMP_WREC220>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_CROSS_DOCK_TEMP_WREC220>()
                .Property(e => e.ID_ESPECIFICA_IDENTIFICADOR)
                .IsUnicode(false);
            #endregion

            #region V_ETIQUETA_LOTE_WREC270
            modelBuilder.Entity<V_ETIQUETA_LOTE_WREC270>()
              .Property(e => e.NU_EXTERNO_ETIQUETA)
              .IsUnicode(false);

            modelBuilder.Entity<V_ETIQUETA_LOTE_WREC270>()
                .Property(e => e.CD_CLIENTE)
                .IsUnicode(false);

            modelBuilder.Entity<V_ETIQUETA_LOTE_WREC270>()
                .Property(e => e.DS_CLIENTE)
                .IsUnicode(false);

            modelBuilder.Entity<V_ETIQUETA_LOTE_WREC270>()
                .Property(e => e.NU_PREDIO)
                .IsUnicode(false);
            #endregion

            #region V_ETIQUETA_PRE_SEP_WREC270
            modelBuilder.Entity<V_ETIQUETA_PRE_SEP_WREC270>()
               .Property(e => e.NU_EXTERNO_ETIQUETA)
               .IsUnicode(false);

            modelBuilder.Entity<V_ETIQUETA_PRE_SEP_WREC270>()
                .Property(e => e.CD_CLIENTE)
                .IsUnicode(false);

            modelBuilder.Entity<V_ETIQUETA_PRE_SEP_WREC270>()
                .Property(e => e.CD_ENDERECO)
                .IsUnicode(false);

            modelBuilder.Entity<V_ETIQUETA_PRE_SEP_WREC270>()
                .Property(e => e.DS_CLIENTE)
                .IsUnicode(false);

            modelBuilder.Entity<V_ETIQUETA_PRE_SEP_WREC270>()
                .Property(e => e.QT_PRODUTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<V_ETIQUETA_PRE_SEP_WREC270>()
                .Property(e => e.ID_CTRL_ACEPTADO)
                .IsUnicode(false);

            modelBuilder.Entity<V_ETIQUETA_PRE_SEP_WREC270>()
                .Property(e => e.NU_PREDIO)
                .IsUnicode(false);
            #endregion

            #region V_AGENDAS_WREC270
            modelBuilder.Entity<V_AGENDAS_WREC270>()
              .Property(e => e.NU_DOCUMENTO)
              .IsUnicode(false);

            modelBuilder.Entity<V_AGENDAS_WREC270>()
                .Property(e => e.NU_DOCUMENTO_REAL)
                .IsUnicode(false);

            modelBuilder.Entity<V_AGENDAS_WREC270>()
                .Property(e => e.TP_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_AGENDAS_WREC270>()
                .Property(e => e.ID_ESTADO)
                .IsUnicode(false);
            #endregion

            #region V_FACTU_SIN_EXPEDIR_REC270

            modelBuilder.Entity<V_FACTU_SIN_EXPEDIR_REC270>()
             .Property(e => e.CD_CLIENTE)
             .IsUnicode(false);
            #endregion

            #region V_PED_CAR_EXP_REC270
            modelBuilder.Entity<V_PED_CAR_EXP_REC270>()
              .Property(e => e.NU_PEDIDO)
              .IsUnicode(false);

            modelBuilder.Entity<V_PED_CAR_EXP_REC270>()
                .Property(e => e.CD_CLIENTE)
                .IsUnicode(false);
            #endregion

            #region T_DOCUMENTO_ANU_PREP_RESERVA

            modelBuilder.Entity<T_DOCUMENTO_ANU_PREP_RESERVA>()
               .Property(e => e.CD_PRODUTO)
               .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_ANU_PREP_RESERVA>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<T_DOCUMENTO_ANU_PREP_RESERVA>()
                .Property(e => e.ID_ESPECIFICA_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_ANU_PREP_RESERVA>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_ANU_PREP_RESERVA>()
                .Property(e => e.QT_ANULAR)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_DOCUMENTO_ANU_PREP_RESERVA>()
                .Property(e => e.ID_ANULACION)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_ANU_PREP_RESERVA>()
                .Property(e => e.ID_ESTADO)
                .IsUnicode(false);

            #endregion

            #region T_DOCUMENTO_PRDC_ENTRADA

            modelBuilder.Entity<T_DOCUMENTO_PRDC_ENTRADA>()
              .HasKey(e => new { e.NU_PREPARACION, e.NU_PEDIDO, e.NU_CONTENEDOR, e.CD_EMPRESA, e.CD_PRODUTO, e.CD_FAIXA, e.NU_IDENTIFICADOR });

            modelBuilder.Entity<T_DOCUMENTO_PRDC_ENTRADA>()
              .Property(e => e.NU_PEDIDO)
              .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_PRDC_ENTRADA>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_PRDC_ENTRADA>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<T_DOCUMENTO_PRDC_ENTRADA>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_PRDC_ENTRADA>()
                .Property(e => e.QT_PRODUTO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_DOCUMENTO_PRDC_ENTRADA>()
                .Property(e => e.VL_DATO_AUDITORIA)
                .IsUnicode(false);

            #endregion

            #region T_DOCUMENTO_AGRUPADOR

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR>()
             .HasKey(e => new { e.NU_AGRUPADOR, e.TP_AGRUPADOR });

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR>()
             .Property(e => e.NU_AGRUPADOR)
             .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR>()
                .Property(e => e.TP_AGRUPADOR)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR>()
                .Property(e => e.ID_ESTADO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR>()
                .Property(e => e.NU_LACRE)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR>()
                .Property(e => e.VL_TOTAL)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR>()
                .Property(e => e.QT_PESO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR>()
                .Property(e => e.VL_DATO_AUDITORIA)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR>()
                .Property(e => e.QT_PESO_LIQUIDO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR>()
                .Property(e => e.DS_MOTORISTA)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR>()
                .Property(e => e.DS_PLACA)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR>()
                .Property(e => e.ANEXO1)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR>()
                .Property(e => e.ANEXO2)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR>()
                .Property(e => e.ANEXO3)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR>()
                .Property(e => e.ANEXO4)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR>()
               .Property(e => e.NU_PREDIO)
               .IsUnicode(false);
            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR>()
                .Property(e => e.DS_MOTIVO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR>()
                .HasOne(e => e.T_TRANSPORTADORA)
                .WithMany(e => e.T_DOCUMENTO_AGRUPADOR)
                .HasForeignKey(e => e.CD_TRANSPORTADORA)
                .IsRequired();

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR>()
                .HasOne(e => e.T_TIPO_VEICULO)
                .WithMany(e => e.T_DOCUMENTO_AGRUPADOR)
                .HasForeignKey(e => e.CD_TIPO_VEICULO)
                .IsRequired();

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR>()
               .HasOne(e => e.T_DOCUMENTO_AGRUPADOR_TIPO)
               .WithMany(e => e.T_DOCUMENTO_AGRUPADOR)
               .HasForeignKey(e => e.TP_AGRUPADOR)
               .IsRequired();

            #endregion

            #region T_DOCUMENTO_AGRUPADOR_TIPO

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR_TIPO>()
                .Property(e => e.TP_AGRUPADOR)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR_TIPO>()
                .Property(e => e.DS_TIPO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR_TIPO>()
                .Property(e => e.FL_HABILITADO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR_TIPO>()
                .Property(e => e.TP_OPERACION)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR_TIPO>()
                .HasMany(e => e.T_DOCUMENTO_AGRUPADOR)
                .WithOne(e => e.T_DOCUMENTO_AGRUPADOR_TIPO)
                .HasForeignKey(e => e.TP_AGRUPADOR)
                .IsRequired();

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR_TIPO>()
                .HasMany(e => e.T_DOCUMENTO_AGRUPADOR_GRUPO)
                .WithOne(e => e.T_DOCUMENTO_AGRUPADOR_TIPO)
                .HasForeignKey(e => e.TP_AGRUPADOR)
                .IsRequired();

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR_TIPO>()
                .Property(e => e.FL_MANEJA_PREDIO)
                .IsUnicode(false);

            #endregion

            #region T_DOCUMENTO_AGRUPADOR_GRUPO

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR_GRUPO>()
             .HasKey(e => new { e.TP_AGRUPADOR, e.TP_DOCUMENTO });

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR_GRUPO>()
                .Property(e => e.TP_AGRUPADOR)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AGRUPADOR_GRUPO>()
                .Property(e => e.TP_DOCUMENTO)
                .IsUnicode(false);

            #endregion

            #region V_DOCUMENTO_SALDO_DETALLE

            modelBuilder.Entity<V_DOCUMENTO_SALDO_DETALLE>()
                   .HasKey(e => new { e.TP_DOCUMENTO, e.NU_DOCUMENTO, e.CD_PRODUTO, e.CD_EMPRESA, e.CD_FAIXA, e.NU_IDENTIFICADOR });

            modelBuilder.Entity<V_DOCUMENTO_SALDO_DETALLE>()
                   .Property(e => e.TP_DUA)
                   .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_SALDO_DETALLE>()
                    .Property(e => e.NU_DUA)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_SALDO_DETALLE>()
                    .Property(e => e.NU_AGRUPADOR)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_SALDO_DETALLE>()
                    .Property(e => e.TP_AGRUPADOR)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_SALDO_DETALLE>()
                    .Property(e => e.NU_DOCUMENTO_INGRESO_DUA)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_SALDO_DETALLE>()
                    .Property(e => e.TP_DOCUMENTO_INGRESO_DUA)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_SALDO_DETALLE>()
                    .Property(e => e.ID_ESTADO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_SALDO_DETALLE>()
                    .Property(e => e.VL_ARBITRAJE)
                    .HasPrecision(38, 0);

            modelBuilder.Entity<V_DOCUMENTO_SALDO_DETALLE>()
                    .Property(e => e.ID_MANUAL)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_SALDO_DETALLE>()
                    .Property(e => e.CD_PRODUTO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_SALDO_DETALLE>()
                    .Property(e => e.CD_FAIXA)
                    .HasPrecision(9, 3);

            modelBuilder.Entity<V_DOCUMENTO_SALDO_DETALLE>()
                    .Property(e => e.NU_IDENTIFICADOR)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_SALDO_DETALLE>()
                    .Property(e => e.NU_DOCUMENTO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_SALDO_DETALLE>()
                    .Property(e => e.TP_DOCUMENTO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_SALDO_DETALLE>()
                    .Property(e => e.QT_INGRESADA)
                    .HasPrecision(38, 0);

            modelBuilder.Entity<V_DOCUMENTO_SALDO_DETALLE>()
                    .Property(e => e.QT_RESERVADA)
                    .HasPrecision(38, 0);

            modelBuilder.Entity<V_DOCUMENTO_SALDO_DETALLE>()
                    .Property(e => e.QT_DESAFECTADA)
                    .HasPrecision(38, 0);

            modelBuilder.Entity<V_DOCUMENTO_SALDO_DETALLE>()
                    .Property(e => e.ID_DISPONIBLE)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_SALDO_DETALLE>()
                    .Property(e => e.QT_MERCADERIA)
                    .HasPrecision(38, 0);

            modelBuilder.Entity<V_DOCUMENTO_SALDO_DETALLE>()
                    .Property(e => e.QT_DISPONIBLE)
                    .HasPrecision(38, 0);

            #endregion

            #region T_DOCUMENTO_TIPO_EXTERNO

            modelBuilder.Entity<T_DOCUMENTO_TIPO_EXTERNO>()
                    .HasKey(e => new { e.TP_DOCUMENTO_EXTERNO, e.TP_DOCUMENTO });

            modelBuilder.Entity<T_DOCUMENTO_TIPO_EXTERNO>()
                    .Property(e => e.TP_DOCUMENTO_EXTERNO)
                    .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_TIPO_EXTERNO>()
                    .Property(e => e.TP_DOCUMENTO)
                    .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_TIPO_EXTERNO>()
                    .Property(e => e.DS_DOCUMENTO_EXTERNO)
                    .IsUnicode(false);

            #endregion

            #region T_DET_DOCU_EGRESO_CAMBIO

            modelBuilder.Entity<T_DET_DOCU_EGRESO_CAMBIO>()
                .HasKey(e => new { e.NU_DOCUMENTO, e.TP_DOCUMENTO, e.NU_SECUENCIA });

            modelBuilder.Entity<T_DET_DOCU_EGRESO_CAMBIO>()
               .Property(e => e.NU_DOCUMENTO)
               .IsUnicode(false);

            modelBuilder.Entity<T_DET_DOCU_EGRESO_CAMBIO>()
                .Property(e => e.TP_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DET_DOCU_EGRESO_CAMBIO>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DET_DOCU_EGRESO_CAMBIO>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<T_DET_DOCU_EGRESO_CAMBIO>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<T_DET_DOCU_EGRESO_CAMBIO>()
                .Property(e => e.TP_DOCUMENTO_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DET_DOCU_EGRESO_CAMBIO>()
                .Property(e => e.NU_DOCUMENTO_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DET_DOCU_EGRESO_CAMBIO>()
                .Property(e => e.QT_DESAFECTADA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_DET_DOCU_EGRESO_CAMBIO>()
                .Property(e => e.VL_FOB)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_DET_DOCU_EGRESO_CAMBIO>()
                .Property(e => e.VL_CIF)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_DET_DOCU_EGRESO_CAMBIO>()
                .Property(e => e.QT_DESCARGADA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_DET_DOCU_EGRESO_CAMBIO>()
                .Property(e => e.VL_DATO_AUDITORIA)
                .IsUnicode(false);

            modelBuilder.Entity<T_DET_DOCU_EGRESO_CAMBIO>()
                .Property(e => e.VL_TRIBUTO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_DET_DOCU_EGRESO_CAMBIO>()
                .Property(e => e.NU_PROCESO)
                .IsUnicode(false);

            #endregion

            #region T_DET_DOCU_EGRESO_RESERV

            modelBuilder.Entity<T_DET_DOCU_EGRESO_RESERV>()
                .HasKey(e => new { e.NU_DOCUMENTO, e.TP_DOCUMENTO, e.NU_SECUENCIA, e.NU_DOCUMENTO_INGRESO, e.TP_DOCUMENTO_INGRESO, e.NU_PREPARACION, e.CD_EMPRESA, e.CD_PRODUTO, e.CD_FAIXA, e.NU_IDENTIFICADOR_PICKING_DET, e.NU_IDENTIFICADOR });

            #endregion

            #region LT_CAMBIO_DOCUMENTO_DET

            modelBuilder.Entity<LT_CAMBIO_DOCUMENTO_DET>()
                .HasKey(e => new { e.CD_PRODUTO, e.CD_FAIXA, e.NU_IDENTIFICADOR, e.CD_EMPRESA, e.NU_DOCUMENTO, e.TP_DOCUMENTO, e.NU_DOCUMENTO_CAMBIO, e.TP_DOCUMENTO_CAMBIO });

            modelBuilder.Entity<LT_CAMBIO_DOCUMENTO_DET>()
               .Property(e => e.CD_PRODUTO)
               .IsUnicode(false);

            modelBuilder.Entity<LT_CAMBIO_DOCUMENTO_DET>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<LT_CAMBIO_DOCUMENTO_DET>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<LT_CAMBIO_DOCUMENTO_DET>()
                .Property(e => e.NU_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<LT_CAMBIO_DOCUMENTO_DET>()
                .Property(e => e.TP_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<LT_CAMBIO_DOCUMENTO_DET>()
                .Property(e => e.NU_DOCUMENTO_CAMBIO)
                .IsUnicode(false);

            modelBuilder.Entity<LT_CAMBIO_DOCUMENTO_DET>()
                .Property(e => e.TP_DOCUMENTO_CAMBIO)
                .IsUnicode(false);

            #endregion

            #region T_CAMBIO_DOCUMENTO_DET

            modelBuilder.Entity<T_CAMBIO_DOCUMENTO_DET>()
             .Property(e => e.ID_PROCESADO)
             .IsUnicode(false);

            modelBuilder.Entity<T_CAMBIO_DOCUMENTO_DET>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_CAMBIO_DOCUMENTO_DET>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<T_CAMBIO_DOCUMENTO_DET>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<T_CAMBIO_DOCUMENTO_DET>()
                .Property(e => e.NU_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_CAMBIO_DOCUMENTO_DET>()
                .Property(e => e.TP_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_CAMBIO_DOCUMENTO_DET>()
                .Property(e => e.NU_DOCUMENTO_CAMBIO)
                .IsUnicode(false);

            modelBuilder.Entity<T_CAMBIO_DOCUMENTO_DET>()
                .Property(e => e.TP_DOCUMENTO_CAMBIO)
                .IsUnicode(false);

            modelBuilder.Entity<T_CAMBIO_DOCUMENTO_DET>()
                .Property(e => e.QT_CAMBIO)
                .HasPrecision(12, 3);

            #endregion

            #region T_PRDC_CUENTA_CAMBIO_DOC

            modelBuilder.Entity<T_PRDC_CUENTA_CAMBIO_DOC>()
                .HasKey(e => new { e.NU_DOCUMENTO_EGRESO, e.NU_DOCUMENTO_EGRESO_PRDC, e.TP_DOCUMENTO_EGRESO_PRDC, e.TP_DOCUMENTO_INGRESO, e.NU_DOCUMENTO_INGRESO, e.TP_DOCUMENTO_INGRESO_ORIGINAL, e.NU_DOCUMENTO_INGRESO_ORIGINAL, e.CD_EMPRESA, e.CD_PRODUTO, e.CD_FAIXA, e.NU_IDENTIFICADOR, e.CD_PRODUTO_PRODUCIDO, e.NU_NIVEL });

            modelBuilder.Entity<T_PRDC_CUENTA_CAMBIO_DOC>()
                .Property(e => e.NU_DOCUMENTO_EGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_CUENTA_CAMBIO_DOC>()
                .Property(e => e.NU_DOCUMENTO_EGRESO_PRDC)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_CUENTA_CAMBIO_DOC>()
                .Property(e => e.TP_DOCUMENTO_EGRESO_PRDC)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_CUENTA_CAMBIO_DOC>()
                .Property(e => e.TP_DOCUMENTO_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_CUENTA_CAMBIO_DOC>()
                .Property(e => e.NU_DOCUMENTO_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_CUENTA_CAMBIO_DOC>()
                .Property(e => e.TP_DOCUMENTO_INGRESO_ORIGINAL)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_CUENTA_CAMBIO_DOC>()
                .Property(e => e.NU_DOCUMENTO_INGRESO_ORIGINAL)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_CUENTA_CAMBIO_DOC>()
                .Property(e => e.QT_DECLARADA_ORIGINAL)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_PRDC_CUENTA_CAMBIO_DOC>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_CUENTA_CAMBIO_DOC>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<T_PRDC_CUENTA_CAMBIO_DOC>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_CUENTA_CAMBIO_DOC>()
                .Property(e => e.QT_MOVIMIENTO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_PRDC_CUENTA_CAMBIO_DOC>()
                .Property(e => e.CD_PRODUTO_PRODUCIDO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_CUENTA_CAMBIO_DOC>()
                .Property(e => e.NU_NIVEL)
                .HasPrecision(38, 0);

            modelBuilder.Entity<T_PRDC_CUENTA_CAMBIO_DOC>()
                .Property(e => e.TP_DOCUMENTO_CAMBIO)
                .IsUnicode(false);

            modelBuilder.Entity<T_PRDC_CUENTA_CAMBIO_DOC>()
                .Property(e => e.NU_DOCUMENTO_CAMBIO)
                .IsUnicode(false);

            #endregion

            #region V_PRDC_SALDO_INGRESADO

            modelBuilder.Entity<V_PRDC_SALDO_INGRESADO>()
                .HasKey(e => new { e.NU_DOCUMENTO_EGR, e.TP_DOCUMENTO_EGR, e.NU_DOCUMENTO_ING, e.TP_DOCUMENTO_ING, e.NU_PRDC_INGRESO });

            modelBuilder.Entity<V_PRDC_SALDO_INGRESADO>()
                .Property(e => e.NU_DOCUMENTO_EGR)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_SALDO_INGRESADO>()
                .Property(e => e.TP_DOCUMENTO_EGR)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_SALDO_INGRESADO>()
                .Property(e => e.NU_DOCUMENTO_ING)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_SALDO_INGRESADO>()
                .Property(e => e.TP_DOCUMENTO_ING)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_SALDO_INGRESADO>()
                .Property(e => e.NU_PRDC_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_SALDO_INGRESADO>()
                .Property(e => e.QT_INGRESADO)
                .HasPrecision(38, 0);

            #endregion

            #region V_DET_DOCUMENTO_RESERVA

            modelBuilder.Entity<V_DET_DOCUMENTO_RESERVA>()
                .HasKey(e => new { e.NU_IDENTIFICADOR, e.CD_PRODUTO, e.CD_EMPRESA, e.NU_DOCUMENTO, e.TP_DOCUMENTO, e.DT_ADDROW, e.ID_ESTADO, e.CD_FAIXA });

            modelBuilder.Entity<V_DET_DOCUMENTO_RESERVA>()
                .Property(e => e.TP_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO_RESERVA>()
                .Property(e => e.NU_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO_RESERVA>()
                .Property(e => e.NU_DOCUMENTO_FORMAT)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO_RESERVA>()
                .Property(e => e.TP_DOCUMENTO_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO_RESERVA>()
                .Property(e => e.NU_DOCUMENTO_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO_RESERVA>()
                .Property(e => e.ID_ESTADO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO_RESERVA>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO_RESERVA>()
                .Property(e => e.CD_NAM)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO_RESERVA>()
                .Property(e => e.DS_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO_RESERVA>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<V_DET_DOCUMENTO_RESERVA>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO_RESERVA>()
                .Property(e => e.NM_EMPRESA)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO_RESERVA>()
                .Property(e => e.TP_DUA_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO_RESERVA>()
                .Property(e => e.NU_DUA_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO_RESERVA>()
                .Property(e => e.NU_AGRUPADOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO_RESERVA>()
                .Property(e => e.TP_AGRUPADOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO_RESERVA>()
                .Property(e => e.VL_MERCADERIA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_DET_DOCUMENTO_RESERVA>()
                .Property(e => e.QT_RESERVADA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_DET_DOCUMENTO_RESERVA>()
                .Property(e => e.QT_DOCUMENTO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_DET_DOCUMENTO_RESERVA>()
                .Property(e => e.VL_CIF_INGRESO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_DET_DOCUMENTO_RESERVA>()
                .Property(e => e.VL_TRIBUTO)
                .HasPrecision(12, 3);

            #endregion

            #region V_PRDC_SALDO_CC

            modelBuilder.Entity<V_PRDC_SALDO_CC>()
               .HasKey(e => new { e.NU_DOCUMENTO, e.TP_DOCUMENTO, e.CD_EMPRESA, e.CD_PRODUTO, e.CD_FAIXA, e.NU_IDENTIFICADOR });

            modelBuilder.Entity<V_PRDC_SALDO_CC>()
               .Property(e => e.NU_DOCUMENTO)
               .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_SALDO_CC>()
                .Property(e => e.TP_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_SALDO_CC>()
                .Property(e => e.TP_DOCUMENTO_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_SALDO_CC>()
                .Property(e => e.NU_DOCUMENTO_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_SALDO_CC>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_SALDO_CC>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<V_PRDC_SALDO_CC>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_SALDO_CC>()
                .Property(e => e.QT_SALDO)
                .HasPrecision(38, 0);

            #endregion

            #region

            modelBuilder.Entity<V_CAMBIO_DOC_DOC401>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC401>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC401>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC401>()
                .Property(e => e.NU_DOCUMENTO_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC401>()
                .Property(e => e.TP_DOCUMENTO_INGRESSO)
                .IsUnicode(false);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC401>()
                .Property(e => e.QT_INGRESADA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC401>()
                .Property(e => e.QT_RESERVADA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC401>()
                .Property(e => e.QT_DESAFECTADA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC401>()
                .Property(e => e.QT_SALDO)
                .HasPrecision(15, 3);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC401>()
                .Property(e => e.NU_DOC)
                .IsUnicode(false);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC401>()
                .Property(e => e.QT_MOVIMIENTO)
                .HasPrecision(15, 3);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC401>()
                .Property(e => e.QT_NACIONALIZADA)
                .HasPrecision(15, 3);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC401>()
                .Property(e => e.QT_EXPEDIDO)
                .HasPrecision(15, 3);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC401>()
                .Property(e => e.TP_DOCUMENTO_CAMBIO)
                .IsUnicode(false);

            #endregion

            #region V_KIT200_DET_PEDIDO_SAIDA

            modelBuilder.Entity<V_KIT200_DET_PEDIDO_SAIDA>()
                .HasKey(e => new { e.NU_PEDIDO, e.CD_CLIENTE, e.CD_EMPRESA, e.CD_PRODUTO, e.CD_FAIXA, e.NU_IDENTIFICADOR });

            modelBuilder.Entity<V_KIT200_DET_PEDIDO_SAIDA>()
                .Property(e => e.NU_PEDIDO)
                .IsUnicode(false);

            modelBuilder.Entity<V_KIT200_DET_PEDIDO_SAIDA>()
                .Property(e => e.CD_CLIENTE)
                .IsUnicode(false);

            modelBuilder.Entity<V_KIT200_DET_PEDIDO_SAIDA>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_KIT200_DET_PEDIDO_SAIDA>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<V_KIT200_DET_PEDIDO_SAIDA>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_KIT200_DET_PEDIDO_SAIDA>()
                .Property(e => e.FL_SEMIACABADO)
                .IsUnicode(false);

            modelBuilder.Entity<V_KIT200_DET_PEDIDO_SAIDA>()
                .Property(e => e.NU_PREDIO)
                .IsUnicode(false);

            modelBuilder.Entity<V_KIT200_DET_PEDIDO_SAIDA>()
                .Property(e => e.FL_CONSUMIBLE)
                .IsUnicode(false);

            #endregion

            #region V_DOCUMENTO_AJUSTE_STOCK

            modelBuilder.Entity<V_DOCUMENTO_AJUSTE_STOCK>()
                .Property(e => e.NM_EMPRESA)
                .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_AJUSTE_STOCK>()
                .Property(e => e.CD_CGC_EMPRESA)
                .IsUnicode(false);

            #endregion

            #region V_DOCUMENTO_MOV_MERC_DOC350

            modelBuilder.Entity<V_DOCUMENTO_MOV_MERC_DOC350>()
                   .HasKey(e => new { e.NU_AGRUPADOR, e.TP_AGRUPADOR, e.NU_DOCUMENTO, e.TP_DOCUMENTO });

            modelBuilder.Entity<V_DOCUMENTO_MOV_MERC_DOC350>()
                   .Property(e => e.NU_AGRUPADOR)
                   .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_MOV_MERC_DOC350>()
                    .Property(e => e.TP_AGRUPADOR)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_MOV_MERC_DOC350>()
                    .Property(e => e.NU_DOCUMENTO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_MOV_MERC_DOC350>()
                    .Property(e => e.TP_DOCUMENTO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_MOV_MERC_DOC350>()
                    .Property(e => e.QT_VOLUMEN)
                    .HasPrecision(12, 3);

            modelBuilder.Entity<V_DOCUMENTO_MOV_MERC_DOC350>()
                    .Property(e => e.QT_PESO_BRUTO)
                    .HasPrecision(12, 3);

            modelBuilder.Entity<V_DOCUMENTO_MOV_MERC_DOC350>()
                    .Property(e => e.QT_PESO_LIQUIDO)
                    .HasPrecision(12, 3);

            modelBuilder.Entity<V_DOCUMENTO_MOV_MERC_DOC350>()
                    .Property(e => e.VL_DOCUMENTO)
                    .HasPrecision(38, 0);

            modelBuilder.Entity<V_DOCUMENTO_MOV_MERC_DOC350>()
                    .Property(e => e.VL_DOCUMENTO_CIF)
                    .HasPrecision(38, 0);

            modelBuilder.Entity<V_DOCUMENTO_MOV_MERC_DOC350>()
                    .Property(e => e.VL_TRIBUTO)
                    .HasPrecision(38, 0);

            #endregion

            #region V_CAMBIO_DOC_DOC500

            modelBuilder.Entity<V_CAMBIO_DOC_DOC500>()
             .HasKey(e => new { e.NU_DOCUMENTO_EGRESO, e.NU_DOCUMENTO_EGRESO_PRDC, e.TP_DOCUMENTO_EGRESO_PRDC, e.TP_DOCUMENTO_INGRESO, e.NU_DOCUMENTO_INGRESO, e.TP_DOCUMENTO_INGRESO_ORIGINAL, e.NU_DOCUMENTO_INGRESO_ORIGINAL, e.CD_EMPRESA, e.CD_PRODUTO, e.CD_FAIXA, e.NU_IDENTIFICADOR, e.CD_PRODUTO_PRODUCIDO, e.NU_NIVEL });

            modelBuilder.Entity<V_CAMBIO_DOC_DOC500>()
             .Property(e => e.NU_DOCUMENTO_EGRESO)
             .IsUnicode(false);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC500>()
                .Property(e => e.NU_DOCUMENTO_EGRESO_PRDC)
                .IsUnicode(false);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC500>()
                .Property(e => e.TP_DOCUMENTO_EGRESO_PRDC)
                .IsUnicode(false);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC500>()
                .Property(e => e.TP_DOCUMENTO_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC500>()
                .Property(e => e.NU_DOCUMENTO_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC500>()
                .Property(e => e.TP_DOCUMENTO_INGRESO_ORIGINAL)
                .IsUnicode(false);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC500>()
                .Property(e => e.NU_DOCUMENTO_INGRESO_ORIGINAL)
                .IsUnicode(false);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC500>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC500>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC500>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC500>()
                .Property(e => e.CD_PRODUTO_PRODUCIDO)
                .IsUnicode(false);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC500>()
                .Property(e => e.NU_NIVEL)
                .HasPrecision(38, 0);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC500>()
                .Property(e => e.NU_DOC)
                .IsUnicode(false);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC500>()
                .Property(e => e.NU_DOC_CONF)
                .IsUnicode(false);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC500>()
                .Property(e => e.TP_DOC)
                .IsUnicode(false);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC500>()
                .Property(e => e.TP_DOCUMENTO_CAMBIO)
                .IsUnicode(false);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC500>()
                .Property(e => e.NU_DOCUMENTO_CAMBIO)
                .IsUnicode(false);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC500>()
                .Property(e => e.QT_MOVIMIENTO)
                .HasPrecision(12, 3);

            #endregion

            #region V_CONSULTA_IP_CUENTA_DOC501

            modelBuilder.Entity<V_CONSULTA_IP_CUENTA_DOC501>()
               .HasKey(e => new { e.NU_DOCUMENTO_EGRESO, e.NU_DOCUMENTO_EGRESO_PRDC, e.TP_DOCUMENTO_EGRESO_PRDC, e.TP_DOCUMENTO_INGRESO, e.NU_DOCUMENTO_INGRESO, e.TP_DOCUMENTO_INGRESO_ORIGINAL, e.NU_DOCUMENTO_INGRESO_ORIGINAL, e.CD_EMPRESA, e.CD_PRODUTO, e.CD_FAIXA, e.NU_IDENTIFICADOR, e.CD_PRODUTO_PRODUCIDO, e.NU_NIVEL });

            modelBuilder.Entity<V_CONSULTA_IP_CUENTA_DOC501>()
               .Property(e => e.NU_DOCUMENTO_EGRESO)
               .IsUnicode(false);

            modelBuilder.Entity<V_CONSULTA_IP_CUENTA_DOC501>()
                .Property(e => e.NU_DOCUMENTO_EGRESO_PRDC)
                .IsUnicode(false);

            modelBuilder.Entity<V_CONSULTA_IP_CUENTA_DOC501>()
                .Property(e => e.TP_DOCUMENTO_EGRESO_PRDC)
                .IsUnicode(false);

            modelBuilder.Entity<V_CONSULTA_IP_CUENTA_DOC501>()
                .Property(e => e.TP_DOCUMENTO_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<V_CONSULTA_IP_CUENTA_DOC501>()
                .Property(e => e.NU_DOCUMENTO_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<V_CONSULTA_IP_CUENTA_DOC501>()
                .Property(e => e.TP_DOCUMENTO_INGRESO_ORIGINAL)
                .IsUnicode(false);

            modelBuilder.Entity<V_CONSULTA_IP_CUENTA_DOC501>()
                .Property(e => e.NU_DOCUMENTO_INGRESO_ORIGINAL)
                .IsUnicode(false);

            modelBuilder.Entity<V_CONSULTA_IP_CUENTA_DOC501>()
                .Property(e => e.QT_DECLARADA_ORIGINAL)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_CONSULTA_IP_CUENTA_DOC501>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_CONSULTA_IP_CUENTA_DOC501>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<V_CONSULTA_IP_CUENTA_DOC501>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_CONSULTA_IP_CUENTA_DOC501>()
                .Property(e => e.QT_MOVIMIENTO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_CONSULTA_IP_CUENTA_DOC501>()
                .Property(e => e.CD_PRODUTO_PRODUCIDO)
                .IsUnicode(false);

            modelBuilder.Entity<V_CONSULTA_IP_CUENTA_DOC501>()
                .Property(e => e.NU_NIVEL)
                .HasPrecision(38, 0);

            #endregion

            #region V_DOCUMENTO_AGRUPABLE_DOC330

            modelBuilder.Entity<V_DOCUMENTO_AGRUPABLE_DOC330>()
                    .HasKey(e => new { e.NU_DOCUMENTO, e.TP_DOCUMENTO });

            modelBuilder.Entity<V_DOCUMENTO_AGRUPABLE_DOC330>()
                    .Property(e => e.TP_DOCUMENTO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_AGRUPABLE_DOC330>()
                    .Property(e => e.NU_DOCUMENTO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_AGRUPABLE_DOC330>()
                    .Property(e => e.DS_DOCUMENTO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_AGRUPABLE_DOC330>()
                    .Property(e => e.ID_ESTADO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_AGRUPABLE_DOC330>()
                    .Property(e => e.NU_AGRUPADOR)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_AGRUPABLE_DOC330>()
                    .Property(e => e.TP_AGRUPADOR)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_AGRUPABLE_DOC330>()
                    .Property(e => e.NM_EMPRESA)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_AGRUPABLE_DOC330>()
                    .Property(e => e.DS_CLIENTE)
                    .IsUnicode(false);

            #endregion

            #region V_DOCUMENTO_AGRUPADOR_DOC320

            modelBuilder.Entity<V_DOCUMENTO_AGRUPADOR_DOC320>()
                    .HasKey(e => new { e.NU_AGRUPADOR, e.TP_AGRUPADOR });

            modelBuilder.Entity<V_DOCUMENTO_AGRUPADOR_DOC320>()
                    .Property(e => e.NU_AGRUPADOR)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_AGRUPADOR_DOC320>()
                    .Property(e => e.TP_AGRUPADOR)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_AGRUPADOR_DOC320>()
                    .Property(e => e.ID_ESTADO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_AGRUPADOR_DOC320>()
                    .Property(e => e.NU_LACRE)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_AGRUPADOR_DOC320>()
                    .Property(e => e.VL_TOTAL)
                    .HasPrecision(12, 3);

            modelBuilder.Entity<V_DOCUMENTO_AGRUPADOR_DOC320>()
                    .Property(e => e.QT_PESO)
                    .HasPrecision(12, 3);

            modelBuilder.Entity<V_DOCUMENTO_AGRUPADOR_DOC320>()
                    .Property(e => e.QT_PESO_LIQUIDO)
                    .HasPrecision(12, 3);

            modelBuilder.Entity<V_DOCUMENTO_AGRUPADOR_DOC320>()
                    .Property(e => e.DS_PLACA)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_AGRUPADOR_DOC320>()
                    .Property(e => e.DS_MOTORISTA)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_AGRUPADOR_DOC320>()
                    .Property(e => e.DS_TIPO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_AGRUPADOR_DOC320>()
                    .Property(e => e.DS_TRANSPORTADORA)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_AGRUPADOR_DOC320>()
                    .Property(e => e.DS_TIPO_VEICULO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_AGRUPADOR_DOC320>()
                    .Property(e => e.ANEXO1)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_AGRUPADOR_DOC320>()
                    .Property(e => e.ANEXO2)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_AGRUPADOR_DOC320>()
                    .Property(e => e.ANEXO3)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_AGRUPADOR_DOC320>()
                    .Property(e => e.ANEXO4)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTO_AGRUPADOR_DOC320>()
                    .Property(e => e.NM_EMPRESA)
                    .IsUnicode(false);

            #endregion

            #region V_CAMBIO_DOC_DOC400

            modelBuilder.Entity<V_CAMBIO_DOC_DOC400>()
              .HasKey(e => new { e.CD_PRODUTO, e.CD_FAIXA, e.NU_IDENTIFICADOR, e.CD_EMPRESA, e.NU_DOCUMENTO_INGRESO, e.TP_DOCUMENTO_INGRESSO });

            modelBuilder.Entity<V_CAMBIO_DOC_DOC400>()
              .Property(e => e.CD_PRODUTO)
              .IsUnicode(false);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC400>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC400>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC400>()
                .Property(e => e.NU_DOCUMENTO_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC400>()
                .Property(e => e.TP_DOCUMENTO_INGRESSO)
                .IsUnicode(false);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC400>()
                .Property(e => e.QT_INGRESADA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC400>()
                .Property(e => e.QT_RESERVADA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC400>()
                .Property(e => e.QT_DESAFECTADA)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC400>()
                .Property(e => e.QT_SALDO)
                .HasPrecision(15, 3);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC400>()
                .Property(e => e.NU_DOC)
                .IsUnicode(false);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC400>()
                .Property(e => e.QT_MOVIMIENTO)
                .HasPrecision(15, 3);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC400>()
               .Property(e => e.QT_NACIONALIZADA)
               .HasPrecision(12, 3);

            modelBuilder.Entity<V_CAMBIO_DOC_DOC400>()
               .Property(e => e.QT_EXPEDIDO)
               .HasPrecision(12, 3);

            #endregion

            #region V_DOC361_AJUSTES_STOCK

            modelBuilder.Entity<V_DOC361_AJUSTES_STOCK>()
              .Property(e => e.CD_PRODUTO)
              .IsUnicode(false);

            modelBuilder.Entity<V_DOC361_AJUSTES_STOCK>()
                .Property(e => e.DS_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DOC361_AJUSTES_STOCK>()
                .Property(e => e.QT_MOVIMIENTO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_DOC361_AJUSTES_STOCK>()
                .Property(e => e.NM_EMPRESA)
                .IsUnicode(false);

            modelBuilder.Entity<V_DOC361_AJUSTES_STOCK>()
                .Property(e => e.CD_CGC_EMPRESA)
                .IsUnicode(false);

            modelBuilder.Entity<V_DOC361_AJUSTES_STOCK>()
                .Property(e => e.DS_MOTIVO_AJUSTE)
                .IsUnicode(false);

            modelBuilder.Entity<V_DOC361_AJUSTES_STOCK>()
                .Property(e => e.USUARIO_MOTIVO)
                .IsUnicode(false);

            #endregion

            #region V_DOC363_AJUSTE_ACTA

            modelBuilder.Entity<V_DOC363_AJUSTE_ACTA>()
             .Property(e => e.CD_PRODUTO)
             .IsUnicode(false);

            modelBuilder.Entity<V_DOC363_AJUSTE_ACTA>()
           .Property(e => e.NU_IDENTIFICADOR)
           .IsUnicode(false);

            modelBuilder.Entity<V_DOC363_AJUSTE_ACTA>()
                .Property(e => e.DS_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DOC363_AJUSTE_ACTA>()
                .Property(e => e.QT_MOVIMIENTO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_DOC363_AJUSTE_ACTA>()
                .Property(e => e.QT_ACTA)
                .HasPrecision(38, 0);

            modelBuilder.Entity<V_DOC363_AJUSTE_ACTA>()
                .Property(e => e.VL_FILTRO)
                .IsUnicode(false);

            #endregion

            #region V_DOC362_DOCUMENTO_INGRESO

            modelBuilder.Entity<V_DOC362_DOCUMENTO_INGRESO>()
               .HasKey(e => new { e.NU_DOCUMENTO, e.TP_DOCUMENTO });

            modelBuilder.Entity<V_DOC362_DOCUMENTO_INGRESO>()
               .Property(e => e.NU_DOCUMENTO)
               .IsUnicode(false);

            modelBuilder.Entity<V_DOC362_DOCUMENTO_INGRESO>()
                .Property(e => e.TP_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DOC362_DOCUMENTO_INGRESO>()
                .Property(e => e.DS_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DOC362_DOCUMENTO_INGRESO>()
                .Property(e => e.NU_AGRUPADOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_DOC362_DOCUMENTO_INGRESO>()
                .Property(e => e.TP_AGRUPADOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_DOC362_DOCUMENTO_INGRESO>()
                .Property(e => e.NU_LACRE)
                .IsUnicode(false);

            modelBuilder.Entity<V_DOC362_DOCUMENTO_INGRESO>()
                .Property(e => e.NU_FACTURA)
                .IsUnicode(false);

            modelBuilder.Entity<V_DOC362_DOCUMENTO_INGRESO>()
                .Property(e => e.QT_DOCUMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<V_DOC362_DOCUMENTO_INGRESO>()
                .Property(e => e.QT_CIF)
                .HasPrecision(38, 0);

            modelBuilder.Entity<V_DOC362_DOCUMENTO_INGRESO>()
                .Property(e => e.ID_ESTADO)
                .IsUnicode(false);

            #endregion

            #region V_ROLLBACK_CAMBIO_DOC

            modelBuilder.Entity<V_ROLLBACK_CAMBIO_DOC>()
              .HasKey(e => new { e.CD_PRODUTO, e.CD_FAIXA, e.NU_IDENTIFICADOR, e.CD_EMPRESA, e.NU_DOCUMENTO, e.TP_DOCUMENTO, e.NU_DOCUMENTO_CAMBIO, e.TP_DOCUMENTO_CAMBIO });

            modelBuilder.Entity<V_ROLLBACK_CAMBIO_DOC>()
              .Property(e => e.CD_PRODUTO)
              .IsUnicode(false);

            modelBuilder.Entity<V_ROLLBACK_CAMBIO_DOC>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<V_ROLLBACK_CAMBIO_DOC>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_ROLLBACK_CAMBIO_DOC>()
                .Property(e => e.NU_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_ROLLBACK_CAMBIO_DOC>()
                .Property(e => e.TP_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_ROLLBACK_CAMBIO_DOC>()
                .Property(e => e.NU_DOCUMENTO_CAMBIO)
                .IsUnicode(false);

            modelBuilder.Entity<V_ROLLBACK_CAMBIO_DOC>()
                .Property(e => e.TP_DOCUMENTO_CAMBIO)
                .IsUnicode(false);

            modelBuilder.Entity<V_ROLLBACK_CAMBIO_DOC>()
                .Property(e => e.QT_INGRESADA)
                .HasPrecision(12, 3);

            #endregion

            #region V_DOC363_DOCUMENTO_INGRESO

            modelBuilder.Entity<V_DOC363_DOCUMENTO_INGRESO>()
                .HasKey(e => new { e.NU_DOCUMENTO, e.TP_DOCUMENTO });

            modelBuilder.Entity<V_DOC363_DOCUMENTO_INGRESO>()
                .Property(e => e.NU_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DOC363_DOCUMENTO_INGRESO>()
                .Property(e => e.TP_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DOC363_DOCUMENTO_INGRESO>()
                .Property(e => e.QT_DOCUMENTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<V_DOC363_DOCUMENTO_INGRESO>()
                .Property(e => e.QT_CIF)
                .HasPrecision(38, 0);

            modelBuilder.Entity<V_DOC363_DOCUMENTO_INGRESO>()
                .Property(e => e.QT_AJUSTES)
                .HasPrecision(38, 0);

            modelBuilder.Entity<V_DOC363_DOCUMENTO_INGRESO>()
               .Property(e => e.VL_FILTRO)
               .IsUnicode(false);

            #endregion

            #region V_DOCUMENTOS_ACTA_DOC310

            modelBuilder.Entity<V_DOCUMENTOS_ACTA_DOC310>()
                       .HasKey(e => new { e.DOCUMENTO_ACTA, e.TIPO_DOCUMENTO_ACTA, e.DOCUMENTO_AFECTADO, e.NU_DOCUMENTO_AFECTADO, e.TIPO_DOCUMENTO_AFECTADO });

            modelBuilder.Entity<V_DOCUMENTOS_ACTA_DOC310>()
                       .Property(e => e.DOCUMENTO_ACTA)
                       .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTOS_ACTA_DOC310>()
                    .Property(e => e.TIPO_DOCUMENTO_ACTA)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTOS_ACTA_DOC310>()
                    .Property(e => e.DOCUMENTO_AFECTADO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTOS_ACTA_DOC310>()
                   .Property(e => e.NU_DOCUMENTO_AFECTADO)
                   .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTOS_ACTA_DOC310>()
                    .Property(e => e.TIPO_DOCUMENTO_AFECTADO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTOS_ACTA_DOC310>()
                    .Property(e => e.DUA_DOCUMENTO_AFECTADO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOCUMENTOS_ACTA_DOC310>()
                    .Property(e => e.TIPO_DUA_DOCUMENTO_AFECTADO)
                    .IsUnicode(false);

            #endregion

            #region V_DET_DOCUMENTO

            modelBuilder.Entity<V_DET_DOCUMENTO>()
              .HasKey(e => new { e.NU_IDENTIFICADOR, e.CD_PRODUTO, e.CD_EMPRESA, e.NU_DOCUMENTO, e.TP_DOCUMENTO, e.DT_ADDROW, e.ID_ESTADO, e.CD_FAIXA });

            modelBuilder.Entity<V_DET_DOCUMENTO>()
              .Property(e => e.TP_DOCUMENTO)
              .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO>()
                .Property(e => e.NU_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO>()
                .Property(e => e.TP_DOCUMENTO_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO>()
                .Property(e => e.NU_DOCUMENTO_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO>()
                .Property(e => e.TP_DOCUMENTO_EGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO>()
                .Property(e => e.NU_DOCUMENTO_EGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO>()
                .Property(e => e.ID_ESTADO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO>()
                .Property(e => e.CD_NAM)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO>()
                .Property(e => e.DS_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<V_DET_DOCUMENTO>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO>()
                .Property(e => e.CD_EMPRESA)
                .HasPrecision(38, 0);

            modelBuilder.Entity<V_DET_DOCUMENTO>()
                .Property(e => e.NM_EMPRESA)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO>()
                .Property(e => e.TP_DUA_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO>()
                .Property(e => e.NU_DUA_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO>()
                .Property(e => e.NU_AGRUPADOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO>()
                .Property(e => e.TP_AGRUPADOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_DOCUMENTO>()
                .Property(e => e.VL_MERCADERIA)
                .HasPrecision(38, 0);

            modelBuilder.Entity<V_DET_DOCUMENTO>()
                .Property(e => e.QT_RESERVADA)
                .HasPrecision(38, 0);

            modelBuilder.Entity<V_DET_DOCUMENTO>()
                .Property(e => e.QT_DOCUMENTO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_DET_DOCUMENTO>()
                .Property(e => e.VL_CIF_INGRESO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_DET_DOCUMENTO>()
                .Property(e => e.VL_FOB_INGRESO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<V_DET_DOCUMENTO>()
                .Property(e => e.VL_TRIBUTO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<V_DET_DOCUMENTO>()
                .Property(e => e.NU_PROCESO)
                .IsUnicode(false);

            #endregion

            #region   V_DOC_SALDO_TEMP_AUX

            modelBuilder.Entity<V_DOC_SALDO_TEMP_AUX>()
                    .HasKey(e => new { e.NU_DOCUMENTO, e.TP_DOCUMENTO });

            modelBuilder.Entity<V_DOC_SALDO_TEMP_AUX>()
                    .Property(e => e.NU_DOCUMENTO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOC_SALDO_TEMP_AUX>()
                    .Property(e => e.TP_DOCUMENTO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_DOC_SALDO_TEMP_AUX>()
                    .Property(e => e.QT_DESAFECTADA)
                    .HasPrecision(38, 0);

            modelBuilder.Entity<V_DOC_SALDO_TEMP_AUX>()
                    .Property(e => e.VL_CIF)
                    .HasPrecision(38, 0);

            #endregion

            #region V_DOC363_SALDO_LINEA_INGRESO

            modelBuilder.Entity<V_DOC363_SALDO_LINEA_INGRESO>()
                .HasKey(e => new { e.NU_DOCUMENTO, e.TP_DOCUMENTO, e.CD_EMPRESA, e.CD_PRODUTO, e.NU_IDENTIFICADOR });

            modelBuilder.Entity<V_DOC363_SALDO_LINEA_INGRESO>()
                .Property(e => e.NU_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DOC363_SALDO_LINEA_INGRESO>()
                .Property(e => e.TP_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DOC363_SALDO_LINEA_INGRESO>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DOC363_SALDO_LINEA_INGRESO>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_DOC363_SALDO_LINEA_INGRESO>()
                .Property(e => e.SALDO)
                .HasPrecision(38, 0);

            modelBuilder.Entity<V_DOC363_SALDO_LINEA_INGRESO>()
                .Property(e => e.VL_FILTRO)
                .IsUnicode(false);

            #endregion

            #region V_DET_INT_SALIDA_BB_KIT260

            modelBuilder.Entity<V_DET_INT_SALIDA_BB_KIT260>()
               .HasKey(e => new { e.NU_INTERFAZ_EJECUCION, e.NU_REGISTRO, e.TIPO });

            modelBuilder.Entity<V_DET_INT_SALIDA_BB_KIT260>()
               .Property(e => e.TIPO)
               .IsUnicode(false);

            modelBuilder.Entity<V_DET_INT_SALIDA_BB_KIT260>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_INT_SALIDA_BB_KIT260>()
                .Property(e => e.DS_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_INT_SALIDA_BB_KIT260>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_INT_SALIDA_BB_KIT260>()
                .Property(e => e.DS_DOMINIO_VALOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_DET_INT_SALIDA_BB_KIT260>()
                .Property(e => e.QT_MOVIMIENTO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<V_DET_INT_SALIDA_BB_KIT260>()
                .Property(e => e.NU_REGISTRO)
                .IsUnicode(false);

            #endregion

            #region V_INTERFACES_SALIDA_BB_KIT250

            modelBuilder.Entity<V_INTERFACES_SALIDA_BB_KIT250>()
              .Property(e => e.NU_PRDC_INGRESO)
              .IsUnicode(false);

            modelBuilder.Entity<V_INTERFACES_SALIDA_BB_KIT250>()
                .Property(e => e.NM_PRDC_DEFINICION)
                .IsUnicode(false);

            modelBuilder.Entity<V_INTERFACES_SALIDA_BB_KIT250>()
                .Property(e => e.CD_PRDC_LINEA)
                .IsUnicode(false);

            modelBuilder.Entity<V_INTERFACES_SALIDA_BB_KIT250>()
                .Property(e => e.CD_CGC_EMPRESA)
                .IsUnicode(false);

            modelBuilder.Entity<V_INTERFACES_SALIDA_BB_KIT250>()
                .Property(e => e.NM_EMPRESA)
                .IsUnicode(false);

            modelBuilder.Entity<V_INTERFACES_SALIDA_BB_KIT250>()
                .Property(e => e.DS_REFERENCIA)
                .IsUnicode(false);

            modelBuilder.Entity<V_INTERFACES_SALIDA_BB_KIT250>()
                .Property(e => e.FL_ERROR_CARGA)
                .IsUnicode(false);

            modelBuilder.Entity<V_INTERFACES_SALIDA_BB_KIT250>()
                .Property(e => e.FL_ERROR_PROCEDIMIENTO)
                .IsUnicode(false);

            #endregion

            #region V_KIT240_MOVIMIENTOS_STOCK_BB

            modelBuilder.Entity<V_KIT240_MOVIMIENTOS_STOCK_BB>()
                    .Property(e => e.NU_MOVIMIENTO_BB)
                    .IsUnicode(false);

            modelBuilder.Entity<V_KIT240_MOVIMIENTOS_STOCK_BB>()
                    .Property(e => e.CD_PRODUTO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_KIT240_MOVIMIENTOS_STOCK_BB>()
                    .Property(e => e.DS_PRODUTO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_KIT240_MOVIMIENTOS_STOCK_BB>()
                    .Property(e => e.NU_IDENTIFICADOR)
                    .IsUnicode(false);

            modelBuilder.Entity<V_KIT240_MOVIMIENTOS_STOCK_BB>()
                    .Property(e => e.QT_MOVIMIENTO)
                    .HasPrecision(12, 3);

            modelBuilder.Entity<V_KIT240_MOVIMIENTOS_STOCK_BB>()
                    .Property(e => e.CD_ENDERECO_ORIGEN)
                    .IsUnicode(false);

            modelBuilder.Entity<V_KIT240_MOVIMIENTOS_STOCK_BB>()
                    .Property(e => e.CD_ENDERECO_DESTINO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_KIT240_MOVIMIENTOS_STOCK_BB>()
                    .Property(e => e.DS_DOMINIO_VALOR)
                    .IsUnicode(false);

            modelBuilder.Entity<V_KIT240_MOVIMIENTOS_STOCK_BB>()
                    .Property(e => e.NM_EMPRESA)
                    .IsUnicode(false);

            modelBuilder.Entity<V_KIT240_MOVIMIENTOS_STOCK_BB>()
                    .Property(e => e.FULLNAME)
                    .IsUnicode(false);

            modelBuilder.Entity<V_KIT240_MOVIMIENTOS_STOCK_BB>()
                   .Property(e => e.NU_PRDC_INGRESO)
                   .IsUnicode(false);

            #endregion

            #region V_STOCK_CONSUMIR_BB_KIT210

            modelBuilder.Entity<V_STOCK_CONSUMIR_BB_KIT210>()
                    .HasKey(e => new { e.CD_EMPRESA, e.CD_PRODUTO, e.CD_FAIXA, e.NU_IDENTIFICADOR, e.CD_ENDERECO });

            modelBuilder.Entity<V_STOCK_CONSUMIR_BB_KIT210>()
                    .Property(e => e.NU_PRDC_INGRESO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_STOCK_CONSUMIR_BB_KIT210>()
                    .Property(e => e.CD_PRDC_LINEA)
                    .IsUnicode(false);

            modelBuilder.Entity<V_STOCK_CONSUMIR_BB_KIT210>()
                    .Property(e => e.NU_PREDIO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_STOCK_CONSUMIR_BB_KIT210>()
                    .Property(e => e.ND_TIPO_LINEA)
                    .IsUnicode(false);

            modelBuilder.Entity<V_STOCK_CONSUMIR_BB_KIT210>()
                    .Property(e => e.CD_PRDC_DEFINICION)
                    .IsUnicode(false);

            modelBuilder.Entity<V_STOCK_CONSUMIR_BB_KIT210>()
                    .Property(e => e.NM_EMPRESA)
                    .IsUnicode(false);

            modelBuilder.Entity<V_STOCK_CONSUMIR_BB_KIT210>()
                    .Property(e => e.DS_PRODUTO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_STOCK_CONSUMIR_BB_KIT210>()
                    .Property(e => e.CD_ENDERECO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_STOCK_CONSUMIR_BB_KIT210>()
                    .Property(e => e.CD_PRODUTO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_STOCK_CONSUMIR_BB_KIT210>()
                    .Property(e => e.CD_FAIXA)
                    .HasPrecision(9, 3);

            modelBuilder.Entity<V_STOCK_CONSUMIR_BB_KIT210>()
                    .Property(e => e.NU_IDENTIFICADOR)
                    .IsUnicode(false);

            modelBuilder.Entity<V_STOCK_CONSUMIR_BB_KIT210>()
                    .Property(e => e.QT_ESTOQUE)
                    .HasPrecision(12, 3);

            modelBuilder.Entity<V_STOCK_CONSUMIR_BB_KIT210>()
                    .Property(e => e.QT_RESERVA_SAIDA)
                    .HasPrecision(12, 3);

            modelBuilder.Entity<V_STOCK_CONSUMIR_BB_KIT210>()
                    .Property(e => e.QT_TRANSITO_ENTRADA)
                    .HasPrecision(12, 3);

            modelBuilder.Entity<V_STOCK_CONSUMIR_BB_KIT210>()
                    .Property(e => e.ID_AVERIA)
                    .IsUnicode(false);

            modelBuilder.Entity<V_STOCK_CONSUMIR_BB_KIT210>()
                    .Property(e => e.ID_CTRL_CALIDAD)
                    .IsUnicode(false);

            modelBuilder.Entity<V_STOCK_CONSUMIR_BB_KIT210>()
                    .Property(e => e.QT_CONSUMIDO)
                    .HasPrecision(38, 0);

            #endregion

            #region V_PRDC_KIT200_STOCK_ENTRADA_BB

            modelBuilder.Entity<V_PRDC_KIT200_STOCK_ENTRADA_BB>()
                    .HasKey(e => new { e.CD_EMPRESA, e.NU_IDENTIFICADOR, e.CD_FAIXA, e.CD_PRODUTO });

            modelBuilder.Entity<V_PRDC_KIT200_STOCK_ENTRADA_BB>()
                    .Property(e => e.CD_ENDERECO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_KIT200_STOCK_ENTRADA_BB>()
                    .Property(e => e.CD_PRODUTO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_KIT200_STOCK_ENTRADA_BB>()
                    .Property(e => e.CD_FAIXA)
                    .HasPrecision(9, 3);

            modelBuilder.Entity<V_PRDC_KIT200_STOCK_ENTRADA_BB>()
                    .Property(e => e.NU_IDENTIFICADOR)
                    .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_KIT200_STOCK_ENTRADA_BB>()
                    .Property(e => e.QT_ESTOQUE)
                    .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_KIT200_STOCK_ENTRADA_BB>()
                    .Property(e => e.QT_RESERVA_SAIDA)
                    .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_KIT200_STOCK_ENTRADA_BB>()
                    .Property(e => e.QT_TRANSITO_ENTRADA)
                    .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_KIT200_STOCK_ENTRADA_BB>()
                    .Property(e => e.ID_AVERIA)
                    .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_KIT200_STOCK_ENTRADA_BB>()
                    .Property(e => e.ID_INVENTARIO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_KIT200_STOCK_ENTRADA_BB>()
                    .Property(e => e.ID_CTRL_CALIDAD)
                    .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_KIT200_STOCK_ENTRADA_BB>()
                    .Property(e => e.DS_PRODUTO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_KIT200_STOCK_ENTRADA_BB>()
                    .Property(e => e.NM_EMPRESA)
                    .IsUnicode(false);

            #endregion

            #region V_STOCK_PRODUCIR_BB_KIT220

            modelBuilder.Entity<V_STOCK_PRODUCIR_BB_KIT220>()
                    .HasKey(e => new { e.CD_EMPRESA, e.CD_PRODUTO, e.CD_FAIXA, e.NU_IDENTIFICADOR, e.CD_ENDERECO });

            modelBuilder.Entity<V_STOCK_PRODUCIR_BB_KIT220>()
                    .Property(e => e.NU_PRDC_INGRESO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_STOCK_PRODUCIR_BB_KIT220>()
                    .Property(e => e.CD_PRDC_LINEA)
                    .IsUnicode(false);

            modelBuilder.Entity<V_STOCK_PRODUCIR_BB_KIT220>()
                    .Property(e => e.NU_PREDIO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_STOCK_PRODUCIR_BB_KIT220>()
                    .Property(e => e.ND_TIPO_LINEA)
                    .IsUnicode(false);

            modelBuilder.Entity<V_STOCK_PRODUCIR_BB_KIT220>()
                    .Property(e => e.CD_PRDC_DEFINICION)
                    .IsUnicode(false);

            modelBuilder.Entity<V_STOCK_PRODUCIR_BB_KIT220>()
                    .Property(e => e.NM_EMPRESA)
                    .IsUnicode(false);

            modelBuilder.Entity<V_STOCK_PRODUCIR_BB_KIT220>()
                    .Property(e => e.DS_PRODUTO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_STOCK_PRODUCIR_BB_KIT220>()
                    .Property(e => e.CD_ENDERECO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_STOCK_PRODUCIR_BB_KIT220>()
                    .Property(e => e.CD_PRODUTO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_STOCK_PRODUCIR_BB_KIT220>()
                    .Property(e => e.CD_FAIXA)
                    .HasPrecision(9, 3);

            modelBuilder.Entity<V_STOCK_PRODUCIR_BB_KIT220>()
                    .Property(e => e.NU_IDENTIFICADOR)
                    .IsUnicode(false);

            modelBuilder.Entity<V_STOCK_PRODUCIR_BB_KIT220>()
                    .Property(e => e.QT_ESTOQUE)
                    .HasPrecision(12, 3);

            modelBuilder.Entity<V_STOCK_PRODUCIR_BB_KIT220>()
                    .Property(e => e.QT_RESERVA_SAIDA)
                    .HasPrecision(12, 3);

            modelBuilder.Entity<V_STOCK_PRODUCIR_BB_KIT220>()
                    .Property(e => e.QT_TRANSITO_ENTRADA)
                    .HasPrecision(12, 3);

            modelBuilder.Entity<V_STOCK_PRODUCIR_BB_KIT220>()
                    .Property(e => e.ID_AVERIA)
                    .IsUnicode(false);

            modelBuilder.Entity<V_STOCK_PRODUCIR_BB_KIT220>()
                    .Property(e => e.ID_CTRL_CALIDAD)
                    .IsUnicode(false);

            modelBuilder.Entity<V_STOCK_PRODUCIR_BB_KIT220>()
                    .Property(e => e.QT_PRODUCIDO)
                    .HasPrecision(38, 0);

            #endregion

            #region V_PRDC_KIT230_STOCK_SALIDA_BB

            modelBuilder.Entity<V_PRDC_KIT230_STOCK_SALIDA_BB>()
                  .HasKey(e => new { e.CD_EMPRESA, e.NU_IDENTIFICADOR, e.CD_FAIXA, e.CD_PRODUTO });

            modelBuilder.Entity<V_PRDC_KIT230_STOCK_SALIDA_BB>()
                  .Property(e => e.CD_ENDERECO)
                  .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_KIT230_STOCK_SALIDA_BB>()
                    .Property(e => e.CD_PRODUTO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_KIT230_STOCK_SALIDA_BB>()
                    .Property(e => e.CD_FAIXA)
                    .HasPrecision(9, 3);

            modelBuilder.Entity<V_PRDC_KIT230_STOCK_SALIDA_BB>()
                    .Property(e => e.NU_IDENTIFICADOR)
                    .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_KIT230_STOCK_SALIDA_BB>()
                    .Property(e => e.QT_ESTOQUE)
                    .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_KIT230_STOCK_SALIDA_BB>()
                    .Property(e => e.QT_RESERVA_SAIDA)
                    .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_KIT230_STOCK_SALIDA_BB>()
                    .Property(e => e.QT_TRANSITO_ENTRADA)
                    .HasPrecision(12, 3);

            modelBuilder.Entity<V_PRDC_KIT230_STOCK_SALIDA_BB>()
                    .Property(e => e.ID_AVERIA)
                    .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_KIT230_STOCK_SALIDA_BB>()
                    .Property(e => e.ID_INVENTARIO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_KIT230_STOCK_SALIDA_BB>()
                    .Property(e => e.ID_CTRL_CALIDAD)
                    .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_KIT230_STOCK_SALIDA_BB>()
                    .Property(e => e.DS_PRODUTO)
                    .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_KIT230_STOCK_SALIDA_BB>()
                    .Property(e => e.NM_EMPRESA)
                    .IsUnicode(false);

            modelBuilder.Entity<V_PRDC_KIT230_STOCK_SALIDA_BB>()
                    .Property(e => e.QT_MOVIMIENTO_BB)
                    .HasPrecision(38, 0);

            modelBuilder.Entity<V_PRDC_KIT230_STOCK_SALIDA_BB>()
                    .Property(e => e.QT_RECHAZO_AVERIA)
                    .HasPrecision(38, 0);

            #endregion

            #region V_ACTA_DET_DOCUMENTO

            modelBuilder.Entity<V_ACTA_DET_DOCUMENTO>()
               .HasKey(e => new { e.NU_DOCUMENTO, e.TP_DOCUMENTO, e.CD_EMPRESA, e.CD_PRODUTO, e.CD_FAIXA });

            modelBuilder.Entity<V_ACTA_DET_DOCUMENTO>()
               .Property(e => e.NU_DOCUMENTO)
               .IsUnicode(false);

            modelBuilder.Entity<V_ACTA_DET_DOCUMENTO>()
                .Property(e => e.TP_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_ACTA_DET_DOCUMENTO>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_ACTA_DET_DOCUMENTO>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<V_ACTA_DET_DOCUMENTO>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_ACTA_DET_DOCUMENTO>()
                .Property(e => e.QT_INGRESADA)
                .HasPrecision(38, 0);

            #endregion

            #region V_PEDIDO_SAIDA_KIT260

            modelBuilder.Entity<V_PEDIDO_SAIDA_KIT260>()
             .HasKey(e => new
             {
                 e.NU_PEDIDO,
                 e.CD_EMPRESA,
                 e.CD_CLIENTE,
                 e.CD_PRODUTO,
                 e.CD_FAIXA,
                 e.NU_IDENTIFICADOR
             });

            modelBuilder.Entity<V_PEDIDO_SAIDA_KIT260>()
             .Property(e => e.NU_PEDIDO)
             .IsUnicode(false);

            modelBuilder.Entity<V_PEDIDO_SAIDA_KIT260>()
                .Property(e => e.CD_CLIENTE)
                .IsUnicode(false);

            modelBuilder.Entity<V_PEDIDO_SAIDA_KIT260>()
                .Property(e => e.NU_PRDC_INGRESO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PEDIDO_SAIDA_KIT260>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<V_PEDIDO_SAIDA_KIT260>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<V_PEDIDO_SAIDA_KIT260>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<V_PEDIDO_SAIDA_KIT260>()
                .Property(e => e.QT_LIBERADO)
                .HasPrecision(38, 0);

            #endregion

            #region T_REGIMEN_ADUANA

            modelBuilder.Entity<T_REGIMEN_ADUANA>()
                   .Property(e => e.DS_REGIMEN_ADUANA)
                   .IsUnicode(false);

            #endregion

            #region T_DOCUMENTO_AJUSTE_STOCK

            modelBuilder.Entity<T_DOCUMENTO_AJUSTE_STOCK>()
              .Property(e => e.CD_PRODUTO)
              .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AJUSTE_STOCK>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<T_DOCUMENTO_AJUSTE_STOCK>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AJUSTE_STOCK>()
                .Property(e => e.QT_MOVIMIENTO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_DOCUMENTO_AJUSTE_STOCK>()
                .Property(e => e.DS_MOTIVO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AJUSTE_STOCK>()
                .Property(e => e.TP_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AJUSTE_STOCK>()
                .Property(e => e.NU_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AJUSTE_STOCK>()
                .Property(e => e.CD_MOTIVO_AJUSTE)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AJUSTE_STOCK>()
                .Property(e => e.NU_PREDIO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AJUSTE_STOCK>()
                .Property(e => e.CD_APLICACAO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AJUSTE_STOCK>()
                .Property(e => e.CD_ENDERECO)
                .IsUnicode(false);

            #endregion

            #region T_DOCUMENTO_AJUSTE_STOCK_HIST

            modelBuilder.Entity<T_DOCUMENTO_AJUSTE_STOCK_HIST>()
              .HasKey(e => new { e.NU_AJUSTE_STOCK, e.TP_OPERACION, e.NU_OPERACION });

            modelBuilder.Entity<T_DOCUMENTO_AJUSTE_STOCK_HIST>()
              .Property(e => e.TP_OPERACION)
              .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AJUSTE_STOCK_HIST>()
                .Property(e => e.CD_PRODUTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AJUSTE_STOCK_HIST>()
                .Property(e => e.CD_FAIXA)
                .HasPrecision(9, 3);

            modelBuilder.Entity<T_DOCUMENTO_AJUSTE_STOCK_HIST>()
                .Property(e => e.NU_IDENTIFICADOR)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AJUSTE_STOCK_HIST>()
                .Property(e => e.QT_MOVIMIENTO)
                .HasPrecision(12, 3);

            modelBuilder.Entity<T_DOCUMENTO_AJUSTE_STOCK_HIST>()
                .Property(e => e.DS_MOTIVO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AJUSTE_STOCK_HIST>()
                .Property(e => e.TP_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AJUSTE_STOCK_HIST>()
                .Property(e => e.NU_DOCUMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AJUSTE_STOCK_HIST>()
                .Property(e => e.CD_MOTIVO_AJUSTE)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AJUSTE_STOCK_HIST>()
                .Property(e => e.NU_PREDIO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AJUSTE_STOCK_HIST>()
                .Property(e => e.CD_APLICACAO)
                .IsUnicode(false);

            modelBuilder.Entity<T_DOCUMENTO_AJUSTE_STOCK_HIST>()
                .Property(e => e.CD_ENDERECO)
                .IsUnicode(false);

            #endregion

            #region V_CAMBIO_DOC_DOC401

            modelBuilder.Entity<V_CAMBIO_DOC_DOC401>()
             .HasKey(e => new { e.CD_PRODUTO, e.CD_FAIXA, e.NU_IDENTIFICADOR, e.CD_EMPRESA, e.NU_DOCUMENTO_INGRESO, e.TP_DOCUMENTO_INGRESSO });

            #endregion

            #region V_LPN_TIPO

            modelBuilder.Entity<V_LPN_TIPO>()
                 .Property(e => e.TP_LPN_TIPO)
                 .IsUnicode(false);

            modelBuilder.Entity<V_LPN_TIPO>()
                 .Property(e => e.NM_LPN_TIPO)
                 .IsUnicode(false);

            modelBuilder.Entity<V_LPN_TIPO>()
                 .Property(e => e.DS_LPN_TIPO)
                 .IsUnicode(false);

            modelBuilder.Entity<V_LPN_TIPO>()
                 .Property(e => e.FL_PERMITE_CONSOLIDAR)
                 .IsUnicode(false);

            modelBuilder.Entity<V_LPN_TIPO>()
                 .Property(e => e.FL_PERMITE_EXTRAER_LINEAS)
                 .IsUnicode(false);

            modelBuilder.Entity<V_LPN_TIPO>()
                 .Property(e => e.FL_PERMITE_AGREGAR_LINEAS)
                 .IsUnicode(false);

            modelBuilder.Entity<V_LPN_TIPO>()
                 .Property(e => e.VL_PREFIJO)
                 .IsUnicode(false);

            modelBuilder.Entity<V_LPN_TIPO>()
                 .Property(e => e.FL_MULTIPRODUCTO)
                 .IsUnicode(false);

            modelBuilder.Entity<V_LPN_TIPO>()
                 .Property(e => e.FL_MULTI_LOTE)
                 .IsUnicode(false);

            modelBuilder.Entity<V_LPN_TIPO>()
                 .Property(e => e.FL_PERMITE_ANIDACION)
                 .IsUnicode(false);

            modelBuilder.Entity<V_LPN_TIPO>()
                 .Property(e => e.NU_TEMPLATE_ETIQUETA)
                 .IsUnicode(false);

            modelBuilder.Entity<V_LPN_TIPO>()
                 .Property(e => e.NU_COMPONENTE)
                 .IsUnicode(false);
            modelBuilder.Entity<V_LPN_TIPO>()
               .Property(e => e.FL_CONTENEDOR_LPN)
               .IsUnicode(false);

            modelBuilder.Entity<V_LPN_TIPO>()
              .Property(e => e.FL_PERMITE_DESTRUIR_ALM)
              .IsUnicode(false);


            #endregion

            #region T_LPN_TIPO
            modelBuilder.Entity<T_LPN_TIPO>()
               .Property(e => e.TP_LPN_TIPO)
               .IsUnicode(false);

            modelBuilder.Entity<T_LPN_TIPO>()
              .Property(e => e.NM_LPN_TIPO)
              .IsUnicode(false);

            modelBuilder.Entity<T_LPN_TIPO>()
            .Property(e => e.DS_LPN_TIPO)
            .IsUnicode(false);
            modelBuilder.Entity<T_LPN_TIPO>()
            .Property(e => e.FL_PERMITE_CONSOLIDAR)
            .IsUnicode(false);
            modelBuilder.Entity<T_LPN_TIPO>()
          .Property(e => e.FL_PERMITE_EXTRAER_LINEAS)
          .IsUnicode(false);
            modelBuilder.Entity<T_LPN_TIPO>()
         .Property(e => e.FL_PERMITE_AGREGAR_LINEAS)
         .IsUnicode(false);
            modelBuilder.Entity<T_LPN_TIPO>()
           .Property(e => e.FL_CREAR_SOLO_AL_INGRESO)
           .IsUnicode(false);
            modelBuilder.Entity<T_LPN_TIPO>()
          .Property(e => e.FL_MULTIPRODUCTO)
          .IsUnicode(false);
            modelBuilder.Entity<T_LPN_TIPO>()
         .Property(e => e.FL_MULTI_LOTE)
         .IsUnicode(false);
            modelBuilder.Entity<T_LPN_TIPO>()
        .Property(e => e.FL_PERMITE_ANIDACION)
        .IsUnicode(false);
            modelBuilder.Entity<T_LPN_TIPO>()
        .Property(e => e.NU_TEMPLATE_ETIQUETA)
        .IsUnicode(false);
            modelBuilder.Entity<T_LPN_TIPO>()
         .Property(e => e.NU_COMPONENTE)
         .IsUnicode(false);
            modelBuilder.Entity<T_LPN_TIPO>()
      .Property(e => e.FL_CONTENEDOR_LPN)
      .IsUnicode(false);
            modelBuilder.Entity<T_LPN_TIPO>()
    .Property(e => e.FL_PERMITE_DESTRUIR_ALM)
    .IsUnicode(false);
            #endregion

            #region T_DET_PALLET_TRANSFERENCIA

            modelBuilder.Entity<T_DET_PALLET_TRANSFERENCIA>()
                .Property(e => e.QT_PRODUTO)
                .HasPrecision(12, 3);

            #endregion

            #endregion

            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetProperties()).Where(p => p.ClrType == typeof(string) && (p.IsUnicode() ?? true) && p.GetColumnType() == null))
            {
                property.SetIsUnicode(false);
            }
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    }
}