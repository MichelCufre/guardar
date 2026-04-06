using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Application.Validation;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Filtering;
using WIS.FormComponent.Execution.Configuration;
using WIS.FormComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Excel;
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.Sorting;
using WIS.Domain.Picking;
using WIS.Domain.Picking.Dtos;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.Recepcion;
using WIS.Exceptions;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Items;
using DocumentFormat.OpenXml.InkML;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.DataModel.Queries.Parametrizacion;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Expedicion;
using WIS.Extension;
using WIS.Domain.StockEntities;
using WIS.Domain.Parametrizacion;
using Newtonsoft.Json;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;
using WIS.TrafficOfficer;
using System.Security.Cryptography;
using WIS.Domain.General;

namespace WIS.Application.Controllers.PRE
{
    public class PRE100AsociarAtributoLpn : AppController
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
        protected List<SortCommand> DefaultSortAtributosSinDefinir { get; }
        protected List<SortCommand> DefaultSortAtributosAsociados { get; }

        public PRE100AsociarAtributoLpn(IUnitOfWorkFactory uowFactory,
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
                "TP_LPN_TIPO", "ID_ATRIBUTO", "FL_CABEZAL"
            };

            this.DefaultSortAtributosSinDefinir = new List<SortCommand>
            {
                new SortCommand("NU_ORDEN", SortDirection.Ascending)
            };

            this.GridKeysAtributosAsociados = new List<string>
            {
                "NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA", "CD_PRODUTO", "CD_FAIXA", "NU_IDENTIFICADOR", "ID_ESPECIFICA_IDENTIFICADOR", "TP_LPN_TIPO", "ID_LPN_EXTERNO", "NU_DET_PED_SAI_ATRIB", "ID_ATRIBUTO","FL_CABEZAL"
            };

            this.DefaultSortAtributosAsociados = new List<SortCommand>
            {
                new SortCommand("ID_ATRIBUTO", SortDirection.Ascending)
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
            _concurrencyControl = concurrencyControl;
        }

        public override PageContext PageLoad(PageContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var datos = GetDatos(context.Parameters);
            context.Parameters.Add(new ComponentParameter("readOnly", PermiteEdicion(uow, datos) ? "N" : "S"));

            uow.CreateTransactionNumber("PRE100AsociarAtributoLpn - PageLoad");
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
                    case "PRE100AsociarAtributoLpn_grid_1": return InitializeGridAtributosSinDefinir(grid, context);
                    case "PRE100AsociarAtributoLpn_grid_2": return InitializeGridAtributosAsociados(grid, context);
                    case "PRE100AsociarAtributoLpn_gridDetalle_1": return InitializeGridDetalleAtributosSinDefinir(grid, context);
                    case "PRE100AsociarAtributoLpn_gridDetalle_2": return InitializeGridDetalleAtributosAsociados(grid, context);
                }
            }

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            switch (grid.Id)
            {
                case "PRE100AsociarAtributoLpn_grid_1": return FetchRowsAtributosSinDefinir(uow, grid, context);
                case "PRE100AsociarAtributoLpn_grid_2": return FetchRowsAtributosAsociados(uow, grid, context);
                case "PRE100AsociarAtributoLpn_gridDetalle_1": return FetchRowsDetalleAtributosSinDefinir(uow, grid, context);
                case "PRE100AsociarAtributoLpn_gridDetalle_2": return FetchRowsDetalleAtributosAsociados(uow, grid, context);
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            switch (grid.Id)
            {
                case "PRE100AsociarAtributoLpn_grid_1": return GridExportExcelAtributosSinDefinir(uow, grid, context);
                case "PRE100AsociarAtributoLpn_grid_2": return GridExportExcelAtributosAsociados(uow, grid, context);
                case "PRE100AsociarAtributoLpn_gridDetalle_1": return GridExportExcelDetalleAtributosSinDefinir(uow, grid, context);
                case "PRE100AsociarAtributoLpn_gridDetalle_2": return GridExportExcelDetalleAtributosAsociados(uow, grid, context);
            }
            return null;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            switch (grid.Id)
            {
                case "PRE100AsociarAtributoLpn_grid_1": return FetchStatsAtributosSinDefinir(uow, grid, context);
                case "PRE100AsociarAtributoLpn_grid_2": return FetchStatsAtributosAsociados(uow, grid, context);
                case "PRE100AsociarAtributoLpn_gridDetalle_1": return FetchStatsDetalleAtributosSinDefinir(uow, grid, context);
                case "PRE100AsociarAtributoLpn_gridDetalle_2": return FetchStatsDetalleAtributosAsociados(uow, grid, context);
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

            form.GetField("tipoLpn").Value = datos.TipoLpn;
            form.GetField("idExternoLpn").Value = datos.IdExternoLpn;
            form.GetField("producto").Value = datos.Producto;
            form.GetField("identificador").Value = datos.Identificador;

            if (datos.Update)
                form.GetField("cantidad").Value = datos.Cantidad.ToString("#.###");
            else
                form.GetField("cantidad").Value = string.Empty;

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                uow.CreateTransactionNumber("PRE100AsociarAtributoLpn - FormSubmit");
                var datos = GetDatos(context.Parameters);

                if (!PermiteConfirmar(uow, datos))
                    throw new ValidationFailedException("PRE100_msg_Error_AtributosRequeridos");

                var cantidad = decimal.Parse(form.GetField("cantidad").Value, _identity.GetFormatProvider());

                long idConfiguracion;
                if (datos.Update && datos.IdConfiguracion != null)
                {
                    CheckIfLocked(datos.GetLockId(lpn: true));

                    var detallePedidoLpnAtributo = uow.ManejoLpnRepository.GetDetallePedidoLpnAtributo(datos.Pedido, datos.Cliente, datos.Empresa, datos.Producto, datos.Faixa, datos.Identificador, datos.IdEspecificaIdentificador, datos.TipoLpn, datos.IdExternoLpn, datos.IdConfiguracion.Value);

                    detallePedidoLpnAtributo.CantidadPedida = cantidad;
                    detallePedidoLpnAtributo.FechaModificacion = DateTime.Now;
                    detallePedidoLpnAtributo.Transaccion = uow.GetTransactionNumber();

                    uow.ManejoLpnRepository.UpdateDetallePedidoLpnAtributo(detallePedidoLpnAtributo);
                    idConfiguracion = datos.IdConfiguracion.Value;
                }
                else
                {
                    idConfiguracion = uow.ManejoLpnRepository.GetNextIdConfiguracion();

                    var detallePedidoLpnAtributo = new DetallePedidoLpnAtributo()
                    {
                        Pedido = datos.Pedido,
                        Cliente = datos.Cliente,
                        Empresa = datos.Empresa,
                        Producto = datos.Producto,
                        Faixa = datos.Faixa,
                        Identificador = datos.Identificador,
                        IdEspecificaIdentificador = datos.IdEspecificaIdentificador,
                        Tipo = datos.TipoLpn,
                        IdLpnExterno = datos.IdExternoLpn,
                        IdConfiguracion = idConfiguracion,
                        CantidadPedida = cantidad,
                        CantidadLiberada = 0,
                        CantidadAnulada = 0,
                        FechaAlta = DateTime.Now,
                        Transaccion = uow.GetTransactionNumber()
                    };

                    uow.ManejoLpnRepository.AddDetallePedidoLpnAtributo(detallePedidoLpnAtributo);
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

            return this._formValidationService.Validate(new PedidoAsociarAtributosLpnFormValidationModule(uow, this._identity, datos), form, context);
        }

        #region Auxs

        public virtual Grid InitializeGridAtributosSinDefinir(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems = new List<IGridItem> {
                new GridButton("btnAgregarAtributos", "PRE100AsociarAtributoLpn_Sec0_btn_AgregarAtributos")
            };

            return this.GridFetchRows(grid, context.FetchContext);
        }
        public virtual Grid InitializeGridAtributosAsociados(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems = new List<IGridItem> {
                new GridButton("btnQuitarAtributos", "PRE100AsociarAtributoLpn_Sec0_btn_QuitarAtributos")
            };

            return this.GridFetchRows(grid, context.FetchContext);
        }
        public virtual Grid InitializeGridDetalleAtributosSinDefinir(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems = new List<IGridItem> {
                new GridButton("btnQuitarAtributosDetalle", "PRE100AsociarAtributoLpn_Sec0_btn_QuitarAtributosDetalle")
            };

            return this.GridFetchRows(grid, context.FetchContext);
        }
        public virtual Grid InitializeGridDetalleAtributosAsociados(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems = new List<IGridItem> {
                new GridButton("btnAgregarAtributosDetalle", "PRE100AsociarAtributoLpn_Sec0_btn_AgregarAtributosDetalle")
            };

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public virtual Grid FetchRowsAtributosSinDefinir(IUnitOfWork uow, Grid grid, GridFetchContext context)
        {
            var datos = GetDatos(context.Parameters);

            var dbQuery = new DetallePedidoLpnAtributosNoAsociadosQuery(datos);
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSortAtributosSinDefinir, this.GridKeysAtributosSinDefinir);

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

            var dbQuery = new DetallePedidoLpnAtributosAsociadosQuery(datos);
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSortAtributosAsociados, this.GridKeysAtributosAsociados);

            if (datos.PageReanOnly)
            {
                foreach (var row in grid.Rows)
                {
                    row.DisabledSelected = true;
                }
            }

            return grid;
        }
        public virtual Grid FetchRowsDetalleAtributosSinDefinir(IUnitOfWork uow, Grid grid, GridFetchContext context)
        {
            var datos = GetDatos(context.Parameters, gridDetalle: true);

            var dbQuery = new DetallePedidoLpnAtributosNoAsociadosQuery(datos);
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSortAtributosSinDefinir, this.GridKeysAtributosSinDefinir);

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

            var dbQuery = new DetallePedidoLpnAtributosAsociadosQuery(datos);
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSortAtributosAsociados, this.GridKeysAtributosAsociados);

            if (datos.PageReanOnly)
            {
                foreach (var row in grid.Rows)
                {
                    row.DisabledSelected = true;
                }
            }

            return grid;
        }

        public virtual byte[] GridExportExcelAtributosSinDefinir(IUnitOfWork uow, Grid grid, GridExportExcelContext context)
        {
            var datos = GetDatos(context.Parameters);
            var dbQuery = new DetallePedidoLpnAtributosNoAsociadosQuery(datos);
            uow.HandleQuery(dbQuery);
            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSortAtributosSinDefinir);
        }
        public virtual byte[] GridExportExcelAtributosAsociados(IUnitOfWork uow, Grid grid, GridExportExcelContext context)
        {
            var datos = GetDatos(context.Parameters);
            var dbQuery = new DetallePedidoLpnAtributosAsociadosQuery(datos);
            uow.HandleQuery(dbQuery);
            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSortAtributosAsociados);
        }
        public virtual byte[] GridExportExcelDetalleAtributosSinDefinir(IUnitOfWork uow, Grid grid, GridExportExcelContext context)
        {
            var datos = GetDatos(context.Parameters, gridDetalle: true);
            var dbQuery = new DetallePedidoLpnAtributosNoAsociadosQuery(datos);
            uow.HandleQuery(dbQuery);
            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSortAtributosSinDefinir);
        }
        public virtual byte[] GridExportExcelDetalleAtributosAsociados(IUnitOfWork uow, Grid grid, GridExportExcelContext context)
        {
            var datos = GetDatos(context.Parameters, gridDetalle: true);
            var dbQuery = new DetallePedidoLpnAtributosAsociadosQuery(datos);
            uow.HandleQuery(dbQuery);
            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSortAtributosAsociados);
        }

        public virtual GridStats FetchStatsAtributosSinDefinir(IUnitOfWork uow, Grid grid, GridFetchStatsContext context)
        {
            var datos = GetDatos(context.Parameters);

            var dbQuery = new DetallePedidoLpnAtributosNoAsociadosQuery(datos);
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

            var dbQuery = new DetallePedidoLpnAtributosAsociadosQuery(datos);
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

            var dbQuery = new DetallePedidoLpnAtributosNoAsociadosQuery(datos);
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

            var dbQuery = new DetallePedidoLpnAtributosAsociadosQuery(datos);
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
                CheckIfLocked(datos.GetLockId(lpn: true));

            var dbQuery = new DetallePedidoLpnAtributosNoAsociadosQuery(datos);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
            {
                keys = dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys)
                    .Select(l => new KeyAtributoTipo()
                    {
                        TipoLpn = l[0],
                        IdAtributo = int.Parse(l[1]),
                        Cabezal = l[2]
                    })
                    .ToList();
            }
            else
            {
                keys = dbQuery.GetSelectedKeys(context.Selection.Keys)
                    .Select(l => new KeyAtributoTipo()
                    {
                        TipoLpn = l[0],
                        IdAtributo = int.Parse(l[1]),
                        Cabezal = l[2]
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
                CheckIfLocked(datos.GetLockId(lpn: true));

            var dbQuery = new DetallePedidoLpnAtributosAsociadosQuery(datos);
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
            uow.CreateTransactionNumber("PRE100AsociarAtributoLpn: MenuItemActionQuitarAtributos");
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
                            var detallePedidoLpnAtributo = uow.ManejoLpnRepository.GetDetallePedidoLpnAtributo(key.Pedido, key.Cliente, key.Empresa, key.Producto, key.Faixa, key.Identificador, key.IdEspecificaIdentificador, key.TipoLpn, key.IdExternoLpn, key.IdConfiguracion);

                            detallePedidoLpnAtributo.FechaModificacion = DateTime.Now;
                            detallePedidoLpnAtributo.Transaccion = uow.GetTransactionNumber();
                            detallePedidoLpnAtributo.TransaccionDelete = uow.GetTransactionNumber();

                            uow.ManejoLpnRepository.UpdateDetallePedidoLpnAtributo(detallePedidoLpnAtributo);
                            uow.SaveChanges();

                            uow.ManejoLpnRepository.DeleteDetallePedidoLpnAtributo(detallePedidoLpnAtributo);

                            terminarEdicion = true;
                        }
                    }
                    else
                    {
                        var atributoTemporal = uow.ManejoLpnRepository.GetDetallePedidoAtributoLpnTemporal(key.Pedido, key.Cliente, key.Empresa, key.Producto, key.Faixa, key.Identificador, key.IdEspecificaIdentificador, key.TipoLpn, key.IdExternoLpn, key.IdAtributo, _identity.UserId, key.IdCabezal);

                        atributoTemporal.FechaModificacion = DateTime.Now;
                        atributoTemporal.Transaccion = uow.GetTransactionNumber();
                        atributoTemporal.TransaccionDelete = uow.GetTransactionNumber();

                        uow.ManejoLpnRepository.UpdateDetallePedidoAtributoLpnTemporal(atributoTemporal);
                        uow.SaveChanges();

                        uow.ManejoLpnRepository.DeleteDetallePedidoAtributoLpnTemporal(atributoTemporal);
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
            var atributosTemps = uow.ManejoLpnRepository.GetDetallesPedidoAtributoLpnTemporal(datos.Pedido, datos.Cliente, datos.Empresa, datos.Producto, datos.Faixa, datos.Identificador, datos.IdEspecificaIdentificador, datos.TipoLpn, datos.IdExternoLpn, _identity.UserId);

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

                uow.ManejoLpnRepository.UpdateDetallePedidoAtributoLpnTemporal(at);
                uow.SaveChanges();

                uow.ManejoLpnRepository.DeleteDetallePedidoAtributoLpnTemporal(at);
            }
        }

        public virtual DetallePedidoLpnEspecifico GetDatos(List<ComponentParameter> parametros, bool gridDetalle = false)
        {
            var dto = new DetallePedidoLpnEspecifico()
            {
                Pedido = parametros.FirstOrDefault(p => p.Id == "pedido").Value,
                Cliente = parametros.FirstOrDefault(p => p.Id == "cliente").Value,
                Empresa = int.Parse(parametros.FirstOrDefault(p => p.Id == "empresa").Value),
                Producto = parametros.FirstOrDefault(p => p.Id == "producto").Value,
                Faixa = decimal.Parse(parametros.FirstOrDefault(p => p.Id == "faixa").Value, _identity.GetFormatProvider()),
                Identificador = parametros.FirstOrDefault(p => p.Id == "identificador").Value,
                IdEspecificaIdentificador = parametros.FirstOrDefault(p => p.Id == "idEspecificaIdentificador").Value,
                TipoLpn = parametros.FirstOrDefault(p => p.Id == "tipoLpn").Value,
                IdExternoLpn = parametros.FirstOrDefault(p => p.Id == "idExternoLpn").Value,
                Update = (parametros.FirstOrDefault(p => p.Id == "update")?.Value ?? "N") == "S",
                UserId = _identity.UserId,
                Detalle = gridDetalle,
                PageReanOnly = (parametros.FirstOrDefault(p => p.Id == "readOnly")?.Value ?? "N") == "S",
            };

            var idConfiguracion = parametros.FirstOrDefault(p => p.Id == "idConfiguracion")?.Value;
            if (!string.IsNullOrEmpty(idConfiguracion))
                dto.IdConfiguracion = long.Parse(idConfiguracion);

            var cantidad = parametros.FirstOrDefault(p => p.Id == "cantidad")?.Value;
            if (!string.IsNullOrEmpty(cantidad))
                dto.Cantidad = decimal.Parse(cantidad, _identity.GetFormatProvider());

            return dto;
        }

        public virtual void CheckIfLocked(string key)
        {
            if (this._concurrencyControl.IsLocked("T_DET_PEDIDO_SAIDA_LPN_ATRIB", key))
                throw new EntityLockedException("General_Sec0_Error_LockedEntity");
        }

        public virtual bool PermiteEdicion(IUnitOfWork uow, DetallePedidoLpnEspecifico datos)
        {
            var detalleTrabajado = false;
            var pedido = uow.PedidoRepository.GetPedido(datos.Empresa, datos.Cliente, datos.Pedido);
            var atributosHabilitados = (_paramService.GetValueByEmpresa(ParamManager.IE_503_HAB_ATRIBUTOS, datos.Empresa) ?? "N") == "S";
            var lpnHabilitados = (_paramService.GetValueByEmpresa(ParamManager.IE_503_HAB_LPN, datos.Empresa) ?? "N") == "S";

            if (datos.Update && datos.IdConfiguracion != null)
            {
                detalleTrabajado = uow.ManejoLpnRepository.AnyDetallePedidoLpnAtributoTrabajado(datos);

                if (this._concurrencyControl.IsLocked("T_DET_PEDIDO_SAIDA_LPN_ATRIB", datos.GetLockId(lpn: true)))
                    detalleTrabajado = true;
                else
                    this._concurrencyControl.AddLock("T_DET_PEDIDO_SAIDA_LPN_ATRIB", datos.GetLockId(lpn: true));
            }

            return pedido.IsManual && atributosHabilitados && lpnHabilitados && !detalleTrabajado;
        }

        public virtual bool PermiteConfirmar(IUnitOfWork uow, DetallePedidoLpnEspecifico datos)
        {
            return uow.ManejoLpnRepository.AnyAtributoLpnAsociado(datos);
        }
        #endregion
    }
}
