using NLog;
using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.OrdenTarea;
using WIS.Domain.Services.Interfaces;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;


namespace WIS.Application.Controllers.ORT
{
    public class ORT090RegistroTareaAmigable : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IAuthorizationService _authService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        public ORT090RegistroTareaAmigable(IUnitOfWorkFactory uowFactory, IIdentityService identity, ISecurityService security, IFormValidationService formValidationService, IAuthorizationService authService)
        {
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._security = security;
            this._formValidationService = formValidationService;
            this._authService = authService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            this.InicializarSelect(uow, form, context);
            //SOLO LECTURA 
            form.GetField("nombre").ReadOnly = true;
            form.GetField("usuario").Value = "";
            form.GetField("password").Value = "";
            form.GetField("password").Disabled = true;

            //CAMPOS READONLY HASTA VALIDACION DE PASSWORD
            form.GetField("numeroOrden").ReadOnly = true;
            form.GetField("descripcionOrden").ReadOnly = true;
            form.GetField("codigoTarea").ReadOnly = true;
            form.GetField("descripcionTarea").ReadOnly = true;
            form.GetField("codigoEmpresa").ReadOnly = true;
            form.GetField("descripcionEmpresa").ReadOnly = true;
            form.GetField("fechaInicio").ReadOnly = true;
            form.GetField("fechaFin").ReadOnly = true;
            form.GetButton("Confirmar").Disabled = true;
            form.GetButton("Terminar").Disabled = true;
            form.GetButton("Cancelar").Disabled = true;
            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();
            try
            {
                switch (context.ButtonId)
                {
                    case "Confirmar":
                        ConfirmarTareaAmigable(uow, form);
                        break;

                    case "Terminar":
                        TerminarTareaAmigable(uow, form);
                        break;
                }

                context.AddSuccessNotification("General_Sec0_Success_SavedChanges");

                uow.SaveChanges();
                uow.Commit();
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

            return this._formValidationService.Validate(new ORT090ValidationModule(uow, this._identity, this._authService, this._identity.UserId, this._identity.Application), form, context);
        }

        #region Metodos Auxiliares

        public virtual void InicializarSelect(IUnitOfWork uow, Form form, FormInitializeContext context)
        {
            FormField selectorNumeroOrden = form.GetField("numeroOrden");
            FormField selectorCodigoTarea = form.GetField("codigoTarea");

            selectorNumeroOrden.Options = new List<SelectOption>();
            selectorCodigoTarea.Options = new List<SelectOption>();

            if (string.IsNullOrEmpty(context.GetParameter("nuOrden")))
            {
                List<Orden> ordenesActivas = uow.OrdenRepository.GetOrdenesActivas();
                foreach (var orden in ordenesActivas)
                {
                    selectorNumeroOrden.Options.Add(new SelectOption(orden.Id.ToString(), $"{orden.Id} - {orden.Descripcion}"));
                }
            }
            else
            {
                var nuOrden = int.Parse(context.GetParameter("nuOrden"));
                var orden = uow.OrdenRepository.GetOrden(nuOrden);

                selectorNumeroOrden.Options.Add(new SelectOption(orden.Id.ToString(), $"{orden.Id} - {orden.Descripcion}"));

                form.GetField("numeroOrden").Value = orden.Id.ToString();
                form.GetField("descripcionOrden").Value = orden.Descripcion;
            }

            List<Tarea> tareas = uow.TareaRepository.GetTareasManualesConRegistroHorasS();
            foreach (var tarea in tareas)
            {
                selectorCodigoTarea.Options.Add(new SelectOption(tarea.Id, $"{tarea.Id} - {tarea.Descripcion}"));
            }
        }

        public virtual void ConfirmarTareaAmigable(IUnitOfWork uow, Form form)
        {
            var orden = uow.OrdenRepository.GetOrden(int.Parse(form.GetField("numeroOrden").Value));
            var userId = uow.SecurityRepository.GetUserIdByLoginName(form.GetField("usuario").Value);
            var cdEmpresa = int.Parse(form.GetField("codigoEmpresa").Value);
            var cdTarea = form.GetField("codigoTarea").Value;

            if (!uow.TareaRepository.AnyOrdenTarea(orden.Id, cdTarea, cdEmpresa))
            {
                var nuevaOrdenTarea = CrearOrdenTarea(form, userId);
                uow.TareaRepository.AddOrdenTarea(nuevaOrdenTarea);

                var ordenTareaFunc = CrearOrdenTareaFuncionario(form, nuevaOrdenTarea.NuTarea, userId.Value);
                uow.TareaRepository.AddOrdenTareaFuncionario(ordenTareaFunc);
            }
            else if (uow.TareaRepository.GetOrdenTarea(orden.Id, cdTarea, cdEmpresa) != null)
            {
                var ordenTarea = uow.TareaRepository.GetOrdenTarea(orden.Id, cdTarea, cdEmpresa);

                if (ordenTarea.Resuelta != "N")
                {
                    ordenTarea.Resuelta = "N";
                    uow.TareaRepository.UpdateOrdenTarea(ordenTarea);
                }

                var ordenTareaFunc = CrearOrdenTareaFuncionario(form, ordenTarea.NuTarea, userId.Value);
                uow.TareaRepository.AddOrdenTareaFuncionario(ordenTareaFunc);
            }
        }

        public virtual void TerminarTareaAmigable(IUnitOfWork uow, Form form)
        {
            var userId = uow.SecurityRepository.GetUserIdByLoginName(form.GetField("usuario").Value);
            var ordenTareaFunc = uow.TareaRepository.GetOrdenTareaFuncionarioAmigableByUserId(userId.Value);

            ordenTareaFunc.FechaHasta = DateTime.Parse(form.GetField("fechaFin").Value, this._identity.GetFormatProvider());

            uow.TareaRepository.UpdateOrdenTareaFuncionario(ordenTareaFunc);
        }

        public virtual OrdenTareaObjeto CrearOrdenTarea(Form form, int? userId)
        {
            return new OrdenTareaObjeto()
            {
                CdTarea = form.GetField("codigoTarea").Value,
                NuOrden = int.Parse(form.GetField("numeroOrden").Value),
                Empresa = int.Parse(form.GetField("codigoEmpresa").Value),
                DtAddrow = DateTime.Now,
                Resuelta = "N",
                CdFuncionarioAddrow = userId
            };
        }

        public virtual OrdenTareaFuncionario CrearOrdenTareaFuncionario(Form form, long nuOrdenTarea, int userId)
        {
            var fechaHastaStr = form.GetField("fechaFin").Value;
            DateTime? fechaHasta = DateTime.TryParse(fechaHastaStr, this._identity.GetFormatProvider(), out var fechaParsed)
                ? fechaParsed
                : null;

            return new OrdenTareaFuncionario()
            {
                NumeroOrdenTarea = nuOrdenTarea,
                CodigoFuncionario = userId,
                FechaDesde = DateTime.Parse(form.GetField("fechaInicio").Value, this._identity.GetFormatProvider()),
                FechaHasta = fechaHasta,
            };
        }

        #endregion
    }
}