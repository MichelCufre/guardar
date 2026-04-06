using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Filtering;
using WIS.FormComponent.Execution.Configuration;
using WIS.FormComponent;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;
using WIS.Exceptions;
using DocumentFormat.OpenXml.InkML;

namespace WIS.Application.Controllers.REC
{
    public class REC280DetallesSugerencia : AppController
    {

        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<REC280DetallesSugerencia> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REC280DetallesSugerencia(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            IIdentityService identity,
            ISecurityService security,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter,
            ILogger<REC280DetallesSugerencia> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
               "NU_ALM_SUGERENCIA_DET", "NU_ALM_ESTRATEGIA", "NU_PREDIO", "TP_ALM_OPERATIVA_ASOCIABLE", "CD_ALM_OPERATIVA_ASOCIABLE", "CD_CLASSE", "CD_GRUPO", "CD_EMPRESA_PRODUTO", "CD_PRODUTO", "CD_REFERENCIA", "CD_AGRUPADOR", "CD_ENDERECO_SUGERIDO", "CD_EMPRESA", "CD_PRODUTO_AGRUPADOR", "CD_FAIXA_AGRUPADOR", "NU_IDENTIFICADOR_AGRUPADOR","NU_ALM_SUGERENCIA"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_ALM_SUGERENCIA_DET", SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._session = session;
            this._identity = identity;
            this._security = security;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._formValidationService = formValidationService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
        }

        #region GRID
        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsEditingEnabled = false;
            query.IsRemoveEnabled = false;
            query.IsAddEnabled = false;
            query.IsCommitEnabled = false;

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int empresa;
            int estrategia;

            string empresaParam = query.Parameters.Find(x => x.Id == "empresa")?.Value;
            string estrategiaParam = query.Parameters.Find(x => x.Id == "estrategia")?.Value;

            if (!int.TryParse(empresaParam, out empresa))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            if (!int.TryParse(estrategiaParam, out estrategia))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            string predio = query.Parameters.Find(x => x.Id == "predio")?.Value;
            string tipoOperativa = query.Parameters.Find(x => x.Id == "tipoOperativa")?.Value;
            string codigoOperativa = query.Parameters.Find(x => x.Id == "codigoOperativa")?.Value;
            string codigoClase = query.Parameters.Find(x => x.Id == "codigoClase")?.Value;
            string codigoGrupo = query.Parameters.Find(x => x.Id == "codigoGrupo")?.Value;
            string producto = query.Parameters.Find(x => x.Id == "producto")?.Value;
            string codigoReferencia = query.Parameters.Find(x => x.Id == "codigoReferencia")?.Value;
            string codigoAgrupador = query.Parameters.Find(x => x.Id == "codigoAgrupador")?.Value;
            string enderecoSugerido = query.Parameters.Find(x => x.Id == "enderecoSugerido")?.Value;
            long nuAlmSugerencia = long.Parse(query.Parameters.Find(x => x.Id == "nuAlmSugerencia")?.Value);

            PanelDeSugerenciasDetalleQuery dbQuery = new PanelDeSugerenciasDetalleQuery(
                estrategia,
                predio,
                tipoOperativa,
                codigoOperativa,
                codigoClase,
                codigoGrupo,
                empresa,
                producto,
                codigoReferencia,
                codigoAgrupador,
                enderecoSugerido,
                nuAlmSugerencia
            );

            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int empresa;
            int estrategia;

            string empresaParam = query.Parameters.Find(x => x.Id == "empresa")?.Value;
            string estrategiaParam = query.Parameters.Find(x => x.Id == "empresa")?.Value;

            if (!int.TryParse(empresaParam, out empresa))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            if (!int.TryParse(estrategiaParam, out estrategia))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            string predio = query.Parameters.Find(x => x.Id == "predio")?.Value;
            string tipoOperativa = query.Parameters.Find(x => x.Id == "tipoOperativa")?.Value;
            string codigoOperativa = query.Parameters.Find(x => x.Id == "codigoOperativa")?.Value;
            string codigoClase = query.Parameters.Find(x => x.Id == "codigoClase")?.Value;
            string codigoGrupo = query.Parameters.Find(x => x.Id == "codigoGrupo")?.Value;
            string producto = query.Parameters.Find(x => x.Id == "producto")?.Value;
            string codigoReferencia = query.Parameters.Find(x => x.Id == "codigoReferencia")?.Value;
            string codigoAgrupador = query.Parameters.Find(x => x.Id == "codigoAgrupador")?.Value;
            string enderecoSugerido = query.Parameters.Find(x => x.Id == "enderecoSugerido")?.Value;
            long nuAlmSugerencia = long.Parse(query.Parameters.Find(x => x.Id == "nuAlmSugerencia")?.Value);
            PanelDeSugerenciasDetalleQuery dbQuery = new PanelDeSugerenciasDetalleQuery(
                estrategia,
                predio,
                tipoOperativa,
                codigoOperativa,
                codigoClase,
                codigoGrupo,
                empresa,
                producto,
                codigoReferencia,
                codigoAgrupador,
                enderecoSugerido,
                nuAlmSugerencia
            );

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

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            int empresa;
            int estrategia;

            string empresaParam = query.Parameters.Find(x => x.Id == "empresa")?.Value;
            string estrategiaParam = query.Parameters.Find(x => x.Id == "empresa")?.Value;

            if (!int.TryParse(empresaParam, out empresa))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            if (!int.TryParse(estrategiaParam, out estrategia))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            string predio = query.Parameters.Find(x => x.Id == "predio")?.Value;
            string tipoOperativa = query.Parameters.Find(x => x.Id == "tipoOperativa")?.Value;
            string codigoOperativa = query.Parameters.Find(x => x.Id == "codigoOperativa")?.Value;
            string codigoClase = query.Parameters.Find(x => x.Id == "codigoClase")?.Value;
            string codigoGrupo = query.Parameters.Find(x => x.Id == "codigoGrupo")?.Value;
            string producto = query.Parameters.Find(x => x.Id == "producto")?.Value;
            string codigoReferencia = query.Parameters.Find(x => x.Id == "codigoReferencia")?.Value;
            string codigoAgrupador = query.Parameters.Find(x => x.Id == "codigoAgrupador")?.Value;
            string enderecoSugerido = query.Parameters.Find(x => x.Id == "enderecoSugerido")?.Value;
            long nuAlmSugerencia = long.Parse(query.Parameters.Find(x => x.Id == "nuAlmSugerencia")?.Value);

            PanelDeSugerenciasDetalleQuery dbQuery = new PanelDeSugerenciasDetalleQuery(
                estrategia,
                predio,
                tipoOperativa,
                codigoOperativa,
                codigoClase,
                codigoGrupo,
                empresa,
                producto,
                codigoReferencia,
                codigoAgrupador,
                enderecoSugerido,
                nuAlmSugerencia
            );

            uow.HandleQuery(dbQuery);

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }

        #endregion

        #region FORM

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            string estrategia = context.GetParameter("estrategia");
            string descripcionEstrategia = context.GetParameter("descripcionEstrategia");
            string codigoOperativa = context.GetParameter("codigoOperativa");
            string tipoOperativa = context.GetParameter("tipoOperativa");
            string predio = context.GetParameter("predio");

            form.GetField("estrategia").Value = descripcionEstrategia;
            form.GetField("codigoOperativa").Value = codigoOperativa;
            form.GetField("tipoOperativa").Value = tipoOperativa;
            form.GetField("predio").Value = predio;

            return form;
        }
        #endregion
    }
}
