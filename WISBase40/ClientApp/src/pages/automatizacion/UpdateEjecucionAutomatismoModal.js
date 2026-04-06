import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Form, FormButton } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';
import { Modal } from 'react-bootstrap';

export function UpdateEjecucionAutomatismoModal(props) {

    const { t } = useTranslation();

    const [automatismoEjecucion, setAutomatismoEjecucion] = useState(null);

    const [showDetails, setShowDetails] = useState(false);

    const handleClose = () => {
        props.onHide();
    };

    //-------------- FORM ----------------

    const onBeforeButtonAction = (context, form, query, nexus) => {
        context.abortServerCall = true;

        if (query.buttonId === "reprocesarInterfaz") {
            context.abortServerCall = false;

            query.parameters = [{ id: "NU_AUT_EJECUCION", value: props.automatismoEjecucion }];

            return;
        }

        if (query.buttonId === "closeFormButton") {
            handleClose();
        }
        else {
            nexus.showConfirmation({
                message: "General_Sec0_Info_DeseaGuardarCambios",
                onAccept: () => commitGrids(nexus)
            });
        }
    };

    function commitGrids(nexus) {
        nexus.getGrid("AUT101EditarEjecuciones_grid_1").commit(true, true);

        if (showDetails)
            nexus.getGrid("AUT101EditarEjecuciones_grid_2").commit(true, true);        
    }

    //------------------------------------

    //-------------- GRID 1 --------------

    const onBeforeInitialize = (context, data, nexus) => {
        setShowDetails(false);
        setAutomatismoEjecucion(props.automatismoEjecucion);
        data.parameters.push({ id: "NU_AUT_EJECUCION", value: props.automatismoEjecucion });
    };

    const onAfterInitialize = (context, grid, parameters, nexus) => {
        const showDetailsParam = parameters.find(d => d.id === "SHOW_DETAILS_GRID");

        if (showDetailsParam != null && showDetailsParam.value == "S") {
            setShowDetails(true);
            nexus.getGrid("AUT101EditarEjecuciones_grid_2").reset();
        }
    };

    const addParameters = (context, data, nexus) => {
        data.parameters = [{ id: "NU_AUT_EJECUCION", value: automatismoEjecucion }];
    };

    const onAfterCommit = (context, rows, data, nexus) => {

        if (context.status === "ERROR") return;

        nexus.getGrid("AUT101EditarEjecuciones_grid_1").refresh();

        if (!showDetails)
            nexus.getForm("AUT101EditarEjecuciones_form_1").clickButton("reprocesarInterfaz");
    };

    //------------------------------------------

    //-------------- GRID DETAILS --------------

    const onBeforeInitializeDetailsGrid = (context, data, nexus) => {
        if (!showDetails) {
            context.abortServerCall = true;
        }
        else {
            data.parameters.push({ id: "NU_AUT_EJECUCION", value: props.automatismoEjecucion });
            data.parameters.push({ id: "SHOW_DETAILS_GRID", value: "S" });
        }
    };

    const addDetailsParameters = (context, data, nexus) => {
        data.parameters = [{ id: "NU_AUT_EJECUCION", value: automatismoEjecucion },
        { id: "SHOW_DETAILS_GRID", value: "S" }];
    };

    const onAfterDetailsCommit = (context, rows, data, nexus) => {

        if (context.status === "ERROR") return;

        nexus.getGrid("AUT101EditarEjecuciones_grid_2").refresh();

        if (showDetails)
            nexus.getForm("AUT101EditarEjecuciones_form_1").clickButton("reprocesarInterfaz");
    };


    const onBeforeValidateRow = (context, data, nexus) => {
        context.abortServerCall = true;
    };

    //------------------------------------------

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-70w" backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title> {t("AUT101ModalEditar_Sec0_modalTitle_Titulo")} </Modal.Title>
            </Modal.Header>
            <Form
                id="AUT101EditarEjecuciones_form_1"
                application="AUT101EditarEjecuciones"
                onBeforeButtonAction={onBeforeButtonAction}
            >
                <Modal.Body>
                    <Grid
                        application="AUT101EditarEjecuciones"
                        id="AUT101EditarEjecuciones_grid_1"
                        onBeforeInitialize={onBeforeInitialize}
                        onAfterInitialize={onAfterInitialize}
                        onBeforeFetch={addParameters}
                        onBeforeFetchStats={addParameters}
                        onBeforeExportExcel={addParameters}
                        onBeforeApplyFilter={addParameters}
                        onBeforeApplySort={addParameters}
                        onBeforeCommit={addParameters}
                        onAfterCommit={onAfterCommit}
                        onBeforeValidateRow={onBeforeValidateRow}
                        rowsToFetch={4}
                        rowsToDisplay={4}
                        enableExcelExport
                    />

                    <div style={{ display: showDetails ? 'block' : 'none' }}>
                        <Grid
                            application="AUT101EditarEjecuciones"
                            id="AUT101EditarEjecuciones_grid_2"
                            onBeforeInitialize={onBeforeInitializeDetailsGrid}
                            onBeforeFetch={addDetailsParameters}
                            onBeforeFetchStats={addDetailsParameters}
                            onBeforeExportExcel={addDetailsParameters}
                            onBeforeApplyFilter={addDetailsParameters}
                            onBeforeApplySort={addDetailsParameters}
                            onBeforeCommit={addDetailsParameters}
                            onAfterCommit={onAfterDetailsCommit}
                            onBeforeValidateRow={onBeforeValidateRow}
                            rowsToFetch={20}
                            rowsToDisplay={7}
                            enableExcelExport
                        />
                    </div>
                </Modal.Body>

                <Modal.Footer>
                    <FormButton id="showFormButton" label="General_Sec0_btn_GuardarReprocesar" variant="success" className="mb-4" />
                    <FormButton id="closeFormButton" label="General_Sec0_btn_Cerrar" variant="outline-secondary" className="mb-4" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}