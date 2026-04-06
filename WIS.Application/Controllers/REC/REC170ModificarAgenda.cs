using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
using WIS.Domain.General;
using WIS.Domain.General.Auxiliares;
using WIS.Domain.Recepcion;
using WIS.Domain.Security;
using WIS.Exceptions;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.REC
{
    public class REC170ModificarAgenda : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;

        public REC170ModificarAgenda(IIdentityService identity, IUnitOfWorkFactory uowFactory, IFormValidationService formValidationService)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var idAgenda = context.GetParameter("keyAgenda");

            if (!string.IsNullOrEmpty(idAgenda))
            {
                var agenda = uow.AgendaRepository.GetAgenda(int.Parse(idAgenda));

                if (agenda == null)
                    throw new ValidationFailedException("REC170_Frm1_Error_AgendaNoExiste", new string[] { idAgenda });

                this.InicializarSelects(form, agenda);

                this.InicializarCamposUpdate(uow, form, context, agenda);
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var idAgenda = context.GetParameter("keyAgenda");

            var agenda = uow.AgendaRepository.GetAgenda(int.Parse(idAgenda));

            if (agenda == null)
                throw new ValidationFailedException("REC170_Frm1_Error_AgendaNoExiste", new string[] { idAgenda });

            uow.CreateTransactionNumber($"{this._identity.Application} - Modificar Agenda");

            if (agenda.PuedeSerEditada())
            {
                agenda.PlacaVehiculo = form.GetField("placa").Value;

                if (DateTime.TryParse(form.GetField("fechaEntrega")?.Value, this._identity.GetFormatProvider(), DateTimeStyles.None, out DateTime fechaEntrega))
                    agenda.FechaEntrega = fechaEntrega;

            }

            if (int.TryParse(form.GetField("funcionarioAsignado")?.Value, out int funcionario))
                agenda.IdFuncionarioAsignado = funcionario;
            else
                agenda.IdFuncionarioAsignado = null;

            agenda.Anexo1 = form.GetField("anexo1").Value;
            agenda.Anexo2 = form.GetField("anexo2").Value;
            agenda.Anexo3 = form.GetField("anexo3").Value;
            agenda.Anexo4 = form.GetField("anexo4").Value;
            agenda.NumeroTransaccion = uow.GetTransactionNumber();

            uow.AgendaRepository.UpdateAgenda(agenda);
            uow.SaveChanges();

            context.AddSuccessNotification("REC170_Frm1_Succes_Edicion", new List<string> { agenda.Id.ToString() });

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new MantenimientoAgendaFormValidationModule(uow, this._identity.UserId, this._identity.Predio), form, context);
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            var idAgenda = context.GetParameter("keyAgenda");

            switch (context.FieldId)
            {
                case "idEmpresa": return this.SearchEmpresa(form, context);
                case "codigoInternoAgente": return this.SearchAgente(form, context);
                case "funcionarioAsignado": return this.SearchFuncionario(form, context);
                case "referencia": return this.SearchReferencia(form, context, int.Parse(idAgenda));

                default: return new List<SelectOption>();
            }
        }

        #region Metodos Auxiliares

        public virtual void InicializarSelects(Form form, Agenda agenda)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var selectPredio = form.GetField("numeroPredio");
            selectPredio.Options = new List<SelectOption>();

            // Predios
            var dbQuery = new GetPrediosUsuarioQuery();
            uow.HandleQuery(dbQuery);

            var predios = dbQuery.GetPrediosUsuario(_identity.UserId);
            foreach (var predio in predios)
            {
                selectPredio.Options.Add(new SelectOption(predio.Numero, $"{predio.Numero} - {predio.Descripcion}")); ;
            }

            if (predios.Count == 1)
                selectPredio.Value = predios.FirstOrDefault().Numero;

            var tipoAgente = uow.AgenteRepository.GetTipoAgente(agenda.CodigoInternoCliente);

            // Tipos de recepción
            var selectTipoExterno = form.GetField("tipoRecepcionExterno");
            selectTipoExterno.Options = new List<SelectOption>();

            var tiposRecepcionExterno = uow.RecepcionTipoRepository.GetRecepcionTiposEmpresaHabilitados(agenda.IdEmpresa, tipoAgente);

            foreach (var tipo in tiposRecepcionExterno)
            {
                selectTipoExterno.Options.Add(new SelectOption(tipo.TipoExterno, $"{tipo.TipoExterno} - {tipo.DescripcionExterna}"));
            }

            if (tiposRecepcionExterno.Count == 1)
                selectTipoExterno.Value = tiposRecepcionExterno.FirstOrDefault().TipoExterno;
        }

        public virtual void InicializarCamposUpdate(IUnitOfWork uow, Form form, FormInitializeContext context, Agenda agenda)
        {
            // Marcar campos solo de lectura
            form.GetField("idEmpresa").ReadOnly = true;
            form.GetField("codigoInternoAgente").ReadOnly = true;
            form.GetField("numeroPredio").ReadOnly = true;
            form.GetField("tipoRecepcionExterno").ReadOnly = true;
            form.GetField("autoCargarDetalle").Disabled = true;
            form.GetField("referenciaLibre").ReadOnly = true;
            form.GetField("referencia").ReadOnly = true;

            if (!agenda.PuedeSerEditada())
            {
                form.GetField("fechaEntrega").ReadOnly = true;
                form.GetField("placa").ReadOnly = true;
            }

            if (agenda.EnEstadoCerrada())
                form.GetField("funcionarioAsignado").ReadOnly = true;

            if (agenda.EnEstadoCancelada())
            {
                form.GetField("funcionarioAsignado").ReadOnly = true;
                form.GetField("anexo1").ReadOnly = true;
                form.GetField("anexo2").ReadOnly = true;
                form.GetField("anexo3").ReadOnly = true;
                form.GetField("anexo4").ReadOnly = true;
            }

            // Cargar valores iniciales
            form.GetField("fechaEntrega").Value = agenda.FechaEntrega.ToIsoString();
            form.GetField("placa").Value = agenda.PlacaVehiculo;
            form.GetField("numeroPredio").Value = agenda.Predio;
            form.GetField("referenciaLibre").Value = agenda.NumeroDocumento;

            // Carga de search Empresa
            var fieldEmpresa = form.GetField("idEmpresa");
            fieldEmpresa.Options = SearchEmpresa(form, new FormSelectSearchContext()
            {
                SearchValue = agenda.IdEmpresa.ToString()
            });

            fieldEmpresa.Value = agenda.IdEmpresa.ToString();

            // Carga de search Agente
            var fieldAgente = form.GetField("codigoInternoAgente");
            fieldAgente.Options = SearchAgente(form, new FormSelectSearchContext()
            {
                SearchValue = agenda.CodigoInternoCliente.ToString()
            });

            fieldAgente.Value = agenda.CodigoInternoCliente.ToString();

            // Carga de search funcionario asignado
            var fieldFuncionario = form.GetField("funcionarioAsignado");

            fieldFuncionario.Options = SearchFuncionario(form, new FormSelectSearchContext()
            {
                SearchValue = agenda.IdFuncionarioAsignado?.ToString()
            }, inicializarFormulario: true);

            fieldFuncionario.Value = agenda.IdFuncionarioAsignado?.ToString();


            form.GetField("autoCargarDetalle").Value = agenda.CargaDetalleAutomatica.ToString();

            var recepcionTipo = uow.RecepcionTipoRepository.GetRecepcionTipoExternoByInterno(agenda.IdEmpresa, agenda.TipoRecepcionInterno);

            form.GetField("tipoRecepcionExterno").Value = recepcionTipo.TipoExterno;

            context.Parameters.Add(new ComponentParameter() { Id = "tipoSeleccion", Value = recepcionTipo.RecepcionTipoInterno.TipoSeleccionReferencia.ToString() });

            if (recepcionTipo.RecepcionTipoInterno.TipoSeleccionReferencia == TipoSeleccionReferenciaDb.MultiSeleccion)
            {
                CargarReferenciasParaSeleccionMultiple(uow, agenda, context.Parameters, recepcionTipo.RecepcionTipoInterno.TipoReferencia);
            }
            else if (recepcionTipo.RecepcionTipoInterno.TipoSeleccionReferencia == TipoSeleccionReferenciaDb.MonoSeleccion)
            {
                // Carga de search funcionario asignado
                var fieldReferencia = form.GetField("referencia");
                fieldReferencia.Options = SearchReferencia(form, new FormSelectSearchContext()
                {
                    SearchValue = agenda.NumeroDocumento.ToString()
                }, agenda.Id);

                var op = fieldReferencia.Options.FirstOrDefault();
                var referencia = uow.ReferenciaRecepcionRepository.GetReferencia(int.Parse(op.Value));

                if (referencia != null)
                    fieldReferencia.Value = referencia.Id.ToString();
            }

            // Se deshailita para que pase la validación
            form.GetField("referencia").Disabled = true;

            form.GetField("anexo1").Value = agenda.Anexo1;
            form.GetField("anexo2").Value = agenda.Anexo2;
            form.GetField("anexo3").Value = agenda.Anexo3;
            form.GetField("anexo4").Value = agenda.Anexo4;

        }

        public virtual void CargarReferenciasParaSeleccionMultiple(IUnitOfWork uow, Agenda agenda, List<ComponentParameter> parameters, string tipoReferencia)
        {
            // Cargar referencias seleccionadas
            var listCheckSeleccinado = new List<CheckboxListItem>();

            if (parameters.Any(d => d.Id == "keyAgenda"))
            {
                var dbQuerySeleccionadas = new ReferenciaRecepcionQuery(agenda.Id);
                uow.HandleQuery(dbQuerySeleccionadas);
                var seleccionadas = dbQuerySeleccionadas.GetReferenciasAsociadas();

                foreach (var referencia in seleccionadas)
                {
                    listCheckSeleccinado.Add(new CheckboxListItem
                    {
                        Id = referencia.Id.ToString(),
                        Label = referencia.ToString(),
                        Selected = true
                    });
                }
            }

            parameters.Add(new ComponentParameter("referenciasAsociadas", JsonConvert.SerializeObject(listCheckSeleccinado)));
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

        public virtual List<SelectOption> SearchAgente(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            if (int.TryParse(form.GetField("idEmpresa").Value, out int idEmpresa))
            {

                var dbQuery = new GetAgentesQuery(context.SearchValue, idEmpresa);
                uow.HandleQuery(dbQuery);

                List<AgenteAuxiliar> agentes = dbQuery.GetByDescripcionOrAgentePartial(context.SearchValue, idEmpresa);

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

        public virtual List<SelectOption> SearchFuncionario(Form form, FormSelectSearchContext context, bool inicializarFormulario = false)
        {
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            if (int.TryParse(form.GetField("idEmpresa").Value, out int idEmpresa))
            {
                var usuarios = new List<Usuario>();

                if (inicializarFormulario && int.TryParse(context.SearchValue, out int funcionario))
                    usuarios = uow.SecurityRepository.GetUsuariosById(funcionario, idEmpresa);
                else if (!inicializarFormulario)
                    usuarios = uow.SecurityRepository.GetUsuariosByDescripcionOrUserNamePartial(context.SearchValue, idEmpresa);

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

        public virtual List<SelectOption> SearchReferencia(Form form, FormSelectSearchContext context, int nuAgenda)
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
                            var dbQuery = new ReferenciaRecepcionQuery(nuAgenda);
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
