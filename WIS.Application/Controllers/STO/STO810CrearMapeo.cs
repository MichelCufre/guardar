using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Stock;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.StockEntities;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.STO
{
    public class STO810CrearMapeo : AppController
    {
        protected readonly IIdentityService _identity;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IFormValidationService _formValidationService;
        protected readonly ISecurityService _security;

        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public STO810CrearMapeo(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            ISecurityService security)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._security = security;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            form.GetField("cdEmpresaOrigen").Value = string.Empty;
            form.GetField("cdProdutoOrigen").Value = string.Empty;
            form.GetField("cantidadOrigen").Value = string.Empty;
            form.GetField("cdEmpresaDestino").Value = string.Empty;
            form.GetField("cdProdutoDestino").Value = string.Empty;
            form.GetField("cantidadDestino").Value = string.Empty;

            this.InicializarSelects(form);

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new TraspasoEmpresasMapeoProductosValidationModule(uow, this._identity.GetFormatProvider(), this._identity, this._security), form, context);
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "cdEmpresaOrigen":
                    return this.SearchEmpresa(form, context);
                case "cdEmpresaDestino":
                    return this.SearchEmpresa(form, context);
                case "cdProdutoOrigen":
                    return this.SearchProducto(form, context, true);
                case "cdProdutoDestino":
                    return this.SearchProducto(form, context, false);
                default:
                    return new List<SelectOption>();
            }
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber("STO810 Alta Mapeo de Productos");
            uow.BeginTransaction();

            try
            {
                var mapeo = new MapeoProducto
                {
                    EmpresaOrigen = int.Parse(form.GetField("cdEmpresaOrigen").Value),
                    EmpresaDestino = int.Parse(form.GetField("cdEmpresaDestino").Value),
                    ProductoOrigen = form.GetField("cdProdutoOrigen").Value,
                    ProductoDestino = form.GetField("cdProdutoDestino").Value,
                    FaixaOrigen = 1,
                    FaixaDestino = 1,
                    CantidadOrigen = decimal.Parse(form.GetField("cantidadOrigen").Value, _identity.GetFormatProvider()),
                    CantidadDestino = decimal.Parse(form.GetField("cantidadDestino").Value, _identity.GetFormatProvider()),
                    FechaAlta = DateTime.Now,
                };

                uow.TraspasoEmpresasRepository.AddMapeoProductos(mapeo);

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");
            }
            catch (ValidationFailedException ex)
            {
                _logger.Error($"Error {ex.Message} - {ex}");
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                uow.Rollback();
            }
            catch (Exception ex)
            {
                _logger.Error($"Error {ex.Message} - {ex}");
                context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }

            return form;
        }

        #region Metodos Auxiliares 

        public virtual void InicializarSelects(Form form)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var selectEmpresaOrigen = form.GetField("cdEmpresaOrigen");
            var selectEmpresaDestino = form.GetField("cdEmpresaOrigen");
            var empresa = uow.EmpresaRepository.GetEmpresaUnicaParaUsuario(_identity.UserId);

            if (empresa != null)
            {
                selectEmpresaOrigen.ReadOnly = true;
                selectEmpresaOrigen.Value = empresa.Id.ToString();
                selectEmpresaOrigen.Options = new List<SelectOption> { new SelectOption(selectEmpresaOrigen.Value, empresa.Nombre) };

                selectEmpresaDestino.ReadOnly = true;
                selectEmpresaDestino.Value = empresa.Id.ToString();
                selectEmpresaDestino.Options = new List<SelectOption> { new SelectOption(selectEmpresaDestino.Value, empresa.Nombre) };
            }
            else
            {
                selectEmpresaOrigen.Value = string.Empty;
                selectEmpresaOrigen.Options = new List<SelectOption>();

                selectEmpresaDestino.Value = string.Empty;
                selectEmpresaDestino.Options = new List<SelectOption>();
            }
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

        public virtual List<SelectOption> SearchProducto(Form form, FormSelectSearchContext context, bool isOrigen)
        {
            string paramEmpresa = isOrigen ? form.GetField("cdEmpresaOrigen").Value : form.GetField("cdEmpresaDestino").Value;

            List<SelectOption> opciones = new List<SelectOption>();

            if (string.IsNullOrEmpty(paramEmpresa))
                return opciones;

            int empresa = int.Parse(paramEmpresa);

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                List<Producto> productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(empresa, context.SearchValue);

                foreach (Producto producto in productos)
                {
                    opciones.Add(new SelectOption(producto.Codigo, $"{producto.Codigo} - {producto.Descripcion}"));
                }
            }

            return opciones;
        }

        #endregion
    }
}
