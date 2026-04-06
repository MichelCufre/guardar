using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Expedicion;
using WIS.Domain.Eventos;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.EVT
{
    public class EVT040DestinatariosInstanciaGrupos : AppController
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

        public EVT040DestinatariosInstanciaGrupos(
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
                "NU_CONTACTO_GRUPO",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_CONTACTO_GRUPO", SortDirection.Descending)
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

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            if (grid.Id == "EVT040DestinatariosGrupos_grid_1")
                grid.MenuItems.Add(new GridButton("btnAgregar", "EVT040_Sec0_btn_AgregarSeleccion"));
            else if (grid.Id == "EVT040DestinatariosGrupos_grid_2")
                grid.MenuItems.Add(new GridButton("btnQuitar", "EVT040_Sec0_btn_QuitarSeleccion"));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int numeroInstancia = int.Parse(context.GetParameter("instancia"));

            var dbQuery = new GruposInstanciaQuery(grid.Id == "EVT040DestinatariosGrupos_grid_1" ? "AGREGAR" : "", numeroInstancia);

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int numeroInstancia = int.Parse(context.GetParameter("instancia"));

            var dbQuery = new GruposInstanciaQuery(grid.Id == "EVT040DestinatariosGrupos_grid_1" ? "AGREGAR" : "", numeroInstancia);

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int numeroInstancia = int.Parse(context.GetParameter("instancia"));

            var dbQuery = new GruposInstanciaQuery(grid.Id == "EVT040DestinatariosGrupos_grid_1" ? "AGREGAR" : "", numeroInstancia);

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("EVT040 Modificar Grupos Instancia");
            var nuTransaccion = uow.GetTransactionNumber();

            try
            {
                if (int.TryParse(context.GetParameter("instancia"), out int NuInstancia))
                {

                    if (context.GridId == "EVT040DestinatariosGrupos_grid_1")
                    {
                        List<int> grupos = this.GetSelectedGrupos(uow, "AGREGAR", NuInstancia, context);

                        foreach (var grupo in grupos)
                        {
                            DestinatarioInstancia obj = new DestinatarioInstancia
                            {
                                Id = uow.DestinatarioRepository.GetNextNuDestinatarioInstancia(),
                                NumeroInstancia = NuInstancia,
                                NumeroGrupo = grupo,
                                FechaAlta = DateTime.Now,
                                NumeroTransaccion = nuTransaccion,
                            };

                            uow.DestinatarioRepository.AddDestinatarioToInstancia(obj);
                        }
                    }
                    else
                    {
                        List<int> grupos = this.GetSelectedGrupos(uow, "QUITAR", NuInstancia, context);

                        foreach (var grupo in grupos)
                        {
                            var grupoInstancia = uow.DestinatarioRepository.GetGrupoInstancia(grupo, NuInstancia);

                            grupoInstancia.FechaModificacion = DateTime.Now;
                            grupoInstancia.NumeroTransaccion = nuTransaccion;
                            grupoInstancia.NumeroTransaccionDelete = nuTransaccion;

                            uow.DestinatarioRepository.UpdateGrupoInstancia(grupoInstancia);
                            uow.SaveChanges();

                            uow.DestinatarioRepository.RemoveGrupoOfInstancia(grupoInstancia);
                        }
                    }

                    uow.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                uow.Rollback();
            }

            return context;
        }

        #region Metodos Auxiliares

        protected virtual List<int> GetSelectedGrupos(IUnitOfWork uow, string tipo, int nroInstancia, GridMenuItemActionContext context)
        {
            var keys = new List<int>();

            foreach (var key in context.Selection.Keys)
            {
                keys.Add(int.Parse(key));
            }

            var dbQuery = new GruposInstanciaQuery(tipo, nroInstancia);

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(keys);

            return dbQuery.GetSelectedKeys(keys);
        }

        #endregion
    }
}
