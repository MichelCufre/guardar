import React, { useState } from 'react';
import { Modal, Button, Row, Col } from 'react-bootstrap';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldToggle, SubmitButton, StatusMessage } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { FormWarningMessage } from '../../components/FormComponents/FormWarningMessage';

export function EXP040UpdatePlanificacionModal(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const initialValues = {
        descripcion: "",
        predio: "",
        codigoEmpresa: "",
    };

    const validationSchema = {
        descripcion: Yup.string().required().max(50),        
        predio: Yup.string().required().max(15),
        codigoEmpresa: Yup.string().max(10),
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = [{ id: "camion", value: props.camion }];
    }

    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        query.parameters = [
            { id: "camion", value: props.camion },
            { id: "isSubmit", value: true }
        ];
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            nexus.getGrid("EXP040_grid_1").refresh();
            props.onHide();
        }
    };

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Form
                id="EXP040_form_UpdatePlanificacion"
                application="EXP040UpdatePlanificacion"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeInitialize={handleFormBeforeInitialize}
                onBeforeSubmit={handleFormBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("EXP040_Sec0_Title_PlanificacionUpdate")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col>
                            <h3>Camion: {props.camion}</h3>
                        </Col>
                    </Row>
                    <hr />
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
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}>
                        {t("EXP040_frm1_btn_cancelar")}
                    </Button>
                    <SubmitButton id="btnUpdatePlanificacion" variant="primary" label="EXP040_frm1_btn_editar" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}