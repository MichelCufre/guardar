using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Parametrizacion;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Parametrizacion;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.PAR
{
    public class PAR400CrearAtributo : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly ILogger<PAR400CrearAtributo> _logger;

        public PAR400CrearAtributo(IUnitOfWorkFactory uowFactory, IIdentityService identity, IFormValidationService formValidationService, ILogger<PAR400CrearAtributo> logger)
        {
            _uowFactory = uowFactory;
            _identity = identity;
            _formValidationService = formValidationService;
            _logger = logger;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            InicializarSelect(uow, form);

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                var atributo = CrearAtributo(form, uow);
                uow.AtributoRepository.AddAtributo(atributo);

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw ex;
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new PAR400CrearAtributoFormValidationModule(uow, this._identity), form, context);
        }

        public virtual Atributo CrearAtributo(Form form, IUnitOfWork uow)
        {
            var tipoSelect = form.GetField("tipo").Value;
            var tipo = uow.AtributoRepository.GetTipoAtributoById(tipoSelect);

            var atributo = new Atributo();

            atributo.Descripcion = form.GetField("descripcion").Value;
            atributo.IdTipo = form.GetField("tipo").Value;
            atributo.Nombre = form.GetField("nombre").Value;

            switch (tipo.Id)
            {
                case TipoAtributoDb.SISTEMA:
                    atributo.Campo = form.GetField("campo").Value;
                    break;
                case TipoAtributoDb.NUMERICO:
                    atributo.Largo = short.Parse(form.GetField("largo").Value);
                    atributo.Decimales = short.Parse(form.GetField("decimales").Value);
                    atributo.Separador = form.GetField("separador").Value;
                    break;
                case TipoAtributoDb.FECHA:
                    atributo.MascaraIngreso = form.GetField("mascaraIngresoFecha").Value;
                    atributo.MascaraDisplay = form.GetField("displayFecha").Value;
                    break;
                case TipoAtributoDb.HORA:
                    atributo.MascaraIngreso = form.GetField("mascaraIngresoHora").Value;
                    atributo.MascaraDisplay = form.GetField("displayHora").Value;
                    break;
                case TipoAtributoDb.DOMINIO:
                    atributo.CodigoDominio = form.GetField("dominio").Value;
                    break;
            }

            return atributo;
        }

        public virtual void InicializarSelect(IUnitOfWork uow, Form form)
        {
            var selectorTipo = form.GetField("tipo");
            var selectorDominios = form.GetField("dominio");
            var selectorSeparador = form.GetField("separador");
            var selectorCampo = form.GetField("campo");
            var selectorFormatoFecha = form.GetField("mascaraIngresoFecha");
            var selectorFormatoHora = form.GetField("mascaraIngresoHora");
            var selectorDisplayFecha = form.GetField("displayFecha");
            var selectorDisplayHora = form.GetField("displayHora");

            selectorFormatoFecha.Options = new List<SelectOption>();
            selectorFormatoHora.Options = new List<SelectOption>();
            selectorDisplayFecha.Options = new List<SelectOption>();
            selectorDisplayHora.Options = new List<SelectOption>();
            selectorCampo.Options = new List<SelectOption>();
            selectorTipo.Options = new List<SelectOption>();
            selectorDominios.Options = new List<SelectOption>();
            selectorSeparador.Options = new List<SelectOption>()
            {
                new SelectOption(",", ","),
                new SelectOption(".", ".")
            };

            var formatosFechas = uow.AtributoRepository.GetAllFormatosFecha();
            foreach (var formato in formatosFechas)
            {
                selectorFormatoFecha.Options.Add(new SelectOption(formato, formato));
                selectorDisplayFecha.Options.Add(new SelectOption(formato, formato));
            }

            var formatosHoras = uow.AtributoRepository.GetAllFormatosHora();
            foreach (var formatoHora in formatosHoras)
            {
                selectorFormatoHora.Options.Add(new SelectOption(formatoHora, $"{formatoHora}"));
                selectorDisplayHora.Options.Add(new SelectOption(formatoHora, $"{formatoHora}"));
            }

            var tipos = uow.AtributoRepository.GetAllTipoAtributos();
            foreach (var tipo in tipos)
            {
                selectorTipo.Options.Add(new SelectOption(tipo.Id, tipo.Descripcion));
            }

            var dominios = uow.DominioRepository.GetAllDominios().Where(d => d.Detalles.Count > 0);
            foreach (var dominio in dominios)
            {
                selectorDominios.Options.Add(new SelectOption(dominio.Codigo, $"{dominio.Codigo} - {dominio.Descripcion}"));
            }

            var atributosSistema = uow.AtributoRepository.GetAllAtributoSistema();
            foreach (var atributoSistema in atributosSistema)
            {
                selectorCampo.Options.Add(new SelectOption(atributoSistema.Nombre, atributoSistema.Descripcion));
            }
        }
    }
}
