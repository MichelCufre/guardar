import React, { useState } from 'react';
import { Button, Col, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { FieldSelect, Form, StatusMessage } from '../../components/FormComponents/Form';
import { GridImportExcelFileHandler } from '../../components/GridComponents/GridImportExcelFileHandler';
import { withPageContext } from '../../components/WithPageContext';
import withToaster from '../../components/WithToaster';

function REG040ImportExcelInternal(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const [predio, setPredio] = useState(null);

    const initialValues = {
        predio: "",
    };

    const validationSchema = {
        predio: Yup.string().required(),
    };

    const handleClose = () => {
        var closeModal = props.eventHandlers.find(item => item.id === "closeImportExcelModal").value;

        closeModal();
    };

    const handleFormAfterInitialize = (context, form, query, nexus) => {
        var predio = query.parameters.find(i => i.id === "REG040_PREDIO_UNICO").value;

        setPredio(predio);

        var form = nexus.getForm("REG040ImportExcel_form_1");

        if (form) form.setFieldValue("predio", predio);
    };

    const handleFormAfterValidateField = (context, form, query, nexus) => {
        setValues(nexus);
    }

    const setValues = (nexus) => {
        var form = nexus.getForm("REG040ImportExcel_form_1");

        if (form == null) return;

        var predioSeleccionado = form.getFieldValue("predio");

        setPredio(predioSeleccionado);
    }

    const getProperties = () => {
        return [{ "id": "predio", "value": predio }];
    }

    return (
        <Modal show={props.showModal} size="lg" onHide={handleClose} className="gr-import-excel">
            <Modal.Header closeButton>
                <Modal.Title>{t("IExcel_Modal_lbl_Title")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>

                <Form
                    application="REG040ImportExcel"
                    id="REG040ImportExcel_form_1"
                    initialValues={initialValues}
                    validationSchema={validationSchema}
                    onAfterValidateField={handleFormAfterValidateField}
                    onAfterInitialize={handleFormAfterInitialize}
                >
                    <Row >
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="predio">{t("Predio")}</label>
                                <FieldSelect name="predio" />
                                <StatusMessage for="predio" />
                            </div>
                        </Col>
                    </Row>

                    <GridImportExcelFileHandler
                        getProperties={getProperties}
                        eventHandlers={props.eventHandlers}
                        closeImportExcelModal={handleClose}
                    />
                </Form >

            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={handleClose}>
                    {t("General_Sec0_btn_LOAD_FILTER_CLOSE")}
                </Button>
            </Modal.Footer>
        </Modal>
    );
}

export const REG040ImportExcel = withToaster(withPageContext(REG040ImportExcelInternal));