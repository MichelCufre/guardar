import React, { useState } from 'react';
import { Modal, Button, Row, Col, Tab, Tabs, Spinner } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { Form, Field, FieldSelect, FieldSelectAsync, StatusMessage } from '../../components/FormComponents/Form';
import * as Yup from 'yup';
import { withPageContext } from '../../components/WithPageContext';

function InternalPRD110AsociarEspacioProduccionModal(props) {
    const { t } = useTranslation();

    const [hasPermission, setHasPermission] = useState(true);
    const [isLoading, setIsLoading] = useState(false);

    const validationSchema = {
        codigo: Yup.string(),
        espacio: Yup.string()
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleSubmit = () => {
        props.nexus.getForm("PRD110_form_AsociarEspacioProduccion").submit();
    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        setIsLoading(false);
        if (context.responseStatus === "OK") {
            props.onHide();
        }
    }

    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        setIsLoading(true);
        form.fields.find(f => f.id === "codigo").value = props.ingresoEditar;
    }

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        form.fields.find(f => f.id === "codigo").value = props.ingresoEditar;
    }

    const handleFormAfterInitialize = (context, form, query, nexus) => {
        setHasPermission(query.parameters.find(f => f.id === "hasPermission").value === "S" ? true : false);
    }

    const onBeforeSelectSearch = (context, form, data, nexus) => {
        data.parameters = [
            { id: "idIngreso", value: props.ingresoEditar },
        ];
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w">
            <Modal.Header closeButton>
                <Modal.Title>{t("PRD110_Sec0_modalTitle_TituloAsociarEspacioProduccion")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Row>
                    <Col>
                        <Form
                            id="PRD110_form_AsociarEspacioProduccion"
                            application="PRD110AsociarEspacioProduccion"
                            validationSchema={validationSchema}
                            onBeforeInitialize={handleFormBeforeInitialize}
                            onAfterInitialize={handleFormAfterInitialize}
                            onAfterSubmit={handleFormAfterSubmit}
                            onBeforeSubmit={handleFormBeforeSubmit}
                            onBeforeSelectSearch={onBeforeSelectSearch}
                        >
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="codigo">{t("PRD110_form1_label_Codigo")}</label>
                                        <Field name="codigo" value={props.ingresoEditar} disabled />
                                        <StatusMessage for="codigo" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="idExterno">{t("PRD110_form1_lbl_idExternoProduccion")}</label>
                                        <Field name="idExterno" disabled />
                                        <StatusMessage for="idExterno" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col md={6}>
                                    <div className="form-group" >
                                        <label htmlFor="espacio">{t("PRD110_form1_label_Espacio")}</label>
                                        <FieldSelect name="espacio" />
                                        <StatusMessage for="espacio" />
                                    </div>
                                </Col>
                            </Row>
                        </Form>
                    </Col>
                </Row>

            </Modal.Body>
            <Modal.Footer>
                <Button variant="danger" onClick={handleClose} disabled={isLoading}>
                    {t("PRD110_frm1_btn_Cancelar")}
                </Button>
                <Button variant="primary" onClick={handleSubmit} disabled={!hasPermission || isLoading}>
                    {t("PRD110_frm1_btn_Asociar")}
                    {
                        isLoading ? (
                            <Spinner animation="border" role="status" size="sm" className={"ml-2"}>
                                <span className="sr-only">Loading...</span>
                            </Spinner>
                        ) : null
                    }
                </Button>
            </Modal.Footer>
        </Modal>
    );
}

export const PRD110AsociarEspacioProduccionModal = withPageContext(InternalPRD110AsociarEspacioProduccionModal);