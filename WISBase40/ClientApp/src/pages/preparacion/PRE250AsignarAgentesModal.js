import React, { useState, useRef } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FieldDateTime } from '../../components/FormComponents/Form';
import { AddRemovePanel } from '../../components/AddRemovePanel';


function InternalPRE250AsignarAgentesModal(props) {
    const { t } = useTranslation();    
    const [infoRegla, setInfoRegla] = useState("");

    const addParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "nuRegla", value: props.regla.find(x => x.id === "nuRegla").value },
            { id: "empresa", value: props.regla.find(x => x.id === "empresa").value },
            { id: "tpAgente", value: props.regla.find(x => x.id === "tpAgente").value }
        ];
    }
    const onAfterInitialize = (context, grid, parameters, nexus) => {
        if (parameters.find(x => x.id === "nuRegla") != null) {
            setInfoRegla(parameters.find(x => x.id === "nuRegla").value);
        }
    }

    const onAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("PRE250AsignarAgentes_grid_1").refresh();
        nexus.getGrid("PRE250AsignarAgentes_grid_2").refresh();
    }

    const handleAdd = (evt, nexus) => {
        nexus.getGrid("PRE250AsignarAgentes_grid_1").triggerMenuAction("btnAgregarAgente", false, evt.ctrlKey);
    };

    const handleRemove = (evt, nexus) => {
        nexus.getGrid("PRE250AsignarAgentes_grid_2").triggerMenuAction("btnQuitarAgente", false, evt.ctrlKey);
    };

    const handleClose = () => {

        props.onHide(null, null, props.nexus);
    };

    return (
        <div>
            <Modal.Header closeButton>
                <Modal.Title>{t("PRE250_Sec0_title_AsignarAgentes")} {`${infoRegla}`}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Page
                    {...props}
                    application="PRE250AsignarAgentes"

                >
                    <AddRemovePanel
                        onAdd={handleAdd}
                        onRemove={handleRemove}
                        from={(
                            <Grid
                                application="PRE250AsignarAgentes"
                                id="PRE250AsignarAgentes_grid_1"
                                rowsToFetch={30}
                                rowsToDisplay={15}
                                enableSelection
                                enableExcelExport
                                onAfterInitialize={onAfterInitialize}
                                onBeforeInitialize={addParameters}
                                onBeforeFetch={addParameters}
                                onBeforeApplyFilter={addParameters}
                                onBeforeApplySort={addParameters}
                                onBeforeExportExcel={addParameters}
                                onBeforeMenuItemAction={addParameters}
                                onAfterMenuItemAction={onAfterMenuItemAction}
                                onBeforeFetchStats={addParameters}
                            />
                        )}
                        to={(
                            <Grid
                                application="PRE250AsignarAgentes"
                                id="PRE250AsignarAgentes_grid_2"
                                rowsToFetch={30}
                                rowsToDisplay={15}
                                enableSelection
                                enableExcelExport
                                onBeforeInitialize={addParameters}
                                onBeforeFetch={addParameters}
                                onBeforeApplyFilter={addParameters}
                                onBeforeApplySort={addParameters}
                                onBeforeMenuItemAction={addParameters}
                                onAfterMenuItemAction={onAfterMenuItemAction}
                                onBeforeCommit={addParameters}
                                onBeforeFetchStats={addParameters}
                                onBeforeExportExcel={addParameters}
                            />
                        )}
                    />
                </Page>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-info" onClick={handleClose}> {t("SEG030_frm1_btn_cerrar")} </Button>
            </Modal.Footer>
        </div>
    );
}
export const PRE250AsignarAgentesModal = withPageContext(InternalPRE250AsignarAgentesModal);