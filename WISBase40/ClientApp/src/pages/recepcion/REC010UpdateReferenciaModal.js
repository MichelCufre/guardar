import React, { useState, useRef } from 'react';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { useCustomTranslation } from '../../components/TranslationHook';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FieldDateTime } from '../../components/FormComponents/Form';
import * as Yup from 'yup';
import { withPageContext } from '../../components/WithPageContext';
import { ConfirmationBox } from '../../components/ConfirmationBox';
import { useToaster } from '../../components/ToasterHook';


function InternalREC010UpdateReferenciaModal(props) {

    const { t } = useCustomTranslation();
    const toaster = useToaster();

    const [esInsertable, setEsInsertable] = useState(false);
    const [fomrSaveUpdate, setfomrSaveUpdate] = useState(false);
    const [keyTab, setKeyTab] = useState('datosReferencias');

    const initialValues = {

        codigo: "",
        tipoReferencia: "",
        idEmpresa: "",
        codigoInternoAgente: "",
        numeroPredio: "",
        fechaVencimiento: "",
        fechaEmitida: "",
        fechaEntrega: "",
        memo: "",
        anexo1: "",
        anexo2: "",
        anexo3: "",
        moneda: ""
    };

    const validationSchema = {

        numeroPredio: Yup.string().required(),
        fechaVencimiento: Yup.date().transform(value => (isNaN(value) ? undefined : value)),
        fechaEmitida: Yup.date().transform(value => (isNaN(value) ? undefined : value)).required(t("General_Sec0_Error_Error25")),
        fechaEntrega: Yup.date().transform(value => (isNaN(value) ? undefined : value)),//.required(t("General_Sec0_Error_Error25")),
        memo: Yup.string(),
        anexo1: Yup.string(),
        anexo2: Yup.string(),
        anexo3: Yup.string(),
        moneda: Yup.string(),
    };

    const handleClose = () => {

        let formHasChange = props.nexus.getForm("REC010Update_form_1").hasChanges(["numeroPredio", "fechaVencimiento", "fechaEmitida", "fechaEntrega", "memo", "anexo1", "anexo2", "anexo3"])

        if ((formHasChange && !fomrSaveUpdate) || props.nexus.getGrid("REC010Update_grid_1").getModifiedRows().length > 0) {

            props.nexus.showConfirmation({
                message: "General_Sec0_Error_CambiosSinSalvar",
                onAccept: () => props.onHide()
            });

        }
        else {

            props.onHide();
        }

    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = [

            { id: "keyReferencia", value: props.referencia }
        ];

    }

    const handleFormBeforeSubmit = (context, form, query, nexus) => {

        if (nexus.getGrid("REC010Update_grid_1").hasError()) {
            context.abortServerCall = true;

            toaster.toastError("REC010_frm1_error_ErroresEnLineas");

            return false;
        }

        const rowsEntrada = nexus.getGrid("REC010Update_grid_1").getModifiedRows();

        query.parameters = [
            { id: "keyReferencia", value: props.referencia },
            { id: "rowsDetalle", value: JSON.stringify(rowsEntrada) },
        ];

    }
    const handleFormBeforeValidateField = (context, form, query, nexus) => {

        const rowsEntrada = nexus.getGrid("REC010Update_grid_1").getModifiedRows();

        query.parameters = [
            { id: "keyReferencia", value: props.referencia },
            { id: "rowsDetalle", value: JSON.stringify(rowsEntrada) },
        ];

    }

    const handleFormAfterValidateField = (context, form, query, nexus) => {

        setfomrSaveUpdate(false);
    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {

        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {

            if (query.buttonId == "btnSubmitConfirmar") {

                nexus.getGrid("REC010_grid_1").refresh();

                props.onHide(null);

            } else {

                nexus.getGrid("REC010Update_grid_1").refresh();
                setfomrSaveUpdate(true);
            }
        }
        else {
            if (query.parameters.find(w => w.id === "rowValidated"))
                nexus.getGrid("REC010Update_grid_1").updateRows(JSON.parse(query.parameters.find(w => w.id === "rowValidated").value));
        }
    }


    const handleGridBeforeValidate = (context, data, nexus) => {

        data.parameters = [
            { id: "empresa", value: nexus.getForm("REC010Update_form_1").getFieldValue("idEmpresa") }
        ];

    }
    const handleGridBeforeSelectSearch = (context, grid, query, nexus) => {
        query.parameters = [{ id: "empresa", value: nexus.getForm("REC010Update_form_1").getFieldValue("idEmpresa") }];
    }

    const addParameters = (context, data, nexus) => {
        data.parameters = [{ id: "keyReferencia", value: props.referencia }];
    }

    const handleBeforeImportExcel = (context, data, nexus) => {

        data.parameters = [
            { id: "keyReferencia", value: props.referencia },
            { id: "empresa", value: nexus.getForm("REC010Update_form_1").getFieldValue("idEmpresa") },
            { id: "importExcel", value: "true" }
        ];

    }
    const handleGridBeforeCommit = (context, data, nexus) => {

        context.abortServerCall = true;

    }

    const handleFormAfterInitialize = (context, form, query, nexus) => {

        const flag = query.parameters.find(d => d.id === "PermiteIngresarLineas");
        setEsInsertable(flag === null ? false : flag);

        if ((flag === null ? false : flag))
            setKeyTab("detail");

    };


    return (
        //  <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
        <Form
            application="REC010Update"
            id="REC010Update_form_1"
            initialValues={initialValues}
            validationSchema={validationSchema}
            onBeforeInitialize={handleFormBeforeInitialize}
            onBeforeSubmit={handleFormBeforeSubmit}
            onAfterSubmit={handleFormAfterSubmit}
            onBeforeValidateField={handleFormBeforeValidateField}
            onAfterValidateField={handleFormAfterValidateField}
            onAfterInitialize={handleFormAfterInitialize}

        >
            <Modal.Header >
                <Modal.Title>{t("REC010_Sec0_mdlUpdate_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Container fluid>
                    <Row>
                        <Col>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="idEmpresa">{t("REC010_frm1_lbl_idEmpresa")}</label>
                                        <FieldSelectAsync name="idEmpresa" />
                                        <StatusMessage for="idEmpresa" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="codigoInternoAgente">{t("REC010_frm1_lbl_codigoInternoAgente")}</label>
                                        <FieldSelectAsync name="codigoInternoAgente" />
                                        <StatusMessage for="codigoInternoAgente" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="tipoReferencia">{t("REC010_frm1_lbl_tipoReferencia")}</label>
                                        <FieldSelect name="tipoReferencia" />
                                        <StatusMessage for="tipoReferencia" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="codigo">{t("REC010_frm1_lbl_codigo")}</label>
                                        <Field name="codigo" maxLength="20" />
                                        <StatusMessage for="codigo" />
                                    </div>
                                </Col>
                            </Row>

                            <Row>
                                <Col>
                                    <Tabs defaultActiveKey="detail" transition={false} id="noanim-tab-example"
                                        activeKey={keyTab}
                                        onSelect={(k) => setKeyTab(k)}
                                    >
                                        <Tab eventKey="detail" title={t("REC010_frm1_tab_detalles")} disabled={!esInsertable}>

                                            <Row className='mt-3'>
                                                <Col>
                                                    <Grid
                                                        application="REC010Update"
                                                        id="REC010Update_grid_1"
                                                        rowsToFetch={30}
                                                        rowsToDisplay={10}
                                                        onBeforeInitialize={addParameters}
                                                        onBeforeFetch={addParameters}
                                                        onBeforeApplyFilter={addParameters}
                                                        onBeforeApplySort={addParameters}
                                                        onBeforeValidateRow={handleGridBeforeValidate}
                                                        onBeforeSelectSearch={handleGridBeforeSelectSearch}
                                                        onBeforeFetchStats={addParameters}
                                                        onBeforeImportExcel={handleBeforeImportExcel}
                                                        onBeforeGenerateExcelTemplate={handleBeforeImportExcel}
                                                        onBeforeCommit={handleGridBeforeCommit}
                                                        enableExcelExport={false}
                                                        enableExcelImport
                                                    //autofocus={true}
                                                    />
                                                </Col>
                                            </Row>

                                        </Tab>
                                        <Tab eventKey="datosReferencias" title={t("REC010_frm1_tab_datosReferencias")}>
                                            <Row >
                                                <Col>
                                                    <Row >
                                                        <Col>
                                                            <div className="form-group" >
                                                                <label htmlFor="numeroPredio">{t("REC010_frm1_lbl_numeroPredio")}</label>
                                                                <FieldSelect name="numeroPredio" />
                                                                <StatusMessage for="numeroPredio" />
                                                            </div>
                                                        </Col>
                                                        <Col>
                                                            <div className="form-group" >
                                                                <label htmlFor="moneda">{t("REC010_frm1_lbl_moneda")}</label>
                                                                <FieldSelect name="moneda" />
                                                                <StatusMessage for="moneda" />
                                                            </div>
                                                        </Col>
                                                    </Row>
                                                    <Row>
                                                        <Col>
                                                            <div className="form-group" >
                                                                <label htmlFor="memo">{t("REC010_frm1_lbl_memo")}</label>
                                                                <FieldTextArea name="memo" maxLength="200" />
                                                                <StatusMessage for="memo" />
                                                            </div>
                                                            <div className="form-group" >
                                                                <label htmlFor="anexo1">{t("REC010_frm1_lbl_anexo1")}</label>
                                                                <FieldTextArea name="anexo1" maxLength="200" />
                                                                <StatusMessage for="anexo1" />
                                                            </div>
                                                        </Col>
                                                    </Row>
                                                </Col>
                                                <Col>
                                                    <Row>
                                                        <Col >
                                                            <div className="form-group" >
                                                                <label htmlFor="fechaVencimiento">{t("REC010_frm1_lbl_fechaVencimiento")}</label>
                                                                <FieldDate name="fechaVencimiento" />
                                                                <StatusMessage for="fechaVencimiento" />
                                                            </div>
                                                        </Col>
                                                        <Col >
                                                            <div className="form-group" >
                                                                <label htmlFor="fechaEmitida">{t("REC010_frm1_lbl_fechaEmitida")}</label>
                                                                <FieldDate name="fechaEmitida" />
                                                                <StatusMessage for="fechaEmitida" />
                                                            </div>
                                                        </Col>
                                                        <Col >
                                                            <div className="form-group" >
                                                                <label htmlFor="fechaEntrega">{t("REC010_frm1_lbl_fechaEntrega")}</label>
                                                                <FieldDate name="fechaEntrega" />
                                                                <StatusMessage for="fechaEntrega" />
                                                            </div>
                                                        </Col>
                                                    </Row>
                                                    <Row>
                                                        <Col>
                                                            <div className="form-group" >
                                                                <label htmlFor="anexo2">{t("REC010_frm1_lbl_anexo2")}</label>
                                                                <FieldTextArea name="anexo2" maxLength="200" />
                                                                <StatusMessage for="anexo2" />
                                                            </div>
                                                            <div className="form-group" >
                                                                <label htmlFor="anexo3">{t("REC010_frm1_lbl_anexo3")}</label>
                                                                <FieldTextArea name="anexo3" maxLength="200" />
                                                                <StatusMessage for="anexo3" />
                                                            </div>
                                                        </Col>
                                                    </Row>
                                                </Col>
                                            </Row>
                                        </Tab>
                                    </Tabs>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                </Container>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("REC010_frm1_btn_cerrar")} </Button>
                <SubmitButton id="btnSubmitGuardar" variant="primary" label="REC010_frm1_btn_guardar" />
                <SubmitButton id="btnSubmitConfirmar" variant="primary" label="REC010_frm1_btn_confirmar" />
            </Modal.Footer>
        </Form>
        // </Modal>

    );

}


export const REC010UpdateReferenciaModal = withPageContext(InternalREC010UpdateReferenciaModal);