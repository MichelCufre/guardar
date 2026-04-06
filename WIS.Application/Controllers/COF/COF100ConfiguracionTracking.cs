using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Tracking.Models;
using WIS.Exceptions;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.COF
{
    public class COF100ConfiguracionTracking : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly ITrackingService _trackingService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        public COF100ConfiguracionTracking(IUnitOfWorkFactory uowFactory, IIdentityService identity, ISecurityService security, ITrackingService trackingService, IFormValidationService formValidationService)
        {
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._security = security;
            this._trackingService = trackingService;
            this._formValidationService = formValidationService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var config = this._trackingService.GetConfig();
            string estadoTracking = config["estadoTracking"];

            form.GetField("estado").Value = estadoTracking;
            form.GetField("estado").ReadOnly = true;
            form.GetField("scope").Value = config["scope"];
            form.GetField("scope").ReadOnly = true;

            form.GetField("idTenant").Value = config["tenant"];
            form.GetField("grantType").Value = config["granType"];

            form.GetField("clientId").Value = config["clientId"];
            form.GetField("clientSecret").Value = config["clientSecret"];

            form.GetField("apiTracking").Value = config["url"];
            form.GetField("apiWMS").Value = config["urlApiWMS"];

            form.GetField("accessTokenURL").Value = config["urlAccesToken"];
            form.GetField("apiUsers").Value = config["urlApiUsers"];

            form.GetField("tpCont").Value = config["tpCont"];
            form.GetField("tpContFicticio").Value = config["tpContFicticio"];
            form.GetField("cantDias").Value = config["cantDiasTarea"];

            form.GetField("agrupacionCD").Value = config["agrupacionCD"];

            if (DateTimeExtension.IsValid_DDMMYYYY(config["fechaInicial"], this._identity.GetFormatProvider()))
            {
                var fechaInicial = DateTimeExtension.FromString_DDMMYYYY(config["fechaInicial"], this._identity.GetFormatProvider());

                form.GetField("fechaInicial").Value = fechaInicial.ToIsoString();
            }

            form.GetField("qtPedidos").Value = uow.TrackingRepository.GetCountPedidosNoPlanificados().ToString();
            form.GetField("qtPedidos").ReadOnly = true;

            if (estadoTracking == CEstadoTracking.EnProceso)
                form.GetButton("btnSincronizarDatos").Disabled = false;
            else
                form.GetButton("btnSincronizarDatos").Disabled = true;

            if (estadoTracking == CEstadoTracking.Activo)
            {
                form.GetButton("btnDesactivarTracking").Disabled = false;
                form.GetButton("btnSincronizarPedidos").Disabled = false;
            }
            else
            {
                form.GetButton("btnDesactivarTracking").Disabled = true;
                form.GetButton("btnSincronizarPedidos").Disabled = true;
            }
            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber(this._identity.Application);
            uow.BeginTransaction();

            try
            {
                if (context.ButtonId == "btnGuardarParametros")
                {
                    VerificarYGuardarParametros(uow, form);
                    uow.SaveChanges();

                    context.AddSuccessNotification("COF100_msg_Succes_ParametrosGuardados");
                }
                else if (context.ButtonId == "btnSincronizarDatos")
                {
                    SincronizarTracking(uow);
                    uow.SaveChanges();

                    form.GetField("estado").Value = CEstadoTracking.Activo;
                    context.AddSuccessNotification("COF100_msg_Succes_Trackingsincronizado");
                }
                else if (context.ButtonId == "btnSincronizarPedidos")
                {
                    _trackingService.SincronizacionInicialPedidos(uow);
                    uow.SaveChanges();

                    form.GetField("qtPedidos").ReadOnly = false;
                    form.GetField("qtPedidos").Value = uow.TrackingRepository.GetCountPedidosNoPlanificados().ToString();
                    form.GetField("qtPedidos").ReadOnly = true;
                    context.AddSuccessNotification("COF100_msg_Succes_PedidosSincronizados");
                }
                else
                {
                    DesactivarTracking(uow);
                    uow.SaveChanges();

                    form.GetField("estado").Value = CEstadoTracking.Inactivo;
                    context.AddSuccessNotification("COF100_msg_Succes_TrackingDesactivado");
                }

                uow.Commit();
            }
            catch (ValidationFailedException ex)
            {
                logger.Debug($"Error {ex.Message} - {ex}");
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                uow.Rollback();
            }
            catch (Exception ex)
            {
                logger.Debug($"Error {ex.Message} - {ex}");
                context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }
            return form;
        }
        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new COF100ValidationModule(uow, this._identity.UserId, this._identity.Application), form, context);
        }

        public virtual void VerificarYGuardarParametros(IUnitOfWork uow, Form form)
        {
            string estado = form.GetField("estado").Value;
            string scope = form.GetField("scope").Value;
            string idTenant = form.GetField("idTenant").Value;
            string grantType = form.GetField("grantType").Value;
            string clientId = form.GetField("clientId").Value;
            string clientSecret = form.GetField("clientSecret").Value;
            string apiTracking = form.GetField("apiTracking").Value;
            string apiWMS = form.GetField("apiWMS").Value;
            string accessTokenURL = form.GetField("accessTokenURL").Value;
            string apiUsers = form.GetField("apiUsers").Value;
            string tpCont = form.GetField("tpCont").Value;
            string tpContFicticio = form.GetField("tpContFicticio").Value;
            string cantDias = form.GetField("cantDias").Value;

            Dictionary<string, string> config = new Dictionary<string, string>()
            {
                { "url", apiTracking },
                { "scope", scope },
                { "tenant", idTenant },
                { "clientId", clientId},
                { "granType", grantType},
                { "clientSecret", clientSecret},
                { "urlAccesToken", accessTokenURL}
            };

            if (!this._trackingService.TestearConexion(config))
                throw new Exception("COF100_Sec0_Error_TestConexion");
            else
            {
                Dictionary<string, string> campoParam = new Dictionary<string, string>()
                {
                    { "estado", ParamManager.TRACKING_ACTIVO},
                    { "scope", ParamManager.TRACKING_SCOPE},
                    { "idTenant", ParamManager.TRACKING_TENANT},
                    { "grantType", ParamManager.TRACKING_GRANT_TYPE},
                    { "clientId", ParamManager.TRACKING_CLIENT_ID},
                    { "clientSecret", ParamManager.TRACKING_CLIENT_SECRET},
                    { "apiTracking", ParamManager.TRACKING_API},
                    { "apiWMS", ParamManager.TRACKING_API_WMS},
                    { "accessTokenURL", ParamManager.TRACKING_ACCESS_TOKEN_URL},
                    { "apiUsers", ParamManager.TRACKING_API_USERS},
                    { "tpCont", ParamManager.TRACKING_TP_CONT_DEFAULT},
                    { "tpContFicticio", ParamManager.TRACKING_TP_CONT_FICTICIO},
                    { "cantDias", ParamManager.TRACKING_TAREA_CANT_DIAS},
                    { "agrupacionCD", ParamManager.TRACKING_AGRUPACION_CD},
                    { "fechaInicial", ParamManager.TRACKING_JOB_FECHA_INICIAL},
                };

                string entidad = ParamManager.PARAM_GRAL;
                foreach (var field in form.Fields.Where(f => f.Id != "estado" && f.Id != "qtPedidos" && f.Id != "fechaInicial"))
                {
                    string value = field.Value;
                    var param = uow.ParametroRepository.GetParametroConfiguracion(campoParam[field.Id], entidad);

                    if (param != null)
                    {
                        param.Valor = value;
                        uow.ParametroRepository.UpdateParametroConfiguracion(param);
                    }
                }

                var paramEstado = uow.ParametroRepository.GetParametroConfiguracion(ParamManager.TRACKING_ACTIVO, entidad);
                if (paramEstado != null)
                {
                    paramEstado.Valor = CEstadoTracking.EnProceso;
                    uow.ParametroRepository.UpdateParametroConfiguracion(paramEstado);

                }

                var paramFechaInicial = uow.ParametroRepository.GetParametroConfiguracion(ParamManager.TRACKING_JOB_FECHA_INICIAL, entidad);
                if (paramFechaInicial != null && DateTime.TryParse(form.GetField("fechaInicial")?.Value, this._identity.GetFormatProvider(), DateTimeStyles.None, out DateTime parsedValue))
                {
                    paramFechaInicial.Valor = parsedValue.ToString("dd/MM/yyyy");
                    uow.ParametroRepository.UpdateParametroConfiguracion(paramFechaInicial);
                }


                form.GetField("estado").Value = CEstadoTracking.EnProceso;
                form.GetButton("btnSincronizarDatos").Disabled = false;

            }
        }

        public virtual void DesactivarTracking(IUnitOfWork uow)
        {
            var param = uow.ParametroRepository.GetParametroConfiguracion(ParamManager.TRACKING_ACTIVO, ParamManager.PARAM_GRAL);
            if (param != null)
            {
                param.Valor = "INACTIVO";
                uow.ParametroRepository.UpdateParametroConfiguracion(param);
            }
        }

        public virtual void SincronizarTracking(IUnitOfWork uow)
        {
            _trackingService.SincronizacionInicial(uow);
            var param = uow.ParametroRepository.GetParametroConfiguracion(ParamManager.TRACKING_ACTIVO, ParamManager.PARAM_GRAL);
            if (param != null)
            {
                param.Valor = CEstadoTracking.Activo;
                uow.ParametroRepository.UpdateParametroConfiguracion(param);
            }
        }
    }
}
