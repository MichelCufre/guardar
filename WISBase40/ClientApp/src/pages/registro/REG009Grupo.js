import React from 'react';
import { Page } from '../../components/Page';
import { Form, Field } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';
import { Container, Row, Col, Modal, Button } from 'react-bootstrap';
import * as Yup from 'yup';

export default function REG009Grupo(props) {

    const { t } = useTranslation();

    const validationSchema = {
        empresa: Yup.string().required(),
        producto: Yup.string().required(),
        grupo: Yup.string()
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {

        if (props.producto && props.producto.Empresa != null) {

            query.parameters = [
                { id: "idProducto", value: props.producto.Producto },
                { id: "idEmpresa", value: props.producto.Empresa }
            ];
        }
    }

    return (
        <Page
            {...props}
            application="REG009Grupo"
        >
            <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
                <Form
                    application="REG009Grupo"
                    id="REG009Grupo_form_1"
                    validationSchema={validationSchema}
                    onBeforeInitialize={handleFormBeforeInitialize}
                >
                    <Modal.Header closeButton>
                        <Modal.Title>{t("REG009Grupo_frm_lbl_Title")}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <Container fluid>
                            <Row>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="empresa">{t("REG009Grupo_frm_lbl_empresa")}</label>
                                        <Field name="empresa" readOnly />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="producto">{t("REG009Grupo_frm_lbl_producto")}</label>
                                        <Field name="producto" readOnly />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="grupo">{t("REG009Grupo_frm_lbl_grupo")}</label>
                                        <Field name="grupo" readOnly />
                                    </div>
                                </Col>
                            </Row>
                        </Container>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant="btn btn-outline-secondary" onClick={handleClose}>{t("REG009Grupo_frm_btn_Cerrar")}</Button>
                    </Modal.Footer>
                </Form >
            </Modal >
        </Page>
    );
}