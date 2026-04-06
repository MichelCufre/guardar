import React, { useState } from 'react';
import { Modal, Col, Row, Button, Container } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';

export default function PRE100DetallePedido(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const [empresaNombre, setEmpresaNombre] = useState("");
    const [agenteDescripcion, setAgenteDescripcion] = useState("");
    const [agenteCodigo, setAgenteCodigo] = useState("");
    const [agenteTipo, setAgenteTipo] = useState("");

    const handleClose = () => {
        props.onHide();
    };

    const handleBeforeInitialize = (context, data, nexus) => {
        data.parameters = [
            { id: "pedido", value: props.pedido },
            { id: "empresa", value: props.empresa },
            { id: "cliente", value: props.cliente }
        ];
    };

    const handleBeforeCommit = (context, data, nexus) => {
        data.parameters = [
            { id: "pedido", value: props.pedido },
            { id: "empresa", value: props.empresa },
            { id: "cliente", value: props.cliente }
        ];
    };
    const onAfterCommit = (context, rows, parameters, nexus) => {
        nexus.getGrid("PRE100DetallePedido_grid_1").refresh();
    }
    const handleBeforeSelectSearch = (context, row, query, nexus) => {
        query.parameters = [
            { id: "pedido", value: props.pedido },
            { id: "empresa", value: props.empresa },
            { id: "cliente", value: props.cliente }
        ];
    }

    const handleBeforeValidateRow = (context, data, nexus) => {
        data.parameters = [
            { id: "pedido", value: props.pedido },
            { id: "empresa", value: props.empresa },
            { id: "cliente", value: props.cliente }
        ];
    }

    const handleOnBeforeApplySort = (context, data, nexus) => {
        data.parameters = [
            { id: "pedido", value: props.pedido },
            { id: "empresa", value: props.empresa },
            { id: "cliente", value: props.cliente }
        ];
    };

    const handleOnBeforeApplyFilter = (context, data, nexus) => {
        data.parameters = [
            { id: "pedido", value: props.pedido },
            { id: "empresa", value: props.empresa },
            { id: "cliente", value: props.cliente }
        ];
    };

    const handleBeforeImportExcel = (context, data, nexus) => {
        data.parameters = [
            { id: "pedido", value: props.pedido },
            { id: "empresa", value: props.empresa },
            { id: "cliente", value: props.cliente },
            { id: "importExcel", value: "true" }
        ];
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
                <Modal.Title>{t("PRE100DetallePedido_Sec0_modalTitle_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Page
                    application="PRE100DetallePedido"
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
                        <Col lg={3} >
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

                    <Grid
                        id="PRE100DetallePedido_grid_1"
                        application="PRE100DetallePedido"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        enableExcelImport
                        onBeforeInitialize={handleBeforeInitialize}
                        onBeforeSelectSearch={handleBeforeSelectSearch}
                        onBeforeValidateRow={handleBeforeValidateRow}
                        onBeforeCommit={handleBeforeCommit}
                        onAfterCommit={onAfterCommit}
                        onBeforeExportExcel={handleBeforeInitialize}
                        onBeforeFetch={handleBeforeInitialize}
                        onBeforeFetchStats={handleBeforeInitialize}
                        onBeforeApplySort={handleOnBeforeApplySort}
                        onBeforeApplyFilter={handleOnBeforeApplyFilter}
                        onBeforeImportExcel={handleBeforeImportExcel}
                        onBeforeGenerateExcelTemplate={handleBeforeImportExcel}
                        onAfterInitialize={onAfterInitialize}
                    />
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