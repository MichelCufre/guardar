import React, { useState, useEffect } from 'react';
import { Button, Col, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { Field, FieldSelect, Form, StatusMessage } from '../../components/FormComponents/Form';
import { Grid } from '../../components/GridComponents/Grid';
import * as Yup from 'yup';
import { withPageContext } from '../../components/WithPageContext';

function InternalPRD111ExpulsarMercaderiaModal(props) {
    const { t } = useTranslation();

    const [stockSelection, setStockSelection] = useState("");
    const [_nexus, setNexus] = useState(null);
    const [isHabilitada, setIsHabilitada] = useState(false);
    const [isExplusarConTransferencia, setIsExplusarConTransferencia] = useState("");
    const [codigoImpresora, setCodigoImpresora] = useState("");
    const [idEspacioProduccion, setIdEspacioProduccion] = useState(props.idEspacioProduccion);

    const validationSchema = {
        idEspacioProduccion: Yup.string(),
    };

    const handleClose = () => {
        props.onHide();
    };

    const expulsarMasivo = (evt) => {
        if (_nexus) {
            setIsHabilitada(true);
            _nexus.getGrid("ProductosExpul_grid_1").triggerMenuAction("btnExpedir", false, evt.ctrlKey);
        }
    };

    const expulsarMasivoTransferencia = (evt) => {
        if (_nexus) {
            setIsHabilitada(true);
            _nexus.getGrid("ProductosExpul_grid_1").triggerMenuAction("btnExpedirTransferencia", false, evt.ctrlKey);
        }
    };

    const onAfterMenuItemAction = (context, data) => {
        if (data.parameters) {
            setIsExplusarConTransferencia(data.parameters.find(w => w.id === "isExplusarConTransferencia")?.value || "");
            setStockSelection(data.parameters.find(w => w.id === "PRD111_ProductosExpulsable")?.value || "");
        }
        context.abortUpdate = true;
        context.abortHideLoading = true;
        _nexus?.getGrid("ProductosExpul_grid_1").commit(true, true);
    };

    const onAfterInitialize = (context, grid, parameters, nexus) => {
        setNexus(nexus);
    };

    const onBeforeCommit = (context, data) => {
        if (_nexus && _nexus.getForm("PRD111Expulsable_form")) {
            data.parameters.push({ id: "codigoImpresora", value: _nexus.getForm("PRD111Expulsable_form").getFieldValue("impresora") });
        }
        data.parameters.push({ id: "PRD111_ProductosExpulsable", value: stockSelection });
        data.parameters.push({ id: "isExplusarConTransferencia", value: isExplusarConTransferencia });
        data.parameters.push({ id: "idEspacioProduccion", value: props.idEspacioProduccion });
    };

    const onAfterCommit = (context, rows) => {
        setIsHabilitada(false);
        if (context.status !== "ERROR") {
            _nexus.getGrid("ProductosExpul_grid_1").refresh();
        }
    };

    const addParameters = (context, data, nexus) => {
        data.parameters.push({ id: "idEspacioProduccion", value: props.idEspacioProduccion });
    }

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        setIdEspacioProduccion(props.idEspacioProduccion);

        query.parameters = [
            { id: "idEspacioProduccion", value: props.idEspacioProduccion },
        ];
    };
    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        context.abortServerCall = true;
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-70w">
            <Modal.Header closeButton>
                <Modal.Title>{t("PRD111ExpulsarMercaderia_Sec0_modalTitle_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form id="PRD111Expulsable_form" application="PRD111ExpulsarMercaderia"
                    onBeforeInitialize={handleFormBeforeInitialize}
                    onBeforeSubmit={handleFormBeforeSubmit}
                >
                    <br />
                    <Row>
                        <Col md={3}>
                            <div className="form-group">
                                <label htmlFor="idEspacioProduccion">{t("PRD111_frm1_lbl_idEspacioProduccion")}</label>
                                <Field name="idEspacioProduccion" value={idEspacioProduccion} readOnly />
                                <StatusMessage for="idEspacioProduccion" />
                            </div>
                        </Col>
                        <Col md={3}>
                            <div className="form-group">
                                <label htmlFor="descripcion">{t("PRD111_frm1_lbl_descripcion")}</label>
                                <Field name="descripcion" value={idEspacioProduccion} readOnly />
                                <StatusMessage for="descripcion" />
                            </div>
                        </Col>
                        <Col md={3}>
                            <div className="form-group">
                                <label htmlFor="tipo">{t("PRD111_frm1_lbl_tipo")}</label>
                                <Field name="tipo" value={idEspacioProduccion} readOnly />
                                <StatusMessage for="tipo" />
                            </div>
                        </Col>
                        <Col md={3}>
                            <div className="form-group">
                                <label htmlFor="impresora">{t("IMP080_frm1_lbl_impresora")}</label>
                                <FieldSelect name="impresora" />
                                <StatusMessage for="impresora" />
                            </div>
                        </Col>
                    </Row>
                    <br />
                    <Row>
                        <Col span={6} style={{ maxWidth: "85%" }}>
                            <Grid
                                id="ProductosExpul_grid_1"
                                application="PRD111ExpulsarMercaderia"
                                rowsToFetch={30}
                                rowsToDisplay={10}
                                enableExcelExport
                                enableSelection
                                onAfterInitialize={onAfterInitialize}
                                onAfterMenuItemAction={onAfterMenuItemAction}
                                onBeforeCommit={onBeforeCommit}
                                onAfterCommit={onAfterCommit}
                                onBeforeInitialize={addParameters}
                                onBeforeFetch={addParameters}
                                onBeforeFetchStats={addParameters}
                                onBeforeApplyFilter={addParameters}
                                onBeforeApplySort={addParameters}
                                onBeforeExportExcel={addParameters}
                                onBeforeMenuItemAction={addParameters}
                            />
                        </Col>
                        <Col span={6} style={{ display: 'flex', flexDirection: 'column', justifyContent: 'center', alignItems: 'center', height: '200px', maxWidth: '15%', marginTop: '90px' }}>
                            <Row style={{ marginBottom: '10px', width: '100%', justifyContent: 'center' }}>
                                <Button variant="primary" onClick={expulsarMasivo} disabled={isHabilitada}>
                                    {t("PRD111_Sec0_btn_Expulsar")}
                                </Button>
                            </Row>
                            <Row style={{ marginBottom: '10px', width: '100%', justifyContent: 'center' }}>
                                <Button variant="primary" onClick={expulsarMasivoTransferencia} disabled={isHabilitada}>
                                    {t("PRD111_Sec0_btn_ExpulsarEtiqueta")}
                                </Button>
                            </Row>
                        </Col>
                    </Row>
                </Form>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={handleClose}>
                    {t("PRD111_frm1_btn_CANCELAR")}
                </Button>
            </Modal.Footer>
        </Modal>
    );
}


export const PRD111ExpulsarMercaderiaModal = withPageContext(InternalPRD111ExpulsarMercaderiaModal);