using Galys.Ptls;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using WIS.AutomationInterpreter.Interfaces;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.Integracion.Dtos;
using static Galys.Ptls.Client;
using static Galys.Ptls.PtlCommand;

namespace WIS.AutomationInterpreter.Services
{
    public class SmartLogPtlClientService : IPtlClientService
    {
        protected readonly AutomatismoClientService _automatismoClientService;
        protected readonly ILogger _logger;

        public SmartLogPtlClientService(ILogger logger,
            AutomatismoClientService automatismoClientService)
        {
            this._logger = logger;
            this._automatismoClientService = automatismoClientService;
        }

        private static readonly ConcurrentDictionary<string, Client> _instance = new ConcurrentDictionary<string, Client>();

        public Client GetInstance(string color, string idPtl = null, bool reset = false, string url = null)
        {
            var key = $"{idPtl}-{color}";
            if (_instance.ContainsKey(key))
                if (reset)
                    _instance.TryRemove(key, out Client c);
                else
                    return _instance.First(s => s.Key == key).Value;

            var split = url.Split(":");

            IPAddress ip = IPAddress.Parse(split[0]);
            int port = int.Parse(split[1]);

            Client newQueue = new Client(ip, port);

            if (!_instance.TryAdd(key, newQueue))
            {
                Console.WriteLine("Ya existe queue para el color: " + color);
                return GetInstance(color);
            }
            else
            {
                newQueue.Processed += PtlClient_Processed;
                newQueue.Queued += PtlClient_Queued;
                newQueue.ErrorReceived += PtlClient_ErrorReceived;
                newQueue.Confirmed += PtlClient_Confirmed;
                newQueue.BusyRejected += PtlClient_BusyRejected;
            }

            return newQueue;
        }
        public static string GetKey(object sender)
        {
            return _instance.First(s => Object.ReferenceEquals(s.Value, sender)).Key;
        }

        private void PtlClient_BusyRejected(object sender, PtlEventArgs e)
        {

        }

        private void PtlClient_Confirmed(object sender, PtlEventArgs e)
        {
            var key = GetKey(sender).Split("-");
            var color = key[1];
            var ipPtl = key[0];

            var res = _automatismoClientService.ConfirmCommand(new PtlCommandConfirmRequest
            {
                Address = e.Response.Address.ToString(),
                Cantidad = e.Response.Message,
                Color = color,
                CommandType = e.Response.Key == Galys.Ptls.PtlResponse.KeyPressed.Fn ? PtlTipoComandoDb.Cancelacion : PtlTipoComandoDb.Confirmacion,
                Id = ipPtl,
                DisplayText = e.Response.Key == Galys.Ptls.PtlResponse.KeyPressed.Fn ? PtlTipoComandoDb.Cancelacion : e.Response.Message
            });
        }

        private void PtlClient_ErrorReceived(object sender, PtlEventArgs e)
        {
            try
            {
                if (e.Response.Message == "Error al escuchar el socket")
                {
                    var key = GetKey(sender).Split("-");
                    var color = key[1];
                    var ipPtl = key[0];
                    GetInstance(color, ipPtl, true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        private void PtlClient_Queued(object sender, PtlEventArgs e)
        {

        }

        private void PtlClient_Processed(object sender, PtlEventArgs e)
        {

        }

        public PtlCommandResponse ResetOfOperation(AutomatismoInterpreterRequest request)
        {
            throw new NotImplementedException();
        }

        public PtlCommandResponse StartOfOperation(AutomatismoInterpreterRequest request)
        {
            throw new NotImplementedException();
        }

        public PtlCommandResponse TurnLigthOnOrOff(AutomatismoInterpreterRequest request)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<List<PtlCommandRequest>>(request.IntegracionServicioConexion.Contenido);

                foreach (var c in obj)
                {
                    switch (c.CommandType)
                    {
                        case PtlTipoComandoDb.PrenderLuz:

                            GetInstance(c.Color, c.Id, url: request.IntegracionServicio.UrlIntegracion).Display(new PtlCommand
                            {
                                Queue = true,
                                Address = int.Parse(c.Address),
                                BlinkLight = PtlBlink.Off,
                                BlinkText = PtlBlink.Off,
                                QueueLight = QueuedLight.BlinkFast,
                                Color = (PtlCommand.PtlColor)Enum.Parse(typeof(PtlCommand.PtlColor), c.Color),
                                Text = c.Text,
                                TextFn = c.TextFn,
                            });

                            break;
                        case PtlTipoComandoDb.ApagarLuz:

                            GetInstance(c.Color, c.Id).TurnOff(int.Parse(c.Address));

                            break;

                        default:
                            throw new Exception("CommandType not found");
                    }
                }
            }
            catch (Exception ex)
            {
                var response = new PtlCommandResponse();
                response.SetError(ex.Message);

                _logger.LogError(ex.Message, ex);

                return response;
            }

            return new PtlCommandResponse { };
        }
    }
}
