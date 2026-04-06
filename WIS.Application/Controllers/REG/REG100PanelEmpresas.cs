using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Domain.Registro;
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
    public class REG100PanelEmpresas : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ILogger<REG100PanelEmpresas> _logger;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }

        public REG100PanelEmpresas(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            ILogger<REG100PanelEmpresas> logger,
            IFormValidationService formValidationService,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "CD_EMPRESA"
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

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsAddEnabled = false;
            context.IsEditingEnabled = true;
            context.IsCommitEnabled = true;
            context.IsRemoveEnabled = false;

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("btnEditar", "General_Sec0_btn_Editar", "far fa-edit"),
                new GridButton("btnVerTipoRecepcion", "REG100_Sec0_btn_VerTipos", "far fa-bookmark"),
                new GridButton("btnVerCodigosMutiDato", "REG100_Sec0_btn_VerCodigosMDato", "fa-solid fa-barcode")
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            EmpresasQuery dbQuery;

            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa").Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new EmpresasQuery(idEmpresa);
            }
            else
            {
                dbQuery = new EmpresasQuery();
            }

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("DT_UPDROW", SortDirection.Descending);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string> { "ACTIVO" });

            DisableButtons(grid, uow);

            return grid;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            EmpresasQuery dbQuery;

            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa").Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new EmpresasQuery(idEmpresa);
            }
            else
            {
                dbQuery = new EmpresasQuery();
            }

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            EmpresasQuery dbQuery;

            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa").Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new EmpresasQuery(idEmpresa);
            }
            else
            {
                dbQuery = new EmpresasQuery();
            }

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("DT_UPDROW", SortDirection.Descending);

            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }
        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                if (grid.Rows.Any())
                {
                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    foreach (var row in grid.Rows)
                    {

                        if (row.IsNew)
                        {
                        }
                        else if (row.IsDeleted)
                        {
                            // rows delete
                        }
                        else
                        {
                            // rows editadas

                            EmpresaMapper empresaMapper = new EmpresaMapper();

                            bool activo = empresaMapper.MapStringToBoolean(row.GetCell("ACTIVO").Value);

                            var codigoEmpresa = row.GetCell("CD_EMPRESA").Value;

                            Empresa empresa = uow.EmpresaRepository.GetEmpresa(int.Parse(codigoEmpresa));

                            this.HandleUpdateRow(uow, empresa, activo);

                        }
                    }

                }

                uow.SaveChanges();

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ExpectedException ex)
            {
                // logger.Warn(ex, "GridCommit");
                context.AddErrorNotification(ex.Message);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "REG220GridCommit");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoEmpresaGridValidationModule(uow), grid, row, context);
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            this.InicializarSelects(form);

            if (form.Id == "REG100Create_form_1")
            {

            }
            if (form.Id == "REG100Update_form_1")
            {
                var codigoEmpresa = context.GetParameter("keyEmpresa");

                if (!string.IsNullOrEmpty(codigoEmpresa))
                {

                    Empresa empresa = uow.EmpresaRepository.GetEmpresaConRelaciones(int.Parse(codigoEmpresa));

                    if (empresa == null)
                        throw new EntityNotFoundException("REG100_Frm1_Error_EmpresaNoExiste");

                    this.InicializarCamposUpdate(uow, form, empresa);

                    context.AddParameter("tipoNotificacionWebhook", CodigoDominioDb.TipoNotificacionWebhook);
                }
            }

            return form;
        }
        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                if (form.Id == "REG100Create_form_1")
                {
                    // Crear nuevo
                    Empresa empresa = this.CrearEmpresa(uow, form);

                    uow.SaveChanges();

                    context.AddSuccessNotification("REG100_Frm1_Succes_Creacion", new List<string> { empresa.Id.ToString() });

                }
                else if (form.Id == "REG100Update_form_1")
                {
                    // Edición 
                    Empresa empresa = this.UpdateEmpresa(uow, form, context);

                    uow.SaveChanges();

                    context.AddSuccessNotification("REG100_Frm1_Succes_Edicion", new List<string> { empresa.Id.ToString() });

                }

            }
            catch (ExpectedException ex)
            {
                this._logger.LogWarning(ex, "FormSubmit");
                context.AddErrorNotification(ex.Message);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "FormSubmit");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return form;
        }
        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new MantenimientoEmpresaFormValidationModule(uow, SearchPais, SearchPaisSubdivision, this._identity.GetFormatProvider()), form, context);
        }
        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "pais": return this.SearchPais(form, context);
                case "paisSubdivision": return this.SearchPaisSubdivision(form, context);
                case "localidad": return this.SearchLocalidad(form, context);
                default: return new List<SelectOption>();
            }
        }

        #region Metodo Auxiliares

        public virtual void InicializarSelects(Form form)
        {
            //Inicializar selects
            FormField selectTipoAlmacenaje = form.GetField("codigoAlmacenaje");
            FormField selectTipoFiscal = form.GetField("tipoFiscal");
            FormField selectTipoNotificacion = form.GetField("tipoNotificacion");

            selectTipoAlmacenaje.Options = new List<SelectOption>();
            selectTipoFiscal.Options = new List<SelectOption>();
            selectTipoNotificacion.Options = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            // Tipos de almacenaje y seguro
            List<TipoDeAlmacenajeYSeguro> tiposAlmacenajes = uow.TipoAlmacenajeSeguroRepository.GetTiposDeAlmacenajeYSeguro();

            foreach (var tipo in tiposAlmacenajes)
            {
                selectTipoAlmacenaje.Options.Add(new SelectOption(tipo.Tipo.ToString(), $"{tipo.Tipo} - {tipo.Descripcion}"));
            }
            selectTipoAlmacenaje.Value = "1";

            // Tipos fiscales
            List<DominioDetalle> tiposFiscales = uow.DominioRepository.GetDominios(CodigoDominioDb.TiposFiscales);
            foreach (var tipo in tiposFiscales)
            {
                selectTipoFiscal.Options.Add(new SelectOption(tipo.Id, tipo.Descripcion));
            }

            // Tipos de notificación
            List<DominioDetalle> tiposNotificaciones = uow.DominioRepository.GetDominios(CodigoDominioDb.TiposNotificaciones);
            foreach (var tipo in tiposNotificaciones)
            {
                selectTipoNotificacion.Options.Add(new SelectOption(tipo.Id, tipo.Descripcion));
            }
        }
        public virtual void InicializarCamposUpdate(IUnitOfWork uow, Form form, Empresa empresa)
        {
            // Marcar campos solo de lectura
            form.GetField("codigoEmpresa").ReadOnly = true;

            // Cargar valores iniciales

            form.GetField("codigoEmpresa").Value = empresa.Id.ToString();

            form.GetField("nombreEmpresa").Value = empresa.Nombre;
            form.GetField("codigoAlmacenaje").Value = empresa.cdTipoDeAlmacenajeYSeguro.ToString();
            form.GetField("minimoStock").Value = empresa.ValorMinimoStock.ToString();
            form.GetField("tipoFiscal").Value = empresa.TipoFiscal == null ? null : empresa.TipoFiscal.Id;
            form.GetField("tipoNotificacion").Value = empresa.TipoNotificacion?.Id;
            form.GetField("payloadUrl").Value = empresa.PayloadUrl;
            form.GetField("numeroFiscal").Value = empresa.NumeroFiscal;
            form.GetField("telefono").Value = empresa.Telefono;
            form.GetField("direccion").Value = empresa.Direccion;
            form.GetField("codigoPostal").Value = empresa.CodigoPostal;
            form.GetField("anexo1").Value = empresa.Anexo1;
            form.GetField("anexo2").Value = empresa.Anexo2;
            form.GetField("anexo3").Value = empresa.Anexo3;
            form.GetField("anexo4").Value = empresa.Anexo4;
            form.GetField("habilitadoCambioSecret").Value = "N";

            if (empresa.Localidad != null)
            {
                // Carga de search Pais
                var fieldPais = form.GetField("pais");
                fieldPais.Options = SearchPais(form, new FormSelectSearchContext()
                {
                    SearchValue = empresa.Localidad.Subdivision.Pais.Id
                });

                fieldPais.Value = empresa.Localidad.Subdivision.Pais.Id;

                // Carga de search Subdivision
                var fieldPaisSubdivision = form.GetField("paisSubdivision");
                fieldPaisSubdivision.Options = SearchPaisSubdivision(form, new FormSelectSearchContext()
                {
                    SearchValue = empresa.Localidad.Subdivision.Id
                });

                fieldPaisSubdivision.Value = empresa.Localidad.Subdivision.Id;

                // Carga de search Localidad
                var fieldLocalidad = form.GetField("localidad");

                fieldLocalidad.Options = SearchLocalidad(form, new FormSelectSearchContext()
                {
                    SearchValue = empresa.Localidad.Id.ToString()
                });

                fieldLocalidad.Value = empresa.Localidad.Id.ToString();

            }

        }

        public virtual List<SelectOption> SearchPais(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Pais> paises = uow.PaisRepository.GetByNombreOrIdPartial(context.SearchValue);

            foreach (var pais in paises)
            {
                opciones.Add(new SelectOption(pais.Id.ToString(), pais.Nombre));
            }

            return opciones;
        }
        public virtual List<SelectOption> SearchPaisSubdivision(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<PaisSubdivision> subdivisiones = null;

            var codigoPais = form.GetField("pais").Value;

            if (string.IsNullOrEmpty(codigoPais))
            {
                subdivisiones = uow.PaisSubdivisionRepository.GetByNombreOrIdPartial(context.SearchValue);
            }
            else
            {
                subdivisiones = uow.PaisSubdivisionRepository.GetByNombreOrIdPartial(context.SearchValue, codigoPais);
            }

            foreach (var subdivision in subdivisiones)
            {
                opciones.Add(new SelectOption(subdivision.Id.ToString(), subdivision.Nombre));
            }

            return opciones;
        }
        public virtual List<SelectOption> SearchLocalidad(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<PaisSubdivisionLocalidad> localidades = null;

            var codigoPais = form.GetField("pais").Value;
            var codigoSubdivision = form.GetField("paisSubdivision").Value;

            if (string.IsNullOrEmpty(codigoPais) && string.IsNullOrEmpty(codigoSubdivision))
            {
                localidades = uow.PaisSubdivisionLocalidadRepository.GetByNombreOrIdPartial(context.SearchValue);
            }
            else
            {
                localidades = uow.PaisSubdivisionLocalidadRepository.GetByNombreOrIdPartial(context.SearchValue, codigoPais, codigoSubdivision);
            }

            foreach (var localidad in localidades)
            {
                opciones.Add(new SelectOption(localidad.Id.ToString(), localidad.Nombre));
            }

            return opciones;
        }

        public virtual Empresa CrearEmpresa(IUnitOfWork uow, Form form)
        {
            Empresa empresa = new Empresa();

            empresa.Id = int.Parse(form.GetField("codigoEmpresa").Value);
            empresa.Nombre = form.GetField("nombreEmpresa").Value;
            empresa.cdTipoDeAlmacenajeYSeguro = short.Parse(form.GetField("codigoAlmacenaje").Value);

            if (decimal.TryParse(form.GetField("minimoStock").Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal minimoStock))
                empresa.ValorMinimoStock = minimoStock;

            if (!string.IsNullOrEmpty(form.GetField("tipoFiscal").Value))
                empresa.TipoFiscal = uow.DominioRepository.GetDominio(form.GetField("tipoFiscal").Value);

            empresa.NumeroFiscal = form.GetField("numeroFiscal").Value;
            empresa.Telefono = form.GetField("telefono").Value;
            empresa.Direccion = form.GetField("direccion").Value;

            SetLocalidad(uow, form, empresa);

            empresa.CodigoPostal = form.GetField("codigoPostal").Value;

            empresa.Anexo1 = form.GetField("anexo1").Value;
            empresa.Anexo3 = form.GetField("anexo3").Value;
            empresa.Anexo2 = form.GetField("anexo2").Value;
            empresa.Anexo4 = form.GetField("anexo4").Value;

            empresa.Estado = EstadoEmpresa.Activo;

            var tipoNotificacion = form.GetField("tipoNotificacion").Value;
            string secret = null;
            empresa.PayloadUrl = null;

            if (!string.IsNullOrEmpty(tipoNotificacion))
            {
                empresa.TipoNotificacion = uow.DominioRepository.GetDominio(tipoNotificacion);

                if (tipoNotificacion == CodigoDominioDb.TipoNotificacionWebhook)
                {
                    var payloadUrl = form.GetField("payloadUrl").Value;
                    secret = form.GetField("secret").Value;
                    empresa.PayloadUrl = payloadUrl;
                }
            }

            new RegistroEmpresa(uow, this._identity.UserId, this._identity.Application).RegistrarEmpresa(empresa, secret);

            return empresa;
        }
        public virtual Empresa UpdateEmpresa(IUnitOfWork uow, Form form, FormSubmitContext context)
        {
            var codigoEmpresa = context.GetParameter("keyEmpresa");

            Empresa empresa = uow.EmpresaRepository.GetEmpresa(int.Parse(codigoEmpresa));

            if (empresa == null)
                throw new EntityNotFoundException("REG100_Frm1_Error_EmpresaNoExiste");

            empresa.Nombre = form.GetField("nombreEmpresa").Value;

            empresa.cdTipoDeAlmacenajeYSeguro = short.Parse(form.GetField("codigoAlmacenaje").Value);

            if (decimal.TryParse(form.GetField("minimoStock").Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal minimoStock))
                empresa.ValorMinimoStock = minimoStock;

            var tipoFiscal = form.GetField("tipoFiscal").Value;
            if (!string.IsNullOrEmpty(tipoFiscal))
                empresa.TipoFiscal = uow.DominioRepository.GetDominio(tipoFiscal);

            empresa.NumeroFiscal = form.GetField("numeroFiscal").Value;
            empresa.Telefono = form.GetField("telefono").Value;
            empresa.Direccion = form.GetField("direccion").Value;

            SetLocalidad(uow, form, empresa);

            empresa.CodigoPostal = form.GetField("codigoPostal").Value;

            empresa.Anexo1 = form.GetField("anexo1").Value;
            empresa.Anexo3 = form.GetField("anexo3").Value;
            empresa.Anexo2 = form.GetField("anexo2").Value;
            empresa.Anexo4 = form.GetField("anexo4").Value;

            var tipoNotificacion = form.GetField("tipoNotificacion").Value;
            string secret = null;
            empresa.PayloadUrl = null;

            if (!string.IsNullOrEmpty(tipoNotificacion))
            {
                empresa.TipoNotificacion = uow.DominioRepository.GetDominio(tipoNotificacion);

                if (tipoNotificacion == CodigoDominioDb.TipoNotificacionWebhook)
                {
                    var payloadUrl = form.GetField("payloadUrl").Value;
                    var habilitadoCambioSecret = bool.Parse(form.GetField("habilitadoCambioSecret").Value);

                    if (habilitadoCambioSecret)
                    {
                        secret = form.GetField("secret").Value;
                    }

                    empresa.PayloadUrl = payloadUrl;
                }
            }

            new RegistroEmpresa(uow, this._identity.UserId, this._identity.Application).ActualizarEmpresa(empresa, secret);

            return empresa;
        }
        public virtual void SetLocalidad(IUnitOfWork uow, Form form, Empresa empresa)
        {
            if (long.TryParse(form.GetField("localidad").Value, out long localidad))
            {
                // Se ingresa una localidad
                empresa.Localidad = uow.PaisSubdivisionLocalidadRepository.GetLocalidad(localidad);
            }
            else
            {
                // No se especifica una localidad, pero se indico un pais o subdivisión, por defecto se ingresara Sin Especificar
                var codigoPais = form.GetField("pais").Value;
                var codigoSubdivision = form.GetField("paisSubdivision").Value;

                if (!string.IsNullOrEmpty(codigoSubdivision))
                {
                    empresa.Localidad = uow.PaisSubdivisionLocalidadRepository.GetLocalidad(codigoSubdivision, GeneralDb.SinEspecificar);
                    empresa.IdLocalidad = empresa.Localidad.Id;
                }
                else if (!string.IsNullOrEmpty(codigoPais))
                {
                    empresa.Localidad = uow.PaisSubdivisionLocalidadRepository.GetLocalidad(codigoPais, $"{codigoPais}-{GeneralDb.SinEspecificar}", GeneralDb.SinEspecificar);
                    empresa.IdLocalidad = empresa.Localidad.Id;
                }
            }
        }

        public virtual void HandleUpdateRow(IUnitOfWork uow, Empresa empresa, bool activo)
        {
            if (empresa == null)
                throw new EntityNotFoundException("REG100_Frm1_Error_EmpresaNoExiste");

            if (activo)
                empresa.Enable();
            else
                empresa.Disable();

            uow.EmpresaRepository.UpdateEmpresa(empresa);
        }

        public virtual void DisableButtons(Grid grid, IUnitOfWork uow)
        {
            var codigoMultidatoHabilitado = uow.ParametroRepository.GetParametroConfiguracion(ParamManager.CODIGO_MULTIDATO_HABILITADO, ParamManager.PARAM_GRAL)?.Valor;

            if (codigoMultidatoHabilitado != "S")
            {
                foreach (var row in grid.Rows)
                {
                    row.DisabledButtons = new List<string>() { "btnVerCodigosMutiDato" };
                }
            }
        }

        #endregion
    }
}