using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.CheckboxListComponent;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.General;
using WIS.Domain.General.Configuracion;
using WIS.Domain.Liberacion;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.PRE
{
    public class PRE052LiberacionOnda : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;

        public PRE052LiberacionOnda(
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

            var empresa = uow.EmpresaRepository.GetEmpresaUnicaParaUsuario(this._identity.UserId);

            this.InicializarSelects(form, empresa == null ? string.Empty : empresa.Id.ToString(), context);

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new LiberacionOndaFiltroFormValidationModule(uow, this._identity.UserId), form, context);
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "idEmpresa": return this.SearchEmpresa(form, context);
                case "onda": return this.SearchOnda(form, context);

                default: return new List<SelectOption>();
            }
        }

        #region Metodos auxiliares

        public virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new EmpresasPedPendientesQuery(_identity.UserId);
            uow.HandleQuery(dbQuery);

            List<Empresa> empresas;

            if (int.TryParse(context.SearchValue, out int cdEmpresa))
                empresas = dbQuery.GetByNombreOrCodePartial(context.SearchValue, cdEmpresa.ToString());
            else
                empresas = dbQuery.GetByNombreOrCodePartial(context.SearchValue);

            foreach (var empresa in empresas)
            {
                opciones.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchOnda(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            string predio = form.GetField("predio").Value;

            int? cdEmpresa = null;
            if (int.TryParse(form.GetField("idEmpresa").Value, out int parsedValue))
                cdEmpresa = parsedValue;

            var dbQuery = new LiberacionOndaQuery(cdEmpresa);
            uow.HandleQuery(dbQuery);

            List<Onda> ondas = dbQuery.GetByNombreOrCodePartial(context.SearchValue, predio);

            foreach (var onda in ondas)
            {
                opciones.Add(new SelectOption(onda.Id.ToString(), $"{onda.Id} - {onda.Descripcion}"));
            }

            return opciones;
        }

        public virtual void InicializarSelects(Form form, string empresa, FormInitializeContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            LiberacionConfiguracion liberaConf = uow.PreparacionRepository.GetLiberacionConfiguracion(empresa);

            FormField selectPredio = form.GetField("predio");
            selectPredio.Options = new List<SelectOption>();

            // Predios
            var dbQuery = new GetPrediosUsuarioQuery();
            uow.HandleQuery(dbQuery);

            var predios = dbQuery.GetPrediosUsuario(_identity.UserId).OrderBy(x => x.Numero);
            foreach (var predio in predios)
            {
                selectPredio.Options.Add(new SelectOption(predio.Numero, $"{predio.Numero} - {predio.Descripcion}")); ;
            }

            if (predios.Count() == 1)
                selectPredio.Value = predios.FirstOrDefault().Numero;

            if (this._identity.Predio != GeneralDb.PredioSinDefinir)
                selectPredio.Value = this._identity.Predio;

            //UbicacionCompleta
            form.GetField("ubicacionCompleta").Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaPalletCompleto)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();

            form.GetField("ubicacionCompleta").ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.UbicacionCompletaHabilitado;
            form.GetField("ubicacionCompleta").Value = liberaConf.UbicacionCompleta;

            //UbicacionIncompleta
            form.GetField("ubicacionIncompleta").Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaPalletIncompleto)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();

            form.GetField("ubicacionIncompleta").ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.UbicacionIncompletaHabilitado;
            form.GetField("ubicacionIncompleta").Value = liberaConf.UbicacionIncompleta;

            //Prepa Solo camion
            form.GetField("prepSoloCamion").Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();

            form.GetField("prepSoloCamion").ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.PrepSoloCamionHabilitado;
            form.GetField("prepSoloCamion").Value = liberaConf.PrepSoloCamion;

            //Agrup Por camion
            form.GetField("agrupPorCamion").Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();

            form.GetField("agrupPorCamion").ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.AgruparCamionHabilitado;
            form.GetField("agrupPorCamion").Value = liberaConf.AgruparCamion;

            //Maneja vida util
            form.GetField("manejaVidaUtil").Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
            .Select(d => new SelectOption(d.Valor, d.Descripcion))
            .ToList();

            form.GetField("manejaVidaUtil").ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.ManejaVidaUtilHabilitado;
            form.GetField("manejaVidaUtil").Value = liberaConf.ManejaVidaUtil;

            //Requiere ubicacion picking 2 fases
            form.GetField("ubicacionPicking2Fases").Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();

            form.GetField("ubicacionPicking2Fases").ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.RequiereUbicacionHabilitado;
            form.GetField("ubicacionPicking2Fases").Value = liberaConf.RequiereUbicacion;

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
            form.GetField("priorizarDesborde").Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();

            form.GetField("priorizarDesborde").ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.PriorizarDesbordeHabilitado;
            form.GetField("priorizarDesborde").Value = liberaConf.PriorizarDesborde;

            //Stock
            form.GetField("stock").Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaStock)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();

            form.GetField("stock").ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.DefaultStockHabilitado;
            form.GetField("stock").Value = liberaConf.DefaultStock;

            //pedidos
            form.GetField("pedidos").Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaPedidos)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();

            form.GetField("pedidos").ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.PedidosHabilitado;
            form.GetField("pedidos").Value = liberaConf.Pedidos;

            //Repartir escazes
            form.GetField("repartirEscasez").Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();

            form.GetField("repartirEscasez").ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.RepartirEscazesHabilitado;
            form.GetField("repartirEscasez").Value = liberaConf.RepartirEscazes;

            //liberarPorUnidades
            form.GetField("liberarPorUnidades").Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();

            form.GetField("liberarPorUnidades").ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.LiberarPorUnidadHabilitado;
            form.GetField("liberarPorUnidades").Value = liberaConf.LiberarPorUnidad;

            //liberarPorCurvas
            form.GetField("liberarPorCurvas").Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();

            form.GetField("liberarPorCurvas").ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.LiberarPorCurvasHabilitado;
            form.GetField("liberarPorCurvas").Value = liberaConf.LiberarPorCurvas;

            //stockDMTI
            form.GetField("stockDtmi").Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();

            form.GetField("stockDtmi").ReadOnly = string.IsNullOrEmpty(empresa) ? true : (liberaConf.ManejoDocumental || !liberaConf.ControlStockDMTIHabilitado);
            form.GetField("stockDtmi").Value = liberaConf.ManejoDocumental ? "S" : liberaConf.ControlStockDMTI;

            //RespetaFifo
            form.GetField("respetaFifo").Options = uow.DominioRepository.GetDominios(CodigoDominioDb.SelectLibOndaSN)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();

            form.GetField("respetaFifo").ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.RespetaFifoHabilitado;
            form.GetField("respetaFifo").Value = liberaConf.RespetaFifo;

            //Agrupador
            form.GetField("agrupacion").Options = uow.DominioRepository.GetDominios(CodigoDominioDb.Agrupacion)
                .Select(d => new SelectOption(d.Valor, d.Descripcion))
                .ToList();

            form.GetField("agrupacion").ReadOnly = string.IsNullOrEmpty(empresa) ? true : !liberaConf.ManejoAgrupadorHabilitado;
            form.GetField("agrupacion").Value = liberaConf.ManejoAgrupador;


            //Se habilitara en el validate por empresa
            form.GetField("onda").ReadOnly = true;

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

            //Manejo documental
            List<CheckboxListItem> checkListDoc = new List<CheckboxListItem>();
            List<CheckboxListItem> checkListTipoDoc = new List<CheckboxListItem>();

            if (liberaConf.ManejoDocumental && int.TryParse(empresa, out int cdEmpresa))
            {
                var tiposDocumentos = uow.DocumentoRepository.GetTiposDocumentosLiberables();
                var documentos = uow.DocumentoRepository.GetDocumentosLiberables(cdEmpresa);

                foreach (var t in tiposDocumentos)
                {
                    checkListTipoDoc.Add(new CheckboxListItem
                    {
                        Id = t.Tipo,
                        Label = t.Descripcion,
                        Selected = false
                    });
                }

                foreach (var d in documentos)
                {
                    checkListDoc.Add(new CheckboxListItem
                    {
                        Id = d.Tipo + "###" + d.Numero,
                        Label = d.Descripcion,
                        Selected = false
                    });
                }
            }

            query.AddParameter("ManejoDocumental", liberaConf.ManejoDocumental.ToString().ToLower());
            query.AddParameter("ListItemsTipoDoc", JsonConvert.SerializeObject(checkListTipoDoc));
            query.AddParameter("ListItemsDoc", JsonConvert.SerializeObject(checkListDoc));
        }

        #endregion
    }
}
