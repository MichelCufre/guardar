using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Permissions;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Domain.Eventos;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Domain.Services.Interfaces;
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
using WIS.Persistence.Database;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.REG
{
    public class REG220PanelAgentes : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ILogger<REG220PanelAgentes> _logger;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ITrackingService _trackingService;

        protected List<string> GridKeys { get; }

        public REG220PanelAgentes(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            ILogger<REG220PanelAgentes> logger,
            IGridValidationService gridValidationService,
            IFormValidationService formValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ITrackingService trackingService)
        {
            this.GridKeys = new List<string>
            {
                "CD_CLIENTE",
                "CD_EMPRESA"
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
            this._formValidationService = formValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._trackingService = trackingService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsAddEnabled = false;
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = false;

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("btnEditar", "General_Sec0_btn_Editar", "far fa-edit"),
                new GridButton("btnRutas", "REG220_Sec0_btn_Rutas", "fas fa-route"),
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            AgentesQuery dbQuery;
            if (context.Parameters.Count > 0)
            {

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa").Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");


                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "cliente").Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idCliente = context.Parameters.FirstOrDefault(s => s.Id == "cliente").Value;

                dbQuery = new AgentesQuery(idCliente, idEmpresa);
            }
            else
            {
                dbQuery = new AgentesQuery();
            }

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("DT_ALTERACAO", SortDirection.Descending);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string> { "ACTIVO" });

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            AgentesQuery dbQuery;

            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa").Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");


                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "cliente").Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idCliente = context.Parameters.FirstOrDefault(s => s.Id == "cliente").Value;

                dbQuery = new AgentesQuery(idCliente, idEmpresa);
            }
            else
            {
                dbQuery = new AgentesQuery();
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

            AgentesQuery dbQuery;
            if (context.Parameters.Count > 0)
            {

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa").Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");


                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "cliente").Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idCliente = context.Parameters.FirstOrDefault(s => s.Id == "cliente").Value;

                dbQuery = new AgentesQuery(idCliente, idEmpresa);
            }
            else
            {
                dbQuery = new AgentesQuery();
            }

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("DT_ALTERACAO", SortDirection.Descending);

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
                        var idCliente = row.GetCell("CD_CLIENTE").Value;
                        var codigoEmpresa = row.GetCell("CD_EMPRESA").Value;

                        Agente agente = uow.AgenteRepository.GetAgente(int.Parse(codigoEmpresa), idCliente);

                        if (!row.IsDeleted && !row.IsNew)
                        {
                            bool activo = row.GetCell("ACTIVO").Value == "S";
                            HandleUpdateRow(uow, agente, activo);
                        }
                    }
                }

                uow.SaveChanges();

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ExpectedException ex)
            {
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

            return this._gridValidationService.Validate(new MantenimientoAgenteGridValidationModule(uow), grid, row, context);
        }


        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (form.Id == "REG220_form_1")
            {
                this.InicializarSelects(form);
                this.InicializarCamposCreate(form);
            }
            if (form.Id == "REG220Update_form_1")
            {

                this.InicializarSelectsUpdate(form);

                var codigoAgente = context.GetParameter("keyCodigo");
                var codigoEmpresa = context.GetParameter("keyEmpresa");

                if (!string.IsNullOrEmpty(codigoAgente) && !string.IsNullOrEmpty(codigoEmpresa))
                {
                    Agente agente = uow.AgenteRepository.GetAgenteConRelaciones(int.Parse(codigoEmpresa), codigoAgente);

                    if (agente == null)
                        throw new EntityNotFoundException("REG220_Frm1_Error_AgenteNoExiste");

                    Empresa empresa = uow.EmpresaRepository.GetEmpresa(agente.Empresa);

                    DominioDetalle tipoAgente = uow.AgenteRepository.GetTipoAgente(agente);

                    this.InicializarCamposUpdate(uow, form, agente, empresa, tipoAgente);
                }
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber(this._identity.Application);
            uow.BeginTransaction();

            try
            {
                if (form.Id == "REG220_form_1")
                {
                    Agente agente = this.CrearAgente(uow, form);

                    uow.SaveChanges();

                    _trackingService.SincronizarAgente(agente, true);

                    uow.AgenteRepository.UpdateAgente(agente);

                    uow.SaveChanges();

                    AddContacto(uow, agente);

                    uow.SaveChanges();

                    context.AddSuccessNotification("REG220_Frm1_Succes_Creacion", new List<string> { agente.CodigoInterno });
                }
                else if (form.Id == "REG220Update_form_1")
                {
                    Agente agente = this.UpdateAgente(uow, form, context);

                    uow.SaveChanges();

                    _trackingService.SincronizarAgente(agente, true);

                    uow.AgenteRepository.UpdateAgente(agente);

                    uow.SaveChanges();

                    UpdateContacto(uow, agente);

                    uow.SaveChanges();

                    context.AddSuccessNotification("REG220_Frm1_Succes_Edicion", new List<string> { agente.CodigoInterno });
                }

                uow.Commit();
            }
            catch (ValidationFailedException ex)
            {
                this._logger.LogWarning(ex, "FormSubmit");
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                uow.Rollback();
            }
            catch (ExpectedException ex)
            {
                this._logger.LogWarning(ex, "FormSubmit");
                context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "FormSubmit");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
                uow.Rollback();
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new MantenimientoAgenteFormValidationModule(uow, SearchPais, SearchPaisSubdivision, this._identity.UserId, this._identity.GetFormatProvider()), form, context);
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "empresa": return this.SearchEmpresa(form, context);
                case "pais": return this.SearchPais(form, context);
                case "paisSubdivision": return this.SearchPaisSubdivision(form, context);
                case "localidad": return this.SearchLocalidad(form, context);
                default: return new List<SelectOption>();
            }
        }

        #region Metodos Auxiliares

        protected virtual void UpdateContacto(IUnitOfWork uow, Agente agente)
        {
            agente.Transaccion = uow.GetTransactionNumber();

            if (uow.DestinatarioRepository.AnyContacto(agente))
                uow.DestinatarioRepository.UpdateContacto(agente);
            else
                AddContacto(uow, agente);
        }

        protected virtual void AddContacto(IUnitOfWork uow, Agente agente)
        {
            uow.DestinatarioRepository.AddContacto(new Contacto
            {
                Id = uow.DestinatarioRepository.GetNextNuContacto(),
                CodigoCliente = agente.CodigoInterno,
                CodigoEmpresa = agente.Empresa,
                Descripcion = agente.Descripcion,
                Email = agente.Email,
                FechaAlta = DateTime.Now,
                FechaModificacion = DateTime.Now,
                Nombre = agente.Descripcion,
                NumeroTransaccion = uow.GetTransactionNumber(),
                Telefono = agente.TelefonoPrincipal,
                TipoDestinatario = TipoDestinatario.Contacto,
            });
        }

        public virtual void InicializarSelects(Form form)
        {
            //Inicializar selects
            FormField selectTipoAgente = form.GetField("tipoAgente");
            FormField selectTipoFiscal = form.GetField("tipoFiscal");
            FormField selectRuta = form.GetField("ruta");

            selectTipoAgente.Options = new List<SelectOption>();
            selectTipoFiscal.Options = new List<SelectOption>();
            selectRuta.Options = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            // Tipos de agentes
            List<DominioDetalle> tiposAgente = uow.DominioRepository.GetDominios(CodigoDominioDb.TiposDeAgentes);

            string[] tiposAgentesExcluidos = new string[] { TipoAgenteDb.Deposito, TipoAgenteDb.Interno };

            foreach (var tipo in tiposAgente.Where(a => !tiposAgentesExcluidos.Contains(a.Valor)).ToList())
            {
                selectTipoAgente.Options.Add(new SelectOption(tipo.Valor, tipo.Descripcion));
            }

            // Tipos fiscales
            List<DominioDetalle> tiposFiscales = uow.DominioRepository.GetDominios(CodigoDominioDb.TiposFiscales);
            foreach (var tipo in tiposFiscales)
            {
                selectTipoFiscal.Options.Add(new SelectOption(tipo.Id, tipo.Descripcion));
            }

            // Rutas
            var dbQuery = new RutaOndaQuery(this._identity.UserId);
            uow.HandleQuery(dbQuery);

            List<Ruta> rutas = dbQuery.GetRutasDisponibles();
            foreach (var ruta in rutas)
            {
                string descRuta = ruta.Descripcion;
                descRuta += " - " + (ruta.Onda == null ? "SIN ONDA" : ruta.Onda?.Descripcion);

                if (!string.IsNullOrEmpty(ruta.Zona))
                    descRuta += " - " + ruta.Zona;

                selectRuta.Options.Add(new SelectOption(ruta.Id.ToString(), descRuta));
            }
        }

        public virtual void InicializarSelectsUpdate(Form form)
        {
            //Inicializar selects
            FormField selectTipoAgente = form.GetField("tipoAgente");
            FormField selectTipoFiscal = form.GetField("tipoFiscal");
            FormField selectRuta = form.GetField("ruta");

            selectTipoAgente.Options = new List<SelectOption>();
            selectTipoFiscal.Options = new List<SelectOption>();
            selectRuta.Options = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            // Tipos de agentes
            List<DominioDetalle> tiposAgente = uow.DominioRepository.GetDominios(CodigoDominioDb.TiposDeAgentes);
            foreach (var tipo in tiposAgente)
            {
                selectTipoAgente.Options.Add(new SelectOption(tipo.Valor, tipo.Descripcion));
            }

            // Tipos fiscales
            List<DominioDetalle> tiposFiscales = uow.DominioRepository.GetDominios(CodigoDominioDb.TiposFiscales);
            foreach (var tipo in tiposFiscales)
            {
                selectTipoFiscal.Options.Add(new SelectOption(tipo.Id, tipo.Descripcion));
            }

            // Rutas
            var dbQuery = new RutaOndaQuery(this._identity.UserId);
            uow.HandleQuery(dbQuery);

            List<Ruta> rutas = dbQuery.GetRutasDisponibles();
            foreach (var ruta in rutas)
            {
                string descRuta = ruta.Descripcion;
                descRuta += " - " + (ruta.Onda == null ? "SIN ONDA" : ruta.Onda?.Descripcion);

                if (!string.IsNullOrEmpty(ruta.Zona))
                    descRuta += " - " + ruta.Zona;

                selectRuta.Options.Add(new SelectOption(ruta.Id.ToString(), descRuta)); ;
            }
        }

        public virtual void InicializarCamposCreate(Form form)
        {

        }

        public virtual void InicializarCamposUpdate(IUnitOfWork uow, Form form, Agente agente, Empresa empresa, DominioDetalle tipoAgente)
        {
            // Marcar campos solo de lectura
            form.GetField("empresa").ReadOnly = true;
            form.GetField("tipoAgente").ReadOnly = true;
            form.GetField("codigo").ReadOnly = true;

            // Cargar valores iniciales

            // Carga de search Empresa
            var fieldEmpresa = form.GetField("empresa");

            fieldEmpresa.Value = Convert.ToString(empresa.Id);
            fieldEmpresa.Options.Add(new SelectOption(fieldEmpresa.Value, empresa.Nombre));

            fieldEmpresa.Options = SearchEmpresa(form, new FormSelectSearchContext()
            {
                SearchValue = empresa.Id.ToString()
            });

            fieldEmpresa.Value = empresa.Id.ToString();

            form.GetField("tipoAgente").Value = tipoAgente.Valor;
            form.GetField("codigo").Value = agente.Codigo;
            form.GetField("descripcion").Value = agente.Descripcion;
            form.GetField("tipoFiscal").Value = agente.TipoFiscal == null ? null : agente.TipoFiscal.Id;
            form.GetField("numeroFiscal").Value = agente.NumeroFiscal;
            form.GetField("otroDatoFiscal").Value = agente.OtroDatoFiscal;
            form.GetField("locacionGlobal").Value = agente.NumeroLocalizacionGlobal.ToString();
            form.GetField("ruta").Value = agente.Ruta.Id.ToString();
            form.GetField("ordenCarga").Value = agente.OrdenDeCarga.ToString();

            form.GetField("valorManejaVidaUtil").Value = agente.ValorManejoVidaUtil.ToString();

            if (agente.Localidad != null)
            {
                // Carga de search Pais

                var fieldPais = form.GetField("pais");

                fieldPais.Value = agente.Localidad.Subdivision.Pais.Id;
                fieldPais.Options.Add(new SelectOption(fieldPais.Value, agente.Localidad.Subdivision.Pais.Nombre));

                // Carga de search Subdivision
                var fieldPaisSubdivision = form.GetField("paisSubdivision");

                fieldPaisSubdivision.Value = agente.Localidad.Subdivision.Id;
                fieldPaisSubdivision.Options.Add(new SelectOption(fieldPaisSubdivision.Value, agente.Localidad.Subdivision.Nombre));

                // Carga de search Localidad
                var fieldLocalidad = form.GetField("localidad");

                fieldLocalidad.Value = agente.Localidad.Id.ToString();
                fieldLocalidad.Options.Add(new SelectOption(fieldLocalidad.Value, agente.Localidad.Nombre));
            }

            form.GetField("direccion").Value = agente.Direccion;
            form.GetField("barrio").Value = agente.Barrio;
            form.GetField("codigoPostal").Value = agente.CodigoPostal;
            form.GetField("telefonoPrincipal").Value = agente.TelefonoPrincipal;
            form.GetField("telefonoSecundario").Value = agente.TelefonoSecundario;

            form.GetField("anexo1").Value = agente.Anexo1;
            form.GetField("anexo2").Value = agente.Anexo2;
            form.GetField("anexo3").Value = agente.Anexo3;
            form.GetField("anexo4").Value = agente.Anexo4;

            form.GetField("email").Value = agente.Email;
        }

        public virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Empresa> empresas = uow.EmpresaRepository.GetByNombreOrCodePartial(context.SearchValue);

            foreach (var empresa in empresas)
            {
                opciones.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}"));
            }

            return opciones;
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

        public virtual Agente CrearAgente(IUnitOfWork uow, Form form)
        {
            Agente agente = new Agente();

            agente.Tipo = form.GetField("tipoAgente").Value;
            agente.Empresa = int.Parse(form.GetField("empresa").Value);
            agente.Codigo = form.GetField("codigo").Value;
            agente.Descripcion = form.GetField("descripcion").Value;

            if (!string.IsNullOrEmpty(form.GetField("tipoFiscal").Value))
                agente.TipoFiscal = uow.DominioRepository.GetDominio(form.GetField("tipoFiscal").Value);

            agente.NumeroFiscal = form.GetField("numeroFiscal").Value;
            agente.OtroDatoFiscal = form.GetField("otroDatoFiscal").Value;

            if (long.TryParse(form.GetField("locacionGlobal").Value, out long numeroLocalizacion))
                agente.NumeroLocalizacionGlobal = numeroLocalizacion;

            if (short.TryParse(form.GetField("ruta").Value, out short codigoRuta))
                agente.Ruta = uow.RutaRepository.GetRuta(codigoRuta);

            if (short.TryParse(form.GetField("ordenCarga").Value, out short ordeCarga))
                agente.OrdenDeCarga = ordeCarga;

            if (string.IsNullOrEmpty(form.GetField("valorManejaVidaUtil")?.Value))
                agente.ValorManejoVidaUtil = 0;
            else if (decimal.TryParse(form.GetField("valorManejaVidaUtil")?.Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal valorManejaVidaUtil))
                agente.ValorManejoVidaUtil = valorManejaVidaUtil;

            SetLocalidad(uow, form, agente);

            agente.Direccion = form.GetField("direccion").Value;
            agente.Barrio = form.GetField("barrio").Value;
            agente.CodigoPostal = form.GetField("codigoPostal").Value;
            agente.TelefonoPrincipal = form.GetField("telefonoPrincipal").Value;
            agente.TelefonoSecundario = form.GetField("telefonoSecundario").Value;
            agente.Anexo1 = form.GetField("anexo1").Value;
            agente.Anexo3 = form.GetField("anexo3").Value;
            agente.Anexo2 = form.GetField("anexo2").Value;
            agente.Anexo4 = form.GetField("anexo4").Value;
            agente.Email = form.GetField("email").Value;

            agente.Estado = EstadoAgente.Activo;

            uow.AgenteRepository.AddAgente(agente);


            var venatanasLiberaconProductos = uow.ProduccionRepository.GetAllVentanasConfiguracionLiberacion(agente.Empresa);
            foreach (var venatanaLiberaconProducto in venatanasLiberaconProductos)
            {
                ClienteDiasValidezVentana clienteDiasValidezVentana = new ClienteDiasValidezVentana();
                clienteDiasValidezVentana.Empresa = agente.Empresa;
                clienteDiasValidezVentana.Cliente = agente.CodigoInterno;
                clienteDiasValidezVentana.CantidadDiasValidezLiberacion = 0;
                clienteDiasValidezVentana.VentanaLiberacion = venatanaLiberaconProducto;

                uow.AgenteRepository.AddVentanaLiberacionCliente(clienteDiasValidezVentana);
            }

            return agente;
        }

        public virtual Agente UpdateAgente(IUnitOfWork uow, Form form, FormSubmitContext context)
        {
            var codigoAgente = context.GetParameter("keyCodigo");
            var codigoEmpresa = context.GetParameter("keyEmpresa");

            Agente agente = uow.AgenteRepository.GetAgenteConRelaciones(int.Parse(codigoEmpresa), codigoAgente);

            if (agente == null)
                throw new EntityNotFoundException("REG220_Frm1_Error_AgenteNoExiste");

            agente.Descripcion = form.GetField("descripcion").Value;

            if (short.TryParse(form.GetField("ruta").Value, out short codigoRuta))
                agente.Ruta = uow.RutaRepository.GetRuta(codigoRuta);

            if (short.TryParse(form.GetField("ordenCarga").Value, out short ordeCarga))
                agente.OrdenDeCarga = ordeCarga;

            if (!string.IsNullOrEmpty(form.GetField("tipoFiscal").Value))
                agente.TipoFiscal = uow.DominioRepository.GetDominio(form.GetField("tipoFiscal").Value);

            agente.NumeroFiscal = form.GetField("numeroFiscal").Value;
            agente.OtroDatoFiscal = form.GetField("otroDatoFiscal").Value;

            if (long.TryParse(form.GetField("locacionGlobal").Value, out long numeroLocalizacion))
                agente.NumeroLocalizacionGlobal = numeroLocalizacion;

            if (string.IsNullOrEmpty(form.GetField("valorManejaVidaUtil")?.Value))
                agente.ValorManejoVidaUtil = 0;
            else if (decimal.TryParse(form.GetField("valorManejaVidaUtil")?.Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal valorManejaVidaUtil))
                agente.ValorManejoVidaUtil = valorManejaVidaUtil;

            SetLocalidad(uow, form, agente);

            agente.Direccion = form.GetField("direccion").Value;
            agente.Barrio = form.GetField("barrio").Value;
            agente.CodigoPostal = form.GetField("codigoPostal").Value;
            agente.TelefonoPrincipal = form.GetField("telefonoPrincipal").Value;
            agente.TelefonoSecundario = form.GetField("telefonoSecundario").Value;
            agente.Anexo1 = form.GetField("anexo1").Value;
            agente.Anexo3 = form.GetField("anexo3").Value;
            agente.Anexo2 = form.GetField("anexo2").Value;
            agente.Anexo4 = form.GetField("anexo4").Value;
            agente.Email = form.GetField("email").Value;

            uow.AgenteRepository.UpdateAgente(agente);

            return agente;
        }

        public virtual void SetLocalidad(IUnitOfWork uow, Form form, Agente agente)
        {
            if (long.TryParse(form.GetField("localidad").Value, out long localidad))
            {
                // Se ingresa una localidad
                agente.Localidad = uow.PaisSubdivisionLocalidadRepository.GetLocalidad(localidad);
            }
            else
            {
                // No se especifica una localidad, pero se indico un pais o subdivisión, por defecto se ingresara Sin Especificar
                var codigoPais = form.GetField("pais").Value;
                var codigoSubdivision = form.GetField("paisSubdivision").Value;

                if (!string.IsNullOrEmpty(codigoSubdivision))
                {
                    agente.Localidad = uow.PaisSubdivisionLocalidadRepository.GetLocalidad(codigoSubdivision, GeneralDb.SinEspecificar);
                }
                else if (!string.IsNullOrEmpty(codigoPais))
                {
                    agente.Localidad = uow.PaisSubdivisionLocalidadRepository.GetLocalidad(codigoPais, $"{codigoPais}-{GeneralDb.SinEspecificar}", GeneralDb.SinEspecificar);
                }
            }
        }

        public virtual void HandleUpdateRow(IUnitOfWork uow, Agente agente, bool activo)
        {
            if (agente == null)
                throw new EntityNotFoundException("REG220_Frm1_Error_AgenteNoExiste");

            if (activo)
                agente.Enable();
            else
                agente.Disable();

            _trackingService.SincronizarAgente(agente, false);
            uow.AgenteRepository.UpdateAgente(agente);
        }


        #endregion
    }
}