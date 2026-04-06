import React, { useState, useRef } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FieldDateTime } from '../../components/FormComponents/Form';
import { AddRemovePanel } from '../../components/AddRemovePanel';


function InternalSEG030RecursosModal(props) {

    const { t } = useTranslation();

    const addParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "idUsuario", value: props.usuario.find(x => x.id === "idUsuario").value }

        ];
    }

    const [infoUsuario, setInfoUsuario] = useState("");

    const onAfterInitialize = (context, grid, parameters, nexus) => {
        if (parameters.find(x => x.id === "SEG030_USUARIO") != null) {
            setInfoUsuario(parameters.find(x => x.id === "SEG030_USUARIO").value);
        }
    }

    const handleGridsAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("SEG030Recursos_grid_1").refresh();
        nexus.getGrid("SEG030Recursos_grid_2").refresh();
    }

    

    const handleAdd = (evt, nexus) => {
        nexus.getGrid("SEG030Recursos_grid_1").triggerMenuAction("btnAsociar", false, evt.ctrlKey);
    };

    const handleRemove = (evt, nexus) => {
        nexus.getGrid("SEG030Recursos_grid_2").triggerMenuAction("btnDesasociar", false, evt.ctrlKey);
    };

    const handleClose = () => {

        props.onHide(null, null, props.nexus);
    };



    return (
        <div>
            <Modal.Header closeButton>
                <Modal.Title>{t("SEG030_Sec0_mdlAsigna_Titulo")} - {`${infoUsuario}`}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Page
                    {...props}
                    application="SEG030Recursos"
                    
                >
                    <Container fluid>
                        <AddRemovePanel
                            onAdd={handleAdd}
                            onRemove={handleRemove}
                            from={(
                                <Grid
                                    application="SEG030Recursos"
                                    id="SEG030Recursos_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                                    enableSelection
                                    onAfterInitialize={onAfterInitialize}
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
                                    application="SEG030Recursos"
                                    id="SEG030Recursos_grid_2" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                                    enableSelection
                                    onBeforeInitialize={addParameters}
                                    onAfterInitialize={onAfterInitialize}
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
                <Button variant="btn btn-outline-info" onClick={handleClose}> {t("SEG030_frm1_btn_cerrar")} </Button>
            </Modal.Footer>
        </div>
    );
}

export const SEG030RecursosModal = withPageContext(InternalSEG030RecursosModal);