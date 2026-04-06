using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Domain.Recepcion;
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
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.REC
{
    public class REC500CrearFactura : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }

        public REC500CrearFactura(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ITrafficOfficerService concurrencyControl)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._concurrencyControl = concurrencyControl;

            this.GridKeys = new List<string>
            {
                "NU_RECEPCION_FACTURA"
            };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = false;
            context.IsRemoveEnabled = false;

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ACTIONS", new List<GridButton>
            {
                new GridButton("btnLineas", "General_Sec0_btn_EditarDetalle", "fas fa-list")
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new FacturaQuery();
            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("NU_RECEPCION_FACTURA", SortDirection.Descending);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new FacturaQuery();
            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("NU_RECEPCION_FACTURA", SortDirection.Descending);

            context.FileName = $"{this._identity.Application}{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new FacturaQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            form.GetField("ivaMinimo").Value = "0";
            form.GetField("ivaBase").Value = "0";

            InicializarSelects(form);

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new FacturaFormValidationModule(uow, this._identity.UserId, this._identity.Predio, this._identity.GetFormatProvider()), form, context);
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            Empresa emp = uow.EmpresaRepository.GetEmpresa(int.Parse(form.GetField("idEmpresa").Value));
            if (emp is null)
                throw new ValidationFailedException("General_Sec0_Error_FacturaEmpresa", new string[] { int.Parse(form.GetField("idEmpresa").Value).ToString() });

            Agente agente = uow.AgenteRepository.GetAgente(emp.Id, form.GetField("codigoInternoAgente").Value);
            if (agente is null)
                throw new ValidationFailedException("General_Sec0_Error_FacturaAgente", new string[] { form.GetField("codigoInternoAgente").Value });

            bool existe = uow.FacturaRepository.AnyFacturaActivaBySerieYNum(agente.Empresa, agente.CodigoInterno, form.GetField("numeroFactura").Value, form.GetField("numeroSerie").Value);
            if (existe)
                throw new ValidationFailedException("General_Sec0_Error_FacturaNumeroYSerie", new string[] { form.GetField("numeroFactura").Value, form.GetField("numeroSerie").Value });

            string numSerie = form.GetField("numeroSerie").Value.ToUpper();
            if (numSerie.Length > 3)
                throw new ValidationFailedException("General_Sec0_Error_FacturaNumSerie", new string[] { form.GetField("numeroSerie").Value });

            if (string.IsNullOrEmpty(form.GetField("moneda").Value))
                throw new ValidationFailedException("Seleccione una Moneda");

            uow.BeginTransaction();
            uow.CreateTransactionNumber("REC500 Crear factura");

            var factura = new Factura()
            {
                IdEmpresa = int.Parse(form.GetField("idEmpresa").Value),
                CodigoInternoCliente = form.GetField("codigoInternoAgente").Value,
                Serie = numSerie,
                Predio = form.GetField("numeroPredio").Value,
                NumeroFactura = form.GetField("numeroFactura").Value,
                TipoFactura = form.GetField("tipoFactura").Value,
                Anexo1 = form.GetField("anexo1").Value,
                Anexo2 = form.GetField("anexo2").Value,
                Anexo3 = form.GetField("anexo3").Value,
                Observacion = form.GetField("observacion").Value,
                CodigoMoneda = form.GetField("moneda").Value,
                IdOrigen = "M",
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now,
                Situacion = SituacionDb.Activo,
                Estado = EstadoFacturaDb.Pendiente,
                NumeroTransaccion = uow.GetTransactionNumber(),
            };

            if (DateTime.TryParse(form.GetField("vencimiento")?.Value, out DateTime vencimiento))
                factura.FechaVencimiento = vencimiento;

            if (DateTime.TryParse(form.GetField("emision")?.Value, out DateTime emision))
                factura.FechaEmision = emision;

            if (decimal.TryParse(form.GetField("totalDigitado")?.Value, System.Globalization.NumberStyles.Number, this._identity.GetFormatProvider(), out decimal totalDigitado))
                factura.TotalDigitado = totalDigitado;

            if (decimal.TryParse(form.GetField("ivaBase")?.Value, System.Globalization.NumberStyles.Number, this._identity.GetFormatProvider(), out decimal ivaBase))
                factura.IvaBase = ivaBase;

            if (decimal.TryParse(form.GetField("ivaMinimo")?.Value, System.Globalization.NumberStyles.Number, this._identity.GetFormatProvider(), out decimal ivaMinimo))
                factura.IvaMinimo = ivaMinimo;

            try
            {
                uow.FacturaRepository.AddFactura(factura);
                factura.SetCrearDetalleStrategy();
                factura.CrearDetallesFactura();
                uow.SaveChanges();
                uow.Commit();
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw;
            }

            context.Parameters?.Clear();
            context.Parameters.Add(new ComponentParameter("idFactura", factura.Id.ToString()));
            context.AddSuccessNotification("REC500_Frm1_Succes_Creacion", new List<string> { factura.Id.ToString() });

            return form;
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "idEmpresa": return this.SearchEmpresa(form, context);
                case "codigoInternoAgente": return this.SearchAgente(form, context);
                default: return new List<SelectOption>();
            }
        }

        #region Metodos Auxiliares

        public virtual void InicializarSelects(Form form)
        {
            FormField selectPredio = form.GetField("numeroPredio");
            FormField selectMoneda = form.GetField("moneda");
            FormField selectTotalDigitado = form.GetField("totalDigitado");
            FormField selectTipoFactura = form.GetField("tipoFactura");

            selectPredio.Options = new List<SelectOption>();
            selectMoneda.Options = new List<SelectOption>();
            selectTotalDigitado.Options = new List<SelectOption>();
            selectTipoFactura.Options = new List<SelectOption>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var monedas = uow.MonedaRepository.GetMonedas();
                foreach (var moneda in monedas)
                {
                    selectMoneda.Options.Add(new SelectOption(moneda.Codigo, moneda.Descripcion));
                }

                var tipos = uow.DominioRepository.GetDominios(CodigoDominioDb.TiposDeFacturas);
                foreach (var tipo in tipos)
                {
                    selectTipoFactura.Options.Add(new SelectOption(tipo.Valor, tipo.Descripcion));
                }
                selectTipoFactura.Value = tipos.FirstOrDefault()?.Valor;

                var dbQuery = new GetPrediosUsuarioQuery();
                uow.HandleQuery(dbQuery);

                List<Predio> predios = dbQuery.GetPrediosUsuario(_identity.UserId);
                foreach (var predio in predios)
                {
                    selectPredio.Options.Add(new SelectOption(predio.Numero, $"{predio.Numero} - {predio.Descripcion}")); ;
                }

                selectPredio.ReadOnly = false;

                if (_identity.Predio != GeneralDb.PredioSinDefinir)
                {
                    selectPredio.Value = _identity.Predio;
                    selectPredio.ReadOnly = true;
                }
                else if (predios.Count == 1)
                {
                    selectPredio.Value = predios.FirstOrDefault().Numero;
                    selectPredio.ReadOnly = true;
                }
            }
        }

        public virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();
            using var uow = this._uowFactory.GetUnitOfWork();

            List<Empresa> empresas = uow.EmpresaRepository.GetEmpresasUsuarioNoDocumentalesByNombreOrCodePartial(context.SearchValue);

            foreach (var empresa in empresas)
            {
                opciones.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}"));
            }
            return opciones;
        }

        public virtual List<SelectOption> SearchAgente(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();
            using var uow = this._uowFactory.GetUnitOfWork();

            if (int.TryParse(form.GetField("idEmpresa").Value, out int idEmpresa))
            {
                List<Agente> agentes = uow.AgenteRepository.GetAgenteByNombrePartial(idEmpresa, AgenteTipo.Proveedor, context.SearchValue);

                foreach (var agente in agentes)
                {
                    opciones.Add(new SelectOption(agente.CodigoInterno, $"{agente.Tipo} - {agente.Codigo} - {agente.Descripcion}"));
                }
            }
            else
            {
                form.GetField("idEmpresa").SetError("General_Sec0_Error_Error25");
            }
            return opciones;
        }

        #endregion

    }
}
