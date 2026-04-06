using Newtonsoft.Json;
using NLog;
using System;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Produccion;
using WIS.Domain.DataModel;
using WIS.Domain.Produccion;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent.Build;
using WIS.GridComponent.Excel;
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.Session;

namespace WIS.Application.Controllers.PRD
{
    public class PRD171 : AppController
    {
        protected readonly ISessionAccessor _session;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public PRD171(ISessionAccessor session,
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFormValidationService formValidationService,
            IGridValidationService gridValidationService)
        {
            this._session = session;
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._formValidationService = formValidationService;
            this._gridValidationService = gridValidationService;
        }

        public override PageContext PageLoad(PageContext data)
        {
            var nroIngreso = data.GetParameter("nroIngreso");

            if (string.IsNullOrEmpty(nroIngreso))
            {
                data.Redirect = "/PRD/PRD170";

                return data;
            }

            using(var uow = this._uowFactory.GetUnitOfWork())
            {
                IngresoWhiteBox ingreso = uow.ProduccionRepository.GetIngresoWhiteBox(nroIngreso);
                
                data.AddParameter("ingreso", JsonConvert.SerializeObject(new
                {
                    ingreso.Id,
                    ingreso.Funcionario,
                    ingreso.Anexo1,
                    ingreso.Anexo2,
                    ingreso.Anexo3,
                    ingreso.Anexo4,
                    ingreso.FechaAlta,
                    ingreso.FechaActualizacion,
                    ingreso.GeneraPedido,                    
                    ingreso.NumeroProduccionOriginal,
                    ingreso.CantidadIteracionesFormula,
                    ingreso.Predio,
                    FuncionarioNombre = uow.SecurityRepository.GetUserFullname(ingreso.Funcionario ?? -1)
                }));

                data.AddParameter("formula", JsonConvert.SerializeObject(new
                {
                    ingreso.Formula.Id,
                    ingreso.Formula.Nombre,
                    ingreso.Formula.Descripcion,
                    ingreso.Formula.CantidadPasadasPorFormula
                }));

                data.AddParameter("linea", JsonConvert.SerializeObject(new { 
                    ingreso.Linea.Id,
                    ingreso.Linea.UbicacionEntrada,
                    ingreso.Linea.UbicacionSalida,
                    ingreso.Linea.Descripcion,
                    ingreso.Linea.FechaAlta,
                    ingreso.Linea.FechaModificacion,
                    ingreso.Linea.Tipo
                }));
            }

            return data;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using(var uow = this._uowFactory.GetUnitOfWork())
            {
                string nroIngreso = context.GetParameter("nroIngreso");
                IngresoWhiteBox ingreso = uow.ProduccionRepository.GetIngresoWhiteBox(nroIngreso);
                Pasada pasada;

                form.GetField("valor").Value = string.Empty;

                if (ingreso.IsFinalizado())
                {
                    pasada = ingreso.GetLatestPasada();

                    form.GetField("valor").Disabled = true;

                    var buttonPasadas = form.GetButton("btnAvanzarMultiplesPasadas");

                    form.GetButton("btnAvanzarPasada").Disabled = true;
                    form.GetButton("btnAbrirModal").Disabled = true;

                    if (buttonPasadas != null)
                        buttonPasadas.Disabled = true;

                    context.AddParameter("finalizado", "true");
                }
                else
                {
                    pasada = ingreso.GetCurrentPasada();
                }

                context.AddParameter("pasada", JsonConvert.SerializeObject(new
                {
                    pasada.Accion?.Descripcion,
                    NumeroPasada = Convert.ToString(pasada.GetNumeroPasadasEnFormula()),
                    Orden = Convert.ToString(pasada.Orden),
                    NumeroFormula = Convert.ToString(pasada.NumeroFormula),
                    MaxOrden = ingreso.Formula.GetCantOrdenPasada(pasada.Numero)
                }));
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (context.ButtonId == "btnAvanzarPasada")
                    this.AvanzarPasada(uow, form, context);
                else if (context.ButtonId == "btnAvanzarMultiplesPasadas")
                    this.AvanzarMultiplesPasadas(uow, form, context);

                uow.SaveChanges();

                context.ResetForm = true;
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            return this._formValidationService.Validate(new ProduccionValidationModule(), form, context);
        }

        #region Metodos Auxiliares

        public virtual void AvanzarPasada(IUnitOfWork uow, Form form, FormSubmitContext context)
        {
            string nroIngreso = context.GetParameter("nroIngreso");

            IngresoWhiteBox ingreso = uow.ProduccionRepository.GetIngresoWhiteBox(nroIngreso);

            if (ingreso.IsFinalizado())
                throw new InvalidOperationException("PRD171_form1_error_IngresoYaFinalizado");

            Pasada pasada = ingreso.GetCurrentPasada();
            pasada.Valor = form.GetField("valor").Value;

            ingreso.Pasadas.Add(pasada);

            uow.ProduccionRepository.AddPasada(ingreso, pasada);

            if (ingreso.ShouldEnsamblar())
            {
                var ensamblaje = new FormulaEnsamblaje(uow, ingreso, pasada);

                ensamblaje.Ensamblar();
            }

            if (ingreso.IsFinalizado())
                context.AddSuccessNotification("PRD171_form1_msg_ProduccionFinalizada");
        }

        public virtual void AvanzarMultiplesPasadas(IUnitOfWork uow, Form form, FormSubmitContext context)
        {
            string nroIngreso = context.GetParameter("nroIngreso");
            int cantidadPasadas = int.Parse(form.GetField("cantidadPasadas").Value);

            IngresoWhiteBox ingreso = uow.ProduccionRepository.GetIngresoWhiteBox(nroIngreso);

            if (ingreso.IsFinalizado())
                throw new InvalidOperationException("PRD171_form1_error_IngresoYaFinalizado");

            while (cantidadPasadas > 0)
            {
                Pasada pasada = ingreso.GetCurrentPasada();

                if (pasada.Accion != null)
                    break;

                pasada.Valor = form.GetField("cantidadPasadas").Value;

                ingreso.Pasadas.Add(pasada);

                uow.ProduccionRepository.AddPasada(ingreso, pasada);

                if (ingreso.ShouldEnsamblar())
                {
                    var ensamblaje = new FormulaEnsamblaje(uow, ingreso, pasada);

                    ensamblaje.Ensamblar();
                }

                if (ingreso.IsFinalizado())
                {
                    context.AddSuccessNotification("PRD171_form1_msg_ProduccionFinalizada");
                    break;
                }

                cantidadPasadas--;
            }
        }

        #endregion
    }
}
