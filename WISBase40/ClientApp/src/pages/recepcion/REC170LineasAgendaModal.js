import React, { useState, useRef } from 'react';
import { Page } from '../../components/Page';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { useCustomTranslation } from '../../components/TranslationHook';
import { Form, SubmitButton, FormButton } from '../../components/FormComponents/Form';
import { withPageContext } from '../../components/WithPageContext';
import { useToaster } from '../../components/ToasterHook';

function InternalREC170LineasAgendaModal(props) {

    const { t } = useCustomTranslation();
    const toaster = useToaster();

    const [infoAgenda, setInfoAgenda] = useState(null);

    const initialValues = {

    };

    const validationSchema = {

    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {

        query.parameters = [

            { id: "keyAgenda", value: props.agenda.find(a => a.id === "idAgenda").value }
        ];

    }
    const handleFormAfterInitialize = (context, form, query, nexus) => {
        if (query.parameters.find(x => x.id === "infoAgenda") != null) {
            setInfoAgenda(query.parameters.find(x => x.id === "infoAgenda").value);
        }


    };

    const handleFormBeforeSubmit = (context, form, query, nexus) => {

        if (nexus.getGrid("REC170Lineas_grid_1").hasError()) {
            context.abortServerCall = true;

            toaster.toastError("REC170_frm1_error_ErroresEnLineas");

            return false;
        }

        const rowsEntrada = nexus.getGrid("REC170Lineas_grid_1").getModifiedRows();

        query.parameters = [
            { id: "keyAgenda", value: props.agenda.find(a => a.id === "idAgenda").value },
            { id: "rowsDetalle", value: JSON.stringify(rowsEntrada) },
        ];

    }
    const handleFormBeforeValidateField = (context, form, query, nexus) => {

        const rowsEntrada = nexus.getGrid("REC170Lineas_grid_1").getModifiedRows();

        query.parameters = [
            { id: "keyAgenda", value: props.agenda.find(a => a.id === "idAgenda").value },
            { id: "rowsDetalle", value: JSON.stringify(rowsEntrada) },
        ];

    }

    const handleFormAfterValidateField = (context, form, query, nexus) => {


    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {

        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {

            if (query.buttonId === "btnSubmitConfirmar") {

                props.nexus.getGrid("REC170_grid_1").refresh();

                props.onHide(null, null);

            } else {

                nexus.getGrid("REC170Lineas_grid_1").refresh();
            }

            props.nexus.getGrid("REC170_grid_1").refresh();

        }
        else {
            if (query.parameters.find(w => w.id === "rowValidated")) {
                nexus.getGrid("REC170Lineas_grid_1").updateRows(JSON.parse(query.parameters.find(w => w.id === "rowValidated").value));
            }
        }
    }


    const onAfterInitialize = (context, grid, parameters, nexus) => {


    }



    const handleGridBeforeValidate = (context, data, nexus) => {

        data.parameters = [
            { id: "keyAgenda", value: props.agenda.find(a => a.id === "idAgenda").value }
        ];

    }
    const handleGridBeforeSelectSearch = (context, grid, query, nexus) => {
        query.parameters = [
            { id: "keyAgenda", value: props.agenda.find(a => a.id === "idAgenda").value }
        ];
    }

    const handleBeforeImportExcel = (context, data, nexus) => {

        data.parameters = [
            { id: "keyAgenda", value: props.agenda.find(a => a.id === "idAgenda").value },
            { id: "importExcel", value: "true" }
        ];

    }
    const handleGridBeforeCommit = (context, data, nexus) => {

        context.abortServerCall = true;

    }

    const handleOnBeforeButtonAction = (context, form, query, nexus) => {
        if (query.buttonId === "btnCerrarModal") {
            context.abortServerCall = true;
            if (nexus.getGrid("REC170Lineas_grid_1").getModifiedRows().length > 0) {

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
            { id: "keyAgenda", value: props.agenda.find(a => a.id === "idAgenda").value },

        ];
    };

    const handleOnBeforeApplySort = (context, data, nexus) => {
        data.parameters = [
            { id: "keyAgenda", value: props.agenda.find(a => a.id === "idAgenda").value },

        ];
    };

    const handleOnBeforeFetchStats = (context, data, nexus) => {
        data.parameters = [
            { id: "keyAgenda", value: props.agenda.find(a => a.id === "idAgenda").value },

        ];
    };

    const handleOnBeforeFetch = (context, data, nexus) => {

        data.parameters = [
            { id: "keyAgenda", value: props.agenda.find(a => a.id === "idAgenda").value },

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
                application="REC170Lineas"
                id="REC170Lineas_form_1"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeInitialize={handleFormBeforeInitialize}
                onAfterInitialize={handleFormAfterInitialize}
                onBeforeValidateField={handleFormBeforeValidateField}
                onAfterValidateField={handleFormAfterValidateField}
                onBeforeSubmit={handleFormBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
                onBeforeButtonAction={handleOnBeforeButtonAction}

            >

                <Modal.Header >
                    <Modal.Title>{t("REC170_Sec0_mdlLineas_Titulo")} {`${infoAgenda}`}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Container fluid>

                        <Grid
                            application="REC170Lineas"
                            id="REC170Lineas_grid_1"
                            rowsToFetch={30}
                            rowsToDisplay={10}
                            onBeforeInitialize={handleOnBeforeFetch}
                            onBeforeFetch={handleOnBeforeFetch}
                            onBeforeFetchStats={handleOnBeforeFetchStats}
                            onBeforeValidateRow={handleGridBeforeValidate}
                            onBeforeSelectSearch={handleGridBeforeSelectSearch}
                            onBeforeImportExcel={handleBeforeImportExcel}
                            onBeforeGenerateExcelTemplate={handleBeforeImportExcel}
                            onBeforeCommit={handleGridBeforeCommit}
                            onBeforeApplyFilter={handleOnBeforeFetch}
                            onAfterApplyFilter={handleOnAfterApplyFilter}
                            onBeforeApplySort={handleOnBeforeApplySort}
                            onBeforeApplyFilter={handleOnBeforeFetch}
                            enableExcelExport={false}
                            enableExcelImport
                            autofocus={true}
                        />

                    </Container>
                </Modal.Body>
                <Modal.Footer>
                    <FormButton id="btnCerrarModal" variant="outline-secondary" label="REC170_frm1_btn_cerrar" />
                    <SubmitButton id="btnSubmitGuardar" variant="primary" label="REC170_frm1_btn_guardar" />
                    <SubmitButton id="btnSubmitConfirmar" variant="primary" label="REC170_frm1_btn_confirmar" />
                </Modal.Footer>
            </Form>
        </Page>

    );
}


export const REC170LineasAgendaModal = withPageContext(InternalREC170LineasAgendaModal);