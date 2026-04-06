using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Domain.General;
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
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.REG
{
    public class REG050AsignacionUbicacionPicking : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ILogger<REG050AsignacionUbicacionPicking> _logger;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly ISessionAccessor _session;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IAutomatismoAutoStoreClientService _automatismoAutoStoreClientService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REG050AsignacionUbicacionPicking(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            ILogger<REG050AsignacionUbicacionPicking> logger,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            ISessionAccessor session,
            IFilterInterpreter filterInterpreter,
            IAutomatismoAutoStoreClientService automatismoAutoStoreClientService)
        {
            this.GridKeys = new List<string>
            {
                "NU_SEC_PICKING_PRODUTO"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_SEC_PICKING_PRODUTO", SortDirection.Descending),
                new SortCommand("CD_PRODUTO", SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._logger = logger;
            this._identity = identity;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            this._session = session;
            this._filterInterpreter = filterInterpreter;
            this._automatismoAutoStoreClientService = automatismoAutoStoreClientService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsAddEnabled = true;
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = true;

            context.AddLink("CD_PRODUTO", "registro/REG009", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_PRODUTO", "producto"), new GridColumnLinkMapping("CD_EMPRESA", "empresa") });
            context.AddLink("CD_ENDERECO", "registro/REG040", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_ENDERECO", "ubicacion") });
            context.AddLink("CD_EMPRESA", "registro/REG100", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            using var uow = this._uowFactory.GetUnitOfWork();

            grid.AddOrUpdateColumn(new GridColumnSelect("CD_UNIDAD_CAJA_AUT", this.SelectTipoSubdivisiónAutomatismo(uow)));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PickingProductoQuery dbQuery;

            if (context.Parameters.Count > 1)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!context.Parameters.Any(s => s.Id == "producto"))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idProducto = context.Parameters.FirstOrDefault(x => x.Id == "producto").Value;
                dbQuery = new PickingProductoQuery(idProducto, idEmpresa);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

                grid.GetColumn("CD_PRODUTO").Hidden = true;
                grid.GetColumn("DS_PRODUTO").Hidden = true;
                grid.GetColumn("CD_EMPRESA").Hidden = true;
                grid.GetColumn("NM_EMPRESA").Hidden = true;
                grid.GetColumn("NU_SEC_PICKING_PRODUTO").Hidden = true;

                Empresa empresa = uow.EmpresaRepository.GetEmpresa(idEmpresa);
                string descProducto = uow.ProductoRepository.GetDescripcion(idEmpresa, idProducto.ToString());

                context.AddParameter("REG050_CD_EMPRESA", idEmpresa.ToString());
                context.AddParameter("REG050_NM_EMPRESA", empresa.Nombre);
                context.AddParameter("REG050_CD_PRODUCTO", idProducto.ToString());
                context.AddParameter("REG050_DS_PRODUCTO", descProducto);
            }
            else
            {
                dbQuery = new PickingProductoQuery();

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

                grid.GetColumn("DS_PRODUTO").Hidden = false;
                grid.GetColumn("CD_PRODUTO").Hidden = false;
                grid.GetColumn("CD_EMPRESA").Hidden = false;
                grid.GetColumn("NM_EMPRESA").Hidden = false;
            }

            grid.SetInsertableColumns(new List<string>
            {
                "QT_DESBORDE",
                "QT_ESTOQUE_MAXIMO",
                "QT_ESTOQUE_MINIMO",
                "CD_EMPRESA",
                "CD_PRODUTO",
                "CD_ENDERECO_SEPARACAO",
                "QT_PADRAO_PICKING",
                "CD_UNIDAD_CAJA_AUT",
                "QT_UNIDAD_CAJA_AUT",
                "FL_CONF_CD_BARRAS_AUT",
                "NU_PRIORIDAD"
            });

            grid.SetEditableCells(new List<string>
            {
                "QT_DESBORDE",
                "QT_ESTOQUE_MAXIMO",
                "QT_ESTOQUE_MINIMO",
                "QT_ESTOQUE_MINIMO",
                "CD_UNIDAD_CAJA_AUT",
                "QT_UNIDAD_CAJA_AUT",
                "FL_CONF_CD_BARRAS_AUT",
                "NU_PRIORIDAD"
            });

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PickingProductoQuery dbQuery;

            if (context.Parameters.Count > 1)
            {

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!context.Parameters.Any(s => s.Id == "producto"))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idProducto = context.Parameters.FirstOrDefault(x => x.Id == "producto").Value;

                dbQuery = new PickingProductoQuery(idProducto, idEmpresa);

                uow.HandleQuery(dbQuery);
            }
            else
            {
                dbQuery = new PickingProductoQuery();
                uow.HandleQuery(dbQuery);
            }

            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PickingProductoQuery dbQuery;

            if (context.Parameters.Count > 1)
            {

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!context.Parameters.Any(s => s.Id == "producto"))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idProducto = context.Parameters.FirstOrDefault(x => x.Id == "producto").Value;


                dbQuery = new PickingProductoQuery(idProducto, idEmpresa);
            }
            else
            {
                dbQuery = new PickingProductoQuery();
            }

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            switch (context.ColumnId)
            {
                case "CD_EMPRESA":
                    return this.SearchEmpresa(grid, row, context);
                case "CD_PRODUTO":
                    return this.SearchProduto(grid, row, context);
                case "CD_ENDERECO_SEPARACAO":
                    return this.SearchUbicacion(grid, row, context);
            }

            return new List<SelectOption>();
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber(this._identity.Application);
            uow.BeginTransaction();

            try
            {
                if (grid.Rows.Any())
                {
                    var ubicacionesPicking = new List<UbicacionPickingTipoOperacion>();

                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    foreach (var row in grid.Rows)
                    {
                        GetDatosParam(context, row, out int empresa, out string codigoProducto);

                        var padronPicking = decimal.Parse(row.GetCell("QT_PADRAO_PICKING").Value, _identity.GetFormatProvider());
                        var stockMinimo = int.Parse(row.GetCell("QT_ESTOQUE_MINIMO")?.Value);
                        var stockMaximo = int.Parse(row.GetCell("QT_ESTOQUE_MAXIMO")?.Value);
                        var desborde = int.Parse(row.GetCell("QT_DESBORDE").Value);
                        var prioridad = int.Parse(row.GetCell("NU_PRIORIDAD").Value);
                        var ubicacion = uow.UbicacionRepository.GetUbicacion(row.GetCell("CD_ENDERECO_SEPARACAO").Value);

                        var codUnidadCajaAut = row.GetCell("CD_UNIDAD_CAJA_AUT").Value;

                        if (string.IsNullOrEmpty(codUnidadCajaAut))
                            codUnidadCajaAut = uow.ParametroRepository.GetParameter(ParamManager.IE_570_CODIGO_UND_CAJA_AUT, new Dictionary<string, string> { [ParamManager.PARAM_GRAL] = $"{ParamManager.PARAM_GRAL}" });

                        int? cantUnidadCajaAut = null;

                        if (int.TryParse(row.GetCell("QT_UNIDAD_CAJA_AUT").Value, out int qtUndCajaAut))
                            cantUnidadCajaAut = qtUndCajaAut;
                        else
                        {
                            var paramValue = uow.ParametroRepository.GetParameter(ParamManager.IE_570_CANT_UND_CAJA_AUT, new Dictionary<string, string> { [ParamManager.PARAM_GRAL] = $"{ParamManager.PARAM_GRAL}" });

                            if (int.TryParse(paramValue, out int parseValue))
                                cantUnidadCajaAut = parseValue;
                        }

                        var flConfirmarCodBarrasAut = "N";

                        if (!string.IsNullOrEmpty(row.GetCell("FL_CONF_CD_BARRAS_AUT").Value) && row.GetCell("FL_CONF_CD_BARRAS_AUT").Value == "S")
                            flConfirmarCodBarrasAut = "S";


                        if (row.IsNew)
                        {
                            CrearPickingProducto(uow, ubicacion, empresa, codigoProducto, padronPicking, stockMinimo, stockMaximo, desborde, ubicacionesPicking, codUnidadCajaAut, cantUnidadCajaAut, flConfirmarCodBarrasAut, prioridad);
                        }
                        else if (row.IsDeleted)
                        {
                            EliminarPickingProducto(uow, ubicacion, empresa, codigoProducto, padronPicking, prioridad, ubicacionesPicking);
                        }
                        else // modificadas
                        {
                            var oldPrioridad = int.Parse(row.GetCell("NU_PRIORIDAD").Old);
                            ModificarPickingProducto(uow, ubicacion, empresa, codigoProducto, padronPicking, stockMinimo, stockMaximo, desborde, ubicacionesPicking, codUnidadCajaAut, cantUnidadCajaAut, flConfirmarCodBarrasAut, prioridad, oldPrioridad);

                        }

                        uow.SaveChanges();
                    }

                    uow.Commit();

                    NotificarAutomatismo(uow, ubicacionesPicking);
                }

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (AutomatismoException ex)
            {
                context.AddErrorNotification(ex.Message);
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                uow.Rollback();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "REG050GridCommit");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
                uow.Rollback();
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoUbicacionesPickingProductoValidationModule(uow, _identity.GetFormatProvider()), grid, row, context);
        }

        #region Metodos auxiliares

        public virtual List<SelectOption> SearchEmpresa(Grid grid, GridRow row, GridSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var empresasAsignadasUsuario = uow.EmpresaRepository.GetByNombreOrCodePartialForUsuario(context.SearchValue, this._identity.UserId);

            foreach (var emp in empresasAsignadasUsuario)
            {
                opciones.Add(new SelectOption(emp.Id.ToString(), $"{emp.Id} - {emp.Nombre}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchProduto(Grid grid, GridRow row, GridSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var productos = new List<Producto>();
            if (context.Parameters.Any(s => s.Id == "empresa"))
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(idEmpresa, context.SearchValue);
            }
            else
            {
                if (!string.IsNullOrEmpty(row.GetCell("CD_EMPRESA").Value))
                    productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(int.Parse(row.GetCell("CD_EMPRESA").Value), context.SearchValue);
                else
                    row.GetCell("CD_EMPRESA").SetError(new ComponentError("General_Sec0_Error_Error25", new List<string>()));
            }

            foreach (var prod in productos)
            {
                opciones.Add(new SelectOption(prod.Codigo, $"{prod.Codigo} - {prod.Descripcion}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchUbicacion(Grid grid, GridRow row, GridSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var ubicaciones = new List<Ubicacion>();

            if (context.Parameters.Any(s => s.Id == "automatismo"))
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "automatismo")?.Value, out int idAutomatismo))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                ubicaciones = uow.UbicacionRepository.GetUbiPickingAutomamismoByCodigoPartial(context.SearchValue, idAutomatismo);
            }
            else
                ubicaciones = uow.UbicacionRepository.GetUbiPickingByCodigoPartial(context.SearchValue);

            foreach (var ubi in ubicaciones)
            {
                opciones.Add(new SelectOption(ubi.Id, $"{ubi.Id}"));
            }

            return opciones;
        }

        public virtual void CrearPickingProducto(IUnitOfWork uow, Ubicacion ubicacion, int empresa, string codigoProducto, decimal padron, int cantidadMinima, int cantidadMaxima, int desborde, List<UbicacionPickingTipoOperacion> ubicacionesPicking, string codUnidadCajaAut, int? cantUnidadCajaAut, string flConfirmarCodBarrasAut, int prioridad)
        {
            var isUbicAutomatismo = uow.AutomatismoRepository.IsUbicacionAutomatismo(ubicacion.Id);
            var prod = uow.ProductoRepository.GetProducto(empresa, codigoProducto);

            var ubicacionPicking = new UbicacionPickingProducto
            {
                Empresa = empresa,
                CodigoProducto = codigoProducto,
                CantidadDesborde = desborde,
                Faixa = 1,
                FechaInsercion = DateTime.Now,
                FechaModificacion = DateTime.Now,
                UbicacionSeparacion = ubicacion.Id,
                Padron = padron,
                StockMinimo = cantidadMinima,
                StockMaximo = cantidadMaxima,
                TipoPicking = isUbicAutomatismo ? "A" : (prod.UnidadBulto == padron ? "C" : "U"),
                Predio = ubicacion.NumeroPredio,
                NuTransaccion = uow.GetTransactionNumber(),
                CodigoUnidadCajaAutomatismo = codUnidadCajaAut,
                CantidadUnidadCajaAutomatismo = cantUnidadCajaAut,
                FlagConfirmarCodBarrasAutomatismo = flConfirmarCodBarrasAut,
                Prioridad = prioridad
            };

            uow.UbicacionPickingProductoRepository.AddUbicacionPicking(ubicacionPicking);
            uow.SaveChanges();

            var ubicacionActualizar = uow.UbicacionRepository.GetUbicacion(ubicacionPicking.UbicacionSeparacion);

            if (ubicacionActualizar != null)
            {
                if (ubicacionActualizar.FacturacionComponente != ubicacionPicking.TipoPicking && ubicacionPicking.TipoPicking == "C")
                    ubicacionActualizar.FacturacionComponente = ubicacionPicking.TipoPicking;
            }

            uow.UbicacionRepository.UpdateUbicacion(ubicacionActualizar);
            uow.SaveChanges();

            ubicacionesPicking.Add(new UbicacionPickingTipoOperacion(ubicacionPicking, TipoOperacionDb.Alta));
        }

        public virtual void ModificarPickingProducto(IUnitOfWork uow, Ubicacion ubicacion, int empresa, string codigoProducto, decimal padron, int cantidadMinima, int cantidadMaxima, int desborde, List<UbicacionPickingTipoOperacion> ubicacionesPicking, string codUnidadCajaAut, int? cantUnidadCajaAut, string flConfirmarCodBarrasAut, int prioridad, int oldPrioridad)
        {
            var ubicacionPicking = uow.UbicacionPickingProductoRepository.GetUbicacionPickingProducto(empresa, codigoProducto, 1, padron, ubicacion.NumeroPredio, ubicacion.Id, oldPrioridad);

            ubicacionPicking.StockMaximo = cantidadMaxima;
            ubicacionPicking.StockMinimo = cantidadMinima;
            ubicacionPicking.CantidadDesborde = desborde;
            ubicacionPicking.FechaModificacion = DateTime.Now;
            ubicacionPicking.NuTransaccion = uow.GetTransactionNumber();
            ubicacionPicking.CantidadUnidadCajaAutomatismo = cantUnidadCajaAut;
            ubicacionPicking.CodigoUnidadCajaAutomatismo = codUnidadCajaAut;
            ubicacionPicking.FlagConfirmarCodBarrasAutomatismo = flConfirmarCodBarrasAut;
            ubicacionPicking.Prioridad = prioridad;
            uow.UbicacionPickingProductoRepository.ActualizarUbicacionPicking(ubicacionPicking);
            uow.SaveChanges();

            ubicacionesPicking.Add(new UbicacionPickingTipoOperacion(ubicacionPicking, TipoOperacionDb.Sobrescritura));
        }

        public virtual void EliminarPickingProducto(IUnitOfWork uow, Ubicacion ubicacion, int empresa, string codigoProducto, decimal padron, int prioridad, List<UbicacionPickingTipoOperacion> ubicacionesPicking)
        {
            var ubicacionPicking = uow.UbicacionPickingProductoRepository.GetUbicacionPickingProducto(empresa, codigoProducto, 1, padron, ubicacion.NumeroPredio, ubicacion.Id, prioridad);

            var stock = uow.StockRepository.GetStock(ubicacionPicking.Empresa, ubicacionPicking.CodigoProducto, ubicacionPicking.Faixa, ubicacionPicking.UbicacionSeparacion);

            if (stock != null && !stock.IsUbicacionVacia())
                throw new ValidationFailedException("REG050_grid1_msg_PickingConStock", new string[] { stock.Ubicacion, stock.Producto });

            ubicacionPicking.NuTransaccion = uow.GetTransactionNumber();
            ubicacionPicking.NuTransaccionDelete = uow.GetTransactionNumber();

            uow.UbicacionPickingProductoRepository.ActualizarUbicacionPicking(ubicacionPicking);
            uow.SaveChanges();

            uow.UbicacionPickingProductoRepository.BorrarUbicacionPicking(ubicacionPicking);
            uow.SaveChanges();

            ubicacionesPicking.Add(new UbicacionPickingTipoOperacion(ubicacionPicking, TipoOperacionDb.Baja));
        }

        public virtual void GetDatosParam(GridFetchContext context, GridRow row, out int empresa, out string codigoProducto)
        {
            empresa = -1;
            var paramEmpresa = context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value;
            paramEmpresa = string.IsNullOrEmpty(paramEmpresa) ? row.GetCell("CD_EMPRESA").Value : paramEmpresa;

            if (int.TryParse(paramEmpresa, out int parsedValue))
                empresa = parsedValue;

            var paramProducto = context.Parameters.FirstOrDefault(s => s.Id == "producto")?.Value;
            codigoProducto = string.IsNullOrEmpty(paramProducto) ? row.GetCell("CD_PRODUTO").Value : paramProducto;
        }

        public virtual void GetDatosParam1(GridFetchContext context, GridRow row, out int empresa, out string codigoProducto)
        {
            empresa = -1;
            codigoProducto = string.Empty;

            if (context.Parameters.Count > 1)
            {
                if (context.Parameters.Any(s => s.Id == "empresa") && int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa").Value, out int value))
                    empresa = value;

                if (context.Parameters.Any(s => s.Id == "producto") && !string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "producto").Value))
                    codigoProducto = context.Parameters.FirstOrDefault(s => s.Id == "producto").Value;
            }
            else
            {
                empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                codigoProducto = row.GetCell("CD_PRODUTO").Value;
            }
        }

        protected virtual void NotificarAutomatismo(IUnitOfWork uow, List<UbicacionPickingTipoOperacion> ubicacionesPicking)
        {
            if (_automatismoAutoStoreClientService.IsEnabled() && ubicacionesPicking.Count > 0)
            {
                var nuTransaccion = uow.GetTransactionNumber();
                var ubicacionesPickingPorKey = new Dictionary<string, UbicacionPickingTipoOperacion>();
                var ubicacionesAutomatismo = uow.AutomatismoRepository.GetUbicacionesPickingAutomatismo(ubicacionesPicking.Select(up => new UbicacionPickingProducto
                {
                    UbicacionSeparacion = up.Ubicacion.UbicacionSeparacion,
                    Empresa = up.Ubicacion.Empresa,
                    CodigoProducto = up.Ubicacion.CodigoProducto,
                }));

                if (ubicacionesAutomatismo.Count() > 0)
                {
                    foreach (var ubicacionPicking in ubicacionesAutomatismo)
                    {
                        var key = GetUbicacionPickingKey(ubicacionPicking);
                        ubicacionesPickingPorKey[key] = new UbicacionPickingTipoOperacion();
                    }

                    foreach (var ubicacionPicking in ubicacionesPicking)
                    {
                        var key = GetUbicacionPickingKey(ubicacionPicking);

                        if (ubicacionesPickingPorKey.ContainsKey(key))
                            ubicacionesPickingPorKey[key] = ubicacionPicking;
                    }

                    var ubicacionesNotificablesPorEmpresa = new Dictionary<int, List<UbicacionPickingAutomatismoRequest>>();

                    foreach (var ubicacionPicking in ubicacionesPickingPorKey.Values)
                    {
                        var empresa = ubicacionPicking.Ubicacion.Empresa;

                        if (!ubicacionesNotificablesPorEmpresa.ContainsKey(empresa))
                            ubicacionesNotificablesPorEmpresa[empresa] = new List<UbicacionPickingAutomatismoRequest>();

                        ubicacionesNotificablesPorEmpresa[empresa].Add(new UbicacionPickingAutomatismoRequest
                        {
                            Ubicacion = ubicacionPicking.Ubicacion.UbicacionSeparacion,
                            Producto = ubicacionPicking.Ubicacion.CodigoProducto,
                            TipoOperacion = ubicacionPicking.TipoOperacion,
                        });
                    }

                    foreach (var empresa in ubicacionesNotificablesPorEmpresa.Keys)
                    {
                        _automatismoAutoStoreClientService.SendUbicacionesPicking(new UbicacionesPickingAutomatismoRequest
                        {
                            DsReferencia = nuTransaccion.ToString(),
                            Empresa = empresa,
                            Ubicaciones = ubicacionesNotificablesPorEmpresa[empresa],
                        });
                    }
                }
            }
        }

        protected virtual string GetUbicacionPickingKey(UbicacionPickingProducto ubicacionPicking)
        {
            return $"{ubicacionPicking.Empresa}.{ubicacionPicking.CodigoProducto}.{ubicacionPicking.UbicacionSeparacion}";
        }

        protected virtual string GetUbicacionPickingKey(UbicacionPickingTipoOperacion ubicacionPicking)
        {
            return GetUbicacionPickingKey(ubicacionPicking.Ubicacion);
        }

        protected virtual List<SelectOption> SelectTipoSubdivisiónAutomatismo(IUnitOfWork uow)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            var dominios = uow.DominioRepository.GetDominios("UNDCAJAAUT");

            foreach (DominioDetalle dominio in dominios)
                opciones.Add(new SelectOption(dominio.Valor, dominio.Descripcion));

            return opciones;
        }

        #endregion
    }
}