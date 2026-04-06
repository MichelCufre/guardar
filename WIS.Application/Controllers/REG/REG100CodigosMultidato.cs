using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Domain.General;
using WIS.Domain.Recepcion;
using WIS.Exceptions;
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
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.REG
{
    public class REG100CodigosMultidato : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ILogger<REG100CodigosMultidato> _logger;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REG100CodigosMultidato(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            ILogger<REG100CodigosMultidato> logger,
            IFormValidationService formValidationService,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "CD_EMPRESA",
                "CD_CODIGO_MULTIDATO",
                "CD_APLICACION",
                "CD_CAMPO",
                "CD_AI"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_CODIGO_MULTIDATO", SortDirection.Ascending),
                new SortCommand("CD_APLICACION", SortDirection.Ascending),
                new SortCommand("CD_CAMPO", SortDirection.Ascending),
                new SortCommand("NU_ORDEN", SortDirection.Ascending),
            };

            this._uowFactory = uowFactory;
            this._logger = logger;
            this._formValidationService = formValidationService;
            this._gridValidationService = gridValidationService;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        #region FORM

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var codigoEmpresa = int.Parse(query.GetParameter("empresa"));
            var codigoMultidato = query.GetParameter("codigoMultidato");

            var empresa = uow.EmpresaRepository.GetEmpresa(codigoEmpresa);
            var descripcionCodigo = uow.CodigoMultidatoRepository.GetCodigoMultidato(codigoMultidato).Descripcion;

            form.GetField("codigoEmpresa").Value = empresa.Id.ToString();
            form.GetField("nombreEmpresa").Value = empresa.Nombre.ToString();
            form.GetField("codigoMultidato").Value = codigoMultidato;
            form.GetField("descripcionCodigo").Value = descripcionCodigo;

            return form;
        }

        #endregion

        #region GRID

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsAddEnabled = true;
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = true;

            grid.SetInsertableColumns(new List<string> {
                "CD_APLICACION",
                "CD_CAMPO",
                "CD_AI"
            });

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_UP", new List<GridButton>
            {
                new GridButton("btnUp", "REC275_frm1_btn_Subir", "fas fa-arrow-up"),
            }));

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_DOWN", new List<GridButton>
            {
                new GridButton("btnDown", "REC275_frm1_btn_Bajar", "fas fa-arrow-down"),
            }));

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "empresa").Value, out int codigoEmpresa))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            var dbQuery = new CodigosMultidatoQuery(codigoEmpresa);

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            DisableButtons(grid, uow, query);

            return grid;
        }

        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext query)
        {
            switch (query.ColumnId)
            {
                case "CD_APLICACION":
                    return this.SearchAplicacion(grid, row, query);
                case "CD_CAMPO":
                    return this.SearchCampo(grid, row, query);
                case "CD_AI":
                    return this.SearchCodigoAi(grid, row, query);
            }

            return new List<SelectOption>();
        }

        public virtual List<SelectOption> SearchAplicacion(Grid grid, GridRow row, GridSelectSearchContext query)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            List<SelectOption> opciones = new List<SelectOption>();

            List<Aplicacion> aplicaciones = uow.CodigoMultidatoRepository.GetAplicacionesHabilitadaByDescriptionOrCodePartial(query.SearchValue);

            foreach (var aplicacion in aplicaciones)
            {
                opciones.Add(new SelectOption(aplicacion.Codigo, aplicacion.Codigo + " - " + aplicacion.Descripcion));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchCampo(Grid grid, GridRow row, GridSelectSearchContext query)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var aplicacion = row.GetCell("CD_APLICACION").Value;
            var codigoMultidato = query.GetParameter("codigoMultidato");
            var empresa = int.Parse(query.GetParameter("empresa"));

            List<SelectOption> opciones = new List<SelectOption>();

            if (string.IsNullOrEmpty(aplicacion)
                || string.IsNullOrEmpty(codigoMultidato))
                return opciones;

            List<AplicacionCampo> campos = uow.CodigoMultidatoRepository.GetCampoHabilitadoByDescriptionOrCodePartial(aplicacion, query.SearchValue, empresa, codigoMultidato);

            foreach (var campo in campos)
            {
                opciones.Add(new SelectOption(campo.CodigoCampo, campo.CodigoCampo + " - " + campo.Descripcion));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchCodigoAi(Grid grid, GridRow row, GridSelectSearchContext query)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var codigoMultidato = query.GetParameter("codigoMultidato");

            List<SelectOption> opciones = new List<SelectOption>();

            if (string.IsNullOrEmpty(codigoMultidato))
                return opciones;

            List<DetalleCodigoMultidato> codigosAi = uow.CodigoMultidatoRepository.GetCodigoAiByDescriptionOrCodePartial(codigoMultidato, query.SearchValue);

            foreach (var codigo in codigosAi)
            {
                opciones.Add(new SelectOption(codigo.CodigoAI, codigo.CodigoAI + " - " + codigo.Descripcion));
            }

            return opciones;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "empresa").Value, out int codigoEmpresa))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            var dbQuery = new CodigosMultidatoQuery(codigoEmpresa);

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

            if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "empresa").Value, out int codigoEmpresa))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            var dbQuery = new CodigosMultidatoQuery(codigoEmpresa);

            uow.HandleQuery(dbQuery);


            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new REG100CodigosMultidatoGridValidationModule(uow), grid, row, context);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                try
                {
                    if (grid.Rows.Any())
                    {
                        if (grid.HasNewDuplicates(this.GridKeys))
                            throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                        uow.CreateTransactionNumber(this._identity.Application);
                        uow.BeginTransaction();
                        var nuTransaccion = uow.GetTransactionNumber();

                        var empresa = int.Parse(query.GetParameter("empresa"));

                        foreach (var row in grid.Rows)
                        {
                            if (row.IsNew)
                            {
                                var nuevoCodigo = CrearObjetoCodigoEmpresa(uow, row, query, empresa, nuTransaccion);
                                nuevoCodigo.NumeroOrden = uow.CodigoMultidatoRepository.ObtenerProximoNuOrden(empresa, nuevoCodigo.CodigoMultidato, nuevoCodigo.CodigoAplicacion, nuevoCodigo.CodigoCampo);

                                uow.CodigoMultidatoRepository.AddDetalleCodigoMultidatoEmpresa(nuevoCodigo);
                            }
                            else if (row.IsDeleted)
                            {
                                var codigoMultidato = row.GetCell("CD_CODIGO_MULTIDATO").Value;
                                var aplicacion = row.GetCell("CD_APLICACION").Value;
                                var campo = row.GetCell("CD_CAMPO").Value;
                                var codigoAI = row.GetCell("CD_AI").Value;

                                var codigo = uow.CodigoMultidatoRepository.GetDetalleCodigoMultidatoEmpresa(empresa, codigoMultidato, aplicacion, campo, codigoAI);

                                codigo.FechaModificacion = DateTime.Now;
                                codigo.NumeroTransaccion = nuTransaccion;
                                codigo.NumeroTransaccionDelete = nuTransaccion;

                                uow.CodigoMultidatoRepository.UpdateDetalleCodigoMultidatoEmpresa(codigo);
                                uow.SaveChanges();

                                var codigosPosteriores = uow.CodigoMultidatoRepository.GetDetalleCodigoMultidatoEmpresaPosteriores(codigo);

                                foreach (var codigoPosterior in codigosPosteriores)
                                {
                                    codigoPosterior.NumeroOrden--;
                                    codigoPosterior.FechaModificacion = DateTime.Now;
                                    codigoPosterior.NumeroTransaccion = nuTransaccion;
                                    codigoPosterior.NumeroTransaccionDelete = nuTransaccion;

                                    uow.CodigoMultidatoRepository.UpdateDetalleCodigoMultidatoEmpresa(codigoPosterior);
                                }

                                uow.CodigoMultidatoRepository.DeleteDetalleCodigoMultidatoEmpresa(codigo);
                            }
                        }
                    }

                    uow.SaveChanges();
                    uow.Commit();

                    query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
                }
                catch (ExpectedException ex)
                {
                    query.AddErrorNotification(ex.Message);
                    uow.Rollback();
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "REG100GridCommitCodigosMultidato");
                    query.AddErrorNotification("General_Sec0_Error_Operacion");
                    uow.Rollback();
                }
            }

            return grid;
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                uow.CreateTransactionNumber(this._identity.Application);
                uow.BeginTransaction();
                var nuTransaccion = uow.GetTransactionNumber();

                var empresa = int.Parse(context.GetParameter("empresa"));
                var codigoMultidato = context.GetParameter("codigoMultidato");

                switch (context.ButtonId)
                {
                    case "btnUp":
                        try
                        {
                            var codigoAplicacion = context.Row.GetCell("CD_APLICACION").Value;
                            var codigoCampo = context.Row.GetCell("CD_CAMPO").Value;
                            var codigoAI = context.Row.GetCell("CD_AI").Value;

                            var codigoMultidatoEmpresaSubir = uow.CodigoMultidatoRepository.GetDetalleCodigoMultidatoEmpresa(empresa, codigoMultidato, codigoAplicacion, codigoCampo, codigoAI);

                            short numeroOrden = Convert.ToInt16(codigoMultidatoEmpresaSubir.NumeroOrden - 1);

                            var codigoMultidatoEmpresaBajar = uow.CodigoMultidatoRepository.GetDetalleCodigoMultidatoEmpresaCambiarOrden(empresa, codigoMultidato, codigoAplicacion, codigoCampo, numeroOrden);

                            codigoMultidatoEmpresaSubir.NumeroOrden--;
                            codigoMultidatoEmpresaSubir.FechaModificacion = DateTime.Now;
                            codigoMultidatoEmpresaSubir.NumeroTransaccion = nuTransaccion;

                            codigoMultidatoEmpresaBajar.NumeroOrden++;
                            codigoMultidatoEmpresaBajar.FechaModificacion = DateTime.Now;
                            codigoMultidatoEmpresaBajar.NumeroTransaccion = nuTransaccion;

                            uow.CodigoMultidatoRepository.UpdateDetalleCodigoMultidatoEmpresa(codigoMultidatoEmpresaSubir);
                            uow.CodigoMultidatoRepository.UpdateDetalleCodigoMultidatoEmpresa(codigoMultidatoEmpresaBajar);

                            uow.SaveChanges();
                            uow.Commit();
                        }
                        catch (Exception ex)
                        {
                            uow.Rollback();
                        }

                        break;

                    case "btnDown":
                        try
                        {
                            var codigoAplicacion = context.Row.GetCell("CD_APLICACION").Value;
                            var codigoCampo = context.Row.GetCell("CD_CAMPO").Value;
                            var codigoAI = context.Row.GetCell("CD_AI").Value;

                            var codigoMultidatoEmpresaBajar = uow.CodigoMultidatoRepository.GetDetalleCodigoMultidatoEmpresa(empresa, codigoMultidato, codigoAplicacion, codigoCampo, codigoAI);

                            short numeroOrden = Convert.ToInt16(codigoMultidatoEmpresaBajar.NumeroOrden + 1);

                            var codigoMultidatoEmpresaSubir = uow.CodigoMultidatoRepository.GetDetalleCodigoMultidatoEmpresaCambiarOrden(empresa, codigoMultidato, codigoAplicacion, codigoCampo, numeroOrden);

                            codigoMultidatoEmpresaBajar.NumeroOrden++;
                            codigoMultidatoEmpresaBajar.FechaModificacion = DateTime.Now;
                            codigoMultidatoEmpresaBajar.NumeroTransaccion = nuTransaccion;

                            codigoMultidatoEmpresaSubir.NumeroOrden--;
                            codigoMultidatoEmpresaSubir.FechaModificacion = DateTime.Now;
                            codigoMultidatoEmpresaSubir.NumeroTransaccion = nuTransaccion;


                            uow.CodigoMultidatoRepository.UpdateDetalleCodigoMultidatoEmpresa(codigoMultidatoEmpresaSubir);
                            uow.CodigoMultidatoRepository.UpdateDetalleCodigoMultidatoEmpresa(codigoMultidatoEmpresaBajar);

                            uow.SaveChanges();
                            uow.Commit();
                        }
                        catch (Exception ex)
                        {
                            uow.Rollback();
                        }

                        break;
                }
            }

            return context;
        }


        #endregion

        #region AUX

        public virtual CodigoMultidatoEmpresaDetalle CrearObjetoCodigoEmpresa(IUnitOfWork uow, GridRow row, GridFetchContext query, int empresa, long nuTrasnaction)
        {
            return new CodigoMultidatoEmpresaDetalle
            {
                CodigoEmpresa = empresa,
                CodigoMultidato = query.GetParameter("codigoMultidato"),
                CodigoAplicacion = row.GetCell("CD_APLICACION").Value,
                CodigoCampo = row.GetCell("CD_CAMPO").Value,
                CodigoAI = row.GetCell("CD_AI").Value,
                FechaAlta = DateTime.Now,
                NumeroTransaccion = nuTrasnaction,
            };
        }

        public virtual void DisableButtons(Grid grid, IUnitOfWork uow, GridFetchContext query)
        {
            var empresa = int.Parse(query.GetParameter("empresa"));
            var codigoMultidato = query.GetParameter("codigoMultidato");

            foreach (var row in grid.Rows)
            {
                var codigoAplicacion = row.GetCell("CD_APLICACION").Value;
                var codigoCampo = row.GetCell("CD_CAMPO").Value;
                var codigoAI = row.GetCell("CD_AI").Value;

                var codigoMultidatoEmpresa = uow.CodigoMultidatoRepository.GetDetalleCodigoMultidatoEmpresa(empresa, codigoMultidato, codigoAplicacion, codigoCampo, codigoAI);
                var codigosPosteriores = uow.CodigoMultidatoRepository.GetDetalleCodigoMultidatoEmpresaPosteriores(codigoMultidatoEmpresa);

                if (codigoMultidatoEmpresa.NumeroOrden == 1 && codigosPosteriores.Count == 0)
                {
                    row.DisabledButtons.Add("btnUp");
                    row.DisabledButtons.Add("btnDown");
                }

                if (codigoMultidatoEmpresa.NumeroOrden == 1)
                    row.DisabledButtons.Add("btnUp");

                if (codigosPosteriores.Count == 0)
                    row.DisabledButtons.Add("btnDown");
            }
        }

        #endregion

    }
}