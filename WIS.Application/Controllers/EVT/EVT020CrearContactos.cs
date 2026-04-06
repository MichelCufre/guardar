using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.Eventos;
using WIS.Domain.General;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent.Build;
using WIS.GridComponent.Excel;
using WIS.Security;

namespace WIS.Application.Controllers.EVT
{
    public class EVT020CrearContactos : AppController
    {
        protected readonly IIdentityService _identity;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<EVT020CrearContactos> _logger;

        protected List<string> GridKeys { get; }

        public EVT020CrearContactos(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<EVT020CrearContactos> logger)
        {

            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("EVT020 Crear Contacto");
            var nuTransaccion = uow.GetTransactionNumber();

            try
            {
                var nuevoContacto = MapContacto(form, context);

                nuevoContacto.Id = uow.DestinatarioRepository.GetNextNuContacto();
                nuevoContacto.NumeroTransaccion = nuTransaccion;

                uow.DestinatarioRepository.AddContacto(nuevoContacto);

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("EVT020_frm1_success_addContacto");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "EVT020FormSubmit");
                context.AddErrorNotification("EVT020_Sec0_Error_Error01");
                uow.Rollback();
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new EVT020CrearContactoValidationModule(uow), form, context);
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "CD_EMPRESA":
                    return this.SearchEmpresa(form, context);
                case "CD_CLIENTE":
                    return this.SearchAgente(form, context);
                default:
                    return new List<SelectOption>();
            }
        }

        #region Metodos Auxiliares

        protected virtual Contacto MapContacto(Form form, FormSubmitContext context)
        {
            var cliente = form.GetField("CD_CLIENTE").Value;

            Contacto contacto = new Contacto()
            {
                Nombre = form.GetField("NM_CONTACTO").Value,
                Descripcion = form.GetField("DS_CONTACTO").Value,
                Email = form.GetField("DS_EMAIL").Value,
                Telefono = form.GetField("NU_TELEFONO").Value,
                CodigoEmpresa = int.Parse(form.GetField("CD_EMPRESA").Value),
                FechaAlta = DateTime.Now,
            };

            if (!string.IsNullOrEmpty(cliente))
                contacto.CodigoCliente = cliente;

            return contacto;
        }

        protected virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Empresa> empresas = uow.EmpresaRepository.GetByNombreOrCodePartial(context.SearchValue);

            foreach (Empresa empresa in empresas)
            {
                opciones.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}"));
            }

            return opciones;
        }

        protected virtual List<SelectOption> SearchAgente(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            if (int.TryParse(form.GetField("CD_EMPRESA").Value, out int empresa))
            {
                using var uow = this._uowFactory.GetUnitOfWork();

                List<Agente> clientes = uow.AgenteRepository.GetByDescripcionOrAgentePartial(context.SearchValue, empresa);

                foreach (var cliente in clientes)
                {
                    opciones.Add(new SelectOption(cliente.CodigoInterno, cliente.Codigo + "-" + cliente.Tipo + "-" + cliente.Descripcion));
                }
            }
            else
                context.AddErrorNotification("General_Sec0_Error_EmpresaNecesaria");

            return opciones;
        }

        #endregion
    }
}
