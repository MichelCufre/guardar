using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Preparacion;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.Picking;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.PRE
{
    public class PRE811CreatePreferencia : AppController
    {
        protected readonly Logger _logger;

        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;

        public PRE811CreatePreferencia(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            InicializarSelectPredio(uow, form);

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                bool controlAccesoHabilitado = form.GetField("habilitarControlAcceso").IsChecked();

                var preferencia = new Preferencia
                {
                    NU_PREFERENCIA = uow.PreferenciaRepository.GetNextNuPreferencia(),
                    DS_PREFERENCIA = form.GetField("dsPreferencia").Value,
                    NU_PREDIO = form.GetField("predio").Value,

                    FL_HABILITADO_CONT_ACCESO = controlAccesoHabilitado ? "S" : "N",
                    FL_HABILITADO_LIB_COMPLETO = Mapper.MapStringToBooleanString(form.GetField("habilitarLiberadoCompleto").Value),
                    FL_HABILITADO_EMPRESA = Mapper.MapStringToBooleanString(form.GetField("habilitarEmpresa").Value),
                    FL_HABILITADO_CLIENTE = Mapper.MapStringToBooleanString(form.GetField("habilitarCliente").Value),
                    FL_HABILITADO_ZONA = Mapper.MapStringToBooleanString(form.GetField("habilitarZona").Value),
                    FL_HABILITADO_COND_LIBERACION = Mapper.MapStringToBooleanString(form.GetField("habilitarLiberacion").Value),
                    FL_HABILITADO_TP_PEDIDO = Mapper.MapStringToBooleanString(form.GetField("habilitarTpPedido").Value),
                    FL_HABILITADO_TP_EXPEDICION = Mapper.MapStringToBooleanString(form.GetField("habilitarTpExpedicion").Value),
                    FL_HABILITADO_CLASE = Mapper.MapStringToBooleanString(form.GetField("habilitarClase").Value),
                    FL_HABILITADO_FAMILIA = Mapper.MapStringToBooleanString(form.GetField("habilitarFamilia").Value),
                    FL_HABILITADO_RUTA = Mapper.MapStringToBooleanString(form.GetField("habilitarRuta").Value),
                    FL_HABILITADO_PEDIDO_COMPLETO = controlAccesoHabilitado ? Mapper.MapStringToBooleanString(form.GetField("habilitarPedCompleto").Value) : "N",
                    ID_BLOQUE_MIN = controlAccesoHabilitado ? string.Empty : (string.IsNullOrEmpty(form.GetField("bloqueMin").Value) ? "A" : form.GetField("bloqueMin").Value),
                    ID_BLOQUE_MAX = controlAccesoHabilitado ? string.Empty : (string.IsNullOrEmpty(form.GetField("bloqueMax").Value) ? "Z" : form.GetField("bloqueMax").Value),
                    ID_CALLE_MIN = controlAccesoHabilitado ? string.Empty : (string.IsNullOrEmpty(form.GetField("calleMin").Value) ? "A" : form.GetField("calleMin").Value),
                    ID_CALLE_MAX = controlAccesoHabilitado ? string.Empty : (string.IsNullOrEmpty(form.GetField("calleMax").Value) ? "Z" : form.GetField("calleMax").Value),
                    NU_COLUMNA_MIN = controlAccesoHabilitado ? null : (string.IsNullOrEmpty(form.GetField("columnaMin").Value) ? 0 : int.Parse(form.GetField("columnaMin").Value)),
                    NU_COLUMNA_MAX = controlAccesoHabilitado ? null : (string.IsNullOrEmpty(form.GetField("columnaMax").Value) ? 0 : int.Parse(form.GetField("columnaMax").Value)),
                    NU_ALTURA_MIN = controlAccesoHabilitado ? null : (string.IsNullOrEmpty(form.GetField("alturaMin").Value) ? 0 : int.Parse(form.GetField("alturaMin").Value)),
                    NU_ALTURA_MAX = controlAccesoHabilitado ? null : (string.IsNullOrEmpty(form.GetField("alturaMax").Value) ? 0 : int.Parse(form.GetField("alturaMax").Value)),                    
                };

                decimal.TryParse(form.GetField("pesoMax").Value, _identity.GetFormatProvider(), out decimal pesoMaximo);
                preferencia.PS_BRUTO_MAXIMO = pesoMaximo * 1000;

                decimal.TryParse(form.GetField("volumenMax").Value, _identity.GetFormatProvider(), out decimal cubagemMaximo);
                preferencia.VL_CUBAGEM_MAXIMO = cubagemMaximo;

                int.TryParse(form.GetField("clientesSimultaneos").Value, out int cantidadCliente);
                preferencia.QT_CLIENTES = cantidadCliente;

                int.TryParse(form.GetField("pedidosSimultaneos").Value, out int cantidadPedido);
                preferencia.QT_PEDIDOS = cantidadPedido;

                int.TryParse(form.GetField("maxPickeos").Value, out int cantidadMaximoPicking);
                preferencia.QT_MAXIMO_PICKEOS = cantidadMaximoPicking;

                int.TryParse(form.GetField("maxUnidades").Value, out int cantidadMaximoUnidad);
                preferencia.QT_MAXIMO_UNIDADES = cantidadMaximoUnidad;

                uow.PreferenciaRepository.AddPreferencia(preferencia);

                uow.SaveChanges();
                uow.Commit();

                context.Parameters?.Clear();
                context.Parameters.Add(new ComponentParameter("nuPreferencia", preferencia.NU_PREFERENCIA.ToString()));

                context.AddSuccessNotification("PRE811_Frm1_Succes_Creacion", new List<string> { preferencia.NU_PREFERENCIA.ToString() });
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                uow.Rollback();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "FormSubmit");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
                uow.Rollback();
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new PreferenciaFormValidationModule(uow, this._identity.GetFormatProvider()), form, context);
        }

        #region Metodos Auxiliares

        protected virtual void InicializarSelectPredio(IUnitOfWork uow, Form form)
        {
            form.GetField("habilitarPedCompleto").Disabled = !form.GetField("habilitarControlAcceso").IsChecked();

            // Predios
            var selectPredio = form.GetField("predio");
            selectPredio.Options = new List<SelectOption>();

            var dbQuery = new GetPrediosUsuarioQuery();
            uow.HandleQuery(dbQuery);

            var predios = dbQuery.GetPrediosUsuario(_identity.UserId).OrderBy(x => x.Numero);
            foreach (var pred in predios)
            {
                selectPredio.Options.Add(new SelectOption(pred.Numero, $"{pred.Numero} - {pred.Descripcion}"));
            }

            if (predios.Count() == 1)
                selectPredio.Value = predios.FirstOrDefault().Numero;

            if (this._identity.Predio != GeneralDb.PredioSinDefinir)
                selectPredio.Value = this._identity.Predio;
        }

        #endregion
    }
}
