using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Porteria;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Porteria;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.Porteria;
using WIS.Domain.Recepcion;
using WIS.Extension;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Sorting;

namespace WIS.Application.Controllers.POR
{
    public class POR020DatosPorteria : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpeter;

        //protected List<string> Grid1Keys { get; }

        protected List<string> GridPersonasKeys { get; }
        protected List<string> GridContainersKeys { get; }

        public POR020DatosPorteria(
            IUnitOfWorkFactory uowFactory,
            IGridService service,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridPersonasKeys = new List<string>
            {
                "NU_PORTERIA_REGISTRO_PERSONA"
            };

            this.GridContainersKeys = new List<string>
            {
                "NU_SEQ_CONTAINER"
            };

            this._uowFactory = uowFactory;
            this._gridService = service;
            this._formValidationService = formValidationService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            switch (grid.Id)
            {
                //case "POR020_grid_1": return return this.Grid1FetchRows(service, grid, query.FetchQuery);
                case "POR020_grid_Personas":
                    {

                        grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton> {

                        new GridButton("btnBorrar", "General_Sec0_btn_Borrar", "fas fa-trash-alt"),

                    }));

                        return this.GridPersonasFetchRows(grid, query.FetchContext);
                    }
                case "POR020_grid_Containers":
                    {

                        grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton> {

                        new GridButton("btnBorrar", "General_Sec0_btn_Borrar", "fas fa-trash-alt"),

                    }));

                        return this.GridContainersFetchRows(grid, query.FetchContext);
                    }
            }

            return grid;
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            switch (grid.Id)
            {
                //case "POR020_grid_1": return this.Grid1FetchRows(service, grid, query);
                case "POR020_grid_Personas": return this.GridPersonasFetchRows(grid, query);
                case "POR020_grid_Containers": return this.GridContainersFetchRows(grid, query);
            }

            return grid;
        }
        public virtual Grid GridPersonasFetchRows(Grid grid, GridFetchContext query)
        {
            SortCommand defaultSort = new SortCommand("NU_POTERIA_PERSONA", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            string sPOR020_NU_PORTERIA_VEHICULO = query.GetParameter("NU_PORTERIA_VEHICULO");
            List<int> listaPersonas = JsonConvert.DeserializeObject<List<int>>(query.GetParameter("SelectionGridPersonas"));
            List<int> listaPersonasRemove = JsonConvert.DeserializeObject<List<int>>(query.GetParameter("notSelectionGridPersonas"));

            long? numPorVehiculo = null;
            if (!string.IsNullOrEmpty(sPOR020_NU_PORTERIA_VEHICULO) && sPOR020_NU_PORTERIA_VEHICULO != "-1")
            {
                numPorVehiculo = long.Parse(sPOR020_NU_PORTERIA_VEHICULO);
            }

            var dbQuery = new DatosPorteriaQuery(numPorVehiculo, listaPersonas, listaPersonasRemove);

            uow.HandleQuery(dbQuery, true);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, defaultSort, this.GridPersonasKeys);

            return grid;
        }
        public virtual Grid GridContainersFetchRows(Grid grid, GridFetchContext query)
        {
            SortCommand defaultSort = new SortCommand("NU_SEQ_CONTAINER", SortDirection.Descending);

            List<int> listaPersonas = JsonConvert.DeserializeObject<List<int>>(query.GetParameter("SelectionGridContainers"));

            using var uow = this._uowFactory.GetUnitOfWork();

            PorteriaContainersQuery dbQuery = null;

            dbQuery = new PorteriaContainersQuery(listaPersonas);

            uow.HandleQuery(dbQuery, true);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, defaultSort, this.GridContainersKeys);

            return grid;
        }
        public override GridButtonActionContext GridButtonAction(GridButtonActionContext data)
        {

            if (data.GridId == "POR020_grid_Personas")
            {

                ComponentParameter parameterPersonas = data.Parameters.FirstOrDefault(w => w.Id == "SelectionGridPersonas");
                ComponentParameter parameterPersonasRemove = data.Parameters.FirstOrDefault(w => w.Id == "notSelectionGridPersonas");

                List<int> listaPersonas = JsonConvert.DeserializeObject<List<int>>(parameterPersonas.Value);
                List<int> listaPersonasRemove = JsonConvert.DeserializeObject<List<int>>(parameterPersonasRemove.Value);

                int nuPersona = data.Row.GetCell("NU_POTERIA_PERSONA").Value.ToNumber<int>();

                if (listaPersonas.Any(w => w == nuPersona))
                {
                    listaPersonas.Remove(nuPersona);
                }
                else
                {
                    if (!listaPersonasRemove.Any(w => w == nuPersona))
                        listaPersonasRemove.Add(nuPersona);
                }

                parameterPersonas.Value = JsonConvert.SerializeObject(listaPersonas);
                parameterPersonasRemove.Value = JsonConvert.SerializeObject(listaPersonasRemove);

            }
            else if (data.GridId == "POR020_grid_Containers")
            {
                ComponentParameter parameterContainers = data.Parameters.FirstOrDefault(w => w.Id == "SelectionGridContainers");

                List<int> listaContainers = JsonConvert.DeserializeObject<List<int>>(parameterContainers.Value);

                listaContainers.Remove(data.Row.Id.ToNumber<int>());

                parameterContainers.Value = JsonConvert.SerializeObject(listaContainers);

            }

            return data;
        }
        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext selection)
        {
            switch (selection.GridId)
            {
                case "POR020_grid_Personas": return this.GridAgregarMenuItemAction(selection);
            }

            return selection;
        }
        public virtual GridMenuItemActionContext GridAgregarMenuItemAction(GridMenuItemActionContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber("GridAgregarMenuItemAction");
            uow.BeginTransaction();

            try
            {
                string sPOR020_NU_PORTERIA_VEHICULO = query.GetParameter("NU_PORTERIA_VEHICULO");

                int? numPorVehiculo = null;
                if (!string.IsNullOrEmpty(sPOR020_NU_PORTERIA_VEHICULO) && sPOR020_NU_PORTERIA_VEHICULO != "-1")
                {
                    numPorVehiculo = int.Parse(sPOR020_NU_PORTERIA_VEHICULO);
                }

                string FL_CON_VEHICULO = query.GetParameter("FL_CON_VEHICULO");
                bool FL_CON_CONTENEDORES = query.GetParameter("FL_CON_CONTENEDORES") == "S";

                PorteriaRegistroVehiculo currVehiculo = null;

                if (FL_CON_VEHICULO == "N")
                {
                    currVehiculo = new PorteriaRegistroVehiculo();
                    currVehiculo.ND_TRANSPORTE = DominioPorteriaDb.CD_DOMINIO_PORTERIA_TASPORTE_APIE;
                    currVehiculo.DT_UPDROW = DateTime.Now;
                    currVehiculo.DT_PORTERIA_SALIDA = DateTime.Now;
                    uow.PorteriaRepository.AddRegistroVehiculo(currVehiculo);
                    uow.SaveChanges();

                    numPorVehiculo = currVehiculo.NU_PORTERIA_VEHICULO;
                }

                List<int> listaPersonas = JsonConvert.DeserializeObject<List<int>>(query.GetParameter("SelectionGridPersonas"));
                List<int> listaPersonasRemove = JsonConvert.DeserializeObject<List<int>>(query.GetParameter("notSelectionGridPersonas"));

                DatosPorteriaQuery dbQuery = new DatosPorteriaQuery(numPorVehiculo, listaPersonas, listaPersonasRemove);
                uow.HandleQuery(dbQuery, true);

                List<string> keysRowSelected = dbQuery.GetKeysRowsSelected();

                keysRowSelected.ForEach(x =>
                {
                    PorteriaRegistroPersona enty = uow.PorteriaRepository.getRegistroPersonaByNum(int.Parse(x));
                    if (enty != null)
                    {
                        enty.NU_PORTERIA_VEHICULO_SALIDA = numPorVehiculo;
                        enty.DT_PERSONA_SALIDA = DateTime.Now;

                        DominioDetalle dom = uow.DominioRepository.GetDominio(DominioPorteriaDb.ESTADO_PORTERIA_SALIDA);
                        if (dom != null)
                        {
                            enty.ND_ESTADO = dom.Id;
                        }
                    }

                    uow.PorteriaRepository.UpdateRegistroPersona(enty);
                    uow.SaveChanges();
                });

                if (FL_CON_VEHICULO != "N")
                {
                    currVehiculo = uow.PorteriaRepository.GetRegistroVehiculoByNum(numPorVehiculo ?? 1);
                    currVehiculo.DT_PORTERIA_SALIDA = DateTime.Now;
                    uow.PorteriaRepository.UpdateRegistroVehiculo(currVehiculo);

                    List<Camion> currCamionPorteria = uow.CamionRepository.GetCamionesSinPorteriaByMatricula(currVehiculo.VL_MATRICULA_1);

                    currCamionPorteria.ForEach(w =>
                    {
                        PorteriaCamion porCamion = new PorteriaCamion();
                        porCamion.cdCamion = w.Id;
                        porCamion.nuPorteriaVehiculo = currVehiculo.NU_PORTERIA_VEHICULO;

                        uow.PorteriaCamionRepository.AddCamion(porCamion);
                    });

                    uow.SaveChanges();

                    if (FL_CON_CONTENEDORES)
                    {
                        List<short> containers = JsonConvert.DeserializeObject<List<short>>(query.GetParameter("SelectionGridContainers"));

                        containers?.ForEach(nuSeqContainer =>
                        {
                            Container container = uow.PorteriaRepository.GetContainer(nuSeqContainer);

                            if (container != null)
                            {
                                container.FechaSalida = DateTime.Now;
                                container.NumeroTransaccion = uow.GetTransactionNumber();

                                uow.PorteriaRepository.UpdateContainer(container);
                            }
                        });

                        uow.SaveChanges();
                    }
                }

                uow.Commit();
            }
            catch (Exception ex)
            {
                query.AddErrorNotification("POR020_frm1_Error_Error2");
            }

            query.AddSuccessNotification("POR020_frm1_Msg_Ok1");

            return query;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            switch (grid.Id)
            {
                //case "POR020_grid_1": return this.Grid1FetchRows(service, grid, query);
                case "POR020_grid_Personas":
                    {
                        string sPOR020_NU_PORTERIA_VEHICULO = query.GetParameter("NU_PORTERIA_VEHICULO");
                        List<int> listaPersonas = JsonConvert.DeserializeObject<List<int>>(query.GetParameter("SelectionGridPersonas"));
                        List<int> listaPersonasRemove = JsonConvert.DeserializeObject<List<int>>(query.GetParameter("notSelectionGridPersonas"));

                        long? numPorVehiculo = null;
                        if (!string.IsNullOrEmpty(sPOR020_NU_PORTERIA_VEHICULO) && sPOR020_NU_PORTERIA_VEHICULO != "-1")
                        {
                            numPorVehiculo = long.Parse(sPOR020_NU_PORTERIA_VEHICULO);
                        }

                        var dbQuery = new DatosPorteriaQuery(numPorVehiculo, listaPersonas, listaPersonasRemove);

                        uow.HandleQuery(dbQuery, true);
                        dbQuery.ApplyFilter(this._filterInterpeter, query.Filters);

                        return new GridStats
                        {
                            Count = dbQuery.GetCount()
                        };
                    }
                case "POR020_grid_Containers":
                    {
                        List<int> listaPersonas = JsonConvert.DeserializeObject<List<int>>(query.GetParameter("SelectionGridContainers"));


                        PorteriaContainersQuery dbQuery = null;

                        dbQuery = new PorteriaContainersQuery(listaPersonas);

                        uow.HandleQuery(dbQuery, true);
                        dbQuery.ApplyFilter(this._filterInterpeter, query.Filters);

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
            var selectionField = form.GetField("FL_CON_CONTENEDORES");

            selectionField.Options = new List<SelectOption>
            {
                new SelectOption("S", "POR020_frm1_opt_ConVehiculo"),
                new SelectOption("N", "REC020_frm1_opt_SinVehiculo")
            };

            selectionField.Value = "S";



            return form;
        }
        public override Form FormButtonAction(Form form, FormButtonActionContext query)
        {
            ComponentParameter parameterPersonas = query.Parameters.FirstOrDefault(w => w.Id == "SelectionGridPersonas");
            ComponentParameter parameterPersonasRemove = query.Parameters.FirstOrDefault(w => w.Id == "notSelectionGridPersonas");

            List<int> listaPersonas = JsonConvert.DeserializeObject<List<int>>(parameterPersonas.Value);
            List<int> listaPersonasRemove = JsonConvert.DeserializeObject<List<int>>(parameterPersonasRemove.Value);

            FormField fieldNU_DOCUMENTO_SEARCH = form.GetField("NU_DOCUMENTO_SEARCH");

            int nuPersona = fieldNU_DOCUMENTO_SEARCH.Value.ToNumber<int>();


            fieldNU_DOCUMENTO_SEARCH.Value = "";
            fieldNU_DOCUMENTO_SEARCH.Options = new List<SelectOption>();

            if (listaPersonasRemove.Any(w => w == nuPersona))
            {
                listaPersonasRemove.Remove(nuPersona);
            }
            else
            {
                if (!listaPersonas.Any(w => w == nuPersona))
                    listaPersonas.Add(nuPersona);
            }

            parameterPersonas.Value = JsonConvert.SerializeObject(listaPersonas);
            parameterPersonasRemove.Value = JsonConvert.SerializeObject(listaPersonasRemove);

            return form;
        }
        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new DatosPorteriaFormValidationModule(uow), form, context);
        }
        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext query)
        {
            switch (query.FieldId)
            {
                case "NU_DOCUMENTO_SEARCH": return this.SearchContacto(query.SearchValue);
            }

            return new List<SelectOption>();
        }

        public virtual List<SelectOption> SearchContacto(string searchValue)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return uow.PorteriaRepository.GetPersonaParaSalidaByKeysPartial(searchValue)
                    .Select(w => new SelectOption(w.NU_POTERIA_PERSONA.ToString(), w.NM_PERSONA + " " + w.AP_PERSONA))
                    .ToList();
        }



    }
}
