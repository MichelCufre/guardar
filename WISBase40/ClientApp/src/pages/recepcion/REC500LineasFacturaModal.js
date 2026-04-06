import React, { useState, useRef } from 'react';
import { Page } from '../../components/Page';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { useCustomTranslation } from '../../components/TranslationHook';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FieldDateTime, FormButton } from '../../components/FormComponents/Form';
import * as Yup from 'yup';
import { withPageContext } from '../../components/WithPageContext';
import { ConfirmationBox } from '../../components/ConfirmationBox';
import { useToaster } from '../../components/ToasterHook';


function InternalREC500LineasFacturaModal(props) {

    const { t } = useCustomTranslation();
    const toaster = useToaster();
    const facturaParams = props.factura || [];

    const [infoFactura, setInfoFactura] = useState(null);
    const [NotAsignada, setNotAsignada] = useState(true);


    const initialValues = {
        idEmpresa: facturaParams.find(a => a.id === "idEmpresa")?.value,
        cdCliente: facturaParams.find(a => a.id === "cdCliente")?.value,
        idReferencias: "",
    };

    const validationSchema = {

    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {

        query.parameters = [

            { id: "keyFactura", value: props.factura.find(a => a.id === "idFactura").value }
        ];

    }
    const handleFormAfterInitialize = (context, form, query, nexus) => {
        if (query.parameters.find(x => x.id === "infoFactura") != null) {
            setInfoFactura(query.parameters.find(x => x.id === "infoFactura").value);
        }
        if (query.parameters.find(x => x.id === "NotAsignada") != null) {
            setNotAsignada(false);
        }



    };

    const handleFormBeforeSubmit = (context, form, query, nexus) => {

        const rowsEntrada = nexus.getGrid("REC500Lineas_grid_1").getModifiedRows();

        if (nexus.getGrid("REC500Lineas_grid_1").hasError()) {
            context.abortServerCall = true;

            toaster.toastError("REC500_frm1_error_ErroresEnLineas");

            return false;
        }
        

        query.parameters = [
            { id: "keyFactura", value: props.factura.find(a => a.id === "idFactura").value },
            { id: "rowsDetalle", value: JSON.stringify(rowsEntrada) },
            { id: "cdCliente", value: props.factura.find(a => a.id === "cdCliente") },
            { id: "idEmpresa", value: props.factura.find(a => a.id === "idEmpresa") },
        ];

    }

    const handleFormBeforeValidateField = (context, form, query, nexus) => {

        const rowsEntrada = nexus.getGrid("REC500Lineas_grid_1").getModifiedRows();

        query.parameters = [
            { id: "keyFactura", value: props.factura.find(a => a.id === "idFactura").value },
            { id: "rowsDetalle", value: JSON.stringify(rowsEntrada) },
        ];

    }

    const handleFormAfterValidateField = (context, form, query, nexus) => {


    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {

        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {

            if (query.buttonId === "btnSubmitConfirmar") {

                props.nexus.getGrid("REC500_grid_1").refresh();

                props.onHide(null, null);

            } else {

                nexus.getGrid("REC500Lineas_grid_1").refresh();
            }

            props.nexus.getGrid("REC500_grid_1").refresh();

        }
        else {
            if (query.parameters.find(w => w.id === "rowValidated")) {
                nexus.getGrid("REC500Lineas_grid_1").updateRows(JSON.parse(query.parameters.find(w => w.id === "rowValidated").value));
            }
        }
    }


    const onAfterInitialize = (context, grid, parameters, nexus) => {


    }



    const handleGridBeforeValidate = (context, data, nexus) => {

        data.parameters = [
            { id: "keyFactura", value: props.factura.find(a => a.id === "idFactura").value }
        ];

    }
    const handleGridBeforeSelectSearch = (context, grid, query, nexus) => {
        query.parameters = [
            { id: "keyFactura", value: props.factura.find(a => a.id === "idFactura").value }
        ];
    }

    const handleBeforeImportExcel = (context, data, nexus) => {

        data.parameters = [
            { id: "keyFactura", value: props.factura.find(a => a.id === "idFactura").value },
            { id: "importExcel", value: "true" }
        ];

    }

    const handleBeforeExportExcel = (context, data, nexus) => {

        data.parameters = [
            { id: "keyFactura", value: props.factura.find(a => a.id === "idFactura").value },
        ];

    }
    const handleGridBeforeCommit = (context, data, nexus) => {

        context.abortServerCall = true;

    }

    const handleOnBeforeButtonAction = (context, form, query, nexus) => {

        if (query.buttonId === "btnCerrarModal") {
            context.abortServerCall = true;
            if (nexus.getGrid("REC500Lineas_grid_1").getModifiedRows().length > 0) {

                nexus.showConfirmation({
                    message: "General_Sec0_Error_CambiosSinSalvar",
                    onAccept: () => props.onHide(null, null)
                });

            }
            else {

                props.onHide(null, null);
            }

        }
    };

    const handleOnAfterApplyFilter = (context, data, nexus) => {
        data.parameters = [
            { id: "keyFactura", value: props.factura.find(a => a.id === "idFactura").value },

        ];
    };

    const handleOnBeforeApplySort = (context, data, nexus) => {
        data.parameters = [
            { id: "keyFactura", value: props.factura.find(a => a.id === "idFactura").value },

        ];
    };

    const handleOnBeforeFetchStats = (context, data, nexus) => {
        data.parameters = [
            { id: "keyFactura", value: props.factura.find(a => a.id === "idFactura").value },

        ];
    };

    const handleOnBeforeFetch = (context, data, nexus) => {

        data.parameters = [
            { id: "keyFactura", value: props.factura.find(a => a.id === "idFactura").value },

        ];
    };

    const handleFormBeforeSelectSearch = (context, form, query, nexus) => {
        query.parameters = [
            { id: "keyFactura", value: props.factura.find(a => a.id === "idFactura").value },

        ];
    };

    if (!props.show) {
        return null;
    }

    return (
        <Page

            //title={t("REC170_Sec0_pageTitle_Titulo")}
            {...props}
        >

            <Form
                application="REC500Lineas"
                id="REC500Lineas_form_1"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeInitialize={handleFormBeforeInitialize}
                onAfterInitialize={handleFormAfterInitialize}
                onBeforeValidateField={handleFormBeforeValidateField}
                onBeforeSelectSearch={handleFormBeforeSelectSearch}
                onAfterValidateField={handleFormAfterValidateField}
                onBeforeSubmit={handleFormBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
                onBeforeButtonAction={handleOnBeforeButtonAction}

            >
                <Field name="idEmpresa" type="hidden" />
                <Field name="cdCliente" type="hidden" />
                <Field name="idReferencias" type="hidden" />

                <Modal.Header >
                    <Modal.Title>{t("REC500_Sec0_mdlLineas_Titulo")} {`${infoFactura}`}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Tabs defaultActiveKey="Buscador" transition={false} id="noanim-tab-example">
                        <Tab eventKey="Buscador" title="Agregar detalles por referencias">
                        <br></br>
                            <Container fluid>
                                <Row>
                                    <Col lg={10}>
                                        <div className="form-group"  >
                                            <FieldSelectAsync name="idReferencias" isClearable searchUrl="/api/REC/REC500Lineas/SearchReferencia"
                                                labelField="label" valueField="value" />
                                            <StatusMessage for="idReferencias" />
                                        </div>
                                    </Col>
                                    <Col lg={2}>
                                        <div style={{ textAlign: "right" }}>
                                            <SubmitButton id="btnConfirmarReferencia" variant="primary" label="REC500_frm1_btn_confirmarReferencia" />
                                        </div>
                                    </Col>
                                </Row>
                            </Container>
                        </Tab>
                    </Tabs>
                    <br></br>
                    <Container fluid>

                        <Grid
                            application="REC500Lineas"
                            id="REC500Lineas_grid_1"
                            rowsToFetch={30}
                            rowsToDisplay={10}
                            onBeforeInitialize={handleOnBeforeFetch}
                            onBeforeFetch={handleOnBeforeFetch}
                            onBeforeFetchStats={handleOnBeforeFetchStats}
                            onBeforeValidateRow={handleGridBeforeValidate}
                            onBeforeSelectSearch={handleGridBeforeSelectSearch}

                            onBeforeImportExcel={handleBeforeImportExcel}
                            onBeforeGenerateExcelTemplate={handleBeforeImportExcel}
                            onBeforeExportExcel={handleBeforeExportExcel}

                            onBeforeCommit={handleGridBeforeCommit}
                            onBeforeApplyFilter={handleOnBeforeFetch}
                            onAfterApplyFilter={handleOnAfterApplyFilter}
                            onBeforeApplySort={handleOnBeforeApplySort}
                            onBeforeApplyFilter={handleOnBeforeFetch}

                            enableExcelExport
                            enableExcelImport={NotAsignada}
                            autofocus={true}
                        />

                    </Container>
                </Modal.Body>
                <Modal.Footer>
                    <FormButton id="btnCerrarModal" variant="outline-secondary" label="REC500_frm1_btn_cerrar" />
                    <SubmitButton id="btnSubmitGuardar" variant="primary" label="REC500_frm1_btn_guardar" />
                    <SubmitButton id="btnSubmitConfirmar" variant="primary" label="REC500_frm1_btn_confirmar" />
                </Modal.Footer>
            </Form>
        </Page>

    );
}


export const REC500LineasFacturaModal = withPageContext(InternalREC500LineasFacturaModal);