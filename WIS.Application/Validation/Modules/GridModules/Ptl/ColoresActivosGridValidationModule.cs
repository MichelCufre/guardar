using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Ptl;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.Services.Interfaces;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Security;
using WIS.Session;
using WIS.TrafficOfficer;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Ptl
{
    public class ColoresActivosGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _formatProvider;
        protected readonly IIdentityService _identity;
        protected readonly ISessionAccessor _session;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IBarcodeService _barcodeService;

        public ColoresActivosGridValidationModule(IUnitOfWork uow, IIdentityService identity, ISessionAccessor session, ITrafficOfficerService concurrencyControl, IBarcodeService barcodeService)
        {
            _uow = uow;
            _formatProvider = identity.GetFormatProvider();
            _identity = identity;
            _session = session;
            _concurrencyControl = concurrencyControl;
            Schema = new GridValidationSchema
            {
                ["Contenedor"] = ValidateContenedor,
            };
            _barcodeService = barcodeService;
        }

        public virtual GridValidationGroup ValidateContenedor(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var preparacion = int.Parse(row.GetCell("Preparacion").Value);

            var rules = new List<IValidationRule>
            {
                new StringMaxLengthValidationRule(cell.Value, 16)
            };

            if (cell.Value != cell.Old) rules.Add(new PTL010NotificarPTLModalContenedorValidationRule(_uow, _identity, _session, _concurrencyControl, cell.Value, preparacion, _barcodeService));

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules
            };
        }
    }
}
