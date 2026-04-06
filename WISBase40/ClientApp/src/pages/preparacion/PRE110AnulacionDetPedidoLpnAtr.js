import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, Field,  StatusMessage} from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';
import { Container, Row, Col, Modal, Tab, Tabs, Button } from 'react-bootstrap';
import PRE110DetPedidoLpnAtr from './PRE110DetPedidoLpnAtr'

export default function PRE110AnulacionDetPedidoLpnAtr(props) {

    const { t } = useTranslation();
    const onAfterCommit = (context, rows, parameters, nexus) => {
        nexus.getGrid("PRE110AnulacionDetPedidoLpnAtr_grid_1").refresh();
    }
    const onAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("PRE110AnulacionDetPedidoLpnAtr_grid_1").refresh();
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
    const [_nexus, setnex] = useState(false);

    const addParameter = (context, data, nexus) => {

        if (props.Pedido !== undefined) {
        
            data.parameters = [
            { id: "Pedido", value: props.Pedido },
            { id: "Empresa", value: props.Empresa },
            { id: "Cliente", value: props.Cliente },
            { id: "IdEspecificaIdentificador", value: props.IdEspecificaIdentificador },
            { id: "IdLpnExteno", value: props.IdLpnExteno },
            { id: "LpnTipo", value: props.LpnTipo },
            { id: "Producto", value: props.Producto },
            { id: "Identificador", value: props.Identificador },
            { id: "Faixa", value: props.Faixa }

            ];
        }
    };
    const handleFormBeforeInitialize = (context, form, query, nexus) => {

        if (props.Pedido !== undefined) {
            query.parameters = [
                    { id: "Pedido", value: props.Pedido },
                    { id: "Empresa", value: props.Empresa },
                    { id: "Cliente", value: props.Cliente },
                    { id: "IdEspecificaIdentificador", value: props.IdEspecificaIdentificador },
                    { id: "IdLpnExteno", value: props.IdLpnExteno },
                    { id: "LpnTipo", value: props.LpnTipo },
                    { id: "Producto", value: props.Producto },
                    { id: "Identificador", value: props.Identificador },
                    { id: "Faixa", value: props.Faixa }
            ];
        }
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
        setNuDetPedSaiAtrib(context.parameters.find(f => f.id === "NU_DET_PED_SAI_ATRIB").value);
        setshowModalUpdateAtributo(true);
    }
    const closeFormDialogAtributoPedidoLpn = (datos) => {
        setshowModalUpdateAtributo(false);
    }

    return (

        <Page
            {...props}
        >
           
            <Modal.Header closeButton>
                <Modal.Title>{t("PRE110AnulacionDetPedidoLpnAtr_Sec0_modalTitle_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form
                    application="PRE110AnulacionDetPedidoLpnAtr"
                    onBeforeInitialize={handleFormBeforeInitialize}
                >

                    <Row>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="pedido">{t("PRE110AnulacionLpnAtr_frm_lbl_pedido")}</label>
                                <Field name="pedido" disabled={true} />
                                <StatusMessage for="pedido" />

                            </div>
                        </Col>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="codEmpresa">{t("PRE110AnulacionLpnAtr_frm_lbl_codEmpresa")}</label>
                                <Field name="codEmpresa" disabled={true} />
                                <StatusMessage for="codEmpresa" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="empresa">{t("PRE110AnulacionLpnAtr_frm_lbl_empresa")}</label>
                                <Field name="empresa" disabled={true} />
                                <StatusMessage for="empresa" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="codCliente">{t("PRE110AnulacionLpnAtr_frm_lbl_codCliente")}</label>
                                <Field name="codCliente" disabled={true} />
                                <StatusMessage for="codCliente" />

                            </div>
                        </Col>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="cliente">{t("PRE110AnulacionLpnAtr_frm_lbl_cliente")}</label>
                                <Field name="cliente" disabled={true} />
                                <StatusMessage for="cliente" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="codProducto">{t("PRE110AnulacionLpnAtr_frm_lbl_codProducto")}</label>
                                <Field name="codProducto" disabled={true} />
                                <StatusMessage for="codProducto" />

                            </div>
                        </Col>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="producto">{t("PRE110AnulacionLpnAtr_frm_lbl_producto")}</label>
                                <Field name="producto" disabled={true} />
                                <StatusMessage for="producto" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="identificador">{t("PRE110AnulacionLpnAtr_frm_lbl_identificador")}</label>
                                <Field name="identificador" disabled={true} />
                                <StatusMessage for="identificador" />
                            </div>
                        </Col>
                    </Row>
                </Form>


                <div className="row mb-4">
                    <div className="col-12">
                        <Grid id="PRE110AnulacionDetPedidoLpnAtr_grid_1"
                            application="PRE110AnulacionDetPedidoLpnAtr"
                            rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                            onAfterCommit={onAfterCommit}
                            onAfterMenuItemAction={onAfterMenuItemAction}
                            onBeforeInitialize={addParameter}
                            onBeforeCommit={addParameter}
                            onBeforeFetch={addParameter}
                            onBeforeFetchStats={addParameter}
                            onBeforeApplyFilter={addParameter}
                            onBeforeMenuItemAction={addParameter}
                            enableSelection
                            onAfterButtonAction={onAfterButtonAction}
                        />
                    </div>
                </div>
            </Modal.Body>
            <Modal show={showModalAtributoPedidoLpn} onHide={closeFormDialogAtributoPedidoLpn} dialogClassName="modal-70w" backdrop="static" >
                <PRE110DetPedidoLpnAtr show={showModalAtributoPedidoLpn} onHide={closeFormDialogAtributoPedidoLpn}
                    Pedido={_pedido} Empresa={_empresa} Cliente={_cliente} IdEspecificaIdentificador={_idEspecificaIdentificador}
                    IdLpnExteno={_idLpnExteno} LpnTipo={_lpnTipo} Producto={_producto} Identificador={_identificador} Faixa={_faixa} NuDetPedSaiAtrib={_nuDetPedSaiAtrib}
                />
            </Modal>
        </Page>
    );
}