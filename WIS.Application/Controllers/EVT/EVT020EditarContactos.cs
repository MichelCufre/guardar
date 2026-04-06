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
    public class EVT020EditarContactos : AppController
    {
        protected readonly IIdentityService _identity;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<EVT020EditarContactos> _logger;

        protected List<string> GridKeys { get; }

        public EVT020EditarContactos(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<EVT020EditarContactos> logger)
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
            if (int.TryParse(context.GetParameter("numeroContacto"), out int numeroContacto))
            {
                using var uow = this._uowFactory.GetUnitOfWork();

                Contacto contacto = uow.DestinatarioRepository.GetContacto(numeroContacto);

                form.GetField("NU_CONTACTO").Value = contacto.Id.ToString();
                form.GetField("DS_CONTACTO").Value = contacto.Descripcion;
                form.GetField("NM_CONTACTO").Value = contacto.Nombre;
                form.GetField("DS_EMAIL").Value = contacto.Email;
                form.GetField("NU_TELEFONO").Value = contacto.Telefono;

                var selectEmpresa = form.GetField("CD_EMPRESA");
                var selectAgente = form.GetField("CD_CLIENTE");

                selectEmpresa.Options = SearchEmpresa(form, new FormSelectSearchContext()
                {
                    SearchValue = contacto.CodigoEmpresa.ToString()
                });

                selectEmpresa.Value = contacto.CodigoEmpresa.ToString();

                if (contacto.CodigoCliente != null)
                {
                    var cliente = uow.AgenteRepository.GetAgente(contacto.CodigoEmpresa, contacto.CodigoCliente);
                    
                    selectAgente.Options = SearchAgente(form, new FormSelectSearchContext()
                    {
                        SearchValue = cliente.Codigo
                    });

                    selectAgente.Value = contacto.CodigoCliente.ToString();
                }
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("EVT020 Editar Contacto");
            var nuTransaccion = uow.GetTransactionNumber();

            try
            {
                var numeroContacto = int.Parse(context.GetParameter("numeroContacto"));

                var contacto = uow.DestinatarioRepository.GetContacto(numeroContacto);

                var contactoModificado = this.MapContacto(form, contacto, context);
                contactoModificado.NumeroTransaccion = nuTransaccion;

                uow.DestinatarioRepository.UpdateContacto(contactoModificado);

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("EVT020_frm1_success_updateContacto");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "EVT020FormSubmit");
                context.AddErrorNotification("EVT020_Sec0_Error_Error02");
                uow.Rollback();
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new EVT020EditarContactoValidationModule(uow), form, context);
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

        protected virtual Contacto MapContacto(Form form, Contacto contacto, FormSubmitContext context)
        {
            var cliente = form.GetField("CD_CLIENTE").Value;

            contacto.Nombre = form.GetField("NM_CONTACTO").Value;
            contacto.Descripcion = form.GetField("DS_CONTACTO").Value;
            contacto.Email = form.GetField("DS_EMAIL").Value;
            contacto.Telefono = form.GetField("NU_TELEFONO").Value;
            contacto.CodigoEmpresa = int.Parse(form.GetField("CD_EMPRESA").Value);
            contacto.FechaModificacion = DateTime.Now;

            if (!string.IsNullOrEmpty(cliente))
                contacto.CodigoCliente = cliente;
            else
                contacto.CodigoCliente = null;

            return contacto;
        }

        public virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext context)
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
