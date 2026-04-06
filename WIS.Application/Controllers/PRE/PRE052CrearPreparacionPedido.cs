using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.PRE
{
    public class PRE052CrearPreparacionPedido : AppController
    {
        protected readonly ISecurityService _security;
        protected readonly IIdentityService _identity;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IFormValidationService _formValidationService;

        public PRE052CrearPreparacionPedido(
            ISecurityService security,
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IFormValidationService formValidationService)
        {
            this._security = security;
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._formValidationService = formValidationService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var configuracion = uow.PreparacionRepository.GetConfiguracionPreparacionManual();

            form.GetField("descripcion").Value = string.Empty;
            form.GetField("predio").Value = string.Empty;
            form.GetField("empresa").Value = string.Empty;

            InicializarSelectTipoPreparacion(form);
            InicializarSelectPredio(uow, form);

            form.GetField("permitePickearVencido").Value = configuracion.PermitePickearVencido.ToString();
            form.GetField("permitePickearVencido").Disabled = !configuracion.PermitePickearVencidoHabilitado;

            form.GetField("permitePickearAveriado").Value = configuracion.PermitePickearAveriado.ToString();
            form.GetField("permitePickearAveriado").Disabled = !configuracion.PermitePickearAveriadoHabilitado;

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.BeginTransaction();
            try
            {
                uow.CreateTransactionNumber("PRE052 Crear Preparacion manual por Pedidos");

                var empresa = form.GetField("empresa").Value;

                var preparacion = CrearPreparacion(uow, form);

                var nuPreparacion = uow.PreparacionRepository.AddPreparacion(preparacion);

                uow.SaveChanges();
                uow.Commit();

                context.Parameters?.Clear();
                context.AddParameter("idPreparacion", nuPreparacion.ToString());
                context.AddParameter("empresa", empresa);

                context.AddSuccessNotification("PRE052_Sucess_msg_PrepPedido", new List<string> { nuPreparacion.ToString() });
            }
            catch (Exception ex)
            {
                context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new CreatePrepManualPedidoValidationModule(uow, this._identity, this._security), form, context);
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "empresa": return this.SearchEmpresa(form, context);
            }

            return new List<SelectOption>();
        }

        #region Metodos Auxiliares
        public virtual void InicializarSelectTipoPreparacion(Form form)
        {
            form.GetField("tipoPreparacion").Options = new List<SelectOption>()
            {
                new SelectOption(TipoPreparacionDb.Pedido, "General_Sec0_lbl_DominioTipoPreparacionPedido")
            };

            form.GetField("tipoPreparacion").Value = TipoPreparacionDb.Pedido;
            form.GetField("tipoPreparacion").Disabled = true;
        }

        public virtual void InicializarSelectPredio(IUnitOfWork uow, Form form)
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

        public virtual Preparacion CrearPreparacion(IUnitOfWork uow, Form form)
        {
            return new Preparacion()
            {
                Descripcion = string.IsNullOrEmpty(form.GetField("descripcion").Value) ? "Preparación manual por pedidos" : form.GetField("descripcion").Value,
                Empresa = int.Parse(form.GetField("empresa").Value),
                FechaInicio = DateTime.Now,
                Tipo = TipoPreparacionDb.Pedido,
                Situacion = SituacionDb.PreparacionIniciada,
                Usuario = this._identity.UserId,
                CodigoContenedorValidado = "TPOPED",
                Predio = form.GetField("predio").Value,
                Transaccion = uow.GetTransactionNumber(),
                PermitePickVencido = bool.Parse(form.GetField("permitePickearVencido").Value),
                AceptaMercaderiaAveriada = bool.Parse(form.GetField("permitePickearAveriado").Value),
                ValidarProductoProveedor = false,
            };
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

        #endregion
    }
}
