import React from 'react';
import { useTranslation } from 'react-i18next';
import { Form, Field, FieldSelectAsync, StatusMessage, SubmitButton, FieldSelect } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Container } from 'react-bootstrap';
import * as Yup from 'yup';

export function REG310ReglasParametrosModal(props) {

    const { t } = useTranslation();
    const initialValues = {
        codigoParametro: "",
        descripcionParametro: "",
        valorParametro: "",
        tipoParametro: "",
    };

    const validationSchema = {
        codigoParametro: Yup.string(),
        tipoParametro: Yup.string(),
        descripcionParametro: Yup.string(),
        valorParametro: Yup.string().max(100),
    };

    const handleClose = () => {
        props.onHide(null, props.nexus);
    };

    const applyParameters = (context, form, query, nexus) => {
        if (query)
            query.parameters = [{ id: "codigoParametro", value: props.codigoParametro },
                { id: "descripcionParametro", value: props.descripcionParametro },
                { id: "valorParametro", value: props.valorParametro },
                { id: "tipoParametro", value: props.tipoParametro }];
    }

    const onBeforeSubmit = (context, form, query, nexus) => {
        context.abortServerCall = true;
        var valorParametro = form.fields.find(f => f.id == "valorParametro").value;
        props.onHide(valorParametro, nexus);
    }

    const renderFieldValorCampo = () => {
        switch (props.tipoParametro) {
            case "BOOL":
            case "SELECT":
                return (<FieldSelect name="valorParametro" isClearable={true} />);
            case "SEARCH":
                return (<FieldSelectAsync name="valorParametro" isClearable={true} />)
            default:
                return (<Field name="valorParametro" />);
        }
    }

    return (

        <Form
            application={props.application}
            id="REG310ReglasParametros_form_1"
            initialValues={initialValues}
            validationSchema={validationSchema}
            onBeforeInitialize={applyParameters}
            onBeforeSubmit={onBeforeSubmit}
        >
            <Modal.Header closeButton>
                <Modal.Title>{t("REG300_Sec0_Title_EditarParametro")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Container fluid>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="codigoParametro">{t("REG310ReglasParametros_frm_label_CodigoParametro")}</label>
                                <Field name="codigoParametro" readOnly />
                                <StatusMessage for="codigoParametro" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="tipoParametro">{t("REG310ReglasParametros_frm_label_TipoParametro")}</label>
                                <Field name="tipoParametro" readOnly />
                                <StatusMessage for="tipoParametro" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="descripcionParametro">{t("REG310ReglasParametros_frm_label_DescripcionParametro")}</label>
                                <Field name="descripcionParametro" readOnly />
                                <StatusMessage for="descripcionParametro" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="valorParametro">{t("REG310ReglasParametros_frm_label_ValorParametro")}</label>
                                {renderFieldValorCampo()}
                                <StatusMessage for="valorParametro" />
                            </div>
                        </Col>
                    </Row>
                </Container>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("REG300_frm_btn_Cancelar")} </Button>
                <SubmitButton id="btnSubmitConfirmarParametro" variant="primary" label="REG300_frm_btn_Crear" />
            </Modal.Footer>
        </Form>
    );
}