using IdentityModel.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using WIS.Configuration;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence;
using WIS.Persistence.Database;
using WIS.XmlData.WInterface.Models;
using WIS.XmlData.WInterface.Services.Interfaces;

namespace WIS.XmlData.WInterface.Helpers
{
    public class XmlDataExternalManager : IXmlDataExternalManager
    {
        protected const string ARCHIVO_NAME = "WIS-WEBSERVICE";
        protected const string PARAM_IEX = "PARAM_IEX";
        protected const string CD_INTERFAZ_TP_OBJECTO_FILE = "FILE";

        protected readonly IConfiguration _configuration;
        protected readonly ILogger<XmlDataExternalManager> _logger;
        protected readonly IOptions<DatabaseSettings> _dbSettings;
        protected readonly IDatabaseConfigurationService _dbConfigService;
        protected readonly IDapper _dapper;
        protected readonly IXmlDataQueryManager _queryManager;

        protected enum XDEstado { SESION, INCIO_EJECUCION, ENVIO_DATOS, CONSULTAR_ESTADO, EJECUCIONES_PENDIENTES, CONSULTAR_EJECUCION, EJECUCION_LEIDA, CONSULTAR_DATOS, MENSAJE }

        public XmlDataExternalManager(IConfiguration configuration,
            ILogger<XmlDataExternalManager> logger,
            IOptions<DatabaseSettings> dbSettings,
            IDatabaseConfigurationService dbConfigService,
            IDapper dapper,
            IXmlDataQueryManager queryManager)
        {
            _configuration = configuration;
            _logger = logger;
            _dbSettings = dbSettings;
            _dbConfigService = dbConfigService;
            _dapper = dapper;
            _queryManager = queryManager;
        }

        #region Session

        public virtual async Task<RESPUESTA_INTERFAZ> Sesion(INTERFAZ_SESSION interfaz)
        {
            RESPUESTA_INTERFAZ respuesta;

            try
            {
                if (interfaz == null)
                {
                    throw new XDInterfazException(XDCDError.ENTIDAD_NULL);
                }

                if (interfaz.SESION != "I" && interfaz.SESION != "F")
                {
                    throw new XDInterfazException(XDCDError.SESION_NODO_ERROR);
                }

                var clientId = interfaz.ID_USER;
                var clientSecret = HttpUtility.HtmlDecode(interfaz.PASSWORD);

                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                {
                    throw new XDInterfazException(XDCDError.NO_LOGIN);
                }

                respuesta = new RESPUESTA_INTERFAZ(EResultado.OK);

                if (interfaz.SESION == "I")
                {
                    var address = _configuration.GetValue<string>("AuthSettings:TokenUrl");
                    var scope = _configuration.GetValue<string>("AuthSettings:AccessScope");
                    var token = await GetTokenAsync(address, clientId, clientSecret, scope);
                    respuesta.TOKEN = token;
                }
            }
            catch (XDInterfazException ex)
            {
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddErrores(ex.Errores);
            }
            catch (Exception ex)
            {
                var err = GetErrorGenerico(interfaz, ex);
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddError(err);
            }

            return respuesta;
        }

        #endregion

        #region Entrada

        public virtual async Task<RESPUESTA_INTERFAZ> Inciar_Ejecucion(INTERFAZ interfaz)
        {
            _logger.LogDebug("============COMIENZO INICIAR_EJECUCION===========");

            RESPUESTA_INTERFAZ respuesta = null;
            long NU_INTERFAZ_EJECUCION = -1;
            bool hay_error = false;

            try
            {
                if (interfaz == null)
                {
                    throw new XDInterfazException(XDCDError.ENTIDAD_NULL);
                }

                var tHelper = new TransactionHelper();

                using (var context = GetNewDbContext())
                using (var tran = tHelper.BeginTransaction(context))
                {
                    var loginName = await Token_Valido(context, interfaz.TOKEN);
                    var nodes_interfaz = GetValidNodes(interfaz, XDEstado.INCIO_EJECUCION);
                    var curInterfaz = context.T_INTERFAZ.FirstOrDefault(x => x.CD_INTERFAZ == nodes_interfaz.CD_INTERFAZ_EXTERNA);

                    if (curInterfaz == null)
                        throw new XDNodeValidateException(XDCDError.CD_INTERFAZ_EXTERNA_INVALIDA);

                    Validate_Interfaz(context, loginName, nodes_interfaz, XDEstado.INCIO_EJECUCION);

                    string ds_referencia_unico = ParamManager.GetParamValue(context, ParamManager.INTERFAZ_DS_REFERENCIA_UNICO, new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>(PARAM_IEX, string.Format("{0}_{1}", PARAM_IEX, interfaz.CD_INTERFAZ_EXTERNA))
                        })?.ToString();

                    if (string.IsNullOrEmpty(ds_referencia_unico))
                    {
                        throw new XDInterfazException(XDCDError.FALTA_PARAMETRO_EN_DB, ParamManager.INTERFAZ_DS_REFERENCIA_UNICO);
                    }

                    if (ds_referencia_unico != "N")
                    {
                        T_INTERFAZ_EJECUCION ejec_tmp = context.T_INTERFAZ_EJECUCION.FirstOrDefault(x => x.DS_REFERENCIA == interfaz.DS_REFERENCIA);
                        if (ejec_tmp != null)
                        {
                            throw new XDInterfazException(XDCDError.DS_REFERENCIA_REPETIDA, ejec_tmp.NU_INTERFAZ_EJECUCION.ToString());
                        }
                    }

                    _logger.LogDebug("Iniciando Ejecución");

                    T_INTERFAZ_EJECUCION intfz = INCIAR_EJECUCION_INTERFAZ(context, loginName, nodes_interfaz.CD_INTERFAZ_EXTERNA, ARCHIVO_NAME, interfaz.DS_REFERENCIA, nodes_interfaz.TOKEN);
                    NU_INTERFAZ_EJECUCION = intfz.NU_INTERFAZ_EJECUCION;

                    _logger.LogDebug("Interfaz: " + intfz.NU_INTERFAZ_EJECUCION);

                    context.SaveChanges();

                    Save_INTERFAZ_EJECION_DATEXT(context, NU_INTERFAZ_EJECUCION, nodes_interfaz, true);
                    Save_INTERFAZ_EJECUCION_DATEXTDET(context, NU_INTERFAZ_EJECUCION, nodes_interfaz, true);

                    context.SaveChanges();

                    Save_INTERFAZ_EJECUCION_DATA(context, NU_INTERFAZ_EJECUCION, nodes_interfaz, true);

                    respuesta = new RESPUESTA_INTERFAZ(EResultado.PENDIENTE);
                    respuesta.NU_INTERFAZ_EJECUCION = NU_INTERFAZ_EJECUCION.ToString();

                    context.SaveChanges();

                    if (curInterfaz.ID_ENTRADA_SALIDA.Equals("A"))
                    {
                        _logger.LogDebug("Salida A");

                        intfz.CD_SITUACAO = SituacionDb.ArchivoRespaldado;
                        intfz = INCIAR_EJECUCION_INTERFAZ(context, loginName, nodes_interfaz.CD_INTERFAZ_EXTERNA, ARCHIVO_NAME, "Interfaz creada a partir de:" + intfz.NU_INTERFAZ_EJECUCION, nodes_interfaz.TOKEN);

                        //DATA
                        if (!nodes_interfaz.DATA.IsNullOrEmpty())
                        {
                            string str_xml = Base64ToXmlString(nodes_interfaz.DATA);
                            T_INTERFAZ_EJECUCION_DATA data = context.T_INTERFAZ_EJECUCION_DATA.FirstOrDefault(x => x.NU_INTERFAZ_EJECUCION == NU_INTERFAZ_EJECUCION);
                            byte[] array = ByteString.GetBytes(str_xml);

                            data.DATA = array;
                            context.SaveChanges();
                        }

                        intfz.CD_EMPRESA = int.Parse(interfaz.CD_EMPRESA);

                        respuesta = new RESPUESTA_INTERFAZ(EResultado.PENDIENTE);
                        respuesta.NU_INTERFAZ_EJECUCION = intfz.NU_INTERFAZ_EJECUCION.ToString();

                        context.SaveChanges();
                    }

                    tran.Commit();
                }
            }
            catch (XDTokenException ex)
            {
                _logger.LogError("ERROR");
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddErrores(ex.Errores);
            }
            catch (XDNodeValidateException ex)
            {
                _logger.LogError("ERROR");
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddErrores(ex.Errores);
            }
            catch (XDInterfazException ex)
            {
                _logger.LogError("ERROR");
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddErrores(ex.Errores);
                hay_error = true;
            }
            catch (Exception ex)
            {
                var err = GetErrorGenerico(interfaz, ex);
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddError(err);
            }

            if (hay_error)
            {
                SaveErrorInterfaz(NU_INTERFAZ_EJECUCION, respuesta.ERRORES);
            }

            return respuesta;
        }

        public virtual async Task<RESPUESTA_INTERFAZ> Enviar_Datos(INTERFAZ interfaz)
        {
            _logger.LogDebug("============COMIENZO ENVIAR_DATOS===========");

            RESPUESTA_INTERFAZ respuesta = null;
            long NU_INTERFAZ_EJECUCION = -1;
            bool hay_error = false;

            try
            {
                if (interfaz == null)
                {
                    throw new XDInterfazException(XDCDError.ENTIDAD_NULL);
                }

                var tHelper = new TransactionHelper();

                using (var context = GetNewDbContext())
                using (var tran = tHelper.BeginTransaction(context))
                {
                    var loginName = await Token_Valido(context, interfaz.TOKEN);
                    ValidNodes nodes_interfaz = GetValidNodes(interfaz, XDEstado.ENVIO_DATOS);
                    RUsuario repoU = new RUsuario();
                    USERS usuario = repoU.GetUserByLoginName(context, loginName);

                    _logger.LogDebug("============User by token: " + usuario.USERID + "===========");

                    NU_INTERFAZ_EJECUCION = nodes_interfaz.NU_INTERFAZ_EJECUCION;

                    Validate_Interfaz(context, loginName, nodes_interfaz, XDEstado.ENVIO_DATOS);

                    _logger.LogDebug("Fin Validate_Interfaz");

                    T_INTERFAZ_EJECUCION interfaz_ejecucion = context.T_INTERFAZ_EJECUCION.FirstOrDefault(x => x.NU_INTERFAZ_EJECUCION == nodes_interfaz.NU_INTERFAZ_EJECUCION);

                    try
                    {
                        _logger.LogDebug("Tiene grupo consulta asignado");

                        if (ContainsGrupoConsulta(context, usuario.USERID, interfaz_ejecucion.CD_GRUPO_CONSULTA))
                        {
                            _logger.LogDebug("Paso grupo consulta...Guardando Datext");

                            Save_INTERFAZ_EJECION_DATEXT(context, NU_INTERFAZ_EJECUCION, nodes_interfaz, false);
                            context.SaveChanges();

                            _logger.LogDebug("Guardando Datextdet");

                            Save_INTERFAZ_EJECUCION_DATEXTDET(context, NU_INTERFAZ_EJECUCION, nodes_interfaz, false);
                            context.SaveChanges();

                            _logger.LogDebug("Guardando Data");

                            Save_INTERFAZ_EJECUCION_DATA(context, NU_INTERFAZ_EJECUCION, nodes_interfaz, false);
                            context.SaveChanges();

                            respuesta = new RESPUESTA_INTERFAZ(EResultado.PENDIENTE);
                            respuesta.MENSAJE = "";
                            respuesta.NU_INTERFAZ_EJECUCION = interfaz.NU_INTERFAZ_EJECUCION;
                        }

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("ERROR: " + ex.Message + "//" + ex.InnerException);
                        context.SaveChanges();
                        tran.Commit();
                        throw ex;
                    }
                }
            }
            catch (XDTokenException ex)
            {
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.NU_INTERFAZ_EJECUCION = interfaz.NU_INTERFAZ_EJECUCION;
                respuesta.AddErrores(ex.Errores);
            }
            catch (XDNodeValidateException ex)
            {
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddErrores(ex.Errores);
            }
            catch (XDInterfazException ex)
            {
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.NU_INTERFAZ_EJECUCION = interfaz.NU_INTERFAZ_EJECUCION;
                respuesta.AddErrores(ex.Errores);
                hay_error = true;
            }
            catch (Exception ex)
            {
                var err = GetErrorGenerico(interfaz, ex);
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddError(err);
            }

            if (hay_error)
            {
                SaveErrorInterfaz(NU_INTERFAZ_EJECUCION, respuesta.ERRORES);
            }

            return respuesta;
        }

        public virtual async Task<RESPUESTA_INTERFAZ> Consultar_Estado(INTERFAZ interfaz)
        {
            List<int> interfacesEntrada = new List<int>();
            RESPUESTA_INTERFAZ respuesta = null;
            long NU_INTERFAZ_EJECUCION = -1;
            bool hay_error = false;

            try
            {
                if (interfaz == null)
                {
                    throw new XDInterfazException(XDCDError.ENTIDAD_NULL);
                }

                var tHelper = new TransactionHelper();

                using (var context = GetNewDbContext())
                using (var tran = tHelper.BeginTransaction(context))
                {
                    interfacesEntrada = context.T_INTERFAZ.Where(x => x.ID_ENTRADA_SALIDA == "E").Select(s => s.CD_INTERFAZ).ToList();

                    var loginName = await Token_Valido(context, interfaz.TOKEN);
                    ValidNodes nodes_interfaz = GetValidNodes(interfaz, XDEstado.CONSULTAR_ESTADO);

                    NU_INTERFAZ_EJECUCION = nodes_interfaz.NU_INTERFAZ_EJECUCION;

                    Validate_Interfaz(context, loginName, nodes_interfaz, XDEstado.CONSULTAR_ESTADO);

                    T_INTERFAZ_EJECUCION interfaz_ejecucion = context.T_INTERFAZ_EJECUCION.FirstOrDefault(x => x.NU_INTERFAZ_EJECUCION == NU_INTERFAZ_EJECUCION);
                    RUsuario repoU = new RUsuario();
                    USERS usuario = repoU.GetUserByLoginName(context, loginName);
                    bool hay_errores = true;

                    if (ContainsGrupoConsulta(context, usuario.USERID, interfaz_ejecucion.CD_GRUPO_CONSULTA))
                    {
                        if (interfaz_ejecucion.FL_ERROR_CARGA != "S" && interfaz_ejecucion.FL_ERROR_PROCEDIMIENTO != "S")
                        {
                            hay_errores = false;
                        }

                        if (interfaz_ejecucion.CD_SITUACAO == SituacionDb.EjecucionIniciada || interfaz_ejecucion.CD_SITUACAO == SituacionDb.ArchivoProcesado || (interfaz_ejecucion.CD_SITUACAO == SituacionDb.ProcesandoInterfaz && interfacesEntrada.Contains((int)interfaz_ejecucion.CD_INTERFAZ_EXTERNA)))
                        {
                            respuesta = new RESPUESTA_INTERFAZ(EResultado.PENDIENTE);
                            respuesta.MENSAJE = "";
                            respuesta.NU_INTERFAZ_EJECUCION = interfaz.NU_INTERFAZ_EJECUCION;
                        }
                        else
                        {
                            if (!hay_errores)
                            {
                                respuesta = new RESPUESTA_INTERFAZ(EResultado.OK);
                                respuesta.MENSAJE = "";
                                respuesta.NU_INTERFAZ_EJECUCION = interfaz.NU_INTERFAZ_EJECUCION;
                            }
                            else
                            {
                                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                                respuesta.MENSAJE = "Se encontraron Errores";
                                respuesta.NU_INTERFAZ_EJECUCION = interfaz.NU_INTERFAZ_EJECUCION;

                                List<T_INTERFAZ_EJECUCION_ERROR> errores = context.T_INTERFAZ_EJECUCION_ERROR.Where(x => x.NU_INTERFAZ_EJECUCION == NU_INTERFAZ_EJECUCION).ToList();
                                if (errores != null && errores.Count > 0)
                                {
                                    errores = errores.OrderBy(x => x.NU_ERROR).ToList();
                                    foreach (T_INTERFAZ_EJECUCION_ERROR error in errores)
                                    {
                                        ERROR xd_error = new ERROR();
                                        xd_error.CD_ERROR = error.CD_ERROR;
                                        xd_error.DS_ERROR = error.DS_ERROR + "//Registro: " + error.NU_REGISTRO;
                                        xd_error.NU_ERROR = error.NU_ERROR;
                                        respuesta.ERRORES.Add(xd_error);
                                    }
                                }
                            }
                        }
                    }

                    context.SaveChanges();
                    tran.Commit();
                }
            }
            catch (XDTokenException ex)
            {
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddErrores(ex.Errores);
                respuesta.NU_INTERFAZ_EJECUCION = interfaz.NU_INTERFAZ_EJECUCION;
            }
            catch (XDNodeValidateException ex)
            {
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddErrores(ex.Errores);
            }
            catch (XDInterfazException ex)
            {
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.MENSAJE = "ERROR";
                respuesta.AddErrores(ex.Errores);
                respuesta.NU_INTERFAZ_EJECUCION = interfaz.NU_INTERFAZ_EJECUCION;
                hay_error = true;
            }
            catch (Exception ex)
            {
                var err = GetErrorGenerico(interfaz, ex);
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddError(err);
            }

            return respuesta;
        }

        #endregion

        #region Salida

        public virtual async Task<RESPUESTA_INTERFAZ> Ejecuciones_Pendientes(INTERFAZ interfaz)
        {
            List<int> interfacesAdmitidas = new List<int>
            {
                CInterfazExterna.AjustesDeStock,
                CInterfazExterna.ConfirmacionDePedido,
                CInterfazExterna.Facturacion,
                CInterfazExterna.ConfirmacionDeRecepcion,
                CInterfazExterna.PedidosAnulados,
                CInterfazExterna.ConsultaDeStock,
            };

            RESPUESTA_INTERFAZ respuesta = new RESPUESTA_INTERFAZ(EResultado.OK);

            try
            {
                CSTransfer transfer = new CSTransfer();
                CSManager manager = new CSManager(_logger, _configuration);
                transfer.jsonData = JsonConvert.SerializeObject(interfacesAdmitidas);
                transfer = manager.InvokeInterfacesCustomMethod(ECSInterfacesMethods.INTERFAZ_CargarListaInterfacesCustom, transfer);

                if (transfer.status != CSResult.NOT_IMPLEMENTED)
                {
                    if (transfer.status == CSResult.ERROR)
                        throw new CMNException(transfer.errorMessage);

                    interfacesAdmitidas = JsonConvert.DeserializeObject<List<int>>(transfer.jsonData);
                }

                if (interfaz == null)
                {
                    throw new XDInterfazException(XDCDError.ENTIDAD_NULL);
                }

                using (var context = GetNewDbContext())
                {
                    var loginName = await Token_Valido(context, interfaz.TOKEN);
                    ValidNodes nodes_interfaz = GetValidNodes(interfaz, XDEstado.EJECUCIONES_PENDIENTES);
                    RUsuario repoU = new RUsuario();
                    USERS usuario = repoU.GetUserByLoginName(context, loginName);
                    List<T_INTERFAZ_EJECUCION> lst_interfaz_ejecucion = context.T_INTERFAZ_EJECUCION
                        .Where(x => x.CD_EMPRESA == nodes_interfaz.CD_EMPRESA
                            && x.FL_ERROR_CARGA != "S"
                            && x.CD_SITUACAO == SituacionDb.ProcesandoInterfaz
                            && interfacesAdmitidas.Contains(x.CD_INTERFAZ_EXTERNA ?? -1))
                        .ToList();

                    if (lst_interfaz_ejecucion != null)
                    {
                        foreach (T_INTERFAZ_EJECUCION ejecucion in lst_interfaz_ejecucion)
                        {
                            if (ContainsGrupoConsulta(context, usuario.USERID, ejecucion.CD_GRUPO_CONSULTA))
                            {
                                EJECUCION ejecXML = new EJECUCION();
                                ejecXML.NU_INTERFAZ_EJECUCION = ejecucion.NU_INTERFAZ_EJECUCION;
                                ejecXML.CD_INTERFAZ_EXTERNA = (int)ejecucion.CD_INTERFAZ_EXTERNA;
                                respuesta.EJECUCIONES.Add(ejecXML);
                            }
                        }
                    }
                }
            }
            catch (XDTokenException ex)
            {
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddErrores(ex.Errores);
            }
            catch (XDNodeValidateException ex)
            {
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddErrores(ex.Errores);
            }
            catch (XDInterfazException ex)
            {
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddErrores(ex.Errores);
            }
            catch (Exception ex)
            {
                var err = GetErrorGenerico(interfaz, ex);
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddError(err);
            }

            return respuesta;
        }

        public virtual async Task<RESPUESTA_INTERFAZ> Consultar_Ejecucion(INTERFAZ interfaz)
        {
            _logger.LogDebug("============COMIENZO CONSULTAR_EJECUCION===========");

            RESPUESTA_INTERFAZ respuesta = new RESPUESTA_INTERFAZ(EResultado.OK);

            try
            {
                if (interfaz == null)
                {
                    throw new XDInterfazException(XDCDError.ENTIDAD_NULL);
                }

                using (var context = GetNewDbContext())
                {
                    var loginName = await Token_Valido(context, interfaz.TOKEN);
                    ValidNodes nodes_interfaz = GetValidNodes(interfaz, XDEstado.CONSULTAR_EJECUCION);
                    T_INTERFAZ_EJECUCION interfaz_ejecucion = context.T_INTERFAZ_EJECUCION.FirstOrDefault(x => x.NU_INTERFAZ_EJECUCION == nodes_interfaz.NU_INTERFAZ_EJECUCION);

                    Validate_NU_INTERFAZ_EJECUCION_OUT(interfaz_ejecucion);

                    T_INTERFAZ_EJECUCION_DATEXT datext = context.T_INTERFAZ_EJECUCION_DATEXT.FirstOrDefault(x => x.NU_INTERFAZ_EJECUCION == nodes_interfaz.NU_INTERFAZ_EJECUCION);
                    if (datext == null)
                    {
                        throw new XDTokenException(XDCDError.PAQUETE_SIN_INFORMACION);
                    }

                    T_INTERFAZ_EJECUCION_DATEXTDET datext_det = context.T_INTERFAZ_EJECUCION_DATEXTDET
                        .FirstOrDefault(x => x.NU_INTERFAZ_EJECUCION == nodes_interfaz.NU_INTERFAZ_EJECUCION
                            && x.NU_PAQUETE == nodes_interfaz.NU_PAQUETE);

                    if (datext_det == null)
                    {
                        throw new XDTokenException(XDCDError.PAQUETE_SIN_INFORMACION);
                    }

                    RUsuario repoU = new RUsuario();
                    USERS usuario = repoU.GetUserByLoginName(context, loginName);

                    _logger.LogDebug("============USUARIO: " + usuario.LOGINNAME + "===========");

                    if (ContainsGrupoConsulta(context, usuario.USERID, interfaz_ejecucion.CD_GRUPO_CONSULTA))
                    {
                        _logger.LogDebug("============USUARIO: " + usuario.LOGINNAME + "===========");
                        _logger.LogDebug("============CONSUME INTERFAZ: " + interfaz.NU_INTERFAZ_EJECUCION + "===========");
                        _logger.LogDebug("============NU_PAQUETE: " + datext_det.NU_PAQUETE + "===========");

                        respuesta.NU_INTERFAZ_EJECUCION = interfaz.NU_INTERFAZ_EJECUCION;
                        respuesta.CD_INTERFAZ_EXTERNA = interfaz_ejecucion.CD_INTERFAZ_EXTERNA.ToString();
                        respuesta.PAQUETE = datext_det.NU_PAQUETE.ToString();
                        respuesta.TOTAL_PAQUETES = datext.NU_TOTAL_PAQUETES.ToString();
                        respuesta.DATA = System.Text.Encoding.UTF8.GetString(datext_det.DATA);
                    }
                }
            }
            catch (XDTokenException ex)
            {
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddErrores(ex.Errores);
            }
            catch (XDNodeValidateException ex)
            {
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddErrores(ex.Errores);
            }
            catch (XDInterfazException ex)
            {
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddErrores(ex.Errores);
            }
            catch (Exception ex)
            {
                var err = GetErrorGenerico(interfaz, ex);
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddError(err);
            }

            return respuesta;
        }

        public virtual async Task<RESPUESTA_INTERFAZ> Ejecucion_Leida(INTERFAZ interfaz)
        {
            _logger.LogDebug("============COMIENZO EJECUCION_LEIDA===========");

            RESPUESTA_INTERFAZ respuesta = null;

            try
            {
                if (interfaz == null)
                {
                    throw new XDInterfazException(XDCDError.ENTIDAD_NULL);
                }

                var tHelper = new TransactionHelper();

                using (var context = GetNewDbContext())
                using (var tran = tHelper.BeginTransaction(context))
                {
                    var loginName = await Token_Valido(context, interfaz.TOKEN);
                    ValidNodes nodes_interfaz = GetValidNodes(interfaz, XDEstado.EJECUCION_LEIDA);
                    long _NU_INTERFAZ_EJECUCION = nodes_interfaz.NU_INTERFAZ_EJECUCION;
                    T_INTERFAZ_EJECUCION interfaz_ejecucion = context.T_INTERFAZ_EJECUCION.FirstOrDefault(x => x.NU_INTERFAZ_EJECUCION == _NU_INTERFAZ_EJECUCION);

                    Validate_NU_INTERFAZ_EJECUCION_OUT(interfaz_ejecucion);

                    T_INTERFAZ_EJECUCION_DATEXT datext = context.T_INTERFAZ_EJECUCION_DATEXT.FirstOrDefault(x => x.NU_INTERFAZ_EJECUCION == _NU_INTERFAZ_EJECUCION);
                    if (datext == null)
                    {
                        throw new XDTokenException(XDCDError.PAQUETE_SIN_INFORMACION);
                    }

                    if (interfaz.ERRORES != null && interfaz.ERRORES.Count > 0)
                    {
                        SaveErrorInterfaz(_NU_INTERFAZ_EJECUCION, interfaz.ERRORES, true);
                    }
                    else
                    {
                        RUsuario repoU = new RUsuario();
                        USERS usuario = repoU.GetUserByLoginName(context, loginName);

                        _logger.LogDebug("============USUARIO: " + usuario.LOGINNAME + "===========");

                        if (ContainsGrupoConsulta(context, usuario.USERID, interfaz_ejecucion.CD_GRUPO_CONSULTA))
                        {
                            _logger.LogDebug("============MARCANDO EJECUCION_LEIDA: " + interfaz.NU_INTERFAZ_EJECUCION + "::USUARIO: " + usuario.LOGINNAME + " ===========");
                            interfaz_ejecucion.CD_SITUACAO = SituacionDb.ArchivoRespaldado;
                            datext.FINALIZA_EJECUCION = "S";
                        }
                    }

                    context.SaveChanges();
                    tran.Commit();

                    respuesta = new RESPUESTA_INTERFAZ(EResultado.OK);
                    respuesta.NU_INTERFAZ_EJECUCION = interfaz.NU_INTERFAZ_EJECUCION;
                }
            }
            catch (XDTokenException ex)
            {
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddErrores(ex.Errores);
            }
            catch (XDNodeValidateException ex)
            {
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddErrores(ex.Errores);
            }
            catch (XDInterfazException ex)
            {
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddErrores(ex.Errores);
            }
            catch (Exception ex)
            {
                var err = GetErrorGenerico(interfaz, ex);
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddError(err);
            }

            return respuesta;
        }

        #endregion

        #region Consulta 

        public virtual async Task<RESPUESTA_INTERFAZ> Consultar_Datos(INTERFAZ interfaz)
        {
            RESPUESTA_INTERFAZ respuesta = null;
            bool hay_error = false;
            long NU_INTERFAZ_EJECUCION = -1;

            try
            {
                if (interfaz == null)
                {
                    throw new XDInterfazException(XDCDError.ENTIDAD_NULL);
                }

                var tHelper = new TransactionHelper();

                using (var context = GetNewDbContext())
                {
                    var loginName = await Token_Valido(context, interfaz.TOKEN);
                    var nodes_interfaz = GetValidNodes(interfaz, XDEstado.CONSULTAR_DATOS);

                    Validate_Interfaz(context, loginName, nodes_interfaz, XDEstado.CONSULTAR_DATOS);

                    _logger.LogDebug($"Iniciando Ejecución de Consulta {nodes_interfaz.CD_INTERFAZ_EXTERNA}");

                    nodes_interfaz.FINALIZA_EJECUCION = "TRUE";

                    using (var tran = tHelper.BeginTransaction(context))
                    {
                        var intfz = INCIAR_EJECUCION_INTERFAZ(context, loginName, nodes_interfaz.CD_INTERFAZ_EXTERNA, ARCHIVO_NAME, interfaz.DS_REFERENCIA, nodes_interfaz.TOKEN);

                        NU_INTERFAZ_EJECUCION = intfz.NU_INTERFAZ_EJECUCION;

                        _logger.LogDebug($"Interfaz {intfz.NU_INTERFAZ_EJECUCION} Generada para Consulta {nodes_interfaz.CD_INTERFAZ_EXTERNA}");

                        context.SaveChanges();

                        Save_INTERFAZ_EJECUCION_DATA(context, NU_INTERFAZ_EJECUCION, nodes_interfaz, true);

                        context.SaveChanges();
                        tran.Commit();
                    }

                    _logger.LogDebug($"Ejecutando Consulta {NU_INTERFAZ_EJECUCION}");

                    var xmlData = await _queryManager.GetXmlData(context, interfaz.TOKEN, loginName, nodes_interfaz.CD_INTERFAZ_EXTERNA, nodes_interfaz.CD_EMPRESA, NU_INTERFAZ_EJECUCION);

                    _logger.LogDebug($"Consulta {NU_INTERFAZ_EJECUCION} Ejecutada");

                    respuesta = new RESPUESTA_INTERFAZ();
                    respuesta.DATA = Convert.ToBase64String(ByteString.GetBytes(xmlData));
                    respuesta.RESULTADO = EResultado.OK.ToString();
                    respuesta.CD_INTERFAZ_EXTERNA = interfaz.CD_INTERFAZ_EXTERNA;
                    respuesta.NU_INTERFAZ_EJECUCION = NU_INTERFAZ_EJECUCION.ToString();

                    Change_NU_INTERFAZ_ESTADO(context, NU_INTERFAZ_EJECUCION, SituacionDb.ArchivoRespaldado);

                    context.SaveChanges();
                }
            }
            catch (XDTokenException ex)
            {
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddErrores(ex.Errores);
            }
            catch (XDNodeValidateException ex)
            {
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddErrores(ex.Errores);
            }
            catch (XDInterfazException ex)
            {
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddErrores(ex.Errores);
                hay_error = true;
            }
            catch (ApiResponseException ex)
            {
                var err = GetErrorGenericoApi(interfaz, ex);
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddError(err);
            }
            catch (Exception ex)
            {
                var err = GetErrorGenerico(interfaz, ex);
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddError(err);
            }

            if (hay_error)
            {
                SaveErrorInterfaz(NU_INTERFAZ_EJECUCION, respuesta.ERRORES);
            }

            return respuesta;
        }

        #endregion

        #region Mensaje

        public virtual async Task<RESPUESTA_INTERFAZ> Notificacion(INTERFAZ interfaz)
        {
            RESPUESTA_INTERFAZ respuesta = null;

            try
            {
                if (interfaz == null)
                {
                    throw new XDInterfazException(XDCDError.ENTIDAD_NULL);
                }

                var tHelper = new TransactionHelper();

                using (var context = GetNewDbContext())
                using (var tran = tHelper.BeginTransaction(context))
                {
                    var loginName = await Token_Valido(context, interfaz.TOKEN);
                    ValidNodes nodes_interfaz = GetValidNodes(interfaz, XDEstado.MENSAJE);
                    string xmlData = this.Base64ToXmlString(interfaz.DATA);
                    NOTIFICACIONES notificaciones = BaseSerializableXml.LoadFromXMLString<NOTIFICACIONES>(xmlData);

                    if (notificaciones != null && notificaciones.Count > 0)
                    {
                        List<ERROR> errores = new List<ERROR>();
                        int num = 1;

                        foreach (NOTIFICACION notificacion in notificaciones)
                        {
                            if (!string.IsNullOrEmpty(notificacion.DS_MENSAJE))
                            {
                                T_NOTIFICACIONES DB_NOTIFICACION = new T_NOTIFICACIONES();
                                DB_NOTIFICACION.ID_NOTIFICACION = BLSecuencia.GetNextValS_NOTIFICACIONES(context, _dapper);
                                DB_NOTIFICACION.VL_CATEGORIA = string.IsNullOrEmpty(notificacion.VL_CATEGORIA) ? "S/C" : notificacion.VL_CATEGORIA;
                                DB_NOTIFICACION.VL_NIVEL = string.IsNullOrEmpty(notificacion.VL_NIVEL) ? "BAJO" : notificacion.VL_NIVEL;
                                DB_NOTIFICACION.VL_ESTADO = string.IsNullOrEmpty(notificacion.VL_ESTADO) ? "ADD" : notificacion.VL_ESTADO;

                                DB_NOTIFICACION.DS_MENSAJE = notificacion.DS_MENSAJE;
                                if (notificacion.DS_MENSAJE.Length > 399)
                                {
                                    DB_NOTIFICACION.DS_MENSAJE = notificacion.DS_MENSAJE.Substring(0, 399);
                                }

                                DB_NOTIFICACION.VL_SERIALIZADO = notificacion.VL_SERIALIZADO;

                                if (!string.IsNullOrEmpty(notificacion.VL_SERIALIZADO) && notificacion.VL_SERIALIZADO.Length > 499)
                                {
                                    DB_NOTIFICACION.VL_SERIALIZADO = notificacion.VL_SERIALIZADO.Substring(0, 499);
                                }


                                notificacion.ID_NOTIFICACION = DB_NOTIFICACION.ID_NOTIFICACION.ToString();

                                context.T_NOTIFICACIONES.Add(DB_NOTIFICACION);
                            }
                            else
                            {
                                ERROR error = new ERROR();
                                error.NU_ERROR = num;
                                error.CD_ERROR = "DS_MENSAJE no puede ser VACIO";
                                error.DS_ERROR = BaseSerializableXml.ToXml(notificacion);
                                errores.Add(error);
                                num++;
                            }
                        }

                        context.SaveChanges();
                        tran.Commit();

                        respuesta = new RESPUESTA_INTERFAZ();

                        string xml = BaseSerializableXml.ToXml(notificaciones);
                        string encode64 = Encrypter.EncodeBase64(xml);

                        respuesta.DATA = encode64;

                        if (errores.Count > 0)
                        {
                            respuesta.RESULTADO = EResultado.ERROR.ToString();
                            respuesta.ERRORES = errores;
                            respuesta.MENSAJE = "Algunas Notificaciones no se grabaron, ver tag Errores";
                        }
                        else
                        {
                            respuesta.RESULTADO = EResultado.OK.ToString();
                            respuesta.MENSAJE = "Se grabo Correctamente";
                        }
                    }
                }
            }
            catch (XDTokenException ex)
            {
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddErrores(ex.Errores);
            }
            catch (XDNodeValidateException ex)
            {
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddErrores(ex.Errores);
            }
            catch (XDInterfazException ex)
            {
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddErrores(ex.Errores);
            }
            catch (Exception ex)
            {
                var err = GetErrorGenerico(interfaz, ex);
                respuesta = new RESPUESTA_INTERFAZ(EResultado.ERROR);
                respuesta.AddError(err);
            }

            return respuesta;
        }

        #endregion

        #region Utils

        #region Token

        protected static async Task<string> GetTokenAsync(string address, string clientId, string clientSecret, string scope)
        {
            using (var client = new HttpClient())
            {
                var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = address,
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    Scope = scope
                });

                if (response.IsError)
                {
                    if (response.HttpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        throw new XDInterfazException(XDCDError.NO_LOGIN);
                    else
                        throw new Exception($"An error occurred while retrieving an access token: {response.Error}");
                }

                return response.AccessToken;
            }
        }

        protected virtual async Task<string> Token_Valido(WISDB context, string token)
        {
            _logger.LogDebug("Checking token");

            if (token == null)
            {
                _logger.LogError("Token inválido(NULL)");
                throw new XDTokenException(XDCDError.TOKEN_INVALIDO_NULL);
            }

            var issuer = _configuration.GetValue<string>("AuthSettings:Issuer");

            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                issuer + ".well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever());

            var validatedToken = await ValidateToken(token, issuer, configurationManager);

            if (validatedToken == null)
            {
                _logger.LogError("Token inválido");
                throw new XDTokenException(XDCDError.TOKEN_INVALIDO);
            }

            _logger.LogDebug("USERID: " + validatedToken.Subject);
            _logger.LogDebug("Token Válido");

            return validatedToken.Subject;
        }

        protected virtual async Task<JwtSecurityToken> ValidateToken(
            string token,
            string issuer,
            IConfigurationManager<OpenIdConnectConfiguration> configurationManager,
            CancellationToken ct = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token));
            if (string.IsNullOrEmpty(issuer)) throw new ArgumentNullException(nameof(issuer));

            var discoveryDocument = await configurationManager.GetConfigurationAsync(ct);
            var signingKeys = discoveryDocument.SigningKeys;

            var validationParameters = new TokenValidationParameters
            {
                RequireExpirationTime = true,
                RequireSignedTokens = true,
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = signingKeys,
                ValidateLifetime = true,
                // Allow for some drift in server time
                // (a lower value is better; we recommend two minutes or less)
                ClockSkew = TimeSpan.FromMinutes(2),
                // See additional validation for aud below
                ValidateAudience = false,
            };

            try
            {
                var principal = new JwtSecurityTokenHandler()
                    .ValidateToken(token, validationParameters, out var rawValidatedToken);

                return (JwtSecurityToken)rawValidatedToken;
            }
            catch (SecurityTokenValidationException ex)
            {
                _logger.LogError(ex, $"Error al validar el token {token}");
                return null;
            }
        }

        #endregion

        #region Database

        protected virtual WISDB GetNewDbContext()
        {
            return new WISDB(_dbConfigService, _dbSettings.Value.ConnectionString, _dbSettings.Value.Schema);
        }

        #endregion

        #region Error

        protected virtual XDErrorCodigo GetErrorGenerico(INTERFAZ_SESSION interfaz, Exception ex)
        {
            _logger.LogError("ERROR");

            var msg = ex.ToString();
            var obj = ToXML<INTERFAZ_SESSION>(interfaz);
            msg = msg + "                                           === OBJETO  IN === " + obj;

            return new XDErrorCodigo(XDCDError.GENERICO_AL_VALIDAR, msg);
        }

        protected virtual XDErrorCodigo GetErrorGenericoApi(INTERFAZ interfaz, ApiResponseException ex)
        {
            _logger.LogError("ERROR");

            var msg = ex.ProblemDetails.Detail;
            var obj = ToXML<INTERFAZ>(interfaz);
            msg = msg + "                                           === OBJETO  IN === " + obj;

            return new XDErrorCodigo(XDCDError.GENERICO_AL_VALIDAR, msg);
        }

        protected virtual XDErrorCodigo GetErrorGenerico(INTERFAZ interfaz, Exception ex)
        {
            _logger.LogError("ERROR");

            var msg = ex.ToString();
            var obj = ToXML<INTERFAZ>(interfaz);
            msg = msg + "                                           === OBJETO  IN === " + obj;

            return new XDErrorCodigo(XDCDError.GENERICO_AL_VALIDAR, msg);
        }

        protected virtual void SaveErrorInterfaz(long NU_INTERFAZ_EJECUCION, List<ERROR> ERRORES, bool from_cliente = false)
        {
            var errorBuilder = new List<string> { "ERROR EN INTERFACE" };

            if (NU_INTERFAZ_EJECUCION != -1)
            {
                if (ERRORES != null && ERRORES.Count > 0)
                {
                    using (var context = GetNewDbContext())
                    {
                        try
                        {
                            T_INTERFAZ_EJECUCION_DATEXT data_ext = context.T_INTERFAZ_EJECUCION_DATEXT.FirstOrDefault(x => x.NU_INTERFAZ_EJECUCION == NU_INTERFAZ_EJECUCION);

                            if (data_ext == null)
                            {
                                throw new XDInterfazException(XDCDError.NU_INTERFAZ_EJECUCION_INVALIDO);
                            }

                            data_ext.FINALIZA_EJECUCION = "E";

                            string DS_REFERENCIA = "WIS_WEB_INTERFAZ";

                            if (from_cliente)
                                DS_REFERENCIA += "_ERROR_DE_CLIENTE";

                            int NU_ERROR = 1;

                            foreach (ERROR error in ERRORES)
                            {
                                T_INTERFAZ_EJECUCION_ERROR error_db = new T_INTERFAZ_EJECUCION_ERROR();
                                error_db.NU_INTERFAZ_EJECUCION = NU_INTERFAZ_EJECUCION;
                                error_db.NU_REGISTRO = error.NU_ERROR;

                                if (from_cliente)
                                {
                                    error_db.NU_ERROR = NU_ERROR;
                                }
                                else
                                {
                                    error_db.NU_ERROR = error.NU_ERROR;
                                }

                                error_db.CD_ERROR = error.CD_ERROR;
                                error_db.DS_REFERENCIA = DS_REFERENCIA;
                                error_db.DS_ERROR = error.DS_ERROR;

                                context.T_INTERFAZ_EJECUCION_ERROR.Add(error_db);

                                errorBuilder.Add("ERROR: " + error_db.DS_REFERENCIA + "::://:::" + error_db.DS_ERROR);

                                NU_ERROR++;
                            }

                            T_INTERFAZ_EJECUCION interfaz = context.T_INTERFAZ_EJECUCION.FirstOrDefault(x => x.NU_INTERFAZ_EJECUCION == NU_INTERFAZ_EJECUCION);
                            if (interfaz == null)
                            {
                                throw new XDInterfazException(XDCDError.NU_INTERFAZ_EJECUCION_INVALIDO);
                            }

                            interfaz.FL_ERROR_CARGA = "S";
                            interfaz.CD_SITUACAO = SituacionDb.ArchivoRespaldado;
                            interfaz.DT_SITUACAO = DateTime.Now;

                            context.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(string.Join(Environment.NewLine, errorBuilder));
                            _logger.LogError(ex, "SaveErrorInterfaz");

                            return;
                        }
                    }
                }
            }
            else
            {
                errorBuilder.Add("--------------------------------------------------------");

                foreach (ERROR error in ERRORES)
                {
                    string line = string.Format("NU_ERROR: {0}, CD_ERROR: {1}, DS_ERROR: {2}", error.NU_ERROR, error.CD_ERROR, error.DS_ERROR);
                    errorBuilder.Add(line);
                }

                errorBuilder.Add("--------------------------------------------------------");
            }

            _logger.LogError(string.Join(Environment.NewLine, errorBuilder));
        }

        #endregion

        #region Format

        protected virtual string ToXML<T>(object respuesta)
        {
            string resu = "";
            try
            {
                XmlSerializer xsSubmit = new XmlSerializer(typeof(T));
                var subReq = respuesta;

                using (StringWriter sww = new StringWriter())
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, subReq);
                    resu = sww.ToString();
                }
            }
            catch (Exception ex)
            {
                resu = "EXCEPTION " + ex.ToString();
            }
            return resu;
        }

        protected virtual string Base64ToXmlString(string base64data)
        {
            string str_xml = "";
            Regex badAmpersand = new Regex("&(?!amp;)");
            string goodAmpersand = "&amp;";

            try
            {
                str_xml = Encrypter.DecodeBase64(base64data);
                str_xml = badAmpersand.Replace(str_xml, goodAmpersand);
            }
            catch (Exception ex)
            {
                throw new XDInterfazException(XDCDError.ERROR_BASE64_TO_STRINGXML);
            }

            return str_xml;
        }

        protected virtual string Base64ToFileString(string base64data)
        {
            string str_file = "";

            try
            {
                str_file = Encrypter.DecodeBase64(base64data);
            }
            catch (Exception ex)
            {
                //INVALIDO XML
                _logger.LogError("ERROR BASE64 to string");
                throw new XDInterfazException(XDCDError.ERROR_BASE64_TO_STRINGXML);
            }

            return str_file;
        }

        protected virtual bool EsXmlDocument(string str_xml)
        {
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                //De esta forma puedo cambiar SOLO los & que estan mal, no los que fueron enviados de forma correcta
                xmldoc.LoadXml(str_xml);
                _logger.LogDebug("Correcto, es XMLDocument");
                return true;
            }
            catch (Exception ex)
            {
                //INVALIDO XML
                _logger.LogError("Estructura XML invalida");
                throw new XDInterfazException(XDCDError.ERROR_ESTRUCTURA_XML);
            }
        }

        #endregion

        #region Nodos

        protected enum Node
        {
            NU_INTERFAZ_EJECUCION,
            CD_EMPRESA,
            CD_INTERFAZ_EXTERNA,
            NU_PAQUETE,
            TOTAL_PAQUETES,
            DS_REFERENCIA,
            TOKEN,
            DATA,
            FINALIZA_EJECUCION
        }

        protected class ValidNodes
        {
            public ValidNodes()
            {
                NU_INTERFAZ_EJECUCION = -1;
                CD_EMPRESA = -1;
                CD_INTERFAZ_EXTERNA = -1;
                NU_PAQUETE = -1;
                TOTAL_PAQUETES = -1;
            }

            public long NU_INTERFAZ_EJECUCION { get; set; }
            public int CD_EMPRESA { get; set; }
            public int CD_INTERFAZ_EXTERNA { get; set; }
            public int NU_PAQUETE { get; set; }
            public int TOTAL_PAQUETES { get; set; }
            public string DS_REFERENCIA { get; set; }
            public string TOKEN { get; set; }
            public string DATA { get; set; }
            public string FINALIZA_EJECUCION { get; set; }

            public bool SiFinalizaEjecucion()
            {
                return (FINALIZA_EJECUCION == "TRUE");
            }

            public bool SiData()
            {
                return !string.IsNullOrEmpty(DATA);
            }
        }

        protected virtual ValidNodes GetValidNodes(INTERFAZ interfaz, XDEstado estado)
        {
            ValidNodes nodes = null;
            List<Node> nodes_list = null;

            switch (estado)
            {
                case XDEstado.INCIO_EJECUCION:
                    if (interfaz.FINALIZA_EJECUCION.Equals("TRUE"))
                        nodes_list = new List<Node>() { Node.CD_INTERFAZ_EXTERNA, Node.CD_EMPRESA, Node.DS_REFERENCIA, Node.FINALIZA_EJECUCION };
                    else
                        nodes_list = new List<Node>() { Node.CD_INTERFAZ_EXTERNA, Node.CD_EMPRESA, Node.DS_REFERENCIA, Node.FINALIZA_EJECUCION, Node.TOTAL_PAQUETES };
                    break;

                case XDEstado.ENVIO_DATOS:
                    nodes_list = new List<Node>() { Node.NU_INTERFAZ_EJECUCION, Node.NU_PAQUETE, Node.TOTAL_PAQUETES, Node.FINALIZA_EJECUCION };
                    break;

                case XDEstado.CONSULTAR_ESTADO:
                    nodes_list = new List<Node>() { Node.NU_INTERFAZ_EJECUCION };
                    break;

                case XDEstado.EJECUCIONES_PENDIENTES:
                    nodes_list = new List<Node>() { Node.CD_EMPRESA };
                    break;

                case XDEstado.CONSULTAR_EJECUCION:
                    nodes_list = new List<Node>() { Node.NU_INTERFAZ_EJECUCION, Node.NU_PAQUETE };
                    break;

                case XDEstado.EJECUCION_LEIDA:
                    nodes_list = new List<Node>() { Node.NU_INTERFAZ_EJECUCION };
                    break;

                case XDEstado.CONSULTAR_DATOS:
                    nodes_list = new List<Node>() { Node.CD_INTERFAZ_EXTERNA, Node.CD_EMPRESA };
                    break;

                case XDEstado.MENSAJE:
                    nodes_list = new List<Node>() { Node.CD_INTERFAZ_EXTERNA };
                    break;
            }

            nodes = ValidateNodes(interfaz, nodes_list);

            nodes.DS_REFERENCIA = interfaz.DS_REFERENCIA;
            nodes.TOKEN = interfaz.TOKEN;
            nodes.DATA = interfaz.DATA;
            nodes.FINALIZA_EJECUCION = interfaz.FINALIZA_EJECUCION;
            nodes.TOKEN = interfaz.TOKEN;

            if (estado == XDEstado.INCIO_EJECUCION)
            {
                nodes_list = new List<Node>() { Node.NU_PAQUETE, Node.TOTAL_PAQUETES };
                ValidNodes nodes2 = ValidateNodes(interfaz, nodes_list, false);
                nodes.NU_PAQUETE = nodes2.NU_PAQUETE;
                nodes.TOTAL_PAQUETES = nodes2.TOTAL_PAQUETES;
            }

            return nodes;
        }

        protected virtual ValidNodes ValidateNodes(INTERFAZ interfaz, List<Node> nodes, bool obligatorio = true)
        {
            ValidNodes validnodes = new ValidNodes();
            XDNodeValidateException error = null;

            foreach (Node node in nodes)
            {
                switch (node)
                {
                    case Node.NU_INTERFAZ_EJECUCION:
                        try
                        {
                            validnodes.NU_INTERFAZ_EJECUCION = Convert.ToInt64(interfaz.NU_INTERFAZ_EJECUCION);
                        }
                        catch (Exception ex)
                        {
                            if (obligatorio)
                            {
                                error = AddNodeValidateErrorToException(error, XDCDError.NU_INTERFAZ_EJECUCION_INVALIDO);
                            }
                        }
                        break;

                    case Node.CD_EMPRESA:
                        try
                        {
                            validnodes.CD_EMPRESA = Convert.ToInt32(interfaz.CD_EMPRESA);
                        }
                        catch (Exception ex)
                        {
                            if (obligatorio)
                            {
                                error = AddNodeValidateErrorToException(error, XDCDError.EMPRESA);
                            }
                        }
                        break;

                    case Node.CD_INTERFAZ_EXTERNA:
                        try
                        {
                            validnodes.CD_INTERFAZ_EXTERNA = Convert.ToInt32(interfaz.CD_INTERFAZ_EXTERNA);
                        }
                        catch (Exception ex)
                        {
                            if (obligatorio)
                            {
                                error = AddNodeValidateErrorToException(error, XDCDError.CD_INTERFAZ_EXTERNA_INVALIDA);
                            }
                        }
                        break;

                    case Node.NU_PAQUETE:
                        try
                        {
                            validnodes.NU_PAQUETE = Convert.ToInt32(interfaz.NU_PAQUETE);
                        }
                        catch (Exception ex)
                        {
                            if (obligatorio)
                            {
                                error = AddNodeValidateErrorToException(error, XDCDError.NU_PAQUETE_INVALIDO);
                            }
                        }
                        break;

                    case Node.TOTAL_PAQUETES:
                        try
                        {
                            validnodes.TOTAL_PAQUETES = Convert.ToInt32(interfaz.TOTAL_PAQUETES);
                        }
                        catch (Exception ex)
                        {
                            if (obligatorio)
                            {
                                error = AddNodeValidateErrorToException(error, XDCDError.TOTAL_PAQUETES_INVALIDO);
                            }
                        }
                        break;

                    case Node.DS_REFERENCIA:
                        if (string.IsNullOrEmpty(interfaz.DS_REFERENCIA))
                        {
                            if (obligatorio)
                            {
                                error = AddNodeValidateErrorToException(error, XDCDError.DS_REFERENCIA_NULL);
                            }
                        }
                        else
                        {
                            validnodes.DS_REFERENCIA = interfaz.DS_REFERENCIA;
                        }
                        break;

                    case Node.FINALIZA_EJECUCION:

                        if (interfaz.FINALIZA_EJECUCION != "TRUE" && interfaz.FINALIZA_EJECUCION != "FALSE")
                        {
                            error = AddNodeValidateErrorToException(error, XDCDError.FINALIZA_EJECUCION_INVALIDO);
                        }
                        break;
                }
            }

            if (error != null)
                throw error;

            return validnodes;
        }

        protected virtual XDNodeValidateException AddNodeValidateErrorToException(XDNodeValidateException error, XDCDError cd_error, string extradesc = null)
        {
            if (error == null)
            {
                error = new XDNodeValidateException(cd_error, extradesc);
            }

            error.AddError(cd_error, extradesc);

            return error;
        }

        #endregion

        #region Interface

        protected virtual bool Validate_Interfaz(WISDB context, string loginName, ValidNodes nodes, XDEstado estado)
        {
            XDNodeValidateException error = null;

            _logger.LogDebug("====VALIDATE INTERFACE");

            if (estado == XDEstado.CONSULTAR_DATOS)
            {
                int _CD_INTERFAZ_EXTERNA = nodes.CD_INTERFAZ_EXTERNA;
                int _CD_EMPRESA = nodes.CD_EMPRESA;
                T_EMPRESA empresa = context.T_EMPRESA.FirstOrDefault(x => x.CD_EMPRESA == _CD_EMPRESA);

                if (empresa == null)
                {
                    error = AddNodeValidateErrorToException(error, XDCDError.EMPRESA);
                }
                else
                {
                    T_EMPRESA_FUNCIONARIO empresa_func = GetEmpresaFuncionario(context, loginName, _CD_EMPRESA);

                    if (empresa_func == null)
                    {
                        error = AddNodeValidateErrorToException(error, XDCDError.EMPRESA_USUARIO);
                    }

                    if (!EmpresaHabilitadaToInterfazExterna(context, _CD_EMPRESA, _CD_INTERFAZ_EXTERNA))
                        error = AddNodeValidateErrorToException(error, XDCDError.EMPRESA_INTERFAZ);
                }

                if (!context.T_INTERFAZ_EXTERNA.Any(x => x.CD_INTERFAZ_EXTERNA == _CD_INTERFAZ_EXTERNA))
                {
                    error = AddNodeValidateErrorToException(error, XDCDError.CD_INTERFAZ_EXTERNA_INVALIDA);
                }

                if (!nodes.SiData())
                {
                    error = AddNodeValidateErrorToException(error, XDCDError.DATA_VACIO);
                }
            }

            if (estado == XDEstado.INCIO_EJECUCION)
            {
                _logger.LogDebug("Inicio Ejecución");

                int _CD_INTERFAZ_EXTERNA = nodes.CD_INTERFAZ_EXTERNA;
                int _CD_EMPRESA = nodes.CD_EMPRESA;
                T_EMPRESA empresa = context.T_EMPRESA.FirstOrDefault(x => x.CD_EMPRESA == _CD_EMPRESA);

                if (empresa == null)
                {
                    _logger.LogError("========ERROR========Empresa = null");
                    error = AddNodeValidateErrorToException(error, XDCDError.EMPRESA);
                }
                else
                {
                    T_EMPRESA_FUNCIONARIO empresa_func = GetEmpresaFuncionario(context, loginName, _CD_EMPRESA);

                    if (empresa_func == null)
                    {
                        error = AddNodeValidateErrorToException(error, XDCDError.EMPRESA_USUARIO);
                    }

                    if (!EmpresaHabilitadaToInterfazExterna(context, _CD_EMPRESA, _CD_INTERFAZ_EXTERNA))
                        error = AddNodeValidateErrorToException(error, XDCDError.EMPRESA_INTERFAZ);
                }

                //T_EMPRESA_FUNCIONARIO
                if (!context.T_INTERFAZ_EXTERNA.Any(x => x.CD_INTERFAZ_EXTERNA == _CD_INTERFAZ_EXTERNA))
                {
                    _logger.LogError("========ERROR========T_INTERFAZ_EXTERNA INVALIDA");
                    error = AddNodeValidateErrorToException(error, XDCDError.CD_INTERFAZ_EXTERNA_INVALIDA);
                }

                T_INTERFAZ interfaz = context.T_INTERFAZ.FirstOrDefault(x => x.CD_INTERFAZ == nodes.CD_INTERFAZ_EXTERNA);
                if (nodes.SiFinalizaEjecucion() && !interfaz.ID_ENTRADA_SALIDA.Equals("A"))
                {
                    if (!nodes.SiData())
                    {
                        error = AddNodeValidateErrorToException(error, XDCDError.DATA_VACIO);
                    }
                }
            }

            if (estado != XDEstado.INCIO_EJECUCION && estado != XDEstado.CONSULTAR_DATOS)
            {
                long _NU_INTERFAZ_EJECUCION = nodes.NU_INTERFAZ_EJECUCION;

                if (_NU_INTERFAZ_EJECUCION == 0)
                    throw new XDNodeValidateException(XDCDError.NU_INTERFAZ_EJECUCION_INVALIDO, "Verifique que el tag <NU_INTERFAZ_EJECUCION> este cargado correctamente");

                T_INTERFAZ_EJECUCION interfaz_ejecucion_db = context.T_INTERFAZ_EJECUCION.FirstOrDefault(x => x.NU_INTERFAZ_EJECUCION == _NU_INTERFAZ_EJECUCION);
                if (interfaz_ejecucion_db == null)
                {
                    error = AddNodeValidateErrorToException(error, XDCDError.NU_INTERFAZ_EJECUCION_INVALIDO);
                }

                if (estado == XDEstado.ENVIO_DATOS)
                {
                    if (interfaz_ejecucion_db.FL_ERROR_CARGA == "S")
                    {
                        error = AddNodeValidateErrorToException(error, XDCDError.FL_ERROR_CARGA_S);
                    }
                    else
                    {
                        if (interfaz_ejecucion_db.CD_SITUACAO != SituacionDb.EjecucionIniciada && interfaz_ejecucion_db.CD_SITUACAO != SituacionDb.ArchivoProcesado)
                        {
                            error = AddNodeValidateErrorToException(error, XDCDError.NU_INTERFAZ_EJECUCION_ESTADO_INVALIDO);
                        }
                    }

                    if (nodes.SiData())
                    {
                        if (nodes.NU_PAQUETE < 1)
                        {
                            error = AddNodeValidateErrorToException(error, XDCDError.NU_PAQUETE_INVALIDO);
                        }

                        if (nodes.TOTAL_PAQUETES < 1)
                        {
                            error = AddNodeValidateErrorToException(error, XDCDError.TOTAL_PAQUETES_INVALIDO);
                        }
                    }
                }
            }

            if (error != null)
                throw error;

            return true;
        }

        protected virtual T_EMPRESA_FUNCIONARIO GetEmpresaFuncionario(WISDB context, string loginName, int _CD_EMPRESA)
        {
            return context.T_EMPRESA_FUNCIONARIO
                .Join(context.USERS,
                    ef => ef.USERID,
                    u => u.USERID,
                    (ef, u) => new { EmpresFuncionario = ef, Usuario = u })
                .AsNoTracking()
                .Where(x => x.EmpresFuncionario.CD_EMPRESA == _CD_EMPRESA
                    && x.Usuario.LOGINNAME == loginName)
                .Select(x => x.EmpresFuncionario)
                .FirstOrDefault();
        }

        protected virtual bool EmpresaHabilitadaToInterfazExterna(WISDB context, int cdEmpresa, int cdIntExterna)
        {
            var paramEmpresaHabilitada = "S";
            List<KeyValuePair<string, string>> entities = new List<KeyValuePair<string, string>>();

            entities.Add(new KeyValuePair<string, string>(ParamManager.PARAM_EMPR, $"{ParamManager.PARAM_EMPR}_{cdEmpresa}"));

            switch (cdIntExterna.ToString())
            {
                #region Entrada
                case "500":
                    paramEmpresaHabilitada = (string)ParamManager.GetParamValue(context, ParamManager.IE_500_HABILITADA, entities);
                    paramEmpresaHabilitada = "S";
                    break;
                case "503":
                    paramEmpresaHabilitada = (string)ParamManager.GetParamValue(context, ParamManager.IE_503_HABILITADA, entities);
                    paramEmpresaHabilitada = "S";
                    break;
                case "505":
                    paramEmpresaHabilitada = (string)ParamManager.GetParamValue(context, ParamManager.IE_505_HABILITADA, entities);
                    paramEmpresaHabilitada = "S";
                    break;
                case "506":
                    paramEmpresaHabilitada = (string)ParamManager.GetParamValue(context, ParamManager.IE_506_HABILITADA, entities);
                    paramEmpresaHabilitada = "S";
                    break;
                case "501":
                    paramEmpresaHabilitada = (string)ParamManager.GetParamValue(context, ParamManager.IE_501_HABILITADA, entities);
                    paramEmpresaHabilitada = "S";
                    break;
                case "507":
                    paramEmpresaHabilitada = (string)ParamManager.GetParamValue(context, ParamManager.IE_507_HABILITADA, entities);
                    paramEmpresaHabilitada = "S";
                    break;
                case "510":
                    paramEmpresaHabilitada = (string)ParamManager.GetParamValue(context, ParamManager.IE_510_HABILITADA, entities);
                    paramEmpresaHabilitada = "S";
                    break;
                case "517":
                    paramEmpresaHabilitada = (string)ParamManager.GetParamValue(context, ParamManager.IE_517_HABILITADA, entities);
                    paramEmpresaHabilitada = "S";
                    break;
                case "518":
                    paramEmpresaHabilitada = (string)ParamManager.GetParamValue(context, ParamManager.IE_518_HABILITADA, entities);
                    paramEmpresaHabilitada = "S";
                    break;
                case "520":
                    paramEmpresaHabilitada = (string)ParamManager.GetParamValue(context, ParamManager.IE_520_HABILITADA, entities);
                    paramEmpresaHabilitada = "S";
                    break;
                case "550":
                    paramEmpresaHabilitada = "S";
                    break;
                #endregion

                #region Salida
                case "502":
                    paramEmpresaHabilitada = (string)ParamManager.GetParamValue(context, ParamManager.IS_502_HABILITADA, entities);
                    paramEmpresaHabilitada = "S";
                    break;
                case "504":
                    paramEmpresaHabilitada = (string)ParamManager.GetParamValue(context, ParamManager.IS_504_HABILITADA, entities);
                    paramEmpresaHabilitada = "S";
                    break;
                case "509":
                    paramEmpresaHabilitada = (string)ParamManager.GetParamValue(context, ParamManager.IS_509_HABILITADA, entities);
                    paramEmpresaHabilitada = "S";
                    break;
                case "508":
                    paramEmpresaHabilitada = (string)ParamManager.GetParamValue(context, ParamManager.IS_508_HABILITADA, entities);
                    paramEmpresaHabilitada = "S";
                    break;
                case "512":
                    paramEmpresaHabilitada = (string)ParamManager.GetParamValue(context, ParamManager.IS_512_HABILITADA, entities);
                    paramEmpresaHabilitada = "S";
                    break;
                case "601":
                    paramEmpresaHabilitada = (string)ParamManager.GetParamValue(context, ParamManager.IS_601_HABILITADA, entities);
                    paramEmpresaHabilitada = "S";
                    break;
                case "602":
                    paramEmpresaHabilitada = (string)ParamManager.GetParamValue(context, ParamManager.IS_602_HABILITADA, entities);
                    paramEmpresaHabilitada = "S";
                    break;
                    #endregion
            }

            if (paramEmpresaHabilitada.Equals("S"))
                return true;

            return false;
        }

        protected T_INTERFAZ_EJECUCION INCIAR_EJECUCION_INTERFAZ(WISDB context, string loginName, int p_interfaz_externa, string p_archivo, string p_referencia, string token)
        {
            T_INTERFAZ_EJECUCION nuevaInterfaz;
            long nu_interfaz_ejecucion = BLSecuencia.GetNextValSecIntfc(context, _dapper);
            RUsuario repo = new RUsuario();
            USERS usuario = repo.GetUserByLoginName(context, loginName);

            //Cuando se necesite un cliente, hay que cambiar los ""
            string parametro = GetParamUsuario(context, usuario.USERID, "");

            nuevaInterfaz = new T_INTERFAZ_EJECUCION();
            nuevaInterfaz.NU_INTERFAZ_EJECUCION = nu_interfaz_ejecucion;
            nuevaInterfaz.CD_INTERFAZ_EXTERNA = p_interfaz_externa;
            nuevaInterfaz.DS_REFERENCIA = p_referencia;
            nuevaInterfaz.FL_ERROR_CARGA = "N";
            nuevaInterfaz.FL_ERROR_PROCEDIMIENTO = "N";
            nuevaInterfaz.DT_COMIENZO = DateTime.Now;
            nuevaInterfaz.NM_ARCHIVO = p_archivo;
            nuevaInterfaz.DT_SITUACAO = DateTime.Now;
            nuevaInterfaz.CD_SITUACAO = SituacionDb.EjecucionIniciada;
            nuevaInterfaz.USERID = usuario.USERID;
            nuevaInterfaz.CD_GRUPO_CONSULTA = parametro;

            context.T_INTERFAZ_EJECUCION.Add(nuevaInterfaz);
            context.SaveChanges();

            return nuevaInterfaz;
        }

        protected virtual string GetParamUsuario(WISDB context, int userId, string cliente)
        {
            List<KeyValuePair<string, string>> colParams = new List<KeyValuePair<string, string>>();
            string codigoFuncionario = ParamManager.PARAM_USER + "_" + userId;

            cliente = ParamManager.PARAM_CLIENTE + "_" + userId;

            colParams.Add(new KeyValuePair<string, string>(ParamManager.PARAM_USER, codigoFuncionario));
            colParams.Add(new KeyValuePair<string, string>(ParamManager.PARAM_CLIENTE, cliente));

            return (string)ParamManager.GetParamValue(context, ParamManager.GRUPO_CONSULTA, colParams);
        }

        protected virtual void Save_INTERFAZ_EJECUCION_DATA(WISDB context, long NU_INTERFAZ_EJECION, ValidNodes nodes, bool inicio)
        {
            if (inicio)
            {
                if (nodes.SiFinalizaEjecucion())
                {
                    _logger.LogDebug("FINALIZA EJECUCION");

                    string str_xml = "";
                    T_INTERFAZ curInterfaz = context.T_INTERFAZ.FirstOrDefault(x => x.CD_INTERFAZ == nodes.CD_INTERFAZ_EXTERNA);

                    if (curInterfaz.TP_OBJETO_BD.Equals(CD_INTERFAZ_TP_OBJECTO_FILE))
                    {
                        _logger.LogDebug("Base64 to string");
                        str_xml = Base64ToFileString(nodes.DATA);
                    }
                    else
                    {
                        str_xml = Base64ToXmlString(nodes.DATA);

                        if (!str_xml.Equals(""))
                            EsXmlDocument(str_xml);
                    }

                    _logger.LogDebug("Getbytes");

                    T_INTERFAZ_EJECUCION_DATA data = new T_INTERFAZ_EJECUCION_DATA();
                    data.NU_INTERFAZ_EJECUCION = NU_INTERFAZ_EJECION;
                    data.DT_ADDROW = DateTime.Now;

                    byte[] array = ByteString.GetBytes(str_xml);

                    data.DATA = array;
                    context.T_INTERFAZ_EJECUCION_DATA.Add(data);

                    _logger.LogDebug("Guardando Context");
                    context.SaveChanges();

                    _logger.LogDebug("Cambiando estado");

                    Change_NU_INTERFAZ_ESTADO(context, NU_INTERFAZ_EJECION, SituacionDb.ArchivoProcesado);
                }
            }
            else
            {
                if (nodes.SiFinalizaEjecucion())
                {
                    _logger.LogDebug("FINALIZA EJECUCION");

                    T_INTERFAZ_EJECUCION_DATEXT interfaz = context.T_INTERFAZ_EJECUCION_DATEXT.FirstOrDefault(x => x.NU_INTERFAZ_EJECUCION == NU_INTERFAZ_EJECION);
                    if (interfaz == null)
                    {
                        _logger.LogDebug("ERROR: Interfaz == null");
                        XDInterfazException error = new XDInterfazException(XDCDError.NU_INTERFAZ_EJECUCION_INVALIDO);
                        throw error;
                    }

                    short NU_TOTAL_PAQUETES = interfaz.NU_TOTAL_PAQUETES;
                    List<T_INTERFAZ_EJECUCION_DATEXTDET> list = context.T_INTERFAZ_EJECUCION_DATEXTDET.Where(x => x.NU_INTERFAZ_EJECUCION == NU_INTERFAZ_EJECION).ToList();

                    int count = list.Count;

                    if (NU_TOTAL_PAQUETES != count)
                    {

                        string extradesc = string.Format(" NU_TOTAL_PAQUETES: {0}, Paquetes encontrados: {1}", NU_TOTAL_PAQUETES, count);
                        _logger.LogDebug("ERROR EN CANTIDAD DE PAQUETES:://(Faltan paquetes)" + extradesc);
                        XDInterfazException error = new XDInterfazException(XDCDError.FALTAN_PAQUETES, extradesc);
                        throw error;
                    }
                    else
                    {
                        _logger.LogDebug("Juntando paquetes");

                        string base64String = "";
                        short nu_paquete = 1;

                        list = list.OrderBy(x => x.NU_PAQUETE).ToList();

                        string estructura = "";

                        foreach (T_INTERFAZ_EJECUCION_DATEXTDET detalle in list)
                        {
                            estructura += Base64ToXmlString(System.Text.Encoding.UTF8.GetString(detalle.DATA));

                            //CONTROL QUE LOS PAQUETES ESTEN 
                            if (detalle.NU_PAQUETE != nu_paquete)
                            {
                                XDInterfazException error = new XDInterfazException(XDCDError.NO_SE_ENCONTRO_PAQUETE_ALGENERAR_XML, nu_paquete.ToString());
                                throw error;
                            }
                            else
                            {
                                nu_paquete++;
                            }

                        }
                        _logger.LogDebug("Fin Foreache de detalle");

                        string str_xml = estructura;

                        _logger.LogDebug("Es xmlDocument?");

                        if (!str_xml.IsNullOrEmpty())
                        {
                            EsXmlDocument(str_xml);
                        }

                        T_INTERFAZ_EJECUCION_DATA data = new T_INTERFAZ_EJECUCION_DATA();
                        data.NU_INTERFAZ_EJECUCION = NU_INTERFAZ_EJECION;
                        data.DT_ADDROW = DateTime.Now;

                        _logger.LogDebug("GetBytes");

                        byte[] array = ByteString.GetBytes(str_xml);
                        data.DATA = array;

                        _logger.LogDebug("DATA = ARRAY!!!");

                        context.T_INTERFAZ_EJECUCION_DATA.Add(data);

                        _logger.LogDebug("Saving context");

                        context.SaveChanges();

                        Change_NU_INTERFAZ_ESTADO(context, NU_INTERFAZ_EJECION, SituacionDb.ArchivoProcesado);
                    }

                    _logger.LogDebug("FIN FINALIZA EJECUCION");
                }
            }
        }

        protected virtual void Change_NU_INTERFAZ_ESTADO(WISDB context, long NU_INTERFAZ_EJECION, short CD_SITUACAO)
        {
            T_INTERFAZ_EJECUCION interfaz = context.T_INTERFAZ_EJECUCION.FirstOrDefault(x => x.NU_INTERFAZ_EJECUCION == NU_INTERFAZ_EJECION);

            if (interfaz == null)
            {
                throw new XDInterfazException(XDCDError.NU_INTERFAZ_EJECUCION_INVALIDO);
            }

            interfaz.CD_SITUACAO = CD_SITUACAO;
            interfaz.DT_SITUACAO = DateTime.Now;
        }

        protected virtual void Save_INTERFAZ_EJECION_DATEXT(WISDB context, long NU_INTERFAZ_EJECION, ValidNodes nodes, bool inicio)
        {
            if (inicio)
            {
                int _CD_EMPRESA = nodes.CD_EMPRESA;

                T_INTERFAZ_EJECUCION interfaz_ejecucion = context.T_INTERFAZ_EJECUCION.FirstOrDefault(x => x.NU_INTERFAZ_EJECUCION == NU_INTERFAZ_EJECION);
                interfaz_ejecucion.CD_EMPRESA = _CD_EMPRESA;

                T_INTERFAZ_EJECUCION_DATEXT data_ext = new T_INTERFAZ_EJECUCION_DATEXT();
                data_ext.NU_INTERFAZ_EJECUCION = NU_INTERFAZ_EJECION;
                data_ext.CD_EMPRESA = _CD_EMPRESA;

                if (nodes.SiFinalizaEjecucion())
                {
                    data_ext.NU_TOTAL_PAQUETES = 1;
                    data_ext.FINALIZA_EJECUCION = "S";
                }
                else
                {
                    data_ext.FINALIZA_EJECUCION = "N";
                    if (nodes.TOTAL_PAQUETES > 0)
                    {
                        data_ext.NU_TOTAL_PAQUETES = Convert.ToInt16(nodes.TOTAL_PAQUETES);
                    }
                    else
                    {
                        data_ext.NU_TOTAL_PAQUETES = -1;
                    }
                }

                context.T_INTERFAZ_EJECUCION_DATEXT.Add(data_ext);
            }
            else
            {
                T_INTERFAZ_EJECUCION_DATEXT interfaz = context.T_INTERFAZ_EJECUCION_DATEXT.FirstOrDefault(x => x.NU_INTERFAZ_EJECUCION == NU_INTERFAZ_EJECION);
                if (interfaz != null)
                {
                    if (interfaz.NU_TOTAL_PAQUETES == -1)
                    {
                        interfaz.NU_TOTAL_PAQUETES = Convert.ToInt16(nodes.TOTAL_PAQUETES);
                    }

                    if (nodes.FINALIZA_EJECUCION == "TRUE")
                    {
                        interfaz.FINALIZA_EJECUCION = "S";
                    }
                }
                else
                {
                    throw new XDInterfazException(XDCDError.NU_INTERFAZ_EJECUCION_INVALIDO);
                }
            }
        }

        protected virtual void Save_INTERFAZ_EJECUCION_DATEXTDET(WISDB context, long NU_INTERFAZ_EJECION, ValidNodes nodes, bool inicio)
        {
            short NU_PAQUETE = Convert.ToInt16(nodes.NU_PAQUETE);

            _logger.LogDebug("============Numero de paquete: " + NU_PAQUETE + "===========");

            T_INTERFAZ_EJECUCION_DATEXTDET temp = context.T_INTERFAZ_EJECUCION_DATEXTDET
                .FirstOrDefault(x => x.NU_INTERFAZ_EJECUCION == NU_INTERFAZ_EJECION
                    && x.NU_PAQUETE == NU_PAQUETE);

            if (temp != null)
            {
                XDInterfazException error = new XDInterfazException(XDCDError.NU_PAQUETE_EXISTENTE);
                throw error;
            }

            if (nodes.SiData())
            {
                T_INTERFAZ_EJECUCION_DATEXTDET data_ext_det = new T_INTERFAZ_EJECUCION_DATEXTDET();
                data_ext_det.NU_INTERFAZ_EJECUCION = NU_INTERFAZ_EJECION;
                data_ext_det.NU_PAQUETE = NU_PAQUETE;
                data_ext_det.DT_ADDROW = DateTime.Now;

                byte[] newBytes = ByteString.GetBytes(nodes.DATA);
                data_ext_det.DATA = newBytes;
                context.T_INTERFAZ_EJECUCION_DATEXTDET.Add(data_ext_det);
            }
        }

        protected virtual bool ContainsGrupoConsulta(WISDB context, int userId, string CD_GRUPO_CONSULTA)
        {
            bool aux = false;
            RGrupoConsulta repoGru = new RGrupoConsulta();
            List<T_GRUPO_CONSULTA> lstGrupoCons = repoGru.GetGrupoConsultaUsuario(context, userId);

            if (lstGrupoCons.Any(x => x.CD_GRUPO_CONSULTA == CD_GRUPO_CONSULTA))
                aux = true;

            return aux;
        }

        protected virtual bool Validate_NU_INTERFAZ_EJECUCION_OUT(T_INTERFAZ_EJECUCION interfaz_ejecucion)
        {
            List<int> interfacesAdmitidas = new List<int>
            {
                CInterfazExterna.AjustesDeStock,
                CInterfazExterna.ConfirmacionDePedido,
                CInterfazExterna.Facturacion,
                CInterfazExterna.ConfirmacionDeRecepcion,
                CInterfazExterna.PedidosAnulados,
                CInterfazExterna.ConsultaDeStock,
            };

            CSTransfer transfer = new CSTransfer();
            CSManager manager = new CSManager(_logger, _configuration);

            transfer.jsonData = JsonConvert.SerializeObject(interfacesAdmitidas);
            transfer = manager.InvokeInterfacesCustomMethod(ECSInterfacesMethods.INTERFAZ_CargarListaInterfacesCustom, transfer);

            if (transfer.status != CSResult.NOT_IMPLEMENTED)
            {
                if (transfer.status == CSResult.ERROR)
                    throw new CMNException(transfer.errorMessage);

                interfacesAdmitidas = JsonConvert.DeserializeObject<List<int>>(transfer.jsonData);
            }

            if (interfaz_ejecucion == null)
            {
                throw new XDTokenException(XDCDError.NU_INTERFAZ_EJECUCION_INVALIDO);
            }

            if (interfaz_ejecucion.FL_ERROR_CARGA == "S" || interfaz_ejecucion.FL_ERROR_PROCEDIMIENTO == "S")
            {
                throw new XDTokenException(XDCDError.FL_ERROR_CARGA_S);
            }

            if (interfaz_ejecucion.CD_SITUACAO != SituacionDb.ProcesandoInterfaz)
            {
                throw new XDTokenException(XDCDError.NU_INTERFAZ_EJECUCION_ESTADO_INVALIDO);
            }

            if (!interfacesAdmitidas.Contains(interfaz_ejecucion.CD_INTERFAZ_EXTERNA ?? -1))
            {
                throw new XDTokenException(XDCDError.NU_INTERFAZ_EJECUCION_INVALIDO);
            }

            return true;
        }

        #endregion

        #endregion
    }
}
