import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, FieldSelect, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Container } from 'react-bootstrap';
import * as Yup from 'yup';

function InternalIMP080ImpresionEtiquetasRecModal(props) {

    const { t } = useTranslation();

    const [modalShowSeleccion, setModalShowSeleccion] = useState(false);

    const [modalShowImpresora, setModalShowImpresora] = useState(false);

    const [rowSeleccionadasImprimir, setRowSeleccionadasImprimir] = useState(null);

    const validationSchema = {

        predio: Yup.string().required(),
        impresora: Yup.string().required(),
        estilo: Yup.string().required(),
        lenguaje: Yup.string().required(),
        descripcionLenguaje: Yup.string(),
        numCopias: Yup.string().required(),
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {

        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {

            handleClose();
        }
    }

    const onBeforeSubmit = (context, form, query, nexus) => {

        query.parameters = [{ id: "ListaFilasSeleccionadas", value: rowSeleccionadasImprimir }];
        query.parameters.push({ id: "isSubmit", value: true });
    }

    const applyParameters = (context, data, nexus) => {
        if (props.agenda)
            data.parameters = [{ id: "agenda", value: props.agenda.find(x => x.id === "idAgenda").value }];
    }

    const GridOnAfterMenuItemAction = (context, data, nexus) => {
        let jsonAdded = data.parameters.find(w => w.id === "ListaFilasSeleccionadas").value;

        setRowSeleccionadasImprimir(jsonAdded);

        changeModal();
    }

    const changeModal = () => {
        setModalShowImpresora(true);
    }

    const handleClose = () => {
        setModalShowSeleccion(false);
        setModalShowImpresora(false);
        props.onHide();
    };

    return (
        <Page
            application="IMP080ImpresionEtiqRec"
            {...props}
        >
            <Modal show={props.show && !modalShowImpresora} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
                <Modal.Header closeButton>
                    <Modal.Title>{t("REC170ImpresionEtiq_Sec0_mdl_ImpresionEtiqModal_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Page
                        application="REC170ImpresionEtiq"
                        {...props}
                    >
                        <Grid
                            application="REC170ImpresionEtiq"
                            id="REC170ImpresionEtiq_grid_1"
                            rowsToFetch={30}
                            rowsToDisplay={15}
                            enableExcelExport
                            enableSelection
                            onBeforeInitialize={applyParameters}
                            onBeforeFetch={applyParameters}
                            onBeforeExportExcel={applyParameters}
                            onBeforeButtonAction={applyParameters}
                            onBeforeMenuItemAction={applyParameters}
                            onBeforeFetchStats={applyParameters}
                            onBeforeApplyFilter={applyParameters}
                            onBeforeApplySort={applyParameters}
                            onAfterMenuItemAction={GridOnAfterMenuItemAction}
                            onAfterValidateRow={applyParameters}
                        />
                    </Page>
                </Modal.Body>

                { /*
                 * TODO: CAMBIAR A QUE CAPTURE EL EVENTO DEL GridMenuItemAction y lo triggeree cuando hago Connfirmar en el boton
                 * 
                    <Modal.Footer>
                        <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("REC170ImpresionEtiq_frm1_btn_cerrar")} </Button>
                        <Button variant="btn btn-outline-primary" onClick={HandleMenuButtonAction}> {t("REC170ImpresionEtiq_frm1_btn_confirmar")} </Button>
                    </Modal.Footer>
                *
                *
                */}
            </Modal>

            {/* MODAL DE FORMULARIO DE IMPRESORA */}
            <Modal show={modalShowImpresora} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
                <Form
                    application="IMP080ImpresionEtiqRec"
                    id="IMP080ImpresionEtiqRec_form_1"
                    validationSchema={validationSchema}
                    onBeforeSubmit={onBeforeSubmit}
                    onAfterSubmit={handleFormAfterSubmit}
                >

                    <Modal.Header closeButton>
                        <Modal.Title>{t("IMP080_Sec0_mdl_ImpresionEtiquetasRec_Titulo")}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <Container fluid>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="predio">{t("IMP080_frm1_lbl_predio")}</label>
                                        <FieldSelect name="predio" />
                                        <StatusMessage for="predio" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="impresora">{t("IMP080_frm1_lbl_impresora")}</label>
                                        <FieldSelect name="impresora" />
                                        <StatusMessage for="impresora" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="estilo">{t("IMP080_frm1_lbl_estilo")}</label>
                                        <FieldSelect name="estilo" />
                                        <StatusMessage for="estilo" />
                                    </div>
                                </Col>
                                <Col>
                                    <Row>
                                        <Col sm={4}>
                                            <div className="form-group" >
                                                <label htmlFor="lenguaje">{t("IMP080_frm1_lbl_lenguaje")}</label>
                                                <Field name="lenguaje" />
                                                <StatusMessage for="lenguaje" />
                                            </div>
                                        </Col>
                                        <Col sm={8}>
                                            <div className="form-group" >
                                                <label htmlFor="descripcionLenguaje">{t("IMP080_frm1_lbl_descripcionLenguaje")}</label>
                                                <Field name="descripcionLenguaje" />
                                                <StatusMessage for="descripcionLenguaje" />
                                            </div>
                                        </Col>
                                    </Row>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="numCopias">{t("IMP080_frm1_lbl_numCopias")}</label>
                                        <Field name="numCopias" />
                                        <StatusMessage for="numCopias" />
                                    </div>
                                </Col>
                            </Row>
                        </Container>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("IMP080_frm1_btn_cerrar")} </Button>
                        <SubmitButton id="btnSubmitConfirmar" variant="primary" label="IMP080_frm1_btn_confirmar" />
                    </Modal.Footer>
                </Form>
            </Modal>

        </Page>
    );
}

export const IMP080ImpresionEtiquetasRecModal = withPageContext(InternalIMP080ImpresionEtiquetasRecModal);