using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Preparacion;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.Picking;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.PRE
{
    public class PRE811UpdatePreferencia : AppController
    {
        protected readonly Logger _logger;

        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;

        public PRE811UpdatePreferencia(
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

            if (!int.TryParse(context.GetParameter("keyPreferencia"), out int nuPreferencia))
                throw new MissingParameterException("General_Sec0_Error_SinParametros");

            var preferencia = uow.PreferenciaRepository.GetPreferencia(nuPreferencia);

            if (preferencia == null)
                throw new EntityNotFoundException("PRE811_Frm1_Error_PreferenciaNoExiste");

            InicializarFormulario(uow, form, preferencia);

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                var codigo = form.GetField("nuPreferencia").Value;

                var preferencia = uow.PreferenciaRepository.GetPreferencia(int.Parse(codigo));

                if (preferencia == null)
                    throw new ValidationFailedException("PRE811_Frm1_Error_PreferenciaNoExiste");

                var controlAccesoHabilitado = form.GetField("habilitarControlAcceso").IsChecked();

                preferencia.DS_PREFERENCIA = form.GetField("dsPreferencia").Value;
                preferencia.NU_PREDIO = form.GetField("predio").Value;
                preferencia.FL_HABILITADO_LIB_COMPLETO = Mapper.MapStringToBooleanString(form.GetField("habilitarLiberadoCompleto").Value);
                preferencia.FL_HABILITADO_PEDIDO_COMPLETO = controlAccesoHabilitado ? Mapper.MapStringToBooleanString(form.GetField("habilitarPedCompleto").Value) : "N";
                preferencia.ID_BLOQUE_MIN = controlAccesoHabilitado ? string.Empty : (string.IsNullOrEmpty(form.GetField("bloqueMin").Value) ? "A" : form.GetField("bloqueMin").Value);
                preferencia.ID_BLOQUE_MAX = controlAccesoHabilitado ? string.Empty : (string.IsNullOrEmpty(form.GetField("bloqueMax").Value) ? "Z" : form.GetField("bloqueMax").Value);
                preferencia.ID_CALLE_MIN = controlAccesoHabilitado ? string.Empty : (string.IsNullOrEmpty(form.GetField("calleMin").Value) ? "A" : form.GetField("calleMin").Value);
                preferencia.ID_CALLE_MAX = controlAccesoHabilitado ? string.Empty : (string.IsNullOrEmpty(form.GetField("calleMax").Value) ? "Z" : form.GetField("calleMax").Value);
                preferencia.NU_COLUMNA_MIN = controlAccesoHabilitado ? null : (string.IsNullOrEmpty(form.GetField("columnaMin").Value) ? 0 : int.Parse(form.GetField("columnaMin").Value));
                preferencia.NU_COLUMNA_MAX = controlAccesoHabilitado ? null : (string.IsNullOrEmpty(form.GetField("columnaMax").Value) ? 0 : int.Parse(form.GetField("columnaMax").Value));
                preferencia.NU_ALTURA_MIN = controlAccesoHabilitado ? null : (string.IsNullOrEmpty(form.GetField("alturaMin").Value) ? 0 : int.Parse(form.GetField("alturaMin").Value));
                preferencia.NU_ALTURA_MAX = controlAccesoHabilitado ? null : (string.IsNullOrEmpty(form.GetField("alturaMax").Value) ? 0 : int.Parse(form.GetField("alturaMax").Value));

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

                if (controlAccesoHabilitado)
                    preferencia.FL_HABILITADO_CONT_ACCESO = "S";
                else
                {
                    preferencia.FL_HABILITADO_CONT_ACCESO = "N";

                    var controles = uow.PreferenciaRepository.GetAllControlAccesoPreferencia(preferencia.NU_PREFERENCIA);
                    uow.PreferenciaRepository.RemoveControlAccesoPreferencia(controles);
                }

                if (form.GetField("habilitarEmpresa").IsChecked())
                    preferencia.FL_HABILITADO_EMPRESA = "S";
                else
                {
                    preferencia.FL_HABILITADO_EMPRESA = "N";

                    var empresas = uow.PreferenciaRepository.GetAllEmpresaPreferencia(preferencia.NU_PREFERENCIA);
                    uow.PreferenciaRepository.RemoveEmpresasPreferencia(empresas);
                }

                if (form.GetField("habilitarCliente").IsChecked())
                    preferencia.FL_HABILITADO_CLIENTE = "S";
                else
                {
                    preferencia.FL_HABILITADO_CLIENTE = "N";

                    var clientes = uow.PreferenciaRepository.GetAllClientePreferencia(preferencia.NU_PREFERENCIA);
                    uow.PreferenciaRepository.RemoveClientesPreferencia(clientes);
                }

                if (form.GetField("habilitarRuta").IsChecked())
                    preferencia.FL_HABILITADO_RUTA = "S";
                else
                {
                    preferencia.FL_HABILITADO_RUTA = "N";

                    var rutas = uow.PreferenciaRepository.GetAllRutaPreferencia(preferencia.NU_PREFERENCIA);
                    uow.PreferenciaRepository.RemoveRutasPreferencia(rutas);
                }

                if (form.GetField("habilitarZona").IsChecked())
                    preferencia.FL_HABILITADO_ZONA = "S";
                else
                {
                    preferencia.FL_HABILITADO_ZONA = "N";

                    var zonas = uow.PreferenciaRepository.GetAllZonaPreferencia(preferencia.NU_PREFERENCIA);
                    uow.PreferenciaRepository.RemoveZonasPreferencia(zonas);
                }

                if (form.GetField("habilitarLiberacion").IsChecked())
                    preferencia.FL_HABILITADO_COND_LIBERACION = "S";
                else
                {
                    preferencia.FL_HABILITADO_COND_LIBERACION = "N";
                    var condiciones = uow.PreferenciaRepository.GetAllCondLibPreferencia(preferencia.NU_PREFERENCIA);
                    uow.PreferenciaRepository.RemoveCondLibPreferencia(condiciones);
                }

                if (form.GetField("habilitarTpPedido").IsChecked())
                    preferencia.FL_HABILITADO_TP_PEDIDO = "S";
                else
                {
                    preferencia.FL_HABILITADO_TP_PEDIDO = "N";

                    var tipos = uow.PreferenciaRepository.GetAllTpPedidoPreferencia(preferencia.NU_PREFERENCIA);
                    uow.PreferenciaRepository.RemoveTpPedidoPreferencia(tipos);
                }

                if (form.GetField("habilitarTpExpedicion").IsChecked())
                    preferencia.FL_HABILITADO_TP_EXPEDICION = "S";
                else
                {
                    preferencia.FL_HABILITADO_TP_EXPEDICION = "N";

                    var tipos = uow.PreferenciaRepository.GetAllTpExpedicionPreferencia(preferencia.NU_PREFERENCIA);
                    uow.PreferenciaRepository.RemoveTpExpedicionPreferencia(tipos);
                }

                if (form.GetField("habilitarClase").IsChecked())
                    preferencia.FL_HABILITADO_CLASE = "S";
                else
                {
                    preferencia.FL_HABILITADO_CLASE = "N";

                    var clases = uow.PreferenciaRepository.GetAllClasePreferencia(preferencia.NU_PREFERENCIA);
                    uow.PreferenciaRepository.RemoveClassePreferencia(clases);
                }

                if (form.GetField("habilitarFamilia").IsChecked())
                    preferencia.FL_HABILITADO_FAMILIA = "S";
                else
                {
                    preferencia.FL_HABILITADO_FAMILIA = "N";

                    var familias = uow.PreferenciaRepository.GetAllFamiliaPreferencia(preferencia.NU_PREFERENCIA);
                    uow.PreferenciaRepository.RemoveFamiliasPreferencia(familias);
                }

                uow.PreferenciaRepository.UpdatePreferencia(preferencia);

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("PRE811_Frm1_Succes_Edicion", new List<string> { preferencia.NU_PREFERENCIA.ToString() });
            }
            catch (ValidationFailedException ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "FormSubmit");
                uow.Rollback();
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }
            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new PreferenciaFormValidationModule(uow, this._identity.GetFormatProvider()), form, context);
        }

        #region Metodos Auxiliares

        public virtual void InicializarFormulario(IUnitOfWork uow, Form form, Preferencia preferencia)
        {
            form.GetField("nuPreferencia").Value = preferencia.NU_PREFERENCIA.ToString();
            form.GetField("dsPreferencia").Value = preferencia.DS_PREFERENCIA;

            var selectPredio = form.GetField("predio");
            selectPredio.Options = new List<SelectOption>();

            var dbQuery = new GetPrediosUsuarioQuery();
            uow.HandleQuery(dbQuery);

            var predios = dbQuery.GetPrediosUsuario(this._identity.UserId).OrderBy(x => x.Numero);
            foreach (var predio in predios)
            {
                selectPredio.Options.Add(new SelectOption(predio.Numero, $"{predio.Numero} - {predio.Descripcion}")); ;
            }
            selectPredio.Value = preferencia.NU_PREDIO;
            selectPredio.ReadOnly = true;

            form.GetField("habilitarControlAcceso").Value = preferencia.FL_HABILITADO_CONT_ACCESO;
            form.GetField("habilitarLiberadoCompleto").Value = preferencia.FL_HABILITADO_LIB_COMPLETO;
            form.GetField("habilitarPedCompleto").Value = preferencia.FL_HABILITADO_PEDIDO_COMPLETO;
            form.GetField("habilitarLiberacion").Value = preferencia.FL_HABILITADO_COND_LIBERACION;
            form.GetField("habilitarClase").Value = preferencia.FL_HABILITADO_CLASE;
            form.GetField("habilitarFamilia").Value = preferencia.FL_HABILITADO_FAMILIA;
            form.GetField("habilitarRuta").Value = preferencia.FL_HABILITADO_RUTA;
            form.GetField("habilitarZona").Value = preferencia.FL_HABILITADO_ZONA;
            form.GetField("habilitarEmpresa").Value = preferencia.FL_HABILITADO_EMPRESA;
            form.GetField("habilitarCliente").Value = preferencia.FL_HABILITADO_CLIENTE;
            form.GetField("habilitarTpPedido").Value = preferencia.FL_HABILITADO_TP_PEDIDO;
            form.GetField("habilitarTpExpedicion").Value = preferencia.FL_HABILITADO_TP_EXPEDICION;

            if (preferencia.PS_BRUTO_MAXIMO != null)
            {
                var pesoMaxKilos = preferencia.PS_BRUTO_MAXIMO / 1000;
                form.GetField("pesoMax").Value = pesoMaxKilos?.ToString(_identity.GetFormatProvider());
            }

            form.GetField("volumenMax").Value = preferencia.VL_CUBAGEM_MAXIMO?.ToString(_identity.GetFormatProvider());
            form.GetField("clientesSimultaneos").Value = preferencia.QT_CLIENTES?.ToString((_identity.GetFormatProvider()));
            form.GetField("pedidosSimultaneos").Value = preferencia.QT_PEDIDOS?.ToString((_identity.GetFormatProvider()));
            form.GetField("maxPickeos").Value = preferencia.QT_MAXIMO_PICKEOS?.ToString((_identity.GetFormatProvider()));
            form.GetField("maxUnidades").Value = preferencia.QT_MAXIMO_UNIDADES?.ToString((_identity.GetFormatProvider()));

            form.GetField("bloqueMin").Value = preferencia.ID_BLOQUE_MIN;
            form.GetField("bloqueMax").Value = preferencia.ID_BLOQUE_MAX;
            form.GetField("calleMin").Value = preferencia.ID_CALLE_MIN;
            form.GetField("calleMax").Value = preferencia.ID_CALLE_MAX;
            form.GetField("columnaMin").Value = preferencia.NU_COLUMNA_MIN?.ToString();
            form.GetField("columnaMax").Value = preferencia.NU_COLUMNA_MAX?.ToString();
            form.GetField("alturaMin").Value = preferencia.NU_ALTURA_MIN?.ToString();
            form.GetField("alturaMax").Value = preferencia.NU_ALTURA_MAX?.ToString();

            var controlAccesoHabilitado = preferencia.FL_HABILITADO_CONT_ACCESO == "S";

            form.GetField("bloqueMin").ReadOnly = controlAccesoHabilitado;
            form.GetField("bloqueMax").ReadOnly = controlAccesoHabilitado;
            form.GetField("calleMin").ReadOnly = controlAccesoHabilitado;
            form.GetField("calleMax").ReadOnly = controlAccesoHabilitado;
            form.GetField("columnaMin").ReadOnly = controlAccesoHabilitado;
            form.GetField("columnaMax").ReadOnly = controlAccesoHabilitado;
            form.GetField("alturaMin").ReadOnly = controlAccesoHabilitado;
            form.GetField("alturaMax").ReadOnly = controlAccesoHabilitado;

            form.GetField("habilitarPedCompleto").Disabled = !controlAccesoHabilitado;

        }

        #endregion
    }
}
