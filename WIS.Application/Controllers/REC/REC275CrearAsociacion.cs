using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Documento;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.General;
using WIS.Domain.Recepcion;
using WIS.Exceptions;
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
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.REC
{
    public class REC275CrearAsociacion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<REC275CrearAsociacion> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REC275CrearAsociacion(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            IIdentityService identity,
            ISecurityService security,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter,
            ILogger<REC275CrearAsociacion> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "NU_PREDIO", "TP_ENTIDAD", "CD_ENTIDAD", "CD_EMPRESA", "TP_ALM_OPERATIVA_ASOCIABLE", "TP_RECEPCION",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("TP_RECEPCION", SortDirection.Ascending),
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

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            query.IsAddEnabled = false;
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = false;
            query.IsCommitEnabled = false;

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            GetParametros(query, out string estrategia, out string predio, out string tpEntidad, out string cdEntidad, out string dsEntidad, out string cdEmpresa, out string dsEmpresa);

            int.TryParse(estrategia, out int nuEstrategia);

            if (!string.IsNullOrEmpty(predio) && !string.IsNullOrEmpty(tpEntidad) && !string.IsNullOrEmpty(cdEntidad))
            {
                OprerativaAsociableQuery dbQuery = new OprerativaAsociableQuery(predio, tpEntidad, cdEntidad, dsEntidad, cdEmpresa, dsEmpresa, nuEstrategia);

                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

                foreach (var row in grid.Rows)
                {
                    var estrategiaAsociada = row.GetCell("NU_ALM_ESTRATEGIA")?.Value;
                    var tpEntidadAsociada = row.GetCell("TP_ENTIDAD")?.Value;
                    var cdEntidadAsociada = row.GetCell("CD_ENTIDAD")?.Value;
                    var cdEmpresaAsociada = row.GetCell("CD_EMPRESA")?.Value;
                    var predioAsociado = row.GetCell("NU_PREDIO")?.Value;
                    var asociada = row.GetCell("FL_ASOCIADA");

                    if (estrategiaAsociada == estrategia)
                        asociada.Value = "S";

                    asociada.Editable = string.IsNullOrEmpty(estrategiaAsociada);
                }
            }
            return grid;
        }
        
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var count = 0;

            GetParametros(query, out string estrategia, out string predio, out string tpEntidad, out string cdEntidad, out string dsEntidad, out string cdEmpresa, out string dsEmpresa);

            int.TryParse(estrategia, out int nuEstrategia);

            if (!string.IsNullOrEmpty(predio))
            {
                OprerativaAsociableQuery dbQuery = new OprerativaAsociableQuery(predio, tpEntidad, cdEntidad, dsEntidad, cdEmpresa, dsEmpresa, nuEstrategia);

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                count = dbQuery.GetCount();
            }

            return new GridStats
            {
                Count = count
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            GetParametros(query, out string estrategia, out string predio, out string tpEntidad, out string cdEntidad, out string dsEntidad, out string cdEmpresa, out string dsEmpresa);

            int.TryParse(estrategia, out int nuEstrategia);

            if (!string.IsNullOrEmpty(predio))
            {
                OprerativaAsociableQuery dbQuery = new OprerativaAsociableQuery(predio, tpEntidad, cdEntidad, dsEntidad, cdEmpresa, dsEmpresa, nuEstrategia);
                uow.HandleQuery(dbQuery);

                var columnaAsociada = grid.Columns.Where(p => p.Id == "FL_ASOCIADA").FirstOrDefault();
                grid.Columns.Remove(columnaAsociada);

                return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
            }

            return new byte[0];
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var cdEstrategia = context.Parameters.FirstOrDefault(s => s.Id == "codigoEstrategia")?.Value;

            form.GetField("estrategia").Value = cdEstrategia;

            var estrategia = uow.EstrategiaRepository.GetEstrategiaByCod(cdEstrategia);
            if (estrategia != null)
                form.GetField("descripcionEstrategia").Value = estrategia.Descripcion;

            this.InicializarSelect(uow, form);

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new REC275CrearAsociacionValidationModule(uow, this._identity), form, context);
        }
        
        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            MantenimientoPanelDeEstrategias mantenimientoEstrategias = new MantenimientoPanelDeEstrategias(uow, this._identity.UserId, this._identity.Application);

            try
            {
                List<GridRow> rowsEntrada = JsonConvert.DeserializeObject<List<GridRow>>(context.GetParameter("rowsEntrada"));

                if (rowsEntrada.Count > 0)
                {
                    string estrategia = form.GetField("estrategia").Value;
                    string predio = form.GetField("predio").Value;
                    string tipoAsociacion = form.GetField("asociacion").Value;

                    List<AsociacionEstrategia> lineasParametros = this.MapFormulaEntrada(rowsEntrada, estrategia, predio, tipoAsociacion, form);

                    if (lineasParametros.Count != 0)
                    {
                        foreach (var parametro in lineasParametros)
                        {
                            mantenimientoEstrategias.RegistrarAsociacionEstrategia(parametro);
                        }
                    }
                    else
                    {
                        throw new ValidationFailedException("General_Sec0_Error_SeleccionarOperativa");
                    }
                }
                else
                {
                    throw new ValidationFailedException("General_Sec0_Error_SeleccionarOperativa");
                }


                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw ex;
            }

            return form;
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "empresa":
                    return this.SearchEmpresa(form, context);

                case "producto":
                    return this.SearchProduto(form, context);

                default:
                    return new List<SelectOption>();
            }
        }

        #region Metodos Auxiliares
        protected virtual void GetParametros(ComponentContext query, out string estrategia, out string predio, out string tpEntidad, out string cdEntidad, out string dsEntidad, out string cdEmpresa, out string dsEmpresa)
        {
            estrategia = "";
            predio = "";
            tpEntidad = "";
            cdEntidad = "";
            dsEntidad = "";
            cdEmpresa = "";
            dsEmpresa = "";

            if (!string.IsNullOrEmpty(query.GetParameter("predio")))
            {
                estrategia = query.GetParameter("estrategia");
                predio = query.GetParameter("predio");
                cdEmpresa = query.GetParameter("codigoEmpresa");
                dsEmpresa = query.GetParameter("descripcionEmpresa");
                tpEntidad = query.GetParameter("tipoEntidad");
                cdEntidad = query.GetParameter("codigoEntidad");
                dsEntidad = query.GetParameter("descripcionEntidad");
            }
        }

        public virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                List<Empresa> empresasAsignadasUsuario = uow.EmpresaRepository.GetByNombreOrCodePartialForUsuario(context.SearchValue, this._identity.UserId);

                foreach (var emp in empresasAsignadasUsuario)
                {
                    opciones.Add(new SelectOption(emp.Id.ToString(), $"{emp.Id} - {emp.Nombre}"));
                }
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchProduto(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();
            List<Producto> productos = new List<Producto>();

            using var uow = this._uowFactory.GetUnitOfWork();
            string codigoEmpresa = form.GetField("empresa").Value;

            if (!string.IsNullOrEmpty(codigoEmpresa))
            {
                productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(int.Parse(codigoEmpresa), context.SearchValue);

                foreach (var prod in productos)
                {
                    opciones.Add(new SelectOption(prod.Codigo, $"{prod.Codigo} - {prod.Descripcion}"));
                }
            }

            return opciones;
        }

        public virtual void InicializarSelect(IUnitOfWork uow, Form form)
        {
            FormField selectorPredio = form.GetField("predio");
            FormField selectorAsociacion = form.GetField("asociacion");
            FormField selectorGrupo = form.GetField("grupo");
            FormField selectorClase = form.GetField("clase");

            selectorPredio.Options = new List<SelectOption>();
            selectorAsociacion.Options = new List<SelectOption>();
            selectorGrupo.Options = new List<SelectOption>();
            selectorClase.Options = new List<SelectOption>();

            List<Predio> predios = uow.PredioRepository.GetPrediosUsuario(this._identity.UserId);
            foreach (var predio in predios)
            {
                selectorPredio.Options.Add(new SelectOption(predio.Numero.ToString(), $"{predio.Numero} - {predio.Descripcion}"));
            }

            selectorAsociacion.Options.Add(new SelectOption(AlmacenamientoDb.TIPO_ENTIDAD_CLASE, "CLASE"));
            selectorAsociacion.Options.Add(new SelectOption(AlmacenamientoDb.TIPO_ENTIDAD_GRUPO, "GRUPO"));
            //selectorAsociacion.Options.Add(new SelectOption(AlmacenamientoDb.TIPO_ENTIDAD_PRODUCTO, "PRODUCTO"));

            var grupos = uow.GrupoRepository.GetGrupos();
            foreach (var grupo in grupos)
            {
                selectorGrupo.Options.Add(new SelectOption(grupo.Id, $"{grupo.Id} - {grupo.Descripcion}"));

            }
            List<Clase> clases = uow.ClaseRepository.GetClases();
            foreach (var clase in clases)
            {
                selectorClase.Options.Add(new SelectOption(clase.Id.ToString(), $"{clase.Id} - {clase.Descripcion}"));
            }
        }

        public virtual List<AsociacionEstrategia> MapFormulaEntrada(List<GridRow> rowsEntrada, string numeroEstrategia, string predio, string tipoAsociacion, Form form)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var lineasEntrada = new List<AsociacionEstrategia>();

            foreach (var row in rowsEntrada)
            {
                string asociar = row.GetCell("FL_ASOCIADA").Value;

                if (asociar == "S")
                {
                    string codigoOperativa = row.GetCell("TP_RECEPCION").Value;
                    var tipo = row.GetCell("TP_ALM_OPERATIVA_ASOCIABLE").Value;
                    AsociacionEstrategia asociacion = new AsociacionEstrategia();

                    if (tipoAsociacion == AlmacenamientoDb.TIPO_ENTIDAD_CLASE)
                    {
                        asociacion.Clase = form.GetField("clase").Value;
                    }

                    if (tipoAsociacion == AlmacenamientoDb.TIPO_ENTIDAD_PRODUCTO)
                    {
                        asociacion.Empresa = int.Parse(form.GetField("empresa").Value);
                        asociacion.Producto = form.GetField("producto").Value;
                    }

                    if (tipoAsociacion == AlmacenamientoDb.TIPO_ENTIDAD_GRUPO)
                    {
                        asociacion.Grupo = form.GetField("grupo").Value;
                    }

                    asociacion.FechaRegistro = DateTime.Now;
                    asociacion.Estrategia = int.Parse(numeroEstrategia);
                    asociacion.Predio = predio;
                    asociacion.Tipo = tipoAsociacion;
                    asociacion.Operativa = new OperativaAlmacenaje
                    {
                        Codigo = codigoOperativa,
                        Tipo = tipo,
                    };

                    lineasEntrada.Add(asociacion);
                }
            }

            return lineasEntrada;
        }
        #endregion
    }
}
