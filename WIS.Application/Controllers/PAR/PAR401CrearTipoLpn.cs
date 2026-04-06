using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Parametrizacion;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.PRE
{
    public class PAR401CrearTipoLpn : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;

        public PAR401CrearTipoLpn(IIdentityService identity, IUnitOfWorkFactory uowFactory, IFormValidationService formValidationService, ISecurityService security)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._security = security;
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var opciones = uow.EstiloEtiquetaRepository.GetEtiquetaEstilos(EstiloEtiquetaDb.EtiquetaLpn)
                    .Select(e => new SelectOption(e.Id, $"{e.Id} - {e.Descripcion}")).ToList();

                form.GetField("NU_TEMPLATE_ETIQUETA").Options = opciones;

                InicializarToggles(form);
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                var letraPrefrijo = uow.ParametroRepository.GetParameter(ParametrosTemporal.LPN_RECEPCION_PREFIJO_ETIQUETA);
                if (letraPrefrijo.Length > 1)
                    throw new ValidationFailedException("PAR401_Sec0_Error_Er01_ConfiguracionPrefijoInvalido");

                var cantidad = uow.ManejoLpnRepository.GetCantidadDeTipoLpnPrefijoExistente(letraPrefrijo) + 1;
                if (cantidad > 99)
                    throw new ValidationFailedException("PAR401_Sec0_Error_Er02_DebeCambiarDePrefijoEtiqueta");

                var tipoEtiqueta = $"{letraPrefrijo}{cantidad}";
                if (uow.ManejoLpnRepository.AnyTipoDeEtiquetaDeRecepcion(tipoEtiqueta))
                    throw new ValidationFailedException("PAR401_Sec0_Error_Er02_DebeCambiarDePrefijoEtiqueta");

                var puntajeLpn = new LpnTipoPuntajePicking()
                {
                    IgualSinReserva = 0,
                    IgualConReserva = 0,
                    MenorSinReserva = 0,
                    MenorConReserva = 0,
                    Mayor = 0,
                    Inexistente = 0,
                    Bonus = 0,
                };

                var tipoLpn = new LpnTipo()
                {
                    Tipo = form.GetField("TP_LPN_TIPO").Value,
                    Nombre = form.GetField("NM_LPN_TIPO").Value,
                    Descripcion = form.GetField("DS_LPN_TIPO").Value,
                    PermiteConsolidar = Mapper.MapStringToBooleanString(form.GetField("FL_PERMITE_CONSOLIDAR").Value),
                    PermiteExtraerLineas = "S",
                    PermiteAgregarLineas = Mapper.MapStringToBooleanString(form.GetField("FL_PERMITE_AGREGAR_LINEAS").Value),
                    CrearSoloAlIngreso = "S",
                    MultiProducto = Mapper.MapStringToBooleanString(form.GetField("FL_MULTIPRODUCTO").Value),
                    MultiLote = Mapper.MapStringToBooleanString(form.GetField("FL_MULTI_LOTE").Value),
                    PermiteAnidacion = "N",
                    NumeroTemplate = form.GetField("NU_TEMPLATE_ETIQUETA").Value,
                    NumeroComponente = form.GetField("NU_COMPONENTE").Value.ToUpper(),
                    ContenedorLPN = Mapper.MapStringToBooleanString(form.GetField("FL_CONTENEDOR_LPN").Value),
                    PermiteGenerar = Mapper.MapStringToBooleanString(form.GetField("FL_PERMITE_GENERAR").Value),
                    IngresoRecepcionAtributo = Mapper.MapStringToBooleanString(form.GetField("FL_INGRESO_RECEPCION_ATRIBUTO").Value),
                    IngresoPickingAtributo = "N",
                    PermiteDestruirAlmacenaje = Mapper.MapStringToBooleanString(form.GetField("FL_PERMITE_DESTRUIR_ALM").Value),
                    Prefijo = form.GetField("VL_PREFIJO").Value.ToUpper(),
                    NumeroSecuencia = long.Parse(form.GetField("NU_SEQ_LPN").Value),
                    EtiquetaRecepcion = tipoEtiqueta,
                    PuntajePicking = puntajeLpn,
                };

                uow.ManejoLpnRepository.AddTipoLpn(tipoLpn);

                var tipoContenedor = new TipoContenedor()
                {
                    Id = tipoLpn.Tipo,
                    Descripcion = tipoLpn.Nombre,
                    RangoInicial = -1,
                    RangoFinal = -1,
                    UltimaSecuencia = -1,
                    ClientePredefinido = false,
                    Retornable = false,
                    TpObjetoTracking = "BULTO",
                    Habilitado = true,
                    NombreSecuencia = null
                };

                uow.ContenedorRepository.AddTipoContenedor(tipoContenedor);

                uow.SaveChanges();
                uow.Commit();
                context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");
            }
            catch (ValidationFailedException ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    context.AddErrorNotification(ex.Message, new List<string>(ex.StrArguments ?? new string[0]));
                uow.Rollback();
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new TipoLpnFormValidationModule(uow, this._identity, this._security), form, context);
        }

        public virtual void InicializarToggles(Form form)
        {
            form.GetField("FL_PERMITE_CONSOLIDAR").Value = "true";
            form.GetField("FL_PERMITE_AGREGAR_LINEAS").Value = "true";
            form.GetField("FL_MULTIPRODUCTO").Value = "true";
            form.GetField("FL_MULTI_LOTE").Value = "true";
            form.GetField("FL_INGRESO_RECEPCION_ATRIBUTO").Value = "true";
            form.GetField("FL_PERMITE_DESTRUIR_ALM").Value = "true";
            form.GetField("FL_CONTENEDOR_LPN").Value = "true";
            form.GetField("FL_PERMITE_GENERAR").Value = "true";
        }

    }
}
