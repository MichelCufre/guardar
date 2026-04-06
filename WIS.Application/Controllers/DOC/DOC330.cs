using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Validation;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Documento;
using WIS.Domain.Documento;
using WIS.Domain.Documento.Constants;
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
using WIS.GridComponent.Items;
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.DOC
{
    public class DOC330 : AppController
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

        public DOC330(ISessionAccessor session,
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
                "NU_DOCUMENTO", "TP_DOCUMENTO"
            };
        }

        #region PAGE
        public override PageContext PageLoad(PageContext context)
        {
            string nroAgrupador = context.GetParameter("nuAgrupador");
            string tipoAgrupador = context.GetParameter("tpAgrupador");

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var agrupador = uow.DocumentoRepository.GetAgrupador(nroAgrupador, tipoAgrupador);

                if (agrupador != null)
                {
                    this._session.SetValue("DOC330_CD_EMPRESA", agrupador.Empresa != null ? agrupador.Empresa.ToString() : "");
                }
            }

            return context;
        }
        #endregion

        #region FORM

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            string nroAgrupador = context.GetParameter("nuAgrupador");
            string tipoAgrupador = context.GetParameter("tpAgrupador");

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var agrupador = uow.DocumentoRepository.GetAgrupador(nroAgrupador, tipoAgrupador);

                if (agrupador != null)
                {
                    form.GetField("NU_AGRUPADOR").Value = agrupador.Numero;
                    form.GetField("DS_AGRUPADOR").Value = agrupador.Tipo.Descripcion;
                    form.GetField("EMPRESA").Value = agrupador.Empresa != null ? uow.EmpresaRepository.GetEmpresa((int)agrupador.Empresa).Nombre : "-";
                    form.GetField("transportadora").Value = agrupador.Transportadora != null ? agrupador.Transportadora.Descripcion : "-";
                    form.GetField("placa").Value = agrupador.Placa;
                    form.GetField("ID_ESTADO").Value = agrupador.Estado;
                    form.GetField("DT_SALIDA").Value = agrupador.FechaSalida == null ? "-" : agrupador.FechaSalida.ToString();
                    form.GetField("NU_LACRE").Value = agrupador.NumeroLacre;

                    IConfirmMessage confirmarAgrupacion = new ConfirmMessage()
                    {
                        AcceptLabel = "DOC330_Sec0_btn_ConfirmarOk",
                        CancelLabel = "DOC330_Sec0_btn_ConfirmarCancel",
                        Message = "DOC330_Sec0_btn_MsgConfirmar"
                    };

                    form.GetButton("btnSubmit").ConfirmMessage = confirmarAgrupacion;

                    if (agrupador.Estado == EstadoDocumentoAgrupador.CONFIRMADO)
                    {
                        form.GetButton("btnSubmit").Disabled = true;
                    }
                }
            }

            return form;
        }
        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            try
            {
                this.ConfirmarAgrupamiento(form, context);
            }
            catch (Exception ex)
            {
                context.AddErrorNotification(ex.Message);
            }

            return form;
        }

        #endregion FORM

        #region GRID
        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems = (new List<IGridItem> {
                new GridItemHeader("DOC080_Sec0_lbl_Acciones"),
                new GridButton("btnSeleccionar", "DOC330_grid1_btn_ConfirmarSeleccion", "fas fa-plus"),
                new GridButton("btnDeseleccionar", "DOC330_grid1_btn_CancelarSeleccion", "fas fa-minus")
            });

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            string nroAgrupador = context.GetParameter("nuAgrupador");
            string tipoAgrupador = context.GetParameter("tpAgrupador");

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                //Obtener agrupador
                var tipo = uow.DocumentoRepository.GetDocumentoAgrupadorTipo(tipoAgrupador);

                var dbQuery = new DocumentoAgrupableDOC330Query(nroAgrupador, tipoAgrupador, tipo.Grupos.Select(a => a.TipoDocumento).ToList(), this.ObtenerEmpresaAgrupador());

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);
            }

            this.FormatGrid(grid, nroAgrupador, tipoAgrupador);

            return grid;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            switch (context.ButtonId)
            {
                case "btnSeleccionar":
                    this.ProcesarSeleccion(context);
                    break;

                case "btnDeseleccionar":
                    this.ProcesarDeseleccion(context);
                    break;
            }

            return context;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            string nroAgrupador = context.GetParameter("nuAgrupador");
            string tipoAgrupador = context.GetParameter("tpAgrupador");

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                //Obtener agrupador
                var tipo = uow.DocumentoRepository.GetDocumentoAgrupadorTipo(tipoAgrupador);

                var dbQuery = new DocumentoAgrupableDOC330Query(nroAgrupador, tipoAgrupador, tipo.Grupos.Select(a => a.TipoDocumento).ToList(), this.ObtenerEmpresaAgrupador());

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string nroAgrupador = context.GetParameter("nuAgrupador");
            string tipoAgrupador = context.GetParameter("tpAgrupador");

            //Obtener agrupador
            var tipo = uow.DocumentoRepository.GetDocumentoAgrupadorTipo(tipoAgrupador);

            var dbQuery = new DocumentoAgrupableDOC330Query(nroAgrupador, tipoAgrupador, tipo.Grupos.Select(a => a.TipoDocumento).ToList(), this.ObtenerEmpresaAgrupador());

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        #endregion GRID

        #region METODOS

        public virtual void FormatGrid(Grid grid, string agrupador, string tipoAgrupador)
        {
            grid.Rows.ForEach(row =>
            {
                var nuAgr = row.GetCell("NU_AGRUPADOR").Value;
                var tpAgr = row.GetCell("TP_AGRUPADOR").Value;

                if (nuAgr == agrupador && tpAgr == tipoAgrupador)
                {
                    row.Cells.ForEach(cell => { cell.CssClass = "asigned"; });
                }
                else if (!string.IsNullOrEmpty(nuAgr) && !string.IsNullOrEmpty(tpAgr))
                {
                    row.Cells.ForEach(cell => { cell.CssClass = "used"; });
                }
            });
        }

        public virtual GridMenuItemActionContext ProcesarSeleccion(GridMenuItemActionContext context)
        {
            string nuAgrupador = context.GetParameter("nuAgrupador");
            string tpAgrupador = context.GetParameter("tpAgrupador");

            try
            {
                //Obtener agrupador
                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    uow.CreateTransactionNumber("DOC330 ProcesarSeleccion");

                    var agrupador = uow.DocumentoRepository.GetAgrupador(nuAgrupador, tpAgrupador);

                    switch (agrupador.Tipo.TipoOperacion)
                    {
                        case TipoOperacionAgrupador.INGRESO:
                            this.ProcesarSeleccionIngresos(context, uow, nuAgrupador, tpAgrupador);
                            break;
                        case TipoOperacionAgrupador.EGRESO:
                            this.ProcesarSeleccionEgresos(context, uow, nuAgrupador, tpAgrupador);
                            break;
                    }

                }

            }
            catch (Exception ex)
            {
                context.AddErrorNotification("DOC330_frm1_not_ErrorEX", new List<string>() { ex.Message });
            }

            return context;
        }

        public virtual void ProcesarSeleccionIngresos(GridMenuItemActionContext context, IUnitOfWork uow, string nroAgrupador, string tipoAgrupador)
        {
            bool error;

            var nuTransaccion = uow.GetTransactionNumber();
            var documentosSeleccionados = this.ObtenerDocumentosIngresoSeleccionados(context, nroAgrupador, tipoAgrupador, out error);

            if (!error)
            {
                if (documentosSeleccionados.Count() > 0)
                {
                    if (!documentosSeleccionados.Any(d => !d.Validado))
                    {
                        //Obtener tipo de agrupador y documentos agrupados para controlar limite de documentos agrupables
                        var tipo = uow.DocumentoRepository.GetDocumentoAgrupadorTipo(tipoAgrupador);
                        var agrupador = uow.DocumentoRepository.GetAgrupadorWithDetail(nroAgrupador, tipoAgrupador);

                        if ((tipo.CantidadMaximaDocumentos == null) || ((documentosSeleccionados.Count + agrupador.LineasIngresoAgrupadas.Count) <= tipo.CantidadMaximaDocumentos))
                        {
                            var acciones = uow.DocumentoRepository.GetEstadosAgrupacion(AccionDocumento.Agrupar);

                            //Agrupar documentos seleccionados
                            foreach (IDocumentoIngreso ingreso in documentosSeleccionados)
                            {
                                if (ingreso.NumeroAgrupador != nroAgrupador && ingreso.TipoAgrupador != tipoAgrupador)
                                {
                                    var estado = acciones.First(a => a.Origen.Id == ingreso.Estado && a.TipoDocumento == ingreso.Tipo)?.Destino?.Id;

                                    if (string.IsNullOrEmpty(estado))
                                        throw new ValidationFailedException("DOC330_Sec0_Not_AgrupacionNoDefinida", new string[] { ingreso.Tipo, ingreso.Estado });

                                    ingreso.Estado = estado;
                                    ingreso.Agrupar(nroAgrupador, tipoAgrupador);
                                    uow.DocumentoRepository.UpdateIngreso(ingreso, nuTransaccion);
                                }
                            }

                            uow.SaveChanges();
                            context.AddSuccessNotification("DOC330_frm1_not_SuccesAgrupacion", new List<string>() { tipoAgrupador, nroAgrupador });
                        }
                        else
                        {
                            context.AddErrorNotification("DOC330_frm1_not_ErrorLimiteDocumentos", new List<string>() { tipo.CantidadMaximaDocumentos.ToString(), tipoAgrupador });
                        }
                    }
                    else
                    {
                        context.AddErrorNotification("DOC330_frm1_not_ErrorDocNoValidado");
                    }
                }
                else
                {
                    context.AddErrorNotification("DOC330_frm1_not_ErrorAgrupadorSinDocumentos");
                }
            }
        }

        public virtual void ProcesarSeleccionEgresos(GridMenuItemActionContext context, IUnitOfWork uow, string nroAgrupador, string tipoAgrupador)
        {
            bool error;

            var nuTransaccion = uow.GetTransactionNumber();
            var documentosSeleccionados = this.ObtenerDocumentosEgresoSeleccionados(context, nroAgrupador, tipoAgrupador, out error);

            if (!error)
            {
                if (documentosSeleccionados.Count() > 0)
                {
                    //Obtener tipo de agrupador y documentos agrupados para controlar limite de documentos agrupables
                    var tipo = uow.DocumentoRepository.GetDocumentoAgrupadorTipo(tipoAgrupador);
                    var agrupador = uow.DocumentoRepository.GetAgrupadorWithDetail(nroAgrupador, tipoAgrupador);

                    if ((tipo.CantidadMaximaDocumentos == null) || ((documentosSeleccionados.Count + agrupador.LineasEgresoAgrupadas.Count) <= tipo.CantidadMaximaDocumentos))
                    {
                        var acciones = uow.DocumentoRepository.GetEstadosAgrupacion(AccionDocumento.Agrupar);

                        //Agrupar documentos seleccionados
                        foreach (IDocumentoEgreso egreso in documentosSeleccionados)
                        {
                            if (egreso.NumeroAgrupador != nroAgrupador && egreso.TipoAgrupador != tipoAgrupador)
                            {
                                var estado = acciones.First(a => a.Origen.Id == egreso.Estado && a.TipoDocumento == egreso.Tipo)?.Destino?.Id;

                                if (string.IsNullOrEmpty(estado))
                                    throw new ValidationFailedException("DOC330_Sec0_Not_AgrupacionNoDefinida", new string[] { egreso.Tipo, egreso.Estado });

                                egreso.Estado = estado;
                                egreso.Agrupar(nroAgrupador, tipoAgrupador);

                                uow.DocumentoRepository.UpdateEgreso(egreso, nuTransaccion);
                            }
                        }
                        uow.SaveChanges();
                        context.AddSuccessNotification("DOC330_frm1_not_SuccesAgrupacion", new List<string>() { tipoAgrupador, nroAgrupador });
                    }
                    else
                    {
                        context.AddErrorNotification("DOC330_frm1_not_ErrorLimiteDocumentos", new List<string>() { tipo.CantidadMaximaDocumentos.ToString(), tipoAgrupador });
                    }
                }
                else
                {
                    context.AddErrorNotification("DOC330_frm1_not_ErrorAgrupadorSinDocumentos");
                }

            }
        }

        public virtual GridMenuItemActionContext ProcesarDeseleccion(GridMenuItemActionContext context)
        {
            string nuAgrupador = context.GetParameter("nuAgrupador");
            string tpAgrupador = context.GetParameter("tpAgrupador");

            try
            {
                //Obtener agrupador
                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    uow.CreateTransactionNumber("DOC330 ProcesarDeseleccion");

                    var agrupador = uow.DocumentoRepository.GetAgrupador(nuAgrupador, tpAgrupador);

                    switch (agrupador.Tipo.TipoOperacion)
                    {
                        case TipoOperacionAgrupador.INGRESO:
                            this.ProcesarDeseleccionIngreso(context, uow, nuAgrupador, tpAgrupador);
                            break;
                        case TipoOperacionAgrupador.EGRESO:
                            this.ProcesarDeseleccionEgreso(context, uow, nuAgrupador, tpAgrupador);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                context.AddErrorNotification("DOC330_frm1_not_ErrorEX", new List<string>() { ex.Message });
            }

            return context;
        }

        public virtual void ProcesarDeseleccionIngreso(GridMenuItemActionContext context, IUnitOfWork uow, string nroAgrupador, string tipoAgrupador)
        {
            bool error;
            var nuTransaccion = uow.GetTransactionNumber();
            var documentosSeleccionados = this.ObtenerDocumentosIngresoSeleccionados(context, nroAgrupador, tipoAgrupador, out error);

            if (!error)
            {
                var acciones = uow.DocumentoRepository.GetEstadosAgrupacion(AccionDocumento.Desagrupar);

                //Agrupar documentos seleccionados
                foreach (IDocumentoIngreso ingreso in documentosSeleccionados)
                {
                    if (ingreso.NumeroAgrupador == nroAgrupador && ingreso.TipoAgrupador == tipoAgrupador)
                    {
                        var estado = acciones.First(a => a.Origen.Id == ingreso.Estado && a.TipoDocumento == ingreso.Tipo)?.Destino?.Id;

                        if (string.IsNullOrEmpty(estado))
                            throw new ValidationFailedException("DOC330_Sec0_Not_DesagrupacionNoDefinida", new string[] { ingreso.Tipo, ingreso.Estado });

                        ingreso.Estado = estado;
                        ingreso.Desagrupar();
                        uow.DocumentoRepository.UpdateIngreso(ingreso, nuTransaccion);
                    }
                }

                uow.SaveChanges();
                context.AddSuccessNotification("DOC330_frm1_not_SuccesDesAgrupar", new List<string>() { tipoAgrupador, nroAgrupador });
            }
        }

        public virtual void ProcesarDeseleccionEgreso(GridMenuItemActionContext context, IUnitOfWork uow, string nroAgrupador, string tipoAgrupador)
        {
            bool error;
            var nuTransaccion = uow.GetTransactionNumber();
            var documentosSeleccionados = this.ObtenerDocumentosEgresoSeleccionados(context, nroAgrupador, tipoAgrupador, out error);

            if (!error)
            {
                var acciones = uow.DocumentoRepository.GetEstadosAgrupacion(AccionDocumento.Desagrupar);

                //Agrupar documentos seleccionados
                foreach (IDocumentoEgreso egreso in documentosSeleccionados)
                {
                    if (egreso.NumeroAgrupador == nroAgrupador && egreso.TipoAgrupador == tipoAgrupador)
                    {
                        var estado = acciones.First(a => a.Origen.Id == egreso.Estado && a.TipoDocumento == egreso.Tipo)?.Destino?.Id;

                        if (string.IsNullOrEmpty(estado))
                            throw new ValidationFailedException("DOC330_Sec0_Not_DesagrupacionNoDefinida", new string[] { egreso.Tipo, egreso.Estado });

                        egreso.Estado = estado;
                        egreso.Desagrupar();
                        uow.DocumentoRepository.UpdateEgreso(egreso, nuTransaccion);
                    }
                }

                uow.SaveChanges();
                context.AddSuccessNotification("DOC330_frm1_not_SuccesDesAgrupar", new List<string>() { tipoAgrupador, nroAgrupador });

            }
        }

        public virtual List<IDocumentoIngreso> ObtenerDocumentosIngresoSeleccionados(GridMenuItemActionContext context, string nroAgrupador, string tipoAgrupador, out bool error)
        {
            error = false;
            List<IDocumentoIngreso> documentoSeleccionados = new List<IDocumentoIngreso>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (context.Selection.AllSelected) //Select all
                {
                    this.HandleSelectAllIngresos(context, nroAgrupador, tipoAgrupador, documentoSeleccionados, uow, out error);
                }
                else //Select individual
                {
                    this.HandleSelectIngresos(context, nroAgrupador, tipoAgrupador, documentoSeleccionados, uow, out error);
                }
            }

            return documentoSeleccionados;
        }
        public virtual List<IDocumentoEgreso> ObtenerDocumentosEgresoSeleccionados(GridMenuItemActionContext context, string nroAgrupador, string tipoAgrupador, out bool error)
        {
            error = false;
            List<IDocumentoEgreso> documentoSeleccionados = new List<IDocumentoEgreso>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (context.Selection.AllSelected) //Select all
                {
                    this.HandleSelectAllEgresos(context, nroAgrupador, tipoAgrupador, documentoSeleccionados, uow, out error);
                }
                else //Select individual
                {
                    this.HandleSelectEgresos(context, nroAgrupador, tipoAgrupador, documentoSeleccionados, uow, out error);
                }
            }

            return documentoSeleccionados;
        }

        public virtual List<IDocumentoIngreso> HandleSelectIngresos(GridMenuItemActionContext context, string nroAgrupador, string tipoAgrupador, List<IDocumentoIngreso> documentoSeleccionados, IUnitOfWork uow, out bool error)
        {
            error = false;

            //Obtener las keys de los documentos
            List<KeyValuePair<string, string>> keysDocumentos = new List<KeyValuePair<string, string>>();
            foreach (string keys in context.Selection.Keys)
            {
                keysDocumentos.Add(new KeyValuePair<string, string>(keys.Split('$')[0], keys.Split('$')[1]));
            }

            if (keysDocumentos.Count > 0)
            {
                //Obtener documentos de ingreso seleccionados
                documentoSeleccionados = uow.DocumentoRepository.GetDocumentosIngresoAgrupablesByKeys(keysDocumentos);
            }

            //Controlar que los documentos no esten bajo otro agrupador
            if (documentoSeleccionados.Any(d => d.NumeroAgrupador != nroAgrupador && !string.IsNullOrEmpty(d.NumeroAgrupador) && !string.IsNullOrEmpty(d.TipoAgrupador)))
            {
                context.AddErrorNotification("DOC330_frm1_not_ErrorAgrupadoBajoOtroAgrupador");

                error = true;
            }

            return documentoSeleccionados;
        }

        public virtual List<IDocumentoIngreso> HandleSelectAllIngresos(GridMenuItemActionContext context, string nroAgrupador, string tipoAgrupador, List<IDocumentoIngreso> documentoSeleccionados, IUnitOfWork uow, out bool error)
        {
            error = false;

            //Obtener agrupador
            var tipo = uow.DocumentoRepository.GetDocumentoAgrupadorTipo(tipoAgrupador);

            var dbQuery = new DocumentoAgrupableDOC330Query(nroAgrupador, tipoAgrupador, tipo.Grupos.Select(a => a.TipoDocumento).ToList(), this.ObtenerEmpresaAgrupador());

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            var keysSelectAll = dbQuery.GetResult().Select(r => new KeyValuePair<string, string>(r.NU_DOCUMENTO, r.TP_DOCUMENTO)).ToList();

            List<KeyValuePair<string, string>> keysDocumentosDepurados = new List<KeyValuePair<string, string>>();

            foreach (string keys in context.Selection.Keys)
            {
                var documentoDesmarcado = keysSelectAll.FirstOrDefault(k => k.Key == keys.Split('$')[0] && k.Value == keys.Split('$')[1]);

                if (!string.IsNullOrEmpty(documentoDesmarcado.Key) && !string.IsNullOrEmpty(documentoDesmarcado.Value))
                    keysSelectAll.Remove(documentoDesmarcado);
            }

            if (keysSelectAll.Count > 0)
            {
                //Obtener documentos de ingreso seleccionados
                documentoSeleccionados = uow.DocumentoRepository.GetDocumentosIngresoAgrupablesByKeys(keysSelectAll);
            }

            //Controlar que los documentos no esten bajo otro agrupador
            if (documentoSeleccionados.Any(d => d.NumeroAgrupador != nroAgrupador && !string.IsNullOrEmpty(d.NumeroAgrupador) && !string.IsNullOrEmpty(d.TipoAgrupador)))
            {
                context.AddErrorNotification("DOC330_frm1_not_ErrorAgrupadoBajoOtroAgrupador");

                error = true;
            }

            return documentoSeleccionados;
        }

        public virtual List<IDocumentoEgreso> HandleSelectEgresos(GridMenuItemActionContext context, string nroAgrupador, string tipoAgrupador, List<IDocumentoEgreso> documentoSeleccionados, IUnitOfWork uow, out bool error)
        {
            error = false;

            //Obtener las keys de los documentos
            List<KeyValuePair<string, string>> keysDocumentos = new List<KeyValuePair<string, string>>();
            foreach (string keys in context.Selection.Keys)
            {
                keysDocumentos.Add(new KeyValuePair<string, string>(keys.Split('$')[0], keys.Split('$')[1]));
            }

            if (keysDocumentos.Count > 0)
            {
                //Obtener documentos de ingreso seleccionados
                documentoSeleccionados = uow.DocumentoRepository.GetDocumentosEgresoAgrupablesByKeys(keysDocumentos);
            }

            //Controlar que los documentos no esten bajo otro agrupador
            if (documentoSeleccionados.Any(d => d.NumeroAgrupador != nroAgrupador && !string.IsNullOrEmpty(d.NumeroAgrupador) && !string.IsNullOrEmpty(d.TipoAgrupador)))
            {
                context.AddErrorNotification("DOC330_frm1_not_ErrorAgrupadoBajoOtroAgrupador");

                error = true;
            }

            return documentoSeleccionados;
        }

        public virtual List<IDocumentoEgreso> HandleSelectAllEgresos(GridMenuItemActionContext context, string nroAgrupador, string tipoAgrupador, List<IDocumentoEgreso> documentoSeleccionados, IUnitOfWork uow, out bool error)
        {
            error = false;

            //Obtener agrupador
            var tipo = uow.DocumentoRepository.GetDocumentoAgrupadorTipo(tipoAgrupador);

            var dbQuery = new DocumentoAgrupableDOC330Query(nroAgrupador, tipoAgrupador, tipo.Grupos.Select(a => a.TipoDocumento).ToList(), this.ObtenerEmpresaAgrupador());

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            var keysSelectAll = dbQuery.GetResult().Select(r => new KeyValuePair<string, string>(r.NU_DOCUMENTO, r.TP_DOCUMENTO)).ToList();

            List<KeyValuePair<string, string>> keysDocumentosDepurados = new List<KeyValuePair<string, string>>();

            foreach (string keys in context.Selection.Keys)
            {
                var documentoDesmarcado = keysSelectAll.FirstOrDefault(k => k.Key == keys.Split('$')[0] && k.Value == keys.Split('$')[1]);

                if (!string.IsNullOrEmpty(documentoDesmarcado.Key) && !string.IsNullOrEmpty(documentoDesmarcado.Value))
                    keysSelectAll.Remove(documentoDesmarcado);
            }

            if (keysSelectAll.Count > 0)
            {
                //Obtener documentos de ingreso seleccionados
                documentoSeleccionados = uow.DocumentoRepository.GetDocumentosEgresoAgrupablesByKeys(keysSelectAll);
            }

            //Controlar que los documentos no esten bajo otro agrupador
            if (documentoSeleccionados.Any(d => d.NumeroAgrupador != nroAgrupador && !string.IsNullOrEmpty(d.NumeroAgrupador) && !string.IsNullOrEmpty(d.TipoAgrupador)))
            {
                context.AddErrorNotification("DOC330_frm1_not_ErrorAgrupadoBajoOtroAgrupador");

                error = true;
            }

            return documentoSeleccionados;
        }

        public virtual Form ConfirmarAgrupamiento(Form form, FormSubmitContext context)
        {
            var nuAgrupador = context.GetParameter("nuAgrupador");
            var tpAgrupador = context.GetParameter("tpAgrupador");

            if (string.IsNullOrEmpty(nuAgrupador) || string.IsNullOrEmpty(tpAgrupador))
                throw new Exception("DOC330_frm1_not_ErrorDatoAgrupador");

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                uow.CreateTransactionNumber("DOC330 ConfirmarAgrupamiento");

                var nuTransaccion = uow.GetTransactionNumber();
                var agrupador = uow.DocumentoRepository.GetAgrupadorWithDetail(nuAgrupador, tpAgrupador);

                if (agrupador == null)
                    throw new Exception("DOC330_frm1_not_ErrorNoEncontrado");

                if ((agrupador.Tipo.TipoOperacion == TipoOperacionAgrupador.INGRESO && agrupador.LineasIngresoAgrupadas.Count == 0) || (agrupador.Tipo.TipoOperacion == TipoOperacionAgrupador.EGRESO && agrupador.LineasEgresoAgrupadas.Count == 0))
                    throw new Exception("DOC330_frm1_not_ErrorAgrupadorSinLineas");

                //Confirmar agrupacion
                agrupador.ConfirmarAgrupador();

                uow.DocumentoRepository.UpdateAgrupador(agrupador, nuTransaccion);

                uow.SaveChanges();
            }

            context.Redirect("/documento/DOC320", new List<ComponentParameter>() { });

            return form;
        }

        public virtual int? ObtenerEmpresaAgrupador()
        {
            string empresaParam = this._session.GetValue<string>("DOC330_CD_EMPRESA");

            int empresaAuxiliar;
            int? empresa = null;
            if (int.TryParse(empresaParam, out empresaAuxiliar))
                empresa = empresaAuxiliar;

            return empresa;
        }

        #endregion METODOS
    }
}