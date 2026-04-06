using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using WIS.Serialization;
using WIS.Serialization.Binders;

namespace WIS.Webhook.Execution.Serialization
{
    public class WebhookWrapper : TransferWrapper, IWebhookWrapper
    {
        public override ISerializationBinder GetSerializationBinder()
        {
            return new CustomSerializationBinder(new List<Type> {
                typeof(WebhookRequest),
                typeof(WebhookContent)
            });
        }
    }
}
