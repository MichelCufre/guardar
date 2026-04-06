import React, { useState } from 'react';
import { Form, Field, FieldCheckbox, FieldTextArea, FieldSelect, SubmitButton, FormButton, StatusMessage } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';
import { Modal, Button, Row, Col, Container } from 'react-bootstrap';
import * as Yup from 'yup';


export function EXP110ImpresionBultosModal(props) {
    const { t } = useTranslation();
    const [contenedorBultoData, setContenedorBultoData] = useState(null);

    const validationSchema = {

        nuContenedorBulto: Yup.string().required(),
        cdEstiloBulto: Yup.string().required(),
        cantidadBulto: Yup.number().transform(value => (isNaN(value) ? undefined : value)).required(),
        comentarios: Yup.string().max(200),
        status: Yup.string(),
        ImpimirPrimerBulto: Yup.string()
    };

    const onAfterInitialize = (context, form, data, nexus) => {
        setFocusField("nuContenedorBulto");

    }

    const handleFormAfterSubmit = (context, form, data, nexus) => {
        if (context.responseStatus === "ERROR") return;

        if (data.buttonId === "nuContenedorBulto") {
            setFocusField("cantidadBulto");
            if (data.parameters.some(x => x.id == "AUX_DATOS_CONT_BULTO")) {
                var value = data.parameters.find(x => x.id == "AUX_DATOS_CONT_BULTO").value;
                setContenedorBultoData(value);
            }
        }
        else {

            var imprimirResumenPicking = "";
            var datosContBulto = "";
            if (data.parameters.some(x => x.id == "AUX_IMP_RESUMEN")) {
                imprimirResumenPicking = data.parameters.find(x => x.id == "AUX_IMP_RESUMEN").value;
            }
            if (data.parameters.some(x => x.id == "AUX_DATOS_CONT_BULTO")) {
                datosContBulto = data.parameters.find(x => x.id == "AUX_DATOS_CONT_BULTO").value;
            }

            props.onHide(nexus, imprimirResumenPicking, datosContBulto, data);
        }
    }

    const onBeforeSubmit = (context, form, data, nexus) => {
        var nuContenedorBultoValue = form.fields.find(x => x.id == "nuContenedorBulto").value;

        if (nuContenedorBultoValue === "") {
            context.abortServerCall = true;
        }
        else {
            data.parameters = [
                {
                    id: "AUX_DATOS_CONT_BULTO",
                    value: contenedorBultoData
                },
                {
                    id: "CONF_INICIAL",
                    value: props.conf_inicial
                },
                {
                    id: "isSubmit",
                    value: true
                }
            ];
        }
    }

    const handleClose = () => {
        props.onHide(props.nexus);
    };

    const addParameters = (context, form, data, nexus) => {

        data.parameters = [
            {
                id: "AUX_DATOS_CONT_BULTO",
                value: contenedorBultoData
            },
            {
                id: "CONF_INICIAL",
                value: props.conf_inicial
            }
        ];
    };

    const onAfterValidateField = (context, form, data, nexus) => {
        var isSubmiting = "";
        if (data.parameters.some(x => x.id == "isSubmit")) {
            isSubmiting = data.parameters.find(x => x.id == "isSubmit").value;
        }

        if (data.parameters.some(x => x.id == "AUX_DATOS_CONT_BULTO") && !isSubmiting) {
            var value = data.parameters.find(x => x.id == "AUX_DATOS_CONT_BULTO").value;
            setContenedorBultoData(value);
        }
    };

    const setFocusField = (fieldName) => {
        document.getElementsByName(fieldName)[0].focus();     
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Form
                application="EXP110ImpBultoModal"
                id="EXP110ImpBultoModal_form_1"
                validationSchema={validationSchema}
                onBeforeSubmit={onBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
                onBeforeInitialize={addParameters}
                onAfterValidateField={onAfterValidateField}
                onAfterInitialize={onAfterInitialize}
            >

                <Modal.Header closeButton>
                    <Modal.Title>{t("EXP110ImpBultoModal_Sec0_mdl_Configuracion_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Container>
                        <Row>
                            <Col md={6}>
                                <div className="form-group" >
                                    <label htmlFor="nuContenedorBulto">{t("EXP110ImpBultoModal_frm1_lbl_nuContenedorBulto")}</label>
                                    {/*<Field name="nuContenedorBulto" />*/}
                                    <Field className="form-control pepe" name="nuContenedorBulto" onKeyPress={(event) => {
                                        if (event.key === "Enter") {
                                            //setFocusField("nuContenedorBulto");
                                            props.nexus.getForm("EXP110ImpBultoModal_form_1").submit("nuContenedorBulto");
                                        }
                                    }} />
                                    <StatusMessage for="nuContenedorBulto" />
                                </div>
                                <div className="form-group" hidden>
                                    <label htmlFor="nuPreparacionBulto">{t("EXP110ImpBultoModal_frm1_lbl_nuPreparacionBulto")}</label>
                                    <Field className="form-control-sm pepe" name="nuPreparacionBulto" />
                                    <StatusMessage for="nuPreparacionBulto" />
                                </div>
                            </Col>
                            <Col md={6}>
                                <div className="form-group" >
                                    <label htmlFor="cdEstiloBulto">{t("EXP110ImpBultoModal_frm1_lbl_cdEstiloBulto")}</label>
                                    <Field className="form-control pepe" name="cdEstiloBulto" />
                                    <StatusMessage for="cdEstiloBulto" />
                                </div>
                            </Col>
                        </Row>
                        <Row>
                            <Col md={6}>
                                <div className="form-group" >
                                    <label htmlFor="cantidadBulto">{t("EXP110ImpBultoModal_frm1_lbl_cantidadBulto")}</label>
                                    <Field className="form-control pepe" name="cantidadBulto" />
                                    <StatusMessage for="cantidadBulto" />
                                </div>
                            </Col>
                            <Col md={6}>
                                <div className="form-group" >
                                    <FieldCheckbox
                                        name="ImpimirPrimerBulto"
                                        label={t("EXP110ImpBultoModal_frm1_lbl_ImpimirPrimerBulto")}
                                        className="mb-2"
                                    />
                                    <StatusMessage for="ImpimirPrimerBulto" />
                                </div>
                            </Col>
                        </Row>
                        <Row>
                            <Col md={12}>
                                <div className="form-group" >
                                    <label htmlFor="comentarios">{t("EXP110ImpBultoModal_frm1_lbl_comentarios")}</label>
                                    <FieldTextArea name="comentarios" />
                                    <StatusMessage for="comentarios" />
                                </div>
                            </Col>
                        </Row>
                        <Row>
                            <Col md={12}>
                                <div className="form-group" >
                                    <label htmlFor="status">{t("EXP110ImpBultoModal_frm1_lbl_status")}</label>
                                    <Field name="status" readOnly />
                                    <StatusMessage for="status" />
                                </div>
                            </Col>
                        </Row>

                    </Container>
                </Modal.Body>
                <Modal.Footer>
                    <Container>
                        <Row>
                            <Col md={4}>
                                <div align="center">
                                    <SubmitButton id="btnSubmitGuardar" variant="primary" label="EXP110ImpBultoModal_frm1_btn_guardar" />
                                </div>
                            </Col>
                            <Col md={4}>
                                <div align="center">
                                    <SubmitButton id="btnSubmitImprimir" variant="primary" label="EXP110ImpBultoModal_frm1_btn_imprimir" />
                                </div>
                            </Col>
                            <Col md={4}>
                                <div align="center">
                                    <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("EXP110ImpBultoModal_frm1_btn_cerrar")} </Button>
                                </div>
                            </Col>
                        </Row>
                    </Container>
                </Modal.Footer>
            </Form>

        </Modal>
    );
}

