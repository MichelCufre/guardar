import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, FieldSelect, StatusMessage, SubmitButton, FieldToggle } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Container } from 'react-bootstrap';
import * as Yup from 'yup';
import { Grid } from '../../components/GridComponents/Grid';
import { notificationType } from '../../components/Enums';
import { REC275EditarParametroModal } from './REC275EditarParametroModal';
import { useToaster } from '../../components/ToasterHook';
function InternalREC275AgregarLogicaModal(props) {

    const [numeroLogica, setNumeroLogica] = useState("");
    const [descripcionProceso, setDescripcionProceso] = useState("");
    const [showModal, setShowModal] = useState(false);
    const [row, setRow] = useState(null);
    const [codigoParametro, setCodigoParametro] = useState(null);
    const [descripcionParametro, setDescripcionParametro] = useState(null);
    const [valorParametro, setValorParametro] = useState(null);
    const toaster = useToaster();
    const { t } = useTranslation();

    const initialValues = {
        logica: "",
        descripcionProceso: "",
        codigoEstrategia: "",
    };

    const validationSchema = {
        logica: Yup.string(),
        descripcionProceso: Yup.string(),
        ordenDeUbicaciones: Yup.boolean(),
    };

    const onBeforeValidateField = (context, form, query, nexus) => {
        setNumeroLogica(form.fields.find(w => w.id === "logica").value);
        setDescripcionProceso(form.fields.find(w => w.id === "descripcionProceso").value);
    };

    const onAfterValidateField = (context, form, query, nexus) => {
        if (query.fieldId === "logica") {
            nexus.getGrid("REC275_grid_3").refresh();
        }
    };

    const handleClose = () => {
        handleCloseModal();
        props.onHide(null);
    };

    const handleCloseModal = (valorParametro, nexus) => {
        if (nexus && valorParametro != null) {
            var grid = nexus.getGrid("REC275_grid_3");
            var cellVlParametro = row.cells.find(c => c.column == "VL_ALM_PARAMETRO_DEFAULT");
            var cellNmParametro = row.cells.find(c => c.column == "NM_ALM_PARAMETRO");
            var relatedRow = null;

            cellVlParametro.modified = cellVlParametro.value !== valorParametro;
            cellVlParametro.old = cellVlParametro.value;
            cellVlParametro.value = valorParametro;

            grid.updateRow(row);

            if (cellNmParametro.value.indexOf("_DESDE") >= 0) {
                relatedRow = grid.getModifiedRows().find(r => r.cells.some(c => c.column === "NM_ALM_PARAMETRO" && c.value === cellNmParametro.value.replace(/DESDE/gi, "HASTA")));
            } else if (cellNmParametro.value.indexOf("_HASTA") >= 0) {
                relatedRow = grid.getModifiedRows().find(r => r.cells.some(c => c.column === "NM_ALM_PARAMETRO" && c.value === cellNmParametro.value.replace(/HASTA/gi, "DESDE")));
            }else if (cellNmParametro.value === "MODALIDAD_REABASTECIMIENTO") {
                relatedRow = grid.getModifiedRows().find(r => r.cells.some(c => c.column === "NM_ALM_PARAMETRO" && c.value === "PORCENTAJE_FORZADO"));
            } else if (cellNmParametro.value === "PORCENTAJE_FORZADO") {
                relatedRow = grid.getModifiedRows().find(r => r.cells.some(c => c.column === "NM_ALM_PARAMETRO" && c.value === "MODALIDAD_REABASTECIMIENTO"));
            }

            if (relatedRow) {
                grid.validateRow(relatedRow);
            }
        }

        setRow(null);
        setCodigoParametro(null);
        setDescripcionParametro(null);
        setValorParametro(null);
        setShowModal(false);
    };

    const handleGridBeforeValidate = (context, data, nexus) => {
        const rowsEntrada = nexus.getGrid("REC275_grid_3").getModifiedRows();

        data.parameters = [
            { id: "rowsEntrada", value: JSON.stringify(rowsEntrada) },
        ];
    }

    const applyParameters = (context, data, nexus) => {

        data.parameters = [
            { id: "numeroLogica", value: numeroLogica },
        ];
    };

    const onBeforeSubmit = (context, form, query, nexus) => {

        if (nexus.getGrid("REC275_grid_3").hasError()) {
            context.abortServerCall = true;

            nexus.toast(notificationType.error, "Hay errores en la grilla, no se puede confirmar. Corrija los errores antes de continuar");

            return false;
        }

        const rowsEntrada = nexus.getGrid("REC275_grid_3").getModifiedRows();

        query.parameters = [
            { id: "modoEditar", value: true },
            { id: "numeroEstrategia", value: props.codigoEstrategia },
            { id: "numeroLogica", value: numeroLogica },
            { id: "rowsEntrada", value: JSON.stringify(rowsEntrada) },
        ];
    }

    const onAfterSubmit = (context, form, query, nexus) => {
        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {
            props.onHide(nexus);
        }
    }

    const onBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = [
            { id: "modoEditar", value: true },
            { id: "numeroEstrategia", value: props.codigoEstrategia },
            { id: "numeroLogica", value: numeroLogica },
        ];
    };

    const handleGridBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditarParametro") {
            context.abortServerCall = true;
            setRow(data.row);
            setCodigoParametro(data.row.cells.find(c => c.column == 'NM_ALM_PARAMETRO').value);
            setDescripcionParametro(data.row.cells.find(c => c.column == 'DS_ALM_PARAMETRO').value);
            setValorParametro(data.row.cells.find(c => c.column == 'VL_ALM_PARAMETRO_DEFAULT').value);
            if (!descripcionProceso) {
                toaster.toastError("REC275_Sec0_Error_DescripcionRequerida");
            } else {
                setShowModal(true);
            }
        }
    }

    return (

        <Form
            application="REC275AgregarLogica"
            id="REC275FormModalAgregarLogica"
            initialValues={initialValues}
            validationSchema={validationSchema}
            onBeforeSubmit={onBeforeSubmit}
            onAfterSubmit={onAfterSubmit}
            onBeforeValidateField={onBeforeValidateField}
            onAfterValidateField={onAfterValidateField}
            onBeforeInitialize={onBeforeInitialize}
        >
            <Modal.Header closeButton>
                <Modal.Title>{t("REC275_Sec0_title_AsignarLogica")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Container fluid>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="codigoEstrategia">{t("REC275_grid1_colname_CodigoEstrategia")}</label>
                                <Field name="codigoEstrategia" readOnly />
                                <StatusMessage for="codigoEstrategia" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="descripcionEstrategia">{t("REC275_form_lbl_DescripcionEstrategia")}</label>
                                <Field name="descripcionEstrategia" readOnly />
                                <StatusMessage for="descripcionEstrategia" />
                            </div>
                        </Col>
                    </Row>

                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="descripcionProceso">{t("REC275_Modal_Campo_NombreLogica")}</label>
                                <Field name="descripcionProceso" />
                                <StatusMessage for="descripcionProceso" />
                            </div>
                        </Col>

                        <Col>
                            <div className="form-group" >
                                <label htmlFor="logica">{t("REC275_select_colname_Logica")}</label>
                                <FieldSelect name="logica" />
                                <StatusMessage for="logica" />
                            </div>
                        </Col>
                    </Row>

                    <Row>
                        <Col>
                            <div style={{ float: "left" }}>
                                <div style={{ textAlign: "left" }}>
                                    <label htmlFor="ordenDeUbicaciones">{t("REC275_Modal_Campo_Toggle")}</label>
                                </div>
                                <Row>
                                    <Col>
                                        <div style={{ textAlign: "right" }}>
                                            <label htmlFor="ordenDeUbicaciones">{t("REC275_Modal_Campo_Descendente")}</label>
                                        </div>
                                    </Col>
                                    <Col>
                                        <FieldToggle name="ordenDeUbicaciones" label={t("REC275_Modal_Campo_Ascendente")} />
                                        <StatusMessage for="ordenDeUbicaciones" />
                                    </Col>
                                </Row>
                            </div>
                        </Col>

                    </Row>
                </Container>
                <div className="col-12">
                    <Grid application="REC275AgregarLogica" id="REC275_grid_3" rowsToFetch={30} rowsToDisplay={15} enableExcelExport={true} validateAllRows
                        onBeforeFetch={applyParameters}
                        onBeforeInitialize={applyParameters}
                        onBeforeExportExcel={applyParameters}
                        onBeforeFetchStats={applyParameters}
                        onBeforeApplyFilter={applyParameters}
                        onBeforeValidateRow={handleGridBeforeValidate}
                        onBeforeApplySort={applyParameters}
                        onBeforeButtonAction={handleGridBeforeButtonAction}
                    />
                </div>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("PRE052_frm1_btn_cerrar")} </Button>
                <SubmitButton id="btnSubmitConfirmarEstrategia" onClick={props.closeFormDialog} variant="primary" label="REC275_frm1_btn_Confirmar" />
            </Modal.Footer>

            <Modal show={showModal} onHide={handleCloseModal} backdrop="static">
                <REC275EditarParametroModal application="REC275AgregarLogica"
                    logica={numeroLogica}
                    codigoEstrategia={props.codigoEstrategia}
                    codigoInstancia={props.codigoInstancia}
                    descripcionProceso={descripcionProceso}
                    codigoParametro={codigoParametro}
                    descripcionParametro={descripcionParametro}
                    valorParametro={valorParametro}
                    onHide={handleCloseModal}
                />
            </Modal>
        </Form>
    );
}

export const REC275AgregarLogicaModal = withPageContext(InternalREC275AgregarLogicaModal);