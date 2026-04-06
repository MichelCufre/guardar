using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.Impresiones;
using WIS.Domain.Services.Interfaces;

namespace WIS.TicketPrintingProcess.Services
{
    public class ApplicationControl : IApplicationControl
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IPrintingService _printingService;
        private readonly ILogger<ApplicationControl> _logger;

        public ApplicationControl(IUnitOfWorkFactory uowFactory, 
            IPrintingService printingService, 
            ILogger<ApplicationControl> logger)
        {
            this._uowFactory = uowFactory;
            this._printingService = printingService;
            this._logger = logger;
        }

        public async Task StartAsync()
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                this._logger.LogDebug($"Iniciando proceso");

                List<Impresion> impresiones = await uow.ImpresionRepository.GetImpresionesPendientes();

                foreach (var impresion in impresiones)
                {
                    this._logger.LogDebug($"Procesando impresión {impresion.Id}");

                    try
                    {
                        await this._printingService.SendToPrintAsync(uow, impresion, CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(ex, $"Error al procesar la impresión {impresion.Id}");
                    }
                }

                this._logger.LogDebug($"Proceso finalizado");
            }
        }
    }
}
