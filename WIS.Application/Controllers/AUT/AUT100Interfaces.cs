using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common.Select;
using WIS.Domain.Automatismo;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Automatizacion;
using WIS.Domain.Integracion;
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
using WIS.Sorting;

namespace WIS.Application.Controllers.AUT
{
    public class AUT100Interfaces : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public AUT100Interfaces(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            IGridValidationService gridValidationService)
        {
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._gridValidationService = gridValidationService;

            this.GridKeys = new List<string>
            {
                "NU_AUTOMATISMO_INTERFAZ"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_AUTOMATISMO_INTERFAZ", SortDirection.Descending)
            };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            var nuAutomatismo = context.GetParameter("AUT100_NU_AUTOMATISMO");

            if (string.IsNullOrEmpty(nuAutomatismo) || !int.TryParse(nuAutomatismo, out int numeroAutomatismo))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            context.IsEditingEnabled = true;
            context.IsAddEnabled = false;
            context.IsRemoveEnabled = false;
            context.IsCommitEnabled = false;

            grid.SetColumnDefaultValues(new Dictionary<string, string>
            {
                ["NU_AUTOMATISMO"] = nuAutomatismo,
            });

            grid.AddOrUpdateColumn(new GridColumnSelect("ND_PROTOCOLO_COMUNICACION", this.InitializeSelectProtocoloComunicacion()));
            grid.AddOrUpdateColumn(new GridColumnSelect("CD_INTERFAZ", this.InitializeSelectCodigoInterfaz()));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuAutomatismo = context.GetParameter("AUT100_NU_AUTOMATISMO");

            if (string.IsNullOrEmpty(nuAutomatismo) || !int.TryParse(nuAutomatismo, out int numeroAutomatismo))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            var query = new InterfacesAutomatismoQuery(numeroAutomatismo);

            uow.HandleQuery(query);

            grid.Rows = _gridService.GetRows(query, grid.Columns, context, this.DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string>
            {
                "NU_INTEGRACION_SERVICIO",
                "ND_PROTOCOLO_COMUNICACION",
                "CD_INTERFAZ",
                "CD_INTERFAZ_EXTERNA",
                "VL_URL"
            });

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuAutomatismo = context.GetParameter("AUT100_NU_AUTOMATISMO");

            if (string.IsNullOrEmpty(nuAutomatismo) || !int.TryParse(nuAutomatismo, out int numeroAutomatismo))
            {
                context.AddErrorNotification("AUT100Posiciones_Sec0_Error_FormatoNumeroAutomatismoIncorrecto");

                return null;
            }

            var query = new InterfacesAutomatismoQuery(numeroAutomatismo);

            uow.HandleQuery(query);

            query.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats()
            {
                Count = query.GetCount()
            };
        }
        
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuAutomatismo = context.GetParameter("AUT100_NU_AUTOMATISMO");

            if (string.IsNullOrEmpty(nuAutomatismo) || !int.TryParse(nuAutomatismo, out int numeroAutomatismo))
            {
                context.AddErrorNotification("AUT100Posiciones_Sec0_Error_FormatoNumeroAutomatismoIncorrecto");

                return null;
            }

            var dbQuery = new InterfacesAutomatismoQuery(numeroAutomatismo);

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application +"-"+ DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                grid.Rows.ForEach(row =>
                {
                    if (row.IsModified)
                        UpdateInterface(uow, row);
                });

                uow.SaveChanges();

                context.AddSuccessNotification("AUT100Interfaces_Sec0_Success_AutomatismoInterfazModificado");
            }
            catch (Exception e)
            {
                context.AddErrorNotification(e.Message);
            }

            return grid;
        }
        
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new AutomatismoInterfacesGridValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }
        
        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            switch (context.ColumnId)
            {
                case "NU_INTEGRACION_SERVICIO": return this.SearchServicioIntegracion(grid, context);
                case "CD_INTERFAZ_EXTERNA": return this.SearchInterfazExterna(grid, context, row);
            }

            return new List<SelectOption>();
        }

        #region Metodos Auxiliares

        public virtual List<SelectOption> SearchServicioIntegracion(Grid grid, GridSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<IntegracionServicio> servicios = uow.IntegracionServicioRepository.GetByDescriptionOrCodePartial(context.SearchValue);

            foreach (IntegracionServicio servicio in servicios)
            {
                opciones.Add(new SelectOption(servicio.Numero.ToString(), servicio.Numero + " - " + servicio.Descripcion));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchInterfazExterna(Grid grid, GridSelectSearchContext context, GridRow row)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            if (int.TryParse(row.GetCell("CD_INTERFAZ").Value, out int idInterfaz))
            {
                var interfacesExternas = uow.InterfazRepository.GetInterfacesExternasByCodigoInterfaz(idInterfaz);

                foreach (var interfaz in interfacesExternas)
                {
                    opciones.Add(new SelectOption(interfaz.CodigoInterfazExterna.ToString(), interfaz.CodigoInterfazExterna + " - " + interfaz.Descripcion));
                }
            }

            return opciones;
        }

        public virtual List<SelectOption> InitializeSelectProtocoloComunicacion()
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var dominios = uow.DominioRepository.GetDominios(IntegracionServicioDb.TIPO_PROTOCOLO_DOMAIN);

            foreach (var dominio in dominios)
                opciones.Add(new SelectOption(dominio.Id, dominio.Descripcion));

            return opciones;
        }

        public virtual List<SelectOption> InitializeSelectCodigoInterfaz()
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var interfaces = uow.InterfazRepository.GetInterfaces();

            foreach (var interfaz in interfaces)
                opciones.Add(new SelectOption(interfaz.Id.ToString(), $"{interfaz.Id} - {interfaz.NombreInterfaz}"));

            return opciones;
        }

        public virtual void UpdateInterface(IUnitOfWork uow, GridRow row)
        {
            AutomatismoInterfaz automatismoInterfaz = GetAutomatismoInterfazWithModifiedData(uow, row);

            uow.AutomatismoInterfazRepository.Update(automatismoInterfaz);
        }

        public virtual AutomatismoInterfaz GetAutomatismoInterfazWithModifiedData(IUnitOfWork uow, GridRow row)
        {
            var numeroAutomatismoInterfaz = int.Parse(row.GetCell("NU_AUTOMATISMO_INTERFAZ").Value);

            AutomatismoInterfaz automatismoInterfaz = uow.AutomatismoInterfazRepository.GetAutomatismoInterfazById(numeroAutomatismoInterfaz);

            var url = row.GetCell("VL_URL").Value;

            if (int.TryParse(row.GetCell("NU_INTEGRACION_SERVICIO").Value, out int integracionServicioID))
            {
                automatismoInterfaz.IdIntegracionServicio = integracionServicioID;
            }
            else
            {
                automatismoInterfaz.IdIntegracionServicio = null;
            }

            if (!string.IsNullOrEmpty(url) && !url.StartsWith('/')) url = $"/{url}";

            automatismoInterfaz.Method = url;

            automatismoInterfaz.IdProtocoloComunicacion = row.GetCell("ND_PROTOCOLO_COMUNICACION").Value;

            automatismoInterfaz.Interfaz = int.TryParse(row.GetCell("CD_INTERFAZ").Value, out int cdInterfaz) ? cdInterfaz : 0;

            automatismoInterfaz.InterfazExterna = int.TryParse(row.GetCell("CD_INTERFAZ_EXTERNA").Value, out int cdInterfazExterna) ? cdInterfazExterna : 0;

            return automatismoInterfaz;
        }

        #endregion
    }
}
