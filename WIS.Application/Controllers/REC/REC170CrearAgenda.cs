using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.CheckboxListComponent;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.Recepcion;
using WIS.Domain.Recepcion.Enums;
using WIS.Exceptions;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.REC
{
    public class REC170CrearAgenda : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IFormValidationService _formValidationService;

        public REC170CrearAgenda(IIdentityService identity, ITrafficOfficerService concurrencyControl, IUnitOfWorkFactory uowFactory, IFormValidationService formValidationService)
        {
            this._identity = identity;
            this._concurrencyControl = concurrencyControl;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            this.InicializarSelects(form);

            form.GetField("fechaEntrega").Value = DateTime.Now.ToIsoString();

            if (_identity.Predio != GeneralDb.PredioSinDefinir)
            {
                form.GetField("numeroPredio").Value = _identity.Predio;
                form.GetField("numeroPredio").ReadOnly = true;
            }

            form.GetField("autoCargarDetalle").Disabled = true;
            form.GetButton("btnSubmitConfirmarIrDetalle").Disabled = true;

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var tipoRecepcion = uow.RecepcionTipoRepository.GetRecepcionTipoByExterno(int.Parse(form.GetField("idEmpresa").Value), form.GetField("tipoRecepcionExterno").Value);

            var agenda = new Agenda()
            {
                IdEmpresa = int.Parse(form.GetField("idEmpresa").Value),
                CodigoInternoCliente = form.GetField("codigoInternoAgente").Value,
                TipoRecepcionInterno = tipoRecepcion.Tipo,
                Predio = form.GetField("numeroPredio").Value,
                PlacaVehiculo = form.GetField("placa").Value,
                Anexo1 = form.GetField("anexo1").Value,
                Anexo2 = form.GetField("anexo2").Value,
                Anexo3 = form.GetField("anexo3").Value,
                Anexo4 = form.GetField("anexo4").Value,
                Estado = EstadoAgenda.Abierta,
                FechaInsercion = DateTime.Now,
                FechaModificacion = DateTime.Now,
                CodigoOperacion = OperacionAgendaDb.RecepcionAgrupada,
                Averiado = false,
                EnviaDocumentacion = false,
                TipoRecepcion = tipoRecepcion,
                CargaDetalleAutomatica = bool.Parse(form.GetField("autoCargarDetalle").Value)
            };

            if (DateTime.TryParse(form.GetField("fechaEntrega")?.Value, this._identity.GetFormatProvider(), DateTimeStyles.None, out DateTime fechaEntrega))
                agenda.FechaEntrega = fechaEntrega;

            if (int.TryParse(form.GetField("funcionarioAsignado")?.Value, out int funcionario))
                agenda.IdFuncionarioAsignado = funcionario;

            var referenciasSeleccionadas = new List<int>();

            if (tipoRecepcion.TipoSeleccionReferencia == TipoSeleccionReferenciaDb.Bolsa)
            {
                agenda.NumeroDocumento = TipoSeleccionReferenciaDb.TextoBolsa;
            }
            else if (tipoRecepcion.TipoSeleccionReferencia == TipoSeleccionReferenciaDb.MonoSeleccion)
            {
                var idReferencia = form.GetField("referencia").Value;

                var referencia = uow.ReferenciaRecepcionRepository.GetReferencia(int.Parse(idReferencia));
                if (referencia != null)
                    agenda.NumeroDocumento = referencia.Numero;
                else
                    agenda.NumeroDocumento = idReferencia;

                referenciasSeleccionadas.Add(int.Parse(idReferencia));
            }
            else if (tipoRecepcion.TipoSeleccionReferencia == TipoSeleccionReferenciaDb.MultiSeleccion)
            {
                agenda.NumeroDocumento = TipoSeleccionReferenciaDb.TextoVarios;

                var listaJson = JsonConvert.DeserializeObject<List<CheckboxListItem>>(context.Parameters.FirstOrDefault(s => s.Id == "listaSeleccion").Value);

                if (listaJson == null || listaJson.Count == 0)
                    throw new ValidationFailedException("General_Sec0_Error_SinSeleccionReferencia");

                referenciasSeleccionadas = listaJson.Select(x => int.Parse(x.Id)).ToList();
            }
            else
            {
                agenda.NumeroDocumento = form.GetField("referenciaLibre").Value;
            }

            uow.CreateTransactionNumber($"{this._identity.Application} - Crear Agenda");
            uow.BeginTransaction();

            try
            {
                agenda.NumeroTransaccion = uow.GetTransactionNumber();

                uow.AgendaRepository.AddAgenda(agenda);

                agenda.SetCrearDetalleStrategy(uow, _concurrencyControl, referenciasSeleccionadas, new List<int>());
                agenda.CrearDetallesAgenda();

                uow.SaveChanges();
                uow.Commit();
            }
            catch (Exception ex)
            {
                uow.Rollback();

                throw ex;
            }

            context.Parameters?.Clear();
            context.Parameters.Add(new ComponentParameter("idAgenda", agenda.Id.ToString()));
            context.Parameters.Add(new ComponentParameter() { Id = "tipoRecepcion", Value = tipoRecepcion.TipoSeleccionReferencia.ToString() });

            context.AddSuccessNotification("REC170_Frm1_Succes_Creacion", new List<string> { agenda.Id.ToString() });

            return form;

        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new MantenimientoAgendaFormValidationModule(uow, this._identity.UserId, this._identity.Predio), form, context);
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "idEmpresa": return this.SearchEmpresa(form, context);
                case "codigoInternoAgente": return this.SearchAgente(form, context);
                case "funcionarioAsignado": return this.SearchFuncionario(form, context);
                case "referencia": return this.SearchReferencia(form, context);

                default: return new List<SelectOption>();
            }
        }

        #region Metodos Auxiliares

        public virtual void InicializarSelects(Form form)
        {
            var selectPredio = form.GetField("numeroPredio");            
            selectPredio.Options = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            // Predios
            var dbQuery = new GetPrediosUsuarioQuery();

            uow.HandleQuery(dbQuery);

            var predios = dbQuery.GetPrediosUsuario(_identity.UserId);
            foreach (var predio in predios)
            {
                selectPredio.Options.Add(new SelectOption(predio.Numero, $"{predio.Numero} - {predio.Descripcion}")); ;
            }

            if (!_identity.Predio.Equals(GeneralDb.PredioSinDefinir))
                selectPredio.Value = _identity.Predio;
            else if (predios.Count == 1)
                selectPredio.Value = predios.FirstOrDefault().Numero;
        }

        public virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var empresas = uow.EmpresaRepository.GetEmpresasUsuarioNoDocumentalesByNombreOrCodePartial(context.SearchValue);

            foreach (var empresa in empresas)
            {
                opciones.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchAgente(Form form, FormSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            if (int.TryParse(form.GetField("idEmpresa").Value, out int idEmpresa))
            {
                var dbQuery = new GetAgentesQuery(context.SearchValue, idEmpresa);
                uow.HandleQuery(dbQuery);

                var agentes = dbQuery.GetByDescripcionOrAgentePartial(context.SearchValue, idEmpresa);

                foreach (var agente in agentes)
                {
                    opciones.Add(new SelectOption(agente.CodigoInterno, $"{agente.Tipo} - {agente.Codigo} - {agente.Descripcion}"));
                }
            }
            else
            {
                form.GetField("idEmpresa").SetError("General_Sec0_Error_Error25");
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchFuncionario(Form form, FormSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            if (int.TryParse(form.GetField("idEmpresa").Value, out int idEmpresa))
            {
                var usuarios = uow.SecurityRepository.GetUsuariosByDescripcionOrUserNamePartial(context.SearchValue, idEmpresa);

                foreach (var usuario in usuarios)
                {
                    opciones.Add(new SelectOption(usuario.UserId.ToString(), $"{usuario.Username} - {usuario.Name}"));
                }
            }
            else
            {
                form.GetField("idEmpresa").SetError("General_Sec0_Error_Error25");
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchReferencia(Form form, FormSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            if (int.TryParse(form.GetField("idEmpresa").Value, out int idEmpresa))
            {
                if (!string.IsNullOrEmpty(form.GetField("codigoInternoAgente").Value))
                {
                    if (!string.IsNullOrEmpty(form.GetField("tipoRecepcionExterno").Value))
                    {
                        if (!string.IsNullOrEmpty(form.GetField("numeroPredio").Value))
                        {
                            // obtener el tipo de referencia
                            var tipoRecepcion = uow.RecepcionTipoRepository.GetRecepcionTipoByExterno(idEmpresa, form.GetField("tipoRecepcionExterno").Value);

                            var dbQuery = new GetReferenciasDisponiblesQuery(idEmpresa, tipoRecepcion.TipoReferencia, form.GetField("codigoInternoAgente").Value, form.GetField("numeroPredio").Value);
                            uow.HandleQuery(dbQuery);

                            var referencias = dbQuery.GetByMemoOrNumeroPartial(context.SearchValue);

                            foreach (var referencia in referencias)
                            {
                                opciones.Add(new SelectOption(referencia.Id.ToString(), $"{referencia.Numero} - {referencia.Memo}"));
                            }
                        }
                        else
                        {
                            form.GetField("numeroPredio").SetError("General_Sec0_Error_Error25");
                        }
                    }
                    else
                    {
                        form.GetField("tipoRecepcionExterno").SetError("General_Sec0_Error_Error25");
                    }
                }
                else
                {
                    form.GetField("codigoInternoAgente").SetError("General_Sec0_Error_Error25");
                }
            }
            else
            {
                form.GetField("idEmpresa").SetError("General_Sec0_Error_Error25");
            }

            return opciones;
        }

        #endregion
    }
}
