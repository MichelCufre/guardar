using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRE
{
    public class PRE640SeguimientoPedidos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeysAgenda { get; }

        public PRE640SeguimientoPedidos(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this._identity = identity;
            this.GridKeysAgenda = new List<string>
            {
                "NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA"
            };
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
        }


        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            //grid.MenuItems = new List<IGridItem> {
            //        new GridButton("btnGuardar", "General_Sec0_btn_Guardar"),
            //    };

            FiltrosPedidosResumen filtros = JsonConvert.DeserializeObject<FiltrosPedidosResumen>(context.GetParameter("FILTROS"));

            AjustarFiltros(filtros);

            context.FetchContext.Parameters.RemoveAll(p => p.Id == "FILTROS");
            context.FetchContext.AddParameter("FILTROS", JsonConvert.SerializeObject(filtros));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                FiltrosPedidosResumen filtros = JsonConvert.DeserializeObject<FiltrosPedidosResumen>(context.GetParameter("FILTROS"));

                AjustarFiltros(filtros);

                var dbQuery = new ResumenesPedidos(filtros, _identity);

                uow.HandleQuery(dbQuery, false, false, false);

                List<SortCommand> defaultSort = new List<SortCommand>() {
                    new SortCommand("DT_ADDROW",SortDirection.Descending)
                };

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeysAgenda);
            }

            var seleccion = new List<string>();

            grid.Rows.ForEach(w =>
            {
                string nuColor = w.GetCell("STATUS_PEOR").Value;
                if (!string.IsNullOrEmpty(w.GetCell("DT_LIBERADO").Value))
                {
                    w.GetCell("DT_LIBERADO").Value = w.GetCell("DT_LIBERADO").Value.Replace('+', ' ').Replace('-', ' ').Substring(6, 13);

                }
                if (!string.IsNullOrEmpty(w.GetCell("DT_INI_PEDIDO").Value))
                {
                    w.GetCell("DT_INI_PEDIDO").Value = w.GetCell("DT_INI_PEDIDO").Value.Replace('+', ' ').Replace('-', ' ').Substring(6, 13);

                }
                if (!string.IsNullOrEmpty(w.GetCell("DT_FIN_PEDIDO").Value))
                {
                    w.GetCell("DT_FIN_PEDIDO").Value = w.GetCell("DT_FIN_PEDIDO").Value.Replace('+', ' ').Replace('-', ' ').Substring(6, 13);

                }
                //if (w.GetCell("CD_FUN_RESP").Value != "" && nuColor == "Nuevo Con Responsable")
                //{
                //    seleccion.Add(w.Id);
                //}

                switch (nuColor)
                {

                    case "Nuevo":
                        w.CssClass = "redBlack";
                        break;
                    case "Liberado":
                        w.DisabledSelected = true;
                        w.CssClass = "orange";
                        break;
                    case "En Preparacion":
                        w.DisabledSelected = true;
                        w.CssClass = "yellow";
                        break;
                    case "Cargando Camion":
                        w.DisabledSelected = true;
                        w.CssClass = "green";
                        break;
                    case "Nuevo Con Responsable":
                        w.CssClass = "red";
                        break;

                    default:
                        w.DisabledSelected = true;
                        break;
                }
            });

            //context.AddParameter("preSeleccion", JsonConvert.SerializeObject(seleccion));

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                FiltrosPedidosResumen filtros = new FiltrosPedidosResumen();

                if (context.Parameters.Any(s => s.Id == "FILTROS"))
                {
                    filtros = JsonConvert.DeserializeObject<FiltrosPedidosResumen>(context.Parameters.FirstOrDefault(s => s.Id == "FILTROS")?.Value);
                }

                AjustarFiltros(filtros);

                var dbQuery = new ResumenesPedidos(filtros, _identity);

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            SortCommand defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                FiltrosPedidosResumen filtros = JsonConvert.DeserializeObject<FiltrosPedidosResumen>(context.GetParameter("FILTROS"));

                AjustarFiltros(filtros);

                var dbQuery = new ResumenesPedidos(filtros, _identity);

                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return _excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            var FieldPredio = form.GetField("NU_PREDIO");

            if (_identity.Predio != GeneralDb.PredioSinDefinir)
            {
                FieldPredio.Value = _identity.Predio;
                FieldPredio.ReadOnly = true;
            }

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                form.GetField("NU_PREDIO").Options = this.SelectPredio(uow);
                form.GetField("FL_FINALIZADOS").Options = this.SelectFinalizado(uow);
            }

            context.Parameters.RemoveAll(p => p.Id == "FILTROS");
            context.AddParameter("FILTROS", JsonConvert.SerializeObject(new { USERID = "".ToString(), CD_EMPRESA = "", NU_PREDIO = FieldPredio.Value, FL_FINALIZADOS = "" }));

            return this.FormButtonAction(form, new FormButtonActionContext() { Parameters = context.Parameters });
        }

        public override Form FormButtonAction(Form form, FormButtonActionContext context)
        {
            FiltrosPedidosResumen filtros = JsonConvert.DeserializeObject<FiltrosPedidosResumen>(context.GetParameter("FILTROS"));

            if (string.IsNullOrEmpty(filtros.NU_PREDIO))
            {
                filtros.NU_PREDIO = _identity.Predio;
            }

            AjustarFiltros(filtros);

            var dbQuery = new ResumenesPedidos(filtros, _identity);

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                uow.HandleQuery(dbQuery, false, false, false);

                List<CantidadResumenPedido> objetoConCantidades = dbQuery.GetCantidad(uow);

                form.GetField("QT_NUEVOS").Value = objetoConCantidades.FirstOrDefault(w => w.STATUS_PEOR == "Nuevo")?.CANTIDAD.ToString();
                form.GetField("QT_LIBERADOS").Value = objetoConCantidades.FirstOrDefault(w => w.STATUS_PEOR == "Liberado")?.CANTIDAD.ToString();
                form.GetField("QT_PREPARACION").Value = objetoConCantidades.FirstOrDefault(w => w.STATUS_PEOR == "En Preparacion")?.CANTIDAD.ToString();
                form.GetField("QT_PEDIDOS_CARGANDO").Value = objetoConCantidades.FirstOrDefault(w => w.STATUS_PEOR == "Cargando Camion")?.CANTIDAD.ToString();
            }

            return form;
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "CD_EMPRESA": return this.SearchEmpresa(context.SearchValue);

                default:
                    return new List<SelectOption>();
            }
        }

        #region Metodos Auxiliares

        public virtual List<SelectOption> SearchEmpresa(string searchValue)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return uow.EmpresaRepository.GetByNombreOrCodePartialForUsuario(searchValue, _identity.UserId)
                    .Select(w => new SelectOption(w.Id.ToString(), $"{w.Id} - {w.Nombre}"))
                    .ToList();
            }
        }

        public virtual List<SelectOption> SelectPredio(IUnitOfWork uow)
        {
            var query = uow.PredioRepository.GetPrediosUsuario(_identity.UserId);

            if (_identity.Predio != GeneralDb.PredioSinDefinir)
            {
                query.Where(p => p.Numero == _identity.Predio);
            }

            return query
                  .Select(w => new SelectOption(w.Numero, $"{w.Numero} - {w.Descripcion}"))
                  .ToList();
        }

        public virtual List<SelectOption> SelectFinalizado(IUnitOfWork uow)
        {
            return new List<SelectOption>
            {
                new SelectOption("S", "SI"),
                new SelectOption("N", "NO"),
            };
        }


        public virtual void AjustarFiltros(FiltrosPedidosResumen filtros)
        {
            if (_identity.Predio != GeneralDb.PredioSinDefinir)
            {
                filtros.NU_PREDIO = _identity.Predio;
            }
        }

        #endregion
    }
}
