import React, { useLayoutEffect, useState } from 'react';
import { Button, Col, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { FieldToggle, Form, SubmitButton } from '../../components/FormComponents/Form';
import { FieldSelect } from '../../components/FormComponents/FormSelect';
import { StatusMessage } from '../../components/FormComponents/FormStatusMessage';
import { Grid } from '../../components/GridComponents/Grid';
import { PRE052CreatePrepAdministrativoModal } from '../preparacion/PRE052CreatePrepAdministrativoModal';

export function STO820AsignarPreparacionModal(props) {
    const { t } = useTranslation();

    const lastNuPreparacionRef = React.useRef(null);
    const lastConfPedidoDestRef = React.useRef(null);
    const empresaOrigen = React.useRef(null);
    const [showGrid, setShowGrid] = useState(false);
    const [showBtnSeleccion, setShowBtnSeleccion] = useState(false);
    const buttonId = React.useRef("");
    const [showCrearPreparacionPopup, setShowCrearPreparacionPopup] = useState(false);
    const stockSuelto = React.useRef(false);
    const stockLPN = React.useRef(false);
    const [_nexus, setNexus] = useState(null);
    const estadoEdicion = "ESTRASP_EN_EDICION";
    const estadoProceso = "ESTRASP_EN_PROCESO";
    const traspasoPda = "TPTRASPASO_PDA";
    const traspasoPreparacionOrigen = "TPTRASPASO_ADM_PREP_ORG";
    const traspasoPreparacionPendiente = "TPTRASPASO_ADM_PREP_LIB";
    const tipoExpediciontraspasoNoExpedible = "TRSENE";
    const tipoPedido = "TRASP";

    const initialValues = {
        nuPreparacion: "",
        confPedidoDestino: "",
    };

    const validationSchema = {
        nuPreparacion: Yup.string().required(),
        confPedidoDestino: Yup.string(),
    };

    useLayoutEffect(() => {
        if (props.show) {
            resetState();
        }
    }, [props.show]);

    const resetState = () => {
        lastNuPreparacionRef.current = null;
        lastConfPedidoDestRef.current = null;
        empresaOrigen.current = null;
        setShowGrid(false);
        setShowBtnSeleccion(false);
        buttonId.current = "";
        stockSuelto.current = false;
        stockLPN.current = false;
    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        setNexus(nexus);
        query.parameters = [{ id: "idTraspaso", value: props.traspaso.Id }];
    };

    const handleFormAfterInitialize = (context, form, query, nexus) => {
        stockSuelto.current = false;
        stockLPN.current = false;
        setShowGrid(query.parameters.find(d => d.id === "showGrid").value === "true");
        setShowBtnSeleccion(query.parameters.find(d => d.id === "showBtnSeleccion").value === "true");
        empresaOrigen.current = query.parameters.find(d => d.id === "empresaOrigen").value;
    };

    const onBeforeButtonAction = (context, form, query, nexus) => {
        query.parameters = [
            { id: "idTraspaso", value: props.traspaso.Id },
            { id: "nuPreparacion", value: lastNuPreparacionRef.current }
        ];
    };

    const onAfterButtonAction = (context, form, query, nexus) => {
        _nexus.getGrid("STO820AsignarPreparacion_grid_1").reset();
    };

    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        query.parameters = [
            { id: "idTraspaso", value: props.traspaso.Id },
            { id: "nuPreparacion", value: lastNuPreparacionRef.current }
        ];

        buttonId.current = query.buttonId;

        nexus.getGrid("STO820AsignarPreparacion_grid_1").commit(true, true);
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        context.abortHideLoading = true;
    };

    const handleClose = () => {
        resetState();
        props.onHide(null, null, props.nexus);
    };

    const onBeforeValidateField = (context, form, query, nexus) => {
        query.parameters = [{ id: "idTraspaso", value: props.traspaso.Id }];
    }

    const onAfterValidateField = (context, form, query, nexus) => {
        var form = nexus.getForm("STO820_form_AsignarPreparacion");
        var nuPreparacion = form.getFieldValue("nuPreparacion");
        var confPedidoDestino = form.getFieldValue("confPedidoDestino");

        if (nuPreparacion && nuPreparacion !== lastNuPreparacionRef.current) {
            lastNuPreparacionRef.current = nuPreparacion;
            query.parameters = [{ id: "nuPreparacion", value: nuPreparacion }];
            nexus.getGrid("STO820AsignarPreparacion_grid_1").reset();
        }
        else if (confPedidoDestino && confPedidoDestino !== lastConfPedidoDestRef.current) {
            lastConfPedidoDestRef.current = confPedidoDestino;

            query.parameters = [{ id: "confPedidoDestino", value: confPedidoDestino }];
            nexus.getGrid("STO820AsignarPreparacion_grid_1").reset();
        }
    }

    const onBeforeCommit = (context, data, nexus) => {
        const nuPreparacion = lastNuPreparacionRef.current;
        const confPedidoDestino = lastConfPedidoDestRef.current;

        data.parameters.push({ id: "idTraspaso", value: props.traspaso.Id });
        data.parameters.push({ id: "nuPreparacion", value: nuPreparacion });
        data.parameters.push({ id: "confPedidoDestino", value: confPedidoDestino });
        data.parameters.push({ id: "buttonId", value: buttonId.current });
    }

    const onAfterCommit = (context, rows, data, nexus) => {
        if (context.status === "OK") {
            nexus.getGrid("STO820_grid_1").refresh();
            props.onHide();
        }
        var form = _nexus.getForm("STO820_form_AsignarPreparacion");
        form.hideLoadingOverlay();
    };

    const applyParameters = (context, data, nexus) => {
        var form = nexus.getForm("STO820_form_AsignarPreparacion");
        var confPedidoDestino = form.getFieldValue("confPedidoDestino");

        data.parameters = [
            { id: "idTraspaso", value: props.traspaso.Id },
            { id: "nuPreparacion", value: lastNuPreparacionRef.current },
            { id: "confPedidoDestino", value: confPedidoDestino },
            { id: "buttonId", value: buttonId.current }
        ];
    }

    const handleClickCrearPorSeleccion = () => {
        if (!stockSuelto.current && !stockLPN.current) {
            _nexus.toastException({ name: "", message: "STO820AsignarPreparacion_Sec0_Error_DebeMarcarOpcion" });
            return;
        }

        setShowCrearPreparacionPopup(true);
    }

    const closeCrearPreparacionDialog = (preparacion) => {
        setShowCrearPreparacionPopup(false);
        _nexus.getGrid("STO820AsignarPreparacion_grid_1").refresh();

        if (preparacion !== "" && preparacion !== null) {
            lastNuPreparacionRef.current = preparacion;

            const form = _nexus.getForm("STO820_form_AsignarPreparacion");
            form.clickButton("btnAsignarPreparacion");
        }
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-70w" backdrop="static">
            <div style={{ display: !showCrearPreparacionPopup ? "block" : "none" }}>
                <Form
                    id="STO820_form_AsignarPreparacion"
                    application="STO820AsignarPreparacion"
                    initialValues={initialValues}
                    validationSchema={validationSchema}
                    onBeforeInitialize={handleFormBeforeInitialize}
                    onAfterInitialize={handleFormAfterInitialize}
                    onBeforeSubmit={handleFormBeforeSubmit}
                    onAfterSubmit={handleFormAfterSubmit}
                    onBeforeValidateField={onBeforeValidateField}
                    onAfterValidateField={onAfterValidateField}
                    onBeforeButtonAction={onBeforeButtonAction}
                    onAfterButtonAction={onAfterButtonAction}
                >
                    <Modal.Header closeButton>
                        <Modal.Title>{(props?.traspaso?.Estado === estadoEdicion) ? t("STO820_Sec0_mdlTitle_AsigPreparacionTitulo") : t("STO820_Sec0_mdlTitle_DetallePreparacionTitulo")}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <Row>
                            <Col>
                                <h4>{t("STO820_frm1_lbl_Traspaso")}: {props?.traspaso?.Id}</h4>
                            </Col>
                        </Row>
                        <hr />
                        <Row>
                            <Col md={6}>
                                <div className="form-group">
                                    <label htmlFor="nuPreparacion">{t("STO820_frm2_lbl_nuPreparacion")}</label>
                                    <FieldSelect name="nuPreparacion" />
                                    <StatusMessage for="nuPreparacion" />
                                </div>
                            </Col>
                            {showBtnSeleccion ? (
                                <>
                                    <Col md={2}>
                                        <br />
                                        <FieldToggle
                                            name={"stockSuelto"}
                                            label={t("STO820_frm2_lbl_stockSuelto")}
                                            onChange={val => stockSuelto.current = val}
                                        />
                                    </Col>
                                    <Col md={2}>
                                        <br />
                                        <FieldToggle
                                            name={"stockLPN"}
                                            label={t("STO820_frm2_lbl_stockLPN")}
                                            onChange={val => stockLPN.current = val}
                                        />
                                    </Col>
                                    <Col md={2}>
                                        <div className="form-group">
                                            <br />
                                            <Button variant="primary" onClick={handleClickCrearPorSeleccion}> {t("STO820_frm2_btn_CrearPorSeleccion")} </Button>
                                        </div>
                                    </Col>
                                </>
                            ) : null}
                        </Row>
                        <div style={{ display: showGrid ? "block" : "none" }}>
                            <Row>
                                <Col md={6}>
                                    <div className="form-group">
                                        <label htmlFor="confPedidoDestino">{t("STO820_frm2_lbl_confPedidoDestino")}</label>
                                        <FieldSelect name="confPedidoDestino" />
                                        <StatusMessage for="confPedidoDestino" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Grid
                                    id="STO820AsignarPreparacion_grid_1"
                                    application="STO820AsignarPreparacion"
                                    rowsToFetch={10}
                                    rowsToDisplay={10}
                                    enableExcelExport
                                    onBeforeInitialize={applyParameters}
                                    onBeforeFetch={applyParameters}
                                    onBeforeFetchStats={applyParameters}
                                    onBeforeExportExcel={applyParameters}
                                    onBeforeApplyFilter={applyParameters}
                                    onBeforeApplySort={applyParameters}
                                    onBeforeCommit={onBeforeCommit}
                                    onAfterCommit={onAfterCommit}
                                />
                            </Row>
                        </div>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("STO820_frm2_btn_Cerrar")} </Button>
                        {(props?.traspaso?.Estado === estadoProceso &&
                            (props?.traspaso?.TipoTraspaso === traspasoPreparacionOrigen
                                || props?.traspaso?.TipoTraspaso === traspasoPreparacionPendiente))
                            ?
                            (<SubmitButton id="btnSubmitGuardar" variant="primary" label="STO820_frm2_btn_Guardar" />)
                            : null
                        }

                        {props?.traspaso?.Estado === estadoEdicion ? (
                            <SubmitButton
                                id="btnSubmitAsignarPreparacion"
                                variant="primary"
                                label="STO820_frm2_btn_Confirmar"
                            />
                        ) : props?.traspaso?.Estado === estadoProceso ? (
                            <SubmitButton
                                id="btnSubmitEjecutarTraspaso"
                                variant="primary"
                                label={
                                    props?.traspaso?.TipoTraspaso === traspasoPda
                                        ? "STO820_frm2_btn_Finalizar"
                                        : "STO820_frm2_btn_Ejecutar"
                                }
                            />
                        ) : null}

                    </Modal.Footer>
                </Form>
            </div>
            <PRE052CreatePrepAdministrativoModal
                show={showCrearPreparacionPopup}
                onHide={closeCrearPreparacionDialog}
                isHabilitadoTabSuelto={stockSuelto.current}
                isHabilitadoTabLpn={stockLPN.current}
                empresa={empresaOrigen.current}
                tpExpedicion={tipoExpediciontraspasoNoExpedible}
                tpPedido={tipoPedido}
            />
        </Modal>
    );
}
