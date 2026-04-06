import React from 'react';
import { Button, Col, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { AddRemovePanel } from '../../components/AddRemovePanel';
import { Field, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { Grid } from '../../components/GridComponents/Grid';

export function EVT030ModificarGrupoModal(props) {

    const { t } = useTranslation("translation", { useSuspense: false });

    const validationSchema = {
        NU_CONTACTO_GRUPO: Yup.string(),
        NM_GRUPO: Yup.string().required().max(100),
    };

    const applyParameters = (context, data, nexus) => {
        data.parameters = [
            ...data.parameters,
            { id: "grupo", value: props.nuGrupo },
            { id: "nmGrupo", value: props.nmGrupo }
        ];
    };

    const handleAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("EVT030_grid_Contactos").refresh();
        nexus.getGrid("EVT030_grid_ContactosSel").refresh();
    }

    const handleAdd = (evt, nexus) => {
        nexus.getGrid("EVT030_grid_Contactos").triggerMenuAction("btnAgregar", false, evt.ctrlKey);
    };

    const handleRemove = (evt, nexus) => {
        nexus.getGrid("EVT030_grid_ContactosSel").triggerMenuAction("btnQuitar", false, evt.ctrlKey);
    };

    const handleClose = () => {
        props.onHide();
    }

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = [
            { id: "grupo", value: props.nuGrupo },
            { id: "nmGrupo", value: props.nmGrupo }
        ];
    }

    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        query.parameters = [
            { id: "grupo", value: props.nuGrupo },
            { id: "nmGrupo", value: props.nmGrupo },
            { id: "isSubmit", value: true }
        ];
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK" && query.buttonId === "btnSubmitModificarGrupo") {
            nexus.getGrid("EVT030_grid_Grupos").refresh();
            props.onHide();
        }
    };

    const handleAfterValidateField = (context, form, query, nexus) => {
        query.parameters = [
            { id: "grupo", value: props.nuGrupo },
            { id: "nmGrupo", value: props.nmGrupo }
        ];
    };

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
            <Form
                id="EVT030_form_1"
                application="EVT030ModificarGrupo"
                validationSchema={validationSchema}
                onBeforeInitialize={handleFormBeforeInitialize}
                onAfterValidateField={handleAfterValidateField}
                onBeforeSubmit={handleFormBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("EVT030ModificarGrupoModal_Sec0_modalTitle_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>

                    <Row>
                        <Col lg={1}>
                            <label htmlFor="NU_CONTACTO_GRUPO">{t("EVT030_gridGrupos_colname_NU_CONTACTO_GRUPO")}</label>
                            <Field name="NU_CONTACTO_GRUPO" readOnly />
                            <StatusMessage for="NU_CONTACTO_GRUPO" />
                        </Col>
                        <Col lg={4}>
                            <label htmlFor="NM_GRUPO">{t("EVT030_gridGrupos_colname_NM_GRUPO")}</label>
                            <Field name="NM_GRUPO" />
                            <StatusMessage for="NM_GRUPO" />
                        </Col>
                    </Row>

                    <hr />
                    <AddRemovePanel
                        onAdd={handleAdd}
                        onRemove={handleRemove}
                        from={(
                            <Grid
                                application="EVT030ModificarGrupo"
                                id="EVT030_grid_Contactos"
                                rowsToFetch={30}
                                rowsToDisplay={15}
                                onBeforeInitialize={applyParameters}
                                onBeforeFetch={applyParameters}
                                onBeforeFetchStats={applyParameters}
                                onBeforeApplyFilter={applyParameters}
                                onBeforeApplySort={applyParameters}
                                onBeforeMenuItemAction={applyParameters}
                                onAfterMenuItemAction={handleAfterMenuItemAction}
                                onBeforeExportExcel={applyParameters}
                                enableExcelExport
                                enableSelection
                            />
                        )}
                        to={(
                            <Grid
                                application="EVT030ModificarGrupo"
                                id="EVT030_grid_ContactosSel"
                                rowsToFetch={30}
                                rowsToDisplay={15}
                                onBeforeInitialize={applyParameters}
                                onBeforeFetch={applyParameters}
                                onBeforeFetchStats={applyParameters}
                                onBeforeApplyFilter={applyParameters}
                                onBeforeApplySort={applyParameters}
                                onBeforeMenuItemAction={applyParameters}
                                onAfterMenuItemAction={handleAfterMenuItemAction}
                                onBeforeExportExcel={applyParameters}
                                enableExcelExport
                                enableSelection
                            />
                        )}
                    />

                </Modal.Body>
                <Modal.Footer>

                    <Button onClick={handleClose} id="btnCerrar" variant="outline-secondary">{t("EXP040_frm1_btn_cerrar")}</Button>
                    <SubmitButton id="btnSubmitModificarGrupo" variant="primary" label="EVT030_frm1_btn_editar" />

                </Modal.Footer>
            </Form>
        </Modal>
    );
}