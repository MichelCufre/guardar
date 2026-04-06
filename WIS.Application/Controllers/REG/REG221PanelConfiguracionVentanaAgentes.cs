using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
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
using WIS.Domain.StockEntities;
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
    public class REG221PanelConfiguracionVentanaAgentes : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ILogger<REG221PanelConfiguracionVentanaAgentes> _logger;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ITrackingService _trackingService;

        protected List<string> GridKeys { get; }

        public REG221PanelConfiguracionVentanaAgentes(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            ILogger<REG221PanelConfiguracionVentanaAgentes> logger,
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
                "CD_EMPRESA",
                "CD_VENTANA_LIBERACION"
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
            context.IsRemoveEnabled = true;
            context.IsCommitEnabled = true;

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("btnEditar", "General_Sec0_btn_Editar", "far fa-edit"),
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ConfiguracionVentanaAgentesQuery dbQuery;
            if (context.Parameters.Count > 0)
            {

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa").Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");


                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "cliente").Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idCliente = context.Parameters.FirstOrDefault(s => s.Id == "cliente").Value;

                dbQuery = new ConfiguracionVentanaAgentesQuery(idCliente, idEmpresa);
            }
            else
            {
                dbQuery = new ConfiguracionVentanaAgentesQuery();
            }

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);


            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ConfiguracionVentanaAgentesQuery dbQuery;

            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa").Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");


                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "cliente").Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idCliente = context.Parameters.FirstOrDefault(s => s.Id == "cliente").Value;

                dbQuery = new ConfiguracionVentanaAgentesQuery(idCliente, idEmpresa);
            }
            else
            {
                dbQuery = new ConfiguracionVentanaAgentesQuery();
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

            ConfiguracionVentanaAgentesQuery dbQuery;
            if (context.Parameters.Count > 0)
            {

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa").Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");


                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "cliente").Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idCliente = context.Parameters.FirstOrDefault(s => s.Id == "cliente").Value;

                dbQuery = new ConfiguracionVentanaAgentesQuery(idCliente, idEmpresa);
            }
            else
            {
                dbQuery = new ConfiguracionVentanaAgentesQuery();
            }

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber(this._identity.Application);
            uow.BeginTransaction(uow.GetSnapshotIsolationLevel());

            try
            {
                foreach (var row in grid.Rows)
                {
                    var empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                    var cliente = row.GetCell("CD_CLIENTE").Value;
                    var ventanaLiberacion = row.GetCell("CD_VENTANA_LIBERACION").Value;
                    if (row.IsDeleted)
                    {
                        var clienteDiasValidezVentana = uow.AgenteRepository.GetVentanaLiberacionCliente(empresa, cliente, ventanaLiberacion);

                        uow.AgenteRepository.DeleteVentanaLiberacionCLiente(clienteDiasValidezVentana);
                    }
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null
                  && uow.IsSnapshotException(ex.InnerException))
                {
                    return this.GridCommit(grid, context);
                }
            }
            catch (ExpectedException ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message);
            }
            catch (Exception ex)
            {
                uow.Rollback();
                context.AddErrorNotification("General_Sec0_Error_Operacion_Reintente");
            }
            finally
            {
                uow.EndTransaction();
            }

            return grid;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            InicializarSelects(form, uow);

            if (form.Id == "REG221Update_form_1")
            {
                var empresa = int.Parse(context.GetParameter("empresa"));
                var cliente = context.GetParameter("cliente");
                var ventanaLiberacion = context.GetParameter("ventanaLiberacion");

                var clienteDiasValidezVentana = uow.AgenteRepository.GetVentanaLiberacionCliente(empresa, cliente, ventanaLiberacion);

                InicializarCamposUpdate(uow, form, clienteDiasValidezVentana);
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
                if (form.Id == "REG221Create_form_1")
                {
                    AddVentanaLiberacionCliente(form, uow);
                }
                else if (form.Id == "REG221Update_form_1")
                {
                    UpdateVentanaLiberacionCliente(form, uow);
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

            return this._formValidationService.Validate(new ClienteVentanaLiberacionFormValidationModule(uow, this._identity.UserId, this._identity.GetFormatProvider()), form, context);
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "empresa": return this.SearchEmpresa(form, context);
                case "cliente": return this.SearchCliente(form, context);
                default: return new List<SelectOption>();
            }
        }

        #region Metodos Auxiliares

        public virtual void InicializarSelects(Form form, IUnitOfWork uow)
        {
            var selectVentanaLiberacion = form.GetField("ventanaLiberacion");
            selectVentanaLiberacion.Options = new List<SelectOption>();
            List<DominioDetalle> dominios = uow.DominioRepository.GetDominios(CodigoDominioDb.VentanaLiberacion);
            foreach (var dominio in dominios)
            {
                selectVentanaLiberacion.Options.Add(new SelectOption(dominio.Valor.ToString(), $"{dominio.Valor} - {dominio.Descripcion}")); ;
            }
        }

        public virtual void InicializarCamposUpdate(IUnitOfWork uow, Form form, ClienteDiasValidezVentana clienteDiasValidezVentana)
        {
            form.GetField("cantidadDiasValidacion").Value = clienteDiasValidezVentana.CantidadDiasValidezLiberacion.ToString();

            var fieldVentanaLiberacion = form.GetField("ventanaLiberacion");
            fieldVentanaLiberacion.ReadOnly = true;
            fieldVentanaLiberacion.Value = clienteDiasValidezVentana.VentanaLiberacion;

            var fieldEmpresa = form.GetField("empresa");
            fieldEmpresa.ReadOnly = true;

            fieldEmpresa.Options = SearchEmpresa(form, new FormSelectSearchContext()
            {
                SearchValue = clienteDiasValidezVentana.Empresa.ToString()
            });

            fieldEmpresa.Value = clienteDiasValidezVentana.Empresa.ToString();

            var fieldCliente = form.GetField("cliente");
            fieldCliente.ReadOnly = true;

            fieldCliente.Options = SearchCliente(form, new FormSelectSearchContext()
            {
                SearchValue = clienteDiasValidezVentana.Cliente
            });

            fieldCliente.Value = clienteDiasValidezVentana.Cliente;

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

        public virtual List<SelectOption> SearchCliente(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            var empresa = form.GetField("empresa").Value;

            if (string.IsNullOrEmpty(empresa) || !int.TryParse(empresa, out int empresaId))
                return opciones;

            using var uow = this._uowFactory.GetUnitOfWork();

            var clientes = uow.AgenteRepository.GetClienteByDescripcionOrAgentePartial(context.SearchValue, empresaId);

            foreach (var cliente in clientes)
            {
                opciones.Add(new SelectOption(cliente.CodigoInterno, $"{cliente.Empresa} - {cliente.Codigo} - {cliente.Descripcion}"));
            }

            return opciones;
        }

        public virtual void UpdateVentanaLiberacionCliente(Form form, IUnitOfWork uow)
        {
            var empresa = int.Parse(form.GetField("empresa").Value);
            var cliente = form.GetField("cliente").Value;
            var ventanaLiberacion = form.GetField("ventanaLiberacion").Value;
            var cantidadDias = short.Parse(form.GetField("cantidadDiasValidacion").Value);

            var clienteDiasValidezVentana = uow.AgenteRepository.GetVentanaLiberacionCliente(empresa, cliente, ventanaLiberacion);
            clienteDiasValidezVentana.CantidadDiasValidezLiberacion = cantidadDias;

            uow.AgenteRepository.UpdateVentanaLiberacionCliente(clienteDiasValidezVentana);

            uow.SaveChanges();
        }

        public virtual void AddVentanaLiberacionCliente(Form form, IUnitOfWork uow)
        {
            var empresa = int.Parse(form.GetField("empresa").Value);
            var cliente = form.GetField("cliente").Value;
            var cantidadDias = short.Parse(form.GetField("cantidadDiasValidacion").Value);
            var ventanaLiberacion = form.GetField("ventanaLiberacion").Value;

            ClienteDiasValidezVentana clienteDiasValidezVentana = new ClienteDiasValidezVentana();
            clienteDiasValidezVentana.Empresa = empresa;
            clienteDiasValidezVentana.Cliente = cliente;
            clienteDiasValidezVentana.CantidadDiasValidezLiberacion = cantidadDias;
            clienteDiasValidezVentana.VentanaLiberacion = ventanaLiberacion;

            uow.AgenteRepository.AddVentanaLiberacionCliente(clienteDiasValidezVentana);

            uow.SaveChanges();
        }

        #endregion
    }
}