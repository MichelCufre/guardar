import React from 'react';
import { Button, Col, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { FieldSelect, Form, StatusMessage } from '../../components/FormComponents/Form';
import { Grid } from '../../components/GridComponents/Grid';

export function PTL010ColoresActivosModal(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const handleClose = () => {
        props.nexus.getGrid("PTL010_grid_1").refresh();
        props.onHide();
    };

    //********************************** FORM **********************************
    const validationSchema = {
        automatismo: Yup.string(),
    };

    const initialValues = {
        automatismo: "",
    };

    const FormOnBeforeInitialize = (context, form, query, nexus) => {
        var parameters = [{ id: "INITIALIZE_BEFORE_FORM", value: "S" }];

        nexus.getGrid("PTL010ColoresActivos_grid_1").reset(parameters);
    };

    const onAfterInitialize = (context, form, query, nexus) => {
        const gridFetchQuery = query.parameters.find(p => p.id === "GRID_FETCH_QUERY");

        if (gridFetchQuery) {
            var parameters = [{ id: "NU_AUTOMATISMO", value: gridFetchQuery.value }];

            nexus.getGrid("PTL010ColoresActivos_grid_1").refresh(parameters);
        }
    };


    const onAfterValidateField = (context, form, query, nexus) => {
        if (query.fieldId == "automatismo") {
            nexus.getGrid("PTL010ColoresActivos_grid_1").refresh();
        }
    }
    //***************************************************************************

    //********************************** GRID **********************************

    const addParameters = (context, data, nexus) => {
        data.parameters.push({ id: "NU_AUTOMATISMO", value: nexus.getForm("PTL010ColoresActivos_form_1").getFieldValue("automatismo") });
    }

    const GridOnBeforeIntitialize = (context, data, nexus) => {
        if (!data.parameters.find(i => i.id == "INITIALIZE_BEFORE_FORM"))
            context.abortServerCall = true;

        addParameters(context, data, nexus);
    }

    const onBeforeButtonAction = (context, data, nexus) => {
        nexus.getGrid("PTL010ColoresActivos_grid_1").refresh();
        addParameters(context, data, nexus);
    }

    const onAfterButtonAction = (data, nexus) => {
        nexus.getGrid("PTL010ColoresActivos_grid_1").refresh();       
    };

    const onBeforeApplyFilter = (context, data, nexus) => {
        nexus.getGrid("PTL010ColoresActivos_grid_1").refresh();
        addParameters(context, data, nexus);
    }

    const onAfterCommit = (context, rows, parameters, nexus) => {
        nexus.getGrid("PTL010ColoresActivos_grid_1").refresh();        
    }

    //***************************************************************************

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">

            <Modal.Header closeButton>
                <Modal.Title>{t("PTL010ColoresActivos_Sec0_modalTitle_ColoresActivos")}</Modal.Title>
            </Modal.Header>

            <Form
                id="PTL010ColoresActivos_form_1"
                application="PTL010ColoresActivosForm"
                validationSchema={validationSchema}
                initialValues={initialValues}
                onBeforeInitialize={FormOnBeforeInitialize}
                onAfterInitialize={onAfterInitialize}
                onAfterValidateField={onAfterValidateField}
            >

                <Modal.Body>

                    <Row>
                        <Col md={4}>
                            <div className="form-group">
                                <label htmlFor="automatismo">{t("PTL010ColoresActivos_frm1_lbl_automatismo")}</label>
                                <FieldSelect name="automatismo" />
                                <StatusMessage for="automatismo" />
                            </div>
                        </Col>
                    </Row>

                    <Grid
                        id="PTL010ColoresActivos_grid_1"
                        application="PTL010ColoresActivos"
                        onBeforeInitialize={GridOnBeforeIntitialize}
                        onBeforeFetch={addParameters}
                        onBeforeFetchStats={addParameters}
                        onBeforeExportExcel={addParameters}
                        onBeforeApplyFilter={onBeforeApplyFilter}
                        onBeforeButtonAction={onBeforeButtonAction}
                        onAfterButtonAction={onAfterButtonAction}
                        onAfterCommit={onAfterCommit}
                        rowsToDisplay={10}
                        rowsToFetch={20}
                        enableExcelExport
                    />

                </Modal.Body>

                <Modal.Footer>
                    <Button variant="outline-secondary" onClick={handleClose}>
                        {t("PTL010NotificarPTL_frm1_btn_cancelar")}
                    </Button>
                </Modal.Footer>
            </Form>
        </Modal>
    );
}