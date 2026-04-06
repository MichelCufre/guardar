import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import PRE110AnulacionDetPedidoLpnAtr from './PRE110AnulacionDetPedidoLpnAtr'
import { Modal } from 'react-bootstrap';
export default function PRE110AnulacionDetPedidoLpn(props) {

    const { t } = useTranslation();
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
    const [_nexus, setnex] = useState(false);
    const onAfterCommit = (context, rows, parameters, nexus) => {
        nexus.getGrid("PRE110AnulacionDetPedidoLpn_grid_1").refresh();
    }
    const onAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("PRE110AnulacionDetPedidoLpn_grid_1").refresh();
    }
    const closeFormDialogAtributoPedidoLpn = (datos) => {
        setshowModalUpdateAtributo(false);
        _nexus.getGrid("PRE110AnulacionDetPedidoLpn_grid_1").refresh();
    }
    const onAfterButtonAction = (context, nexus) => {
        setnex(nexus);
        setPedido(context.parameters.find(f => f.id === "NU_PEDIDO").value);
        setEmpresa(context.parameters.find(f => f.id === "CD_EMPRESA").value);
        setCliente(context.parameters.find(f => f.id === "CD_CLIENTE").value);
        setIdEspecificaIdentificador(context.parameters.find(f => f.id === "ID_ESPECIFICA_IDENTIFICADOR").value);
        setIdLpnExteno(context.parameters.find(f => f.id === "ID_LPN_EXTERNO").value);
        setLpnTipo(context.parameters.find(f => f.id === "TP_LPN_TIPO").value);
        setProducto(context.parameters.find(f => f.id === "CD_PRODUTO").value);
        setIdentificador(context.parameters.find(f => f.id === "NU_IDENTIFICADOR").value);
        setFaixa(context.parameters.find(f => f.id === "CD_FAIXA").value);
        setshowModalUpdateAtributo(true);
    }
    return (

        <Page
            title={t("PRE110Lpn_Sec0_pageTitle_Titulo")}
            application="PRE110AnulacionDetPedidoLpn"
            {...props}
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="PRE110AnulacionDetPedidoLpn_grid_1"
                        application= "PRE110AnulacionDetPedidoLpn"
                        rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                        onAfterCommit={onAfterCommit}
                        onAfterMenuItemAction={onAfterMenuItemAction}
                        onAfterButtonAction={onAfterButtonAction}
                        enableSelection
                    />
                </div>
            </div>

            <Modal show={showModalAtributoPedidoLpn} onHide={closeFormDialogAtributoPedidoLpn} dialogClassName="modal-90w" backdrop="static" >
                <PRE110AnulacionDetPedidoLpnAtr show={showModalAtributoPedidoLpn} onHide={closeFormDialogAtributoPedidoLpn}
                    Pedido={_pedido} Empresa={_empresa} Cliente={_cliente} IdEspecificaIdentificador={_idEspecificaIdentificador}
                    IdLpnExteno={_idLpnExteno} LpnTipo={_lpnTipo} Producto={_producto} Identificador={_identificador} Faixa={ _faixa}
                />
            </Modal>
        </Page>
    );
}