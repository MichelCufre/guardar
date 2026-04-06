using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.General;
using WIS.Domain.Impresiones;
using WIS.Domain.Services;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.REG
{
    public class REG010CrearEquipo : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IBarcodeService _barcodeService;
        protected readonly IFormValidationService _formValidationService;

        public REG010CrearEquipo(IUnitOfWorkFactory uowFactory, IIdentityService identity, IFormValidationService formValidationService, IBarcodeService barcodeService)
        {
            _uowFactory = uowFactory;
            _identity = identity;
            _formValidationService = formValidationService;
            _barcodeService = barcodeService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            form.GetField("descripcion").Value = string.Empty;
            form.GetField("tipo").Value = string.Empty;
            form.GetField("predio").Value = string.Empty;

            this.InicializarSelects(uow, form);
            return form;
        }
        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                CrearEquipo(uow, form);
                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");
            }
            catch (ValidationFailedException ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw ex;
            }
            return form;
        }
        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new CreateEquipoFormValidationModule(uow, this._identity.UserId, this._identity.Predio), form, context);
        }

        public virtual void InicializarSelects(IUnitOfWork uow, Form form)
        {
            // Predios
            FormField selectPredio = form.GetField("predio");
            selectPredio.Options = new List<SelectOption>();
            selectPredio.ReadOnly = false;

            var dbQuery = new GetPrediosUsuarioQuery();
            uow.HandleQuery(dbQuery);

            List<Predio> predios = dbQuery.GetPrediosUsuario(_identity.UserId);
            foreach (var predio in predios)
            {
                selectPredio.Options.Add(new SelectOption(predio.Numero, $"{predio.Numero} - {predio.Descripcion}"));
            }

            if (_identity.Predio != GeneralDb.PredioSinDefinir)
            {
                selectPredio.Value = _identity.Predio;
                selectPredio.ReadOnly = true;
            }
            else if (predios.Count == 1)
                selectPredio.Value = predios.FirstOrDefault().Numero;


            //Tipos de Herramientas
            FormField selectTipo = form.GetField("tipo");
            selectTipo.Options = new List<SelectOption>();

            var herramientas = uow.EquipoRepository.GetHerramientas(false);
            foreach (var h in herramientas)
            {
                selectTipo.Options.Add(new SelectOption(h.Id.ToString(), $"{h.Id} - {h.Descripcion}"));
            }

            if (herramientas.Count() == 1)
                selectTipo.Value = herramientas.FirstOrDefault().Id.ToString();
        }
        public virtual void CrearEquipo(IUnitOfWork uow, Form form)
        {
            var predio = form.GetField("predio").Value;
            var cdEquipo = uow.EquipoRepository.GetEquipoId();

            var cdEndereco = _barcodeService.GenerateBarcode(cdEquipo.ToString(), BarcodeDb.TIPO_ET_EQUIPO_MANUAL, predio: predio, prefijo: uow.ParametroRepository.GetParameter(ParamManager.PREFIJO_EQUIPO) ?? "");

            var ubicacion = new Ubicacion
            {
                Id = cdEndereco,
                IdEmpresa = 1,
                IdUbicacionTipo = 0,
                IdProductoRotatividad = 5,
                IdProductoFamilia = 1,
                CodigoClase = "GR",
                IdUbicacionArea = 21,
                CodigoSituacion = 15,
                EsUbicacionBaja = true,
                NecesitaReabastecer = false,
                FechaInsercion = DateTime.Now,
                FechaModificacion = DateTime.Now,
                IdUbicacionZona = TipoUbicacionZonaDb.Normal,
                NumeroPredio = predio,
            };

            uow.UbicacionRepository.AddUbicacion(ubicacion);

            var equipo = new Equipo
            {
                Id = cdEquipo,
                Descripcion = form.GetField("descripcion").Value,
                CodigoHerramienta = short.Parse(form.GetField("tipo").Value),
                CodigoUbicacion = ubicacion.Id,
                FechaInsercion = DateTime.Now,
                Aplicacion = _identity.Application,
            };

            uow.EquipoRepository.AddEquipo(equipo);
        }
    }
}
