using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Application.Validation;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.REC
{
    public class REC170SincronizarTracking : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;
        protected readonly ITrackingService _trackingService;

        public REC170SincronizarTracking(IUnitOfWorkFactory uowFactory, IIdentityService identity, ISecurityService security, IFormValidationService formValidationService, ITrackingService trackingService)
        {
            _uowFactory = uowFactory;
            _identity = identity;
            _security = security;
            _formValidationService = formValidationService;
            _trackingService = trackingService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int nuAgenda = int.Parse(context.GetParameter("nuAgenda"));

            var agenda = uow.AgendaRepository.GetAgenda(nuAgenda);
            var agente = uow.AgenteRepository.GetAgente(agenda.IdEmpresa, agenda.CodigoInternoCliente);

            context.AddParameter("tipoAgente", agente.Tipo);
            context.AddParameter("codigoAgente", agente.Codigo);
            context.AddParameter("codigoCliente", agente.CodigoInterno);

            FormField selectPuntosDeEntrega = form.GetField("puntosDeEntrega");
            selectPuntosDeEntrega.Options = new List<SelectOption>();

            var puntosDeEntrega = uow.TrackingRepository.GetPuntosEntregaCliente(agente.CodigoInterno, agente.Empresa);

            foreach (var p in puntosDeEntrega)
            {
                selectPuntosDeEntrega.Options.Add(new SelectOption(p.PuntoEntregaPedido, $"{p.PuntoEntregaPedido} - { p.DireccionPedido}"));
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                int nuAgenda = int.Parse(context.GetParameter("nuAgenda"));
                var agenda = uow.AgendaRepository.GetAgenda(nuAgenda);
                var puntoEntrega = form.GetField("puntosDeEntrega").Value;

                _trackingService.SincronizarDevolucion(uow, agenda, puntoEntrega);

                uow.CreateTransactionNumber($"{this._identity.Application} - Sincronizar tracking");

                agenda.NumeroTransaccion = uow.GetTransactionNumber();

                uow.AgendaRepository.UpdateAgenda(agenda);
                uow.SaveChanges();

                context.AddSuccessNotification("REC170_Sec0_Sucess_DevolucionSincronizada");
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                context.AddErrorNotification(ex.Message);
            }
            return form;
        }


    }
}
