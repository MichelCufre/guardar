import React, { useState } from 'react';
import { Modal, Col, Row, Button, Container } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import PRE100DetallePedidoLpnAtributo from './PRE100DetallePedidoLpnAtributo';

export default function PRE100DetallePedidoLpn(props) {
    const { t } = useTranslation();

    const [empresaNombre, setEmpresaNombre] = useState("");
    const [agenteDescripcion, setAgenteDescripcion] = useState("");
    const [agenteCodigo, setAgenteCodigo] = useState("");
    const [agenteTipo, setAgenteTipo] = useState("");

    const [datos, setDatos] = useState(null);
    const [showDetallesAtributoLpnPopup, setShowDetallesAtributoLpnPopup] = useState(false);

    const handleClose = () => {
        props.onHide();
    };

    const applyParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "pedido", value: props.pedido },
            { id: "empresa", value: props.empresa },
            { id: "cliente", value: props.cliente }
        ];
    };
    const onBeforeSelectSearch = (context, row, query, nexus) => {
        query.parameters = [
            { id: "pedido", value: props.pedido },
            { id: "empresa", value: props.empresa },
            { id: "cliente", value: props.cliente }
        ];
    };

    const onBeforeCommit = (context, data, nexus) => {
        data.parameters = [
            { id: "pedido", value: props.pedido },
            { id: "empresa", value: props.empresa },
            { id: "cliente", value: props.cliente },
        ];
    }
    const onAfterCommit = (context, rows, parameters, nexus) => {

        if (context.status !== "ERROR") {
            nexus.getGrid("PRE100DetallePedidoLpn_grid_1").refresh();
        }
    }

    const handleBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnDetalleLpnAtributo") {
            context.abortServerCall = true;

            setDatos(
                [
                    { id: "pedido", value: props.pedido },
                    { id: "empresa", value: props.empresa },
                    { id: "cliente", value: props.cliente },
                    { id: "producto", value: data.row.cells.find(d => d.column === "CD_PRODUTO").value },
                    { id: "faixa", value: data.row.cells.find(d => d.column === "CD_FAIXA").value },
                    { id: "identificador", value: data.row.cells.find(d => d.column === "NU_IDENTIFICADOR").value },
                    { id: "idEspecificaIdentificador", value: data.row.cells.find(d => d.column === "ID_ESPECIFICA_IDENTIFICADOR").value },
                    { id: "tipoLpn", value: data.row.cells.find(d => d.column === "TP_LPN_TIPO").value },
                    { id: "idExternoLpn", value: data.row.cells.find(d => d.column === "ID_LPN_EXTERNO").value },
                ]
            );
            setShowDetallesAtributoLpnPopup(true);
        }
    }

    const closeDetallesAtributoLpnFormDialog = () => {
        setShowDetallesAtributoLpnPopup(false);
    }

    const onAfterInitialize = (context, grid, parameters, nexus) => {

        if (parameters.find(s => s.id === "empresaNombre")) {
            setEmpresaNombre(parameters.find(d => d.id === "empresaNombre").value);
            setAgenteDescripcion(parameters.find(d => d.id === "agenteDescripcion").value);
            setAgenteCodigo(parameters.find(d => d.id === "agenteCodigo").value);
            setAgenteTipo(parameters.find(d => d.id === "agenteTipo").value);
        }
    };

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-70w" backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title>{t("PRE100DetallePedidoLpn_Sec0_modalTitle_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Page
                    application="PRE100DetallePedidoLpn"
                    {...props}
                >

                    <Row>
                        <Col lg={5}>
                            <Row >
                                <Col lg={2}>
                                    <span style={{ fontWeight: "bold" }}> {t("PRE100_frm1_lbl_pedido")}: </span>
                                </Col>
                                <Col lg={10}>
                                    <span >{`${props.pedido}`}</span>
                                </Col>
                            </Row>
                        </Col>
                        <Col lg={3}>
                            <Row>
                                <Col lg={4}>
                                    <span style={{ fontWeight: "bold" }}>{t("PRE100_frm1_lbl_empresa")}: </span>
                                </Col>
                                <Col lg={6}>
                                    <span> {`${props.empresa}`} - {`${empresaNombre}`}</span>
                                </Col>
                            </Row>
                        </Col>
                        <Col lg={4}>
                            <Row>
                                <Col lg={4}>
                                    <span style={{ fontWeight: "bold" }}>{t("PRE100_frm1_lbl_cliente")}:</span>
                                </Col>
                                <Col lg={6}>
                                    <span> {`${agenteTipo}`}-{`${agenteCodigo}`}-{`${agenteDescripcion}`}  </span>
                                </Col>
                            </Row>
                        </Col>
                    </Row>

                    <hr />

                    <Grid
                        id="PRE100DetallePedidoLpn_grid_1"
                        application="PRE100DetallePedidoLpn"
                        onBeforeInitialize={applyParameters}
                        onBeforeSelectSearch={onBeforeSelectSearch}
                        onBeforeValidateRow={applyParameters}
                        onBeforeCommit={onBeforeCommit}
                        onAfterCommit={onAfterCommit}
                        onBeforeExportExcel={applyParameters}
                        onBeforeFetch={applyParameters}
                        onBeforeFetchStats={applyParameters}
                        onBeforeApplySort={applyParameters}
                        onBeforeApplyFilter={applyParameters}
                        onBeforeButtonAction={handleBeforeButtonAction}
                        onAfterInitialize={onAfterInitialize}
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                    />
                    <PRE100DetallePedidoLpnAtributo show={showDetallesAtributoLpnPopup} onHide={closeDetallesAtributoLpnFormDialog} datos={datos} />

                </Page>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="outline-secondary" onClick={handleClose}>
                    {t("General_Sec0_btn_Cerrar")}
                </Button>
            </Modal.Footer>
        </Modal>
    );
}