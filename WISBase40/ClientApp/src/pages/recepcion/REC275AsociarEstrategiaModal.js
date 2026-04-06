import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Container } from 'react-bootstrap';
import * as Yup from 'yup';
import { Grid } from '../../components/GridComponents/Grid';
import { REC275CrearAsociacionModal } from './REC275CrearAsociacionModal';

function InternalREC275AsociarEstrategiaModal(props) {

    const { t } = useTranslation();

    const [showModalCrearAsociacion, setShowModalCrearAsociacion] = useState(false);

    const [showPopupCrearAsociacion, setShowPopupCrearAsociacion] = useState(false);


    const handleClose = () => {
        props.onHide(null, props.nexus);
    };

    const initialValues = {
        codigoEstrategia: "",
    };

    const validationSchema = {
        codigoEstrategia: Yup.string(),
    };

    const openFormCrearAsociacion = () => {
        setShowPopupCrearAsociacion(true);
        setShowModalCrearAsociacion(true);

    }

    const applyParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "numeroEstrategia", value: props.codigoEstrategia },
        ];
    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = [
            { id: "numeroEstrategia", value: props.codigoEstrategia },
        ];
    };

    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        query.parameters = [
            { id: "numeroEstrategia", value: props.codigoEstrategia },
        ];
    }

    const closeFormCrearAsociacion = (nexus) => {
        setShowPopupCrearAsociacion(false);
        setShowModalCrearAsociacion(false);

        if (nexus) {
            nexus.getGrid("REC275_grid_1").refresh();
            nexus.getGrid("REC275_grid_Asociaciones").refresh();
        }

    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        var idButton = query.buttonId;

        if (idButton === "btnSubmitConfirmarAsociaciones") {
            props.onHide(null, nexus);
        }
    }

    const onAfterCommit = (context, rows, parameters, nexus) => {
        nexus.getGrid("REC275_grid_1").refresh();
        nexus.getGrid("REC275_grid_Asociaciones").refresh();
    }

    const showFormCrearAsociacionModal = () => { return (<REC275CrearAsociacionModal show={showPopupCrearAsociacion} onHide={closeFormCrearAsociacion} codigoEstrategia={props.codigoEstrategia} />); }


    return (

        <Form
            application="REC275AsociarEstrategia"
            id="REC275_form_AsociarEstrategia"
            initialValues={initialValues}
            validationSchema={validationSchema}
            onBeforeSubmit={handleFormBeforeSubmit}
            onBeforeInitialize={handleFormBeforeInitialize}
            onAfterSubmit={handleFormAfterSubmit}
        >
            <Modal.Header closeButton>
                <Modal.Title>{t("REC275_Sec0_mdlCreate_AsociarT")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Container fluid>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="codigoEstrategia">{t("REC275_grid1_colname_CodigoEstrategia")}</label>
                                <Field name="codigoEstrategia" readOnly />
                                <StatusMessage for="codigoEstrategia" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="descripcionEstrategia">{t("REC275_form_lbl_DescripcionEstrategia")}</label>
                                <Field name="descripcionEstrategia" readOnly />
                                <StatusMessage for="descripcionEstrategia" />
                            </div>
                        </Col>
                    </Row>

                    <div style={{ textAlign: "center" }}>
                        <button id="btnAsociarEstrategia" onClick={openFormCrearAsociacion} className="btn btn-primary">{t("REC275_Sec0_button_CrearAsociacion")}</button>
                    </div>

                    <div className="col-12">
                        <Grid application="REC275AsociarEstrategia" id="REC275_grid_Asociaciones" rowsToFetch={30} rowsToDisplay={15} enableExcelExport={true}
                            onBeforeFetch={applyParameters}
                            onBeforeInitialize={applyParameters}
                            onBeforeExportExcel={applyParameters}
                            onBeforeFetchStats={applyParameters}
                            onAfterCommit={onAfterCommit}
                            onBeforeApplyFilter={applyParameters}
                            onBeforeApplySort={applyParameters}
                        />
                    </div>
                </Container>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("REC275_frm1_btn_Cerrar")} </Button>
                <SubmitButton id="btnSubmitConfirmarAsociaciones" variant="primary" label="REC275_frm1_btn_Confirmar" />
            </Modal.Footer>
            <Modal show={showModalCrearAsociacion} onHide={closeFormCrearAsociacion} dialogClassName="modal-90w" backdrop="static" >
                {showPopupCrearAsociacion ? showFormCrearAsociacionModal() : null}
            </Modal>
        </Form>
    );
}

export const REC275AsociarEstrategiaModal = withPageContext(InternalREC275AsociarEstrategiaModal);