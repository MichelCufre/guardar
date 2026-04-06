using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Impresiones;
using WIS.Domain.Parametrizacion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent.Build;
using WIS.GridComponent.Excel;
using WIS.Security;
using WIS.Session;

namespace WIS.Application.Controllers.STO
{
    public class STO700CrearLPN : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<STO700CrearLPN> _logger;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IRecepcionService _recepcionService;
        protected readonly IPrintingService _printingService;
        protected readonly List<string> _prediosNoEspecificados = new List<string> { GeneralDb.PredioSinDefinir, GeneralDb.SinEspecificar, GeneralDb.PredioSinPredio };

        public STO700CrearLPN(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            IIdentityService identity,
            ISecurityService security,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter,
            ILogger<STO700CrearLPN> logger,
            IGridValidationService gridValidationService,
            IRecepcionService recepcionService,
            IPrintingService printingService)
        {

            this._uowFactory = uowFactory;
            this._session = session;
            this._identity = identity;
            this._security = security;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._formValidationService = formValidationService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
            this._recepcionService = recepcionService;
            this._printingService = printingService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            form.GetField("estilo").ReadOnly = true;
            form.GetField("lenguaje").ReadOnly = true;
            form.GetField("descripcion").ReadOnly = true;

            var impresion = uow.ImpresionRepository.ObtenerImpresoraUltimaImpresion(this._identity.UserId, this._identity.Predio);

            if (impresion != null && !string.IsNullOrEmpty(impresion.CodigoImpresora))
            {
                if (uow.ImpresoraRepository.ExisteImpresora(impresion.CodigoImpresora, impresion.Predio))//Por si se cambia el codigo de impresora
                {
                    var impresora = uow.ImpresoraRepository.GetImpresora(impresion.CodigoImpresora, impresion.Predio);
                    var lenguajeImpresion = uow.ImpresionRepository.GetLenguajeImpresion(impresora?.CodigoLenguajeImpresion);

                    form.GetField("impresora").Value = impresion.CodigoImpresora;
                    form.GetField("lenguaje").Value = lenguajeImpresion?.Id;
                    form.GetField("descripcion").Value = lenguajeImpresion?.Descripcion;
                }

                form.GetField("impresora").ReadOnly = false;
                form.GetField("predio").Value = impresion.Predio;
            }
            else
                form.GetField("predio").Value = this._identity.Predio;

            if (!_prediosNoEspecificados.Contains(this._identity.Predio))
                form.GetField("predio").Disabled = true;

            InicializarSelect(uow, form);

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                int numImpresion = -1;

                uow.CreateTransactionNumber("STO700 Alta Lpn");
                uow.BeginTransaction(uow.GetSnapshotIsolationLevel());

                var tipo = uow.ManejoLpnRepository.GetTipoLpn(form.GetField("tipo").Value);

                var lpns = GenerarLpns(uow, form, tipo);
                GenerarAtributosLpns(uow, tipo, lpns);

                uow.SaveChanges();

                var generarImprimir = form.GetField("generarImprimir").Value;

                if (generarImprimir == "true")
                {
                    if (lpns.Count > 0)
                    {
                        var predio = form.GetField("predio").Value;
                        var descripcionEstilo = form.GetField("estilo").Value;
                        var estilo = uow.ManejoLpnRepository.GetCodigoEstiloEtiquetaLPN(tipo.Tipo);
                        var lenguaje = form.GetField("lenguaje").Value;
                        var impresora = form.GetField("impresora").Value;
                        var cantidadCopias = int.Parse(form.GetField("cantidadCopias").Value);
                        var clavesLpnReferencia = string.Empty;

                        foreach (var lpn in lpns)
                        {
                            clavesLpnReferencia += lpn.NumeroLPN + " - ";
                        }

                        var estiloTemplate = new EstiloTemplate(uow, estilo);
                        var strategy = new LpnImpresionStrategy(estiloTemplate, lpns, uow, _printingService);
                        var builder = new ImpresionBuilder(uow.ImpresoraRepository.GetImpresora(impresora, predio), strategy, _printingService);

                        var impresion = builder.GenerarCabezal(this._identity.UserId, predio)
                            .GenerarDetalle()
                            .Build();

                        impresion.Referencia = string.Format("Cant. Lpns: {0}", lpns.Count());

                        numImpresion = uow.ImpresionRepository.Add(impresion);

                        uow.SaveChanges();

                        int iterador = 1;
                        int cantidadRegistros = 1;

                        var detalleImpresionInsercion = new DetalleImpresion()
                        {
                            NumeroImpresion = numImpresion,
                            FechaProcesado = DateTime.Now,
                            Estado = _printingService.GetEstadoInicial(),
                        };

                        foreach (var detalle in impresion.Detalles.OrderBy(s => s.NumeroImpresion))
                        {
                            for (int i = 0; i < cantidadCopias; i++)
                            {
                                detalleImpresionInsercion.Contenido += detalle.Contenido + "\n";

                                if (detalleImpresionInsercion.Contenido.Length > 2000)
                                {
                                    detalleImpresionInsercion.Registro = cantidadRegistros;

                                    uow.ImpresionRepository.AddDetalleImpresion(detalleImpresionInsercion);

                                    cantidadRegistros++;
                                    detalleImpresionInsercion.Contenido = string.Empty;
                                }
                                else if (cantidadCopias == iterador)
                                {
                                    detalleImpresionInsercion.Registro = cantidadRegistros;

                                    uow.ImpresionRepository.AddDetalleImpresion(detalleImpresionInsercion);

                                    cantidadRegistros++;
                                    detalleImpresionInsercion.Contenido = string.Empty;
                                }

                                iterador++;
                            }

                            iterador = 1;
                        }

                        uow.ImpresionRepository.ActualizarImpresion(numImpresion, cantidadRegistros - 1);

                        uow.SaveChanges();

                        context.AddSuccessNotification("IMP050_Sec0_error_ImpresoCorrectamete");
                    }
                }

                uow.Commit();

                _printingService.SendToPrint(numImpresion);

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ExpectedException ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message);
            }
            catch (Exception ex)
            {
                uow.Rollback();
                this._logger.LogError(ex, "STO700GridCommit");
                context.AddErrorNotification("General_Sec0_Error_Operacion_Reintente");
            }
            finally
            {
                uow.EndTransaction();
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new STO700CreacionLPNValidationModule(uow, this._identity.UserId, this._identity.Predio), form, context);
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "tipo":
                    return SearchTiposLPN(form, context);

                case "empresa":
                    return SearchEmpresa(form, context);

                default:
                    return new List<SelectOption>();
            }
        }

        #region Auxs

        public virtual void InicializarSelect(IUnitOfWork uow, Form form)
        {
            var selectorImpresora = form.GetField("impresora");
            var selectorPredio = form.GetField("predio");

            selectorImpresora.Options = new List<SelectOption>();
            selectorPredio.Options = new List<SelectOption>();

            var prediosUsuario = uow.PredioRepository.GetPrediosUsuario(this._identity.UserId);
            foreach (var predio in prediosUsuario)
            {
                selectorPredio.Options.Add(new SelectOption(predio.Numero, predio.Numero));
            }

            //Impresora
            List<Impresora> impresoras;
            if (this._identity.Predio.Equals(GeneralDb.PredioSinDefinir))
                impresoras = uow.ImpresoraRepository.GetListaImpresorasPredio(form.GetField("predio").Value);
            else
            {
                impresoras = uow.ImpresoraRepository.GetListaImpresorasPredio(this._identity.Predio);
                form.GetField("impresora").ReadOnly = false;
            }

            foreach (var impresora in impresoras)
            {
                selectorImpresora.Options.Add(new SelectOption(impresora.Id, $"{impresora.Id} - {impresora.Descripcion}"));
            }
        }

        public virtual List<SelectOption> SearchTiposLPN(Form form, FormSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var tipos = uow.ManejoLpnRepository.GetAllTipoLPNByDescriptionOrCodePartial(context.SearchValue);

                foreach (var tipo in tipos)
                {
                    if (tipo.PermiteGenerar == "S")
                        opciones.Add(new SelectOption(tipo.Tipo, $"{tipo.Tipo} - {tipo.Descripcion}"));
                }
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var empresas = uow.EmpresaRepository.GetByNombreOrCodePartialForUsuario(context.SearchValue, this._identity.UserId);

                foreach (var empresa in empresas)
                {
                    opciones.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}"));
                }
            }

            return opciones;
        }

        public virtual List<Lpn> GenerarLpns(IUnitOfWork uow, Form form, LpnTipo tipoLpn)
        {
            var cantidadGenerar = int.Parse(form.GetField("cantidadLPN").Value);
            var numeroSecuencia = uow.ManejoLpnRepository.GetNumeroSequenciaTipoLpn(tipoLpn.Tipo);

            var empresa = int.Parse(form.GetField("empresa").Value);
            var colParams = new Dictionary<string, string>
            {
                [ParamManager.PARAM_EMPR] = $"{ParamManager.PARAM_EMPR}_{empresa}"
            };
            var generaCodBarrasIdExterno = (uow.ParametroRepository.GetParameter(ParamManager.GENERAR_CB_ID_EXTERNO_LPN, colParams))?.ToUpper() == "S";

            if (numeroSecuencia == null)
                numeroSecuencia = 0;

            var lpns = new List<Lpn>();

            for (int i = 0; i < cantidadGenerar; i++)
            {
                numeroSecuencia = (numeroSecuencia ?? 0) + 1;

                while (true)
                {
                    if (uow.ManejoLpnRepository.ExisteTipoLpnExterno(tipoLpn.Prefijo + (numeroSecuencia).ToString(), tipoLpn.Tipo))
                        numeroSecuencia++;
                    else
                        break;
                }

                var lpn = new Lpn
                {
                    FechaAdicion = DateTime.Now,
                    Empresa = int.Parse(form.GetField("empresa").Value),
                    Tipo = tipoLpn.Tipo,
                    Estado = EstadosLPN.Generado,
                    IdExterno = tipoLpn.Prefijo + (numeroSecuencia).ToString(),
                    NumeroTransaccion = uow.GetTransactionNumber(),
                    IdPacking = form.GetField("packingList").Value
                };

                uow.ManejoLpnRepository.AddLPN(lpn);
                lpns.Add(lpn);

                var codigoBarras = lpn.IdExterno.Substring(tipoLpn.Prefijo.Length).ToString().PadLeft(15, '0');
                int sumaCaracteres = 0;

                foreach (var digito in codigoBarras)
                {
                    sumaCaracteres += int.Parse(digito.ToString());
                }

                var mod = sumaCaracteres % DefaultDb.ModuloDigitoVerificacion;

                var lpnBarra = new LpnBarras
                {
                    IdLpnBarras = uow.ManejoLpnRepository.GetNextLpnBarras(),
                    NumeroLpn = lpn.NumeroLPN,
                    Tipo = BarcodeDb.TIPO_LPN_CB,
                    CodigoBarras = tipoLpn.Prefijo + codigoBarras + mod.ToString(),
                    Orden = 0
                };

                uow.ManejoLpnRepository.AddLPNBarras(lpnBarra);

                if (generaCodBarrasIdExterno)
                {
                    var lpnBarraIdExterno = new LpnBarras
                    {
                        IdLpnBarras = uow.ManejoLpnRepository.GetNextLpnBarras(),
                        NumeroLpn = lpn.NumeroLPN,
                        Tipo = BarcodeDb.TIPO_LPN_CB,
                        CodigoBarras = lpn.IdExterno,
                        Orden = 1
                    };

                    uow.ManejoLpnRepository.AddLPNBarras(lpnBarraIdExterno);
                }
            }

            tipoLpn.NumeroSecuencia = numeroSecuencia;
            uow.ManejoLpnRepository.UpdateTipoLpn(tipoLpn);

            return lpns;
        }

        public virtual void GenerarAtributosLpns(IUnitOfWork uow, LpnTipo tipo, List<Lpn> lpns)
        {
            var atributosTipo = uow.ManejoLpnRepository.GetTipoAsociadoAtributo(tipo.Tipo);

            for (int i = 0; i < lpns.Count; i++)
            {
                foreach (var atr in atributosTipo)
                {
                    var atributo = new LpnAtributo();

                    atributo.NumeroTransaccion = uow.GetTransactionNumber();
                    atributo.Valor = atr.ValorInicial;
                    atributo.NumeroLpn = lpns[i].NumeroLPN;
                    atributo.Tipo = atr.TipoLpn;
                    atributo.Id = atr.IdAtributo;
                    atributo.Estado = atr.EstadoInicial;

                    uow.ManejoLpnRepository.AddAtributoAsociado(atributo);
                }
            }
        }

        #endregion
    }
}
