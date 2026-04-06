using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Facturacion;
using WIS.Domain.Facturacion;
using WIS.Exceptions;
using WIS.Extension;
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

namespace WIS.Application.Controllers.FAC
{
    public class FAC001PanelFacturacion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<FAC001PanelFacturacion> _logger;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public FAC001PanelFacturacion(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            ISecurityService security,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter,
            ILogger<FAC001PanelFacturacion> logger)
        {
            this.GridKeys = new List<string>
            {
                "NU_EJECUCION",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_EJECUCION", SortDirection.Descending),
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._security = security;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._formValidationService = formValidationService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem>
            {
                new GridButton("btnEditar", "FAC001_grid1_btn_Editar", "fas fa-edit"),
                new GridButton("btnAsignarProcesos", "FAC001_grid1_btn_AsignarProcesos", "fas fa-plus-square"),
                new GridButton("btnDetalles", "FAC001_grid1_btn_Detalles", "fas fa-list"),
                new GridButton("btnAnularEjecucion", "FAC001_grid1_btn_AnularEjecucion", "fas fa-ban", new ConfirmMessage("FAC001_grid1_btn_ConfirmarAnularEjecucion")),
                new GridButton("btnHabilitarEjecucion", "FAC001_grid1_btn_HabilitarEjecucion", "fas fa-play"),
                new GridButton("btnIngresoResultadoManual", "FAC001_grid1_btn_IngresoResultadoManual", "fas fa-calculator"),
                new GridButton("btnEdicionDeResultados", "FAC001_grid1_btn_EdicionDeResultados", "fas fa-pen"),
                new GridButton("btnResultadosDeEjecucion", "FAC001_grid1_btn_ResultadosDeEjecucion", "fas fa-list"),
                new GridButton("btnCalculosNoRealizadosPorError", "FAC001_grid1_btn_CalculosNoRealizadosPorError", "fas fa-exclamation-triangle"),
                new GridButton("btnAceptarRechazarCalculos", "FAC001_grid1_btn_AceptarRechazarCalculos", "fas fa-check"),
                new GridButton("btnPreviewPrecios", "FAC001_grid1_btn_btnPreviewPrecios", "fas fa-search-dollar"),
                new GridButton("btnHabilitarFacturacion", "FAC001_grid1_btn_HabilitarFacturacion", "fas fa-dollar-sign"),
                new GridButton("btnDescargarRegistro", "FAC001_grid1_btn_DescargarRegistro", "fas fa-arrow-down"),

            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PanelFacturacionQuery();

            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            DisableButtons(grid.Rows, uow);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PanelFacturacionQuery();

            uow.HandleQuery(dbQuery);

            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PanelFacturacionQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                switch (context.ButtonId)
                {
                    case "btnAsignarProcesos":
                        context.Redirect("/facturacion/FAC010", true, new List<ComponentParameter>()
                        {
                            new ComponentParameter(){ Id = "nuEjecucion", Value = context.Row.GetCell("NU_EJECUCION").Value },
                            new ComponentParameter(){ Id = "parcial", Value = context.Row.GetCell("FL_EJEC_POR_HORA").Value },
                        });
                        break;
                    case "btnDetalles":
                        context.Redirect("/facturacion/FAC004", true, new List<ComponentParameter>()
                        {
                            new ComponentParameter(){ Id = "nuEjecucion", Value = context.Row.GetCell("NU_EJECUCION").Value },
                        });
                        break;
                    case "btnAnularEjecucion":
                        AnularEjecucion(uow, context);
                        break;
                    case "btnHabilitarEjecucion":
                        HabilitarEjecucion(uow, context);
                        break;
                    case "btnIngresoResultadoManual":
                        context.Redirect("/facturacion/FAC007", true, new List<ComponentParameter>()
                        {
                            new ComponentParameter(){ Id = "nuEjecucion", Value = context.Row.GetCell("NU_EJECUCION").Value },
                        });
                        break;
                    case "btnEdicionDeResultados":
                        context.Redirect("/facturacion/FAC009", true, new List<ComponentParameter>()
                        {
                            new ComponentParameter(){ Id = "nuEjecucion", Value = context.Row.GetCell("NU_EJECUCION").Value },
                        });
                        break;
                    case "btnResultadosDeEjecucion":
                        context.Redirect("/facturacion/FAC006", true, new List<ComponentParameter>()
                        {
                            new ComponentParameter(){ Id = "nuEjecucion", Value = context.Row.GetCell("NU_EJECUCION").Value },
                        });
                        break;
                    case "btnCalculosNoRealizadosPorError":
                        context.Redirect("/facturacion/FAC003", true, new List<ComponentParameter>()
                        {
                            new ComponentParameter(){ Id = "nuEjecucion", Value = context.Row.GetCell("NU_EJECUCION").Value },
                        });
                        break;
                    case "btnAceptarRechazarCalculos":
                        context.Redirect("/facturacion/FAC002", true, new List<ComponentParameter>()
                        {
                            new ComponentParameter(){ Id = "nuEjecucion", Value = context.Row.GetCell("NU_EJECUCION").Value },
                        });
                        break;
                    case "btnPreviewPrecios":
                        context.Redirect("/facturacion/FAC012", true, new List<ComponentParameter>()
                        {
                            new ComponentParameter(){ Id = "nuEjecucion", Value = context.Row.GetCell("NU_EJECUCION").Value },
                        });
                        break;
                    case "btnHabilitarFacturacion":
                        HabilitarFacturacion(uow, context);
                        break;
                    case "btnDescargarRegistro":
                        context.Redirect("/api/File/Download", true, new List<ComponentParameter> {
                            new ComponentParameter() { Id = "fileId", Value = context.Row.GetCell("NU_EJECUCION").Value} ,
                            new ComponentParameter() { Id = "application", Value = _identity.Application }
                        });
                        break;

                }
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                throw;
            }

            return context;
        }


        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            ClearForm(form);

            if (int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "nuEjecucion")?.Value, out int nuEjecucion))
            {
                using var uow = this._uowFactory.GetUnitOfWork();
                FacturacionEjecucion facturacionEjecucion = uow.FacturacionRepository.GetFacturacionEjecucion(nuEjecucion);

                form.GetField("nombre").Value = facturacionEjecucion.Nombre;
                form.GetField("fechaHasta").Value = facturacionEjecucion.FechaHasta.ToIsoString();
                form.GetField("fechaProgramacion").Value = facturacionEjecucion.FechaProgramacion.ToIsoString();

                if (facturacionEjecucion.EjecucionPorHora == "S")
                {
                    form.GetField("parcial").Value = "S";
                    context.Parameters.Add(new ComponentParameter()
                    {
                        Id = "showParcial",
                        Value = "true"
                    });

                    form.GetField("horaHasta").Value = DateTimeToHourMinuteString((DateTime)facturacionEjecucion.FechaHasta);
                    form.GetField("horaFechaProgramacion").Value = DateTimeToHourMinuteString((DateTime)facturacionEjecucion.FechaProgramacion);
                }

                if (facturacionEjecucion.FechaDesde != null)
                {
                    form.GetField("ingresarFechaDesde").Value = "true";
                    context.Parameters.Add(new ComponentParameter()
                    {
                        Id = "showFechaDesde",
                        Value = "true"
                    });

                    form.GetField("fechaDesde").Value = facturacionEjecucion.FechaDesde.ToIsoString();

                    if (facturacionEjecucion.EjecucionPorHora == "S")
                        form.GetField("horaDesde").Value = DateTimeToHourMinuteString((DateTime)facturacionEjecucion.FechaDesde);
                }
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                uow.CreateTransactionNumber("FAC001 Panel de facturación");

                if (int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "nuEjecucion")?.Value, out int nuEjecucion))
                {
                    var facturacionEjecucion = uow.FacturacionRepository.GetFacturacionEjecucion(nuEjecucion);
                    ProcesarFacturacionEjecucion(form, facturacionEjecucion);
                    uow.FacturacionRepository.UpdateFacturacionEjecucion(facturacionEjecucion);
                    uow.FacturacionRepository.UpdateFacturacionEjecucionEmpresa(facturacionEjecucion);
                }
                else
                {
                    var facturacionEjecucion = ProcesarFacturacionEjecucion(form);
                    uow.FacturacionRepository.AddFacturacionEjecucion(facturacionEjecucion);
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

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new MantenimientoFacturacionEjecucionValidationModule(), form, context);
        }

        #region Metodos Auxiliares

        public virtual FacturacionEjecucion ProcesarFacturacionEjecucion(Form form, FacturacionEjecucion facturacionEjecucion = null)
        {
            if (facturacionEjecucion == null)
            {
                facturacionEjecucion = new FacturacionEjecucion();
                facturacionEjecucion.FechaIngresado = DateTime.Now;
                facturacionEjecucion.CodigoSituacion = SituacionDb.EJECUCION_EN_PROGRAMACION;
                facturacionEjecucion.CodigoFuncProgramacion = this._identity.UserId;
            }
            else
                facturacionEjecucion.FechaModificacion = DateTime.Now;

            facturacionEjecucion.Nombre = form.GetField("nombre").Value;

            var flParcial = form.GetField("parcial").Value == "true";

            //Fecha Hasta
            if (flParcial)
            {
                DateTime time = DateTime.ParseExact(form.GetField("horaHasta").Value, "HH:mm", CultureInfo.InvariantCulture);
                DateTime fecha = DateTime.Parse(form.GetField("fechaHasta").Value, this._identity.GetFormatProvider());
                facturacionEjecucion.FechaHasta = new DateTime(fecha.Year, fecha.Month, fecha.Day, time.Hour, time.Minute, 0);
            }
            else
            {
                DateTime fecha = DateTime.Parse(form.GetField("fechaHasta").Value, this._identity.GetFormatProvider());
                facturacionEjecucion.FechaHasta = new DateTime(fecha.Year, fecha.Month, fecha.Day, 23, 59, 59);
            }

            //Fecha Desde
            if (form.GetField("ingresarFechaDesde").Value == "true")
            {
                if (flParcial)
                {
                    DateTime time = DateTime.ParseExact(form.GetField("horaDesde").Value, "HH:mm", CultureInfo.InvariantCulture);
                    DateTime fecha = DateTime.Parse(form.GetField("fechaDesde").Value, this._identity.GetFormatProvider());
                    facturacionEjecucion.FechaDesde = new DateTime(fecha.Year, fecha.Month, fecha.Day, time.Hour, time.Minute, 0);
                }
                else
                {
                    DateTime fecha = DateTime.Parse(form.GetField("fechaDesde").Value, this._identity.GetFormatProvider());
                    facturacionEjecucion.FechaDesde = new DateTime(fecha.Year, fecha.Month, fecha.Day, 00, 00, 00);
                }
            }
            else
                facturacionEjecucion.FechaDesde = null;

            //Fecha Programacion
            if (flParcial)
            {
                DateTime time = DateTime.ParseExact(form.GetField("horaFechaProgramacion").Value, "HH:mm", CultureInfo.InvariantCulture);
                DateTime fecha = DateTime.Parse(form.GetField("fechaProgramacion").Value, this._identity.GetFormatProvider());
                facturacionEjecucion.FechaProgramacion = new DateTime(fecha.Year, fecha.Month, fecha.Day, time.Hour, time.Minute, 0);
            }
            else
            {
                DateTime fecha = DateTime.Parse(form.GetField("fechaProgramacion").Value, this._identity.GetFormatProvider());
                facturacionEjecucion.FechaProgramacion = new DateTime(fecha.Year, fecha.Month, fecha.Day, 00, 00, 00);
            }

            facturacionEjecucion.EjecucionPorHora = flParcial ? "S" : "N";

            return facturacionEjecucion;
        }

        public virtual void AnularEjecucion(IUnitOfWork uow, GridButtonActionContext context)
        {
            int nuEjecucion = int.Parse(context.Row.GetCell("NU_EJECUCION").Value);

            FacturacionEjecucion facturacionEjecucion = uow.FacturacionRepository.GetFacturacionEjecucion(nuEjecucion);

            if (facturacionEjecucion.CodigoSituacion != SituacionDb.EJECUCION_EN_PROGRAMACION
                && !uow.FacturacionRepository.AnyResultadoError(nuEjecucion))
                throw new ValidationFailedException("FAC001_Sec0_Error_SituacionInvalida");

            facturacionEjecucion.CodigoSituacion = SituacionDb.EJECUCION_ANULADA;

            uow.FacturacionRepository.UpdateFacturacionEjecucion(facturacionEjecucion);

            uow.SaveChanges();
            context.AddSuccessNotification("FAC001_Sec0_Success_EjecucionAnulada");
        }

        public virtual void HabilitarEjecucion(IUnitOfWork uow, GridButtonActionContext context)
        {
            int nuEjecucion = int.Parse(context.Row.GetCell("NU_EJECUCION").Value);

            FacturacionEjecucion facturacionEjecucion = uow.FacturacionRepository.GetFacturacionEjecucion(nuEjecucion);

            if (facturacionEjecucion.CodigoSituacion != SituacionDb.EJECUCION_EN_PROGRAMACION)
                throw new ValidationFailedException("FAC001_Sec0_Error_SituacionInvalida");

            var ejecucionSolapada = uow.FacturacionRepository.GetAnyFacturacionEjecucionSolapada(nuEjecucion);

            if (ejecucionSolapada != null)
                throw new ValidationFailedException("FAC001_Sec0_Error_EjecucionesSolapadas", new string[] { ejecucionSolapada.NumeroEjecucion.ToString() });

            facturacionEjecucion.CodigoSituacion = SituacionDb.EJECUCION_PENDIENTE;

            uow.FacturacionRepository.UpdateFacturacionEjecucion(facturacionEjecucion);
            uow.SaveChanges();

            context.AddSuccessNotification("FAC001_Sec0_Success_EjecucionHabilitada");
        }

        public virtual void HabilitarFacturacion(IUnitOfWork uow, GridButtonActionContext context)
        {
            var sitHab = new List<short?>() { SituacionDb.CALCULO_EJECUTADO, SituacionDb.CALCULO_ACEPTADO };
            int nuEjecucion = int.Parse(context.Row.GetCell("NU_EJECUCION").Value);

            ValidarFacturacion(uow, sitHab, nuEjecucion);

            FacturacionEjecucion facturacionEjecucion = uow.FacturacionRepository.GetFacturacionEjecucion(nuEjecucion);
            facturacionEjecucion.CodigoSituacion = SituacionDb.EJECUCION_ENVIADA; //305
            facturacionEjecucion.FechaEnviada = DateTime.Now;

            uow.CreateTransactionNumber("HabilitarFacturacion");

            uow.FacturacionRepository.UpdateFacturacionEjecucion(facturacionEjecucion);

            var empresasListasPrecios = uow.FacturacionRepository.GetEmpresaListasPrecios(nuEjecucion, sitHab);
            var listasPrecios = uow.FacturacionRepository.GetListasDePrecios(nuEjecucion, sitHab);
            var listasCotizacion = uow.FacturacionRepository.GetListasDeCotizacion(nuEjecucion, sitHab);

            var resultados = uow.FacturacionRepository.GetResultadosEjecucionSituacion(nuEjecucion, sitHab);
            foreach (var res in resultados)
            {
                var cdlistaPrecio = empresasListasPrecios.GetValueOrDefault(res.CodigoEmpresa, null);
                var listaPrecio = listasPrecios.GetValueOrDefault(cdlistaPrecio ?? -1, null);
                var cotizacion = listasCotizacion.GetValueOrDefault($"{listaPrecio?.Id}.{res.CodigoFacturacion}.{res.NumeroComponente}", null);

                res.PrecioUnitario = cotizacion?.CantidadImporte;
                res.PrecioMinimo = cotizacion?.CantidadImporteMinimo;
                res.Moneda = listaPrecio?.IdMoneda;
                res.CodigoSituacion = SituacionDb.CALCULO_ENVIADO;
                res.NumeroTransaccion = uow.GetTransactionNumber();

                uow.FacturacionRepository.UpdateFacturacionResultado(res);
            }

            var detallesPallets = uow.FacturacionRepository.GetDetallesPalletsHabilitados(nuEjecucion);
            foreach (var detPallet in detallesPallets)
            {
                detPallet.Estado = FacturacionDb.ESTADO_FAC;
                uow.FacturacionRepository.UpdateFacturacionPalletDet(detPallet);
            }

            uow.SaveChanges();
            context.AddSuccessNotification("FAC001_Sec0_Success_EjecucionHabilitadaFacturar");
        }

        public virtual void ValidarFacturacion(IUnitOfWork uow, List<short?> sitHab, int nuEjecucion)
        {
            if (uow.FacturacionRepository.AnyResultadoError(nuEjecucion))
                throw new ValidationFailedException("FAC001_Sec0_Error_CalculosPendientes");
            else if (!uow.FacturacionRepository.AnyFacturacionHabilitadaCalculo(nuEjecucion))
                throw new ValidationFailedException("FAC001_Sec0_Error_CalculosPendientes");
            else if (uow.EmpresaRepository.AnyEmpresaSinListaPrecio(nuEjecucion))
                throw new ValidationFailedException("FAC001_Sec0_Error_EmpresaSinListaDePrecios");
            else if (uow.FacturacionRepository.AnyResultadoSinPrecioUnitario(nuEjecucion, sitHab))
                throw new ValidationFailedException("FAC001_Sec0_Error_ResultadosSinPrecioUnitario");
        }

        public virtual bool PuedeFacturar(IUnitOfWork uow, GridRow row, bool ControlAvanzado = false)
        {
            int nuEjecucion = int.Parse(row.GetCell("NU_EJECUCION").Value);
            short cdSituacion = short.Parse(row.GetCell("CD_SITUACAO").Value);
            var sitHab = new List<short?>() { SituacionDb.CALCULO_EJECUTADO, SituacionDb.CALCULO_ACEPTADO };

            if (cdSituacion != SituacionDb.EJECUCION_REALIZADA)
                return false;

            if (ControlAvanzado)
            {
                if (uow.FacturacionRepository.AnyResultadoError(nuEjecucion))
                    return false;
                else if (uow.FacturacionRepository.AnyResultadoSituacion(nuEjecucion, SituacionDb.CALCULO_CON_ERRORES))
                    return false;
                else if (uow.EmpresaRepository.AnyEmpresaSinListaPrecio(nuEjecucion))
                    return false;
                else if (uow.FacturacionRepository.AnyResultadoSinPrecioUnitario(nuEjecucion, sitHab))
                    return false;
            }
            return true;
        }

        public virtual void DisableButtons(List<GridRow> rows, IUnitOfWork uow)
        {
            Dictionary<string, bool> result = this._security.CheckPermissions(new List<string>
            {
                "WFAC001_grid1_btn_AsignarProcesos",
                "WFAC001_grid1_btn_DetallesEjecucion",
                "WFAC001_grid1_btn_AnularEjecucion",
                "WFAC001_grid1_btn_HabilitarEjecucion",
                "WFAC001_grid1_btn_IngresoResultadoManual",
                "WFAC001_grid1_btn_EdicionResultado",
                "WFAC001_grid1_btn_ResultadosDeEjecucion",
                "WFAC001_grid1_btn_CalculosNoRealizadosPorError",
                "WFAC001_grid1_btn_AceptarRechazarCalculos",
                "WFAC001_grid1_btn_PreviewPrecios",
                "WFAC001_grid1_btn_HabilitarFacturacion",
                "WFAC001_grid1_btn_DescargarRegistro"
            });

            foreach (var row in rows)
            {
                int nuEjecucion = int.Parse(row.GetCell("NU_EJECUCION").Value);
                short cdSituacion = short.Parse(row.GetCell("CD_SITUACAO").Value);

                if (!cdSituacion.Equals(SituacionDb.EJECUCION_EN_PROGRAMACION))
                    row.DisabledButtons.Add("btnEditar");

                if (!result["WFAC001_grid1_btn_AsignarProcesos"] || !cdSituacion.Equals(SituacionDb.EJECUCION_EN_PROGRAMACION))
                    row.DisabledButtons.Add("btnAsignarProcesos");

                if (!result["WFAC001_grid1_btn_DetallesEjecucion"])
                    row.DisabledButtons.Add("btnDetalles");

                if (!result["WFAC001_grid1_btn_AnularEjecucion"] || (!cdSituacion.Equals(SituacionDb.EJECUCION_EN_PROGRAMACION) && !uow.FacturacionRepository.AnyResultadoError(nuEjecucion)) || uow.FacturacionRepository.AnyFacturacionAnulada(nuEjecucion))
                    row.DisabledButtons.Add("btnAnularEjecucion");

                if (!result["WFAC001_grid1_btn_HabilitarEjecucion"] || !cdSituacion.Equals(SituacionDb.EJECUCION_EN_PROGRAMACION) || !uow.FacturacionRepository.AnyFacturacionEjecucion(nuEjecucion))
                    row.DisabledButtons.Add("btnHabilitarEjecucion");

                if (!result["WFAC001_grid1_btn_IngresoResultadoManual"] || !(cdSituacion.Equals(SituacionDb.EJECUCION_PENDIENTE) || cdSituacion.Equals(SituacionDb.EJECUCION_REALIZADA)))
                    row.DisabledButtons.Add("btnIngresoResultadoManual");

                if (!result["WFAC001_grid1_btn_EdicionResultado"] || !cdSituacion.Equals(SituacionDb.EJECUCION_REALIZADA))
                    row.DisabledButtons.Add("btnEdicionDeResultados");

                var estadosConsultaResultados = new List<short>
                {
                    SituacionDb.EJECUCION_REALIZADA,
                    SituacionDb.EJECUCION_ENVIADA,
                    SituacionDb.CALCULO_FACTURADO
                };

                if (!result["WFAC001_grid1_btn_ResultadosDeEjecucion"] || !estadosConsultaResultados.Contains(cdSituacion))
                    row.DisabledButtons.Add("btnResultadosDeEjecucion");

                if (!result["WFAC001_grid1_btn_DescargarRegistro"] || !estadosConsultaResultados.Contains(cdSituacion))
                    row.DisabledButtons.Add("btnDescargarRegistro");

                if (!result["WFAC001_grid1_btn_CalculosNoRealizadosPorError"] || !cdSituacion.Equals(SituacionDb.EJECUCION_REALIZADA))
                    row.DisabledButtons.Add("btnCalculosNoRealizadosPorError");

                if (!result["WFAC001_grid1_btn_AceptarRechazarCalculos"] || !cdSituacion.Equals(SituacionDb.EJECUCION_REALIZADA))
                    row.DisabledButtons.Add("btnAceptarRechazarCalculos");

                if (!result["WFAC001_grid1_btn_PreviewPrecios"] || !cdSituacion.Equals(SituacionDb.EJECUCION_REALIZADA))
                    row.DisabledButtons.Add("btnPreviewPrecios");

                if (!result["WFAC001_grid1_btn_HabilitarFacturacion"] || !PuedeFacturar(uow, row))
                    row.DisabledButtons.Add("btnHabilitarFacturacion");
            }
        }

        public virtual string DateTimeToHourMinuteString(DateTime fecha)
        {
            string hora = fecha.Hour.ToString();
            if (fecha.Hour < 10)
                hora = "0" + hora;

            string minutos = fecha.Minute.ToString();
            if (fecha.Minute < 10)
                minutos = "0" + minutos;

            string resultado = hora + ":" + minutos;

            return resultado;
        }

        public virtual void ClearForm(Form form)
        {
            form.GetField("nombre").Value = string.Empty;
            form.GetField("ingresarFechaDesde").Value = "N";
            form.GetField("parcial").Value = "N";
            form.GetField("fechaDesde").Value = string.Empty;
            form.GetField("fechaHasta").Value = string.Empty;
            form.GetField("fechaProgramacion").Value = string.Empty;
            form.GetField("horaDesde").Value = string.Empty;
            form.GetField("horaHasta").Value = string.Empty;
            form.GetField("horaFechaProgramacion").Value = string.Empty;
        }

        #endregion
    }
}
