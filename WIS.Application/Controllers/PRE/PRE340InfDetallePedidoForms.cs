using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Domain.DataModel;
using WIS.Domain.Picking.Dtos;
using WIS.Extension;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent.Build;
using WIS.GridComponent.Excel;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRE
{
    public class PRE340InfDetallePedidoForms : AppController
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
        protected List<SortCommand> DefaultSort { get; }
        public PRE340InfDetallePedidoForms(
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
        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            context.AddParameter("BTNID", "btnInfoCamionPedido");

            return form;
        }

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
            form.GetField("CD_SITUACAO").Value = Pedido.CD_SITUACAO.ToString();
            form.GetField("AUX_CD_SITUACAO").Value = Pedido.DS_SITUACAO;

            form.GetField("CD_CLIENTE").Value = Pedido.CD_CLIENTE;
            form.GetField("DS_CLIENTE").Value = Pedido.DS_CLIENTE;
            form.GetField("DT_ADDROW").Value = Pedido.DT_ADDROW.ToString("dd/MM/yyyy");
            form.GetField("CD_CONDICION_LIBERACION").Value = Pedido.CD_CONDICION_LIBERACION;
            form.GetField("CD_EMPRESA").Value = Pedido.CD_EMPRESA.ToString();
            form.GetField("DS_EMPRESA").Value = Pedido.NM_EMPRESA;
            form.GetField("DT_ENTREGA").Value = Pedido.DT_ENTREGA.ToString("dd/MM/yyyy");
            form.GetField("NU_ULT_PREPARACION").Value = Pedido.NU_ULT_PREPARACION.ToString();
            form.GetField("DS_ENDERECO").Value = Pedido.DS_ENDERECO;
            form.GetField("CD_ROTA").Value = Pedido.CD_ROTA.ToString();
            form.GetField("AUX_CD_ROTA").Value = Pedido.DS_ROTA;

            form.GetField("DS_ANEXO4").Value = Pedido.DS_ANEXO4;
            form.GetField("CD_TRANSPORTISTA").Value = Pedido.CD_TRANSPORTADORA.ToString();
            //      form.GetField("DT_FACTURACION").Value = Pedido.DT_FACTURACION.ToString("dd/MM/yyyy");
            return form;
        }

        #endregion

    }
}


