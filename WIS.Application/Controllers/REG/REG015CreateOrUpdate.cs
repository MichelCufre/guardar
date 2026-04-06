using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Eventos;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.REG
{
    public class REG015CreateOrUpdate : AppController
    {
        protected readonly ILogger<REG015CreateOrUpdate> _logger;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;

        public REG015CreateOrUpdate(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            ISecurityService security,
            IFormValidationService formValidationService,
            ILogger<REG015CreateOrUpdate> logger)
        {
            _logger = logger;
            _identity = identity;
            _security = security;
            _uowFactory = uowFactory;
            _formValidationService = formValidationService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                InicializarFormulario(uow, form, context);
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.AddErrorNotification(ex.Message);
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
                var isUpdate = context.Parameters.FirstOrDefault(i => i.Id == "isUpdate")?.Value == "S";

                if (!isUpdate)
                    CrearProductoProveedor(uow, form);
                else
                    ModificarProductoProveedor(uow, form, context);

                context.AddSuccessNotification("General_Sec0_Success_SavedChanges");

                uow.SaveChanges();
                uow.Commit();
            }
            catch (ValidationFailedException ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                uow.Rollback();
                context.AddErrorNotification(ex.Message);
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new RegistroProductosProveedorValidationModule(uow, this._security), form, context);
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "empresa": return this.SearchEmpresa(form, context);
                case "cliente": return this.SearchCliente(form, context);
                case "producto": return this.SearchProducto(form, context);
            }

            return new List<SelectOption>();
        }

        #region Metodos Auxiliares

        public virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Empresa> empresas = uow.EmpresaRepository.GetByNombreOrCodePartial(context.SearchValue);

            foreach (var empresa in empresas)
            {
                opciones.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchCliente(Form form, FormSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();

            var empresa = form.GetField("empresa").Value;

            if (string.IsNullOrEmpty(empresa) || !int.TryParse(empresa, out int empresaId))
                return opciones;

            using var uow = this._uowFactory.GetUnitOfWork();

            var agentes = uow.AgenteRepository.GetByDescripcionOrAgentePartial(context.SearchValue, empresaId);

            foreach (var agente in agentes)
            {
                opciones.Add(new SelectOption(agente.CodigoInterno, $"{agente.Empresa} - {agente.Codigo} - {agente.Tipo} - {agente.Descripcion} "));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchProducto(Form form, FormSelectSearchContext context)
        {
            var empresa = form.GetField("empresa").Value;

            List<SelectOption> opciones = new List<SelectOption>();

            if (string.IsNullOrEmpty(empresa) || !int.TryParse(empresa, out int empresaId))
                return opciones;

            using var uow = this._uowFactory.GetUnitOfWork();

            var productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(empresaId, context.SearchValue);

            foreach (var producto in productos)
            {
                opciones.Add(new SelectOption(producto.Codigo, producto.Codigo + " - " + producto.Descripcion));
            }

            return opciones;
        }

        public virtual void InicializarFormulario(IUnitOfWork uow, Form form, FormInitializeContext context)
        {
            form.GetField("empresa").ReadOnly = false;
            form.GetField("cliente").ReadOnly = false;
            form.GetField("producto").ReadOnly = false;
            form.GetField("codigoExterno").Value = string.Empty;

            var isUpdate = context.Parameters.FirstOrDefault(i => i.Id == "isUpdate")?.Value == "S";

            if (isUpdate)
            {
                var empresa = context.GetParameter("empresa");
                var cliente = context.GetParameter("cliente");
                var producto = context.GetParameter("producto");

                if (!int.TryParse(empresa, out int idEmpresa)
                    || string.IsNullOrEmpty(cliente)
                    || string.IsNullOrEmpty(producto))
                {
                    throw new ValidationFailedException("General_Sec0_Error_ParametrosInvalidos");
                }

                var prodProveedor = uow.ProductoRepository.GetProductoProveedor(idEmpresa, producto, cliente);
                if (prodProveedor == null)
                    throw new ValidationFailedException("REG015_Sec0_Error_ProdProveedorNoEncontrado", new string[] { empresa, cliente, producto });

                this.InicializarSelectEmpresa(form, empresa);
                this.InicializarSelectCliente(form, cliente);
                this.InicializarSelectProducto(form, producto);

                form.GetField("codigoExterno").Value = prodProveedor.CodigoExterno;
            }
        }

        public virtual void InicializarSelectEmpresa(Form form, string empresa)
        {
            form.GetField("empresa").Options = this.SearchEmpresa(form, new FormSelectSearchContext()
            {
                SearchValue = empresa
            });
            form.GetField("empresa").Value = empresa;
            form.GetField("empresa").ReadOnly = true;
        }

        public virtual void InicializarSelectCliente(Form form, string cliente)
        {
            form.GetField("cliente").Options = this.SearchCliente(form, new FormSelectSearchContext()
            {
                SearchValue = cliente
            });
            form.GetField("cliente").Value = cliente;
            form.GetField("cliente").ReadOnly = true;
        }

        public virtual void InicializarSelectProducto(Form form, string producto)
        {
            form.GetField("producto").Options = this.SearchProducto(form, new FormSelectSearchContext()
            {
                SearchValue = producto
            });
            form.GetField("producto").Value = producto;
            form.GetField("producto").ReadOnly = true;
        }

        public virtual void CrearProductoProveedor(IUnitOfWork uow, Form form)
        {
            var newProductoProveedor = new ProductoProveedor()
            {
                Empresa = int.Parse(form.GetField("empresa").Value),
                Cliente = form.GetField("cliente").Value,
                CodigoProducto = form.GetField("producto").Value,
                CodigoExterno = form.GetField("codigoExterno").Value,
                FechaIngreso = DateTime.Now
            };

            uow.ProductoRepository.AddProductoProveedor(newProductoProveedor);
        }

        public virtual void ModificarProductoProveedor(IUnitOfWork uow, Form form, FormSubmitContext context)
        {
            var empresa = int.Parse(form.GetField("empresa").Value);
            var cliente = form.GetField("cliente").Value;
            var producto = form.GetField("producto").Value;

            var prodProveedor = uow.ProductoRepository.GetProductoProveedor(empresa, producto, cliente);
            if (prodProveedor == null)
                throw new ValidationFailedException("REG015_Sec0_Error_ProdProveedorNoEncontrado", new string[] { empresa.ToString(), cliente, producto });

            prodProveedor.CodigoExterno = form.GetField("codigoExterno").Value;
            prodProveedor.FechaModificacion = DateTime.Now;

            uow.ProductoRepository.UpdateProductoProveedor(prodProveedor);
        }

        #endregion
    }
}
