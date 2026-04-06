using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Facturacion;
using WIS.Domain.OrdenTarea;
using WIS.Domain.OrdenTarea.Constants;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.ORT
{
    public class ORT070OrdenTareaManipuleoInsumo : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<ORT070OrdenTareaManipuleoInsumo> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public ORT070OrdenTareaManipuleoInsumo(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            IIdentityService identity,
            ISecurityService security,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter,
            ILogger<ORT070OrdenTareaManipuleoInsumo> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "NU_ORT_ORDEN_TAREA_DATO"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_ORT_ORDEN_TAREA_DATO", SortDirection.Descending),
            };

            this._uowFactory = uowFactory;
            this._session = session;
            this._identity = identity;
            this._security = security;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._formValidationService = formValidationService;
            _filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;

        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            OrdenTareaManipuleoInsumoQuery dbQuery = new OrdenTareaManipuleoInsumoQuery();
            if (query.Parameters.Count > 0)
            {
                long nuOrdenTarea = query.Parameters.Any(s => s.Id == "numeroOrdenTarea") ? long.Parse(query.Parameters.FirstOrDefault(s => s.Id == "numeroOrdenTarea").Value) : -1;

                dbQuery = new OrdenTareaManipuleoInsumoQuery(nuOrdenTarea);
            }
            else
            {
                dbQuery = new OrdenTareaManipuleoInsumoQuery();
            }

            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {

            using var uow = this._uowFactory.GetUnitOfWork();

            var nuOrden = query.Parameters.FirstOrDefault(s => s.Id == "numeroOrden").Value;
            var resuelta = query.Parameters.FirstOrDefault(s => s.Id == "resuelta").Value;
            var cdTarea = query.Parameters.FirstOrDefault(s => s.Id == "codigoTarea").Value;
            var cdEmpresa = int.Parse(query.Parameters.FirstOrDefault(s => s.Id == "codigoEmpresa").Value);

            var tarea = uow.TareaRepository.GetTarea(cdTarea);
            var orden = uow.OrdenRepository.GetOrden(int.Parse(nuOrden));

            //Validaciones para activar el modo solo lectura
            if (orden.Estado == OrdenTareaDb.ESTADO_ORDEN_CERRADA || resuelta == "S" || (tarea.RegistroManipuleo == "N" && tarea.RegistroInsumos == "N"))
            {
                //Display modoLectura
                query.AddParameter("ModoLectura", "S");
                query.IsAddEnabled = false;
                query.IsEditingEnabled = false;
                query.IsRemoveEnabled = false;
            }
            else
            {
                query.IsAddEnabled = true;
                query.IsEditingEnabled = true;
                query.IsRemoveEnabled = true;
            }

            grid.SetInsertableColumns(new List<string> {
                "CD_INSUMO_MANIPULEO",
                "QT_INSUMO_MANIPULEO",
            });


            if (tarea.RegistroManipuleo == "N")
                grid.AddOrUpdateColumn(new GridColumnSelect("CD_INSUMO_MANIPULEO", this.SelectInsumos(uow, cdEmpresa)));
            else if (tarea.RegistroInsumos == "N")
                grid.AddOrUpdateColumn(new GridColumnSelect("CD_INSUMO_MANIPULEO", this.SelectManipuleos(uow, cdEmpresa)));
            else
                grid.AddOrUpdateColumn(new GridColumnSelect("CD_INSUMO_MANIPULEO", this.SelectInsumosManipuleos(uow, cdEmpresa)));

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            OrdenTareaManipuleoInsumoQuery dbQuery = new OrdenTareaManipuleoInsumoQuery();
            if (query.Parameters.Count > 0)
            {
                long nuOrdenTarea = query.Parameters.Any(s => s.Id == "numeroOrdenTarea") ? long.Parse(query.Parameters.FirstOrDefault(s => s.Id == "numeroOrdenTarea").Value) : -1;

                dbQuery = new OrdenTareaManipuleoInsumoQuery(nuOrdenTarea);
            }
            else
            {
                dbQuery = new OrdenTareaManipuleoInsumoQuery();
            }

            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string> {
                "QT_INSUMO_MANIPULEO",
            });

            var cdTarea = query.Parameters.FirstOrDefault(s => s.Id == "codigoTarea").Value;
            var tarea = uow.TareaRepository.GetTarea(cdTarea);

            foreach (var row in grid.Rows)
            {
                var cellQtInsumoManipuleo = row.GetCell("QT_INSUMO_MANIPULEO");
                var tipoInsumoManipuleo = uow.InsumoManipuleoRepository.GetInsumoManipuleo(row.GetCell("CD_INSUMO_MANIPULEO").Value).Tipo;
                if ((tarea.RegistroManipuleo == "N" && tipoInsumoManipuleo == OrdenTareaDb.TipoManipuleo) || (tarea.RegistroInsumos == "N" && tipoInsumoManipuleo == OrdenTareaDb.TipoInsumo))
                    cellQtInsumoManipuleo.Editable = false;
            }

            return grid;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                var numeroOrden = query.Parameters.FirstOrDefault(s => s.Id == "numeroOrden").Value;
                var numeroOrdenTarea = long.Parse(query.Parameters.FirstOrDefault(s => s.Id == "numeroOrdenTarea").Value);
                var orden = uow.OrdenRepository.GetOrden(int.Parse(numeroOrden));
                var ordenTarea = uow.TareaRepository.GetOrdenTarea(numeroOrdenTarea);

                if (orden.Estado == OrdenTareaDb.ESTADO_ORDEN_CERRADA || ordenTarea.Resuelta == "S")
                {
                    throw new ValidationFailedException("General_Sec0_Error_Error_EstadoDeOrdenOTareaNoPermiteIngresarModificar");
                }

                if (grid.Rows.Any())
                {
                    RegistroOrdenTareaManipuleoInsumo registroOrdenTareaManipuleoInsumo = new RegistroOrdenTareaManipuleoInsumo(uow, this._identity.UserId, this._identity.Application);

                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    var cdTarea = query.Parameters.FirstOrDefault(s => s.Id == "codigoTarea").Value;
                    var tarea = uow.TareaRepository.GetTarea(cdTarea);

                    foreach (var row in grid.Rows)
                    {
                        var tipoInsumoManipuleo = uow.InsumoManipuleoRepository.GetInsumoManipuleo(row.GetCell("CD_INSUMO_MANIPULEO").Value).Tipo;

                        if ((tarea.RegistroManipuleo == "N" && tipoInsumoManipuleo == OrdenTareaDb.TipoManipuleo) || (tarea.RegistroInsumos == "N" && tipoInsumoManipuleo == OrdenTareaDb.TipoInsumo))
                        {
                            if (tipoInsumoManipuleo == OrdenTareaDb.TipoManipuleo)
                                throw new ValidationFailedException("ORT070_Error_Error_LasConfiguracionDeTareaNoPermiteInsumo");
                            else
                                throw new ValidationFailedException("ORT070_Error_Error_LasConfiguracionDeTareaNoPermiteManipuleo");
                        }

                        if (row.IsNew)
                        {
                            OrdenTareaManipuleoInsumo ordenTareaManipuleoInsumo = this.CrearOrdenTareaManipuleoInsumo(uow, row, query);
                            registroOrdenTareaManipuleoInsumo.RegistrarOrdenTareaManipuleoInsumo(ordenTareaManipuleoInsumo);
                        }
                        else if (row.IsDeleted)
                        {
                            DeleteOrdenTareaManipuleoInsumo(uow, row, query);
                        }
                        else
                        {
                            OrdenTareaManipuleoInsumo ordenTareaManipuleoInsumo = this.ModificarTareaManipuleoInsumo(uow, row, query);
                            registroOrdenTareaManipuleoInsumo.UpdateOrdenTareaManipuleoInsumo(ordenTareaManipuleoInsumo);
                        }

                    }

                }

                uow.SaveChanges();

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ValidationFailedException ex)
            {
                this._logger.LogError(ex, "ORT070GridCommit");
                query.AddErrorNotification(ex.Message, new List<string>(ex.StrArguments ?? new string[0]));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "FAC249GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new OrdenTareaManipuleoInsumoValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            OrdenTareaManipuleoInsumoQuery dbQuery = new OrdenTareaManipuleoInsumoQuery();
            if (query.Parameters.Count > 0)
            {
                long nuOrdenTarea = query.Parameters.Any(s => s.Id == "numeroOrdenTarea") ? long.Parse(query.Parameters.FirstOrDefault(s => s.Id == "numeroOrdenTarea").Value) : -1;

                dbQuery = new OrdenTareaManipuleoInsumoQuery(nuOrdenTarea);
            }
            else
            {
                dbQuery = new OrdenTareaManipuleoInsumoQuery();
            }

            using var uow = this._uowFactory.GetUnitOfWork();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        #region Metodos Auxiliares

        public virtual void DeleteOrdenTareaManipuleoInsumo(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            var nuOrdenTareaDato = long.Parse(row.GetCell("NU_ORT_ORDEN_TAREA_DATO").Value);
            var tareaDato = uow.TareaRepository.GetManipuleoInsumo(nuOrdenTareaDato);
            if (tareaDato != null)
                uow.TareaRepository.DeleteOrdenTareaDato(tareaDato);
            else
                throw new EntityNotFoundException("General_ORT070_Error_RegistroNoExiste");
        }

        public virtual OrdenTareaManipuleoInsumo CrearOrdenTareaManipuleoInsumo(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            string codigoTarea = query.Parameters.FirstOrDefault(s => s.Id == "numeroOrdenTarea").Value;

            OrdenTareaManipuleoInsumo nuevaOrdenInsumo = new OrdenTareaManipuleoInsumo();

            nuevaOrdenInsumo.Cantidad = decimal.Parse(row.GetCell("QT_INSUMO_MANIPULEO").Value, this._identity.GetFormatProvider());
            nuevaOrdenInsumo.CodigoInsumoManipuleo = row.GetCell("CD_INSUMO_MANIPULEO").Value;
            nuevaOrdenInsumo.NumeroOrdenTarea = long.Parse(codigoTarea);

            return nuevaOrdenInsumo;
        }

        public virtual OrdenTareaManipuleoInsumo ModificarTareaManipuleoInsumo(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            var numeroOrdenTareaDato = long.Parse(row.GetCell("NU_ORT_ORDEN_TAREA_DATO").Value);

            OrdenTareaManipuleoInsumo tareaManipuleoInsumo = uow.TareaRepository.GetManipuleoInsumo(numeroOrdenTareaDato);

            tareaManipuleoInsumo.Cantidad = decimal.Parse(row.GetCell("QT_INSUMO_MANIPULEO").Value, this._identity.GetFormatProvider());

            return tareaManipuleoInsumo;
        }

        public virtual List<SelectOption> SelectInsumosManipuleos(IUnitOfWork uow, int cdEmpresa)
        {
            return uow.InsumoManipuleoRepository.GetInsumosManipuleos(cdEmpresa)
                .Select(w => new SelectOption(w.Id, w.Id + "  -  " + w.Descripcion))
                .ToList();
        }

        public virtual List<SelectOption> SelectInsumos(IUnitOfWork uow, int cdEmpresa)
        {
            return uow.InsumoManipuleoRepository.GetInsumos(cdEmpresa)
                .Select(w => new SelectOption(w.Id, w.Id + "  -  " + w.Descripcion))
                .ToList();
        }

        public virtual List<SelectOption> SelectManipuleos(IUnitOfWork uow, int cdEmpresa)
        {
            return uow.InsumoManipuleoRepository.GetManipuleos(cdEmpresa)
                .Select(w => new SelectOption(w.Id, w.Id + "  -  " + w.Descripcion))
                .ToList();
        }

        #endregion

    }
}
