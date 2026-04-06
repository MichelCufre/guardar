import React, { useState } from 'react';
import { Modal, Col, Row, Button, FormGroup } from 'react-bootstrap';
import { StatusMessage, SubmitButton, Form, FieldSelect} from '../../components/FormComponents/Form';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';
import { Page } from '../../components/Page';

export function REC170SincronizarTrackingModal(props) {
    const { t } = useTranslation("translation", { useSuspense: false });
    const [codigoCliente, setCodigoCliente] = useState("");
    const [codigoAgente, setCodigoAgente] = useState("");
    const [tipoAgente, setTipoAgente] = useState("");

    const initialValues = {

        puntosDeEntrega: ""
    };

    const validationSchema = {
        puntosDeEntrega: Yup.string().required()
    };

    const handleClose = () => {
        props.onHide();
    };

    const addParameters = (context, form, query, nexus) => {

        query.parameters = [

            { id: "nuAgenda", value: props.agenda.find(a => a.id === "idAgenda").value }
        ];

    }
    const handleFormAfterInitialize = (context, form, query, nexus) => {
        setCodigoCliente(query.parameters.find(d => d.id === "codigoCliente").value);
        setCodigoAgente(query.parameters.find(d => d.id === "codigoAgente").value);
        setTipoAgente(query.parameters.find(d => d.id === "tipoAgente").value);
    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {

        if (context.responseStatus === "OK") {
            nexus.getGrid("REC170_grid_1").refresh();
            props.onHide();
        }
    };

    return (
        <Modal show={props.show} onHide={handleClose} backdrop="static">

            <Form
                id="REC170Tracking_form_1"
                application="REC170Tracking"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onAfterSubmit={handleFormAfterSubmit}
                onBeforeSubmit={addParameters}
                onBeforeInitialize={addParameters}
                onAfterInitialize={handleFormAfterInitialize}
            >

                <Modal.Header closeButton>
                    <Modal.Title>{t("REC170Tracking_Sec0_title_SincronizarTracking")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col>
                            <strong>{t("REC170_frm1_lbl_cliente")}: {codigoCliente}</strong>
                        </Col>
                        <Col>
                            <strong>{t("REC170_frm1_lbl_Agente")}: {codigoAgente}</strong>
                        </Col>
                        <Col>
                            <strong>{t("REC170_frm1_lbl_TipoAgente")}: {tipoAgente}</strong>
                        </Col>
                    </Row>
                    <hr />
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="puntosDeEntrega">{t("REC170_frm1_lbl_puntosDeEntrega")}</label>
                                <FieldSelect name="puntosDeEntrega" />
                                <StatusMessage for="puntosDeEntrega" />
                            </div>
                        </Col>
                    </Row>

                </Modal.Body>
                <Modal.Footer>
                    <Button variant="outline-secondary" onClick={handleClose}>
                        {t("General_Sec0_btn_Cerrar")}
                    </Button>
                    <SubmitButton id="btnEnviarTracking" variant="primary" label="General_Sec0_btn_Confirmar" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}