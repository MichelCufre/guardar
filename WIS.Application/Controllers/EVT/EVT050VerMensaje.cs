using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.EVT
{
    public class EVT050VerMensaje : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;

        public EVT050VerMensaje(
            IIdentityService identity, 
            IUnitOfWorkFactory uowFactory)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            var nuNotificacion = context.GetParameter("notificacion");

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var notificacion = uow.NotificacionRepository.GetNotificacionEmail(long.Parse(nuNotificacion));

                form.GetField("destinatario").Value = notificacion.EmailRecibe;
                form.GetField("asunto").Value = notificacion.Asunto;
                form.GetField("cuerpo").Value = notificacion.Cuerpo;

                context.AddParameter("isHtmlBody", notificacion.IsHtml.ToString().ToLower());
            }

            form.GetField("notificacion").Value = nuNotificacion;

            return form;
        }
    }
}
