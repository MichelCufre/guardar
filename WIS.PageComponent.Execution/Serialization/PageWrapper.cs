using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common;
using WIS.Components.Common.Redirection;
using WIS.Components.Common.Select;
using WIS.Serialization;
using WIS.Serialization.Binders;

namespace WIS.PageComponent.Execution.Serialization
{
    public class PageWrapper : TransferWrapper, ITransferWrapper, IPageWrapper
    {
        public PageAction Action { get; set; }

        public PageWrapper() : base()
        {
        }

        public override ISerializationBinder GetSerializationBinder()
        {
            //Esto se define por seguridad, no se permite pasar tipos no esperados
            return new CustomSerializationBinder(new List<Type> {
                typeof(ApplicationNotification),
                typeof(ComponentParameter),
                typeof(ComponentContext),
                typeof(SelectOption),
                typeof(ConfirmMessage),
                typeof(PageContext),
                typeof(PageRedirection)
            });
        }
    }
}
