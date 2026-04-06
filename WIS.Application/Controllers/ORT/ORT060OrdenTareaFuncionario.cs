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
    public class ORT060OrdenTareaFuncionario : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<ORT060OrdenTareaFuncionario> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public ORT060OrdenTareaFuncionario(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            IIdentityService identity,
            ISecurityService security,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter,
            ILogger<ORT060OrdenTareaFuncionario> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "NU_ORT_ORDEN_TAREA_FUNC"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_ORT_ORDEN_TAREA_FUNC", SortDirection.Descending),
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


        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var numeroOrden = query.Parameters.FirstOrDefault(s => s.Id == "numeroOrden").Value;
            var resuelta = query.Parameters.FirstOrDefault(s => s.Id == "resuelta").Value;
            var cdTarea = query.Parameters.FirstOrDefault(s => s.Id == "codigoTarea").Value;

            var orden = uow.OrdenRepository.GetOrden(int.Parse(numeroOrden));
            var tarea = uow.TareaRepository.GetTarea(cdTarea);

            //Validaciones para activar el modo solo lectura
            if (orden.Estado == OrdenTareaDb.ESTADO_ORDEN_CERRADA || resuelta == "S" || tarea.RegistroHoras == "N")
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
                "CD_FUNCIONARIO",
                "DT_DESDE",
                "DT_HASTA",
                "DS_MEMO",
            });

            grid.AddOrUpdateColumn(new GridColumnSelect("CD_FUNCIONARIO", this.SelectCodigoFuncionario(uow)));

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            OrdenTareaFuncionarioQuery dbQuery = new OrdenTareaFuncionarioQuery();
            if (query.Parameters.Count > 0)
            {
                long nuOrdenTarea = query.Parameters.Any(s => s.Id == "numeroOrdenTarea") ? long.Parse(query.Parameters.FirstOrDefault(s => s.Id == "numeroOrdenTarea").Value) : -1;

                dbQuery = new OrdenTareaFuncionarioQuery(nuOrdenTarea);
            }
            else
            {
                dbQuery = new OrdenTareaFuncionarioQuery();
            }

            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            foreach (var row in grid.Rows)
            {
                if (!string.IsNullOrEmpty(row.GetCell("DT_HASTA").Value))
                {
                    grid.SetEditableCells(new List<string> {
                        "DT_DESDE",
                        "DT_HASTA",
                        "DS_MEMO"
                    });
                }
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            OrdenTareaFuncionarioQuery dbQuery = new OrdenTareaFuncionarioQuery();
            if (query.Parameters.Count > 0)
            {
                long nuOrdenTarea = query.Parameters.Any(s => s.Id == "numeroOrdenTarea") ? long.Parse(query.Parameters.FirstOrDefault(s => s.Id == "numeroOrdenTarea").Value) : -1;

                dbQuery = new OrdenTareaFuncionarioQuery(nuOrdenTarea);
            }
            else
            {
                dbQuery = new OrdenTareaFuncionarioQuery();
            }

            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            OrdenTareaFuncionarioQuery dbQuery = new OrdenTareaFuncionarioQuery();
            if (query.Parameters.Count > 0)
            {
                long nuOrdenTarea = query.Parameters.Any(s => s.Id == "numeroOrdenTarea") ? long.Parse(query.Parameters.FirstOrDefault(s => s.Id == "numeroOrdenTarea").Value) : -1;

                dbQuery = new OrdenTareaFuncionarioQuery(nuOrdenTarea);
            }
            else
            {
                dbQuery = new OrdenTareaFuncionarioQuery();
            }

            using var uow = this._uowFactory.GetUnitOfWork();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new OrdenTareaFuncionarioValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
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
                    RegistroOrdenTareaFuncionario registroOrdenTareaFuncionario = new RegistroOrdenTareaFuncionario(uow, this._identity.UserId, this._identity.Application);

                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    foreach (var row in grid.Rows)
                    {
                        if (row.IsNew)
                        {
                            OrdenTareaFuncionario ordenTareaFuncionario = CrearOrdenTareaFuncionario(uow, row, query);
                            registroOrdenTareaFuncionario.RegistrarOrdenTareaFuncionario(ordenTareaFuncionario);
                        }
                        else if (row.IsDeleted)
                        {
                            DeleteOrdenTareaFuncionario(uow, row, query);
                        }
                        else
                        {
                            // rows editadas
                            OrdenTareaFuncionario ordenTareaFuncionario = ModificarOrdenTareaFuncionario(uow, row, query);
                            registroOrdenTareaFuncionario.UpdateOrdenTareaFuncionario(ordenTareaFuncionario);
                        }
                    }
                }

                uow.SaveChanges();

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ValidationFailedException ex)
            {
                this._logger.LogError(ex, "ORT060GridCommit");
                query.AddErrorNotification(ex.Message);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "ORT060GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }

        #region Metodos Auxiliares


        public virtual OrdenTareaFuncionario CrearOrdenTareaFuncionario(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            string numeroOrden = query.Parameters.FirstOrDefault(s => s.Id == "numeroOrden").Value;
            string codigoTarea = query.Parameters.FirstOrDefault(s => s.Id == "codigoTarea").Value;
            string codigoEmpresa = query.Parameters.FirstOrDefault(s => s.Id == "codigoEmpresa").Value;
            string numeroOrdenTarea = query.Parameters.FirstOrDefault(s => s.Id == "numeroOrdenTarea").Value;

            DateTime FechaInicio = DateTime.Parse(row.GetCell("DT_DESDE").Value, this._identity.GetFormatProvider());
            DateTime? FechaFin = null;

            if (row.GetCell("DT_HASTA").Value != "")
                FechaFin = DateTime.Parse(row.GetCell("DT_HASTA").Value, this._identity.GetFormatProvider());

            OrdenTareaFuncionario nuevaOrdenTareaFuncionario = new OrdenTareaFuncionario();
            nuevaOrdenTareaFuncionario.CodigoFuncionario = int.Parse(row.GetCell("CD_FUNCIONARIO").Value);
            nuevaOrdenTareaFuncionario.FechaDesde = FechaInicio;
            nuevaOrdenTareaFuncionario.FechaHasta = FechaFin;
            nuevaOrdenTareaFuncionario.DescripcionMemo = row.GetCell("DS_MEMO").Value;
            nuevaOrdenTareaFuncionario.NumeroOrdenTarea = long.Parse(numeroOrdenTarea);

            return nuevaOrdenTareaFuncionario;
        }

        public virtual OrdenTareaFuncionario ModificarOrdenTareaFuncionario(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            var nuOrdenTareaFuncionario = long.Parse(row.GetCell("NU_ORT_ORDEN_TAREA_FUNC").Value);
            var orden = uow.TareaRepository.GetOrdenTareaFuncionario(nuOrdenTareaFuncionario);

            if (!string.IsNullOrEmpty(row.GetCell("DT_HASTA").Value))
            {
                orden.FechaHasta = DateTime.Parse(row.GetCell("DT_HASTA").Value, this._identity.GetFormatProvider());
            }
            else
            {
                orden.FechaHasta = null;
            }

            orden.DescripcionMemo = row.GetCell("DS_MEMO").Value;

            return orden;
        }

        public virtual void DeleteOrdenTareaFuncionario(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            var nuOrdenTareaFuncionario = long.Parse(row.GetCell("NU_ORT_ORDEN_TAREA_FUNC").Value);
            var ordenTareaFuncionario = uow.TareaRepository.GetOrdenTareaFuncionario(nuOrdenTareaFuncionario);

            if (ordenTareaFuncionario != null)
                uow.TareaRepository.DeleteOrdenTareaFuncionario(ordenTareaFuncionario);
            else
                throw new EntityNotFoundException("General_ORT060_Error_RegistroNoExiste");
        }

        public virtual List<SelectOption> SelectCodigoFuncionario(IUnitOfWork uow)
        {
            return uow.FuncionarioRepository.GetFuncionarios()
                .Select(w => new SelectOption(w.Id.ToString(), w.Id + "  -  " + w.Nombre))
                .ToList();
        }

        #endregion
    }
}
