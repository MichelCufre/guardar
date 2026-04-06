using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.Expedicion;
using WIS.Domain.Expedicion.Enums;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.EXP
{
    public class EXP040ModificarEgreso : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;
        protected readonly ITrackingService _trackingService;
        protected readonly IParameterService _parameterService;

        public EXP040ModificarEgreso(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            ISecurityService security,
            ITrackingService trackingService,
            IParameterService parameterService)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._security = security;
            this._trackingService = trackingService;
            this._parameterService = parameterService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            if (!int.TryParse(context.GetParameter("camion"), out int camionId))
                throw new ValidationFailedException("EXP040_Sec0_Error_EgresonoValido");

            using var uow = this._uowFactory.GetUnitOfWork();

            Camion camion = uow.CamionRepository.GetCamion(camionId);
            this.InicializarSelects(form, camion, uow);

            return form;
        }
        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            string empresa = form.GetField("codigoEmpresa").Value;
            string puerta = form.GetField("codigoPuerta").Value;
            string ruta = form.GetField("codigoRuta").Value;
            string vehiculo = form.GetField("codigoVehiculo").Value;

            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber("EXP040 Edicion Camion");

            int camionId = int.Parse(context.GetParameter("camion"));

            Camion camion = uow.CamionRepository.GetCamion(camionId);

            int? empresaValue = string.IsNullOrEmpty(empresa) ? null : (int?)int.Parse(empresa);

            ExpedicionConfiguracionService expedicionService = new ExpedicionConfiguracionService(uow, this._parameterService, new ParametroMapper());

            if (camion.IsAguarandoCarga())
            {
                camion.Puerta = string.IsNullOrEmpty(puerta) ? null : (short?)short.Parse(puerta);

                bool tieneClienteAsociado = uow.CamionRepository.AnyClienteAsociadoCamion(camion.Id);

                if (!expedicionService.IsManejoDocumentalHabilitado(empresaValue ?? -1) || !tieneClienteAsociado)
                {
                    camion.Descripcion = form.GetField("descripcion").Value;
                    camion.Matricula = form.GetField("matricula").Value;
                    camion.Transportista = int.Parse(form.GetField("codigoTransportista").Value);
                    camion.Vehiculo = string.IsNullOrEmpty(vehiculo) ? null : (short?)short.Parse(vehiculo);
                    camion.RespetaOrdenCarga = bool.Parse(form.GetField("respetaOrdenCarga").Value);
                    camion.IsRuteoHabilitado = bool.Parse(form.GetField("habilitarRuteo").Value);
                    camion.IsTrackingHabilitado = bool.Parse(form.GetField("habilitarTracking").Value);
                    camion.IsControlContenedoresHabilitado = bool.Parse(form.GetField("controlContenedores").Value);
                    camion.Empresa = empresaValue;

                    if (!tieneClienteAsociado)
                    {
                        camion.Predio = form.GetField("predio").Value;
                        camion.Ruta = string.IsNullOrEmpty(ruta) ? null : (short?)short.Parse(ruta);
                    }
                }
            }
            else
            {
                if (camion.IsCargando())
                    camion.Puerta = string.IsNullOrEmpty(puerta) ? null : (short?)short.Parse(puerta);

                camion.Matricula = form.GetField("matricula").Value;
                camion.IsRuteoHabilitado = bool.Parse(form.GetField("habilitarRuteo").Value);
                camion.IsTrackingHabilitado = bool.Parse(form.GetField("habilitarRuteo").Value);
                camion.IsControlContenedoresHabilitado = bool.Parse(form.GetField("controlContenedores").Value);
            }

            camion.FechaModificacion = DateTime.Now;
            camion.NumeroTransaccion = uow.GetTransactionNumber();

            uow.CamionRepository.UpdateCamion(camion);

            uow.SaveChanges();

            context.AddSuccessNotification("General_Sec0_Error_Er022_EditSuccess");

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new CreateEgresoFormValidationModule(uow, this._identity, this._security), form, context);
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            return context.FieldId switch
            {
                "codigoEmpresa" => this.SearchEmpresa(form, context),
                "codigoRuta" => this.SearchRuta(form, context),
                "codigoPuerta" => this.SearchPuerta(form, context),
                "codigoTransportista" => this.SearchTransportista(form, context),
                "codigoVehiculo" => this.SearchVehiculo(form, context),
                _ => new List<SelectOption>(),
            };
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
        
        public virtual List<SelectOption> SearchRuta(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            string predio = form.GetField("predio").Value;

            if (string.IsNullOrEmpty(predio))
                return opciones;

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Ruta> rutas = uow.RutaRepository.GetByDescripcionOrCodePartial(context.SearchValue, predio);

            foreach (var ruta in rutas)
            {
                string rutaDesc = $"{ruta.Id} - {ruta.Descripcion}";
                if (!string.IsNullOrEmpty(ruta.Zona))
                    rutaDesc += $"- {ruta.Zona}";

                opciones.Add(new SelectOption(ruta.Id.ToString(), rutaDesc));
            }

            return opciones;
        }
        
        public virtual List<SelectOption> SearchPuerta(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            string predio = form.GetField("predio").Value;

            if (string.IsNullOrEmpty(predio))
                return opciones;

            using var uow = this._uowFactory.GetUnitOfWork();

            List<PuertaEmbarque> puertas = uow.PuertaEmbarqueRepository.GetByDescripcionOrCodePartial(context.SearchValue, predio);
            puertas = puertas.FindAll(p => p.Tipo != TipoPuertaEmbarqueDb.Entrada);
            foreach (var puerta in puertas)
            {
                opciones.Add(new SelectOption(puerta.Id.ToString(), $"{puerta.Id} - {puerta.Descripcion}"));
            }

            return opciones;
        }
        
        public virtual List<SelectOption> SearchTransportista(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Transportista> transportistas = uow.TransportistaRepository.GetByDescripcionOrCodePartial(context.SearchValue);

            foreach (var transportista in transportistas)
            {
                opciones.Add(new SelectOption(transportista.Id.ToString(), $"{transportista.Id} - {transportista.Descripcion}"));
            }

            return opciones;
        }
        
        public virtual List<SelectOption> SearchVehiculo(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            string predio = form.GetField("predio").Value;

            if (string.IsNullOrEmpty(predio))
                return opciones;

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Vehiculo> vehiculos = uow.VehiculoRepository.GetByDescripcionOrCodePartial(context.SearchValue, predio);

            foreach (var vehiculo in vehiculos)
            {
                opciones.Add(new SelectOption(vehiculo.Id.ToString(), $"{vehiculo.Id} - {vehiculo.Descripcion}"));
            }

            return opciones;
        }

        public virtual void InicializarSelects(Form form, Camion camion, IUnitOfWork uow)
        {
            FormField fieldRuta = form.GetField("codigoRuta");
            FormField fieldEmpresa = form.GetField("codigoEmpresa");
            FormField fieldPuerta = form.GetField("codigoPuerta");
            FormField fieldVehiculo = form.GetField("codigoVehiculo");
            FormField fieldMatricula = form.GetField("matricula");
            FormField fieldTransportista = form.GetField("codigoTransportista");

            string empresa = Convert.ToString(camion.Empresa);
            string ruta = Convert.ToString(camion.Ruta);
            string puerta = Convert.ToString(camion.Puerta);
            string vehiculo = Convert.ToString(camion.Vehiculo);
            string transportista = Convert.ToString(camion.Transportista);

            form.GetField("descripcion").Value = camion.Descripcion;
            form.GetField("predio").Value = camion.Predio;
            form.GetField("respetaOrdenCarga").Value = camion.RespetaOrdenCarga.ToString();
            form.GetField("habilitarRuteo").Value = camion.IsRuteoHabilitado.ToString();
            form.GetField("controlContenedores").Value = camion.IsControlContenedoresHabilitado.ToString();

            fieldMatricula.Value = camion.Matricula;
            form.GetField("predio").Value = camion.Predio;

            if (camion.Estado == CamionEstado.Cargando)
            {
                foreach (var field in form.Fields)
                {
                    field.ReadOnly = true;
                }
                form.GetField("descripcion").ReadOnly = false;
                form.GetField("matricula").ReadOnly = false;
            }
            else if (uow.CamionRepository.AnyClienteAsociadoCamion(camion.Id))
            {
                form.GetField("predio").ReadOnly = true;
                form.GetField("codigoEmpresa").ReadOnly = true;
                form.GetField("codigoRuta").ReadOnly = true;
            }

            if (camion.IsFacturado())
                form.GetField("controlContenedores").Disabled = true;

            form.GetField("habilitarTracking").Value = camion.IsTrackingHabilitado.ToString();
            form.GetField("habilitarTracking").Disabled = true;

            if (_trackingService.TrackingHabilitado())
                form.GetField("habilitarTracking").Disabled = false;

            fieldEmpresa.Options = SearchEmpresa(form, new FormSelectSearchContext()
            {
                SearchValue = empresa
            });

            fieldEmpresa.Value = empresa;

            fieldRuta.Options = SearchRuta(form, new FormSelectSearchContext()
            {
                SearchValue = ruta
            });

            fieldRuta.Value = ruta;

            fieldPuerta.Options = SearchPuerta(form, new FormSelectSearchContext()
            {
                SearchValue = puerta
            });

            fieldPuerta.Value = puerta;

            fieldVehiculo.Options = SearchVehiculo(form, new FormSelectSearchContext()
            {
                SearchValue = vehiculo
            });

            fieldVehiculo.Value = vehiculo;

            fieldTransportista.Options = SearchTransportista(form, new FormSelectSearchContext()
            {
                SearchValue = transportista
            });

            fieldTransportista.Value = transportista;

            if (!string.IsNullOrEmpty(vehiculo))
            {
                fieldMatricula.ReadOnly = true;
                fieldTransportista.ReadOnly = true;
            }

            // Predios
            FormField selectPredio = form.GetField("predio");
            selectPredio.Options = new List<SelectOption>();
            var dbQuery = new GetPrediosUsuarioQuery();
            uow.HandleQuery(dbQuery);

            var predios = dbQuery.GetPrediosUsuario(_identity.UserId).OrderBy(x => x.Numero);
            foreach (var pred in predios)
            {
                selectPredio.Options.Add(new SelectOption(pred.Numero, $"{pred.Numero} - {pred.Descripcion}"));
            }
        }

        #endregion
    }
}
