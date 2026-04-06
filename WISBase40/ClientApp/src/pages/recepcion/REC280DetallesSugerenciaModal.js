import React, { useState } from 'react';
import { Modal, Button, Row, Col, Tab, Tabs } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Grid } from '../../components/GridComponents/Grid';
import { Form, Field, FieldSelect, FieldSelectAsync, StatusMessage } from '../../components/FormComponents/Form';
import * as Yup from 'yup';

function InternalREC280DetallesSugerenciaModal(props) {
    const { t } = useTranslation();

    const handleClose = () => {
        props.onHide();
    };

    const applyParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "estrategia", value: props.estrategia },
            { id: "predio", value: props.predio },
            { id: "tipoOperativa", value: props.tipoOperativa },
            { id: "codigoOperativa", value: props.codigoOperativa },
            { id: "codigoClase", value: props.codigoClase },
            { id: "codigoGrupo", value: props.codigoGrupo },
            { id: "empresa", value: props.empresa },
            { id: "producto", value: props.producto },
            { id: "codigoReferencia", value: props.codigoReferencia },
            { id: "codigoAgrupador", value: props.codigoAgrupador },
            { id: "enderecoSugerido", value: props.enderecoSugerido },
            { id: "nuAlmSugerencia", value: props.nuAlmSugerencia }
        ];
    };

    const applyParametersForm = (context, form, query, nexus) => {
        query.parameters = [
            { id: "estrategia", value: props.estrategia },
            { id: "descripcionEstrategia", value: props.descripcionEstrategia },
            { id: "codigoOperativa", value: props.codigoOperativa },
            { id: "tipoOperativa", value: props.tipoOperativa },
            { id: "predio", value: props.predio },
        ];
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w">
            <Modal.Header closeButton>
                <Modal.Title>{t("REC280_Sec0_modalTitle_DetallesSugerencia")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form
                    id="REC280_DetallesSugerencia_form"
                    application="REC280DetallesSugerencia"
                    onBeforeInitialize={applyParametersForm}
                >
                    <Row>
                        <Col md={3}>
                            <div className="form-group" >
                                <label htmlFor="estrategia">{t("REC280_frm1_lbl_Estrategia")}</label>
                                <Field name="estrategia" readOnly />
                                <StatusMessage for="estrategia" />
                            </div>
                        </Col>
                        <Col md={3}>
                            <div className="form-group" >
                                <label htmlFor="codigoOperativa">{t("REC280_frm1_lbl_CodigoOperativa")}</label>
                                <Field name="codigoOperativa" readOnly />
                                <StatusMessage for="codigoOperativa" />
                            </div>
                        </Col>
                        <Col md={3}>
                            <div className="form-group" >
                                <label htmlFor="tipoOperativa">{t("REC280_frm1_lbl_TipoOperativa")}</label>
                                <Field name="tipoOperativa" readOnly />
                                <StatusMessage for="tipoOperativa" />
                            </div>
                        </Col>
                        <Col md={3}>
                            <div className="form-group" >
                                <label htmlFor="predio">{t("REC280_frm1_lbl_Predio")}</label>
                                <Field name="predio" readOnly />
                                <StatusMessage for="predio" />
                            </div>
                        </Col>
                    </Row>
                </Form>

                <Row>
                    <Col>
                        <Grid
                            application="REC280DetallesSugerencia"
                            id="REC280Detalles_grid_1"
                            rowsToFetch={16}
                            rowsToDisplay={8}
                            enableExcelExport
                            onBeforeInitialize={applyParameters}
                            onBeforeFetch={applyParameters}
                            onBeforeExportExcel={applyParameters}
                            onBeforeFetchStats={applyParameters}
                            onBeforeApplySort={applyParameters}
                            onBeforeApplyFilter={applyParameters}
                        />
                    </Col>
                </Row>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("General_Sec0_btn_Cerrar")} </Button>
            </Modal.Footer>
        </Modal>
    );
}

export const REC280DetallesSugerenciaModal = withPageContext(InternalREC280DetallesSugerenciaModal);