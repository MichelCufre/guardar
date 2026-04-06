import React from 'react';
import { Button, Col, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { FieldSelect, FieldSelectAsync, FieldToggle, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';

export function STO800CreateConfiguracionModal(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const initialValues = {
        cdEmpresaOrigen: "",
        flTodaEmpresa: "",
        flTodoTipoTraspaso: "",
        flCabezalAuto: "",
        flReplicaProductos: "",
        flReplicaCB: "",
        flCtrlCaractIguales: "",
        flReplicaAgentes: "",
        cdTipoDocuIngreso: "",
        cdTipoDocuEgreso: "",
    };

    const validationSchema = {
        cdEmpresaOrigen: Yup.string().required(),
        flTodaEmpresa: Yup.boolean(),
        flTodoTipoTraspaso: Yup.boolean(),
        flCabezalAuto: Yup.boolean(),
        flReplicaProductos: Yup.boolean(),
        flReplicaCB: Yup.boolean(),
        flCtrlCaractIguales: Yup.boolean(),
        flReplicaAgentes: Yup.boolean(),
        cdTipoDocuIngreso: Yup.string(),
        cdTipoDocuEgreso: Yup.string(),
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            nexus.getGrid("STO800_grid_1").refresh();
            props.onHide();
        }
    };

    const onBeforeSubmit = (context, form, query, nexus) => {
        query.parameters.push({ id: "isSubmit", value: true });
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Form
                id="STO800_form_CreateConfig"
                application="STO800CrearConfiguracion"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeSubmit={onBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("STO800_Sec0_modalTitle_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col md={4}>
                            <div className="form-group">
                                <label htmlFor="cdEmpresaOrigen">{t("STO800_frm1_lbl_cdEmpresaOrigen")}</label>
                                <FieldSelectAsync name="cdEmpresaOrigen" />
                                <StatusMessage for="cdEmpresaOrigen" />
                            </div>
                            <div className="form-group">
                                <FieldToggle name="flTodaEmpresa" label={t("STO800_frm1_lbl_flTodaEmpresa")} />
                                <StatusMessage for="flTodaEmpresa" />
                            </div>
                            <div className="form-group">
                                <FieldToggle name="flReplicaProductos" label={t("STO800_frm1_lbl_flReplicaProductos")} />
                                <StatusMessage for="flReplicaProductos" />
                            </div>
                            <div className="form-group">
                                <FieldToggle name="flCtrlCaractIguales" label={t("STO800_frm1_lbl_flCtrlCaractIguales")} />
                                <StatusMessage for="flCtrlCaractIguales" />
                            </div>
                        </Col>
                        <Col md={4}>
                            <div className="form-group">
                                <label htmlFor="cdTipoDocuIngreso">{t("STO800_frm1_lbl_cdTipoDocuIngreso")}</label>
                                <FieldSelect name="cdTipoDocuIngreso" />
                                <StatusMessage for="cdTipoDocuIngreso" />
                            </div>

                            <div className="form-group">
                                <FieldToggle name="flTodoTipoTraspaso" label={t("STO800_frm1_lbl_flTodoTipoTraspaso")} />
                                <StatusMessage for="flTodoTipoTraspaso" />
                            </div>

                            <div className="form-group">
                                <FieldToggle name="flReplicaCB" label={t("STO800_frm1_lbl_flReplicaCB")} />
                                <StatusMessage for="flReplicaCB" />
                            </div>

                        </Col>
                        <Col md={4}>
                            <div className="form-group">
                                <label htmlFor="cdTipoDocuEgreso">{t("STO800_frm1_lbl_cdTipoDocuEgreso")}</label>
                                <FieldSelect name="cdTipoDocuEgreso" />
                                <StatusMessage for="cdTipoDocuEgreso" />
                            </div>
                            <div className="form-group">
                                <FieldToggle name="flCabezalAuto" label={t("STO800_frm1_lbl_flCabezalAuto")} />
                                <StatusMessage for="flCabezalAuto" />
                            </div>
                            <div className="form-group">
                                <FieldToggle name="flReplicaAgentes" label={t("STO800_frm1_lbl_flReplicaAgentes")} />
                                <StatusMessage for="flReplicaAgentes" />
                            </div>

                        </Col>
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="outline-secondary" onClick={handleClose}>
                        {t("STO800_frm1_btn_Cancelar")}
                    </Button>
                    <SubmitButton id="btnSubmitCreateConfig" variant="primary" label="STO800_frm1_btn_Crear" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}