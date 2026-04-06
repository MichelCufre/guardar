import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Form, Field, FieldSelectAsync, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Container } from 'react-bootstrap';
import * as Yup from 'yup';
import { Grid } from '../../components/GridComponents/Grid';
import { notificationType } from '../../components/Enums';
import { REG310ReglasParametrosModal } from './REG310ReglasParametrosModal';

export function REG310ReglasCreateModal(props) {

    const [showModal, setShowModal] = useState(false);
    const [row, setRow] = useState(null);

    const [codigoParametro, setCodigoParametro] = useState(null);
    const [descripcionParametro, setDescripcionParametro] = useState(null);
    const [valorParametro, setValorParametro] = useState(null);
    const [tipoParametro, setTipoParametro] = useState(null);

    const { t } = useTranslation();

    const initialValues = {
        nuRegla: "",
        descripcion: "",
        grupo: "",
    };

    const validationSchema = {
        descripcion: Yup.string().required(),
        grupo: Yup.string().required(),
    };

    const handleClose = () => {
        handleCloseModal();
        props.onHide(null);
    };

    const handleCloseModal = (valorParametro, nexus) => {
        if (nexus && valorParametro != null) {
            var grid = nexus.getGrid("REG310ReglasCreate_grid_1");

            var cellNmParametro = row.cells.find(c => c.column == "NM_PARAM");
            var cellVlParametro = row.cells.find(c => c.column == "VL_PARAM");

            var relatedRow = null;

            cellVlParametro.modified = cellVlParametro.value !== valorParametro;
            cellVlParametro.value = valorParametro;

            grid.updateRow(row);

            if (cellNmParametro.value.indexOf("_DESDE") >= 0) {
                relatedRow = grid.getModifiedRows().find(r => r.cells.some(c => c.column === "NM_PARAM" && c.value === cellNmParametro.value.replace(/DESDE/gi, "HASTA")));
            } else if (cellNmParametro.value.indexOf("_HASTA") >= 0) {
                relatedRow = grid.getModifiedRows().find(r => r.cells.some(c => c.column === "NM_PARAM" && c.value === cellNmParametro.value.replace(/HASTA/gi, "DESDE")));
            }

            if (relatedRow) {
                grid.validateRow(relatedRow);
            }
        }

        setRow(null);
        setCodigoParametro(null);
        setDescripcionParametro(null);
        setTipoParametro(null);
        setValorParametro(null);
        setShowModal(false);
    };

    const handleGridBeforeValidate = (context, data, nexus) => {
        const rowsEntrada = nexus.getGrid("REG310ReglasCreate_grid_1").getModifiedRows();

        data.parameters = [
            { id: "rowsEntrada", value: JSON.stringify(rowsEntrada) },
        ];
    }

    const onBeforeSubmit = (context, form, query, nexus) => {

        if (nexus.getGrid("REG310ReglasCreate_grid_1").hasError()) {
            context.abortServerCall = true;

            nexus.toast(notificationType.error, "Hay errores en la grilla, no se puede confirmar. Corrija los errores antes de continuar");

            return false;
        }

        const rowsEntrada = nexus.getGrid("REG310ReglasCreate_grid_1").getModifiedRows();

        query.parameters = [
            { id: "rowsEntrada", value: JSON.stringify(rowsEntrada) },
        ];
    }

    const onAfterSubmit = (context, form, query, nexus) => {
        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {
            props.onHide(nexus);
        }
    }

    const handleGridBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditarParametro") {
            context.abortServerCall = true;
            setRow(data.row);
            setCodigoParametro(data.row.cells.find(c => c.column == 'NM_PARAM').value);
            setDescripcionParametro(data.row.cells.find(c => c.column == 'DS_PARAM').value);
            setTipoParametro(data.row.cells.find(c => c.column == 'TP_PARAM').value);
            setValorParametro(data.row.cells.find(c => c.column == 'VL_PARAM').value);
            setShowModal(true);
        }
    }

    return (

        <Form
            application="REG310ReglasCreate"
            id="REG310ReglasCreate_form_1"
            initialValues={initialValues}
            validationSchema={validationSchema}
            onBeforeSubmit={onBeforeSubmit}
            onAfterSubmit={onAfterSubmit}
        >
            <Modal.Header closeButton>
                <Modal.Title>{t("REG300_Sec0_Title_NuevaRegla")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Container fluid>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="nuRegla">{t("REG310ReglasCreate_frm_lbl_Regla")}</label>
                                <Field name="nuRegla" readOnly />
                                <StatusMessage for="nuRegla" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="descripcion">{t("REG310ReglasCreate_frm_lbl_Descripcion")}</label>
                                <Field name="descripcion" />
                                <StatusMessage for="descripcion" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="grupo">{t("REG310ReglasCreate_frm_lbl_Grupo")}</label>
                                <FieldSelectAsync name="grupo" />
                                <StatusMessage for="grupo" />
                            </div>
                        </Col>
                    </Row>
                </Container>
                <div className="col-12">
                    <Grid
                        application="REG310ReglasCreate"
                        id="REG310ReglasCreate_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        validateAllRows
                        onBeforeValidateRow={handleGridBeforeValidate}
                        onBeforeButtonAction={handleGridBeforeButtonAction}
                    />
                </div>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("REG300_frm_btn_Cancelar")} </Button>
                <SubmitButton id="btnSubmitConfirmarRegla" onClick={props.closeFormDialog} variant="primary" label="REG300_frm_btn_Crear" />
            </Modal.Footer>

            <Modal show={showModal} onHide={handleCloseModal} backdrop="static">
                <REG310ReglasParametrosModal
                    application="REG310ReglasCreate"
                    codigoParametro={codigoParametro}
                    descripcionParametro={descripcionParametro}
                    tipoParametro={tipoParametro}
                    valorParametro={valorParametro}
                    onHide={handleCloseModal}
                />
            </Modal>
        </Form>
    );
}