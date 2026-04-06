using DocumentFormat.OpenXml.Bibliography;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Controllers.INV;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Recepcion;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Recepcion;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.REC
{
    public class REC400CrearEstacion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IFormValidationService _formValidationService;
        protected readonly ILogger<REC400CrearEstacion> _logger;

        public REC400CrearEstacion(IIdentityService identity, 
            ITrafficOfficerService concurrencyControl, 
            IUnitOfWorkFactory uowFactory, 
            IFormValidationService formValidationService, 
            ILogger<REC400CrearEstacion> logger)
        {
            this._identity = identity;
            this._concurrencyControl = concurrencyControl;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._logger = logger;
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            InicializarSelects(uow, form);

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber($"{this._identity.Application} - Crear Estación");

            uow.BeginTransaction();

            try
            {
                CrearEstacion(uow, form);
                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("REC400_Frm1_Succes_Creacion");
            }
            catch (Exception ex)
            {
                uow.Rollback();
                context.AddErrorNotification("General_Sec0_Error_ErrorOperacionPage");
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new REC400CrearEstacionFormValidationModule(uow, this._identity.UserId), form, context);
        }

        public virtual void CrearEstacion(IUnitOfWork uow, Form form)
        {
            var nuevaEstacion = new EstacionDeClasificacion
            {
                Codigo = uow.MesaDeClasificacionRepository.GetNextCodigoEstacion(),
                Descripcion = form.GetField("descripcion").Value,
                Predio = form.GetField("predio").Value,
                FechaAdicion = DateTime.Now
            };

            var prefijo = uow.ParametroRepository.GetParameter(ParamManager.PREFIJO_CLASIFICACION) ?? BarcodeDb.PREFIX_UBIC_CLASIFICACION;

            var idUbicacion = $"{nuevaEstacion.Predio}{prefijo}{nuevaEstacion.Codigo}";

            var ubicacion = new Ubicacion
            {
                Id = idUbicacion,
                IdEmpresa = MesaDeClasificacionDb.Empresa,
                Bloque = null,
                NumeroPredio = nuevaEstacion.Predio,
                CodigoClase = MesaDeClasificacionDb.ProductoClase,
                IdProductoRotatividad = MesaDeClasificacionDb.ProductoRotatividad,
                IdProductoFamilia = MesaDeClasificacionDb.ProductoFamilia,
                IdUbicacionTipo = TipoUbicacionDb.WIS,
                CodigoSituacion = SituacionDb.Activo,
                EsUbicacionBaja = true,
                NecesitaReabastecer = false,
                FechaInsercion = DateTime.Now,
                FechaModificacion = DateTime.Now,
                IdUbicacionArea = AreaUbicacionDb.Transferencia,
                IdUbicacionZona = TipoUbicacionZonaDb.Normal,
                Profundidad = 1,
                CodigoBarras = $"{BarcodeDb.PREFIX_UBICACION}{idUbicacion}"
            };

            uow.UbicacionRepository.AddUbicacion(ubicacion);
            uow.SaveChanges();

            nuevaEstacion.Ubicacion = ubicacion.Id;

            uow.MesaDeClasificacionRepository.AddEstacion(nuevaEstacion);
            uow.SaveChanges();
        }

        public virtual void InicializarSelects(IUnitOfWork uow, Form form)
        {
            //Inicializar selects
            var selectPredio = form.GetField("predio");

            selectPredio.Options = new List<SelectOption>();

            List<Predio> predios = uow.PredioRepository.GetPrediosUsuario(_identity.UserId);
            foreach (var predio in predios)
            {
                selectPredio.Options.Add(new SelectOption(predio.Numero, $"{predio.Numero} - {predio.Descripcion}")); ;
            }

            form.GetField("predio").ReadOnly = false;

            if (_identity.Predio != GeneralDb.PredioSinDefinir)
            {
                form.GetField("predio").Value = _identity.Predio;
                form.GetField("predio").ReadOnly = true;
            }
            else if (predios.Count == 1)
            {
                selectPredio.Value = predios.FirstOrDefault().Numero;
                form.GetField("predio").ReadOnly = true;
            }
        }
    }
}
