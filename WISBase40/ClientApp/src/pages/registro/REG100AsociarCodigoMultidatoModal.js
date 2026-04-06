import React, { useState, useEffect } from 'react';
import { Modal, Button, Row, Col, Tab, Tabs } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { withPageContext } from '../../components/WithPageContext';
import { AddRemovePanel } from '../../components/AddRemovePanel';
import { REG100CodigosMultidatoModal } from './REG100CodigosMultidatoModal';
import { Form, Field, FieldNumber, StatusMessage, SubmitButton, FormButton } from '../../components/FormComponents/Form';

function InternalREG100AsociarCodigoMultidatoModal(props) {
    const { t } = useTranslation();

    const [showPopupDetallesCodigosMultidato, setShowPopupDetallesCodigosMultidato] = useState(false);
    const [codigoMultidato, setCodigoMultidato] = useState(null);

    const [codigosMultidatoConDetalles, setCodigosMultidatoConDetalles] = useState(null);
    const [codigosMultidatoDesasociar, setCodigosMultidatoDesasociar] = useState(null);
    const [detallesCodigoMultidato, setDetallesCodigoMultidato] = useState(null);
    const [_nexus, setNexus] = useState(null);

    useEffect(() => {
        if (codigosMultidatoConDetalles !== null) {
            setDetallesCodigoMultidato(codigosMultidatoConDetalles.split("$"));
        } else {
            setDetallesCodigoMultidato(null);
        }
    }, [codigosMultidatoConDetalles]);

    const addParameters = (context, data, nexus) => {
        setNexus(nexus);

        data.parameters = [{ id: "empresa", value: props.empresa }];
    }

    const onAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("REG100AsociarCodigoEmpresa_grid_1").refresh();
        nexus.getGrid("REG100AsociarCodigoEmpresa_grid_2").refresh();
    }

    const handleAdd = (evt, nexus) => {
        nexus.getGrid("REG100AsociarCodigoEmpresa_grid_1").triggerMenuAction("btnAgregar", false, evt.ctrlKey);
    };

    const handleRemove = (evt, nexus) => {
        nexus.getGrid("REG100AsociarCodigoEmpresa_grid_2").triggerMenuAction("btnQuitar", false, evt.ctrlKey);
    };

    const onBeforeMenuItemAction = (context, data, nexus) => {
        let keys = [];

        if (_nexus.parameters === undefined || !_nexus.parameters.some(x => x.id == "isMenuItemActionConfirm")) {
            if (detallesCodigoMultidato != null) {
                if (detallesCodigoMultidato.length > 0) {
                    if (data.selection.allSelected) {
                        context.abortServerCall = true;
                        nexus.showConfirmation({
                            message: "REG100_frm1_msg_ConfirmarDetalles",
                            acceptLabel: "General_Sec0_btn_CONFIRM_BOX_YES",
                            cancelLabel: "General_Sec0_btn_CONFIRM_BOX_NO",
                            onAccept: () => { onAcceptConfirmationDesasociar(nexus) },
                            onCancel: () => { }
                        });
                    } else if (data.selection.keys) {
                        keys = data.selection.keys.map(key => key.replace(/^1\$/, ''));

                        if (detallesCodigoMultidato.some(code => keys.includes(code))) {
                            context.abortServerCall = true;
                            if (keys.length > 1) {
                                nexus.showConfirmation({
                                    message: "REG100_frm1_msg_ConfirmarDetalles",
                                    acceptLabel: "General_Sec0_btn_CONFIRM_BOX_YES",
                                    cancelLabel: "General_Sec0_btn_CONFIRM_BOX_NO",
                                    onAccept: () => { onAcceptConfirmationDesasociar(nexus) },
                                    onCancel: () => { }
                                });
                            } else {
                                nexus.showConfirmation({
                                    message: "REG100_frm1_msg_ConfirmarDetalle",
                                    acceptLabel: "General_Sec0_btn_CONFIRM_BOX_YES",
                                    cancelLabel: "General_Sec0_btn_CONFIRM_BOX_NO",
                                    onAccept: () => { onAcceptConfirmationDesasociar(nexus) },
                                    onCancel: () => { }
                                });
                            }
                        }
                    }
                }
            }
        } else {
            _nexus.parameters = [];
        }
        data.parameters = [
            { id: "empresa", value: props.empresa },
        ];
    }

    const onAcceptConfirmationDesasociar = (nexus) => {
        setCodigosMultidatoConDetalles(null);
        let grid2 = _nexus.getGrid("REG100AsociarCodigoEmpresa_grid_2");
        if (grid2 !== undefined) {
            _nexus.parameters = [
                { id: "isMenuItemActionConfirm", value: "S" }
            ];

            grid2.triggerMenuAction("btnQuitar");
        }

    }

    const GridOnBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditar") {
            context.abortServerCall = true;
            setCodigoMultidato(data.row.cells.find(w => w.column == "CD_CODIGO_MULTIDATO").value);
            setShowPopupDetallesCodigosMultidato(true);
        }
    };

    const onAfterInitialize = (context, grid, parameters, nexus) => {
        if (parameters.find(d => d.id === "detallesAsociados")) {
            setCodigosMultidatoConDetalles(parameters.find(d => d.id === "detallesAsociados").value);
        }
    };

    const onAfterFetch = (context, newRows, parameters, nexus) => {
        if (parameters.find(d => d.id === "detallesAsociados")) {
            setCodigosMultidatoConDetalles(parameters.find(d => d.id === "detallesAsociados").value);
        }
    };

    const onBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = [
            { id: "empresa", value: props.empresa }
        ];
    }

    const closePopupDetallesCodigos = () => {
        setShowPopupDetallesCodigosMultidato(false);
    }

    return (
        <Modal dialogClassName="modal-70w" show={props.show} onHide={props.onHide} >
            <Modal.Header closeButton>
                <Modal.Title>{t("REG100_Sec0_mdlTitle_AsociarCodigoMul")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form
                    application="REG100AsociarCodigoMultidato"
                    id="REG100AsociarCodigo_form_1"
                    onBeforeInitialize={onBeforeInitialize}
                >
                    <Row>
                        <Col lg={3}>
                            <label htmlFor="codigoEmpresa">{t("REG100_frm1_colname_CD_EMPRESA")}</label>
                            <Field name="codigoEmpresa" readOnly />
                            <StatusMessage for="codigoEmpresa" readOnly />
                        </Col >
                        <Col lg={3}>
                            <label htmlFor="nombreEmpresa">{t("REG100_frm1_colname_NM_EMPRESA")}</label>
                            <Field name="nombreEmpresa" readOnly />
                            <StatusMessage for="nombreEmpresa" readOnly />
                        </Col >
                        <Col lg={6}>
                            <label htmlFor="url">{t("REG100_frm1_colname_URL")}</label>
                            <Field name="url" readOnly />
                            <StatusMessage for="url" readOnly />
                        </Col >
                    </Row>
                    <br />
                </Form>
                <AddRemovePanel
                    onAdd={handleAdd}
                    onRemove={handleRemove}
                    from={(
                        <Grid
                            application="REG100AsociarCodigoMultidato"
                            id="REG100AsociarCodigoEmpresa_grid_1"
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
                            application="REG100AsociarCodigoMultidato"
                            id="REG100AsociarCodigoEmpresa_grid_2"
                            rowsToFetch={30}
                            rowsToDisplay={15}
                            enableExcelExport
                            enableSelection
                            onBeforeInitialize={addParameters}
                            onAfterInitialize={onAfterInitialize}
                            onBeforeFetch={addParameters}
                            onAfterFetch={onAfterFetch}
                            onBeforeFetchStats={addParameters}
                            onBeforeApplyFilter={addParameters}
                            onBeforeApplySort={addParameters}
                            onBeforeExportExcel={addParameters}
                            onBeforeCommit={addParameters}
                            onBeforeMenuItemAction={onBeforeMenuItemAction}
                            onAfterMenuItemAction={onAfterMenuItemAction}
                            onBeforeButtonAction={GridOnBeforeButtonAction}
                        />
                    )}
                />
            </Modal.Body>
            <REG100CodigosMultidatoModal show={showPopupDetallesCodigosMultidato} onHide={closePopupDetallesCodigos} empresa={props.empresa} codigoMultidato={codigoMultidato} />
        </Modal>
    );
}

export const REG100AsociarCodigoMultidatoModal = withPageContext(InternalREG100AsociarCodigoMultidatoModal);