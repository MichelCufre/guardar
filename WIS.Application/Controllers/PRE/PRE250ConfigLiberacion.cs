
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.CheckboxListComponent;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.Configuracion;
using WIS.Domain.Liberacion;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.PRE
{
    public class PRE250ConfigLiberacion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;

        public PRE250ConfigLiberacion(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService)
        {
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._formValidationService = formValidationService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            Empresa empresa = uow.EmpresaRepository.GetEmpresaUnicaParaUsuario(this._identity.UserId);

            if (context.Parameters.Count > 0)
            {
                if (int.TryParse(context.Parameters.Find(x => x.Id == "nuRegla")?.Value, out int nuRegla))
                    this.InicializarSelectsUpdate(uow, form, context, nuRegla);
                else
                    this.InicializarSelects(uow, form, empresa == null ? string.Empty : empresa.Id.ToString(), context);
            }
            else
                this.InicializarSelects(uow, form, empresa == null ? string.Empty : empresa.Id.ToString(), context);

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            if (context.ButtonId == "btnSubmitConfirmarConf")
            {
                using var uow = this._uowFactory.GetUnitOfWork();
                uow.BeginTransaction();

                try
                {
                    ReglaLiberacion regla = null;

                    if (int.TryParse(context.Parameters.Find(x => x.Id == "nuRegla")?.Value, out int nuRegla))
                        regla = uow.LiberacionRepository.GetReglaLiberacion(nuRegla, true);

                    if (regla == null)
                        CrearRegla(uow, nuRegla, context.Parameters);
                    else
                        UpdateRegla(uow, regla.NuRegla, regla.ValorVidaUtil, context.Parameters);

                    uow.SaveChanges();
                    uow.Commit();

                    context.Parameters?.Clear();
                    if (regla == null)
                        context.AddSuccessNotification("PRE250_msg_Sucess_ReglaCreada");
                    else
                        context.AddSuccessNotification("PRE250_msg_Sucess_ReglaEditada");

                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    throw;
                }
            }
            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            return this._formValidationService.Validate(new PRE250LiberacionValidationModule(uow, this._identity.UserId, this._identity.GetFormatProvider()), form, context);
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "empresa": return this.SearchEmpresa(form, context);
                default: return new List<SelectOption>();
            }
        }

        #region Metodos auxiliares

        protected virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext FormQuery)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Empresa> empresas = uow.EmpresaRepository.GetByNombreOrCodePartial(FormQuery.SearchValue);

            foreach (var empresa in empresas)
            {
                opciones.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}"));
            }

            return opciones;
        }


        public virtual void InicializarSelects(IUnitOfWork uow, Form form, string empresa, FormInitializeContext query)
        {
            form.GetField("clientesPorPrep").Value = string.Empty;
            form.GetField("pedidosNuevos").Value = "false";

            //predios
            var selectPredio = form.GetField("predio");
            selectPredio.Options = new List<SelectOption>
            {
                new SelectOption("", "Pedidos sin predio")
            };

            var predios = uow.PredioRepository.GetPrediosUsuario(this._identity.UserId).OrderBy(s => s.Numero);

            foreach (var p in predios)
            {
                selectPredio.Options.Add(new SelectOption(p.Numero, $"{p.Numero} - {p.Descripcion}"));
            }
            selectPredio.Value = string.Empty;

            if (predios.Count() == 1)
                selectPredio.Value = predios.FirstOrDefault().Numero;

            if (this._identity.Predio != GeneralDb.PredioSinDefinir)
                selectPredio.Value = this._identity.Predio;

            //onda
            var fieldOnda = form.GetField("onda");
            fieldOnda.Options = new List<SelectOption>();

            foreach (var onda in uow.OndaRepository.GetOndasActivas(this._identity.Predio).OrderBy(s => s.Id))
            {
                fieldOnda.Options.Add(new SelectOption(onda.Id.ToString(), $"{onda.Id} - {onda.Descripcion}"));
            }
            fieldOnda.Value = string.Empty;


            var tpAgente = form.GetField("tpAgente");
            tpAgente.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibAutomaticaTipoAgente)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            tpAgente.Value = CodigoDominioDb.SelectLibAutomaticaTipoAgenteTodos;

            var orden = form.GetField("ordenPedidosAuto");
            orden.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibAutomaticaOrdenPedidos)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            orden.Value = TipoAgenteDb.Cliente;

            ////Inicializar selects

            var liberaConf = uow.PreparacionRepository.GetLiberacionConfiguracion(empresa);

            //UbicacionCompleta
            var selectUbicacionCompleta = form.GetField("ubicacionCompleta");
            selectUbicacionCompleta.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaPalletCompleto)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            selectUbicacionCompleta.ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.UbicacionCompletaHabilitado;
            selectUbicacionCompleta.Value = liberaConf.UbicacionCompleta;

            //UbicacionIncompleta
            var selectUbicacionIncompleta = form.GetField("ubicacionIncompleta");
            selectUbicacionIncompleta.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaPalletIncompleto)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            selectUbicacionIncompleta.ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.UbicacionIncompletaHabilitado;
            selectUbicacionIncompleta.Value = liberaConf.UbicacionIncompleta;

            //Prepa Solo camion
            var selectPrepSoloCam = form.GetField("prepSoloCamion");
            selectPrepSoloCam.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            selectPrepSoloCam.ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.PrepSoloCamionHabilitado;
            selectPrepSoloCam.Value = liberaConf.PrepSoloCamion;

            //Agrup Por camion
            var selectAgrupPorCamion = form.GetField("agrupPorCamion");
            selectAgrupPorCamion.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            selectAgrupPorCamion.ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.AgruparCamionHabilitado;
            selectAgrupPorCamion.Value = liberaConf.AgruparCamion;

            //liberarPorUnidades
            var liberarPorUnidades = form.GetField("liberarPorUnidades");
            liberarPorUnidades.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            liberarPorUnidades.ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.LiberarPorUnidadHabilitado;
            liberarPorUnidades.Value = liberaConf.LiberarPorUnidad;

            //liberarPorCurvas
            var liberarPorCurvas = form.GetField("liberarPorCurvas");
            liberarPorCurvas.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            liberarPorCurvas.ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.LiberarPorCurvasHabilitado;
            liberarPorCurvas.Value = liberaConf.LiberarPorCurvas;

            //Maneja vida util
            var manejaVidaUtil = form.GetField("manejaVidaUtil");
            manejaVidaUtil.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            manejaVidaUtil.ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.ManejaVidaUtilHabilitado;
            manejaVidaUtil.Value = liberaConf.ManejaVidaUtil;

            //Agrupador
            var agrupacion = form.GetField("agrupacion");
            agrupacion.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.Agrupacion)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            agrupacion.ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.ManejoAgrupadorHabilitado;
            agrupacion.Value = liberaConf.ManejoAgrupador;

            //Stock
            var stock = form.GetField("stock");
            stock.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaStock)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            stock.ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.DefaultStockHabilitado;
            stock.Value = liberaConf.DefaultStock;

            //pedidos
            var pedidos = form.GetField("pedidos");
            pedidos.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaPedidos)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            pedidos.ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.PedidosHabilitado;
            pedidos.Value = liberaConf.Pedidos;

            //Repartir escasez
            var repartirEscasez = form.GetField("repartirEscasez");
            repartirEscasez.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            repartirEscasez.ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.RepartirEscazesHabilitado;
            repartirEscasez.Value = liberaConf.RepartirEscazes;

            //RespetaFifo
            var respetaFifo = form.GetField("respetaFifo");
            respetaFifo.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            respetaFifo.ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.RespetaFifoHabilitado;
            respetaFifo.Value = liberaConf.RespetaFifo;

            //Excluir ubicaciones de picking
            if (liberaConf.ManejaVidaUtil == "S")
            {
                form.GetField("excluirUbicPicking").Value = "true";
                form.GetField("excluirUbicPicking").ReadOnly = true;
                form.GetField("excluirUbicPicking").Disabled = true;

                form.GetField("usarSoloStkPicking").Value = "false";
                form.GetField("usarSoloStkPicking").ReadOnly = true;
                form.GetField("usarSoloStkPicking").Disabled = true;
            }
            else
            {
                form.GetField("excluirUbicPicking").Value = liberaConf.ExcluirUbicacionesPicking == "S" ? "true" : "false";
                form.GetField("excluirUbicPicking").ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.ExcluirUbicacionesPickingHabilitado;
                form.GetField("excluirUbicPicking").Disabled = string.IsNullOrEmpty(empresa) ? true : !liberaConf.ExcluirUbicacionesPickingHabilitado;

                form.GetField("usarSoloStkPicking").Value = liberaConf.UsarSoloStkPicking == "S" && liberaConf.ExcluirUbicacionesPicking != "S" ? "true" : "false";
                form.GetField("usarSoloStkPicking").ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.UsarSoloStkPickingHabilitado;
                form.GetField("usarSoloStkPicking").Disabled = string.IsNullOrEmpty(empresa) ? true : !liberaConf.UsarSoloStkPickingHabilitado;
            }

            //PriorizarDesborde
            var priorizarDesborde = form.GetField("priorizarDesborde");
            priorizarDesborde.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            priorizarDesborde.ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.PriorizarDesbordeHabilitado;
            priorizarDesborde.Value = liberaConf.PriorizarDesborde;

            //stockDMTI
            var stockDtmi = form.GetField("stockDtmi");
            stockDtmi.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            stockDtmi.ReadOnly = string.IsNullOrEmpty(empresa) ? true : (liberaConf.ManejoDocumental || !liberaConf.ControlStockDMTIHabilitado);
            stockDtmi.Value = liberaConf.ManejoDocumental ? "S" : liberaConf.ControlStockDMTI;


            //Item list condicion
            List<CheckboxListItem> checkListCondicion = new List<CheckboxListItem>();
            foreach (var condLib in uow.LiberacionRepository.GetCondicionLiberaciones())
            {
                checkListCondicion.Add(new CheckboxListItem
                {
                    Id = condLib.Condicion,
                    Label = condLib.Descripcion,
                    Selected = condLib.MostrarMarcada
                });
            }
            query.AddParameter("ListItemsCondicion", JsonConvert.SerializeObject(checkListCondicion));


            //Item list tpExpedicion
            List<CheckboxListItem> checkListTpExpedicion = new List<CheckboxListItem>();
            foreach (var tpExp in uow.PedidoRepository.GetConfiguracionesExpedicion())
            {
                if (tpExp.Tipo == TipoExpedicion.ReservasPrepManual)
                    continue;

                checkListTpExpedicion.Add(new CheckboxListItem
                {
                    Id = tpExp.Tipo,
                    Label = tpExp.Tipo + " - " + tpExp.Descripcion
                });
            }
            query.AddParameter("ListItemsTpExpedicion", JsonConvert.SerializeObject(checkListTpExpedicion));


            //Item list TpPedido
            List<CheckboxListItem> checkListTpPedido = new List<CheckboxListItem>();
            foreach (var tpPed in uow.PedidoRepository.GetTiposPedido())
            {
                if (tpPed.Key == TipoPedidoDb.Reserva)
                    continue;

                checkListTpPedido.Add(new CheckboxListItem
                {
                    Id = tpPed.Key,
                    Label = tpPed.Key + " - " + tpPed.Value

                });
            }
            query.AddParameter("ListItemsTpPedido", JsonConvert.SerializeObject(checkListTpPedido));
            query.AddParameter("emp", "");

            //Día cola de trabajo
            var diasColaTrabajo = form.GetField("diasColaTrabajo");
            diasColaTrabajo.Options = new List<SelectOption>
            {
                new SelectOption("1","-"),
                new SelectOption("2","Lunes"),
                new SelectOption("3", "Martes"),
                new SelectOption("4", "Miércoles"),
                new SelectOption("5", "Jueves"),
                new SelectOption("6", "Viernes"),
                new SelectOption("7", "Sábado"),
                new SelectOption("8", "Domingo"),
            };

        }

        public virtual void InicializarSelectsUpdate(IUnitOfWork uow, Form form, FormInitializeContext query, int nuRegla)
        {
            var regla = uow.LiberacionRepository.GetReglaLiberacion(nuRegla, true);
            LiberacionConfiguracion liberaConf = uow.PreparacionRepository.GetLiberacionConfiguracion(regla.CdEmpresa.ToString());

            form.GetField("clientesPorPrep").Value = (regla.NuClisPorPreparacion ?? 0).ToString();
            form.GetField("pedidosNuevos").Value = regla.FlSoloPedidosNuevos ? "true" : "false";

            //predio
            var predio = form.GetField("predio");
            predio.Options = new List<SelectOption>
            {
                new SelectOption("", "Pedidos sin predio")
            };

            foreach (var p in uow.PredioRepository.GetPrediosUsuario(this._identity.UserId).OrderBy(s => s.Numero))
            {
                predio.Options.Add(new SelectOption(p.Numero, p.Numero));
            }
            predio.Value = regla.NuPredio;

            //onda
            var fieldOnda = form.GetField("onda");
            fieldOnda.Options = new List<SelectOption>();

            foreach (var onda in uow.OndaRepository.GetOndasActivas(regla.NuPredio).OrderBy(s => s.Id))
            {
                fieldOnda.Options.Add(new SelectOption(onda.Id.ToString(), $"{onda.Id} - {onda.Descripcion}"));
            }
            fieldOnda.Value = regla.CdOnda.ToString();

            //tpAgente
            var tpAgente = form.GetField("tpAgente");
            tpAgente.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibAutomaticaTipoAgente)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            tpAgente.Value = string.IsNullOrEmpty(regla.TpAgente) ? CodigoDominioDb.SelectLibAutomaticaTipoAgenteTodos : regla.TpAgente;

            //ordenPedidos
            var orden = form.GetField("ordenPedidosAuto");
            orden.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibAutomaticaOrdenPedidos)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            orden.Value = regla.CdOrdenPedidosAuto;

            //UbicacionCompleta
            var selectUbicacionCompleta = form.GetField("ubicacionCompleta");
            selectUbicacionCompleta.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaPalletCompleto)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            selectUbicacionCompleta.ReadOnly = !liberaConf.UbicacionCompletaHabilitado;

            if (liberaConf.UbicacionCompletaHabilitado)
                selectUbicacionCompleta.Value = regla.CdPalletCompeto;
            else
                selectUbicacionCompleta.Value = liberaConf.UbicacionCompleta;

            //UbicacionIncompleta
            var selectUbicacionIncompleta = form.GetField("ubicacionIncompleta");
            selectUbicacionIncompleta.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaPalletIncompleto)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            selectUbicacionIncompleta.ReadOnly = !liberaConf.UbicacionIncompletaHabilitado;

            if (liberaConf.UbicacionIncompletaHabilitado)
                selectUbicacionIncompleta.Value = regla.CdpalletIncompleto;
            else
                selectUbicacionIncompleta.Value = liberaConf.UbicacionIncompleta;

            //Prepa Solo camion
            var selectPrepSoloCam = form.GetField("prepSoloCamion");
            selectPrepSoloCam.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            selectPrepSoloCam.ReadOnly = !liberaConf.PrepSoloCamionHabilitado;

            if (liberaConf.PrepSoloCamionHabilitado)
                selectPrepSoloCam.Value = regla.CdPrepararSoloCamion;
            else
                selectPrepSoloCam.Value = liberaConf.PrepSoloCamion;


            //Agrup Por camion
            var selectAgrupPorCamion = form.GetField("agrupPorCamion");
            selectAgrupPorCamion.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            selectAgrupPorCamion.ReadOnly = !liberaConf.AgruparCamionHabilitado;

            if (liberaConf.AgruparCamionHabilitado)
                selectAgrupPorCamion.Value = regla.CdAgruparPorCamion;
            else
                selectAgrupPorCamion.Value = liberaConf.AgruparCamion;

            //liberarPorUnidades
            var liberarPorUnidades = form.GetField("liberarPorUnidades");
            liberarPorUnidades.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            liberarPorUnidades.ReadOnly = !liberaConf.LiberarPorUnidadHabilitado;

            if (liberaConf.LiberarPorUnidadHabilitado)
                liberarPorUnidades.Value = regla.CdLiberarPorUnidad;
            else
                liberarPorUnidades.Value = liberaConf.LiberarPorUnidad;

            //liberarPorCurvas
            var liberarPorCurvas = form.GetField("liberarPorCurvas");
            liberarPorCurvas.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            liberarPorCurvas.ReadOnly = !liberaConf.LiberarPorCurvasHabilitado;

            if (liberaConf.LiberarPorCurvasHabilitado)
                liberarPorCurvas.Value = regla.CdLiberarPorCurvas;
            else
                liberarPorCurvas.Value = liberaConf.LiberarPorCurvas;

            //Maneja vida util
            var manejaVidaUtil = form.GetField("manejaVidaUtil");
            manejaVidaUtil.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            manejaVidaUtil.ReadOnly = !liberaConf.ManejaVidaUtilHabilitado;

            if (liberaConf.ManejaVidaUtilHabilitado)
                manejaVidaUtil.Value = regla.ManejaVidaUtil ? "S" : "N";
            else
                manejaVidaUtil.Value = liberaConf.ManejaVidaUtil;

            //Agrupador
            var agrupacion = form.GetField("agrupacion");
            agrupacion.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.Agrupacion)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            agrupacion.ReadOnly = !liberaConf.ManejoAgrupadorHabilitado;

            if (liberaConf.ManejoAgrupadorHabilitado)
                agrupacion.Value = regla.CdAgrupacion;
            else
                agrupacion.Value = liberaConf.ManejoAgrupador;

            //Stock
            var stock = form.GetField("stock");
            stock.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaStock)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            stock.ReadOnly = !liberaConf.DefaultStockHabilitado;

            if (liberaConf.DefaultStockHabilitado)
                stock.Value = regla.CdStock;
            else
                stock.Value = liberaConf.DefaultStock;

            //pedidos
            var pedidos = form.GetField("pedidos");
            pedidos.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaPedidos)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            pedidos.ReadOnly = !liberaConf.PedidosHabilitado;

            if (liberaConf.PedidosHabilitado)
                pedidos.Value = regla.CdOrdenPedidos;
            else
                pedidos.Value = liberaConf.Pedidos;

            //Repartir escasez
            var repartirEscasez = form.GetField("repartirEscasez");
            repartirEscasez.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            repartirEscasez.ReadOnly = !liberaConf.RepartirEscazesHabilitado;

            if (liberaConf.RepartirEscazesHabilitado)
                repartirEscasez.Value = regla.CdRepartirEscasez;
            else
                repartirEscasez.Value = liberaConf.RepartirEscazes;

            //RespetaFifo
            var respetaFifo = form.GetField("respetaFifo");
            respetaFifo.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            respetaFifo.ReadOnly = !liberaConf.RespetaFifoHabilitado;

            if (liberaConf.RespetaFifoHabilitado)
                respetaFifo.Value = regla.CdRespetarFifo;
            else
                respetaFifo.Value = liberaConf.RespetaFifo;

            //Excluir ubicaciones de picking
            if (liberaConf.ManejaVidaUtil == "S")
            {
                form.GetField("excluirUbicPicking").Value = "true";
                form.GetField("excluirUbicPicking").ReadOnly = true;
                form.GetField("excluirUbicPicking").Disabled = true;

                form.GetField("usarSoloStkPicking").Value = "false";
                form.GetField("usarSoloStkPicking").ReadOnly = true;
                form.GetField("usarSoloStkPicking").Disabled = true;
            }
            else
            {
                var excluirUbicPicking = liberaConf.ExcluirUbicacionesPicking == "S";

                form.GetField("excluirUbicPicking").ReadOnly = !liberaConf.ExcluirUbicacionesPickingHabilitado;
                form.GetField("excluirUbicPicking").Disabled = !liberaConf.ExcluirUbicacionesPickingHabilitado;

                if (liberaConf.ExcluirUbicacionesPickingHabilitado)
                    excluirUbicPicking = regla.ExcluirUbicacionesPicking;

                form.GetField("excluirUbicPicking").Value = excluirUbicPicking ? "true" : "false";

                form.GetField("usarSoloStkPicking").ReadOnly = !liberaConf.UsarSoloStkPickingHabilitado;
                form.GetField("usarSoloStkPicking").Disabled = !liberaConf.UsarSoloStkPickingHabilitado;

                var usarSoloStkPicking = liberaConf.UsarSoloStkPicking == "S";

                if (liberaConf.UsarSoloStkPickingHabilitado)
                    usarSoloStkPicking = regla.UsarSoloStkPicking;

                form.GetField("usarSoloStkPicking").Value = usarSoloStkPicking && !excluirUbicPicking ? "true" : "false";
            }

            //PriorizarDesborde
            var priorizarDesborde = form.GetField("priorizarDesborde");
            priorizarDesborde.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            priorizarDesborde.ReadOnly = !liberaConf.PriorizarDesbordeHabilitado;

            if (liberaConf.PriorizarDesbordeHabilitado)
                priorizarDesborde.Value = regla.PriozarDesborde ? "S" : "N";
            else
                priorizarDesborde.Value = liberaConf.PriorizarDesborde;


            //stockDMTI
            var stockDtmi = form.GetField("stockDtmi");
            stockDtmi.Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();
            stockDtmi.ReadOnly = liberaConf.ManejoDocumental || !liberaConf.ControlStockDMTIHabilitado;

            if (liberaConf.ControlStockDMTIHabilitado)
                stockDtmi.Value = liberaConf.ManejoDocumental ? "S" : regla.CdControlaStock;
            else
                stockDtmi.Value = liberaConf.ManejoDocumental ? "S" : liberaConf.ControlStockDMTI;

            //Item list condicion
            query.AddParameter("ListItemsCondicion", JsonConvert.SerializeObject(CargarCondiciones(uow, regla)));

            //Item list condicion
            query.AddParameter("ListItemsTpExpedicion", JsonConvert.SerializeObject(CargarTpExpedicion(uow, regla)));

            //Item list condicion
            query.AddParameter("ListItemsTpPedido", JsonConvert.SerializeObject(CargarTpPedido(uow, regla)));

            //empresa
            var fieldEmpresa = form.GetField("empresa");
            fieldEmpresa.Options = SearchEmpresa(form, new FormSelectSearchContext()
            {
                SearchValue = regla.CdEmpresa.ToString()
            });
            fieldEmpresa.Value = regla.CdEmpresa.ToString();
            query.AddParameter("emp", regla.CdEmpresa.ToString());

            //Día cola de trabajo
            var diasColaTrabajo = form.GetField("diasColaTrabajo");
            diasColaTrabajo.Options = new List<SelectOption>
            {
                new SelectOption("1","-"),
                new SelectOption("2","Lunes"),
                new SelectOption("3", "Martes"),
                new SelectOption("4", "Miércoles"),
                new SelectOption("5", "Jueves"),
                new SelectOption("6", "Viernes"),
                new SelectOption("7", "Sábado"),
                new SelectOption("8", "Domingo"),
            };

            diasColaTrabajo.Value = regla.NuDiasColaTrabajo.ToString();
        }

        public virtual List<CheckboxListItem> CargarCondiciones(IUnitOfWork uow, ReglaLiberacion regla)
        {
            List<CheckboxListItem> checkListCondiciones = new List<CheckboxListItem>();
            var listCdCond = uow.LiberacionRepository.GetCdCondLiberacionRegla(regla.LstReglaCondicionLiberacion);

            foreach (var condLib in uow.LiberacionRepository.GetCondicionLiberaciones())
            {
                checkListCondiciones.Add(new CheckboxListItem
                {
                    Id = condLib.Condicion,
                    Label = condLib.Descripcion,
                    Selected = listCdCond.Contains(condLib.Condicion) ? true : false
                });
            }
            return checkListCondiciones;
        }
        public virtual List<CheckboxListItem> CargarTpExpedicion(IUnitOfWork uow, ReglaLiberacion regla)
        {
            List<CheckboxListItem> checkListTpExpedicion = new List<CheckboxListItem>();
            var listTpExp = uow.LiberacionRepository.GetCdTpExpedicionRegla(regla.LstReglaTipoExpedicion);

            foreach (var tpExp in uow.PedidoRepository.GetConfiguracionesExpedicion())
            {
                if (tpExp.Tipo == TipoExpedicion.ReservasPrepManual)
                    continue;

                checkListTpExpedicion.Add(new CheckboxListItem
                {
                    Id = tpExp.Tipo,
                    Label = tpExp.Tipo + " - " + tpExp.Descripcion,
                    Selected = listTpExp.Contains(tpExp.Tipo) ? true : false
                });
            }
            return checkListTpExpedicion;
        }
        public virtual List<CheckboxListItem> CargarTpPedido(IUnitOfWork uow, ReglaLiberacion regla)
        {
            List<CheckboxListItem> checkListTpPedido = new List<CheckboxListItem>();
            var listTpPed = uow.LiberacionRepository.GetCdTpPedidoRegla(regla.LstReglaTipoPedido);

            foreach (var tpPed in uow.PedidoRepository.GetTiposPedido())
            {
                if (tpPed.Key == TipoPedidoDb.Reserva)
                    continue;

                checkListTpPedido.Add(new CheckboxListItem
                {
                    Id = tpPed.Key,
                    Label = tpPed.Key + " - " + tpPed.Value,
                    Selected = listTpPed.Contains(tpPed.Key) ? true : false
                });
            }
            return checkListTpPedido;
        }

        protected virtual void CrearRegla(IUnitOfWork uow, int nuRegla, List<ComponentParameter> parameters)
        {
            var regla = MapReglaLiberacion(uow.LiberacionRepository.GetNextNuReglaLiberacion(), parameters);

            uow.LiberacionRepository.AddReglaLiberacion(regla);

            foreach (var x in regla.LstReglaCondicionLiberacion)
                uow.LiberacionRepository.AddReglaCondicionLiberacion(x);
            foreach (var x in regla.LstReglaTipoPedido)
                uow.LiberacionRepository.AddReglaTipoPedido(x);
            foreach (var x in regla.LstReglaTipoExpedicion)
                uow.LiberacionRepository.AddReglaTipoExpedicion(x);
        }
        protected virtual void UpdateRegla(IUnitOfWork uow, int nuRegla, short? ValorVidaUtil, List<ComponentParameter> parameters)
        {

            var regla = MapReglaLiberacion(nuRegla, parameters, ValorVidaUtil);

            var se = regla.LstReglaCliente;
            foreach (var c in uow.LiberacionRepository.GetReglaClientes(regla.NuRegla).Where(s => s.NuOrden != null))
            {
                var d = regla.LstReglaCliente.FirstOrDefault(s => s.Cliente == c.Cliente && s.Empesa == c.Empesa);
                if (d != null)
                    d.NuOrden = c.NuOrden;
            }
            uow.LiberacionRepository.UpdateReglaLiberacion(regla);
            uow.LiberacionRepository.DeleteReglaCondicionLiberacionByRegla(nuRegla);
            uow.LiberacionRepository.DeleteReglaTipoExpedicionByRegla(nuRegla);
            uow.LiberacionRepository.DeleteReglaTipoPedidoByRegla(nuRegla);

            foreach (var x in regla.LstReglaCondicionLiberacion)
                uow.LiberacionRepository.AddReglaCondicionLiberacion(x);
            foreach (var x in regla.LstReglaTipoPedido)
                uow.LiberacionRepository.AddReglaTipoPedido(x);
            foreach (var x in regla.LstReglaTipoExpedicion)
                uow.LiberacionRepository.AddReglaTipoExpedicion(x);
        }

        protected virtual ReglaLiberacion MapReglaLiberacion(int nuRegla, List<ComponentParameter> parameters, short? ValorVidaUtil = null)
        {
            DateTime? dtInicio = DateTimeExtension.ParseFromIso(parameters.FirstOrDefault(s => s.Id == "dtInicio")?.Value)?.Date;
            DateTime? dtFin = DateTimeExtension.ParseFromIso(parameters.FirstOrDefault(s => s.Id == "dtFin")?.Value)?.Date;

            var diaColaTrabajo = short.TryParse(parameters.FirstOrDefault(s => s.Id == "diasColaTrabajo").Value, out short d) ? d : (short)1;

            var regla = new ReglaLiberacion()
            {
                //formRegla
                NuRegla = nuRegla,
                DsRegla = parameters.FirstOrDefault(s => s.Id == "descripcion")?.Value,
                NuOrden = short.Parse(parameters.FirstOrDefault(s => s.Id == "nuOrden")?.Value),
                DtInicio = dtInicio,
                DtFin = dtFin,
                HrInicio = null,
                HrFin = null,
                NuFrecuencia = null,
                TpFrecuencia = parameters.FirstOrDefault(s => s.Id == "tpFrecuencia")?.Value,
                FlActiva = parameters.FirstOrDefault(s => s.Id == "activa")?.Value == "true" ? true : false,
                RespetarIntervalo = parameters.FirstOrDefault(s => s.Id == "respetarIntervalos")?.Value == "true" ? true : false,

                //formLiberacion
                CdPalletCompeto = parameters.FirstOrDefault(s => s.Id == "ubicacionCompleta")?.Value,
                CdpalletIncompleto = parameters.FirstOrDefault(s => s.Id == "ubicacionIncompleta")?.Value,
                CdPrepararSoloCamion = parameters.FirstOrDefault(s => s.Id == "prepSoloCamion")?.Value,
                CdAgruparPorCamion = parameters.FirstOrDefault(s => s.Id == "agrupPorCamion")?.Value,
                CdLiberarPorUnidad = parameters.FirstOrDefault(s => s.Id == "liberarPorUnidades")?.Value,
                CdLiberarPorCurvas = parameters.FirstOrDefault(s => s.Id == "liberarPorCurvas")?.Value,
                ManejaVidaUtil = parameters.FirstOrDefault(s => s.Id == "manejaVidaUtil")?.Value == "S" ? true : false,
                ValorVidaUtil = ValorVidaUtil,
                CdAgrupacion = parameters.FirstOrDefault(s => s.Id == "agrupacion")?.Value,
                CdStock = parameters.FirstOrDefault(s => s.Id == "stock")?.Value,
                CdOrdenPedidos = parameters.FirstOrDefault(s => s.Id == "pedidos")?.Value,
                CdRepartirEscasez = parameters.FirstOrDefault(s => s.Id == "repartirEscasez")?.Value,
                CdRespetarFifo = parameters.FirstOrDefault(s => s.Id == "respetaFifo")?.Value,
                PriozarDesborde = parameters.FirstOrDefault(s => s.Id == "priorizarDesborde")?.Value == "S" ? true : false,
                ExcluirUbicacionesPicking = parameters.Find(x => x.Id == "excluirUbicPicking")?.Value == "true" ? true : false,
                UsarSoloStkPicking = parameters.Find(x => x.Id == "usarSoloStkPicking")?.Value == "true" ? true : false,
                CdControlaStock = parameters.FirstOrDefault(s => s.Id == "stockDtmi")?.Value,

                //formFiltros
                CdEmpresa = int.Parse(parameters.FirstOrDefault(s => s.Id == "empresa")?.Value),
                NuPredio = parameters.FirstOrDefault(s => s.Id == "predio")?.Value,
                FlSoloPedidosNuevos = parameters.FirstOrDefault(s => s.Id == "pedidosNuevos")?.Value == "true" ? true : false,
                CdOnda = short.Parse(parameters.FirstOrDefault(s => s.Id == "onda")?.Value),
                TpAgente = parameters.FirstOrDefault(s => s.Id == "tpAgente")?.Value,
                CdOrdenPedidosAuto = parameters.FirstOrDefault(s => s.Id == "ordenPedidosAuto")?.Value,
                NuClisPorPreparacion = short.Parse(parameters.FirstOrDefault(s => s.Id == "clientesPorPrep")?.Value),
                NuDiasColaTrabajo = (diaColaTrabajo == 1) ? null : diaColaTrabajo,
            };

            regla.TpAgente = regla.TpAgente == CodigoDominioDb.SelectLibAutomaticaTipoAgenteTodos ? string.Empty : regla.TpAgente;

            regla.UsarSoloStkPicking = regla.UsarSoloStkPicking && !regla.ExcluirUbicacionesPicking;

            if (regla.TpFrecuencia != "D")
            {
                regla.HrInicio = TimeSpan.Parse(parameters.FirstOrDefault(s => s.Id == "horaInicio")?.Value, this._identity.GetFormatProvider());
                regla.HrFin = TimeSpan.Parse(parameters.FirstOrDefault(s => s.Id == "horaFin")?.Value, this._identity.GetFormatProvider());
                regla.NuFrecuencia = int.Parse(parameters.FirstOrDefault(s => s.Id == "nuFrecuencia")?.Value);
            }

            var dias = JsonConvert.DeserializeObject<List<CheckboxListItem>>(parameters.FirstOrDefault(s => s.Id == "dias").Value ?? "[]");
            var lstCondLib = JsonConvert.DeserializeObject<List<CheckboxListItem>>(parameters.FirstOrDefault(s => s.Id == "condicionLiberacion").Value ?? "[]");
            var lstTpExpedicion = JsonConvert.DeserializeObject<List<CheckboxListItem>>(parameters.FirstOrDefault(s => s.Id == "tpExpedicion").Value ?? "[]");
            var lstTpPedido = JsonConvert.DeserializeObject<List<CheckboxListItem>>(parameters.FirstOrDefault(s => s.Id == "tpPedido").Value ?? "[]");

            string dsDias = string.Empty;
            foreach (var dia in dias)
            {
                if (dia.Selected)
                    dsDias += dia.Id;
            }
            regla.DsDias = dsDias;

            if (lstCondLib != null)
            {
                foreach (var c in lstCondLib)
                {
                    if (c.Selected)
                        regla.LstReglaCondicionLiberacion.Add(new ReglaCondicionLiberacion()
                        {
                            cdCondicionLiberacion = c.Id,
                            nuRegla = nuRegla
                        });
                }
            }

            if (lstTpExpedicion != null)
            {
                foreach (var tpE in lstTpExpedicion)
                {
                    if (tpE.Selected)
                        regla.LstReglaTipoExpedicion.Add(new ReglaTipoExpedicion()
                        {
                            tpExpedicion = tpE.Id,
                            nuRegla = nuRegla
                        });
                }
            }

            if (lstTpPedido != null)
            {
                foreach (var tpP in lstTpPedido)
                {
                    if (tpP.Selected)
                        regla.LstReglaTipoPedido.Add(new ReglaTipoPedido()
                        {
                            tpPedido = tpP.Id,
                            nuRegla = nuRegla
                        });
                }
            }

            return regla;
        }

        #endregion
    }
}
