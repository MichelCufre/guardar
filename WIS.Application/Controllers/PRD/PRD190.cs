using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Produccion;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Mappers.Produccion;
using WIS.Domain.DataModel.Queries.Produccion;
using WIS.Domain.General;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Constants;
using WIS.Domain.Produccion.Enums;
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
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRD
{
    public class PRD190 : AppController
    {
        protected readonly ISessionAccessor _session;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected List<string> GridKeys { get; }

        public PRD190(ISessionAccessor session,
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFormValidationService formValidationService,
            IGridValidationService gridValidationService,
            IFilterInterpreter filterInterpreter)
        {
            this._session = session;
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._formValidationService = formValidationService;
            this._gridValidationService = gridValidationService;
            this._filterInterpreter = filterInterpreter;

            this.GridKeys = new List<string>
            {
                "CD_PRDC_LINEA"
            };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsAddEnabled = false;
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = true;

            grid.SetInsertableColumns(new List<string>
            {
                "DS_PRDC_LINEA"
            });

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton> {
                new GridButton("btnIdentificadores", "General_Sec0_btn_Editar", "fas fa-edit")
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var dbQuery = new LineaProduccionQuery();

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("CD_PRDC_LINEA", SortDirection.Descending);

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);

                this.FormatGrid1(grid, uow);
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var dbQuery = new LineaProduccionQuery();

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("CD_PRDC_LINEA", SortDirection.Descending);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                try
                {
                    if (grid.Rows.Any())
                    {

                        if (grid.HasNewDuplicates(this.GridKeys))
                            throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                        foreach (var row in grid.Rows)
                        {
                            var numerolinea = row.GetCell("CD_PRDC_LINEA").Value;
                            var descripcion = row.GetCell("DS_PRDC_LINEA").Value;

                            if (!row.IsNew)
                            {
                                if (row.IsDeleted)
                                    this.RemoveLinea(uow, numerolinea);
                                else // Lineas editadas
                                    this.UpdateLinea(uow, numerolinea, descripcion);
                            }
                        }

                    }

                    uow.SaveChanges();

                    context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
                }
                catch (ValidationFailedException ex)
                {
                    this._logger.Error(ex, ex.Message);
                    context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                }
                catch (Exception ex)
                {
                    this._logger.Error(ex, ex.Message);
                    throw ex;
                }
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new LineaProduccionQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }


        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            SelectPredio(form);
            SelectTipoLinea(form);

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                try
                {
                    var descripcion = form.GetField("descripcion").Value;
                    var tipoLinea = form.GetField("tipoLinea").Value;
                    var predio = form.GetField("predio").Value;

                    // Creación de la linea de producción
                    var linea = this.CrearLineaProduccion(descripcion, tipoLinea, predio, uow);
                    uow.LineaRepository.AddLinea(linea);
                    uow.SaveChanges();

                    context.AddSuccessNotification("PRD190_Sec0_Succes_Succes01", new List<string> { linea.Id });
                }
                catch (Exception ex)
                {
                    this._logger.Error(ex, ex.Message);
                    throw ex;
                }
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new PRD111FormValidationModule(uow, this._identity), form, context);
        }

        #region Metodos Auxiliares
        public virtual void FormatGrid1(Grid grid, IUnitOfWork uow)
        {
            foreach (var row in grid.Rows)
            {
                row.SetEditableCells(new List<string>() { "DS_PRDC_LINEA" });
            }
        }

        public virtual void SelectPredio(Form form)
        {
            FormField selectTipoIngreso = form.GetField("predio");
            selectTipoIngreso.Options = new List<SelectOption>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                List<Predio> predios = uow.PredioRepository.GetPrediosUsuario(this._identity.UserId);

                foreach (Predio predio in predios)
                {
                    selectTipoIngreso.Options.Add(new SelectOption(predio.Numero, predio.Descripcion));
                }

                selectTipoIngreso.Value = predios.FirstOrDefault().Numero;
            }
        }

        public virtual void SelectTipoLinea(Form form)
        {
            FormField selectTipoIngreso = form.GetField("tipoLinea");
            selectTipoIngreso.Options = new List<SelectOption>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                List<DominioDetalle> dominiosTipoLinea = uow.DominioRepository.GetDominios(TipoLineaProduccion.DominioTipoLinea);

                foreach (DominioDetalle tipoLinea in dominiosTipoLinea)
                {
                    selectTipoIngreso.Options.Add(new SelectOption(tipoLinea.Id, tipoLinea.Descripcion));
                }

                selectTipoIngreso.Value = dominiosTipoLinea.LastOrDefault().Id;
            }
        }

        public virtual void SelectPredios(Form form)
        {
            FormField selectPredio = form.GetField("predio");
            selectPredio.Options = new List<SelectOption>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                List<string> predios = uow.SecurityRepository.GetPrediosUsuario(this._identity.UserId);

                foreach (var predio in predios)
                {
                    selectPredio.Options.Add(new SelectOption(predio, predio));
                }


            }
        }

        public virtual ILinea CrearLineaProduccion(string descripcion, string tipoLinea, string predio, IUnitOfWork uow)
        {
            LineaMapper mapper = new LineaMapper();
            TipoProduccionLinea tpLinea = mapper.MapStringToTipoLinea(tipoLinea);
            ILinea linea = new LineaFactory().Create(tpLinea);

            if (linea != null)
            {
                linea.Id = uow.LineaRepository.GetNumeroLinea();
                linea.Descripcion = descripcion;
                linea.Tipo = tpLinea;
                linea.Predio = predio;
                linea.FechaAlta = DateTime.Now;
            }

            // Crear ubicaciones de linea
            this.AgregarUbicacionesLinea(uow, linea);

            return linea;
        }

        public virtual void AgregarUbicacionesLinea(IUnitOfWork uow, ILinea linea)
        {
            //Para la creación de las ubicaciones de las lineas se toma la siguiente regla: PREDIO+ZP+XXX+YY
            //Predio definido por pantalla + Parametro BloqueCalle + secuenciaLinea (3 digitos) + 01 si es IN, 02 si es OUT , 00 si es BlackBox
            var bloque = uow.ParametroRepository.GetParameter(ParamManager.ProduccionLineaUbicacionBloque);
            var calle = uow.ParametroRepository.GetParameter(ParamManager.ProduccionLineaUbicacionCalle);

            if (string.IsNullOrEmpty(bloque) || string.IsNullOrEmpty(calle))
            {
                throw new ValidationFailedException("PRD190_Sec0_Error_Error01");
            }

            var empresaPropietaria = uow.ParametroRepository.GetParameter(ParamManager.SistemaEmpresaPropietaria);

            if (string.IsNullOrEmpty(empresaPropietaria))
            {
                throw new ValidationFailedException("General_Sec0_Error_Er005_ParametroEmpresa");
            }

            var zonaDefecto = uow.ParametroRepository.GetParameter(ParamManager.ProduccionLineaUbicacionZonaDefecto);

            var columnaUbicacion = linea.Id.PadLeft(3, '0');
            var predio = linea.Predio;

            var alturaEntrada = "01";
            var alturaSalida = "02";
            var alturaBB = "00";

            string formatoUbicacion = "{0}{1}{2}{3}{4}";

            string ubicacionEntrada = string.Format(formatoUbicacion, predio, bloque, calle, columnaUbicacion, alturaEntrada);
            string ubicacionSalida = string.Format(formatoUbicacion, predio, bloque, calle, columnaUbicacion, alturaSalida);
            string ubicacionBlackBox = string.Format(formatoUbicacion, predio, bloque, calle, columnaUbicacion, alturaBB);

            //Creación de ubicacion de entrada y asiganción a la linea
            var entrada = new Ubicacion()
            {
                Id = ubicacionEntrada,
                IdEmpresa = int.Parse(empresaPropietaria),
                IdUbicacionTipo = TipoUbicacionDb.General,
                IdProductoRotatividad = RotatividadDb.NoEspecificada,
                IdProductoFamilia = FamiliaProductoDb.NoEspecificada,
                CodigoClase = ClaseProductoDb.General,
                CodigoSituacion = SituacionDb.UBICACION_ACTIVA,
                EsUbicacionBaja = true,
                NecesitaReabastecer = false,
                IdUbicacionArea = AreaUbicacionDb.StockGeneral,
                IdUbicacionZona = zonaDefecto,
                NumeroPredio = predio,
                Bloque = bloque,
                Calle = calle,
                Columna = int.Parse(columnaUbicacion),
                Altura = int.Parse(alturaEntrada),
                CodigoBarras = $"{BarcodeDb.PREFIX_UBICACION}{ubicacionEntrada}",
                Profundidad = 1
            };

            uow.UbicacionRepository.AddUbicacion(entrada);
            linea.UbicacionEntrada = ubicacionEntrada;

            //Creación de ubicacion de salida y asiganción a la linea
            var salida = new Ubicacion()
            {
                Id = ubicacionSalida,
                IdEmpresa = int.Parse(empresaPropietaria),
                IdUbicacionTipo = TipoUbicacionDb.General,
                IdProductoRotatividad = RotatividadDb.NoEspecificada,
                IdProductoFamilia = FamiliaProductoDb.NoEspecificada,
                CodigoClase = ClaseProductoDb.General,
                CodigoSituacion = SituacionDb.UBICACION_ACTIVA,
                EsUbicacionBaja = true,
                NecesitaReabastecer = false,
                IdUbicacionArea = AreaUbicacionDb.StockGeneral,
                IdUbicacionZona = zonaDefecto,
                NumeroPredio = predio,
                Bloque = bloque,
                Calle = calle,
                Columna = int.Parse(columnaUbicacion),
                Altura = int.Parse(alturaSalida),
                CodigoBarras = $"{BarcodeDb.PREFIX_UBICACION}{ubicacionSalida}",
                Profundidad = 1
            };

            uow.UbicacionRepository.AddUbicacion(salida);
            linea.UbicacionSalida = ubicacionSalida;

            //Creación de ubicacion de blackbox y asiganción a la linea si corresponde al tipo de linea
            if (linea.Tipo == TipoProduccionLinea.BlackBox)
            {
                var blackbox = new Ubicacion()
                {
                    Id = ubicacionBlackBox,
                    IdEmpresa = int.Parse(empresaPropietaria),
                    IdUbicacionTipo = TipoUbicacionDb.General,
                    IdProductoRotatividad = RotatividadDb.NoEspecificada,
                    IdProductoFamilia = FamiliaProductoDb.NoEspecificada,
                    CodigoClase = ClaseProductoDb.General,
                    CodigoSituacion = SituacionDb.UBICACION_ACTIVA,
                    EsUbicacionBaja = true,
                    NecesitaReabastecer = false,
                    IdUbicacionArea = AreaUbicacionDb.BlackBox,
                    IdUbicacionZona = zonaDefecto,
                    NumeroPredio = predio,
                    Bloque = bloque,
                    Calle = calle,
                    Columna = int.Parse(columnaUbicacion),
                    Altura = int.Parse(alturaBB),
                    CodigoBarras = $"{BarcodeDb.PREFIX_UBICACION}{ubicacionBlackBox}",
                    Profundidad = 1
                };

                uow.UbicacionRepository.AddUbicacion(blackbox);

                LineaBlackBox lineaBlackBox = (LineaBlackBox)linea;
                lineaBlackBox.UbicacionBlackBox = ubicacionBlackBox;
            }
        }

        public virtual void RemoveLinea(IUnitOfWork uow, string numeroLinea)
        {
            var linea = uow.LineaRepository.GetLinea(numeroLinea);

            if (linea == null)
            {
                throw new ValidationFailedException("PRD190_Sec0_Error_Error02");
            }

            // Control que no exista ordenes de produccion activa
            if (uow.ProduccionRepository.ExisteIngresoActivoEnLinea(linea.Id))
            {
                throw new ValidationFailedException("PRD190_Sec0_Error_Error04");
            }

            //Controlar que las ubicacion de entrada no tenga stock asociado
            if (uow.StockRepository.UbicacionConReserva(linea.UbicacionEntrada))
            {
                throw new ValidationFailedException("PRD190_Sec0_Error_Error03");
            }

            uow.LineaRepository.RemoveLinea(linea);

            // Actualmente no borra las ubicaciones creadas en la linea, en caso de realizarlo 
            // verificar que no exista stock en dichas ubicaciones.
        }

        public virtual void UpdateLinea(IUnitOfWork uow, string numeroLinea, string descripcion)
        {
            var linea = uow.LineaRepository.GetLinea(numeroLinea);

            if (linea == null)
            {
                throw new ValidationFailedException("PRD190_Sec0_Error_Error02");
            }

            linea.Descripcion = descripcion;

            uow.LineaRepository.UpdateLinea(linea);
        }

        #endregion
    }
}
