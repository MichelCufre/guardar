using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Application.Validation.Modules.GridModules.Preparacion;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.Impresiones;
using WIS.Domain.Picking;
using WIS.Domain.Picking.Logic;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;
using WIS.TrafficOfficer;
using TipoExpedicion = WIS.Domain.DataModel.Mappers.Constants.TipoExpedicion;

namespace WIS.Application.Controllers.PRE
{
    public class PRE052CreatePrepAdm : AppController
    {
        protected readonly ISecurityService _security;
        protected readonly IIdentityService _identity;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IFormValidationService _formValidationService;
        protected readonly ITrackingService _trackingService;
        protected readonly IParameterService _parameterService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected List<string> GridKeys1 { get; set; }
        protected List<SortCommand> DefaultSort1 { get; }
        protected List<string> GridKeys2 { get; set; }
        protected List<SortCommand> DefaultSort2 { get; }
        public PRE052CreatePrepAdm(
            ISecurityService security,
            ITrafficOfficerService concurrencyControl,
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IFormValidationService formValidationService,
            ITrackingService trackingService,
            IParameterService parameterService,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this._security = security;
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._formValidationService = formValidationService;
            this._trackingService = trackingService;
            _parameterService = parameterService;
            this._gridService = gridService;
            this._gridValidationService = gridValidationService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._concurrencyControl = concurrencyControl;

            this.GridKeys1 = new List<string> { "CD_ENDERECO", "CD_EMPRESA", "CD_PRODUTO", "CD_FAIXA", "NU_IDENTIFICADOR" };
            this.DefaultSort1 = new List<SortCommand> { new SortCommand("CD_ENDERECO", SortDirection.Ascending) };
            this.GridKeys2 = new List<string> { "NU_LPN" };
            this.DefaultSort2 = new List<SortCommand> { new SortCommand("NU_LPN", SortDirection.Ascending) };

        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            form.GetField("descripcion").Value = string.Empty;
            form.GetField("predio").Value = string.Empty;
            form.GetField("empresa").Value = string.Empty;
            form.GetField("cliente").Value = string.Empty;

            InicializarSelectTipoExpedicion(uow, form);
            InicializarSelectTipoPedido(uow, form);
            InicializarSelectPredio(uow, form, context);

            var tpAgente = form.GetField("condicionStock");
            tpAgente.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectCondicionStock)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            tpAgente.Value = CodigoDominioDb.SelectCondicionStockTodos;

            form.GetField("permitePickearVencido").Value = false.ToString();


            var fieldpredio = form.GetField("predio");

            form.GetField("permitePickearVencido").Value = "false";
            form.GetField("traspasoParcial").Value = "false";
            context.Parameters.Add(new ComponentParameter() { Id = "permitePickearVencido", Value = "false" });
            context.Parameters.Add(new ComponentParameter() { Id = "cantidadPrevVenc", Value = "0" });
            context.Parameters.Add(new ComponentParameter() { Id = "condicionStock", Value = CodigoDominioDb.SelectCondicionStockTodos });
       

            form.GetField("cantidadPrevVenc").Disabled = false;
            form.GetField("cantidadPrevVenc").ReadOnly = false;

            if (context.Parameters.Any(x => x.Id == "empresa"))
            {
                var fieldEmpresa = form.GetField("empresa");
                var fieldtpExpedicion = form.GetField("tipoExpedicion");
                var fieldtpPedido = form.GetField("tipoPedido");
                var fielRuta = form.GetField("ruta");

                var codEmpresa = context.Parameters.FirstOrDefault(x => x.Id == "empresa").Value;
                var predio = context.Parameters.FirstOrDefault(x => x.Id == "predio").Value;
                var tpExpedicion = context.Parameters.FirstOrDefault(x => x.Id == "tpExpedicion").Value;
                var tpPedido = context.Parameters.FirstOrDefault(x => x.Id == "tpPedido").Value;
                var ruta = context.Parameters.FirstOrDefault(x => x.Id == "ruta")?.Value;

                bool isEmpresaDocumental = uow.EmpresaRepository.IsEmpresaDocumental(int.Parse(codEmpresa));
                context.Parameters.Add(new ComponentParameter() { Id = "isEmpresaDocumental", Value = isEmpresaDocumental ? "S" : "N" });

                fieldEmpresa.Value = codEmpresa;

                var empresa = uow.EmpresaRepository.GetEmpresa(int.Parse(codEmpresa));

                if (empresa != null)
                {
                    fieldEmpresa.Disabled = true;
                    fieldEmpresa.ReadOnly = true;
                    fieldEmpresa.Value = empresa.Id.ToString();
                    fieldEmpresa.Options = new List<SelectOption> { new SelectOption(fieldEmpresa.Value, empresa.Nombre) };
                } 

                fieldtpExpedicion.Value = tpExpedicion;
                fieldtpExpedicion.Disabled = true;
                fieldtpExpedicion.ReadOnly = true;

                var opcionesTipos = new List<SelectOption>();
                var tipos = uow.PedidoRepository.GetTiposPedido(tpExpedicion);

                foreach (var tipo in tipos)
                {
                    opcionesTipos.Add(new SelectOption(tipo.Key, tipo.Value));
                }

                fieldtpPedido.Options = opcionesTipos;
                fieldtpPedido.Value = tpPedido;

                fieldtpPedido.Disabled = true;
                fieldtpPedido.ReadOnly = true;

                fieldpredio.Value = predio;
            }

            if (!string.IsNullOrEmpty(fieldpredio.Value))
            {
                context.Parameters.Add(new Components.Common.ComponentParameter() { Id = "predio", Value = fieldpredio.Value });
                fieldpredio.Disabled = true;
                fieldpredio.ReadOnly = true;
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {

            return form;
        }
        
        public override Form FormButtonAction(Form form, FormButtonActionContext context)
        {
            if (context.ButtonId == "disabledAllField")
            {
                var pedido = form.GetField("pedido");
                pedido.ReadOnly = true;
                pedido.Disabled = true;
                form.GetField("cliente").Disabled = true;
                form.GetField("ruta").Disabled = true;
                form.GetField("descripcion").Disabled = true;
                form.GetField("descripcion").ReadOnly = true;
                form.GetField("fechaEntrega").Disabled = true;
                form.GetField("tipoExpedicion").Disabled = true;
                form.GetField("tipoPedido").Disabled = true;
            }
            return form;
        }
        
        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var expedicionService = new ExpedicionConfiguracionService(uow, this._parameterService, new ParametroMapper());
            var configuracion = expedicionService.GetConfiguracionPedido();
            return this._formValidationService.Validate(new PreparacionAdmFormValidationModule(uow, this._identity, this._security, configuracion), form, context);

        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "empresa": return this.SearchEmpresa(form, context);
                case "cliente": return this.SearchCliente(form, context);
                case "ruta": return this.SearchRuta(form, context);
            }

            return new List<SelectOption>();
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            if (grid.Id == "PickingSuelto_grid_1")
                context.IsEditingEnabled = true;
            else
                context.IsEditingEnabled = false;

            context.IsAddEnabled = false;
            context.IsCommitEnabled = false;
            context.IsRemoveEnabled = false;

            return GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var empresa = context.Parameters.FirstOrDefault(x => x.Id == "empresa")?.Value;
            var predio = context.Parameters.FirstOrDefault(x => x.Id == "predio")?.Value;

            var permitePickearVencido = context.Parameters.FirstOrDefault(x => x.Id == "permitePickearVencido")?.Value == "true";
            var cantidadPrevVenc = 0;
            if (!string.IsNullOrEmpty(context.Parameters.FirstOrDefault(x => x.Id == "cantidadPrevVenc")?.Value) && int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "cantidadPrevVenc")?.Value, out int _cantidadPrevVenc))
                cantidadPrevVenc = _cantidadPrevVenc;

            var condicionStock = context.Parameters.FirstOrDefault(x => x.Id == "condicionStock")?.Value;

            if (int.TryParse(empresa, out int cdEmpresa) && !string.IsNullOrEmpty(predio))
            {

                if (grid.Id == "PickingSuelto_grid_1")
                {

                    var traspasoParcial = context.Parameters.FirstOrDefault(x => x.Id == "traspasoParcial")?.Value == "true";


                    var dbQuery = new StockSueltoPickingQuery(cdEmpresa, predio, permitePickearVencido, cantidadPrevVenc, condicionStock);

                    uow.HandleQuery(dbQuery);

                    grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort1, this.GridKeys1);

                    if (traspasoParcial)
                    {
                        grid.SetEditableCells(new List<string> { "QT_PICKING" });
                    }
                }
                else if (grid.Id == "PickingLpn_grid_2")
                {

                    var dbQuery = new LpnPickingQuery(cdEmpresa, predio, permitePickearVencido, cantidadPrevVenc, condicionStock);

                    uow.HandleQuery(dbQuery);

                    grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort2, this.GridKeys2);

                }
            }


            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var empresa = context.Parameters.FirstOrDefault(x => x.Id == "empresa")?.Value;
            var predio = context.Parameters.FirstOrDefault(x => x.Id == "predio")?.Value;

            if (int.TryParse(empresa, out int cdEmpresa) && !string.IsNullOrEmpty(predio))
            {
                var permitePickearVencido = context.Parameters.FirstOrDefault(x => x.Id == "permitePickearVencido")?.Value == "true";
                var cantidadPrevVenc = 0;
                if (!string.IsNullOrEmpty(context.Parameters.FirstOrDefault(x => x.Id == "cantidadPrevVenc")?.Value) && int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "cantidadPrevVenc")?.Value, out int _cantidadPrevVenc))
                    cantidadPrevVenc = _cantidadPrevVenc;

                var condicionStock = context.Parameters.FirstOrDefault(x => x.Id == "condicionStock")?.Value;

                if (grid.Id == "PickingSuelto_grid_1")
                {
                    var dbQuery = new StockSueltoPickingQuery(cdEmpresa, predio, permitePickearVencido, cantidadPrevVenc, condicionStock);

                    uow.HandleQuery(dbQuery);

                    context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                    return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort1);
                }
                else if (grid.Id == "PickingLpn_grid_2")
                {

                    var dbQuery = new LpnPickingQuery(cdEmpresa, predio, permitePickearVencido, cantidadPrevVenc, condicionStock);

                    uow.HandleQuery(dbQuery);

                    context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                    return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort2);

                }
            }
            return null;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new PRE052PickingParcialGridValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var empresa = query.Parameters.FirstOrDefault(x => x.Id == "empresa")?.Value;
            var predio = query.Parameters.FirstOrDefault(x => x.Id == "predio")?.Value;

            if (int.TryParse(empresa, out int cdEmpresa) && !string.IsNullOrEmpty(predio))
            {
                var permitePickearVencido = query.Parameters.FirstOrDefault(x => x.Id == "permitePickearVencido")?.Value == "true";
                var cantidadPrevVenc = 0;
                if (!string.IsNullOrEmpty(query.Parameters.FirstOrDefault(x => x.Id == "cantidadPrevVenc")?.Value) && int.TryParse(query.Parameters.FirstOrDefault(x => x.Id == "cantidadPrevVenc")?.Value, out int _cantidadPrevVenc))
                    cantidadPrevVenc = _cantidadPrevVenc;

                var condicionStock = query.Parameters.FirstOrDefault(x => x.Id == "condicionStock")?.Value;

                if (grid.Id == "PickingSuelto_grid_1")
                {
                    var dbQuery = new StockSueltoPickingQuery(cdEmpresa, predio, permitePickearVencido, cantidadPrevVenc, condicionStock);

                    uow.HandleQuery(dbQuery);

                    dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                    return new GridStats
                    {
                        Count = dbQuery.GetCount()
                    };
                }
                else if (grid.Id == "PickingLpn_grid_2")
                {

                    var dbQuery = new LpnPickingQuery(cdEmpresa, predio, permitePickearVencido, cantidadPrevVenc, condicionStock);

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

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (context.GridId == "PickingSuelto_grid_1")
            {
                GridMenuItemActionGridPickingSuelto(context, uow);
            }
            else if (context.GridId == "PickingLpn_grid_2")
            {
                GridMenuItemActionGridPickingLpn(context, uow);
            }

            return context;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var transactionTO = this._concurrencyControl.CreateTransaccion();

            uow.BeginTransaction();

            uow.CreateTransactionNumber("Expulsar Producto Producción");
            var nuTransaccion = uow.GetTransactionNumber();

            try
            {
                ObtenerFilasAPreparar(grid, context, uow, out List<StockPicking> stocksPicking, out List<LpnDetalle> detallesLpns);

                if ((stocksPicking == null || stocksPicking.Count() == 0) && (detallesLpns == null || detallesLpns.Count() == 0))
                {
                    throw new ValidationFailedException("PRD113_Sec0_Error_03");
                }

                AgregarBloqueos(stocksPicking, detallesLpns, transactionTO);

                var preparacion = ProcesarOperacion(context, uow, nuTransaccion, stocksPicking, detallesLpns);

                uow.SaveChanges();
                uow.Commit();


                context.AddSuccessNotification("PRE052_msg_Success_PreparacionGeneradoa", new List<string> { preparacion.ToString() });

            }
            catch (EntityLockedException ex)
            {
                context.Parameters.Add(new ComponentParameter() { Id = "ERROR", Value = "" });
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (ValidationFailedException ex)
            {
                context.Parameters.Add(new ComponentParameter() { Id = "ERROR", Value = "" });
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                context.Parameters.Add(new ComponentParameter() { Id = "ERROR", Value = "" });
                context.AddErrorNotification(ex.Message);
            }
            finally
            {
                this._concurrencyControl.DeleteTransaccion(transactionTO);
            }
            return grid;
        }

        public virtual int ProcesarOperacion(GridFetchContext context, IUnitOfWork uow, long nuTransaccion, List<StockPicking> stocksPicking, List<LpnDetalle> detallesLpns)
        {
            Impresion impresion = null;
            decimal? nuEtiquetaExterna = null;

            var idPedido = context.Parameters.FirstOrDefault(x => x.Id == "pedido").Value;
            var descripcion = string.IsNullOrEmpty(context.Parameters.FirstOrDefault(x => x.Id == "descripcion").Value) ? "Preparación administrativa" : context.Parameters.FirstOrDefault(x => x.Id == "descripcion").Value;
            var empresa = int.Parse(context.Parameters.FirstOrDefault(x => x.Id == "empresa").Value);
            var cliente = context.Parameters.FirstOrDefault(x => x.Id == "cliente").Value;
            var predio = context.Parameters.FirstOrDefault(x => x.Id == "predio").Value;
            var tpPedido = context.Parameters.FirstOrDefault(x => x.Id == "tpPedido").Value;
            var tpExpedicion = context.Parameters.FirstOrDefault(x => x.Id == "tpExpedicion").Value;
            var ruta = int.Parse(context.Parameters.FirstOrDefault(x => x.Id == "ruta").Value);
            int? nroPreparacion = null;
            bool crearPreparacionCarga = true;

            if (int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "preparacion")?.Value, out int prep))
            {
                crearPreparacionCarga = false;
                nroPreparacion = prep;
            }

            DateTime? fechaEntrega = null;

            if (DateTime.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "fechaEntrega").Value, this._identity.GetFormatProvider(), DateTimeStyles.None, out DateTime entrega))
            {
                fechaEntrega = entrega;
            }

            PickingLogic logic = new PickingLogic(_identity, _trackingService);

            logic.PickingAdministrativo(uow, nuTransaccion, descripcion, stocksPicking, detallesLpns, ref idPedido, ruta, empresa, cliente, predio, tpPedido, tpExpedicion, ref nroPreparacion, crearPreparacionCarga, fechaEntrega);

            context.Parameters.RemoveAll(x => x.Id == "pedido");
            context.Parameters.RemoveAll(x => x.Id == "preparacion");
            context.Parameters.Add(new ComponentParameter() { Id = "pedido", Value = idPedido });
            context.Parameters.Add(new ComponentParameter() { Id = "preparacion", Value = nroPreparacion.ToString() });


            return nroPreparacion ?? 0;
        }

        #region Metodos Auxiliares

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

            string empresa = form.GetField("empresa").Value;

            if (string.IsNullOrEmpty(empresa) || !int.TryParse(empresa, out int empresaId))
                return opciones;

            using var uow = this._uowFactory.GetUnitOfWork();

            var expedicionService = new ExpedicionConfiguracionService(uow, this._parameterService, new ParametroMapper());
            var configuracion = expedicionService.GetConfiguracionPedido();

            if (configuracion.PermitePedidosAProveedores)
            {
                List<Agente> clientes = uow.AgenteRepository.GetByDescripcionOrAgentePartial(context.SearchValue, empresaId);

                foreach (var cliente in clientes)
                {
                    opciones.Add(new SelectOption(cliente.CodigoInterno, $"{cliente.Empresa} - {cliente.Tipo} - {cliente.Codigo} - {cliente.Descripcion} "));
                }
            }
            else
            {
                List<Agente> clientes = uow.AgenteRepository.GetClienteByDescripcionOrAgentePartial(context.SearchValue, empresaId);

                foreach (var cliente in clientes)
                {
                    opciones.Add(new SelectOption(cliente.CodigoInterno, $"{cliente.Empresa} - {cliente.Codigo} - {cliente.Descripcion}"));
                }
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchRuta(Form form, FormSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();

            string predio = form.GetField("predio").Value;
            if (!string.IsNullOrEmpty(predio))
            {
                using var uow = this._uowFactory.GetUnitOfWork();

                var rutas = uow.RutaRepository.GetByDescripcionOrCodePartial(context.SearchValue, predio);
                foreach (var ruta in rutas)
                {
                    var descRuta = $"{ruta.Id} - {ruta.Descripcion}";
                    descRuta += $" - {(ruta.Onda == null ? "SIN ONDA" : ruta.Onda?.Descripcion)}";

                    if (!string.IsNullOrEmpty(ruta.Zona))
                        descRuta += $" - {ruta.Zona}";

                    opciones.Add(new SelectOption(ruta.Id.ToString(), descRuta));
                }

            }
            return opciones;
        }

        public virtual void InicializarSelectTipoExpedicion(IUnitOfWork uow, Form form)
        {
            var opcionesConfiguraciones = new List<SelectOption>();
            var configuracionesExpedicion = uow.PedidoRepository.GetConfiguracionesExpedicion();

            foreach (var configExp in configuracionesExpedicion)
            {
                if (configExp.Tipo == TipoExpedicion.ReservasPrepManual)
                    continue;

                opcionesConfiguraciones.Add(new SelectOption(configExp.Tipo, configExp.Descripcion));
            }

            form.GetField("tipoExpedicion").Options = opcionesConfiguraciones;
            form.GetField("tipoExpedicion").Value = TipoExpedicion.Facturables;
        }

        public virtual void InicializarSelectTipoPedido(IUnitOfWork uow, Form form)
        {
            var opcionesTipos = new List<SelectOption>();
            var tipos = uow.PedidoRepository.GetTiposPedido(TipoExpedicion.Facturables);

            foreach (var tipo in tipos)
            {
                opcionesTipos.Add(new SelectOption(tipo.Key, tipo.Value));
            }

            form.GetField("tipoPedido").Options = opcionesTipos;
            form.GetField("tipoPedido").Value = TipoPedidoDb.Venta;
        }

        public virtual void InicializarSelectPredio(IUnitOfWork uow, Form form, FormInitializeContext context)
        {
            var selectPredio = form.GetField("predio");
            selectPredio.Options = new List<SelectOption>();
            var dbQuery = new GetPrediosUsuarioQuery();
            uow.HandleQuery(dbQuery);

            var predios = dbQuery.GetPrediosUsuario(_identity.UserId).OrderBy(x => x.Numero);
            foreach (var pred in predios)
            {
                selectPredio.Options.Add(new SelectOption(pred.Numero, $"{pred.Numero} - {pred.Descripcion}")); ;
            }

            if (predios.Count() == 1)
                selectPredio.Value = predios.FirstOrDefault().Numero;

            if (!this._identity.Predio.Equals(GeneralDb.PredioSinDefinir))
                selectPredio.Value = this._identity.Predio;
        }

        public virtual Preparacion CrearPreparacion(Form form, IUnitOfWork uow)
        {
            return new Preparacion()
            {
                Descripcion = string.IsNullOrEmpty(form.GetField("descripcion").Value) ? "Preparación manual libre" : form.GetField("descripcion").Value,
                Empresa = int.Parse(form.GetField("empresa").Value),
                FechaInicio = DateTime.Now,
                Tipo = TipoPreparacionDb.Libre,
                Situacion = SituacionDb.PreparacionIniciada,
                Usuario = this._identity.UserId,
                CodigoContenedorValidado = "TPOPED",
                Predio = form.GetField("predio").Value,
                Transaccion = uow.GetTransactionNumber(),
                PermitePickVencido = bool.Parse(form.GetField("permitePickearVencido").Value),
                AceptaMercaderiaAveriada = bool.Parse(form.GetField("permitePickearAveriado").Value),
                ValidarProductoProveedor = bool.Parse(form.GetField("validarProductoProveedor").Value),
            };
        }

        public virtual void GridMenuItemActionGridPickingSuelto(GridMenuItemActionContext context, IUnitOfWork uow)
        {
            var selection = context.Selection.GetSelection(this.GridKeys1);
            var stock = selection.Select(item => new StockPicking
            {
                Ubicacion = item["CD_ENDERECO"],
                Empresa = int.Parse(item["CD_EMPRESA"]),
                Producto = item["CD_PRODUTO"],
                Faixa = decimal.Parse(item["CD_FAIXA"], _identity.GetFormatProvider()),
                Identificador = item["NU_IDENTIFICADOR"]
            }).ToList();

            var empresa = context.Parameters.FirstOrDefault(x => x.Id == "empresa")?.Value;
            var predio = context.Parameters.FirstOrDefault(x => x.Id == "predio")?.Value;
            var permitePickearVencido = context.Parameters.FirstOrDefault(x => x.Id == "permitePickearVencido")?.Value == "true";
            var condicionStock = context.Parameters.FirstOrDefault(x => x.Id == "condicionStock")?.Value;

            var cantidadPrevVenc = 0;
            if (!string.IsNullOrEmpty(context.Parameters.FirstOrDefault(x => x.Id == "cantidadPrevVenc")?.Value) && int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "cantidadPrevVenc")?.Value, out int _cantidadPrevVenc))
                cantidadPrevVenc = _cantidadPrevVenc;

            if (context.Selection.AllSelected)
            {
                var dbQuery = new StockSueltoPickingQuery(int.Parse(empresa), predio, permitePickearVencido, cantidadPrevVenc, condicionStock);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                var stockSeleccion = dbQuery.GetProductosPicking(_identity);

                stock = stockSeleccion.Join(stock,
                    sts => new { sts.Ubicacion, sts.Producto, sts.Empresa, sts.Identificador, sts.Faixa },
                    st => new { st.Ubicacion, st.Producto, st.Empresa, st.Identificador, st.Faixa },
                    (sts, st) => sts).ToList();

                stock = stockSeleccion.Except(stock).ToList();
            }
            else
            {
                var dbQuery = new StockSueltoPickingQuery(int.Parse(empresa), predio, permitePickearVencido, cantidadPrevVenc, condicionStock);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                var stockSeleccion = dbQuery.GetProductosPicking(_identity);

                stock = stockSeleccion.Join(stock,
                    sts => new { sts.Ubicacion, sts.Producto, sts.Empresa, sts.Identificador, sts.Faixa },
                    st => new { st.Ubicacion, st.Producto, st.Empresa, st.Identificador, st.Faixa },
                    (sts, st) => sts).ToList();
            }

            context.AddParameter("PRD052_StockPicking", JsonConvert.SerializeObject(stock));
        }

        public virtual void GridMenuItemActionGridPickingLpn(GridMenuItemActionContext context, IUnitOfWork uow)
        {
            var selection = context.Selection.GetSelection(this.GridKeys2);
            var lpns = selection.Select(item => new LpnPicking
            {
                Lpn = long.Parse(item["NU_LPN"]),
            }).ToList();

            var empresa = context.Parameters.FirstOrDefault(x => x.Id == "empresa")?.Value;
            var predio = context.Parameters.FirstOrDefault(x => x.Id == "predio")?.Value;
            var permitePickearVencido = context.Parameters.FirstOrDefault(x => x.Id == "permitePickearVencido")?.Value == "true";
            var condicionStock = context.Parameters.FirstOrDefault(x => x.Id == "condicionStock")?.Value;

            var cantidadPrevVenc = 0;
            if (!string.IsNullOrEmpty(context.Parameters.FirstOrDefault(x => x.Id == "cantidadPrevVenc")?.Value) && int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "cantidadPrevVenc")?.Value, out int _cantidadPrevVenc))
                cantidadPrevVenc = _cantidadPrevVenc;


            if (context.Selection.AllSelected)
            {
                var dbQuery = new LpnPickingQuery(int.Parse(empresa), predio, permitePickearVencido, cantidadPrevVenc, condicionStock);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                var stockSeleccion = dbQuery.GetLpnPicking(_identity);

                lpns = stockSeleccion.Join(lpns,
                    sts => new { sts.Lpn },
                    st => new { st.Lpn },
                    (sts, st) => sts).ToList();

                lpns = stockSeleccion.Except(lpns).ToList();
            }
            else
            {
                var dbQuery = new LpnPickingQuery(int.Parse(empresa), predio, permitePickearVencido, cantidadPrevVenc, condicionStock);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                var stockSeleccion = dbQuery.GetLpnPicking(_identity);

                lpns = stockSeleccion.Join(lpns,
                    sts => new { sts.Lpn },
                    st => new { st.Lpn },
                    (sts, st) => sts).ToList();
            }

            context.AddParameter("PRD052_LpnPicking", JsonConvert.SerializeObject(lpns));
        }

        public virtual void ObtenerFilasAPreparar(Grid grid, GridFetchContext context, IUnitOfWork uow, out List<StockPicking> stocksSeleccionado, out List<LpnDetalle> detallesLpns)
        {
            int rowId = 0;
            stocksSeleccionado = JsonConvert.DeserializeObject<List<StockPicking>>(context.Parameters.FirstOrDefault(x => x.Id == "PRD052_StockPicking")?.Value);
            List<StockPicking> LpnstocksSeleccionado = new List<StockPicking>();
            foreach (var row in grid.Rows)
            {
                string ubicacion = row.GetCell("CD_ENDERECO").Value;
                int empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                string producto = row.GetCell("CD_PRODUTO").Value;
                decimal faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value, _identity.GetFormatProvider());
                string identificador = row.GetCell("NU_IDENTIFICADOR").Value;
                decimal cantidadPicking = decimal.Parse(row.GetCell("QT_PICKING").Value, _identity.GetFormatProvider());
                var stockSeleccionado = stocksSeleccionado.FirstOrDefault(x => x.Ubicacion == ubicacion && x.Empresa == empresa && x.Producto == producto && x.Faixa == faixa && x.Identificador == identificador);
                DateTime? vencimiento = string.IsNullOrEmpty(row.GetCell("DT_FABRICACAO").Value) ? null : DateTime.Parse(row.GetCell("DT_FABRICACAO").Value, _identity.GetFormatProvider());
                if (stockSeleccionado == null)
                {
                    stocksSeleccionado.Add(new StockPicking
                    {
                        Ubicacion = ubicacion,
                        Empresa = empresa,
                        Producto = producto,
                        Faixa = faixa,
                        Identificador = identificador,
                        Cantidad = cantidadPicking,
                        Vencimiento = vencimiento
                    });
                }
                else
                {
                    stocksSeleccionado.Remove(stockSeleccionado);
                    stocksSeleccionado.Add(new StockPicking
                    {
                        Ubicacion = ubicacion,
                        Empresa = empresa,
                        Producto = producto,
                        Faixa = faixa,
                        Identificador = identificador,
                        Cantidad = cantidadPicking,
                        Vencimiento = vencimiento
                    });
                }
            }
            List<LpnPicking> lpnSeleccionado = JsonConvert.DeserializeObject<List<LpnPicking>>(context.Parameters.FirstOrDefault(x => x.Id == "PRD052_LpnPicking")?.Value);
            uow.PreparacionRepository.AddPickingLpnTemporal(lpnSeleccionado);
            detallesLpns = uow.PreparacionRepository.GetDetallesLpnsPreparar();

            if (stocksSeleccionado != null && stocksSeleccionado.Count() > 0)
            {
                foreach (var det in stocksSeleccionado)
                {
                    det.IdRow = rowId;
                    rowId = rowId + 1;
                }
            }
        }

        public virtual void AgregarBloqueos(List<StockPicking> stocksPreparar, List<LpnDetalle> detallesLpns, TrafficOfficerTransaction transactionTO)
        {
            var detallesStockSuelto = stocksPreparar
                .GroupBy(g => new { g.Ubicacion, g.Empresa, g.Producto, g.Faixa, g.Identificador })
                .Select(s => new { key = $"{s.Key.Ubicacion}#{s.Key.Empresa}#{s.Key.Producto}#{s.Key.Faixa}#{s.Key.Identificador}" }).Select(x => x.key);
            var detallesStockLpns = detallesLpns
               .GroupBy(g => new { g.Ubicacion, g.Empresa, g.CodigoProducto, g.Faixa, g.Lote })
               .Select(s => new { key = $"{s.Key.Ubicacion}#{s.Key.Empresa}#{s.Key.CodigoProducto}#{s.Key.Faixa}#{s.Key.Lote}" }).Select(x => x.key);

            var detallesStock = detallesStockSuelto.Union(detallesStockLpns).Distinct();
            if (detallesStock.Count() > 0)
            {
                var listLock = this._concurrencyControl.GetLockList("T_STOCK", detallesStock.ToList(), transactionTO);

                if (listLock.Count > 0)
                {
                    var keyBloqueo = listLock.FirstOrDefault().Id_Bloqueo.Split("#");
                    throw new EntityLockedException("PRD113_msg_Error_StockBloqueada", new string[] { keyBloqueo[2], keyBloqueo[4] });
                }
                this._concurrencyControl.AddLockList("T_STOCK", detallesStock.ToList(), transactionTO);

            }

            if (detallesLpns.Count > 0)
            {
                var lpns = detallesLpns.GroupBy(x => x.NumeroLPN).Select(x => new { key = $"{x.Key}" }).Select(x => x.key);
                var listLpnsLock = this._concurrencyControl.GetLockList("T_LPN", lpns.ToList(), transactionTO);

                if (listLpnsLock.Count > 0)
                {
                    var keyBloqueo = listLpnsLock.FirstOrDefault().Id_Bloqueo.Split("#");
                    throw new EntityLockedException("PRD113_msg_Error_StockBloqueada", new string[] { keyBloqueo[2], keyBloqueo[4] });
                }

                this._concurrencyControl.AddLockList("T_LPN", lpns.ToList(), transactionTO, true);


                var detallesLpnsKey = detallesLpns.Select(x => new { key = $"{x.Id}#{x.NumeroLPN}#{x.CodigoProducto}#{x.Faixa}#{x.Empresa}#{x.Lote}" }).Select(x => x.key);

                var listDetallesLpnsLock = this._concurrencyControl.GetLockList("T_LPN_DET", detallesLpnsKey.ToList(), transactionTO);

                if (listDetallesLpnsLock.Count > 0)
                {
                    var keyBloqueo = listLpnsLock.FirstOrDefault().Id_Bloqueo.Split("#");
                    throw new EntityLockedException("PRD113_msg_Error_StockBloqueada", new string[] { keyBloqueo[2], keyBloqueo[4] });
                }

                this._concurrencyControl.AddLockList("T_LPN_DET", detallesLpnsKey.ToList(), transactionTO,true);
            }
        }

        #endregion
    }
}
