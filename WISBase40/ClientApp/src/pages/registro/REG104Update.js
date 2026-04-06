import React from 'react';
import { Button, Col, Container, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, FieldSelect, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { Page } from '../../components/Page';

export default function REG104Update(props) {
    const { t } = useTranslation();

    const validationSchema = {
        CD_ZONA: Yup.string().max(20).required(),
        NM_ZONA: Yup.string().max(100).required(),
        DS_ZONA: Yup.string().max(200),
        CD_LOCALIDAD: Yup.string().required(),
    };

    const handleClose = () => {
        props.onHide(props.nexxus);
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            props.onHide(props.nexxus);
        }
    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {

        if (props.zona != null) {

            query.parameters = [
                { id: "cdZona", value: props.zona }
            ];
        }
    }

    return (
        <Page
            {...props}
            application="REG104Update"
        >
            <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
                <Form
                    application="REG104Update"
                    id="REG104Update_form_1"
                    validationSchema={validationSchema}
                    onAfterSubmit={handleFormAfterSubmit}
                    onBeforeInitialize={handleFormBeforeInitialize}
                >
                    <Modal.Header closeButton>
                        <Modal.Title>{t("REG104_Sec0_modalTitle_EditarZona")}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <Container fluid>
                            <Row>
                                <Col lg={4}>
                                    <div className="form-group">
                                        <label htmlFor="CD_ZONA">{t("REG104_frm1_colname_CD_ZONA")}</label>
                                        <Field id="CD_ZONA" name="CD_ZONA" />
                                        <StatusMessage for="CD_ZONA" />
                                    </div>
                                </Col>
                                <Col lg={8}>
                                    <div className="form-group">
                                        <label htmlFor="NM_ZONA">{t("REG104_frm1_colname_NM_ZONA")}</label>
                                        <Field name="NM_ZONA" />
                                        <StatusMessage for="NM_ZONA" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col lg={8}>
                                    <div className="form-group">
                                        <label htmlFor="DS_ZONA">{t("REG104_frm1_colname_DS_ZONA")}</label>
                                        <Field name="DS_ZONA" />
                                        <StatusMessage for="DS_ZONA" />
                                    </div>
                                </Col >
                                <Col lg={4}>
                                    <div className="form-group">
                                        <label htmlFor="CD_LOCALIDAD">{t("REG104_frm1_colname_CD_LOCALIDAD")}</label>
                                        <FieldSelect name="CD_LOCALIDAD" />
                                        <StatusMessage for="CD_LOCALIDAD" />
                                    </div>
                                </Col >
                            </Row>
                        </Container>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant="btn btn-outline-secondary" onClick={handleClose}>{t("REG104_frm_btn_Cancelar")}</Button>
                        <SubmitButton id="btnUpdateZona" label="REG104_frm1_btn_Guardar" variant="primary" className="mt-2" />
                    </Modal.Footer>
                </Form>
            </Modal >
        </Page >
    )
}