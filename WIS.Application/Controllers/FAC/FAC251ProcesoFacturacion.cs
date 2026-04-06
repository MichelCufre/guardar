using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Eventos;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Facturacion;
using WIS.Domain.Facturacion;
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
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.FAC
{
    public class FAC251ProcesoFacturacion : AppController
    {

        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<FAC251ProcesoFacturacion> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public FAC251ProcesoFacturacion(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            IIdentityService identity,
            ISecurityService security,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter,
            ILogger<FAC251ProcesoFacturacion> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "CD_PROCESO",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_PROCESO", SortDirection.Descending),
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
            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("btnEditar", "FAC251_frm1_btn_MODIFICAR", "fas fa-edit"),

            }));

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                RegistroProcesoFacturacion registroProcesoFacturacion = new RegistroProcesoFacturacion(uow, this._identity.UserId, this._identity.Application);
                FacturacionProceso ProcesoFacuracion = new FacturacionProceso();
                FacturacionCodigo facturacionCodigo = uow.FacturacionRepository.GetFacturacionCodigo(form.GetField("codigoFactura").Value);

                ProcesoFacuracion.CodigoProceso = form.GetField("codigoProceso").Value;
                ProcesoFacuracion.CodigoSituacionError = SituacionDb.SITUACION_PROCESO;
                ProcesoFacuracion.CodigoFacturacion = form.GetField("codigoFactura").Value;
                ProcesoFacuracion.NombreProcedimiento = null;
                ProcesoFacuracion.DescripcionProceso = form.GetField("descripcionProceso").Value;
                ProcesoFacuracion.NumeroComponente = form.GetField("componente").Value;
                ProcesoFacuracion.NumeroCuentaContable = form.GetField("numeroCuentaContable").Value; ;
                ProcesoFacuracion.TipoProceso = facturacionCodigo.TipoCalculo;
                ProcesoFacuracion.EjecucionPorHora = FacturacionDb.EJECUCION_HORA;

                //Si no es readonly es creacion sino es update
                if (form.GetField("codigoFactura").ReadOnly == false)
                    registroProcesoFacturacion.RegistrarProcesoFacturacion(ProcesoFacuracion);
                else
                    registroProcesoFacturacion.UpdateProcesoFacturacion(ProcesoFacuracion);

                uow.SaveChanges();

                uow.Commit();


                context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw ex;
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            //Si es readonly es una modificacion por lo cual solo controlaremos dos campos del form
            if (form.GetField("codigoFactura").ReadOnly == false)
                return this._formValidationService.Validate(new RegistrarProcesoFacturacionValidationModule(uow, this._identity.UserId, this._identity.Predio), form, context);
            else
                return this._formValidationService.Validate(new ActualizarProcesoFacturacionValidationModule(uow, this._identity.UserId, this._identity.Predio), form, context);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ProcesoFacturacionQuery dbQuery = new ProcesoFacturacionQuery();
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);
            return grid;
        }
        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            form.GetField("componente").ReadOnly = true;
            using var uow = this._uowFactory.GetUnitOfWork();
            this.InicializarSelect(uow, form);


            if (context.Parameters.Any(x => x.Id == "codigoProceso"))
            {
                string codigoProceso = context.Parameters.FirstOrDefault(s => s.Id == "codigoProceso").Value;


                FacturacionProceso procesoFacturacion = uow.FacturacionRepository.GetFacturacionProceso(codigoProceso);

                form.GetField("descripcionProceso").Value = procesoFacturacion.DescripcionProceso;
                form.GetField("numeroCuentaContable").Value = procesoFacturacion.NumeroCuentaContable;
                form.GetField("componente").Value = procesoFacturacion.NumeroComponente;
                form.GetField("componente").ReadOnly = true;
                form.GetField("codigoFactura").Value = procesoFacturacion.CodigoFacturacion;
                form.GetField("codigoFactura").ReadOnly = true;
                form.GetField("codigoProceso").Value = procesoFacturacion.CodigoProceso.ToString();
                form.GetField("codigoProceso").ReadOnly = true;
            }
            return form;
        }
        public virtual void InicializarSelect(IUnitOfWork uow, Form form)
        {
            FormField selectorCodigoFactura = form.GetField("codigoFactura");
            FormField selectorComponente = form.GetField("componente");
            FormField numeroCuentaContable = form.GetField("numeroCuentaContable");

            selectorCodigoFactura.Options = new List<SelectOption>();
            numeroCuentaContable.Options = new List<SelectOption>();
            selectorComponente.Options = new List<SelectOption>();
            //CodigoFactura
            List<FacturacionCodigo> codigosDeFacturacion = uow.FacturacionRepository.GetAllFacturacionCodigos();
            foreach (var codigo in codigosDeFacturacion)
            {
                selectorCodigoFactura.Options.Add(new SelectOption(codigo.CodigoFacturacion, $"{codigo.CodigoFacturacion} - {codigo.DescripcionFacturacion}"));
            }

            //Tipo de Calculo
            List<CuentaContable> cuentasContables = uow.FacturacionRepository.GetAllCuentaContables();
            foreach (var cuentas in cuentasContables)
            {
                numeroCuentaContable.Options.Add(new SelectOption(cuentas.Id, $"{cuentas.Id} - {cuentas.Descripcion}"));
            }

            //Componentes
            List<FacturacionCodigoComponente> codigosComponentes = uow.FacturacionRepository.GetAllFacturacionCodigoComponente();
            foreach (var componente in codigosComponentes)
            {
                selectorComponente.Options.Add(new SelectOption(componente.NumeroComponente, $"{componente.NumeroComponente} - {componente.Descripcion}"));
            }


        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ProcesoFacturacionQuery dbQuery = new ProcesoFacturacionQuery();

            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ProcesoFacturacionQuery dbQuery = new ProcesoFacturacionQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
    }
}
