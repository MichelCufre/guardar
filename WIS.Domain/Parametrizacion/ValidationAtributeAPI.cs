using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using WIS.Common.API.Dtos;
using WIS.Domain.Validation;

namespace WIS.Domain.Parametrizacion
{
    public class ValidationAtributeAPI
    {
        public virtual void HttpPost(string valorAValidar, string URL, int idValidacion, out Error error)
        {
            error = null;
            try
            {
                using (var client = new HttpClient())
                {
                    AttributeValidationRequest requests = new AttributeValidationRequest();
                    requests.Value = valorAValidar;
                    requests.Id = idValidacion;

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var address = new Uri(URL);

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, address);
                    request.Method = HttpMethod.Post;
                    request.Content = new StringContent(JsonConvert.SerializeObject(requests), Encoding.UTF8, "application/json");

                    request.Headers.ConnectionClose = true;

                    var response = client.SendAsync(request).GetAwaiter().GetResult();

                    if (!response.IsSuccessStatusCode)
                        error = new Error("Error, status: " + response.StatusCode + " - " + response.ReasonPhrase + "-" + response.Content.ReadAsStringAsync());
                    else
                    {
                        string result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        var resultWrapper = JsonConvert.DeserializeObject<AttributeValidationResponse>(result);
                        if (!resultWrapper.IsValid)
                        {
                            string msg = resultWrapper.Error;
                            if (string.IsNullOrEmpty(msg))
                                msg = "General_lbl_Title_ErrorValidacion";

                            error = new Error(resultWrapper.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error = new Error(ex.Message ?? "");
            }
        }

    }
}
