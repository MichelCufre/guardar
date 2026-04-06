using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.General;
using WIS.Domain.General.Auxiliares;
using WIS.Domain.Recepcion;
using WIS.Exceptions;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.REC
{
    public class REC500ModificarFactura : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;

        public REC500ModificarFactura(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var idFactura = context.GetParameter("keyFactura");

            if (!string.IsNullOrEmpty(idFactura))
            {
                Factura factura = uow.FacturaRepository.GetFactura(int.Parse(idFactura));

                if (factura == null)
                    throw new ValidationFailedException("REC500_Frm1_Error_FacturaNoExiste", new string[] { idFactura });//

                InicializarSelects(form, factura);

                InicializarCamposUpdate(uow, form, context, factura);
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new FacturaFormValidationModule(uow, this._identity.UserId, this._identity.Predio, this._identity.GetFormatProvider()), form, context);
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var idFactura = context.GetParameter("keyFactura");

            var factura = uow.FacturaRepository.GetFactura(int.Parse(idFactura));

            if (factura == null)
                throw new ValidationFailedException("REC500_Frm1_Error_FacturaNoExiste", new string[] { idFactura });

            uow.CreateTransactionNumber("REC500 Modificar factura");

            if (!factura.Agenda.HasValue || (factura.Agenda.HasValue && !uow.AgendaRepository.IsAgendaFacturaValida(factura.Agenda.Value)))
            {
                factura.TipoFactura = form.GetField("tipoFactura").Value;
                factura.CodigoMoneda = form.GetField("moneda").Value;
                factura.Situacion = SituacionDb.Activo;

                if (DateTime.TryParse(form.GetField("vencimiento")?.Value, _identity.GetFormatProvider(), out DateTime vencimiento))
                    factura.FechaVencimiento = vencimiento;

                if (DateTime.TryParse(form.GetField("emision")?.Value, _identity.GetFormatProvider(), out DateTime emision))
                    factura.FechaEmision = emision;

                if (decimal.TryParse(form.GetField("totalDigitado").Value, _identity.GetFormatProvider(), out decimal totalDigitado))
                    factura.TotalDigitado = totalDigitado;

                if (decimal.TryParse(form.GetField("ivaBase")?.Value, _identity.GetFormatProvider(), out decimal ivaBase))
                    factura.IvaBase = ivaBase;

                if (decimal.TryParse(form.GetField("ivaMinimo")?.Value, _identity.GetFormatProvider(), out decimal ivaMinimo))
                    factura.IvaMinimo = ivaMinimo;
            }

            factura.Anexo1 = form.GetField("anexo1").Value;
            factura.Anexo2 = form.GetField("anexo2").Value;
            factura.Anexo3 = form.GetField("anexo3").Value;
            factura.Observacion = form.GetField("observacion").Value;
            factura.NumeroTransaccion = uow.GetTransactionNumber();
            factura.FechaModificacion = DateTime.Now;

            uow.FacturaRepository.UpdateFactura(factura);
            uow.SaveChanges();

            context.AddSuccessNotification("REC500_Frm1_Warning_Edicion", new List<string> { factura.Id.ToString() });

            context.Parameters?.Clear();
            context.Parameters.Add(new ComponentParameter("idFactura", factura.Id.ToString()));

            return form;
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            var idFactura = context.GetParameter("keyFactura");

            switch (context.FieldId)
            {
                case "idEmpresa": return this.SearchEmpresa(form, context);
                case "codigoInternoAgente": return this.SearchAgente(form, context);

                default: return new List<SelectOption>();
            }
        }

        #region Metodos Auxiliares

        public virtual void InicializarSelects(Form form, Factura factura)
        {
            //Inicializar selects
            FormField selectPredio = form.GetField("numeroPredio");
            FormField selectMoneda = form.GetField("moneda");
            FormField selectTipoFactura = form.GetField("tipoFactura");

            selectPredio.Options = new List<SelectOption>();
            selectMoneda.Options = new List<SelectOption>();
            selectTipoFactura.Options = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            // Predios
            var dbQuery = new GetPrediosUsuarioQuery();
            uow.HandleQuery(dbQuery);

            List<Predio> predios = dbQuery.GetPrediosUsuario(_identity.UserId);
            foreach (var predio in predios)
            {
                selectPredio.Options.Add(new SelectOption(predio.Numero, $"{predio.Numero} - {predio.Descripcion}")); ;
            }

            if (predios.Count == 1)
                selectPredio.Value = predios.FirstOrDefault().Numero;

            var monedas = uow.MonedaRepository.GetMonedas();
            foreach (var moneda in monedas)
            {
                selectMoneda.Options.Add(new SelectOption(moneda.Codigo, moneda.Descripcion));
            }

            var tipos = uow.DominioRepository.GetDominios(CodigoDominioDb.TiposDeFacturas);
            foreach (var tipo in tipos)
            {
                selectTipoFactura.Options.Add(new SelectOption(tipo.Valor, tipo.Descripcion));
            }
            selectTipoFactura.Value = tipos.FirstOrDefault()?.Valor;

        }

        public virtual void InicializarCamposUpdate(IUnitOfWork uow, Form form, FormInitializeContext context, Factura factura)
        {
            // Marcar campos solo de lectura
            form.GetField("idEmpresa").ReadOnly = true;
            form.GetField("numeroPredio").ReadOnly = true;
            form.GetField("codigoInternoAgente").ReadOnly = true;
            form.GetField("numeroSerie").ReadOnly = true;
            form.GetField("numeroFactura").ReadOnly = true;

            form.GetField("tipoFactura").ReadOnly = false;
            form.GetField("moneda").ReadOnly = false;
            form.GetField("vencimiento").ReadOnly = false;
            form.GetField("emision").ReadOnly = false;
            form.GetField("totalDigitado").ReadOnly = false;
            form.GetField("ivaBase").ReadOnly = false;
            form.GetField("ivaMinimo").ReadOnly = false;

            if (factura.Agenda.HasValue && uow.AgendaRepository.IsAgendaFacturaValida(factura.Agenda.Value))
            {
                form.GetField("tipoFactura").ReadOnly = true;
                form.GetField("moneda").ReadOnly = true;
                form.GetField("vencimiento").ReadOnly = true;
                form.GetField("emision").ReadOnly = true;
                form.GetField("totalDigitado").ReadOnly = true;
                form.GetField("ivaBase").ReadOnly = true;
                form.GetField("ivaMinimo").ReadOnly = true;
            }

            // Cargar valores iniciales
            form.GetField("numeroPredio").Value = factura.Predio;
            form.GetField("tipoFactura").Value = factura.TipoFactura;
            form.GetField("moneda").Value = factura.CodigoMoneda;
            form.GetField("numeroSerie").Value = factura.Serie;
            form.GetField("numeroFactura").Value = factura.NumeroFactura;
            form.GetField("vencimiento").Value = factura.FechaVencimiento.ToIsoString();
            form.GetField("emision").Value = factura.FechaEmision.ToIsoString();
            form.GetField("totalDigitado").Value = factura.TotalDigitado?.ToString(this._identity.GetFormatProvider());
            form.GetField("ivaBase").Value = factura.IvaBase?.ToString(this._identity.GetFormatProvider());
            form.GetField("ivaMinimo").Value = factura.IvaMinimo?.ToString(this._identity.GetFormatProvider());

            // Carga de search Empresa
            var fieldEmpresa = form.GetField("idEmpresa");
            fieldEmpresa.Options = SearchEmpresa(form, new FormSelectSearchContext()
            {
                SearchValue = factura.IdEmpresa.ToString()
            });
            fieldEmpresa.Value = factura.IdEmpresa.ToString();

            // Carga de search Agente
            var fieldAgente = form.GetField("codigoInternoAgente");
            fieldAgente.Options = SearchAgente(form, new FormSelectSearchContext()
            {
                SearchValue = factura.CodigoInternoCliente.ToString()
            });
            fieldAgente.Value = factura.CodigoInternoCliente.ToString();


            form.GetField("anexo1").Value = factura.Anexo1;
            form.GetField("anexo2").Value = factura.Anexo2;
            form.GetField("anexo3").Value = factura.Anexo3;
            form.GetField("observacion").Value = factura.Observacion;
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

        #endregion
    }
}
