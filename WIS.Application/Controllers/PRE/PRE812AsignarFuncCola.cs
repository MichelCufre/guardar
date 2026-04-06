using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.Security;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.PRE
{
    public class PRE812AsignarFuncCola : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;

        protected readonly Logger _logger;

        public PRE812AsignarFuncCola(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity)
        {
            this._uowFactory = uowFactory;
            this._identity = identity;

            this._logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            this.InicializarSelect(form);

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            List<string[]> keysRowSelected = JsonConvert.DeserializeObject<List<string[]>>(context.Parameters.FirstOrDefault(x => x.Id == "ListaFilasSeleccionadas")?.Value);
            string usuario = form.GetField("usuario").Value;

            if (!string.IsNullOrEmpty(usuario))
            {
                using var uow = this._uowFactory.GetUnitOfWork();
                uow.BeginTransaction();

                try
                {
                    uow.CreateTransactionNumber("PRE812 Asignación de funcionarios");

                    if (uow.ManejoLpnRepository.AnyLpnActivo() || uow.ManejoLpnRepository.AnyLpnGeneradoConUbicacion())
                        throw new ValidationFailedException("PRE812_Grid_Error_ExisteLPNActivo");

                    keysRowSelected.ForEach(linea =>
                    {
                        string producto = linea[0];
                        int empresa = int.Parse(linea[1]);
                        string lote = linea[2];
                        decimal faixa = decimal.Parse(linea[3]);
                        int preparacion = int.Parse(linea[4]);
                        string pedido = linea[5];
                        string cliente = linea[6];
                        string ubicacion = linea[7];
                        int secPreparacion = int.Parse(linea[8]);

                        uow.ColaDeTrabajoRepository.AsignarFuncionarios(producto, empresa, lote, faixa, preparacion, pedido, cliente, ubicacion, secPreparacion, int.Parse(usuario));
                    });

                    uow.SaveChanges();
                    uow.Commit();
                    context.AddSuccessNotification("PRE812_Sec0_Msg_asociadosConExito");
                }
                catch (ValidationFailedException ex)
                {
                    _logger.Error(ex, "FormSubmit");
                    context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "FormSubmit");
                    context.AddErrorNotification("PRE812_Sec0_Msg_ErrorAlAsociar");
                }
            }
            else
                context.AddErrorNotification("PRE812_Sec0_Msg_ErrorSelecUsuario");
            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            return form;
        }

        public virtual void InicializarSelect(Form form)
        {
            FormField selectorUsuario = form.GetField("usuario");

            selectorUsuario.Options = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Usuario> usuarios;

            usuarios = uow.SecurityRepository.GetUsuariosByPredio(this._identity.Predio);
            form.GetField("usuario").ReadOnly = false;

            foreach (var usuario in usuarios)
            {
                selectorUsuario.Options.Add(new SelectOption(usuario.UserId.ToString(), $"{usuario.UserId} - {usuario.Name}"));
            }
        }
    }
}