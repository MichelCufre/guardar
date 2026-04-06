using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Security;
using WIS.Domain.DataModel;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Domain.General;
using WIS.Components.Common.Select;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common;
using WIS.Domain.Registro;
using WIS.Domain.Recepcion;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.General.Auxiliares;
using WIS.Application.Security;
using WIS.Extension;
using WIS.Application.Validation;
using System.Globalization;
using WIS.Domain.DataModel.Mappers.Constants;

namespace WIS.Application.Controllers.REC
{
    public class REC010CrearReferenciaRecepcion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;

        public REC010CrearReferenciaRecepcion(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            ISecurityService security)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._security = security;
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!uow.ReferenciaRecepcionRepository.GetReferenciaConfiguracion().PermiteDigitacion)
                throw new ValidationFailedException("REC010_frm1_error_DigitacionLineasNoPermitido");

            this.InicializarSelects(form);

            form.GetField("fechaEmitida").Value = DateTime.Now.Date.ToIsoString();

            if (this._security.IsUserAllowed(SecurityResources.WREC011_Page_Access_Puede_Insertar))
            {
                query.AddParameter("PermiteIngresarLineas", "true");
            }

            return form;
        }
        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext query)
        {
            switch (query.FieldId)
            {
                case "idEmpresa": return this.SearchEmpresa(form, query);
                case "codigoInternoAgente": return this.SearchAgente(form, query);

                default: return new List<SelectOption>();
            }
        }
        public override Form FormSubmit(Form form, FormSubmitContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!uow.ReferenciaRecepcionRepository.GetReferenciaConfiguracion().PermiteDigitacion)
                throw new ValidationFailedException("REC010_frm1_error_DigitacionLineasNoPermitido");
                        
            uow.CreateTransactionNumber($"{this._identity.Application} - Crear Referencia de Recepcion");

            var referencia = new ReferenciaRecepcion()
            {
                Numero = form.GetField("codigo").Value,
                TipoReferencia = form.GetField("tipoReferencia").Value,
                IdEmpresa = int.Parse(form.GetField("idEmpresa").Value),
                CodigoCliente = form.GetField("codigoInternoAgente").Value,
                IdPredio = form.GetField("numeroPredio").Value,

                FechaEmitida = DateTime.Parse(form.GetField("fechaEmitida").Value, _identity.GetFormatProvider()),
                Memo = form.GetField("memo").Value,
                Anexo1 = form.GetField("anexo1").Value,
                Anexo2 = form.GetField("anexo2").Value,
                Anexo3 = form.GetField("anexo3").Value,
                Estado = EstadoReferenciaRecepcionDb.Abierta,
                Moneda = form.GetField("moneda").Value,

                NumeroTransaccion = uow.GetTransactionNumber(),
            };

            if (DateTime.TryParse(form.GetField("fechaVencimiento").Value, this._identity.GetFormatProvider(), DateTimeStyles.None, out DateTime vencimiento))
            {
                referencia.FechaVencimientoOrden = vencimiento;
            }

            if (DateTime.TryParse(form.GetField("fechaEntrega").Value, this._identity.GetFormatProvider(), DateTimeStyles.None, out DateTime entrega))
            {
                referencia.FechaEntrega = entrega;
            }

            var registro = new RegistroReferenciaRecepcion(uow, this._identity.UserId, this._identity.Application);
            registro.RegistrarReferencia(referencia);

            uow.SaveChanges();

            query.Parameters?.Clear();
            query.Parameters.Add(new ComponentParameter("idReferencia", referencia.Id.ToString()));

            return form;
        }
        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new MantenimientoReferenciaRecepcionFormValidationModule(uow, this._identity.UserId), form, context);
        }

        public virtual void InicializarSelects(Form form)
        {
            //Inicializar selects
            FormField selectTipoReferencia = form.GetField("tipoReferencia");
            FormField selectPredio = form.GetField("numeroPredio");
            FormField selectMoneda = form.GetField("moneda");

            selectTipoReferencia.Options = new List<SelectOption>();
            selectPredio.Options = new List<SelectOption>();
            selectMoneda.Options = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            // Tipos de recepción
            List<ReferenciaRecepcionTipo> tiposAlmacenajes = uow.ReferenciaRecepcionRepository.GetReferenciaRecepcionTipos();

            foreach (var tipo in tiposAlmacenajes)
            {
                selectTipoReferencia.Options.Add(new SelectOption(tipo.Tipo.ToString(), $"{tipo.Tipo} - {tipo.Descripcion }"));
            }

            // Predios
            var dbQuery = new GetPrediosUsuarioQuery();
            uow.HandleQuery(dbQuery);

            List<Predio> predios = dbQuery.GetPrediosUsuario(_identity.UserId);
            foreach (var predio in predios)
            {
                selectPredio.Options.Add(new SelectOption(predio.Numero, $"{predio.Numero} - { predio.Descripcion}")); ;
            }

            if (predios.Count == 1)
                selectPredio.Value = predios.FirstOrDefault().Numero;

            // Moneda
            List<Moneda> monedas = uow.MonedaRepository.GetMonedas();

            foreach (var moneda in monedas)
            {
                selectMoneda.Options.Add(new SelectOption(moneda.Codigo, $"{moneda.Codigo} - {moneda.Descripcion }"));
            }
        }
        public virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Empresa> empresas = uow.EmpresaRepository.GetByNombreOrCodePartial(context.SearchValue);

            foreach (var empresa in empresas)
            {
                opciones.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}"));
            }

            return opciones;
        }
        public virtual List<SelectOption> SearchAgente(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            if (int.TryParse(form.GetField("idEmpresa").Value, out int idEmpresa))
            {

                var dbQuery = new GetAgentesQuery(context.SearchValue, idEmpresa);
                uow.HandleQuery(dbQuery);

                List<AgenteAuxiliar> agentes = dbQuery.GetByDescripcionOrAgentePartial(context.SearchValue, idEmpresa);

                foreach (var agente in agentes)
                {
                    opciones.Add(new SelectOption(agente.CodigoInterno, $"{agente.Tipo} - {agente.Codigo} - {agente.Descripcion}"));
                }

            }
            else
            {
                form.GetField("idEmpresa").SetError("General_Sec0_Error_Error25");
            }

            return opciones;
        }
    }
}
