import React, { useState } from 'react';
import { Modal, Button, Row, Col } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { Form, Field, FieldToggle, StatusMessage, FieldSelect } from '../../components/FormComponents/Form';
import * as Yup from 'yup';
import { withPageContext } from '../../components/WithPageContext';

function InternalPRD113ConsumoParcialModal(props) {
    const { t } = useTranslation();

    const [hasPermission, setHasPermission] = useState(true);

    const validationSchema = {
        sobrante: Yup.string(),
        consumo: Yup.string(),
        consumido: Yup.string()
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleSubmit = () => {
        props.nexus.getForm("PRD113ConsumoParcial_form_1").submit();
    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            props.onHide();
        }
    }

    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        query.parameters = [
            { id: "idInsumoProduccion", value: props.idInsumoProduccion },
            { id: "idIngresoProduccion", value: props.idIngresoProduccion },
            { id: "esConsumible", value: props.esConsumible },
            { id: "cantidadReal", value: props.cantidadReal },
            { id: "codigoProducto", value: props.codigoProducto },
            { id: "numeroIdentificador", value: props.numeroIdentificador },
        ];
    }

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = [
            { id: "idInsumoProduccion", value: props.idInsumoProduccion },
            { id: "idIngresoProduccion", value: props.idIngresoProduccion },
            { id: "esConsumible", value: props.esConsumible },
            { id: "cantidadReal", value: props.cantidadReal },
            { id: "codigoProducto", value: props.codigoProducto },
            { id: "numeroIdentificador", value: props.numeroIdentificador },
        ];
    }

    const handleFormAfterInitialize = (context, form, query, nexus) => {

    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w">
            <Modal.Header closeButton>
                <Modal.Title>{t("PRD113_Sec0_modalTitle_TituloParcialProduccion")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Row>
                    <Col>
                        <Form
                            id="PRD113ConsumoParcial_form_1"
                            application="PRD113ConsumoParcial"
                            validationSchema={validationSchema}
                            onBeforeInitialize={handleFormBeforeInitialize}
                            onAfterInitialize={handleFormAfterInitialize}
                            onAfterSubmit={handleFormAfterSubmit}
                            onBeforeSubmit={handleFormBeforeSubmit}
                        >
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="producto">{t("PRD113_form1_label_Producto")}</label>
                                        <Field name="producto" disabled/>
                                        <StatusMessage for="producto" />
                                    </div>
                                </Col>
                                <Col >
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="descProducto">{t("PRD113_form1_label_DsProducto")}</label>
                                            <Field name="descProducto" disabled/>
                                            <StatusMessage for="descProducto" />
                                        </div>
                                    </Col>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="identificador">{t("PRD113_form1_label_Identificador")}</label>
                                        <Field name="identificador" disabled />
                                        <StatusMessage for="identificador" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="cantidadReal">{t("PRD113_form1_label_CantidadReal")}</label>
                                        <Field name="cantidadReal" disabled />
                                        <StatusMessage for="cantidadReal" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="consumido">{t("PRD113_form1_label_Consumido")}</label>
                                        <Field name="consumido" disabled />
                                        <StatusMessage for="consumido" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="consumo">{t("PRD113_form1_label_Consumo")}</label>
                                        <Field name="consumo" />
                                        <StatusMessage for="consumo" />
                                    </div>
                                </Col>
                                <Col >
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="motivo">{t("PRD113_form1_label_Motivo")}</label>
                                            <FieldSelect id="motivo" name="motivo" />
                                            <StatusMessage for="motivo" />
                                        </div>
                                    </Col>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="text-center form-group" style={{ display: 'flex', height: '100%' }}>
                                        <FieldToggle name="isSobrante" id="isSobrante" />
                                        <label style={{ marginLeft: '8px' }}>{t("PRD113_form1_label_Sobrante")}</label>
                                    </div>
                                </Col>
                            </Row>
                        </Form>
                    </Col>
                </Row>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="danger" onClick={handleClose}>
                    {t("PRD113_frm1_btn_Cancelar")}
                </Button>
                <Button variant="primary" onClick={handleSubmit} disabled={!hasPermission}>
                    {t("PRD113_frm1_btn_Confirmar")}
                </Button>
            </Modal.Footer>
        </Modal>
    );
}

export const PRD113ConsumoParcialModal = withPageContext(InternalPRD113ConsumoParcialModal);