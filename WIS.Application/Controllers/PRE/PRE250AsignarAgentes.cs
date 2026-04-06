using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.Liberacion;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRE
{
	public class PRE250AsignarAgentes : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE250AsignarAgentes(IUnitOfWorkFactory uowFactory, IIdentityService identity, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter, IGridValidationService gridValidationService)
        {
            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_EMPRESA",SortDirection.Ascending),
                new SortCommand("CD_CLIENTE",SortDirection.Ascending)
            };

            this.GridKeys = new List<string>
            {
                "CD_CLIENTE","CD_EMPRESA"
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            _gridValidationService = gridValidationService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {

            switch (grid.Id)
            {
                case "PRE250AsignarAgentes_grid_1":
                    grid.MenuItems = new List<IGridItem> { new GridButton("btnAgregarAgente", "PRE250_grid1_btn_AddCliente") };
                    break;

                case "PRE250AsignarAgentes_grid_2":
                    context.IsAddEnabled = false;
                    context.IsCommitEnabled = true;
                    context.IsRemoveEnabled = false;
                    context.IsEditingEnabled = true;
                    context.IsRollbackEnabled = true;
                    grid.MenuItems = new List<IGridItem> { new GridButton("btnQuitarAgente", "PRE250_grid1_btn_RemoveCliente") };
                    break;
            }
            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            switch (grid.Id)
            {
                case "PRE250AsignarAgentes_grid_1":
                    return this.FetchRowsClientesDisponibles(uow, grid, context);
                case "PRE250AsignarAgentes_grid_2":
                    return this.FetchRowsClientesSeleccionados(uow, grid, context);
            }

            return grid;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            if (grid.Id == "PRE250AsignarAgentes_grid_2")
            {
                if (grid.HasNewDuplicates(new List<string>() { "CD_CLIENTE", "CD_EMPRESA" }))
                    throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                if (int.TryParse(context.Parameters.Find(s => s.Id == "nuRegla")?.Value, out int nuRegla))
                {
                    using var uow = this._uowFactory.GetUnitOfWork();

                    grid.Rows.ForEach(row =>
                    {
                        short? nuOrden = null;
                        if (short.TryParse(row.GetCell("NU_ORDEN").Value, out short n))
                            nuOrden = n;

                        string cdCliente = row.GetCell("CD_CLIENTE").Value;
                        int cdEmpresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                        var reglaCli = uow.LiberacionRepository.GetReglaCliente(nuRegla, cdEmpresa, cdCliente);
                        if (reglaCli != null)
                        {
                            reglaCli.NuOrden = nuOrden;
                            uow.LiberacionRepository.UpdateReglaCliente(reglaCli);
                        }
                        else
                            throw new InvalidOperationException("PRE250_frm1_lbl_ERR_UPD");
                    });

                    uow.SaveChanges();
                    context.AddSuccessNotification("PRE120_msg_Sucess_UpdateCliente");
                }
                else
                    throw new InvalidOperationException("PRE250_frm1_lbl_ERR_UPD");
            }

            return grid;
        }
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            return this._gridValidationService.Validate(new PRE250AsignarAgentesGridValidationModule(uow), grid, row, context);
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (context.GridId == "PRE250AsignarAgentes_grid_1" && context.ButtonId == "btnAgregarAgente")
            {
                this.ProcesarAgregar(uow, context);
            }
            else if (context.GridId == "PRE250AsignarAgentes_grid_2" && context.ButtonId == "btnQuitarAgente")
            {
                this.ProcesarQuitar(uow, context);
            }

            return context;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (grid.Id == "PRE250AsignarAgentes_grid_1")
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "nuRegla")?.Value, out int nuRegla))
                    throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "empresa")?.Value, out int empresa))
                    throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

                string tpAgente = context.Parameters.FirstOrDefault(x => x.Id == "tpAgente")?.Value;

                var dbQuery = new ReglaClientesDisponiblesQuery(nuRegla, empresa, tpAgente);
                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application +"-"+ DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
            }
            else
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "nuRegla")?.Value, out int nuRegla))
                    throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

                var dbQuery = new ReglaClientesSeleccionadosQuery(nuRegla);
                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application +"-"+ DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);

            }
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (grid.Id == "PRE250AsignarAgentes_grid_1")
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(x => x.Id == "nuRegla")?.Value, out int nuRegla))
                    throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

                if (!int.TryParse(query.Parameters.FirstOrDefault(x => x.Id == "empresa")?.Value, out int empresa))
                    throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

                string tpAgente = query.Parameters.FirstOrDefault(x => x.Id == "tpAgente")?.Value;

                var dbQuery = new ReglaClientesDisponiblesQuery(nuRegla, empresa, tpAgente);

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(x => x.Id == "nuRegla")?.Value, out int nuRegla))
                    throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

                var dbQuery = new ReglaClientesSeleccionadosQuery(nuRegla);

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
        }


        #region AUX

        protected virtual Grid FetchRowsClientesDisponibles(IUnitOfWork uow, Grid grid, GridFetchContext context)
        {
            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "nuRegla")?.Value, out int nuRegla))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "empresa")?.Value, out int empresa))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            string tpAgente = context.Parameters.FirstOrDefault(x => x.Id == "tpAgente")?.Value;

            var dbQuery = new ReglaClientesDisponiblesQuery(nuRegla, empresa, tpAgente);

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }
        protected virtual Grid FetchRowsClientesSeleccionados(IUnitOfWork uow, Grid grid, GridFetchContext context)
        {
            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "nuRegla")?.Value, out int nuRegla))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            var dbQuery = new ReglaClientesSeleccionadosQuery(nuRegla);

            uow.HandleQuery(dbQuery);

            SortCommand defaultSort = new SortCommand("NU_ORDEN", SortDirection.Ascending);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, new List<string>() { "NU_REGLA", "CD_CLIENTE", "CD_EMPRESA" });
            grid.SetEditableCells(new List<string> { "NU_ORDEN" });

            return grid;
        }

        public virtual void ProcesarAgregar(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            int nuRegla = int.Parse(context.GetParameter("nuRegla"));

            uow.CreateTransactionNumber("Asignar agentes: ProcesarAgregar");
            uow.BeginTransaction();

            try
            {
                var regla = uow.LiberacionRepository.GetReglaLiberacion(nuRegla, false);

                if (regla == null)
                    throw new EntityNotFoundException("Regla no existe");

                List<ReglaCliente> reglas = new List<ReglaCliente>();

                foreach (var item in GetSelectedAdd(uow, regla, context))
                {
                    reglas.Add(new ReglaCliente
                    {
                        NuRegla = nuRegla,
                        Cliente = item[0],
                        Empesa = int.Parse(item[1]),
                        NuOrden = null,
                    });
                }

                var mantLiberacion = new MantenimientoLiberacion(uow, this._identity.UserId, this._identity.Application);
                mantLiberacion.AgregarReglaClientes(reglas);
                uow.SaveChanges();


                regla.ValorVidaUtil = (short?)uow.LiberacionRepository.ObtenerMenorValorVidaUtil(regla.NuRegla);
                uow.LiberacionRepository.UpdateReglaLiberacion(regla);
                uow.SaveChanges();

                uow.Commit();
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw;
            }
        }
        public virtual void ProcesarQuitar(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            int nuRegla = int.Parse(context.GetParameter("nuRegla"));

            uow.CreateTransactionNumber("Quitar agentes: ProcesarQuitar");
            uow.BeginTransaction();

            try
            {
                var regla = uow.LiberacionRepository.GetReglaLiberacion(nuRegla, false);
                if (regla == null)
                    throw new EntityNotFoundException("Regla no existe");

                List<ReglaCliente> reglas = new List<ReglaCliente>();

                foreach (var item in GetSelectedRemove(uow, regla, context))
                {
                    reglas.Add(new ReglaCliente
                    {
                        NuRegla = nuRegla,
                        Cliente = item[1],
                        Empesa = int.Parse(item[2]),
                        NuOrden = null,
                    });
                }

                var mantLiberacion = new MantenimientoLiberacion(uow, this._identity.UserId, this._identity.Application);
                mantLiberacion.DesasociarReglaClientes(reglas);
                uow.SaveChanges();

                regla.ValorVidaUtil = (short?)uow.LiberacionRepository.ObtenerMenorValorVidaUtil(regla.NuRegla);
                uow.LiberacionRepository.UpdateReglaLiberacion(regla);
                uow.SaveChanges();

                uow.Commit();
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw;
            }
        }

        public virtual List<string[]> GetSelectedAdd(IUnitOfWork uow, ReglaLiberacion regla, GridMenuItemActionContext context)
        {
            var dbQuery = new ReglaClientesDisponiblesQuery(regla.NuRegla, regla.CdEmpresa, regla.TpAgente);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys);

            return dbQuery.GetSelectedKeys(context.Selection.Keys);
        }
        public virtual List<string[]> GetSelectedRemove(IUnitOfWork uow, ReglaLiberacion regla, GridMenuItemActionContext context)
        {
            var dbQuery = new ReglaClientesSeleccionadosQuery(regla.NuRegla);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys);

            return dbQuery.GetSelectedKeys(context.Selection.Keys);
        }

        #endregion
    }
}
