using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;
using WIS.Exceptions;
using WIS.Domain.DataModel.Queries.Seguridad;
using WIS.Domain.Security;
using WIS.Filtering;

namespace WIS.Application.Controllers.SEG
{
    public class SEG030UsuarioAsignarPermisos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public SEG030UsuarioAsignarPermisos(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("RESOURCEID",SortDirection.Ascending)
            };

            this.GridKeys = new List<string>
            {
                "RESOURCEID"
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            switch (grid.Id)
            {
                case "SEG030Recursos_grid_1":
                    grid.MenuItems = new List<IGridItem> { new GridButton("btnAsociar", "SEG030_Sec0_mdlAsigna_btnAsociar") };
                    break;

                case "SEG030Recursos_grid_2":
                    grid.MenuItems = new List<IGridItem> { new GridButton("btnDesasociar", "SEG030_Sec0_mdlAsigna_btnDesasociar") };
                    break;
            }

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "idUsuario")?.Value, out int idUsuario))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            if (grid.Id == "SEG030Recursos_grid_1")
            {
                var dbQuery = new RecursosDisponiblesUsuarioQuery(idUsuario);
                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
            }
            else
            {
                var dbQuery = new RecursosAsignadosUsuarioQuery(idUsuario);
                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
            }

            var user = uow.SecurityRepository.GetUsuario(idUsuario);

            context.AddParameter("SEG030_USUARIO", user.Username);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "idUsuario")?.Value, out int idUsuario))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            if (grid.Id == "SEG030Recursos_grid_1")
            {
                var dbQuery = new RecursosDisponiblesUsuarioQuery(idUsuario);
                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
            }
            else
            {
                var dbQuery = new RecursosAsignadosUsuarioQuery(idUsuario);
                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
            }
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "idUsuario")?.Value, out int idUsuario))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            if (grid.Id == "SEG030Recursos_grid_1")
            {
                var dbQuery = new RecursosDisponiblesUsuarioQuery(idUsuario);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else
            {
                var dbQuery = new RecursosAsignadosUsuarioQuery(idUsuario);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            return context;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            switch (context.GridId)
            {
                case "SEG030Recursos_grid_1": return this.GridMenuItemAccionAsociarRecursos(context);
                case "SEG030Recursos_grid_2": return this.GridMenuItemDesasociarRecursos(context);
            }
            return context;
        }

        #region Metodos Auxiliares

        public virtual GridMenuItemActionContext GridMenuItemAccionAsociarRecursos(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "idUsuario")?.Value, out int idUsuario))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            var dbQuery = new RecursosDisponiblesUsuarioQuery(idUsuario);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            var select = dbQuery.GetResult().Select(g => new { g.RESOURCEID }).ToList();

            var retornarKeysSelected = new List<int>();

            if (context.Selection.AllSelected)
            {
                retornarKeysSelected = select.Select(r => r.RESOURCEID).ToList();
            }
            else
            {
                retornarKeysSelected = select.Where(r => context.Selection.Keys.Contains(r.RESOURCEID.ToString())).Select(x => x.RESOURCEID).ToList();
            }

            new UserService().AgregarRecursosUsuario(uow, retornarKeysSelected, idUsuario);

            uow.SaveChanges();

            context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");

            return context;
        }

        public virtual GridMenuItemActionContext GridMenuItemDesasociarRecursos(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "idUsuario")?.Value, out int idUsuario))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            var dbQuery = new RecursosAsignadosUsuarioQuery(idUsuario);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);


            var select = dbQuery.GetResult().Select(g => new { g.RESOURCEID }).ToList();

            var retornarKeysSelected = new List<int>();

            if (context.Selection.AllSelected)
            {
                retornarKeysSelected = select.Select(r => r.RESOURCEID).ToList();
            }
            else
            {
                retornarKeysSelected = select.Where(r => context.Selection.Keys.Contains(r.RESOURCEID.ToString())).Select(x => x.RESOURCEID).ToList();
            }

            new UserService().QuitarRecursosUsuario(uow, retornarKeysSelected, idUsuario);

            uow.SaveChanges();

            context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");

            return context;
        }

        #endregion
    }
}
