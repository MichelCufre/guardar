using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto.Signers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WIS.Common.Extensions;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.API.Dtos;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Interfaces;
using WIS.Domain.Security;
using WIS.Domain.Services;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Validation
{
    public class EmpresaAccessValidation : ActionFilterAttribute
    {
        protected readonly bool _isGet = false;
        protected readonly bool _validarInterfazHabilitada = true;
        protected readonly int _interfazExterna;
        protected readonly string _parametroHabilitacion;
        protected readonly bool _isAutomatismo = false;

        public EmpresaAccessValidation(int interfazExterna, bool isGet, bool validarInterfazHabilitada = true)
        {
            _isGet = isGet;
            _interfazExterna = interfazExterna;
            _validarInterfazHabilitada = validarInterfazHabilitada;
        }

        public EmpresaAccessValidation(int interfazExterna, string parametroHabilitacion = null, bool isAutomatismo = false)
        {
            _interfazExterna = interfazExterna;
            _parametroHabilitacion = parametroHabilitacion;
            _isAutomatismo = isAutomatismo;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            object request = null;
            Error error = null;

            var empresa = -1;
            var isValid = true;
            var isBadRequest = false;
            var archivo = string.Empty;
            var idRequest = string.Empty;
            var dsReferencia = string.Empty;

            if (_isGet)
            {
                empresa = (int)context.ActionArguments["empresa"];
            }
            else
            {
                string body;
                context.HttpContext.Request.EnableBuffering();
                context.HttpContext.Request.Body.Position = 0;

                using (var reader = new StreamReader(context.HttpContext.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
                {
                    body = reader.ReadToEnd();
                    context.HttpContext.Request.Body.Position = 0;
                }

                if (!string.IsNullOrEmpty(body))
                {
                    var requestType = context.ActionDescriptor.EndpointMetadata
                      .OfType<SwaggerRequestType>()
                      .FirstOrDefault();

                    if (requestType != null)
                    {
                        try
                        {
                            request = JsonConvert.DeserializeObject(body, requestType.Type);
                        }
                        catch (JsonSerializationException ex)
                        {
                            isValid = false;
                            isBadRequest = true;
                            error = new Error("WMSAPI_msg_Error_ModelStateValidation", ex.Message);
                        }
                        catch (JsonReaderException ex)
                        {
                            isValid = false;
                            isBadRequest = true;
                            error = new Error("WMSAPI_msg_Error_ModelStateValidation", ex.Message);
                        }
                    }
                    else
                        request = context.ActionArguments["request"];

                    if (request != null)
                    {
                        empresa = (int)request.GetType().GetProperty("Empresa").GetValue(request);

                        if (request.GetType().GetProperty("DsReferencia") != null)
                            dsReferencia = (string)request.GetType().GetProperty("DsReferencia").GetValue(request);

                        if (request.GetType().GetProperty("Archivo") != null)
                            archivo = (string)request.GetType().GetProperty("Archivo").GetValue(request);

                        if (request.GetType().GetProperty("IdRequest") != null)
                            idRequest = (string)request.GetType().GetProperty("IdRequest").GetValue(request);
                    }
                    else if (isBadRequest)
                    {
                        var jObject = JObject.Parse(body);

                        empresa = JsonExtension.GetField(jObject, "Empresa")?.Value<int>() ?? 0;
                        dsReferencia = JsonExtension.GetField(jObject, "DsReferencia")?.Value<string>();
                        idRequest = JsonExtension.GetField(jObject, "IdRequest")?.Value<string>();

                        request = jObject;
                    }
                    else
                    {
                        isValid = false;
                        error = new Error("WMSAPI_msg_Error_DatosRequeridos");
                    }
                }
                else
                {
                    isValid = false;
                    error = new Error("WMSAPI_msg_Error_DatosRequeridos");
                }
            }

            var loginName = context.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var usuario = request?.GetType()?.GetProperty("Usuario");

            if (usuario != null && !(usuario.GetValue(request) is UsuarioRequest))
                usuario = null;

            var uowFactory = context.HttpContext.RequestServices.GetService(typeof(IUnitOfWorkFactory)) as IUnitOfWorkFactory;
            var validationService = context.HttpContext.RequestServices.GetService(typeof(IValidationService)) as IValidationService;

            using (var uow = uowFactory.GetUnitOfWork())
            {
                if (isValid && !uow.EmpresaRepository.AnyEmpresa(empresa))
                {
                    isValid = false;
                    error = new Error("WMSAPI_msg_Error_ExisteEmpresaValidation", empresa);
                }

                if (isValid && usuario != null)
                {
                    var usuarioRequest = usuario.GetValue(request) as UsuarioRequest;

                    if (SecurityLogic.IsValidUser(uow, usuarioRequest.LoginName, usuarioRequest.Hash))
                        loginName = usuarioRequest.LoginName;
                    else
                    {
                        isValid = false;
                        error = new Error("WMSAPI_msg_Error_UsuarioInvalido", loginName);
                    }
                }

                if (isValid)
                {
                    var interfazHabilitada = ParamManager.GetParamInterfazHabilitada(uow, _interfazExterna, empresa, _parametroHabilitacion);

                    if (!interfazHabilitada && _validarInterfazHabilitada)
                    {
                        isValid = false;
                        error = new Error("WMSAPI_msg_Error_EmpresaInterfazInhabilitada", _interfazExterna, empresa);

                    }
                    else if (!uow.EmpresaRepository.EmpresaAsignada(empresa, loginName).Result)
                    {
                        isValid = false;
                        error = new Error("WMSAPI_msg_Error_EmpresaNoAsignada", loginName, empresa);

                    }
                }

                if (!isValid)
                {
                    if (isBadRequest)
                    {
                        var errores = error.Argumentos.Select(a => a?.ToString()).ToList();                        
                        var nuInterfazEjecucion = Validations.GuardarError(uow, loginName, empresa, idRequest, _interfazExterna, dsReferencia, request, errores);

                        var problemDetails = new ProblemDetails()
                        {
                            Instance = context.HttpContext.Request.Path,
                            Status = StatusCodes.Status400BadRequest,
                            Type = $"https://tools.ietf.org/html/rfc7231#section-6.5.1",
                            Title = validationService.Translate(new Error("WMSAPI_msg_Error_ModelStateValidation")),
                            Detail = JsonConvert.SerializeObject(errores),
                        };

                        problemDetails.Extensions["NumeroInterfazEjecucion"] = nuInterfazEjecucion;
                        context.Result = new ObjectResult(problemDetails);
                    }
                    else
                        context.Result = new ObjectResult(validationService.Translate(error)) { StatusCode = StatusCodes.Status401Unauthorized };
                }
                else
                    base.OnActionExecuting(context);
            }
        }

    }
}
