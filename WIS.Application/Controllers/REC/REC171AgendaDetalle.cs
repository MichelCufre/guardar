using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Security;
using WIS.GridComponent;
using WIS.Components.Common;
using WIS.GridComponent.Build;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Columns;
using WIS.Domain.DataModel;
using WIS.GridComponent.Build.Configuration;
using WIS.Sorting;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.Domain.General;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.GridComponent.Items;
using WIS.Exceptions;
using WIS.Filtering;

namespace WIS.Application.Controllers.REC
{
    public class REC171AgendaDetalle : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridValidationService _gridValidationService;
        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REC171AgendaDetalle(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "QT_ETIQUETA_IMPRIMIR",
                "NU_AGENDA",
                "CD_FAIXA",
                "NU_IDENTIFICADOR",
                "CD_PRODUTO",
                "CD_EMPRESA",
                "QT_AGENDADO",
                "QT_UNIDADES_BULTO"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_AGENDA",SortDirection.Ascending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._gridValidationService = gridValidationService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsEditingEnabled = true;
            query.IsCommitEnabled = false;
            query.IsRollbackEnabled = false;
            query.IsAddEnabled = false;
            query.IsRemoveEnabled = false;

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_ARRAY", new List<IGridItem>
            {
                new GridButton("btnEtiquetasProducto", "REC171_grid1_btn_EtiquetasProducto", "fas fa-tags"),
                new GridButton("btnProblemasRecepcion", "REC171_grid1_btn_btnProblemasRecepcion", "fas fa-exclamation-triangle"),
            }));

            grid.MenuItems = new List<IGridItem>
            {
                new GridButton ("btnImprimir", "REC171_grid1_btn_Imprimir")
            };

            query.AddLink("CD_PRODUTO", "/registro/REG009", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_PRODUTO", "producto"), new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            query.AddLink("CD_EMPRESA", "/registro/REG100", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            var a = grid.Rows;

            using var uow = this._uowFactory.GetUnitOfWork();

            AgendaDetalleQuery dbQuery;

            bool sinEtiqueta = false;
            if (query.Parameters.Any(x => x.Id == "SIN_ETIQUETA"))
                sinEtiqueta = bool.Parse(query.Parameters.Find(x => x.Id == "SIN_ETIQUETA").Value);

            if (query.Parameters.Count > 3 && query.Parameters.Find(x => x.Id == "producto") != null)
            {
                if (!int.TryParse(query.Parameters.Find(x => x.Id == "agenda")?.Value, out int nroAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!int.TryParse(query.Parameters.Find(x => x.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(query.Parameters.Find(x => x.Id == "producto")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idProducto = query.Parameters.Find(x => x.Id == "producto").Value;

                dbQuery = new AgendaDetalleQuery(nroAgenda, idEmpresa, idProducto, sinEtiqueta);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

                string empresaDesc = uow.EmpresaRepository.GetNombre(idEmpresa);

                query.AddParameter("REC171_CD_EMPRESA", idEmpresa.ToString());
                query.AddParameter("REC171_NM_EMPRESA", empresaDesc);
                query.AddParameter("REC171_NU_AGENDA", nroAgenda.ToString());
                query.AddParameter("REC171_CD_PRODUTO", idProducto);
                query.AddParameter("REC171_DS_PRODUTO", uow.ProductoRepository.GetDescripcion(idEmpresa, idProducto));

                grid.GetColumn("CD_EMPRESA").Hidden = true;
                grid.GetColumn("NM_EMPRESA").Hidden = true;
                grid.GetColumn("NU_AGENDA").Hidden = true;
                grid.GetColumn("CD_PRODUTO").Hidden = true;
                grid.GetColumn("DS_PRODUTO").Hidden = true;
            }
            else if (query.Parameters.Count > 3 && query.Parameters.Find(x => x.Id == "cliente") != null)
            {
                if (!int.TryParse(query.Parameters.Find(x => x.Id == "agenda")?.Value, out int nroAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!int.TryParse(query.Parameters.Find(x => x.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(query.Parameters.Find(x => x.Id == "cliente")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idCliente = query.Parameters.Find(x => x.Id == "cliente").Value;

                dbQuery = new AgendaDetalleQuery(nroAgenda, idEmpresa, sinEtiqueta);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

                string empresaDesc = uow.EmpresaRepository.GetNombre(idEmpresa);
                Agente agente = uow.AgenteRepository.GetAgente(idEmpresa, idCliente);

                query.AddParameter("REC171_CD_EMPRESA", idEmpresa.ToString());
                query.AddParameter("REC171_NM_EMPRESA", empresaDesc);
                query.AddParameter("REC171_DS_TIPO_AGENTE", agente.Tipo.ToString());
                query.AddParameter("REC171_CD_AGENTE", agente.Codigo);
                query.AddParameter("REC171_DS_AGENTE", agente.Descripcion);
                query.AddParameter("REC171_NU_AGENDA", nroAgenda.ToString());

                grid.GetColumn("CD_EMPRESA").Hidden = true;
                grid.GetColumn("NM_EMPRESA").Hidden = true;
                grid.GetColumn("NU_AGENDA").Hidden = true;
            }
            else if (query.Parameters.Count > 1)
            {
                if (!int.TryParse(query.Parameters.Find(x => x.Id == "agenda")?.Value, out int nroAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new AgendaDetalleQuery(nroAgenda, sinEtiqueta);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);
            }
            else
            {
                dbQuery = new AgendaDetalleQuery(sinEtiqueta);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);
            }

            foreach (var row in grid.Rows)
                row.SetEditableCells(
                    new List<string>()
                    {
                        "QT_ETIQUETA_IMPRIMIR",
                    });

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            AgendaDetalleQuery dbQuery;

            bool sinEtiqueta = false;
            if (query.Parameters.Any(x => x.Id == "SIN_ETIQUETA"))
                sinEtiqueta = bool.Parse(query.Parameters.Find(x => x.Id == "SIN_ETIQUETA").Value);

            if (query.Parameters.Count > 3 && query.Parameters.Find(x => x.Id == "producto") != null)
            {
                if (!int.TryParse(query.Parameters.Find(x => x.Id == "agenda")?.Value, out int nroAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!int.TryParse(query.Parameters.Find(x => x.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(query.Parameters.Find(x => x.Id == "producto")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idProducto = query.Parameters.Find(x => x.Id == "producto").Value;

                dbQuery = new AgendaDetalleQuery(nroAgenda, idEmpresa, idProducto, sinEtiqueta);

            }
            else if (query.Parameters.Count > 3 && query.Parameters.Find(x => x.Id == "cliente") != null)
            {
                if (!int.TryParse(query.Parameters.Find(x => x.Id == "agenda")?.Value, out int nroAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!int.TryParse(query.Parameters.Find(x => x.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new AgendaDetalleQuery(nroAgenda, idEmpresa, sinEtiqueta);

            }
            else
            {
                dbQuery = new AgendaDetalleQuery(sinEtiqueta);

            }

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("NU_AGENDA", SortDirection.Ascending);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, defaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            AgendaDetalleQuery dbQuery;

            bool sinEtiqueta = false;
            if (query.Parameters.Any(x => x.Id == "SIN_ETIQUETA"))
                sinEtiqueta = bool.Parse(query.Parameters.Find(x => x.Id == "SIN_ETIQUETA").Value);

            if (query.Parameters.Count > 3 && query.Parameters.Find(x => x.Id == "producto") != null)
            {
                if (!int.TryParse(query.Parameters.Find(x => x.Id == "agenda")?.Value, out int nroAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!int.TryParse(query.Parameters.Find(x => x.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(query.Parameters.Find(x => x.Id == "producto")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idProducto = query.Parameters.Find(x => x.Id == "producto").Value;

                dbQuery = new AgendaDetalleQuery(nroAgenda, idEmpresa, idProducto, sinEtiqueta);

            }
            else if (query.Parameters.Count > 3 && query.Parameters.Find(x => x.Id == "cliente") != null)
            {
                if (!int.TryParse(query.Parameters.Find(x => x.Id == "agenda")?.Value, out int nroAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!int.TryParse(query.Parameters.Find(x => x.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new AgendaDetalleQuery(nroAgenda, idEmpresa, sinEtiqueta);

            }
            else
            {
                dbQuery = new AgendaDetalleQuery(sinEtiqueta);

            }
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext data)
        {
            switch (data.ButtonId)
            {
                case "btnEtiquetasProducto":
                    data.Redirect("/recepcion/REC150", new List<ComponentParameter> {
                        new ComponentParameter() { Id = "agenda", Value = data.Row.GetCell("NU_AGENDA").Value },
                        new ComponentParameter() { Id = "producto", Value = data.Row.GetCell("CD_PRODUTO").Value },
                        new ComponentParameter() { Id = "identificador", Value = data.Row.GetCell("NU_IDENTIFICADOR").Value },
                        new ComponentParameter() { Id = "embalaje", Value = data.Row.GetCell("CD_FAIXA").Value },

                    });
                    break;

                case "btnProblemasRecepcion":
                    data.Redirect("/recepcion/REC141", new List<ComponentParameter> {
                        new ComponentParameter() { Id = "agenda", Value = data.Row.GetCell("NU_AGENDA").Value },
                        new ComponentParameter() { Id = "producto", Value = data.Row.GetCell("CD_PRODUTO").Value },
                        new ComponentParameter() { Id = "identificador", Value = data.Row.GetCell("NU_IDENTIFICADOR").Value },
                        new ComponentParameter() { Id = "embalaje", Value = data.Row.GetCell("CD_FAIXA").Value },

                    });
                    break;
            }
            return data;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            try
            {
                using var uow = this._uowFactory.GetUnitOfWork();
                List<string> selectedRows = this.GetSelectedKeys(uow, context);

                context.Parameters.Add(new ComponentParameter()
                {
                    Id = "SELECTED_KEYS",
                    Value = JsonConvert.SerializeObject(selectedRows)
                });
            }
            catch (ValidationFailedException vex)
            {
                context.AddErrorNotification(vex.Message);
            }
            return context;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new DetalleAgendaGridValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }

        protected virtual List<string> GetSelectedKeys(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            AgendaDetalleQuery dbQuery;
            bool sinEtiqueta = false;

            if (context.Parameters.Any(x => x.Id == "SIN_ETIQUETA"))
                sinEtiqueta = bool.Parse(context.Parameters.Find(x => x.Id == "SIN_ETIQUETA").Value);

            if (context.Parameters.Count > 3 && context.Parameters.Find(x => x.Id == "producto") != null)
            {
                if (!int.TryParse(context.Parameters.Find(x => x.Id == "agenda")?.Value, out int nroAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!int.TryParse(context.Parameters.Find(x => x.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(context.Parameters.Find(x => x.Id == "producto")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idProducto = context.Parameters.Find(x => x.Id == "producto").Value;

                dbQuery = new AgendaDetalleQuery(nroAgenda, idEmpresa, idProducto, sinEtiqueta);
            }
            else if (context.Parameters.Count > 3 && context.Parameters.Find(x => x.Id == "cliente") != null)
            {
                if (!int.TryParse(context.Parameters.Find(x => x.Id == "agenda")?.Value, out int nroAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!int.TryParse(context.Parameters.Find(x => x.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new AgendaDetalleQuery(nroAgenda, idEmpresa, sinEtiqueta);
            }
            else dbQuery = new AgendaDetalleQuery(sinEtiqueta);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            string pattern = @"^([^$]+)\$(.*)$";

            List<GridRow> modifiedRows = JsonConvert.DeserializeObject<List<GridRow>>(context.GetParameter("MODIFIED_ROWS"));

            var keysSelectedRows =
                context.Selection.AllSelected
                    ? dbQuery.GetKeysAndExclude(context.Selection.Keys)
                    : context.Selection.Keys;

            if (keysSelectedRows == null || keysSelectedRows.Count == 0)
                throw new ValidationFailedException("REC171_grid1_Error_DebeSeleccionarFilas");

            if (!modifiedRows.Any())
                return keysSelectedRows;

            var modifiedMap = new Dictionary<string, string>();

            foreach (var modified in modifiedRows)
            {
                var matchMod = Regex.Match(modified.Id, pattern);
                if (matchMod.Success)
                {
                    string modifiedKey = matchMod.Groups[2].Value;

                    modifiedMap[modifiedKey] = modifiedRows
                        .FirstOrDefault(x => x.Id == modified.Id)
                        .GetCell("QT_ETIQUETA_IMPRIMIR")
                        .Value;
                }
            }

            for (int i = 0; i < keysSelectedRows.Count; i++)
            {
                string key = keysSelectedRows[i];
                Match matchKey = Regex.Match(key, pattern);

                if (matchKey.Success)
                {
                    string keyGrilla = matchKey.Groups[2].Value;

                    if (modifiedMap.TryGetValue(keyGrilla, out string cantVirtualModificada))
                    {
                        if (!int.TryParse(cantVirtualModificada, _identity.GetFormatProvider(), out var qtEtiqueta))
                            throw new ValidationFailedException("REC171_grid1_Error_CantidadFormatoIncorrecto");

                        if (qtEtiqueta <= 0)
                            throw new ValidationFailedException("REC171_grid1_Error_CantidadFormatoIncorrecto");

                        keysSelectedRows[i] = qtEtiqueta + "$" + keyGrilla;
                    }
                }
            }

            return keysSelectedRows;
        }
    }
}