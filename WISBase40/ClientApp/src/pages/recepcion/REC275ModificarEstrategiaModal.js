import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, StatusMessage, SubmitButton, FormButton } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Container } from 'react-bootstrap';
import * as Yup from 'yup';
import { Grid } from '../../components/GridComponents/Grid';
import { REC275AgregarLogicaModal } from './REC275AgregarLogicaModal';
import { REC275ModificarLogicaModal } from './REC275ModificarLogicaModal';

function InternalREC275ModificarEstrategiaModal(props) {

    const { t } = useTranslation();

    const [logica, setLogica] = useState("");
    const [codigoInstancia, setCodigoInstancia] = useState("");
    const [descripcionProceso, setDescripcionProceso] = useState("");
    const [ordenAscendente, setOrdenAscendente] = useState("");

    const [showModal, setShowModal] = useState(false);
    const [showPopupAgregarLogica, setShowPopupAgregarLogica] = useState(false);
    const [showPopupModificarLogica, setShowPopupModificarLogica] = useState(false);

    const initialValues = {
        nombreEstrategia: "",
        codigoEstrategia: "",
    };

    const validationSchema = {
        nombreEstrategia: Yup.string(),
    };

    const handleClose = () => {
        props.onHide(null, props.nexus);
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {
            props.onHide(null, nexus);
        }
    }

    const openFormAgregarLogica = () => {
        setShowPopupAgregarLogica(true);
        setShowModal(true);
    }

    const openFormModificarLogica = () => {
        setShowPopupModificarLogica(true);
        setShowModal(true);
    }

    const afterButtonAction = (data, nexus) => {
        if (data.buttonId === "btnDown" || data.buttonId === "btnUp") {
            nexus.getGrid("REC275_grid_2").refresh();
        }
    }

    const GridOnBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditarInstanciaLogica") {
            context.abortServerCall = true;
            setLogica(data.row.cells.find(w => w.column == "NU_ALM_LOGICA").value);
            setCodigoInstancia(data.row.cells.find(w => w.column == "NU_ALM_LOGICA_INSTANCIA").value);
            setDescripcionProceso(data.row.cells.find(w => w.column == "DS_ALM_LOGICA_INSTANCIA").value);
            setOrdenAscendente(data.row.cells.find(w => w.column == "FL_ORDEN_ASC").value);
            openFormModificarLogica();
        }
    }

    const closeFormDialog = (nexus) => {
        setShowPopupAgregarLogica(false);
        setShowPopupModificarLogica(false);
        setShowModal(false);

        if (nexus) {
            nexus.getGrid("REC275_grid_2").refresh();
        }

        document.getElementById("btnAsignarLogica").disabled = false;
    };

    const applyParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "modoEditar", value: true },
            { id: "numeroEstrategia", value: props.codigoEstrategia },
        ];
    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = [
            { id: "modoEditar", value: true },
            { id: "numeroEstrategia", value: props.codigoEstrategia },
        ];
    };

    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        query.parameters = [
            { id: "modoEditar", value: true },
            { id: "numeroEstrategia", value: props.codigoEstrategia },
        ];
    }

    const onAfterCommit = (context, rows, parameters, nexus) => {
        nexus.getGrid("REC275_grid_2").refresh();
    }

    const showFormAgregarLogica = () => {
        return (<REC275AgregarLogicaModal show={showPopupAgregarLogica} onHide={closeFormDialog} codigoEstrategia={props.codigoEstrategia} />);
    }

    const showFormModificarLogica = () => {
        return (<REC275ModificarLogicaModal show={showPopupModificarLogica} onHide={closeFormDialog} codigoEstrategia={props.codigoEstrategia} logica={logica} codigoInstancia={codigoInstancia} descripcionProceso={descripcionProceso} ordenAscendente={ordenAscendente} />);
    }

    const handleFormOnBeforeButtonAction = (context, form, query, nexus) => {
        if (query.buttonId === "btnAsignarLogica") {
            context.abortServerCall = true;
            openFormAgregarLogica();
        }
    }

    return (

        <Form
            application="REC275"
            id="REC275_form_1"
            initialValues={initialValues}
            validationSchema={validationSchema}
            onAfterSubmit={handleFormAfterSubmit}
            onBeforeSubmit={handleFormBeforeSubmit}
            onBeforeInitialize={handleFormBeforeInitialize}
            onBeforeButtonAction={handleFormOnBeforeButtonAction}
        >
            <Modal.Header closeButton>
                <Modal.Title>{t("REC275_Sec0_mdlEdit_Titulo")}</Modal.Title>
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
                                <label htmlFor="nombreEstrategia">{t("REC275_grid1_colname_NombreEstrategia")}</label>
                                <Field name="nombreEstrategia" />
                                <StatusMessage for="nombreEstrategia" />
                            </div>
                        </Col>

                    </Row>
                    <div style={{ textAlign: "center" }}>
                        <div>
                            <FormButton id="btnAsignarLogica" className="btn btn-primary" value={t("REC275_Sec0_button_AgregarLogica")} />
                        </div>
                    </div>
                    <div className="row mb-4" >
                        <div className="col-12">
                            <Grid id="REC275_grid_2" rowsToFetch={30} rowsToDisplay={15} enableExcelExport={true}
                                onBeforeFetch={applyParameters}
                                onBeforeInitialize={applyParameters}
                                onBeforeExportExcel={applyParameters}
                                onAfterButtonAction={afterButtonAction}
                                onBeforeButtonAction={GridOnBeforeButtonAction}
                                onBeforeFetchStats={applyParameters}
                                onAfterCommit={onAfterCommit}
                                onBeforeApplyFilter={applyParameters}
                                onBeforeApplySort={applyParameters}
                            />
                        </div>
                    </div>
                </Container>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("REC275_frm1_btn_Cerrar")} </Button>
                <SubmitButton id="btnSubmitConfirmarEstrategia" variant="primary" label="REC275_frm1_btn_Confirmar" />
            </Modal.Footer>
            <Modal show={showModal} onHide={closeFormDialog} dialogClassName="modal-90w" backdrop="static" >
                {showPopupAgregarLogica ? showFormAgregarLogica() : null}
                {showPopupModificarLogica ? showFormModificarLogica() : null}
            </Modal>
        </Form>
    );
}

export const REC275ModificarEstrategiaModal = withPageContext(InternalREC275ModificarEstrategiaModal);