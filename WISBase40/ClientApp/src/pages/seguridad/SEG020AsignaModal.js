import React, { useState, useRef } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FieldDateTime } from '../../components/FormComponents/Form';
import { AddRemovePanel } from '../../components/AddRemovePanel';


function InternalSEG020AsignacionModal(props) {

    const { t } = useTranslation();

    const handleBeforeButtonAction = (context, data, nexus) => {

        if (data.buttonId === "btnEditar") {
            context.abortServerCall = true;
        }
    };

    const addParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "idPerfil", value: props.usuario.find(x => x.id === "idPerfil").value }

        ];
    }

    const handleGridsAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("SEG020Asignar_grid_1").refresh();
        nexus.getGrid("SEG020Asignar_grid_2").refresh();
    }

    const handleAdd = (evt, nexus) => {
        nexus.getGrid("SEG020Asignar_grid_1").triggerMenuAction("btnAsociar", false, evt.ctrlKey);
    };

    const handleRemove = (evt, nexus) => {
        nexus.getGrid("SEG020Asignar_grid_2").triggerMenuAction("btnDesasociar", false, evt.ctrlKey);
    };

    const handleClose = (id) => {
        if (id.currentTarget.id == "btnAtras") {
            props.onHide(null, id.currentTarget.id, props.nexus);
        } else {

            props.onHide(null, null, props.nexus);
        }
    };

    return (
        <div>
            <Modal.Header closeButton>
                <Modal.Title>{t("SEG020_Sec0_mdlAsigna_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Page
                    {...props}
                    application="SEG020Asignar"

                >
                    <Container fluid>
                        <AddRemovePanel
                            onAdd={handleAdd}
                            onRemove={handleRemove}
                            from={(
                                <Grid
                                    application="SEG020Asignar"
                                    id="SEG020Asignar_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                                    enableSelection
                                    onBeforeInitialize={addParameters}
                                    onBeforeFetch={addParameters}
                                    onBeforeApplyFilter={addParameters}
                                    onBeforeApplySort={addParameters}
                                    onBeforeExportExcel={addParameters}
                                    onBeforeMenuItemAction={addParameters}
                                    onAfterMenuItemAction={handleGridsAfterMenuItemAction}
                                    onBeforeFetchStats={addParameters}
                                />
                            )}
                            to={(
                                <Grid
                                    application="SEG020Asignar"
                                    id="SEG020Asignar_grid_2" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                                    enableSelection
                                    onBeforeInitialize={addParameters}
                                    onBeforeFetch={addParameters}
                                    onBeforeApplyFilter={addParameters}
                                    onBeforeApplySort={addParameters}
                                    onBeforeExportExcel={addParameters}
                                    onBeforeMenuItemAction={addParameters}
                                    onAfterMenuItemAction={handleGridsAfterMenuItemAction}
                                    onBeforeFetchStats={addParameters}
                                />
                            )}
                        />
                    </Container>
                </Page>
            </Modal.Body>
            <Modal.Footer>
                <Button id="btnAtras" variant="btn btn-outline-secondary" onClick={handleClose.bind(this)}> {t("SEG020_frm1_btn_atras")} </Button>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("SEG020_frm1_btn_cerrar")} </Button>
            </Modal.Footer>
        </div>
    );
}

export const SEG020AsignaModal = withPageContext(InternalSEG020AsignacionModal);