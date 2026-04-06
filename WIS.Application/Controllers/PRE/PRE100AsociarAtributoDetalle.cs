using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.Parametrizacion;
using WIS.Domain.Picking.Dtos;
using WIS.Domain.Picking;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.FormComponent.Execution.Configuration;
using WIS.FormComponent;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.Sorting;
using WIS.TrafficOfficer;
using WIS.Domain.General;
using WIS.Components.Common.Select;
using DocumentFormat.OpenXml.Vml.Office;
using DocumentFormat.OpenXml.Drawing.Charts;
using WIS.Domain.Produccion;
using WIS.Persistence.Database;
using WIS.Extension;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.AspNetCore.Mvc;

namespace WIS.Application.Controllers.PRE
{
    public class PRE100AsociarAtributoDetalle : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IFormValidationService _formValidationService;
        protected readonly ISecurityService _security;
        protected readonly IParameterService _paramService;
        protected readonly ITrafficOfficerService _concurrencyControl;

        protected List<string> GridKeysAtributosSinDefinir { get; }
        protected List<string> GridKeysAtributosAsociados { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE100AsociarAtributoDetalle(IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            IFormValidationService formValidationService,
            ISecurityService security,
            IParameterService paramService,
            ITrafficOfficerService concurrencyControl)
        {

            this.GridKeysAtributosSinDefinir = new List<string>
            {
                "ID_ATRIBUTO", "FL_CABEZAL"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("ID_ATRIBUTO", SortDirection.Ascending)
            };

            this.GridKeysAtributosAsociados = new List<string>
            {
                "NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA", "CD_PRODUTO", "CD_FAIXA", "NU_IDENTIFICADOR", "ID_ESPECIFICA_IDENTIFICADOR", "NU_DET_PED_SAI_ATRIB", "ID_ATRIBUTO", "FL_CABEZAL"
            };


            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._filterInterpreter = filterInterpreter;
            this._formValidationService = formValidationService;
            this._security = security;
            this._paramService = paramService;
            this._concurrencyControl = concurrencyControl;
        }

        public override PageContext PageLoad(PageContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var datos = GetDatos(context.Parameters);
            context.Parameters.Add(new ComponentParameter("readOnly", PermiteEdicion(uow, datos) ? "N" : "S"));

            uow.CreateTransactionNumber("PRE100AsociarAtributoDetalle - PageLoad");
            GuardarYLimpiarDatosTemporales(uow, datos);
            uow.SaveChanges();

            var empresa = uow.EmpresaRepository.GetEmpresa(datos.Empresa);
            var cliente = uow.AgenteRepository.GetAgente(datos.Empresa, datos.Cliente);

            context.AddParameter("empresaNombre", empresa.Nombre);
            context.AddParameter("agenteDescripcion", cliente.Descripcion);
            context.AddParameter("agenteCodigo", cliente.Codigo);
            context.AddParameter("agenteTipo", cliente.Tipo);
            return base.PageLoad(context);
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            var readOnly = (context.Parameters.FirstOrDefault(p => p.Id == "readOnly")?.Value ?? "N") == "S";

			if (!readOnly)
            {
                switch (grid.Id)
                {
                    case "PRE100AsociarAtributoDetalle_grid_1": return InitializeGridAtributosSinDefinir(grid, context);
                    case "PRE100AsociarAtributoDetalle_grid_2": return InitializeGridAtributosAsociados(grid, context);
                    case "PRE100AsociarAtributoDetalle_gridDetalle_1": return InitializeGridDetalleAtributosSinDefinir(grid, context);
                    case "PRE100AsociarAtributoDetalle_gridDetalle_2": return InitializeGridDetalleAtributosAsociados(grid, context);
                }
            }
            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            switch (grid.Id)
            {
                case "PRE100AsociarAtributoDetalle_grid_1": return FetchRowsAtributosSinDefinir(uow, grid, context);
                case "PRE100AsociarAtributoDetalle_grid_2": return FetchRowsAtributosAsociados(uow, grid, context);
                case "PRE100AsociarAtributoDetalle_gridDetalle_1": return FetchRowsDetalleAtributosSinDefinir(uow, grid, context);
                case "PRE100AsociarAtributoDetalle_gridDetalle_2": return FetchRowsDetalleAtributosAsociados(uow, grid, context);
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            switch (grid.Id)
            {
                case "PRE100AsociarAtributoDetalle_grid_1": return GridExportExcelAtributosSinDefinir(uow, grid, context);
                case "PRE100AsociarAtributoDetalle_grid_2": return GridExportExcelAtributosAsociados(uow, grid, context);
                case "PRE100AsociarAtributoDetalle_gridDetalle_1": return GridExportExcelDetalleAtributosSinDefinir(uow, grid, context);
                case "PRE100AsociarAtributoDetalle_gridDetalle_2": return GridExportExcelDetalleAtributosAsociados(uow, grid, context);
            }
            return null;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            switch (grid.Id)
            {
                case "PRE100AsociarAtributoDetalle_grid_1": return FetchStatsAtributosSinDefinir(uow, grid, context);
                case "PRE100AsociarAtributoDetalle_grid_2": return FetchStatsAtributosAsociados(uow, grid, context);
                case "PRE100AsociarAtributoDetalle_gridDetalle_1": return FetchStatsDetalleAtributosSinDefinir(uow, grid, context);
                case "PRE100AsociarAtributoDetalle_gridDetalle_2": return FetchStatsDetalleAtributosAsociados(uow, grid, context);
            }
            return new GridStats();
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            switch (context.ButtonId)
            {
                case "btnAgregarAtributos": return MenuItemActionGuardarKeysAtributos(uow, context);
                case "btnQuitarAtributos": return MenuItemActionQuitarAtributos(uow, context);
                case "btnAgregarAtributosDetalle": return MenuItemActionGuardarKeysAtributos(uow, context, gridDetalle: true);
                case "btnQuitarAtributosDetalle": return MenuItemActionQuitarAtributos(uow, context, gridDetalle: true);
            }

            return context;
        }


        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var datos = GetDatos(context.Parameters);

            form.GetField("producto").ReadOnly = false;
            form.GetField("identificador").ReadOnly = false;

            if (datos.Update)
            {
                form.GetField("producto").Value = datos.Producto;
                form.GetField("identificador").Value = datos.Identificador;
                form.GetField("cantidad").Value = datos.Cantidad.ToString("#.###");

                var searchContext = new FormSelectSearchContext()
                {
                    SearchValue = datos.Producto
                };
                searchContext.AddParameter("empresa", datos.Empresa.ToString());

                form.GetField("producto").Options = SearchProducto(form, searchContext);
                form.GetField("producto").Value = datos.Producto;
                form.GetField("producto").ReadOnly = true;
                form.GetField("identificador").ReadOnly = true;
            }
            else
            {
                form.GetField("producto").Value = string.Empty;
                form.GetField("identificador").Value = string.Empty;
                form.GetField("cantidad").Value = string.Empty;
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            if (context.ButtonId == "btnDeshabilitarCampos")
                DeshabilitarCampos(form, deshabilitar: true);
            else if (context.ButtonId == "btnHabilitarCampos")
                DeshabilitarCampos(form, deshabilitar: false);
            else
                ConfirmarOperacion(form, context);

            return form;
        }

        public override Form FormButtonAction(Form form, FormButtonActionContext query)
        {
            if (query.ButtonId == "btnCerrar")
                this._concurrencyControl.ClearToken();

            return base.FormButtonAction(form, query);
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var datos = GetDatos(context.Parameters);

            return this._formValidationService.Validate(new PedidoAsociarAtributosFormValidationModule(uow, this._identity, datos), form, context);
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "producto": return SearchProducto(form, context);
            }

            return new List<SelectOption>();
        }

        #region Auxs

        public virtual Grid InitializeGridAtributosSinDefinir(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems = new List<IGridItem> {
                new GridButton("btnAgregarAtributos", "PRE100AsociarAtributoDetalle_Sec0_btn_AgregarAtributos")
            };

            return this.GridFetchRows(grid, context.FetchContext);
        }
        public virtual Grid InitializeGridAtributosAsociados(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems = new List<IGridItem> {
                new GridButton("btnQuitarAtributos", "PRE100AsociarAtributoDetalle_Sec0_btn_QuitarAtributos")
            };

            return this.GridFetchRows(grid, context.FetchContext);
        }
        public virtual Grid InitializeGridDetalleAtributosSinDefinir(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems = new List<IGridItem> {
                new GridButton("btnQuitarAtributosDetalle", "PRE100AsociarAtributoDetalle_Sec0_btn_QuitarAtributosDetalle")
            };

            return this.GridFetchRows(grid, context.FetchContext);
        }
        public virtual Grid InitializeGridDetalleAtributosAsociados(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems = new List<IGridItem> {
                new GridButton("btnAgregarAtributosDetalle", "PRE100AsociarAtributoDetalle_Sec0_btn_AgregarAtributosDetalle")
            };

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public virtual Grid FetchRowsAtributosSinDefinir(IUnitOfWork uow, Grid grid, GridFetchContext context)
        {
            var datos = GetDatos(context.Parameters);

            var dbQuery = new DetallePedidoAtributosNoAsociadosQuery(datos);
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeysAtributosSinDefinir);

            if (datos.PageReanOnly)
            {
                foreach (var row in grid.Rows)
                {
                    row.DisabledSelected = true;
                }
            }

            return grid;
        }
        public virtual Grid FetchRowsAtributosAsociados(IUnitOfWork uow, Grid grid, GridFetchContext context)
        {
            var datos = GetDatos(context.Parameters);

            var dbQuery = new DetallePedidoAtributosAsociadosQuery(datos);
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeysAtributosAsociados);

            if (datos.PageReanOnly)
            {
                foreach (var row in grid.Rows)
                {
                    row.DisabledSelected = true;
                }
            }

            context.Parameters.RemoveAll(w => w.Id == "coutRowGrid");
            context.Parameters.Add(new ComponentParameter() { Id = "coutRowGrid", Value = grid.Rows.Count().ToString() });

            return grid;
        }
        public virtual Grid FetchRowsDetalleAtributosSinDefinir(IUnitOfWork uow, Grid grid, GridFetchContext context)
        {
            var datos = GetDatos(context.Parameters, gridDetalle: true);

            var dbQuery = new DetallePedidoAtributosNoAsociadosQuery(datos);
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeysAtributosSinDefinir);

            if (datos.PageReanOnly)
            {
                foreach (var row in grid.Rows)
                {
                    row.DisabledSelected = true;
                }
            }

            return grid;
        }
        public virtual Grid FetchRowsDetalleAtributosAsociados(IUnitOfWork uow, Grid grid, GridFetchContext context)
        {
            var datos = GetDatos(context.Parameters, gridDetalle: true);

            var dbQuery = new DetallePedidoAtributosAsociadosQuery(datos);
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeysAtributosAsociados);

            if (datos.PageReanOnly)
            {
                foreach (var row in grid.Rows)
                {
                    row.DisabledSelected = true;
                }
            }

            context.Parameters.RemoveAll(w => w.Id == "coutRowGridDetalle");
            context.Parameters.Add(new ComponentParameter() { Id = "coutRowGridDetalle", Value = grid.Rows.Count().ToString() });

            return grid;
        }

        public virtual byte[] GridExportExcelAtributosSinDefinir(IUnitOfWork uow, Grid grid, GridExportExcelContext context)
        {
            var datos = GetDatos(context.Parameters);
            var dbQuery = new DetallePedidoAtributosNoAsociadosQuery(datos);
            uow.HandleQuery(dbQuery);
            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }
        public virtual byte[] GridExportExcelAtributosAsociados(IUnitOfWork uow, Grid grid, GridExportExcelContext context)
        {
            var datos = GetDatos(context.Parameters);
            var dbQuery = new DetallePedidoAtributosAsociadosQuery(datos);
            uow.HandleQuery(dbQuery);
            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }
        public virtual byte[] GridExportExcelDetalleAtributosSinDefinir(IUnitOfWork uow, Grid grid, GridExportExcelContext context)
        {
            var datos = GetDatos(context.Parameters, gridDetalle: true);
            var dbQuery = new DetallePedidoAtributosNoAsociadosQuery(datos);
            uow.HandleQuery(dbQuery);
            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }
        public virtual byte[] GridExportExcelDetalleAtributosAsociados(IUnitOfWork uow, Grid grid, GridExportExcelContext context)
        {
            var datos = GetDatos(context.Parameters, gridDetalle: true);
            var dbQuery = new DetallePedidoAtributosAsociadosQuery(datos);
            uow.HandleQuery(dbQuery);
            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public virtual GridStats FetchStatsAtributosSinDefinir(IUnitOfWork uow, Grid grid, GridFetchStatsContext context)
        {
            var datos = GetDatos(context.Parameters);

            var dbQuery = new DetallePedidoAtributosNoAsociadosQuery(datos);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public virtual GridStats FetchStatsAtributosAsociados(IUnitOfWork uow, Grid grid, GridFetchStatsContext context)
        {
            var datos = GetDatos(context.Parameters);

            var dbQuery = new DetallePedidoAtributosAsociadosQuery(datos);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public virtual GridStats FetchStatsDetalleAtributosSinDefinir(IUnitOfWork uow, Grid grid, GridFetchStatsContext context)
        {
            var datos = GetDatos(context.Parameters, gridDetalle: true);

            var dbQuery = new DetallePedidoAtributosNoAsociadosQuery(datos);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public virtual GridStats FetchStatsDetalleAtributosAsociados(IUnitOfWork uow, Grid grid, GridFetchStatsContext context)
        {
            var datos = GetDatos(context.Parameters, gridDetalle: true);

            var dbQuery = new DetallePedidoAtributosAsociadosQuery(datos);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public virtual GridMenuItemActionContext MenuItemActionGuardarKeysAtributos(IUnitOfWork uow, GridMenuItemActionContext context, bool gridDetalle = false)
        {
            var keys = new List<KeyAtributoTipo>();
            var datos = GetDatos(context.Parameters, gridDetalle);

            if (datos.Update)
                CheckIfLocked(datos.GetLockId(lpn: false));
            else if (string.IsNullOrEmpty(datos.Producto) || datos.Cantidad == 0)
                throw new MissingParameterException("PRE100_msg_Error_CamposRequeridos");

            var dbQuery = new DetallePedidoAtributosNoAsociadosQuery(datos);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
            {
                keys = dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys)
                    .Select(l => new KeyAtributoTipo()
                    {
                        IdAtributo = int.Parse(l[0]),
                        Cabezal = l[1]
                    })
                    .ToList();
            }
            else
            {
                keys = dbQuery.GetSelectedKeys(context.Selection.Keys)
                    .Select(l => new KeyAtributoTipo()
                    {
                        IdAtributo = int.Parse(l[0]),
                        Cabezal = l[1]
                    })
                    .ToList();
            }

            if (keys.Count > 0)
                context.Parameters.Add(new ComponentParameter() { Id = "listAtributos", Value = JsonConvert.SerializeObject(keys) });
            else
                context.Parameters.Add(new ComponentParameter() { Id = "listAtributos", Value = string.Empty });

            return context;
        }
        public virtual GridMenuItemActionContext MenuItemActionQuitarAtributos(IUnitOfWork uow, GridMenuItemActionContext context, bool gridDetalle = false)
        {
            var keys = new List<KeyAtributoAsignado>();

            var datos = GetDatos(context.Parameters, gridDetalle);

            if (datos.Update)
                CheckIfLocked(datos.GetLockId(lpn: false));

            var dbQuery = new DetallePedidoAtributosAsociadosQuery(datos);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
                keys = dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys, _identity.GetFormatProvider());
            else
                keys = dbQuery.GetSelectedKeys(context.Selection.Keys, _identity.GetFormatProvider());

            ProcesarQuitar(uow, context, datos, keys);

            return context;
        }

        public virtual void ProcesarQuitar(IUnitOfWork uow, GridMenuItemActionContext context, DetallePedidoLpnEspecifico datos, List<KeyAtributoAsignado> keysAtributos)
        {
            uow.CreateTransactionNumber("PRE100AsociarAtributoDetalle: MenuItemActionQuitarAtributos");
            uow.BeginTransaction();

            try
            {
                context.Parameters.Add(new ComponentParameter { Id = "update", Value = datos.Update ? "S" : "N" });

                var terminarEdicion = false;
                foreach (var key in keysAtributos)
                {
                    if (key.IdConfiguracion != -1)
                    {
                        var atributoDefinicion = uow.ManejoLpnRepository.GetDetallePedidoAtributoDefinicion(key.IdConfiguracion, key.IdAtributo, key.IdCabezal);

                        atributoDefinicion.FechaModificacion = DateTime.Now;
                        atributoDefinicion.Transaccion = uow.GetTransactionNumber();
                        atributoDefinicion.TransaccionDelete = uow.GetTransactionNumber();

                        uow.ManejoLpnRepository.UpdateDetallePedidoAtributoDefinicion(atributoDefinicion);
                        uow.SaveChanges();

                        uow.ManejoLpnRepository.DeleteDetallePedidoAtributoDefinicion(atributoDefinicion);
                        uow.SaveChanges();

                        if (!uow.ManejoLpnRepository.AnyDetallePedidoAtributoDefinicion(key.IdConfiguracion))
                        {
                            var detallePedidoAtributo = uow.ManejoLpnRepository.GetDetallePedidoAtributo(key.Pedido, key.Cliente, key.Empresa, key.Producto, key.Faixa, key.Identificador, key.IdEspecificaIdentificador, key.IdConfiguracion);

                            detallePedidoAtributo.FechaModificacion = DateTime.Now;
                            detallePedidoAtributo.Transaccion = uow.GetTransactionNumber();
                            detallePedidoAtributo.TransaccionDelete = uow.GetTransactionNumber();

                            uow.ManejoLpnRepository.UpdateDetallePedidoAtributo(detallePedidoAtributo);
                            uow.SaveChanges();

                            uow.ManejoLpnRepository.DeleteDetallePedidoAtributo(detallePedidoAtributo);

                            var detallePedido = uow.PedidoRepository.GetDetallePedido(key.Pedido, key.Empresa, key.Cliente, key.Producto, key.Identificador, key.Faixa, key.IdEspecificaIdentificador);

                            detallePedido.Cantidad = (detallePedido.Cantidad ?? 0) - detallePedidoAtributo.CantidadPedida;
                            detallePedido.FechaModificacion = DateTime.Now;
                            detallePedido.Transaccion = uow.GetTransactionNumber();

                            uow.PedidoRepository.UpdateDetallePedido(detallePedido);

                            terminarEdicion = true;
                        }
                    }
                    else
                    {
                        var atributoTemporal = uow.ManejoLpnRepository.GetDetallePedidoAtributoTemporal(key.Pedido, key.Cliente, key.Empresa, key.Producto, key.Faixa, key.Identificador, key.IdEspecificaIdentificador, key.IdAtributo, _identity.UserId, key.IdCabezal);

                        atributoTemporal.FechaModificacion = DateTime.Now;
                        atributoTemporal.Transaccion = uow.GetTransactionNumber();
                        atributoTemporal.TransaccionDelete = uow.GetTransactionNumber();

                        uow.ManejoLpnRepository.UpdateDetallePedidoAtributoTemporal(atributoTemporal);
                        uow.SaveChanges();

                        uow.ManejoLpnRepository.DeleteDetallePedidoAtributoTemporal(atributoTemporal);
                    }
                }

                if (terminarEdicion)
                {
                    context.Parameters.RemoveAll(w => w.Id == "update");
                    context.Parameters.RemoveAll(w => w.Id == "terminarOperacion");
                    context.Parameters.Add(new ComponentParameter { Id = "update", Value = "N" });
                    context.Parameters.Add(new ComponentParameter { Id = "terminarOperacion", Value = "S" });
                    context.AddInfoNotification("PRE100_msg_Info_SinAtributosTerminaEdicion");
                }

                uow.SaveChanges();
                uow.Commit();
            }
            catch (Exception)
            {
                uow.Rollback();
                throw;
            }
        }

        public virtual void GuardarYLimpiarDatosTemporales(IUnitOfWork uow, DetallePedidoLpnEspecifico datos, long? idConfiguracion = null)
        {
            var atributosTemps = uow.ManejoLpnRepository.GetDetallesPedidoAtributoTemporal(datos.Pedido, datos.Cliente, datos.Empresa, datos.Producto, datos.Faixa, datos.Identificador, datos.IdEspecificaIdentificador, _identity.UserId);

            foreach (var at in atributosTemps)
            {
                if (idConfiguracion != null)
                {
                    uow.ManejoLpnRepository.AddDetallePedidoAtributoDefinicion(new DetallePedidoAtributoDefinicion()
                    {
                        IdConfiguracion = idConfiguracion.Value,
                        IdAtributo = at.IdAtributo,
                        IdCabezal = at.IdCabezal,
                        Valor = at.Valor,
                        FechaAlta = DateTime.Now,
                        Transaccion = uow.GetTransactionNumber()
                    });
                }

                at.FechaModificacion = DateTime.Now;
                at.Transaccion = uow.GetTransactionNumber();
                at.TransaccionDelete = uow.GetTransactionNumber();

                uow.ManejoLpnRepository.UpdateDetallePedidoAtributoTemporal(at);
                uow.SaveChanges();

                uow.ManejoLpnRepository.DeleteDetallePedidoAtributoTemporal(at);
            }
        }

        public virtual DetallePedidoLpnEspecifico GetDatos(List<ComponentParameter> parametros, bool gridDetalle = false)
        {
            var dto = new DetallePedidoLpnEspecifico()
            {
                Pedido = parametros.FirstOrDefault(p => p.Id == "pedido").Value,
                Cliente = parametros.FirstOrDefault(p => p.Id == "cliente").Value,
                Empresa = int.Parse(parametros.FirstOrDefault(p => p.Id == "empresa").Value),
                Update = (parametros.FirstOrDefault(p => p.Id == "update")?.Value ?? "N") == "S",
                UserId = _identity.UserId,
                Detalle = gridDetalle,
                PageReanOnly = (parametros.FirstOrDefault(p => p.Id == "readOnly")?.Value ?? "N") == "S",

                Producto = parametros.FirstOrDefault(p => p.Id == "producto")?.Value,
                Identificador = parametros.FirstOrDefault(p => p.Id == "identificador")?.Value,
            };

            if (dto.Update)
            {
                dto.Faixa = decimal.Parse(parametros.FirstOrDefault(p => p.Id == "faixa").Value, _identity.GetFormatProvider());
                dto.IdEspecificaIdentificador = parametros.FirstOrDefault(p => p.Id == "idEspecificaIdentificador").Value;
                dto.IdConfiguracion = long.Parse(parametros.FirstOrDefault(p => p.Id == "idConfiguracion").Value);
            }
            else
            {
                dto.Faixa = 1;
                dto.IdEspecificaIdentificador = (!string.IsNullOrEmpty(dto.Identificador) && dto.Identificador != ManejoIdentificadorDb.IdentificadorAuto) ? "S" : "N";
            }

            var cantidad = parametros.FirstOrDefault(p => p.Id == "cantidad")?.Value;
            if (!string.IsNullOrEmpty(cantidad))
                dto.Cantidad = decimal.Parse(cantidad, _identity.GetFormatProvider());

            return dto;
        }

        public virtual void CheckIfLocked(string key)
        {
            if (this._concurrencyControl.IsLocked("T_DET_PEDIDO_SAIDA_ATRIB", key))
                throw new EntityLockedException("General_Sec0_Error_LockedEntity");
        }

        public virtual bool PermiteEdicion(IUnitOfWork uow, DetallePedidoLpnEspecifico datos)
        {
            var detalleTrabajado = false;
            var pedido = uow.PedidoRepository.GetPedido(datos.Empresa, datos.Cliente, datos.Pedido);
            var atributosHabilitados = (_paramService.GetValueByEmpresa(ParamManager.IE_503_HAB_ATRIBUTOS, datos.Empresa) ?? "N") == "S";

            if (datos.Update)
            {
                detalleTrabajado = uow.ManejoLpnRepository.AnyDetallePedidoAtributoTrabajado(datos);

                if (this._concurrencyControl.IsLocked("T_DET_PEDIDO_SAIDA_ATRIB", datos.GetLockId(lpn: false)))
                    detalleTrabajado = true;
                else
                    this._concurrencyControl.AddLock("T_DET_PEDIDO_SAIDA_ATRIB", datos.GetLockId(lpn: false));
            }

            return pedido.IsManual && atributosHabilitados && !detalleTrabajado;
        }

        public virtual bool PermiteConfirmar(IUnitOfWork uow, DetallePedidoLpnEspecifico datos)
        {
            return (uow.ManejoLpnRepository.AnyAtributoAsociadoDetPed(datos)
                && !uow.ManejoLpnRepository.AnyDetallePedidoAtributoTrabajado(datos));
        }

        public virtual List<SelectOption> SearchProducto(Form form, FormSelectSearchContext context)
        {
            var empresa = int.Parse(context.GetParameter("empresa"));

            var options = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(empresa, context.SearchValue);

            foreach (var producto in productos)
            {
                options.Add(new SelectOption(producto.Codigo, producto.Codigo + " - " + producto.Descripcion));
            }

            return options;
        }

        public virtual void DeshabilitarCampos(Form form, bool deshabilitar)
        {
            form.GetField("producto").ReadOnly = deshabilitar;
            form.GetField("identificador").ReadOnly = deshabilitar;
            form.GetField("identificador").Disabled = deshabilitar;
        }

        public virtual void ConfirmarOperacion(Form form, FormSubmitContext context)
        {
            var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                uow.CreateTransactionNumber("PRE100AsociarAtributoDetalle - FormSubmit");
                var datos = GetDatos(context.Parameters);

                if (!datos.Update)
                {
                    datos.Producto = form.GetField("producto").Value;
                    datos.Faixa = 1;
                    datos.Identificador = form.GetField("identificador").Value;
                    datos.IdEspecificaIdentificador = (!string.IsNullOrEmpty(datos.Identificador) && datos.Identificador != ManejoIdentificadorDb.IdentificadorAuto) ? "S" : "N";
                }

                if (!PermiteConfirmar(uow, datos))
                    throw new ValidationFailedException("PRE100_msg_Error_AtributosRequeridos");

                var cantidad = decimal.Parse(form.GetField("cantidad").Value, _identity.GetFormatProvider());

                long idConfiguracion;

                var detallePedido = uow.PedidoRepository.GetDetallePedido(datos.Pedido, datos.Empresa, datos.Cliente, datos.Producto, datos.Identificador, datos.Faixa, datos.IdEspecificaIdentificador);

                if (datos.Update && datos.IdConfiguracion != null)
                {
                    CheckIfLocked(datos.GetLockId(lpn: false));

                    var detallePedidoAtributo = uow.ManejoLpnRepository.GetDetallePedidoAtributo(datos.Pedido, datos.Cliente, datos.Empresa, datos.Producto, datos.Faixa, datos.Identificador, datos.IdEspecificaIdentificador, datos.IdConfiguracion.Value);

                    var diferencia = cantidad - detallePedidoAtributo.CantidadPedida;

                    detallePedido.Cantidad = (detallePedido.Cantidad ?? 0) + diferencia;
                    detallePedido.FechaModificacion = DateTime.Now;
                    detallePedido.Transaccion = uow.GetTransactionNumber();

                    uow.PedidoRepository.UpdateDetallePedido(detallePedido);

                    detallePedidoAtributo.CantidadPedida = cantidad;
                    detallePedidoAtributo.FechaModificacion = DateTime.Now;
                    detallePedidoAtributo.Transaccion = uow.GetTransactionNumber();

                    uow.ManejoLpnRepository.UpdateDetallePedidoAtributo(detallePedidoAtributo);

                    idConfiguracion = datos.IdConfiguracion.Value;
                }
                else
                {
                    if (detallePedido == null)
                    {
                        var nuevoDetalle = new DetallePedido()
                        {
                            Id = datos.Pedido,
                            Cliente = datos.Cliente,
                            Empresa = datos.Empresa,
                            Producto = datos.Producto,
                            Faixa = datos.Faixa,
                            Identificador = datos.Identificador.Trim(),
                            EspecificaIdentificador = (datos.IdEspecificaIdentificador == "S"),
                            Agrupacion = Agrupacion.Pedido,
                            Cantidad = cantidad,
                            CantidadLiberada = 0,
                            CantidadAnulada = 0,
                            CantidadOriginal = cantidad,
                            FechaAlta = DateTime.Now,
                            Transaccion = uow.GetTransactionNumber()
                        };
                        uow.PedidoRepository.AddDetallePedido(nuevoDetalle);
                    }
                    else
                    {
                        detallePedido.Cantidad = (detallePedido.Cantidad ?? 0) + cantidad;
                        detallePedido.FechaModificacion = DateTime.Now;
                        detallePedido.Transaccion = uow.GetTransactionNumber();

                        uow.PedidoRepository.UpdateDetallePedido(detallePedido);
                    }

                    idConfiguracion = uow.ManejoLpnRepository.GetNextIdConfiguracion();

                    var detallePedidoAtributo = new DetallePedidoAtributo()
                    {
                        Pedido = datos.Pedido,
                        Cliente = datos.Cliente,
                        Empresa = datos.Empresa,
                        Producto = datos.Producto,
                        Faixa = datos.Faixa,
                        Identificador = datos.Identificador,
                        IdEspecificaIdentificador = datos.IdEspecificaIdentificador,
                        IdConfiguracion = idConfiguracion,
                        CantidadPedida = cantidad,
                        CantidadAnulada = 0,
                        CantidadLiberada = 0,
                        FechaAlta = DateTime.Now,
                        Transaccion = uow.GetTransactionNumber()
                    };

                    uow.ManejoLpnRepository.AddDetallePedidoAtributo(detallePedidoAtributo);
                }

                GuardarYLimpiarDatosTemporales(uow, datos, idConfiguracion);

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                uow.Rollback();
            }
            catch (Exception ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message);
            }
        }

        #endregion
    }
}
