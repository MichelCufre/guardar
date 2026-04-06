using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Domain.General;
using WIS.Domain.General.Configuracion;
using WIS.Domain.Recorridos;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.REG
{
    public class REG040PanelUbicaciones : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ILogger<REG040PanelUbicaciones> _logger;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IValidationService _validationService;
        protected readonly IUbicacionService _ubicacionService;
        protected readonly UbicacionMapper _mapper;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REG040PanelUbicaciones(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            ILogger<REG040PanelUbicaciones> logger,
            IGridValidationService gridValidationService,
            ITrafficOfficerService concurrencyControl,
            ISecurityService security,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            IValidationService validationService,
            IUbicacionService ubicacionService)
        {
            this.GridKeys = new List<string>
            {
                "CD_ENDERECO",
            };


            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("DT_UPDROW",SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
            this._concurrencyControl = concurrencyControl;
            this._security = security;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._validationService = validationService;
            this._ubicacionService = ubicacionService;
            this._mapper = new UbicacionMapper();

        }


        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsAddEnabled = false;
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = true;

            grid.AddOrUpdateColumn(new GridColumnSelect("CD_CLASSE", this.OptionSelectClase()));
            grid.AddOrUpdateColumn(new GridColumnSelect("CD_AREA_ARMAZ", this.OptionSelectArea()));
            grid.AddOrUpdateColumn(new GridColumnSelect("CD_ROTATIVIDADE", this.OptionSelectRotatividad()));
            grid.AddOrUpdateColumn(new GridColumnSelect("CD_TIPO_ENDERECO", this.OptionSelectTipoUbicacion()));
            grid.AddOrUpdateColumn(new GridColumnSelect("CD_ZONA_UBICACION", this.OptionSelectZona()));
            grid.AddOrUpdateColumn(new GridColumnSelect("CD_CONTROL_ACCESO", this.OptionSelectControlAcceso()));
            grid.AddOrUpdateColumn(new GridColumn { Id = "VL_ORDEN_DEFECTO", Name = "VL_ORDEN_DEFECTO", Insertable = true, Hidden = true });

            grid.MenuItems = new List<IGridItem>
            {
                new GridButton("btnImprimir", "IMP050_grid1_btn_imprimir")
            };

            var insertableColumns = new List<string>()
            {
                "CD_ENDERECO",
                "CD_EMPRESA",
                "CD_TIPO_ENDERECO",
                "CD_ROTATIVIDADE",
                "CD_FAMILIA_PRINCIPAL",
                "CD_CLASSE",
                "ID_ENDERECO_BAIXO",
                "CD_AREA_ARMAZ",
                "NU_COMPONENTE",
                "CD_ZONA_UBICACION",
                "ID_BLOQUE",
                "ID_CALLE",
                "NU_COLUMNA",
                "NU_ALTURA",
                "CD_CONTROL",
                "NU_PROFUNDIDAD",
                "VL_ORDEN_DEFECTO",
                "CD_BARRAS"
            };

            grid.SetInsertableColumns(insertableColumns);

            var result = this._security.CheckPermissions(
            [
                "WREG040_grid1_btn_ImportarUbicaciones",
            ]);

            if (result["WREG040_grid1_btn_ImportarUbicaciones"]) query.AddParameter("REG040_IMPORT_HABILITADO", "S");

            else query.AddParameter("REG040_IMPORT_HABILITADO", "N");

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            UbicacionesQuery dbQuery;

            if (query.Parameters.Any(i => i.Id == "ubicacion"))
            {
                if (string.IsNullOrEmpty(query.Parameters.FirstOrDefault(d => d.Id == "ubicacion").Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string ubicacion = query.Parameters.FirstOrDefault(d => d.Id == "ubicacion").Value;
                dbQuery = new UbicacionesQuery(ubicacion);
            }
            else
            {
                dbQuery = new UbicacionesQuery();
            }

            uow.HandleQuery(dbQuery);


            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            var configuracion = uow.UbicacionRepository.GetUbicacionConfiguracion();

            foreach (var row in grid.Rows)
            {
                if (configuracion.AreasMantenibles.Contains(short.Parse(row.GetCell("CD_AREA_ARMAZ").Value)))
                {
                    var editableColumns = new List<string>
                    {
                            "CD_EMPRESA",
                            "CD_CLASSE",
                            "CD_AREA_ARMAZ",
                            "CD_FAMILIA_PRINCIPAL",
                            "CD_ROTATIVIDADE",
                            "CD_TIPO_ENDERECO",
                            "ID_ENDERECO_BAIXO",
                            "CD_CONTROL",
                            "NU_COMPONENTE",
                            "CD_ZONA_UBICACION",
                            "CD_CONTROL_ACCESO"
                    };

                    row.SetEditableCells(editableColumns);
                }
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            UbicacionesQuery dbQuery;
            if (query.Parameters.Count() > 0)
            {

                if (string.IsNullOrEmpty(query.Parameters.FirstOrDefault(d => d.Id == "ubicacion").Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string ubicacion = query.Parameters.FirstOrDefault(d => d.Id == "ubicacion").Value;
                dbQuery = new UbicacionesQuery(ubicacion);
            }
            else
            {
                dbQuery = new UbicacionesQuery();
            }

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            UbicacionesQuery dbQuery;
            if (query.Parameters.Count() > 0)
            {

                if (string.IsNullOrEmpty(query.Parameters.FirstOrDefault(d => d.Id == "ubicacion").Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string ubicacion = query.Parameters.FirstOrDefault(d => d.Id == "ubicacion").Value;
                dbQuery = new UbicacionesQuery(ubicacion);
            }
            else
            {
                dbQuery = new UbicacionesQuery();
            }

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("CD_ENDERECO", SortDirection.Ascending);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, defaultSort);
        }

        public override Grid GridImportExcel(Grid grid, GridImportExcelContext context)
        {
            if (!context.FetchContext.HasParameter("predio") || string.IsNullOrEmpty(context.FetchContext.GetParameter("predio")))
                throw new MissingParameterException("REG040_Err0_Error_Err006_FaltaPredio");

            if (context.Payload == null)
                throw new MissingParameterException("General_Sec0_Error_ParametrosInvalidos");

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var predio = context.FetchContext.GetParameter("predio");

                var colParams = new Dictionary<string, string>
                {
                    [ParamManager.PARAM_PRED] = $"{ParamManager.PARAM_PRED}_{predio}"
                };

                var importarHabilitado = uow.ParametroRepository.GetParameter(ParamManager.REG040_PERMITE_IMPORT_UBIC, colParams)?.ToUpper() == "S";

                if (!importarHabilitado)
                    throw new Exception("REG040_Err0_Error_Err006_ImportarNoHabilitadoParaPredio");

                grid = GridFetchRows(grid, context.FetchContext);

                using (var excelImporter = new GridExcelImporter(context.Translator, context.FileName, grid.Columns, context.Payload))
                {
                    uow.BeginTransaction();

                    uow.CreateTransactionNumber("GridImportExcel", _identity.Application, _identity.UserId);

                    this._concurrencyControl.CreateToken();

                    var transaction = this._concurrencyControl.CreateTransaccion();

                    try
                    {
                        if (this._concurrencyControl.IsLocked("T_PREDIO", predio, true))
                            throw new ValidationFailedException("REG040_msg_Error_PredioBloqueado");

                        this._concurrencyControl.AddLock("T_PREDIO", predio, transaction, true);

                        excelImporter.CleanErrors();

                        var excelRows = excelImporter.BuildRows();

                        var ubicacionesExternas = ProcessExcelRows(excelRows, grid.Columns, predio);

                        if (!ubicacionesExternas.Any())
                            throw new ValidationFailedException("REG040_msg_Error_ExcelVacio");

                        grid.Rows = excelRows;

                        if (grid.HasNewDuplicates(GridKeys) || grid.HasDuplicates(GridKeys))
                            throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                        var importResult = _ubicacionService.ImportarUbicaciones(ubicacionesExternas, uow).GetAwaiter().GetResult();

                        HandleImportResult(importResult, context, excelImporter);

                        uow.SaveChanges();
                        uow.Commit();

                        grid = GridFetchRows(grid, context.FetchContext);
                    }
                    catch (ValidationFailedException ex)
                    {
                        uow.Rollback();

                        _logger.LogError(ex, ex.Message);

                        var payload = Convert.ToBase64String(excelImporter.GetAsByteArray());

                        throw new GridExcelImporterException(ex.Message, payload, ex.StrArguments);
                    }
                    catch (Exception ex)
                    {
                        uow.Rollback();
                        _logger.LogError(ex, ex.Message);
                        context.AddErrorNotification(ex.Message);
                        throw;
                    }
                    finally
                    {
                        this._concurrencyControl.DeleteTransaccion(transaction);
                    }
                }
            }

            return grid;
        }

        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext query)
        {
            switch (query.ColumnId)
            {
                case "CD_EMPRESA": return this.SearchEmpresa(grid, row, query);
                case "CD_FAMILIA_PRINCIPAL": return this.SearchProductoFamilia(grid, row, query);
            }

            return new List<SelectOption>();
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoUbicacionesValidationModule(uow), grid, row, context);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber("GridCommit", _identity.Application, _identity.UserId);

            uow.BeginTransaction();

            try
            {
                if (grid.Rows.Count != 0)
                {
                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    UbicacionConfiguracion configuracion = uow.UbicacionRepository.GetUbicacionConfiguracion();
                    List<UbicacionArea> areas = new List<UbicacionArea>();

                    List<Recorrido> recorridosPorDefecto = new List<Recorrido>();
                    foreach (var row in grid.Rows)
                    {
                        if (row.IsDeleted) EliminarUbicacion(uow, row);

                        else if (!row.IsNew) EditarUbicacion(uow, row, configuracion, areas, recorridosPorDefecto);
                    }
                }

                uow.SaveChanges();
                uow.Commit();

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ExpectedException ex)
            {
                uow.Rollback();
                this._logger.LogWarning(ex, "GridCommit");
                query.AddErrorNotification(ex.Message);
            }
            catch (Exception ex)
            {
                uow.Rollback();
                this._logger.LogError(ex, "REG040GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext selection)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            List<string> filasSeleccionadas = this.ObtenerKeyLineasSeleccionadas(uow, selection);

            selection.AddParameter("ListaFilasSeleccionadas", JsonConvert.SerializeObject(filasSeleccionadas));

            return selection;
        }

        #region Metodos Auxiliares

        public virtual List<string> ObtenerKeyLineasSeleccionadas(IUnitOfWork uow, GridMenuItemActionContext selection)
        {
            UbicacionesQuery dbQuery;

            if (selection.Parameters.Count() > 0)
            {
                if (string.IsNullOrEmpty(selection.Parameters.FirstOrDefault(d => d.Id == "ubicacion").Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string ubicacionParam = selection.Parameters.FirstOrDefault(d => d.Id == "ubicacion").Value;
                dbQuery = new UbicacionesQuery(ubicacionParam);
            }
            else
            {
                dbQuery = new UbicacionesQuery();
            }

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, selection.Filters);

            List<string> resultado = new List<string>();

            string ubicacion;
            if (selection.Selection.AllSelected)
            {
                var selectAll = dbQuery.GetResult().Select(g => new { g.CD_ENDERECO }).ToList();

                foreach (var noSeleccionKeys in selection.Selection.Keys)
                {

                    ubicacion = noSeleccionKeys;

                    selectAll.Remove(selectAll.FirstOrDefault(z => z.CD_ENDERECO == ubicacion));
                }

                foreach (var key in selectAll)
                {
                    resultado.Add(string.Join("$", new List<string> { key.CD_ENDERECO }));
                }
            }
            else
            {
                foreach (var key in selection.Selection.Keys)
                {
                    resultado.Add(string.Join("$", key));
                }
            }

            return resultado;
        }

        public virtual List<SelectOption> SearchEmpresa(Grid grid, GridRow row, GridSelectSearchContext query)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Empresa> empresas = uow.EmpresaRepository.GetByNombreOrCodePartial(query.SearchValue);

            foreach (var empresa in empresas)
            {
                opciones.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchProductoFamilia(Grid grid, GridRow row, GridSelectSearchContext query)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<ProductoFamilia> familias = uow.ProductoFamiliaRepository.GetByNombreOrCodePartial(query.SearchValue);

            foreach (var familia in familias)
            {
                opciones.Add(new SelectOption(familia.Id.ToString(), $"{familia.Id} - {familia.Descripcion}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> OptionSelectClase()
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Clase> clases = uow.ClaseRepository.GetClases();

            foreach (var clase in clases)
            {
                opciones.Add(new SelectOption(clase.Id, $"{clase.Id} - {clase.Descripcion}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> OptionSelectArea()
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<UbicacionArea> areas = uow.UbicacionAreaRepository.GetUbicacionAreasMantenibles();

            foreach (var area in areas)
            {
                opciones.Add(new SelectOption(area.Id.ToString(), $"{area.Id} - {area.Descripcion}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> OptionSelectZona()
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<ZonaUbicacion> zonas = uow.ZonaUbicacionRepository.GetZonasHabilitadas();

            foreach (var zona in zonas)
            {
                opciones.Add(new SelectOption(zona.Id, $"{zona.Id} - {zona.Descripcion}"));
            }
            return opciones;
        }

        public virtual List<SelectOption> OptionSelectRotatividad()
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<ProductoRotatividad> rotatividades = uow.ProductoRotatividadRepository.GetProductoRotatividades();

            foreach (var rotatividad in rotatividades)
            {
                opciones.Add(new SelectOption(rotatividad.Id.ToString(), $"{rotatividad.Id} - {rotatividad.Descripcion}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> OptionSelectTipoUbicacion()
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<UbicacionTipo> tipos = uow.UbicacionTipoRepository.GetUbicacionTipos();

            foreach (var tipo in tipos)
            {
                opciones.Add(new SelectOption(tipo.Id.ToString(), $"{tipo.Id} - {tipo.Descripcion}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> OptionSelectControlAcceso()
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<ControlAcceso> controlesAcceso = uow.ZonaUbicacionRepository.GetControlesAcceso();

            foreach (var controlAcceso in controlesAcceso)
            {
                opciones.Add(new SelectOption(controlAcceso.Id, $"{controlAcceso.Id} - {controlAcceso.Descripcion}"));
            }

            return opciones;
        }


        public virtual Ubicacion EditarUbicacion(IUnitOfWork uow, GridRow row, UbicacionConfiguracion configuracion, List<UbicacionArea> areas, List<Recorrido> recorridosPorDefecto)
        {
            Ubicacion ubicacion = uow.UbicacionRepository.GetUbicacion(row.GetCell("CD_ENDERECO").Value);

            if (ubicacion == null)
                throw new EntityNotFoundException("REG040_Grid_Error_UbicacionNoExiste", new string[] { row.GetCell("CD_ENDERECO").Value });

            var fieldArea = row.GetCell("CD_AREA_ARMAZ");

            if (short.TryParse(fieldArea.Value, out short idArea))
            {
                if (!configuracion.AreasMantenibles.Contains(idArea))
                    throw new ValidationFailedException("REG040_Grid_Error_AreaNoMantenimbre", new string[] { idArea.ToString() });

                if (fieldArea.Old != fieldArea.Value)
                {
                    if (!uow.StockRepository.AnyStockUbicacion(ubicacion.Id))
                    {
                        ubicacion.IdUbicacionArea = idArea;

                        var area = areas.FirstOrDefault(x => x.Id == idArea);
                        if (area == null)
                        {
                            area = uow.UbicacionAreaRepository.GetUbicacionArea(idArea);
                            areas.Add(area);
                        }

                        var recorridoPorDefecto = recorridosPorDefecto.FirstOrDefault(x => x.Predio == row.GetCell("NU_PREDIO").Value);
                        if (recorridoPorDefecto == null)
                        {
                            recorridoPorDefecto = uow.RecorridoRepository.GetRecorridoPorDefectoParaPredio(row.GetCell("NU_PREDIO").Value);
                            recorridosPorDefecto.Add(recorridoPorDefecto);
                        }

                        if (area.EsAreaMantenible && ((area.EsAreaStockGeneral && area.DisponibilizaStock)
                             || (area.EsAreaPicking && area.DisponibilizaStock)
                             || area.EsAreaAveria))
                        {
                            InsertarDetalleRecorrido(uow, recorridoPorDefecto, ubicacion);
                        }
                        else
                        {
                            uow.RecorridoRepository.EliminarUbicacionDeRecorridos(ubicacion);
                        }
                    }
                }
            }

            var fieldEmpresa = row.GetCell("CD_EMPRESA");

            if (fieldEmpresa.Old != fieldEmpresa.Value && !uow.StockRepository.AnyStockUbicacion(ubicacion.Id))
                ubicacion.IdEmpresa = int.Parse(row.GetCell("CD_EMPRESA").Value);

            ubicacion.CodigoClase = row.GetCell("CD_CLASSE").Value;

            if (int.TryParse(row.GetCell("CD_FAMILIA_PRINCIPAL").Value, out int idProductoFamilia))
                ubicacion.IdProductoFamilia = idProductoFamilia;

            if (short.TryParse(row.GetCell("CD_ROTATIVIDADE").Value, out short idProductoRotatividad))
                ubicacion.IdProductoRotatividad = idProductoRotatividad;

            if (short.TryParse(row.GetCell("CD_TIPO_ENDERECO").Value, out short idUbicacionTipo))
                ubicacion.IdUbicacionTipo = idUbicacionTipo;

            ubicacion.EsUbicacionBaja = !string.IsNullOrEmpty(row.GetCell("ID_ENDERECO_BAIXO").Value) && row.GetCell("ID_ENDERECO_BAIXO").Value == "S";

            ubicacion.CodigoControl = row.GetCell("CD_CONTROL").Value;

            if (string.IsNullOrEmpty(row.GetCell("CD_ZONA_UBICACION").Value))
                ubicacion.IdUbicacionZona = configuracion.UbicacionZonaPorDefecto;
            else
                ubicacion.IdUbicacionZona = row.GetCell("CD_ZONA_UBICACION").Value;

            ubicacion.IdControlAcceso = row.GetCell("CD_CONTROL_ACCESO").Value;

            uow.UbicacionRepository.UpdateUbicacion(ubicacion);

            return ubicacion;
        }

        public virtual void EliminarUbicacion(IUnitOfWork uow, GridRow row)
        {
            var idUbicacion = row.GetCell("CD_ENDERECO").Value;

            var ubicacion = uow.UbicacionRepository.GetUbicacion(idUbicacion);

            if (ubicacion == null)
            {
                throw new EntityNotFoundException("REG040_Grid_Error_UbicacionNoExiste", new string[] { idUbicacion });
            }

            // Control, No debe existir stock en la ubicacion
            if (uow.StockRepository.AnyStockUbicacion(idUbicacion))
            {
                throw new ValidationFailedException("REG040_Grid_Error_ExisteStockUbicacionDelete", new string[] { idUbicacion });
            }

            // Control, La ubicación no puede estar asignada a picking producto
            if (uow.UbicacionPickingProductoRepository.AnyUbicacionPickingProducto(idUbicacion))
            {
                throw new ValidationFailedException("REG040_Grid_Error_PickingProductoAsignadoUbicacionDelete", new string[] { idUbicacion });
            }

            // Control, La ubicación no puede estar asignada a un inventario
            if (uow.InventarioRepository.AnyInventarioEnUbicacion(idUbicacion))
            {
                throw new ValidationFailedException("REG040_Grid_Error_UbicacionEnInventarioDelete", new string[] { idUbicacion });
            }

            // Control, La ubicación no puede estar asiganda a una estación de trabajo
            if (uow.EstacionDeTrabajoRepository.AnyEstacion(idUbicacion))
            {
                throw new ValidationFailedException("REG040_Grid_Error_UbicacionAsigandaAEstacionDeTrabajoDelete", new string[] { idUbicacion });
            }

            // Control, La ubicación no puede tener legajo en el trace de stock
            if (uow.StockTraceRepository.AnyStockTrace(idUbicacion))
            {
                throw new ValidationFailedException("REG040_Grid_Error_UbicacionEnTraceStockDelete", new string[] { idUbicacion });
            }

            uow.RecorridoRepository.EliminarUbicacionDeRecorridos(ubicacion);

            uow.UbicacionRepository.RemoveUbicacion(ubicacion);
        }

        public virtual void InsertarDetalleRecorrido(IUnitOfWork uow, Recorrido recorridoPorDefecto, Ubicacion ubicacion)
        {
            if (!uow.RecorridoRepository.AnyUbicacionAsociadaRecorrido(recorridoPorDefecto.Id, ubicacion.Id))
            {
                var detalleRecorrido = new DetalleRecorrido
                {
                    IdRecorrido = recorridoPorDefecto.Id,
                    Ubicacion = ubicacion.Id,
                    ValorOrden = ubicacion.OrdenDefecto != null ? $"{ubicacion.OrdenDefecto}".PadLeft(40, '0') : ubicacion.Id,
                    NumeroOrden = ubicacion.OrdenDefecto == null ? -1 : ubicacion.OrdenDefecto,
                    Transaccion = uow.GetTransactionNumber()
                };

                uow.RecorridoRepository.AddDetalleRecorrido(detalleRecorrido);
            }
        }


        #region GRID IMPORT UTILS
        public virtual List<UbicacionExterna> ProcessExcelRows(IEnumerable<GridRow> excelRows, List<IGridColumn> columns, string predio)
        {
            var ubicacionesExternas = new List<UbicacionExterna>();
            var uniqueRowIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var row in excelRows)
            {
                AddMissingCellsToRow(row, columns);

                var rowId = row.GetCell("CD_ENDERECO").Value.ToUpper();

                if (!uniqueRowIds.Add(rowId))
                    throw new ValidationFailedException("REG040_msg_Error_UbicacionesRepetidas", [rowId]);

                row.Id = rowId;
                ubicacionesExternas.Add(GetObjectFromRow(row, predio));
            }

            return ubicacionesExternas;
        }

        public virtual void AddMissingCellsToRow(GridRow row, List<IGridColumn> columns)
        {
            foreach (var column in columns)
            {
                if (!row.Cells.Any(c => c.Column.Id == column.Id))
                {
                    row.AddCell(new GridCell { Column = column });
                }
            }
        }

        public virtual UbicacionExterna GetObjectFromRow(GridRow row, string predio)
        {
            #region - VARIABLES - 
            var codigoBarras = row.GetCell("CD_BARRAS").Value.ToUpper();
            var clase = row.GetCell("CD_CLASSE").Value.ToUpper();
            var idUbicacionBaja = row.GetCell("ID_ENDERECO_BAIXO").Value.ToUpper();
            var codigoControl = _mapper.NullIfEmpty(row.GetCell("CD_CONTROL").Value.ToUpper());
            var componenteFacturacion = _mapper.NullIfEmpty(row.GetCell("NU_COMPONENTE").Value.ToUpper());
            var zona = row.GetCell("CD_ZONA_UBICACION").Value.ToUpper();
            var bloque = row.GetCell("ID_BLOQUE").Value.ToUpper();
            var calle = row.GetCell("ID_CALLE").Value.ToUpper();
            long? ordenDefecto = long.TryParse(row.GetCell("VL_ORDEN_DEFECTO").Value, out long vlOrden) ? vlOrden : null;
            var valorOrden = ordenDefecto == null ? string.Empty : $"{ordenDefecto}".PadLeft(40, '0');
            var controlAcceso = row.GetCell("CD_CONTROL_ACCESO").Value;
            #endregion

            return new UbicacionExterna
            {
                Id = row.Id,
                CodigoSituacion = SituacionDb.Activo,
                IdEsUbicacionSeparacion = "N",
                IdNecesitaReabastecer = "N",
                CodigoBarras = codigoBarras,
                CodigoClase = clase,
                IdUbicacionBaja = idUbicacionBaja,
                FechaInsercion = DateTime.Now,
                CodigoControl = codigoControl,
                FacturacionComponente = componenteFacturacion,
                IdUbicacionZona = zona,
                NumeroPredio = predio,
                Bloque = bloque,
                Calle = calle,
                IdControlAcceso = null,
                OrdenDefecto = ordenDefecto,
                ValorDefecto = valorOrden,

                CodigoEmpresa = _mapper.NullIfEmpty(row.GetCell("CD_EMPRESA").Value),
                CodigoTipoUbicacion = _mapper.NullIfEmpty(row.GetCell("CD_TIPO_ENDERECO").Value),
                CodigoRotatividad = _mapper.NullIfEmpty(row.GetCell("CD_ROTATIVIDADE").Value),
                CodigoFamilia = _mapper.NullIfEmpty(row.GetCell("CD_FAMILIA_PRINCIPAL").Value),
                CodigoArea = _mapper.NullIfEmpty(row.GetCell("CD_AREA_ARMAZ").Value),
                NuColumna = _mapper.NullIfEmpty(row.GetCell("NU_COLUMNA").Value),
                NuAltura = _mapper.NullIfEmpty(row.GetCell("NU_ALTURA").Value),
                NuProfundidad = _mapper.NullIfEmpty(row.GetCell("NU_PROFUNDIDAD").Value),
                CodigoControlAcceso = _mapper.NullIfEmpty(row.GetCell("CD_CONTROL_ACCESO").Value),
            };
        }

        public virtual void HandleImportResult(ValidationsResult importResult, GridImportExcelContext context, GridExcelImporter excelImporter)
        {
            if (!importResult.HasError())
            {
                context.AddSuccessNotification("REG040_Sec0_Success_Suc006_UbicacionesImportadas");
            }
            else
            {
                var dictErrors = new Dictionary<int, List<string>>();

                foreach (var error in importResult.Errors)
                {
                    if (!dictErrors.ContainsKey(error.ItemId))
                        dictErrors[error.ItemId] = new List<string>();

                    dictErrors[error.ItemId].AddRange(error.Messages.Select(m => m.TrimEnd('.')));
                }

                excelImporter.SetErrors(dictErrors);

                throw new ValidationFailedException("REG040_Err0_Error_Err006_UbicacionesImportadas");
            }
        }

        #endregion

        #endregion
    }
}