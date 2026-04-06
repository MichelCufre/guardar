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
using WIS.Domain.Registro;
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
using WIS.Sorting;

namespace WIS.Application.Controllers.REG
{
    public class REG603PanelCodigosBarra : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ILogger<REG603PanelCodigosBarra> _logger;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IAutomatismoAutoStoreClientService _automatismoAutoStoreClientService;
        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REG603PanelCodigosBarra(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            ILogger<REG603PanelCodigosBarra> logger,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            IAutomatismoAutoStoreClientService automatismoAutoStoreClientService)
        {
            this.GridKeys = new List<string>
            {
                "CD_EMPRESA", "CD_PRODUTO", "CD_BARRAS"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_EMPRESA", SortDirection.Descending),
                new SortCommand("CD_PRODUTO", SortDirection.Descending),
                new SortCommand("CD_BARRAS", SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._automatismoAutoStoreClientService = automatismoAutoStoreClientService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsAddEnabled = true;
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = true;

            grid.AddOrUpdateColumn(new GridColumnSelect("TP_CODIGO_BARRAS", this.OptionSelectTipoCodigoBarra()));

            List<string> listaInsertables = new List<string> {
                    "CD_BARRAS",
                    "TP_CODIGO_BARRAS",
                    "NU_PRIORIDADE_USO"
            };

            if (query.FetchContext.Parameters.Count > 1)
            {
                grid.SetInsertableColumns(listaInsertables);
            }
            else
            {
                listaInsertables.Add("CD_EMPRESA");
                listaInsertables.Add("CD_PRODUTO");

                grid.SetInsertableColumns(listaInsertables);
            }

            query.AddLink("CD_PRODUTO", "registro/REG009", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_PRODUTO", "producto"), new GridColumnLinkMapping("CD_EMPRESA", "empresa") });
            query.AddLink("CD_EMPRESA", "registro/REG100", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            CodigoBarrasProductosQuery dbQuery;

            if (query.Parameters.Count > 1)
            {

                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!query.Parameters.Any(x => x.Id == "producto"))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idProducto = query.Parameters.FirstOrDefault(s => s.Id == "producto")?.Value;
                dbQuery = new CodigoBarrasProductosQuery(idEmpresa, idProducto.ToString());

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

                grid.GetColumn("DS_PRODUTO").Hidden = true;
                grid.GetColumn("CD_PRODUTO").Hidden = true;
                grid.GetColumn("CD_EMPRESA").Hidden = true;
                grid.GetColumn("NM_EMPRESA").Hidden = true;

                Empresa empresa = uow.EmpresaRepository.GetEmpresa(idEmpresa);
                string descProducto = uow.ProductoRepository.GetDescripcion(idEmpresa, idProducto.ToString());

                query.AddParameter("REG603_CD_EMPRESA", idEmpresa.ToString());
                query.AddParameter("REG603_NM_EMPRESA", empresa.Nombre);
                query.AddParameter("REG603_CD_PRODUCTO", idProducto.ToString());
                query.AddParameter("REG603_DS_PRODUCTO", descProducto);
            }
            else if (query.Parameters.Count > 0)
            {
                if (!query.Parameters.Any(s => s.Id == "empresa"))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idEmpresa = query.Parameters.Any(s => s.Id == "empresa") ? query.Parameters.FirstOrDefault(s => s.Id == "empresa").Value : string.Empty;

                if (!string.IsNullOrEmpty(idEmpresa))
                {
                    dbQuery = new CodigoBarrasProductosQuery(int.Parse(idEmpresa));

                    uow.HandleQuery(dbQuery);

                    grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);
                    grid.GetColumn("CD_EMPRESA").Hidden = true;
                    grid.GetColumn("NM_EMPRESA").Hidden = true;

                    Empresa empresa = uow.EmpresaRepository.GetEmpresa(int.Parse(idEmpresa));

                    query.AddParameter("REG603_CD_EMPRESA", idEmpresa.ToString());
                    query.AddParameter("REG603_NM_EMPRESA", empresa.Nombre);
                }
            }
            else
            {
                dbQuery = new CodigoBarrasProductosQuery();

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);
            }

            grid.SetEditableCells(new List<string>{
                "CD_BARRAS",
                "TP_CODIGO_BARRAS",
                "NU_PRIORIDADE_USO"
            });

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            CodigoBarrasProductosQuery dbQuery = new CodigoBarrasProductosQuery();

            if (query.Parameters.Count > 1)
            {

                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!query.Parameters.Any(s => s.Id == "producto"))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idProducto = query.Parameters.FirstOrDefault(s => s.Id == "producto").Value;

                dbQuery = new CodigoBarrasProductosQuery(idEmpresa, idProducto);

                uow.HandleQuery(dbQuery);

            }
            else if (query.Parameters.Count > 0)
            {
                if (!query.Parameters.Any(s => s.Id == "empresa"))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idEmpresa = query.Parameters.Any(s => s.Id == "empresa") ? query.Parameters.FirstOrDefault(s => s.Id == "empresa").Value : string.Empty;


                if (!string.IsNullOrEmpty(idEmpresa))
                {
                    dbQuery = new CodigoBarrasProductosQuery(int.Parse(idEmpresa));

                    uow.HandleQuery(dbQuery);

                }
            }
            else
            {
                uow.HandleQuery(dbQuery);
            }

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            CodigoBarrasProductosQuery dbQuery = new CodigoBarrasProductosQuery();

            if (query.Parameters.Count > 1)
            {

                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!query.Parameters.Any(s => s.Id == "producto"))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idProducto = query.Parameters.FirstOrDefault(s => s.Id == "producto").Value;

                dbQuery = new CodigoBarrasProductosQuery(idEmpresa, idProducto);

            }
            else if (query.Parameters.Count > 0)
            {
                if (!query.Parameters.Any(s => s.Id == "empresa"))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idEmpresa = query.Parameters.Any(s => s.Id == "empresa") ? query.Parameters.FirstOrDefault(s => s.Id == "empresa").Value : string.Empty;


                if (!string.IsNullOrEmpty(idEmpresa))
                {
                    dbQuery = new CodigoBarrasProductosQuery(int.Parse(idEmpresa));
                }
            }

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext query)
        {
            switch (query.ColumnId)
            {
                case "CD_EMPRESA":
                    return this.SearchEmpresa(grid, row, query);
                case "CD_PRODUTO":
                    return this.SearchProduto(grid, row, query);
            }

            return new List<SelectOption>();
        }

        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                if (grid.Rows.Any())
                {
                    var codigosBarras = new List<CodigoBarrasTipoOperacion>();

                    uow.CreateTransactionNumber(this._identity.Application);
                    uow.BeginTransaction();

                    var registroModificacionCB = new RegistroModificacionProductoCodigoBarras(uow, this._identity.UserId, this._identity.Application);

                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    foreach (var row in grid.Rows)
                    {
                        if (row.IsNew)
                        {
                            var codigoBarras = this.CrearCodigoDeBarra(uow, row, query);
                            registroModificacionCB.RegistrarCodigoBarra(codigoBarras);
                            codigosBarras.Add(new CodigoBarrasTipoOperacion(codigoBarras, TipoOperacionDb.Alta));
                        }
                        else if (row.IsDeleted)
                        {
                            // rows delete
                            var codigoBarras = this.DeleteCodigoBarra(uow, row, query);
                            codigosBarras.Add(new CodigoBarrasTipoOperacion(codigoBarras, TipoOperacionDb.Baja));
                        }
                        else
                        {
                            // rows editadas
                            var codigoBarras = this.UpdateCodigoBarra(uow, row, query);
                            ProductoCodigoBarra codigoBarrasOld = GetCodigoBarraOldValue(uow, row, query);

                            if (row.GetCell("CD_BARRAS").Value == row.GetCell("CD_BARRAS").Old)
                            {
                                registroModificacionCB.ModificarCodigoBarra(codigoBarras);
                            }
                            else
                            {
                                registroModificacionCB.ModificarCodigoBarra(codigoBarras, row.GetCell("CD_BARRAS").Old, int.Parse(row.GetCell("CD_EMPRESA").Value));
                            }

                            codigosBarras.Add(new CodigoBarrasTipoOperacion(codigoBarrasOld, TipoOperacionDb.Baja));
                            codigosBarras.Add(new CodigoBarrasTipoOperacion(codigoBarras, TipoOperacionDb.Alta));
                        }
                    }

                    uow.SaveChanges();
                    uow.Commit();

                    NotificarAutomatismo(uow, codigosBarras);
                }

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (AutomatismoException ex)
            {
                query.AddErrorNotification(ex.Message);
            }
            catch (ExpectedException ex)
            {
                // logger.Warn(ex, "GridCommit");
                query.AddErrorNotification(ex.Message);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "REG603GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }

        public virtual void NotificarAutomatismo(IUnitOfWork uow, List<CodigoBarrasTipoOperacion> codigosBarras)
        {
            if (_automatismoAutoStoreClientService.IsEnabled() && codigosBarras.Count > 0)
            {
                var nuTransaccion = codigosBarras.First().CodigoBarra.NumeroTransaccion;
                var codigoBarrasPorCodigo = new Dictionary<string, CodigoBarrasTipoOperacion>();
                var codigosAutomatismo = uow.AutomatismoRepository.GetCodigosBarrasAutomatismo(codigosBarras.Select(cb => new CodigoBarras
                {
                    Codigo = cb.CodigoBarra.CodigoBarra,
                    Empresa = cb.CodigoBarra.IdEmpresa,
					Producto = cb.CodigoBarra.IdProducto,
				}));

                if (codigosAutomatismo.Count() > 0)
                {
                    foreach (var codigoBarras in codigosAutomatismo)
                    {
                        codigoBarrasPorCodigo[codigoBarras.Codigo] = new CodigoBarrasTipoOperacion();
                    }

                    foreach (var codigoBarras in codigosBarras)
                    {
                        var codigo = codigoBarras.CodigoBarra.CodigoBarra;

                        if (codigoBarrasPorCodigo.ContainsKey(codigo))
                            codigoBarrasPorCodigo[codigo] = codigoBarras;
                    }

                    var codigosNotificablesPorEmpresa = new Dictionary<int, List<CodigoBarraAutomatismoRequest>>();

                    foreach (var codigoBarras in codigoBarrasPorCodigo.Values)
                    {
                        var empresa = codigoBarras.CodigoBarra.IdEmpresa;

                        if (!codigosNotificablesPorEmpresa.ContainsKey(empresa))
                            codigosNotificablesPorEmpresa[empresa] = new List<CodigoBarraAutomatismoRequest>();

                        codigosNotificablesPorEmpresa[empresa].Add(new CodigoBarraAutomatismoRequest
                        {
                            Codigo = codigoBarras.CodigoBarra.CodigoBarra,
                            Producto = codigoBarras.CodigoBarra.IdProducto,
                            TipoOperacion = codigoBarras.TipoOperacion,
                        });
                    }

                    foreach (var empresa in codigosNotificablesPorEmpresa.Keys)
                    {
                        _automatismoAutoStoreClientService.SendCodigosBarras(new CodigosBarrasAutomatismoRequest
                        {
                            DsReferencia = nuTransaccion.ToString(),
                            Empresa = empresa,
                            CodigosDeBarras = codigosNotificablesPorEmpresa[empresa],
                        });
                    }
                }
            }
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoProductoCodigoBarrasValidationModule(uow), grid, row, context);
        }

        public virtual ProductoCodigoBarra CrearCodigoDeBarra(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            var nuevoCodigo = new ProductoCodigoBarra();

            if (query.Parameters.Count > 1)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "empresa").Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!query.Parameters.Any(s => s.Id == "producto"))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                var idProducto = query.Parameters.FirstOrDefault(s => s.Id == "producto").Value;
                var empresa = uow.EmpresaRepository.GetEmpresa(idEmpresa);

                nuevoCodigo.IdEmpresa = idEmpresa;
                nuevoCodigo.IdProducto = idProducto;

            }
            else if (query.Parameters.Count > 0)
            {
                nuevoCodigo.IdEmpresa = query.Parameters.Any(s => s.Id == "empresa") ? int.Parse(query.Parameters.FirstOrDefault(s => s.Id == "empresa").Value) : int.Parse(row.GetCell("CD_EMPRESA").Value);
                nuevoCodigo.IdProducto = query.Parameters.Any(s => s.Id == "producto") ? query.Parameters.FirstOrDefault(s => s.Id == "producto").Value : row.GetCell("CD_PRODUTO").Value;
            }
            else
            {
                nuevoCodigo.IdEmpresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                nuevoCodigo.IdProducto = row.GetCell("CD_PRODUTO").Value;
            }

            var codigoBarra = row.GetCell("CD_BARRAS").Value;

            nuevoCodigo.IdTipoCodigoBarra = int.Parse(row.GetCell("TP_CODIGO_BARRAS").Value);
            nuevoCodigo.CodigoBarra = codigoBarra;
            nuevoCodigo.NumPrioridadeUso = short.Parse(row.GetCell("NU_PRIORIDADE_USO").Value);
            nuevoCodigo.FechaInsercion = DateTime.Now;
            nuevoCodigo.TipoCodigoDeBarra = uow.ProductoCodigoBarraRepository.GetProductoCodigoBarraTipo(nuevoCodigo.IdTipoCodigoBarra);
            nuevoCodigo.NumeroTransaccion = uow.GetTransactionNumber();
            nuevoCodigo.NumeroTransaccionDelete = null;

            return nuevoCodigo;
        }

        public virtual ProductoCodigoBarra UpdateCodigoBarra(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            var viejoCodigoBarra = row.GetCell("CD_BARRAS").Old;
            var nuevoCodigoBarra = row.GetCell("CD_BARRAS").Value;

            var idEmpresa = query.Parameters.Any(s => s.Id == "empresa") ? query.Parameters.FirstOrDefault(s => s.Id == "empresa").Value : row.GetCell("CD_EMPRESA").Value;
            var idProducto = query.Parameters.Any(s => s.Id == "producto") ? query.Parameters.FirstOrDefault(s => s.Id == "producto").Value : row.GetCell("CD_PRODUTO").Value;

            var codigoBarraUpd = new ProductoCodigoBarra();

            if (viejoCodigoBarra == nuevoCodigoBarra)
            {
                codigoBarraUpd = uow.ProductoCodigoBarraRepository.GetProductoCodigoBarra(viejoCodigoBarra, int.Parse(idEmpresa));

                codigoBarraUpd.NumPrioridadeUso = short.Parse(row.GetCell("NU_PRIORIDADE_USO").Value);
                codigoBarraUpd.IdTipoCodigoBarra = int.Parse(row.GetCell("TP_CODIGO_BARRAS").Value);
                codigoBarraUpd.TipoCodigoDeBarra = uow.ProductoCodigoBarraRepository.GetProductoCodigoBarraTipo(int.Parse(row.GetCell("TP_CODIGO_BARRAS").Value));
                codigoBarraUpd.FechaModificacion = DateTime.Now;
            }
            else
            {
                if (uow.ProductoCodigoBarraRepository.ExisteCodigoBarra(viejoCodigoBarra, int.Parse(idEmpresa)))
                {
                    codigoBarraUpd = uow.ProductoCodigoBarraRepository.GetProductoCodigoBarra(viejoCodigoBarra, int.Parse(idEmpresa));

                    if (!uow.ProductoCodigoBarraRepository.ExisteCodigoBarra(nuevoCodigoBarra, int.Parse(idEmpresa)))
                    {
                        codigoBarraUpd.CodigoBarra = nuevoCodigoBarra;
                        codigoBarraUpd.NumPrioridadeUso = short.Parse(row.GetCell("NU_PRIORIDADE_USO").Value);
                        codigoBarraUpd.IdTipoCodigoBarra = int.Parse(row.GetCell("TP_CODIGO_BARRAS").Value);
                        codigoBarraUpd.TipoCodigoDeBarra = uow.ProductoCodigoBarraRepository.GetProductoCodigoBarraTipo(int.Parse(row.GetCell("TP_CODIGO_BARRAS").Value));
                        codigoBarraUpd.FechaModificacion = DateTime.Now;
                    }
                }
            }

            codigoBarraUpd.NumeroTransaccion = uow.GetTransactionNumber();
            codigoBarraUpd.NumeroTransaccionDelete = null;

            return codigoBarraUpd;
        }

        public virtual ProductoCodigoBarra DeleteCodigoBarra(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            var idCodigoBarra = row.GetCell("CD_BARRAS").Value;
            var idEmpresa = query.Parameters.Any(s => s.Id == "empresa") ? query.Parameters.FirstOrDefault(s => s.Id == "empresa").Value : row.GetCell("CD_EMPRESA").Value;
            var idProducto = query.Parameters.Any(s => s.Id == "producto") ? query.Parameters.FirstOrDefault(s => s.Id == "producto").Value : row.GetCell("CD_PRODUTO").Value;
            var empresa = int.Parse(idEmpresa);
            var nuTransaccion = uow.GetTransactionNumber();

            if (uow.ProductoCodigoBarraRepository.ExisteCodigoBarra(idCodigoBarra, empresa))
            {
                var codigoBarra = uow.ProductoCodigoBarraRepository.GetProductoCodigoBarra(idCodigoBarra, empresa);
                codigoBarra.NumeroTransaccion = uow.GetTransactionNumber();
                codigoBarra.NumeroTransaccionDelete = codigoBarra.NumeroTransaccion;
                uow.ProductoCodigoBarraRepository.UpdateCodigoBarras(codigoBarra);
                uow.SaveChanges();
                uow.ProductoCodigoBarraRepository.DeleteCodigoBarra(idCodigoBarra, empresa);
            }
            else
            {
                throw new EntityNotFoundException("REG603_Sec0_Error_Er001_CodigoBarrasNoExisteEliminar");
            }

            return new ProductoCodigoBarra
            {
                CodigoBarra = idCodigoBarra,
                IdEmpresa = empresa,
                IdProducto = idProducto,
                NumeroTransaccion = nuTransaccion,
                NumeroTransaccionDelete = nuTransaccion,
            };
        }

        public virtual List<SelectOption> OptionSelectTipoCodigoBarra()
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<ProductoCodigoBarraTipo> listaTiposCodigoBarras = uow.ProductoCodigoBarraRepository.GetTiposCodigosBarras();

            foreach (var tipo in listaTiposCodigoBarras)
            {
                opciones.Add(new SelectOption(tipo.Id.ToString(), $"{tipo.Id.ToString()} - {tipo.Descripcion}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchEmpresa(Grid grid, GridRow row, GridSelectSearchContext query)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Empresa> empresasAsignadasUsuario = uow.EmpresaRepository.GetByNombreOrCodePartialForUsuario(query.SearchValue, this._identity.UserId);

            foreach (var emp in empresasAsignadasUsuario)
            {
                opciones.Add(new SelectOption(emp.Id.ToString(), $"{emp.Id} - {emp.Nombre}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchProduto(Grid grid, GridRow row, GridSelectSearchContext query)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Producto> productos = new List<Producto>();

            if (query.Parameters.Count > 0)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "empresa").Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(idEmpresa, query.SearchValue);
            }
            else
            {
                if (!string.IsNullOrEmpty(row.GetCell("CD_EMPRESA").Value))
                {
                    productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(int.Parse(row.GetCell("CD_EMPRESA").Value), query.SearchValue);
                }
                else
                {
                    row.GetCell("CD_EMPRESA").SetError(new ComponentError("General_Sec0_Error_Error25", new List<string>()));
                }
            }

            foreach (var prod in productos)
            {
                opciones.Add(new SelectOption(prod.Codigo, $"{prod.Codigo} - {prod.Descripcion}"));
            }

            return opciones;
        }

        public virtual ProductoCodigoBarra GetCodigoBarraOldValue(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            ProductoCodigoBarra codBarr = new ProductoCodigoBarra();
            string idCodigoBarra = row.GetCell("CD_BARRAS").Old;
            string idEmpresa = query.Parameters.Any(s => s.Id == "empresa") ? query.Parameters.FirstOrDefault(s => s.Id == "empresa").Value : row.GetCell("CD_EMPRESA").Old;
            string idProducto = query.Parameters.Any(s => s.Id == "producto") ? query.Parameters.FirstOrDefault(s => s.Id == "producto").Value : row.GetCell("CD_PRODUTO").Old;

            codBarr.IdEmpresa = int.Parse(idEmpresa);
            codBarr.CodigoBarra = idCodigoBarra;
            codBarr.IdProducto = idProducto;

            return codBarr;
        }
    }
}