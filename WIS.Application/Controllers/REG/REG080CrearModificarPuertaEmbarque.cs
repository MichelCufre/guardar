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
using WIS.Domain.Registro;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.REG
{
    public class REG080CrearModificarPuertaEmbarque : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly PuertaEmbarqueMapper _mapperPuerta;
        protected readonly IFormValidationService _formValidationService;

        public REG080CrearModificarPuertaEmbarque(IIdentityService identity, IUnitOfWorkFactory uowFactory, IFormValidationService formValidationService)
        {
            this._identity = identity;
            this._mapperPuerta = new PuertaEmbarqueMapper();
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            this.InicializarSelects(uow, form);

            if (query.Parameters.Count > 0)
            {
                if (!short.TryParse(query.Parameters.FirstOrDefault(x => x.Id == "idPuertaEmbarque")?.Value, out short idPuertaEmbarque))
                    throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

                var puerta = uow.PuertaEmbarqueRepository.GetPuertaEmbarque(idPuertaEmbarque);

                form.GetField("CD_PORTA").Value = puerta.Id.ToString();
                form.GetField("CD_PORTA").ReadOnly = true;

                if (puerta.Ubicacion != null)
                {
                    form.GetField("CD_ENDERECO").Value = puerta.Ubicacion.Id;
                    form.GetField("ID_BLOQUE").Value = puerta.Ubicacion.Bloque;
                }

                form.GetField("CD_ENDERECO").ReadOnly = true;
                form.GetField("ID_BLOQUE").ReadOnly = true;

                form.GetField("NU_PREDIO").Value = puerta.NumPredio.ToString();
                form.GetField("NU_PREDIO").ReadOnly = true;

                form.GetField("DS_PORTA").Value = puerta.Descripcion;
                form.GetField("CD_SITUACAO").Value = puerta.Estado?.ToString();
                form.GetField("TP_PUERTA").Value = puerta.Tipo;

                var fieldEmp = form.GetField("CD_EMPRESA");
                fieldEmp.Options = SearchEmpresa(form, new FormSelectSearchContext()
                {
                    SearchValue = puerta.Ubicacion.IdEmpresa.ToString()
                });

                fieldEmp.Value = puerta.Ubicacion.IdEmpresa.ToString();
            }
            else
            {
                if (this._identity.Predio != GeneralDb.PredioSinDefinir)
                    form.GetField("NU_PREDIO").Value = this._identity.Predio;

                form.GetField("CD_PORTA").ReadOnly = false;
                form.GetField("NU_PREDIO").ReadOnly = false;
                form.GetField("ID_BLOQUE").ReadOnly = false;

                FormField fieldEmpresa = form.GetField("CD_EMPRESA");

                fieldEmpresa.ReadOnly = false;

                Empresa empresa = uow.EmpresaRepository.GetEmpresaUnicaParaUsuario(_identity.UserId);

                if (empresa != null)
                {
                    fieldEmpresa.Options = SearchEmpresa(form, new FormSelectSearchContext()
                    {
                        SearchValue = empresa.Id.ToString()
                    });

                    fieldEmpresa.Value = empresa.Id.ToString();
                    fieldEmpresa.ReadOnly = true;
                }
            }

            return form;
        }
        public override Form FormSubmit(Form form, FormSubmitContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                var cdPorta = short.Parse(form.GetField("CD_PORTA").Value);
                var puerta = uow.PuertaEmbarqueRepository.GetPuertaEmbarque(cdPorta);

                if (puerta == null)
                    CrearPuertaEmbarque(uow, form);
                else
                    ModificarPuertaEmbarque(uow, form, puerta);

                uow.SaveChanges();
                uow.Commit();
                query.AddSuccessNotification("General_Db_Success_Update");
            }
            catch (ValidationFailedException ex)
            {
                uow.Rollback();
                query.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                uow.Rollback();
                query.AddErrorNotification(ex.Message);
            }
            return form;
        }
        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new REG080FormValidationModule(uow, this._identity), form, context);
        }
        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext query)
        {
            switch (query.FieldId)
            {
                case "CD_EMPRESA":
                    return this.SearchEmpresa(form, query);
                default:
                    return new List<SelectOption>();
            }
        }

        #region Metodos Auxiliares

        public virtual void InicializarSelects(IUnitOfWork uow, Form form)
        {
            FormField selectSituacion = form.GetField("CD_SITUACAO");
            FormField selectPredios = form.GetField("NU_PREDIO");
            FormField selectTpPuerta = form.GetField("TP_PUERTA");

            selectSituacion.Options = new List<SelectOption>();
            selectPredios.Options = new List<SelectOption>();
            selectTpPuerta.Options = new List<SelectOption>();


            //Situacion
            selectSituacion.Options = new List<SelectOption>
            {
                new SelectOption(SituacionDb.Activo.ToString(),"General_Sec0_Opt_SitActivo"),
                new SelectOption(SituacionDb.Inactivo.ToString(),"General_Sec0_Opt_SitInactivo"),
            };

            form.GetField("CD_SITUACAO").Value = SituacionDb.Activo.ToString();

            //Predio
            List<Predio> userPredios = uow.PredioRepository.GetPrediosUsuario(_identity.UserId);

            foreach (var predio in userPredios)
            {
                selectPredios.Options.Add(new SelectOption(predio.Numero, $"{predio.Numero} - {predio.Descripcion}"));
            }

            if (this._identity.Predio != GeneralDb.PredioSinDefinir)
                form.GetField("NU_PREDIO").Value = this._identity.Predio;

            //Situacion
            selectTpPuerta.Options = new List<SelectOption>
            {
                new SelectOption(TipoPuertaEmbarqueDb.Entrada,"General_Sec0_Opt_TipoPuertaEntrada"),
                new SelectOption(TipoPuertaEmbarqueDb.Salida,"General_Sec0_Opt_TipoPuertaSalida"),
                new SelectOption(TipoPuertaEmbarqueDb.EntradaSalida,"General_Sec0_Opt_TipoPuertaEntraSale"),
            };
        }

        public virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext FormQuery)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Empresa> empresas = uow.EmpresaRepository.GetByNombreOrCodePartial(FormQuery.SearchValue);

            foreach (Empresa empresa in empresas)
            {
                opciones.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}"));
            }

            return opciones;
        }

        public virtual void CrearPuertaEmbarque(IUnitOfWork uow, Form form)
        {
            var bloque = form.GetField("ID_BLOQUE").Value;
            var predio = form.GetField("NU_PREDIO").Value;
            var cdPorta = short.Parse(form.GetField("CD_PORTA").Value);
            var empresa = int.Parse(form.GetField("CD_EMPRESA").Value);

            var puertaConfiguracion = uow.PuertaEmbarqueRepository.GetConfiguracionPuertaEmbarque(empresa);

            ValidarParametrosConfiguracion(uow, puertaConfiguracion);

            var idUbicacion = (predio + bloque + puertaConfiguracion.PrefijoPuerta + cdPorta.ToString().PadLeft(3, '0'))?.ToUpper();

            var ubicacion = new Ubicacion
            {
                Id = idUbicacion,
                IdEmpresa = empresa,
                Bloque = bloque,
                NumeroPredio = predio,
                CodigoClase = puertaConfiguracion.Clase,
                IdProductoRotatividad = puertaConfiguracion.Rotatividad,
                IdProductoFamilia = puertaConfiguracion.FamiliaPrincipal,
                IdUbicacionTipo = TipoUbicacionDb.WIS,
                CodigoSituacion = SituacionDb.UbicacionVacia,
                EsUbicacionBaja = true,
                NecesitaReabastecer = false,
                FechaInsercion = DateTime.Now,
                FechaModificacion = DateTime.Now,
                IdUbicacionArea = AreaUbicacionDb.PuertaEmbarque,
                IdUbicacionZona = TipoUbicacionZonaDb.Normal,
                Profundidad = 1,
                CodigoBarras = $"{BarcodeDb.PREFIX_UBICACION}{idUbicacion}"
            };

            uow.UbicacionRepository.AddUbicacion(ubicacion);

            var puerta = new PuertaEmbarque()
            {
                Id = cdPorta,
                NumPredio = predio,
                Tipo = form.GetField("TP_PUERTA").Value,
                Descripcion = form.GetField("DS_PORTA").Value,
                Estado = short.Parse(form.GetField("CD_SITUACAO").Value),
                Ubicacion = ubicacion,
                CodigoUbicacion = ubicacion.Id,
                FechaAlta = DateTime.Now,
                FechaModificacion = DateTime.Now,
            };

            uow.PuertaEmbarqueRepository.AddPuertaEmbarque(puerta);
        }

        public virtual void ModificarPuertaEmbarque(IUnitOfWork uow, Form form, PuertaEmbarque puerta)
        {
            puerta.Descripcion = form.GetField("DS_PORTA").Value;
            puerta.Estado = short.Parse(form.GetField("CD_SITUACAO").Value);
            puerta.Tipo = form.GetField("TP_PUERTA").Value;

            var ubicacion = uow.UbicacionRepository.GetUbicacion(puerta.CodigoUbicacion);
            ubicacion.IdEmpresa = int.Parse(form.GetField("CD_EMPRESA").Value);

            uow.UbicacionRepository.UpdateUbicacion(ubicacion);
            uow.PuertaEmbarqueRepository.UpdatePuertaEmbarque(puerta);
        }

        public virtual void ValidarParametrosConfiguracion(IUnitOfWork uow, PuertaEmbarqueConfiguracion config)
        {
            if (!uow.ClaseRepository.AnyClase(config.Clase))
                throw new ValidationFailedException("REG080_Sec0_Error_ParamClaseIncorrecto");
            else if (!uow.ProductoFamiliaRepository.AnyFamiliaProducto(config.FamiliaPrincipal))
                throw new ValidationFailedException("REG080_Sec0_Error_ParamFamiliaIncorrecto");
            else if (!uow.ProductoRotatividadRepository.AnyProductoRotatividad(config.Rotatividad))
                throw new ValidationFailedException("REG080_Sec0_Error_ParamRotatividadIncorrecto");
        }

        #endregion
    }
}
