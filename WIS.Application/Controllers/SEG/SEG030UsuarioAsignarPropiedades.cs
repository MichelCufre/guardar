using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Seguridad;
using WIS.Domain.Security;
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

namespace WIS.Application.Controllers.SEG
{
    public class SEG030UsuarioAsignarPropiedades : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        public SEG030UsuarioAsignarPropiedades(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }


        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            switch (grid.Id)
            {
                case "SEG030Asignar_grid_1":
                    grid.MenuItems = new List<IGridItem>
                    {
                        new GridButton("btnAgregar", "General_Sec0_btn_Agregar")
                    };
                    break;

                case "SEG030Asignar_grid_2":
                    grid.MenuItems = new List<IGridItem>
                    {
                        new GridButton("btnQuitar", "General_Sec0_btn_Quitar")
                    };
                    break;

                case "SEG030Asignar_grid_3":
                    grid.MenuItems = new List<IGridItem>
                    {
                        new GridButton("btnAgregar", "General_Sec0_btn_Agregar")
                    };
                    break;
                case "SEG030Asignar_grid_4":
                    grid.MenuItems = new List<IGridItem>
                    {
                        new GridButton("btnQuitar", "General_Sec0_btn_Quitar")
                    };
                    break;
                case "SEG030Asignar_grid_5":
                    grid.MenuItems = new List<IGridItem>
                    {
                        new GridButton("btnAgregar", "General_Sec0_btn_Agregar")
                    };
                    break;
                case "SEG030Asignar_grid_6":
                    grid.MenuItems = new List<IGridItem>
                    {
                        new GridButton("btnQuitar", "General_Sec0_btn_Quitar")
                    };
                    break;
            }
            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            switch (grid.Id)
            {
                case "SEG030Asignar_grid_1":
                    this.GridFetchRowsPrediosDisponiblesUsuario(grid, context, uow);
                    break;

                case "SEG030Asignar_grid_2":
                    this.GridFetchRowsPrediosAsociadosUsuario(grid, context, uow);
                    break;

                case "SEG030Asignar_grid_3":
                    this.GridFetchRowsEmpresasDisponiblesUsuario(grid, context, uow);
                    break;
                case "SEG030Asignar_grid_4":
                    this.GridFetchRowsEmpresasAsociadasUsuario(grid, context, uow);
                    break;
                case "SEG030Asignar_grid_5":
                    this.GridFetchRowsGrupoConsultaDisponiblesUsuario(grid, context, uow);
                    break;
                case "SEG030Asignar_grid_6":
                    this.GridFetchRowsGrupoConsultaAsociadosUsuario(grid, context, uow);
                    break;
            }

            Usuario user = uow.SecurityRepository.GetUsuario(int.Parse(context.Parameters.FirstOrDefault(x => x.Id == "idUsuario").Value));
            context.AddParameter("SEG030_USUARIO", user.Username);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "idUsuario")?.Value, out int idUsuario))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            if (grid.Id == "SEG030Asignar_grid_1")
            {
                List<SortCommand> DefaultSort = new List<SortCommand> { new SortCommand("NU_PREDIO", SortDirection.Ascending) };

                PrediosDisponiblesQuery dbQuery;
                dbQuery = new PrediosDisponiblesQuery(idUsuario);
                uow.HandleQuery(dbQuery);
                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, DefaultSort);
            }
            else if (grid.Id == "SEG030Asignar_grid_2")
            {
                List<SortCommand> DefaultSort = new List<SortCommand> { new SortCommand("NU_PREDIO", SortDirection.Ascending) };

                PrediosUsuarioQuery dbQuery;
                dbQuery = new PrediosUsuarioQuery(idUsuario);
                uow.HandleQuery(dbQuery);
                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, DefaultSort);
            }
            else if (grid.Id == "SEG030Asignar_grid_3")
            {
                List<SortCommand> DefaultSort = new List<SortCommand> { new SortCommand("CD_EMPRESA", SortDirection.Ascending) };

                UsuarioEmpresaQuery dbQuery;
                dbQuery = new UsuarioEmpresaQuery(idUsuario, false);
                uow.HandleQuery(dbQuery);
                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, DefaultSort);
            }
            else if (grid.Id == "SEG030Asignar_grid_4")
            {
                List<SortCommand> DefaultSort = new List<SortCommand> { new SortCommand("CD_EMPRESA", SortDirection.Ascending) };

                UsuarioEmpresaQuery dbQuery;
                dbQuery = new UsuarioEmpresaQuery(idUsuario, true);
                uow.HandleQuery(dbQuery);
                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, DefaultSort);
            }
            else if (grid.Id == "SEG030Asignar_grid_5")
            {
                List<SortCommand> DefaultSort = new List<SortCommand> { new SortCommand("CD_GRUPO_CONSULTA", SortDirection.Ascending) };

                GrupoConsultaQuery dbQuery;
                dbQuery = new GrupoConsultaQuery(idUsuario, false);
                uow.HandleQuery(dbQuery);
                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, DefaultSort);
            }
            else
            {
                List<SortCommand> DefaultSort = new List<SortCommand> { new SortCommand("CD_GRUPO_CONSULTA", SortDirection.Ascending) };

                GrupoConsultaQuery dbQuery;
                dbQuery = new GrupoConsultaQuery(idUsuario, true);
                uow.HandleQuery(dbQuery);
                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, DefaultSort);
            }
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "idUsuario")?.Value, out int idUsuario))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            if (grid.Id == "SEG030Asignar_grid_1")
            {
                PrediosDisponiblesQuery dbQuery;
                dbQuery = new PrediosDisponiblesQuery(idUsuario);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else if (grid.Id == "SEG030Asignar_grid_2")
            {
                PrediosUsuarioQuery dbQuery;
                dbQuery = new PrediosUsuarioQuery(idUsuario);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else if (grid.Id == "SEG030Asignar_grid_3")
            {
                UsuarioEmpresaQuery dbQuery;
                dbQuery = new UsuarioEmpresaQuery(idUsuario, false);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else if (grid.Id == "SEG030Asignar_grid_4")
            {
                UsuarioEmpresaQuery dbQuery;
                dbQuery = new UsuarioEmpresaQuery(idUsuario, true);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else if (grid.Id == "SEG030Asignar_grid_5")
            {
                GrupoConsultaQuery dbQuery;
                dbQuery = new GrupoConsultaQuery(idUsuario, false);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else
            {
                GrupoConsultaQuery dbQuery;
                dbQuery = new GrupoConsultaQuery(idUsuario, true);
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

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoAsignacionInfoValidationModule(uow), grid, row, context);
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            int user = int.Parse(context.Parameters.FirstOrDefault(x => x.Id == "idUsuario")?.Value);

            switch (context.GridId)
            {
                case "SEG030Asignar_grid_1":
                    this.AgregarPredios(context, user);
                    break;
                case "SEG030Asignar_grid_2":
                    this.QuitarPredios(context, user);
                    break;
                case "SEG030Asignar_grid_3":
                    this.AgregarEmpresa(context, user);
                    break;
                case "SEG030Asignar_grid_4":
                    this.QuitarEmpresa(context, user);
                    break;
                case "SEG030Asignar_grid_5":
                    this.AgregarGrupoConsulta(context, user);
                    break;
                case "SEG030Asignar_grid_6":
                    this.QuitarGrupoConsulta(context, user);
                    break;
            }
            return context;
        }

        #region Metodos Auxiliares

        public virtual Grid GridFetchRowsPrediosDisponiblesUsuario(Grid grid, GridFetchContext context, IUnitOfWork uow)
        {
            List<SortCommand> DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_PREDIO",SortDirection.Ascending)
            };

            List<string> GridKeys = new List<string> { "NU_PREDIO" };

            PrediosDisponiblesQuery dbQuery;

            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "idUsuario")?.Value, out int idUsuario))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            dbQuery = new PrediosDisponiblesQuery(idUsuario);
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, DefaultSort, GridKeys);

            return grid;
        }

        public virtual Grid GridFetchRowsPrediosAsociadosUsuario(Grid grid, GridFetchContext context, IUnitOfWork uow)
        {
            List<SortCommand> DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_PREDIO",SortDirection.Ascending)
            };

            List<string> GridKeys = new List<string> { "NU_PREDIO" };

            PrediosUsuarioQuery dbQuery;

            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "idUsuario")?.Value, out int idUsuario))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            dbQuery = new PrediosUsuarioQuery(idUsuario);
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, DefaultSort, GridKeys);

            return grid;
        }

        public virtual Grid GridFetchRowsEmpresasDisponiblesUsuario(Grid grid, GridFetchContext context, IUnitOfWork uow)
        {
            List<SortCommand> DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_EMPRESA",SortDirection.Ascending)
            };

            List<string> GridKeys = new List<string> { "CD_EMPRESA" };

            UsuarioEmpresaQuery dbQuery;

            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "idUsuario")?.Value, out int idUsuario))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            dbQuery = new UsuarioEmpresaQuery(idUsuario, false);
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, DefaultSort, GridKeys);

            return grid;
        }

        public virtual Grid GridFetchRowsEmpresasAsociadasUsuario(Grid grid, GridFetchContext context, IUnitOfWork uow)
        {
            List<SortCommand> DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_EMPRESA",SortDirection.Ascending)
            };

            List<string> GridKeys = new List<string> { "CD_EMPRESA" };

            UsuarioEmpresaQuery dbQuery;

            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "idUsuario")?.Value, out int idUsuario))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            dbQuery = new UsuarioEmpresaQuery(idUsuario, true);
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, DefaultSort, GridKeys);

            return grid;
        }

        public virtual Grid GridFetchRowsGrupoConsultaDisponiblesUsuario(Grid grid, GridFetchContext context, IUnitOfWork uow)
        {

            List<SortCommand> DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_GRUPO_CONSULTA",SortDirection.Descending),
            };

            List<string> GridKeys = new List<string> { "CD_GRUPO_CONSULTA" };

            GrupoConsultaQuery dbQuery;

            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "idUsuario")?.Value, out int idUsuario))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            dbQuery = new GrupoConsultaQuery(idUsuario, false);

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, DefaultSort, GridKeys);

            return grid;
        }

        public virtual Grid GridFetchRowsGrupoConsultaAsociadosUsuario(Grid grid, GridFetchContext context, IUnitOfWork uow)
        {
            List<SortCommand> DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_GRUPO_CONSULTA",SortDirection.Descending),
            };

            List<string> GridKeys = new List<string> { "CD_GRUPO_CONSULTA" };

            GrupoConsultaQuery dbQuery;

            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "idUsuario")?.Value, out int idUsuario))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            dbQuery = new GrupoConsultaQuery(idUsuario, true);
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, DefaultSort, GridKeys);

            return grid;
        }

        public virtual void AgregarPredios(GridMenuItemActionContext context, int user)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.BeginTransaction();
            uow.CreateTransactionNumber("SEG030 Asignar predios al usuario");
            try
            {
                List<Dictionary<string, string>> selection = context.Selection.GetSelection(new List<string> { "NU_PREDIO" });

                List<string> predios = new List<string>();

                selection.ForEach(item =>
                {
                    predios.Add(item["NU_PREDIO"]);
                }
                );

                if (context.Selection.AllSelected)
                {
                    var dbQuery = new PrediosDisponiblesQuery(user);
                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);
                    var prediosList = dbQuery.GetPredios();
                    predios = prediosList.Except(predios).ToList();
                }


                foreach (var pre in predios)
                {
                    uow.PredioRepository.AsignarPredioUsuarios(pre, new List<int> { user });
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("SEG030_Sec0_error_prediosActualizados");
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw;
            }
        }

        public virtual void QuitarPredios(GridMenuItemActionContext context, int user)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.BeginTransaction();
            uow.CreateTransactionNumber("SEG030 Quitar predios al usuario");
            try
            {
                List<Dictionary<string, string>> selection = context.Selection.GetSelection(new List<string> { "NU_PREDIO" });

                List<string> predios = new List<string>();

                selection.ForEach(item =>
                {
                    predios.Add(item["NU_PREDIO"]);
                }
                );


                if (context.Selection.AllSelected)
                {
                    var dbQuery = new PrediosUsuarioQuery(user);
                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);
                    var prediosList = dbQuery.GetPrediosAsociados();
                    predios = prediosList.Except(predios).ToList();
                }

                foreach (var pre in predios)
                {
                    uow.PredioRepository.RemoverPredioUsuarios(pre, new List<int> { user });
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("SEG030_Sec0_error_prediosActualizados");
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw;
            }
        }

        public virtual void AgregarEmpresa(GridMenuItemActionContext context, int user)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.BeginTransaction();
            uow.CreateTransactionNumber("SEG030 Asignar empresas al usuario");
            try
            {
                List<int> empresasSelected = new List<int>();

                List<Dictionary<string, string>> selection = context.Selection.GetSelection(new List<string> { "CD_EMPRESA" });

                selection.ForEach(item =>
                {
                    empresasSelected.Add(int.Parse(item["CD_EMPRESA"]));
                });

                if (context.Selection.AllSelected)
                {
                    var dbQuery = new UsuarioEmpresaQuery(user, false);
                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);
                    var empresasList = dbQuery.GetEmpresas();
                    empresasSelected = empresasList.Except(empresasSelected).ToList();
                }


                uow.EmpresaRepository.AsignarEmpresasUsuario(user, empresasSelected);
                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("SEG030_Sec0_error_empresasActualizadas");
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw;
            }
        }

        public virtual void QuitarEmpresa(GridMenuItemActionContext context, int user)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.BeginTransaction();
            uow.CreateTransactionNumber("SEG030 Quitar empresas al usuario");
            try
            {
                List<int> empresasSelected = new List<int>();

                List<Dictionary<string, string>> selection = context.Selection.GetSelection(new List<string> { "CD_EMPRESA" });

                selection.ForEach(item =>
                {
                    empresasSelected.Add(int.Parse(item["CD_EMPRESA"]));
                });

                if (context.Selection.AllSelected)
                {
                    var dbQuery = new UsuarioEmpresaQuery(user, true);
                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);
                    var empresasList = dbQuery.GetEmpresas();
                    empresasSelected = empresasList.Except(empresasSelected).ToList();
                }

                uow.EmpresaRepository.RemoverEmpresasUsuarios(empresasSelected, new List<int> { user });

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("SEG030_Sec0_error_empresasActualizadas");
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw;
            }
        }

        public virtual void AgregarGrupoConsulta(GridMenuItemActionContext context, int user)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.BeginTransaction();
            uow.CreateTransactionNumber("SEG030 Asignar grupo consulta al usuario");
            try
            {
                List<string> grupos = new List<string>();
                List<Dictionary<string, string>> selection = context.Selection.GetSelection(new List<string> { "CD_GRUPO_CONSULTA" });

                selection.ForEach(item =>
                {
                    grupos.Add(item["CD_GRUPO_CONSULTA"]);
                });
                if (context.Selection.AllSelected)
                {
                    var dbQuery = new GrupoConsultaQuery(user, false);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);
                    uow.HandleQuery(dbQuery);
                    var gruposList = dbQuery.GetGruposConsulta();
                    grupos = gruposList.Except(grupos).ToList();
                }

                uow.GrupoConsultaRepository.AsignarGruposConsultaUsuarios(grupos, new List<int> { user });

                uow.SaveChanges();
                uow.Commit();


                context.AddSuccessNotification("SEG030_Sec0_error_grupConsuActualizados");
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw;
            }
        }

        public virtual void QuitarGrupoConsulta(GridMenuItemActionContext context, int user)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.BeginTransaction();
            uow.CreateTransactionNumber("SEG030 Quitar grupo consulta al usuario");
            try
            {
                List<string> grupos = new List<string>();
                List<Dictionary<string, string>> selection = context.Selection.GetSelection(new List<string> { "CD_GRUPO_CONSULTA" });

                selection.ForEach(item =>
                {
                    grupos.Add(item["CD_GRUPO_CONSULTA"]);
                });
                if (context.Selection.AllSelected)
                {
                    var dbQuery = new GrupoConsultaQuery(user, true);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);
                    uow.HandleQuery(dbQuery);
                    var gruposList = dbQuery.GetGruposConsulta();
                    grupos = gruposList.Except(grupos).ToList();
                }

                uow.GrupoConsultaRepository.RemoverGruposConsultaUsuarios(grupos, new List<int> { user });
                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("SEG030_Sec0_error_grupConsuActualizados");
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw;
            }
        }

        #endregion
    }
}
