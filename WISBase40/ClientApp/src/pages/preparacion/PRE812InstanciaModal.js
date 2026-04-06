import React, { useState } from 'react';
import { Button, Col, FormGroup, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, Form, StatusMessage } from '../../components/FormComponents/Form';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { withPageContext } from '../../components/WithPageContext';

function InternalPRE812InstanciaModal(props) {
    const { t } = useTranslation();

    const [empresaKey, setEmpresaKey] = useState(null);
    const [clienteKey, setClienteKey] = useState(null);
    const [pedidoKey, setPedidoKey] = useState(null);
    const [puntuacion, setPuntuacion] = useState(null);

    const handleClose = () => {
        props.onHide();
    };

    const addParameters = (context, data, nexus) => {

        data.parameters = [
            { id: "nuPedido", value: props.datosKey.find(a => a.id === "nuPedido").value },
            { id: "cdEmpresa", value: props.datosKey.find(a => a.id === "cdEmpresa").value },
            { id: "cdCliente", value: props.datosKey.find(a => a.id === "cdCliente").value },
            { id: "puntuacion", value: props.datosKey.find(a => a.id === "puntuacion").value }
        ];
        setEmpresaKey(props.datosKey.find(a => a.id === "cdEmpresa").value);
        setClienteKey(props.datosKey.find(a => a.id === "cdCliente").value);
        setPedidoKey(props.datosKey.find(a => a.id === "nuPedido").value);
        setPuntuacion(props.datosKey.find(a => a.id === "puntuacion").value);
    }

    const initialValues = {
        qtPonderacion: ""

    };

    const validationSchema = {
        qtPonderacion: Yup.string()
    };

    const onBeforeSubmit = (context, form, query, nexus) => {
        query.parameters = [
            { id: "nuPedido", value: pedidoKey },
            { id: "cdEmpresa", value: empresaKey },
            { id: "cdCliente", value: clienteKey },
        ];
    };

    const onAfterSubmit = (context, form, query, nexus) => {
        if (context.status === "ERROR")
            return false;
        nexus.getGrid("PRE812Ins_grid_1").reset();
    };

    const formOnBeforeInitialize = (context, form, query, nexus) => {

        query.parameters = [
            { id: "nuPedido", value: props.datosKey.find(a => a.id === "nuPedido").value },
            { id: "cdEmpresa", value: props.datosKey.find(a => a.id === "cdEmpresa").value },
            { id: "cdCliente", value: props.datosKey.find(a => a.id === "cdCliente").value },
        ];
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title>{t("PRE812Ins_Sec0_modalTitle_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Page
                    {...props}
                >
                    <div className="row">
                        <div className="col-12">
                            <Row>
                                <Col lg={3}>
                                    <span style={{ fontWeight: "bold" }}>{t("PRE812_frm1_lbl_cdEmpresa")}:</span>
                                    <span> {`${empresaKey}`}</span>
                                </Col>
                                <Col lg={3}>
                                    <span style={{ fontWeight: "bold" }}>{t("PRE812_frm1_lbl_cdCliente")}:</span>
                                    <span> {`${clienteKey}`}</span>
                                </Col>
                                <Col lg={3}>
                                    <span style={{ fontWeight: "bold" }}>{t("PRE812_frm1_lbl_nuPedido")}:</span>
                                    <span> {`${pedidoKey}`}</span>
                                </Col>
                                <Col lg={3}>
                                    <span style={{ fontWeight: "bold" }}>{t("PRE812_frm1_lbl_Puntuacion")}:</span>
                                    <span> {`${puntuacion}`}</span>
                                </Col>
                            </Row>
                            <hr />
                        </div>
                        <div className="col-12">
                            <Grid
                                id="PRE812Ins_grid_1"
                                application="PRE812InstanciaModal"
                                rowsToFetch={30}
                                rowsToDisplay={10}
                                enableExcelExport
                                onBeforeInitialize={addParameters}
                                onBeforeSelectSearch={addParameters}
                                onBeforeExportExcel={addParameters}
                                onBeforeFetch={addParameters}
                                onBeforeFetchStats={addParameters}
                                onBeforeApplySort={addParameters}
                                onBeforeApplyFilter={addParameters}
                            />
                        </div>
                        <div className="col-12">
                            <Form
                                id="PRE812Ins_form_1"
                                application="PRE812InstanciaModal"
                                initialValues={initialValues}
                                validationSchema={validationSchema}
                                onBeforeSubmit={onBeforeSubmit}
                                onAfterSubmit={onAfterSubmit}
                                onBeforeInitialize={formOnBeforeInitialize}
                            >
                                <Row>
                                    <Col lg={4}>
                                        <FormGroup>
                                            <label htmlFor="qtPonderacion">{t("PRE812_frm1_lbl_qtPonderacion")}</label>
                                            <Field name="qtPonderacion" />
                                            <StatusMessage for="qtPonderacion" />
                                        </FormGroup>
                                    </Col>
                                    <Col lg={8}>
                                        <FormGroup>
                                            <button className="btn btn-primary" id="btnGuardar" style={{ position: "absolute", bottom: "0em", marginBottom: "16px" }}>{t("PRE812_frm1_btn_Guardar")}</button>
                                        </FormGroup>
                                    </Col>
                                </Row>
                            </Form>
                        </div>
                    </div>
                </Page>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}>
                    {t("General_Sec0_btn_Cerrar")}
                </Button>
            </Modal.Footer>
        </Modal >
    );
}

export const PRE812InstanciaModal = withPageContext(InternalPRE812InstanciaModal);