using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Domain.General;
using WIS.Domain.Impresiones;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Persistence.Database;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.REG
{
    public class REG100AsociarCodigoMultidato : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REG100AsociarCodigoMultidato(
            ISessionAccessor session,
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "CD_EMPRESA", "CD_CODIGO_MULTIDATO",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_EMPRESA", SortDirection.Descending)
            };

            this._session = session;
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
        }

        #region FORM

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var codigoEmpresa = int.Parse(query.GetParameter("empresa"));
            var empresa = uow.EmpresaRepository.GetEmpresa(codigoEmpresa);

            form.GetField("codigoEmpresa").Value = empresa.Id.ToString();
            form.GetField("nombreEmpresa").Value = empresa.Nombre.ToString();

            var entidad = string.Format("{0}_{1}", ParamManager.PARAM_EMPR, codigoEmpresa);

            var parametro = uow.ParametroRepository.GetParametroConfiguracion(ParamManager.CODIGO_MULTIDATO_URL_API, entidad);

            if (parametro != null)
                form.GetField("url").Value = parametro.Valor;

            return form;
        }

        #endregion

        #region GRID

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            if (grid.Id == "REG100AsociarCodigoEmpresa_grid_1")
                grid.MenuItems.Add(new GridButton("btnAgregar", "EVT040_Sec0_btn_AgregarSeleccion"));

            if (grid.Id == "REG100AsociarCodigoEmpresa_grid_2")
            {
                grid.MenuItems.Add(new GridButton("btnQuitar", "EVT040_Sec0_btn_QuitarSeleccion"));

                grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
                {
                    new GridButton("btnEditar", "General_Sec0_btn_Editar", "far fa-edit"),
                }));

                query.IsAddEnabled = false;
                query.IsEditingEnabled = true;
                query.IsRemoveEnabled = false;
            }

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int empresa = int.Parse(query.GetParameter("empresa"));

            var dbQuery = new AsociarCodigoMultidatoQuery(grid.Id == "REG100AsociarCodigoEmpresa_grid_1" ? "N" : "S", empresa);

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            if (grid.Id == "REG100AsociarCodigoEmpresa_grid_2")
            {
                grid.SetEditableCells(new List<string> { "FL_HABILITADO" });
                ObtenerCodigosMultidatoConDetalles(grid, uow, query);
            }

            return grid;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("REG100 Editar Asociación Código Multidato");
            var nuTransaccion = uow.GetTransactionNumber();

            var empresa = int.Parse(query.GetParameter("empresa"));

            try
            {
                if (grid.Rows.Any())
                {
                    foreach (var row in grid.Rows)
                    {
                        if (row.IsModified)
                        {
                            var codigoMultidato = row.GetCell("CD_CODIGO_MULTIDATO").Value;
                            var relacionCodigoEmpresa = uow.CodigoMultidatoRepository.GetCodigoMultidatoEmpresa(empresa, codigoMultidato);

                            relacionCodigoEmpresa.FechaModificacion = DateTime.Now;
                            relacionCodigoEmpresa.NumeroTransaccion = nuTransaccion;
                            relacionCodigoEmpresa.Habilitado = row.GetCell("FL_HABILITADO").Value;

                            uow.CodigoMultidatoRepository.UpdateCodigoMultidatoEmpresa(relacionCodigoEmpresa);
                            uow.SaveChanges();
                        }
                    }
                }

                uow.SaveChanges();
                uow.Commit();

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                uow.Rollback();
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int empresa = int.Parse(query.GetParameter("empresa"));

            var dbQuery = new AsociarCodigoMultidatoQuery(grid.Id == "REG100AsociarCodigoEmpresa_grid_1" ? "N" : "S", empresa);

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

            int empresa = int.Parse(query.GetParameter("empresa"));

            var dbQuery = new AsociarCodigoMultidatoQuery(grid.Id == "REG100AsociarCodigoEmpresa_grid_1" ? "N" : "S", empresa);

            uow.HandleQuery(dbQuery);

            query.FileName = "CodigosMultidatoEmpresa" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("REG100 Asociar Código Multidato");
            var nuTransaccion = uow.GetTransactionNumber();

            try
            {
                if (int.TryParse(context.GetParameter("empresa"), out int empresa))
                {

                    if (context.GridId == "REG100AsociarCodigoEmpresa_grid_1")
                    {
                        List<string> codigosMultidato = this.GetSelectedCodigosMultidato(uow, context, empresa, "N");

                        foreach (var codigo in codigosMultidato)
                        {
                            var asociacionCodigoEmpresa = new CodigoMultidatoEmpresa
                            {
                                CodigoEmpresa = empresa,
                                CodigoMultidato = codigo,
                                Habilitado = "S",
                                FechaAlta = DateTime.Now,
                                NumeroTransaccion = nuTransaccion,
                            };

                            uow.CodigoMultidatoRepository.AddCodigoMultidatoEmpresa(asociacionCodigoEmpresa);

                            var detalles = uow.CodigoMultidatoRepository.GetDetallesCodigoMultidatoEmpresaPorDefecto(empresa, codigo);

                            foreach (var detalle in detalles)
                            {
                                detalle.NumeroTransaccion = nuTransaccion;
                                uow.CodigoMultidatoRepository.AddDetalleCodigoMultidatoEmpresa(detalle);
                            }
                        }
                    }
                    else
                    {
                        List<string> codigosMultidato = this.GetSelectedCodigosMultidato(uow, context, empresa, "S");

                        foreach (var codigo in codigosMultidato)
                        {
                            var detalles = uow.CodigoMultidatoRepository.GetDetallesCodigoMultidatoEmpresa(empresa, codigo);

                            foreach (var detalle in detalles)
                            {
                                detalle.NumeroTransaccion = nuTransaccion;
                                detalle.NumeroTransaccionDelete = nuTransaccion;
                                detalle.FechaModificacion = DateTime.Now;

                                uow.CodigoMultidatoRepository.UpdateDetalleCodigoMultidatoEmpresa(detalle);
                                uow.SaveChanges();

                                uow.CodigoMultidatoRepository.DeleteDetalleCodigoMultidatoEmpresa(detalle);
                            }

                            uow.SaveChanges();

                            var codigoMultidatoEmpresa = uow.CodigoMultidatoRepository.GetCodigoMultidatoEmpresa(empresa, codigo);

                            codigoMultidatoEmpresa.FechaModificacion = DateTime.Now;
                            codigoMultidatoEmpresa.NumeroTransaccion = nuTransaccion;
                            codigoMultidatoEmpresa.NumeroTransaccionDelete = nuTransaccion;

                            uow.CodigoMultidatoRepository.UpdateCodigoMultidatoEmpresa(codigoMultidatoEmpresa);
                            uow.SaveChanges();

                            uow.CodigoMultidatoRepository.RemoveCodigoMultidatoEmpresa(codigoMultidatoEmpresa);
                        }
                    }

                    uow.SaveChanges();
                    uow.Commit();
                }
            }
            catch (Exception ex)
            {
                uow.Rollback();
            }

            return context;
        }

        #endregion

        #region AUX

        public virtual void ObtenerCodigosMultidatoConDetalles(Grid grid, IUnitOfWork uow, GridFetchContext query)
        {
            var empresa = int.Parse(query.GetParameter("empresa"));
            var codigosMultidatoConDetalles = new List<string>();

            foreach (var row in grid.Rows)
            {
                var codigoMultidato = row.GetCell("CD_CODIGO_MULTIDATO").Value;
                var detalleAsociado = uow.CodigoMultidatoRepository.ExisteDetalleCodigoMultidatoEmpresa(empresa, codigoMultidato);

                if (detalleAsociado)
                    codigosMultidatoConDetalles.Add(codigoMultidato);
            }

            if (codigosMultidatoConDetalles.Any())
            {
                var codigos = string.Join("$", codigosMultidatoConDetalles);

                query.Parameters.Add(new ComponentParameter
                {
                    Id = "detallesAsociados",
                    Value = codigos
                });
            }
        }

        public virtual List<string> GetSelectedCodigosMultidato(IUnitOfWork uow, GridMenuItemActionContext context, int empresa, string asociados)
        {
            var keys = new List<string>();

            foreach (var key in context.Selection.Keys)
            {
                var codigoMultidato = key.Split('$')[1];
                keys.Add(codigoMultidato);
            }

            var dbQuery = new AsociarCodigoMultidatoQuery(asociados, empresa);

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(keys);

            return keys;
        }

        #endregion
    }
}
