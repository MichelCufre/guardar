import React, { useState } from 'react';
import { Modal, Col, Row, Button, Container } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { AddRemovePanel } from '../../components/AddRemovePanel';

export default function PRE100PedidoLpn(props) {
    const { t } = useTranslation();

    const [empresaNombre, setEmpresaNombre] = useState("");
    const [agenteDescripcion, setAgenteDescripcion] = useState("");
    const [agenteCodigo, setAgenteCodigo] = useState("");
    const [agenteTipo, setAgenteTipo] = useState("");

    const applyParameters = (context, data, nexus) => {

        data.parameters = [
            { id: "pedido", value: props.pedido },
            { id: "empresa", value: props.empresa },
            { id: "cliente", value: props.cliente }
        ];
    };

    const handleBeforeLoad = (data) => {
        data.parameters = [...data.parameters,
        { id: "pedido", value: props.pedido },
        { id: "empresa", value: props.empresa },
        { id: "cliente", value: props.cliente }
        ];
    }

    const handleAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("PRE100PedidoLpn_grid_1").refresh();
        nexus.getGrid("PRE100PedidoLpn_grid_2").refresh();
    }

    const handleAdd = (evt, nexus) => {
        nexus.getGrid("PRE100PedidoLpn_grid_1").triggerMenuAction("btnAgregar", false, evt.ctrlKey);
    };

    const handleRemove = (evt, nexus) => {
        nexus.getGrid("PRE100PedidoLpn_grid_2").triggerMenuAction("btnQuitar", false, evt.ctrlKey);
    };

    const handleClose = () => {
        props.onHide();
    };

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
                <Modal.Title>{t("PRE100PedidoLpn_Sec0_lbl_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Page
                    {...props}
                    application="PRE100PedidoLpn"
                    onBeforeLoad={handleBeforeLoad}
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
                                <Col lg={8}>
                                    <span> {`${agenteTipo}`}-{`${agenteCodigo}`}-{`${agenteDescripcion}`}  </span>
                                </Col>
                            </Row>
                        </Col>
                    </Row>

                    <hr />

                    <AddRemovePanel
                        onAdd={handleAdd}
                        onRemove={handleRemove}
                        from={(
                            <Grid
                                application="PRE100PedidoLpn"
                                id="PRE100PedidoLpn_grid_1"
                                rowsToFetch={30}
                                rowsToDisplay={15}
                                onBeforeInitialize={applyParameters}
                                onBeforeFetch={applyParameters}
                                onBeforeFetchStats={applyParameters}
                                onBeforeMenuItemAction={applyParameters}
                                onBeforeApplyFilter={applyParameters}
                                onBeforeApplySort={applyParameters}
                                onAfterMenuItemAction={handleAfterMenuItemAction}
                                onBeforeExportExcel={applyParameters}
                                onAfterInitialize={onAfterInitialize}
                                enableExcelExport
                                enableSelection
                            />
                        )}
                        to={(
                            <Grid
                                application="PRE100PedidoLpn"
                                id="PRE100PedidoLpn_grid_2"
                                rowsToFetch={30}
                                rowsToDisplay={15}
                                onBeforeInitialize={applyParameters}
                                onBeforeFetch={applyParameters}
                                onBeforeFetchStats={applyParameters}
                                onBeforeMenuItemAction={applyParameters}
                                onBeforeApplyFilter={applyParameters}
                                onBeforeApplySort={applyParameters}
                                onAfterMenuItemAction={handleAfterMenuItemAction}
                                onBeforeExportExcel={applyParameters}
                                enableExcelExport
                                enableSelection
                            />
                        )}
                    />
                </Page>
            </Modal.Body>
            <Modal.Footer>
                <Button onClick={handleClose} id="btnCerrar" variant="outline-secondary">{t("General_Sec0_btn_Cerrar")}</Button>
            </Modal.Footer>
        </Modal>
    );
}