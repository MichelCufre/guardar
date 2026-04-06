import React from 'react';
import { Button, Col, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { Field, Form, SubmitButton } from '../../components/FormComponents/Form';
import { FormWarningMessage } from '../../components/FormComponents/FormWarningMessage';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';

export const REC410DetalleProductoSinClasificar = (props) => {

    const { t } = useTranslation();

    const CloseModal = () => {
        props.onHide();
    };

    const applyParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "NumeroExterno", value: props.etiqueta },
            { id: "TipoEtiqueta", value: props.tipoEtiqueta },
            { id: "estacion", value: props.estacion }
        ];
    };

    const onBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = [
            { id: "NumeroExterno", value: props.etiqueta },
            { id: "TipoEtiqueta", value: props.tipoEtiqueta }
        ];
    };

    const onBeforeValidateField = (context, form, query, nexus) => {
        query.parameters = [
            { id: "estacion", value: props.estacion }
        ];
    };

    const onBeforeSubmit = (context, form, data, nexus) => {
        data.parameters = [
            { id: "NumeroExterno", value: props.etiqueta },
            { id: "TipoEtiqueta", value: props.tipoEtiqueta },
            { id: "estacion", value: props.estacion }
        ];
    };

    const onAfterSubmit = (context, form, data, nexus) => {
        if (context.responseStatus === "ERROR") return;
        props.onHide();
    };

    return (
        <Page
            application="REC410DetalleProductoSinClasificar"
            {...props}
        >
            <Form
                application="REC410DetProdSinClasificar"
                id="REC410DetProdSinClasi_form_1"
                onBeforeInitialize={onBeforeInitialize}
                onAfterSubmit={onAfterSubmit}
                onBeforeValidateField={onBeforeValidateField}
                onBeforeSubmit={onBeforeSubmit}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("REC410DetProdSinClasi_Sec0_mdl_SelecPosicion_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col md={6}>
                            <div className="form-group" >
                                <label htmlFor="etiqueta">{t("REC410DetProdSinClasificar_frm1_lbl_etiqueta")}</label>
                                <Field name="etiqueta" className="form-control-sm wis-field" readOnly />
                            </div>
                        </Col>
                        <Col md={6}>
                            <div className="form-group" >
                                <label htmlFor="tipoEtiqueta">{t("REC410DetProdSinClasificar_frm1_lbl_tipoEtiqueta")}</label>
                                <Field name="tipoEtiqueta" className="form-control-sm wis-field" readOnly />
                            </div>
                        </Col>
                    </Row>
                    <FormWarningMessage message={t("REC410DetProdSinClasi_Sec0_mdl_MensajeAdvertencia")} show={true} />
                    <Row>
                        <Col md={12}>
                            <fieldset className="form-group border p-2 grid" >
                                <legend align="center" className="w-auto"></legend>
                                <Grid
                                    id="REC410DetProdSinClasi_grid_1"
                                    rowsToFetch={10}
                                    rowsToDisplay={5}
                                    enableExcelExport
                                    application="REC410DetProdSinClasificar"
                                    onBeforeInitialize={applyParameters}
                                    onBeforeFetch={applyParameters}
                                    onBeforeFetchStats={applyParameters}
                                    onBeforeApplySort={applyParameters}
                                    onBeforeExportExcel={applyParameters}
                                />
                            </fieldset>
                        </Col>
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button id="btnCerrar" variant="outline-secondary" onClick={CloseModal}>{t("REC410SelecPos_frm1_btn_cerrar")} </Button>
                    <SubmitButton id="btnConfirmar" variant="primary" label="REC410SelecPos_frm1_btn_confirmar" />
                </Modal.Footer>
            </Form>
        </Page>
    );
};