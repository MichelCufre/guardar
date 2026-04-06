using System;
using System.Collections.Generic;
using System.Linq;

namespace WIS.Domain.DataModel.Mappers.Constants
{
    public class ParamManager
    {
        #region Tipo de entidad de parametro
        public const string PARAM_APP = "PARAM_APP";
        public const string PARAM_EMPR = "PARAM_EMPR";
        public const string PARAM_GRAL = "PARAM_GRAL";
        public const string PARAM_AGEN = "PARAM_AGEN";
        public const string PARAM_USER = "PARAM_USER";
        public const string PARAM_PRED = "PARAM_PRED";
        public const string PARAM_CLIENTE = "PARAM_CLIENTE";
        public const string PARAM_DOMAIN = "PARAM_DOM";
        public const string PARAM_GENERICO = "PARAM_AUTO";
        public const string PARAM_ENVASE = "PARAM_ENVA";
        public const string PARAM_MERCADERIA = "PARAM_MERC";
        public const string PARAM_ZONA = "PARAM_ZONA";
        public const string PARAM_TPED = "PARAM_TPED";
        #endregion


        #region General
        public const string DATETIME_FORMAT_24H = "DATETIME_FORMAT_24H";
        public const string DATETIME_FORMAT_DATE = "DATETIME_FORMAT_DATE";
        public const string DATETIME_FORMAT_DATE_HOURS = "DATETIME_FORMAT_DATE_HOURS";
        public const string DATETIME_FORMAT_DATE_SECONDS = "DATETIME_FORMAT_DATE_SECONDS";
        public const string DATETIME_FORMAT_DATE_SECONDS_H = "DATETIME_FORMAT_DATE_SECONDS_H";
        public const string DATETIME_FORMAT_FILE = "DATETIME_FORMAT_FILE";
        public const string NUMBER_DECIMAL_SEPARATOR = "NUMBER_DECIMAL_SEPARATOR";
        public const string GRUPO_CONSULTA = "GRUPO_CONSULTA";
        public const string CUSTOM_API_MAPPING = "CUSTOM_API_MAPPING";
        public const string IS_CONFIRMACION_OMISIBLE = "IS_CONFIRMACION_OMISIBLE";
        public const string IS_FILTRA_GRUPO_CONSULTA = "IS_FILTRA_GRUPO_CONSULTA";

        public const string API_UBICACION_EQUIPO = "API_UBICACION_EQUIPO";

        public const string USUARIO_PERFIL_DEFAULT = "USUARIO_PERFIL_DEFAULT";
        public const string USUARIO_PREDIO_DEFAULT = "USUARIO_PREDIO_DEFAULT";
        public const string USUARIO_EMPRESA_DEFAULT = "USUARIO_EMPRESA_DEFAULT";
        public const string PANTALLAS_MODO_CONSULTA = "PANTALLAS_MODO_CONSULTA";
        public const string MODO_CONSULTA = "MODO_CONSULTA";

        public const string PREFIJO_CLASIFICACION = "PREFIJO_CLASIFICACION";
        public const string PREFIJO_EQUIPO = "PREFIJO_EQUIPO";
        public const string PREFIJO_AUTOMATISMO = "PREFIJO_AUTOMATISMO";
        public const string PREFIJO_PUERTA = "PREFIJO_PUERTA";
        public const string PREFIJO_PRODUCCION = "PREFIJO_PRODUCCION";
        public const string PREFIJO_EQUIPO_FUN = "PREFIJO_EQUIPO_FUN";
        public const string CANTIDAD_MAXIMA_IMPRESIONES = "CANTIDAD_MAXIMA_IMPRESIONES";
        public const string MOBILE_LOV_ID_SEPARATOR = "MOBILE_LOV_ID_SEPARATOR";
        public const string CARACTERES_NO_PERMITIDOS_LOTE = "CARACTERES_NO_PERMITIDOS_LOTE";
        public const string USAR_PRODUCTOS_INACTIVOS = "USAR_PRODUCTOS_INACTIVOS";
        public const string GENERAR_PARAMETROS_INTERFACES = "GENERAR_PARAMETROS_INTERFACES";
        public const string IMPORT_EXCEL_DESCARGA_ERRORES = "IMPORT_EXCEL_DESCARGA_ERRORES";

        #endregion

        #region Agenda
        public const string IE_525_HABILITADA = "IE_525_HABILITADA";
        #endregion

        #region Agente
        public const string IE_507_HABILITADA = "IE_507_HABILITADA";
        public const string IE_507_CD_ROTA = "IE_507_CD_ROTA";
        public const string IE_507_CD_SITUACAO = "IE_507_CD_SITUACAO";

        public const string IE_507_CAMPOS_INMUTABLES = "IE_507_CAMPOS_INMUTABLES";

        #endregion

        #region AjustesDeStock
        public const string IS_508_HABILITADA = "IS_508_HABILITADA";
        #endregion

        #region Almacenamiento
        public const string IS_526_HABILITADA = "IS_526_HABILITADA";
        public const string MANEJO_SUGERENCIAS_ALMACENAJE = "MANEJO_SUGERENCIAS_ALMACENAJE";
        #endregion

        #region Anulacion
        public const string ENVIO_INT_ANULACION = "ENVIO_INT_ANULACION";

        #endregion

        #region AnulacionReferenciaRecepcion
        public const string IE_513_HABILITADA = "IE_513_HABILITADA";
        #endregion

        #region Automatismo

        public const string IE_1500_HABILITADA = "IE_1500_HABILITADA";
        public const string IE_1550_HABILITADA = "IE_1550_HABILITADA";
        public const string IE_1600_HABILITADA = "IE_1600_HABILITADA";
        public const string IE_1700_HABILITADA = "IE_1700_HABILITADA";
        public const string IE_1800_HABILITADA = "IE_1800_HABILITADA";

        #endregion

        #region Stock
        public const string IE_998_HABILITADA = "IE_998_HABILITADA";
        public const string IE_999_HABILITADA = "IE_999_HABILITADA";
        public const string API_TRANSF_UBIC_EQUIPO = "API_TRANSF_UBIC_EQUIPO";

        #endregion

        #region CodigoBarras
        public const string IE_505_HABILITADA = "IE_505_HABILITADA";
        public const string IE_505_HAB_SUBRESCRIBIR_BARR = "IE_505_HAB_SUBRESCRIBIR_BARR";
        public const string IE_505_TP_CODIGO_BARRAS = "IE_505_TP_CODIGO_BARRAS";
        public const string LISTA_CARACTERES_COD_BARRA = "LISTA_CARACTERES_COD_BARRA";

        public const string IE_505_CAMPOS_INMUTABLES = "IE_505_CAMPOS_INMUTABLES";
        #endregion

        #region CodigoMultidato

        public const string CODIGO_MULTIDATO_HABILITADO = "CODIGO_MULTIDATO_HABILITADO";
        public const string CODIGO_MULTIDATO_URL_API = "CODIGO_MULTIDATO_URL_API";
        public const string CODIGO_MULTIDATO_FNC1 = "CODIGO_MULTIDATO_FNC1";

        #endregion

        #region ConfirmacionDePedido
        public const string IS_512_HABILITADA = "IS_512_HABILITADA";
        #endregion

        #region ConfirmacionDeRecepcion
        public const string IS_502_HABILITADA = "IS_502_HABILITADA";
        #endregion

        #region ConsultaDeStock
        public const string IC_601_HABILITADA = "IC_601_HABILITADA";
        #endregion

        #region CrossDocking
        public const string IE_994_HABILITADA = "IE_994_HABILITADA";

        #endregion

        #region ControlDeCalidad
        public const string IE_540_HABILITADA = "IE_540_HABILITADA";
        #endregion

        #region Documental

        public const string MANEJO_DOCUMENTAL = "MANEJO_DOCUMENTAL";
        public const string MANEJO_DOCUMENTAL_PREDIO_DEF = "MANEJO_DOCUMENTAL_PREDIO_DEF";
        public const string TP_DOC_INGRESO_PRODUCCION = "TP_DOC_INGRESO_PRODUCCION";
        public const string TP_DOC_EGRESO_PRODUCCION = "TP_DOC_EGRESO_PRODUCCION";
        public const string TP_DOC_INGRESO_TRANSFERENCIA = "TP_DOC_INGRESO_TRANSFERENCIA";
        public const string TP_DOC_EGRESO_TRANSFERENCIA = "TP_DOC_EGRESO_TRANSFERENCIA";
        public const string TP_DOC_ACTA_STOCK = "TP_DOC_ACTA_STOCK";
        public const string TP_DOC_ACTA = "TP_DOC_ACTA";

        #endregion

        #region Egreso
        public const string IE_530_HABILITADA = "IE_530_HABILITADA";

        public const string WEXP040_CAMION_CIERRE_PARCIAL = "WEXP040_CAMION_CIERRE_PARCIAL";
        public const string RESPETA_ORDEN_CARGA_DEFAULT = "RESPETA_ORDEN_CARGA_DEFAULT";
        public const string RUTEO_HABILITADO_DEFAULT = "RUTEO_HABILITADO_DEFAULT";
        public const string CD_TRANSPORTADORA_DEFAULT = "CD_TRANSPORTADORA_DEFAULT";
        public const string WEXP040_CONTROLAR_FACT_CIERRE = "WEXP040_CONTROLAR_FACT_CIERRE";
        public const string VL_DIF_CD_GRUPO_EXPEDICION_PED = "VL_DIF_CD_GRUPO_EXPEDICION_PED";

        public const string CREAR_CAMION_AUTO_CONTROL_CONT = "CREAR_CAMION_AUTO_CONTROL_CONT";
        public const string EXP040_PUERTA_CARGA_AUTO = "EXP040_PUERTA_CARGA_AUTO";

        #endregion

        #region Empresa
        public const string IE_522_HABILITADA = "IE_522_HABILITADA";
        public const string IE_522_CD_SITUACAO = "IE_522_CD_SITUACAO";
        public const string IE_522_ASIGNAR_USUARIOS = "IE_522_ASIGNAR_USUARIOS";
        public const string IE_522_TIPOS_RECEPCION = "IE_522_TIPOS_RECEPCION";

        public const string IE_522_CAMPOS_INMUTABLES = "IE_507_CAMPOS_INMUTABLES";
        #endregion

        #region Factura
        public const string IE_425_HABILITADA = "IE_425_HABILITADA";
        public const string IE_425_VALIDAR_FECHAS = "IE_425_VALIDAR_FECHAS";
        #endregion

        #region Facturacion
        public const string IS_516_HABILITADA = "IS_516_HABILITADA";
        public const string FACTURA_PUERTA_AUTO = "FACTURA_PUERTA_AUTO";
        #endregion

        #region Inventario
        public const string INV410_DEFAULT_ACTUALIZAR_CONT = "INV410_DEFAULT_ACTUALIZAR_CONT";
        public const string INV410_ENABLED_ACTUALIZAR_CONT = "INV410_ENABLED_ACTUALIZAR_CONT";
        public const string INV410_DEFAULT_BLOQ_USR_CONTEO = "INV410_DEFAULT_BLOQ_USR_CONTEO";
        public const string INV410_ENABLED_BLOQ_USR_CONTEO = "INV410_ENABLED_BLOQ_USR_CONTEO";
        public const string INV410_DEFAULT_CTR_VENCIMIENTO = "INV410_DEFAULT_CTR_VENCIMIENTO";
        public const string INV410_ENABLED_CTR_VENCIMIENTO = "INV410_ENABLED_CTR_VENCIMIENTO";
        public const string INV410_DEFAULT_MARCAR_DIF = "INV410_DEFAULT_MARCAR_DIF";
        public const string INV410_ENABLED_MARCAR_DIF = "INV410_ENABLED_MARCAR_DIF";
        public const string INV410_DEFAULT_INGR_MOTIVO = "INV410_DEFAULT_INGR_MOTIVO";
        public const string INV410_ENABLED_INGR_MOTIVO = "INV410_ENABLED_INGR_MOTIVO";
        public const string INV410_DEFAULT_EXCLUIR_SUELTOS = "INV410_DEFAULT_EXCLUIR_SUELTOS";
        public const string INV410_ENABLED_EXCLUIR_SUELTOS = "INV410_ENABLED_EXCLUIR_SUELTOS";
        public const string INV410_DEFAULT_EXCLUIR_LPNS = "INV410_DEFAULT_EXCLUIR_LPNS";
        public const string INV410_ENABLED_EXCLUIR_LPNS = "INV410_ENABLED_EXCLUIR_LPNS";
        public const string INV410_DEFAULT_REST_LPN_FIN = "INV410_DEFAULT_REST_LPN_FIN";
        public const string INV410_ENABLED_REST_LPN_FIN = "INV410_ENABLED_REST_LPN_FIN";
        public const string INV410_DEFAULT_PRIMER_CONTEO = "INV410_DEFAULT_PRIMER_CONTEO";
        public const string INV410_ENABLED_PRIMER_CONTEO = "INV410_ENABLED_PRIMER_CONTEO";
        public const string INV410_DEFAULT_UBIC_OTRO_INV = "INV410_DEFAULT_UBIC_OTRO_INV";
        public const string INV410_ENABLED_UBIC_OTRO_INV = "INV410_ENABLED_UBIC_OTRO_INV";

        public const string INV_RESOLVER_VENCIMIENTO = "INV_RESOLVER_VENCIMIENTO";
        public const string INV_ACEPTAR_CTRL_CALIDAD_PEND = "INV_ACEPTAR_CTRL_CALIDAD_PEND";

        #endregion

        #region Lpn
        public const string IE_535_HABILITADA = "IE_535_HABILITADA";

        public const string IE_535_TP_LPN_TIPO = "IE_535_TP_LPN_TIPO";

        public const string FL_MODIFICAR_ID_PACKING_LPN = "FL_MODIFICAR_ID_PACKING_LPN";

        public const string GENERAR_CB_ID_EXTERNO_LPN = "GENERAR_CB_ID_EXTERNO_LPN";

        //Puntaje Lpn
        public const string LPN_PICKING_DF_SCORE_EQNR = "LPN_PICKING_DF_SCORE_EQNR";
        public const string LPN_PICKING_DF_SCORE_EQR = "LPN_PICKING_DF_SCORE_EQR";
        public const string LPN_PICKING_DF_SCORE_LTNR = "LPN_PICKING_DF_SCORE_LTNR";
        public const string LPN_PICKING_DF_SCORE_LTR = "LPN_PICKING_DF_SCORE_LTR";
        public const string LPN_PICKING_DF_SCORE_GT = "LPN_PICKING_DF_SCORE_GT";
        public const string LPN_PICKING_DF_SCORE_NE = "LPN_PICKING_DF_SCORE_NE";
        public const string LPN_PICKING_DF_SCORE_BONUS = "LPN_PICKING_DF_SCORE_BONUS";


        #endregion

        #region ModificarDetalleReferencia
        public const string IE_520_HABILITADA = "IE_520_HABILITADA";
        #endregion

        #region ModificarPedido
        public const string IE_585_HABILITADA = "IE_585_HABILITADA";

        #endregion

        #region Pedido
        public const string IE_503_HABILITADA = "IE_503_HABILITADA";
        public const string IE_503_CD_ROTA = "IE_503_CD_ROTA";
        public const string IE_503_TP_PEDIDO = "IE_503_TP_PEDIDO";
        public const string IE_503_TP_EXPEDICION = "IE_503_TP_EXPEDICION";
        public const string IE_503_CD_TRANSPORTADORA = "IE_503_CD_TRANSPORTADORA";
        public const string IE_503_CD_CONDICION_LIBERACION = "IE_503_CD_CONDICION_LIBERACION";
        public const string IE_503_HAB_DUPLICADOS = "IE_503_HAB_DUPLICADOS";
        public const string IE_503_HAB_LPN = "IE_503_HAB_LPN";
        public const string IE_503_HAB_ATRIBUTOS = "IE_503_HAB_ATRIBUTOS";
        public const string IE_503_VALIDAR_FECHAS = "IE_503_VALIDAR_FECHAS";

        public const string PEDIDO_NUMERICO = "PEDIDO_NUMERICO";
        public const string WPRE100_VALIDAR_HORAS_ENTRE_DT = "WPRE100_VALIDAR_HORAS_ENTRE_DT";
        public const string FILTRAR_TP_AGENTE_CLI = "FILTRAR_TP_AGENTE_CLI";

        #endregion

        #region PedidosAnulados
        public const string IS_509_HABILITADA = "IS_509_HABILITADA";
        #endregion

        #region Preparacion
        public const string IE_997_HABILITADA = "IE_997_HABILITADA";
        public const string IE_996_HABILITADA = "IE_996_HABILITADA";
        public const string IE_995_HABILITADA = "IE_995_HABILITADA";
        public const string IE_990_HABILITADA = "IE_990_HABILITADA";
        public const string IE_980_HABILITADA = "IE_980_HABILITADA";

        public const string PRE052_ENABLED_PICK_VENCIDO = "PRE052_ENABLED_PICK_VENCIDO";
        public const string PRE052_DEFAULT_PICK_VENCIDO = "PRE052_DEFAULT_PICK_VENCIDO";
        public const string PRE052_ENABLED_PICK_AVERIADO = "PRE052_ENABLED_PICK_AVERIADO";
        public const string PRE052_DEFAULT_PICK_AVERIADO = "PRE052_DEFAULT_PICK_AVERIADO";
        public const string PRE052_DEFAULT_PROD_PROVEEDOR = "PRE052_DEFAULT_PROD_PROVEEDOR";
        public const string PRE052_ENABLED_PROD_PROVEEDOR = "PRE052_ENABLED_PROD_PROVEEDOR";

        public const string WPRE300_Control_Picking_Total = "WPRE300_Control_Picking_Total";

        #endregion

        #region Produccion
        public const string IE_700_HABILITADA = "IE_700_HABILITADA";
        public const string IS_790_HABILITADA = "IS_790_HABILITADA";

        public const string IE_701_HABILITADA = "IE_701_HABILITADA";
        public const string IE_702_HABILITADA = "IE_702_HABILITADA";

        public const string SistemaEmpresaPropietaria = "SISTEMA_EMPRESA_PROPIETARIA";

        public const string ProduccionLineaUbicacionBloque = "PRDC_BLOQUE_UBIC_LINEA";
        public const string ProduccionLineaUbicacionCalle = "PRDC_CALLE_UBIC_LINEA";
        public const string ProduccionLineaUbicacionZonaDefecto = "WREG040_CD_ZONA_UBICACION";

        public const string PRODUCCION_MOT_CONS_DEFAULT = "PRODUCCION_MOT_CONS_DEFAULT";
        public const string PRODUCCION_MOT_PROD_DEFAULT = "PRODUCCION_MOT_PROD_DEFAULT";
        public const string PRD100_VALIDAR_PROD_DISTINTOS = "PRD100_VALIDAR_PROD_DISTINTOS";
        #endregion

        #region Producto
        public const string IE_500_HABILITADA = "IE_500_HABILITADA";
        public const string LISTA_CARACTERES_COD_PROD = "LISTA_CARACTERES_COD_PROD";

        public const string IE_500_FAMILIA_PRODUTO = "IE_500_FAMILIA_PRODUTO";
        public const string IE_500_ROTATIVIDADE = "IE_500_ROTATIVIDADE";
        public const string IE_500_CLASSE = "IE_500_CLASSE";
        public const string IE_500_ESTOQUE_MINIMO = "IE_500_ESTOQUE_MINIMO";
        public const string IE_500_ESTOQUE_MAXIMO = "IE_500_ESTOQUE_MAXIMO";
        public const string IE_500_PS_LIQUIDO = "IE_500_PS_LIQUIDO";
        public const string IE_500_PS_BRUTO = "IE_500_PS_BRUTO";
        public const string IE_500_VL_CUBAGEM = "IE_500_VL_CUBAGEM";
        public const string IE_500_VL_PRECO_VENDA = "IE_500_VL_PRECO_VENDA";
        public const string IE_500_VL_CUSTO_ULT_ENT = "IE_500_VL_CUSTO_ULT_ENT";
        public const string IE_500_UND_DISTRIBUCION = "IE_500_UND_DISTRIBUCION";
        public const string IE_500_UND_BULTO = "IE_500_UND_BULTO";
        public const string IE_500_ID_MANEJO_IDENT = "IE_500_ID_MANEJO_IDENT";
        public const string IE_500_UNIDA_DE_MEDIDA = "IE_500_UNIDA_DE_MEDIDA";
        public const string IE_500_TP_MANEJO_FECHA = "IE_500_TP_MANEJO_FECHA";
        public const string IE_500_CD_SITUACAO = "IE_500_CD_SITUACAO";
        public const string IE_500_CD_GRUPO_CONSULTA = "IE_500_CD_GRUPO_CONSULTA";
        public const string IE_500_TP_DISPLAY = "IE_500_TP_DISPLAY";
        public const string IE_500_QT_DIAS_DURACAO = "IE_500_QT_DIAS_DURACAO";
        public const string IE_500_QT_DIAS_VALIDADEA = "IE_500_QT_DIAS_VALIDADEA";
        public const string IE_500_ID_AGRUPACION = "IE_500_ID_AGRUPACION";
        public const string IE_500_CD_RAMO_PRODUTO = "IE_500_CD_RAMO_PRODUTO";
        public const string IE_500_FL_ACEPTA_DECIMALES = "IE_500_FL_ACEPTA_DECIMALES";

        public const string IE_500_HAB_INGRESO_RAMO = "IE_500_HAB_INGRESO_RAMO";
        public const string IE_500_HAB_INGRESO_CLASE = "IE_500_HAB_INGRESO_CLASE";
        public const string IE_500_HAB_INGRESO_FAMILIA = "IE_500_HAB_INGRESO_FAMILIA";
        public const string IE_500_CAMPOS_INMUTABLES = "IE_500_CAMPOS_INMUTABLES";
        public const string PERMITIR_MOD_DATOS_LOGISTICOS = "PERMITIR_MOD_DATOS_LOGISTICOS";

        #endregion

        #region ProductoProveedor
        public const string IE_506_HABILITADA = "IE_506_HABILITADA";
        #endregion

        #region Recepcion
        public const string REC170_LIBERA_FACTURAS = "REC170_LIBERA_FACTURAS";

        #endregion

        #region ReferenciaRecepcion

        public const string IE_510_HABILITADA = "IE_510_HABILITADA";
        public const string IE_510_VALIDAR_FECHAS = "IE_510_VALIDAR_FECHAS";

        #endregion

        #region Reporte
        public const string REPORTE_RESOURCE_PATH = "REPORTE_RESOURCE_PATH";
        public const string REPORTE_RESOURCE_LOGO = "REPORTE_RESOURCE_LOGO";
        public const string REPORTE_BACKUP_PATH = "REPORTE_BACKUP_PATH";
        public const string REPORTE_ON_DEMAND = "REPORTE_ON_DEMAND";

        public const string REPIMP_PACKING_LIST = "REPIMP_PACKING_LIST";
        public const string REPIMP_CONTENEDORES_CAMION = "REPIMP_CONTENEDORES_CAMION";
        public const string REPIMP_CONTROL_CAMBIO = "REPIMP_CONTROL_CAMBIO";
        public const string REPIMP_CONFIRMACION_RECEPCION = "REPIMP_CONFIRMACION_RECEPCION";
        public const string REPIMP_NOTA_DEVOLUCION = "REPIMP_NOTA_DEVOLUCION";
        public const string REPIMP_PACKING_LIST_SIN_LPN = "REPIMP_PACKING_LIST_SIN_LPN";

        #endregion

        #region Tracking
        public const string TRACKING_API = "TRACKING_API";
        public const string TRACKING_SCOPE = "TRACKING_SCOPE";
        public const string TRACKING_TENANT = "TRACKING_TENANT";
        public const string TRACKING_ACTIVO = "TRACKING_ACTIVO";
        public const string TRACKING_API_WMS = "TRACKING_API_WMS";
        public const string TRACKING_API_USERS = "TRACKING_API_USERS";
        public const string TRACKING_CLIENT_ID = "TRACKING_CLIENT_ID";
        public const string TRACKING_GRANT_TYPE = "TRACKING_GRANT_TYPE";
        public const string TRACKING_CLIENT_SECRET = "TRACKING_CLIENT_SECRET";
        public const string TRACKING_AGRUPACION_CD = "TRACKING_AGRUPACION_CD";
        public const string TRACKING_TP_CONT_DEFAULT = "TRACKING_TP_CONT_DEFAULT";
        public const string TRACKING_TAREA_CANT_DIAS = "TRACKING_TAREA_CANT_DIAS";
        public const string TRACKING_ACCESS_TOKEN_URL = "TRACKING_ACCESS_TOKEN_URL";
        public const string TRACKING_TP_CONT_FICTICIO = "TRACKING_TP_CONT_FICTICIO";
        public const string TRACKING_JOB_FECHA_INICIAL = "TRACKING_JOB_FECHA_INICIAL";

        #endregion

        #region Ubicacion

        public const string REG040_PERMITE_IMPORT_UBIC = "REG040_PERMITE_IMPORT_UBIC";

        public const string WREG040_VL_LONG_PREDIO = "WREG040_VL_LONG_PREDIO";
        public const string WREG040_VL_TIPO_PREDIO = "WREG040_VL_TIPO_PREDIO";
        public const string WREG040_VL_LONG_BLOQUE = "WREG040_VL_LONG_BLOQUE";
        public const string WREG040_VL_TIPO_BLOQUE = "WREG040_VL_TIPO_BLOQUE";
        public const string WREG040_VL_LONG_CALLE = "WREG040_VL_LONG_CALLE";
        public const string WREG040_VL_TIPO_CALLE = "WREG040_VL_TIPO_CALLE";
        public const string WREG040_VL_LONG_FRENTE = "WREG040_VL_LONG_FRENTE";
        public const string WREG040_VL_LONG_ALTURA = "WREG040_VL_LONG_ALTURA";
        public const string WREG040_CD_SITUACAO = "WREG040_CD_SITUACAO";
        public const string WREG040_CD_ZONA_UBICACION = "WREG040_CD_ZONA_UBICACION";

        #endregion

        #region PickingProducto
        public const string IE_570_HABILITADA = "IE_570_HABILITADA";
        public const string IE_570_CAMPOS_INMUTABLES = "IE_570_CAMPOS_INMUTABLES";

        public const string IE_570_CODIGO_UND_CAJA_AUT = "IE_570_CODIGO_UND_CAJA_AUT";
        public const string IE_570_CANT_UND_CAJA_AUT = "IE_570_CANT_UND_CAJA_AUT";
        public const string IE_570_FL_CONFIRMAR_BARRA_AUT = "IE_570_FL_CONFIRMAR_BARRA_AUT";

        #endregion

        #region Webhooks

        public const string WEBHOOK_TIMEOUT = "WEBHOOK_TIMEOUT";
        public const string WEBHOOK_REINTENTOS = "WEBHOOK_REINTENTOS";
        public const string WEBHOOK_ESPERA_REINTENTO = "WEBHOOK_ESPERA_REINTENTO";
        public const string WEBHOOK_CAMELCASE_ENABLED = "WEBHOOK_CAMELCASE_ENABLED";
        public const string WEBHOOK_BLOQUEAR_EMPRESA = "WEBHOOK_BLOQUEAR_EMPRESA";

        #endregion

        [Obsolete]
        public static Dictionary<string, string> GetParamInterfazHabilitada(IUnitOfWork uow, string controllerName, string method, int? empresa = null, int? interfazExterna = null, string parametroHabilitacion = null)
        {
            var result = new Dictionary<string, string>();
            string param = string.Empty;
            string cdIntExt = "-1";

            if (interfazExterna.HasValue)
            {
                param = parametroHabilitacion;
                cdIntExt = interfazExterna.ToString();
            }
            else
            {
                switch (controllerName)
                {
                    //Entrada
                    case "Agente":
                        param = IE_507_HABILITADA;
                        cdIntExt = CInterfazExterna.Agentes.ToString();
                        break;
                    case "Agenda":
                        param = IE_525_HABILITADA;
                        cdIntExt = CInterfazExterna.Agendas.ToString();
                        break;
                    case "AnulacionReferenciaRecepcion":
                        param = IE_513_HABILITADA;
                        cdIntExt = CInterfazExterna.AnulacionReferenciaRecepcion.ToString();
                        break;
                    case "CodigoBarras":
                        param = IE_505_HABILITADA;
                        cdIntExt = CInterfazExterna.CodigoDeBarras.ToString();
                        break;
                    case "Empresa":
                        param = IE_522_HABILITADA;
                        cdIntExt = CInterfazExterna.Empresas.ToString();
                        break;
                    case "ModificarDetalleReferencia":
                        param = IE_520_HABILITADA;
                        cdIntExt = CInterfazExterna.ModificarDetalleReferenciaRecepcion.ToString();
                        break;
                    case "Pedido":
                        param = IE_503_HABILITADA;
                        cdIntExt = CInterfazExterna.Pedidos.ToString();
                        break;
                    case "Producto":
                        param = IE_500_HABILITADA;
                        cdIntExt = CInterfazExterna.Producto.ToString();
                        break;
                    case "ProductoProveedor":
                        param = IE_506_HABILITADA;
                        cdIntExt = CInterfazExterna.ProductoProveedor.ToString();
                        break;
                    case "ReferenciaRecepcion":
                        param = IE_510_HABILITADA;
                        cdIntExt = CInterfazExterna.ReferenciaDeRecepcion.ToString();
                        break;
                    case "ModificarPedido":
                        param = IE_585_HABILITADA;
                        cdIntExt = CInterfazExterna.ModificarPedido.ToString();
                        break;
                    case "Stock":
                        switch (method)
                        {
                            case "Ajustar":
                            case "ReprocesarAjuste":
                                param = IE_998_HABILITADA;
                                cdIntExt = CInterfazExterna.AjustarStock.ToString();
                                break;
                            case "Transferir":
                            case "ReprocesarTransferencia":
                            case "MovimientoStockAutomatismo":
                                param = IE_999_HABILITADA;
                                cdIntExt = CInterfazExterna.TransferenciaStock.ToString();
                                break;
                            default:
                                param = string.Empty;
                                cdIntExt = "-1";
                                break;
                        }
                        break;
                    case "Preparacion":
                        switch (method)
                        {
                            case "Picking":
                            case "ReprocesarPicking":
                                param = IE_997_HABILITADA;
                                cdIntExt = CInterfazExterna.Picking.ToString();
                                break;
                            case "AnularPicking":
                            case "ReprocesarAnulacionPreparacion":
                                param = IE_996_HABILITADA;
                                cdIntExt = CInterfazExterna.AnulacionPicking.ToString();
                                break;
                            case "AnularPickingPedidoPendiente":
                            case "AnularPickingPedidoPendienteAutomatismo":
                            case "ReprocesarAnulacionPreparacionPedidoPicking":
                                param = IE_990_HABILITADA;
                                cdIntExt = CInterfazExterna.AnularPickingPedidoPendiente.ToString();
                                break;
                            case "SepararPicking":
                            case "ReprocesarSeparacion":
                                param = IE_995_HABILITADA;
                                cdIntExt = CInterfazExterna.SepararPicking.ToString();
                                break;
                            case "CambiarContenedor":
                            case "ReprocesarCambioContenedor":
                                param = IE_980_HABILITADA;
                                cdIntExt = CInterfazExterna.CambiarContenedor.ToString();
                                break;

                            default:
                                param = string.Empty;
                                cdIntExt = "-1";
                                break;

                        }
                        break;
                    case "Produccion":
                        param = IE_700_HABILITADA;
                        cdIntExt = CInterfazExterna.Produccion.ToString();
                        break;
                    case "ProducirProduccion":
                        param = IE_701_HABILITADA;
                        cdIntExt = CInterfazExterna.ProducirProduccion.ToString();
                        break;
                    case "ConsumirProduccion":
                        param = IE_702_HABILITADA;
                        cdIntExt = CInterfazExterna.ConsumirProduccion.ToString();
                        break;
                    case "CrossDocking":
                        switch (method)
                        {
                            case "CrossDockingUnaFase":
                            case "ReprocesarCrossDockingUnaFase":
                                param = IE_994_HABILITADA;
                                cdIntExt = CInterfazExterna.CrossDockingUnaFase.ToString();
                                break;
                            /*case "CrossDockingDosFases":
                            case "ReprocesarCrossDockingDosFases":
                                param = ;
                                cdIntExt = ;
                                break;*/
                            default:
                                param = string.Empty;
                                cdIntExt = "-1";
                                break;

                        }
                        break;
                    case "Egreso":
                        param = IE_530_HABILITADA;
                        cdIntExt = CInterfazExterna.Egresos.ToString();
                        break;
                    case "Lpn":
                        param = IE_535_HABILITADA;
                        cdIntExt = CInterfazExterna.Lpn.ToString();
                        break;
                    case "Factura":
                        param = IE_425_HABILITADA;
                        cdIntExt = CInterfazExterna.Facturas.ToString();
                        break;
                    case "PickingProducto":
                        param = IE_570_HABILITADA;
                        cdIntExt = CInterfazExterna.PickingProducto.ToString();
                        break;

                    //Salida
                    case "ConfirmacionDeRecepcion":
                        param = IS_502_HABILITADA;
                        cdIntExt = CInterfazExterna.ConfirmacionDeRecepcion.ToString();
                        break;
                    case "AjustesDeStock":
                        param = IS_508_HABILITADA;
                        cdIntExt = CInterfazExterna.AjustesDeStock.ToString();
                        break;
                    case "PedidosAnulados":
                        param = IS_509_HABILITADA;
                        cdIntExt = CInterfazExterna.PedidosAnulados.ToString();
                        break;
                    case "ConfirmacionDePedido":
                        param = IS_512_HABILITADA;
                        cdIntExt = CInterfazExterna.ConfirmacionDePedido.ToString();
                        break;
                    case "Facturacion":
                        param = IS_516_HABILITADA;
                        cdIntExt = CInterfazExterna.Facturacion.ToString();
                        break;
                    case "ConsultaDeStock":
                        param = IC_601_HABILITADA;
                        cdIntExt = CInterfazExterna.ConsultaDeStock.ToString();
                        break;
                    case "Almacenamiento":
                        param = IS_526_HABILITADA;
                        cdIntExt = CInterfazExterna.Almacenamiento.ToString();
                        break;
                    case "ControlDeCalidad":
                        param = IE_540_HABILITADA;
                        cdIntExt = CInterfazExterna.ControlDeCalidad.ToString();
                        break;
                    case "Salida":
                        param = string.Empty;
                        cdIntExt = "-1";
                        break;
                    case "ConfirmacionDeProduccion":
                        param = IS_790_HABILITADA;
                        cdIntExt = CInterfazExterna.ConfirmacionProduccion.ToString();
                        break;

                    default:
                        param = CUSTOM_API_MAPPING;
                        cdIntExt = "-1";
                        break;

                }
            }

            if (param != CUSTOM_API_MAPPING)
            {
                if (empresa == null || string.IsNullOrEmpty(param))
                {
                    return new Dictionary<string, string>()
                    {
                        { "interfazHabilitada", "N" },
                        { "cdInterfazExterna", cdIntExt}
                    };
                }
                else
                {
                    string interfazHabilitada = uow.ParametroRepository.GetParametro(param, ParamManager.PARAM_EMPR, empresa.ToString()).Result ?? "N";

                    return new Dictionary<string, string>()
                    {
                        { "interfazHabilitada", interfazHabilitada },
                        { "cdInterfazExterna", cdIntExt}
                    };
                }
            }
            else
            {
                var vlParametro = uow.ParametroRepository.GetParametro(param, ParamManager.PARAM_EMPR, empresa?.ToString()).Result ?? null;

                if (string.IsNullOrEmpty(vlParametro))
                    return null;
                else
                    param = string.Empty;

                var datosApis = vlParametro.Split('@', StringSplitOptions.RemoveEmptyEntries);

                foreach (var datosApi in datosApis)
                {
                    var datos = datosApi.Split('#', StringSplitOptions.RemoveEmptyEntries);
                    if (controllerName == datos[0])
                    {
                        param = datos[1];
                        cdIntExt = datos[2];
                        break;
                    }
                }

                if (string.IsNullOrEmpty(param))
                    return null;
                else
                {
                    string interfazHabilitada = uow.ParametroRepository.GetParametro(param, ParamManager.PARAM_EMPR, empresa.ToString()).Result ?? "N";

                    return new Dictionary<string, string>()
                    {
                        { "interfazHabilitada", interfazHabilitada },
                        { "cdInterfazExterna", cdIntExt.ToString() }
                    };
                }
            }
        }

        public static bool GetParamInterfazHabilitada(IUnitOfWork uow, int codigoInterfazExterna, int empresa, string parametroHabilitacion = null)
        {
            string param = string.Empty;

            if (!string.IsNullOrEmpty(parametroHabilitacion))
                param = parametroHabilitacion;
            else
            {
                var interfazExterna = uow.InterfazRepository.GetInterfazExterna(codigoInterfazExterna);

                if (interfazExterna != null && !string.IsNullOrEmpty(interfazExterna.ParametroDeHabilitacion))
                    param = interfazExterna.ParametroDeHabilitacion;
                else
                    param = CUSTOM_API_MAPPING;
            }

            if (param != CUSTOM_API_MAPPING)
            {
                return (uow.ParametroRepository.GetParametro(param, ParamManager.PARAM_EMPR, empresa.ToString()).Result ?? "N") == "S";
            }
            else
            {
                var vlParametro = uow.ParametroRepository.GetParametro(param, ParamManager.PARAM_EMPR, empresa.ToString() ?? "1").Result ?? null;

                if (string.IsNullOrEmpty(vlParametro))
                    return false;
                else
                    param = string.Empty;

                var datosApis = vlParametro.Split('@', StringSplitOptions.RemoveEmptyEntries);

                foreach (var datosApi in datosApis)
                {
                    var datos = datosApi.Split('#', StringSplitOptions.RemoveEmptyEntries);
                    if (codigoInterfazExterna.ToString() == datos[2])
                    {
                        param = datos[1];
                        break;
                    }
                }

                if (string.IsNullOrEmpty(param))
                    return false;
                else
                    return (uow.ParametroRepository.GetParametro(param, ParamManager.PARAM_EMPR, empresa.ToString()).Result ?? "N") == "S";
            }
        }

        public static int? GetCodigoInterfazExternaByControllerName(IUnitOfWork uow, string controllerName, string method, int? empresa)
        {
            int? cdIntExt = null;

            switch (controllerName)
            {
                // Entrada
                case "Agente":
                    cdIntExt = CInterfazExterna.Agentes;
                    break;
                case "Agenda":
                    cdIntExt = CInterfazExterna.Agendas;
                    break;
                case "AnulacionReferenciaRecepcion":
                    cdIntExt = CInterfazExterna.AnulacionReferenciaRecepcion;
                    break;
                case "CodigoBarras":
                    cdIntExt = CInterfazExterna.CodigoDeBarras;
                    break;
                case "Empresa":
                    cdIntExt = CInterfazExterna.Empresas;
                    break;
                case "ModificarDetalleReferencia":
                    cdIntExt = CInterfazExterna.ModificarDetalleReferenciaRecepcion;
                    break;
                case "Pedido":
                    cdIntExt = CInterfazExterna.Pedidos;
                    break;
                case "Producto":
                    cdIntExt = CInterfazExterna.Producto;
                    break;
                case "ProductoProveedor":
                    cdIntExt = CInterfazExterna.ProductoProveedor;
                    break;
                case "ReferenciaRecepcion":
                    cdIntExt = CInterfazExterna.ReferenciaDeRecepcion;
                    break;
                case "ModificarPedido":
                    cdIntExt = CInterfazExterna.ModificarPedido;
                    break;
                case "Stock":
                    switch (method)
                    {
                        case "Ajustar":
                        case "ReprocesarAjuste":
                            cdIntExt = CInterfazExterna.AjustarStock;
                            break;
                        case "Transferir":
                        case "ReprocesarTransferencia":
                        case "MovimientoStockAutomatismo":
                            cdIntExt = CInterfazExterna.TransferenciaStock;
                            break;
                    }
                    break;
                case "Preparacion":
                    switch (method)
                    {
                        case "Picking":
                        case "ReprocesarPicking":
                            cdIntExt = CInterfazExterna.Picking;
                            break;
                        case "AnularPicking":
                        case "ReprocesarAnulacionPreparacion":
                            cdIntExt = CInterfazExterna.AnulacionPicking;
                            break;
                        case "AnularPickingPedidoPendiente":
                        case "AnularPickingPedidoPendienteAutomatismo":
                        case "ReprocesarAnulacionPreparacionPedidoPicking":
                            cdIntExt = CInterfazExterna.AnularPickingPedidoPendiente;
                            break;
                        case "SepararPicking":
                        case "ReprocesarSeparacion":
                            cdIntExt = CInterfazExterna.SepararPicking;
                            break;
                        case "CambiarContenedor":
                        case "ReprocesarCambioContenedor":
                            cdIntExt = CInterfazExterna.CambiarContenedor;
                            break;
                    }
                    break;
                case "Produccion":
                    cdIntExt = CInterfazExterna.Produccion;
                    break;
                case "ProducirProduccion":
                    cdIntExt = CInterfazExterna.ProducirProduccion;
                    break;
                case "ConsumirProduccion":
                    cdIntExt = CInterfazExterna.ConsumirProduccion;
                    break;
                case "CrossDocking":
                    switch (method)
                    {
                        case "CrossDockingUnaFase":
                        case "ReprocesarCrossDockingUnaFase":
                            cdIntExt = CInterfazExterna.CrossDockingUnaFase;
                            break;
                    }
                    break;
                case "Egreso":
                    cdIntExt = CInterfazExterna.Egresos;
                    break;
                case "Lpn":
                    cdIntExt = CInterfazExterna.Lpn;
                    break;
                case "Factura":
                    cdIntExt = CInterfazExterna.Facturas;
                    break;
                case "PickingProducto":
                    cdIntExt = CInterfazExterna.PickingProducto;
                    break;

                // Salida
                case "ConfirmacionDeRecepcion":
                    cdIntExt = CInterfazExterna.ConfirmacionDeRecepcion;
                    break;
                case "AjustesDeStock":
                    cdIntExt = CInterfazExterna.AjustesDeStock;
                    break;
                case "PedidosAnulados":
                    cdIntExt = CInterfazExterna.PedidosAnulados;
                    break;
                case "ConfirmacionDePedido":
                    cdIntExt = CInterfazExterna.ConfirmacionDePedido;
                    break;
                case "Facturacion":
                    cdIntExt = CInterfazExterna.Facturacion;
                    break;
                case "ConsultaDeStock":
                    cdIntExt = CInterfazExterna.ConsultaDeStock;
                    break;
                case "Almacenamiento":
                    cdIntExt = CInterfazExterna.Almacenamiento;
                    break;
                case "ControlDeCalidad":
                    cdIntExt = CInterfazExterna.ControlDeCalidad;
                    break;
                case "Salida":
                    cdIntExt = null;
                    break;
                case "ConfirmacionDeProduccion":
                    cdIntExt = CInterfazExterna.ConfirmacionProduccion;
                    break;
            }

            if (!cdIntExt.HasValue)
            {
                var vlParametro = uow.ParametroRepository.GetParametro(CUSTOM_API_MAPPING, ParamManager.PARAM_EMPR, empresa?.ToString() ?? "0").Result ?? null;

                if (string.IsNullOrEmpty(vlParametro))
                    return null;

                var datosApis = vlParametro.Split('@', StringSplitOptions.RemoveEmptyEntries);

                foreach (var datosApi in datosApis)
                {
                    var datos = datosApi.Split('#', StringSplitOptions.RemoveEmptyEntries);
                    if (controllerName == datos[0])
                    {
                        if (int.TryParse(datos[2], out int parsedValue))
                        {
                            cdIntExt = parsedValue;
                            break;
                        }
                    }
                }
            }

            return cdIntExt;
        }

        public virtual string GetParamInterfazHabilitada(IUnitOfWork uow, int codigoInterfazExterna, int empresa)
        {
            var interfazExterna = uow.InterfazRepository.GetInterfazExterna(codigoInterfazExterna);

            if (interfazExterna != null && !string.IsNullOrEmpty(interfazExterna.ParametroDeHabilitacion))
            {
                return interfazExterna.ParametroDeHabilitacion;
            }
            else
            {
                var vlParametro = uow.ParametroRepository.GetParametro(CUSTOM_API_MAPPING, ParamManager.PARAM_EMPR, empresa.ToString()).Result ?? null;

                if (string.IsNullOrEmpty(vlParametro))
                    return null;

                var datosApis = vlParametro.Split('@', StringSplitOptions.RemoveEmptyEntries);

                foreach (var datosApi in datosApis)
                {
                    if (datosApi.Contains(codigoInterfazExterna.ToString()))
                    {
                        var datos = datosApi.Split('#', StringSplitOptions.RemoveEmptyEntries);
                        return datos[1];
                    }
                }
            }

            return null;
        }

        public static List<string> GetParametrosInterfaces(IUnitOfWork uow)
        {
            return uow.InterfazRepository.GetInterfacesExternas()
                .Where(x => !string.IsNullOrEmpty(x.ParametroDeHabilitacion))
                .Select(x => x.ParametroDeHabilitacion)
                .ToList();
        }
    }
}
