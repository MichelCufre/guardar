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
    public class STO810ModificarMapeo : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;

        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public STO810ModificarMapeo(IIdentityService identity,
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
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(context.GetParameter("cdEmpresaOrigen"), out int cdEmpresaOrigen)
                || !int.TryParse(context.GetParameter("cdEmpresaDestino"), out int cdEmpresaDestino)
                || string.IsNullOrEmpty(context.GetParameter("cdProdutoOrigen")))
            {
                throw new ValidationFailedException("STO810_Sec0_Error_MapeoNoValido");
            }
            else if (!uow.TraspasoEmpresasRepository.AnyMapeoProducto(cdEmpresaOrigen, context.GetParameter("cdProdutoOrigen"), 1, cdEmpresaDestino))
            {
                throw new ValidationFailedException("STO810_Sec0_Error_MapeoNoValido");
            }

            var mapeo = uow.TraspasoEmpresasRepository.GetMapeoProducto(cdEmpresaOrigen, context.GetParameter("cdProdutoOrigen"), 1, cdEmpresaDestino);

            InicializarSelects(uow, form, mapeo);

            form.GetField("cantidadOrigen").Value = mapeo.CantidadOrigen.ToString(_identity.GetFormatProvider());
            form.GetField("cantidadDestino").Value = mapeo.CantidadDestino.ToString(_identity.GetFormatProvider());

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
                case "cdProdutoDestino":
                    return this.SearchProducto(form, context, false);
                default:
                    return new List<SelectOption>();
            }
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber("STO800 Modificación Mapeo de Productos");
            uow.BeginTransaction();

            try
            {
                if (!int.TryParse(context.GetParameter("cdEmpresaOrigen"), out int cdEmpresaOrigen)
                   || !int.TryParse(context.GetParameter("cdEmpresaDestino"), out int cdEmpresaDestino)
                   || string.IsNullOrEmpty(context.GetParameter("cdProdutoOrigen")))
                {
                    throw new ValidationFailedException("STO810_Sec0_Error_MapeoNoValido");
                }
                else if (!uow.TraspasoEmpresasRepository.AnyMapeoProducto(cdEmpresaOrigen, context.GetParameter("cdProdutoOrigen"), 1, cdEmpresaDestino))
                {
                    throw new ValidationFailedException("STO810_Sec0_Error_MapeoNoValido");
                }

                var mapeo = uow.TraspasoEmpresasRepository.GetMapeoProducto(cdEmpresaOrigen, context.GetParameter("cdProdutoOrigen"), 1, cdEmpresaDestino);

                mapeo.ProductoDestino = form.GetField("cdProdutoDestino").Value;
                mapeo.CantidadOrigen = decimal.Parse(form.GetField("cantidadOrigen").Value, _identity.GetFormatProvider());
                mapeo.CantidadDestino = decimal.Parse(form.GetField("cantidadDestino").Value, _identity.GetFormatProvider());
                mapeo.FaixaDestino = 1;
                mapeo.FechaModificacion = DateTime.Now;

                uow.TraspasoEmpresasRepository.UpdateMapeoProducto(mapeo);

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

        public virtual void InicializarSelects(IUnitOfWork uow, Form form, MapeoProducto mapeo)
        {
            var empresaOrigen = uow.EmpresaRepository.GetEmpresa(mapeo.EmpresaOrigen);
            var empresaDestino = uow.EmpresaRepository.GetEmpresa(mapeo.EmpresaDestino);
            var productoOrigen = uow.ProductoRepository.GetProducto(empresaOrigen.Id, mapeo.ProductoOrigen);
            var productoDestino = uow.ProductoRepository.GetProducto(empresaDestino.Id, mapeo.ProductoDestino);

            var selectEmpresaOrigen = form.GetField("cdEmpresaOrigen");
            selectEmpresaOrigen.Options = SearchEmpresa(form, new FormSelectSearchContext()
            {
                SearchValue = empresaOrigen.Id.ToString()
            });
            selectEmpresaOrigen.Value = empresaOrigen.Id.ToString();
            selectEmpresaOrigen.ReadOnly = true;

            var selectEmpresaDestino = form.GetField("cdEmpresaDestino");
            selectEmpresaDestino.Options = SearchEmpresa(form, new FormSelectSearchContext()
            {
                SearchValue = empresaDestino.Id.ToString()
            });
            selectEmpresaDestino.Value = empresaDestino.Id.ToString();
            selectEmpresaDestino.ReadOnly = true;

            var selectProductoOrigen = form.GetField("cdProdutoOrigen");
            selectProductoOrigen.Options = SearchProducto(form, new FormSelectSearchContext()
            {
                SearchValue = productoOrigen.Codigo
            }, true);
            selectProductoOrigen.Value = productoOrigen.Codigo;
            selectProductoOrigen.ReadOnly = true;

            var selectProductoDestino = form.GetField("cdProdutoDestino");
            selectProductoDestino.Options = SearchProducto(form, new FormSelectSearchContext()
            {
                SearchValue = productoDestino.Codigo
            }, false);
            selectProductoDestino.Value = productoDestino.Codigo;
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
