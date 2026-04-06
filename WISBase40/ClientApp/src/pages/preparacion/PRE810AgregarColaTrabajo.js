import React, { useState } from 'react';
import { Button, Col, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, FieldNumber, FieldSelect, FieldToggle, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { withPageContext } from '../../components/WithPageContext';

export function InternalPRE810AgregarColaTrabajo(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const [nuEntrega, setNuEntrega] = useState("");
    const [tipoEntrega, setTipoEntrega] = useState("");

    const initialValues = {
        nuColaTrabajo: "",
        nuPredio: "",
        descripcion: "",
    };

    const validationSchema = {
        nuColaTrabajo: Yup.string(),
        nuPredio: Yup.string(),
        descripcion: Yup.string().max(200)
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            nexus.getForm("PRE810_form_1").reset();
            props.onHide();
        }
    };


    return (
        <Modal dialogClassName="modal-50w" show={props.show} onHide={props.onHide} >
            <Form
                application="PRE810CreateCola"
                id="PRE810_form_Add"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onAfterSubmit={handleFormAfterSubmit}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("PRE810_Sec0_modalTitle_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row >
                        <Col lg={4}>
                            <div className="form-group">
                                <label htmlFor="nuColaTrabajo">{t("PRE810_frm1_lbl_NU_COLATRABAJO")}</label>
                                <FieldNumber name="nuColaTrabajo" />
                                <StatusMessage for="nuColaTrabajo" />
                            </div>
                        </Col>
                        <Col lg={8}>
                            <label htmlFor="nuPredio">{t("PRE810_frm1_lbl_NU_PREDIO")}</label>
                            <FieldSelect name="nuPredio" />
                            <StatusMessage for="nuPredio" />

                        </Col>
                    </Row>
                    <Row >
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="descripcion">{t("PRE810_frm1_lbl_DESCRIPCION")}</label>
                                <Field name="descripcion" />
                                <StatusMessage for="descripcion" />
                            </div>
                        </Col>
                    </Row>
                    <Row >
                        <Col>
                            <div className="form-check form-check-inline" style={{ marginRight: "-1%" }}>
                                <label htmlFor="flOrdenCalendario">{t("PRE810_frm1_lbl_flOrdenCalendario")}</label>
                                <FieldToggle className="form-check-inline" name="flOrdenCalendario" />
                            </div>
                        </Col>
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="outline-secondary" onClick={handleClose}>
                        {t("General_Sec0_btn_Cerrar")}
                    </Button>
                    <SubmitButton id="btnAgregar" variant="primary" label="PRE810_frm1_btn_Agregar" className="float-right" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}

export const PRE810AgregarColaTrabajo = withPageContext(InternalPRE810AgregarColaTrabajo);