import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Modal } from 'react-bootstrap';
import PRE110DetPedidoAtr from './PRE110DetPedidoAtr'

export default function PRE110AnulacionDetPedidoAtri(props) {

    const { t } = useTranslation();
    const onAfterCommit = (context, rows, parameters, nexus) => {
        nexus.getGrid("PRE110AnulacionDetPedidoAtri_grid_1").refresh();
    }
    const onAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("PRE110AnulacionDetPedidoAtri_grid_1").refresh();
    }
    const [showModalAtributoPedidoLpn, setshowModalUpdateAtributo] = useState(false);

    const [_pedido, setPedido] = useState(false);
    const [_empresa, setEmpresa] = useState(false);
    const [_cliente, setCliente] = useState(false);
    const [_idEspecificaIdentificador, setIdEspecificaIdentificador] = useState(false);
    const [_idLpnExteno, setIdLpnExteno] = useState(false);
    const [_lpnTipo, setLpnTipo] = useState(false);
    const [_producto, setProducto] = useState(false);
    const [_identificador, setIdentificador] = useState(false);
    const [_faixa, setFaixa] = useState(false);
    const [_nuDetPedSaiAtrib, setNuDetPedSaiAtrib] = useState(false);

    const onAfterButtonAction = (context, nexus) => {
        setPedido(context.parameters.find(f => f.id === "NU_PEDIDO").value);
        setEmpresa(context.parameters.find(f => f.id === "CD_EMPRESA").value);
        setCliente(context.parameters.find(f => f.id === "CD_CLIENTE").value);
        setIdEspecificaIdentificador(context.parameters.find(f => f.id === "ID_ESPECIFICA_IDENTIFICADOR").value);
        setProducto(context.parameters.find(f => f.id === "CD_PRODUTO").value);
        setIdentificador(context.parameters.find(f => f.id === "NU_IDENTIFICADOR").value);
        setFaixa(context.parameters.find(f => f.id === "CD_FAIXA").value);
        setNuDetPedSaiAtrib(context.parameters.find(f => f.id === "NU_DET_PED_SAI_ATRIB").value);
        setshowModalUpdateAtributo(true);
    }
    const closeFormDialogAtributoPedidoLpn = (datos) => {
        setshowModalUpdateAtributo(false);
    }
    return (

        <Page
            title={t("PRE110AnulacionDetPedidoAtri_page_lbl_Title")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="PRE110AnulacionDetPedidoAtri_grid_1"
                        application="PRE110AnulacionDetPedidoAtri"
                        rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                        onAfterCommit={onAfterCommit}
                        onAfterMenuItemAction={onAfterMenuItemAction}
                        enableSelection
                        onAfterButtonAction={onAfterButtonAction}

                    />
                </div>
            </div>
          
            <Modal show={showModalAtributoPedidoLpn} onHide={closeFormDialogAtributoPedidoLpn} dialogClassName="modal-70w" backdrop="static" >
                <PRE110DetPedidoAtr show={showModalAtributoPedidoLpn} onHide={closeFormDialogAtributoPedidoLpn}
                    Pedido={_pedido} Empresa={_empresa} Cliente={_cliente} IdEspecificaIdentificador={_idEspecificaIdentificador}
                    Producto={_producto} Identificador={_identificador} Faixa={_faixa} NuDetPedSaiAtrib={_nuDetPedSaiAtrib}
                />
            </Modal>
        </Page>
    );
}