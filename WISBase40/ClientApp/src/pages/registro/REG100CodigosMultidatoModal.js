import React from 'react';
import { useTranslation } from 'react-i18next';
import { Modal, Button, Row, Col, Tab, Tabs } from 'react-bootstrap';
import * as Yup from 'yup';
import { Grid } from '../../components/GridComponents/Grid';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, FieldNumber, StatusMessage, SubmitButton, FormButton } from '../../components/FormComponents/Form';

function InternalREG100CodigosMultidatoModal(props) {
    const { t } = useTranslation();

    const handleClose = () => {
        props.onHide();
    }

    const onBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = [
            { id: "empresa", value: props.empresa },
            { id: "codigoMultidato", value: props.codigoMultidato }
        ];
    }

    const addParams = (context, data, nexus) => {
        data.parameters = [
            { id: "empresa", value: props.empresa },
            { id: "codigoMultidato", value: props.codigoMultidato }
        ];
    }

    const onBeforeCommit = (context, data, nexus) => {
        data.parameters = [
            { id: "empresa", value: props.empresa },
            { id: "codigoMultidato", value: props.codigoMultidato }
        ];
    }

    const onBeforeSelectSearch = (context, row, query, nexus) => {
        query.parameters = [
            { id: "empresa", value: props.empresa },
            { id: "codigoMultidato", value: props.codigoMultidato }
        ];
    }

    const onAfterButtonAction = (data, nexus) => {
        nexus.getGrid("REG100CodigoMultidato_grid_1").refresh();
    }

    return (
        <Modal dialogClassName="modal-70w" show={props.show} onHide={props.onHide} >
            <Form
                application="REG100CodigoMultidato"
                id="REG100CodigoMultidato_form_1"
                onBeforeInitialize={onBeforeInitialize}

            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("REG100_Sec0_Codigos_Multidato_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col lg={6}>
                            <label htmlFor="codigoEmpresa">{t("REG100_frm1_colname_CD_EMPRESA")}</label>
                            <Field name="codigoEmpresa" readOnly />
                            <StatusMessage for="codigoEmpresa" readOnly />
                        </Col >
                        <Col lg={6}>
                            <label htmlFor="nombreEmpresa">{t("REG100_frm1_colname_NM_EMPRESA")}</label>
                            <Field name="nombreEmpresa" readOnly />
                            <StatusMessage for="nombreEmpresa" readOnly />
                        </Col >
                    </Row>
                    <Row>
                        <Col lg={6}>
                            <label htmlFor="codigoMultidato">{t("REG100_frm1_colname_CD_CODIGO_MULTIDATO")}</label>
                            <Field name="codigoMultidato" readOnly />
                            <StatusMessage for="codigoMultidato" readOnly />
                        </Col >
                        <Col lg={6}>
                            <label htmlFor="descripcionCodigo">{t("REG100_frm1_colname_DS_CODIGO_MULTIDATO")}</label>
                            <Field name="descripcionCodigo" readOnly />
                            <StatusMessage for="descripcionCodigo" readOnly />
                        </Col >
                    </Row>
                    <br />
                    <Grid
                        application="REG100CodigoMultidato"
                        id="REG100CodigoMultidato_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onBeforeFetch={addParams}
                        onBeforeFetchStats={addParams}
                        onBeforeApplyFilter={addParams}
                        onBeforeApplySort={addParams}
                        onBeforeExportExcel={addParams}
                        onBeforeInitialize={addParams}
                        onBeforeValidateRow={addParams}
                        onBeforeButtonAction={addParams}
                        onBeforeCommit={onBeforeCommit}
                        onBeforeSelectSearch={onBeforeSelectSearch}
                        onAfterButtonAction={onAfterButtonAction}
                    />

                </Modal.Body>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("General_Sec0_btn_Cerrar")} </Button>
                </Modal.Footer>

            </Form>
        </Modal>
    );
}

export const REG100CodigosMultidatoModal = withPageContext(InternalREG100CodigosMultidatoModal);