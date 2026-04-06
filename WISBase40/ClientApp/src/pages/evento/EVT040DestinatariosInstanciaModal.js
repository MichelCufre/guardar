import React, { useState, useEffect } from 'react';
import { Button, Col, Modal, Row, Tab, Tabs } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, FieldSelect, FieldSelectAsync, FieldToggle, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { Grid } from '../../components/GridComponents/Grid';
import { AddRemovePanel } from '../../components/AddRemovePanel';

import './EVT040Advertencia.css';

export function EVT040DestinatariosInstanciaModal(props) {
    const { t } = useTranslation();

    const [nexus, setNexus] = useState(null);
    const [keyTab, setKeyTab] = useState(null);
    const [instancia, setInstancia] = useState(null);

    const [manejaEmpresa, setManejaEmpresa] = useState("hidden");
    const [manejaTipoAgente, setManejaTipoAgente] = useState("hidden");
    const [manejaCodigoAgente, setManejaCodigoAgente] = useState("hidden");
    const [advertencia, setAdvertencia] = useState("hidden");
    const [mensaje, setMensaje] = useState("");

    const initialValues = {
        evento: "",
        instancia: "",
        descripcion: "",
    };

    const handleClose = () => {
        setNexus(null);
        setManejaEmpresa("hidden");
        setManejaTipoAgente("hidden");
        setManejaCodigoAgente("hidden");
        setAdvertencia("hidden");

        props.onHide();
    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {

        let parameters = [
            { id: "instancia", value: props.instancia }
        ];

        query.parameters = parameters;
    }

    const onAfterInitialize = (context, form, query, nexus) => {
        const usaEmpresa = query.parameters.find(p => p.id === "manejaEmpresa").value;
        const usaTipoAgente = query.parameters.find(p => p.id === "manejaTipoAgente").value;
        const usaCodigoAgente = query.parameters.find(p => p.id === "manejaCodigoAgente").value;

        if (usaEmpresa === 'True') {
            setManejaEmpresa("");
        }
        if (usaTipoAgente === 'True') {
            setManejaTipoAgente("");
        }
        if (usaCodigoAgente === 'True') {
            setManejaCodigoAgente("");
        }

        setKeyTab("contactos");
        setNexus(nexus);
    }

    const handleAdd = (evt, nexus) => {
        nexus.getGrid("EVT040DestinatariosContactos_grid_1").triggerMenuAction("btnAgregar", false, evt.ctrlKey);
    };

    const handleRemove = (evt, nexus) => {
        nexus.getGrid("EVT040DestinatariosContactos_grid_2").triggerMenuAction("btnQuitar", false, evt.ctrlKey);
    };

    const handleAddGrupo = (evt, nexus) => {
        nexus.getGrid("EVT040DestinatariosGrupos_grid_1").triggerMenuAction("btnAgregar", false, evt.ctrlKey);
    };

    const handleRemoveGrupo = (evt, nexus) => {
        nexus.getGrid("EVT040DestinatariosGrupos_grid_2").triggerMenuAction("btnQuitar", false, evt.ctrlKey);
    };


    const addParameters = (context, data, nexus) => {
        data.parameters.push({ id: "instancia", value: props.instancia });
    }

    const onAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("EVT040DestinatariosContactos_grid_1").refresh();
        nexus.getGrid("EVT040DestinatariosContactos_grid_2").refresh();
        nexus.getGrid("EVT040DestinatariosGrupos_grid_1").refresh();
        nexus.getGrid("EVT040DestinatariosGrupos_grid_2").refresh();
    }

    const onAfterFetch = (context, newRows, parameters, nexus) => {
        if (parameters && parameters.find(f => f.id === "mensaje") !== undefined) {
            setMensaje(parameters.find(f => f.id === "mensaje").value);
            setAdvertencia("");
        }
    };

    const GridOnAfterInitialize = (context, grid, parameters, nexus) => {
        if (parameters && parameters.find(f => f.id === "mensaje") !== undefined) {
            setMensaje(parameters.find(f => f.id === "mensaje").value);
            setAdvertencia("");
        }
    };

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">

            <Modal.Header closeButton>
                <Modal.Title>{t("EVT040_Sec0_modalTitle_Destinatarios")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form
                    application="EVT040DestinatariosInstancia"
                    initialValues={initialValues}
                    onBeforeInitialize={handleFormBeforeInitialize}
                    onAfterInitialize={onAfterInitialize}
                >
                    <Row>
                        <Col lg={4}>
                            <div className="form-group">
                                <label htmlFor="instancia">{t("EVT040_frm_lbl_Instancia")}</label>
                                <Field name="instancia" readOnly />
                            </div>
                        </Col>
                        <Col lg={4}>
                            <div className="form-group">
                                <label htmlFor="descripcion">{t("EVT040_frm_lbl_Descripcion")}</label>
                                <Field name="descripcion" readOnly />
                                <StatusMessage for="descripcion" />
                            </div>
                        </Col>
                        <Col lg={4}>
                            <div className="form-group">
                                <label htmlFor="evento">{t("EVT040_frm_lbl_Evento")}</label>
                                <Field name="evento" readOnly />
                                <StatusMessage for="evento" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col lg={4} className={manejaEmpresa !== "hidden" ? '' : 'd-none'}>
                            <div className="form-group">
                                <label htmlFor="empresa">{t("EVT040_frm_lbl_Empresa")}</label>
                                <Field name="empresa" readOnly />
                            </div>
                        </Col>
                        <Col lg={4} className={manejaCodigoAgente !== "hidden" ? '' : 'd-none'}>
                            <div className="form-group">
                                <label htmlFor="codigoAgente">{t("EVT040_frm_lbl_CodigoAgente")}</label>
                                <Field name="codigoAgente" readOnly />
                                <StatusMessage for="codigoAgente" />
                            </div>
                        </Col>
                        <Col lg={4} className={manejaTipoAgente !== "hidden" ? '' : 'd-none'}>
                            <div className="form-group">
                                <label htmlFor="tipoAgente">{t("EVT040_frm_lbl_TipoAgente")}</label>
                                <Field name="tipoAgente" readOnly />
                                <StatusMessage for="tipoAgente" />
                            </div>
                        </Col>
                    </Row>
                </Form>
                <br />
                <Tabs defaultActiveKey="contactos" transition={false} id="noanim-tab-example"
                    activeKey={keyTab}
                    onSelect={(k) => setKeyTab(k)}
                >
                    <Tab eventKey="contactos" title={t("EVT040_Nav1_lbl_tab_1")}>
                        <br />
                        <Row className={advertencia}>
                            <Col className="d-flex justify-content-center">
                                <div className="advertencia">{t(mensaje)}</div>
                            </Col>
                        </Row>
                        <br />
                        <AddRemovePanel
                            onAdd={handleAdd}
                            onRemove={handleRemove}
                            from={(
                                <Grid
                                    application="EVT040ContactosInstancia"
                                    id="EVT040DestinatariosContactos_grid_1"
                                    rowsToFetch={30}
                                    rowsToDisplay={15}
                                    enableExcelExport
                                    enableSelection
                                    onBeforeInitialize={addParameters}
                                    onBeforeFetch={addParameters}
                                    onBeforeFetchStats={addParameters}
                                    onBeforeApplyFilter={addParameters}
                                    onBeforeApplySort={addParameters}
                                    onBeforeExportExcel={addParameters}
                                    onBeforeMenuItemAction={addParameters}
                                    onAfterMenuItemAction={onAfterMenuItemAction}
                                />
                            )}
                            to={(
                                <Grid
                                    application="EVT040ContactosInstancia"
                                    id="EVT040DestinatariosContactos_grid_2"
                                    rowsToFetch={30}
                                    rowsToDisplay={15}
                                    enableExcelExport
                                    enableSelection
                                    onBeforeInitialize={addParameters}
                                    onBeforeFetch={addParameters}
                                    onAfterFetch={onAfterFetch}
                                    onBeforeFetchStats={addParameters}
                                    onBeforeApplyFilter={addParameters}
                                    onBeforeApplySort={addParameters}
                                    onBeforeExportExcel={addParameters}
                                    onBeforeMenuItemAction={addParameters}
                                    onAfterMenuItemAction={onAfterMenuItemAction}
                                    onAfterInitialize={GridOnAfterInitialize}
                                />
                            )}
                        />
                    </Tab>
                    <Tab eventKey="grupo" title={t("EVT040_Nav1_lbl_tab_2")}>
                        <br />
                        <AddRemovePanel
                            onAdd={handleAddGrupo}
                            onRemove={handleRemoveGrupo}
                            from={(
                                <Grid
                                    application="EVT040GrupoInstancia"
                                    id="EVT040DestinatariosGrupos_grid_1"
                                    rowsToFetch={30}
                                    rowsToDisplay={15}
                                    enableExcelExport
                                    enableSelection
                                    onBeforeInitialize={addParameters}
                                    onBeforeFetch={addParameters}
                                    onBeforeFetchStats={addParameters}
                                    onBeforeApplyFilter={addParameters}
                                    onBeforeApplySort={addParameters}
                                    onBeforeExportExcel={addParameters}
                                    onBeforeMenuItemAction={addParameters}
                                    onAfterMenuItemAction={onAfterMenuItemAction}
                                />
                            )}
                            to={(
                                <Grid
                                    application="EVT040GrupoInstancia"
                                    id="EVT040DestinatariosGrupos_grid_2"
                                    rowsToFetch={30}
                                    rowsToDisplay={15}
                                    enableExcelExport
                                    enableSelection
                                    onBeforeInitialize={addParameters}
                                    onBeforeFetch={addParameters}
                                    onBeforeFetchStats={addParameters}
                                    onBeforeApplyFilter={addParameters}
                                    onBeforeApplySort={addParameters}
                                    onBeforeExportExcel={addParameters}
                                    onBeforeMenuItemAction={addParameters}
                                    onAfterMenuItemAction={onAfterMenuItemAction}
                                />
                            )}
                        />
                    </Tab>

                </Tabs>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}>
                    {t("EVT040_frm_btn_Cerrar")}
                </Button>
            </Modal.Footer>

        </Modal>
    );
}
