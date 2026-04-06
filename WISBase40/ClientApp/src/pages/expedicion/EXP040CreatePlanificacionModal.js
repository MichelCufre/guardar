import React, { useState } from 'react';
import { Button, Col, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, FieldSelect, FieldSelectAsync, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';

export function EXP040CreatePlanificacionModal(props) {
    const { t } = useTranslation("translation", { useSuspense: false });
    const [warnRutaAsociada, setWarningRutaAsociada] = useState(false);

    const initialValues = {
        descripcion: "",
        predio: "",
        codigoEmpresa: "",
    };

    const validationSchema = {
        descripcion: Yup.string().required().max(50),
        codigoEmpresa: Yup.string().max(10),
        predio: Yup.string().required().max(15)
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleFormAfterValidateField = (context, form, query, nexus) => {
        if (query.fieldId === "codigoRuta" || query.fieldId === "predio") {
            const parameter = query.parameters.find(d => d.id === "rutaYaAsociada" && d.value === "S");

            if (parameter) {
                setWarningRutaAsociada(true);
            } else {
                setWarningRutaAsociada(false);
            }
        }
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            nexus.getGrid("EXP040_grid_1").refresh();
            props.onHide();
        }
    };

    const onBeforeSubmit = (context, form, query, nexus) => {
        query.parameters.push({ id: "isSubmit", value: true });
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Form
                id="EXP040_form_CreatePlanificacion"
                application="EXP040CreatePlanificacion"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeSubmit={onBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
                onAfterValidateField={handleFormAfterValidateField}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("EXP040_Sec0_Title_PlanificacionCreate")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="descripcion">{t("EXP040_frm1_lbl_descripcion")}</label>
                                <Field name="descripcion" />
                                <StatusMessage for="descripcion" />
                            </div>
                            <div className="form-group">
                                <label htmlFor="predio">{t("EXP040_frm1_lbl_predio")}</label>
                                <FieldSelect name="predio" />
                                <StatusMessage for="predio" />
                            </div>
                            <div className="form-group">
                                <label htmlFor="codigoEmpresa">{t("EXP040_frm1_lbl_empresa")}</label>
                                <FieldSelectAsync name="codigoEmpresa" isClearable={true} />
                                <StatusMessage for="codigoEmpresa" />
                            </div>
                        </Col>
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="outline-secondary" onClick={handleClose}>
                        {t("EXP040_frm1_btn_cancelar")}
                    </Button>
                    <SubmitButton id="btnCreatePlanificacion" variant="primary" label="EXP040_frm1_btn_crear" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}