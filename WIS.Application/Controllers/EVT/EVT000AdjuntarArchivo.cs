using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Eventos;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Evento;
using WIS.Domain.Eventos;
using WIS.Domain.Eventos.Enums;
using WIS.Domain.Services.Interfaces;
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
using WIS.Http;
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.EVT
{
    public class EVT000AdjuntarArchivo : AppController
    {
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IWebApiClient _apiClient;
        protected readonly IParameterService _parameterService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }

        public EVT000AdjuntarArchivo(
            ISessionAccessor session,
            IIdentityService identity,
            IFormValidationService formValidationService,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IWebApiClient apiClient,
            IParameterService parameterService,
            IFilterInterpreter filterInterpreter,
            ISecurityService security)
        {
            this.GridKeys = new List<string>
            {
                "NU_ARCHIVO_ADJUNTO","CD_EMPRESA"//,"CD_MANEJO","DS_REFERENCIA"
            };

            this._session = session;
            this._identity = identity;
            this._formValidationService = formValidationService;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._apiClient = apiClient;
            this._parameterService = parameterService;
            this._filterInterpreter = filterInterpreter;
            this._security = security;
        }

        public override PageContext PageLoad(PageContext context)
        {
            List<AuxCell20> infoCells = null;

            string flAux = "F";
            string manejo = "PRO";
            string json = _session.GetValue<string>("EVT000_JSON");
            _session.SetValue("EVT000_JSON", null);

            if (!string.IsNullOrEmpty(json))
            {
                JsonArchivoAdjunto aux = JsonConvert.DeserializeObject<JsonArchivoAdjunto>(json);

                _session.SetValue("EVT000_CD_EMPRESA", aux.CD_EMPRESA);
                _session.SetValue("EVT000_CD_MANEJO", aux.CD_MANEJO);
                _session.SetValue("EVT000_DS_REFERENCIA", aux.DS_REFERENCIA);

                infoCells = JsonConvert.DeserializeObject<List<AuxCell20>>(aux.DATA_INFO);
                flAux = "I";
                manejo = aux.CD_MANEJO;
            }
            else
            {
                string jsonAverias = _session.GetValue<string>("EVT000_AVERIAS");

                _session.SetValue("EVT000_AVERIAS", null);

                if (string.IsNullOrEmpty(jsonAverias))
                {
                    _session.SetValue("EVT000_CD_EMPRESA", null);
                    _session.SetValue("EVT000_CD_MANEJO", null);
                    _session.SetValue("EVT000_DS_REFERENCIA", null);

                    flAux = "F";
                }
                else
                {
                    _session.SetValue("EVT000_AVERIAS_INT", jsonAverias);
                    flAux = "A";
                    manejo = "AVE";
                }
            }

            using var uow = this._uowFactory.GetUnitOfWork();

            context.AddParameter("IP_COMPARTIDA", $"{this._parameterService.GetValue("IP_ARCHIVOS_DIGITALES")}/ARCHIVOS_DEGITALES/");

            ArchivoManejo archivoManejo = uow.ArchivoRepository.GetManejo(manejo);

            _session.SetValue("EVT000_DS_ANEXOS", JsonConvert.SerializeObject(archivoManejo.DescripcionAnexos));

            if (flAux == "I")
            {
                List<string> codigosCampos = archivoManejo.CodigosCampos;
                List<string> descripcionCampos = archivoManejo.DescripcionCampos;

                _session.SetValue($"EVT000_DS_ANEXO1", null);
                _session.SetValue($"EVT000_DS_ANEXO2", null);
                _session.SetValue($"EVT000_DS_ANEXO3", null);
                _session.SetValue($"EVT000_DS_ANEXO4", null);
                _session.SetValue($"EVT000_DS_ANEXO5", null);
                _session.SetValue($"EVT000_DS_ANEXO6", null);

                for (int i = 0; i < codigosCampos.Count; i++)
                {
                    string valor = infoCells.FirstOrDefault(w => w.datafield == codigosCampos[i])?.value;

                    context.AddParameter($"COL{i + 1}", $"{descripcionCampos[i]}:{valor}");
                    _session.SetValue($"EVT000_DS_ANEXO{i + 1}", valor);
                }
            }

            _session.SetValue("EVT000_FL_AUX", flAux);
            context.AddParameter("FL_AUX", flAux);

            return context;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems = new List<IGridItem> {
                new GridButton("btnNotificar", "General_Sec0_btn_Notificar"),
            };

            var listButton = new List<IGridItem>
            {
                new GridButton("btnDetalles", "General_Sec0_btn_Detalles", "fas fa-list"),
                new GridButton("btnVer", "General_Sec0_btn_Ver", "far fa-eye"),
                new GridButton("btnVideo", "General_Sec0_btn_UbicacionArchivo", ""),
                //new GridButton("btnDescargar", "General_Sec0_btn_Descargar", "fas fa-file-download"),
            };

            using var uow = this._uowFactory.GetUnitOfWork();

            if (this._security.IsUserAllowed("EVT000_grid1_btn_Editar"))
                listButton.Add(new GridButton("btnEditar", "General_Sec0_btn_Editar", "far fa-edit"));

            if (this._security.IsUserAllowed("EVT000_grid1_btn_Borrar"))
                listButton.Add(new GridButton("btnBorrar", "General_Sec0_btn_Borrar", ""));

            if (this._security.IsUserAllowed("General_Sec0_btn_Inactivar"))
                listButton.Add(new GridButton("btnInactivar", "General_Sec0_btn_Inactivar", ""));

            List<string> descripcionAnexos = JsonConvert.DeserializeObject<List<string>>(_session.GetValue<string>("EVT000_DS_ANEXOS"));

            for (int i = 1; i <= descripcionAnexos.Count; i++)
            {
                var column = grid.Columns.FirstOrDefault(w => w.Id == $"DS_ANEXO{i}");
                column.Name = descripcionAnexos[i - 1];
            }

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", listButton));

            return (_session.GetValue<string>("EVT000_FL_AUX") == "F") ? grid : this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            SortCommand defaultSort = new SortCommand("NU_ARCHIVO_ADJUNTO", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            AdjuntarArchivoQuery dbQuery = null;

            if (_session.GetValue<string>("EVT000_FL_AUX") == "I")
            {
                dbQuery = new AdjuntarArchivoQuery(
                    _session.GetValue<string>("EVT000_CD_EMPRESA").ToNumber<int>(),
                    _session.GetValue<string>("EVT000_CD_MANEJO"),
                    _session.GetValue<string>("EVT000_DS_REFERENCIA")
                );
            }
            else if (_session.GetValue<string>("EVT000_FL_AUX") == "A")
            {
                dbQuery = new AdjuntarArchivoQuery(JsonConvert.DeserializeObject<List<string>>(_session.GetValue<string>("EVT000_AVERIAS_INT")));
            }
            else
            {
                dbQuery = new AdjuntarArchivoQuery(
                    context.GetParameter("CD_EMPRESA"),
                    context.GetParameter("DS_REFERENCIA1"),
                    context.GetParameter("TP_DOCUMENTO"),
                    context.GetParameter("VL_FILTER")
                );
            }

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            SortCommand defaultSort = new SortCommand("NU_ARCHIVO_ADJUNTO", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            AdjuntarArchivoQuery dbQuery = null;

            if (_session.GetValue<string>("EVT000_FL_AUX") == "I")
            {

                dbQuery = new AdjuntarArchivoQuery(
                    _session.GetValue<string>("EVT000_CD_EMPRESA").ToNumber<int>(),
                    _session.GetValue<string>("EVT000_CD_MANEJO"),
                    _session.GetValue<string>("EVT000_DS_REFERENCIA")
                );
            }
            else if (_session.GetValue<string>("EVT000_FL_AUX") == "A")
            {
                dbQuery = new AdjuntarArchivoQuery(JsonConvert.DeserializeObject<List<string>>(_session.GetValue<string>("EVT000_AVERIAS_INT")));
            }
            else
            {
                dbQuery = new AdjuntarArchivoQuery(
                    context.GetParameter("CD_EMPRESA"),
                    context.GetParameter("DS_REFERENCIA1"),
                    context.GetParameter("TP_DOCUMENTO"),
                    context.GetParameter("VL_FILTER")
                );
            }

            uow.HandleQuery(dbQuery);

            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return _excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            if (context.ButtonId == "btnDetalles")
            {
                _session.SetValue("EVT000_NU_ARCHIVO_ADJUNTO", context.Row.GetCell("NU_ARCHIVO_ADJUNTO").Value);
                context.Redirect("/evento/EVT001");
                return context;
            }

            using var uow = this._uowFactory.GetUnitOfWork();

            long nuArchivoAdjunto = context.Row.GetCell("NU_ARCHIVO_ADJUNTO").Value.ToNumber<long>();

            if (context.ButtonId == "btnBorrar")
            {
                string ruta = this._parameterService.GetValue("PATH_ARCHIVOS_DIGITALES");
                List<string> listRutas = uow.ArchivoRepository.DeleteArchivo(nuArchivoAdjunto, ruta);
                uow.SaveChanges();

                this.BorrarArchivo(this._parameterService.GetValue("IP_ARCHIVOS_DIGITALES"), listRutas);

                context.AddSuccessNotification("General_Db_Success_Delete");
            }
            else if (context.ButtonId == "btnInactivar")
            {
                ArchivoAdjunto archivo = uow.ArchivoRepository.GetArchivoAdjunto(nuArchivoAdjunto);
                archivo.CD_SITUACAO = (short)EstadoArchivo.Inactivo;
                uow.ArchivoRepository.UpdateArchivoAdjunto(archivo, null);
                uow.SaveChanges();

                context.AddSuccessNotification("General_Db_Success_Inactive");
            }
            else if (context.ButtonId == "btnActivar")
            {
                ArchivoAdjunto archivo = uow.ArchivoRepository.GetArchivoAdjunto(nuArchivoAdjunto);
                archivo.CD_SITUACAO = (short)EstadoArchivo.Activo;
                uow.ArchivoRepository.UpdateArchivoAdjunto(archivo, null);
                uow.SaveChanges();

                context.AddSuccessNotification("General_Db_Success_Active");
            }
            else if (context.ButtonId == "btnVideo")
            {
                var splitRuta = context.Row.GetCell("LK_RUTA").Value.Split('\\').ToList();
                splitRuta = splitRuta.Take(splitRuta.Count() - 1).ToList();

                context.AddParameter("LK_RUTA", $"{this._parameterService.GetValue("COMPARTIDA_ARCHIVOS_DIGITALES")}{string.Join("\\", splitRuta)}");
            }

            return context;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            AdjuntarArchivoQuery dbQuery = null;

            if (_session.GetValue<string>("EVT000_FL_AUX") == "I")
            {

                dbQuery = new AdjuntarArchivoQuery(
                    _session.GetValue<string>("EVT000_CD_EMPRESA").ToNumber<int>(),
                    _session.GetValue<string>("EVT000_CD_MANEJO"),
                    _session.GetValue<string>("EVT000_DS_REFERENCIA")
                );

            }
            else if (_session.GetValue<string>("EVT000_FL_AUX") == "A")
            {
                dbQuery = new AdjuntarArchivoQuery(JsonConvert.DeserializeObject<List<string>>(_session.GetValue<string>("EVT000_AVERIAS_INT")));
            }
            else
            {
                dbQuery = new AdjuntarArchivoQuery(
                    context.GetParameter("CD_EMPRESA"),
                    context.GetParameter("DS_REFERENCIA1"),
                    context.GetParameter("TP_DOCUMENTO"),
                    context.GetParameter("VL_FILTER")
                );
            }

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            var keysRowSelected = dbQuery.GetKeysRowsSelected(context.Selection.AllSelected, context.Selection.Keys);

            Evento eventoArchivo = uow.EventoRepository.GetEvento(8);

            keysRowSelected.GroupBy(w => w.CD_EMPRESA).ToList().ForEach(w =>
            {
                ListaArchivos listaArchivo = new ListaArchivos { Empresa = w.Key, NumerosDeArchivos = w.Select(s => s.NU_ARCHIVO_ADJUNTO).ToList() };

                eventoArchivo.CrearBandeja(uow, JsonConvert.SerializeObject(listaArchivo));

            });

            uow.SaveChanges();

            context.AddSuccessNotification("General_Db_Success_Update");

            return context;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            AdjuntarArchivoQuery dbQuery = null;

            if (_session.GetValue<string>("EVT000_FL_AUX") == "I")
            {

                dbQuery = new AdjuntarArchivoQuery(
                    _session.GetValue<string>("EVT000_CD_EMPRESA").ToNumber<int>(),
                    _session.GetValue<string>("EVT000_CD_MANEJO"),
                    _session.GetValue<string>("EVT000_DS_REFERENCIA")
                );

            }
            else if (_session.GetValue<string>("EVT000_FL_AUX") == "A")
            {
                dbQuery = new AdjuntarArchivoQuery(JsonConvert.DeserializeObject<List<string>>(_session.GetValue<string>("EVT000_AVERIAS_INT")));
            }
            else
            {
                dbQuery = new AdjuntarArchivoQuery(
                    context.GetParameter("CD_EMPRESA"),
                    context.GetParameter("DS_REFERENCIA1"),
                    context.GetParameter("TP_DOCUMENTO"),
                    context.GetParameter("VL_FILTER")
                );
            }

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            FormField FieldTP_DOCUMENTO = form.GetField("TP_DOCUMENTO");
            FormField FieldARCHIVO = form.GetField("ARCHIVO");
            FieldARCHIVO.Options.Clear();

            var rowkey = context.GetParameter("FORM_UPDATE");

            if (!rowkey.IsNullOrEmpty())
            {
                var NU_ARCHIVO_ADJUNTO = context.GetParameter("NU_ARCHIVO_ADJUNTO");
                var CD_EMPRESA = context.GetParameter("CD_EMPRESA");
                var CD_MANEJO = context.GetParameter("CD_MANEJO");
                var DS_REFERENCIA = context.GetParameter("DS_REFERENCIA");

                ArchivoAdjunto obj = uow.ArchivoRepository.GetArchivoAdjunto(NU_ARCHIVO_ADJUNTO.ToNumber<long>(), CD_EMPRESA.ToNumber<int>(), CD_MANEJO, DS_REFERENCIA);

                if (obj == null) throw new ValidationFailedException("EVT000_Db_Error_ArchivoAdjuntoNoExiste");

                FieldTP_DOCUMENTO.Options = this.SelectTipoDocumento(uow);
                FieldTP_DOCUMENTO.Value = obj.TipoDocumento;
                form.GetField("DS_OBSERVACION").Value = obj.Observacion;
                form.GetField("DS_REFERENCIA2").Value = obj.DS_REFERENCIA2;

                _session.SetValue("EVT000_NU_ARCHIVO_ADJUNTO", obj.NU_ARCHIVO_ADJUNTO.ToString());
                _session.SetValue("EVT000_CD_EMPRESA", obj.CD_EMPRESA.ToString());
                _session.SetValue("EVT000_CD_MANEJO", obj.CD_MANEJO);
                _session.SetValue("EVT000_DS_REFERENCIA", obj.DS_REFERENCIA);

                context.AddParameter("FL_AUX", "U");
            }
            else
            {
                FieldTP_DOCUMENTO.Options = this.SelectTipoDocumento(uow);
                FieldTP_DOCUMENTO.Value = FieldTP_DOCUMENTO.Options.FirstOrDefault()?.Value ?? "";
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            bool IsUpdate = context.GetParameter("FL_AUX") == "U";

            ArchivoAdjunto archivo = IsUpdate ? uow.ArchivoRepository.GetArchivoAdjunto(
                _session.GetValue<string>("EVT000_NU_ARCHIVO_ADJUNTO").ToNumber<long>(),
                _session.GetValue<string>("EVT000_CD_EMPRESA").ToNumber<int>(),
                _session.GetValue<string>("EVT000_CD_MANEJO"),
                _session.GetValue<string>("EVT000_DS_REFERENCIA")
            ) : new ArchivoAdjunto();

            if (archivo == null) throw new ValidationFailedException("EVT000_Db_Error_ArchivoAdjuntoNoExiste");
            FormField FieldARCHIVO = form.GetField("ARCHIVO");

            string tpArchivo = FieldARCHIVO.Options?.FirstOrDefault(w => w.Label == "FILEEXTENSION")?.Value;

            archivo.TipoDocumento = form.GetField("TP_DOCUMENTO").Value;
            if (archivo.TipoDocumento.Equals("FDPC"))
            {
                if (uow.ArchivoRepository.AnyArchivoAdjunto(_session.GetValue<string>("EVT000_CD_EMPRESA").ToNumber<int>(), _session.GetValue<string>("EVT000_CD_MANEJO"), _session.GetValue<string>("EVT000_DS_REFERENCIA"), archivo.TipoDocumento))
                {
                    context.AddErrorNotification("EVT00_Sec0_Error_Error15_ProductoYaTieneFotoCatalogo");
                    return form;
                }
                else if (!tpArchivo.ToUpper().Equals("PNG") && !tpArchivo.ToUpper().Equals("JPG"))
                {
                    context.AddErrorNotification("EVT00_Sec0_Error_Error16_ArchivoNoValidoParaFotoDeCatalogo");
                    return form;
                }
            }
            archivo.Observacion = form.GetField("DS_OBSERVACION").Value;
            archivo.DS_REFERENCIA2 = form.GetField("DS_REFERENCIA2").Value;

            string payLoad = FieldARCHIVO.Options?.FirstOrDefault(w => w.Label == "PAYLOAD")?.Value;
            string fileName = FieldARCHIVO.Options?.FirstOrDefault(w => w.Label == "FILENAME")?.Value;

            if (string.IsNullOrEmpty(payLoad) /*&& new List<string> { "mp4" }.Contains(tpArchivo)*/)
            {
                archivo.NombreArchivo = fileName;
            }

            string cdManejo = _session.GetValue<string>("EVT000_CD_MANEJO");

            ArchivoManejo manejo = uow.ArchivoRepository.GetManejo(cdManejo);

            ArchivoVersion version = new ArchivoVersion
            {
                CD_FUNCIONARIO = _identity.UserId,
                TP_ARCHIVO = tpArchivo,
                SUB_LINK = manejo.Ruta,
            };

            if (string.IsNullOrEmpty(tpArchivo))
                throw new ValidationFailedException("Ingrese archivo");

            if (IsUpdate)
            {
                ArchivoVersion versionActiva = uow.ArchivoRepository.GetArchivoVersion(archivo);

                if ($"ARCTP{version.TP_ARCHIVO.ToUpper()}" != versionActiva.TP_ARCHIVO.ToUpper())
                {
                    throw new ValidationFailedException("EVT000_Op_Error_TipoArchivoDistVersionAnt");
                }

                uow.ArchivoRepository.UpdateArchivoAdjunto(archivo, version);
            }
            else
            {
                archivo.CD_EMPRESA = _session.GetValue<string>("EVT000_CD_EMPRESA").ToNumber<int>();
                archivo.CD_MANEJO = cdManejo;
                archivo.DS_REFERENCIA = _session.GetValue<string>("EVT000_DS_REFERENCIA");
                archivo.CD_SITUACAO = (short)EstadoArchivo.Activo;

                archivo.Anexo1 = _session.GetValue<string>("EVT000_DS_ANEXO1");
                archivo.Anexo2 = _session.GetValue<string>("EVT000_DS_ANEXO2");
                archivo.Anexo3 = _session.GetValue<string>("EVT000_DS_ANEXO3");
                archivo.Anexo4 = _session.GetValue<string>("EVT000_DS_ANEXO4");
                archivo.Anexo5 = _session.GetValue<string>("EVT000_DS_ANEXO5");
                archivo.Anexo6 = _session.GetValue<string>("EVT000_DS_ANEXO6");

                _session.SetValue("EVT000_DS_ANEXO1", null);
                _session.SetValue("EVT000_DS_ANEXO2", null);
                _session.SetValue("EVT000_DS_ANEXO3", null);
                _session.SetValue("EVT000_DS_ANEXO4", null);
                _session.SetValue("EVT000_DS_ANEXO5", null);
                _session.SetValue("EVT000_DS_ANEXO6", null);

                uow.ArchivoRepository.AddArchivoAdjunto(archivo, version);
            }

            uow.SaveChanges();

            if (!string.IsNullOrEmpty(tpArchivo))
            {
                this.GuardarArchivoAdjunto(
                        $"{this._parameterService.GetValue("IP_ARCHIVOS_DIGITALES")}",
                        $"{this._parameterService.GetValue("PATH_ARCHIVOS_DIGITALES")}{version.LK_RUTA}",
                        payLoad,
                        archivo.NombreArchivo
                    );
            }

            context.AddSuccessNotification(IsUpdate ? "General_Db_Success_Update" : "General_Db_Success_Insert");

            context.ResetForm = true;

            if (!string.IsNullOrEmpty(archivo.NombreArchivo))
            {
                context.AddWarningNotification("EVT000_Op_Warning_RecuerdeSubirElArchivo");
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new AdjuntarArchivoFormValidationModule(uow), form, context);
        }

        #region Metodos

        protected virtual List<SelectOption> SelectTipoDocumento(IUnitOfWork uow)
        {
            string tpManejo = _session.GetValue<string>("EVT000_CD_MANEJO");

            if (!string.IsNullOrEmpty(tpManejo))
            {
                return uow.ArchivoRepository.GetTipoDocumentoByManejo(tpManejo)
                    .Select(w => new SelectOption(w.Codigo, w.Descripcion))
                    .ToList();
            }
            else
            {
                return uow.ArchivoRepository.GetTipoDocumento()
                    .Select(w => new SelectOption(w.Codigo, w.Descripcion))
                    .ToList();
            }
        }

        protected virtual string GuardarArchivoAdjunto(string url, string ruta, string payLoad, string fileName)
        {
            var result = this._apiClient.Post(new Uri(new Uri(url), "GuardarArchivoAdjunto"), new ArchivoDigital
            {
                Ruta = ruta,
                Data = payLoad,
                NombreArchivo = fileName,
            });

            return result.Content.ReadAsStringAsync().Result;
        }

        protected virtual string BorrarArchivo(string url, List<string> listRutas)
        {
            var result = this._apiClient.Post(new Uri(new Uri(url), "BorrarArchivos"), JsonConvert.SerializeObject(listRutas));

            return result.Content.ReadAsStringAsync().Result;
        }

        #endregion
    }
}
