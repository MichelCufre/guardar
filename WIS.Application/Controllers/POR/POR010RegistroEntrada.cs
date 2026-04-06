using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Porteria;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Porteria;
using WIS.Domain.General;
using WIS.Domain.Porteria;
using WIS.Domain.Recepcion;
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
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.POR
{
    public class POR010RegistroEntrada : AppController
    {
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IParameterService _parameterService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> Grid1Keys { get; }
        protected List<string> GridPersonasKeys { get; }

        public POR010RegistroEntrada(
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            IParameterService parameterService,
            IFilterInterpreter filterInterpreter)
        {
            this.Grid1Keys = new List<string>
            {
                "NU_PORTERIA_REGISTRO_PERSONA"
            };

            this.GridPersonasKeys = new List<string>
            {
                "NU_POTERIA_PERSONA"
            };

            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._parameterService = parameterService;
            this._filterInterpreter = filterInterpreter;

        }


        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            switch (grid.Id)
            {
                case "POR010_grid_1": return this.Grid1FetchRows(grid, query.FetchContext);
                case "POR010_grid_Personas":
                    {

                        grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton> {

                        new GridButton("btnBorrar", "General_Sec0_btn_Borrar", "fas fa-trash-alt"),

                    }));

                        return this.GridPersonasFetchRows(grid, query.FetchContext);
                    }
            }

            return grid;
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            switch (grid.Id)
            {
                case "POR010_grid_1": return this.Grid1FetchRows(grid, query);
                case "POR010_grid_Personas": return this.GridPersonasFetchRows(grid, query);
            }

            return grid;
        }
        public virtual Grid Grid1FetchRows(Grid grid, GridFetchContext query)
        {
            SortCommand defaultSort = new SortCommand("NU_PORTERIA_REGISTRO_PERSONA", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PorteriaRegistroEntradaQuery();

            uow.HandleQuery(dbQuery, true);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, defaultSort, this.Grid1Keys);

            return grid;
        }
        public virtual Grid GridPersonasFetchRows(Grid grid, GridFetchContext query)
        {
            SortCommand defaultSort = new SortCommand("NU_POTERIA_PERSONA", SortDirection.Descending);

            List<int> listaPersonas = JsonConvert.DeserializeObject<List<int>>(query.GetParameter("SelectionGridPersonas"));

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PorteriaRegistroEntradaPersonasQuery(listaPersonas);

            uow.HandleQuery(dbQuery, true);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, defaultSort, this.GridPersonasKeys);

            return grid;
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            if (grid.Id == "POR010_grid_Personas") return new byte[0];

            SortCommand defaultSort = new SortCommand("NU_PORTERIA_REGISTRO_PERSONA", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PorteriaRegistroEntradaQuery();

            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}{DateTime.Now:yyyy-MM-dd_HH:mm}.xlsx";

            return _excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, defaultSort);
        }
        public override GridButtonActionContext GridButtonAction(GridButtonActionContext data)
        {
            List<int> listaPersonas = JsonConvert.DeserializeObject<List<int>>(data.GetParameter("SelectionGridPersonas"));

            listaPersonas.Remove(data.Row.Id.ToNumber<int>());

            data.AddParameter("SelectionGridPersonas", JsonConvert.SerializeObject(listaPersonas));

            return data;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            switch (grid.Id)
            {
                case "POR010_grid_1":
                    {
                        var dbQuery = new PorteriaRegistroEntradaQuery();

                        uow.HandleQuery(dbQuery);
                        dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                        return new GridStats
                        {
                            Count = dbQuery.GetCount()
                        };
                    }
                case "POR010_grid_Personas":
                    {
                        List<int> listaPersonas = JsonConvert.DeserializeObject<List<int>>(query.GetParameter("SelectionGridPersonas"));
                        var dbQuery = new PorteriaRegistroEntradaPersonasQuery(listaPersonas);

                        uow.HandleQuery(dbQuery);
                        dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                        return new GridStats
                        {
                            Count = dbQuery.GetCount()
                        };
                    }
            }
            return null;
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            //query.AddParameter("tiposPorteriaHabilitados", uow.PorteriaRepository.GetTipoPorterias().Select(w => w.Valor).ToArray().Serialize());

            query.AddParameter("motivosSegunTipoPorteria", JsonConvert.SerializeObject(new
            {
                PRO = this._parameterService.GetValueByDomain("POR010_ND_POTERIA_MOTIVO", "PORTPREGPRO"),
                EMP = this._parameterService.GetValueByDomain("POR010_ND_POTERIA_MOTIVO", "PORTPREGEMP"),
                CLI = this._parameterService.GetValueByDomain("POR010_ND_POTERIA_MOTIVO", "PORTPREGCLI"),
                POF = this._parameterService.GetValueByDomain("POR010_ND_POTERIA_MOTIVO", "PORTPREGPOF")
            }));

            form.GetField("CD_POTERIA_MOTIVO").Options = this.SelectMotivo(uow);

            form.GetField("CD_TRANSPORTE_V1").Options = this.SelectTipoTransporte(uow);

            FormField FieldNU_PREDIO = form.GetField("NU_PREDIO");
            FieldNU_PREDIO.Options = this.SelectPredio(uow);
            FieldNU_PREDIO.Value = _identity.Predio == GeneralDb.PredioSinDefinir ? FieldNU_PREDIO.Options?.FirstOrDefault()?.Value ?? "" : _identity.Predio;
            FieldNU_PREDIO.ReadOnly = false;

            FormField fieldCD_SECTOR = form.GetField("CD_SECTOR");
            fieldCD_SECTOR.Options = this.SelectSector(uow, FieldNU_PREDIO.Value);
            fieldCD_SECTOR.Value = fieldCD_SECTOR.Options?.FirstOrDefault()?.Value ?? "";
            fieldCD_SECTOR.ReadOnly = false;

            form.GetField("CD_EMPRESA").ReadOnly = false;
            form.GetField("CD_CLIENTE").ReadOnly = false;

            form.GetField("CD_TRANSPORTE_V1").ReadOnly = false;
            form.GetField("VL_MATRICULA_1_ENTRADA").ReadOnly = false;
            form.GetField("VL_MATRICULA_2_ENTRADA").ReadOnly = false;
            form.GetField("CD_POTERIA_MOTIVO").ReadOnly = false;

            return form;
        }
        public override Form FormSubmit(Form form, FormSubmitContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            bool isPreRegistro = query.GetParameter("FL_PRE_REGISTRO") == "S";

            if (query.ButtonId == "btnConfirmar")
            {
                uow.CreateTransactionNumber(query.ButtonId);
                uow.BeginTransaction();

                bool tienePreRegistro = false;

                int? cdEmpresa = form.GetField("CD_EMPRESA").Value.ToNumber<int?>();
                string cdTpRegistro = form.GetField("CD_TP_POTERIA_REGISTRO").Value;
                string dsNota = form.GetField("DS_NOTA")?.Value;
                string nuPredio = form.GetField("NU_PREDIO")?.Value;
                string cdSector = form.GetField("CD_SECTOR")?.Value;
                string motivo = form.GetField("CD_POTERIA_MOTIVO").Value;

                List<int> listaPersonas = JsonConvert.DeserializeObject<List<int>>(query.GetParameter("SelectionGridPersonas"));

                if (string.IsNullOrEmpty(motivo) || !motivo.Equals("BAL"))
                {
                    if (listaPersonas == null || listaPersonas.Count == 0)
                        throw new ValidationFailedException("WPOR010_Sec0_Error_Er012_SelecPersonaRegistroEntrada");
                }

                Agente agente = cdEmpresa == null ? null : uow.AgenteRepository.GetAgente((int)cdEmpresa, form.GetField("CD_CLIENTE").Value);

                string cdAgente = agente == null ? null : agente.Codigo;

                #region ***************** SET VEHICULO *****************

                string matricula1 = form.GetField("VL_MATRICULA_1_ENTRADA").Value?.ToUpper();

                PorteriaRegistroVehiculo nuevoRegistroVehiculo = uow.PorteriaRepository.GetPreRegistroVehiculoByMatricula(matricula1);
                bool personaAgregada = false;

                decimal? vlPesoEntradaVe = form.GetField("VL_PESO_ENTRADA_VE").Value.ToNumber<decimal?>();

                if (nuevoRegistroVehiculo != null && DentroIntervaloFacturacion(uow, nuevoRegistroVehiculo.DT_ADDROW ?? DateTime.Now, DateTime.Now) && nuevoRegistroVehiculo.NU_EJECUCION_ENTRADA != null && nuevoRegistroVehiculo.NU_EJECUCION_SALIDA == null && cdTpRegistro != "POF")
                {
                    nuevoRegistroVehiculo.VL_PESO_SALIDA = vlPesoEntradaVe;
                    nuevoRegistroVehiculo.DT_PORTERIA_SALIDA = DateTime.Now;

                    foreach (PorteriaRegistroPersona persona in nuevoRegistroVehiculo.Personas)
                    {
                        persona.NU_PORTERIA_VEHICULO_SALIDA = nuevoRegistroVehiculo.NU_PORTERIA_VEHICULO;
                        persona.DT_PERSONA_SALIDA = DateTime.Now;

                        if (!string.IsNullOrEmpty(persona.DS_NOTA))
                        {
                            persona.DS_NOTA += " \n " + dsNota;
                            personaAgregada = true;
                        }
                        else
                        {
                            persona.DS_NOTA = dsNota;
                        }

                        persona.ND_ESTADO = uow.DominioRepository.GetDominio(DominioPorteriaDb.CD_DOMINIO_PORTERIA_ESTADO, "SAL")?.Id;

                        uow.PorteriaRepository.UpdateRegistroPersona(persona);
                    }

                    uow.PorteriaRepository.UpdateRegistroVehiculo(nuevoRegistroVehiculo);

                }
                else if (nuevoRegistroVehiculo != null && nuevoRegistroVehiculo.DT_PORTERIA_ENTRADA == null)
                {
                    if (isPreRegistro)
                    {
                        throw new ValidationFailedException("Este vehiculo ya tiene un pre-registro, no puede reingresar otro.");
                    }

                    tienePreRegistro = true;

                    nuevoRegistroVehiculo.DT_PORTERIA_ENTRADA = DateTime.Now;

                    uow.PorteriaRepository.UpdateRegistroVehiculo(nuevoRegistroVehiculo);
                }
                else
                {
                    nuevoRegistroVehiculo = new PorteriaRegistroVehiculo();

                    nuevoRegistroVehiculo.ND_TRANSPORTE = form.GetField("CD_TRANSPORTE_V1").Value;

                    nuevoRegistroVehiculo.VL_MATRICULA_1 = matricula1;
                    nuevoRegistroVehiculo.VL_MATRICULA_2 = form.GetField("VL_MATRICULA_2_ENTRADA").Value?.ToUpper();

                    nuevoRegistroVehiculo.CD_EMPRESA = cdEmpresa;

                    if (!isPreRegistro)
                    {
                        nuevoRegistroVehiculo.DT_PORTERIA_ENTRADA = DateTime.Now;
                    }

                    nuevoRegistroVehiculo.VL_PESO_ENTRADA = vlPesoEntradaVe;
                    nuevoRegistroVehiculo.FL_SOLO_BALANZA = "N";
                    //nuevoRegistroVehiculo.ND_TP_FACTURACION = "TPFACCON"; //por defecto en CONTADO

                    nuevoRegistroVehiculo.CD_EMPRESA = cdEmpresa;

                    nuevoRegistroVehiculo.CD_AGENTE = cdAgente;
                    nuevoRegistroVehiculo.TP_AGENTE = agente == null ? null : agente.Tipo;

                    nuevoRegistroVehiculo.CD_SECTOR = cdSector;
                    nuevoRegistroVehiculo.ND_POTERIA_MOTIVO = motivo;
                    //nuevoRegistroVehiculo.ND_SECTOR = uow.DominioRepository.GetDominio(CDominio.CD_DOMINIO_PORTERIA_SECTOR, cdSector).Id;

                    nuevoRegistroVehiculo.NU_PREDIO = nuPredio;

                    uow.PorteriaRepository.AddRegistroVehiculo(nuevoRegistroVehiculo);
                }

                string estado = uow.DominioRepository.GetDominio(DominioPorteriaDb.CD_DOMINIO_PORTERIA_ESTADO, "ENT")?.Id;

                #endregion

                #region ***************** SET PERSONAS *****************

                if (listaPersonas != null) // Registro con personas
                {
                    listaPersonas.ForEach(nuPersona =>
                    {

                        PorteriaRegistroPersona nuevoRegistroPersona = uow.PorteriaRepository.GetPreRegistroPersona(nuPersona, nuevoRegistroVehiculo.NU_PORTERIA_VEHICULO);

                        if (nuevoRegistroPersona == null)
                        {
                            nuevoRegistroPersona = new PorteriaRegistroPersona();

                            nuevoRegistroPersona.NU_PORTERIA_VEHICULO_ENTRADA = nuevoRegistroVehiculo.NU_PORTERIA_VEHICULO;
                            nuevoRegistroPersona.NU_POTERIA_PERSONA = nuPersona;
                            nuevoRegistroPersona.DS_NOTA = dsNota;

                            if (uow.PorteriaRepository.PersonaTieneRegistroSalidaSinMarcar(nuevoRegistroPersona.NU_POTERIA_PERSONA))
                            {
                                throw new ValidationFailedException("WPOR010_Sec0_Error_Er013_NoRegistradaSalidaPersona", new string[] { nuevoRegistroPersona.NU_POTERIA_PERSONA.ToString() });
                            }

                            nuevoRegistroPersona.ND_ESTADO = estado ?? DominioPorteriaDb.CD_DOMINIO_PORTERIA_SECTOR_GENERAL;
                            nuevoRegistroPersona.ND_POTERIA_MOTIVO = motivo;
                            nuevoRegistroPersona.ND_SECTOR = cdSector;
                            nuevoRegistroPersona.CD_SECTOR = cdSector;
                            nuevoRegistroPersona.ND_TP_POTERIA_REGISTRO = uow.DominioRepository.GetDominio(DominioPorteriaDb.CD_DOMINIO_TP_PORTERIA_REGISTRON, cdTpRegistro)?.Id;

                            if (!isPreRegistro)
                            {
                                nuevoRegistroPersona.DT_PERSONA_ENTRADA = DateTime.Now;
                            }

                            nuevoRegistroPersona.CD_FUNCIONARIO = _identity.UserId;
                            nuevoRegistroPersona.CD_EMPRESA = cdEmpresa;
                            nuevoRegistroPersona.CD_AGENTE = agente?.Codigo;
                            nuevoRegistroPersona.TP_AGENTE = agente == null ? null : agente.Codigo;
                            nuevoRegistroPersona.NU_PREDIO = nuPredio;

                            uow.PorteriaRepository.AddRegistroPersona(nuevoRegistroPersona);
                        }
                        else
                        {
                            nuevoRegistroPersona.DT_PERSONA_ENTRADA = DateTime.Now;
                            uow.PorteriaRepository.UpdateRegistroPersona(nuevoRegistroPersona);
                        }
                    });
                }
                else  //Solo servicio para balanza (Sin personas)
                {
                    //siempre y cuando la persona agregada sea false
                    if (!personaAgregada)
                    {
                        PorteriaRegistroPersona nuevoRegistroBalanza = new PorteriaRegistroPersona();
                        nuevoRegistroBalanza.NU_PORTERIA_VEHICULO_ENTRADA = nuevoRegistroVehiculo.NU_PORTERIA_VEHICULO;
                        nuevoRegistroBalanza.CD_FUNCIONARIO = _identity.UserId;
                        nuevoRegistroBalanza.DS_NOTA = dsNota;

                        nuevoRegistroBalanza.ND_ESTADO = DominioPorteriaDb.CD_DOMINIO_PORTERIA_ESTADO_TRANSITO;
                        nuevoRegistroBalanza.ND_POTERIA_MOTIVO = motivo;
                        //nuevoRegistroBalanza.ND_SECTOR = CDominio.CD_DOMINIO_PORTERIA_SECTOR_BALANZA;
                        nuevoRegistroBalanza.ND_TP_POTERIA_REGISTRO = cdTpRegistro;

                        nuevoRegistroBalanza.CD_AGENTE = cdAgente;
                        nuevoRegistroBalanza.TP_AGENTE = agente == null ? null : agente.Tipo;
                        nuevoRegistroBalanza.CD_SECTOR = cdSector;

                        //Marco salida del veh�culo
                        nuevoRegistroVehiculo.DT_PORTERIA_SALIDA = DateTime.Now;
                        //nuevoRegistroVehiculo.FL_SOLO_BALANZA = "S";

                        uow.PorteriaRepository.AddRegistroPersona(nuevoRegistroBalanza);
                    }
                }

                #endregion

                #region ***************** SET AGENDAS *****************

                if (!nuevoRegistroVehiculo.VL_MATRICULA_1.IsNullOrEmpty())
                {
                    List<Agenda> agendas = uow.AgendaRepository.GetAgendasByMatriculaVehiculo(nuevoRegistroVehiculo.VL_MATRICULA_1);

                    if (agendas != null)
                    {
                        agendas.ForEach(agenda =>
                        {
                            if (!uow.PorteriaRepository.AnyVehiculoAgenda(nuevoRegistroVehiculo.NU_PORTERIA_VEHICULO, agenda.Id))
                            {

                                uow.PorteriaRepository.AddPorteriaVehiculoAgenda(new PorteriaVehiculoAgenda
                                {
                                    NU_AGENDA = agenda.Id,
                                    NU_PORTERIA_VEHICULO = nuevoRegistroVehiculo.NU_PORTERIA_VEHICULO,
                                });
                            }
                        });
                    }
                }

                #endregion

                #region ***************** SET OBJETOS *****************

                if (!nuevoRegistroVehiculo.VL_MATRICULA_1.IsNullOrEmpty())
                {
                    List<short> containers = null;

                    if (isPreRegistro)
                    {
                        containers = JsonConvert.DeserializeObject<List<short>>(query.GetParameter("SelectionGridContainers"));
                    }
                    else if (tienePreRegistro)
                    {
                        containers = uow.PorteriaRepository.GetContainersDeVehiculo(nuevoRegistroVehiculo.NU_PORTERIA_VEHICULO);
                    }
                    else
                    {
                        containers = uow.AgendaRepository.GetContainersAgendaParaEntradaByMatricula(nuevoRegistroVehiculo.VL_MATRICULA_1);
                    }

                    if (containers != null)
                    {
                        containers.ForEach(nuSeqConteiner =>
                        {
                            if (!isPreRegistro)
                            {
                                Container container = uow.PorteriaRepository.GetContainer(nuSeqConteiner);

                                if (container != null && container.FechaEntrada == null)
                                {
                                    container.FechaEntrada = DateTime.Now;
                                    container.NumeroTransaccion = uow.GetTransactionNumber();

                                    uow.PorteriaRepository.UpdateContainer(container);
                                }
                            }

                            if (!uow.PorteriaRepository.AnyVehiculoObjeto(nuSeqConteiner.ToString(), "CON", nuevoRegistroVehiculo.NU_PORTERIA_VEHICULO))
                            {

                                uow.PorteriaRepository.AddVehiculoObjeto(new PorteriaVehiculoObjeto
                                {
                                    CD_OBJETO = nuSeqConteiner.ToString(),
                                    TP_OBJETO = "CON",
                                    NU_PORTERIA_VEHICULO = nuevoRegistroVehiculo.NU_PORTERIA_VEHICULO,
                                });
                            }
                        });
                    }
                }

                #endregion

                uow.SaveChanges();
                uow.Commit();

                query.AddSuccessNotification("General_Db_Success_Insert");
            }
            else if (query.ButtonId == "btnConfirmarContacto")
            {
                List<int> listaPersonas = JsonConvert.DeserializeObject<List<int>>(query.GetParameter("SelectionGridPersonas"));

                FormField FieldNU_DOCUMENTO_SEARCH = form.GetField("NU_DOCUMENTO_SEARCH");
                FormField FieldNU_DOCUMENTO = form.GetField("NU_DOCUMENTO");
                FormField FieldNM_PERSONA = form.GetField("NM_PERSONA");
                FormField FieldAP_PERSONA = form.GetField("AP_PERSONA");
                FormField FieldNU_CELULAR = form.GetField("NU_CELULAR");

                PorteriaPersona obj = null;

                if (!FieldNU_DOCUMENTO_SEARCH.Value.IsNullOrEmpty())
                {
                    obj = uow.PorteriaRepository.GetPorteriaPersona(FieldNU_DOCUMENTO_SEARCH.Value.ToNumber<int>());
                    FieldNU_DOCUMENTO_SEARCH.Value = "";
                }
                else
                {
                    obj = uow.PorteriaRepository.GetPorteriaPersonaByDocumento(FieldNU_DOCUMENTO.Value);
                }

                if (obj != null && uow.PorteriaRepository.PersonaTieneRegistroSalidaSinMarcar(obj.NU_POTERIA_PERSONA))
                {
                    query.AddErrorNotification("WPOR010_Sec0_Error_Er013_NoRegistradaSalidaPersona", new List<string> { obj.NM_PERSONA + " " + obj.AP_PERSONA });
                }
                else
                {
                    if (obj == null)
                    {
                        uow.CreateTransactionNumber(query.ButtonId);

                        obj = new PorteriaPersona();

                        if (FieldNU_DOCUMENTO.Value.IsNullOrEmpty()) throw new ValidationFailedException("Ingrese decuento");
                        if (FieldNM_PERSONA.Value.IsNullOrEmpty()) throw new ValidationFailedException("Ingrese nombre");
                        if (FieldAP_PERSONA.Value.IsNullOrEmpty()) throw new ValidationFailedException("Ingrese apellido");
                        //if (FieldNU_CELULAR.Value.IsNullOrEmpty()) throw new WISException("Ingrese celular");

                        obj.NU_DOCUMENTO = FieldNU_DOCUMENTO.Value;
                        obj.NM_PERSONA = FieldNM_PERSONA.Value?.ToUpper();
                        obj.AP_PERSONA = FieldAP_PERSONA.Value?.ToUpper();
                        obj.NU_CELULAR = FieldNU_CELULAR.Value;
                        obj.ND_TP_DOCUMENTO = "PERTPDOCCDI";
                        obj.ND_TP_PERSONA = "TPPERFUN";
                        obj.CD_EMPRESA = form.GetField("CD_EMPRESA").Value.ToNumber<int?>();

                        //nueva_persona.CD_PAIS_EMISOR = paisMayus;

                        uow.PorteriaRepository.AddPersona(obj);

                        uow.SaveChanges();
                    }

                    if (!listaPersonas.Any(w => w == obj.NU_POTERIA_PERSONA))
                    {
                        listaPersonas.Add(obj.NU_POTERIA_PERSONA);
                    }
                }

                FieldNU_DOCUMENTO_SEARCH.Value = "";
                FieldNU_DOCUMENTO_SEARCH.Options = new List<SelectOption>();
                FieldNU_DOCUMENTO.Value = "";
                FieldNM_PERSONA.Value = "";
                FieldAP_PERSONA.Value = "";
                FieldNU_CELULAR.Value = "";

                query.AddParameter("SelectionGridPersonas", JsonConvert.SerializeObject(listaPersonas));
            }
            else if (query.ButtonId == "btnConfirmarContainer")
            {
                List<int> listaContainers = JsonConvert.DeserializeObject<List<int>>(query.GetParameter("SelectionGridContainers"));

                FormField FieldNU_CONTAINER = form.GetField("NU_CONTAINER");

                int nuContainer = FieldNU_CONTAINER.Value.ToNumber<int>();

                if (!listaContainers.Any(w => w == nuContainer))
                    listaContainers.Add(nuContainer);

                FieldNU_CONTAINER.Value = "";
                FieldNU_CONTAINER.Options = new List<SelectOption>();

                query.AddParameter("SelectionGridContainers", JsonConvert.SerializeObject(listaContainers));
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            string current = form.GetField("CURRENT").Value;

            using var uow = this._uowFactory.GetUnitOfWork();

            if (current == "tabDatosVehiculo")
                return this._formValidationService.Validate(new RegistroEntradaDatosVehiculoFormValidationModule(uow, this._identity.UserId, this._identity.Predio, context.IsSubmitting, this._identity.GetFormatProvider()), form, context);
            else if (current == "tabDatosGenerales")
                return this._formValidationService.Validate(new RegistroEntradaDatosGeneralesFormValidationModule(uow, this._identity.GetFormatProvider()), form, context);
            else if (current == "tabAsociarContactos")
                return this._formValidationService.Validate(new RegistroEntradaAsociarContactosFormValidationModule(uow), form, context);

            return form;
        }
        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext query)
        {
            switch (query.FieldId)
            {
                case "CD_CLIENTE": return this.SearchProvedor(query.SearchValue, form.GetField("CD_EMPRESA").Value);
                case "CD_EMPRESA": return this.SearchEmpresa(query.SearchValue);
                case "NU_DOCUMENTO_SEARCH": return this.SearchContacto(query.SearchValue);
                case "NU_CONTAINER": return this.SearchContainer(query.SearchValue);
            }

            return new List<SelectOption>();
        }

        public virtual List<SelectOption> SearchProvedor(string searchValue, string cdEmpresa)
        {
            if (cdEmpresa.IsNullOrEmpty()) throw new ValidationFailedException("Ingrese la empresa");

            using var uow = this._uowFactory.GetUnitOfWork();

            return uow.PorteriaRepository.GetAgenteByKeysPartialAndEmpresa(searchValue, cdEmpresa.ToNumber<int>())
                       .Select(w => new SelectOption(w.CodigoInterno, w.Descripcion))
                       .ToList();
        }
        public virtual List<SelectOption> SearchEmpresa(string searchValue)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return uow.EmpresaRepository.GetByNombreOrCodePartial(searchValue)
                    .Select(w => new SelectOption(w.Id.ToString(), w.Nombre))
                    .ToList();
        }
        public virtual List<SelectOption> SearchContacto(string searchValue)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return uow.PorteriaRepository.GetPersonaParaEntradaByKeysPartial(searchValue)
                    .Select(w => new SelectOption(w.NU_POTERIA_PERSONA.ToString(), w.NM_PERSONA + " " + w.AP_PERSONA))
                    .ToList();
        }
        public virtual List<SelectOption> SearchContainer(string searchValue)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return uow.PorteriaRepository.GetContainerParaPreRegistroByKeysPartial(searchValue)
                    .Select(w => new SelectOption(w.Id.ToString(), w.Numero))
                    .ToList();
        }
        public virtual List<SelectOption> SelectMotivo(IUnitOfWork uow)
        {
            return uow.PorteriaRepository.GetMotivos()
                   .Select(w => new SelectOption(w.Id, w.Descripcion))
                   .ToList();
        }
        public virtual List<SelectOption> SelectTipoTransporte(IUnitOfWork uow)
        {
            return uow.PorteriaRepository.GetTipoTransportes()
                   .Select(w => new SelectOption(w.Id, w.Descripcion))
                   .ToList();
        }
        public virtual List<SelectOption> SelectSector(IUnitOfWork uow, string predio)
        {
            return uow.PorteriaRepository.GetSectoresByPredio(predio)
                   .Select(w => new SelectOption(w.CD_SECTOR, w.DS_SECTOR))
                   .ToList();
        }
        public virtual List<SelectOption> SelectPredio(IUnitOfWork uow)
        {
            return uow.SecurityRepository.GetPrediosByUserLogin(_identity.UserId, _identity.Predio)
                   .Select(w => new SelectOption(w.Numero, w.Descripcion))
                   .ToList();
        }

        public virtual bool DentroIntervaloFacturacion(IUnitOfWork uow, DateTime horaEntrada, DateTime horaSalida)
        {
            string vlParmIntervalo = this._parameterService.GetValue("CS_COR_INTERV_TIEMPO_FACT_BAL");

            int intervaloMinutos = -1;
            if (vlParmIntervalo != String.Empty)
                intervaloMinutos = int.Parse(vlParmIntervalo);

            TimeSpan ventanaDescuento = horaSalida.Subtract(horaEntrada);

            return ventanaDescuento.TotalMinutes <= intervaloMinutos; //si esta dentro del intervalo facturo cor_30, de lo contario cor_29
        }
    }
}
