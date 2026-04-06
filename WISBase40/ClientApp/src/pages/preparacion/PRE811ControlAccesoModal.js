import React, { useState, useRef, useEffect } from 'react';
import { Modal, Button, Row, Col, Tab, Tabs } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { Form, FieldToggle, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import * as Yup from 'yup';
import { useCustomTranslation } from '../../components/TranslationHook';
import { withPageContext } from '../../components/WithPageContext';
import { Page } from '../../components/Page';
import { AddRemovePanel } from '../../components/AddRemovePanel';


function InternalPRE811ControlAccesoModal(props) {

    const { t } = useCustomTranslation();

    const validationSchema = {
        habilitarPedCompleto: Yup.string(),
    };

    const handleBeforeLoad = (data) => {
        data.parameters = [...data.parameters, { id: "keyPreferencia", value: props.preferencia }];
    }

    const applyParameters = (context, data, nexus) => {
        data.parameters = [{ id: "keyPreferencia", value: props.preferencia }];
    };

    const applyFormParameters = (context, form, query, nexus) => {
        query.parameters = [
            { id: "keyPreferencia", value: props.preferencia },
        ];
    }

    const handleAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("AgregarControlAcceso_grid_1").refresh();
        nexus.getGrid("QuitarControlAcceso_grid_2").refresh();
    }

    const handleAdd = (evt, nexus) => {
        nexus.getGrid("AgregarControlAcceso_grid_1").triggerMenuAction("btnAgregar", false, evt.ctrlKey);
    }

    const handleRemove = (evt, nexus) => {
        nexus.getGrid("QuitarControlAcceso_grid_2").triggerMenuAction("btnQuitar", false, evt.ctrlKey);
    }

    const handleClose = () => {
        props.onHide();
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title>{t("PRE811CoAc_Sec0_modal_title")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Page
                    {...props}
                    onBeforeLoad={handleBeforeLoad}
                    application="PRE811PreferenciaCtrlAcceso"
                >
                    <Form
                        id="PRE811CoAc_form_1"
                        application="PRE811PreferenciaCtrlAcceso"
                        validationSchema={validationSchema}
                        onBeforeInitialize={applyFormParameters}
                        onBeforeSubmit={applyFormParameters}
                    >
                        <Row>
                            <div class="w-50 ml-3">
                                <h4>{t("PRE811_frm1_lbl_Preferencia")}: {props.preferencia}</h4>
                            </div>
                            <div class="ml-5 mt-2">
                                <label htmlFor="habilitarPedCompleto">{t("PRE811CoAc_frm1_lbl_FlagPedCompleto")}</label>
                            </div>
                            <div class="ml-3 mt-2">
                                <FieldToggle name="habilitarPedCompleto" />
                                <StatusMessage for="habilitarPedCompleto" />
                            </div>
                            <div class="ml-3">
                                <SubmitButton id="btnGuardar" variant="primary" label="PRE811CoAc_frm1_btn_GUARDAR" />
                            </div>
                        </Row>
                    </Form>
                    <hr />
                    <AddRemovePanel
                        onAdd={handleAdd}
                        onRemove={handleRemove}
                        from={(
                            <Grid
                                application="PRE811PreferenciaCtrlAcceso"
                                id="AgregarControlAcceso_grid_1"
                                rowsToFetch={30}
                                rowsToDisplay={15}
                                onBeforeInitialize={applyParameters}
                                onBeforeFetch={applyParameters}
                                onBeforeApplyFilter={applyParameters}
                                onBeforeApplySort={applyParameters}
                                onBeforeMenuItemAction={applyParameters}
                                onAfterMenuItemAction={handleAfterMenuItemAction}
                                onBeforeExportExcel={applyParameters}
                                onBeforeFetchStats={applyParameters}
                                enableExcelExport
                                enableSelection
                            />
                        )}
                        to={(
                            <Grid
                                application="PRE811PreferenciaCtrlAcceso"
                                id="QuitarControlAcceso_grid_2"
                                rowsToFetch={30}
                                rowsToDisplay={15}
                                onBeforeInitialize={applyParameters}
                                onBeforeFetch={applyParameters}
                                onBeforeApplyFilter={applyParameters}
                                onBeforeApplySort={applyParameters}
                                onBeforeMenuItemAction={applyParameters}
                                onAfterMenuItemAction={handleAfterMenuItemAction}
                                onBeforeExportExcel={applyParameters}
                                onBeforeFetchStats={applyParameters}
                                enableExcelExport
                                enableSelection
                            />
                        )}
                    />
                </Page>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}>
                    {t("PRE811_frm1_btn_cerrar")}
                </Button>
            </Modal.Footer>
        </Modal>
    );
}

export const PRE811ControlAccesoModal = withPageContext(InternalPRE811ControlAccesoModal);