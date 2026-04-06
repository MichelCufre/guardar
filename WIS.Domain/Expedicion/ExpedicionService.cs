using NLog;
using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Reportes;
using WIS.Domain.Services.Interfaces;
using WIS.Security;

namespace WIS.Domain.Expedicion
{
    public class ExpedicionService : IExpedicionService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IDapper _dapper;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();
        protected readonly IParameterService _parameterService;
        protected readonly IReportKeyService _reporteKeyService;
        protected readonly IIdentityService _identity;
        protected readonly IFactoryService _factoryService;
        protected readonly ITaskQueueService _taskQueue;
        protected readonly IBarcodeService _barcodeService;

        public ExpedicionService(IUnitOfWorkFactory uowFactory,
            IDapper dapper,
            IParameterService parameterService,
            IIdentityService identity,
            IReportKeyService reporteKeyService,
            IFactoryService factoryService,
            ITaskQueueService taskQueue,
            IBarcodeService barcodeService)
        {
            _uowFactory = uowFactory;
            _dapper = dapper;
            _parameterService = parameterService;
            _identity = identity;
            _factoryService = factoryService;
            _reporteKeyService = reporteKeyService;
            _taskQueue = taskQueue;
            _barcodeService = barcodeService;
        }

        public virtual void CierreCamionAuto()
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var keys = new List<string>();
                var camiones = uow.CamionRepository.GetCamionesPendienteCierre();

                foreach (var cdCamion in camiones)
                {
                    try
                    {
                        var camion = uow.CamionRepository.GetCamionWithCargas(cdCamion);
                        var logic = new CierreEgreso(uow, camion, _dapper, _parameterService, _identity, _factoryService, _reporteKeyService, _barcodeService, _taskQueue);

                        logic.CierreCamionAuto();
                        keys.Add(camion.Id.ToString());
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                        continue;
                    }
                }

                if (_taskQueue.IsEnabled())
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.ConfirmacionDePedido, keys);
            }
        }
    }
}
