using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Security;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Build.Configuration;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Sorting;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Excel;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Domain.General;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General.Enums;
using WIS.Components.Common.Select;
using WIS.Exceptions;
using WIS.Application.Validation;
using WIS.Filtering;

namespace WIS.Application.Controllers.REG
{
    public class REG130PanelRutasEntrega : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }

        public REG130PanelRutasEntrega(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService, IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "CD_ROTA"
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsAddEnabled = true;
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = true;

            grid.SetInsertableColumns(new List<string>
            {
                "CD_ROTA",
                "DS_ROTA",
                "CD_PORTA",
                "CD_ONDA",
                "CD_TRANSPORTADORA"
            });

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new RutasDeEntregaQuery();

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("DT_ALTERACAO", SortDirection.Descending);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, defaultSort, this.GridKeys);

            foreach (var row in grid.Rows)
            {
                row.SetEditableCells(new List<string>
                {
                    "DS_ROTA",
                    "CD_PORTA",
                    "CD_ONDA",
                    "CD_TRANSPORTADORA",
                    "ACTIVO"
                });
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new RutasDeEntregaQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new RutasDeEntregaQuery();

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("CD_ROTA", SortDirection.Ascending);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, defaultSort);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (grid.Rows.Any())
            {

                if (grid.HasNewDuplicates(this.GridKeys))
                    throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                if (grid.HasDuplicates(new List<string>() { "DS_ROTA" }))
                    throw new ValidationFailedException("REG130_Grid_Error_DescripcionRutaDuplicada");

                RutaMapper rutaMapper = new RutaMapper();

                foreach (var row in grid.Rows)
                {
                    if (row.IsNew)
                    {
                        this.CrearRutaDeEntrega(uow, row);
                    }
                    else if (row.IsDeleted)
                    {
                        throw new ValidationFailedException("General_Sec0_msg_DeleteNotImplemented");
                    }
                    else
                    {
                        // rows editadas
                        this.EditarRutaDeEntrega(uow, rutaMapper, row);

                    }
                }
            }

            uow.SaveChanges();

            query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoRutasDeEntregaValidationModule(uow), grid, row, context);
        }

        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext query)
        {
            switch (query.ColumnId)
            {
                case "CD_PORTA":
                    return this.SearchPuerta(grid, row, query);
                case "CD_ONDA":
                    return this.SearchOnda(grid, row, query);
                case "CD_TRANSPORTADORA":
                    return this.SearchTransportista(grid, row, query);
            }

            return new List<SelectOption>();
        }

        #region Metodos Auxiliares

        public virtual Ruta CrearRutaDeEntrega(IUnitOfWork uow, GridRow row)
        {
            var id = row.GetCell("CD_ROTA").Value;
            var descripcion = row.GetCell("DS_ROTA").Value;

            Ruta ruta = new Ruta()
            {
                Id = short.Parse(id),
                Descripcion = descripcion,
                ControlaOrdenDeCarga = false,
                Estado = EstadoRutaDeEntrega.Activo
            };

            if (short.TryParse(row.GetCell("CD_ONDA").Value, out short idOnda))
                ruta.Onda = uow.OndaRepository.GetOnda(idOnda);

            if (short.TryParse(row.GetCell("CD_PORTA").Value, out short codigoPuerta))
                ruta.PuertaEmbarque = uow.PuertaEmbarqueRepository.GetPuertaEmbarque(codigoPuerta);

            if (int.TryParse(row.GetCell("CD_TRANSPORTADORA").Value, out int idTransportadora))
                ruta.Transportista = idTransportadora;

            uow.RutaRepository.AddRuta(ruta);

            return ruta;
        }
        
        public virtual Ruta EditarRutaDeEntrega(IUnitOfWork uow, RutaMapper rutaMapper, GridRow row)
        {
            Ruta ruta = null;

            if (short.TryParse(row.GetCell("CD_ROTA").Value, out short idRuta))
                ruta = uow.RutaRepository.GetRuta(idRuta);

            if (ruta == null)
                throw new EntityNotFoundException("REG130_Grid_Error_RutaNoExiste", new string[] { row.GetCell("CD_ROTA").Value });

            ruta.Descripcion = row.GetCell("DS_ROTA").Value;

            if (short.TryParse(row.GetCell("CD_ONDA").Value, out short idOnda))
                ruta.Onda = uow.OndaRepository.GetOnda(idOnda);

            if (int.TryParse(row.GetCell("CD_TRANSPORTADORA").Value, out int idTransportadora))
                ruta.Transportista = idTransportadora;

            if (short.TryParse(row.GetCell("CD_PORTA").Value, out short codigoPuerta))
                ruta.PuertaEmbarque = uow.PuertaEmbarqueRepository.GetPuertaEmbarque(codigoPuerta);


            bool activo = rutaMapper.MapStringToBoolean(row.GetCell("ACTIVO").Value);
            if (rutaMapper.MapEstadoBooleanToEnum(activo) != ruta.Estado)
            {
                if (activo)
                    ruta.Enable();
                else
                    ruta.Disable();
            }


            uow.RutaRepository.UpdateRuta(ruta);

            return ruta;
        }

        public virtual List<SelectOption> SearchPuerta(Grid grid, GridRow row, GridSelectSearchContext query)
        {
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();
                        
            var puertas = uow.PuertaEmbarqueRepository.GetByDescripcionOrCodePartial(query.SearchValue, _identity.Predio);

            foreach (var puerta in puertas)
            {
                opciones.Add(new SelectOption(puerta.Id.ToString(), $"{puerta.Id} - {puerta.Descripcion}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchOnda(Grid grid, GridRow row, GridSelectSearchContext query)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            if (short.TryParse(row.GetCell("CD_PORTA").Value, out short cdPuerta))
            {
                using var uow = this._uowFactory.GetUnitOfWork();

                var puerta = uow.PuertaEmbarqueRepository.GetPuertaEmbarque(cdPuerta);

                if (puerta != null && !string.IsNullOrEmpty(puerta.NumPredio))
                {
                    var ondas = uow.OndaRepository.GetByDescripcionOrCodePartial(query.SearchValue, puerta.NumPredio);

                    foreach (var onda in ondas)
                    {
                        opciones.Add(new SelectOption(onda.Id.ToString(), $"{onda.Id} - {onda.Descripcion}"));
                    }
                }
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchTransportista(Grid grid, GridRow row, GridSelectSearchContext query)
        {
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var transportistas = uow.TransportistaRepository.GetByDescripcionOrCodePartial(query.SearchValue);

            foreach (var transporte in transportistas)
            {
                opciones.Add(new SelectOption(transporte.Id.ToString(), $"{transporte.Id} - {transporte.Descripcion}"));
            }

            return opciones;
        }

        #endregion
    }
}