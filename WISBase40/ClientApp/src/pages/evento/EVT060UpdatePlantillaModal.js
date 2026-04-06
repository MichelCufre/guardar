import React, { useEffect, useState } from 'react';
import { Button, Col, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, FieldSelect, FieldSelectAsync, FieldText, FieldToggle, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';

export function EVT060UpdatePlantillaModal(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const [habilitarHtml, setHabilitarHtml] = useState(false);
    const [cuerpoNotificacion, setCuerpoNotificacion] = useState("");
    const [nexus, setNexus] = useState(null);

    const initialValues = {
        codigoPlantilla: "",
        descripcionPlantilla: "",
        codigoEvento: "",
        tipoNotificacion: "",
        asuntoNotificacion: "",
        habilitarHtml: "",
        cuerpoNotificacion: "",
        previewCuerpoNotificacion: ""
    };

    const validationSchema = {
        codigoPlantilla: Yup.string().required().max(15),
        descripcionPlantilla: Yup.string().max(200),
        codigoEvento: Yup.string().required().max(10),
        tipoNotificacion: Yup.string().required().max(60),
        asuntoNotificacion: Yup.string().max(70),
        habilitarHtml: Yup.boolean(),
        cuerpoNotificacion: Yup.string()
    };

    useEffect(() => {
        if (nexus) {
            var form = nexus.getForm("EVT060UpdatePlantilla_form1");
            form.setFieldValue("previewCuerpoNotificacion", cuerpoNotificacion);
        }
    }, [cuerpoNotificacion]);

    const handleClose = () => {
        props.onHide();
    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        setHabilitarHtml(props.isHtml);

        query.parameters = [
            { id: "nuEvento", value: props.plantilla.nuEvento },
            { id: "tpNotificacion", value: props.plantilla.tpNotificacion },
            { id: "cdPlantilla", value: props.plantilla.cdPlantilla }
        ];
    }

    const handleFormAfterInitialize = (context, form, parameters, nexus) => {
        setNexus(nexus);
    }

    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        query.parameters = [
            { id: "nuEvento", value: props.plantilla.nuEvento },
            { id: "tpNotificacion", value: props.plantilla.tpNotificacion },
            { id: "cdPlantilla", value: props.plantilla.cdPlantilla },
            { id: "isSubmit", value: true }
        ];
    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            nexus.getGrid("EVT060_grid_1").refresh();
            props.onHide();
        }
    };

    const handleFormBeforeValidateField = (context, form, query, nexus) => {
        setHabilitarHtml(form.fields.find(d => d.id === "habilitarHtml").value);
    }

    const handleCuerpoNotificacionChange = (value) => {
        setCuerpoNotificacion(value);
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Form
                id="EVT060UpdatePlantilla_form1"
                application="EVT060UpdatePlantilla"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onAfterInitialize={handleFormAfterInitialize}
                onBeforeInitialize={handleFormBeforeInitialize}
                onBeforeValidateField={handleFormBeforeValidateField}
                onBeforeSubmit={handleFormBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("EVT060UpdatePlantilla_Sec0_modalTitle_Update")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col md={6}>
                            <div className="form-group">
                                <label htmlFor="codigoPlantilla">{t("EVT060UpdatePlantilla_frm1_lbl_codigoPlantilla")}</label>
                                <Field name="codigoPlantilla" />
                                <StatusMessage for="codigoPlantilla" />
                            </div>
                        </Col>
                        <Col md={6}>
                            <div className="form-group">
                                <label htmlFor="descripcionPlantilla">{t("EVT060UpdatePlantilla_frm1_lbl_descripcionPlantilla")}</label>
                                <Field name="descripcionPlantilla" />
                                <StatusMessage for="descripcionPlantilla" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col md={6}>
                            <div className="form-group">
                                <label htmlFor="codigoEvento">{t("EVT060UpdatePlantilla_frm1_lbl_codigoEvento")}</label>
                                <FieldSelect name="codigoEvento" />
                                <StatusMessage for="codigoEvento" />
                            </div>
                        </Col>
                        <Col md={6}>
                            <div className="form-group">
                                <label htmlFor="tipoNotificacion">{t("EVT060UpdatePlantilla_frm1_lbl_tipoNotificacion")}</label>
                                <FieldSelect name="tipoNotificacion" />
                                <StatusMessage for="tipoNotificacion" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col md={9}>
                            <div className="form-group">
                                <label htmlFor="asuntoNotificacion">{t("EVT060UpdatePlantilla_frm1_lbl_asuntoNotificacion")}</label>
                                <Field name="asuntoNotificacion" />
                                <StatusMessage for="asuntoNotificacion" />
                            </div>
                        </Col>
                        <Col md={3} className="align-self-center">
                            <div className="form-group">
                                <FieldToggle name="habilitarHtml" label={t("EVT060UpdatePlantilla_frm1_lbl_habilitarHtml")} />
                                <StatusMessage for="habilitarHtml" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col lg={habilitarHtml ? 6 : 12}>
                            <label htmlFor="cuerpoNotificacion">{t("EVT060UpdatePlantilla_frm1_lbl_cuerpoNotificacion")}</label>
                            <FieldText name="cuerpoNotificacion" height={250} isHtml={false} value={cuerpoNotificacion} onChange={handleCuerpoNotificacionChange} />
                        </Col>
                        <Col lg={6} className={habilitarHtml ? '' : 'hidden'}>
                            <label htmlFor="previewCuerpoNotificacion">{t("EVT060UpdatePlantilla_frm1_lbl_previewCuerpoNotificacion")}</label>
                            <FieldText name="previewCuerpoNotificacion" height={250} isHtml value={cuerpoNotificacion} />
                        </Col>
                    </Row>

                </Modal.Body>
                <Modal.Footer>
                    <Button variant="outline-secondary" onClick={handleClose}>
                        {t("EVT060UpdatePlantilla_frm1_btn_cancelar")}
                    </Button>
                    <SubmitButton id="btnSubmitUpdatePlantilla" variant="primary" label="EVT060UpdatePlantilla_frm1_btn_modificar" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}