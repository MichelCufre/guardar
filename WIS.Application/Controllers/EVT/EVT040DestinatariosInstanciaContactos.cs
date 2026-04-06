using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Expedicion;
using WIS.Domain.Eventos;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.EVT
{
    public class EVT040DestinatariosInstanciaContactos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public EVT040DestinatariosInstanciaContactos(
            ISessionAccessor session,
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_CONTACTO",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_CONTACTO", SortDirection.Descending)
            };

            this._session = session;
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            if (grid.Id == "EVT040DestinatariosContactos_grid_1")
                grid.MenuItems.Add(new GridButton("btnAgregar", "EVT040_Sec0_btn_AgregarSeleccion"));
            else if (grid.Id == "EVT040DestinatariosContactos_grid_2")
                grid.MenuItems.Add(new GridButton("btnQuitar", "EVT040_Sec0_btn_QuitarSeleccion"));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int numeroInstancia = int.Parse(context.GetParameter("instancia"));

            var dbQuery = new ContactosInstanciasQuery(grid.Id == "EVT040DestinatariosContactos_grid_1" ? "AGREGAR" : "", numeroInstancia);

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            this.AdvertenciasModal(grid, uow, context, numeroInstancia);

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int numeroInstancia = int.Parse(context.GetParameter("instancia"));

            var dbQuery = new ContactosInstanciasQuery(grid.Id == "EVT040DestinatariosContactos_grid_1" ? "AGREGAR" : "", numeroInstancia);

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

            int numeroInstancia = int.Parse(context.GetParameter("instancia"));

            var dbQuery = new ContactosInstanciasQuery(grid.Id == "EVT040DestinatariosContactos_grid_1" ? "AGREGAR" : "", numeroInstancia);

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("EVT040 Modificar Contactos Instancia");
            var nuTransaccion = uow.GetTransactionNumber();

            try
            {
                if (int.TryParse(context.GetParameter("instancia"), out int NuInstancia))
                {

                    if (context.GridId == "EVT040DestinatariosContactos_grid_1")
                    {
                        List<int> contactos = this.GetSelectedContactos(uow, "AGREGAR", NuInstancia, context);

                        foreach (var contacto in contactos)
                        {
                            DestinatarioInstancia obj = new DestinatarioInstancia
                            {
                                Id = uow.DestinatarioRepository.GetNextNuDestinatarioInstancia(),
                                NumeroInstancia = NuInstancia,
                                NumeroContacto = contacto,
                                FechaAlta = DateTime.Now,
                                NumeroTransaccion = nuTransaccion,
                            };

                            uow.DestinatarioRepository.AddDestinatarioToInstancia(obj);
                        }
                    }
                    else
                    {
                        List<int> contactos = this.GetSelectedContactos(uow, "QUITAR", NuInstancia, context);

                        foreach (var contacto in contactos)
                        {
                            var destinatarioInstancia = uow.DestinatarioRepository.GetDestinatarioInstancia(contacto, NuInstancia);

                            destinatarioInstancia.FechaModificacion = DateTime.Now;
                            destinatarioInstancia.NumeroTransaccion = nuTransaccion;
                            destinatarioInstancia.NumeroTransaccionDelete = nuTransaccion;

                            uow.DestinatarioRepository.UpdateDestinatarioInstancia(destinatarioInstancia);
                            uow.SaveChanges();

                            uow.DestinatarioRepository.RemoveContactoOfInstancia(destinatarioInstancia);
                        }
                    }

                    uow.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                uow.Rollback();
            }

            return context;
        }

        #region Metodos Auxiliares

        public virtual List<int> GetSelectedContactos(IUnitOfWork uow, string tipo, int nroInstancia, GridMenuItemActionContext context)
        {
            var keys = new List<int>();

            foreach (var key in context.Selection.Keys)
            {
                keys.Add(int.Parse(key));
            }

            var dbQuery = new ContactosInstanciasQuery(tipo, nroInstancia);

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(keys);

            return keys;
        }

        public virtual void AdvertenciasModal(Grid grid, IUnitOfWork uow, GridFetchContext context, int numeroInstancia)
        {
            if (grid.Id == "EVT040DestinatariosContactos_grid_2")
            {
                var parametrosInstancia = uow.EventoRepository.GetParametrosInstancia(numeroInstancia);

                int.TryParse(parametrosInstancia.Find(x => x.Codigo == EventoParametroDb.CD_EMPRESA)?.Valor, out int parametroEmpresa);
                var parametroTipoAgente = parametrosInstancia.Find(x => x.Codigo == EventoParametroDb.TP_AGENTE)?.Valor;
                var parametroCodigoAgente = parametrosInstancia.Find(x => x.Codigo == EventoParametroDb.CD_AGENTE)?.Valor;

                var diferenteEmpresa = false;
                var diferenteTipoAgente = false;
                var diferenteCodigoAgente = false;

                foreach (var row in grid.Rows)
                {
                    var numeroContacto = int.Parse(row.GetCell("NU_CONTACTO").Value);
                    var contacto = uow.DestinatarioRepository.GetContacto(numeroContacto);

                    if (parametroEmpresa != 0 && contacto.CodigoEmpresa != parametroEmpresa)
                    {
                        diferenteEmpresa = true;
                    }

                    if (contacto.CodigoCliente != null)
                    {
                        if (parametroCodigoAgente != null)
                        {
                            if (contacto.CodigoCliente != parametroCodigoAgente)
                                diferenteCodigoAgente = true;
                        }

                        if (parametroTipoAgente != null)
                        {
                            var tipoAgente = uow.AgenteRepository.GetTipoAgente(contacto.CodigoCliente);

                            if (tipoAgente != parametroTipoAgente)
                                diferenteTipoAgente = true;
                        }
                    }
                }

                if (diferenteEmpresa || diferenteCodigoAgente || diferenteTipoAgente)
                {
                    var mensaje = "";
                    var key = (diferenteEmpresa, diferenteCodigoAgente, diferenteTipoAgente);

                    switch (key)
                    {
                        case (true, false, false):
                            mensaje = "EVT040_Sec0_msg_DiferenciaEmpresa";
                            break;
                        case (false, true, false):
                            mensaje = "EVT040_Sec0_msg_DiferenciaCodigoAgente";
                            break;
                        case (false, false, true):
                            mensaje = "EVT040_Sec0_msg_DiferenciaTipoAgente";
                            break;
                        case (true, true, false):
                            mensaje = "EVT040_Sec0_msg_DiferenciaEmpresaCodAgente";
                            break;
                        case (true, false, true):
                            mensaje = "EVT040_Sec0_msg_DiferenciaEmpresaTipoAgente";
                            break;
                        case (false, true, true):
                            mensaje = "EVT040_Sec0_msg_DiferenciaCodTipAgente";
                            break;
                        case (true, true, true):
                            mensaje = "EVT040_Sec0_msg_DiferenciasTotal";
                            break;
                    }

                    context.Parameters.Add(new ComponentParameter { Id = "mensaje", Value = "EVT040_Sec0_msg_DiferenciasTotal" });
                }
            }
        }

        #endregion
    }
}
