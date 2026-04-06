using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Domain.DataModel;
using WIS.Domain.Picking.Dtos;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent.Build;
using WIS.GridComponent.Excel;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRE
{
    public class PRE340InfDetallePedidoCamion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected List<string> GridKeys1 { get; }
        protected List<string> GridKeys2 { get; }
        protected List<string> GridKeys3 { get; }

        protected List<SortCommand> DefaultSort { get; }
        public PRE340InfDetallePedidoCamion(
          ISecurityService security,
          IUnitOfWorkFactory uowFactory,
          IIdentityService identity,
          IFormValidationService formValidationService,
          IGridService gridService,
          IGridExcelService excelService,
          IFilterInterpreter filterInterpreter)
        {

            _security = security;
            _uowFactory = uowFactory;
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));
            _formValidationService = formValidationService;
            _gridService = gridService;
            _excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        #region Form
        public override Form FormInitialize(Form form, FormInitializeContext context)
        {

            string pedido = context.GetParameter("NU_PEDIDO");
            int empresa;
            int.TryParse(context.GetParameter("CD_EMPRESA"), out empresa);
            string cliente = context.GetParameter("CD_CLIENTE");

            if (string.IsNullOrEmpty(pedido) ||
                empresa == 0 ||
                string.IsNullOrEmpty(cliente)
              )
                return form;


            using var uow = this._uowFactory.GetUnitOfWork();
            InfoPedidoPre340 Pedido = uow.EmpaquetadoPickingRepository.GetInfoPedido(pedido, cliente, empresa);

            form.GetField("NU_PEDIDO").Value = Pedido.NU_PEDIDO;
            form.GetField("TP_PEDIDO").Value = Pedido.TP_PEDIDO;


            form.GetField("CD_CLIENTE").Value = Pedido.CD_CLIENTE;
            form.GetField("DS_CLIENTE").Value = Pedido.DS_CLIENTE;

            form.GetField("CD_EMPRESA").Value = Pedido.CD_EMPRESA.ToString();
            form.GetField("DS_EMPRESA").Value = Pedido.NM_EMPRESA;
            return form;
        }

        #endregion
    }
}


